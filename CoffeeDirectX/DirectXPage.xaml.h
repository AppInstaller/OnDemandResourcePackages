#pragma once

#include "DirectXPage.g.h"
#include "Common\DeviceResources.h"
#include "DirectXandXamlCppMain.h"

namespace CoffeeDirectX
{
	// A page that hosts a DirectX SwapChainPanel.
	public ref class DirectXPage sealed
	{
	public:
		DirectXPage();
		virtual ~DirectXPage();

		void SaveInternalState(Windows::Foundation::Collections::IPropertySet^ state);
		void LoadInternalState(Windows::Foundation::Collections::IPropertySet^ state);

	private:
		// XAML low-level rendering event handler.
		void OnRendering(Platform::Object^ sender, Platform::Object^ args);

		// Window event handlers.
		void OnVisibilityChanged(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::VisibilityChangedEventArgs^ args);

		// DisplayInformation event handlers.
		void OnDpiChanged(Windows::Graphics::Display::DisplayInformation^ sender, Platform::Object^ args);
		void OnOrientationChanged(Windows::Graphics::Display::DisplayInformation^ sender, Platform::Object^ args);
		void OnDisplayContentsInvalidated(Windows::Graphics::Display::DisplayInformation^ sender, Platform::Object^ args);

		// Other event handlers.
		void AppBarButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void OnCompositionScaleChanged(Windows::UI::Xaml::Controls::SwapChainPanel^ sender, Object^ args);
		void OnSwapChainPanelSizeChanged(Platform::Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e);

		// Track our independent input on a background worker thread.
		Windows::Foundation::IAsyncAction^ m_inputLoopWorker;
		Windows::UI::Core::CoreIndependentInputSource^ m_coreInput;

		// Independent input handling functions.
		void OnPointerPressed(Platform::Object^ sender, Windows::UI::Core::PointerEventArgs^ e);
		void OnPointerMoved(Platform::Object^ sender, Windows::UI::Core::PointerEventArgs^ e);
		void OnPointerReleased(Platform::Object^ sender, Windows::UI::Core::PointerEventArgs^ e);

		// Resources used to render the DirectX content in the XAML page background.
		std::shared_ptr<DX::DeviceResources> m_deviceResources;
		std::unique_ptr<DirectXandXamlCppMain> m_main; 

		bool m_windowVisible;
		void backButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void OnBackRequested(Platform::Object ^sender, Windows::UI::Core::BackRequestedEventArgs ^args);
		void TryGoBack();
		void Cleanup();
		bool m_isCleanupDone;

		void swapChainPanel_KeyDown(Platform::Object^ sender, Windows::UI::Xaml::Input::KeyRoutedEventArgs^ e);
		void swapChainPanel_KeyUp(Platform::Object^ sender, Windows::UI::Xaml::Input::KeyRoutedEventArgs^ e);

		Windows::Graphics::Display::DisplayInformation^ m_currentDisplayInformation;
		Windows::UI::Core::CoreWindow^ m_window;

		Windows::Foundation::EventRegistrationToken m_backRequestedHandlerToken;
		Windows::Foundation::EventRegistrationToken m_DpiChangedToken;
		Windows::Foundation::EventRegistrationToken m_OrientationChangedToken;
		Windows::Foundation::EventRegistrationToken m_DisplayContentsInvalidatedToken;
		Windows::Foundation::EventRegistrationToken m_CompositionScaleChangedToken;
		Windows::Foundation::EventRegistrationToken m_SizeChangedToken;
		Windows::Foundation::EventRegistrationToken m_VisibilityChangedToken;
		Windows::Foundation::EventRegistrationToken m_PointerPressedToken;
		Windows::Foundation::EventRegistrationToken m_PointerMovedToken;
		Windows::Foundation::EventRegistrationToken m_PointerReleasedToken;

		Windows::Foundation::EventRegistrationToken m_swapChainPanelKeyDownToken;
		Windows::Foundation::EventRegistrationToken m_swapChainPanelKeyUpToken;
		Windows::Foundation::EventRegistrationToken m_buttonClickToken;

		Windows::System::Threading::WorkItemHandler^ m_workItemHandler;
	};
}

