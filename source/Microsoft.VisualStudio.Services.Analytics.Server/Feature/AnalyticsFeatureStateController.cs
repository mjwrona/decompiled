// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Feature.AnalyticsFeatureStateController
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.OnPrem;
using Microsoft.VisualStudio.Services.Analytics.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Analytics.Feature
{
  [VersionedApiControllerCustomName(Area = "Analytics", ResourceName = "State")]
  public class AnalyticsFeatureStateController : TfsApiController
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<AnalyticsStateInvalidTransitionException>(HttpStatusCode.BadRequest);
    }

    [HttpGet]
    public AnalyticsStateDetails GetState()
    {
      AnalyticsStateDetails state = new AnalyticsStateDetails();
      if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException("Get analytics state is invalid on hosted.");
      IAnalyticsStateService service = this.TfsRequestContext.GetService<IAnalyticsStateService>();
      state.AnalyticsState = new AnalyticsState?(service.GetState(this.TfsRequestContext));
      state.ChangedDate = new DateTimeOffset?(service.GetChangedDate(this.TfsRequestContext));
      return state;
    }

    [HttpPatch]
    [ValidateRequestFromCollectionAdmin]
    [ClientInclude(~RestClientLanguages.Go)]
    public void UpdateState(AnalyticsState state)
    {
      if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException("Analytics state cannot be changed on hosted.");
      IAnalyticsStateService service = this.TfsRequestContext.GetService<IAnalyticsStateService>();
      switch (state)
      {
        case AnalyticsState.Disabled:
          service.Delete(this.TfsRequestContext);
          break;
        case AnalyticsState.Enabled:
          service.Enable(this.TfsRequestContext);
          break;
        case AnalyticsState.Paused:
          service.Pause(this.TfsRequestContext);
          break;
        case AnalyticsState.Deleting:
          throw new AnalyticsStateInvalidTransitionException("Cannot manually enter Deleting state.");
        case AnalyticsState.Preparing:
          throw new AnalyticsStateInvalidTransitionException("Cannot manually enter Preparing state.");
        default:
          throw new AnalyticsStateInvalidStateException("Requested state is not recognized.");
      }
    }
  }
}
