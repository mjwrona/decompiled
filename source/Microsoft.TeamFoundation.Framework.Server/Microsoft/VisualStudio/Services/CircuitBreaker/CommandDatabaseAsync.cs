// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.CommandDatabaseAsync
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  internal class CommandDatabaseAsync : CommandAsync
  {
    internal CommandDatabaseAsync(
      IVssRequestContext requestContext,
      CircuitBreakerDatabaseProperties databaseProperties,
      CommandSetter setter,
      Func<Task> run = null,
      Func<Task> fallback = null)
      : this(setter.GroupKey, setter.CommandKey, (ICircuitBreaker) null, (ICommandProperties) new CommandPropertiesDatabase(databaseProperties, setter.CommandKey, setter.CommandPropertiesDefaults), (CommandMetrics) null, (IEventNotifier) EventNotifierPerformanceCounters.Instance, run, fallback)
    {
    }

    private CommandDatabaseAsync(
      CommandGroupKey group,
      CommandKey key,
      ICircuitBreaker circuitBreaker,
      ICommandProperties properties,
      CommandMetrics metrics,
      IEventNotifier eventNotifier,
      Func<Task> run = null,
      Func<Task> fallback = null)
      : base(group, key, circuitBreaker, properties, metrics, eventNotifier, run, fallback, true)
    {
    }
  }
}
