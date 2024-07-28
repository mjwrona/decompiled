// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ResourceUsage.Server.Controller.ProjectLimitController
// Assembly: Microsoft.TeamFoundation.ResourceUsage.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55568389-A340-4F60-8DD1-887E0E3F1980
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.ResourceUsage.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.ResourceUsage.Server.Service;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.ResourceUsage.Server.Controller
{
  [VersionedApiControllerCustomName("ResourceUsage", "Project", 1)]
  public class ProjectLimitController : TfsProjectApiController
  {
    private static readonly string s_layer = nameof (ProjectLimitController);

    public override string TraceArea => "ResourceUsage";

    protected string Layer => ProjectLimitController.s_layer;

    [TraceFilter(94000011, 94000020)]
    [HttpGet]
    public Dictionary<string, Usage> GetProjectLimit()
    {
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      this.TfsRequestContext.TraceEnter(94000011, this.TraceArea, ProjectLimitController.s_layer, nameof (GetProjectLimit));
      try
      {
        Dictionary<string, Usage> projectLimit = (Dictionary<string, Usage>) null;
        if (this.TfsRequestContext.IsFeatureEnabled("ResourceUsage.Service.Project"))
          projectLimit = this.TfsRequestContext.GetService<IResourceUsageService>().GetProjectLimits(this.TfsRequestContext, this.ProjectId);
        return projectLimit;
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(94000012, this.TraceArea, ProjectLimitController.s_layer, ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(94000012, this.TraceArea, ProjectLimitController.s_layer, nameof (GetProjectLimit));
      }
    }
  }
}
