// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemTypeFieldBaseController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  public abstract class WorkItemTypeFieldBaseController : WorkItemTrackingApiController
  {
    protected IEnumerable<WorkItemTypeFieldInstance> GetWorkItemTypeFieldInstances(
      string type,
      WorkItemTypeFieldsExpandLevel expand = WorkItemTypeFieldsExpandLevel.None)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType workItemType = !string.IsNullOrWhiteSpace(type) ? this.GetWorkItemTypeFromName(type) : throw new VssPropertyValidationException(nameof (type), ResourceStrings.NullOrEmptyParameter((object) nameof (type)));
      AdditionalWorkItemTypeProperties workItemTypeDetails = workItemType.GetAdditionalProperties(this.WitRequestContext.RequestContext);
      IWorkItemTrackingProcessService service = this.WitRequestContext.RequestContext.GetService<IWorkItemTrackingProcessService>();
      ProcessDescriptor projectProcess = (ProcessDescriptor) null;
      if (this.ProjectId != Guid.Empty)
        service.TryGetProjectProcessDescriptor(this.WitRequestContext.RequestContext, this.ProjectId, out projectProcess);
      return workItemType.GetFieldIds(this.WitRequestContext.RequestContext, false).Select<int, WorkItemTypeFieldInstance>((Func<int, WorkItemTypeFieldInstance>) (fid => WorkItemTypeFieldInstanceFactory.Create(this.WitRequestContext, this.WitRequestContext.FieldDictionary.GetField(fid), workItemTypeDetails, projectProcess, this.ProjectId, workItemType, expand)));
    }

    protected WorkItemTypeFieldInstance GetWorkItemTypeFieldInstance(
      string type,
      string field,
      WorkItemTypeFieldsExpandLevel expand = WorkItemTypeFieldsExpandLevel.None)
    {
      if (string.IsNullOrWhiteSpace(type))
        throw new VssPropertyValidationException(nameof (type), ResourceStrings.NullOrEmptyParameter((object) nameof (type)));
      if (string.IsNullOrWhiteSpace(field))
        throw new VssPropertyValidationException(nameof (field), ResourceStrings.NullOrEmptyParameter((object) nameof (field)));
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType itemTypeFromName = this.GetWorkItemTypeFromName(type);
      IWorkItemTrackingProcessService service = this.WitRequestContext.RequestContext.GetService<IWorkItemTrackingProcessService>();
      ProcessDescriptor processDescriptor = (ProcessDescriptor) null;
      if (this.ProjectId != Guid.Empty)
        service.TryGetProjectProcessDescriptor(this.WitRequestContext.RequestContext, this.ProjectId, out processDescriptor);
      return WorkItemTypeFieldInstanceFactory.Create(this.WitRequestContext, this.GetFieldEntryFromName(field), itemTypeFromName.GetAdditionalProperties(this.WitRequestContext.RequestContext), processDescriptor, this.ProjectId, itemTypeFromName, expand);
    }

    protected Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType GetWorkItemTypeFromName(
      string type)
    {
      return this.WitRequestContext.RequestContext.GetService<IWorkItemTypeService>().GetWorkItemTypeByReferenceName(this.WitRequestContext.RequestContext, this.ProjectId, type);
    }

    protected FieldEntry GetFieldEntryFromName(string field)
    {
      FieldEntry field1;
      if (!this.WitRequestContext.FieldDictionary.TryGetField(field, out field1))
        throw new WorkItemTrackingFieldDefinitionNotFoundException(field);
      return field1;
    }
  }
}
