// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement.DisposingMigrationProcessingJobQueuerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement
{
  public class DisposingMigrationProcessingJobQueuerBootstrapper : 
    IBootstrapper<IDisposingFeedJobQueuer>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IFeedJobQueuer migrationProcessingJobQueuer;

    public DisposingMigrationProcessingJobQueuerBootstrapper(
      IVssRequestContext requestContext,
      IFeedJobQueuer migrationProcessingJobQueuer)
    {
      this.requestContext = requestContext;
      this.migrationProcessingJobQueuer = migrationProcessingJobQueuer;
    }

    public IDisposingFeedJobQueuer Bootstrap() => (IDisposingFeedJobQueuer) new DisposingFeedJobQueuer(this.migrationProcessingJobQueuer, this.requestContext);
  }
}
