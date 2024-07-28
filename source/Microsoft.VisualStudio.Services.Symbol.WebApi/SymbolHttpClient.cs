// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Symbol.WebApi.SymbolHttpClient
// Assembly: Microsoft.VisualStudio.Services.Symbol.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52CDDA61-EF2D-4AD8-A25B-09A8F04FE8C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Symbol.WebApi.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Symbol.WebApi
{
  [ResourceArea("AF607F94-69BA-4821-8159-F04E37B66350")]
  public class SymbolHttpClient : ArtifactHttpClient, ISymbolHttpClient, IArtifactHttpClient
  {
    private static readonly ApiResourceVersion CurrentApiVersion;
    private static Dictionary<string, Type> translatedExceptions;
    private static Lazy<HttpClient> basicHttpClient = new Lazy<HttpClient>();

    static SymbolHttpClient()
    {
      SymbolHttpClient.translatedExceptions = new Dictionary<string, Type>();
      SymbolHttpClient.CurrentApiVersion = new ApiResourceVersion("2.0-preview");
    }

    public SymbolHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public SymbolHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public SymbolHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public SymbolHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public SymbolHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) SymbolHttpClient.translatedExceptions;

    protected override ServicePointExtensions.ServicePointConfig GetServicePointSettings()
    {
      ServicePointExtensions.ServicePointConfig servicePointSettings = base.GetServicePointSettings();
      servicePointSettings.TcpKeepAlive.Value.KeepAliveTime = TimeSpan.FromSeconds(60.0);
      return servicePointSettings;
    }

    public override Guid ResourceId => SymbolResourceIds.SymSrvLocation;

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken) => (await this.SendAsync(HttpMethod.Get, SymbolResourceIds.AvailabilityLocation, version: SymbolHttpClient.CurrentApiVersion, cancellationToken: cancellationToken)).StatusCode == HttpStatusCode.OK;

    public async Task<DebugEntry> CreateDebugEntryAsync(
      string requestId,
      DebugEntry entry,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<string>(requestId, nameof (requestId));
      ArgumentUtility.CheckForNull<DebugEntry>(entry, nameof (entry));
      DebugEntryCreateBatch batch = new DebugEntryCreateBatch((IEnumerable<DebugEntry>) new List<DebugEntry>()
      {
        entry
      }, DebugEntryCreateBehavior.ThrowIfExists);
      List<DebugEntry> source = await this.CreateDebugEntriesAsync(requestId, batch, cancellationToken).ConfigureAwait(false);
      return source.Any<DebugEntry>() ? source[0] : throw new SymbolException("Service returned a null entry when posting entry" + Environment.NewLine + entry.ToJson() + Environment.NewLine + "to request " + requestId);
    }

    public Task<List<DebugEntry>> CreateDebugEntriesAsync(
      string requestId,
      DebugEntryCreateBatch batch,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<string>(requestId, nameof (requestId));
      ArgumentUtility.CheckForNull<DebugEntryCreateBatch>(batch, nameof (batch));
      ArgumentUtility.CheckEnumerableForEmpty((IEnumerable) batch.DebugEntries, "DebugEntries");
      return this.PostAsync<DebugEntryCreateBatch, List<DebugEntry>>(batch, SymbolResourceIds.RequestsDebugEntriesLocation, (object) new
      {
        resource = "requests",
        requestId = Uri.EscapeDataString(requestId),
        collection = "debugentries"
      }, SymbolHttpClient.CurrentApiVersion, cancellationToken: cancellationToken);
    }

    public Task<Request> CreateRequestAsync(Request request, CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<Request>(request, nameof (request));
      return this.PostAsync<Request, Request>(request, SymbolResourceIds.RequestsV2Location, (object) new
      {
        resource = "requests"
      }, SymbolHttpClient.CurrentApiVersion, cancellationToken: cancellationToken);
    }

    public Task<HttpResponseMessage> DeleteRequestAsync(
      string requestId,
      CancellationToken cancellationToken,
      bool synchronous = false)
    {
      ArgumentUtility.CheckForNull<string>(requestId, nameof (requestId));
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary.Add(nameof (synchronous), synchronous.ToString());
      Guid requestsV2Location = SymbolResourceIds.RequestsV2Location;
      var routeValues = new
      {
        resource = "requests",
        requestId = Uri.EscapeDataString(requestId)
      };
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) dictionary;
      ApiResourceVersion currentApiVersion = SymbolHttpClient.CurrentApiVersion;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      return this.DeleteAsync(requestsV2Location, (object) routeValues, currentApiVersion, queryParameters, cancellationToken: cancellationToken1);
    }

    public async Task<int> DeleteRequestsByNameAsync(
      string[] requestNames,
      bool synchronous,
      CancellationToken cancellationToken,
      object userState = null)
    {
      SymbolHttpClient symbolHttpClient1 = this;
      ArgumentUtility.CheckForNull<string[]>(requestNames, nameof (requestNames));
      int count = 0;
      List<VssServiceException> exceptionList = new List<VssServiceException>();
      string[] strArray = requestNames;
      for (int index = 0; index < strArray.Length; ++index)
      {
        string str = strArray[index];
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        dictionary.Add("requestName", str);
        dictionary.Add(nameof (synchronous), synchronous.ToString());
        try
        {
          SymbolHttpClient symbolHttpClient2 = symbolHttpClient1;
          Guid requestsV2Location = SymbolResourceIds.RequestsV2Location;
          var routeValues = new{ resource = "requests" };
          IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) dictionary;
          ApiResourceVersion currentApiVersion = SymbolHttpClient.CurrentApiVersion;
          IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
          object userState1 = userState;
          CancellationToken cancellationToken1 = cancellationToken;
          HttpResponseMessage httpResponseMessage = await symbolHttpClient2.DeleteAsync(requestsV2Location, (object) routeValues, currentApiVersion, queryParameters, userState1, cancellationToken1).ConfigureAwait(false);
        }
        catch (VssServiceResponseException ex)
        {
          if (ex.HttpStatusCode != HttpStatusCode.NotFound)
            exceptionList.Add((VssServiceException) ex);
          else
            continue;
        }
        ++count;
      }
      strArray = (string[]) null;
      if (exceptionList.Count > 0)
        throw new AggregateException("Deletion of one or more symbol requests failed, First error: " + exceptionList.First<VssServiceException>().Message, (IEnumerable<Exception>) exceptionList);
      int num = count;
      exceptionList = (List<VssServiceException>) null;
      return num;
    }

    public Task<Request> FinalizeRequestAsync(
      string requestId,
      DateTime? expirationDate,
      bool isUpdateOperation,
      CancellationToken cancellationToken)
    {
      Request request1 = new Request();
      request1.Id = requestId;
      request1.Status = isUpdateOperation ? RequestStatus.None : RequestStatus.Sealed;
      request1.ExpirationDate = expirationDate;
      Request request2 = request1;
      ArgumentUtility.CheckForNull<string>(requestId, nameof (requestId));
      return this.PatchAsync<Request, Request>(request2, SymbolResourceIds.RequestsV2Location, (object) new
      {
        resource = "requests",
        requestId = Uri.EscapeDataString(requestId)
      }, SymbolHttpClient.CurrentApiVersion, cancellationToken: cancellationToken);
    }

    public Task<List<Request>> GetAllRequestsAsync(
      CancellationToken cancellationToken,
      SizeOptions sizeOptions = null,
      ExpirationDateOptions expirationDateOptions = null,
      IDomainId domainIdOption = null,
      RetrievalOptions retrievalOptions = RetrievalOptions.ExcludeSoftDeleted,
      RequestStatus? requestStatus = null)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      if (sizeOptions != null)
        dictionary.Add(nameof (sizeOptions), sizeOptions.ToString());
      if (expirationDateOptions != null)
        dictionary.Add(nameof (expirationDateOptions), expirationDateOptions.ToString());
      if (domainIdOption != (IDomainId) null)
        dictionary.Add(nameof (domainIdOption), domainIdOption.Serialize());
      dictionary.Add(nameof (retrievalOptions), retrievalOptions.ToString());
      if (requestStatus.HasValue)
        dictionary.Add(nameof (requestStatus), requestStatus.Value.ToString());
      Guid requestsV2Location = SymbolResourceIds.RequestsV2Location;
      var routeValues = new{ resource = "requests" };
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) dictionary;
      ApiResourceVersion currentApiVersion = SymbolHttpClient.CurrentApiVersion;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      CancellationToken cancellationToken1 = cancellationToken;
      return this.GetAsync<List<Request>>(requestsV2Location, (object) routeValues, currentApiVersion, queryParameters, cancellationToken: cancellationToken1);
    }

    public Task<List<DebugEntry>> GetDebugEntriesAsync(
      string path,
      int? startEntry,
      int? maxEntries,
      DebugEntrySortOrder sortOrder,
      CancellationToken cancellationToken)
    {
      Dictionary<string, string> queryParameters = new Dictionary<string, string>();
      if (startEntry.HasValue)
        queryParameters.Add(nameof (startEntry), startEntry.ToString());
      if (maxEntries.HasValue)
        queryParameters.Add(nameof (maxEntries), maxEntries.ToString());
      if (sortOrder != DebugEntrySortOrder.None)
        queryParameters.Add(nameof (sortOrder), sortOrder.ToString());
      return this.GetAsync<List<DebugEntry>>(SymbolResourceIds.DebugEntriesLocation, (object) new
      {
        resource = "debugentries",
        debugEntryClientKey = (path == null ? path : Uri.EscapeDataString(path))
      }, SymbolHttpClient.CurrentApiVersion, (IEnumerable<KeyValuePair<string, string>>) queryParameters, cancellationToken: cancellationToken);
    }

    public Task<HttpResponseMessage> GetOptionsAsync(
      Guid location,
      CancellationToken cancellationToken)
    {
      return this.SendAsync(HttpMethod.Options, location, (object) cancellationToken);
    }

    public Task<Request> GetRequestAsync(string requestId, CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<string>(requestId, nameof (requestId));
      return this.GetAsync<Request>(SymbolResourceIds.RequestsV2Location, (object) new
      {
        resource = "requests",
        requestId = Uri.EscapeDataString(requestId)
      }, SymbolHttpClient.CurrentApiVersion, cancellationToken: cancellationToken);
    }

    public Task<Request> GetRequestByNameAsync(
      string requestName,
      CancellationToken cancellationToken)
    {
      Dictionary<string, string> queryParameters = new Dictionary<string, string>();
      ArgumentUtility.CheckForNull<string>(requestName, nameof (requestName));
      queryParameters.Add(nameof (requestName), requestName);
      return this.GetAsync<Request>(SymbolResourceIds.RequestsV2Location, (object) new
      {
        resource = "requests"
      }, SymbolHttpClient.CurrentApiVersion, (IEnumerable<KeyValuePair<string, string>>) queryParameters, cancellationToken: cancellationToken);
    }

    public Task<List<Request>> GetRequestPaginatedAsync(
      string continueFromRequestId,
      int pageSize,
      CancellationToken cancellationToken,
      SizeOptions sizeOptions = null,
      ExpirationDateOptions expirationDateOptions = null,
      IDomainId domainIdOption = null,
      RetrievalOptions retrievalOptions = RetrievalOptions.ExcludeSoftDeleted,
      RequestStatus? requestStatus = null)
    {
      ArgumentUtility.CheckBoundsInclusive(pageSize, 1, 10000, nameof (pageSize));
      Dictionary<string, string> queryParameters = new Dictionary<string, string>()
      {
        {
          nameof (continueFromRequestId),
          continueFromRequestId ?? string.Empty
        },
        {
          nameof (pageSize),
          pageSize.ToString()
        }
      };
      if (sizeOptions != null)
        queryParameters.Add(nameof (sizeOptions), sizeOptions.ToString());
      if (expirationDateOptions != null)
        queryParameters.Add(nameof (expirationDateOptions), expirationDateOptions.ToString());
      if (domainIdOption != (IDomainId) null)
        queryParameters.Add(nameof (domainIdOption), domainIdOption.Serialize());
      queryParameters.Add(nameof (retrievalOptions), retrievalOptions.ToString());
      if (requestStatus.HasValue)
        queryParameters.Add(nameof (requestStatus), requestStatus.Value.ToString());
      return this.GetAsync<List<Request>>(SymbolResourceIds.RequestsV2Location, (object) new
      {
        resource = "requests"
      }, SymbolHttpClient.CurrentApiVersion, (IEnumerable<KeyValuePair<string, string>>) queryParameters, cancellationToken: cancellationToken);
    }

    public Task<List<DebugEntry>> GetRequestDebugEntriesAsync(
      string requestId,
      string debugEntryId,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<string>(requestId, nameof (requestId));
      return this.GetAsync<List<DebugEntry>>(SymbolResourceIds.RequestsDebugEntriesLocation, (object) new
      {
        resource = "requests",
        requestId = Uri.EscapeDataString(requestId),
        collection = "debugentries",
        debugEntryId = (debugEntryId == null ? debugEntryId : Uri.EscapeDataString(debugEntryId))
      }, SymbolHttpClient.CurrentApiVersion, cancellationToken: cancellationToken);
    }

    public Task<HttpResponseMessage> GetSymSrvItemAsync(
      string debugEntryClientKey,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<string>(debugEntryClientKey, nameof (debugEntryClientKey));
      return this.GetAsync(SymbolResourceIds.SymSrvLocation, (object) new
      {
        resource = "symsrv",
        debugEntryClientKey = Uri.EscapeDataString(debugEntryClientKey)
      }, SymbolHttpClient.CurrentApiVersion, cancellationToken: cancellationToken);
    }

    public async Task<HttpResponseMessage> GetSymSrvDebugEntryContentAsync(
      DebugEntry debugEntry,
      CancellationToken cancellationToken)
    {
      SymbolHttpClient symbolHttpClient = this;
      if (debugEntry.BlobIdentifier.GetBlobDedupLevel() == BlobDedupLevel.FileLevel)
        return await SymbolHttpClient.basicHttpClient.Value.GetAsync(debugEntry.BlobUri).ConfigureAwait(false);
      // ISSUE: explicit non-virtual call
      HttpResponseMessage entryContentAsync = await __nonvirtual (symbolHttpClient.GetSymSrvItemAsync(debugEntry.ClientKey, cancellationToken)).ConfigureAwait(false);
      if (entryContentAsync.StatusCode == HttpStatusCode.Found)
      {
        Uri redirectUri = entryContentAsync.Headers.Location;
        ConfiguredTaskAwaitable<HttpResponseMessage> configuredTaskAwaitable = symbolHttpClient.Client.GetAsync(redirectUri).ConfigureAwait(false);
        entryContentAsync = await configuredTaskAwaitable;
        if (entryContentAsync.StatusCode >= HttpStatusCode.BadRequest)
        {
          configuredTaskAwaitable = SymbolHttpClient.basicHttpClient.Value.GetAsync(redirectUri).ConfigureAwait(false);
          entryContentAsync = await configuredTaskAwaitable;
        }
        redirectUri = (Uri) null;
      }
      return entryContentAsync;
    }

    public Task<Request> UpdateRequestAsync(
      string requestId,
      DateTime expirationDate,
      CancellationToken cancellationToken)
    {
      Request request1 = new Request();
      request1.Id = requestId;
      request1.ExpirationDate = new DateTime?(expirationDate);
      Request request2 = request1;
      ArgumentUtility.CheckForNull<string>(requestId, nameof (requestId));
      return this.PatchAsync<Request, Request>(request2, SymbolResourceIds.RequestsV2Location, (object) new
      {
        resource = "requests",
        requestId = Uri.EscapeDataString(requestId)
      }, SymbolHttpClient.CurrentApiVersion, cancellationToken: cancellationToken);
    }
  }
}
