// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize.IMutableVersionListWithSizeFile
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize
{
  public interface IMutableVersionListWithSizeFile : 
    ILazyVersionListWithSizeFile,
    IVersionCountsImplementationMetrics
  {
    void AddPackageVersionToFeed(
      IPackageIdentity packageIdentity,
      DateTime modTime,
      List<IPackageFile> packageFiles);

    void SetPackageVersionDeletedState(
      IPackageIdentity packageIdentity,
      bool isDeleted,
      DateTime modTime);

    void PermanentlyDeletePackageVersionFromFeed(IPackageIdentity packageIdentity, DateTime modTime);
  }
}
