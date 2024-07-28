// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.DistributedTaskApiControllerHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  internal static class DistributedTaskApiControllerHelper
  {
    internal static IList<int> ParseArray(string array, char delimiter = ',')
    {
      if (string.IsNullOrEmpty(array))
        return (IList<int>) Array.Empty<int>();
      List<int> array1 = new List<int>();
      string str = array;
      char[] separator = new char[1]{ delimiter };
      foreach (string s in str.Split(separator, StringSplitOptions.RemoveEmptyEntries))
        array1.Add(int.Parse(s, NumberStyles.Integer));
      return (IList<int>) array1;
    }
  }
}
