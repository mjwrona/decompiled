// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.SqlErrorExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal static class SqlErrorExtensions
  {
    public static long ExtractLong(this SqlError error, string key)
    {
      long result;
      return long.TryParse(error.ExtractString(key), out result) ? result : -1L;
    }

    public static TaskVersion ExtractTaskVersion(this SqlError error) => new TaskVersion()
    {
      Major = error.ExtractInt("majorVersion"),
      Minor = error.ExtractInt("minorVersion"),
      Patch = error.ExtractInt("patchVersion"),
      IsTest = error.ExtractInt("isTest") > 0
    };
  }
}
