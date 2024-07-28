// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureFrontDoor.AfdRetryHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureFrontDoor
{
  public static class AfdRetryHelper
  {
    private const int c_maxRetryCount = 10;

    public static T ExecuteWithRetries<T>(Func<T> action, ITFLogger logger)
    {
      int num = 0;
      while (true)
      {
        try
        {
          return action();
        }
        catch (TaskCanceledException ex) when (num++ < 10)
        {
          logger.Info("Operation was canceled. Retrying... ({0})", (object) num);
        }
      }
    }

    public static void ExecuteWithRetries(Action action, ITFLogger logger)
    {
      int num = 0;
      while (true)
      {
        try
        {
          action();
          break;
        }
        catch (TaskCanceledException ex) when (num++ < 10)
        {
          logger.Info("Operation was canceled. Retrying... ({0})", (object) num);
        }
      }
    }
  }
}
