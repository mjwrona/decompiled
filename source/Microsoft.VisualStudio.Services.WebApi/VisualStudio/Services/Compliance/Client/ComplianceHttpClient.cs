// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Compliance.Client.ComplianceHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Compliance.Client
{
  [ResourceArea("7E7BAADD-B7D6-46A0-9CE5-A6F95DDA0E62")]
  [Obsolete("This type is no longer used.")]
  public class ComplianceHttpClient : VssHttpClientBase
  {
    private static readonly Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>();
    protected static readonly Version apiVersion = new Version(1, 0);

    public ComplianceHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ComplianceHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ComplianceHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ComplianceHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ComplianceHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task<ComplianceConfiguration> GetComplianceConfiguration(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ComplianceHttpClient complianceHttpClient = this;
      ComplianceConfiguration complianceConfiguration;
      using (new VssHttpClientBase.OperationScope("Compliance", nameof (GetComplianceConfiguration)))
        complianceConfiguration = await complianceHttpClient.SendAsync<ComplianceConfiguration>(HttpMethod.Get, ComplianceResourceIds.ConfigurationLocationId, version: new ApiResourceVersion(ComplianceHttpClient.apiVersion, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      return complianceConfiguration;
    }

    public virtual async Task<AccountRightsValidation> ValidateAccountRights(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ComplianceHttpClient complianceHttpClient1 = this;
      AccountRightsValidation rightsValidation;
      using (new VssHttpClientBase.OperationScope("Compliance", nameof (ValidateAccountRights)))
      {
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        ComplianceHttpClient complianceHttpClient2 = complianceHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid rightsLocationId = ComplianceResourceIds.AccountRightsLocationId;
        ApiResourceVersion version = new ApiResourceVersion(ComplianceHttpClient.apiVersion, 1);
        object obj = userState;
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        rightsValidation = await complianceHttpClient2.SendAsync<AccountRightsValidation>(get, rightsLocationId, version: version, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return rightsValidation;
    }

    public virtual async Task<ComplianceValidation> ValidateBusinessPolicy(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ComplianceHttpClient complianceHttpClient1 = this;
      ComplianceValidation complianceValidation;
      using (new VssHttpClientBase.OperationScope("Compliance", nameof (ValidateBusinessPolicy)))
      {
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        ComplianceHttpClient complianceHttpClient2 = complianceHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid validationLocationId = ComplianceResourceIds.ValidationLocationId;
        ApiResourceVersion version = new ApiResourceVersion(ComplianceHttpClient.apiVersion, 1);
        object obj = userState;
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        complianceValidation = await complianceHttpClient2.SendAsync<ComplianceValidation>(get, validationLocationId, version: version, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return complianceValidation;
    }

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) ComplianceHttpClient.s_translatedExceptions;
  }
}
