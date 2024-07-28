// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessBehaviors2Controller
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "processes", ResourceName = "behaviors", ResourceVersion = 2)]
  [ControllerApiVersion(5.0)]
  [FeatureEnabled("WebAccess.Process.Hierarchy")]
  public class ProcessBehaviors2Controller : WorkItemTrackingApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (ProcessBehavior), null, null)]
    [ClientExample("GET__process_behavior.json", "Get the process behavior", null, null)]
    [ClientExample("GET__process_behavior_fields.json", "Get the process behavior with Fields option", null, null)]
    [ClientExample("GET__process_behavior_combined_fields.json", "Get the process behavior with CombinedFields option", null, null)]
    [ClientLocationId("D1800200-F184-4E75-A5F2-AD0B04B4373E")]
    public HttpResponseMessage GetProcessBehavior(
      Guid processId,
      string behaviorRefName,
      [FromUri(Name = "$expand")] GetBehaviorsExpand expand = GetBehaviorsExpand.None)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(behaviorRefName) || string.IsNullOrWhiteSpace(behaviorRefName))
        throw new VssPropertyValidationException(nameof (behaviorRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (behaviorRefName)));
      Behavior behavior = this.TfsRequestContext.GetService<IBehaviorService>().GetBehavior(this.TfsRequestContext, processId, behaviorRefName);
      return this.Request.CreateResponse<ProcessBehavior>(HttpStatusCode.OK, ProcessBehaviorFactory.Create(this.TfsRequestContext, processId, behavior, expand));
    }

    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<ProcessBehavior>), null, null)]
    [ClientLocationId("D1800200-F184-4E75-A5F2-AD0B04B4373E")]
    [ClientExample("GET__process_behavior_list.json", "Get the list of process behaviors", null, null)]
    [ClientExample("GET__process_behavior_list_fields.json", "Get the list of process behaviors with Fields option", null, null)]
    [ClientExample("GET__process_behavior_list_combined_fields.json", "Get the list of process behaviors with CombinedFields option", null, null)]
    public HttpResponseMessage GetProcessBehaviors(Guid processId, [FromUri(Name = "$expand")] GetBehaviorsExpand expand = GetBehaviorsExpand.None)
    {
      IReadOnlyCollection<Behavior> behaviors = !(processId == Guid.Empty) ? this.TfsRequestContext.GetService<IBehaviorService>().GetBehaviors(this.TfsRequestContext, processId) : throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      return this.Request.CreateResponse<IEnumerable<ProcessBehavior>>(HttpStatusCode.OK, ProcessBehaviorFactory.Create(this.TfsRequestContext, processId, (IEnumerable<Behavior>) behaviors, expand));
    }

    [HttpPost]
    [ClientResponseType(typeof (ProcessBehavior), null, null)]
    [ClientLocationId("D1800200-F184-4E75-A5F2-AD0B04B4373E")]
    [ClientExample("POST__process_behavior.json", "Create a process behavior", null, null)]
    public HttpResponseMessage CreateProcessBehavior(
      Guid processId,
      ProcessBehaviorCreateRequest behavior)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (behavior == null)
        throw new VssPropertyValidationException(nameof (behavior), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (behavior)));
      IBehaviorService service = this.TfsRequestContext.GetService<IBehaviorService>();
      Behavior behavior1 = service.CreateBehavior(this.TfsRequestContext, processId, behavior.Inherits, behavior.Name, behavior.Color, behavior.ReferenceName);
      Behavior behavior2 = service.GetBehavior(this.TfsRequestContext, processId, behavior1.ReferenceName, bypassCache: true);
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Behavior", "Create");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("BehaviorName", (object) behavior.Name);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse<ProcessBehavior>(HttpStatusCode.Created, ProcessBehaviorFactory.Create(this.TfsRequestContext, processId, behavior2, GetBehaviorsExpand.CombinedFields));
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("D1800200-F184-4E75-A5F2-AD0B04B4373E")]
    [ClientExample("DELETE__process_behavior.json", "Delete a process behavior", null, null)]
    public HttpResponseMessage DeleteProcessBehavior(Guid processId, string behaviorRefName)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(behaviorRefName) || string.IsNullOrWhiteSpace(behaviorRefName))
        throw new VssPropertyValidationException(nameof (behaviorRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (behaviorRefName)));
      IBehaviorService service = this.TfsRequestContext.GetService<IBehaviorService>();
      Behavior behavior = service.GetBehavior(this.TfsRequestContext, processId, behaviorRefName);
      service.DeleteBehavior(this.TfsRequestContext, processId, behaviorRefName);
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
    [ClientResponseType(typeof (ProcessBehavior), null, null)]
    [ClientLocationId("D1800200-F184-4E75-A5F2-AD0B04B4373E")]
    [ClientExample("PUT__process_behavior.json", "Update a process behavior", null, null)]
    public HttpResponseMessage UpdateProcessBehavior(
      Guid processId,
      string behaviorRefName,
      [FromBody] ProcessBehaviorUpdateRequest behaviorData)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(behaviorRefName) || string.IsNullOrWhiteSpace(behaviorRefName))
        throw new VssPropertyValidationException(nameof (behaviorRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (behaviorRefName)));
      if (behaviorData == null)
        throw new VssPropertyValidationException(nameof (behaviorData), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (behaviorData)));
      IBehaviorService service = this.TfsRequestContext.GetService<IBehaviorService>();
      Behavior behavior1 = service.UpdateBehavior(this.TfsRequestContext, processId, behaviorRefName, behaviorData.Name, behaviorData.Color);
      Behavior behavior2 = service.GetBehavior(this.TfsRequestContext, processId, behavior1.ReferenceName, bypassCache: true);
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Behavior", "Edit");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("BehaviorName", (object) behaviorData.Name);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse<ProcessBehavior>(HttpStatusCode.OK, ProcessBehaviorFactory.Create(this.TfsRequestContext, processId, behavior2, GetBehaviorsExpand.CombinedFields));
    }
  }
}
