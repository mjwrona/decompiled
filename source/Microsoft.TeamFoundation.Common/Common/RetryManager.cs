// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.RetryManager
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Common
{
  public sealed class RetryManager
  {
    private readonly int m_retryCount;
    private readonly TimeSpan m_delay;
    private readonly Action<Exception> m_onException;
    private readonly Func<Exception, bool> m_canRetryException;

    public RetryManager(int retryCount)
      : this(retryCount, TimeSpan.Zero, (Action<Exception>) null)
    {
    }

    public RetryManager(int retryCount, Action<Exception> onException)
      : this(retryCount, TimeSpan.Zero, onException)
    {
    }

    public RetryManager(int retryCount, TimeSpan delay)
      : this(retryCount, delay, (Action<Exception>) null)
    {
    }

    public RetryManager(int retryCount, TimeSpan delay, Action<Exception> onException)
      : this(retryCount, delay, onException, (Func<Exception, bool>) null)
    {
    }

    public RetryManager(
      int retryCount,
      TimeSpan delay,
      Action<Exception> onException,
      Func<Exception, bool> canRetryException)
    {
      this.m_retryCount = retryCount;
      this.m_delay = delay;
      this.m_onException = onException;
      this.m_canRetryException = canRetryException;
    }

    public bool Try(Func<bool> func)
    {
      if (func == null)
        throw new ArgumentNullException("action");
      int retryCount = this.m_retryCount;
      while (true)
      {
        try
        {
          if (func())
            return true;
          if (--retryCount < 0)
            return false;
          if (this.m_delay != TimeSpan.Zero)
            Thread.Sleep(this.m_delay);
        }
        catch (Exception ex)
        {
          if (this.m_onException != null)
            this.m_onException(ex);
          if (this.m_canRetryException != null && !this.m_canRetryException(ex))
            throw;
          else if (--retryCount < 0)
            throw;
          else if (this.m_delay != TimeSpan.Zero)
            Thread.Sleep(this.m_delay);
        }
      }
    }

    public void Invoke(Action action)
    {
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      int retryCount = this.m_retryCount;
      while (true)
      {
        try
        {
          action();
          break;
        }
        catch (Exception ex)
        {
          if (this.m_onException != null)
            this.m_onException(ex);
          if (this.m_canRetryException != null && !this.m_canRetryException(ex))
            throw;
          else if (--retryCount < 0)
            throw;
          else if (this.m_delay != TimeSpan.Zero)
            Thread.Sleep(this.m_delay);
        }
      }
    }

    public void Invoke<T>(Action<T> action, T op)
    {
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      this.Invoke((Action) (() => action(op)));
    }

    public void Invoke<T1, T2>(Action<T1, T2> action, T1 op1, T2 op2)
    {
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      this.Invoke((Action) (() => action(op1, op2)));
    }

    public void Invoke<T1, T2, T3>(Action<T1, T2, T3> action, T1 op1, T2 op2, T3 op3)
    {
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      this.Invoke((Action) (() => action(op1, op2, op3)));
    }

    public void Invoke<T1, T2, T3, T4>(
      Action<T1, T2, T3, T4> action,
      T1 op1,
      T2 op2,
      T3 op3,
      T4 op4)
    {
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      this.Invoke((Action) (() => action(op1, op2, op3, op4)));
    }

    public TResult InvokeFunc<TResult>(Func<TResult> func)
    {
      if (func == null)
        throw new ArgumentNullException(nameof (func));
      int retryCount = this.m_retryCount;
      while (true)
      {
        try
        {
          return func();
        }
        catch (Exception ex)
        {
          if (this.m_onException != null)
            this.m_onException(ex);
          if (this.m_canRetryException != null && !this.m_canRetryException(ex))
            throw;
          else if (--retryCount < 0)
            throw;
          else if (this.m_delay != TimeSpan.Zero)
            Thread.Sleep(this.m_delay);
        }
      }
    }

    public TResult InvokeFunc<T, TResult>(Func<T, TResult> func, T op) => func != null ? this.InvokeFunc<TResult>((Func<TResult>) (() => func(op))) : throw new ArgumentNullException(nameof (func));

    public TResult InvokeFunc<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 op1, T2 op2)
    {
      if (func == null)
        throw new ArgumentNullException(nameof (func));
      return this.InvokeFunc<TResult>((Func<TResult>) (() => func(op1, op2)));
    }

    public TResult InvokeFunc<T1, T2, T3, TResult>(
      Func<T1, T2, T3, TResult> func,
      T1 op1,
      T2 op2,
      T3 op3)
    {
      if (func == null)
        throw new ArgumentNullException(nameof (func));
      return this.InvokeFunc<TResult>((Func<TResult>) (() => func(op1, op2, op3)));
    }

    public TResult InvokeFunc<T1, T2, T3, T4, TResult>(
      Func<T1, T2, T3, T4, TResult> func,
      T1 op1,
      T2 op2,
      T3 op3,
      T4 op4)
    {
      if (func == null)
        throw new ArgumentNullException(nameof (func));
      return this.InvokeFunc<TResult>((Func<TResult>) (() => func(op1, op2, op3, op4)));
    }

    public async Task InvokeAsync(Func<Task> actionAsync)
    {
      if (actionAsync == null)
        throw new ArgumentNullException(nameof (actionAsync));
      int retriesRemaining = this.m_retryCount;
      while (true)
      {
        do
        {
          int num;
          do
          {
            try
            {
              await actionAsync();
              goto label_20;
            }
            catch (Exception ex)
            {
              num = 1;
            }
          }
          while (num != 1);
          Exception exception = ex;
          Action<Exception> onException = this.m_onException;
          if (onException != null)
            onException(exception);
          if (this.m_canRetryException != null && !this.m_canRetryException(exception))
          {
            if (!((object) ex is Exception source))
              throw (object) ex;
            ExceptionDispatchInfo.Capture(source).Throw();
          }
          if (--retriesRemaining < 0)
          {
            if (!((object) ex is Exception source))
              throw (object) ex;
            ExceptionDispatchInfo.Capture(source).Throw();
          }
        }
        while (!(this.m_delay != TimeSpan.Zero));
        await Task.Delay(this.m_delay);
      }
label_20:;
    }

    public async Task<TResult> InvokeFuncAsync<TResult>(Func<Task<TResult>> funcAsync)
    {
      if (funcAsync == null)
        throw new ArgumentNullException("func");
      int retriesRemaining = this.m_retryCount;
      TResult result;
      while (true)
      {
        do
        {
          int num;
          do
          {
            try
            {
              result = await funcAsync();
              goto label_20;
            }
            catch (Exception ex)
            {
              num = 1;
            }
          }
          while (num != 1);
          Exception exception = ex;
          Action<Exception> onException = this.m_onException;
          if (onException != null)
            onException(exception);
          if (this.m_canRetryException != null && !this.m_canRetryException(exception))
          {
            if (!((object) ex is Exception source))
              throw (object) ex;
            ExceptionDispatchInfo.Capture(source).Throw();
          }
          if (--retriesRemaining < 0)
          {
            if (!((object) ex is Exception source))
              throw (object) ex;
            ExceptionDispatchInfo.Capture(source).Throw();
          }
        }
        while (!(this.m_delay != TimeSpan.Zero));
        await Task.Delay(this.m_delay);
      }
label_20:
      return result;
    }

    public async Task<TResult> InvokeFuncAsync<TResult>(Func<TResult> func)
    {
      if (func == null)
        throw new ArgumentNullException(nameof (func));
      return await this.InvokeFuncAsync<TResult>((Func<Task<TResult>>) (() => Task.FromResult<TResult>(func())));
    }
  }
}
