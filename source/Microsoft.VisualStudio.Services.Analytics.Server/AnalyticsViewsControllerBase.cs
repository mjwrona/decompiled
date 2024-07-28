// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsViewsControllerBase
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Analytics.Views.Security;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using System.Net;

namespace Microsoft.VisualStudio.Services.Analytics
{
  [ValidateViewAnalyticsPermission]
  [ValidateExecAnalyticsViewsPermission]
  [ValidateAnalyticsEnabledAndModelReadyAttribute]
  public abstract class AnalyticsViewsControllerBase : TfsProjectApiController
  {
    public override string ActivityLogArea => "AnalyticsViews";

    protected AnalyticsViewScope GetViewScope() => new AnalyticsViewScope()
    {
      Id = this.ProjectId,
      Name = this.ProjectInfo.Name,
      Type = AnalyticsViewScopeType.Project
    };

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<AnalyticsAccessCheckException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<AnalyticsNotEnabledException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ModelNotReadyException>(HttpStatusCode.ServiceUnavailable);
      exceptionMap.AddStatusCode<ModelSyncingException>(HttpStatusCode.ServiceUnavailable);
    }
  }
}
