// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldUsage
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class FieldUsage
  {
    public FieldDefinition FieldDefinition { get; private set; }

    public FieldDefinition Object { get; private set; }

    public FieldDefinition DirectObject { get; private set; }

    public bool IsCore { get; private set; }

    public bool OftenQueried { get; private set; }

    internal static void AttachFieldUsage(
      FieldUsageRecord usage,
      IDictionary<int, FieldDefinition> fieldMap)
    {
      FieldUsage fieldUsage = new FieldUsage()
      {
        OftenQueried = usage.OftenQueried,
        IsCore = usage.Core
      };
      FieldDefinition fieldDefinition1;
      if (!fieldMap.TryGetValue(usage.ObjectId, out fieldDefinition1))
        return;
      fieldUsage.Object = fieldDefinition1;
      FieldDefinition fieldDefinition2;
      if (!fieldMap.TryGetValue(usage.FieldId, out fieldDefinition2))
        return;
      fieldUsage.FieldDefinition = fieldDefinition2;
      fieldDefinition2.Usages |= FieldHelpers.GetFieldUsage(fieldDefinition1.Id);
      fieldDefinition2.SupportsTextQuery |= usage.SupportsTextQuery;
      FieldDefinition fieldDefinition3;
      if (usage.DirectObjectId != 0 && fieldMap.TryGetValue(usage.DirectObjectId, out fieldDefinition3))
        fieldUsage.DirectObject = fieldDefinition3;
      fieldDefinition1.FieldUsages.Add(fieldUsage);
    }
  }
}
