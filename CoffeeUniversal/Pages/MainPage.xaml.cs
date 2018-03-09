using CoffeeUniversal.ViewModels;
using System;
using System.Diagnostics;
using System.Linq;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace CoffeeUniversal.Pages
{
    public sealed partial class MainPage : Page
    {

        #region Fields

        private const double HORIZONTAL_BORDER = 36;
        private const double TITLE_FONT_LARGE = 96;
        private const double TITLE_FONT_SMALL = 30;
        private const double BUTTON_HEIGHT_THRESHOLD = 32;
        private const double WINDOW_HEIGHT_THRESHOLD = 500;
        private const double ASPECT_RATIO_THRESHOLD = 0.6666;
        private const int ROWCOUNT_DEFAULT = 12;
        private const int COLCOUNT_DEFAULT = 3;
        private const int COLCOUNT_MAX = 4;
        private const double BUTTON_HEIGHT_RATIO = 0.4;
		private SolidColorBrush whiteBrush = new SolidColorBrush(Colors.White);
		private Thickness buttonMargin = new Thickness(0, 0, 4, 4);
		private Thickness buttonPadding = new Thickness(0.5);
        private Thickness TITLE_MARGIN_SMALL = new Thickness(19, -6, 0, 0);
        private Thickness TITLE_MARGIN_LARGE = new Thickness(19, -20, 0, -12);
        private const double VERTICAL_SPACE_LANDSCAPE = 80;
        private const double VERTICAL_SPACE_PORTRAIT_MOBILE = 175;
        private const double VERTICAL_SPACE_PORTRAIT_DESKTOP = 150;

        private double verticalSpaceUsed;
        private int rowCount;
		private int colCount;
        private int oldColCount;
        private double steeringWheelHeight;

        #endregion


        #region Init

        public MainPage()
		{
			App.IsActivationDone = true;
            rowCount = ROWCOUNT_DEFAULT;
            oldColCount = colCount = COLCOUNT_DEFAULT;

            if (App.IsHardwareBackButtonAvailable)
            {
                ApplicationView currentView = ApplicationView.GetForCurrentView();
                currentView.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
                double heightIncludingSystemTray = currentView.VisibleBounds.Height + currentView.VisibleBounds.Top;
                steeringWheelHeight = Window.Current.Bounds.Height - heightIncludingSystemTray;
                currentView.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseVisible);
            }

            InitializeComponent();
            versionString.Text = App.VersionString;
			PopulateMenu();
		}

		private void PopulateMenu()
		{
            int itemCount = App.ViewModel.MenuItems.Count;
            rowCount = (itemCount - 1) / colCount + 1;
            int i = 0;

            for (int r = 0; r < rowCount; r++)
            {
                for (int c = 0; c < colCount; c++)
                {
                    if (i == itemCount) break;
					MenuItem item = App.ViewModel.MenuItems[i++];

					Button b = new Button() { Name = item.Name, Content = item.Content, IsEnabled = item.IsEnabled };
					b.SetValue(Grid.RowProperty, r);
					b.SetValue(Grid.ColumnProperty, c);
					b.Click += menuItem_Click;
                    b.Margin = buttonMargin;
                    b.Padding = buttonPadding;
                    string automationId = item.Name + "Button";
                    b.SetValue(AutomationProperties.AutomationIdProperty, automationId);

                    // UNDONE: disable selected menuitems.
                    //if (item.Name == "ink" && App.IsHardwareBackButtonAvailable)
                    //{
                    //	b.IsEnabled = false;
                    //}

					contentPanel.Children.Add(b);
				}
            }
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            menuStoryboard.Begin();

            if (e.NavigationMode == NavigationMode.Back)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        #endregion


        #region SizeChanged

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
		{
            if (ActualHeight < WINDOW_HEIGHT_THRESHOLD)
            {
                appTitle.FontSize = TITLE_FONT_SMALL;
                verticalSpaceUsed = VERTICAL_SPACE_LANDSCAPE - steeringWheelHeight;
                appTitle.Margin = TITLE_MARGIN_SMALL;
                versionString.Visibility = Visibility.Collapsed;
            }
            else
            {
                appTitle.FontSize = TITLE_FONT_LARGE;
                appTitle.Margin = TITLE_MARGIN_LARGE;
                versionString.Visibility = Visibility.Visible;

                if (App.IsHardwareBackButtonAvailable)
                {
                    verticalSpaceUsed = VERTICAL_SPACE_PORTRAIT_MOBILE;
                }
                else
                {
                    verticalSpaceUsed = VERTICAL_SPACE_PORTRAIT_DESKTOP;
                }
            }

            if ((ActualHeight / ActualWidth) < ASPECT_RATIO_THRESHOLD)
            {
                colCount = COLCOUNT_MAX;
            }
            else
            {
                colCount = COLCOUNT_DEFAULT;
            }
            if (colCount != oldColCount)
            {
                RedefineRowsAndColumns();
            }
            ResizeButtons();
		}

        private void RedefineRowsAndColumns()
        {
            oldColCount = colCount;
            int itemCount = App.ViewModel.MenuItems.Count;
            rowCount = (itemCount - 1) / colCount + 1;

            contentPanel.RowDefinitions.Clear();
            for (int r = 0; r < rowCount; r++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                contentPanel.RowDefinitions.Add(rowDefinition);
            }

            contentPanel.ColumnDefinitions.Clear();
            for (int c = 0; c < colCount; c++)
            {
                ColumnDefinition colDefinition = new ColumnDefinition();
                contentPanel.ColumnDefinitions.Add(colDefinition);
            }

            int i = 0;
            for (int r = 0; r < rowCount; r++)
            {
                for (int c = 0; c < colCount; c++)
                {
                    if (i == itemCount) break;

                    Button button = contentPanel.Children[i++] as Button;
                    if (button != null)
                    {
                        button.SetValue(Grid.RowProperty, r);
                        button.SetValue(Grid.ColumnProperty, c);
                    }
                }
            }
        }

        private void ResizeButtons()
		{
            double availableWidth = ActualWidth - HORIZONTAL_BORDER;
            double newButtonWidth = availableWidth / colCount;
            double availableHeight = ActualHeight - verticalSpaceUsed - steeringWheelHeight; 
            double newButtonHeight = availableHeight / rowCount;
            double fontSize = newButtonHeight * BUTTON_HEIGHT_RATIO;

			foreach (UIElement child in contentPanel.Children)
			{
				Button button = child as Button;
				if (button != null)
				{
					button.Width = newButtonWidth;
					button.Height = newButtonHeight;
					button.FontSize = fontSize;
				}
			}
		}

        #endregion


        #region menuItem_Click

        private void menuItem_Click(object sender, RoutedEventArgs e)
		{
			Button b = sender as Button;
			MenuItem selectedItem = App.ViewModel.MenuItems.Where(item => item.Name == b.Name).First();
			if (selectedItem != null)
			{
				Frame.Navigate(selectedItem.PageType);
			}
		}

		#endregion

	}
}
