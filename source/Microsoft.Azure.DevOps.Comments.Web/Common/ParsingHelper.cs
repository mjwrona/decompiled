// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.Comments.Web.Common.ParsingHelper
// Assembly: Microsoft.Azure.DevOps.Comments.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6538262-E3F2-45F5-B799-587642D68EAC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.Comments.Web.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.Comments.Web.Common
{
  internal class ParsingHelper
  {
    public static string[] ParseCommaSeparatedString(string strings)
    {
      if (string.IsNullOrEmpty(strings))
        return new string[0];
      return strings.Split(',');
    }

    public static ICollection<int> ParseIds(string ids)
    {
      List<string> stringList = new List<string>();
      List<int> ids1 = new List<int>();
      string str = ids;
      char[] chArray = new char[1]{ ',' };
      foreach (string s in str.Split(chArray))
      {
        int result;
        if (int.TryParse(s, out result) && result > 0)
          ids1.Add(result);
        else
          stringList.Add(s);
      }
      if (stringList.Any<string>())
        throw new VssPropertyValidationException(nameof (ids), ResourceStrings.IdsOutOfRange((object) string.Join(",", (IEnumerable<string>) stringList)));
      return (ICollection<int>) ids1;
    }
  }
}
