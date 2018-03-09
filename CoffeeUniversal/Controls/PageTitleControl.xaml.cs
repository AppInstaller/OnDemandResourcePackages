using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CoffeeUniversal.Controls
{
    public sealed partial class PageTitleControl : UserControl
    {
        public Button BackButton
        {
            get { return backButton; }
        }

        public string PageTitle
        {
            set
            {
                pageTitle.Text = value;
            }
        }

        public PageTitleControl()
        {
            InitializeComponent();

            if (App.IsHardwareBackButtonAvailable)
            {
                backButton.Visibility = Visibility.Collapsed;
                pageTitle.Margin = new Thickness(-80, 0, 30, 40);
            }
        }

        public Page FindParentPage(DependencyObject child)
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;

            Page parent = parentObject as Page;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return FindParentPage(parentObject);
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Page pageRoot = FindParentPage(this);
            Frame frame = pageRoot.Frame;
            if (frame != null && frame.CanGoBack)
            {
                frame.GoBack();
            }
        }
    }
}
