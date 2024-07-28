// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.HttpRetryHelper
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.Threading;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class HttpRetryHelper
  {
    private int m_maxAttempts;
    private Func<Exception, bool> m_canRetryDelegate;
    private static TimeSpan s_minBackoff = TimeSpan.FromSeconds(1.0);
    private static TimeSpan s_maxBackoff = TimeSpan.FromMinutes(1.0);
    private static TimeSpan s_deltaBackoff = TimeSpan.FromSeconds(1.0);

    public HttpRetryHelper(int maxAttempts, Func<Exception, bool> canRetryDelegate = null)
    {
      this.m_maxAttempts = maxAttempts;
      this.m_canRetryDelegate = canRetryDelegate;
    }

    public void Invoke(Action action) => this.Invoke(action, out int _);

    public void Invoke(Action action, out int remainingAttempts)
    {
      remainingAttempts = this.m_maxAttempts;
      while (true)
      {
        try
        {
          action();
          break;
        }
        catch (Exception ex)
        {
          if ((VssNetworkHelper.IsTransientNetworkException(ex) || this.m_canRetryDelegate != null && this.m_canRetryDelegate(ex)) && remainingAttempts > 1)
          {
            this.Sleep(remainingAttempts);
            --remainingAttempts;
          }
          else
            throw;
        }
      }
    }

    public TResult Invoke<TResult>(Func<TResult> function) => this.Invoke<TResult>(function, out int _);

    public TResult Invoke<TResult>(Func<TResult> function, out int remainingAttempts)
    {
      remainingAttempts = this.m_maxAttempts;
      while (true)
      {
        try
        {
          return function();
        }
        catch (Exception ex)
        {
          if ((VssNetworkHelper.IsTransientNetworkException(ex) || this.m_canRetryDelegate != null && this.m_canRetryDelegate(ex)) && remainingAttempts > 1)
          {
            this.Sleep(remainingAttempts);
            --remainingAttempts;
          }
          else
            throw;
        }
      }
    }

    protected virtual void Sleep(int remainingAttempts) => Thread.Sleep(BackoffTimerHelper.GetExponentialBackoff(this.m_maxAttempts - remainingAttempts + 1, HttpRetryHelper.s_minBackoff, HttpRetryHelper.s_maxBackoff, HttpRetryHelper.s_deltaBackoff));

    public int MaxAttempts => this.m_maxAttempts;
  }
}
