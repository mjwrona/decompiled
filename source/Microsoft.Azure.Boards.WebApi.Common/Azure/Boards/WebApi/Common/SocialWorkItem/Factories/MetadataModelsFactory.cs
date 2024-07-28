// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.SocialWorkItem.Factories.MetadataModelsFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.SocialWorkItem.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Boards.WebApi.Common.SocialWorkItem.Factories
{
  public static class MetadataModelsFactory
  {
    internal static Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.SocialWorkItem.Models.WorkItemProjectModel CreateWorkItemProjectModel(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItem,
      bool includeFields = false,
      bool includeLayout = false)
    {
      string projectName = workItem.GetProjectName(requestContext);
      string workItemType = workItem.WorkItemType;
      Project project = requestContext.GetService<WebAccessWorkItemService>().GetProject(requestContext, projectName);
      ProcessReadSecuredObject securedObject = ProcessReadSecuredObject.GetSecuredObject(requestContext, project.Guid);
      return MetadataModelsFactory.CreateWorkItemProjectModel(requestContext, workItem, securedObject, includeFields, includeLayout);
    }

    internal static Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.SocialWorkItem.Models.WorkItemProjectModel CreateWorkItemProjectModel(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItem,
      ProcessReadSecuredObject processReadSecuredObject,
      bool includeFields = false,
      bool includeLayout = false)
    {
      string projectName = workItem.GetProjectName(requestContext);
      string workItemType = workItem.WorkItemType;
      Project project = requestContext.GetService<WebAccessWorkItemService>().GetProject(requestContext, projectName);
      return MetadataModelsFactory.CreateWorkItemProjectModel(requestContext, project.Guid, workItemType, processReadSecuredObject, includeFields, includeLayout);
    }

    internal static Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.SocialWorkItem.Models.WorkItemProjectModel CreateWorkItemProjectModel(
      IVssRequestContext requestContext,
      Guid projectId,
      string workItemTypeName,
      ProcessReadSecuredObject processReadSecuredObject,
      bool includeFields = false,
      bool includeLayout = false)
    {
      return MetadataModelsFactory.CreateWorkItemProjectModel(requestContext, projectId, (IEnumerable<string>) new string[1]
      {
        workItemTypeName
      }, processReadSecuredObject, (includeFields ? 1 : 0) != 0, (includeLayout ? 1 : 0) != 0);
    }

    internal static Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.SocialWorkItem.Models.WorkItemProjectModel CreateWorkItemProjectModel(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> workItemTypeNames,
      ProcessReadSecuredObject processReadSecuredObject,
      bool includeFields = false,
      bool includeLayout = false)
    {
      Project project = requestContext.GetService<WebAccessWorkItemService>().GetProject(requestContext, projectId);
      return new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.SocialWorkItem.Models.WorkItemProjectModel((ISecuredObject) processReadSecuredObject)
      {
        Id = project.Id,
        Name = project.Name,
        Guid = project.Guid,
        WorkItemTypes = workItemTypeNames.Select<string, WorkItemTypeModel>((Func<string, WorkItemTypeModel>) (workItemTypeName => MetadataModelsFactory.CreateWorkItemTypeModel(requestContext, project.Guid, workItemTypeName, processReadSecuredObject, includeFields, includeLayout)))
      };
    }

    public static WorkItemTypeModel CreateWorkItemTypeModel(
      IVssRequestContext requestContext,
      Guid projectId,
      string workItemTypeReferenceName,
      ProcessReadSecuredObject processReadSecuredObject,
      bool includeFields = false,
      ISet<string> fieldReferenceNameFilter = null,
      bool includeLayout = false,
      bool includeStateColors = true)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType typeByReferenceName = requestContext.GetService<IWorkItemTypeService>().GetWorkItemTypeByReferenceName(requestContext, projectId, workItemTypeReferenceName);
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon colorAndIcon = MetadataModelsFactory.GetColorAndIcon(requestContext, projectId, typeByReferenceName.Name);
      IEnumerable<WorkItemTypeFieldModel> source = includeFields | includeLayout ? MetadataModelsFactory.CreateWorkItemTypeFieldsModels(requestContext, processReadSecuredObject, projectId, typeByReferenceName, fieldReferenceNameFilter) : Enumerable.Empty<WorkItemTypeFieldModel>();
      return new WorkItemTypeModel((ISecuredObject) processReadSecuredObject)
      {
        Name = typeByReferenceName.Name,
        ReferenceName = typeByReferenceName.ReferenceName,
        Description = typeByReferenceName.Description,
        Color = colorAndIcon?.Color ?? CommonWITUtils.GetDefaultWorkItemTypeColor(),
        Icon = colorAndIcon?.Icon ?? WorkItemTypeIconUtils.GetDefaultIcon(),
        IsDisabled = typeByReferenceName.IsDisabled,
        Fields = includeFields ? source : Enumerable.Empty<WorkItemTypeFieldModel>(),
        StateColors = includeStateColors ? MetadataModelsFactory.GetWorkItemTypeStateColors(requestContext, projectId, workItemTypeReferenceName) : (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemStateColor>) null,
        Layout = includeLayout ? MetadataLayoutModelsFactory.GetLayout((ISecuredObject) processReadSecuredObject, typeByReferenceName.GetFormLayout(requestContext), (IDictionary<string, string>) source.ToDictionary<WorkItemTypeFieldModel, string, string>((Func<WorkItemTypeFieldModel, string>) (x => x.ReferenceName), (Func<WorkItemTypeFieldModel, string>) (x => x.Name), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName)) : (WorkItemLayout) null
      };
    }

    public static WorkItemTypeModel CreateWorkItemTypeModel(
      IVssRequestContext requestContext,
      Guid projectId,
      string workItemTypeName,
      ProcessReadSecuredObject processReadSecuredObject,
      bool includeFields = false,
      bool includeLayout = false)
    {
      return MetadataModelsFactory.CreateWorkItemTypeModel(requestContext, projectId, workItemTypeName, processReadSecuredObject, includeFields, (ISet<string>) null, includeLayout, true);
    }

    internal static IEnumerable<WorkItemTypeFieldModel> CreateWorkItemTypeFieldsModels(
      IVssRequestContext requestContext,
      ProcessReadSecuredObject processReadSecuredObject,
      Guid projectId,
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType workItemType,
      ISet<string> fieldReferenceNameFilter)
    {
      AdditionalWorkItemTypeProperties workItemTypeDetails = workItemType.GetAdditionalProperties(requestContext);
      IWorkItemTrackingProcessService service = requestContext.GetService<IWorkItemTrackingProcessService>();
      ProcessDescriptor projectProcess = (ProcessDescriptor) null;
      if (projectId != Guid.Empty)
        service.TryGetProjectProcessDescriptor(requestContext, projectId, out projectProcess);
      return workItemType.GetFields(requestContext, true).Where<FieldEntry>((Func<FieldEntry, bool>) (f => fieldReferenceNameFilter == null || fieldReferenceNameFilter.Contains(f.ReferenceName))).Select<FieldEntry, WorkItemTypeFieldModel>((Func<FieldEntry, WorkItemTypeFieldModel>) (fieldEntry =>
      {
        WorkItemTypeFieldInstance typeFieldInstance = WorkItemTypeFieldInstanceFactory.Create(requestContext.WitContext(), fieldEntry, workItemType, workItemTypeDetails, projectProcess: projectProcess, projectId: new Guid?(projectId), includeUrl: false);
        return new WorkItemTypeFieldModel((ISecuredObject) processReadSecuredObject)
        {
          Id = fieldEntry.FieldId,
          Name = typeFieldInstance.Name,
          ReferenceName = typeFieldInstance.ReferenceName,
          Description = fieldEntry.Description,
          Type = MetadataModelsFactory.GetFieldType(fieldEntry.FieldType),
          ReadOnly = fieldEntry.IsReadOnly,
          IsIdentity = typeFieldInstance.IsIdentity,
          HelpText = typeFieldInstance.HelpText,
          AlwaysRequired = typeFieldInstance.AlwaysRequired,
          DefaultValue = typeFieldInstance.DefaultValue
        };
      }));
    }

    public static FieldType GetFieldType(InternalFieldType fieldType)
    {
      switch (fieldType)
      {
        case InternalFieldType.String:
          return FieldType.String;
        case InternalFieldType.Integer:
          return FieldType.Integer;
        case InternalFieldType.DateTime:
          return FieldType.DateTime;
        case InternalFieldType.PlainText:
          return FieldType.PlainText;
        case InternalFieldType.Html:
          return FieldType.Html;
        case InternalFieldType.TreePath:
          return FieldType.TreePath;
        case InternalFieldType.History:
          return FieldType.History;
        case InternalFieldType.Double:
          return FieldType.Double;
        case InternalFieldType.Guid:
          return FieldType.Guid;
        case InternalFieldType.Boolean:
          return FieldType.Boolean;
        case InternalFieldType.Identity:
          return FieldType.Identity;
        case InternalFieldType.PicklistInteger:
          return FieldType.PicklistInteger;
        case InternalFieldType.PicklistString:
          return FieldType.PicklistString;
        case InternalFieldType.PicklistDouble:
          return FieldType.PicklistDouble;
        default:
          return FieldType.String;
      }
    }

    private static Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon GetColorAndIcon(
      IVssRequestContext requestContext,
      Guid projectId,
      string workItemTypeName)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon typeColorAndIcon;
      return requestContext.GetService<IWorkItemMetadataFacadeService>().GetWorkItemTypeColorAndIconsByProjectId(requestContext, projectId).ToDictionary<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon, string, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon, string>) (c => c.WorkItemTypeName), (Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon>) (c => c), (IEqualityComparer<string>) TFStringComparer.WorkItemType).TryGetValue(workItemTypeName, out typeColorAndIcon) ? typeColorAndIcon : (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon) null;
    }

    private static IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemStateColor> GetWorkItemTypeStateColors(
      IVssRequestContext requestContext,
      Guid projectId,
      string workItemTypeName)
    {
      return requestContext.GetService<IWorkItemMetadataFacadeService>().GetWorkItemStateColors(requestContext, projectId, workItemTypeName).Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemStateColor, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemStateColor>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemStateColor, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemStateColor>) (c => new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemStateColor((ISecuredObject) c)
      {
        Name = c.Name,
        Color = c.Color
      }));
    }
  }
}
