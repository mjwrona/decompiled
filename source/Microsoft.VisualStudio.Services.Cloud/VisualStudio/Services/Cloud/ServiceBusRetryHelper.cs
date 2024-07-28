// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusRetryHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.ServiceBus.Messaging;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class ServiceBusRetryHelper
  {
    internal static T ExecuteWithRetries<T>(Func<T> action, int retryCount = 4, int retryDelay = 10)
    {
      ArgumentUtility.CheckForNonPositiveInt(retryCount, nameof (retryCount));
      int num1 = 0;
      int num2 = retryCount + 1;
      while (true)
      {
        try
        {
          return action();
        }
        catch (TimeoutException ex)
        {
          if (++num1 >= num2)
            throw;
        }
        catch (MessagingException ex)
        {
          int num3;
          switch (ex)
          {
            case MessagingEntityAlreadyExistsException _:
            case ServerBusyException _:
            case MessagingEntityNotFoundException _:
              num3 = 1;
              break;
            default:
              num3 = ex.IsTransient ? 1 : 0;
              break;
          }
          if (num3 != 0)
          {
            if (++num1 < num2)
              goto label_11;
          }
          throw;
        }
label_11:
        Thread.Sleep(TimeSpan.FromSeconds((double) (num1 * num1 + retryDelay)));
      }
    }

    internal static void ExecuteWithRetries(Action action, int retryCount = 4, int retryDelay = 10) => ServiceBusRetryHelper.ExecuteWithRetries<int>((Func<int>) (() =>
    {
      try
      {
        action();
      }
      catch (MessagingEntityAlreadyExistsException ex)
      {
      }
      return 0;
    }), retryCount, retryDelay);
  }
}
