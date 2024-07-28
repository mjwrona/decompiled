// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessWorkItemTypesBehaviorsController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories;
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
  [VersionedApiControllerCustomName(Area = "processes", ResourceName = "workItemTypesBehaviors", ResourceVersion = 1)]
  public class ProcessWorkItemTypesBehaviorsController : WorkItemTrackingApiController
  {
    [HttpPost]
    [ClientResponseType(typeof (WorkItemTypeBehavior), null, null)]
    [ClientLocationId("6D765A2E-4E1B-4B11-BE93-F953BE676024")]
    [ClientExample("PATCH__wit_behavior.json", "Add behavior to work item type", null, null)]
    public HttpResponseMessage AddBehaviorToWorkItemType(
      Guid processId,
      string witRefNameForBehaviors,
      [FromBody] WorkItemTypeBehavior behavior)
    {
      ProcessWorkItemTypesBehaviorsController.ValidateCommonParameters(processId, witRefNameForBehaviors);
      if (behavior == null)
        throw new VssPropertyValidationException(nameof (behavior), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullFieldObject());
      string behaviorReferenceName = behavior.Behavior?.Id?.Trim();
      if (string.IsNullOrEmpty(behaviorReferenceName))
        throw new VssPropertyValidationException(nameof (behavior), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingFieldParameter((object) "Id"));
      Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessWorkItemType workItemType = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().AddBehaviorToWorkItemType(this.TfsRequestContext, processId, witRefNameForBehaviors, behaviorReferenceName, behavior.IsDefault);
      BehaviorRelation behaviorRelation = workItemType.BehaviorRelations.First<BehaviorRelation>((Func<BehaviorRelation, bool>) (b => TFStringComparer.BehaviorReferenceName.Equals(behavior.Behavior.Id, b.Behavior.ReferenceName)));
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Behavior", "Add");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("WorkItemTypeReferenceName", (object) witRefNameForBehaviors);
      data.Add("BehaviorName", (object) behaviorRelation?.Behavior?.Name);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse<WorkItemTypeBehavior>(HttpStatusCode.OK, WorkItemTypeBehaviorModelFactory.Create(this.TfsRequestContext, workItemType.ProcessId, workItemType.ReferenceName, behaviorRelation));
    }

    [HttpPatch]
    [ClientResponseType(typeof (WorkItemTypeBehavior), null, null)]
    [ClientLocationId("6D765A2E-4E1B-4B11-BE93-F953BE676024")]
    [ClientExample("PATCH__wit_behavior.json", "Update work item type behavior", null, null)]
    public HttpResponseMessage UpdateBehaviorToWorkItemType(
      Guid processId,
      string witRefNameForBehaviors,
      [FromBody] WorkItemTypeBehavior behavior)
    {
      ProcessWorkItemTypesBehaviorsController.ValidateCommonParameters(processId, witRefNameForBehaviors);
      if (behavior == null)
        throw new VssPropertyValidationException(nameof (behavior), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullFieldObject());
      string behaviorReferenceName = behavior.Behavior?.Id?.Trim();
      if (string.IsNullOrEmpty(behaviorReferenceName))
        throw new VssPropertyValidationException(nameof (behavior), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingFieldParameter((object) "Id"));
      Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessWorkItemType processWorkItemType = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().UpdateDefaultWorkItemTypeForBehavior(this.TfsRequestContext, processId, witRefNameForBehaviors, behaviorReferenceName, behavior.IsDefault);
      BehaviorRelation behaviorRelation = processWorkItemType.BehaviorRelations.First<BehaviorRelation>((Func<BehaviorRelation, bool>) (b => TFStringComparer.BehaviorReferenceName.Equals(behavior.Behavior.Id, b.Behavior.ReferenceName)));
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Behavior", "Update");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("WorkItemTypeReferenceName", (object) witRefNameForBehaviors);
      data.Add("BehaviorName", (object) behaviorRelation?.Behavior?.Name);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse<WorkItemTypeBehavior>(HttpStatusCode.OK, WorkItemTypeBehaviorModelFactory.Create(this.TfsRequestContext, processWorkItemType.ProcessId, processWorkItemType.ReferenceName, behaviorRelation));
    }

    [HttpGet]
    [ClientResponseType(typeof (WorkItemTypeBehavior), null, null)]
    [ClientLocationId("6D765A2E-4E1B-4B11-BE93-F953BE676024")]
    [ClientExample("GET__wit_behavior.json", "Get behavior for work item type", null, null)]
    public HttpResponseMessage GetBehaviorForWorkItemType(
      Guid processId,
      string witRefNameForBehaviors,
      string behaviorRefName)
    {
      ProcessWorkItemTypesBehaviorsController.ValidateCommonParameters(processId, witRefNameForBehaviors, behaviorRefName);
      BehaviorRelation behaviorForWorkItemType = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().GetBehaviorForWorkItemType(this.TfsRequestContext, processId, witRefNameForBehaviors, behaviorRefName, true);
      return this.Request.CreateResponse<WorkItemTypeBehavior>(HttpStatusCode.OK, WorkItemTypeBehaviorModelFactory.Create(this.TfsRequestContext, processId, witRefNameForBehaviors, behaviorForWorkItemType));
    }

    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<WorkItemTypeBehavior>), null, null)]
    [ClientLocationId("6D765A2E-4E1B-4B11-BE93-F953BE676024")]
    [ClientExample("GET__wit_behaviors.json", "Get behaviors for work item type", null, null)]
    public HttpResponseMessage GetBehaviorsForWorkItemType(
      Guid processId,
      string witRefNameForBehaviors)
    {
      ProcessWorkItemTypesBehaviorsController.ValidateCommonParameters(processId, witRefNameForBehaviors);
      IReadOnlyCollection<BehaviorRelation> behaviorsForWorkItemType = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().GetBehaviorsForWorkItemType(this.TfsRequestContext, processId, witRefNameForBehaviors, true);
      return this.Request.CreateResponse<IEnumerable<WorkItemTypeBehavior>>(HttpStatusCode.OK, WorkItemTypeBehaviorModelFactory.Create(this.TfsRequestContext, processId, witRefNameForBehaviors, behaviorsForWorkItemType));
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("6D765A2E-4E1B-4B11-BE93-F953BE676024")]
    [ClientExample("DELETE__wit_behavior.json", "Remove behaviors for work item type", null, null)]
    public HttpResponseMessage RemoveBehaviorFromWorkItemType(
      Guid processId,
      string witRefNameForBehaviors,
      string behaviorRefName)
    {
      ProcessWorkItemTypesBehaviorsController.ValidateCommonParameters(processId, witRefNameForBehaviors, behaviorRefName);
      Behavior behavior = this.TfsRequestContext.GetService<IBehaviorService>().GetBehavior(this.TfsRequestContext, processId, behaviorRefName);
      this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().RemoveBehaviorFromWorkItemType(this.TfsRequestContext, processId, witRefNameForBehaviors, behaviorRefName);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Behavior", "Remove");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("WorkItemTypeReferenceName", (object) witRefNameForBehaviors);
      data.Add("BehaviorReferenceName", (object) behavior.Name);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    private static void ValidateCommonParameters(Guid processId, string witRefName)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
    }

    private static void ValidateCommonParameters(
      Guid processId,
      string witRefName,
      string behaviorRefName)
    {
      ProcessWorkItemTypesBehaviorsController.ValidateCommonParameters(processId, witRefName);
      if (string.IsNullOrEmpty(behaviorRefName))
        throw new VssPropertyValidationException(nameof (behaviorRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (behaviorRefName)));
    }
  }
}
