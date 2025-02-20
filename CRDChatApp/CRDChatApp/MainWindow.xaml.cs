using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Input;
using AppUIBasics.Helper;
using Windows.Graphics;
using System.Runtime.InteropServices;
using WinRT.Interop;




// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CRDChatApp
{
	/// <summary>
	/// An empty window that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainWindow : Window
	{
		private string currentUser = "Local"; // Default to local user
		private bool autoScroll = true; // Track if auto-scroll is enabled
		private AppWindowTitleBar titleBar;

		public MainWindow()
		{
			this.InitializeComponent();

			// Extend content into title bar
			ExtendsContentIntoTitleBar = true;
			SetTitleBar(AppTitleBar);

			// Set up the custom title bar
			InitializeCustomTitleBar();

			WindowId windowId = Win32Interop.GetWindowIdFromWindow(WinRT.Interop.WindowNative.GetWindowHandle(this));
			AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
			appWindow.SetPresenter(AppWindowPresenterKind.Default);
			// Set minimum size for the window
			appWindow.Resize(new Windows.Graphics.SizeInt32(450, 600)); // Example minimum size: 400x300 pixels

			// Set the title text
			TitleBarTextBlock.Text = this.Title;

			DisableMaximizeButton();
		}

		private const int GWL_STYLE = -16;
		private const int WS_MAXIMIZEBOX = 0x00010000;
		private const int WS_CAPTION = 0x00C00000;

		[DllImport("user32.dll", SetLastError = true)]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		private void DisableMaximizeButton()
		{
			IntPtr hWnd = WindowNative.GetWindowHandle(this);
			int windowStyle = GetWindowLong(hWnd, GWL_STYLE);
			windowStyle &= ~WS_MAXIMIZEBOX; // Disable maximize button
			SetWindowLong(hWnd, GWL_STYLE, windowStyle);
		}


		private void InitializeCustomTitleBar()
		{
			var window = this;
			titleBar = window.AppWindow.TitleBar;
			titleBar.ExtendsContentIntoTitleBar = true;
			titleBar.ButtonBackgroundColor = Microsoft.UI.Colors.Transparent;
			titleBar.ButtonInactiveBackgroundColor = Microsoft.UI.Colors.Transparent;
		}


		private void PinToggleButton_Checked(object sender, RoutedEventArgs e)
		{
			// Change the icon to the pinned icon
			FontIcon pinIcon = (FontIcon)PinToggleButton.Content;
			pinIcon.Glyph = "\uE77A";
	

			// Pin the window
			WindowId windowId = Win32Interop.GetWindowIdFromWindow(WinRT.Interop.WindowNative.GetWindowHandle(this));
			AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
			appWindow.SetPresenter(AppWindowPresenterKind.CompactOverlay);

			// Set minimum size for the window
			appWindow.Resize(new Windows.Graphics.SizeInt32(400, 700)); // Example minimum size: 400x300 pixels
		}

		private void PinToggleButton_Unchecked(object sender, RoutedEventArgs e)
		{
			// Change the icon to the unpinned icon
			FontIcon pinIcon = (FontIcon)PinToggleButton.Content;
			pinIcon.Glyph = "\uE718";

			// Unpin the window
			WindowId windowId = Win32Interop.GetWindowIdFromWindow(WinRT.Interop.WindowNative.GetWindowHandle(this));
			AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
			appWindow.SetPresenter(AppWindowPresenterKind.Default);
		}


		// Event handler for sending messages
		private void SendMessage(object sender, RoutedEventArgs e)
		{
			string message = MessageTextBox.Text;
			if (!string.IsNullOrEmpty(message))
			{
				// Create a StackPanel to wrap the PersonPicture and the message
				StackPanel messagePanel = new StackPanel
				{
					Orientation = Orientation.Horizontal,
					Margin = new Thickness(5)
				};

				// Create the PersonPicture for the avatar based on current user
				PersonPicture avatar = new PersonPicture();
				if (currentUser == "Local")
				{
					avatar.Style = (Style)Application.Current.Resources["ActiveLocalUserProfileStyle"];
					avatar.DisplayName = "Kim Younghoon"; // Optionally, set the display name
					avatar.Margin = new Thickness(5);
				}
				else
				{
					avatar.Style = (Style)Application.Current.Resources["ActiveRemoteUserProfileStyle"];
					avatar.DisplayName = "Lee Juyeon"; // Optionally, set the display name
					avatar.Margin = new Thickness(5);
				}
				avatar.Width = 32;
				avatar.Height = 32;


				// Create a Border for the background
				Border messageBorder = new Border
				{
					// Apply BorderBrush from resources (ensure it exists in your resources)
					BorderBrush = (Brush)Application.Current.Resources["SurfaceStrokeColorDefaultBrush"], // Optional border color
					BorderThickness = new Microsoft.UI.Xaml.Thickness(1), // Optional border thickness
					CornerRadius = new Microsoft.UI.Xaml.CornerRadius(10), // Optional rounded corners
					Padding = new Microsoft.UI.Xaml.Thickness(10) // Optional padding inside the border
				};

				// Create the TextBlock with the message
				TextBlock messageText = new TextBlock
				{
					Text = message,
					TextWrapping = TextWrapping.Wrap, // Ensures text will wrap within the available width
					MaxWidth = 295 // Optional: Set a max width to prevent text from becoming too wide
				};

				// Set background and text color based on the user
				if (currentUser == "Local")
				{
					messageBorder.BorderBrush = (Brush)Application.Current.Resources["AccentFillColorDefaultBrush"];
					messageBorder.Background = (Brush)Application.Current.Resources["AccentAcrylicBackgroundFillColorDefaultBrush"]; // Local User background
					messageText.Foreground = new SolidColorBrush(Microsoft.UI.Colors.White); // Optional text color for Local User
					messagePanel.HorizontalAlignment = HorizontalAlignment.Right; // Align to the right for Local User
					messagePanel.Children.Add(messageBorder); // Insert message before avatar
					messagePanel.Children.Add(avatar); // Insert avatar at the end
				}
				else if (currentUser == "Remote")
				{
					messageBorder.Background = (Brush)Application.Current.Resources["CardBackgroundFillColorDefaultBrush"]; // Remote User background
					messageText.Foreground = new SolidColorBrush(Microsoft.UI.Colors.White); // Optional text color for Remote User
					messagePanel.HorizontalAlignment = HorizontalAlignment.Left; // Align to the left for Remote User
					messagePanel.Children.Add(avatar); // Add avatar at the start
					messagePanel.Children.Add(messageBorder); // Add message after avatar
				}

				// Set the TextBlock as the child of the Border
				messageBorder.Child = messageText;

				// Add the StackPanel to the main panel
				MessagePanel.Children.Add(messagePanel);

				// Ensure the ScrollViewer always scrolls to the bottom when a new message is added
				ChatScrollViewer.UpdateLayout();
				ChatScrollViewer.ChangeView(null, ChatScrollViewer.ScrollableHeight, null);

				// Clear the message textbox
				MessageTextBox.Text = string.Empty;

				ElementSoundPlayer.State = ElementSoundPlayerState.On;
				ElementSoundPlayer.Play(ElementSoundKind.Invoke);
			}
		}

		private void MessageTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
		{

			if (e.Key == Windows.System.VirtualKey.Enter)
			{
				SendMessage(sender, e);
			}
		
		}

		private void UserSwitchButton_Click(object sender, RoutedEventArgs e)
		{
			if (UserSwitchButton.IsChecked == true)
			{
				// Local user
				currentUser = "Local";
				MessageTextBox.PlaceholderText = "You are speaking as Local User";

				// Apply ActiveLocalUserProfileStyle to LocalUserProfile
				LocalUserProfile.Style = (Style)Application.Current.Resources["ActiveLocalUserProfileStyle"];

				// Apply InactiveRemoteUserProfileStyle to RemoteUserProfile
				RemoteUserProfile.Style = (Style)Application.Current.Resources["InactiveRemoteUserProfileStyle"];

				RemoteUserProfile.Width = 32;
				RemoteUserProfile.Height = 32;
				LocalUserProfile.Width = 45;
				LocalUserProfile.Height = 45;
			}
			else
			{
				// Remote user
				currentUser = "Remote";
				MessageTextBox.PlaceholderText = "You are speaking as Remote User";

				// Apply InactiveLocalUserProfileStyle to LocalUserProfile
				LocalUserProfile.Style = (Style)Application.Current.Resources["InactiveLocalUserProfileStyle"];

				// Apply ActiveRemoteUserProfileStyle to RemoteUserProfile
				RemoteUserProfile.Style = (Style)Application.Current.Resources["ActiveRemoteUserProfileStyle"];
				RemoteUserProfile.Width = 45;
				RemoteUserProfile.Height = 45;
				LocalUserProfile.Width = 32;
				LocalUserProfile.Height = 32;
			}
		}

	}
}
