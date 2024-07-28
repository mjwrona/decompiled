// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestManagementRetryHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  public class TestManagementRetryHelper
  {
    private readonly int m_maxRetries;
    private readonly Func<Exception, bool> m_canRetryDelegate;
    private readonly TimeSpan m_minBackoff = TimeSpan.FromSeconds(10.0);
    private readonly TimeSpan m_maxBackoff = TimeSpan.FromMinutes(2.0);
    private readonly TimeSpan m_deltaBackoff = TimeSpan.FromSeconds(3.0);

    public TestManagementRetryHelper(
      int maxRetries,
      TimeSpan? minBackoff = null,
      TimeSpan? maxBackoff = null,
      TimeSpan? deltaBackoff = null,
      Func<Exception, bool> canRetryDelegate = null)
    {
      this.m_maxRetries = maxRetries;
      if (minBackoff.HasValue)
        this.m_minBackoff = minBackoff.Value;
      if (maxBackoff.HasValue)
        this.m_maxBackoff = maxBackoff.Value;
      if (deltaBackoff.HasValue)
        this.m_deltaBackoff = deltaBackoff.Value;
      this.m_canRetryDelegate = canRetryDelegate;
    }

    public async Task<T> Invoke<T>(Func<Task<T>> function)
    {
      int remainingRetries = this.m_maxRetries;
      T obj;
      while (true)
      {
        int num;
        do
        {
          try
          {
            obj = await function().ConfigureAwait(false);
            goto label_11;
          }
          catch (Exception ex)
          {
            num = 1;
          }
        }
        while (num != 1);
        Exception ex1 = ex;
        if ((this.IsTransientNetworkException(ex1) || this.m_canRetryDelegate != null && this.m_canRetryDelegate(ex1)) && remainingRetries > 1)
        {
          await this.Sleep(remainingRetries).ConfigureAwait(false);
          --remainingRetries;
        }
        else if ((object) ex is Exception source)
          ExceptionDispatchInfo.Capture(source).Throw();
        else
          break;
      }
      throw (object) ex;
label_11:
      return obj;
    }

    protected virtual Task Sleep(int remainingRetries) => Task.Delay(BackoffTimerHelper.GetExponentialBackoff(this.m_maxRetries - remainingRetries + 1, this.m_minBackoff, this.m_maxBackoff, this.m_deltaBackoff));

    private bool IsTransientNetworkException(Exception ex) => ex is RequestBlockedException;
  }
}
