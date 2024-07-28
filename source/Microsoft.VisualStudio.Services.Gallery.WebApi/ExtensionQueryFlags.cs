// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionQueryFlags
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  [Flags]
  public enum ExtensionQueryFlags
  {
    None = 0,
    IncludeVersions = 1,
    IncludeFiles = 2,
    IncludeCategoryAndTags = 4,
    IncludeSharedAccounts = 8,
    IncludeVersionProperties = 16, // 0x00000010
    ExcludeNonValidated = 32, // 0x00000020
    IncludeInstallationTargets = 64, // 0x00000040
    IncludeAssetUri = 128, // 0x00000080
    IncludeStatistics = 256, // 0x00000100
    IncludeLatestVersionOnly = 512, // 0x00000200
    UseFallbackAssetUri = 1024, // 0x00000400
    IncludeMetadata = 2048, // 0x00000800
    IncludeMinimalPayloadForVsIde = 4096, // 0x00001000
    IncludeLcids = 8192, // 0x00002000
    IncludeSharedOrganizations = 16384, // 0x00004000
    IncludeNameConflictInfo = 32768, // 0x00008000
    AllAttributes = IncludeSharedOrganizations | IncludeStatistics | IncludeAssetUri | IncludeInstallationTargets | IncludeVersionProperties | IncludeSharedAccounts | IncludeCategoryAndTags | IncludeFiles | IncludeVersions, // 0x000041DF
  }
}
