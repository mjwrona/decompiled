// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CounterComponent5
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class CounterComponent5 : CounterComponent4
  {
    public override long? ReserveCounterIds(
      string counterName,
      long countToReserve,
      Guid dataspaceIdentifier,
      bool isolated)
    {
      this.PrepareStoredProcedure("prc_CounterGetNext");
      this.BindString("@counterName", counterName, 128, false, SqlDbType.NVarChar);
      this.BindLong("@countToReserve", countToReserve);
      this.BindInt("@dataspaceId", this.GetDataspaceId(dataspaceIdentifier));
      this.BindBoolean("@isolated", isolated);
      object obj = this.ExecuteScalar();
      return obj is DBNull ? new long?() : new long?((long) obj);
    }
  }
}
