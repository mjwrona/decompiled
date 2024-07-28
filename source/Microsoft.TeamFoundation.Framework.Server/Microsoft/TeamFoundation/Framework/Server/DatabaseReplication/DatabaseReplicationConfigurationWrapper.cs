// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseReplication.DatabaseReplicationConfigurationWrapper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server.DatabaseReplication
{
  [DataContract]
  public sealed class DatabaseReplicationConfigurationWrapper
  {
    public DatabaseReplicationConfigurationWrapper()
    {
    }

    public DatabaseReplicationConfigurationWrapper(
      IVssRequestContext requestContext,
      DatabaseReplicationConfiguration configuration)
    {
      ArgumentUtility.CheckForNull<DatabaseReplicationConfiguration>(configuration, nameof (configuration));
      ArgumentUtility.CheckForNull<ReplicationPartner[]>(configuration.Partners, nameof (Partners));
      this.Partners = new ReplicationPartnerWrapper[configuration.Partners.Length];
      for (int index = 0; index < configuration.Partners.Length; ++index)
      {
        ReplicationPartner partner = configuration.Partners[index];
        this.Partners[index] = new ReplicationPartnerWrapper()
        {
          Name = partner.Name,
          IsLocal = partner.IsLocal,
          ConfigDbConnectionInfo = new SqlConnectionInfoWrapper(requestContext, partner.ConfigDbConnectionInfo)
        };
      }
    }

    [DataMember]
    public ReplicationPartnerWrapper[] Partners { get; set; }

    public DatabaseReplicationConfiguration ToDatabaseReplicationConfiguration(
      IVssRequestContext requestContext)
    {
      DatabaseReplicationConfiguration replicationConfiguration = new DatabaseReplicationConfiguration()
      {
        Partners = new ReplicationPartner[this.Partners.Length]
      };
      for (int index = 0; index < this.Partners.Length; ++index)
      {
        ReplicationPartnerWrapper partner = this.Partners[index];
        replicationConfiguration.Partners[index] = new ReplicationPartner()
        {
          Name = partner.Name,
          IsLocal = partner.IsLocal,
          ConfigDbConnectionInfo = partner.ConfigDbConnectionInfo.ToSqlConnectionInfo(requestContext),
          DataTierConnectionInfo = (ISqlConnectionInfo) null,
          AdminConnectionInfo = (ISqlConnectionInfo) null
        };
      }
      return replicationConfiguration;
    }
  }
}
