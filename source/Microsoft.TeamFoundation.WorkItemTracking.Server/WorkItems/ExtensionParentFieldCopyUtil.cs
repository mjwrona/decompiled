// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.ExtensionParentFieldCopyUtil
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.UpdateState;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class ExtensionParentFieldCopyUtil
  {
    public static void UpdateCurrentExtensions(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemTypeExtension> extensions,
      WorkItemUpdateState updateState)
    {
      ExtensionParentFieldCopyUtil.UpdateCurrentExtensionParentFields(witRequestContext, extensions, updateState);
    }

    public static void UpdateDetachedExtensions(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemTypeExtension> extensions,
      WorkItemUpdateState updateState)
    {
      ExtensionParentFieldCopyUtil.UpdateDetachedExtensionParentFields(witRequestContext, extensions, updateState);
    }

    private static void UpdateCurrentExtensionParentFields(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemTypeExtension> extensions,
      WorkItemUpdateState updateState)
    {
      foreach (WorkItemTypelet workItemTypelet in (IEnumerable<WorkItemTypeExtension>) extensions.OrderBy<WorkItemTypeExtension, int>((Func<WorkItemTypeExtension, int>) (ex => ex.Rank)))
      {
        foreach (WorkItemTypeExtensionFieldEntry field in workItemTypelet.Fields)
        {
          if (field.Field.ParentFieldId != 0 && witRequestContext.RequestContext.GetService<WorkItemTrackingFieldService>().GetFieldById(witRequestContext.RequestContext, field.Field.ParentFieldId) != null)
          {
            object fieldValue = updateState.FieldData.GetFieldValue(witRequestContext, field.Field.FieldId);
            updateState.FieldData.SetFieldValue(witRequestContext, field.Field.ParentFieldId, fieldValue);
          }
        }
      }
    }

    private static void UpdateDetachedExtensionParentFields(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemTypeExtension> extensions,
      WorkItemUpdateState updateState)
    {
      foreach (WorkItemTypelet extension in extensions)
      {
        foreach (WorkItemTypeExtensionFieldEntry field in extension.Fields)
        {
          if (field.Field.ParentFieldId != 0 && witRequestContext.RequestContext.GetService<WorkItemTrackingFieldService>().GetFieldById(witRequestContext.RequestContext, field.Field.ParentFieldId) != null)
            updateState.FieldData.SetFieldValue(witRequestContext, field.Field.ParentFieldId, (object) null);
        }
      }
    }
  }
}
