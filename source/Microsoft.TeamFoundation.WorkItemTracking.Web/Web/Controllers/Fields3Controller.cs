// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Fields3Controller
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
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "fields", ResourceVersion = 3)]
  [ControllerApiVersion(7.1)]
  public class Fields3Controller : WorkItemTrackingApiController
  {
    private const int TraceRange = 5908000;

    public override string TraceArea => "fields";

    [TraceFilter(5908000, 5908010)]
    [HttpGet]
    [ClientExample("GET__wit_fields.json", "Returns information for all fields", null, null)]
    [PublicProjectRequestRestrictions("7.1")]
    public IEnumerable<WorkItemField2> GetWorkItemFields([FromUri(Name = "$expand")] GetFieldsExpand expand = GetFieldsExpand.None)
    {
      IEnumerable<WorkItemField2> workItemFields2 = WitFieldsHelper.GetWorkItemFields2(this.TfsRequestContext, expand, this.controllerSupportIncludePicklistId(), this.ProjectInfo?.Id);
      return workItemFields2 == null ? (IEnumerable<WorkItemField2>) null : (IEnumerable<WorkItemField2>) workItemFields2.ToList<WorkItemField2>();
    }

    [TraceFilter(5908010, 5908020)]
    [HttpGet]
    [ClientExample("GET__wit_fields__fieldName_.json", "Gets information on a specific field", null, null)]
    [PublicProjectRequestRestrictions("7.1")]
    public WorkItemField2 GetWorkItemField(string fieldNameOrRefName) => WitFieldsHelper.GetWorkItemField2(this.TfsRequestContext, fieldNameOrRefName, this.controllerSupportIncludePicklistId(), this.ProjectInfo?.Id);

    [TraceFilter(5908020, 5908030)]
    [HttpDelete]
    public void DeleteWorkItemField(string fieldNameOrRefName)
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
    public WorkItemField2 CreateWorkItemField([FromBody] WorkItemField2 workItemField)
    {
      if (workItemField == null)
        throw new VssPropertyValidationException(nameof (workItemField), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (workItemField)));
      return this.createResponseField(this.TfsRequestContext.GetService<IProcessFieldService>().CreateField(this.TfsRequestContext, workItemField.Name, workItemField.Description, workItemField.Type.ToInternalFieldType(), picklistId: workItemField.PicklistId, referenceName: workItemField.ReferenceName));
    }

    [HttpPatch]
    [ClientResponseType(typeof (WorkItemField2), null, null)]
    [ClientExample("UPDATE_wit_fields_UndeleteField.json", "Undelete a field", null, null)]
    [ClientExample("UPDATE_wit_fields_LockField.json", "Set field locked", null, null)]
    public HttpResponseMessage UpdateWorkItemField(string fieldNameOrRefName, [FromBody] FieldUpdate payload)
    {
      WorkItemField2 workItemField2 = this.UpdateWorkItemFieldInternal(fieldNameOrRefName, payload);
      return workItemField2 == null ? this.Request.CreateResponse(HttpStatusCode.NotFound) : this.Request.CreateResponse<WorkItemField2>(HttpStatusCode.OK, workItemField2);
    }

    internal WorkItemField2 UpdateWorkItemFieldInternal(
      string fieldNameOrRefName,
      FieldUpdate payload)
    {
      this.validateUpdateRequestProperties(fieldNameOrRefName, payload);
      FieldEntry changedField = WitFieldsHelper.UpdateField(fieldNameOrRefName, payload, this.TfsRequestContext);
      return changedField == null ? (WorkItemField2) null : this.createResponseField(changedField);
    }

    private WorkItemField2 createResponseField(FieldEntry changedField) => WorkItemFieldFactory.CreateWithLockInfo(this.TfsRequestContext, changedField);

    private void validateUpdateRequestProperties(string fieldNameOrRefName, FieldUpdate payload)
    {
      if (string.IsNullOrWhiteSpace(fieldNameOrRefName))
        throw new VssPropertyValidationException(nameof (fieldNameOrRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (fieldNameOrRefName)));
      bool? nullable = payload != null ? payload.IsDeleted : throw new VssPropertyValidationException(nameof (payload), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullQueryParameter());
      if (nullable.HasValue)
      {
        nullable = payload.IsLocked;
        if (nullable.HasValue)
          throw new VssPropertyValidationException(nameof (payload), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.UpdateWorkItemUnsupportedPayload());
      }
      nullable = payload.IsDeleted;
      if (!nullable.HasValue)
        return;
      nullable = payload.IsDeleted;
      if (nullable.Value)
        throw new VssPropertyValidationException("IsDeleted", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.RestoreWorkItemUnsupportedPayload());
    }

    protected bool controllerSupportIncludePicklistId() => true;

    protected override bool ControllerSupportsProjectScopedUrls() => true;

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<WorkItemTrackingFieldDefinitionNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<OutOfBoxFieldCannotBeDeletedException>(HttpStatusCode.BadRequest);
    }
  }
}
