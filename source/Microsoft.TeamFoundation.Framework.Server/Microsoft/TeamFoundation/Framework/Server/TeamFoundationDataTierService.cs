// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationDataTierService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server.DatabaseReplication;
using Microsoft.TeamFoundation.Framework.Server.GeoReplication;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationDataTierService : IVssFrameworkService
  {
    private static readonly string s_area = "DataTierService";
    private static readonly string s_layer = "IVssFrameworkService";
    private readonly char[] s_registryPathSeparatorArray = new char[1]
    {
      '/'
    };
    private const int c_tracePointStart = 9700;
    private const string c_lockResource = "DataTierService";
    private const string c_connectionString = "ConnectionString";
    private const string c_connectionStringTo = "ConnectionStringTo";
    private const string c_state = "State";
    private const string c_databaseCount = "DatabaseCount";
    private const string c_databaseDomain = ".database.windows.net";

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.CheckHostedDeployment();
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public TeamFoundationLock AcquireSharedLock(IVssRequestContext deploymentRequestContext) => TeamFoundationDataTierService.AcquireLock(deploymentRequestContext, TeamFoundationLockMode.Shared);

    public static void RegisterDataTierRaw(
      ISqlConnectionInfo connectionInfo,
      string connectionString,
      string userId,
      string unsecuredPassword,
      DataTierState state,
      string tags = null)
    {
      Guid databaseSigningKeyRaw = TeamFoundationSigningService.GetDatabaseSigningKeyRaw(connectionInfo);
      using (DataTierComponent componentRaw = connectionInfo.CreateComponentRaw<DataTierComponent>())
        TeamFoundationDataTierService.RegisterDataTier(componentRaw, connectionString, userId, unsecuredPassword, state, databaseSigningKeyRaw, tags);
    }

    public void RegisterDataTier(
      IVssRequestContext deploymentRequestContext,
      string connectionString,
      string userId,
      string unsecuredPassword,
      DataTierState state,
      string tags = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(deploymentRequestContext, nameof (deploymentRequestContext));
      deploymentRequestContext.CheckServicingRequestContext();
      deploymentRequestContext.CheckDeploymentRequestContext();
      int version = deploymentRequestContext.GetService<TeamFoundationResourceManagementService>().GetServiceVersion(deploymentRequestContext, "DataTier", "Default").Version;
      Guid databaseSigningKey = deploymentRequestContext.GetService<ITeamFoundationSigningService>().GetDatabaseSigningKey(deploymentRequestContext);
      using (DataTierComponent component = deploymentRequestContext.CreateComponent<DataTierComponent>())
        TeamFoundationDataTierService.RegisterDataTier(component, connectionString, userId, unsecuredPassword, state, databaseSigningKey, tags);
      TeamFoundationDataTierService.Trace(deploymentRequestContext, 3, TraceLevel.Info, "RegisterDataTier completed successfully.");
    }

    private static void RegisterDataTier(
      DataTierComponent dataTierComponent,
      string connectionString,
      string userId,
      string unsecuredPassword,
      DataTierState state,
      Guid signingKey,
      string tags = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(connectionString, nameof (connectionString));
      ArgumentUtility.CheckStringForNullOrEmpty(userId, nameof (userId));
      ArgumentUtility.CheckStringForNullOrEmpty(unsecuredPassword, nameof (unsecuredPassword));
      ArgumentUtility.CheckStringForNullOrEmpty(connectionString, nameof (connectionString));
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
      string str = connectionStringBuilder.DataSource;
      if (str.StartsWith(FrameworkServerConstants.TcpProtocolPrefix, StringComparison.OrdinalIgnoreCase))
        str = str.Substring(FrameworkServerConstants.TcpProtocolPrefix.Length);
      connectionStringBuilder.Password = "******";
      TeamFoundationTracingService.TraceRaw(0, TraceLevel.Info, TeamFoundationDataTierService.s_area, TeamFoundationDataTierService.s_layer, "Registering the following data tier: {0}. State: {1}. Tags: {2}", (object) str, (object) state, (object) tags);
      dataTierComponent.AddDataTier(connectionString, state, tags, userId, unsecuredPassword, signingKey);
    }

    public List<DataTierInfo> GetFilteredDataTiers(
      IVssRequestContext deploymentRequestContext,
      bool includeDatabaseCount,
      string tags)
    {
      List<DataTierInfo> dataTiers = this.GetDataTiers(deploymentRequestContext, includeDatabaseCount, true);
      List<DataTierInfo> list;
      if (string.IsNullOrEmpty(tags))
      {
        list = dataTiers.Where<DataTierInfo>((System.Func<DataTierInfo, bool>) (dt => dt.Tags == null)).ToList<DataTierInfo>();
      }
      else
      {
        string[] tagArray = tags.Split(';');
        list = dataTiers.Where<DataTierInfo>((System.Func<DataTierInfo, bool>) (dt => ((IEnumerable<string>) tagArray).All<string>((System.Func<string, bool>) (tag =>
        {
          if (dt.Tags == null)
            return false;
          return ((IEnumerable<string>) dt.Tags.Split(';')).Contains<string>(tag);
        })))).ToList<DataTierInfo>();
      }
      return list;
    }

    public virtual List<DataTierInfo> GetDataTiers(
      IVssRequestContext deploymentRequestContext,
      bool includeDatabaseCount,
      bool includeTagged = false)
    {
      deploymentRequestContext.CheckDeploymentRequestContext();
      TeamFoundationDataTierService.Trace(deploymentRequestContext, 4, TraceLevel.Info, "Getting data tiers.");
      int version = deploymentRequestContext.GetService<TeamFoundationResourceManagementService>().GetServiceVersion(deploymentRequestContext, "DataTier", "Default").Version;
      List<DataTierInfo> dataTierInfo1;
      using (DataTierComponent component = deploymentRequestContext.CreateComponent<DataTierComponent>())
        dataTierInfo1 = component.GetDataTierInfo();
      if (includeDatabaseCount)
      {
        TeamFoundationDataTierService.Trace(deploymentRequestContext, 6, TraceLevel.Info, "Querying database count.");
        foreach (DataTierInfo dataTierInfo2 in dataTierInfo1)
        {
          using (TeamFoundationDataTierComponent componentRaw = dataTierInfo2.ConnectionInfo.CreateComponentRaw<TeamFoundationDataTierComponent>())
          {
            try
            {
              TeamFoundationDataTierService.Trace(deploymentRequestContext, 7, TraceLevel.Info, "Querying the number of databases on the following data tier: {0}.", (object) dataTierInfo2.DataSource);
              dataTierInfo2.DatabaseCount = componentRaw.GetDatabaseCount();
              TeamFoundationDataTierService.Trace(deploymentRequestContext, 8, TraceLevel.Info, "Database Count: {0}.", (object) dataTierInfo2.DatabaseCount);
            }
            catch (Exception ex)
            {
              TeamFoundationDataTierService.Trace(deploymentRequestContext, 9, TraceLevel.Error, "Failed to get database count: {0}", (object) ex);
              TeamFoundationTrace.TraceException(ex);
              dataTierInfo2.DatabaseCount = -1;
            }
          }
        }
      }
      return dataTierInfo1;
    }

    public static List<DataTierInfo> GetDataTiersRaw(ISqlConnectionInfo connectionInfo)
    {
      using (DataTierComponent componentRaw = connectionInfo.CreateComponentRaw<DataTierComponent>())
        return componentRaw.GetDataTierInfo();
    }

    public DataTierInfo FindAssociatedDataTier(
      IVssRequestContext deploymentRequestContext,
      string databaseConnectionString,
      bool throwIfNotFound = true)
    {
      deploymentRequestContext.TraceEnter(0, TeamFoundationDataTierService.s_area, TeamFoundationDataTierService.s_layer, nameof (FindAssociatedDataTier));
      try
      {
        return TeamFoundationDataTierService.FindAssociatedDataTierHelper(this.GetDataTiers(deploymentRequestContext, false, true), databaseConnectionString, throwIfNotFound);
      }
      finally
      {
        deploymentRequestContext.TraceLeave(0, TeamFoundationDataTierService.s_area, TeamFoundationDataTierService.s_layer, nameof (FindAssociatedDataTier));
      }
    }

    public static DataTierInfo FindAssociatedDataTierRaw(
      ISqlConnectionInfo connectionInfo,
      string databaseConnectionString,
      bool throwIfNotFound = true)
    {
      TeamFoundationTracingService.TraceEnterRaw(0, TeamFoundationDataTierService.s_area, TeamFoundationDataTierService.s_layer, nameof (FindAssociatedDataTierRaw), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        return TeamFoundationDataTierService.FindAssociatedDataTierHelper(TeamFoundationDataTierService.GetDataTiersRaw(connectionInfo), databaseConnectionString, throwIfNotFound);
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(0, TeamFoundationDataTierService.s_area, TeamFoundationDataTierService.s_layer, nameof (FindAssociatedDataTierRaw));
      }
    }

    public void SetDataTierState(
      IVssRequestContext deploymentRequestContext,
      string dataSource,
      DataTierState state)
    {
      deploymentRequestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckStringForNullOrEmpty(dataSource, nameof (dataSource));
      TeamFoundationDataTierService.Trace(deploymentRequestContext, 10, TraceLevel.Info, "Setting data tier state. DataSource: {0}, State: {1}.", (object) dataSource, (object) state);
      if (dataSource.StartsWith(FrameworkServerConstants.TcpProtocolPrefix, StringComparison.OrdinalIgnoreCase))
        dataSource = dataSource.Substring(FrameworkServerConstants.TcpProtocolPrefix.Length);
      using (TeamFoundationDataTierService.AcquireExclusiveLock(deploymentRequestContext))
      {
        DataTierInfo dataTierInfo = this.GetDataTiers(deploymentRequestContext, false, true).FirstOrDefault<DataTierInfo>((System.Func<DataTierInfo, bool>) (dt => VssStringComparer.DataSourceIgnoreProtocol.Equals(dt.DataSource, dataSource)));
        if (dataTierInfo == null)
        {
          TeamFoundationDataTierService.Trace(deploymentRequestContext, 11, TraceLevel.Error, "Throwing AgrumentException - data tier is not registered.");
          throw new ArgumentException(FrameworkResources.DataTierNotRegistered());
        }
        using (DataTierComponent component = deploymentRequestContext.CreateComponent<DataTierComponent>())
          component.SetDataTierState(dataTierInfo.ConnectionInfo, state);
      }
      TeamFoundationDataTierService.Trace(deploymentRequestContext, 13, TraceLevel.Info, "SetDataTierState completed successfully.");
    }

    public void SetDataTierTags(
      IVssRequestContext deploymentRequestContext,
      string dataSource,
      string tags)
    {
      deploymentRequestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckStringForNullOrEmpty(dataSource, nameof (dataSource));
      TeamFoundationDataTierService.Trace(deploymentRequestContext, 27, TraceLevel.Info, "Setting data tier tags. DataSource: {0}, tags: {1}.", (object) dataSource, (object) tags);
      if (dataSource.StartsWith(FrameworkServerConstants.TcpProtocolPrefix, StringComparison.OrdinalIgnoreCase))
        dataSource = dataSource.Substring(FrameworkServerConstants.TcpProtocolPrefix.Length);
      using (TeamFoundationDataTierService.AcquireExclusiveLock(deploymentRequestContext))
      {
        DataTierInfo dataTierInfo = this.GetDataTiers(deploymentRequestContext, false, true).FirstOrDefault<DataTierInfo>((System.Func<DataTierInfo, bool>) (dt => VssStringComparer.DataSourceIgnoreProtocol.Equals(dt.DataSource, dataSource)));
        if (dataTierInfo == null)
        {
          TeamFoundationDataTierService.Trace(deploymentRequestContext, 28, TraceLevel.Error, "Throwing AgrumentException - data tier is not registered.");
          throw new ArgumentException(FrameworkResources.DataTierNotRegistered());
        }
        using (DataTierComponent component = deploymentRequestContext.CreateComponent<DataTierComponent>())
          component.SetDataTierTags(dataTierInfo.ConnectionInfo, tags);
      }
      TeamFoundationDataTierService.Trace(deploymentRequestContext, 29, TraceLevel.Info, "SetDataTierTags completed successfully.");
    }

    public void ResetDataTierPassword(
      IVssRequestContext deploymentRequestContext,
      string dataSource)
    {
      this.ResetDataTierPassword(deploymentRequestContext, dataSource, (ITFLogger) null);
    }

    public void ResetDataTierPassword(
      IVssRequestContext deploymentRequestContext,
      string dataSource,
      ITFLogger logger)
    {
      string loginPassword = SqlAzureLoginGenerator.CreateLoginPassword((string) null);
      this.ResetDataTierPassword(deploymentRequestContext, dataSource, loginPassword, true, logger);
    }

    internal void ResetDataTierPassword(
      IVssRequestContext deploymentRequestContext,
      string dataSource,
      string newPassword,
      bool alterSqlLogin,
      ITFLogger logger)
    {
      deploymentRequestContext.CheckServicingRequestContext();
      ArgumentUtility.CheckStringForNullOrEmpty(dataSource, nameof (dataSource));
      if (logger == null)
        logger = (ITFLogger) new ServerTraceLogger();
      TeamFoundationDataTierService.Trace(deploymentRequestContext, 14, TraceLevel.Info, "Resetting data tier password. Data source: {0}", (object) dataSource);
      logger.Info("Resetting data tier password. Data source: {0}", (object) dataSource);
      if (dataSource.StartsWith(FrameworkServerConstants.TcpProtocolPrefix, StringComparison.OrdinalIgnoreCase))
        dataSource = dataSource.Substring(FrameworkServerConstants.TcpProtocolPrefix.Length);
      using (TeamFoundationDataTierService.AcquireExclusiveLock(deploymentRequestContext))
      {
        DataTierInfo dataTier = this.GetDataTiers(deploymentRequestContext, false, true).FirstOrDefault<DataTierInfo>((System.Func<DataTierInfo, bool>) (dt => VssStringComparer.DataSourceIgnoreProtocol.Equals(dt.DataSource, dataSource)));
        if (dataTier == null)
        {
          TeamFoundationDataTierService.Trace(deploymentRequestContext, 15, TraceLevel.Error, "Throwing ArgumentException - data tier is not registered.");
          logger.Error("Data tier '{0}' not registered.", (object) dataSource);
          throw new ArgumentException(FrameworkResources.DataTierNotRegistered());
        }
        if (!(dataTier.ConnectionInfo is ISupportPasswordReset connectionInfo1))
          throw new InvalidOperationException(FrameworkResources.PasswordResetNotSupported());
        string userId = connectionInfo1.UserId;
        Guid databaseSigningKey = deploymentRequestContext.GetService<ITeamFoundationSigningService>().GetDatabaseSigningKey(deploymentRequestContext);
        List<AvailabilityReplica> availabilityGroup = TeamFoundationDatabaseManagementService.GetReplicaNodesFromAvailabilityGroup(dataTier.ConnectionInfo, logger);
        if (availabilityGroup.Any<AvailabilityReplica>((System.Func<AvailabilityReplica, bool>) (r => r.Health != AvailabilityReplicaSynchronizationState.Healthy)))
          throw new InvalidOperationException("Not all nodes in availability group are healthy. Unhealthy node(s): " + string.Join(",", availabilityGroup.Where<AvailabilityReplica>((System.Func<AvailabilityReplica, bool>) (r => r.Health != AvailabilityReplicaSynchronizationState.Healthy)).Select<AvailabilityReplica, string>((System.Func<AvailabilityReplica, string>) (r => r.Node))));
        List<ISqlConnectionInfo> list = availabilityGroup.Select<AvailabilityReplica, ISqlConnectionInfo>((System.Func<AvailabilityReplica, ISqlConnectionInfo>) (n => dataTier.ConnectionInfo.CloneReplaceDataSource(n.Node))).ToList<ISqlConnectionInfo>();
        if (list.Count == 0)
        {
          foreach (string failoverGroupServer in TeamFoundationDataTierService.GetSqlFailoverGroupServers(deploymentRequestContext, dataTier, logger))
            list.Add(dataTier.ConnectionInfo.CloneReplaceDataSource(failoverGroupServer));
        }
        try
        {
          using (DataTierComponent component = deploymentRequestContext.CreateComponent<DataTierComponent>())
          {
            TeamFoundationDataTierService.Trace(deploymentRequestContext, 16, TraceLevel.Info, "Writing Pending Connection String.");
            string unsecuredPassword = connectionInfo1.PasswordChanging(deploymentRequestContext, newPassword);
            component.PendDataTierReset(dataTier.ConnectionInfo, unsecuredPassword, databaseSigningKey);
          }
          if (alterSqlLogin)
          {
            foreach (ISqlConnectionInfo connectionInfo2 in list)
            {
              using (TeamFoundationSqlSecurityComponent componentRaw = connectionInfo2.CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: logger))
              {
                logger.Info("Altering Sql login " + userId + " on " + connectionInfo2.DataSource);
                TeamFoundationDataTierService.Trace(deploymentRequestContext, 17, TraceLevel.Info, "Altering Sql login.");
                componentRaw.AlterSqlLoginPassword(userId, newPassword);
              }
            }
          }
        }
        catch (Exception ex)
        {
          connectionInfo1.AbortPasswordChange(deploymentRequestContext);
          throw;
        }
        using (DataTierComponent component = deploymentRequestContext.CreateComponent<DataTierComponent>())
        {
          TeamFoundationDataTierService.Trace(deploymentRequestContext, 18, TraceLevel.Info, "Writing ConnectionString.");
          component.FlushDataTierReset(dataTier.ConnectionInfo);
          connectionInfo1.PasswordChanged(deploymentRequestContext);
        }
      }
      TeamFoundationDataTierService.Trace(deploymentRequestContext, 20, TraceLevel.Info, "ResetDataTierPassword completed successfully.");
      logger.Info("Password reset successfully. Data source: {0}", (object) dataSource);
    }

    public void RemoveDataTier(IVssRequestContext deploymentRequestContext, string dataSource)
    {
      deploymentRequestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckStringForNullOrEmpty(dataSource, nameof (dataSource));
      TeamFoundationDataTierService.Trace(deploymentRequestContext, 30, TraceLevel.Info, "Removing data tier. Data source: {0}", (object) dataSource);
      if (dataSource.StartsWith(FrameworkServerConstants.TcpProtocolPrefix, StringComparison.OrdinalIgnoreCase))
        dataSource = dataSource.Substring(FrameworkServerConstants.TcpProtocolPrefix.Length);
      using (TeamFoundationDataTierService.AcquireExclusiveLock(deploymentRequestContext))
      {
        DataTierInfo dataTierInfo = this.GetDataTiers(deploymentRequestContext, false, true).FirstOrDefault<DataTierInfo>((System.Func<DataTierInfo, bool>) (dt => VssStringComparer.DataSourceIgnoreProtocol.Equals(dt.DataSource, dataSource)));
        if (dataTierInfo == null)
        {
          TeamFoundationDataTierService.Trace(deploymentRequestContext, 31, TraceLevel.Error, "Throwing AgrumentException - data tier is not registered.");
          throw new ArgumentException(FrameworkResources.DataTierNotRegistered());
        }
        using (DataTierComponent component = deploymentRequestContext.CreateComponent<DataTierComponent>())
        {
          TeamFoundationDataTierService.Trace(deploymentRequestContext, 32, TraceLevel.Info, "Removing Data Tier");
          component.RemoveDataTier(dataTierInfo.ConnectionInfo);
        }
      }
      TeamFoundationDataTierService.Trace(deploymentRequestContext, 20, TraceLevel.Info, "RemoveDataTier completed successfully.");
    }

    public virtual bool DropDatabase(
      IVssRequestContext deploymentRequestContext,
      int databaseId,
      ITFLogger logger,
      out string warnings,
      ISqlConnectionInfo adminConnectionInfo = null,
      bool evenIfWarnings = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(deploymentRequestContext, nameof (deploymentRequestContext));
      deploymentRequestContext.CheckHostedDeployment();
      deploymentRequestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      warnings = (string) null;
      using (deploymentRequestContext.AcquireExemptionLock())
      {
        using (DatabaseManagementComponent component1 = deploymentRequestContext.CreateComponent<DatabaseManagementComponent>())
        {
          try
          {
            component1.BeginTransaction(IsolationLevel.RepeatableRead);
            ITeamFoundationDatabaseProperties properties;
            DataTierInfo info;
            ISqlConnectionInfo connectionInfo;
            bool databaseExists;
            if (!this.CanDropDatabase(deploymentRequestContext, databaseId, adminConnectionInfo, logger, out properties, out info, out connectionInfo, out databaseExists, out warnings))
            {
              logger.Warning(string.Format("Potential problems found with drop database {0}: {1}", (object) databaseId, (object) warnings));
              if (!evenIfWarnings)
              {
                component1.RollbackTransaction();
                return false;
              }
              logger.Info("Warnings have been by passed, continuing to drop the databases");
            }
            if (properties == null)
              throw new DatabaseNotFoundException(databaseId);
            if (info == null)
              throw new DataTierNotFoundException();
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(properties.ConnectionInfoWrapper.ConnectionString);
            DatabaseReplicationContext replicationContext = DatabaseReplicationContext.Default;
            if (databaseExists)
            {
              if (deploymentRequestContext.IsFeatureEnabled(FrameworkServerConstants.SqlFailoverGroupEnabled))
                replicationContext = deploymentRequestContext.GetService<IDatabaseFailoverGroupService>().GetDatabaseReplicationContext(deploymentRequestContext, properties, connectionInfo, logger);
              if (replicationContext != DatabaseReplicationContext.Default && replicationContext.IsPrimary && replicationContext.HasReplicas)
              {
                foreach (IDatabaseReplicaInfo replica in replicationContext.Replicas)
                  this.DropDatabaseInternal(connectionInfo.CloneReplaceDataSource(replica.DataTierConnectionInfo.DataSource), databaseId, connectionStringBuilder.InitialCatalog, logger);
              }
              this.DropDatabaseInternal(connectionInfo, databaseId, connectionStringBuilder.InitialCatalog, logger);
            }
            logger.Info(string.Format("Dropping Credential for Database {0} Name: {1}, Data Tier: {2}", (object) databaseId, (object) connectionStringBuilder.InitialCatalog, (object) info.DataSource));
            List<TeamFoundationDatabaseCredential> credentials;
            using (DatabaseCredentialsComponent component2 = deploymentRequestContext.CreateComponent<DatabaseCredentialsComponent>())
              credentials = component2.QueryDatabaseCredentials(databaseId);
            if (replicationContext != DatabaseReplicationContext.Default && replicationContext.IsPrimary && replicationContext.HasReplicas)
            {
              foreach (IDatabaseReplicaInfo replica in replicationContext.Replicas)
                this.DropLoginInternal(connectionInfo.CloneReplaceDataSource(replica.DataTierConnectionInfo.DataSource), credentials, logger);
            }
            this.DropLoginInternal(connectionInfo, credentials, logger);
            logger.Info("Unregistering database with dbms.");
            component1.RemoveDatabase(databaseId);
            component1.CommitTransaction();
            logger.Info(string.Format("Successfully deleted database {0}", (object) databaseId));
          }
          catch (Exception ex)
          {
            logger.Error(string.Format("Exception: {0}", (object) ex));
            component1.RollbackTransaction();
            throw;
          }
        }
      }
      return true;
    }

    public static string GetDatabaseName(string connectionString)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(connectionString, nameof (connectionString));
      return new SqlConnectionStringBuilder(connectionString).InitialCatalog;
    }

    public static string GetDataSource(string connectionString, DataSourceOptions options = DataSourceOptions.None)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(connectionString, nameof (connectionString));
      return TeamFoundationDataTierService.ManipulateDataSource(new SqlConnectionStringBuilder(connectionString).DataSource, options);
    }

    public static string ManipulateDataSource(string dataSource, DataSourceOptions options)
    {
      if (options == DataSourceOptions.None)
        return dataSource;
      if ((options & DataSourceOptions.RemoveProtocol) != DataSourceOptions.None)
        dataSource = TeamFoundationDataTierService.RemoveProtocol(dataSource);
      if ((options & DataSourceOptions.RemoveDomain) != DataSourceOptions.None)
        dataSource = TeamFoundationDataTierService.RemoveDomain(dataSource);
      if ((options & DataSourceOptions.RemoveDefaultSqlPort) != DataSourceOptions.None)
        dataSource = TeamFoundationDataTierService.RemoveDefaultSqlPort(dataSource);
      return dataSource;
    }

    public static bool CompareDataSource(
      string dataSource1,
      string dataSource2,
      DataSourceOptions options)
    {
      return string.Equals(TeamFoundationDataTierService.ManipulateDataSource(dataSource1, options), TeamFoundationDataTierService.ManipulateDataSource(dataSource2, options), StringComparison.OrdinalIgnoreCase);
    }

    public static string EnsureConnectionStringIncludesProtocol(string connectionString)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(connectionString, nameof (connectionString));
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
      if (string.IsNullOrWhiteSpace(connectionStringBuilder.DataSource))
        throw new TeamFoundationServiceException(FrameworkResources.DataSourceNotInConnectionStringError());
      if (!connectionStringBuilder.DataSource.StartsWith(FrameworkServerConstants.TcpProtocolPrefix, StringComparison.OrdinalIgnoreCase))
      {
        connectionStringBuilder.DataSource = FrameworkServerConstants.TcpProtocolPrefix + connectionStringBuilder.DataSource;
        connectionString = connectionStringBuilder.ConnectionString;
      }
      return connectionString;
    }

    public static string RemoveProtocol(string dataSource)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(dataSource, nameof (dataSource));
      return !dataSource.StartsWith(FrameworkServerConstants.TcpProtocolPrefix, StringComparison.OrdinalIgnoreCase) ? dataSource : dataSource.Substring(FrameworkServerConstants.TcpProtocolPrefix.Length);
    }

    public static string RemoveDomain(string dataSource)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(dataSource, nameof (dataSource));
      string[] strArray1 = dataSource.Split('.');
      if (strArray1.Length > 1)
      {
        dataSource = strArray1[0];
        string[] strArray2 = strArray1[strArray1.Length - 1].Split(new char[1]
        {
          ','
        }, 2);
        if (strArray2.Length > 1)
          dataSource = dataSource + "," + strArray2[1];
      }
      return dataSource;
    }

    public static string RemoveDefaultSqlPort(string dataSource)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(dataSource, nameof (dataSource));
      return dataSource.EndsWith(",1433") ? dataSource.Substring(0, dataSource.Length - ",1433".Length) : dataSource;
    }

    public static List<string> GetSqlFailoverGroupServers(
      IVssRequestContext requestContext,
      DataTierInfo dataTier,
      ITFLogger logger)
    {
      string lowerInvariant = dataTier.GetDataSource(DataSourceOptions.RemoveProtocol).ToLowerInvariant();
      List<string> failoverGroupServers = new List<string>()
      {
        lowerInvariant
      };
      List<GeoReplicaView> geoReplicaViewList;
      using (GeoReplicationRawComponent componentRaw = dataTier.ConnectionInfo.CreateComponentRaw<GeoReplicationRawComponent>())
        geoReplicaViewList = componentRaw.QueryReplicasFromMaster();
      if (geoReplicaViewList.Count > 0 && requestContext.GetService<IGeoReplicationService>().GetGeoReplicationMode(requestContext) != GeoReplicationMode.PartitionDb)
      {
        string str1 = TeamFoundationDataTierService.ManipulateDataSource(lowerInvariant, DataSourceOptions.RemoveProtocolAndDomain);
        if (str1.EndsWith(SqlFailoverGroupConstants.FailoverGroupSuffix))
        {
          string dataSource = str1 + ".secondary.database.windows.net";
          ISqlConnectionInfo connectionInfo = dataTier.ConnectionInfo.CloneReplaceDataSource(dataSource);
          try
          {
            using (GeoReplicationRawComponent componentRaw = connectionInfo.CreateComponentRaw<GeoReplicationRawComponent>(30))
              componentRaw.QueryReplicasFromMaster();
            failoverGroupServers.Add(dataSource);
          }
          catch
          {
            logger.Info("Cannot connect to secondary listener " + dataSource + ". Most likely this is not set up as a failover group");
          }
        }
        else
        {
          logger.Info("Replica from master database.");
          foreach (GeoReplicaView geoReplicaView in geoReplicaViewList)
            logger.Info("Partner database: " + geoReplicaView.PartnerDatabase + ", Partner server: " + geoReplicaView.PartnerServer + " and replication state: " + geoReplicaView.ReplicationState);
          string str2 = "https://dev.azure.com/mseng/AzureDevOps/_wiki/wikis/AzureDevOps.wiki/4793/Setting-up-SQL-Failover-Group?anchor=cleanup-failover-group";
          logger.Warning("Detected replicas, but the data tier " + lowerInvariant + " is not a failover group. This may indicate that a failover group was not properly cleaned up. Setting release status to partial success. " + str2);
          logger.Info("##vso[task.complete result=SucceededWithIssues;]DONE");
        }
      }
      return failoverGroupServers;
    }

    public void FixDatatierLogins(
      IVssRequestContext deploymentContext,
      ISqlConnectionInfo adminConnectionInfo,
      ITFLogger logger)
    {
      foreach (ITeamFoundationDatabaseProperties queryDatabase in deploymentContext.GetService<ITeamFoundationDatabaseManagementService>().QueryDatabases(deploymentContext))
      {
        bool flag = false;
        if (queryDatabase.Status == TeamFoundationDatabaseStatus.Failed || queryDatabase.Status == TeamFoundationDatabaseStatus.Creating)
          logger.Info(string.Format("Skip fixing DT login since {0} is not in a valid state.  Status: {1}", (object) queryDatabase.DatabaseId, (object) queryDatabase.Status));
        else if (queryDatabase.IsExternalDatabase())
        {
          logger.Info(string.Format("Skip fixing DT login since {0} is an external database", (object) queryDatabase.DatabaseId));
        }
        else
        {
          ISqlConnectionInfo connectionInfo = this.FindAssociatedDataTier(deploymentContext, queryDatabase.SqlConnectionInfo.ConnectionString).ConnectionInfo.CloneReplaceInitialCatalog(queryDatabase.SqlConnectionInfo.InitialCatalog);
          string userId = ((ISupportSqlCredential) connectionInfo).UserId;
          try
          {
            using (TeamFoundationSqlSecurityComponent componentRaw = connectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>(30))
            {
              if (componentRaw.GetUserId() == 1)
                logger.Info(userId + " is dbo for database " + queryDatabase.SqlConnectionInfo.InitialCatalog);
              else if (componentRaw.IsRoleMember(DatabaseRoles.DbOwner))
                logger.Info(userId + " is in db_owner role for database " + queryDatabase.SqlConnectionInfo.InitialCatalog);
              else
                flag = true;
            }
          }
          catch (Exception ex)
          {
            logger.Warning("Exception: " + ex.ToReadableStackTrace());
            flag = true;
          }
          if (flag)
          {
            logger.Warning("Adding DataTier login " + userId + " to db_owner of database " + queryDatabase.SqlConnectionInfo.InitialCatalog + " on " + queryDatabase.SqlConnectionInfo.DataSource + "!");
            try
            {
              using (TeamFoundationSqlSecurityComponent componentRaw = adminConnectionInfo.CloneReplaceDataSource(queryDatabase.SqlConnectionInfo.DataSource).CloneReplaceInitialCatalog(queryDatabase.SqlConnectionInfo.InitialCatalog).CreateComponentRaw<TeamFoundationSqlSecurityComponent>())
              {
                logger.Info("Create User " + userId + " for database " + queryDatabase.SqlConnectionInfo.InitialCatalog);
                string user = componentRaw.CreateUser(userId);
                logger.Info("Adding database user " + user + " to the following role: " + DatabaseRoles.DbOwner);
                componentRaw.AddRoleMember(DatabaseRoles.DbOwner, user);
              }
            }
            catch (Exception ex)
            {
              logger.Warning("Exception caught adding " + userId + " to " + queryDatabase.SqlConnectionInfo.DataSource + ": " + ex.ToReadableStackTrace());
            }
          }
        }
      }
    }

    internal static TeamFoundationLock AcquireLock(
      ISqlConnectionInfo configDbConnectionInfo,
      TeamFoundationLockMode lockMode)
    {
      LockingComponent componentRaw = configDbConnectionInfo.CreateComponentRaw<LockingComponent>();
      componentRaw.AcquireLock("DataTierService", lockMode, -1);
      return new TeamFoundationLock(componentRaw, lockMode, false, new string[1]
      {
        "DataTierService"
      });
    }

    private bool CanDropDatabase(
      IVssRequestContext deploymentRequestContext,
      int databaseId,
      ISqlConnectionInfo adminConnectionInfo,
      ITFLogger logger,
      out ITeamFoundationDatabaseProperties properties,
      out DataTierInfo info,
      out ISqlConnectionInfo connectionInfo,
      out bool databaseExists,
      out string warning)
    {
      properties = (ITeamFoundationDatabaseProperties) null;
      info = (DataTierInfo) null;
      connectionInfo = (ISqlConnectionInfo) null;
      warning = (string) null;
      databaseExists = false;
      try
      {
        ITeamFoundationDatabaseManagementService service = deploymentRequestContext.GetService<ITeamFoundationDatabaseManagementService>();
        properties = service.GetDatabase(deploymentRequestContext, databaseId);
      }
      catch (DatabaseNotFoundException ex)
      {
        warning = string.Format("Database {0} could not be found", (object) databaseId);
        return false;
      }
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(properties.ConnectionInfoWrapper.ConnectionString);
      info = this.FindAssociatedDataTier(deploymentRequestContext, connectionStringBuilder.ConnectionString, false);
      if (info == null)
      {
        warning = string.Format("Data Tier for Database {0} {1} could not be found", (object) databaseId, (object) connectionStringBuilder.DataSource);
        return false;
      }
      connectionInfo = adminConnectionInfo != null ? adminConnectionInfo.Create(info.ConnectionInfo.ConnectionString) : info.ConnectionInfo;
      using (TeamFoundationDataTierComponent componentRaw = connectionInfo.CreateComponentRaw<TeamFoundationDataTierComponent>())
        databaseExists = componentRaw.CheckIfDatabaseExists(connectionStringBuilder.InitialCatalog);
      if (databaseExists)
      {
        ISqlConnectionInfo sqlConnectionInfo = properties.ConnectionInfoWrapper.ToSqlConnectionInfo(deploymentRequestContext);
        List<DatabasePartition> source;
        try
        {
          using (DatabasePartitionComponent component = DatabasePartitionComponent.CreateComponent(sqlConnectionInfo))
            source = component.QueryPartitions();
        }
        catch (DatabaseConfigurationException ex)
        {
          source = new List<DatabasePartition>();
        }
        int count1 = source.Count;
        List<DatabasePartition> list = source.Where<DatabasePartition>((System.Func<DatabasePartition, bool>) (p => p.State != DatabasePartitionState.Deleted)).ToList<DatabasePartition>();
        int count2 = list.Count;
        if (count2 > 0)
        {
          TeamFoundationHostManagementService service = deploymentRequestContext.GetService<TeamFoundationHostManagementService>();
          int num1 = deploymentRequestContext.GetService<IVssRegistryService>().GetValue<int>(deploymentRequestContext, (RegistryQuery) FrameworkServerConstants.ServicingCleanupRetainDeletedPartitionHours, 336);
          int num2 = num1 < 168 ? 168 : num1;
          DateTime dateTime1 = DateTime.UtcNow.AddHours((double) -num2);
          DateTime dateTime2 = DateTime.UtcNow.AddHours(-1.0);
          for (int index = count2 - 1; index >= 0; --index)
          {
            DatabasePartition databasePartition = list[index];
            logger.Info(string.Format("Checking partition {0}. State: {1}, Host Id: {2}, Host Type: {3}, State Change Date: {4}", (object) databasePartition.PartitionId, (object) databasePartition.State, (object) databasePartition.ServiceHostId, (object) databasePartition.HostType, (object) databasePartition.StateChangedDate));
            if (databasePartition.StateChangedDate <= dateTime2)
            {
              TeamFoundationServiceHostProperties serviceHostProperties = service.QueryServiceHostProperties(deploymentRequestContext, databasePartition.ServiceHostId);
              if (serviceHostProperties == null)
              {
                list.RemoveAt(index);
                logger.Info(string.Format("There is no host with id: {0}. Ignoring this partition.", (object) databasePartition.ServiceHostId));
              }
              else if (serviceHostProperties.DatabaseId != databaseId)
              {
                logger.Info(string.Format("The host is registered in a different database {0}.", (object) serviceHostProperties.DatabaseId));
                if (databasePartition.State == DatabasePartitionState.Servicing && databasePartition.StateChangedDate < dateTime1)
                {
                  list.RemoveAt(index);
                  logger.Info(string.Format("The host is moved to a different database for more than {0} days. Ignoring this partition.", (object) TimeSpan.FromHours((double) num2).Days));
                }
              }
              else
                logger.Info("Active Host Found for this partition!");
            }
          }
          if (list.Count > 0)
          {
            int num3 = count1 - count2;
            if (num3 > 0)
              warning = string.Format("Database {0} has {1} active partitions and {2} that are pending deletion.\n", (object) databaseId, (object) count2, (object) num3);
            warning += string.Format("Database {0} has {1} hosts registered in the tbl_ServiceHost.", (object) databaseId, (object) list.Count);
            return false;
          }
        }
      }
      return true;
    }

    private void DropDatabaseInternal(
      ISqlConnectionInfo connectionInfo,
      int databaseId,
      string databaseName,
      ITFLogger logger)
    {
      logger.Info(string.Format("Dropping Database {0} Name: {1}, Data Tier: {2}", (object) databaseId, (object) databaseName, (object) connectionInfo.DataSource));
      try
      {
        using (TeamFoundationDataTierComponent componentRaw = connectionInfo.CreateComponentRaw<TeamFoundationDataTierComponent>())
          componentRaw.DropDatabase(databaseName, DropDatabaseOptions.CloseExistingConnections);
      }
      catch (Exception ex)
      {
        logger.Warning(string.Format("Encounter exception when dropping Database Name: {0}, Data Tier: {1}, Exception: {2}", (object) databaseName, (object) connectionInfo.DataSource, (object) ex));
        bool flag;
        using (TeamFoundationDataTierComponent componentRaw = connectionInfo.CreateComponentRaw<TeamFoundationDataTierComponent>())
          flag = componentRaw.CheckIfDatabaseExists(databaseName);
        if (!flag)
          logger.Warning(string.Format("Database {0} Name: {1}, Data Tier: {2} is dropped. Continue..", (object) databaseId, (object) databaseName, (object) connectionInfo.DataSource));
        else
          throw;
      }
    }

    private void DropLoginInternal(
      ISqlConnectionInfo connectionInfo,
      List<TeamFoundationDatabaseCredential> credentials,
      ITFLogger logger)
    {
      using (TeamFoundationSqlSecurityComponent componentRaw = connectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>())
      {
        foreach (TeamFoundationDatabaseCredential credential in credentials)
        {
          if (componentRaw.GetLogin(credential.UserId) != null)
          {
            componentRaw.DropLogin(credential.UserId);
            logger.Info("The following login has been dropped: " + credential.UserId + "; SQL instance " + connectionInfo.DataSource + ".");
          }
          else
            logger.Info("The following login does not exist: " + credential.UserId + "; SQL instance " + connectionInfo.DataSource + ".");
        }
      }
    }

    private static DataTierInfo FindAssociatedDataTierHelper(
      List<DataTierInfo> dataTiers,
      string databaseConnectionString,
      bool throwIfNotFound = true)
    {
      DataSourceOptions options = DataSourceOptions.RemoveProtocol | DataSourceOptions.RemoveDefaultSqlPort;
      string dataSource1 = TeamFoundationDataTierService.GetDataSource(databaseConnectionString, options);
      DataTierInfo dataTierInfo = (DataTierInfo) null;
      foreach (DataTierInfo dataTier in dataTiers)
      {
        string dataSource2 = dataTier.GetDataSource(options);
        if (string.Equals(dataSource2, dataSource1, StringComparison.OrdinalIgnoreCase))
        {
          dataTierInfo = dataTier;
          break;
        }
        if (string.Equals(TeamFoundationDataTierService.ManipulateDataSource(dataSource2, DataSourceOptions.RemoveDomain), dataSource1, StringComparison.OrdinalIgnoreCase))
        {
          dataTierInfo = dataTier;
          break;
        }
      }
      return !(dataTierInfo == null & throwIfNotFound) ? dataTierInfo : throw new DataTierNotFoundException(dataSource1);
    }

    private static TeamFoundationLock AcquireLock(
      IVssRequestContext deploymentRequestContext,
      TeamFoundationLockMode lockMode)
    {
      deploymentRequestContext.CheckDeploymentRequestContext();
      TeamFoundationDataTierService.Trace(deploymentRequestContext, 21, TraceLevel.Info, "Acquiring lock. LockMode: {0}", (object) lockMode);
      TeamFoundationLock teamFoundationLock = deploymentRequestContext.GetService<ITeamFoundationLockingService>().AcquireLock(deploymentRequestContext, lockMode, "DataTierService");
      TeamFoundationDataTierService.Trace(deploymentRequestContext, 22, TraceLevel.Info, "Lock has been acquired.");
      return teamFoundationLock;
    }

    private static TeamFoundationLock AcquireExclusiveLock(
      IVssRequestContext deploymentRequestContext)
    {
      return TeamFoundationDataTierService.AcquireLock(deploymentRequestContext, TeamFoundationLockMode.Exclusive);
    }

    private static bool IsTracing(
      IVssRequestContext requestContext,
      int tracePointOffset,
      TraceLevel traceLevel)
    {
      return requestContext.IsTracing(9700 + tracePointOffset, traceLevel, TeamFoundationDataTierService.s_area, TeamFoundationDataTierService.s_layer);
    }

    private static void Trace(
      IVssRequestContext requestContext,
      int tracePointOffset,
      TraceLevel traceLevel,
      string message)
    {
      requestContext.Trace(9700 + tracePointOffset, traceLevel, TeamFoundationDataTierService.s_area, TeamFoundationDataTierService.s_layer, message);
    }

    private static void Trace(
      IVssRequestContext requestContext,
      int tracePointOffset,
      TraceLevel traceLevel,
      string format,
      params object[] args)
    {
      VssRequestContextExtensions.Trace(requestContext, 9700 + tracePointOffset, traceLevel, TeamFoundationDataTierService.s_area, TeamFoundationDataTierService.s_layer, format, args);
    }
  }
}
