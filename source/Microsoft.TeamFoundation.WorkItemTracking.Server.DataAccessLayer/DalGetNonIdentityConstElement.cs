// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalGetNonIdentityConstElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalGetNonIdentityConstElement : DalSqlElement
  {
    public void JoinBatch(string constantString)
    {
      this.AppendSql(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\r\n                declare @constId int\r\n                select @constId = C.ConstID\r\n                from Constants C \r\n                where \r\n                     C.PartitionId = {0}\r\n                     and C.String = {1}\r\n                select \r\n                case \r\n                    when @@ROWCOUNT = 1 then @constId\r\n                    else 0 \r\n                end\r\n                ", (object) "@partitionId", (object) this.Param((object) constantString)));
      this.m_outputs = 1;
      this.m_index = this.SqlBatch.AddExpectedReturnedDataTables(this.m_outputs);
    }

    public bool TryGetNonIdentityConstant(out int id)
    {
      id = (int) this.SqlBatch.ResultPayload.Tables[this.m_index].Rows[0][0];
      return id != 0;
    }
  }
}
