using CoffeeUniversal.Helpers;
using System;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Contacts;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using CoffeeUniversal.ViewModels;

namespace CoffeeUniversal.Pages
{
	public sealed partial class ContactsPage : Page
	{

		#region Standard stuff

		private NavigationHelper navigationHelper;
		public NavigationHelper NavigationHelper
		{
			get { return navigationHelper; }
		}

		public ContactsPage()
		{
			InitializeComponent();
			navigationHelper = new NavigationHelper(this);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedTo(e);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
            Debug.WriteLine("ContactsPage_OnNavigatedFrom");
            navigationHelper.OnNavigatedFrom(e);
		}

		#endregion


		async private void selectContact_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				ContactPicker contactPicker = new ContactPicker();
				contactPicker.CommitButtonText = "Select";
                contactPicker.SelectionMode = ContactSelectionMode.Contacts;
                Contact tmp = await contactPicker.PickContactAsync();
				if (tmp == null)
				{
					status.Log(LocalizableStrings.CONTACTS_NO_CONTACT_SELECTED);
					return;
				}
				status.Log(LocalizableStrings.CONTACTS_CONTACT_SELECTED);

                ContactStore store = await ContactManager.RequestStoreAsync();

                // BUG on Desktop tmp.Id is an empty string.
                Debug.WriteLine(tmp.Id);
                if (string.IsNullOrEmpty(tmp.Id))
                {
                    status.Log(LocalizableStrings.CONTACTS_NO_CONTACT_DETAILS);
                    return;
                }

                Contact contact = await store.GetContactAsync(tmp.Id);
                if (contact == null)
                {
                    status.Log(LocalizableStrings.CONTACTS_NO_CONTACT_DETAILS);
                    return;
                }
                status.Log(LocalizableStrings.CONTACTS_CONTACT_DETAILS_FOUND);

                BitmapImage bitmap = null;
				if (contact.Thumbnail != null)
				{
					IRandomAccessStreamWithContentType stream = await contact.Thumbnail.OpenReadAsync();
					if (stream != null && stream.Size > 0)
					{
						bitmap = new BitmapImage();
						bitmap.SetSource(stream);
					}
				}
				thumbnail.Source = bitmap;

				string name = contact.DisplayName;
				if (!string.IsNullOrEmpty(name))
				{
					contactName.Text = name;
				}

                ContactEmail email = contact.Emails.FirstOrDefault();
				if (email != null)
				{
					contactEmail.Text = email.Address;
				}

				ContactPhone phone = contact.Phones.FirstOrDefault();
				if (phone != null)
				{
					contactPhone.Text = phone.Number;
				}

				ContactAddress address = contact.Addresses.FirstOrDefault();
				if (address != null)
				{
					contactAddress.Text = address.StreetAddress;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				status.Log(ex.Message);
            }
		}

	}
}
