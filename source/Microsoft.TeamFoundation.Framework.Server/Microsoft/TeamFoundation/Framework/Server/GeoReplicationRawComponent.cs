// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.GeoReplicationRawComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server.GeoReplication;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class GeoReplicationRawComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<GeoReplicationRawComponent>(0, true)
    }, "GeoReplicationRaw");

    public List<GeoReplica> QueryReplicas()
    {
      this.VerifyNotInMaster();
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_QueryGeoReplicas.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "stmt_QueryGeoReplicas", this.RequestContext);
      resultCollection.AddBinder<GeoReplica>((ObjectBinder<GeoReplica>) new GeoReplicaBinder());
      return resultCollection.GetCurrent<GeoReplica>().Items;
    }

    public List<GeoReplicaView> QueryReplicasFromMaster()
    {
      this.VerifyInMaster();
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_QueryGeoReplicasFromMaster.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "stmt_QueryGeoReplicasFromMaster", (IVssRequestContext) null);
      resultCollection.AddBinder<GeoReplicaView>((ObjectBinder<GeoReplicaView>) new GeoReplicaViewBinder());
      return resultCollection.GetCurrent<GeoReplicaView>().Items;
    }
  }
}
