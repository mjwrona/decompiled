// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.Utils.RestUtility
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.Utils
{
  internal static class RestUtility
  {
    public static string GetFirstHeaderValue<T>(IEnumerable<T> headerValues) where T : class
    {
      if (headerValues != null)
      {
        T obj = headerValues.FirstOrDefault<T>();
        if ((object) obj != null)
          return obj.ToString().TrimStart();
      }
      return (string) null;
    }

    public static TimeSpan MaxTimeSpan(TimeSpan val1, TimeSpan val2) => !(val1 > val2) ? val2 : val1;

    internal static T RunWithoutSynchronizationContext<T>(Func<T> actionToRun)
    {
      SynchronizationContext current = SynchronizationContext.Current;
      try
      {
        SynchronizationContext.SetSynchronizationContext((SynchronizationContext) null);
        return actionToRun();
      }
      finally
      {
        SynchronizationContext.SetSynchronizationContext(current);
      }
    }
  }
}
