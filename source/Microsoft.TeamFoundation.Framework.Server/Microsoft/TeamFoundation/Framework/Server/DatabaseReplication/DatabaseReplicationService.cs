// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseReplication.DatabaseReplicationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server.GeoReplication;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server.DatabaseReplication
{
  internal class DatabaseReplicationService : IDatabaseReplicationService, IVssFrameworkService
  {
    private IDisposableReadOnlyList<IDatabaseReplicationContextProvider> m_providers;
    private DatabaseReplicationConfiguration m_configuration;
    private bool m_initialized;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      this.m_providers = systemRequestContext.GetExtensions<IDatabaseReplicationContextProvider>();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_providers == null)
        return;
      this.m_providers.Dispose();
      this.m_providers = (IDisposableReadOnlyList<IDatabaseReplicationContextProvider>) null;
    }

    public DatabaseReplicationContext GetDatabaseReplicationContext(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties databaseProperties,
      ISqlConnectionInfo dataTierConnectionInfo = null,
      ITFLogger logger = null)
    {
      if (requestContext.GetService<IGeoReplicationService>().GetGeoReplicationMode(requestContext) != GeoReplicationMode.PartitionDb)
        return DatabaseReplicationContext.Default;
      ArgumentUtility.CheckForNull<ITeamFoundationDatabaseProperties>(databaseProperties, nameof (databaseProperties));
      logger = logger ?? (ITFLogger) new NullLogger();
      this.EnsureInitialized(requestContext);
      foreach (IDatabaseReplicationContextProvider provider in (IEnumerable<IDatabaseReplicationContextProvider>) this.m_providers)
      {
        DatabaseReplicationContext replicationContext = provider.GetDatabaseReplicationContext(requestContext, databaseProperties, dataTierConnectionInfo, logger);
        if (replicationContext != null && replicationContext != DatabaseReplicationContext.Default)
          return replicationContext;
      }
      return DatabaseReplicationContext.Default;
    }

    private void EnsureInitialized(IVssRequestContext requestContext)
    {
      if (this.m_initialized)
        return;
      if (this.m_configuration == null)
        throw new InvalidOperationException("The database replication configuration must be set before using this service.");
      foreach (IDatabaseReplicationContextProvider provider in (IEnumerable<IDatabaseReplicationContextProvider>) this.m_providers)
        provider.Initialize(requestContext, this.m_configuration);
      this.m_initialized = true;
    }

    public DatabaseReplicationConfiguration Configuration
    {
      get => this.m_configuration;
      set
      {
        this.m_initialized = false;
        this.m_configuration = value;
      }
    }
  }
}
