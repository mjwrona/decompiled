// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostMoveStatusComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class HostMoveStatusComponent : TeamFoundationSqlResourceComponent
  {
    public HostMoveStatusComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public void ClearHostMoveStatus()
    {
      this.PrepareStoredProcedure("prc_ClearHostMoveStatus");
      this.ExecuteNonQuery();
    }

    public void SetHostMoveStatus(HostMoveStatus status)
    {
      this.PrepareStoredProcedure("prc_SetHostMoveStatus");
      this.BindSysname("@schema", status.Schema, false);
      this.BindSysname("@table", status.Table, false);
      this.BindBoolean("@isComplete", status.IsComplete);
      this.ExecuteNonQuery();
    }

    public List<HostMoveStatus> GetHostMoveStatus()
    {
      this.PrepareStoredProcedure("prc_GetHostMoveStatus");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_GetHostMoveStatus", this.RequestContext);
      resultCollection.AddBinder<HostMoveStatus>((ObjectBinder<HostMoveStatus>) new HostMoveStatusColumns());
      return resultCollection.GetCurrent<HostMoveStatus>().Items;
    }
  }
}
