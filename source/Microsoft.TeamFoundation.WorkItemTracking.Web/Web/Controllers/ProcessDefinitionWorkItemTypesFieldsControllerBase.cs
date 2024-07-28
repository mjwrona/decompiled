// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessDefinitionWorkItemTypesFieldsControllerBase
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
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
  public abstract class ProcessDefinitionWorkItemTypesFieldsControllerBase : 
    WorkItemTrackingApiController
  {
    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("976713B4-A62E-499E-94DC-EEB869EA9126")]
    public HttpResponseMessage RemoveFieldFromWorkItemType(
      Guid processId,
      string witRefNameForFields,
      string fieldRefName)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefNameForFields))
        throw new VssPropertyValidationException(nameof (witRefNameForFields), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefNameForFields)));
      if (string.IsNullOrEmpty(fieldRefName))
        throw new VssPropertyValidationException(nameof (fieldRefName), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (fieldRefName)));
      this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().RemoveWorkItemTypeField(this.TfsRequestContext, processId, witRefNameForFields, fieldRefName);
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Field", "Remove");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("WorkItemTypeReferenceName", (object) witRefNameForFields);
      data.Add("FieldReferenceName", (object) fieldRefName);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    protected HttpResponseMessage ProcessGetWorkItemTypeFields(
      Guid processId,
      string witRefNameForFields,
      bool useIdentityRef)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefNameForFields))
        throw new VssPropertyValidationException(nameof (witRefNameForFields), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefNameForFields)));
      ProcessTypelet processTypelet = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().GetTypelet<ProcessTypelet>(this.TfsRequestContext, processId, witRefNameForFields, true);
      if (processTypelet.Fields == null)
        return this.Request.CreateResponse<IEnumerable<WorkItemTypeFieldModel>>(HttpStatusCode.OK, Enumerable.Empty<WorkItemTypeFieldModel>());
      return useIdentityRef ? this.Request.CreateResponse<IEnumerable<WorkItemTypeFieldModel2>>(HttpStatusCode.OK, WorkItemTypeFieldModelFactory.CreateFieldModelsWithIdentityRef(this.TfsRequestContext, processTypelet, processTypelet.Fields)) : this.Request.CreateResponse<IEnumerable<WorkItemTypeFieldModel>>(HttpStatusCode.OK, processTypelet.Fields.Select<WorkItemTypeExtensionFieldEntry, WorkItemTypeFieldModel>((Func<WorkItemTypeExtensionFieldEntry, WorkItemTypeFieldModel>) (f => WorkItemTypeFieldModelFactory.Create(this.TfsRequestContext, processTypelet, f.Field.ReferenceName))));
    }

    protected HttpResponseMessage ProcessGetWorkItemTypeField(
      Guid processId,
      string witRefNameForFields,
      string fieldRefName,
      bool useIdentityRef)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefNameForFields))
        throw new VssPropertyValidationException(nameof (witRefNameForFields), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefNameForFields)));
      ProcessTypelet typelet = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().GetTypelet<ProcessTypelet>(this.TfsRequestContext, processId, witRefNameForFields, true);
      WorkItemTypeExtensionFieldEntry field = typelet.Fields.FirstOrDefault<WorkItemTypeExtensionFieldEntry>((Func<WorkItemTypeExtensionFieldEntry, bool>) (f => f.Field.ReferenceName == fieldRefName));
      if (field == null)
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      return useIdentityRef ? this.Request.CreateResponse<WorkItemTypeFieldModel2>(HttpStatusCode.OK, WorkItemTypeFieldModelFactory.CreateFieldModelWithIdentityRef(this.TfsRequestContext, typelet, field)) : this.Request.CreateResponse<WorkItemTypeFieldModel>(HttpStatusCode.OK, WorkItemTypeFieldModelFactory.Create(this.TfsRequestContext, typelet, field.Field.ReferenceName));
    }

    protected HttpResponseMessage ProcessAddFieldToWorkItemType(
      Guid processId,
      string witRefNameForFields,
      object field)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefNameForFields))
        throw new VssPropertyValidationException(nameof (witRefNameForFields), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (witRefNameForFields)));
      if (field == null)
        throw new VssPropertyValidationException(nameof (field), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullFieldObject());
      WorkItemTypeFieldModel field1 = (WorkItemTypeFieldModel) null;
      WorkItemTypeFieldModel2 field2 = (WorkItemTypeFieldModel2) null;
      string fieldRefName = (string) null;
      if (field is WorkItemTypeFieldModel)
      {
        field1 = field as WorkItemTypeFieldModel;
        fieldRefName = !string.IsNullOrEmpty(field1.ReferenceName) ? field1.ReferenceName : throw new VssPropertyValidationException(nameof (field), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingFieldParameter((object) "ReferenceName"));
      }
      else
      {
        field2 = field is WorkItemTypeFieldModel2 ? field as WorkItemTypeFieldModel2 : throw new ArgumentException("'field' is not valid field model");
        fieldRefName = !string.IsNullOrEmpty(field2.ReferenceName) ? field2.ReferenceName : throw new VssPropertyValidationException(nameof (field), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingFieldParameter((object) "ReferenceName"));
      }
      IProcessWorkItemTypeService service = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>();
      WorkItemTypeletFieldRuleProperties fieldRuleProperties = field1 != null ? WorkItemTypeFieldModelFactory.GetFieldProperties(field1) : WorkItemTypeFieldModelFactory.GetFieldProperties(this.TfsRequestContext, field2);
      ProcessDescriptor processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(this.TfsRequestContext, processId);
      IVssRequestContext tfsRequestContext1 = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Field", "Add");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) processDescriptor?.Name);
      data.Add("WorkItemTypeReferenceName", (object) witRefNameForFields);
      data.Add("FieldReferenceName", (object) fieldRefName);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext1.LogAuditEvent(actionId, data, targetHostId, projectId);
      IVssRequestContext tfsRequestContext2 = this.TfsRequestContext;
      Guid processId1 = processId;
      string witReferenceName = witRefNameForFields;
      string fieldReferenceName = fieldRefName;
      WorkItemTypeletFieldRuleProperties fieldProps = fieldRuleProperties;
      ProcessWorkItemType wit = service.AddOrUpdateWorkItemTypeField(tfsRequestContext2, processId1, witReferenceName, fieldReferenceName, fieldProps);
      return field1 == null ? this.Request.CreateResponse<WorkItemTypeFieldModel2>(HttpStatusCode.OK, WorkItemTypeFieldModelFactory.CreateFieldModelWithIdentityRef(this.TfsRequestContext, (ProcessTypelet) wit, wit.Fields.First<WorkItemTypeExtensionFieldEntry>((Func<WorkItemTypeExtensionFieldEntry, bool>) (f => f.Field.ReferenceName == fieldRefName)))) : this.Request.CreateResponse<WorkItemTypeFieldModel>(HttpStatusCode.OK, WorkItemTypeFieldModelFactory.Create(this.TfsRequestContext, (ProcessTypelet) wit, fieldRefName));
    }

    protected HttpResponseMessage ProcessUpdateWorkItemTypeField(
      Guid processId,
      string witRefName,
      object field)
    {
      WorkItemTypeFieldModel2 itemTypeFieldModel2 = field is WorkItemTypeFieldModel2 ? field as WorkItemTypeFieldModel2 : throw new ArgumentException("'field' is not a valid field model for update");
      if (string.IsNullOrEmpty(itemTypeFieldModel2.ReferenceName))
        throw new VssPropertyValidationException(nameof (field), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingFieldParameter((object) "ReferenceName"));
      string fieldRefName = itemTypeFieldModel2.Description != null ? itemTypeFieldModel2.ReferenceName : throw new VssPropertyValidationException(nameof (field), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.MissingFieldParameter((object) "Description"));
      string description = itemTypeFieldModel2.Description;
      this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().AddOrUpdateHelpTextRule(this.TfsRequestContext, processId, witRefName, fieldRefName, description);
      return this.ProcessGetWorkItemTypeField(processId, witRefName, fieldRefName, true);
    }
  }
}
