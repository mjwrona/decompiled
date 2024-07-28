// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Client.LicensingHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Licensing.Client
{
  [ResourceArea("C73A23A1-59BB-458C-8CE3-02C83215E015")]
  public class LicensingHttpClient : LicensingCompatHttpClientBase, IAccountLicensingHttpClient
  {
    private static readonly Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>()
    {
      {
        "InvalidRightNameException",
        typeof (InvalidRightNameException)
      },
      {
        "InvalidClientVersionException",
        typeof (InvalidClientVersionException)
      },
      {
        "InvalidClientRightsQueryContextException",
        typeof (InvalidClientRightsQueryContextException)
      },
      {
        "InvalidLicensingOperation",
        typeof (InvalidLicensingOperation)
      }
    };
    protected static readonly Version previewApiVersion = new Version(1, 0);
    private static readonly Version searchMemberspreviewApiVersion = new Version(7, 1);

    public LicensingHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public LicensingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public LicensingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public LicensingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public LicensingHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task<IEnumerable<IServiceRight>> GetServiceRightsAsync(
      string rightName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LicensingHttpClient licensingHttpClient1 = this;
      IEnumerable<IServiceRight> serviceRightsAsync;
      using (new VssHttpClientBase.OperationScope("Licensing", "GetServiceRights"))
      {
        if (rightName != null)
          ArgumentUtility.CheckStringForInvalidCharacters(rightName, nameof (rightName));
        LicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid rightsLocationid = LicensingResourceIds.ServiceRightsLocationid;
        var routeValues = new{ rightName = rightName };
        ApiResourceVersion version = new ApiResourceVersion(LicensingHttpClient.previewApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        IEnumerable<ServiceRight> source = await licensingHttpClient2.SendAsync<IEnumerable<ServiceRight>>(get, rightsLocationid, (object) routeValues, version, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
        serviceRightsAsync = source != null ? source.Select<ServiceRight, IServiceRight>((Func<ServiceRight, IServiceRight>) (right => (IServiceRight) right)) : Enumerable.Empty<IServiceRight>();
      }
      return serviceRightsAsync;
    }

    public virtual async Task<ClientRightsContainer> GetClientRightsContainerAsync(
      ClientRightsQueryContext queryContext,
      ClientRightsTelemetryContext telemetryContext = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LicensingHttpClient licensingHttpClient1 = this;
      ClientRightsContainer rightsContainerAsync;
      using (new VssHttpClientBase.OperationScope("Licensing", "GetClientRightsContainer"))
      {
        LicensingCompatHttpClientBase.ValidateClientRightsQueryContext(queryContext);
        LicensingCompatHttpClientBase.ValidateClientRightsTelemetryContext(telemetryContext);
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add("productVersion", queryContext.ProductVersion);
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        if (queryContext.ProductEdition != null)
          keyValuePairList.Add("edition", queryContext.ProductEdition);
        if (queryContext.ReleaseType != null)
          keyValuePairList.Add("relType", queryContext.ReleaseType);
        if (queryContext.IncludeCertificate)
          keyValuePairList.Add("includeCertificate", "true");
        if (queryContext.Canary != null)
          keyValuePairList.Add("canary", queryContext.Canary);
        if (queryContext.MachineId != null)
          keyValuePairList.Add("machineId", queryContext.MachineId);
        LicensingCompatHttpClientBase.SerializeTelemetryContextAsOptionalQueryParameters(telemetryContext, (IList<KeyValuePair<string, string>>) keyValuePairList);
        LicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid rightsLocationid = LicensingResourceIds.ClientRightsLocationid;
        var routeValues = new
        {
          rightName = queryContext.ProductFamily
        };
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
        CancellationToken cancellationToken1 = cancellationToken;
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion(LicensingHttpClient.previewApiVersion, 1);
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = obj;
        CancellationToken cancellationToken2 = cancellationToken1;
        rightsContainerAsync = await licensingHttpClient2.SendAsync<ClientRightsContainer>(get, rightsLocationid, (object) routeValues, version, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return rightsContainerAsync;
    }

    public virtual async Task<IEnumerable<MsdnEntitlement>> GetMsdnEntitlementsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LicensingHttpClient licensingHttpClient1 = this;
      IEnumerable<MsdnEntitlement> entitlementsAsync;
      using (new VssHttpClientBase.OperationScope("Licensing", "GetMsdnEntitlements"))
      {
        LicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid entitlementsLocationId = LicensingResourceIds.MsdnEntitlementsLocationId;
        ApiResourceVersion version = new ApiResourceVersion(LicensingHttpClient.previewApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        entitlementsAsync = await licensingHttpClient2.SendAsync<IEnumerable<MsdnEntitlement>>(get, entitlementsLocationId, version: version, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return entitlementsAsync;
    }

    public virtual async Task<PagedAccountEntitlements> SearchMemberAccountEntitlementsAsync(
      string continuation = null,
      string filter = null,
      string orderBy = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LicensingHttpClient licensingHttpClient1 = this;
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (continuation != null)
        collection.Add(nameof (continuation), continuation);
      if (filter != null)
        collection.Add("$filter", filter);
      if (orderBy != null)
        collection.Add("$orderBy", orderBy);
      PagedAccountEntitlements accountEntitlements;
      using (new VssHttpClientBase.OperationScope("Licensing", "SearchAccountEntitlements"))
      {
        LicensingHttpClient licensingHttpClient2 = licensingHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid searchLocationId = LicensingResourceIds.EntitlementsSearchLocationId;
        ApiResourceVersion apiResourceVersion = new ApiResourceVersion(LicensingHttpClient.searchMemberspreviewApiVersion, 2);
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        object obj = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        var routeValues = new{ action = "Search" };
        ApiResourceVersion version = apiResourceVersion;
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = obj;
        CancellationToken cancellationToken2 = cancellationToken1;
        accountEntitlements = await licensingHttpClient2.SendAsync<PagedAccountEntitlements>(get, searchLocationId, (object) routeValues, version, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return accountEntitlements;
    }

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) LicensingHttpClient.s_translatedExceptions;
  }
}
