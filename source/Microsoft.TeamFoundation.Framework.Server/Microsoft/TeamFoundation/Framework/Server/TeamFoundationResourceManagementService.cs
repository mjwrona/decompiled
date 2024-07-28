// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationResourceManagementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class TeamFoundationResourceManagementService : 
    VssBaseService,
    ITeamFoundationResourceManagementService,
    IVssFrameworkService,
    ISqlComponentCreator
  {
    private IVssServiceHost m_serviceHost;
    private static readonly string s_area = "ResourceManagement";
    private static readonly string s_layer = "Service";

    internal TeamFoundationResourceManagementService()
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(90000, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "ServiceStart");
      this.m_serviceHost = systemRequestContext.ServiceHost;
      systemRequestContext.Trace(90001, TraceLevel.Info, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "Host Id: {0}, Host Type: {1}.", (object) systemRequestContext.ServiceHost.InstanceId, (object) systemRequestContext.ServiceHost.HostType);
      systemRequestContext.TraceLeave(90001, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "ServiceStart");
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public TComponent CreateComponent<TComponent>(IVssRequestContext requestContext) where TComponent : class, ISqlResourceComponent, new() => this.CreateComponent<TComponent>(requestContext, (string) null);

    public TComponent CreateComponent<TComponent>(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid? dataspaceIdentifier = null,
      DatabaseConnectionType? connectionType = null,
      ITFLogger logger = null)
      where TComponent : class, ISqlResourceComponent, new()
    {
      this.ValidateRequestContext(requestContext);
      if (requestContext.IsVirtualServiceHost())
        throw new VirtualServiceHostException();
      requestContext.TraceEnter(90020, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, nameof (CreateComponent));
      try
      {
        if (!connectionType.HasValue)
        {
          VssReadConsistencyLevel consistencyLevel;
          bool flag1 = requestContext.Items.TryGetValue<VssReadConsistencyLevel>("ReadConsistencyLevel", out consistencyLevel);
          int num;
          if (!flag1)
          {
            int hostType1 = (int) requestContext.ServiceHost.HostType;
            TeamFoundationHostType? hostType2 = requestContext.RootContext?.ServiceHost.HostType;
            int valueOrDefault = (int) hostType2.GetValueOrDefault();
            if (hostType1 == valueOrDefault & hostType2.HasValue)
            {
              IVssRequestContext rootContext = requestContext.RootContext;
              bool? nullable;
              if (rootContext == null)
              {
                nullable = new bool?();
              }
              else
              {
                IDictionary<string, object> items = rootContext.Items;
                nullable = items != null ? new bool?(items.TryGetValue<VssReadConsistencyLevel>("ReadConsistencyLevel", out consistencyLevel)) : new bool?();
              }
              num = nullable.GetValueOrDefault() ? 1 : 0;
              goto label_11;
            }
          }
          num = 0;
label_11:
          bool flag2 = num != 0;
          if (consistencyLevel == VssReadConsistencyLevel.Eventual && (flag1 || flag2 && requestContext.IsFeatureEnabled(FrameworkServerConstants.ReadReplicaOptInElevatedFeatureFlag)))
          {
            Type element = typeof (TComponent);
            SupportedSqlAccessIntentAttribute customAttribute = (object) element != null ? element.GetCustomAttribute<SupportedSqlAccessIntentAttribute>() : (SupportedSqlAccessIntentAttribute) null;
            SqlAccessIntent? supportedSqlAccessIntent = customAttribute?.SupportedSqlAccessIntent;
            string accessFeatureFlag = customAttribute?.SqlReadOnlyAccessFeatureFlag;
            if (supportedSqlAccessIntent.HasValue && (supportedSqlAccessIntent.Value & SqlAccessIntent.ReadOnly) == SqlAccessIntent.ReadOnly && (string.IsNullOrEmpty(accessFeatureFlag) || requestContext.IsFeatureEnabled(customAttribute.SqlReadOnlyAccessFeatureFlag)))
            {
              connectionType = new DatabaseConnectionType?(DatabaseConnectionType.IntentReadOnly);
              requestContext.TraceConditionally(90021, TraceLevel.Verbose, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, (Func<string>) (() => "Component '" + typeof (TComponent).FullName + "' will be created using read-only replica connection."));
            }
          }
        }
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        string str;
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && requestContext.Items.TryGetValue<string>(RequestContextItemsKeys.CurrentServicingOperation, out str))
        {
          vssRequestContext.Items[RequestContextItemsKeys.CurrentServicingOperation] = (object) str;
          DatabaseConnectionType? nullable = connectionType;
          DatabaseConnectionType databaseConnectionType = DatabaseConnectionType.IntentReadOnly;
          if (nullable.GetValueOrDefault() == databaseConnectionType & nullable.HasValue)
          {
            connectionType = new DatabaseConnectionType?(DatabaseConnectionType.Default);
            requestContext.Trace(90329, TraceLevel.Warning, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "ConnectionType is converted to ReadWrite in Panic Mode.");
          }
        }
        connectionType.GetValueOrDefault();
        if (!connectionType.HasValue)
          connectionType = new DatabaseConnectionType?(DatabaseConnectionType.Default);
        return vssRequestContext.GetService<TeamFoundationResourceManagementService.IDeploymentHostResourceManagementService>().CreateDatabaseComponent<TComponent>(vssRequestContext, DatabaseManagementConstants.InvalidDatabaseId, requestContext, dataspaceCategory, dataspaceIdentifier ?? Guid.Empty, connectionType.Value, logger);
      }
      finally
      {
        requestContext.TraceLeave(90020, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, nameof (CreateComponent));
      }
    }

    internal TComponent CreateDatabaseComponent<TComponent>(
      IVssRequestContext deploymentRequestContext,
      int databaseId,
      string dataspaceCategory,
      DatabaseConnectionType connectionType = DatabaseConnectionType.Default)
      where TComponent : class, ISqlResourceComponent, new()
    {
      this.ValidateRequestContext(deploymentRequestContext);
      deploymentRequestContext.CheckDeploymentRequestContext();
      if (databaseId == -2)
        throw new VirtualServiceHostException();
      return deploymentRequestContext.GetService<TeamFoundationResourceManagementService.IDeploymentHostResourceManagementService>().CreateDatabaseComponent<TComponent>(deploymentRequestContext, databaseId, (IVssRequestContext) null, dataspaceCategory, Guid.Empty, connectionType, (ITFLogger) null);
    }

    public static ServiceVersionEntry GetServiceVersionRaw(
      ISqlConnectionInfo connectionInfo,
      string serviceName,
      bool handleNoResourceManagementSchema = false)
    {
      return TeamFoundationResourceManagementService.ReadServiceVersionFromDatabase(connectionInfo, handleNoResourceManagementSchema, serviceName, out bool _);
    }

    public static void GetServiceVersionRaw<TComponent>(
      ISqlConnectionInfo connectionInfo,
      out ServiceVersionEntry serviceVersionEntry,
      out IComponentCreator componentCreator,
      bool handleNoResourceManagementSchema = false)
      where TComponent : class, ISqlResourceComponent, new()
    {
      TeamFoundationResourceManagementService.GetServiceVersionRaw<TComponent>(connectionInfo, out serviceVersionEntry, out componentCreator, handleNoResourceManagementSchema, true, out ComponentFactory _, out bool _);
    }

    public static bool TryGetServiceVersionRaw<TComponent>(
      ISqlConnectionInfo connectionInfo,
      out ServiceVersionEntry serviceVersionEntry,
      out IComponentCreator componentCreator,
      bool handleNoResourceManagementSchema = false)
      where TComponent : class, ISqlResourceComponent, new()
    {
      TeamFoundationResourceManagementService.GetServiceVersionRaw<TComponent>(connectionInfo, out serviceVersionEntry, out componentCreator, handleNoResourceManagementSchema, false, out ComponentFactory _, out bool _);
      return componentCreator != null;
    }

    public static TComponent CreateComponentRaw<TComponent>(
      ITeamFoundationDatabaseProperties dbProperties,
      bool handleNoResourceManagementSchema = false,
      DatabaseConnectionType connectionType = DatabaseConnectionType.Default,
      ITFLogger logger = null)
      where TComponent : class, ISqlResourceComponent, new()
    {
      ISqlConnectionInfo connectionInfo;
      switch (connectionType)
      {
        case DatabaseConnectionType.IntentReadOnly:
          connectionInfo = dbProperties.ReadOnlyConnectionInfo;
          break;
        case DatabaseConnectionType.Dbo:
          connectionInfo = dbProperties.DboConnectionInfo ?? dbProperties.SqlConnectionInfo;
          break;
        default:
          connectionInfo = dbProperties.SqlConnectionInfo;
          break;
      }
      return TeamFoundationResourceManagementService.CreateComponentRaw<TComponent>(connectionInfo, dbProperties.RequestTimeout, dbProperties.DeadlockPause, dbProperties.DeadlockRetries, handleNoResourceManagementSchema, logger);
    }

    public static TComponent CreateComponentRaw<TComponent>(
      ISqlConnectionInfo connectionInfo,
      int commandTimeout = 60,
      int deadlockPause = 200,
      int maxDeadlockRetries = 25,
      bool handleNoResourceManagementSchema = false,
      ITFLogger logger = null)
      where TComponent : class, ISqlResourceComponent, new()
    {
      return TeamFoundationResourceManagementService.CreateComponentRaw<TComponent>(connectionInfo, commandTimeout, deadlockPause, maxDeadlockRetries, handleNoResourceManagementSchema, false, true, logger);
    }

    public static bool TryCreateComponentRaw<TComponent>(
      ISqlConnectionInfo connectionInfo,
      int commandTimeout,
      int deadlockPause,
      int maxDeadlockRetries,
      out TComponent component,
      bool handleNoResourceManagementSchema = false,
      ITFLogger logger = null)
      where TComponent : class, ISqlResourceComponent, new()
    {
      component = TeamFoundationResourceManagementService.CreateComponentRaw<TComponent>(connectionInfo, commandTimeout, deadlockPause, maxDeadlockRetries, handleNoResourceManagementSchema, true, false, logger);
      return (object) component != null;
    }

    private static void GetServiceVersionRaw<TComponent>(
      ISqlConnectionInfo connectionInfo,
      out ServiceVersionEntry serviceVersionEntry,
      out IComponentCreator componentCreator,
      bool handleNoResourceManagementSchema,
      bool throwExceptions,
      out ComponentFactory factory,
      out bool resourceManagementSchemaExists)
      where TComponent : class, ISqlResourceComponent, new()
    {
      TeamFoundationTracingService.TraceEnterRaw(0, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, nameof (GetServiceVersionRaw), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        factory = ComponentFactoryCache.Instance.GetComponentFactory<TComponent>();
        TeamFoundationTracingService.TraceRaw(0, TraceLevel.Info, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "Found component factory");
        componentCreator = factory.GetComponentCreator(0, 0);
        if (componentCreator != null)
        {
          serviceVersionEntry = (ServiceVersionEntry) null;
          resourceManagementSchemaExists = false;
        }
        else
        {
          serviceVersionEntry = TeamFoundationResourceManagementService.ReadServiceVersionFromDatabase(connectionInfo, handleNoResourceManagementSchema, factory.ServiceName, out resourceManagementSchemaExists);
          if (serviceVersionEntry == null || serviceVersionEntry.Version == 0)
          {
            componentCreator = factory.TransitionCreator;
            if (componentCreator == null)
            {
              if (!throwExceptions)
                return;
              throw new ServiceNotRegisteredException(FrameworkResources.ServiceNotRegistered((object) factory.ServiceName));
            }
          }
          else
            componentCreator = factory.GetComponentCreator(serviceVersionEntry.Version, serviceVersionEntry.MinVersion);
          if (componentCreator != null)
            return;
          TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "Component creator not found.");
          if (throwExceptions)
            throw new ServiceVersionNotSupportedException(factory.ServiceName, serviceVersionEntry.Version, serviceVersionEntry.MinVersion);
        }
      }
      catch (DatabaseConfigurationException ex)
      {
        if (!(ex.InnerException is SqlException innerException) || innerException.Message.IndexOf("prc_GetServiceVersion", StringComparison.Ordinal) < 0)
          TeamFoundationTracingService.TraceExceptionRaw(0, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, (Exception) ex);
        throw;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(0, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, nameof (GetServiceVersionRaw));
      }
    }

    private static TComponent CreateComponentRaw<TComponent>(
      ISqlConnectionInfo connectionInfo,
      int commandTimeout,
      int deadlockPause,
      int maxDeadlockRetries,
      bool handleNoResourceManagementSchema,
      bool verifyServiceVersion,
      bool throwExceptions,
      ITFLogger logger)
      where TComponent : class, ISqlResourceComponent, new()
    {
      TeamFoundationTracingService.TraceEnterRaw(0, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, nameof (CreateComponentRaw), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      bool flag = false;
      try
      {
        IComponentCreator componentCreator;
        ComponentFactory factory;
        bool resourceManagementSchemaExists;
        TeamFoundationResourceManagementService.GetServiceVersionRaw<TComponent>(connectionInfo, out ServiceVersionEntry _, out componentCreator, handleNoResourceManagementSchema, throwExceptions, out factory, out resourceManagementSchemaExists);
        flag = true;
        TComponent componentRaw = default (TComponent);
        if (componentCreator != null)
        {
          for (int index = 0; index < 10; ++index)
          {
            componentRaw = (TComponent) componentCreator.Create(connectionInfo, commandTimeout, deadlockPause, maxDeadlockRetries, logger, (CircuitBreakerDatabaseProperties) null);
            int databaseVersion;
            int minDatabaseVersion;
            if (verifyServiceVersion & resourceManagementSchemaExists && !componentRaw.VerifyServiceVersion(factory.ServiceName, componentCreator.ServiceVersion, out databaseVersion, out minDatabaseVersion))
            {
              TeamFoundationTracingService.TraceRaw(90025, TraceLevel.Warning, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "Verification of service version failed. Attempt: {0}", (object) index);
              componentRaw.Dispose();
              componentRaw = default (TComponent);
              componentCreator = factory.GetComponentCreator(databaseVersion, minDatabaseVersion);
              if (componentCreator == null)
              {
                TeamFoundationTracingService.TraceRaw(90026, TraceLevel.Error, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "Component creator not found.");
                if (throwExceptions)
                  throw new ServiceVersionNotSupportedException(factory.ServiceName, databaseVersion, minDatabaseVersion);
                return default (TComponent);
              }
            }
            else
              break;
          }
          if ((object) componentRaw == null & throwExceptions)
            throw new ServiceVersionNotSupportedException("Failed to create a component after 10 attempts.");
        }
        return componentRaw;
      }
      catch (Exception ex)
      {
        if (flag)
          TeamFoundationTracingService.TraceExceptionRaw(0, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, ex);
        throw;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(0, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, nameof (CreateComponentRaw));
      }
    }

    public ServiceVersionInfo GetServiceVersion(
      IVssRequestContext requestContext,
      string serviceName,
      string dataspaceCategory)
    {
      this.ValidateRequestContext(requestContext);
      try
      {
        requestContext.TraceEnter(90100, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, nameof (GetServiceVersion));
        ArgumentUtility.CheckStringForNullOrEmpty(serviceName, nameof (serviceName));
        ArgumentUtility.CheckStringForNullOrEmpty(dataspaceCategory, nameof (dataspaceCategory));
        int databaseId = TeamFoundationResourceManagementService.GetDatabaseId(requestContext, dataspaceCategory);
        if (databaseId == -2)
          throw new VirtualServiceHostException();
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        TeamFoundationResourceManagementService.IDeploymentHostResourceManagementService service = vssRequestContext.GetService<TeamFoundationResourceManagementService.IDeploymentHostResourceManagementService>();
        service.EnsureInitialized(vssRequestContext, databaseId);
        return service.GetServiceVersionFromCache(vssRequestContext, databaseId, serviceName);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(90104, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(90109, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, nameof (GetServiceVersion));
      }
    }

    public void SetServiceVersion(
      IVssRequestContext requestContext,
      string serviceName,
      string dataspaceCategory,
      int version,
      int minVersion)
    {
      this.ValidateRequestContext(requestContext);
      try
      {
        requestContext.TraceEnter(90200, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, nameof (SetServiceVersion));
        ArgumentUtility.CheckStringForNullOrEmpty(serviceName, nameof (serviceName));
        ArgumentUtility.CheckStringForNullOrEmpty(dataspaceCategory, nameof (dataspaceCategory));
        int databaseId = TeamFoundationResourceManagementService.GetDatabaseId(requestContext, dataspaceCategory);
        if (databaseId == -2)
          throw new VirtualServiceHostException();
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        vssRequestContext.GetService<TeamFoundationResourceManagementService.IDeploymentHostResourceManagementService>().SetServiceVersion(vssRequestContext, databaseId, serviceName, version, minVersion);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(90104, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(90109, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "GetServiceVersion");
      }
    }

    public bool GetVerifyServiceVersion(IVssRequestContext requestContext, string dataspaceCategory)
    {
      int databaseId = TeamFoundationResourceManagementService.GetDatabaseId(requestContext, dataspaceCategory);
      if (databaseId == -2)
        throw new VirtualServiceHostException();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<TeamFoundationResourceManagementService.IDeploymentHostResourceManagementService>().GetVerifyServiceVersion(vssRequestContext, databaseId, requestContext);
    }

    private static int GetDatabaseId(IVssRequestContext requestContext, string dataspaceCategory) => TeamFoundationResourceManagementService.GetDatabaseId(requestContext, dataspaceCategory, Guid.Empty);

    private static int GetDatabaseId(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier)
    {
      return !dataspaceCategory.Equals("Default", StringComparison.OrdinalIgnoreCase) || !(dataspaceIdentifier == Guid.Empty) || !(requestContext is VssRequestContext) ? requestContext.GetService<IDataspaceService>().QueryDataspace(requestContext, dataspaceCategory, dataspaceIdentifier, true).DatabaseId : requestContext.ServiceHost.ServiceHostInternal().DatabaseId;
    }

    private static ServiceVersionEntry ReadServiceVersionFromDatabase(
      ISqlConnectionInfo connectionInfo,
      bool handleNoResourceManagementSchema,
      string serviceName,
      out bool resourceManagementSchemaExists)
    {
      TeamFoundationTracingService.TraceEnterRaw(0, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, nameof (ReadServiceVersionFromDatabase), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        TeamFoundationTracingService.TraceRaw(0, TraceLevel.Info, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "Service Name: {0}", (object) serviceName);
        ServiceVersionEntry serviceVersion;
        using (ResourceManagementComponent managementComponent = TeamFoundationResourceManagementService.CreateResourceManagementComponent(connectionInfo))
        {
          if (handleNoResourceManagementSchema)
          {
            serviceVersion = managementComponent.TryGetServiceVersion(serviceName, out resourceManagementSchemaExists);
          }
          else
          {
            serviceVersion = managementComponent.GetServiceVersion(serviceName);
            resourceManagementSchemaExists = true;
          }
        }
        if (!resourceManagementSchemaExists)
          TeamFoundationTracingService.TraceRaw(0, TraceLevel.Info, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "ResourceManagement schema doesn't exist in database.");
        else if (serviceVersion == null)
          TeamFoundationTracingService.TraceRaw(0, TraceLevel.Info, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "Service not registered: {0}", (object) serviceName);
        else
          TeamFoundationTracingService.TraceRaw(0, TraceLevel.Info, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "Service Name: {0}, Version: {1}, MinVersion: {2}", (object) serviceVersion.ServiceName, (object) serviceVersion.Version, (object) serviceVersion.MinVersion);
        return serviceVersion;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(0, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, nameof (ReadServiceVersionFromDatabase));
      }
    }

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (this.m_serviceHost.InstanceId != requestContext.ServiceHost.InstanceId)
        throw new InvalidRequestContextHostException(FrameworkResources.ComponentServiceRequestContextHostMessage((object) this.m_serviceHost.InstanceId, (object) requestContext.ServiceHost.InstanceId));
    }

    internal static void SetTargetServiceLevel(
      ISqlConnectionInfo configDbConnectionInfo,
      string targetServiceLevel)
    {
      TeamFoundationResourceManagementService.SetTargetServiceLevel(configDbConnectionInfo, targetServiceLevel, out bool _);
    }

    internal static void SetTargetServiceLevel(
      ISqlConnectionInfo configDbConnectionInfo,
      string targetServiceLevel,
      out bool waitForNotification)
    {
      TeamFoundationResourceManagementService.SetTargetServiceLevel(configDbConnectionInfo, targetServiceLevel, out bool _, out waitForNotification);
    }

    internal static void SetTargetServiceLevel(
      ISqlConnectionInfo configDbConnectionInfo,
      string targetServiceLevel,
      out bool isSqlAzure,
      out bool waitForNotification)
    {
      waitForNotification = true;
      isSqlAzure = false;
      try
      {
        using (ResourceManagementComponent2 componentRaw = (ResourceManagementComponent2) configDbConnectionInfo.CreateComponentRaw<ResourceManagementComponent>())
        {
          ResourceManagementSetting managementSetting = componentRaw.GetResourceManagementSetting(FrameworkServerConstants.ResourceManagementTargetServiceLevelSetting);
          componentRaw.SetResourceManagementSetting(FrameworkServerConstants.ResourceManagementTargetServiceLevelSetting, targetServiceLevel.ToString());
          isSqlAzure = componentRaw.IsSqlAzure;
          if (managementSetting == null || string.IsNullOrEmpty(managementSetting.Value) || !string.Equals(managementSetting.Value, targetServiceLevel, StringComparison.OrdinalIgnoreCase))
            return;
          waitForNotification = false;
        }
      }
      catch (DatabaseConfigurationException ex)
      {
      }
    }

    internal static ResourceManagementComponent CreateResourceManagementComponent(
      ISqlConnectionInfo connectionInfo)
    {
      ResourceManagementComponent2 managementComponent = new ResourceManagementComponent2();
      ((IDBResourceComponent) managementComponent).Initialize(connectionInfo, 60, 200, 25, 0, (ITFLogger) null, (CircuitBreakerDatabaseProperties) null);
      return (ResourceManagementComponent) managementComponent;
    }

    [Browsable(false)]
    [DefaultServiceImplementation(typeof (TeamFoundationResourceManagementService.DeploymentHostResourceManagementService))]
    internal interface IDeploymentHostResourceManagementService : IVssFrameworkService
    {
      TComponent CreateDatabaseComponent<TComponent>(
        IVssRequestContext deploymentRequestContext,
        int databaseId,
        IVssRequestContext requestContext,
        string dataspaceCategory,
        Guid dataspaceIdentifier,
        DatabaseConnectionType connectionType,
        ITFLogger logger)
        where TComponent : class, ISqlResourceComponent, new();

      void EnsureInitialized(IVssRequestContext deploymentRequestContext, int databaseId);

      ServiceVersionInfo GetServiceVersionFromCache(
        IVssRequestContext deploymentRequestContext,
        int databaseId,
        string serviceName);

      void InvalidateCache(IVssRequestContext deploymentRequestContext, int databaseId);

      ServiceVersionEntry ReadServiceVersionFromDatabase(
        IVssRequestContext deploymentContext,
        int databaseId,
        string serviceName);

      void UpdateServiceVersionCache(
        IVssRequestContext deploymentContext,
        int databaseId,
        ServiceVersionEntry serviceVersionEntry);

      void SetServiceVersion(
        IVssRequestContext deploymentRequestContext,
        int databaseId,
        string serviceName,
        int version,
        int minVersion);

      bool GetVerifyServiceVersion(
        IVssRequestContext deploymentRequestContext,
        int databaseId,
        IVssRequestContext requestContext = null);
    }

    [Browsable(false)]
    internal sealed class DeploymentHostResourceManagementService : 
      VssBaseService,
      TeamFoundationResourceManagementService.IDeploymentHostResourceManagementService,
      IVssFrameworkService
    {
      private static XmlSerializer s_serviceVersionSerializer;
      private static XmlSerializer s_settingSerializer;
      private bool m_initialized;
      private string m_targetServiceLevel;
      private readonly ConcurrentDictionary<int, TeamFoundationResourceManagementService.ResourceManagementCacheEntry> m_databaseToServiceVersionMap = new ConcurrentDictionary<int, TeamFoundationResourceManagementService.ResourceManagementCacheEntry>();

      private DeploymentHostResourceManagementService()
      {
      }

      void IVssFrameworkService.ServiceStart(IVssRequestContext deploymentRequestContext) => deploymentRequestContext.CheckDeploymentRequestContext();

      void IVssFrameworkService.ServiceEnd(IVssRequestContext deploymentRequestContext)
      {
      }

      public void EnsureInitialized(IVssRequestContext deploymentRequestContext, int databaseId)
      {
        deploymentRequestContext.CheckDeploymentRequestContext();
        if (databaseId == -2)
          throw new VirtualServiceHostException();
        try
        {
          deploymentRequestContext.TraceEnter(90070, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, nameof (EnsureInitialized));
          TeamFoundationResourceManagementService.ResourceManagementCacheEntry managementCacheEntry;
          if (!this.m_databaseToServiceVersionMap.TryGetValue(databaseId, out managementCacheEntry) || !managementCacheEntry.CacheFresh)
          {
            ILockName databaseLockName = this.CreateDatabaseLockName(deploymentRequestContext, databaseId);
            using (deploymentRequestContext.Lock(databaseLockName))
            {
              if (this.m_databaseToServiceVersionMap.TryGetValue(databaseId, out managementCacheEntry))
              {
                if (managementCacheEntry.CacheFresh)
                  goto label_14;
              }
              using (deploymentRequestContext.AcquireExemptionLock())
              {
                List<ServiceVersionEntry> serviceVersionEntries = this.QueryServiceVersions(deploymentRequestContext, databaseId);
                int count = serviceVersionEntries.Count;
                deploymentRequestContext.Trace(90072, TraceLevel.Info, TeamFoundationResourceManagementService.s_layer, TeamFoundationResourceManagementService.s_layer, "QueryServiceVersions returned {0} entries", (object) count);
                this.m_databaseToServiceVersionMap[databaseId] = new TeamFoundationResourceManagementService.ResourceManagementCacheEntry(databaseId, serviceVersionEntries)
                {
                  ParanoidMode = managementCacheEntry == null ? this.ComputeVerifyServiceVersion(deploymentRequestContext, databaseId) : managementCacheEntry.ParanoidMode
                };
              }
            }
          }
label_14:
          if (this.m_initialized)
            return;
          deploymentRequestContext.GetService<TeamFoundationSqlNotificationService>().RegisterNotification(deploymentRequestContext, "Default", SqlNotificationEventClasses.ResourceManagementSettingsChanged, new SqlNotificationCallback(this.OnResourceManagementSettingsChanged), false);
          using (deploymentRequestContext.Lock(this.CreateLockName(deploymentRequestContext, "Init")))
          {
            if (this.m_initialized)
              return;
            this.UpdateTargetServiceLevel(deploymentRequestContext);
            this.m_initialized = true;
          }
        }
        catch (Exception ex)
        {
          deploymentRequestContext.TraceException(90074, TraceLevel.Error, TeamFoundationResourceManagementService.s_layer, TeamFoundationResourceManagementService.s_layer, ex);
          throw;
        }
        finally
        {
          deploymentRequestContext.TraceLeave(90070, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, nameof (EnsureInitialized));
        }
      }

      public TComponent CreateDatabaseComponent<TComponent>(
        IVssRequestContext deploymentRequestContext,
        int databaseId,
        IVssRequestContext requestContext,
        string dataspaceCategory,
        Guid dataspaceIdentifier,
        DatabaseConnectionType connectionType,
        ITFLogger logger)
        where TComponent : class, ISqlResourceComponent, new()
      {
        deploymentRequestContext.CheckDeploymentRequestContext();
        if (databaseId == -2)
          throw new VirtualServiceHostException();
        TComponent component = default (TComponent);
        try
        {
          deploymentRequestContext.TraceEnter(90300, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, nameof (CreateDatabaseComponent));
          ComponentFactory componentFactory = ComponentFactoryCache.Instance.GetComponentFactory<TComponent>();
          deploymentRequestContext.Trace(90321, TraceLevel.Info, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "Found component factory");
label_4:
          string dataspaceCategory1 = !string.IsNullOrEmpty(dataspaceCategory) ? dataspaceCategory : (componentFactory.DataspaceCategory == null ? "Default" : componentFactory.DataspaceCategory);
          ServiceVersionInfo serviceVersionInfo = new ServiceVersionInfo();
          int num = DatabaseManagementConstants.InvalidDatabaseId;
          if (requestContext != null)
          {
            num = requestContext.ServiceHost.ServiceHostInternal().DatabaseId;
            if (componentFactory.MinDataspaceCategoryVersion > 0)
            {
              databaseId = num;
              this.EnsureInitialized(deploymentRequestContext, databaseId);
              serviceVersionInfo = this.GetServiceVersionFromCache(deploymentRequestContext, databaseId, componentFactory.ServiceName);
              if (serviceVersionInfo.Version >= componentFactory.MinDataspaceCategoryVersion)
                databaseId = DatabaseManagementConstants.InvalidDatabaseId;
              else
                dataspaceCategory1 = "Default";
            }
            if (databaseId == DatabaseManagementConstants.InvalidDatabaseId)
              databaseId = TeamFoundationResourceManagementService.GetDatabaseId(requestContext, dataspaceCategory1, dataspaceIdentifier);
          }
          bool flag = this.GetVerifyServiceVersion(deploymentRequestContext, databaseId, requestContext);
          if (databaseId != num || componentFactory.MinDataspaceCategoryVersion == 0)
          {
            this.EnsureInitialized(deploymentRequestContext, databaseId);
            deploymentRequestContext.Trace(90322, TraceLevel.Info, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "Getting service version from cache.");
            serviceVersionInfo = this.GetServiceVersionFromCache(deploymentRequestContext, databaseId, componentFactory.ServiceName);
          }
          if (deploymentRequestContext.IsTracing(90323, TraceLevel.Info, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer))
            deploymentRequestContext.Trace(90323, TraceLevel.Info, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "Service Name: {0}, Version: {1}, MinVersion: {2}, Database Id: {3}, Dataspace Category: {4}", (object) componentFactory.ServiceName, (object) serviceVersionInfo.Version, (object) serviceVersionInfo.MinVersion, (object) databaseId, (object) dataspaceCategory);
          IComponentCreator componentCreator = componentFactory.GetComponentCreator(serviceVersionInfo.Version, serviceVersionInfo.MinVersion);
          if (componentCreator == null && flag)
          {
            ServiceVersionEntry serviceVersionEntry = this.ReadServiceVersionFromDatabase(deploymentRequestContext, databaseId, componentFactory.ServiceName);
            if (serviceVersionEntry != null)
            {
              this.UpdateServiceVersionCache(deploymentRequestContext, databaseId, serviceVersionEntry);
              serviceVersionInfo = new ServiceVersionInfo(serviceVersionEntry.Version, serviceVersionEntry.MinVersion);
            }
          }
          if (serviceVersionInfo.Version == 0)
          {
            flag = false;
            if (componentCreator == null)
              componentCreator = componentFactory.TransitionCreator;
            if (componentCreator == null)
            {
              deploymentRequestContext.Trace(90328, TraceLevel.Error, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "Service {0} is not registered. Database Id: {1}", (object) componentFactory.ServiceName, (object) databaseId);
              throw new ServiceNotRegisteredException(FrameworkResources.ServiceNotRegistered((object) componentFactory.ServiceName));
            }
            this.UpdateServiceVersionCache(deploymentRequestContext, databaseId, new ServiceVersionEntry(componentFactory.ServiceName, componentCreator.ServiceVersion, componentCreator.ServiceVersion));
          }
          else if (componentCreator == null)
            componentCreator = componentFactory.GetComponentCreator(serviceVersionInfo.Version, serviceVersionInfo.MinVersion);
          if (componentCreator == null)
          {
            deploymentRequestContext.Trace(903024, TraceLevel.Error, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "Component creator not found.");
            throw new ServiceVersionNotSupportedException(componentFactory.ServiceName, serviceVersionInfo.Version, serviceVersionInfo.MinVersion);
          }
          ITeamFoundationDatabaseProperties databaseProperties = (ITeamFoundationDatabaseProperties) null;
          for (int index = 0; index < 3; ++index)
          {
            if (requestContext != null)
            {
              component = (TComponent) componentCreator.Create(requestContext, dataspaceCategory1, dataspaceIdentifier, connectionType, logger);
            }
            else
            {
              if (databaseProperties == null)
                databaseProperties = deploymentRequestContext.GetService<TeamFoundationDatabaseManagementService>().GetDatabase(deploymentRequestContext, databaseId, true);
              component = (TComponent) componentCreator.Create(databaseProperties.SqlConnectionInfo, databaseProperties.RequestTimeout, databaseProperties.DeadlockPause, databaseProperties.DeadlockRetries, logger, new CircuitBreakerDatabaseProperties(databaseProperties));
            }
            int databaseVersion;
            int minDatabaseVersion;
            if (flag && !component.VerifyServiceVersion(componentFactory.ServiceName, componentCreator.ServiceVersion, out databaseVersion, out minDatabaseVersion))
            {
              using (deploymentRequestContext.AcquireExemptionLock())
                this.UpdateServiceVersionCache(deploymentRequestContext, databaseId, new ServiceVersionEntry(componentFactory.ServiceName, databaseVersion, minDatabaseVersion));
              deploymentRequestContext.Trace(90325, TraceLevel.Warning, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "Verification of service version failed. Attempt: {0}", (object) index);
              component.Dispose();
              component = default (TComponent);
              if (requestContext != null && serviceVersionInfo.Version < componentFactory.MinDataspaceCategoryVersion && databaseVersion >= componentFactory.MinDataspaceCategoryVersion)
              {
                databaseId = DatabaseManagementConstants.InvalidDatabaseId;
                goto label_4;
              }
              else
              {
                componentCreator = componentFactory.GetComponentCreator(databaseVersion, minDatabaseVersion);
                if (componentCreator == null)
                {
                  deploymentRequestContext.Trace(90326, TraceLevel.Error, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "Component creator not found. Service name: {0}, Version: {1}, Min Version: {2}, Component Type: {3}, Database Id: {4}", (object) componentFactory.ServiceName, (object) databaseVersion, (object) minDatabaseVersion, (object) typeof (TComponent), (object) databaseId);
                  throw new ServiceVersionNotSupportedException(componentFactory.ServiceName, databaseVersion, minDatabaseVersion);
                }
              }
            }
            else
              break;
          }
          return (object) component != null ? component : throw new ServiceVersionNotSupportedException("Failed to create a component after 3 attempts.");
        }
        catch (Exception ex)
        {
          deploymentRequestContext.TraceException(90327, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, ex);
          if ((object) component != null)
            component.Dispose();
          throw;
        }
        finally
        {
          deploymentRequestContext.TraceLeave(90300, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, nameof (CreateDatabaseComponent));
        }
      }

      public bool GetVerifyServiceVersion(
        IVssRequestContext deploymentRequestContext,
        int databaseId,
        IVssRequestContext requestContext = null)
      {
        deploymentRequestContext.CheckDeploymentRequestContext();
        if (databaseId == -2)
          throw new VirtualServiceHostException();
        bool verifyServiceVersion;
        if (deploymentRequestContext.ExecutionEnvironment.IsOnPremisesDeployment && requestContext != null)
        {
          TeamFoundationServiceHostStatus? status = requestContext.ServiceHost?.Status;
          TeamFoundationServiceHostStatus serviceHostStatus = TeamFoundationServiceHostStatus.Stopped;
          if (status.GetValueOrDefault() == serviceHostStatus & status.HasValue)
          {
            verifyServiceVersion = true;
            goto label_6;
          }
        }
        verifyServiceVersion = this.ComputeVerifyServiceVersion(deploymentRequestContext, databaseId);
label_6:
        TeamFoundationResourceManagementService.ResourceManagementCacheEntry managementCacheEntry;
        if (this.m_databaseToServiceVersionMap.TryGetValue(databaseId, out managementCacheEntry))
        {
          if (!verifyServiceVersion && managementCacheEntry.ParanoidMode)
            managementCacheEntry.CacheFresh = false;
          if (managementCacheEntry.ParanoidMode != verifyServiceVersion)
            managementCacheEntry.ParanoidMode = verifyServiceVersion;
        }
        return verifyServiceVersion;
      }

      public ServiceVersionInfo GetServiceVersionFromCache(
        IVssRequestContext deploymentRequestContext,
        int databaseId,
        string serviceName)
      {
        deploymentRequestContext.CheckDeploymentRequestContext();
        if (databaseId == -2)
          throw new VirtualServiceHostException();
        ServiceVersionInfo versionFromCache = new ServiceVersionInfo();
        TeamFoundationResourceManagementService.ResourceManagementCacheEntry managementCacheEntry;
        if (this.m_databaseToServiceVersionMap.TryGetValue(databaseId, out managementCacheEntry))
          managementCacheEntry.VersionCache.TryGetValue(serviceName, out versionFromCache);
        return versionFromCache;
      }

      public void InvalidateCache(IVssRequestContext deploymentRequestContext, int databaseId)
      {
        deploymentRequestContext.CheckDeploymentRequestContext();
        if (databaseId == -2)
          throw new VirtualServiceHostException();
        TeamFoundationResourceManagementService.ResourceManagementCacheEntry managementCacheEntry;
        if (!this.m_databaseToServiceVersionMap.TryGetValue(databaseId, out managementCacheEntry))
          return;
        managementCacheEntry.CacheFresh = false;
      }

      public ServiceVersionEntry ReadServiceVersionFromDatabase(
        IVssRequestContext deploymentContext,
        int databaseId,
        string serviceName)
      {
        deploymentContext.TraceEnter(90060, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, nameof (ReadServiceVersionFromDatabase));
        deploymentContext.Trace(90061, TraceLevel.Info, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "Service Name: {0}", (object) serviceName);
        if (databaseId == -2)
          throw new VirtualServiceHostException();
        ServiceVersionEntry serviceVersion;
        using (ResourceManagementComponent managementComponent = this.CreateResourceManagementComponent(deploymentContext, databaseId))
          serviceVersion = managementComponent.GetServiceVersion(serviceName);
        if (serviceVersion == null)
          deploymentContext.Trace(90062, TraceLevel.Warning, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "Service not registered");
        else
          deploymentContext.Trace(90063, TraceLevel.Info, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "Service Name: {0}, Version: {1}, MinVersion: {2}", (object) serviceVersion.ServiceName, (object) serviceVersion.Version, (object) serviceVersion.MinVersion);
        deploymentContext.TraceLeave(90069, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, nameof (ReadServiceVersionFromDatabase));
        return serviceVersion;
      }

      public void UpdateServiceVersionCache(
        IVssRequestContext deploymentContext,
        int databaseId,
        ServiceVersionEntry serviceVersionEntry)
      {
        deploymentContext.CheckDeploymentRequestContext();
        ArgumentUtility.CheckForNull<ServiceVersionEntry>(serviceVersionEntry, nameof (serviceVersionEntry));
        ILockName lockName = databaseId != -2 ? this.CreateDatabaseLockName(deploymentContext, databaseId) : throw new VirtualServiceHostException();
        using (deploymentContext.Lock(lockName))
        {
          TeamFoundationResourceManagementService.ResourceManagementCacheEntry managementCacheEntry1;
          if (this.m_databaseToServiceVersionMap.TryGetValue(databaseId, out managementCacheEntry1))
          {
            Dictionary<string, ServiceVersionInfo> dictionary = new Dictionary<string, ServiceVersionInfo>((IDictionary<string, ServiceVersionInfo>) managementCacheEntry1.VersionCache, (IEqualityComparer<string>) StringComparer.Ordinal);
            if (serviceVersionEntry.Version != -1)
              dictionary[serviceVersionEntry.ServiceName] = new ServiceVersionInfo(serviceVersionEntry.Version, serviceVersionEntry.MinVersion);
            else
              dictionary.Remove(serviceVersionEntry.ServiceName);
            managementCacheEntry1.VersionCache = dictionary;
          }
          else
          {
            TeamFoundationResourceManagementService.ResourceManagementCacheEntry managementCacheEntry2 = new TeamFoundationResourceManagementService.ResourceManagementCacheEntry(databaseId, new List<ServiceVersionEntry>()
            {
              serviceVersionEntry
            });
            using (deploymentContext.AcquireExemptionLock())
              managementCacheEntry2.ParanoidMode = this.ComputeVerifyServiceVersion(deploymentContext, databaseId);
            this.m_databaseToServiceVersionMap[databaseId] = managementCacheEntry2;
          }
        }
      }

      private bool ComputeVerifyServiceVersion(
        IVssRequestContext deploymentRequestContext,
        int databaseId)
      {
        deploymentRequestContext.CheckDeploymentRequestContext();
        if (databaseId == -2)
          throw new VirtualServiceHostException();
        int num = deploymentRequestContext is VssRequestContext ? deploymentRequestContext.ServiceHost.ServiceHostInternal().DatabaseId : -1;
        bool verifyServiceVersion;
        if (this.m_targetServiceLevel == null)
        {
          string a;
          verifyServiceVersion = deploymentRequestContext.ExecutionEnvironment.IsOnPremisesDeployment && num >= 0 && databaseId != num && (deploymentRequestContext.Items.TryGetValue<string>(RequestContextItemsKeys.CurrentServicingOperation, out a) && string.Equals(a, ServicingOperationConstants.Snapshot) || !string.Equals(deploymentRequestContext.GetService<TeamFoundationDatabaseManagementService>().GetDatabase(deploymentRequestContext, databaseId, true).ServiceLevel, deploymentRequestContext.ServiceHost.DeploymentServiceHost.DatabaseProperties.ServiceLevel, StringComparison.OrdinalIgnoreCase));
        }
        else
        {
          ITeamFoundationDatabaseProperties databaseProperties = num < 0 || databaseId != num ? deploymentRequestContext.GetService<TeamFoundationDatabaseManagementService>().GetDatabase(deploymentRequestContext, databaseId, true) : deploymentRequestContext.ServiceHost.DeploymentServiceHost.DatabaseProperties;
          verifyServiceVersion = string.IsNullOrEmpty(databaseProperties.ServiceLevel) || !string.Equals(databaseProperties.ServiceLevel, this.m_targetServiceLevel, StringComparison.OrdinalIgnoreCase);
        }
        return verifyServiceVersion;
      }

      private ILockName CreateDatabaseLockName(IVssRequestContext deploymentContext, int databaseId)
      {
        deploymentContext.CheckDeploymentRequestContext();
        return databaseId != -2 ? this.CreateLockName(deploymentContext, "Db" + databaseId.ToString()) : throw new VirtualServiceHostException();
      }

      private ResourceManagementComponent CreateResourceManagementComponent(
        IVssRequestContext deploymentContext,
        int databaseId)
      {
        deploymentContext.CheckDeploymentRequestContext();
        ITeamFoundationDatabaseProperties databaseProperties = databaseId != -2 ? deploymentContext.GetService<TeamFoundationDatabaseManagementService>().GetDatabase(deploymentContext, databaseId, true) : throw new VirtualServiceHostException();
        ResourceManagementComponent2 managementComponent = new ResourceManagementComponent2();
        ((IDBResourceComponent) managementComponent).Initialize(databaseProperties.SqlConnectionInfo, 60, 200, 25, 2, (ITFLogger) null, new CircuitBreakerDatabaseProperties(databaseProperties));
        return (ResourceManagementComponent) managementComponent;
      }

      private void UpdateTargetServiceLevel(IVssRequestContext deploymentContext)
      {
        deploymentContext.CheckDeploymentRequestContext();
        TeamFoundationResourceManagementService.ResourceManagementCacheEntry managementCacheEntry;
        ServiceVersionInfo serviceVersionInfo;
        if (this.m_databaseToServiceVersionMap.TryGetValue(deploymentContext.ServiceHost.ServiceHostInternal().DatabaseId, out managementCacheEntry) && managementCacheEntry.VersionCache.TryGetValue("ResourceManagement", out serviceVersionInfo) && serviceVersionInfo.Version >= 2)
        {
          using (ResourceManagementComponent2 managementComponent = (ResourceManagementComponent2) TeamFoundationResourceManagementService.CreateResourceManagementComponent(deploymentContext.FrameworkConnectionInfo))
          {
            ResourceManagementSetting managementSetting = managementComponent.GetResourceManagementSetting(FrameworkServerConstants.ResourceManagementTargetServiceLevelSetting);
            if (managementSetting != null && !string.IsNullOrEmpty(managementSetting.Value))
              this.m_targetServiceLevel = managementSetting.Value;
            else
              this.m_targetServiceLevel = (string) null;
          }
        }
        else
          this.m_targetServiceLevel = "Dev20.M999";
      }

      private void OnResourceManagementSettingsChanged(
        IVssRequestContext deploymentContext,
        Guid eventClass,
        string eventData)
      {
        deploymentContext.TraceEnter(90090, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, nameof (OnResourceManagementSettingsChanged));
        deploymentContext.Trace(90091, TraceLevel.Info, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, eventData);
        using (StringReader stringReader = new StringReader(eventData))
        {
          try
          {
            ResourceManagementSetting managementSetting = (ResourceManagementSetting) TeamFoundationResourceManagementService.DeploymentHostResourceManagementService.SettingSerializer.Deserialize((TextReader) stringReader);
            deploymentContext.Trace(90093, TraceLevel.Info, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "Name: {0}, Value: {1}", (object) managementSetting.Name, (object) managementSetting.Value);
            if (FrameworkServerConstants.ResourceManagementTargetServiceLevelSetting.Equals(managementSetting.Name, StringComparison.OrdinalIgnoreCase))
            {
              this.m_targetServiceLevel = !string.IsNullOrEmpty(managementSetting.Value) ? managementSetting.Value : (string) null;
              foreach (TeamFoundationResourceManagementService.ResourceManagementCacheEntry managementCacheEntry in (IEnumerable<TeamFoundationResourceManagementService.ResourceManagementCacheEntry>) this.m_databaseToServiceVersionMap.Values)
                managementCacheEntry.ParanoidMode = true;
            }
          }
          catch (Exception ex)
          {
            deploymentContext.TraceException(90092, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, ex);
          }
        }
        deploymentContext.TraceLeave(90092, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, nameof (OnResourceManagementSettingsChanged));
      }

      private List<ServiceVersionEntry> QueryServiceVersions(
        IVssRequestContext deploymentContext,
        int databaseId)
      {
        deploymentContext.CheckDeploymentRequestContext();
        if (databaseId == -2)
          throw new VirtualServiceHostException();
        deploymentContext.TraceEnter(90090, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, nameof (QueryServiceVersions));
        int databaseId1 = deploymentContext.ServiceHost.ServiceHostInternal().DatabaseId;
        List<ServiceVersionEntry> items;
        using (ResourceManagementComponent managementComponent = TeamFoundationResourceManagementService.CreateResourceManagementComponent(databaseId == databaseId1 || databaseId1 == 0 ? TeamFoundationDatabaseManagementService.GetSqlConnectionInfoRaw(deploymentContext.RequestContextInternal().DeploymentFrameworkConnectionInfo, databaseId) : deploymentContext.GetService<TeamFoundationDatabaseManagementService>().GetSqlConnectionInfo(deploymentContext, databaseId)))
          items = managementComponent.QueryServiceVersion().GetCurrent<ServiceVersionEntry>().Items;
        deploymentContext.Trace(90093, TraceLevel.Info, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, "Retrieved: {0} services.", (object) items.Count);
        deploymentContext.TraceLeave(90099, TeamFoundationResourceManagementService.s_area, TeamFoundationResourceManagementService.s_layer, nameof (QueryServiceVersions));
        return items;
      }

      public void SetServiceVersion(
        IVssRequestContext deploymentRequestContext,
        int databaseId,
        string serviceName,
        int version,
        int minVersion)
      {
        deploymentRequestContext.CheckDeploymentRequestContext();
        if (databaseId == -2)
          throw new VirtualServiceHostException();
        this.EnsureInitialized(deploymentRequestContext, databaseId);
        TeamFoundationResourceManagementService.ResourceManagementCacheEntry toServiceVersion = this.m_databaseToServiceVersionMap[databaseId];
        using (ResourceManagementComponent managementComponent = this.CreateResourceManagementComponent(deploymentRequestContext, databaseId))
          managementComponent.SetServiceVersion(serviceName, version, minVersion);
        toServiceVersion.CacheFresh = false;
      }

      private static XmlSerializer ServiceVersionSerializer => TeamFoundationResourceManagementService.DeploymentHostResourceManagementService.s_serviceVersionSerializer ?? (TeamFoundationResourceManagementService.DeploymentHostResourceManagementService.s_serviceVersionSerializer = new XmlSerializer(typeof (ServiceVersionEntry)));

      private static XmlSerializer SettingSerializer => TeamFoundationResourceManagementService.DeploymentHostResourceManagementService.s_settingSerializer ?? (TeamFoundationResourceManagementService.DeploymentHostResourceManagementService.s_settingSerializer = new XmlSerializer(typeof (ResourceManagementSetting)));
    }

    private sealed class ResourceManagementCacheEntry
    {
      public ResourceManagementCacheEntry(
        int databaseId,
        List<ServiceVersionEntry> serviceVersionEntries)
      {
        this.DatabaseId = databaseId != -2 ? databaseId : throw new VirtualServiceHostException();
        Dictionary<string, ServiceVersionInfo> dictionary = new Dictionary<string, ServiceVersionInfo>((IEqualityComparer<string>) StringComparer.Ordinal);
        if (serviceVersionEntries != null)
        {
          foreach (ServiceVersionEntry serviceVersionEntry in serviceVersionEntries)
          {
            if (serviceVersionEntry.Version != -1)
              dictionary[serviceVersionEntry.ServiceName] = new ServiceVersionInfo(serviceVersionEntry.Version, serviceVersionEntry.MinVersion);
          }
        }
        this.VersionCache = dictionary;
        this.CacheFresh = true;
      }

      public int DatabaseId { get; private set; }

      public Dictionary<string, ServiceVersionInfo> VersionCache { get; set; }

      public bool ParanoidMode { get; set; }

      public bool CacheFresh { get; set; }
    }
  }
}
