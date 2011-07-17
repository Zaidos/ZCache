namespace ZCache.Interfaces
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Resource has loaded delegate.
	/// </summary>
	/// <param name="sender">Delegate sender.</param>
	/// <param name="e">Delegate properties.</param>
	public delegate void ResourceLoad(object sender, ResourceLoadedEventArgs e);

	/// <summary>
	/// Cacheable interface.
	/// </summary>
	public interface IZCacheable
	{
		/// <summary>
		/// Local name given to cache resource.
		/// </summary>
		string ResourceName { get; }

		#region Status Properties
		/// <summary>
		/// Returns true if this object has been cached locally.
		/// </summary>
		bool IsLoaded { get; }

		/// <summary>
		/// Returns true if this object has been completed.
		/// </summary>
		bool IsCompleted { get; }

		/// <summary>
		/// Returns true if this object is cacheable.
		/// Should always return true.
		/// </summary>
		bool IsCacheable { get; }
		#endregion

		#region Optional Properties

		/// <summary>
		/// Should this cache be stored in isolated storage.
		/// </summary>
		bool UseIsolatedStorage { get; set; }
		#endregion

		#region Timer Properties
		/// <summary>
		/// Time of the last cache update.
		/// </summary>
		DateTime LastCache { get; set; }

		/// <summary>
		/// Cache update timer in minutes.
		/// </summary>
		int CacheTimer { get; set; }

		bool AutoRefresh { get; set; }
		#endregion

		#region Methods
		/// <summary>
		/// Initial cache load.
		/// </summary>
		void LoadCache();

		/// <summary>
		/// Updates the cache.
		/// </summary>
		void RefreshCache();
		#endregion

		#region Events
		/// <summary>
		/// Called when the resource has finished loading.
		/// Determines whether the load was a success.
		/// </summary>
		event ResourceLoad ResourceLoaded;
		#endregion
	}
}