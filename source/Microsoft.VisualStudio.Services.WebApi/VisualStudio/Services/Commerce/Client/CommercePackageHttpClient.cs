// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Client.CommercePackageHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Commerce.Client
{
  [ResourceArea("365D9DCD-4492-4AE3-B5BA-AD0FF4AB74B3")]
  public class CommercePackageHttpClient : VssHttpClientBase
  {
    protected static readonly Version currentApiVersion = new Version(3, 0);
    internal static readonly Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>()
    {
      {
        "InvalidResourceException",
        typeof (InvalidResourceException)
      },
      {
        "CommerceSecurityException",
        typeof (CommerceSecurityException)
      },
      {
        "AccountNotFoundException",
        typeof (AccountNotFoundException)
      },
      {
        "AccountQuantityException",
        typeof (AccountQuantityException)
      }
    };

    public CommercePackageHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public CommercePackageHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public CommercePackageHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public CommercePackageHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public CommercePackageHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [Obsolete]
    public virtual async Task<CommercePackage> GetCommercePackage(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CommercePackageHttpClient packageHttpClient1 = this;
      CommercePackage commercePackage;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (GetCommercePackage)))
      {
        CommercePackageHttpClient packageHttpClient2 = packageHttpClient1;
        Guid packageLocationId = CommerceResourceIds.CommercePackageLocationId;
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion(CommercePackageHttpClient.currentApiVersion, 1);
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        commercePackage = await packageHttpClient2.GetAsync<CommercePackage>(packageLocationId, version: version, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return commercePackage;
    }

    public virtual async Task<CommercePackage> GetCommercePackage(
      string version,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CommercePackageHttpClient packageHttpClient1 = this;
      CommercePackage commercePackage;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (GetCommercePackage)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (version), version);
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        CommercePackageHttpClient packageHttpClient2 = packageHttpClient1;
        Guid packageLocationId = CommerceResourceIds.CommercePackageLocationId;
        object obj = userState;
        ApiResourceVersion version1 = new ApiResourceVersion(CommercePackageHttpClient.currentApiVersion, 1);
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        commercePackage = await packageHttpClient2.GetAsync<CommercePackage>(packageLocationId, version: version1, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return commercePackage;
    }

    [ExcludeFromCodeCoverage]
    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) CommercePackageHttpClient.s_translatedExceptions;
  }
}
