// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.BackEnd.ServerNodeComponent
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.BackEnd
{
  internal class ServerNodeComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory((IComponentCreator[]) new ComponentCreator<ServerNodeComponent>[1]
    {
      new ComponentCreator<ServerNodeComponent>(1)
    }, "SmartRouter");
    private const string c_smartRouterSchema = "SmartRouter";
    private const string prc_CreateOrUpdateServerNode = "SmartRouter.prc_CreateOrUpdateServerNode";
    private const string prc_DeleteServerNode = "SmartRouter.prc_DeleteServerNode";
    private const string prc_GetServerNode = "SmartRouter.prc_GetServerNode";
    private const string prc_GetActiveServerNodes = "SmartRouter.prc_GetActiveServerNodes";
    private const string prc_PurgeExpiredServerNodes = "SmartRouter.prc_PurgeExpiredServerNodes";
    private const int c_roleNameColumnWidth = 20;
    private const int c_roleInstanceColumnWidth = 50;
    private const int c_ipAddressColumnWidth = 45;

    public ServerNodeComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.None;

    public virtual ServerNodeRecord AddOrUpdateServer(ServerNodeRecord serverRecord)
    {
      serverRecord = serverRecord.CheckArgumentIsNotNull<ServerNodeRecord>(nameof (serverRecord));
      ArgumentUtility.CheckStringLength(serverRecord.RoleName, "RoleName", 20);
      ArgumentUtility.CheckStringLength(serverRecord.RoleInstance, "RoleInstance", 50);
      ArgumentUtility.CheckStringLength(serverRecord.IPAddress, "IPAddress", 45);
      this.PrepareStoredProcedure("SmartRouter.prc_CreateOrUpdateServerNode");
      this.BindRoleNameParam(serverRecord.RoleName);
      this.BindRoleInstanceParam(serverRecord.RoleInstance);
      this.BindIPAddressParam(serverRecord.IPAddress);
      this.BindLastUpdatedParam(serverRecord.LastUpdated);
      this.BindExpirationParam(serverRecord.Expiration);
      return this.ExecuteServerRecordReader().FirstOrDefault<ServerNodeRecord>();
    }

    public ServerNodeRecord GetServer(string roleName, string roleInstance)
    {
      roleName.CheckArgumentIsNotNullOrEmpty(nameof (roleName));
      roleInstance.CheckArgumentIsNotNullOrEmpty(nameof (roleInstance));
      this.PrepareStoredProcedure("SmartRouter.prc_GetServerNode");
      this.BindRoleNameParam(roleName);
      this.BindRoleInstanceParam(roleInstance);
      return this.ExecuteServerRecordReader().FirstOrDefault<ServerNodeRecord>();
    }

    public virtual ServerNodeRecord DeleteServer(string roleName, string roleInstance)
    {
      roleName.CheckArgumentIsNotNullOrEmpty(nameof (roleName));
      roleInstance.CheckArgumentIsNotNullOrEmpty(nameof (roleInstance));
      this.PrepareStoredProcedure("SmartRouter.prc_DeleteServerNode");
      this.BindRoleNameParam(roleName);
      this.BindRoleInstanceParam(roleInstance);
      return this.ExecuteServerRecordReader().FirstOrDefault<ServerNodeRecord>();
    }

    public virtual IEnumerable<ServerNodeRecord> GetActiveServers(string roleName)
    {
      roleName.CheckArgumentIsNotNullOrEmpty(nameof (roleName));
      this.PrepareStoredProcedure("SmartRouter.prc_GetActiveServerNodes");
      this.BindRoleNameParam(roleName);
      return this.ExecuteServerRecordReader();
    }

    public IEnumerable<ServerNodeRecord> PurgeExpiredServers(string roleName)
    {
      roleName.CheckArgumentIsNotNullOrEmpty(nameof (roleName));
      this.PrepareStoredProcedure("SmartRouter.prc_PurgeExpiredServerNodes");
      this.BindRoleNameParam(roleName);
      return this.ExecuteServerRecordReader();
    }

    private IEnumerable<ServerNodeRecord> ExecuteServerRecordReader()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ServerNodeRecord>((ObjectBinder<ServerNodeRecord>) new ServerNodeComponent.ServerRecordBinder());
        return (IEnumerable<ServerNodeRecord>) resultCollection.GetCurrent<ServerNodeRecord>().Items;
      }
    }

    private SqlParameter BindRoleNameParam(string roleName) => this.BindString("@roleName", roleName, 20, BindStringBehavior.Unchanged, SqlDbType.VarChar);

    private SqlParameter BindRoleInstanceParam(string roleInstance) => this.BindString("@roleInstance", roleInstance, 50, BindStringBehavior.Unchanged, SqlDbType.VarChar);

    private SqlParameter BindIPAddressParam(string ipAddress) => this.BindString("@ipAddress", ipAddress, 45, BindStringBehavior.Unchanged, SqlDbType.VarChar);

    private SqlParameter BindLastUpdatedParam(DateTime lastUpdated) => this.BindDateTime2("@lastUpdated", lastUpdated);

    private SqlParameter BindExpirationParam(DateTime expiration) => this.BindDateTime2("@expiration", expiration);

    private class ServerRecordBinder : ObjectBinder<ServerNodeRecord>
    {
      private SqlColumnBinder m_roleName = new SqlColumnBinder("RoleName");
      private SqlColumnBinder m_roleInstance = new SqlColumnBinder("RoleInstance");
      private SqlColumnBinder m_ipAddress = new SqlColumnBinder("IPAddress");
      private SqlColumnBinder m_lastUpdated = new SqlColumnBinder("LastUpdated");
      private SqlColumnBinder m_expiration = new SqlColumnBinder("Expiration");

      protected override ServerNodeRecord Bind()
      {
        string roleName = this.m_roleName.GetString((IDataReader) this.Reader, false);
        string str1 = this.m_roleInstance.GetString((IDataReader) this.Reader, false);
        string str2 = this.m_ipAddress.GetString((IDataReader) this.Reader, false);
        DateTime dateTime1 = this.m_lastUpdated.GetDateTime((IDataReader) this.Reader);
        DateTime dateTime2 = this.m_expiration.GetDateTime((IDataReader) this.Reader);
        string roleInstance = str1;
        string ipAddress = str2;
        DateTime lastUpdated = dateTime1;
        DateTime expiration = dateTime2;
        return new ServerNodeRecord(roleName, roleInstance, ipAddress, lastUpdated, expiration);
      }
    }
  }
}
