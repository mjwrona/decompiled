// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.CommandAsync`1
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  public class CommandAsync<TResult> : CommandAsync
  {
    private readonly Func<Task<TResult>> m_Run;
    private readonly Func<Task<TResult>> m_Fallback;

    public CommandAsync(
      CommandGroupKey group,
      Func<Task<TResult>> run,
      Func<Task<TResult>> fallback = null,
      bool continueOnCapturedContext = false)
      : this(new CommandSetter(group), run, fallback, continueOnCapturedContext)
    {
    }

    public CommandAsync(
      CommandSetter setter,
      Func<Task<TResult>> run,
      Func<Task<TResult>> fallback = null,
      bool continueOnCapturedContext = false)
      : this(setter.GroupKey, setter.CommandKey, (ICircuitBreaker) null, (ICommandProperties) new CommandPropertiesDefault(setter.CommandPropertiesDefaults), (CommandMetrics) null, (IEventNotifier) null, run, fallback, continueOnCapturedContext)
    {
      if (run == null)
        throw new ArgumentNullException(nameof (run));
    }

    protected internal CommandAsync(
      CommandGroupKey group,
      CommandKey key,
      ICircuitBreaker circuitBreaker,
      ICommandProperties properties,
      CommandMetrics metrics,
      IEventNotifier eventNotifier,
      Func<Task<TResult>> run,
      Func<Task<TResult>> fallback,
      bool continueOnCapturedContext,
      ITime time = null)
      : base(group, key, circuitBreaker, properties, metrics, eventNotifier, (Func<Task>) null, (Func<Task>) null, continueOnCapturedContext, time)
    {
      this.m_Run = run;
      this.m_Fallback = fallback;
    }

    public async Task<TResult> Execute()
    {
      CommandAsync<TResult> commandAsync = this;
      TResult result = default (TResult);
      Func<Task> fallback = (Func<Task>) null;
      // ISSUE: variable of a compiler-generated type
      CommandAsync<TResult>.\u003C\u003Ec__DisplayClass5_0 cDisplayClass50;
      if (commandAsync.m_Fallback != null)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        fallback = (Func<Task>) (async () => result = await cDisplayClass50.\u003C\u003E4__this.m_Fallback().ConfigureAwait(cDisplayClass50.\u003C\u003E4__this.m_ContinueOnCapturedContext));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      await commandAsync.Execute((Func<Task>) (async () => result = await cDisplayClass50.\u003C\u003E4__this.m_Run().ConfigureAwait(cDisplayClass50.\u003C\u003E4__this.m_ContinueOnCapturedContext)), fallback, commandAsync.m_ContinueOnCapturedContext).ConfigureAwait(commandAsync.m_ContinueOnCapturedContext);
      return result;
    }
  }
}
