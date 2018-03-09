using CoffeeUniversal.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using CoffeeUniversal.ViewModels;
using Windows.UI.ViewManagement;

namespace CoffeeUniversal.Pages
{
	public sealed partial class InkPage : Page
	{

		#region Standard stuff

		private NavigationHelper navigationHelper;
		public NavigationHelper NavigationHelper
		{
			get { return navigationHelper; }
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedTo(e);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedFrom(e);
		}

		#endregion


		#region Init

		private InkManager inkManager;
		private int pointerId = -1;
		private StringBuilder inkText = new StringBuilder();
		private InkDrawingAttributes drawingAttributes;
		private XamlInkRenderer renderer;
		private Color coffeeColor = Color.FromArgb(255, 102, 51, 0);
		private double horizontalBorder = 40;
        private ApplicationView thisView;

		public InkPage()
		{
			InitializeComponent();

            thisView = ApplicationView.GetForCurrentView();
            if (thisView != null)
            {
                thisView.VisibleBoundsChanged += thisView_VisibleBoundsChanged;
            }

			navigationHelper = new NavigationHelper(this);

			drawingAttributes = new InkDrawingAttributes();
			drawingAttributes.Color = coffeeColor;
			double penSize = 3.0;
			drawingAttributes.Size = new Size(penSize, penSize);
			drawingAttributes.IgnorePressure = true;
			drawingAttributes.FitToCurve = true;

			try
			{
				inkManager = new InkManager();
				inkManager.SetDefaultDrawingAttributes(drawingAttributes);
				renderer = new XamlInkRenderer(inkCanvas);
                renderer.Clear();
			}
			catch (Exception ex)
			{
				App.ShowMessage(ex.ToString(), true);
				if (Frame != null && Frame.CanGoBack)
				{
					Frame.GoBack();
				}
			}

			inkCanvas.PointerPressed += new PointerEventHandler(InkCanvas_PointerPressed);
			inkCanvas.PointerMoved += new PointerEventHandler(InkCanvas_PointerMoved);
			inkCanvas.PointerReleased += new PointerEventHandler(InkCanvas_PointerReleased);
			inkCanvas.PointerExited += new PointerEventHandler(InkCanvas_PointerReleased);
		}

        private void thisView_VisibleBoundsChanged(ApplicationView sender, object args)
        {
            RefreshLayout();
        }

        private void pageRoot_SizeChanged(object sender, SizeChangedEventArgs e)
		{
            RefreshLayout();
		}

        private void RefreshLayout()
        {
            double heightExcludingInkCanvas = headerRow.ActualHeight + notesBlock.ActualHeight + buttonPanel.ActualHeight + status.ActualHeight;
            double baseHeight = (Math.Min(ActualHeight, thisView.VisibleBounds.Height));

            double maximumHeight = baseHeight - heightExcludingInkCanvas;
            if (maximumHeight > 0)
            {
                inkCanvas.Height = maximumHeight;
            }

            double maximumWidth = ActualWidth - horizontalBorder;
            if (maximumWidth > 0)
            {
                inkCanvas.Width = maximumWidth;
            }
            UpdateLayout();
        }

        #endregion


        #region Pointer handling

        public void InkCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
		{
			if (pointerId == -1)
			{
                try
                {
                    PointerPoint pt = e.GetCurrentPoint(inkCanvas);
                    renderer.EnterLiveRendering(pt, drawingAttributes);
                    inkManager.ProcessPointerDown(pt);
                    pointerId = (int)pt.PointerId;
                }
                catch (Exception ex)
                {
                    status.Log(ex.Message);
                }
			}
		}

		private void InkCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
		{
			if (e.Pointer.PointerId == pointerId)
			{
                try
                {
                    PointerPoint pt = e.GetCurrentPoint(inkCanvas);
                    IList<PointerPoint> intermediatePoints = e.GetIntermediatePoints(inkCanvas);
                    for (int i = intermediatePoints.Count - 1; i >= 0; i--)
                    {
                        inkManager.ProcessPointerUpdate(intermediatePoints[i]);
                    }
                    renderer.UpdateLiveRender(pt);
                }
                catch (Exception ex)
                {
                    status.Log(ex.Message);
                }
            }
        }

		public void InkCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
		{
			if (e.Pointer.PointerId == pointerId)
			{
                try
                {
                    PointerPoint pt = e.GetCurrentPoint(inkCanvas);
                    inkManager.ProcessPointerUp(pt);
                    pointerId = -1;

                    renderer.ExitLiveRendering(pt);
                    renderer.AddInk(inkManager.GetStrokes()[inkManager.GetStrokes().Count - 1]);
                }
                catch (Exception ex)
                {
                    status.Log(ex.Message);
                }
            }
        }

		#endregion


		#region AppBar clicks

		private void appBarClear_Click(object sender, RoutedEventArgs e)
		{
			ClearAllStrokes();
		}

		private void ClearAllStrokes()
		{
			try
			{
				pointerId = -1;
				foreach (InkStroke stroke in inkManager.GetStrokes())
				{
					stroke.Selected = true;
				}
				inkManager.DeleteSelected();
				inkCanvas.Children.Clear();
				status.Log(LocalizableStrings.INK_STROKES_CLEAR_SUCCESS);
			}
			catch (Exception ex)
			{
				status.Log(ex.ToString());
			}
		}

		async private void appBarRecognize_Click(object sender, RoutedEventArgs e)
		{
			if (inkManager.GetStrokes().Count > 0)
			{
				try
				{
                    // BUG 339829, 629537, 1846969 SaveAsync on Mobile, non-Chinese recognizers on Mobile.
                    //string zh = "Microsoft 中文(简体)手写识别器";
                    //string zhCN = "Microsoft 中文(简体)手写识别器";
                    //string zhHansCN = "Microsoft 中文(简体)手写识别器";
                    //string zhHant = "Microsoft 中文(繁體)手寫辨識器";
                    //string zhTW = "Microsoft 中文(繁體)手寫辨識器";
                    //string enUS = "Microsoft English (US) Handwriting Recognizer";

                    IReadOnlyList<InkRecognizer> recognizers = inkManager.GetRecognizers();
                    bool recognizerFound = false;

                    if (recognizers != null && recognizers.Count > 0)
                    {
                        foreach (InkRecognizer recognizer in recognizers)
                        {
                            foreach (KeyValuePair<string, string> item in RecognizerHelper.RecognizerNames)
                            {
                                status.Log(string.Format(
                                    CultureInfo.CurrentCulture, LocalizableStrings.INK_RECOGNIZER_SEARCH, recognizer.Name));
                                if (recognizer.Name == item.Value)
                                {
                                    inkManager.SetDefaultRecognizer(recognizer);
                                    recognizerFound = true;
                                    status.Log(string.Format(
                                        CultureInfo.CurrentCulture, LocalizableStrings.INK_RECOGNIZER_FOUND, item.Key));
                                    break;
                                }
                            }
                            if (recognizerFound) break;
                        }

                        if (recognizerFound)
                        {
                            IReadOnlyList<InkRecognitionResult> results = await inkManager.RecognizeAsync(InkRecognitionTarget.All);
                            if (results != null)
                            {
                                inkManager.UpdateRecognitionResults(results);
                                StringBuilder builder = new StringBuilder();
                                builder.Append(LocalizableStrings.INK_RECOGNIZER_PREFIX);
                                foreach (InkRecognitionResult result in results)
                                {
                                    builder.Append(" " + result.GetTextCandidates()[0]);
                                }
                                status.Log(builder.ToString());
                            }
                            else
                            {
                                status.Log(LocalizableStrings.INK_RECOGNIZER_FAIL);
                            }
                        }
                        else
                        {
                            status.Log(LocalizableStrings.INK_RECOGNIZER_NOT_FOUND);
                        }
                    }
                    else
                    {
                        status.Log(LocalizableStrings.INK_NO_RECOGNIZERS);
                    }
				}
				catch (Exception ex)
				{
					Debug.WriteLine("InkPage.appBarRecognize_Click: " +ex.ToString());
                }
			}
			else
			{
				status.Log(LocalizableStrings.INK_EMPTY_INK_RECOGNIZE);
			}
		}

		async private void appBarSave_Click(object sender, RoutedEventArgs e)
		{
			if (inkManager.GetStrokes().Count > 0)
			{
				try
				{
					FileSavePicker savePicker = new FileSavePicker();
					savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
					savePicker.FileTypeChoices.Add("Gif", new List<string> { ".gif" });
					StorageFile file = await savePicker.PickSaveFileAsync();

					if (file != null)
					{
						using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
						{
							// BUG 339829 SaveAsync and misc other bugs now scheduled for OS/Future.
							await inkManager.SaveAsync(stream);
							status.Log(LocalizableStrings.INK_SAVE_SUCCESS);
						}
					}
				}
				catch (Exception ex)
				{
					status.Log(ex.Message);
                }
			}
			else
			{
				status.Log(LocalizableStrings.INK_EMPTY_INK_SAVE);
			}
		}

		async private void appBarLoad_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				FileOpenPicker openPicker = new FileOpenPicker();
				openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
				openPicker.FileTypeFilter.Add(".gif");
				StorageFile file = await openPicker.PickSingleFileAsync();

				if (file != null)
				{
					ClearAllStrokes();
					using (IInputStream stream = await file.OpenSequentialReadAsync())
					{
						await inkManager.LoadAsync(stream);
					}

					foreach (InkStroke stroke in inkManager.GetStrokes())
					{
						Path path = XamlInkRenderer.CreateBezierPath(stroke);
						inkCanvas.Children.Add(path);
					}

					status.Log(LocalizableStrings.INK_LOAD_SUCCESS);
				}
			}
			catch (Exception ex)
			{
				status.Log(ex.Message);
			}
		}

		#endregion

	}
}
