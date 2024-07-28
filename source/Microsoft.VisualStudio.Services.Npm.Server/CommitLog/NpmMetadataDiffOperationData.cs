// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CommitLog.NpmMetadataDiffOperationData
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.Server.CommitLog
{
  public class NpmMetadataDiffOperationData : IPackageOperationData, ICommitOperationData
  {
    public NpmMetadataDiffOperationData(
      IPackageName name,
      IDictionary<string, VersionMetadata> oldVersionMetadata,
      IDictionary<string, VersionMetadata> newVersionMetadata)
    {
      this.PackageName = name;
      this.OldVersionMetadata = oldVersionMetadata;
      this.NewVersionMetadata = newVersionMetadata;
    }

    public IDictionary<string, VersionMetadata> OldVersionMetadata { get; }

    public IDictionary<string, VersionMetadata> NewVersionMetadata { get; }

    public IPackageName PackageName { get; }

    public RingOrder RingOrder => RingOrder.InnerToOuter;

    public FeedPermissionConstants PermissionDemand => FeedPermissionConstants.UpdatePackage;
  }
}
