// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.StorageAccountConfigurationFacade
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public static class StorageAccountConfigurationFacade
  {
    public static IEnumerable<StrongBoxConnectionString> ReadAllStorageAccounts(
      IVssRequestContext deploymentContext,
      PhysicalDomainInfo physicalDomainInfo = null)
    {
      return !deploymentContext.IsFeatureEnabled("Blobstore.Features.DeploymentLevelStorageConfigurationService") ? StorageAccountUtilities.ReadAllStorageAccounts(deploymentContext, physicalDomainInfo) : deploymentContext.GetService<IStorageAccountConfigurationService>().GetStorageAccounts(deploymentContext, physicalDomainInfo);
    }

    public static Microsoft.Azure.Cosmos.Table.LocationMode? GetTableLocationMode(
      IVssRequestContext deploymentContext)
    {
      return !deploymentContext.IsFeatureEnabled("Blobstore.Features.DeploymentLevelStorageConfigurationService") ? StorageAccountUtilities.ParseTableLocationMode(StorageAccountUtilities.GetLocationMode(deploymentContext)) : deploymentContext.GetService<IStorageAccountConfigurationService>().TableLocationMode;
    }

    public static Microsoft.Azure.Storage.RetryPolicies.LocationMode? GetBlobLocationMode(
      IVssRequestContext deploymentContext)
    {
      return !deploymentContext.IsFeatureEnabled("Blobstore.Features.DeploymentLevelStorageConfigurationService") ? StorageAccountUtilities.ParseLocationMode(StorageAccountUtilities.GetLocationMode(deploymentContext)) : deploymentContext.GetService<IStorageAccountConfigurationService>().BlobLocationMode;
    }
  }
}
