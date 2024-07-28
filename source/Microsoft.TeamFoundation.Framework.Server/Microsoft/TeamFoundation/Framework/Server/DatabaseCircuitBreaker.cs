// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseCircuitBreaker
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DatabaseCircuitBreaker
  {
    private CircuitBreakerDatabaseProperties m_baseCircuitBreakerDatabaseProperties;
    private CommandSetter m_componentLevelCommandSetter = new CommandSetter((CommandGroupKey) "Framework.").AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(DatabaseCircuitBreaker.s_defaultBaseCircuitBreakerTimeout).WithExecutionMaxConcurrentRequests(int.MaxValue));
    private DatabaseResourceComponent m_component;
    private IVssRequestContext m_requestContext;
    private static readonly TimeSpan s_defaultBaseCircuitBreakerTimeout = TimeSpan.FromMinutes(5.0);

    public DatabaseCircuitBreaker(
      DatabaseResourceComponent component,
      IVssRequestContext requestContext)
    {
      this.m_component = component;
      this.m_requestContext = requestContext;
    }

    internal ICommandProperties ComponentLevelCircuitBreakerProperties { get; set; }

    public CircuitBreakerDatabaseProperties BaseCircuitBreakerDatabaseProperties
    {
      get => this.m_baseCircuitBreakerDatabaseProperties;
      set => this.m_baseCircuitBreakerDatabaseProperties = value;
    }

    internal CommandSetter ComponentLevelCommandSetter => this.m_componentLevelCommandSetter;

    public void ExecuteCommandWithComponentCircuitBreaker(
      ExecuteType executeType,
      CommandBehavior behavior,
      string performanceGroupName)
    {
      if (this.ComponentLevelCircuitBreakerProperties != null && (ImmutableKey) this.ComponentLevelCommandSetter.CommandKey != (ImmutableKey) null && !string.IsNullOrEmpty(this.ComponentLevelCommandSetter.CommandKey.Name))
        new CommandService(this.m_requestContext, this.ComponentLevelCommandSetter, this.ComponentLevelCircuitBreakerProperties, (Action) (() => this.ExecuteCommandWithBaseCircuitBreaker(executeType, behavior, performanceGroupName))).Execute();
      else
        this.ExecuteCommandWithBaseCircuitBreaker(executeType, behavior, performanceGroupName);
    }

    internal Task ExecuteCommandWithComponentCircuitBreakerAsync(
      ExecuteType executeType,
      CommandBehavior behavior,
      string performanceGroupName)
    {
      return this.ComponentLevelCircuitBreakerProperties != null && (ImmutableKey) this.ComponentLevelCommandSetter.CommandKey != (ImmutableKey) null && !string.IsNullOrEmpty(this.ComponentLevelCommandSetter.CommandKey.Name) ? new CommandServiceAsync(this.m_requestContext, this.ComponentLevelCommandSetter, this.ComponentLevelCircuitBreakerProperties, (Func<Task>) (() => this.ExecuteCommandWithBaseCircuitBreakerAsync(executeType, behavior, performanceGroupName)), continueOnCapturedContext: true).Execute() : this.ExecuteCommandWithBaseCircuitBreakerAsync(executeType, behavior, performanceGroupName);
    }

    private void ExecuteCommandWithBaseCircuitBreaker(
      ExecuteType executeType,
      CommandBehavior behavior,
      string performanceGroupName)
    {
      string str = this.m_component.CommandKey + this.m_component.InitialCatalog + "-" + DatabaseResourceComponent.ToString(this.m_component.ApplicationIntent);
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) str).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(DatabaseCircuitBreaker.s_defaultBaseCircuitBreakerTimeout).WithExecutionMaxConcurrentRequests(this.m_component.MaxPoolSize));
      new CommandDatabase(this.m_requestContext, this.BaseCircuitBreakerDatabaseProperties ?? CircuitBreakerDatabaseProperties.BootstrapBreakerProperties, setter, (Action) (() => this.m_component.ExecuteCommand(executeType, behavior, performanceGroupName))).Execute();
    }

    private Task ExecuteCommandWithBaseCircuitBreakerAsync(
      ExecuteType executeType,
      CommandBehavior behavior,
      string performanceGroupName)
    {
      string str = this.m_component.CommandKey + this.m_component.InitialCatalog + "-" + DatabaseResourceComponent.ToString(this.m_component.ApplicationIntent);
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) str).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(DatabaseCircuitBreaker.s_defaultBaseCircuitBreakerTimeout).WithExecutionMaxConcurrentRequests(this.m_component.MaxPoolSize));
      return new CommandDatabaseAsync(this.m_requestContext, this.BaseCircuitBreakerDatabaseProperties ?? CircuitBreakerDatabaseProperties.BootstrapBreakerProperties, setter, (Func<Task>) (() => this.m_component.ExecuteCommandAsync(executeType, behavior, performanceGroupName))).Execute();
    }
  }
}
