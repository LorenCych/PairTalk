﻿//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using WinRT.Interop;

namespace AppUIBasics.Helper
{
	// Helper class to allow the app to find the Window that contains an
	// arbitrary UIElement (GetWindowForElement).  To do this, we keep track
	// of all active Windows.  The app code must call WindowHelper.CreateWindow
	// rather than "new Window" so we can keep track of all the relevant
	// windows.  In the future, we would like to support this in platform APIs.
	public class WindowHelper
	{
		static public Window CreateWindow()
		{
			Window newWindow = new Window();
			TrackWindow(newWindow);
			return newWindow;
		}

		static public void TrackWindow(Window window)
		{
			window.Closed += (sender, args) => {
				_activeWindows.Remove(window);
			};
			_activeWindows.Add(window);
		}

		static public AppWindow GetAppWindow(Window window)
		{
			IntPtr hWnd = WindowNative.GetWindowHandle(window);
			WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
			return AppWindow.GetFromWindowId(wndId);
		}

		static public Window GetWindowForElement(UIElement element)
		{
			if (element.XamlRoot != null)
			{
				foreach (Window window in _activeWindows)
				{
					if (element.XamlRoot == window.Content.XamlRoot)
					{
						return window;
					}
				}
			}
			return null;
		}

		static public UIElement FindElementByName(UIElement element, string name)
		{
			if (element.XamlRoot != null && element.XamlRoot.Content != null)
			{
				var ele = (element.XamlRoot.Content as FrameworkElement).FindName(name);
				if (ele != null)
				{
					return ele as UIElement;
				}
			}
			return null;
		}

		public static double GetRasterizationScaleForElement(UIElement element)
		{
			var displayInformation = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();
			return displayInformation.RawPixelsPerViewPixel;
		}

			static public List<Window> ActiveWindows { get { return _activeWindows; } }

		static private List<Window> _activeWindows = new List<Window>();
	}
}