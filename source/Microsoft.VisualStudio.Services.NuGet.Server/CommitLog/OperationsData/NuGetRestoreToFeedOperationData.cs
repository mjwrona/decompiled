// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.CommitLog.OperationsData.NuGetRestoreToFeedOperationData
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;

namespace Microsoft.VisualStudio.Services.NuGet.Server.CommitLog.OperationsData
{
  public class NuGetRestoreToFeedOperationData : 
    RestoreToFeedOperationData,
    INuGetRestoreToFeedOperationData,
    IRestoreToFeedOperationData,
    IPackageVersionOperationData,
    IPackageOperationData,
    ICommitOperationData
  {
    public NuGetRestoreToFeedOperationData(IPackageIdentity identity, byte[] nuspecBytes)
      : base(identity)
    {
      this.NuspecBytes = nuspecBytes;
    }

    public byte[] NuspecBytes { get; set; }
  }
}
