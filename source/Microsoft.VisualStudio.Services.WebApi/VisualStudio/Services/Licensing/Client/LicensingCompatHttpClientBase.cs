// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Client.LicensingCompatHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Contracts.Licensing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Licensing.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class LicensingCompatHttpClientBase : VssHttpClientBase
  {
    private static readonly Version previewApiVersion = new Version(1, 0);

    protected LicensingCompatHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    protected LicensingCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    protected LicensingCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    protected LicensingCompatHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    protected LicensingCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<IEnumerable<IUsageRight>> GetUsageRightsAsync(
      string rightName,
      object userState)
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      IEnumerable<IUsageRight> usageRightsAsync;
      using (new VssHttpClientBase.OperationScope("Licensing", "GetUsageRights"))
      {
        if (rightName != null)
          ArgumentUtility.CheckStringForInvalidCharacters(rightName, nameof (rightName));
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod get = HttpMethod.Get;
        Guid rightsLocationid = LicensingResourceIds.UsageRightsLocationid;
        var routeValues = new{ rightName = rightName };
        ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        CancellationToken cancellationToken1 = new CancellationToken();
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        IEnumerable<UsageRight> source = await compatHttpClientBase2.SendAsync<IEnumerable<UsageRight>>(get, rightsLocationid, (object) routeValues, version, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
        usageRightsAsync = source != null ? source.Select<UsageRight, IUsageRight>((Func<UsageRight, IUsageRight>) (right => (IUsageRight) right)) : (IEnumerable<IUsageRight>) null;
      }
      return usageRightsAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<IEnumerable<IUsageRight>> GetUsageRightsAsync(
      string rightName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      IEnumerable<IUsageRight> usageRightsAsync;
      using (new VssHttpClientBase.OperationScope("Licensing", "GetUsageRights"))
      {
        if (rightName != null)
          ArgumentUtility.CheckStringForInvalidCharacters(rightName, nameof (rightName));
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod get = HttpMethod.Get;
        Guid rightsLocationid = LicensingResourceIds.UsageRightsLocationid;
        var routeValues = new{ rightName = rightName };
        ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        IEnumerable<UsageRight> source = await compatHttpClientBase2.SendAsync<IEnumerable<UsageRight>>(get, rightsLocationid, (object) routeValues, version, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
        usageRightsAsync = source != null ? source.Select<UsageRight, IUsageRight>((Func<UsageRight, IUsageRight>) (right => (IUsageRight) right)) : (IEnumerable<IUsageRight>) null;
      }
      return usageRightsAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<IEnumerable<IServiceRight>> GetServiceRightsAsync(
      string rightName,
      object userState)
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      IEnumerable<IServiceRight> serviceRightsAsync;
      using (new VssHttpClientBase.OperationScope("Licensing", "GetServiceRights"))
      {
        if (rightName != null)
          ArgumentUtility.CheckStringForInvalidCharacters(rightName, nameof (rightName));
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod get = HttpMethod.Get;
        Guid rightsLocationid = LicensingResourceIds.ServiceRightsLocationid;
        var routeValues = new{ rightName = rightName };
        ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        CancellationToken cancellationToken1 = new CancellationToken();
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        IEnumerable<ServiceRight> source = await compatHttpClientBase2.SendAsync<IEnumerable<ServiceRight>>(get, rightsLocationid, (object) routeValues, version, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
        serviceRightsAsync = source != null ? source.Select<ServiceRight, IServiceRight>((Func<ServiceRight, IServiceRight>) (right => (IServiceRight) right)) : Enumerable.Empty<IServiceRight>();
      }
      return serviceRightsAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<byte[]> GetCertificateAsync(object userState)
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      byte[] certificateAsync;
      using (new VssHttpClientBase.OperationScope("Licensing", "GetCertificate"))
      {
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod get = HttpMethod.Get;
        Guid certificateLocationid = LicensingResourceIds.CertificateLocationid;
        ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        CancellationToken cancellationToken1 = new CancellationToken();
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        certificateAsync = await (await compatHttpClientBase2.SendAsync(get, certificateLocationid, version: version, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false)).Content.ReadAsByteArrayAsync().ConfigureAwait(false);
      }
      return certificateAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<byte[]> GetCertificateAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      byte[] certificateAsync;
      using (new VssHttpClientBase.OperationScope("Licensing", "GetCertificate"))
      {
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod get = HttpMethod.Get;
        Guid certificateLocationid = LicensingResourceIds.CertificateLocationid;
        ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        certificateAsync = await (await compatHttpClientBase2.SendAsync(get, certificateLocationid, version: version, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false)).Content.ReadAsByteArrayAsync().ConfigureAwait(false);
      }
      return certificateAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<ClientRightsContainer> GetClientRightsContainerAsync(
      ClientRightsQueryContext queryContext,
      ClientRightsTelemetryContext telemetryContext,
      object userState)
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
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
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod get = HttpMethod.Get;
        Guid rightsLocationid = LicensingResourceIds.ClientRightsLocationid;
        var routeValues = new
        {
          rightName = queryContext.ProductFamily
        };
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
        CancellationToken cancellationToken1 = new CancellationToken();
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = obj;
        CancellationToken cancellationToken2 = cancellationToken1;
        rightsContainerAsync = await compatHttpClientBase2.SendAsync<ClientRightsContainer>(get, rightsLocationid, (object) routeValues, version, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return rightsContainerAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<IEnumerable<AccountLicenseUsage>> GetAccountLicensesUsageAsync(
      object userState)
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      IEnumerable<AccountLicenseUsage> licensesUsageAsync;
      using (new VssHttpClientBase.OperationScope("Licensing", "GetAccountLicensesUsage"))
      {
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod get = HttpMethod.Get;
        Guid usageLocationid = LicensingResourceIds.UsageLocationid;
        ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        CancellationToken cancellationToken1 = new CancellationToken();
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        licensesUsageAsync = await compatHttpClientBase2.SendAsync<IEnumerable<AccountLicenseUsage>>(get, usageLocationid, version: version, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false) ?? Enumerable.Empty<AccountLicenseUsage>();
      }
      return licensesUsageAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<IEnumerable<AccountEntitlement>> GetAccountEntitlementsAsync(
      object userState)
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      IEnumerable<AccountEntitlement> entitlementsAsync;
      using (new VssHttpClientBase.OperationScope("Licensing", "GetAccountEntitlements"))
      {
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod get = HttpMethod.Get;
        Guid entitlementsLocationid = LicensingResourceIds.EntitlementsLocationid;
        ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        CancellationToken cancellationToken1 = new CancellationToken();
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        entitlementsAsync = await compatHttpClientBase2.SendAsync<IEnumerable<AccountEntitlement>>(get, entitlementsLocationid, version: version, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return entitlementsAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<AccountEntitlement> GetAccountEntitlementAsync(object userState)
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      AccountEntitlement entitlementAsync;
      using (new VssHttpClientBase.OperationScope("Licensing", "GetAccountEntitlement"))
      {
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod get = HttpMethod.Get;
        Guid entitlementsLocationId = LicensingResourceIds.CurrentUserEntitlementsLocationId;
        ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        CancellationToken cancellationToken1 = new CancellationToken();
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        entitlementAsync = await compatHttpClientBase2.SendAsync<AccountEntitlement>(get, entitlementsLocationId, version: version, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return entitlementAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<AccountEntitlement> AssignEntitlementAsync(
      Guid userId,
      Microsoft.VisualStudio.Services.Licensing.License license,
      object userState)
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      AccountEntitlement accountEntitlement;
      using (new VssHttpClientBase.OperationScope("Licensing", "AssignEntitlement"))
      {
        ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
        ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Licensing.License>(license, nameof (license));
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add<bool>("dontNotifyUser", false);
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod put = HttpMethod.Put;
        Guid entitlementsLocationId = LicensingResourceIds.UserEntitlementsLocationId;
        var routeValues = new{ userId = userId };
        HttpContent contentFor = (HttpContent) compatHttpClientBase1.CreateContentFor<AccountEntitlementUpdateModel>(new AccountEntitlementUpdateModel()
        {
          License = license
        });
        ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        HttpContent content = contentFor;
        CancellationToken cancellationToken1 = new CancellationToken();
        object obj = userState;
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
        object userState1 = obj;
        CancellationToken cancellationToken2 = cancellationToken1;
        accountEntitlement = await compatHttpClientBase2.SendAsync<AccountEntitlement>(put, entitlementsLocationId, (object) routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState1, cancellationToken2).ConfigureAwait(false);
      }
      return accountEntitlement;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<AccountEntitlement> AssignEntitlementAsync(
      Guid userId,
      Microsoft.VisualStudio.Services.Licensing.License license,
      object userState,
      CancellationToken cancellationToken)
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      AccountEntitlement accountEntitlement;
      using (new VssHttpClientBase.OperationScope("Licensing", "AssignEntitlement"))
      {
        ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
        ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Licensing.License>(license, nameof (license));
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add<bool>("dontNotifyUser", false);
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod put = HttpMethod.Put;
        Guid entitlementsLocationId = LicensingResourceIds.UserEntitlementsLocationId;
        var routeValues = new{ userId = userId };
        HttpContent contentFor = (HttpContent) compatHttpClientBase1.CreateContentFor<AccountEntitlementUpdateModel>(new AccountEntitlementUpdateModel()
        {
          License = license
        });
        ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        HttpContent content = contentFor;
        CancellationToken cancellationToken1 = cancellationToken;
        object obj = userState;
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
        object userState1 = obj;
        CancellationToken cancellationToken2 = cancellationToken1;
        accountEntitlement = await compatHttpClientBase2.SendAsync<AccountEntitlement>(put, entitlementsLocationId, (object) routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState1, cancellationToken2).ConfigureAwait(false);
      }
      return accountEntitlement;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<AccountEntitlement> AssignAvailableEntitlementAsync(
      Guid userId,
      object userState)
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      AccountEntitlement accountEntitlement;
      using (new VssHttpClientBase.OperationScope("Licensing", "AssignEntitlement"))
      {
        ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add<bool>("dontNotifyUser", false);
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod put = HttpMethod.Put;
        Guid entitlementsLocationId = LicensingResourceIds.UserEntitlementsLocationId;
        var routeValues = new{ userId = userId };
        HttpContent contentFor = (HttpContent) compatHttpClientBase1.CreateContentFor<AccountEntitlementUpdateModel>(new AccountEntitlementUpdateModel()
        {
          License = Microsoft.VisualStudio.Services.Licensing.License.Auto
        });
        ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        HttpContent content = contentFor;
        CancellationToken cancellationToken1 = new CancellationToken();
        object obj = userState;
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
        object userState1 = obj;
        CancellationToken cancellationToken2 = cancellationToken1;
        accountEntitlement = await compatHttpClientBase2.SendAsync<AccountEntitlement>(put, entitlementsLocationId, (object) routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState1, cancellationToken2).ConfigureAwait(false);
      }
      return accountEntitlement;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<AccountEntitlement> AssignEntitlementAsync(
      Guid userId,
      Microsoft.VisualStudio.Services.Licensing.License license,
      bool dontNotifyUser,
      object userState,
      CancellationToken cancellationToken)
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      AccountEntitlement accountEntitlement;
      using (new VssHttpClientBase.OperationScope("Licensing", "AssignEntitlement"))
      {
        ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
        ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Licensing.License>(license, nameof (license));
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add<bool>(nameof (dontNotifyUser), dontNotifyUser);
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod put = HttpMethod.Put;
        Guid entitlementsLocationId = LicensingResourceIds.UserEntitlementsLocationId;
        var routeValues = new{ userId = userId };
        HttpContent contentFor = (HttpContent) compatHttpClientBase1.CreateContentFor<AccountEntitlementUpdateModel>(new AccountEntitlementUpdateModel()
        {
          License = license
        });
        ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        HttpContent content = contentFor;
        CancellationToken cancellationToken1 = cancellationToken;
        object obj = userState;
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
        object userState1 = obj;
        CancellationToken cancellationToken2 = cancellationToken1;
        accountEntitlement = await compatHttpClientBase2.SendAsync<AccountEntitlement>(put, entitlementsLocationId, (object) routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState1, cancellationToken2).ConfigureAwait(false);
      }
      return accountEntitlement;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<AccountEntitlement> AssignAvailableEntitlementAsync(
      Guid userId,
      object userState,
      CancellationToken cancellationToken)
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      AccountEntitlement accountEntitlement;
      using (new VssHttpClientBase.OperationScope("Licensing", "AssignEntitlement"))
      {
        ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add<bool>("dontNotifyUser", false);
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod put = HttpMethod.Put;
        Guid entitlementsLocationId = LicensingResourceIds.UserEntitlementsLocationId;
        var routeValues = new{ userId = userId };
        HttpContent contentFor = (HttpContent) compatHttpClientBase1.CreateContentFor<AccountEntitlementUpdateModel>(new AccountEntitlementUpdateModel()
        {
          License = Microsoft.VisualStudio.Services.Licensing.License.Auto
        });
        ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        HttpContent content = contentFor;
        CancellationToken cancellationToken1 = cancellationToken;
        object obj = userState;
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
        object userState1 = obj;
        CancellationToken cancellationToken2 = cancellationToken1;
        accountEntitlement = await compatHttpClientBase2.SendAsync<AccountEntitlement>(put, entitlementsLocationId, (object) routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState1, cancellationToken2).ConfigureAwait(false);
      }
      return accountEntitlement;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<AccountEntitlement> AssignAvailableEntitlementAsync(
      Guid userId,
      bool dontNotifyUser,
      object userState,
      CancellationToken cancellationToken)
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      AccountEntitlement accountEntitlement;
      using (new VssHttpClientBase.OperationScope("Licensing", "AssignEntitlement"))
      {
        ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add<bool>(nameof (dontNotifyUser), dontNotifyUser);
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod put = HttpMethod.Put;
        Guid entitlementsLocationId = LicensingResourceIds.UserEntitlementsLocationId;
        var routeValues = new{ userId = userId };
        HttpContent contentFor = (HttpContent) compatHttpClientBase1.CreateContentFor<AccountEntitlementUpdateModel>(new AccountEntitlementUpdateModel()
        {
          License = Microsoft.VisualStudio.Services.Licensing.License.Auto
        });
        ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        HttpContent content = contentFor;
        CancellationToken cancellationToken1 = cancellationToken;
        object obj = userState;
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
        object userState1 = obj;
        CancellationToken cancellationToken2 = cancellationToken1;
        accountEntitlement = await compatHttpClientBase2.SendAsync<AccountEntitlement>(put, entitlementsLocationId, (object) routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState1, cancellationToken2).ConfigureAwait(false);
      }
      return accountEntitlement;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<IDictionary<string, bool>> ComputeExtensionRightsAsync(
      IEnumerable<string> extensionIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      IDictionary<string, bool> extensionRightsAsync;
      using (new VssHttpClientBase.OperationScope("Licensing", "ComputeExtensionRights"))
      {
        ObjectContent<IEnumerable<string>> objectContent = new ObjectContent<IEnumerable<string>>(extensionIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod post = HttpMethod.Post;
        Guid rightsLocationId = LicensingResourceIds.ExtensionRightsLocationId;
        ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        ObjectContent<IEnumerable<string>> content = objectContent;
        CancellationToken cancellationToken1 = cancellationToken;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        extensionRightsAsync = await compatHttpClientBase2.SendAsync<IDictionary<string, bool>>(post, rightsLocationId, version: version, content: (HttpContent) content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return extensionRightsAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<ExtensionRightsResult> GetExtensionRightsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      ExtensionRightsResult extensionRightsAsync;
      using (new VssHttpClientBase.OperationScope("Licensing", "GetExtensionRights"))
      {
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod get = HttpMethod.Get;
        Guid rightsLocationId = LicensingResourceIds.ExtensionRightsLocationId;
        ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        extensionRightsAsync = await compatHttpClientBase2.SendAsync<ExtensionRightsResult>(get, rightsLocationId, version: version, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return extensionRightsAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<IEnumerable<AccountLicenseUsage>> GetAccountLicensesUsageAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      IEnumerable<AccountLicenseUsage> licensesUsageAsync;
      using (new VssHttpClientBase.OperationScope("Licensing", "GetAccountLicensesUsage"))
      {
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod get = HttpMethod.Get;
        Guid usageLocationid = LicensingResourceIds.UsageLocationid;
        ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        licensesUsageAsync = await compatHttpClientBase2.SendAsync<IEnumerable<AccountLicenseUsage>>(get, usageLocationid, version: version, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false) ?? Enumerable.Empty<AccountLicenseUsage>();
      }
      return licensesUsageAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<IEnumerable<AccountEntitlement>> GetAccountEntitlementsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      IEnumerable<AccountEntitlement> entitlementsAsync;
      using (new VssHttpClientBase.OperationScope("Licensing", "GetAccountEntitlements"))
      {
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod get = HttpMethod.Get;
        Guid entitlementsLocationid = LicensingResourceIds.EntitlementsLocationid;
        ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        entitlementsAsync = await compatHttpClientBase2.SendAsync<IEnumerable<AccountEntitlement>>(get, entitlementsLocationid, version: version, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return entitlementsAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<IEnumerable<AccountEntitlement>> GetAccountEntitlementsAsync(
      int top,
      int skip = 0,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add<int>(nameof (top), top);
      collection.Add<int>(nameof (skip), skip);
      List<KeyValuePair<string, string>> keyValuePairList = collection;
      IEnumerable<AccountEntitlement> entitlementsAsync;
      using (new VssHttpClientBase.OperationScope("Licensing", "GetAccountEntitlements"))
      {
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod get = HttpMethod.Get;
        Guid entitlementsLocationid = LicensingResourceIds.EntitlementsLocationid;
        ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        entitlementsAsync = await compatHttpClientBase2.SendAsync<IEnumerable<AccountEntitlement>>(get, entitlementsLocationid, version: version, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return entitlementsAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<PagedAccountEntitlements> SearchAccountEntitlementsAsync(
      string continuation = null,
      string filter = null,
      string orderBy = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
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
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod get = HttpMethod.Get;
        Guid searchLocationId = LicensingResourceIds.EntitlementsSearchLocationId;
        ApiResourceVersion apiResourceVersion = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        object obj = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        var routeValues = new{ action = "Search" };
        ApiResourceVersion version = apiResourceVersion;
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = obj;
        CancellationToken cancellationToken2 = cancellationToken1;
        accountEntitlements = await compatHttpClientBase2.SendAsync<PagedAccountEntitlements>(get, searchLocationId, (object) routeValues, version, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return accountEntitlements;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<IEnumerable<AccountEntitlement>> GetAccountEntitlementsAsync(
      IList<Guid> userIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      IEnumerable<AccountEntitlement> entitlementsAsync;
      using (new VssHttpClientBase.OperationScope("Licensing", "GetAccountEntitlements"))
      {
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) userIds, nameof (userIds));
        ObjectContent<IList<Guid>> objectContent = new ObjectContent<IList<Guid>>(userIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod post = HttpMethod.Post;
        Guid entitlementsBatchLocationId = LicensingResourceIds.UserEntitlementsBatchLocationId;
        ApiResourceVersion apiResourceVersion = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        var routeValues = new
        {
          action = "GetUsersEntitlements"
        };
        ApiResourceVersion version = apiResourceVersion;
        ObjectContent<IList<Guid>> content = objectContent;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        entitlementsAsync = await compatHttpClientBase2.SendAsync<IEnumerable<AccountEntitlement>>(post, entitlementsBatchLocationId, (object) routeValues, version, (HttpContent) content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return entitlementsAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<IList<AccountEntitlement>> ObtainAvailableAccountEntitlementsAsync(
      IList<Guid> userIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      IList<AccountEntitlement> entitlementsAsync;
      using (new VssHttpClientBase.OperationScope("Licensing", "GetAccountEntitlements"))
      {
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) userIds, nameof (userIds));
        ObjectContent<IList<Guid>> objectContent = new ObjectContent<IList<Guid>>(userIds, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod post = HttpMethod.Post;
        Guid entitlementsBatchLocationId = LicensingResourceIds.UserEntitlementsBatchLocationId;
        ApiResourceVersion apiResourceVersion = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        var routeValues = new
        {
          action = "GetAvailableUsersEntitlements"
        };
        ApiResourceVersion version = apiResourceVersion;
        ObjectContent<IList<Guid>> content = objectContent;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        entitlementsAsync = await compatHttpClientBase2.SendAsync<IList<AccountEntitlement>>(post, entitlementsBatchLocationId, (object) routeValues, version, (HttpContent) content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return entitlementsAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<AccountEntitlement> GetAccountEntitlementAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      AccountEntitlement entitlementAsync;
      using (new VssHttpClientBase.OperationScope("Licensing", "GetAccountEntitlement"))
      {
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod get = HttpMethod.Get;
        Guid entitlementsLocationId = LicensingResourceIds.CurrentUserEntitlementsLocationId;
        ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        entitlementAsync = await compatHttpClientBase2.SendAsync<AccountEntitlement>(get, entitlementsLocationId, version: version, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return entitlementAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<AccountEntitlement> GetAccountEntitlementAsync(
      Guid userId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetAccountEntitlementAsync(userId, (List<KeyValuePair<string, string>>) null, userState, cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<AccountEntitlement> GetAccountEntitlementAsync(
      Guid userId,
      bool determineRights,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add<bool>(nameof (determineRights), determineRights);
      List<KeyValuePair<string, string>> queryParams = collection;
      return this.GetAccountEntitlementAsync(userId, queryParams, userState, cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<AccountEntitlement> GetAccountEntitlementAsync(
      Guid userId,
      bool determineRights,
      bool createIfNotExists,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add<bool>(nameof (determineRights), determineRights);
      collection.Add<bool>(nameof (createIfNotExists), createIfNotExists);
      List<KeyValuePair<string, string>> queryParams = collection;
      return this.GetAccountEntitlementAsync(userId, queryParams, userState, cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    private async Task<AccountEntitlement> GetAccountEntitlementAsync(
      Guid userId,
      List<KeyValuePair<string, string>> queryParams,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      AccountEntitlement entitlementAsync;
      using (new VssHttpClientBase.OperationScope("Licensing", "GetAccountEntitlement"))
      {
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod get = HttpMethod.Get;
        Guid entitlementsLocationId = LicensingResourceIds.UserEntitlementsLocationId;
        ApiResourceVersion apiResourceVersion = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        var routeValues = new{ userId = userId };
        ApiResourceVersion version = apiResourceVersion;
        object obj = userState;
        List<KeyValuePair<string, string>> queryParameters = queryParams;
        object userState1 = obj;
        CancellationToken cancellationToken2 = cancellationToken1;
        entitlementAsync = await compatHttpClientBase2.SendAsync<AccountEntitlement>(get, entitlementsLocationId, (object) routeValues, version, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return entitlementAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<AccountEntitlement> AssignEntitlementAsync(
      Guid userId,
      Microsoft.VisualStudio.Services.Licensing.License license,
      bool dontNotifyUser = false,
      LicensingOrigin origin = LicensingOrigin.None,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      AccountEntitlement accountEntitlement;
      using (new VssHttpClientBase.OperationScope("Licensing", "AssignEntitlement"))
      {
        ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
        ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Licensing.License>(license, nameof (license));
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add<bool>(nameof (dontNotifyUser), dontNotifyUser);
        collection.Add<LicensingOrigin>(nameof (origin), origin);
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod put = HttpMethod.Put;
        Guid entitlementsLocationId = LicensingResourceIds.UserEntitlementsLocationId;
        var routeValues = new{ userId = userId };
        HttpContent contentFor = (HttpContent) compatHttpClientBase1.CreateContentFor<AccountEntitlementUpdateModel>(new AccountEntitlementUpdateModel()
        {
          License = license
        });
        ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        HttpContent content = contentFor;
        CancellationToken cancellationToken1 = cancellationToken;
        object obj = userState;
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
        object userState1 = obj;
        CancellationToken cancellationToken2 = cancellationToken1;
        accountEntitlement = await compatHttpClientBase2.SendAsync<AccountEntitlement>(put, entitlementsLocationId, (object) routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState1, cancellationToken2).ConfigureAwait(false);
      }
      return accountEntitlement;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<AccountEntitlement> AssignAvailableEntitlementAsync(
      Guid userId,
      bool dontNotifyUser = false,
      LicensingOrigin origin = LicensingOrigin.None,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      AccountEntitlement accountEntitlement;
      using (new VssHttpClientBase.OperationScope("Licensing", "AssignEntitlement"))
      {
        ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add<bool>(nameof (dontNotifyUser), dontNotifyUser);
        collection.Add<LicensingOrigin>(nameof (origin), origin);
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod put = HttpMethod.Put;
        Guid entitlementsLocationId = LicensingResourceIds.UserEntitlementsLocationId;
        var routeValues = new{ userId = userId };
        HttpContent contentFor = (HttpContent) compatHttpClientBase1.CreateContentFor<AccountEntitlementUpdateModel>(new AccountEntitlementUpdateModel()
        {
          License = Microsoft.VisualStudio.Services.Licensing.License.Auto
        });
        ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        HttpContent content = contentFor;
        CancellationToken cancellationToken1 = cancellationToken;
        object obj = userState;
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
        object userState1 = obj;
        CancellationToken cancellationToken2 = cancellationToken1;
        accountEntitlement = await compatHttpClientBase2.SendAsync<AccountEntitlement>(put, entitlementsLocationId, (object) routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState1, cancellationToken2).ConfigureAwait(false);
      }
      return accountEntitlement;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task DeleteEntitlementAsync(
      Guid userId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
      HttpMethod delete = HttpMethod.Delete;
      Guid entitlementsLocationId = LicensingResourceIds.UserEntitlementsLocationId;
      var routeValues = new{ userId = userId };
      ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
      CancellationToken cancellationToken1 = cancellationToken;
      object obj = userState;
      List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      HttpResponseMessage httpResponseMessage = await compatHttpClientBase2.SendAsync(delete, entitlementsLocationId, (object) routeValues, version, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<bool> RegisterExtensionLicenseAsync(
      ExtensionLicenseData extensionLicenseData,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      bool flag;
      using (new VssHttpClientBase.OperationScope("Licensing", "ExtensionLicenseRegistration"))
      {
        ObjectContent<ExtensionLicenseData> objectContent = new ObjectContent<ExtensionLicenseData>(extensionLicenseData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod post = HttpMethod.Post;
        Guid licenseLocationId = LicensingResourceIds.ExtensionLicenseLocationId;
        ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        ObjectContent<ExtensionLicenseData> content = objectContent;
        CancellationToken cancellationToken1 = cancellationToken;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        flag = await compatHttpClientBase2.SendAsync<bool>(post, licenseLocationId, version: version, content: (HttpContent) content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return flag;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<ExtensionLicenseData> GetExtensionLicenseDataAsync(
      string extensionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      ExtensionLicenseData licenseDataAsync;
      using (new VssHttpClientBase.OperationScope("Licensing", "ExtensionLicenseRegistration"))
      {
        LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
        HttpMethod get = HttpMethod.Get;
        Guid licenseLocationId = LicensingResourceIds.ExtensionLicenseLocationId;
        ApiResourceVersion apiResourceVersion = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
        CancellationToken cancellationToken1 = cancellationToken;
        var routeValues = new{ extensionId = extensionId };
        ApiResourceVersion version = apiResourceVersion;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        licenseDataAsync = await compatHttpClientBase2.SendAsync<ExtensionLicenseData>(get, licenseLocationId, (object) routeValues, version, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
      }
      return licenseDataAsync;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task TransferIdentityRightsAsync(
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap,
      bool? validateOnly = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LicensingCompatHttpClientBase compatHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("8671b016-fa74-4c88-b693-83bbb88c2264");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<KeyValuePair<Guid, Guid>>>(userIdTransferMap, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (validateOnly.HasValue)
        collection.Add(nameof (validateOnly), validateOnly.Value.ToString());
      LicensingCompatHttpClientBase compatHttpClientBase2 = compatHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(LicensingCompatHttpClientBase.previewApiVersion, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await compatHttpClientBase2.SendAsync(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    protected ObjectContent<T> CreateContentFor<T>(T value) => new ObjectContent<T>(value, this.Formatter);

    protected static void ValidateClientRightsQueryContext(ClientRightsQueryContext queryContext)
    {
      ArgumentUtility.CheckForNull<ClientRightsQueryContext>(queryContext, nameof (queryContext));
      ArgumentUtility.CheckStringForNullOrEmpty(queryContext.ProductFamily, "queryContext.ProductFamily");
      ArgumentUtility.CheckStringForInvalidCharacters(queryContext.ProductFamily, "queryContext.ProductFamily");
      ArgumentUtility.CheckStringForNullOrEmpty(queryContext.ProductVersion, "queryContext.ProductVersion");
      ArgumentUtility.CheckStringForInvalidCharacters(queryContext.ProductVersion, "queryContext.ProductVersion");
      if (queryContext.ProductEdition != null)
        ArgumentUtility.CheckStringForInvalidCharacters(queryContext.ProductEdition, "queryContext.ProductEdition");
      if (queryContext.ReleaseType != null)
        ArgumentUtility.CheckStringForInvalidCharacters(queryContext.ReleaseType, "queryContext.ReleaseType");
      if (queryContext.Canary != null)
        ArgumentUtility.CheckStringForInvalidCharacters(queryContext.Canary, "queryContext.Canary");
      if (queryContext.MachineId == null)
        return;
      ArgumentUtility.CheckStringForInvalidCharacters(queryContext.MachineId, "queryContext.MachineId");
    }

    protected static void ValidateClientRightsTelemetryContext(
      ClientRightsTelemetryContext telemetryContext)
    {
      if (telemetryContext == null || telemetryContext.Attributes == null || telemetryContext.Attributes.Count < 1)
        return;
      foreach (KeyValuePair<string, string> attribute in (IEnumerable<KeyValuePair<string, string>>) telemetryContext.Attributes)
      {
        ArgumentUtility.CheckStringForInvalidCharacters(attribute.Key, "Key");
        if (string.IsNullOrEmpty(attribute.Value))
          ArgumentUtility.CheckStringForInvalidCharacters(attribute.Value, "Value");
      }
    }

    protected static void SerializeTelemetryContextAsOptionalQueryParameters(
      ClientRightsTelemetryContext telemetryContext,
      IList<KeyValuePair<string, string>> queryParameters)
    {
      if (telemetryContext == null || telemetryContext.Attributes == null || telemetryContext.Attributes.Count < 1)
        return;
      foreach (KeyValuePair<string, string> attribute in (IEnumerable<KeyValuePair<string, string>>) telemetryContext.Attributes)
        queryParameters.Add(new KeyValuePair<string, string>("t-" + attribute.Key, attribute.Value ?? string.Empty));
    }
  }
}
