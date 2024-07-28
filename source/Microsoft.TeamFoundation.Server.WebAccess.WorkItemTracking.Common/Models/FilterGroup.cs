// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterGroup
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  public class FilterGroup
  {
    public FilterGroup()
    {
    }

    public FilterGroup(int firstRow, int lastRow)
    {
      this.Start = firstRow;
      this.End = lastRow;
    }

    [DataMember(Name = "start", EmitDefaultValue = false)]
    public int Start { get; set; }

    [DataMember(Name = "end", EmitDefaultValue = false)]
    public int End { get; set; }

    [DataMember(Name = "level", EmitDefaultValue = false)]
    public int Level { get; set; }

    public bool Contains(FilterGroup group) => this.Start <= group.Start && this.End >= group.End;

    public bool Contains(int row) => this.Start <= row && this.End >= row;

    public static int AssignLevels(IEnumerable<FilterGroup> groups)
    {
      foreach (FilterGroup group in groups)
        group.Level = 0;
      foreach (FilterGroup group in groups)
        FilterGroup.AssignLevels(groups, group);
      return FilterGroup.MaxLevel(groups);
    }

    private static void AssignLevels(IEnumerable<FilterGroup> groups, FilterGroup group)
    {
      foreach (FilterGroup group1 in groups)
      {
        if (group1 != group && group.Contains(group1))
        {
          FilterGroup.AssignLevels(groups, group1);
          group.Level = Math.Max(group.Level, group1.Level + 1);
        }
      }
    }

    private static int MaxLevel(IEnumerable<FilterGroup> groups) => groups.Any<FilterGroup>() ? groups.Max<FilterGroup>((Func<FilterGroup, int>) (g => g.Level)) : 0;

    public static IEnumerable<FilterGroup> FindContaining(IEnumerable<FilterGroup> groups, int row) => (IEnumerable<FilterGroup>) groups.Where<FilterGroup>((Func<FilterGroup, bool>) (g => g.Contains(row))).OrderByDescending<FilterGroup, int>((Func<FilterGroup, int>) (g => g.Level));
  }
}
