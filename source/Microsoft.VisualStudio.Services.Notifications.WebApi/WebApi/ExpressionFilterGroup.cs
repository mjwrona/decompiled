// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.ExpressionFilterGroup
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  [DataContract]
  public class ExpressionFilterGroup
  {
    public ExpressionFilterGroup()
    {
    }

    public ExpressionFilterGroup(int firstRow, int lastRow)
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

    public bool Contains(ExpressionFilterGroup group) => this.Start <= group.Start && this.End >= group.End;

    public bool Contains(int row) => this.Start <= row && this.End >= row;

    public static int AssignLevels(IEnumerable<ExpressionFilterGroup> groups)
    {
      foreach (ExpressionFilterGroup group in groups)
        group.Level = 0;
      foreach (ExpressionFilterGroup group in groups)
        ExpressionFilterGroup.AssignLevels(groups, group);
      return ExpressionFilterGroup.MaxLevel(groups);
    }

    private static void AssignLevels(
      IEnumerable<ExpressionFilterGroup> groups,
      ExpressionFilterGroup group)
    {
      foreach (ExpressionFilterGroup group1 in groups)
      {
        if (group1 != group && group.Contains(group1))
        {
          ExpressionFilterGroup.AssignLevels(groups, group1);
          group.Level = Math.Max(group.Level, group1.Level + 1);
        }
      }
    }

    private static int MaxLevel(IEnumerable<ExpressionFilterGroup> groups) => groups.Any<ExpressionFilterGroup>() ? groups.Max<ExpressionFilterGroup>((Func<ExpressionFilterGroup, int>) (g => g.Level)) : 0;

    public static IEnumerable<ExpressionFilterGroup> FindContaining(
      IEnumerable<ExpressionFilterGroup> groups,
      int row)
    {
      return (IEnumerable<ExpressionFilterGroup>) groups.Where<ExpressionFilterGroup>((Func<ExpressionFilterGroup, bool>) (g => g.Contains(row))).OrderByDescending<ExpressionFilterGroup, int>((Func<ExpressionFilterGroup, int>) (g => g.Level));
    }
  }
}
