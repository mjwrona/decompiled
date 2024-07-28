// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessWorkItemTypes2Controller
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories;
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
  [VersionedApiControllerCustomName(Area = "processes", ResourceName = "workItemTypes", ResourceVersion = 2)]
  [ControllerApiVersion(5.0)]
  public class ProcessWorkItemTypes2Controller : WorkItemTrackingApiController
  {
    protected const int TraceRange = 5921000;

    [HttpPost]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessWorkItemType), null, null)]
    [ClientExample("POST_work_item_type.json", "Creates a work item type in the process", null, null)]
    [TraceFilter(5921030, 5921040)]
    public HttpResponseMessage CreateProcessWorkItemType(
      Guid processId,
      [FromBody] CreateProcessWorkItemTypeRequest workItemType)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (workItemType == null)
        throw new VssPropertyValidationException(nameof (workItemType), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullWorkItemTypeObject());
      int num = string.IsNullOrEmpty(workItemType.InheritsFrom) ? 1 : 0;
      if (num != 0)
      {
        if (string.IsNullOrEmpty(workItemType.Color))
          throw new VssPropertyValidationException(nameof (workItemType), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) "Color"));
        if (string.IsNullOrEmpty(workItemType.Name))
          throw new VssPropertyValidationException(nameof (workItemType), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingWorkItemTypeParameter((object) "Name"));
      }
      IProcessWorkItemTypeService service = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>();
      Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessWorkItemType workItemType1 = num == 0 ? service.CreateDerivedWorkItemType(this.TfsRequestContext, processId, workItemType.InheritsFrom, workItemType.Description, workItemType.Color, workItemType.Icon, new bool?(workItemType.IsDisabled)) : service.CreateWorkItemType(this.TfsRequestContext, processId, workItemType.Name, workItemType.Color, workItemType.Icon, workItemType.Description, new bool?(workItemType.IsDisabled));
      HttpResponseMessage response = this.Request.CreateResponse<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessWorkItemType>(HttpStatusCode.Created, new ProcessWorkItemTypeFactory().Create(this.TfsRequestContext, processId, workItemType1));
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("WorkItemType", "Create");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("WorkItemTypeName", (object) workItemType.Name);
      data.Add("WorkItemTypeDescription", (object) workItemType.Description);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return response;
    }

    [HttpGet]
    [ClientExample("GET_a_work_item_type.json", "Returns a single work item type in a process", null, null)]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessWorkItemType), null, null)]
    [TraceFilter(5921050, 5921060)]
    public HttpResponseMessage GetProcessWorkItemType(
      Guid processId,
      string witRefName,
      [FromUri(Name = "$expand")] GetWorkItemTypeExpand expand = GetWorkItemTypeExpand.None)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      ComposedWorkItemType itemTypeByRefName = ProcessWorkItemTypes2Controller.GetWorkItemTypeByRefName((IEnumerable<ComposedWorkItemType>) this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().GetAllWorkItemTypes(this.TfsRequestContext, processId, true, true), processId, witRefName);
      return this.Request.CreateResponse<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessWorkItemType>(HttpStatusCode.OK, new ProcessWorkItemTypeFactory().Create(this.TfsRequestContext, processId, itemTypeByRefName, expand));
    }

    [HttpGet]
    [ClientExample("GET_all_work_item_types.json", "Get a list of all work item types in a process", null, null)]
    [ClientExample("GET_all_work_item_types_expand_state.json", "Get a list of all work item types in a process with expand set to states", null, null)]
    [ClientResponseType(typeof (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessWorkItemType>), null, null)]
    [TraceFilter(5921050, 5921060)]
    public HttpResponseMessage GetProcessWorkItemTypes(Guid processId, [FromUri(Name = "$expand")] GetWorkItemTypeExpand expand = GetWorkItemTypeExpand.None)
    {
      IReadOnlyCollection<ComposedWorkItemType> source = !(processId == Guid.Empty) ? this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().GetAllWorkItemTypes(this.TfsRequestContext, processId, true, true) : throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      ProcessWorkItemTypeFactory factory = new ProcessWorkItemTypeFactory();
      return this.Request.CreateResponse<IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessWorkItemType>>(HttpStatusCode.OK, source.Select<ComposedWorkItemType, Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessWorkItemType>((Func<ComposedWorkItemType, Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessWorkItemType>) (wit => factory.Create(this.TfsRequestContext, processId, wit, expand))));
    }

    [HttpPatch]
    [ClientExample("UPDATE_work_item_types.json", "Updates a work item type in the process", null, null)]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessWorkItemType), null, null)]
    public HttpResponseMessage UpdateProcessWorkItemType(
      Guid processId,
      string witRefName,
      [FromBody] UpdateProcessWorkItemTypeRequest workItemTypeUpdate)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      if (workItemTypeUpdate == null)
        throw new VssPropertyValidationException(nameof (workItemTypeUpdate), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (workItemTypeUpdate)));
      Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessWorkItemType workItemType = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().UpdateWorkItemType(this.TfsRequestContext, processId, witRefName, workItemTypeUpdate.Color, workItemTypeUpdate.Icon, workItemTypeUpdate.Description, new bool?(workItemTypeUpdate.IsDisabled));
      ProcessWorkItemTypeFactory workItemTypeFactory = new ProcessWorkItemTypeFactory();
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("WorkItemType", "Update");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("WorkItemTypeReferenceName", (object) witRefName);
      data.Add("WorkItemTypeDescription", (object) workItemTypeUpdate.Description);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessWorkItemType>(HttpStatusCode.OK, workItemTypeFactory.Create(this.TfsRequestContext, processId, workItemType));
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage DeleteProcessWorkItemType(Guid processId, string witRefName)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException("workItemTypeReferenceName", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) "workItemTypeReferenceName"));
      IProcessWorkItemTypeService service = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>();
      IEnumerable<ProjectProcessDescriptorMapping> source = this.TfsRequestContext.GetService<IWorkItemTrackingProcessService>().GetProjectProcessDescriptorMappings(this.TfsRequestContext, expectUnmappedProjects: true).Where<ProjectProcessDescriptorMapping>((Func<ProjectProcessDescriptorMapping, bool>) (m => m.Descriptor.TypeId == processId));
      if (source.Any<ProjectProcessDescriptorMapping>())
      {
        ComposedWorkItemType itemTypeByRefName = ProcessWorkItemTypes2Controller.GetWorkItemTypeByRefName((IEnumerable<ComposedWorkItemType>) service.GetAllWorkItemTypes(this.TfsRequestContext, processId, true, true), processId, witRefName);
        IEnumerable<string> values = source.Select<ProjectProcessDescriptorMapping, string>((Func<ProjectProcessDescriptorMapping, string>) (m => m.Project.Name));
        string wiql = "select * from workitems where [System.WorkItemType] = '" + itemTypeByRefName.Name + "' and [System.TeamProject] in ('" + string.Join("','", values) + "')";
        if (this.TfsRequestContext.GetService<IWorkItemQueryService>().ExecuteQuery(this.TfsRequestContext.Elevate(), wiql, topCount: 1).WorkItemIds.Any<int>())
          throw new ActiveWorkItemsExistException(itemTypeByRefName.Name);
      }
      service.DestroyWorkItemType(this.TfsRequestContext, processId, witRefName);
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

    internal static ComposedWorkItemType GetWorkItemTypeByRefName(
      IEnumerable<ComposedWorkItemType> allWorkItemTypes,
      Guid processId,
      string witRefName)
    {
      IEnumerable<ComposedWorkItemType> source = allWorkItemTypes.Where<ComposedWorkItemType>((Func<ComposedWorkItemType, bool>) (x => TFStringComparer.WorkItemTypeReferenceName.Equals(x.ReferenceName, witRefName) || TFStringComparer.WorkItemTypeReferenceName.Equals(x.ParentTypeRefName, witRefName)));
      return source.Any<ComposedWorkItemType>() ? source.FirstOrDefault<ComposedWorkItemType>() : throw new ProcessWorkItemTypeNotFoundException(processId);
    }
  }
}
