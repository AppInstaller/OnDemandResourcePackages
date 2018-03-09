#include "pch.h"
#include "DirectXPage.xaml.h"

using namespace CoffeeDirectX;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::System::Threading;
using namespace Windows::UI::Core;
using namespace Windows::UI::Input;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace concurrency;
using namespace Windows::Foundation::Metadata;

DirectXPage::DirectXPage():
	m_windowVisible(true),
	m_coreInput(nullptr),
	m_isCleanupDone(false)
{
	InitializeComponent();

	// If there's a hardware back button, hide the software back button, and 
	// hook up a handler for the BackRequested event.
	if (ApiInformation::IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
	{
		backButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
		Thickness margin = Thickness(-50, 0, 30, 40);
		pageTitle->Margin = margin;
	}
	m_backRequestedHandlerToken = SystemNavigationManager::GetForCurrentView()->BackRequested +=
		ref new EventHandler<BackRequestedEventArgs ^>(this, &DirectXPage::OnBackRequested);

	// Register event handlers for page lifecycle.
	m_window = Window::Current->CoreWindow;
	m_VisibilityChangedToken = m_window->VisibilityChanged +=
		ref new TypedEventHandler<CoreWindow^, VisibilityChangedEventArgs^>(this, &DirectXPage::OnVisibilityChanged);

	m_currentDisplayInformation = DisplayInformation::GetForCurrentView();
	m_DpiChangedToken = m_currentDisplayInformation->DpiChanged +=
		ref new TypedEventHandler<DisplayInformation^, Object^>(this, &DirectXPage::OnDpiChanged);
	m_OrientationChangedToken = m_currentDisplayInformation->OrientationChanged +=
		ref new TypedEventHandler<DisplayInformation^, Object^>(this, &DirectXPage::OnOrientationChanged);
	m_DisplayContentsInvalidatedToken = DisplayInformation::DisplayContentsInvalidated +=
		ref new TypedEventHandler<DisplayInformation^, Object^>(this, &DirectXPage::OnDisplayContentsInvalidated);
	m_CompositionScaleChangedToken = swapChainPanel->CompositionScaleChanged +=
		ref new TypedEventHandler<SwapChainPanel^, Object^>(this, &DirectXPage::OnCompositionScaleChanged);
	m_SizeChangedToken = swapChainPanel->SizeChanged +=
		ref new SizeChangedEventHandler(this, &DirectXPage::OnSwapChainPanelSizeChanged);

	// At this point we have access to the device, and we can create the device-dependent resources.
	m_deviceResources = std::make_shared<DX::DeviceResources>();
	m_deviceResources->SetSwapChainPanel(swapChainPanel);

	m_swapChainPanelKeyDownToken = swapChainPanel->KeyDown += ref new ::Windows::UI::Xaml::Input::KeyEventHandler(this, &DirectXPage::swapChainPanel_KeyDown);
	m_swapChainPanelKeyUpToken = swapChainPanel->KeyUp += ref new ::Windows::UI::Xaml::Input::KeyEventHandler(this, &DirectXPage::swapChainPanel_KeyUp);
	m_buttonClickToken = backButton->Click += ref new ::Windows::UI::Xaml::RoutedEventHandler(this, &DirectXPage::backButton_Click);

	// Register our SwapChainPanel to get independent input pointer events
	m_workItemHandler = ref new WorkItemHandler([this](IAsyncAction ^) 
	{
		// The CoreIndependentInputSource will raise pointer events for the specified device types on whichever thread it's created on.
		m_coreInput = swapChainPanel->CreateCoreIndependentInputSource(
			Windows::UI::Core::CoreInputDeviceTypes::Mouse |
			Windows::UI::Core::CoreInputDeviceTypes::Touch |
			Windows::UI::Core::CoreInputDeviceTypes::Pen
			);

		// Register for pointer events, which will be raised on the background thread.
		m_PointerPressedToken = m_coreInput->PointerPressed += ref new TypedEventHandler<Object^, PointerEventArgs^>(this, &DirectXPage::OnPointerPressed);
		m_PointerMovedToken = m_coreInput->PointerMoved += ref new TypedEventHandler<Object^, PointerEventArgs^>(this, &DirectXPage::OnPointerMoved);
		m_PointerReleasedToken = m_coreInput->PointerReleased += ref new TypedEventHandler<Object^, PointerEventArgs^>(this, &DirectXPage::OnPointerReleased);

		// Begin processing input messages as they're delivered.
		m_coreInput->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessUntilQuit);

		m_coreInput->PointerPressed -= m_PointerPressedToken;
		m_coreInput->PointerMoved -= m_PointerMovedToken;
		m_coreInput->PointerReleased -= m_PointerReleasedToken;
	});

	// Run task on a dedicated high priority background thread.
	m_inputLoopWorker = ThreadPool::RunAsync(m_workItemHandler, WorkItemPriority::High, WorkItemOptions::TimeSliced);

	m_main = std::unique_ptr<DirectXandXamlCppMain>(new DirectXandXamlCppMain(m_deviceResources));
	m_main->StartRenderLoop();
}

void DirectXPage::Cleanup()
{
	if (!m_isCleanupDone)
	{
		if (m_coreInput != nullptr)
		{
			m_coreInput->Dispatcher->StopProcessEvents();
		}
		if (m_main != nullptr)
		{
			m_main->StopRenderLoop();
		}

		swapChainPanel->KeyDown -= m_swapChainPanelKeyDownToken;
		swapChainPanel->KeyUp -= m_swapChainPanelKeyUpToken;
		backButton->Click -= m_buttonClickToken;

		swapChainPanel->SizeChanged -= m_SizeChangedToken;
		swapChainPanel->CompositionScaleChanged -= m_CompositionScaleChangedToken;
		DisplayInformation::DisplayContentsInvalidated -= m_DisplayContentsInvalidatedToken;
		m_currentDisplayInformation->OrientationChanged -= m_OrientationChangedToken;
		m_currentDisplayInformation->DpiChanged -= m_DpiChangedToken;
		m_window->VisibilityChanged -= m_VisibilityChangedToken;
		SystemNavigationManager::GetForCurrentView()->BackRequested -= m_backRequestedHandlerToken;

		delete m_workItemHandler;  m_workItemHandler = nullptr;
		delete m_inputLoopWorker; m_inputLoopWorker = nullptr;
		delete m_coreInput; m_coreInput = nullptr;
		delete backButton; backButton = nullptr;
		delete pageTitle; pageTitle = nullptr;
		delete swapChainPanel; swapChainPanel = nullptr;
		m_deviceResources._Decref();
		delete m_currentDisplayInformation; m_currentDisplayInformation = nullptr;
		delete m_window; m_window = nullptr;

		m_isCleanupDone = true;
	}
}


DirectXPage::~DirectXPage()
{
	// Stop rendering and processing events on destruction.
	//if (m_coreInput != nullptr)
	//{
	//	m_coreInput->Dispatcher->StopProcessEvents();
	//}
	//if (m_main != nullptr)
	//{
	//	m_main->StopRenderLoop();
	//}
	Cleanup();
}

// Saves the current state of the app for suspend and terminate events.
void DirectXPage::SaveInternalState(IPropertySet^ state)
{
	critical_section::scoped_lock lock(m_main->GetCriticalSection());
	m_deviceResources->Trim();

	// Stop rendering when the app is suspended.
	m_main->StopRenderLoop();

	// Put code to save app state here.
}

// Loads the current state of the app for resume events.
void DirectXPage::LoadInternalState(IPropertySet^ state)
{
	// Put code to load app state here.

	// Start rendering when the app is resumed.
	m_main->StartRenderLoop();
}

// Window event handlers.
void DirectXPage::OnVisibilityChanged(CoreWindow^ sender, VisibilityChangedEventArgs^ args)
{
	m_windowVisible = args->Visible;
	if (m_windowVisible)
	{
		m_main->StartRenderLoop();
	}
	else
	{
		m_main->StopRenderLoop();
	}
}

// DisplayInformation event handlers.
void DirectXPage::OnDpiChanged(DisplayInformation^ sender, Object^ args)
{
	critical_section::scoped_lock lock(m_main->GetCriticalSection());
	m_deviceResources->SetDpi(sender->LogicalDpi);
	m_main->CreateWindowSizeDependentResources();
}

void DirectXPage::OnOrientationChanged(DisplayInformation^ sender, Object^ args)
{
	critical_section::scoped_lock lock(m_main->GetCriticalSection());
	m_deviceResources->SetCurrentOrientation(sender->CurrentOrientation);
	m_main->CreateWindowSizeDependentResources();
}

void DirectXPage::OnDisplayContentsInvalidated(DisplayInformation^ sender, Object^ args)
{
	critical_section::scoped_lock lock(m_main->GetCriticalSection());
	m_deviceResources->ValidateDevice();
}

void DirectXPage::OnPointerPressed(Object^ sender, PointerEventArgs^ e)
{
	// When the pointer is pressed begin tracking the pointer movement.
	m_main->StartTracking();
}

void DirectXPage::OnPointerMoved(Object^ sender, PointerEventArgs^ e)
{
	// Update the pointer tracking code.
	if (m_main->IsTracking())
	{
		m_main->TrackingUpdate(e->CurrentPoint->Position.X);
	}
}

void DirectXPage::OnPointerReleased(Object^ sender, PointerEventArgs^ e)
{
	// Stop tracking pointer movement when the pointer is released.
	m_main->StopTracking();
}

void DirectXPage::OnCompositionScaleChanged(SwapChainPanel^ sender, Object^ args)
{
	critical_section::scoped_lock lock(m_main->GetCriticalSection());
	m_deviceResources->SetCompositionScale(sender->CompositionScaleX, sender->CompositionScaleY);
	m_main->CreateWindowSizeDependentResources();
}

void DirectXPage::OnSwapChainPanelSizeChanged(Object^ sender, SizeChangedEventArgs^ e)
{
	critical_section::scoped_lock lock(m_main->GetCriticalSection());
	m_deviceResources->SetLogicalSize(e->NewSize);
	m_main->CreateWindowSizeDependentResources();
}

void DirectXPage::TryGoBack()
{
	if (Frame->CanGoBack)
	{
		Cleanup();
		Frame->GoBack();
	}
}

void DirectXPage::backButton_Click(Object^ sender, RoutedEventArgs^ e)
{
	TryGoBack();
}

void DirectXPage::OnBackRequested(Object ^sender, BackRequestedEventArgs ^args)
{
	args->Handled = true;
	SystemNavigationManager::GetForCurrentView()->BackRequested -= m_backRequestedHandlerToken;
	TryGoBack();
}

void DirectXPage::swapChainPanel_KeyDown(Object^ sender, KeyRoutedEventArgs^ e)
{
	if (e->Key == Windows::System::VirtualKey::Left || e->Key == Windows::System::VirtualKey::Right)
	{
		m_main->StartTracking();
		if (m_main->IsTracking())
		{
			m_main->TrackingUpdate(m_main->GetCurrentPosition() - MANUAL_ROTATION_SPEED);
		}
	}
}

void DirectXPage::swapChainPanel_KeyUp(Object^ sender, KeyRoutedEventArgs^ e)
{
	if (e->Key == Windows::System::VirtualKey::Left || e->Key == Windows::System::VirtualKey::Right)
	{
		m_main->StopTracking();
	}
}
