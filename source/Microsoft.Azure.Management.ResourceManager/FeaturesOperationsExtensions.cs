// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.FeaturesOperationsExtensions
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest.Azure;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Management.ResourceManager
{
  public static class FeaturesOperationsExtensions
  {
    public static IPage<FeatureResult> ListAll(this IFeaturesOperations operations) => operations.ListAllAsync().GetAwaiter().GetResult();

    public static async Task<IPage<FeatureResult>> ListAllAsync(
      this IFeaturesOperations operations,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<FeatureResult> body;
      using (AzureOperationResponse<IPage<FeatureResult>> _result = await operations.ListAllWithHttpMessagesAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<FeatureResult> List(
      this IFeaturesOperations operations,
      string resourceProviderNamespace)
    {
      return operations.ListAsync(resourceProviderNamespace).GetAwaiter().GetResult();
    }

    public static async Task<IPage<FeatureResult>> ListAsync(
      this IFeaturesOperations operations,
      string resourceProviderNamespace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<FeatureResult> body;
      using (AzureOperationResponse<IPage<FeatureResult>> _result = await operations.ListWithHttpMessagesAsync(resourceProviderNamespace, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static FeatureResult Get(
      this IFeaturesOperations operations,
      string resourceProviderNamespace,
      string featureName)
    {
      return operations.GetAsync(resourceProviderNamespace, featureName).GetAwaiter().GetResult();
    }

    public static async Task<FeatureResult> GetAsync(
      this IFeaturesOperations operations,
      string resourceProviderNamespace,
      string featureName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeatureResult body;
      using (AzureOperationResponse<FeatureResult> _result = await operations.GetWithHttpMessagesAsync(resourceProviderNamespace, featureName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static FeatureResult Register(
      this IFeaturesOperations operations,
      string resourceProviderNamespace,
      string featureName)
    {
      return operations.RegisterAsync(resourceProviderNamespace, featureName).GetAwaiter().GetResult();
    }

    public static async Task<FeatureResult> RegisterAsync(
      this IFeaturesOperations operations,
      string resourceProviderNamespace,
      string featureName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeatureResult body;
      using (AzureOperationResponse<FeatureResult> _result = await operations.RegisterWithHttpMessagesAsync(resourceProviderNamespace, featureName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<FeatureResult> ListAllNext(
      this IFeaturesOperations operations,
      string nextPageLink)
    {
      return operations.ListAllNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<FeatureResult>> ListAllNextAsync(
      this IFeaturesOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<FeatureResult> body;
      using (AzureOperationResponse<IPage<FeatureResult>> _result = await operations.ListAllNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<FeatureResult> ListNext(
      this IFeaturesOperations operations,
      string nextPageLink)
    {
      return operations.ListNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<FeatureResult>> ListNextAsync(
      this IFeaturesOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<FeatureResult> body;
      using (AzureOperationResponse<IPage<FeatureResult>> _result = await operations.ListNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }
  }
}
