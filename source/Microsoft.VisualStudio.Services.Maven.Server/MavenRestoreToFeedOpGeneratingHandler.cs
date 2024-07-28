// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenRestoreToFeedOpGeneratingHandler
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.OperationData;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class MavenRestoreToFeedOpGeneratingHandler : 
    IAsyncHandler<PackageRequest<MavenPackageIdentity>, IRestoreToFeedOperationData>,
    IHaveInputType<PackageRequest<MavenPackageIdentity>>,
    IHaveOutputType<IRestoreToFeedOperationData>
  {
    public Task<IRestoreToFeedOperationData> Handle(PackageRequest<MavenPackageIdentity> request) => Task.FromResult<IRestoreToFeedOperationData>((IRestoreToFeedOperationData) new MavenRestoreToFeedOperationData(request.PackageId));
  }
}
