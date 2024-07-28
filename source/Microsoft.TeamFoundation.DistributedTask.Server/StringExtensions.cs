// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.StringExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public static class StringExtensions
  {
    public static IList<int> ParseArray(this string array, char delimiter = ',')
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

    public static bool IsLinuxPoolName(this string poolName) => poolName.Contains("Linux");

    public static bool IsVS2015PoolName(this string poolName) => poolName.Contains("2015");

    public static bool IsVS2017PoolName(this string poolName) => poolName.Contains("2017");

    public static bool IsVS2019PoolName(this string poolName) => poolName.Contains("VS2019");

    public static bool IsWindowsContainerPoolName(this string poolName) => poolName.Contains("Windows Container");

    public static bool IsMacPoolName(this string poolName) => poolName.IndexOf("mac", StringComparison.OrdinalIgnoreCase) >= 0;

    public static bool IsHostedUbuntu16Name(this string poolName) => poolName.Contains("Hosted Ubuntu 1604");
  }
}
