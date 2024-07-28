// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Feature.AnalyticsSpaceRequirementsController
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.OnPrem;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Analytics.Feature
{
  [VersionedApiControllerCustomName(Area = "Analytics", ResourceName = "SpaceRequirements")]
  public class AnalyticsSpaceRequirementsController : TfsApiController
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap) => base.InitializeExceptionMap(exceptionMap);

    [HttpGet]
    [ClientInclude(~RestClientLanguages.Go)]
    public int GetSpaceRequirements()
    {
      if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException("Get analytics space estimate is invalid on hosted.");
      return this.TfsRequestContext.GetService<AnalyticsService>().GetSpaceRequirements(this.TfsRequestContext).Where<SpaceRequirements>((Func<SpaceRequirements, bool>) (t => t.CurrentInMB.HasValue)).Select<SpaceRequirements, int>((Func<SpaceRequirements, int>) (t => t.CurrentInMB.Value)).Sum();
    }
  }
}
