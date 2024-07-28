// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.SqlErrorExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
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
