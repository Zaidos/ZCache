namespace ZCache.Interfaces
{
	/// <summary>
	/// Resource loaded event arguments.
	/// </summary>
	public class ResourceLoadedEventArgs
	{
		/// <summary>
		/// Returns true if data request was a success.
		/// </summary>
		public bool Success { get; set; }

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="success">Data request status.</param>
		public ResourceLoadedEventArgs(bool success)
		{
			this.Success = success;
		}
	}
}