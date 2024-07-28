// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.GeoReplication.ReplicationConfigurationExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.DatabaseReplication;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.GeoReplication
{
  public static class ReplicationConfigurationExtensions
  {
    public static ReplicationPartner FindPrimaryPartner(
      this DatabaseReplicationConfiguration configuration,
      IVssRequestContext requestContext,
      bool verifyAll = true)
    {
      ReplicationPartner primaryPartner = (ReplicationPartner) null;
      foreach (ReplicationPartner partner in (IEnumerable<ReplicationPartner>) ((IEnumerable<ReplicationPartner>) configuration.Partners).OrderByDescending<ReplicationPartner, bool>((Func<ReplicationPartner, bool>) (x => x.IsLocal)))
      {
        if (ReplicationConfigurationExtensions.IsPrimaryPartner(requestContext, partner))
        {
          primaryPartner = primaryPartner == null ? partner : throw new InvalidConfigurationException("Found multiple primary instances. " + partner.ConfigDbConnectionInfo.DataSource + " and " + primaryPartner.ConfigDbConnectionInfo.DataSource + ".");
          if (!verifyAll)
            break;
        }
      }
      return primaryPartner;
    }

    private static bool IsPrimaryPartner(
      IVssRequestContext requestContext,
      ReplicationPartner partner)
    {
      bool? nullable = new bool?();
      return ((!partner.IsLocal ? RegistryHelpers.GetValueRaw<bool?>(partner.ConfigDbConnectionInfo, DatabasePartitionConstants.DeploymentHostPartitionId, "/Configuration/GeoReplication/IsPrimary") : requestContext.GetService<IVssRegistryService>().GetValue<bool?>(requestContext, (RegistryQuery) "/Configuration/GeoReplication/IsPrimary", new bool?())) ?? throw new InvalidConfigurationException("Failed to find the isPrimary configuration setting in the registry for instance " + partner.ConfigDbConnectionInfo.DataSource + ".")).Value;
    }
  }
}
