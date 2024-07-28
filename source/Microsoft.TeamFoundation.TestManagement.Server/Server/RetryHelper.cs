// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.RetryHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class RetryHelper
  {
    public static void RetryOnExceptions(Action action, int retryAttempts, Type retryException) => RetryHelper.RetryOnExceptions<bool>((Func<bool>) (() =>
    {
      action();
      return true;
    }), retryAttempts, retryException);

    public static T RetryOnExceptions<T>(Func<T> func, int retryAttempts, Type retryException)
    {
      while (true)
      {
        try
        {
          return func();
        }
        catch (Exception ex)
        {
          if (ex.GetType().Equals(retryException) && retryAttempts > 0)
            --retryAttempts;
          else
            throw;
        }
      }
    }
  }
}
