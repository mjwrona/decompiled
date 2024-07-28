// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlAzureFirewallComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SqlAzureFirewallComponent : TeamFoundationSqlResourceComponent
  {
    private const string c_databaseFirewallRulesQuery = "SELECT  name, start_ip_address, end_ip_address, create_date, modify_date FROM sys.database_firewall_rules";
    private const string c_sqlAzureFirewallRulesQuery = "SELECT  name, start_ip_address, end_ip_address, create_date, modify_date FROM sys.firewall_rules";

    public List<SqlAzureFirewallRule> GetDataseFirewallRules()
    {
      this.PrepareSqlBatch("SELECT  name, start_ip_address, end_ip_address, create_date, modify_date FROM sys.database_firewall_rules".Length);
      this.AddStatement("SELECT  name, start_ip_address, end_ip_address, create_date, modify_date FROM sys.database_firewall_rules");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "c_databaseFirewallRulesQuery", (IVssRequestContext) null))
      {
        resultCollection.AddBinder<SqlAzureFirewallRule>((ObjectBinder<SqlAzureFirewallRule>) new SqlAzureFirewallComponent.SqlAzureFirewallRuleBinder());
        return resultCollection.GetCurrent<SqlAzureFirewallRule>().Items;
      }
    }

    public List<SqlAzureFirewallRule> GetSqlAzureFirewallRules()
    {
      this.PrepareSqlBatch("SELECT  name, start_ip_address, end_ip_address, create_date, modify_date FROM sys.firewall_rules".Length);
      this.AddStatement("SELECT  name, start_ip_address, end_ip_address, create_date, modify_date FROM sys.firewall_rules");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "c_sqlAzureFirewallRulesQuery", (IVssRequestContext) null))
      {
        resultCollection.AddBinder<SqlAzureFirewallRule>((ObjectBinder<SqlAzureFirewallRule>) new SqlAzureFirewallComponent.SqlAzureFirewallRuleBinder());
        return resultCollection.GetCurrent<SqlAzureFirewallRule>().Items;
      }
    }

    public void SetFirewallRule(string name, string startIpAddress, string endIpAddress)
    {
      this.PrepareStoredProcedure("sp_set_firewall_rule");
      this.BindString("@name", name, 128, false, SqlDbType.NVarChar);
      this.BindString("@start_ip_address", startIpAddress, 50, false, SqlDbType.VarChar);
      this.BindString("@end_ip_address", endIpAddress, 50, false, SqlDbType.VarChar);
      this.ExecuteNonQuery();
    }

    public void DeleteFirewallRule(string name)
    {
      this.PrepareStoredProcedure("sp_delete_firewall_rule");
      this.BindString("@name", name, 128, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public void SetDatabaseFirewallRule(string name, string startIpAddress, string endIpAddress)
    {
      this.PrepareStoredProcedure("sp_set_database_firewall_rule");
      this.BindString("@name", name, 128, false, SqlDbType.NVarChar);
      this.BindString("@start_ip_address", startIpAddress, 50, false, SqlDbType.VarChar);
      this.BindString("@end_ip_address", startIpAddress, 50, false, SqlDbType.VarChar);
      this.ExecuteNonQuery();
    }

    public void DeleteDatabaseFirewallRule(string name)
    {
      this.PrepareStoredProcedure("sp_delete_database_firewall_rule");
      this.BindString("@name", name, 128, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    private class SqlAzureFirewallRuleBinder : ObjectBinder<SqlAzureFirewallRule>
    {
      private SqlColumnBinder m_nameColumn = new SqlColumnBinder("name");
      private SqlColumnBinder m_startIpAddressColumn = new SqlColumnBinder("start_ip_address");
      private SqlColumnBinder m_endIpAddressColumn = new SqlColumnBinder("end_ip_address");
      private SqlColumnBinder m_createDateColumn = new SqlColumnBinder("create_date");
      private SqlColumnBinder m_modifyDateColumn = new SqlColumnBinder("modify_date");

      protected override SqlAzureFirewallRule Bind() => new SqlAzureFirewallRule()
      {
        Name = this.m_nameColumn.GetString((IDataReader) this.Reader, false),
        StartIpAddress = this.m_startIpAddressColumn.GetString((IDataReader) this.Reader, false),
        EndIpAddress = this.m_endIpAddressColumn.GetString((IDataReader) this.Reader, false),
        CreateDate = this.m_createDateColumn.GetDateTime((IDataReader) this.Reader),
        ModifyDate = this.m_modifyDateColumn.GetDateTime((IDataReader) this.Reader)
      };
    }
  }
}
