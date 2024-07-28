// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.FieldsController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.Azure.Boards.WebApi.Common.Extensions;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "fields", ResourceVersion = 2)]
  public class FieldsController : WorkItemTrackingApiController
  {
    private const int TraceRange = 5908000;

    public override string TraceArea => "fields";

    [TraceFilter(5908000, 5908010)]
    [HttpGet]
    [ClientExample("GET__wit_fields.json", "Returns information for all fields", null, null)]
    [PublicProjectRequestRestrictions("5.0")]
    public IEnumerable<WorkItemField> GetFields([FromUri(Name = "$expand")] GetFieldsExpand expand = GetFieldsExpand.None)
    {
      if (this.ProjectInfo != null)
      {
        IEnumerable<WorkItemField> fields = WitFieldsHelper.GetFields(this.TfsRequestContext, expand, this.controllerSupportIncludePicklistId(), new Guid?(this.ProjectInfo.Id));
        return fields == null ? (IEnumerable<WorkItemField>) null : (IEnumerable<WorkItemField>) fields.ToList<WorkItemField>();
      }
      IEnumerable<WorkItemField> fields1 = WitFieldsHelper.GetFields(this.TfsRequestContext, expand, this.controllerSupportIncludePicklistId());
      return fields1 == null ? (IEnumerable<WorkItemField>) null : (IEnumerable<WorkItemField>) fields1.ToList<WorkItemField>();
    }

    [TraceFilter(5908010, 5908020)]
    [HttpGet]
    [ClientExample("GET__wit_fields__fieldName_.json", "Gets information on a specific field", null, null)]
    [PublicProjectRequestRestrictions("5.0")]
    public WorkItemField GetField(string fieldNameOrRefName) => this.ProjectInfo != null ? WitFieldsHelper.GetField(this.TfsRequestContext, fieldNameOrRefName, this.controllerSupportIncludePicklistId(), new Guid?(this.ProjectInfo.Id)) : WitFieldsHelper.GetField(this.TfsRequestContext, fieldNameOrRefName, this.controllerSupportIncludePicklistId());

    [TraceFilter(5908020, 5908030)]
    [HttpDelete]
    public void DeleteField(string fieldNameOrRefName)
    {
      if (string.IsNullOrWhiteSpace(fieldNameOrRefName))
        throw new VssPropertyValidationException(nameof (fieldNameOrRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (fieldNameOrRefName)));
      this.TfsRequestContext.GetService<IProcessFieldService>().DeleteField(this.TfsRequestContext, fieldNameOrRefName);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Field", "Delete");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("FieldReferenceName", (object) fieldNameOrRefName);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
    }

    [TraceFilter(5908030, 5908040)]
    [HttpPost]
    [ClientExample("POST_wit_field.json", "Create a new field", null, null)]
    public WorkItemField CreateField([FromBody] WorkItemField workItemField)
    {
      if (workItemField == null)
        throw new VssPropertyValidationException(nameof (workItemField), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (workItemField)));
      return WorkItemFieldFactory.Create(this.TfsRequestContext, this.TfsRequestContext.GetService<IProcessFieldService>().CreateField(this.TfsRequestContext, workItemField.Name, workItemField.Description, workItemField.Type.ToInternalFieldType(), picklistId: workItemField.PicklistId, referenceName: workItemField.ReferenceName));
    }

    [HttpPatch]
    [ClientResponseType(typeof (WorkItemField), null, null)]
    [ClientExample("UPDATE_wit_fields_UndeleteField.json", "Undelete a field", null, null)]
    public HttpResponseMessage UpdateField(string fieldNameOrRefName, [FromBody] UpdateWorkItemField payload)
    {
      if (string.IsNullOrWhiteSpace(fieldNameOrRefName))
        throw new VssPropertyValidationException(nameof (fieldNameOrRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (fieldNameOrRefName)));
      if (payload == null)
        throw new VssPropertyValidationException(nameof (payload), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullQueryParameter());
      if (payload.IsDeleted)
        throw new VssPropertyValidationException("IsDeleted", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.RestoreWorkItemUnsupportedPayload());
      FieldEntry field = this.TfsRequestContext.GetService<IProcessFieldService>().RestoreField(this.TfsRequestContext, fieldNameOrRefName);
      return field == null ? this.Request.CreateResponse(HttpStatusCode.NotFound) : this.Request.CreateResponse<WorkItemField>(HttpStatusCode.OK, WorkItemFieldFactory.Create(this.TfsRequestContext, field));
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<WorkItemTrackingFieldDefinitionNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<OutOfBoxFieldCannotBeDeletedException>(HttpStatusCode.BadRequest);
    }

    protected virtual bool controllerSupportIncludePicklistId() => false;
  }
}
