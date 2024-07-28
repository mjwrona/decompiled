// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.JobManagement.NpmMigrationProcessingJobQueuerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.Constants;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;

namespace Microsoft.VisualStudio.Services.Npm.Server.JobManagement
{
  public class NpmMigrationProcessingJobQueuerBootstrapper : IBootstrapper<IFeedJobQueuer>
  {
    private readonly IVssRequestContext requestContext;

    public NpmMigrationProcessingJobQueuerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFeedJobQueuer Bootstrap() => new FeedJobQueuerBasicBootstrapper(this.requestContext, Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MigrationJobConstants.MigrationProcessingJobType, NpmConstants.MigrationJobConstants.NpmMigrationProcessingJobCreationInfo).Bootstrap();
  }
}
