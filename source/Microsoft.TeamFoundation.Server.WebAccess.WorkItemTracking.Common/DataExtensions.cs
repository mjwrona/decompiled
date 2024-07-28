// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal static class DataExtensions
  {
    public static IEnumerable<T> Bind<T>(this DbDataReader reader, Func<DbDataReader, T> binder)
    {
      while (reader.Read())
        yield return binder(reader);
    }

    public static IEnumerable<T> Bind<T, THelper>(
      this DbDataReader reader,
      Func<DbDataReader, THelper, T> binder,
      THelper helper)
    {
      while (reader.Read())
        yield return binder(reader, helper);
    }

    public static int Walk(this DbDataReader reader, Action<DbDataReader> action)
    {
      int num = 0;
      while (reader.Read())
      {
        ++num;
        action(reader);
      }
      return num;
    }

    public static int Walk<THelper>(
      this DbDataReader reader,
      Action<DbDataReader, THelper> action,
      THelper helper)
    {
      int num = 0;
      while (reader.Read())
      {
        ++num;
        action(reader, helper);
      }
      return num;
    }
  }
}
