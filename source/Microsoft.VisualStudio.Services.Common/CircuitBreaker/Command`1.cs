// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.Command`1
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  public class Command<TResult> : Command
  {
    private readonly Func<TResult> m_Run;
    private readonly Func<TResult> m_Fallback;

    public Command(CommandGroupKey group, Func<TResult> run, Func<TResult> fallback = null)
      : this(new CommandSetter(group), run, fallback)
    {
    }

    public Command(CommandSetter setter, Func<TResult> run, Func<TResult> fallback = null)
      : this(setter.GroupKey, setter.CommandKey, (ICircuitBreaker) null, (ICommandProperties) new CommandPropertiesDefault(setter.CommandPropertiesDefaults), (CommandMetrics) null, (IEventNotifier) null, run, fallback)
    {
      if (run == null)
        throw new ArgumentNullException(nameof (run));
    }

    protected internal Command(
      CommandGroupKey group,
      CommandKey key,
      ICircuitBreaker circuitBreaker,
      ICommandProperties properties,
      CommandMetrics metrics,
      IEventNotifier eventNotifier,
      Func<TResult> run,
      Func<TResult> fallback = null,
      ITime time = null)
      : base(group, key, circuitBreaker, properties, metrics, eventNotifier, (Action) null, time: time)
    {
      this.m_Run = run;
      this.m_Fallback = fallback;
    }

    public TResult Execute()
    {
      TResult result = default (TResult);
      this.Execute((Action) (() => result = this.m_Run()), this.m_Fallback != null ? (Action) (() => result = this.m_Fallback()) : (Action) null);
      return result;
    }
  }
}
