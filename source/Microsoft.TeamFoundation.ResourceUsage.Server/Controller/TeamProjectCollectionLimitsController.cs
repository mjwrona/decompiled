// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ResourceUsage.Server.Controller.TeamProjectCollectionLimitsController
// Assembly: Microsoft.TeamFoundation.ResourceUsage.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55568389-A340-4F60-8DD1-887E0E3F1980
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.ResourceUsage.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.ResourceUsage.Server.Service;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.ResourceUsage.Server.Controller
{
  [VersionedApiControllerCustomName("ResourceUsage", "TeamProjectCollection", 1)]
  public sealed class TeamProjectCollectionLimitsController : TfsApiController
  {
    private static readonly string s_layer = nameof (TeamProjectCollectionLimitsController);

    public override string TraceArea => "ResourceUsage";

    public string Layer => TeamProjectCollectionLimitsController.s_layer;

    [TraceFilter(94000001, 94000010)]
    [HttpGet]
    public Dictionary<string, Usage> GetLimits()
    {
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      this.TfsRequestContext.TraceEnter(94000001, this.TraceArea, this.Layer, nameof (GetLimits));
      try
      {
        Dictionary<string, Usage> limits = (Dictionary<string, Usage>) null;
        if (this.TfsRequestContext.IsFeatureEnabled("ResourceUsage.Service.TeamProjectCollection"))
          limits = this.TfsRequestContext.GetService<IResourceUsageService>().GetLimits(this.TfsRequestContext);
        return limits;
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(94000051, this.TraceArea, this.Layer, ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(94000051, this.TraceArea, this.Layer, nameof (GetLimits));
      }
    }
  }
}
