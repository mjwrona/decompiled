// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ComponentCreator`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class ComponentCreator<TComponent> : IComponentCreator where TComponent : ISqlResourceComponent, IDBResourceComponent, new()
  {
    private readonly int m_serviceVersion;
    private readonly bool m_isTransitionCreator;

    public ComponentCreator(int serviceVersion)
      : this(serviceVersion, false)
    {
    }

    public ComponentCreator(int serviceVersion, bool isTransitionCreator)
    {
      ArgumentUtility.CheckForOutOfRange(serviceVersion, nameof (serviceVersion), 0);
      this.m_serviceVersion = serviceVersion;
      this.m_isTransitionCreator = isTransitionCreator;
    }

    public int ServiceVersion => this.m_serviceVersion;

    public bool IsTransitionCreator => this.m_isTransitionCreator;

    public ISqlResourceComponent Create(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
      TComponent component = new TComponent();
      component.Initialize(requestContext, dataspaceCategory, dataspaceIdentifier, this.m_serviceVersion, connectionType, logger);
      return (ISqlResourceComponent) component;
    }

    public ISqlResourceComponent Create(
      ISqlConnectionInfo connectionInfo,
      int commandTimeout,
      int deadlockPause,
      int maxDeadlockRetries,
      ITFLogger logger,
      CircuitBreakerDatabaseProperties circuitBreakerProperties)
    {
      TComponent component = new TComponent();
      component.Initialize(connectionInfo, commandTimeout, deadlockPause, maxDeadlockRetries, this.m_serviceVersion, logger, circuitBreakerProperties);
      return (ISqlResourceComponent) component;
    }
  }
}
