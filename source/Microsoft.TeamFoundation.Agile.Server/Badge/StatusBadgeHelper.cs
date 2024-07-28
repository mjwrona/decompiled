// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.Badge.StatusBadgeHelper
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Agile.Server.Badge
{
  public static class StatusBadgeHelper
  {
    private const string c_boardBadgeRightBackground = "#008b74";

    public static XDocument GetSVG(
      IVssRequestContext requestContext,
      IList<string> columnNames,
      IDictionary<string, string> columnCounts)
    {
      BadgeOptions options = new BadgeOptions(BadgeLogo.Boards, AgileResources.BoardBadgeLeftText);
      if (columnNames.Count <= 0)
      {
        options.RightText = AgileResources.BoardBadgeClickToSeeTheBoard;
        options.RightBackground = "#BBBBBB";
      }
      else
      {
        StringBuilder stringBuilder = new StringBuilder();
        for (int index = 0; index < columnNames.Count; ++index)
        {
          string columnName = columnNames[index];
          string str = columnCounts.ContainsKey(columnName) ? columnCounts[columnName] : "0";
          stringBuilder.AppendFormat("{0} {1}", (object) columnName, (object) str);
          if (index < columnNames.Count - 1)
            stringBuilder.Append(" | ");
        }
        options.RightText = stringBuilder.ToString();
        options.RightBackground = "#008b74";
      }
      return BadgeSvgGenerator.CreateImage(requestContext, ref options);
    }
  }
}
