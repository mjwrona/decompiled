// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Extensions
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System.Collections.Generic;
using System.Text;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal static class Extensions
  {
    public static string ListItems<T>(this List<T> items)
    {
      StringBuilder stringBuilder = new StringBuilder("[");
      if (items != null)
      {
        stringBuilder.Append("Array(").Append(items.Count).Append("): ");
        int num = 0;
        foreach (T obj in items)
        {
          stringBuilder.Append((object) obj);
          ++num;
          if (num < items.Count)
          {
            stringBuilder.Append(',');
            if (num >= 10)
            {
              stringBuilder.Append("...");
              break;
            }
          }
          else
            break;
        }
      }
      return stringBuilder.Append(']').ToString();
    }
  }
}
