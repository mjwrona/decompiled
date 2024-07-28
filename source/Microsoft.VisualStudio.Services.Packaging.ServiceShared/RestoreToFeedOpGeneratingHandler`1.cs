// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.RestoreToFeedOpGeneratingHandler`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class RestoreToFeedOpGeneratingHandler<TPackageIdentity> : 
    IAsyncHandler<PackageRequest<TPackageIdentity>, IRestoreToFeedOperationData>,
    IHaveInputType<PackageRequest<TPackageIdentity>>,
    IHaveOutputType<IRestoreToFeedOperationData>
    where TPackageIdentity : IPackageIdentity
  {
    public Task<IRestoreToFeedOperationData> Handle(PackageRequest<TPackageIdentity> request) => Task.FromResult<IRestoreToFeedOperationData>((IRestoreToFeedOperationData) new RestoreToFeedOperationData((IPackageIdentity) request.PackageId));
  }
}
