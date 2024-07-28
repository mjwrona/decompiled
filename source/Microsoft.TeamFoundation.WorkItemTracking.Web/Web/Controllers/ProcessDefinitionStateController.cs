// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessDefinitionStateController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

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
  [VersionedApiControllerCustomName(Area = "processDefinitions", ResourceName = "states", ResourceVersion = 1)]
  [FeatureEnabled("WebAccess.Process.Hierarchy")]
  public class ProcessDefinitionStateController : WorkItemTrackingApiController
  {
    [HttpPost]
    [ClientResponseType(typeof (WorkItemStateResultModel), null, null)]
    [ClientLocationId("4303625D-08F4-4461-B14B-32C65BBA5599")]
    public HttpResponseMessage CreateStateDefinition(
      Guid processId,
      string witRefName,
      [FromBody] WorkItemStateInputModel stateModel)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      if (stateModel == null)
        throw new VssPropertyValidationException(nameof (stateModel), ResourceStrings.NullOrEmptyParameter((object) nameof (stateModel)));
      if (string.IsNullOrWhiteSpace(stateModel.Name))
        throw new VssPropertyValidationException("Name", ResourceStrings.NullOrEmptyParameter((object) "Name"));
      if (string.IsNullOrWhiteSpace(stateModel.Color))
        throw new VssPropertyValidationException("Color", ResourceStrings.NullOrEmptyParameter((object) "Color"));
      if (string.IsNullOrWhiteSpace(stateModel.StateCategory))
        throw new VssPropertyValidationException("StateCategory", ResourceStrings.NullOrEmptyParameter((object) "StateCategory"));
      WorkItemStateDefinition stateDefinition = this.TfsRequestContext.GetService<IWorkItemStateDefinitionService>().CreateStateDefinition(this.TfsRequestContext, processId, witRefName, stateModel.ToWorkItemStateDeclaration());
      return this.Request.CreateResponse<WorkItemStateResultModel>(HttpStatusCode.Created, WorkItemStateResultModelFactory.Create(this.TfsRequestContext, processId, witRefName, stateDefinition));
    }

    [HttpDelete]
    [ClientLocationId("4303625D-08F4-4461-B14B-32C65BBA5599")]
    public void DeleteStateDefinition(Guid processId, string witRefName, Guid stateId)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      if (stateId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (stateId), ResourceStrings.NullOrEmptyParameter((object) nameof (stateId)));
      IWorkItemStateDefinitionService service = this.TfsRequestContext.GetService<IWorkItemStateDefinitionService>();
      try
      {
        service.DeleteStateDefinition(this.TfsRequestContext, processId, witRefName, stateId);
      }
      catch (WorkItemStateDefinitionNotFoundException ex)
      {
      }
    }

    [HttpPatch]
    [ClientResponseType(typeof (WorkItemStateResultModel), null, null)]
    [ClientLocationId("4303625D-08F4-4461-B14B-32C65BBA5599")]
    public HttpResponseMessage UpdateStateDefinition(
      Guid processId,
      string witRefName,
      Guid stateId,
      [FromBody] WorkItemStateInputModel stateModel)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      if (stateId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (stateId), ResourceStrings.NullOrEmptyParameter((object) nameof (stateId)));
      if (stateModel == null)
        throw new VssPropertyValidationException(nameof (stateModel), ResourceStrings.NullOrEmptyParameter((object) nameof (stateModel)));
      IWorkItemStateDefinitionService service = this.TfsRequestContext.GetService<IWorkItemStateDefinitionService>();
      WorkItemStateCategory? nullable = WorkItemStateInputModelFactory.ConvertStateCategoryStringToEnum(stateModel.StateCategory);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid processId1 = processId;
      string witReferenceName = witRefName;
      Guid stateId1 = stateId;
      string color = stateModel.Color;
      WorkItemStateCategory? stateCategory = nullable;
      int? order = stateModel.Order;
      WorkItemStateDefinition stateDefinition = service.UpdateStateDefinition(tfsRequestContext, processId1, witReferenceName, stateId1, color, stateCategory, order);
      return this.Request.CreateResponse<WorkItemStateResultModel>(HttpStatusCode.OK, WorkItemStateResultModelFactory.Create(this.TfsRequestContext, processId, witRefName, stateDefinition));
    }

    [HttpGet]
    [ClientResponseType(typeof (WorkItemStateResultModel), null, null)]
    [ClientLocationId("4303625D-08F4-4461-B14B-32C65BBA5599")]
    public HttpResponseMessage GetStateDefinition(Guid processId, string witRefName, Guid stateId)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      if (stateId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (stateId), ResourceStrings.NullOrEmptyParameter((object) nameof (stateId)));
      WorkItemStateDefinition stateDefinition = this.TfsRequestContext.GetService<IWorkItemStateDefinitionService>().GetStateDefinition(this.TfsRequestContext, processId, witRefName, stateId);
      return this.Request.CreateResponse<WorkItemStateResultModel>(HttpStatusCode.OK, WorkItemStateResultModelFactory.Create(this.TfsRequestContext, processId, witRefName, stateDefinition));
    }

    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<WorkItemStateResultModel>), null, null)]
    [ClientLocationId("4303625D-08F4-4461-B14B-32C65BBA5599")]
    public HttpResponseMessage GetStateDefinitions(Guid processId, string witRefName)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      return this.Request.CreateResponse<IEnumerable<WorkItemStateResultModel>>(HttpStatusCode.OK, this.TfsRequestContext.GetService<IWorkItemStateDefinitionService>().GetStateDefinitions(this.TfsRequestContext, processId, witRefName).Select<WorkItemStateDefinition, WorkItemStateResultModel>((Func<WorkItemStateDefinition, WorkItemStateResultModel>) (s => WorkItemStateResultModelFactory.Create(this.TfsRequestContext, processId, witRefName, s))));
    }

    [HttpPut]
    [ClientResponseType(typeof (WorkItemStateResultModel), null, null)]
    [ClientLocationId("4303625D-08F4-4461-B14B-32C65BBA5599")]
    public HttpResponseMessage HideStateDefinition(
      Guid processId,
      string witRefName,
      Guid stateId,
      [FromBody] HideStateModel hideStateModel)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException(nameof (witRefName), ResourceStrings.NullOrEmptyParameter((object) nameof (witRefName)));
      if (stateId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (stateId), ResourceStrings.NullOrEmptyParameter((object) nameof (stateId)));
      if (!hideStateModel.Hidden)
        throw new VssPropertyValidationException(nameof (hideStateModel), ResourceStrings.UnhideStateUsingDeleteState());
      WorkItemStateDefinition stateDefinition = this.TfsRequestContext.GetService<IWorkItemStateDefinitionService>().HideStateDefinition(this.TfsRequestContext, processId, witRefName, stateId);
      return this.Request.CreateResponse<WorkItemStateResultModel>(HttpStatusCode.OK, WorkItemStateResultModelFactory.Create(this.TfsRequestContext, processId, witRefName, stateDefinition));
    }
  }
}
