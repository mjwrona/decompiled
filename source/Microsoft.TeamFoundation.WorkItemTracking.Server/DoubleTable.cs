// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DoubleTable
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class DoubleTable : WorkItemTrackingTableValueParameter<double>
  {
    private static readonly SqlMetaData[] s_metadata = new SqlMetaData[1]
    {
      new SqlMetaData("Val", SqlDbType.Float)
    };

    public DoubleTable(IEnumerable<double> values)
      : base(values, "typ_DoubleTable", DoubleTable.s_metadata)
    {
    }

    public override void SetRecord(double value, SqlDataRecord record) => record.SetDouble(0, value);
  }
}
