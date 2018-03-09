using CoffeeUniversal.ViewModels;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace CoffeeUniversal.Helpers
{
    public enum ShareMode
    {
        Blank, Text, Uri, Files
    }

    public sealed class ShareHelper
    {
        private static ShareMode currentShareMode;
        private static string currentTitle;
        private static object currentObject;
        private static StorageFile currentFile;

        private ShareHelper() { }

        public static void ShareText(string text)
        {
            currentShareMode = ShareMode.Text;
            currentObject = text;
            Initialize();
        }

        public static void ShareUri(Uri uri)
        {
            currentShareMode = ShareMode.Uri;
            currentObject = uri;
            Initialize();
        }

        public static void ShareFiles(StorageFile file)
        {
            currentShareMode = ShareMode.Files;
            currentFile = file;
            Initialize();
        }

        private static void Initialize()
        {
            currentTitle = LocalizableStrings.SHARE_OPERATION_TITLE;
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += dataTransferManager_DataRequested;
            DataTransferManager.ShowShareUI();
        }

        static void dataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.Properties.Title = currentTitle;
            request.Data.Properties.Description = LocalizableStrings.SHARE_DESCRIPTION;

            switch (currentShareMode)
            {
                case ShareMode.Text:
                    request.Data.SetText((string)currentObject);
                    break;
                case ShareMode.Uri:
                    request.Data.SetWebLink((Uri)currentObject);
                    break;
                case ShareMode.Files:
                    List<IStorageItem> storageItems = new List<IStorageItem>();
                    storageItems.Add(currentFile);
                    request.Data.SetStorageItems(storageItems);
                    break;
            }
        }
    }
}
