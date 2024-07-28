// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.RestHelpers
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class RestHelpers
  {
    public static IList<Guid> ToGuidList(string value)
    {
      if (string.IsNullOrEmpty(value))
        return (IList<Guid>) Array.Empty<Guid>();
      List<Guid> guidList = new List<Guid>();
      string str = value;
      char[] separator = new char[1]{ ',' };
      foreach (string input in str.Split(separator, StringSplitOptions.RemoveEmptyEntries))
      {
        Guid result;
        if (!Guid.TryParse(input, out result))
          throw new InvalidBuildQueryException(BuildServerResources.InvalidGuidValue((object) input));
        guidList.Add(result);
      }
      return (IList<Guid>) guidList;
    }

    public static IList<int> ToInt32List(string value)
    {
      if (string.IsNullOrEmpty(value))
        return (IList<int>) null;
      List<int> intList = new List<int>();
      string str = value;
      char[] separator = new char[1]{ ',' };
      foreach (string s in str.Split(separator, StringSplitOptions.RemoveEmptyEntries))
      {
        int result;
        if (!int.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
          throw new InvalidBuildQueryException(BuildServerResources.InvalidIntegerValue((object) s));
        intList.Add(result);
      }
      return intList.Count != 0 ? (IList<int>) intList : (IList<int>) null;
    }

    public static IList<string> ToStringList(string value)
    {
      if (string.IsNullOrEmpty(value))
        return (IList<string>) null;
      string[] strArray = value.Split(new char[1]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
      return strArray.Length != 0 ? (IList<string>) strArray : (IList<string>) null;
    }
  }
}
