namespace ZCache
{
	using System;
	using System.Windows;
	using System.Collections.Generic;
	using System.Data.Services.Client;
	using System.ComponentModel;

	/// <summary>
	/// A cached item. Contains the entity and query to pull data.
	/// </summary>
	/// <typeparam name="T">ADO.NET Entity object</typeparam>
	public class CacheItem<T> : ZCache.Interfaces.IZCacheable
		where T : class, INotifyPropertyChanged
	{
		/// <summary>
		/// Listing of cached item.
		/// </summary>
		private List<T> EntityCache { get; set; }
		/// <summary>
		/// Cache name.
		/// </summary>
		private string CacheName { get; set; }

		/// <summary>
		/// Data service query.
		/// </summary>
		private DataServiceQuery<T> CacheQuery { get; set; }

		/// <summary>
		/// Total entities on database. 
		/// </summary>
		private int TotalEntityCount { get; set; }

		/// <summary>
		/// Counts databases entities. 
		/// </summary>
		private BackgroundWorker bwCounter { get; set; }

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="resourceName">Resource name.</param>
		/// <param name="cacheQuery">DataServiceQuery</param>
		public CacheItem(string resourceName, DataServiceQuery<T> cacheQuery)
		{
			this.CacheName = resourceName;
			this.CacheQuery = cacheQuery;
			this.EntityCache = null;

			this.bwCounter = new BackgroundWorker();
			this.bwCounter.WorkerReportsProgress = true;
			this.bwCounter.WorkerSupportsCancellation = true;
			this.bwCounter.DoWork += new DoWorkEventHandler(bwCounter_DoWork);
		}

		void bwCounter_DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker me = sender as BackgroundWorker;

			while ( true )
			{
				if ( me.CancellationPending ) { e.Cancel = true; return; }
				else
				{
					// Does cache support auto refreshing?
					if ( this.AutoRefresh &&
						DateTime.Now.Subtract(LastCache).Minutes >= CacheTimer )
					{
						DataServiceQuery<T> updateQuery =
							this.CacheQuery.AddQueryOption(
							"$skip",
							this.EntityCache.Count);

						// Update the total server count.
						updateQuery.BeginExecute(result =>
						{
							if ( result.IsCompleted )
							{
								try
								{
									foreach ( T item in updateQuery.EndExecute(result) )
									{
										this.EntityCache.Add(item);
									}

									this.LastCache = DateTime.Now;
								}
								catch ( Exception ex ) { new Windows.ErrorWindow(ex).Show(); }
							}
						}, null);
					}
					// Take a nap.
					System.Threading.Thread.Sleep(CacheTimer * 60000);
				}
			}
		}

		/// <summary>
		/// Retreive stored cache.
		/// </summary>
		/// <returns></returns>
		public List<T> GetCache()
		{
			if ( this.EntityCache != null && this.EntityCache.Count > 0 )
				return this.EntityCache;
			else
				return null;
		}

		/// <summary>
		/// Returns true if cache has been loaded.
		/// </summary>
		public bool IsLoaded
		{
			get
			{
				return this.IsCompleted ? (this.EntityCache.Count > 0) : false;
			}
		}

		/// <summary>
		/// Returns true if cache has completed all events.
		/// </summary>
		public bool IsCompleted { get { return (this.EntityCache != null); } }

		/// <summary>
		/// Returns true.
		/// </summary>
		public bool IsCacheable { get { return true; } }

		/// <summary>
		/// Returns true if cache should use isolated storage.
		/// </summary>
		public bool UseIsolatedStorage
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Datetime of last cache update.
		/// </summary>
		public DateTime LastCache { get; set; }

		/// <summary>
		/// Refresh interval of cache (in minutes).
		/// </summary>
		public int CacheTimer { get; set; }

		/// <summary>
		/// Does cache auto-refresh in background.
		/// </summary>
		public bool AutoRefresh { get; set; }

		/// <summary>
		/// Local name for cache resource.
		/// </summary>
		public string ResourceName { get { return this.CacheName; } }

		/// <summary>
		/// Initial cache load.
		/// </summary>
		public void LoadCache()
		{
			try
			{
				if ( this.CacheQuery != null )
				{
					// Bad. Ass Lamb. Da.
					// BALD.
					this.CacheQuery.BeginExecute(result =>
					{
						if ( result.IsCompleted )
						{
							try
							{
								this.EntityCache = new List<T>();

								foreach ( T item in this.CacheQuery.EndExecute(result) )
								{
									this.EntityCache.Add(item);
								}

								bwCounter.RunWorkerAsync();

								this.LastCache = DateTime.Now;
								CheckCompletion();
							}
							catch ( Exception ex ) { new Windows.ErrorWindow(ex).Show(); }
						}
					}, null);
				}
			}
			catch ( Exception ex ) { new Windows.ErrorWindow(ex).Show(); }
		}

		/// <summary>
		/// Checks for completion of cache save.
		/// </summary>
		private void CheckCompletion()
		{
			if ( this.IsCompleted )
			{
				ResourceLoaded(
					this,
					new ZCache.Interfaces.ResourceLoadedEventArgs(this.IsLoaded));
			}
		}

		/// <summary>
		/// Resource loaded event.
		/// </summary>
		public event Interfaces.ResourceLoad ResourceLoaded;
	}
}