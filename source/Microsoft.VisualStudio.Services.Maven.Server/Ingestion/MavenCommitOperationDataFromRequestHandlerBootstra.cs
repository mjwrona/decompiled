// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Ingestion.MavenCommitOperationDataFromRequestHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.OperationData;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;

namespace Microsoft.VisualStudio.Services.Maven.Server.Ingestion
{
  public class MavenCommitOperationDataFromRequestHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<IStorablePackageInfo<MavenPackageIdentity, MavenPackageFileInfo>, MavenCommitOperationData>>
  {
    private readonly IVssRequestContext requestContext;

    public MavenCommitOperationDataFromRequestHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<IStorablePackageInfo<MavenPackageIdentity, MavenPackageFileInfo>, MavenCommitOperationData> Bootstrap() => (IAsyncHandler<IStorablePackageInfo<MavenPackageIdentity, MavenPackageFileInfo>, MavenCommitOperationData>) new MavenCommitOperationDataFromStorablePackageInfoHandler((IProvenanceInfoProvider) new ProvenanceInfoProviderBootstrapper(this.requestContext).Bootstrap(), (ITimeProvider) new DefaultTimeProvider());
  }
}
