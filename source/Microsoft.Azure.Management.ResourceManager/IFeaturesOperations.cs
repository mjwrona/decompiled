// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.IFeaturesOperations
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest.Azure;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Management.ResourceManager
{
  public interface IFeaturesOperations
  {
    Task<AzureOperationResponse<IPage<FeatureResult>>> ListAllWithHttpMessagesAsync(
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<FeatureResult>>> ListWithHttpMessagesAsync(
      string resourceProviderNamespace,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<FeatureResult>> GetWithHttpMessagesAsync(
      string resourceProviderNamespace,
      string featureName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<FeatureResult>> RegisterWithHttpMessagesAsync(
      string resourceProviderNamespace,
      string featureName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<FeatureResult>>> ListAllNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<FeatureResult>>> ListNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
