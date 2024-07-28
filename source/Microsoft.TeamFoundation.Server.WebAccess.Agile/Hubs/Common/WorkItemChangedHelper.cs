// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.Common.WorkItemChangedHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.Common
{
  internal static class WorkItemChangedHelper
  {
    private const string StackRankFieldRefName = "Microsoft.VSTS.Common.StackRank";

    public static ChangeTypes GetChangeType(WorkItemChangedEvent workItemChangedEvent)
    {
      ArgumentUtility.CheckForNull<WorkItemChangedEvent>(workItemChangedEvent, "workItemChangeEvent");
      ChangeTypes changeType = workItemChangedEvent.ChangeType;
      if (workItemChangedEvent.ChangeType == ChangeTypes.Change && WorkItemChangedHelper.IsDeletedFieldChanged(workItemChangedEvent))
        changeType = WorkItemChangedHelper.GetBooleanField(workItemChangedEvent, "System.IsDeleted") ? ChangeTypes.Delete : ChangeTypes.Restore;
      return changeType;
    }

    public static bool HasChangedChildren(WorkItemChangedEvent workItemChangedEvent)
    {
      ArgumentUtility.CheckForNull<WorkItemChangedEvent>(workItemChangedEvent, "workItemChangeEvent");
      if (workItemChangedEvent.ChangeType != ChangeTypes.Change)
        return false;
      return WorkItemChangedHelper.HasChangedChildren((IWorkItemChangedRelation[]) workItemChangedEvent.AddedRelations) || WorkItemChangedHelper.HasChangedChildren((IWorkItemChangedRelation[]) workItemChangedEvent.DeletedRelations) || WorkItemChangedHelper.HasChangedChildren((IWorkItemChangedRelation[]) workItemChangedEvent.ChangedRelations);
    }

    public static bool HasChangedTypeOrTitle(WorkItemChangedEvent workItemChangedEvent)
    {
      ArgumentUtility.CheckForNull<WorkItemChangedEvent>(workItemChangedEvent, "workItemChangeEvent");
      return workItemChangedEvent.ChangeType == ChangeTypes.Change && workItemChangedEvent.ChangedFields != null && ((IEnumerable<StringField>) workItemChangedEvent.ChangedFields.StringFields).Any<StringField>((System.Func<StringField, bool>) (field => TFStringComparer.WorkItemFieldReferenceName.Equals(field.ReferenceName, CoreFieldReferenceNames.Title) || TFStringComparer.WorkItemFieldReferenceName.Equals(field.ReferenceName, CoreFieldReferenceNames.WorkItemType) || TFStringComparer.WorkItemFieldReferenceName.Equals(field.ReferenceName, CoreFieldReferenceNames.TeamProject)));
    }

    private static bool HasChangedChildren(IWorkItemChangedRelation[] changes) => changes != null && ((IEnumerable<IWorkItemChangedRelation>) changes).Any<IWorkItemChangedRelation>((System.Func<IWorkItemChangedRelation, bool>) (change => change.IsBacklogChildLink));

    public static bool GetBooleanField(
      WorkItemChangedEvent workItemChangedEvent,
      string fieldReferenceName)
    {
      ArgumentUtility.CheckForNull<WorkItemChangedEvent>(workItemChangedEvent, "WorkItemChangedEvent");
      ArgumentUtility.CheckForNull<string>(fieldReferenceName, "FieldReferenceName");
      BooleanField booleanField = ((IEnumerable<BooleanField>) workItemChangedEvent.CoreFields.BooleanFields).First<BooleanField>((System.Func<BooleanField, bool>) (fld => fld.ReferenceName.Equals(fieldReferenceName, StringComparison.Ordinal)));
      return booleanField != null && booleanField.NewValue;
    }

    public static int GetIntField(
      WorkItemChangedEvent workItemChangedEvent,
      string fieldReferenceName)
    {
      ArgumentUtility.CheckForNull<WorkItemChangedEvent>(workItemChangedEvent, "WorkItemChangedEvent");
      ArgumentUtility.CheckForNull<string>(fieldReferenceName, "FieldReferenceName");
      IntegerField integerField = ((IEnumerable<IntegerField>) workItemChangedEvent.CoreFields.IntegerFields).First<IntegerField>((System.Func<IntegerField, bool>) (fld => fld.ReferenceName.Equals(fieldReferenceName, StringComparison.Ordinal)));
      return integerField == null ? 0 : integerField.NewValue;
    }

    public static string GetStringField(
      WorkItemChangedEvent workItemChangedEvent,
      string fieldReferenceName)
    {
      ArgumentUtility.CheckForNull<WorkItemChangedEvent>(workItemChangedEvent, "WorkItemChangedEvent");
      ArgumentUtility.CheckForNull<string>(fieldReferenceName, "FieldReferenceName");
      return ((IEnumerable<StringField>) workItemChangedEvent.CoreFields.StringFields).First<StringField>((System.Func<StringField, bool>) (fld => fld.ReferenceName.Equals(fieldReferenceName, StringComparison.Ordinal)))?.NewValue;
    }

    public static string GetCoreOrCustomFieldValue(
      IVssRequestContext requestContext,
      WorkItemChangedEvent workItemChangedEvent,
      string fieldReferenceName)
    {
      ArgumentUtility.CheckForNull<WorkItemChangedEvent>(workItemChangedEvent, "WorkItemChangedEvent");
      ArgumentUtility.CheckForNull<string>(fieldReferenceName, "FieldReferenceName");
      StringField stringField = ((IEnumerable<StringField>) workItemChangedEvent.CoreFields.StringFields).FirstOrDefault<StringField>((System.Func<StringField, bool>) (fld => fld.ReferenceName.Equals(fieldReferenceName, StringComparison.Ordinal)));
      if (stringField == null && workItemChangedEvent.ChangedFields != null)
        stringField = ((IEnumerable<StringField>) workItemChangedEvent.ChangedFields.StringFields).FirstOrDefault<StringField>((System.Func<StringField, bool>) (fld => fld.ReferenceName.Equals(fieldReferenceName, StringComparison.Ordinal)));
      if (stringField == null)
      {
        WebAccessWorkItemService service = requestContext.GetService<WebAccessWorkItemService>();
        int result;
        if (int.TryParse(workItemChangedEvent.WorkItemId, out result))
        {
          IDataRecord dataRecord = service.GetWorkItems(requestContext, (IEnumerable<int>) new int[1]
          {
            result
          }, (IEnumerable<string>) new string[1]
          {
            fieldReferenceName
          }).ToList<IDataRecord>().FirstOrDefault<IDataRecord>();
          if (dataRecord != null)
          {
            object obj = dataRecord[fieldReferenceName];
            if (obj != null)
              return obj.ToString();
          }
        }
      }
      return stringField?.NewValue;
    }

    public static (int ParentId, ParentChangeType ChangeStatus) GetParentChangedStatus(
      WorkItemChangedEvent workItemChangedEvent)
    {
      ArgumentUtility.CheckForNull<WorkItemChangedEvent>(workItemChangedEvent, "WorkItemChangedEvent");
      AddedRelation[] addedRelations = workItemChangedEvent.AddedRelations;
      AddedRelation addedRelation = addedRelations != null ? ((IEnumerable<AddedRelation>) addedRelations).FirstOrDefault<AddedRelation>((System.Func<AddedRelation, bool>) (relation => relation.IsBacklogParentLink)) : (AddedRelation) null;
      int result;
      if (addedRelation != null && int.TryParse(addedRelation.WorkItemId, out result))
        return (result, ParentChangeType.Added);
      DeletedRelation[] deletedRelations = workItemChangedEvent.DeletedRelations;
      return (deletedRelations != null ? ((IEnumerable<DeletedRelation>) deletedRelations).FirstOrDefault<DeletedRelation>((System.Func<DeletedRelation, bool>) (relation => relation.IsBacklogParentLink)) : (DeletedRelation) null) != null ? (0, ParentChangeType.Removed) : (0, ParentChangeType.Unchanged);
    }

    public static string GetStackRankAssociatedWithTheChange(
      WorkItemChangedEvent workItemChangedEvent)
    {
      ArgumentUtility.CheckForNull<WorkItemChangedEvent>(workItemChangedEvent, "WorkItemChangedEvent");
      if (!WorkItemChangedHelper.IsOnlyStackRankChange(workItemChangedEvent))
        return string.Empty;
      StringField stringField = ((IEnumerable<StringField>) workItemChangedEvent.ChangedFields.StringFields).FirstOrDefault<StringField>((System.Func<StringField, bool>) (fld => fld.ReferenceName.Equals("Microsoft.VSTS.Common.StackRank", StringComparison.Ordinal)));
      return stringField == null ? string.Empty : stringField.NewValue;
    }

    private static bool IsOnlyStackRankChange(WorkItemChangedEvent workItemChangedEvent) => workItemChangedEvent.ChangedFields != null && ((IEnumerable<BooleanField>) workItemChangedEvent.ChangedFields.BooleanFields).IsNullOrEmpty<BooleanField>() && !((IEnumerable<IntegerField>) workItemChangedEvent.ChangedFields.IntegerFields).IsNullOrEmpty<IntegerField>() && !((IEnumerable<StringField>) workItemChangedEvent.ChangedFields.StringFields).IsNullOrEmpty<StringField>() && workItemChangedEvent.ChangedFields.IntegerFields.Length == 2 && workItemChangedEvent.ChangedFields.StringFields.Length == 3 && ((IEnumerable<StringField>) workItemChangedEvent.ChangedFields.StringFields).Any<StringField>((System.Func<StringField, bool>) (x => x.ReferenceName.Equals("Microsoft.VSTS.Common.StackRank", StringComparison.Ordinal)));

    private static bool IsDeletedFieldChanged(WorkItemChangedEvent workItemChangedEvent)
    {
      bool flag = false;
      if (workItemChangedEvent.ChangedFields != null && workItemChangedEvent.ChangedFields.BooleanFields != null)
        flag = ((IEnumerable<BooleanField>) workItemChangedEvent.ChangedFields.BooleanFields).Where<BooleanField>((System.Func<BooleanField, bool>) (fld => fld.ReferenceName.Equals("System.IsDeleted", StringComparison.Ordinal))).Any<BooleanField>();
      return flag;
    }
  }
}
