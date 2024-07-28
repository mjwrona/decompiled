// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Routing.GlobalEndpointManager
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Common;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.Azure.Cosmos.Routing
{
  internal class GlobalEndpointManager : IGlobalEndpointManager, IDisposable
  {
    private const int DefaultBackgroundRefreshLocationTimeIntervalInMS = 300000;
    private const string BackgroundRefreshLocationTimeIntervalInMS = "BackgroundRefreshLocationTimeIntervalInMS";
    private const string MinimumIntervalForNonForceRefreshLocationInMS = "MinimumIntervalForNonForceRefreshLocationInMS";
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private readonly LocationCache locationCache;
    private readonly Uri defaultEndpoint;
    private readonly ConnectionPolicy connectionPolicy;
    private readonly IDocumentClientInternal owner;
    private readonly AsyncCache<string, AccountProperties> databaseAccountCache = new AsyncCache<string, AccountProperties>();
    private readonly TimeSpan MinTimeBetweenAccountRefresh = TimeSpan.FromSeconds(15.0);
    private readonly int backgroundRefreshLocationTimeIntervalInMS = 300000;
    private readonly object backgroundAccountRefreshLock = new object();
    private readonly object isAccountRefreshInProgressLock = new object();
    private bool isAccountRefreshInProgress;
    private bool isBackgroundAccountRefreshActive;
    private DateTime LastBackgroundRefreshUtc = DateTime.MinValue;

    public GlobalEndpointManager(IDocumentClientInternal owner, ConnectionPolicy connectionPolicy)
    {
      this.locationCache = new LocationCache(new ReadOnlyCollection<string>((IList<string>) connectionPolicy.PreferredLocations), owner.ServiceEndpoint, connectionPolicy.EnableEndpointDiscovery, connectionPolicy.MaxConnectionLimit, connectionPolicy.UseMultipleWriteLocations);
      this.owner = owner;
      this.defaultEndpoint = owner.ServiceEndpoint;
      this.connectionPolicy = connectionPolicy;
      this.connectionPolicy.PreferenceChanged += new NotifyCollectionChangedEventHandler(this.OnPreferenceChanged);
      if (Assembly.GetEntryAssembly() != (Assembly) null)
      {
        string appSetting = ConfigurationManager.AppSettings[nameof (BackgroundRefreshLocationTimeIntervalInMS)];
        if (!string.IsNullOrEmpty(appSetting) && !int.TryParse(appSetting, out this.backgroundRefreshLocationTimeIntervalInMS))
          this.backgroundRefreshLocationTimeIntervalInMS = 300000;
      }
      string environmentVariable = Environment.GetEnvironmentVariable(nameof (MinimumIntervalForNonForceRefreshLocationInMS));
      if (string.IsNullOrEmpty(environmentVariable))
        return;
      int result;
      if (int.TryParse(environmentVariable, out result))
        this.MinTimeBetweenAccountRefresh = TimeSpan.FromMilliseconds((double) result);
      else
        DefaultTrace.TraceError("GlobalEndpointManager: Failed to parse MinimumIntervalForNonForceRefreshLocationInMS; Value:" + environmentVariable);
    }

    public ReadOnlyCollection<Uri> ReadEndpoints => this.locationCache.ReadEndpoints;

    public ReadOnlyCollection<Uri> WriteEndpoints => this.locationCache.WriteEndpoints;

    public int PreferredLocationCount => this.connectionPolicy.PreferredLocations == null ? 0 : this.connectionPolicy.PreferredLocations.Count;

    public static Task<AccountProperties> GetDatabaseAccountFromAnyLocationsAsync(
      Uri defaultEndpoint,
      IList<string>? locations,
      Func<Uri, Task<AccountProperties>> getDatabaseAccountFn,
      CancellationToken cancellationToken)
    {
      return new GlobalEndpointManager.GetAccountPropertiesHelper(defaultEndpoint, locations?.GetEnumerator(), getDatabaseAccountFn, cancellationToken).GetAccountPropertiesAsync();
    }

    public virtual Uri ResolveServiceEndpoint(DocumentServiceRequest request) => this.locationCache.ResolveServiceEndpoint(request);

    public string GetLocation(Uri endpoint) => this.locationCache.GetLocation(endpoint);

    public bool TryGetLocationForGatewayDiagnostics(Uri endpoint, out string regionName) => this.locationCache.TryGetLocationForGatewayDiagnostics(endpoint, out regionName);

    public virtual void MarkEndpointUnavailableForRead(Uri endpoint)
    {
      DefaultTrace.TraceInformation("GlobalEndpointManager: Marking endpoint {0} unavailable for read", (object) endpoint);
      this.locationCache.MarkEndpointUnavailableForRead(endpoint);
    }

    public virtual void MarkEndpointUnavailableForWrite(Uri endpoint)
    {
      DefaultTrace.TraceInformation("GlobalEndpointManager: Marking endpoint {0} unavailable for Write", (object) endpoint);
      this.locationCache.MarkEndpointUnavailableForWrite(endpoint);
    }

    public bool CanUseMultipleWriteLocations(DocumentServiceRequest request) => this.locationCache.CanUseMultipleWriteLocations(request);

    public void Dispose()
    {
      this.connectionPolicy.PreferenceChanged -= new NotifyCollectionChangedEventHandler(this.OnPreferenceChanged);
      if (this.cancellationTokenSource.IsCancellationRequested)
        return;
      this.cancellationTokenSource.Cancel();
      this.cancellationTokenSource.Dispose();
    }

    public virtual void InitializeAccountPropertiesAndStartBackgroundRefresh(
      AccountProperties databaseAccount)
    {
      if (this.cancellationTokenSource.IsCancellationRequested)
        return;
      this.locationCache.OnDatabaseAccountRead(databaseAccount);
      if (this.isBackgroundAccountRefreshActive)
        return;
      lock (this.backgroundAccountRefreshLock)
      {
        if (this.isBackgroundAccountRefreshActive)
          return;
        this.isBackgroundAccountRefreshActive = true;
      }
      try
      {
        this.StartLocationBackgroundRefreshLoop();
      }
      catch
      {
        this.isBackgroundAccountRefreshActive = false;
        throw;
      }
    }

    public virtual async Task RefreshLocationAsync(bool forceRefresh = false)
    {
      if (this.cancellationTokenSource.IsCancellationRequested)
        return;
      await this.RefreshDatabaseAccountInternalAsync(forceRefresh);
    }

    private async void StartLocationBackgroundRefreshLoop()
    {
      if (this.cancellationTokenSource.IsCancellationRequested)
        return;
      DefaultTrace.TraceInformation("GlobalEndpointManager: StartLocationBackgroundRefreshWithTimer() refreshing locations");
      bool canRefreshInBackground;
      if (!this.locationCache.ShouldRefreshEndpoints(out canRefreshInBackground) && !canRefreshInBackground)
      {
        DefaultTrace.TraceInformation("GlobalEndpointManager: StartLocationBackgroundRefreshWithTimer() stropped.");
        lock (this.backgroundAccountRefreshLock)
          this.isBackgroundAccountRefreshActive = false;
      }
      else
      {
        try
        {
          await Task.Delay(this.backgroundRefreshLocationTimeIntervalInMS, this.cancellationTokenSource.Token);
          DefaultTrace.TraceInformation("GlobalEndpointManager: StartLocationBackgroundRefreshWithTimer() - Invoking refresh");
          if (this.cancellationTokenSource.IsCancellationRequested)
            return;
          await this.RefreshDatabaseAccountInternalAsync(false);
        }
        catch (Exception ex)
        {
          if (this.cancellationTokenSource.IsCancellationRequested)
          {
            switch (ex)
            {
              case OperationCanceledException _:
                return;
              case ObjectDisposedException _:
                return;
            }
          }
          DefaultTrace.TraceCritical("GlobalEndpointManager: StartLocationBackgroundRefreshWithTimer() - Unable to refresh database account from any location. Exception: {0}", (object) ex.ToString());
        }
        this.StartLocationBackgroundRefreshLoop();
      }
    }

    private Task<AccountProperties> GetDatabaseAccountAsync(Uri serviceEndpoint) => this.owner.GetDatabaseAccountInternalAsync(serviceEndpoint, this.cancellationTokenSource.Token);

    private void OnPreferenceChanged(object sender, NotifyCollectionChangedEventArgs e) => this.locationCache.OnLocationPreferenceChanged(new ReadOnlyCollection<string>((IList<string>) this.connectionPolicy.PreferredLocations));

    private async Task RefreshDatabaseAccountInternalAsync(bool forceRefresh)
    {
      if (this.cancellationTokenSource.IsCancellationRequested || this.SkipRefresh(forceRefresh))
        return;
      lock (this.isAccountRefreshInProgressLock)
      {
        if (this.SkipRefresh(forceRefresh) || this.isAccountRefreshInProgress)
          return;
        this.isAccountRefreshInProgress = true;
      }
      try
      {
        this.LastBackgroundRefreshUtc = DateTime.UtcNow;
        LocationCache locationCache = this.locationCache;
        locationCache.OnDatabaseAccountRead(await this.GetDatabaseAccountAsync(true));
        locationCache = (LocationCache) null;
      }
      finally
      {
        lock (this.isAccountRefreshInProgressLock)
          this.isAccountRefreshInProgress = false;
      }
    }

    internal async Task<AccountProperties> GetDatabaseAccountAsync(bool forceRefresh = false)
    {
      GlobalEndpointManager globalEndpointManager = this;
      // ISSUE: reference to a compiler-generated method
      return await globalEndpointManager.databaseAccountCache.GetAsync(string.Empty, (AccountProperties) null, new Func<Task<AccountProperties>>(globalEndpointManager.\u003CGetDatabaseAccountAsync\u003Eb__38_0), globalEndpointManager.cancellationTokenSource.Token, forceRefresh);
    }

    private bool SkipRefresh(bool forceRefresh)
    {
      TimeSpan timeSpan = DateTime.UtcNow - this.LastBackgroundRefreshUtc;
      return (this.isAccountRefreshInProgress || this.MinTimeBetweenAccountRefresh > timeSpan) && !forceRefresh;
    }

    private class GetAccountPropertiesHelper
    {
      private readonly CancellationTokenSource CancellationTokenSource;
      private readonly Uri DefaultEndpoint;
      private readonly IEnumerator<string>? Locations;
      private readonly Func<Uri, Task<AccountProperties>> GetDatabaseAccountFn;
      private readonly List<Exception> TransientExceptions = new List<Exception>();
      private AccountProperties? AccountProperties;
      private Exception? NonRetriableException;

      public GetAccountPropertiesHelper(
        Uri defaultEndpoint,
        IEnumerator<string>? locations,
        Func<Uri, Task<AccountProperties>> getDatabaseAccountFn,
        CancellationToken cancellationToken)
      {
        this.DefaultEndpoint = defaultEndpoint;
        this.Locations = locations;
        this.GetDatabaseAccountFn = getDatabaseAccountFn;
        this.CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
      }

      public async Task<AccountProperties> GetAccountPropertiesAsync()
      {
        if (this.Locations == null)
          return await this.GetOnlyGlobalEndpointAsync();
        Task globalEndpointTask = this.GetAndUpdateAccountPropertiesAsync(this.DefaultEndpoint);
        Task task1 = Task.Delay(TimeSpan.FromSeconds(5.0));
        Task task2 = await Task.WhenAny(globalEndpointTask, task1);
        if (this.AccountProperties != null)
          return this.AccountProperties;
        if (this.NonRetriableException != null)
          ExceptionDispatchInfo.Capture(this.NonRetriableException).Throw();
        HashSet<Task> tasksToWaitOn = new HashSet<Task>()
        {
          globalEndpointTask,
          this.TryGetAccountPropertiesFromAllLocationsAsync(),
          this.TryGetAccountPropertiesFromAllLocationsAsync()
        };
        while (tasksToWaitOn.Any<Task>())
        {
          Task task3 = await Task.WhenAny((IEnumerable<Task>) tasksToWaitOn);
          if (this.AccountProperties != null)
            return this.AccountProperties;
          if (this.NonRetriableException != null)
            ExceptionDispatchInfo.Capture(this.NonRetriableException).Throw();
          tasksToWaitOn.Remove(task3);
        }
        if (this.TransientExceptions.Count == 0)
          throw new ArgumentException("Account properties and NonRetriableException are null and there are no TransientExceptions.");
        if (this.TransientExceptions.Count == 1)
          ExceptionDispatchInfo.Capture(this.TransientExceptions[0]).Throw();
        throw new AggregateException((IEnumerable<Exception>) this.TransientExceptions);
      }

      private async Task<AccountProperties> GetOnlyGlobalEndpointAsync()
      {
        if (this.Locations != null)
          throw new ArgumentException("GetOnlyGlobalEndpointAsync should only be called if there are no other regions");
        await this.GetAndUpdateAccountPropertiesAsync(this.DefaultEndpoint);
        if (this.AccountProperties != null)
          return this.AccountProperties;
        if (this.NonRetriableException != null)
          throw this.NonRetriableException;
        if (this.TransientExceptions.Count == 0)
          throw new ArgumentException("Account properties and NonRetriableException are null and there are no TransientExceptions.");
        if (this.TransientExceptions.Count == 1)
          throw this.TransientExceptions[0];
        throw new AggregateException((IEnumerable<Exception>) this.TransientExceptions);
      }

      private async Task TryGetAccountPropertiesFromAllLocationsAsync()
      {
        string location;
        while (this.TryMoveNextLocationThreadSafe(out location))
        {
          if (location == null)
          {
            DefaultTrace.TraceCritical("GlobalEndpointManager: location is null for TryMoveNextLocationThreadSafe");
            break;
          }
          await this.TryGetAccountPropertiesFromRegionalEndpointsAsync(location);
        }
      }

      private bool TryMoveNextLocationThreadSafe(out string? location)
      {
        if (this.CancellationTokenSource.IsCancellationRequested || this.Locations == null)
        {
          location = (string) null;
          return false;
        }
        lock (this.Locations)
        {
          if (!this.Locations.MoveNext())
          {
            location = (string) null;
            return false;
          }
          location = this.Locations.Current;
          return true;
        }
      }

      private Task TryGetAccountPropertiesFromRegionalEndpointsAsync(string location) => this.GetAndUpdateAccountPropertiesAsync(LocationHelper.GetLocationEndpoint(this.DefaultEndpoint, location));

      private async Task GetAndUpdateAccountPropertiesAsync(Uri endpoint)
      {
        try
        {
          if (this.CancellationTokenSource.IsCancellationRequested)
          {
            lock (this.TransientExceptions)
              this.TransientExceptions.Add((Exception) new OperationCanceledException("GlobalEndpointManager: Get account information canceled"));
          }
          else
          {
            AccountProperties accountProperties = await this.GetDatabaseAccountFn(endpoint);
            if (accountProperties == null)
              return;
            this.AccountProperties = accountProperties;
            this.CancellationTokenSource.Cancel();
          }
        }
        catch (Exception ex)
        {
          DefaultTrace.TraceInformation("GlobalEndpointManager: Fail to reach gateway endpoint {0}, {1}", (object) endpoint, (object) ex.ToString());
          if (GlobalEndpointManager.GetAccountPropertiesHelper.IsNonRetriableException(ex))
          {
            DefaultTrace.TraceInformation("GlobalEndpointManager: Exception is not retriable");
            this.CancellationTokenSource.Cancel();
            this.NonRetriableException = ex;
          }
          else
          {
            lock (this.TransientExceptions)
              this.TransientExceptions.Add(ex);
          }
        }
      }

      private static bool IsNonRetriableException(Exception exception)
      {
        if (exception is DocumentClientException documentClientException)
        {
          HttpStatusCode? statusCode = documentClientException.StatusCode;
          HttpStatusCode httpStatusCode1 = HttpStatusCode.Unauthorized;
          if (!(statusCode.GetValueOrDefault() == httpStatusCode1 & statusCode.HasValue))
          {
            statusCode = documentClientException.StatusCode;
            HttpStatusCode httpStatusCode2 = HttpStatusCode.Forbidden;
            if (!(statusCode.GetValueOrDefault() == httpStatusCode2 & statusCode.HasValue))
              goto label_4;
          }
          return true;
        }
label_4:
        return false;
      }
    }
  }
}
