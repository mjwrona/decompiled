// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseReplication.DatabaseReplicationContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server.DatabaseReplication
{
  public class DatabaseReplicationContext
  {
    public static DatabaseReplicationContext Default = new DatabaseReplicationContext(true);

    public DatabaseReplicationContext(bool isPrimary)
    {
      this.IsPrimary = isPrimary;
      this.Replicas = new List<IDatabaseReplicaInfo>();
    }

    public bool IsPrimary { get; set; }

    public List<IDatabaseReplicaInfo> Replicas { get; }

    public bool HasReplicas => this.Replicas.Count > 0;

    public string PrimaryServerName { get; set; }

    public virtual void WaitForDatabaseCopy(IVssRequestContext requestContext)
    {
    }
  }
}
