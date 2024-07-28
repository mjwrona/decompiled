// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.StorageMigrationLogger
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  public class StorageMigrationLogger
  {
    private StorageMigration m_currentContainer;
    private ISqlConnectionInfo m_connectionInfo;
    private DateTime m_lastUpdate;
    private static readonly TimeSpan s_maxUpdateInterval = TimeSpan.FromSeconds(60.0);

    public StorageMigrationLogger(
      ISqlConnectionInfo connectionInfo,
      StorageMigration storageContainer)
    {
      this.m_connectionInfo = connectionInfo;
      this.m_lastUpdate = DateTime.MinValue;
      this.m_currentContainer = storageContainer;
    }

    public virtual void LogMessage(string message)
    {
      try
      {
        if (message.StartsWith("Warning:") && DateTime.UtcNow - this.m_lastUpdate < StorageMigrationLogger.s_maxUpdateInterval || this.m_connectionInfo == null || this.m_currentContainer == null)
          return;
        using (HostMigrationComponent componentRaw = this.m_connectionInfo.CreateComponentRaw<HostMigrationComponent>())
        {
          componentRaw.UpdateContainerMigrationStatus(this.m_currentContainer, this.m_currentContainer.Status, message);
          this.m_lastUpdate = DateTime.UtcNow;
        }
      }
      catch (Exception ex)
      {
      }
    }
  }
}
