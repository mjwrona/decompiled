// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalGetDbStampElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalGetDbStampElement : DalSqlElement
  {
    internal static readonly PayloadTableSchema DbStampTableSchema = new PayloadTableSchema(new PayloadTableSchema.Column[1]
    {
      new PayloadTableSchema.Column("DbStamp", typeof (string))
    });

    public virtual void JoinBatch()
    {
      this.AppendSql("select Dbo.GetDbStamp(");
      this.AppendPartitionIdVariable();
      if (this.SqlBatch.RequestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(this.SqlBatch.RequestContext).MetadataFilterEnabled)
        this.AppendSql("@PersonId");
      else
        this.AppendSql("default");
      this.AppendSql(",");
      this.AppendSql(DalSqlElement.InlineInt(this.ClientVersion));
      this.AppendSql(") as DbStamp");
      if (this.IsSchemaPartitioned)
      {
        this.AppendSql(" OPTION (OPTIMIZE FOR (");
        this.AppendSql("@partitionId");
        this.AppendSql(" UNKNOWN))");
      }
      this.AppendSql(Environment.NewLine);
      this.m_outputs = 1;
      this.SqlBatch.SetTableSchema(1, (IPayloadTableSchema) DalGetDbStampElement.DbStampTableSchema);
      this.m_index = this.SqlBatch.AddExpectedReturnedDataTables(this.m_outputs);
    }

    public virtual string GetDbStamp() => (string) this.SqlBatch.ResultPayload.Tables[this.m_index].Rows[0][0];
  }
}
