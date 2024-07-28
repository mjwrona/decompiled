// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.GeoReplication.GeoReplicationHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Framework.Server.GeoReplication
{
  public static class GeoReplicationHelper
  {
    private const string c_area = "GeoReplication";
    private const string c_layer = "GeoReplicationHelper";

    private static Func<TComponent, bool> MakeFunc<TComponent>(Action<TComponent> action) => (Func<TComponent, bool>) (t =>
    {
      action(t);
      return true;
    });

    public static void PerformWrite<TComponent>(
      IVssRequestContext requestContext,
      Func<TComponent> createComponent,
      Action<TComponent> action,
      [CallerMemberName] string callerName = "")
      where TComponent : TeamFoundationSqlResourceComponent, new()
    {
      GeoReplicationHelper.PerformWriteWithRead<TComponent, bool>(requestContext, createComponent, GeoReplicationHelper.MakeFunc<TComponent>(action), callerName);
    }

    public static TResult PerformWriteWithRead<TComponent, TResult>(
      IVssRequestContext requestContext,
      Func<TComponent> createComponent,
      Func<TComponent, TResult> executeWrite,
      [CallerMemberName] string callerName = "")
      where TComponent : TeamFoundationSqlResourceComponent, new()
    {
      requestContext.CheckDeploymentRequestContext();
      string dataSource = (string) null;
      ITeamFoundationDatabaseProperties properties = (ITeamFoundationDatabaseProperties) null;
      DatabaseReadOnlyException readOnlyException = (DatabaseReadOnlyException) null;
      bool flag = requestContext.GetService<IGeoReplicationService>().GetGeoReplicationMode(requestContext) == GeoReplicationMode.PartitionDb;
      if (flag)
        requestContext.RootContext.Items[RequestContextItemsKeys.RedirectWritesOnReadOnlyDatabase] = (object) true;
      for (int index = 0; index < 3; ++index)
      {
        TComponent component = default (TComponent);
        try
        {
          if (index % 2 == 0)
          {
            component = createComponent();
            properties = component.DatabaseProperties;
          }
          else
            component = TeamFoundationResourceManagementService.CreateComponentRaw<TComponent>(properties.SqlConnectionInfo.CloneReplaceDataSource(dataSource), properties.RequestTimeout, properties.DeadlockPause, properties.DeadlockRetries);
          readOnlyException = (DatabaseReadOnlyException) null;
          return executeWrite(component);
        }
        catch (DatabaseReadOnlyException ex)
        {
          requestContext.TraceException(1932599845, "GeoReplication", nameof (GeoReplicationHelper), (Exception) ex);
          readOnlyException = ex;
          if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment || index == 2)
            throw;
          else if (index == 0)
          {
            component.Dispose();
            component = default (TComponent);
            GeoReplica geoReplica = requestContext.GetService<IGeoReplicationService>().QueryReplicas(requestContext, properties).FirstOrDefault<GeoReplica>();
            if (geoReplica == null)
            {
              requestContext.Trace(1616326506, TraceLevel.Info, "GeoReplication", nameof (GeoReplicationHelper), "Could not find replica for database {0}", (object) properties.DatabaseName);
              throw;
            }
            else if (geoReplica.IsPrimary)
            {
              requestContext.Trace(1960276636, TraceLevel.Info, "GeoReplication", nameof (GeoReplicationHelper), "Database {0} is primary already, retrying...", (object) properties.DatabaseName);
              ++index;
            }
            else
            {
              dataSource = geoReplica.PartnerServer;
              requestContext.Trace(1932067494, TraceLevel.Info, "GeoReplication", nameof (GeoReplicationHelper), "Found primary server {0} for database {1}", (object) dataSource, (object) properties.DatabaseName);
            }
          }
        }
        finally
        {
          if ((object) component != null)
            component.Dispose();
          if (flag)
            requestContext.RootContext.Items.Remove(RequestContextItemsKeys.RedirectWritesOnReadOnlyDatabase);
        }
      }
      if (readOnlyException != null)
        throw readOnlyException;
      throw new Exception("Failed to Execute a write to database " + callerName);
    }

    public static bool IsPrimaryInstance(IVssRequestContext requestContext) => requestContext.GetService<IGeoReplicationService>().IsPrimaryInstance(requestContext);

    public static string GetLogicalSqlServerName(string input) => input.Split('.')[0];

    public static SqlException CreateSqlException(int errorCode)
    {
      SqlErrorCollection sqlErrorCollection = typeof (SqlErrorCollection).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0].Invoke((object[]) null) as SqlErrorCollection;
      ((ArrayList) sqlErrorCollection.GetType().GetField("errors", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue((object) sqlErrorCollection)).Add((object) (((IEnumerable<ConstructorInfo>) typeof (SqlError).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)).FirstOrDefault<ConstructorInfo>((Func<ConstructorInfo, bool>) (f => f.GetParameters().Length == 8)).Invoke(new object[8]
      {
        (object) errorCode,
        (object) (byte) 0,
        (object) (byte) 0,
        (object) "",
        (object) "",
        (object) "",
        (object) 0,
        (object) 0U
      }) as SqlError));
      return Activator.CreateInstance(typeof (SqlException), BindingFlags.Instance | BindingFlags.NonPublic, (Binder) null, new object[4]
      {
        (object) string.Format("Error {0}", (object) errorCode),
        (object) sqlErrorCollection,
        null,
        (object) Guid.NewGuid()
      }, (CultureInfo) null) as SqlException;
    }

    public static void CreateTemporarySqlLogin(
      IVssRequestContext requestContext,
      ISqlConnectionInfo connectionInfo,
      string userId,
      string rawPassword,
      ITFLogger logger)
    {
      string[] strArray = new string[2]
      {
        "LoginManager",
        "DBManager"
      };
      requestContext.GetService<TeamFoundationDatabaseManagementService>().CreateSqlLogin(requestContext, connectionInfo, userId, rawPassword, true, logger);
      using (TeamFoundationSqlSecurityComponent componentRaw = connectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: logger))
      {
        string user = componentRaw.CreateUser(userId);
        foreach (string roleName in strArray)
          componentRaw.AddRoleMember(roleName, user);
      }
    }

    public static void RemoveTemporarySqlLogin(
      ISqlConnectionInfo connectionInfo,
      string userId,
      ITFLogger logger)
    {
      using (TeamFoundationSqlSecurityComponent componentRaw = connectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: logger))
        componentRaw.DropLogin(userId);
    }

    public static void CheckForGeoReplicationLinkCreationError(
      ISqlConnectionInfo dataTierConnectionInfo,
      string databaseName,
      TimeSpan waitTime)
    {
      using (SqlScriptResourceComponent componentRaw = dataTierConnectionInfo.CreateComponentRaw<SqlScriptResourceComponent>())
      {
        int num = -(int) Math.Round(10.0 * waitTime.TotalSeconds);
        string sqlStatement = string.Format("SELECT error_code FROM sys.dm_operation_status WHERE resource_type = 0 AND major_resource_id = '{0}' AND operation = 'CREATE CONTINUOUS DATABASE COPY' AND state = 3 AND last_modify_time > DATEADD(SECOND, {1}, GETDATE())", (object) databaseName, (object) num);
        SqlDataReader sqlDataReader = componentRaw.ExecuteStatementReader(sqlStatement, 300);
        if (sqlDataReader.Read())
          throw GeoReplicationHelper.CreateSqlException(sqlDataReader.GetInt32(0));
      }
    }

    public static bool TryParseDatabaseString(
      string input,
      out string sqlServer,
      out string databaseName)
    {
      string[] strArray = input.Split(';');
      if (strArray.Length != 2)
      {
        sqlServer = (string) null;
        databaseName = (string) null;
        return false;
      }
      sqlServer = strArray[0];
      databaseName = strArray[1];
      return true;
    }

    public static bool TryGetSourceDatabaseInfo(
      ISqlConnectionInfo sourceConfigDbConnectionInfo,
      int sourceDatabaseId,
      out ITeamFoundationDatabaseProperties dbProperties)
    {
      try
      {
        using (DatabaseManagementComponent componentRaw = sourceConfigDbConnectionInfo.CreateComponentRaw<DatabaseManagementComponent>())
          dbProperties = (ITeamFoundationDatabaseProperties) componentRaw.GetDatabase(sourceDatabaseId);
      }
      catch (DatabaseNotFoundException ex)
      {
        dbProperties = (ITeamFoundationDatabaseProperties) null;
        return false;
      }
      return true;
    }
  }
}
