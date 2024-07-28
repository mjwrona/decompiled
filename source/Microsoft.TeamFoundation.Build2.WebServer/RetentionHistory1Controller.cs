// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.RetentionHistory1Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(6.1)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "history", ResourceVersion = 1)]
  public class RetentionHistory1Controller : BuildApiController
  {
    [HttpGet]
    public virtual BuildRetentionHistory GetRetentionHistory([ClientQueryParameter] int daysToLookback = 30)
    {
      if (!this.TfsRequestContext.IsFeatureEnabled("Build2.BlockGetRetentionHistoryAPICalls"))
        return new BuildRetentionHistory(this.BuildService.GetRetentionHistory(this.TfsRequestContext, daysToLookback));
      throw new RestApiToBeDeprecatedException(Resources.RestApiToBeDeprecated());
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<RestApiToBeDeprecatedException>(HttpStatusCode.NotFound);
    }
  }
}
