// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.GlobalEndpointManager
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Routing
{
  internal class GlobalEndpointManager : IDisposable
  {
    private const int DefaultBackgroundRefreshLocationTimeIntervalInMS = 300000;
    private const string BackgroundRefreshLocationTimeIntervalInMS = "BackgroundRefreshLocationTimeIntervalInMS";
    private int backgroundRefreshLocationTimeIntervalInMS = 300000;
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private readonly LocationCache locationCache;
    private readonly Uri defaultEndpoint;
    private readonly ConnectionPolicy connectionPolicy;
    private readonly IDocumentClientInternal owner;
    private readonly object refreshLock;
    private readonly AsyncCache<string, DatabaseAccount> databaseAccountCache;
    private bool isRefreshing;

    public GlobalEndpointManager(IDocumentClientInternal owner, ConnectionPolicy connectionPolicy)
    {
      this.locationCache = new LocationCache(new ReadOnlyCollection<string>((IList<string>) connectionPolicy.PreferredLocations), owner.ServiceEndpoint, connectionPolicy.EnableEndpointDiscovery, connectionPolicy.MaxConnectionLimit, connectionPolicy.UseMultipleWriteLocations);
      this.owner = owner;
      this.defaultEndpoint = owner.ServiceEndpoint;
      this.connectionPolicy = connectionPolicy;
      this.databaseAccountCache = new AsyncCache<string, DatabaseAccount>();
      this.connectionPolicy.PreferenceChanged += new NotifyCollectionChangedEventHandler(this.OnPreferenceChanged);
      this.isRefreshing = false;
      this.refreshLock = new object();
    }

    public ReadOnlyCollection<Uri> ReadEndpoints => this.locationCache.ReadEndpoints;

    public ReadOnlyCollection<Uri> WriteEndpoints => this.locationCache.WriteEndpoints;

    public static async Task<DatabaseAccount> GetDatabaseAccountFromAnyLocationsAsync(
      Uri defaultEndpoint,
      IList<string> locations,
      Func<Uri, Task<DatabaseAccount>> getDatabaseAccountFn)
    {
      ExceptionDispatchInfo capturedException = (ExceptionDispatchInfo) null;
      try
      {
        return await getDatabaseAccountFn(defaultEndpoint);
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceInformation("Fail to reach global gateway {0}, {1}", (object) defaultEndpoint, (object) ex.ToString());
        if (GlobalEndpointManager.IsNonRetriableException(ex))
        {
          DefaultTrace.TraceInformation("Exception is not retriable");
          throw;
        }
        else if (locations.Count == 0)
          throw;
        else
          capturedException = ExceptionDispatchInfo.Capture(ex);
      }
      for (int index = 0; index < locations.Count; ++index)
      {
        try
        {
          return await getDatabaseAccountFn(LocationHelper.GetLocationEndpoint(defaultEndpoint, locations[index]));
        }
        catch (Exception ex)
        {
          DefaultTrace.TraceInformation("Fail to reach location {0}, {1}", (object) locations[index], (object) ex.ToString());
          if (index == locations.Count - 1)
          {
            capturedException?.Throw();
            throw;
          }
        }
      }
      throw new Exception();
    }

    public virtual Uri ResolveServiceEndpoint(DocumentServiceRequest request) => this.locationCache.ResolveServiceEndpoint(request);

    public string GetLocation(Uri endpoint) => this.locationCache.GetLocation(endpoint);

    public void MarkEndpointUnavailableForRead(Uri endpoint)
    {
      DefaultTrace.TraceInformation("Marking endpoint {0} unavailable for read", (object) endpoint);
      this.locationCache.MarkEndpointUnavailableForRead(endpoint);
    }

    public void MarkEndpointUnavailableForWrite(Uri endpoint)
    {
      DefaultTrace.TraceInformation("Marking endpoint {0} unavailable for Write", (object) endpoint);
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

    public async Task RefreshLocationAsync(DatabaseAccount databaseAccount, bool forceRefresh = false)
    {
      if (this.cancellationTokenSource.IsCancellationRequested)
        return;
      if (forceRefresh)
      {
        this.locationCache.OnDatabaseAccountRead(await this.RefreshDatabaseAccountInternalAsync());
      }
      else
      {
        lock (this.refreshLock)
        {
          if (this.isRefreshing)
            return;
          this.isRefreshing = true;
        }
        try
        {
          await this.RefreshLocationPrivateAsync(databaseAccount);
        }
        catch
        {
          this.isRefreshing = false;
          throw;
        }
      }
    }

    private async Task RefreshLocationPrivateAsync(DatabaseAccount databaseAccount)
    {
      if (this.cancellationTokenSource.IsCancellationRequested)
        return;
      DefaultTrace.TraceInformation("RefreshLocationAsync() refreshing locations");
      if (databaseAccount != null)
        this.locationCache.OnDatabaseAccountRead(databaseAccount);
      bool canRefreshInBackground = false;
      if (this.locationCache.ShouldRefreshEndpoints(out canRefreshInBackground))
      {
        if (databaseAccount == null && !canRefreshInBackground)
        {
          databaseAccount = await this.RefreshDatabaseAccountInternalAsync();
          this.locationCache.OnDatabaseAccountRead(databaseAccount);
        }
        this.StartRefreshLocationTimerAsync();
      }
      else
        this.isRefreshing = false;
    }

    [SuppressMessage("", "AsyncFixer03", Justification = "Async start is by-design")]
    private async void StartRefreshLocationTimerAsync()
    {
      if (this.cancellationTokenSource.IsCancellationRequested)
        return;
      try
      {
        await Task.Delay(this.backgroundRefreshLocationTimeIntervalInMS, this.cancellationTokenSource.Token);
        DefaultTrace.TraceInformation("StartRefreshLocationTimerAsync() - Invoking refresh");
        await this.RefreshLocationPrivateAsync(await this.RefreshDatabaseAccountInternalAsync());
      }
      catch (Exception ex)
      {
        if (this.cancellationTokenSource.IsCancellationRequested)
        {
          switch (ex)
          {
            case TaskCanceledException _:
              return;
            case ObjectDisposedException _:
              return;
          }
        }
        DefaultTrace.TraceCritical("StartRefreshLocationTimerAsync() - Unable to refresh database account from any location. Exception: {0}", (object) ex.ToString());
        this.StartRefreshLocationTimerAsync();
      }
    }

    private Task<DatabaseAccount> GetDatabaseAccountAsync(Uri serviceEndpoint) => this.owner.GetDatabaseAccountInternalAsync(serviceEndpoint, this.cancellationTokenSource.Token);

    private void OnPreferenceChanged(object sender, NotifyCollectionChangedEventArgs e) => this.locationCache.OnLocationPreferenceChanged(new ReadOnlyCollection<string>((IList<string>) this.connectionPolicy.PreferredLocations));

    private Task<DatabaseAccount> RefreshDatabaseAccountInternalAsync() => this.databaseAccountCache.GetAsync(string.Empty, (DatabaseAccount) null, (Func<Task<DatabaseAccount>>) (() => GlobalEndpointManager.GetDatabaseAccountFromAnyLocationsAsync(this.defaultEndpoint, (IList<string>) this.connectionPolicy.PreferredLocations, new Func<Uri, Task<DatabaseAccount>>(this.GetDatabaseAccountAsync))), this.cancellationTokenSource.Token, true);

    private static bool IsNonRetriableException(Exception exception)
    {
      if (exception is DocumentClientException documentClientException)
      {
        HttpStatusCode? statusCode = documentClientException.StatusCode;
        HttpStatusCode httpStatusCode = HttpStatusCode.Unauthorized;
        if (statusCode.GetValueOrDefault() == httpStatusCode & statusCode.HasValue)
          return true;
      }
      return false;
    }
  }
}
