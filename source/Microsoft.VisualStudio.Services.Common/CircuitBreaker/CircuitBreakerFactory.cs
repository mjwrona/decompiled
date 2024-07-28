// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.CircuitBreakerFactory
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  public static class CircuitBreakerFactory
  {
    internal static readonly ConcurrentDictionary<CommandKey, ICircuitBreaker> Instances = new ConcurrentDictionary<CommandKey, ICircuitBreaker>();

    public static ICircuitBreaker GetInstance(
      CommandKey commandKey,
      ICommandProperties properties,
      CommandMetrics metrics,
      ITime time = null)
    {
      return CircuitBreakerFactory.Instances.GetOrAdd(commandKey, (Func<CommandKey, ICircuitBreaker>) (w => (ICircuitBreaker) new CircuitBreakerImpl(properties, metrics, time)));
    }

    internal static void SetInstance(CommandKey commandKey, ICircuitBreaker circuitBreaker) => CircuitBreakerFactory.Instances.AddOrUpdate(commandKey, circuitBreaker, (Func<CommandKey, ICircuitBreaker, ICircuitBreaker>) ((k, cb1) => circuitBreaker));

    public static ICircuitBreaker GetInstance(CommandKey commandKey)
    {
      ICircuitBreaker instance = (ICircuitBreaker) null;
      CircuitBreakerFactory.Instances.TryGetValue(commandKey, out instance);
      return instance;
    }

    public static void RemoveOlderThan(TimeSpan time)
    {
      foreach (KeyValuePair<CommandKey, ICircuitBreaker> instance in CircuitBreakerFactory.Instances)
      {
        if (instance.Value.IsOlderThan(time))
          CircuitBreakerFactory.Instances.TryRemove(instance.Key, out ICircuitBreaker _);
      }
    }

    internal static void Reset() => CircuitBreakerFactory.Instances.Clear();
  }
}
