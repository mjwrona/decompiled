// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WorkItemHub.WorkItemChangedHelper
// Assembly: Microsoft.Azure.Boards.WorkItemHub, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 749A696A-54F8-4B6F-8877-B350F1725C24
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.WorkItemHub.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Boards.WorkItemHub
{
  internal static class WorkItemChangedHelper
  {
    public static ChangeType GetChangeType(WorkItemChangedEvent workItemChangedEvent)
    {
      ArgumentUtility.CheckForNull<WorkItemChangedEvent>(workItemChangedEvent, nameof (workItemChangedEvent));
      ChangeTypes actualValue = workItemChangedEvent.ChangeType;
      if (workItemChangedEvent.ChangeType == ChangeTypes.Change && WorkItemChangedHelper.IsDeletedFieldChanged(workItemChangedEvent))
        actualValue = WorkItemChangedHelper.GetCoreBooleanField(workItemChangedEvent, "System.IsDeleted") ? ChangeTypes.Delete : ChangeTypes.Restore;
      switch (actualValue)
      {
        case ChangeTypes.New:
          return ChangeType.New;
        case ChangeTypes.Change:
          return ChangeType.Change;
        case ChangeTypes.Delete:
          return ChangeType.Delete;
        case ChangeTypes.Restore:
          return ChangeType.Restore;
        default:
          throw new ArgumentOutOfRangeException("changeType", (object) actualValue, "New ChangeTypes enum not handled");
      }
    }

    private static bool GetCoreBooleanField(
      WorkItemChangedEvent workItemChangedEvent,
      string fieldReferenceName)
    {
      ArgumentUtility.CheckForNull<WorkItemChangedEvent>(workItemChangedEvent, nameof (workItemChangedEvent));
      ArgumentUtility.CheckForNull<string>(fieldReferenceName, nameof (fieldReferenceName));
      BooleanField booleanField = ((IEnumerable<BooleanField>) workItemChangedEvent.CoreFields.BooleanFields).First<BooleanField>((Func<BooleanField, bool>) (fld => fld.ReferenceName.Equals(fieldReferenceName, StringComparison.Ordinal)));
      return booleanField != null && booleanField.NewValue;
    }

    public static int GetCoreIntField(
      WorkItemChangedEvent workItemChangedEvent,
      string fieldReferenceName)
    {
      ArgumentUtility.CheckForNull<WorkItemChangedEvent>(workItemChangedEvent, nameof (workItemChangedEvent));
      ArgumentUtility.CheckForNull<string>(fieldReferenceName, nameof (fieldReferenceName));
      IntegerField integerField = ((IEnumerable<IntegerField>) workItemChangedEvent.CoreFields.IntegerFields).First<IntegerField>((Func<IntegerField, bool>) (fld => fld.ReferenceName.Equals(fieldReferenceName, StringComparison.Ordinal)));
      return integerField == null ? 0 : integerField.NewValue;
    }

    private static bool IsDeletedFieldChanged(WorkItemChangedEvent workItemChangedEvent)
    {
      bool flag = false;
      if (workItemChangedEvent.ChangedFields != null && workItemChangedEvent.ChangedFields.BooleanFields != null)
        flag = ((IEnumerable<BooleanField>) workItemChangedEvent.ChangedFields.BooleanFields).Where<BooleanField>((Func<BooleanField, bool>) (fld => fld.ReferenceName.Equals("System.IsDeleted", StringComparison.Ordinal))).Any<BooleanField>();
      return flag;
    }
  }
}
