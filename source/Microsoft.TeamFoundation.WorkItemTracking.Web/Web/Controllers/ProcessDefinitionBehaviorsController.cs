// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessDefinitionBehaviorsController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ProcessDefinitions.Factories;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [FeatureEnabled("WebAccess.Process.Hierarchy")]
  [VersionedApiControllerCustomName(Area = "processDefinitions", ResourceName = "behaviors", ResourceVersion = 1)]
  public class ProcessDefinitionBehaviorsController : WorkItemTrackingApiController
  {
    [HttpPost]
    [ClientResponseType(typeof (BehaviorModel), null, null)]
    [ClientLocationId("47A651F4-FB70-43BF-B96B-7C0BA947142B")]
    public HttpResponseMessage CreateBehavior(Guid processId, BehaviorCreateModel behavior)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (behavior == null)
        throw new VssPropertyValidationException(nameof (behavior), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (behavior)));
      Behavior behavior1 = this.TfsRequestContext.GetService<IBehaviorService>().CreateBehavior(this.TfsRequestContext, processId, behavior.Inherits, behavior.Name, behavior.Color, behavior.Id);
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Behavior", "Create");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("BehaviorName", (object) behavior.Name);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse<BehaviorModel>(HttpStatusCode.Created, BehaviorModelFactory.Create(this.TfsRequestContext, processId, behavior1));
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("47A651F4-FB70-43BF-B96B-7C0BA947142B")]
    public HttpResponseMessage DeleteBehavior(Guid processId, string behaviorId)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(behaviorId))
        throw new VssPropertyValidationException(nameof (behaviorId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (behaviorId)));
      IBehaviorService service = this.TfsRequestContext.GetService<IBehaviorService>();
      Behavior behavior = service.GetBehavior(this.TfsRequestContext, processId, behaviorId);
      service.DeleteBehavior(this.TfsRequestContext, processId, behaviorId);
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Behavior", "Delete");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("BehaviorName", (object) behavior.Name);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    [HttpPut]
    [ClientResponseType(typeof (BehaviorModel), null, null)]
    [ClientLocationId("47A651F4-FB70-43BF-B96B-7C0BA947142B")]
    public HttpResponseMessage ReplaceBehavior(
      Guid processId,
      string behaviorId,
      [FromBody] BehaviorReplaceModel behaviorData)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(behaviorId))
        throw new VssPropertyValidationException(nameof (behaviorId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (behaviorId)));
      if (behaviorData == null)
        throw new VssPropertyValidationException(nameof (behaviorData), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (behaviorData)));
      Behavior serverBehavior = this.TfsRequestContext.GetService<IBehaviorService>().UpdateBehavior(this.TfsRequestContext, processId, behaviorId, behaviorData.Name, behaviorData.Color);
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Behavior", "Edit");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("BehaviorName", (object) behaviorData.Name);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse<BehaviorModel>(HttpStatusCode.OK, BehaviorModelFactory.Create(this.TfsRequestContext, processId, serverBehavior));
    }

    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<BehaviorModel>), null, null)]
    [ClientLocationId("47A651F4-FB70-43BF-B96B-7C0BA947142B")]
    public HttpResponseMessage GetBehaviors(Guid processId)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      return this.Request.CreateResponse<IEnumerable<BehaviorModel>>(HttpStatusCode.OK, this.TfsRequestContext.GetService<IBehaviorService>().GetBehaviors(this.TfsRequestContext, processId, false, true).Select<Behavior, BehaviorModel>((Func<Behavior, BehaviorModel>) (behavior => BehaviorModelFactory.Create(this.TfsRequestContext, processId, behavior))));
    }

    [HttpGet]
    [ClientResponseType(typeof (BehaviorModel), null, null)]
    [ClientLocationId("47A651F4-FB70-43BF-B96B-7C0BA947142B")]
    public HttpResponseMessage GetBehavior(Guid processId, string behaviorId)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(behaviorId))
        throw new VssPropertyValidationException(nameof (behaviorId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (behaviorId)));
      Behavior behavior = this.TfsRequestContext.GetService<IBehaviorService>().GetBehavior(this.TfsRequestContext, processId, behaviorId, false, true);
      return this.Request.CreateResponse<BehaviorModel>(HttpStatusCode.OK, BehaviorModelFactory.Create(this.TfsRequestContext, processId, behavior));
    }
  }
}
