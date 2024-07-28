// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.UserLicensing.Client.UserLicensingHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Licensing;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.UserLicensing.Client
{
  [ResourceArea("5B508ADE-4C35-4913-A78E-6312FF28F84E")]
  public class UserLicensingHttpClient : UserLicensingHttpClientBase
  {
    public UserLicensingHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public UserLicensingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public UserLicensingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public UserLicensingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public UserLicensingHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public new virtual async Task<Stream> GetCertificateAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      Stream certificateAsync;
      using (new VssHttpClientBase.OperationScope("UserLicensing", "GetCertificate"))
        certificateAsync = await base.GetCertificateAsync(userState, cancellationToken).ConfigureAwait(false);
      return certificateAsync;
    }

    public virtual async Task<List<MsdnEntitlement>> GetMsdnEntitlementsAsync(
      string descriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      List<MsdnEntitlement> entitlementsAsync;
      using (new VssHttpClientBase.OperationScope("UserLicensing", "GetMsdnEntitlements"))
      {
        // ISSUE: reference to a compiler-generated method
        entitlementsAsync = await this.\u003C\u003En__1(descriptor, userState, cancellationToken).ConfigureAwait(false);
      }
      return entitlementsAsync;
    }

    public virtual async Task<ClientRightsContainer> GetClientRightsContainerAsync(
      string descriptor,
      ClientRightsQueryContext queryContext,
      ClientRightsTelemetryContext telemetryContext = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ClientRightsContainer rightsContainerAsync;
      using (new VssHttpClientBase.OperationScope("Licensing", "GetClientRightsContainer"))
      {
        UserLicensingHttpClient.ValidateClientRightsQueryContext(queryContext);
        UserLicensingHttpClient.ValidateClientRightsTelemetryContext(telemetryContext);
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
        UserLicensingHttpClient.SerializeTelemetryContextAsOptionalQueryParameters(telemetryContext, (IList<KeyValuePair<string, string>>) keyValuePairList);
        // ISSUE: reference to a compiler-generated method
        rightsContainerAsync = await this.\u003C\u003En__2(descriptor, queryContext.ProductFamily, queryContext.ProductVersion, queryContext.ProductEdition, queryContext.ReleaseType, new bool?(queryContext.IncludeCertificate), queryContext.Canary, queryContext.MachineId, userState, cancellationToken).ConfigureAwait(false);
      }
      return rightsContainerAsync;
    }

    public Task SetVisualStudioTrialInfoAsync(
      SubjectDescriptor descriptor,
      int majorVersion,
      int productFamilyId,
      int productEditionId,
      DateTime expirationDate,
      DateTime createdDate,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SetVisualStudioTrialInfoAsync((string) descriptor, majorVersion, productFamilyId, productEditionId, expirationDate, createdDate, userState, cancellationToken);
    }

    public Task<long> GetVisualStudioTrialExpirationAsync(
      SubjectDescriptor descriptor,
      string machineId,
      int majorVersion,
      int productFamilyId,
      int productEditionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetVisualStudioTrialExpirationAsync((string) descriptor, machineId, majorVersion, productFamilyId, productEditionId, userState, cancellationToken);
    }

    private static void ValidateClientRightsQueryContext(ClientRightsQueryContext queryContext)
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

    private static void ValidateClientRightsTelemetryContext(
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

    private static void SerializeTelemetryContextAsOptionalQueryParameters(
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
