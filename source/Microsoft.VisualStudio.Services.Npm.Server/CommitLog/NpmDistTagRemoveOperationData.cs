// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CommitLog.NpmDistTagRemoveOperationData
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;

namespace Microsoft.VisualStudio.Services.Npm.Server.CommitLog
{
  public class NpmDistTagRemoveOperationData : IPackageOperationData, ICommitOperationData
  {
    public NpmDistTagRemoveOperationData(IPackageName packageName, string tag)
    {
      this.PackageName = packageName;
      this.Tag = tag;
    }

    public IPackageName PackageName { get; }

    public string Tag { get; }

    public RingOrder RingOrder => RingOrder.OuterToInner;

    public FeedPermissionConstants PermissionDemand => FeedPermissionConstants.UpdatePackage;
  }
}
