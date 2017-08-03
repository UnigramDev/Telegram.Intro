#pragma once

#include "OpenGLES.h"
#include "SimpleRenderer.h"

using namespace Windows::UI::Xaml::Controls;

namespace Telegram
{
	namespace Intro
	{
		public ref class TLIntroRenderer sealed
		{
		public:
			TLIntroRenderer(SwapChainPanel^ swapChainPanel, bool dark);
			virtual ~TLIntroRenderer();

			void Loaded();

			property float CurrentScroll
			{
				float get() { return mCurrentScroll; }
				void set(float value)
				{
					mCurrentScroll = value;
				}
			}

			property int CurrentPage
			{
				int get() { return mCurrentPage; }
				void set(int value)
				{
					mCurrentPage = value;
				}
			}

			property bool IsDarkTheme
			{
				bool get() { return mDarkTheme; }
				void set(bool value)
				{
					mDarkTheme = value;
				}
			}

		internal:
			TLIntroRenderer(OpenGLES* openGLES, SwapChainPanel^ swapChainPanel, int dark);

		private:
			void OnVisibilityChanged(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::VisibilityChangedEventArgs^ args);
			void CreateRenderSurface();
			void DestroyRenderSurface();
			void RecoverFromLostDevice();
			void StartRenderLoop();
			void StopRenderLoop();

			float mCurrentScale;

			float mCurrentScroll;
			int mCurrentPage;
			int mDarkTheme;

			OpenGLES* mOpenGLES;
			OpenGLES mOpenGLESHolder;

			SwapChainPanel^ mSwapChainPanel;

			EGLSurface mRenderSurface;     // This surface is associated with a swapChainPanel on the page
			Concurrency::critical_section mRenderSurfaceCriticalSection;
			Windows::Foundation::IAsyncAction^ mRenderLoopWorker;
		};
	}
}
