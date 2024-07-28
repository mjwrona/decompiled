// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.OperationData.MavenRestoreToFeedOperationData
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Contracts;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;

namespace Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.OperationData
{
  public class MavenRestoreToFeedOperationData : 
    RestoreToFeedOperationData,
    IMavenRestoreToFeedOperationData,
    IRestoreToFeedOperationData,
    IPackageVersionOperationData,
    IPackageOperationData,
    ICommitOperationData
  {
    public MavenRestoreToFeedOperationData(MavenPackageIdentity packageIdentity)
      : base((IPackageIdentity) packageIdentity)
    {
    }

    public MavenPackageIdentity Identity => base.Identity as MavenPackageIdentity;
  }
}
