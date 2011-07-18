namespace ZCache.Windows
{
	using System;
	using System.Windows.Controls;

	/// <summary>
	/// Error window that displays exceptions, URI errors, and messages.
	/// </summary>
	public partial class ErrorWindow : ChildWindow
	{
		/// <summary>
		/// Constructor that accepts an exception.
		/// </summary>
		/// <param name="e">Exception.</param>
		public ErrorWindow(Exception e)
		{
			InitializeComponent();
			if ( e != null )
			{
				ErrorTextBox.Text =
					e.Message +
					Environment.NewLine +
					Environment.NewLine +
					e.StackTrace;
			}
		}

		/// <summary>
		/// Constructor that accepts a URI.
		/// </summary>
		/// <param name="uri">URI.</param>
		public ErrorWindow(Uri uri)
		{
			InitializeComponent();
			if ( uri != null )
			{
				ErrorTextBox.Text = "Page not found: \"" + uri.ToString() + "\"";
			}
		}

		/// <summary>
		/// Constructor that accepts a brief message, and details.
		/// </summary>
		/// <param name="message">Message header.</param>
		/// <param name="details">Details (usually stacktrace).</param>
		public ErrorWindow(string message, string details)
		{
			InitializeComponent();

			ErrorTextBox.Text =
				message +
				Environment.NewLine +
				Environment.NewLine +
				details;
		}

	}
}