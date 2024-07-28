// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemTrackingUtils
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class WorkItemTrackingUtils
  {
    private const string fieldSeparator = "','";

    public static string EscapeWiqlFieldValue(string fieldValue) => fieldValue.Replace("'", "''");

    public static IReadOnlyCollection<string> EscapeWiqlFieldValues(IEnumerable<string> fieldValues) => (IReadOnlyCollection<string>) fieldValues.Select<string, string>((Func<string, string>) (str => WorkItemTrackingUtils.EscapeWiqlFieldValue(str))).ToList<string>();

    public static string FieldValuesToInClause(
      string comparandField,
      IReadOnlyCollection<string> fieldValues)
    {
      if (fieldValues.Count == 0)
        return string.Empty;
      return fieldValues.Count == 1 ? "[" + comparandField + "] = '" + WorkItemTrackingUtils.EscapeWiqlFieldValue(fieldValues.First<string>()) + "'" : "[" + comparandField + "] IN ('" + string.Join("','", (IEnumerable<string>) WorkItemTrackingUtils.EscapeWiqlFieldValues((IEnumerable<string>) fieldValues)) + "')";
    }
  }
}
