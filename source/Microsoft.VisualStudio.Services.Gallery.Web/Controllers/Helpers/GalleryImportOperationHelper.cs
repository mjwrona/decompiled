// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Controllers.Helpers.GalleryImportOperationHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.VisualStudio.Services.Operations;
using System;
using System.Web;

namespace Microsoft.VisualStudio.Services.Gallery.Web.Controllers.Helpers
{
  public class GalleryImportOperationHelper : GalleryControllerHelper
  {
    public GalleryImportOperationHelper(GalleryController controller)
      : base(controller)
    {
    }

    public Operation GetImportOperation(Guid jobId)
    {
      if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new HttpException(404, WACommonResources.PageNotFound);
      TeamFoundationJobDefinition foundationJobDefinition = this.TfsRequestContext.GetService<ITeamFoundationJobService>().QueryJobDefinition(this.TfsRequestContext, jobId);
      if (foundationJobDefinition == null || !string.Equals(foundationJobDefinition.ExtensionName, "Microsoft.VisualStudio.Services.Gallery.Extensions.JobAgentPlugins.PublishExtensionJob", StringComparison.OrdinalIgnoreCase))
        throw new HttpException(404, WACommonResources.PageNotFound);
      return this.GetOperation(this.TfsRequestContext.To(TeamFoundationHostType.Deployment).Elevate(), jobId);
    }

    protected virtual Operation GetOperation(IVssRequestContext deploymentContext, Guid jobId) => JobOperationsUtility.GetOperation(deploymentContext, jobId);
  }
}
