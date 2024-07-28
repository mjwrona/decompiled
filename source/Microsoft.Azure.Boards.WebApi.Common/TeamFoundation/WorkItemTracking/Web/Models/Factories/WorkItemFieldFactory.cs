// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories.WorkItemFieldFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories
{
  internal static class WorkItemFieldFactory
  {
    public static IEnumerable<WorkItemFieldOperation> GetSupportedOperations(
      IVssRequestContext requestContext,
      InternalFieldType fieldType,
      FieldEntry fieldEntry,
      ISecuredObject securedObject = null)
    {
      foreach (string availableOperator in WiqlOperatorHelper.GetAvailableOperators(requestContext, fieldType, fieldEntry))
        yield return new WorkItemFieldOperation(securedObject)
        {
          Name = availableOperator,
          ReferenceName = WiqlOperatorHelper.GetSupportedOperationReferenceName(availableOperator)
        };
    }

    public static WorkItemField Create(
      IVssRequestContext requestContext,
      FieldEntry field,
      Dictionary<string, ProcessFieldDefinition> systemFields = null,
      Guid? projectId = null,
      ISecuredObject securedObject = null,
      bool returnProjectScopedUrl = true,
      bool includePicklist = false)
    {
      string helpText = systemFields == null || !systemFields.ContainsKey(field.ReferenceName) ? (string) null : systemFields[field.ReferenceName].HelpText;
      IWorkItemPickListService service = requestContext.GetService<IWorkItemPickListService>();
      bool flag = false;
      Guid? nullable1 = field.PickListId;
      if (nullable1.HasValue)
      {
        nullable1 = field.PickListId;
        if (!nullable1.Equals((object) Guid.Empty))
        {
          IWorkItemPickListService itemPickListService = service;
          IVssRequestContext requestContext1 = requestContext;
          nullable1 = field.PickListId;
          Guid listId = nullable1 ?? Guid.Empty;
          flag = itemPickListService.GetList(requestContext1, listId).IsSuggested(requestContext, Guid.Empty, 0);
        }
      }
      int startIndex1 = field.FieldId.ToString().Length + "*Del".Length;
      int startIndex2 = field.FieldId.ToString().Length + ".Del".Length;
      WorkItemField workItemField = new WorkItemField(securedObject);
      workItemField.Name = field.IsDeleted ? field.Name.Substring(startIndex2) : field.Name;
      workItemField.ReferenceName = field.IsDeleted ? field.ReferenceName.Substring(startIndex1) : field.ReferenceName;
      workItemField.Type = WorkItemFieldFactory.GetFieldType(field.FieldType);
      workItemField.ReadOnly = WorkItemFieldFactory.IsReadOnly(field);
      workItemField.CanSortBy = field.CanSortBy;
      workItemField.IsQueryable = field.IsQueryable;
      workItemField.Usage = WorkItemFieldFactory.GetFieldUsage(field.Usage);
      workItemField.Description = string.IsNullOrWhiteSpace(field.Description) ? helpText : field.Description;
      workItemField.Url = field.IsDeleted ? (string) null : WitUrlHelper.GetFieldUrl(requestContext, field.ReferenceName, projectId, returnProjectScopedUrl);
      workItemField.SupportedOperations = WorkItemFieldFactory.GetSupportedOperations(requestContext, field.FieldType, field, securedObject);
      workItemField.IsIdentity = field.IsIdentity;
      workItemField.IsPicklist = field.IsPicklist;
      workItemField.IsPicklistSuggested = flag;
      Guid? nullable2;
      if (includePicklist && field.IsPicklist)
      {
        nullable1 = field.PickListId;
        Guid empty = Guid.Empty;
        if ((nullable1.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
        {
          nullable2 = field.PickListId;
          goto label_7;
        }
      }
      nullable1 = new Guid?();
      nullable2 = nullable1;
label_7:
      workItemField.PicklistId = nullable2;
      workItemField.IsDeleted = field.IsDeleted;
      return workItemField;
    }

    public static WorkItemField2 CreateWithLockInfo(
      IVssRequestContext requestContext,
      FieldEntry field,
      Dictionary<string, ProcessFieldDefinition> systemFields = null,
      Guid? projectId = null,
      ISecuredObject securedObject = null,
      bool returnProjectScopedUrl = true,
      bool includePicklist = false)
    {
      string helpText = systemFields == null || !systemFields.ContainsKey(field.ReferenceName) ? (string) null : systemFields[field.ReferenceName].HelpText;
      IWorkItemPickListService service = requestContext.GetService<IWorkItemPickListService>();
      bool flag = false;
      Guid? nullable1 = field.PickListId;
      if (nullable1.HasValue)
      {
        nullable1 = field.PickListId;
        if (!nullable1.Equals((object) Guid.Empty))
        {
          IWorkItemPickListService itemPickListService = service;
          IVssRequestContext requestContext1 = requestContext;
          nullable1 = field.PickListId;
          Guid listId = nullable1 ?? Guid.Empty;
          flag = itemPickListService.GetList(requestContext1, listId).IsSuggested(requestContext, Guid.Empty, 0);
        }
      }
      int startIndex1 = field.FieldId.ToString().Length + "*Del".Length;
      int startIndex2 = field.FieldId.ToString().Length + ".Del".Length;
      WorkItemField2 withLockInfo = new WorkItemField2(securedObject);
      withLockInfo.Name = field.IsDeleted ? field.Name.Substring(startIndex2) : field.Name;
      withLockInfo.ReferenceName = field.IsDeleted ? field.ReferenceName.Substring(startIndex1) : field.ReferenceName;
      withLockInfo.Type = WorkItemFieldFactory.GetFieldType(field.FieldType);
      withLockInfo.ReadOnly = WorkItemFieldFactory.IsReadOnly(field);
      withLockInfo.CanSortBy = field.CanSortBy;
      withLockInfo.IsQueryable = field.IsQueryable;
      withLockInfo.Usage = WorkItemFieldFactory.GetFieldUsage(field.Usage);
      withLockInfo.Description = string.IsNullOrWhiteSpace(field.Description) ? helpText : field.Description;
      withLockInfo.Url = field.IsDeleted ? (string) null : WitUrlHelper.GetFieldUrl(requestContext, field.ReferenceName, projectId, returnProjectScopedUrl);
      withLockInfo.SupportedOperations = WorkItemFieldFactory.GetSupportedOperations(requestContext, field.FieldType, field, securedObject);
      withLockInfo.IsIdentity = field.IsIdentity;
      withLockInfo.IsPicklist = field.IsPicklist;
      withLockInfo.IsPicklistSuggested = flag;
      Guid? nullable2;
      if (includePicklist && field.IsPicklist)
      {
        nullable1 = field.PickListId;
        Guid empty = Guid.Empty;
        if ((nullable1.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
        {
          nullable2 = field.PickListId;
          goto label_7;
        }
      }
      nullable1 = new Guid?();
      nullable2 = nullable1;
label_7:
      withLockInfo.PicklistId = nullable2;
      withLockInfo.IsDeleted = field.IsDeleted;
      withLockInfo.IsLocked = field.IsLocked;
      return withLockInfo;
    }

    private static FieldType GetFieldType(InternalFieldType fieldType)
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
          throw new ArgumentException("InternalFieldType");
      }
    }

    private static FieldUsage GetFieldUsage(InternalFieldUsages internalFieldUsage)
    {
      switch (internalFieldUsage)
      {
        case InternalFieldUsages.None:
          return FieldUsage.None;
        case InternalFieldUsages.WorkItem:
          return FieldUsage.WorkItem;
        case InternalFieldUsages.WorkItemLink:
          return FieldUsage.WorkItemLink;
        case InternalFieldUsages.Tree:
          return FieldUsage.Tree;
        case InternalFieldUsages.WorkItemTypeExtension:
          return FieldUsage.WorkItemTypeExtension;
        default:
          return FieldUsage.None;
      }
    }

    private static bool IsReadOnly(FieldEntry field)
    {
      switch (field.ReferenceName)
      {
        case "System.ChangedDate":
        case "System.CreatedDate":
        case "System.CreatedBy":
          return true;
        case "System.AreaPath":
        case "System.IterationPath":
          return false;
        default:
          return field.IsReadOnly;
      }
    }
  }
}
