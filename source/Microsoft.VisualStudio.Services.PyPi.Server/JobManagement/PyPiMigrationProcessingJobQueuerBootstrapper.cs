// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.JobManagement.PyPiMigrationProcessingJobQueuerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.PyPi.Server.Constants;

namespace Microsoft.VisualStudio.Services.PyPi.Server.JobManagement
{
  public class PyPiMigrationProcessingJobQueuerBootstrapper : IBootstrapper<IFeedJobQueuer>
  {
    private readonly IVssRequestContext requestContext;

    public PyPiMigrationProcessingJobQueuerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFeedJobQueuer Bootstrap() => new FeedJobQueuerBasicBootstrapper(this.requestContext, Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MigrationJobConstants.MigrationProcessingJobType, PyPiJobConstants.MigrationJobConstants.PyPiMigrationProcessingJobCreationInfo).Bootstrap();
  }
}
