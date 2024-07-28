// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.DataAccess.GroupKeyHelper
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Reporting.DataServices.DataAccess
{
  public static class GroupKeyHelper
  {
    public static readonly int MaxLengthForGroupKey = 73;
    public static readonly string WITChartScope = "WorkitemTracking.Queries";

    public static IEnumerable<string> updateGroupKeys(IEnumerable<string> groupKeys, string scope)
    {
      if (scope != GroupKeyHelper.WITChartScope)
        return groupKeys;
      string[] array = groupKeys.ToArray<string>();
      for (int index = 0; index < array.Length; ++index)
        array[index] = GroupKeyHelper.updateGroupKey(array[index]);
      return (IEnumerable<string>) array;
    }

    public static string updateGroupKey(string groupKey, string scope) => scope != GroupKeyHelper.WITChartScope ? groupKey : GroupKeyHelper.updateGroupKey(groupKey);

    public static string updateGroupKey(string groupKey) => string.IsNullOrEmpty(groupKey) || groupKey.Length <= GroupKeyHelper.MaxLengthForGroupKey ? groupKey : groupKey.Substring(0, GroupKeyHelper.MaxLengthForGroupKey);
  }
}
