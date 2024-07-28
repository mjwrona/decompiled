// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessDefinitionFieldController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.Azure.Boards.WebApi.Common.Extensions;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ProcessDefinitions.Factories;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [FeatureEnabled("WebAccess.Process.Hierarchy")]
  [VersionedApiControllerCustomName(Area = "processDefinitions", ResourceName = "fields", ResourceVersion = 1)]
  public class ProcessDefinitionFieldController : WorkItemTrackingApiController
  {
    [HttpPost]
    [ClientResponseType(typeof (FieldModel), null, null)]
    public HttpResponseMessage CreateField(Guid processId, [FromBody] FieldModel field)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (field == null)
        throw new VssPropertyValidationException(nameof (field), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullFieldObject());
      if (string.IsNullOrEmpty(field.Name))
        throw new VssPropertyValidationException(nameof (field), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingFieldParameter((object) "name"));
      Guid? picklistId = new Guid?();
      if (field.PickList != null)
      {
        if (field.PickList.Id == Guid.Empty)
          throw new VssPropertyValidationException(nameof (field), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.InvalidPicklistId());
        picklistId = new Guid?(field.PickList.Id);
      }
      FieldEntry field1 = this.TfsRequestContext.GetService<IProcessFieldService>().CreateField(this.TfsRequestContext, field.Name, field.Description, field.Type.ToInternalFieldType(), new Guid?(processId), picklistId, field.Id);
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Field", "Create");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("FieldName", (object) field.Name);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse<FieldModel>(HttpStatusCode.Created, FieldModelFactory.Create(this.TfsRequestContext, processId, field1));
    }

    [HttpPatch]
    [ClientResponseType(typeof (FieldModel), null, null)]
    public HttpResponseMessage UpdateField(Guid processId, [FromBody] FieldUpdate field)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (field == null)
        throw new VssPropertyValidationException(nameof (field), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullFieldObject());
      if (string.IsNullOrEmpty(field.Id))
        throw new VssPropertyValidationException("Id", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingFieldParameter((object) "Id"));
      if (field.Description == null)
        throw new VssPropertyValidationException("Description", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingFieldParameter((object) "Description"));
      FieldEntry field1 = this.TfsRequestContext.GetService<IProcessFieldService>().UpdateField(this.TfsRequestContext, field.Id, field.Description);
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Field", "Edit");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("FieldName", (object) field1.Name);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse<FieldModel>(HttpStatusCode.OK, FieldModelFactory.Create(this.TfsRequestContext, processId, field1));
    }
  }
}
