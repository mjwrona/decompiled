// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionType
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models
{
  public static class RuleConditionType
  {
    public const string When = "$when";
    public const string WhenWas = "$whenWas";
    public const string WhenNot = "$whenNot";
    public const string WhenChanged = "$whenChanged";
    public const string WhenNotChanged = "$whenNotChanged";
    public static readonly IReadOnlyDictionary<string, int> SortOrder = (IReadOnlyDictionary<string, int>) new Dictionary<string, int>()
    {
      {
        "$whenWas",
        1
      },
      {
        "$when",
        2
      },
      {
        "$whenNot",
        3
      },
      {
        "$whenChanged",
        4
      },
      {
        "$whenNotChanged",
        5
      }
    };
  }
}
