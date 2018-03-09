using CoffeeUniversal.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CoffeeUniversal.ViewModels;

namespace CoffeeUniversal.Pages
{
    public sealed partial class AppBarPage : Page
    {

        #region Init

        private NavigationHelper navigationHelper;
        private ObservableCollection<string> eventStrings = new ObservableCollection<string>();

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

        public AppBarPage()
        {
            InitializeComponent();
            navigationHelper = new NavigationHelper(this);
            eventList.ItemsSource = eventStrings;
            coffeeImage.RightTapped += new RightTappedEventHandler(coffeeImage_RightTapped);
            sampleText.ContextMenuOpening += new ContextMenuOpeningEventHandler(sampleText_ContextMenuOpening);
        }

        #endregion


        #region AppBar

        private void appBarLatte_Click(object sender, RoutedEventArgs e)
        {
            eventStrings.Add("latte");
        }

        private void appBarCappuccino_Click(object sender, RoutedEventArgs e)
        {
            eventStrings.Add("cappuccino");
        }

        private void appBarAmericano_Click(object sender, RoutedEventArgs e)
        {
            eventStrings.Add("americano");
        }

        private void appBarEspresso_Click(object sender, RoutedEventArgs e)
        {
            eventStrings.Add("espresso");
        }

        private void eventList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (eventList.SelectedIndex > -1)
            {
                status.Log(string.Format(CultureInfo.CurrentCulture,
                    "{0} [{1}]", eventList.SelectedItem, eventList.SelectedIndex));
            }
        }

        #endregion


        #region Popups

        private async void coffeeImage_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            status.Log(string.Format(CultureInfo.CurrentCulture, LocalizableStrings.CONTEXT_MENU_POPUP_INVOKED, "Image"));
            PopupMenu menu = new PopupMenu();
            try
            {
                menu.Commands.Add(new UICommand("Command 1", (command) =>
                {
                    status.Log(string.Format(
                        CultureInfo.CurrentCulture, LocalizableStrings.CONTEXT_MENU_SELECTED, command.Label));
                }));
                menu.Commands.Add(new UICommand("Command 2", (command) =>
                {
                    status.Log(string.Format(
                        CultureInfo.CurrentCulture, LocalizableStrings.CONTEXT_MENU_SELECTED, command.Label));
                }));

                FrameworkElement element = (FrameworkElement)sender;
                GeneralTransform transform = element.TransformToVisual(null);
                Point point = transform.TransformPoint(new Point());
                Rect rect = new Rect(point, new Size(element.ActualWidth, element.ActualHeight));

                IUICommand chosenCommand = await menu.ShowForSelectionAsync(rect);
            }
            catch (Exception ex)
            {
                status.Log("coffeeImage_RightTapped: " + ex.Message);
            }
        }

        private Rect GetTextboxSelectionRect(TextBox textbox)
        {
            Rect rectFirst, rectLast;
            if (textbox.SelectionStart == textbox.Text.Length)
            {
                rectFirst = textbox.GetRectFromCharacterIndex(textbox.SelectionStart - 1, true);
            }
            else
            {
                rectFirst = textbox.GetRectFromCharacterIndex(textbox.SelectionStart, false);
            }

            int lastIndex = textbox.SelectionStart + textbox.SelectionLength;
            if (lastIndex == textbox.Text.Length)
            {
                rectLast = textbox.GetRectFromCharacterIndex(lastIndex - 1, true);
            }
            else
            {
                rectLast = textbox.GetRectFromCharacterIndex(lastIndex, false);
            }

            GeneralTransform transform = textbox.TransformToVisual(null);
            Point point = transform.TransformPoint(new Point());

            // Make sure that we return a valid rect if selection is on multiple lines
            // and end of the selection is to the left of the start of the selection.
            double x, y, dx, dy;
            y = point.Y + rectFirst.Top;
            dy = rectLast.Bottom - rectFirst.Top;
            if (rectLast.Right > rectFirst.Left)
            {
                x = point.X + rectFirst.Left;
                dx = rectLast.Right - rectFirst.Left;
            }
            else
            {
                x = point.X + rectLast.Right;
                dx = rectFirst.Left - rectLast.Right;
            }

            return new Rect(x, dx, y, dy);
        }
        
        // NOTE: On Mobile, the long-tap in this context shows only the clipboard icon (selectable), not a context menu.
        private async void sampleText_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            status.Log(string.Format(CultureInfo.CurrentCulture, LocalizableStrings.CONTEXT_MENU_POPUP_INVOKED, "TextBox"));
            try
            {
                e.Handled = true;
                TextBox textbox = (TextBox)sender;
                if (textbox.SelectionLength > 0)
                {
                    PopupMenu menu = new PopupMenu();
                    menu.Commands.Add(new UICommand("Copy", null, 1)); // Copy is the default command on the standard TextBox selection context menu.
                    menu.Commands.Add(new UICommandSeparator());
                    menu.Commands.Add(new UICommand("Option 2", null, 2));
                    menu.Commands.Add(new UICommand("Option 3", null, 3));

                    Rect rect = GetTextboxSelectionRect(textbox);
                    IUICommand command = await menu.ShowForSelectionAsync(rect);
                    if (command != null)
                    {
                        switch ((int)command.Id)
                        {
                            case 1:
                                string selectedText = ((TextBox)sender).SelectedText;
                                DataPackage dataPackage = new DataPackage();
                                dataPackage.SetText(selectedText);
                                Clipboard.SetContent(dataPackage);
                                status.Log(string.Format(
                                    CultureInfo.CurrentCulture, LocalizableStrings.CONTEXT_MENU_COPY_TO_CLIPBOARD, selectedText));
                                break;

                            case 2:
                                status.Log(string.Format(
                                    CultureInfo.CurrentCulture, LocalizableStrings.CONTEXT_MENU_SELECTED, command.Label));
                                break;

                            case 3:
                                status.Log(string.Format(
                                    CultureInfo.CurrentCulture, LocalizableStrings.CONTEXT_MENU_SELECTED, command.Label));
                                break;
                        }
                    }
                }
                else
                {
                    status.Log(LocalizableStrings.CONTEXT_MENU_NO_SELECTION);
                }
            }
            catch (Exception ex)
            {
                status.Log("sampleText_ContextMenuOpening: " + ex.Message);
            }
        }

        #endregion
    }
}
