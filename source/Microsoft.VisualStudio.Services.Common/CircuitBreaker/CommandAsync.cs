// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.CommandAsync
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  public class CommandAsync
  {
    public const string DontTriggerCircuitBreaker = "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}";
    protected internal readonly ITime m_time;
    protected internal readonly IEventNotifier m_eventNotifier;
    protected internal readonly ICircuitBreaker m_circuitBreaker;
    protected internal readonly ICommandProperties m_properties;
    protected internal readonly CommandMetrics m_metrics;
    protected internal readonly CommandKey m_CommandKey;
    protected internal readonly CommandGroupKey m_CommandGroup;
    private readonly Func<Task> m_Run;
    private readonly Func<Task> m_Fallback;
    protected readonly bool m_ContinueOnCapturedContext;
    protected internal long invocationStartTime = -1;
    private static string s_classname = "Command";
    private static string s_featurearea = nameof (CircuitBreaker);

    public CommandAsync(
      CommandGroupKey group,
      Func<Task> run,
      Func<Task> fallback = null,
      bool continueOnCapturedContext = false)
      : this(new CommandSetter(group), run, fallback, continueOnCapturedContext)
    {
    }

    public CommandAsync(
      CommandSetter setter,
      Func<Task> run,
      Func<Task> fallback = null,
      bool continueOnCapturedContext = false)
      : this(setter.GroupKey, setter.CommandKey, (ICircuitBreaker) null, (ICommandProperties) new CommandPropertiesDefault(setter.CommandPropertiesDefaults), (CommandMetrics) null, (IEventNotifier) null, run, fallback, continueOnCapturedContext)
    {
      if (run == null)
        throw new ArgumentNullException(nameof (run));
    }

    protected CommandAsync(
      CommandGroupKey group,
      CommandKey key,
      ICircuitBreaker circuitBreaker,
      ICommandProperties properties,
      CommandMetrics metrics,
      IEventNotifier eventNotifier,
      Func<Task> run,
      Func<Task> fallback,
      bool continueOnCapturedContext,
      ITime time = null)
    {
      this.m_CommandGroup = !((ImmutableKey) group == (ImmutableKey) null) ? group : throw new ArgumentNullException(nameof (group));
      this.m_CommandKey = key ?? new CommandKey(this.GetType());
      this.m_properties = properties;
      this.m_eventNotifier = eventNotifier ?? (IEventNotifier) EventNotifierDefault.Instance;
      this.m_time = time ?? (ITime) ITimeDefault.Instance;
      this.m_metrics = metrics ?? CommandMetrics.GetInstance(this.m_CommandKey, this.m_CommandGroup, this.m_properties, this.m_eventNotifier, this.m_time);
      this.m_circuitBreaker = this.m_properties.CircuitBreakerDisabled ? (ICircuitBreaker) new CircuitBreakerNoOpImpl() : circuitBreaker ?? CircuitBreakerFactory.GetInstance(this.m_CommandKey, this.m_properties, this.m_metrics, this.m_time);
      this.m_Run = run;
      this.m_Fallback = fallback;
      this.m_ContinueOnCapturedContext = continueOnCapturedContext;
    }

    public CommandGroupKey CommandGroup => this.m_CommandGroup;

    public CommandKey CommandKey => this.m_CommandKey;

    internal ICircuitBreaker CircuitBreaker => this.m_circuitBreaker;

    public CommandMetrics Metrics => this.m_metrics;

    public ICommandProperties Properties => this.m_properties;

    public bool IsCircuitBreakerOpen => this.m_circuitBreaker.IsOpen(this.m_properties);

    public CircuitBreakerStatus CircuitBreakerState => this.m_circuitBreaker.GetCircuitBreakerState(this.m_properties);

    public Task Execute() => this.Execute(this.m_Run, this.m_Fallback, this.m_ContinueOnCapturedContext);

    protected async Task Execute(
      Func<Task> run,
      Func<Task> fallback,
      bool continueOnCapturedContext)
    {
      if (Interlocked.CompareExchange(ref this.invocationStartTime, this.m_time.GetCurrentTimeInMillis(), -1L) != -1L)
        throw new InvalidOperationException("This instance can only be executed once. Please instantiate a new instance.");
      string circuitBreakerErrorMessage;
      if (this.m_circuitBreaker.AllowRequest(this.m_properties))
      {
        if (this.m_circuitBreaker.ExecutionSemaphore.TryAcquire(this.m_properties.ExecutionMaxConcurrentRequests))
        {
          try
          {
            bool flag = false;
            if (this.m_properties.ExecutionMaxRequests != int.MaxValue)
            {
              if (this.m_circuitBreaker.ExecutionRequests.GetRollingSum() < (long) this.m_properties.ExecutionMaxRequests)
                this.m_circuitBreaker.ExecutionRequests.Increment();
              else
                flag = true;
            }
            if (!flag)
            {
              try
              {
                long numberOfPermitsUsed = (long) this.m_circuitBreaker.ExecutionSemaphore.GetNumberOfPermitsUsed();
                this.m_eventNotifier.MarkExecutionCount(this.CommandGroup, this.CommandKey, this.m_circuitBreaker.ExecutionRequests.GetRollingSum());
                this.m_eventNotifier.MarkExecutionConcurrency(this.CommandGroup, this.CommandKey, numberOfPermitsUsed);
                if (numberOfPermitsUsed > this.m_metrics.maxConcurrency)
                {
                  this.m_eventNotifier.MarkExecutionConcurrency(this.CommandGroup, this.CommandKey, numberOfPermitsUsed * -1L);
                  this.m_metrics.maxConcurrency = numberOfPermitsUsed;
                }
                this.invocationStartTime = this.m_time.GetCurrentTimeInMillis();
                try
                {
                  await run().ConfigureAwait(continueOnCapturedContext);
                }
                catch (AggregateException ex)
                {
                  ExceptionDispatchInfo.Capture((Exception) ex).Throw();
                }
                long elapsedTimeInMilliseconds = this.m_time.GetCurrentTimeInMillis() - this.invocationStartTime;
                if ((double) elapsedTimeInMilliseconds > this.m_properties.ExecutionTimeout.TotalMilliseconds)
                {
                  this.m_metrics.MarkTimeout();
                }
                else
                {
                  this.m_metrics.MarkSuccess();
                  this.m_circuitBreaker.MarkSuccess();
                  this.m_eventNotifier.MarkCommandExecution(this.CommandGroup, this.CommandKey, elapsedTimeInMilliseconds);
                }
              }
              catch (Exception ex)
              {
                if (ex.Data.Contains((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}"))
                {
                  ex.Data.Remove((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}");
                  throw;
                }
                else
                {
                  this.m_metrics.MarkFailure();
                  this.m_metrics.LastException = ex;
                  string message = "failed executing run delegate due to an exception";
                  this.Trace(10003202, TraceLevel.Error, CommandAsync.s_featurearea, CommandAsync.s_classname, this.CircuitBreakerErrorMessage(message, this.m_metrics.LastException));
                  if (!await this.TryFallbackAsync(message, fallback, continueOnCapturedContext).ConfigureAwait(continueOnCapturedContext))
                    throw;
                }
              }
            }
            else
            {
              this.m_metrics.MarkLimitRejected();
              string message = "exceeded the execution limit of " + this.m_properties.ExecutionMaxRequests.ToString();
              circuitBreakerErrorMessage = this.CircuitBreakerErrorMessage(message);
              this.Trace(10003201, TraceLevel.Error, CommandAsync.s_featurearea, CommandAsync.s_classname, circuitBreakerErrorMessage);
              if (!await this.TryFallbackAsync(message, fallback, continueOnCapturedContext).ConfigureAwait(continueOnCapturedContext))
                throw new CircuitBreakerExceededExecutionLimitException(circuitBreakerErrorMessage);
              circuitBreakerErrorMessage = (string) null;
            }
          }
          finally
          {
            this.m_circuitBreaker.ExecutionSemaphore.Release();
          }
        }
        else
        {
          this.m_metrics.MarkConcurrencyRejected();
          string message = "exceeded the concurrency limit of " + this.m_properties.ExecutionMaxConcurrentRequests.ToString();
          circuitBreakerErrorMessage = this.CircuitBreakerErrorMessage(message);
          this.Trace(10003203, TraceLevel.Error, CommandAsync.s_featurearea, CommandAsync.s_classname, circuitBreakerErrorMessage);
          if (!await this.TryFallbackAsync(message, fallback, continueOnCapturedContext).ConfigureAwait(continueOnCapturedContext))
            throw new CircuitBreakerExceededConcurrencyException(circuitBreakerErrorMessage);
          circuitBreakerErrorMessage = (string) null;
        }
      }
      else
      {
        this.m_metrics.MarkShortCircuited();
        string errorMessage = "short-circuited";
        this.TraceConditionally(10003204, TraceLevel.Info, CommandAsync.s_featurearea, CommandAsync.s_classname, (Func<string>) (() => this.CircuitBreakerErrorMessage(errorMessage, this.m_metrics.LastException)));
        if (!await this.TryFallbackAsync(errorMessage, fallback, continueOnCapturedContext).ConfigureAwait(continueOnCapturedContext))
          throw new CircuitBreakerShortCircuitException(this.CircuitBreakerErrorMessage(errorMessage, this.m_metrics.LastException));
      }
    }

    protected virtual void Trace(
      int tracepoint,
      TraceLevel level,
      string featurearea,
      string classname,
      string message)
    {
      this.m_eventNotifier.TraceRaw(tracepoint, level, featurearea, classname, message);
    }

    protected virtual void TraceConditionally(
      int tracepoint,
      TraceLevel level,
      string featurearea,
      string classname,
      Func<string> message)
    {
      this.m_eventNotifier.TraceRawConditionally(tracepoint, level, featurearea, classname, message);
    }

    private async Task<bool> TryFallbackAsync(
      string message,
      Func<Task> fallback,
      bool continueOnCapturedContext)
    {
      if (fallback == null)
        return false;
      if (!this.m_properties.FallbackDisabled)
      {
        if (this.m_circuitBreaker.FallbackSemaphore.TryAcquire(this.m_properties.FallbackMaxConcurrentRequests))
        {
          try
          {
            bool flag = false;
            if (this.m_properties.FallbackMaxRequests != int.MaxValue)
            {
              if (this.m_circuitBreaker.FallbackRequests.GetRollingSum() < (long) this.m_properties.FallbackMaxRequests)
                this.m_circuitBreaker.FallbackRequests.Increment();
              else
                flag = true;
            }
            if (!flag)
            {
              try
              {
                this.m_eventNotifier.MarkFallbackConcurrency(this.CommandGroup, this.CommandKey, (long) this.m_circuitBreaker.FallbackSemaphore.GetNumberOfPermitsUsed());
                this.m_eventNotifier.MarkFallbackCount(this.CommandGroup, this.CommandKey, this.m_circuitBreaker.FallbackRequests.GetRollingSum());
                try
                {
                  await fallback().ConfigureAwait(continueOnCapturedContext);
                }
                catch (AggregateException ex)
                {
                  ExceptionDispatchInfo.Capture((Exception) ex).Throw();
                }
                this.m_metrics.MarkFallbackSuccess();
                return true;
              }
              catch (Exception ex)
              {
                if (!ex.Data.Contains((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}"))
                {
                  this.m_metrics.MarkFallbackFailure();
                  string message1 = message + " and failed executing fallback delegate due to an exception";
                  this.Trace(10003206, TraceLevel.Error, CommandAsync.s_featurearea, CommandAsync.s_classname, this.CircuitBreakerErrorMessage(message1, ex));
                }
              }
            }
            else
            {
              this.m_metrics.MarkFallbackLimitRejected();
              string message2 = message + " and fallback exceeded the fallback limit of " + this.m_properties.FallbackMaxRequests.ToString();
              this.Trace(10003209, TraceLevel.Warning, CommandAsync.s_featurearea, CommandAsync.s_classname, this.CircuitBreakerErrorMessage(message2));
            }
          }
          finally
          {
            this.m_circuitBreaker.FallbackSemaphore.Release();
          }
        }
        else
        {
          this.m_metrics.MarkFallbackConcurrencyRejected();
          string message3 = message + " and fallback exceeded the concurrency limit of " + this.m_properties.FallbackMaxConcurrentRequests.ToString();
          this.Trace(10003207, TraceLevel.Warning, CommandAsync.s_featurearea, CommandAsync.s_classname, this.CircuitBreakerErrorMessage(message3));
        }
      }
      else
      {
        string message4 = message + " and fallback disabled";
        this.Trace(10003208, TraceLevel.Warning, CommandAsync.s_featurearea, CommandAsync.s_classname, this.CircuitBreakerErrorMessage(message4));
      }
      return false;
    }

    private string CircuitBreakerErrorMessage(string message, Exception e = null)
    {
      string str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Circuit Breaker \"{0}\" {1}. In the last {2} milliseconds, there were: {3} failure, {4} timeout, {5} short circuited, {6} concurrency rejected, and {7} limit rejected.", (object) this.m_CommandKey.Name, (object) message, (object) this.m_properties.MetricsRollingStatisticalWindowInMilliseconds, (object) this.m_metrics.failure.GetRollingSum(), (object) this.m_metrics.timeout.GetRollingSum(), (object) this.m_metrics.shortCircuited.GetRollingSum(), (object) this.m_metrics.concurrencyRejected.GetRollingSum(), (object) this.m_metrics.limitRejected.GetRollingSum());
      if (e != null)
        str = str + " Last exception: " + e.GetType().FullName + " " + e.Message;
      return str;
    }
  }
}
