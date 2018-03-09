namespace CoffeeUniversal.ViewModels
{
	public class LocalizableStrings
	{
        public const string APP_UNHANDLED_EXCEPTION = "Caught an unhandled exception - please restart the app.";
        public const string APP_NO_INITIAL_PAGE = "Failed to create initial page.";
		public const string APP_FAILED_TO_LOAD_PAGE = "Failed to load Page {0}";
		public const string APP_INVALID_FRAME_KEY = "Frames can only be registered to one session state key.";
		public const string APP_INVALID_FRAME_REGISTRATION = "Frames must be either be registered before accessing frame session state, or not registered at all.";
		public const string APP_SUSPENSION_MANAGER_FAIL = "SuspensionManager failed.";

        public const string APPSERVICE_CREATE_SUCCESS = "App service activation success.";
        public const string APPSERVICE_CREATE_FAIL = "App service activation failed.";
        public const string APPSERVICE_SEND_FAIL = "Failed to send message.";
        public const string APPSERVICE_SEND_SUCCESS = "Sent message to app service.";
        public const string APPSERVICE_SEND_RESULT = "Sent {0}, received {1}.";
        public const string APPSERVICE_INVALID_INPUT = "Invalid input.";
        public const string APPSERVICE_CLOSED = "App service closed.";
        public const string APPSERVICE_CONSUMER_RECEIPT = "From app service: [{0},{1},{2}].";

        public const string BACKGROUND_AUDIO_TASK_INVOKED = "Audio task invoked.";
        public const string BACKGROUND_AUDIO_TASK_INITIALIZED = "Audio task initialized.";
        public const string BACKGROUND_AUDIO_INIT_FAIL = "Audio Task could not initialize: {0}";
        public const string BACKGROUND_AUDIO_SKIP_FORWARD = "Audio skip forward.";
        public const string BACKGROUND_AUDIO_SKIP_BACK = "Audio skip back.";
        public const string BACKGROUND_AUDIO_STARTED = "Audio playback started.";
        public const string BACKGROUND_AUDIO_PAUSED = "Audio paused.";

        public const string BACKGROUND_TASK_TIMEZONE_NAME = "CoffeeTimeZoneTask";
        public const string BACKGROUND_TASK_TIMEZONE_ENTRYPOINT = "CoffeeBackgroundWinRT.CoffeeBackgroundTimeZoneTask";
        public const string BACKGROUND_TASK_APPTRIGGER_NAME = "CoffeeAppTask";
        public const string BACKGROUND_TASK_APPTRIGGER_ENTRYPOINT = "CoffeeBackgroundWinRT.CoffeeBackgroundAppTask";
        public const string BACKGROUND_TASK_REGISTRATION_FAIL = "Background task cannot be registered.";
		public const string BACKGROUND_TASK_REGISTRATION_SUCCESS = "Background task successfully registered.";
		public const string BACKGROUND_TASK_UNREGISTERED = "Background tasks unregistered.";

        public const string BACKGROUND_TRANSFER_INVALID_URI = "Invalid URI.";
        public const string BACKGROUND_TRANSFER_INVALID_FILENAME = "Invalid local filename.";
        public const string BACKGROUND_TRANSFER_CANCELLED = "Transfer cancelled: {0}.";
        public const string BACKGROUND_TRANSFER_FOUND = "Found download: {0}, status={1}.";
        public const string BACKGROUND_TRANSFER_PROGRESS = "Progress: {0}, status={1}.";
        public const string BACKGROUND_TRANSFER_BYTES = "Transfered bytes: {0} of {1}, {2}%.";
        public const string BACKGROUND_TRANSFER_RUNNING = "Transfer running: {0}.";
        public const string BACKGROUND_TRANSFER_COMPLETED = "Completed: {0}, status={1}.";
        public const string BACKGROUND_TRANSFER_PAUSED = "Paused: {0}.";
        public const string BACKGROUND_TRANSFER_RESUMED = "Resumed: {0}.";
        public const string BACKGROUND_TRANSFER_DOWNLOAD_COUNT = "Downloads: {0}.";
        public const string BACKGROUND_TRANSFER_SKIPPED = "Skipped: {0}, status={1}.";
        public const string BACKGROUND_TRANSFER_CANCEL = "Canceling downloads.";
        public const string BACKGROUND_TRANSFER_NOT_FOUND = "No transfers found.";

        public const string CALENDAR_APPOINTMENTSTORE_FIND_SUCCESS = "Appointment store found.";
        public const string CALENDAR_APPOINTMENTSTORE_FIND_FAIL = "Appointment store not found.";
        public const string CALENDAR_FIND_CALENDARS_RESULT = "{0} calendars found.";
        public const string CALENDAR_FIND_APPOINTMENTS_RESULT = "{0} appointments found.";
        public const string CALENDAR_APPOINTMENT_REPORT = "Details: {0}";
        public const string CALENDAR_APPOINTMENT_SUBJECT = "Espresso-{0:D3}";
        public const string CALENDAR_APPOINTMENT_DETAILS = "Lorem ipsum dolor.";
        public const string CALENDAR_APPOINTMENT_LOCATION = "The Cafe";
        public const string CALENDAR_APPOINTMENT_ADD_SUCCESS = "Add appointment success.";
        public const string CALENDAR_APPOINTMENT_REMOVE_SUCCESS = "Remove appointment success.";
        public const string CALENDAR_APPOINTMENT_ADD_FAIL = "Add appointment failed.";
        public const string CALENDAR_APPOINTMENT_REMOVE_FAIL = "Remove appointment failed.";

        public const string CAMERA_NOT_FOUND = "Cannot find camera.";
        public const string CAMERA_INITIALIZATION_SUCCESS = "Camera initialization succeeded.";
        public const string CAMERA_INITIALIZATION_FAIL = "Camera initialization failed.";
        public const string CAMERA_PREVIEW_SUCCESS = "Camera preview succeeded.";
        public const string CAMERA_CAPTURE_SUCCESS = "Camera capture succeeded.";

        public const string CONTEXT_MENU_SELECTED = "{0} selected.";
        public const string CONTEXT_MENU_COPY_TO_CLIPBOARD = "'{0}' copied to clipboard.";
        public const string CONTEXT_MENU_NO_SELECTION = "No text selected.";
        public const string CONTEXT_MENU_POPUP_INVOKED = "{0} popup menu invoked.";

        public const string CONTACTS_CONTACT_SELECTED = "Contact selected.";
        public const string CONTACTS_NO_CONTACT_SELECTED = "No contact selected.";
        public const string CONTACTS_CONTACT_DETAILS_FOUND = "Found contact details.";
        public const string CONTACTS_NO_CONTACT_DETAILS = "Cannot retrieve contact details.";

        public const string DESKTOP_EXTENSION_SETTINGS_FOUND = " Desktop Settings API found.";
        public const string DESKTOP_EXTENSION_SETTINGS_NOT_FOUND = "Desktop Settings API not found.";

        public const string DISPLAY_REQUEST_ACTIVATED = "Request activated [{0}].";
        public const string DISPLAY_REQUEST_ALREADY_ACTIVATED = "Previous activation [{0}].";
        public const string DISPLAY_REQUEST_NOT_ACTIVATED = "No active request.";
        public const string DISPLAY_REQUEST_RELEASED = "Request released.";

        public const string ENERGY_BATTERY_PRESENT = "Battery found.";
        public const string ENERGY_NO_BATTERY_PRESENT = "No battery found.";
        public const string FOREGROUND_ENERGY_MANAGER_CONNECTED = "Foreground EnergyManager connected.";
        public const string FOREGROUND_ENERGY_MANAGER_FAIL = "Foreground EnergyManager not supported.";
        public const string ENERGY_WORK_STARTED = "Started {0}";
        public const string ENERGY_WORK_STOPPED = "Stopped {0}";

        public const string ENTERPRISE_SETTINGS_CONTAINER_NOT_FOUND = "Enterprise container not found.";
        public const string ENTERPRISE_SETTINGS_WELCOME_FOUND = "Found Welcome setting.";
        public const string ENTERPRISE_SETTINGS_DESCRIPTION_FOUND = "Found Description setting.";
        public const string ENTERPRISE_SETTINGS_WELCOME_NOT_FOUND = "Welcome setting not found.";
        public const string ENTERPRISE_SETTINGS_DESCRIPTION_NOT_FOUND = "Description setting not found.";

        public const string EXTENDED_EXECUTION_DESCRIPTION = "Test Extended Execution";
        public const string EXTENDED_EXECUTION_REVOKED = "Extended execution revoked.";
        public const string EXTENDED_EXECUTION_TEXT = "Extended execution: {0}.";
        public const string EXTENDED_EXECUTION_CANCELLED = "Extended execution cancelled.";
        public const string EXTENDED_EXECUTION_SUSPENDING = "Suspending.";
        public const string EXTENDED_EXECUTION_WORK_STARTED = "Work started.";
        public const string EXTENDED_EXECUTION_WORK_STOPPED = "Work stopped.";

        public const string FILE_PICKER_SELECTED_FILE = "Picked the \"{0}\" file.";
		public const string FILE_PICKER_NO_SELECTION = "No file selected.";
		public const string FILE_PICKER_SAVE_SUCCESS = "File \"{0}\" saved.";
		public const string FILE_PICKER_SAVE_FAIL = "Failed to save \"{0}\".";

		public const string INK_EMPTY_INK_RECOGNIZE = "Cannot recognize empty ink.";
		public const string INK_EMPTY_INK_SAVE = "Cannot save empty ink.";
		public const string INK_STROKES_CLEAR_SUCCESS = "All strokes cleared.";
		public const string INK_LOAD_SUCCESS = "Ink load completed.";
		public const string INK_SAVE_SUCCESS = "Ink save completed.";
		public const string INK_RECOGNIZER_PREFIX = "Recognized: ";
        public const string INK_RECOGNIZER_FAIL = "Failed to recognize ink.";
        public const string INK_RECOGNIZER_SEARCH = "Searching for {0}.";
        public const string INK_RECOGNIZER_FOUND = "Found recognizer: {0}.";
        public const string INK_RECOGNIZER_NOT_FOUND = "Preferred recognizer not found.";
        public const string INK_NO_RECOGNIZERS = "No recognizers available.";

        public const string IOT_GPIO_NONE = "No gpio controller found.";
        public const string IOT_GPIO_PIN_NOTOPEN = "Pin {0} is not open.";
        public const string IOT_GPIO_PIN_INUSE = "Pin {0}: In use or not a valid pin.";
        public const string IOT_GPIO_PIN_OPENED = "Pin {0}: Opened successfully.";

        public const string IOT_SPI_FOUND = "{0} SPI device(s) found. If settings are changed, device must be reactivated.";
        public const string IOT_SPI_NONE = "No SPI controller found. Exception: {0}";
        public const string IOT_SPI_SETTINGS_INVALID = "{0} settings invalid: {1}";
        public const string IOT_SPI_ACTIVATE_ERROR = "Error activating {0}: {1}";
        public const string IOT_SPI_ACTIVATED = "{0} activated.";
        public const string IOT_SPI_OUT = "{0} Out: {1}";
        public const string IOT_SPI_IN = "{0} In: {1}";
        public const string IOT_SPI_WROTE = "{0} Wrote: {1}";
        public const string IOT_SPI_NOT_INITIALIZED = "{0} Controller not yet initialized.";

        public const string IOT_I2C_NONE = "No I²C controllers found. Exception: {0}";
        public const string IOT_I2C_FOUND = "{0} I²C controller(s) found. If settings are changed, device must be reactivated.";
        public const string IOT_I2C_WRITE = "{0} wrote {1} to {2}.";
        public const string IOT_I2C_READ = "{0} read {1} from {2}.";
        public const string IOT_I2C_NORESPONSE = "{0} received no response from {1}.";
        public const string IOT_I2C_NOT_INITIALIZED = "{0} is not yet initialized.";
        public const string IOT_I2C_ADDRESS_INVALID = "Address entered incorrectly, cannot activate {0}.";
        public const string IOT_I2C_IN_USE = "Address {0} on {1} is in use, could not initialize.";
        public const string IOT_I2C_INITIALIZED = "Initialized address {0} on {1}.";
        public const string IOT_I2C_INITIALIZE_FAILED = "I²C Initialization failed. Exception: {0}";

        public const string LOCATION_NOT_ALLOWED = "Location tracking not allowed.";
		public const string LOCATION_CANNOT_GET_CURRENT_POSITION = "Cannot get current position.";
		public const string LOCATION_CANNOT_GET_TARGET_POSITION = "Cannot get target position.";
		public const string LOCATION_ROUTE_ERROR = "Error: MapRouteFinderStatus = {0}";
		public const string LOCATION_GETTING_ROUTE = "Getting route...";
        public const string LOCATION_FOUND_ROUTE = "Route directions found.";

        public const string LAUNCHERS_LOREM_IPSUM_BODY = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
        public const string LAUNCHERS_APP_LAUNCH_SUCCESS = "App for \"{0}\" launched.";
        public const string LAUNCHERS_APP_LAUNCH_FAIL = "Failed to launch app for \"{0}\".";
        public const string LAUNCHERS_INVALID_FILE = "{0} not found.";
        public const string LAUNCHERS_EMAIL_SUCCESS = "Launched email.";
        public const string LAUNCHERS_EMAIL_FAIL = "Failed to launch email.";
        public const string LAUNCHERS_STORE_SUCCESS = "Launched store app.";
        public const string LAUNCHERS_STORE_FAIL = "Failed to launch store app.";
        public const string LAUNCHERS_NO_CONTACT = "No contact selected.";
        public const string LAUNCHERS_NO_EMAIL = "No email selected.";
        public const string LAUNCHERS_FALLBACK_URI_SUCCESS = "Launched browser and navigated to {0}.";
        public const string LAUNCHERS_FALLBACK_URI_FAIL = "Failed to launch browser and navigate to {0}.";
        public const string LAUNCHERS_FOR_RESULTS_SUCCESS = "Launch Uri ForResults succeeded: {0}.";
        public const string LAUNCHERS_FOR_RESULTS_FAIL = "Launch Uri ForResults failed: {0}.";
        public const string LAUNCHERS_FOR_RESULTS_WITH_DATA_SUCCESS = "Launch Uri ForResultsWithData succeeded: {0}.";
        public const string LAUNCHERS_TARGET_PFN_BAD_URI_SUCCESS = "Launch failed as expected for URI={0} and PFN={1}.";
        public const string LAUNCHERS_TARGET_PFN_BAD_URI_FAIL = "Launch did NOT fail as expected for URI={0} and PFN={1}.";
        public const string LAUNCHERS_UNSUPPORTED_URI_SUCCESS = "Verify that Store launched and shows app search results for {0}.";
        public const string LAUNCHERS_UNSUPPORTED_URI_FAIL = "Failed to launch Store with app search results for {0}.";
        public const string LAUNCHERS_URI_BAD_PFN_SUCCESS = "Launch failed as expected with bad PFN ({0}).";
        public const string LAUNCHERS_URI_BAD_PFN_FAIL = "Launch did NOT fail as expected with bad PFN ({0}).";
        public const string LAUNCHERS_URI_BAD_PROTOCOL_SUCCESS = "Launch failed as expected with bad protocol ({0}).";
        public const string LAUNCHERS_URI_BAD_PROTOCOL_FAIL = "Launch did NOT fail as expected with bad protocol ({0}).";
        public const string LAUNCHERS_FILE_BAD_FILE_SUCCESS = "Verify that Store launched and shows app search results for .wechat showing wechat app.";
        public const string LAUNCHERS_FILE_BAD_FILE_FAIL = "Failed to launch Store launched with app search results for .wechat showing wechat app.";
        public const string LAUNCHERS_PFN_BAD_FILE_SUCCESS = "Launch failed as expected for extension=.wechat and PFN={0}.";
        public const string LAUNCHERS_PFN_BAD_FILE_FAIL = "Launch did NOT fail as expected for extension=.wechat and PFN={0}.";
        public const string LAUNCHERS_CREATE_FILE_FAIL = "Failed to create storage file ({0}).";
        public const string LAUNCHERS_LAUNCH_FOLDER_SUCCESS = "File explorer launched for folder {0}.";
        public const string LAUNCHERS_LAUNCH_FOLDER_FAIL = "Failed to launch file explorer for folder {0}.";
        public const string LAUNCHERS_OPEN_FOLDER_FAIL = "Failed to open storage folder {0}.";
        public const string LAUNCHERS_FOLDER_SELECTED_SUCCESS = "File explorer launched for folder {0}. Verify 2 files selected.";
        public const string LAUNCHERS_FOLDER_SELECTED_FAIL = "Failed to launch file explorer launched for folder {0}.";

        public const string MAINMENU_APPBAR = "AppBar";
        public const string MAINMENU_APPSERVICE = "AppService";
        public const string MAINMENU_BACKGROUND_AUDIO = "BG Audio";
        public const string MAINMENU_BACKGROUND_TASKS = "BG Tasks";
        public const string MAINMENU_BACKGROUND_TRANSFER = "BG Transfer";
        public const string MAINMENU_CALENDAR = "Calendar";
        public const string MAINMENU_CAMERA = "Camera";
        public const string MAINMENU_CONTACTS = "Contacts";
        public const string MAINMENU_CRASH = "Crash";
        public const string MAINMENU_DESKTOP_EXTENSIONS = "Desk Ext";
        public const string MAINMENU_DIRECTX = "DirectX";
        public const string MAINMENU_DISPLAY_REQUESTS = "Disp Reqs";
        public const string MAINMENU_ENERGY = "Energy";
        public const string MAINMENU_EXTENDED_EXECUTION = "Extended Ex";
        public const string MAINMENU_FILE_PICKERS = "Pickers";
        public const string MAINMENU_INK = "Ink";
        public const string MAINMENU_IOT_EXTENSIONS = "IoT Ext";
        public const string MAINMENU_LAUNCHERS = "Launchers";
        public const string MAINMENU_LOCATION = "Location";
        public const string MAINMENU_MANDATORY_UPDATES = "Mandatory Updates"; //Jasosal
        public const string MAINMENU_MEDIA = "Media";
        public const string MAINMENU_MEMORY = "Memory";
        public const string MAINMENU_MOBILE_EXTENSIONS = "Mobile Ext";
        public const string MAINMENU_MULTIVIEW = "MultiView";
        public const string MAINMENU_OPTIONAL_PACKAGES = "Optional Packages"; //Jasosal
        public const string MAINMENU_ORIENTATION = "Orientation";
        public const string MAINMENU_PUBLISHER_FOLDER = "Pub Folder";
        public const string MAINMENU_RESOURCES = "Resources";
        public const string MAINMENU_SENSORS = "Sensors";
        public const string MAINMENU_STORAGE = "Settings";
        public const string MAINMENU_SHARE = "Share";
        public const string MAINMENU_SPEECH = "Speech";
        public const string MAINMENU_TILES = "Tiles";
        public const string MAINMENU_TOASTS = "Toasts";
        public const string MAINMENU_WEB_REQUESTS = "Web Reqs";
        public const string MAINMENU_WEBVIEW = "WebView";
        public const string MAINMENU_WINRT = "WinRT";
        public const string MAINMENU_XBOX_EXTENSIONS = "Xbox Ext";
        public const string MAINMENU_EXTENSIONS = "Extensions";

        public const string MEDIA_LOCAL_PLAY_STARTED = "Local playback started.";
        public const string MEDIA_LOCAL_PLAY_PAUSED = "Local playback paused.";
        public const string MEDIA_REMOTE_PLAY_STARTED = "Remote playback started.";
        public const string MEDIA_REMOTE_PLAY_PAUSED = "Remote playback paused.";
        public const string MEDIA_VIDEO_NOT_FOUND = "{0} video not found.";

        public const string MOBILE_EXTENSION_BATTERY_FOUND = "Mobile Battery API found.";
        public const string MOBILE_EXTENSION_BATTERY_NOT_FOUND = "Mobile Battery API not found.";

        public const string MULTIVIEW_DEFAULT_TITLE = "Espresso-{0:D3}";
        public const string MULTIVIEW_CREATE_VIEW_SUCCESS = "Created view {0}.";
        public const string MULTIVIEW_CREATE_VIEW_FAIL = "Failed to create view.";
        public const string MULTIVIEW_SHOW_VIEW_SUCCESS = "View {0} shown.";
        public const string MULTIVIEW_SHOW_VIEW_FAIL = "Failed to show view.";

        public const string PUBLISHER_FOLDER_FILE_CREATED = "Created file \"{0}\".";
        public const string PUBLISHER_FOLDER_FILE_CONTENTS = "File contents: \"{0}\".";
        public const string PUBLISHER_FOLDER_FILES_DELETED = "All files in shared folder deleted.";

        public const string SENSORS_NO_ACCELEROMETER = "No accelerometer found.";
        public const string SENSORS_ACCELEROMETER_INITIALIZED = "Accelerometer initialized.";
        public const string SENSORS_NO_GYROMETER = "No gyrometer found.";
        public const string SENSORS_GYROMETER_INITIALIZED = "Gyrometer initialized.";
        public const string SENSORS_NO_COMPASS = "No compass found.";
        public const string SENSORS_COMPASS_INITIALIZED = "Compass initialized.";
        public const string SENSORS_COMPASS_STARTED = "Compass readings started.";
        public const string SENSORS_COMPASS_STOPPED = "Compass readings stopped.";
        public const string SENSORS_ACCELEROMETER_STARTED = "Accelerometer readings started.";
        public const string SENSORS_ACCELEROMETER_STOPPED = "Accelerometer readings stopped.";
        public const string SENSORS_GYROMETER_STARTED = "Gyrometer readings started.";
        public const string SENSORS_GYROMETER_STOPPED = "Gyrometer readings stopped.";

        public const string SHARE_OPERATION_TITLE = "Share from Coffee";
		public const string SHARE_DESCRIPTION = "Some description.";
		public const string SHARE_TEXT_CONTENT = "Sed ut perspiciatis unde omnis, iste natus error, sit voluptatem accusantium doloremque laudantium";
		public const string SHARE_TYPE_TEXT = "text";
		public const string SHARE_TYPE_URI = "uri";
		public const string SHARE_TYPE_FILE = "file";

        public const string SPEECH_RECOGNITION_DISABLED = "Speech recognition is disabled. Go to Settings | Privacy | Speech if you want to re-enable it.";

		public const string STORAGE_COPIED_TO_CLIPBOARD = "\"{0}\" copied to Clipboard.";

        public const string TILE_TITLE_ESPRESSO = "Espresso";
		public const string TILE_TITLE_LATTE = "Latte";
		public const string TILE_TITLE_CAPPUCCINO = "Cappuccino";
        public const string TILE_LOREM_IPSUM_BODY_1 = "Sed ut perspiciatis unde omnis, iste natus error, sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae, ab illo inventore veritatis, et quasi architecto beatae vitae, dicta sunt explicabo.";
		public const string TILE_LOREM_IPSUM_BODY_2 = "Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molest.";
		public const string TILE_LOREM_IPSUM_BODY_3 = "Et harum quidem rerum facilis est et expedita distinctio. Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omni.";

        public const string TOAST_SENT = "Sent toast ({0}).";
        public const string TOAST_FAILED = "Toast failed: ({0}).";
        public const string TOAST_SIMPLE_TEXT = "simple text";
		public const string TOAST_LOCAL_IMAGE = "local image";
		public const string TOAST_WEB_IMAGE = "web image";
		public const string TOAST_WITH_SOUND = "with sound";
		public const string TOAST_LONG_DURATION = "long duration";
		public const string TOAST_WITH_PARAMETERS = "with parameters";
		public const string TOAST_LAUNCHED_FROM_TOAST = "Launched from toast: {0}";
        public const string TOAST_PROTOCOL_TITLE = "Tap me!";
        public const string TOAST_PROTOCOL_TEXT = "...to find coffee images.";
        public const string TOAST_PROTOCOL = "protocol";
        public const string TOAST_INPUT = "input";
        public const string TOAST_SNOOZE = "snooze";
        public const string TOAST_ICON_OVERRIDE = "icon override";

        public const string WEB_REQUEST_INIT = "Scanning for coffee images.";
        public const string WEB_REQUEST_DATA = "Found images: downloading...";

        public const string XBOX_EXTENSION_SOUNDCLIP_FOUND = "Xbox SoundClip API found.";
        public const string XBOX_EXTENSION_USER_FOUND = "Xbox User API found.";
        public const string XBOX_EXTENSION_SOUNDCLIP_NOT_FOUND = "Xbox SoundClip API not found.";
        public const string XBOX_EXTENSION_USER_NOT_FOUND = "Xbox User API not found.";

    }
}
