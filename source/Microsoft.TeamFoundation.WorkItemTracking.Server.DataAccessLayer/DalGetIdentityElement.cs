// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalGetIdentityElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalGetIdentityElement : DalSqlElement
  {
    public void JoinBatch(string identityName)
    {
      if (this.IsSchemaPartitioned)
        this.AppendSql(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\r\ndeclare @parentId int\r\ndeclare @constId int\r\nselect @constId = C.ConstID,\r\n@parentId = S.ParentID\r\nfrom Constants C \r\nleft join dbo.Sets S\r\n     on C.ConstID = S.ConstID\r\n     and (S.ParentID = {3} or S.ParentID = {2})\r\n     and S.PartitionId = {0}\r\n     and S.fDeleted = 0\r\nwhere \r\n     C.PartitionId = {0}\r\n     and C.String = {1}\r\n     and C.SID is not null\r\n\r\nselect \r\ncase \r\n    when @@ROWCOUNT = 1 then @constId\r\n    else 0 \r\nend,\r\ncase  \r\n    when @parentId = {2} then 1\r\n    else 0\r\nend,\r\ncase  \r\n    when @parentId = {3} then 1\r\n    else 0\r\nend\r\n", (object) "@partitionId", (object) this.Param((object) identityName), (object) -2, (object) -10));
      else
        this.AppendSql(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\r\ndeclare @parentId int\r\ndeclare @constId int\r\nselect @constId = C.ConstID,\r\n@parentId = S.ParentID\r\nfrom Constants C \r\nleft join dbo.Sets S\r\n    on C.ConstID = S.ConstID\r\n    and (S.ParentID = {2} or S.ParentID = {1})\r\n    and S.fDeleted = 0\r\nwhere \r\n    C.String = {0}\r\n\r\nselect \r\ncase \r\n    when @@ROWCOUNT = 1 then @constId\r\n    else 0\r\nend, \r\ncase  \r\n    when @parentId = {1} then 1\r\n    else 0\r\nend,\r\ncase  \r\n    when @parentId = {2} then 1\r\n    else 0\r\nend\r\n", (object) this.Param((object) identityName), (object) -2, (object) -10));
      this.m_outputs = 1;
      this.m_index = this.SqlBatch.AddExpectedReturnedDataTables(this.m_outputs);
    }

    public bool TryGetIdentity(out int id, out bool isValidUser, out bool isValidGroup)
    {
      id = (int) this.SqlBatch.ResultPayload.Tables[this.m_index].Rows[0][0];
      if (id != 0)
      {
        isValidUser = (int) this.SqlBatch.ResultPayload.Tables[this.m_index].Rows[0][1] != 0;
        isValidGroup = (int) this.SqlBatch.ResultPayload.Tables[this.m_index].Rows[0][2] != 0;
        return true;
      }
      isValidUser = false;
      isValidGroup = false;
      return false;
    }
  }
}
