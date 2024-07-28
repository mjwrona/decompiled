// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types.UpstreamStatusCategoryExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 208E7E0C-C249-4CB0-B738-E2A4534A31E8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.dll

using System;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types
{
  public static class UpstreamStatusCategoryExtensions
  {
    private static readonly IReadOnlyList<UpstreamStatusCategoryDetails> DetailsList = (IReadOnlyList<UpstreamStatusCategoryDetails>) new UpstreamStatusCategoryDetails[15]
    {
      new UpstreamStatusCategoryDetails(UpstreamStatusCategory.FullRefreshSuccess, CategoryType.Ok, false, true),
      new UpstreamStatusCategoryDetails(UpstreamStatusCategory.Aborted, CategoryType.Ok, false, false),
      new UpstreamStatusCategoryDetails(UpstreamStatusCategory.TargetViewInsufficientVisibility, CategoryType.Failure, true, false),
      new UpstreamStatusCategoryDetails(UpstreamStatusCategory.TargetViewDeleted, CategoryType.Failure, true, false),
      new UpstreamStatusCategoryDetails(UpstreamStatusCategory.TargetFeedDeleted, CategoryType.Failure, true, false),
      new UpstreamStatusCategoryDetails(UpstreamStatusCategory.TargetProjectDeleted, CategoryType.Failure, true, false),
      new UpstreamStatusCategoryDetails(UpstreamStatusCategory.IngestDownloadFailure, CategoryType.Warning, false, true),
      new UpstreamStatusCategoryDetails(UpstreamStatusCategory.IngestProcessingFailure, CategoryType.Warning, false, true),
      new UpstreamStatusCategoryDetails(UpstreamStatusCategory.CustomPublicUpstreamFailure, CategoryType.Failure, false, false),
      new UpstreamStatusCategoryDetails(UpstreamStatusCategory.PublicUpstreamFailure, CategoryType.Warning, false, true),
      new UpstreamStatusCategoryDetails(UpstreamStatusCategory.UnknownFailure, CategoryType.Warning, false, true),
      new UpstreamStatusCategoryDetails(UpstreamStatusCategory.TargetOrganizationInaccessible, CategoryType.Failure, true, false),
      new UpstreamStatusCategoryDetails(UpstreamStatusCategory.CustomPublicUpstreamFailure_DuplicatePackageVersions, CategoryType.Failure, false, true),
      new UpstreamStatusCategoryDetails(UpstreamStatusCategory.TargetOrganizationServiceConnectionFailure, CategoryType.Failure, true, false),
      new UpstreamStatusCategoryDetails(UpstreamStatusCategory.BlockedBySystem, CategoryType.Failure, true, false)
    };

    public static UpstreamStatusCategoryDetails Details(this UpstreamStatusCategory category)
    {
      int index = (int) category;
      if (index < 0 || index >= UpstreamStatusCategoryExtensions.DetailsList.Count)
        throw new ArgumentOutOfRangeException(nameof (category), (object) category, "Unknown category");
      return UpstreamStatusCategoryExtensions.DetailsList[index];
    }
  }
}
