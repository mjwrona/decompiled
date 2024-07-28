// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.WitFieldsHelper
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.Metadata;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  public static class WitFieldsHelper
  {
    public static IEnumerable<WorkItemField> GetFields(
      IVssRequestContext requestContext,
      GetFieldsExpand expand = GetFieldsExpand.None,
      bool includePickList = true,
      Guid? projectId = null)
    {
      return (IEnumerable<WorkItemField>) WitFieldsHelper.GetWorkItemFields2(requestContext, expand, includePickList, projectId);
    }

    public static IEnumerable<WorkItemField2> GetWorkItemFields2(
      IVssRequestContext requestContext,
      GetFieldsExpand expand = GetFieldsExpand.None,
      bool includePickList = true,
      Guid? projectId = null)
    {
      IFieldTypeDictionary fieldDictionary = requestContext.WitContext().FieldDictionary;
      WorkItemTrackingFieldService service = requestContext.GetService<WorkItemTrackingFieldService>();
      IEnumerable<FieldEntry> source = (expand & GetFieldsExpand.IncludeDeleted) != 0 ? service.GetFieldEntries(requestContext, 0L, out long _, includeDeleted: true).AsEnumerable<FieldEntry>() : (IEnumerable<FieldEntry>) fieldDictionary.GetAllFields();
      if ((expand & GetFieldsExpand.ExtensionFields) == 0)
        source = (IEnumerable<FieldEntry>) source.Where<FieldEntry>((Func<FieldEntry, bool>) (f => (f.Usage & InternalFieldUsages.WorkItemTypeExtension) == InternalFieldUsages.None)).ToList<FieldEntry>();
      Dictionary<string, ISecuredObject> projectFieldReferenceNamesDict = (Dictionary<string, ISecuredObject>) null;
      if (projectId.HasValue)
      {
        Guid? nullable = projectId;
        Guid empty = Guid.Empty;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
        {
          projectFieldReferenceNamesDict = WitFieldsHelper.GetProjectFieldReferenceNames(requestContext, projectId.Value);
          source = source.Where<FieldEntry>((Func<FieldEntry, bool>) (field => projectFieldReferenceNamesDict.ContainsKey(field.ReferenceName)));
        }
      }
      Dictionary<string, ProcessFieldDefinition> systemFields = new Dictionary<string, ProcessFieldDefinition>();
      if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext))
        systemFields = requestContext.GetService<IProcessFieldService>().GetAllOutOfBoxFieldDefinitions(requestContext).ToDictionary<ProcessFieldDefinition, string>((Func<ProcessFieldDefinition, string>) (f => f.ReferenceName));
      return (IEnumerable<WorkItemField2>) source.Select<FieldEntry, WorkItemField2>((Func<FieldEntry, WorkItemField2>) (field =>
      {
        IVssRequestContext requestContext1 = requestContext;
        FieldEntry field1 = field;
        Dictionary<string, ProcessFieldDefinition> systemFields1 = systemFields;
        Guid? nullableProjectId = WitFieldsHelper.GetNullableProjectId(projectId);
        Dictionary<string, ISecuredObject> dictionary = projectFieldReferenceNamesDict;
        ISecuredObject valueOrDefault = dictionary != null ? dictionary.GetValueOrDefault<string, ISecuredObject>(field.ReferenceName, (ISecuredObject) null) : (ISecuredObject) null;
        int num = includePickList ? 1 : 0;
        return WorkItemFieldFactory.CreateWithLockInfo(requestContext1, field1, systemFields1, nullableProjectId, valueOrDefault, includePicklist: num != 0);
      })).OrderBy<WorkItemField2, string>((Func<WorkItemField2, string>) (a => a.Name));
    }

    public static WorkItemField GetField(
      IVssRequestContext requestContext,
      string fieldNameOrRefName,
      bool includePickList = true,
      Guid? projectId = null)
    {
      return (WorkItemField) WitFieldsHelper.GetWorkItemField2(requestContext, fieldNameOrRefName, includePickList, projectId);
    }

    public static WorkItemField2 GetWorkItemField2(
      IVssRequestContext requestContext,
      string fieldNameOrRefName,
      bool includePickList = true,
      Guid? projectId = null)
    {
      FieldEntry field = requestContext.WitContext().FieldDictionary.GetField(fieldNameOrRefName);
      ISecuredObject securedObject = (ISecuredObject) null;
      if (projectId.HasValue)
      {
        Guid? nullable = projectId;
        Guid empty = Guid.Empty;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0 && !WitFieldsHelper.FieldExistsInProject(requestContext, projectId.Value, field, out securedObject))
          throw new WorkItemTrackingFieldDefinitionNotFoundException(fieldNameOrRefName);
      }
      Dictionary<string, ProcessFieldDefinition> dictionary = requestContext.GetService<IProcessFieldService>().GetAllOutOfBoxFieldDefinitions(requestContext).ToDictionary<ProcessFieldDefinition, string>((Func<ProcessFieldDefinition, string>) (f => f.ReferenceName));
      return WorkItemFieldFactory.CreateWithLockInfo(requestContext, field, dictionary, WitFieldsHelper.GetNullableProjectId(projectId), securedObject, includePicklist: includePickList);
    }

    public static FieldEntry UpdateField(
      string fieldNameOrRefName,
      FieldUpdate payload,
      IVssRequestContext tfsRequestContext)
    {
      IProcessFieldService service = tfsRequestContext.GetService<IProcessFieldService>();
      if (payload.IsLocked.HasValue)
        return service.SetFieldLocked(tfsRequestContext, fieldNameOrRefName, payload.IsLocked.Value);
      return payload.IsDeleted.HasValue && !payload.IsDeleted.Value ? service.RestoreField(tfsRequestContext, fieldNameOrRefName) : (FieldEntry) null;
    }

    public static bool IsFieldUpdatable(IVssRequestContext requestContext, string fieldReference) => requestContext.WitContext().FieldDictionary.GetField(fieldReference).IsUpdatable;

    private static bool FieldExistsInProject(
      IVssRequestContext requestContext,
      Guid projectId,
      FieldEntry fieldEntry,
      out ISecuredObject securedObject)
    {
      securedObject = (ISecuredObject) null;
      Dictionary<string, ISecuredObject> fieldReferenceNames = WitFieldsHelper.GetProjectFieldReferenceNames(requestContext, projectId);
      if (!fieldReferenceNames.ContainsKey(fieldEntry.ReferenceName))
        return false;
      securedObject = fieldReferenceNames[fieldEntry.ReferenceName];
      return true;
    }

    private static Guid? GetNullableProjectId(Guid? projectId)
    {
      Guid? nullable = projectId;
      Guid empty = Guid.Empty;
      return (nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) != 0 ? new Guid?() : projectId;
    }

    public static Dictionary<string, ISecuredObject> GetProjectFieldReferenceNames(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      Dictionary<string, ISecuredObject> fieldReferenceNamesSecuredObjectMap = new Dictionary<string, ISecuredObject>();
      Guid processTypeId;
      if (WitFieldsHelper.TryGetProjectProcessTypeId(requestContext, projectId, out processTypeId))
        WitFieldsHelper.GetComposedWorkItemTypesForProcess(requestContext, processTypeId).ForEach<ComposedWorkItemType>((Action<ComposedWorkItemType>) (composedWorkItemType => composedWorkItemType.GetLegacyFields(requestContext).ForEach<ProcessFieldResult>((Action<ProcessFieldResult>) (processFieldResult => fieldReferenceNamesSecuredObjectMap.TryAdd<string, ISecuredObject>(processFieldResult.ReferenceName, (ISecuredObject) composedWorkItemType)))));
      else
        WitFieldsHelper.GetWorkItemTypesForProject(requestContext, projectId).ForEach<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType>((Action<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType>) (workItemType => workItemType.GetFields(requestContext, false).ForEach<FieldEntry>((Action<FieldEntry>) (processFieldResult => fieldReferenceNamesSecuredObjectMap.TryAdd<string, ISecuredObject>(processFieldResult.ReferenceName, (ISecuredObject) workItemType)))));
      return fieldReferenceNamesSecuredObjectMap;
    }

    private static bool TryGetProjectProcessTypeId(
      IVssRequestContext requestContext,
      Guid projectId,
      out Guid processTypeId)
    {
      if (!WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext) && !WorkItemTrackingFeatureFlags.AreXMLProcessesEnabled(requestContext))
      {
        processTypeId = Guid.Empty;
        return false;
      }
      ProjectProperty projectProperty = requestContext.GetService<IProjectService>().GetProjectProperties(requestContext, projectId, ProcessTemplateIdPropertyNames.ProcessTemplateType).FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (property => StringComparer.OrdinalIgnoreCase.Equals(property.Name, ProcessTemplateIdPropertyNames.ProcessTemplateType)));
      if (projectProperty != null)
      {
        string input = (string) projectProperty.Value;
        processTypeId = Guid.Parse(input);
        return true;
      }
      processTypeId = Guid.Empty;
      return false;
    }

    private static IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType> GetWorkItemTypesForProject(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType>) requestContext.GetService<IWorkItemTypeService>().GetWorkItemTypes(requestContext, projectId);
    }

    public static IEnumerable<ComposedWorkItemType> GetComposedWorkItemTypesForProcess(
      IVssRequestContext requestContext,
      Guid processTypeId)
    {
      return (IEnumerable<ComposedWorkItemType>) requestContext.GetService<IProcessWorkItemTypeService>().GetAllWorkItemTypes(requestContext, processTypeId, true);
    }
  }
}
