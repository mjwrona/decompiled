// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseResourceComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class DatabaseResourceComponent : 
    DatabaseComponentTracing,
    IDBResourceComponent,
    IDisposable,
    ICancelable
  {
    private readonly string s_commandKey;
    private string m_initialCatalog;
    private ApplicationIntent m_applicationIntent;
    private int m_maxPoolSize = int.MaxValue;

    protected DatabaseResourceComponent(string commandKey, string traceLayer)
      : base(traceLayer)
    {
      this.s_commandKey = commandKey;
    }

    void IDBResourceComponent.Initialize(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
      this.Initialize(requestContext, dataspaceCategory, dataspaceIdentifier, serviceVersion, connectionType, logger);
    }

    void IDBResourceComponent.Initialize(
      ISqlConnectionInfo connectionInfo,
      int commandTimeout,
      int deadlockPause,
      int maxDeadlockRetries,
      int serviceVersion,
      ITFLogger logger,
      CircuitBreakerDatabaseProperties circuitBreakerProperties)
    {
      this.Initialize(connectionInfo, commandTimeout, deadlockPause, maxDeadlockRetries, serviceVersion, logger, circuitBreakerProperties);
    }

    protected virtual void Initialize(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
    }

    protected virtual void Initialize(
      ISqlConnectionInfo connectionInfo,
      int commandTimeout,
      int deadlockPause,
      int maxDeadlockRetries,
      int serviceVersion,
      ITFLogger logger,
      CircuitBreakerDatabaseProperties circuitBreakerProperties)
    {
    }

    public virtual bool VerifyServiceVersion(
      string serviceName,
      int serviceVersion,
      out int databaseVersion,
      out int minDatabaseVersion)
    {
      databaseVersion = -1;
      minDatabaseVersion = -1;
      return false;
    }

    protected virtual ITFLogger GetDefaultLogger() => (ITFLogger) new NullLogger();

    protected static void TagIgnorableExceptionsForCircuitBreaker(
      Exception exception,
      HashSet<Type> circuitBreakerTrippableExceptions)
    {
      Type type = exception.GetType();
      if (circuitBreakerTrippableExceptions.Contains(type) || exception.Data.Contains((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}"))
        return;
      exception.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
    }

    public virtual void Dispose()
    {
    }

    public virtual void Cancel()
    {
    }

    protected static string GetNonVersionedComponentClassName(Type type)
    {
      if (type == (Type) null || string.IsNullOrEmpty(type.Name))
        return string.Empty;
      while (char.IsDigit(type.Name[type.Name.Length - 1]))
      {
        type = type.BaseType;
        if (type == (Type) null || string.IsNullOrEmpty(type.Name))
          return string.Empty;
      }
      return type.FullName;
    }

    internal static string ToString(ApplicationIntent applicationIntent) => applicationIntent != ApplicationIntent.ReadOnly ? "ReadWrite" : "ReadOnly";

    protected virtual void SetupComponentCircuitBreaker(
      IVssRequestContext requestContext,
      string databaseName,
      ApplicationIntent applicationIntent)
    {
    }

    internal virtual void ExecuteCommand(
      ExecuteType executeType,
      CommandBehavior behavior,
      string performanceGroupName)
    {
    }

    internal abstract Task ExecuteCommandAsync(
      ExecuteType executeType,
      CommandBehavior behavior,
      string performanceGroupName);

    public string Database => !string.IsNullOrEmpty(this.InitialCatalog) ? this.InitialCatalog : TeamFoundationSqlResourceComponent.Master;

    public string InitialCatalog
    {
      get => this.m_initialCatalog;
      protected set => this.m_initialCatalog = value;
    }

    public ApplicationIntent ApplicationIntent
    {
      get => this.m_applicationIntent;
      protected set => this.m_applicationIntent = value;
    }

    public int MaxPoolSize
    {
      get => this.m_maxPoolSize;
      set => this.m_maxPoolSize = value;
    }

    protected DatabaseCircuitBreaker CircuitBreaker { get; set; }

    protected CommandSetter ComponentLevelCommandSetter => this.CircuitBreaker.ComponentLevelCommandSetter;

    protected ICommandProperties ComponentLevelCircuitBreakerProperties
    {
      get => this.CircuitBreaker.ComponentLevelCircuitBreakerProperties;
      set => this.CircuitBreaker.ComponentLevelCircuitBreakerProperties = value;
    }

    public string CommandKey => this.s_commandKey;
  }
}
