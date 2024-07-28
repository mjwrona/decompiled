// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.AzureBlobGeoRedundancyConstants
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy
{
  public static class AzureBlobGeoRedundancyConstants
  {
    public const string FeatureFlag = "Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy";
    public const string FeatureFlagServiceSideCopy = "Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.EnableServiceSideCopy";
    public const string QueueName = "azureblobgeoredundancyqueue";
    public const string QueuedSnapshotTableName = "AzureBlobGeoRedundancyQueued";
    public const string CompletedSnapshotTableName = "AzureBlobGeoRedundancyCompleted";
    public const string CheckpointContainerName = "azureblobgeoredundancycheckpoints";
    public const string FileServicePrimaryStoragePrefix = "FileServiceStorageAccount";
    public const string FileServiceSecondaryStoragePrefix = "FileServiceOmegaStorageAccount";
    public const string BaseRegistryPath = "/Service/AzureBlobGeoRedundancy";
    public static readonly IReadOnlyCollection<string> ExcludedContainers = (IReadOnlyCollection<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "azureblobgeoredundancycheckpoints"
    };
  }
}
