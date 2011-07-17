namespace ZCache
{
	using System.Collections.Generic;
	using System;
	using System.Windows;
	using System.Linq;
	using System.ComponentModel;

	/// <summary>
	/// Container for all cacheable items.
	/// </summary>
	public class Caches
	{
		#region Public Properties
		/// <summary>
		/// Dictionary of cached entities.
		/// </summary
		public Dictionary<string, ZCache.Interfaces.IZCacheable> CacheList { get; set; }
		#endregion

		#region Private Properties
		/// <summary>
		/// Load window. 
		/// Reports cache status.
		/// </summary>
		private ZCache.Windows.LoadWindow loadWindow;
		#endregion

		#region Status Checks
		/// <summary>
		/// Returns true if all caches have been loaded.
		/// </summary>
		public bool IsLoaded { get { return this.CacheList.All(f => f.Value.IsLoaded); } }

		/// <summary>
		/// Returns true if all caches have been completed.
		/// </summary>
		public bool IsComplete { get { return this.CacheList.All(f => f.Value.IsCompleted); } }
		#endregion

		#region Constructors
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Caches()
		{
			this.CacheList = new Dictionary<string, Interfaces.IZCacheable>();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Adds an IZCacheable object to cacheable list.
		/// </summary>
		/// <param name="cacheableObject"></param>
		public void AddToCache(Interfaces.IZCacheable cacheableObject)
		{
			try
			{
				// Reduntant. I know...
				if ( cacheableObject.IsCacheable )
				{
					this.CacheList.Add(
						cacheableObject.ResourceName,
						cacheableObject);
				}
			}
			catch ( Exception ex ) { new Windows.ErrorWindow(ex).Show(); }
		}

		public ZCache.Interfaces.IZCacheable GetCache(string cacheName)
		{
			// This might be slow. Improve with a foreach or something.
			if ( this.CacheList.Keys.Contains(cacheName) )
				return this.CacheList[cacheName];
			else
				return null;
		}

		/// <summary>
		/// Attempts to load caches in cacheable list.
		/// </summary>
		public void LoadCaches()
		{
			if ( !this.IsLoaded )
			{
				if ( this.CacheList.Count > 0 )
				{
					this.loadWindow = new Windows.LoadWindow();
					this.loadWindow.IsLoading = true;
					this.loadWindow.labelResults.Content = string.Empty;
					this.loadWindow.Show();

					// Open pop-up window
					foreach ( var resource in this.CacheList.Values )
					{
						// B.A.L.D.
						resource.ResourceLoaded += (sender, e) =>
						{
							this.loadWindow.UpdateStatus(
								(ZCache.Interfaces.IZCacheable)sender,
								e.Success ? "Loaded" : "Failed to load");

							if ( this.IsComplete )
							{
								this.loadWindow.IsLoading = false;
								this.loadWindow.CompleteLoad(this.IsComplete);
							}
						};

						this.loadWindow.UpdateStatus(resource, "Loading");

						resource.LoadCache();
					}
				}
				else
				{
					new Windows.ErrorWindow(
						"Cache list is empty.",
						"Please add entities to the cache list.").Show();
				}
			}
		}
		#endregion

		#region Private Methods

		#endregion
	}
}