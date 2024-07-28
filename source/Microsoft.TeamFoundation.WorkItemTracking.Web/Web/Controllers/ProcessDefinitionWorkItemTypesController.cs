// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessDefinitionWorkItemTypesController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common.ExpandResults;
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
  [VersionedApiControllerCustomName(Area = "processDefinitions", ResourceName = "workItemTypes", ResourceVersion = 1)]
  public class ProcessDefinitionWorkItemTypesController : WorkItemTrackingApiController
  {
    private const string c_expandStates = "states";

    [HttpPost]
    [ClientResponseType(typeof (WorkItemTypeBehavior), null, null)]
    [ClientLocationId("921DFB88-EF57-4C69-94E5-DD7DA2D7031D")]
    public HttpResponseMessage AddBehaviorToWorkItemType(
      Guid processId,
      string witRefNameForBehaviors,
      [FromBody] WorkItemTypeBehavior behavior)
    {
      ProcessDefinitionWorkItemTypesController.ValidateCommonParameters(processId, witRefNameForBehaviors);
      if (behavior == null)
        throw new VssPropertyValidationException(nameof (behavior), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullFieldObject());
      string behaviorReferenceName = behavior.Behavior?.Id?.Trim();
      if (string.IsNullOrEmpty(behaviorReferenceName))
        throw new VssPropertyValidationException(nameof (behavior), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingFieldParameter((object) "Id"));
      ProcessWorkItemType workItemType = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().AddBehaviorToWorkItemType(this.TfsRequestContext, processId, witRefNameForBehaviors, behaviorReferenceName, behavior.IsDefault);
      BehaviorRelation behaviorRelation = workItemType.BehaviorRelations.First<BehaviorRelation>((Func<BehaviorRelation, bool>) (b => TFStringComparer.BehaviorReferenceName.Equals(behavior.Behavior.Id, b.Behavior.ReferenceName)));
      return this.Request.CreateResponse<WorkItemTypeBehavior>(HttpStatusCode.OK, WorkItemTypeBehaviorModelFactory.Create(this.TfsRequestContext, workItemType.ProcessId, workItemType.ReferenceName, behaviorRelation));
    }

    [HttpPatch]
    [ClientResponseType(typeof (WorkItemTypeBehavior), null, null)]
    [ClientLocationId("921DFB88-EF57-4C69-94E5-DD7DA2D7031D")]
    public HttpResponseMessage UpdateBehaviorToWorkItemType(
      Guid processId,
      string witRefNameForBehaviors,
      [FromBody] WorkItemTypeBehavior behavior)
    {
      ProcessDefinitionWorkItemTypesController.ValidateCommonParameters(processId, witRefNameForBehaviors);
      if (behavior == null)
        throw new VssPropertyValidationException(nameof (behavior), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullFieldObject());
      string behaviorReferenceName = behavior.Behavior?.Id?.Trim();
      if (string.IsNullOrEmpty(behaviorReferenceName))
        throw new VssPropertyValidationException(nameof (behavior), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingFieldParameter((object) "Id"));
      ProcessWorkItemType processWorkItemType = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().UpdateDefaultWorkItemTypeForBehavior(this.TfsRequestContext, processId, witRefNameForBehaviors, behaviorReferenceName, behavior.IsDefault);
      BehaviorRelation behaviorRelation = processWorkItemType.BehaviorRelations.First<BehaviorRelation>((Func<BehaviorRelation, bool>) (b => TFStringComparer.BehaviorReferenceName.Equals(behavior.Behavior.Id, b.Behavior.ReferenceName)));
      return this.Request.CreateResponse<WorkItemTypeBehavior>(HttpStatusCode.OK, WorkItemTypeBehaviorModelFactory.Create(this.TfsRequestContext, processWorkItemType.ProcessId, processWorkItemType.ReferenceName, behaviorRelation));
    }

    [HttpGet]
    [ClientResponseType(typeof (WorkItemTypeBehavior), null, null)]
    [ClientLocationId("921DFB88-EF57-4C69-94E5-DD7DA2D7031D")]
    public HttpResponseMessage GetBehaviorForWorkItemType(
      Guid processId,
      string witRefNameForBehaviors,
      string behaviorRefName)
    {
      ProcessDefinitionWorkItemTypesController.ValidateCommonParameters(processId, witRefNameForBehaviors, behaviorRefName);
      BehaviorRelation behaviorForWorkItemType = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().GetBehaviorForWorkItemType(this.TfsRequestContext, processId, witRefNameForBehaviors, behaviorRefName, true);
      return this.Request.CreateResponse<WorkItemTypeBehavior>(HttpStatusCode.OK, WorkItemTypeBehaviorModelFactory.Create(this.TfsRequestContext, processId, witRefNameForBehaviors, behaviorForWorkItemType));
    }

    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<WorkItemTypeBehavior>), null, null)]
    [ClientLocationId("921DFB88-EF57-4C69-94E5-DD7DA2D7031D")]
    public HttpResponseMessage GetBehaviorsForWorkItemType(
      Guid processId,
      string witRefNameForBehaviors)
    {
      ProcessDefinitionWorkItemTypesController.ValidateCommonParameters(processId, witRefNameForBehaviors);
      IReadOnlyCollection<BehaviorRelation> behaviorsForWorkItemType = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().GetBehaviorsForWorkItemType(this.TfsRequestContext, processId, witRefNameForBehaviors, true);
      return this.Request.CreateResponse<IEnumerable<WorkItemTypeBehavior>>(HttpStatusCode.OK, WorkItemTypeBehaviorModelFactory.Create(this.TfsRequestContext, processId, witRefNameForBehaviors, behaviorsForWorkItemType));
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("921DFB88-EF57-4C69-94E5-DD7DA2D7031D")]
    public HttpResponseMessage RemoveBehaviorFromWorkItemType(
      Guid processId,
      string witRefNameForBehaviors,
      string behaviorRefName)
    {
      ProcessDefinitionWorkItemTypesController.ValidateCommonParameters(processId, witRefNameForBehaviors, behaviorRefName);
      this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().RemoveBehaviorFromWorkItemType(this.TfsRequestContext, processId, witRefNameForBehaviors, behaviorRefName);
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
      ProcessDefinitionWorkItemTypesController.ValidateCommonParameters(processId, witRefName);
      if (string.IsNullOrEmpty(behaviorRefName))
        throw new VssPropertyValidationException(nameof (behaviorRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (behaviorRefName)));
    }

    [HttpPost]
    [ClientResponseType(typeof (WorkItemTypeModel), null, null)]
    [ClientLocationId("1CE0ACAD-4638-49C3-969C-04AA65BA6BEA")]
    public HttpResponseMessage CreateWorkItemType(Guid processId, [FromBody] WorkItemTypeModel workItemType)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (workItemType == null)
        throw new VssPropertyValidationException(nameof (workItemType), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullWorkItemTypeObject());
      IProcessWorkItemTypeService service = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>();
      ProcessWorkItemType wit;
      if (string.IsNullOrEmpty(workItemType.Inherits))
      {
        if (string.IsNullOrEmpty(workItemType.Name))
          throw new VssPropertyValidationException(nameof (workItemType), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingWorkItemTypeParameter((object) "Name"));
        if (string.IsNullOrEmpty(workItemType.Color))
          throw new VssPropertyValidationException(nameof (workItemType), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingWorkItemTypeParameter((object) "Color"));
        wit = service.CreateWorkItemType(this.TfsRequestContext, processId, workItemType.Name, workItemType.Color, workItemType.Icon, workItemType.Description, workItemType.IsDisabled, workItemType.Id);
      }
      else
      {
        if (string.IsNullOrEmpty(workItemType.Inherits))
          throw new VssPropertyValidationException(nameof (workItemType), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingWorkItemTypeParameter((object) "Inherits"));
        wit = service.CreateDerivedWorkItemType(this.TfsRequestContext, processId, workItemType.Inherits, workItemType.Description, workItemType.Color, workItemType.Icon, workItemType.IsDisabled, workItemType.Id);
      }
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("WorkItemType", "Create");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("WorkItemTypeName", (object) workItemType.Name);
      data.Add("WorkItemTypeId", (object) workItemType.Id);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse<WorkItemTypeModel>(HttpStatusCode.Created, WorkItemTypeModelFactory.Create(this.TfsRequestContext, wit));
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("1CE0ACAD-4638-49C3-969C-04AA65BA6BEA")]
    public HttpResponseMessage DeleteWorkItemType(Guid processId, string witRefName)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().DestroyWorkItemType(this.TfsRequestContext, processId, witRefName);
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("WorkItemType", "Delete");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("WorkItemTypeReferenceName", (object) witRefName);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    [HttpPatch]
    [ClientResponseType(typeof (WorkItemTypeModel), null, null)]
    [ClientLocationId("1CE0ACAD-4638-49C3-969C-04AA65BA6BEA")]
    public HttpResponseMessage UpdateWorkItemType(
      Guid processId,
      string witRefName,
      [FromBody] WorkItemTypeUpdateModel workItemTypeUpdate)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      if (workItemTypeUpdate == null)
        throw new VssPropertyValidationException(nameof (workItemTypeUpdate), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (workItemTypeUpdate)));
      ProcessWorkItemType wit = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().UpdateWorkItemType(this.TfsRequestContext, processId, witRefName, workItemTypeUpdate.Color, workItemTypeUpdate.Icon, workItemTypeUpdate.Description, workItemTypeUpdate.IsDisabled);
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("WorkItemType", "Update");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("WorkItemTypeReferenceName", (object) witRefName);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse<WorkItemTypeModel>(HttpStatusCode.OK, WorkItemTypeModelFactory.Create(this.TfsRequestContext, wit));
    }

    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<WorkItemTypeModel>), null, null)]
    [ClientLocationId("1CE0ACAD-4638-49C3-969C-04AA65BA6BEA")]
    public HttpResponseMessage GetWorkItemTypes(Guid processId, [FromUri(Name = "$expand")] GetWorkItemTypeExpand expand = GetWorkItemTypeExpand.None)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      IProcessWorkItemTypeService service = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>();
      IReadOnlyCollection<BaseWorkItemType> workItemTypes = service.GetWorkItemTypes(this.TfsRequestContext, processId, true, true);
      List<ComposedWorkItemType> source = new List<ComposedWorkItemType>();
      foreach (BaseWorkItemType baseWorkItemType in (IEnumerable<BaseWorkItemType>) workItemTypes)
      {
        ComposedWorkItemType workItemType = service.GetWorkItemType(this.TfsRequestContext, processId, baseWorkItemType.ReferenceName, true);
        source.Add(workItemType);
      }
      return this.Request.CreateResponse<IEnumerable<WorkItemTypeModel>>(HttpStatusCode.OK, source.Select<ComposedWorkItemType, WorkItemTypeModel>((Func<ComposedWorkItemType, WorkItemTypeModel>) (wit =>
      {
        GetWorkItemTypeExpandResult typeExpandResult = new GetWorkItemTypeExpandResult(expand, this.TfsRequestContext, wit);
        return WorkItemTypeModelFactory.Create(this.TfsRequestContext, processId, wit, typeExpandResult.States, typeExpandResult.Behaviors, typeExpandResult.Layout);
      })));
    }

    [HttpGet]
    [ClientResponseType(typeof (WorkItemTypeModel), null, null)]
    [ClientLocationId("1CE0ACAD-4638-49C3-969C-04AA65BA6BEA")]
    public HttpResponseMessage GetWorkItemType(
      Guid processId,
      string witRefName,
      [FromUri(Name = "$expand")] GetWorkItemTypeExpand expand = GetWorkItemTypeExpand.None)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      ComposedWorkItemType workItemType = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().GetWorkItemType(this.TfsRequestContext, processId, witRefName, true, true);
      GetWorkItemTypeExpandResult typeExpandResult = new GetWorkItemTypeExpandResult(expand, this.TfsRequestContext, workItemType);
      return this.Request.CreateResponse<WorkItemTypeModel>(HttpStatusCode.OK, WorkItemTypeModelFactory.Create(this.TfsRequestContext, processId, workItemType, typeExpandResult.States, typeExpandResult.Behaviors, typeExpandResult.Layout));
    }
  }
}
