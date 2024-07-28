// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.GeoReplication.GeoReplicationComponent
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.GeoReplication;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Cloud.GeoReplication
{
  internal class GeoReplicationComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<GeoReplicationComponent>(1),
      (IComponentCreator) new ComponentCreator<GeoReplicationComponent2>(2)
    }, "GeoReplication");

    public List<GeoReplica> QueryReplicas()
    {
      this.PrepareStoredProcedure("prc_QueryGeoReplicas");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryGeoReplicas", this.RequestContext);
      resultCollection.AddBinder<GeoReplica>((ObjectBinder<GeoReplica>) new GeoReplicaBinder());
      return resultCollection.GetCurrent<GeoReplica>().Items;
    }

    public virtual void WaitForDatabaseCopy()
    {
    }
  }
}
