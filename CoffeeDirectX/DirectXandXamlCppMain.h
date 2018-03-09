#pragma once

#include <DirectXMath.h>
#include "Common\StepTimer.h"
#include "Common\DeviceResources.h"
#include "Content\Sample3DSceneRenderer.h"
#include "Content\SampleFpsTextRenderer.h"

#define MANUAL_ROTATION_SPEED 45
//XMGLOBALCONST DirectX::XMVECTORF32 Coffee = { 58.0/255.0, 31.0/255.0, 8.0/255.0, 1.000000000f };
XMGLOBALCONST DirectX::XMVECTORF32 Coffee = { 0.2274509803921569f, 0.1215686274509804f, 0.0313725490196078f, 1.000000000f };

// Renders Direct2D and 3D content on the screen.
namespace CoffeeDirectX
{
	class DirectXandXamlCppMain : public DX::IDeviceNotify
	{
	public:
		DirectXandXamlCppMain(const std::shared_ptr<DX::DeviceResources>& deviceResources);
		~DirectXandXamlCppMain();
		void CreateWindowSizeDependentResources();
		void StartTracking() { m_sceneRenderer->StartTracking(); }
		void TrackingUpdate(float positionX) { m_pointerLocationX = positionX; }
		void StopTracking() { m_sceneRenderer->StopTracking(); }
		bool IsTracking() { return m_sceneRenderer->IsTracking(); }
		void StartRenderLoop();
		void StopRenderLoop();
		Concurrency::critical_section& GetCriticalSection() { return m_criticalSection; }
		float GetCurrentPosition() { return m_pointerLocationX; }

		// IDeviceNotify
		virtual void OnDeviceLost();
		virtual void OnDeviceRestored();

	private:
		void ProcessInput();
		void Update();
		bool Render();

		// Cached pointer to device resources.
		std::shared_ptr<DX::DeviceResources> m_deviceResources;
		std::unique_ptr<Sample3DSceneRenderer> m_sceneRenderer;
		std::unique_ptr<SampleFpsTextRenderer> m_fpsTextRenderer;
		Windows::Foundation::IAsyncAction^ m_renderLoopWorker;
		Concurrency::critical_section m_criticalSection;

		// Rendering loop timer.
		DX::StepTimer m_timer;

		// Track current input pointer position.
		float m_pointerLocationX;

		Windows::System::Threading::WorkItemHandler^ m_workItemHandler;
	};
}