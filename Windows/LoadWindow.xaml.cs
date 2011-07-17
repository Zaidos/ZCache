using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ZCache.Windows
{
	/// <summary>
	/// Cache load pop-up window.
	/// </summary>
	public partial class LoadWindow : ChildWindow
	{
		/// <summary>
		/// Resource names and contents.
		/// </summary>
		private Dictionary<ZCache.Interfaces.IZCacheable, string> resources
			= new Dictionary<Interfaces.IZCacheable, string>();
		/// <summary>
		/// Returns true if currently loading caches.
		/// </summary>
		public bool IsLoading { get; set; }

		/// <summary>
		/// Message that is displayed to user.
		/// </summary>
		private string Message
		{
			get
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();

				foreach ( var resource in resources )
				{
					sb.AppendLine(string.Format(
						"{0} {1}",
						resource.Value,
						resource.Key.ResourceName));
				}
				return sb.ToString();
			}
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public LoadWindow()
		{
			InitializeComponent();
			this.OKButton.Visibility = Visibility.Collapsed;
			this.DataContext = this;
		}


		/// <summary>
		/// Updates resource status and message.
		/// </summary>
		/// <param name="resource">Resource to be udpated.</param>
		/// <param name="message">Resource status message.</param>
		public void UpdateStatus(ZCache.Interfaces.IZCacheable resource,
			string message)
		{
			// Check if resource exists
			if ( this.resources.Keys.Contains(resource) )
				resources[resource] = message;
			else
				resources.Add(resource, message);

			// Update label
			this.labelResource.Content = this.Message;
		}
		/// <summary>
		/// Informs window that all data has been loaded successfuly.
		/// </summary>
		/// <param name="success">Data has loaded successfuly?</param>
		public void CompleteLoad(bool success)
		{
			if ( success )
			{
				labelResults.Content =
					"Data has been loaded successfully. Closing this window";
				System.Windows.Threading.DispatcherTimer timer =
					new System.Windows.Threading.DispatcherTimer();

				// Auto close window after 3 seconds.
				timer.Tick += new EventHandler(timer_Tick);
				timer.Interval = TimeSpan.FromSeconds(3.0);
				timer.Start();
			}
			else
			{
				labelResults.Content =
					"Could not load data at this time. " +
					"Please reload the page to try again.";
				this.OKButton.Visibility = Visibility.Visible;
			}
		}
		/// <summary>
		/// Event handler for dispatch timer (window auto-close).
		/// </summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event arguments.</param>
		private void timer_Tick(object sender, EventArgs e)
		{
			CloseWindow();
		}

		/// <summary>
		/// Clears resources and closes this window.
		/// </summary>
		private void CloseWindow()
		{
			this.resources.Clear();
			this.DialogResult = true;
			this.Close();
		}

		/// <summary>
		/// OKButton Handler. Refreshes HTML page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			System.Windows.Browser.HtmlPage.Document.Submit();
		}
	}
}