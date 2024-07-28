// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessStatesController
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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [FeatureEnabled("WebAccess.Process.Hierarchy")]
  [VersionedApiControllerCustomName(Area = "processes", ResourceName = "states", ResourceVersion = 1)]
  public class ProcessStatesController : WorkItemTrackingApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<WorkItemStateResultModel>), null, null)]
    [ClientLocationId("31015D57-2DFF-4A46-ADB3-2FB4EE3DCEC9")]
    [ClientExample("GET__wit_states.json", "Get state definitions", null, null)]
    public HttpResponseMessage GetStateDefinitions(Guid processId, string witRefName)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      return this.Request.CreateResponse<IEnumerable<WorkItemStateResultModel>>(HttpStatusCode.OK, this.GetStateDefinitions(this.TfsRequestContext, processId, witRefName).Select<WorkItemStateDefinition, WorkItemStateResultModel>((Func<WorkItemStateDefinition, WorkItemStateResultModel>) (s => WorkItemStateResultModelFactory.Create(this.TfsRequestContext, processId, witRefName, s).ToProcessModel())));
    }

    [HttpGet]
    [ClientResponseType(typeof (WorkItemStateResultModel), null, null)]
    [ClientLocationId("31015D57-2DFF-4A46-ADB3-2FB4EE3DCEC9")]
    [ClientExample("GET__wit_state.json", "Get state definition", null, null)]
    public HttpResponseMessage GetStateDefinition(Guid processId, string witRefName, Guid stateId)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      if (stateId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (stateId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (stateId)));
      bool apiForDerivedWit = this.ReturnsSystemStatesInGetStateApiForDerivedWIT();
      WorkItemStateDefinition stateDefinition = this.TfsRequestContext.GetService<IWorkItemStateDefinitionService>().GetStateDefinition(this.TfsRequestContext, processId, witRefName, stateId, apiForDerivedWit);
      return this.Request.CreateResponse<WorkItemStateResultModel>(HttpStatusCode.OK, WorkItemStateResultModelFactory.Create(this.TfsRequestContext, processId, witRefName, stateDefinition).ToProcessModel());
    }

    [HttpPost]
    [ClientResponseType(typeof (WorkItemStateResultModel), null, null)]
    [ClientLocationId("31015D57-2DFF-4A46-ADB3-2FB4EE3DCEC9")]
    [ClientExample("CREATE__wit_state.json", "Create state definition", null, null)]
    public HttpResponseMessage CreateStateDefinition(
      Guid processId,
      string witRefName,
      [FromBody] WorkItemStateInputModel stateModel)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      if (stateModel == null)
        throw new VssPropertyValidationException(nameof (stateModel), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (stateModel)));
      if (string.IsNullOrWhiteSpace(stateModel.Name))
        throw new VssPropertyValidationException("Name", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) "Name"));
      if (string.IsNullOrWhiteSpace(stateModel.Color))
        throw new VssPropertyValidationException("Color", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) "Color"));
      if (string.IsNullOrWhiteSpace(stateModel.StateCategory))
        throw new VssPropertyValidationException("StateCategory", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) "StateCategory"));
      IWorkItemStateDefinitionService service = this.TfsRequestContext.GetService<IWorkItemStateDefinitionService>();
      WorkItemStateDefinition itemStateDefinition = new WorkItemStateDefinition();
      WorkItemStateDefinition stateDefinition = !(stateModel.StateCategory == "Completed") ? service.CreateStateDefinition(this.TfsRequestContext, processId, witRefName, stateModel.ToWorkItemStateDeclaration()) : service.ReplaceCompletedStateDefinition(this.TfsRequestContext, processId, witRefName, stateModel.ToWorkItemStateDeclaration());
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("State", "Create");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("WorkItemTypeReferenceName", (object) witRefName);
      data.Add("StateName", (object) stateModel.Name);
      data.Add("StateCategory", (object) stateModel.StateCategory);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse<WorkItemStateResultModel>(HttpStatusCode.Created, WorkItemStateResultModelFactory.Create(this.TfsRequestContext, processId, witRefName, stateDefinition));
    }

    [HttpDelete]
    [ClientLocationId("31015D57-2DFF-4A46-ADB3-2FB4EE3DCEC9")]
    [ClientExample("DELETE__wit_state.json", "Delete state definition", null, null)]
    public void DeleteStateDefinition(Guid processId, string witRefName, Guid stateId)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      if (stateId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (stateId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (stateId)));
      IWorkItemStateDefinitionService service = this.TfsRequestContext.GetService<IWorkItemStateDefinitionService>();
      try
      {
        WorkItemStateDefinition stateDefinition = service.GetStateDefinition(this.TfsRequestContext, processId, witRefName, stateId);
        service.DeleteStateDefinition(this.TfsRequestContext, processId, witRefName, stateId);
        ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        string actionId = ProcessAuditConstants.GetActionId("State", "Delete");
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("ProcessName", (object) processDescriptor?.Name);
        data.Add("WorkItemTypeReferenceName", (object) witRefName);
        data.Add("StateName", (object) stateDefinition.Name);
        data.Add("StateCategory", (object) stateDefinition.StateCategory);
        Guid targetHostId = new Guid();
        Guid projectId = new Guid();
        tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      }
      catch (WorkItemStateDefinitionNotFoundException ex)
      {
      }
    }

    [HttpPatch]
    [ClientResponseType(typeof (WorkItemStateResultModel), null, null)]
    [ClientLocationId("31015D57-2DFF-4A46-ADB3-2FB4EE3DCEC9")]
    [ClientExample("UPDATE__wit_state.json", "Update state definition", null, null)]
    public HttpResponseMessage UpdateStateDefinition(
      Guid processId,
      string witRefName,
      Guid stateId,
      [FromBody] WorkItemStateInputModel stateModel)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      if (stateId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (stateId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (stateId)));
      if (stateModel == null)
        throw new VssPropertyValidationException(nameof (stateModel), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (stateModel)));
      IWorkItemStateDefinitionService service = this.TfsRequestContext.GetService<IWorkItemStateDefinitionService>();
      WorkItemStateCategory? stateCategory = WorkItemStateInputModelFactory.ConvertStateCategoryStringToEnum(stateModel.StateCategory);
      WorkItemStateDefinition stateDefinition = service.UpdateStateDefinition(this.TfsRequestContext, processId, witRefName, stateId, stateModel.Color, stateCategory, stateModel.Order);
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      string name = stateModel.Name;
      if (string.IsNullOrEmpty(name))
        name = service.GetStateDefinition(this.TfsRequestContext, processId, witRefName, stateId).Name;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("State", "Update");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("WorkItemTypeReferenceName", (object) witRefName);
      data.Add("StateName", (object) name);
      data.Add("StateCategory", (object) stateModel.StateCategory);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse<WorkItemStateResultModel>(HttpStatusCode.OK, WorkItemStateResultModelFactory.Create(this.TfsRequestContext, processId, witRefName, stateDefinition));
    }

    [HttpPut]
    [ClientResponseType(typeof (WorkItemStateResultModel), null, null)]
    [ClientLocationId("31015D57-2DFF-4A46-ADB3-2FB4EE3DCEC9")]
    [ClientExample("HIDE__wit_state.json", "Hide state definition", null, null)]
    public HttpResponseMessage HideStateDefinition(
      Guid processId,
      string witRefName,
      Guid stateId,
      [FromBody] HideStateModel hideStateModel)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      if (stateId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (stateId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (stateId)));
      if (!hideStateModel.Hidden)
        throw new VssPropertyValidationException(nameof (hideStateModel), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.UnhideStateUsingDeleteState());
      WorkItemStateDefinition stateDefinition = this.TfsRequestContext.GetService<IWorkItemStateDefinitionService>().HideStateDefinition(this.TfsRequestContext, processId, witRefName, stateId);
      return this.Request.CreateResponse<WorkItemStateResultModel>(HttpStatusCode.OK, WorkItemStateResultModelFactory.Create(this.TfsRequestContext, processId, witRefName, stateDefinition));
    }

    internal IEnumerable<WorkItemStateDefinition> GetStateDefinitions(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName)
    {
      IReadOnlyCollection<WorkItemStateDefinition> stateDefinitions = this.TfsRequestContext.GetService<IWorkItemStateDefinitionService>().GetStateDefinitions(this.TfsRequestContext, processId, witRefName, true);
      HashSet<Guid> childStateIds = new HashSet<Guid>(stateDefinitions.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.WorkItemTypeReferenceName == witRefName)).Select<WorkItemStateDefinition, Guid>((Func<WorkItemStateDefinition, Guid>) (s => s.Id)));
      return stateDefinitions.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.WorkItemTypeReferenceName == witRefName || !childStateIds.Contains(s.Id)));
    }

    protected virtual bool ReturnsSystemStatesInGetStateApiForDerivedWIT() => false;
  }
}
