// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.UserLicensing.Client.UserLicensingHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Licensing;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.UserLicensing.Client
{
  [ResourceArea("5B508ADE-4C35-4913-A78E-6312FF28F84E")]
  public abstract class UserLicensingHttpClientBase : VssHttpClientBase
  {
    public UserLicensingHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public UserLicensingHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public UserLicensingHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public UserLicensingHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public UserLicensingHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    protected virtual async Task<Stream> GetCertificateAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      UserLicensingHttpClientBase licensingHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0f7e6aa1-8d3f-428b-b6d2-5e52d08c343a");
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await licensingHttpClientBase.CreateRequestMessageAsync(method, locationId, version: new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await licensingHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    protected virtual Task<ClientRightsContainer> GetClientRightsAsync(
      string descriptor,
      string rightName = null,
      string productVersion = null,
      string edition = null,
      string relType = null,
      bool? includeCertificate = null,
      string canary = null,
      string machineId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2cc58bfd-3b77-4dc1-b0b3-74b0775d41cb");
      object routeValues = (object) new
      {
        descriptor = descriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (rightName != null)
        keyValuePairList.Add(nameof (rightName), rightName);
      if (productVersion != null)
        keyValuePairList.Add(nameof (productVersion), productVersion);
      if (edition != null)
        keyValuePairList.Add(nameof (edition), edition);
      if (relType != null)
        keyValuePairList.Add(nameof (relType), relType);
      if (includeCertificate.HasValue)
        keyValuePairList.Add(nameof (includeCertificate), includeCertificate.Value.ToString());
      if (canary != null)
        keyValuePairList.Add(nameof (canary), canary);
      if (machineId != null)
        keyValuePairList.Add(nameof (machineId), machineId);
      return this.SendAsync<ClientRightsContainer>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    protected virtual Task<List<MsdnEntitlement>> GetEntitlementsAsync(
      string descriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<MsdnEntitlement>>(new HttpMethod("GET"), new Guid("58dde369-bec9-4f13-93de-e8dfa381293c"), (object) new
      {
        descriptor = descriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    protected virtual Task<long> GetVisualStudioTrialExpirationAsync(
      string descriptor,
      string machineId,
      int majorVersion,
      int productFamilyId,
      int productEditionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2083f3ec-0e90-4267-8122-394a68664a6e");
      object routeValues = (object) new
      {
        descriptor = descriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (machineId), machineId);
      keyValuePairList.Add(nameof (majorVersion), majorVersion.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (productFamilyId), productFamilyId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (productEditionId), productEditionId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<long>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    protected virtual async Task SetVisualStudioTrialInfoAsync(
      string descriptor,
      int majorVersion,
      int productFamilyId,
      int productEditionId,
      DateTime expirationDate,
      DateTime createdDate,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      UserLicensingHttpClientBase licensingHttpClientBase = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("2083f3ec-0e90-4267-8122-394a68664a6e");
      object routeValues = (object) new
      {
        descriptor = descriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (majorVersion), majorVersion.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (productFamilyId), productFamilyId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (productEditionId), productEditionId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      licensingHttpClientBase.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (expirationDate), expirationDate);
      licensingHttpClientBase.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (createdDate), createdDate);
      using (await licensingHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }
  }
}
