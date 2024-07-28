// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TfsTestManagementHost
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TfsTestManagementHost : TestManagementHost
  {
    internal override ISecurityManager SecurityManager
    {
      get
      {
        if (this.m_securityManager == null)
          this.m_securityManager = (ISecurityManager) new Microsoft.TeamFoundation.TestManagement.Server.SecurityManager();
        return this.m_securityManager;
      }
    }

    internal override ITestManagementReplicator Replicator
    {
      get
      {
        if (this.m_replicator == null)
          this.m_replicator = (ITestManagementReplicator) new Microsoft.TeamFoundation.TestManagement.Server.Replicator();
        return this.m_replicator;
      }
    }

    public override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      this.m_replicator = (ITestManagementReplicator) new Microsoft.TeamFoundation.TestManagement.Server.Replicator();
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", CssCacheSQLNotifConstants.RebuildCssCacheEventClass, new SqlNotificationCallback(this.m_replicator.RefreshCache), false);
    }

    public override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Default", CssCacheSQLNotifConstants.RebuildCssCacheEventClass, new SqlNotificationCallback(this.m_replicator.RefreshCache), false);
      base.ServiceEnd(systemRequestContext);
    }
  }
}
