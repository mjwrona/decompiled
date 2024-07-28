// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.Command
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  public class Command : CommandAsync
  {
    private readonly Action m_Run;
    private readonly Action m_Fallback;
    private static Task DummyTask = (Task) Task.FromResult<bool>(true);

    public Command(CommandGroupKey group, Action run, Action fallback = null)
      : this(new CommandSetter(group), run, fallback)
    {
    }

    public Command(CommandSetter setter, Action run, Action fallback = null)
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
      Action run,
      Action fallback = null,
      ITime time = null)
      : base(group, key, circuitBreaker, properties, metrics, eventNotifier, (Func<Task>) null, (Func<Task>) null, false, time)
    {
      this.m_Run = run;
      this.m_Fallback = fallback;
    }

    public void Execute() => this.Execute(this.m_Run, this.m_Fallback);

    protected void Execute(Action run, Action fallback)
    {
      try
      {
        this.Execute((Func<Task>) (() =>
        {
          run();
          return Command.DummyTask;
        }), fallback != null ? (Func<Task>) (() =>
        {
          fallback();
          return Command.DummyTask;
        }) : (Func<Task>) null, true).Wait();
      }
      catch (AggregateException ex)
      {
        ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
      }
    }
  }
}
