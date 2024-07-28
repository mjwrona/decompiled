// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StrongBoxComponent10
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class StrongBoxComponent10 : StrongBoxComponent9
  {
    public override void CreateDrawer(string drawerName, Guid drawerId, Guid signingKeyId)
    {
      this.PrepareStoredProcedure("prc_CreateDrawer");
      this.BindString("@name", drawerName, (int) byte.MaxValue, false, SqlDbType.NVarChar);
      this.BindGuid("@drawerId", drawerId);
      this.BindGuid("@signingKeyId", signingKeyId);
      this.ExecuteNonQuery();
    }

    internal override StrongBoxDrawerInfo GetDrawerInfo(string drawerName)
    {
      this.PrepareStoredProcedure("prc_GetDrawerInfoByName");
      this.BindString("@name", drawerName, (int) byte.MaxValue, false, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<(string, Guid, Guid, DateTime)>((ObjectBinder<(string, Guid, Guid, DateTime)>) new DrawerInfoColumns());
        List<(string, Guid, Guid, DateTime)> items = resultCollection.GetCurrent<(string, Guid, Guid, DateTime)>().Items;
        if (items == null || items.Count <= 0)
          return (StrongBoxDrawerInfo) null;
        (string, Guid, Guid, DateTime) tuple = items[0];
        return new StrongBoxDrawerInfo(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
      }
    }

    internal override StrongBoxDrawerInfo GetDrawerInfo(Guid drawerId)
    {
      this.PrepareStoredProcedure("prc_GetDrawerInfoById");
      this.BindGuid("@drawerId", drawerId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<(string, Guid, Guid, DateTime)>((ObjectBinder<(string, Guid, Guid, DateTime)>) new DrawerInfoColumns());
        List<(string, Guid, Guid, DateTime)> items = resultCollection.GetCurrent<(string, Guid, Guid, DateTime)>().Items;
        if (items == null || items.Count <= 0)
          return (StrongBoxDrawerInfo) null;
        (string, Guid, Guid, DateTime) tuple = items[0];
        return new StrongBoxDrawerInfo(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
      }
    }

    internal override void UpdateDrawerSigningKey(Guid drawerId, Guid signingKeyId)
    {
      this.PrepareStoredProcedure("prc_UpdateStrongBoxDrawerSigningKey");
      this.BindGuid(nameof (drawerId), drawerId);
      this.BindGuid(nameof (signingKeyId), signingKeyId);
      this.ExecuteScalar();
    }
  }
}
