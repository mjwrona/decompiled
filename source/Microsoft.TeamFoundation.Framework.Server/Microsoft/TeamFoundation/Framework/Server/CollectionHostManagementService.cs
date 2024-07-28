// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CollectionHostManagementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.HostManagement;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class CollectionHostManagementService : IVssFrameworkService, IHostCreator
  {
    private static readonly string s_defaultResourceDirectory = "_tfs_resources";
    private const string s_area = "Collection";
    private const string s_layer = "CollectionManagementService";

    public void ServiceStart(IVssRequestContext requestContext) => requestContext.CheckDeploymentRequestContext();

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public ServicingJobDetail QueueCreateCollectionHost(
      IVssRequestContext requestContext,
      Guid parentHostId,
      TeamProjectCollectionProperties collectionProperties,
      IDictionary<string, string> servicingTokens,
      ISqlConnectionInfo dataTierConnectionInfo,
      JobPriorityClass priorityClass,
      JobPriorityLevel priorityLevel,
      Microsoft.VisualStudio.Services.Identity.Identity requestingIdentity)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForEmptyGuid(parentHostId, nameof (parentHostId));
      ServicingJobData servicingJobData = this.GetServicingJobData(requestContext, parentHostId, collectionProperties, servicingTokens, dataTierConnectionInfo, requestingIdentity, false);
      using (IVssRequestContext parentContext = this.CreateParentContext(requestContext, parentHostId))
        return parentContext.GetService<TeamFoundationServicingService>().QueueServicingJob(parentContext, servicingJobData, priorityClass, priorityLevel, new Guid?());
    }

    public virtual ServicingJobDetail CreateHost(
      IVssRequestContext requestContext,
      ServiceHostProperties hostProperties,
      IDictionary<string, string> servicingTokens = null,
      Microsoft.VisualStudio.Services.Identity.Identity requestingIdentity = null)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForNull<ServiceHostProperties>(hostProperties, nameof (hostProperties));
      ArgumentUtility.CheckForEmptyGuid(hostProperties.ParentHostId, "hostProperties.ParentHostId");
      if (hostProperties.HostType != ServiceHostType.Collection)
        throw new ArgumentException("The supplied ServiceHostProperties must be of type Collection in order to call this method.");
      if (hostProperties.HostId == Guid.Empty)
        hostProperties.HostId = Guid.NewGuid();
      TeamProjectCollectionProperties collectionProperties = new TeamProjectCollectionProperties()
      {
        Id = hostProperties.HostId,
        Name = hostProperties.Name,
        Description = string.Empty,
        IsDefault = false,
        State = TeamFoundationServiceHostStatus.Started
      };
      ServicingJobData servicingJobData = this.GetServicingJobData(requestContext, hostProperties.ParentHostId, collectionProperties, servicingTokens, (ISqlConnectionInfo) null, requestingIdentity, true);
      using (IVssRequestContext parentContext = this.CreateParentContext(requestContext, hostProperties.ParentHostId))
      {
        ServicingJobDetail host = parentContext.GetService<TeamFoundationServicingService>().PerformServicingJob(parentContext, servicingJobData, collectionProperties.Id, DateTime.UtcNow, (ITFLogger) null);
        if (host.Result == ServicingJobResult.Failed)
          throw new ConfigurationErrorsException(FrameworkResources.CreateCollectionFailed((object) host.JobId.ToString("D")));
        CollectionHostModifiedMessage hostModifiedMessage = new CollectionHostModifiedMessage();
        hostModifiedMessage.HostId = hostProperties.HostId;
        hostModifiedMessage.HostName = hostProperties.Name;
        hostModifiedMessage.ModificationType = HostModificationType.Created;
        CollectionHostModifiedMessage notificationEvent = hostModifiedMessage;
        requestContext.GetService<TeamFoundationEventService>().PublishNotification(requestContext, (object) notificationEvent);
        return host;
      }
    }

    public virtual void CreateVirtualHost(
      IVssRequestContext requestContext,
      ServiceHostProperties hostProperties)
    {
      requestContext.GetService<IVirtualHostInstanceMappingRegistrationService>().RegisterVirtualHostInstanceMapping(requestContext, hostProperties.HostId);
      TeamFoundationServiceHostProperties serviceHostProperties = new TeamFoundationServiceHostProperties();
      serviceHostProperties.Id = hostProperties.HostId;
      serviceHostProperties.ParentId = hostProperties.ParentHostId;
      serviceHostProperties.Name = hostProperties.Name;
      serviceHostProperties.Description = hostProperties.Description;
      serviceHostProperties.Status = TeamFoundationServiceHostStatus.Started;
      serviceHostProperties.StatusReason = "Creating Collection Host";
      serviceHostProperties.HostType = TeamFoundationHostType.ProjectCollection;
      serviceHostProperties.DatabaseId = -2;
      serviceHostProperties.StorageAccountId = -2;
      serviceHostProperties.ServiceLevel = requestContext.ServiceHost.DeploymentServiceHost.DatabaseProperties.ServiceLevel;
      TeamFoundationServiceHostProperties hostProperties1 = serviceHostProperties;
      requestContext.GetService<ITeamFoundationHostManagementService>().CreateServiceHost(requestContext, hostProperties1, (ISqlConnectionInfo) null, CreateHostOptions.None);
    }

    private ServicingJobData GetServicingJobData(
      IVssRequestContext requestContext,
      Guid parentHostId,
      TeamProjectCollectionProperties collectionProperties,
      IDictionary<string, string> servicingTokens,
      ISqlConnectionInfo dataTierConnectionInfo,
      Microsoft.VisualStudio.Services.Identity.Identity requestingIdentity,
      bool collectionIdCanAlreadyExist)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<TeamProjectCollectionProperties>(collectionProperties, nameof (collectionProperties));
      ArgumentUtility.CheckStringForNullOrEmpty(collectionProperties.Name, "Name");
      TeamFoundationExecutionEnvironment executionEnvironment = requestContext.ExecutionEnvironment;
      collectionProperties.VirtualDirectory = !executionEnvironment.IsOnPremisesDeployment ? "~/" : "~/" + collectionProperties.Name + "/";
      this.NormalizeAndValidateNewlyCreatedCollectionProperties(requestContext, parentHostId, collectionProperties, collectionIdCanAlreadyExist);
      collectionProperties.State = TeamFoundationServiceHostStatus.Stopped;
      if (dataTierConnectionInfo == null)
        dataTierConnectionInfo = requestContext.FrameworkConnectionInfo;
      if (dataTierConnectionInfo == null)
        dataTierConnectionInfo = collectionProperties.DefaultConnectionInfo;
      SqlConnectionStringBuilder connectionStringBuilder = dataTierConnectionInfo != null ? new SqlConnectionStringBuilder(dataTierConnectionInfo.ConnectionString) : throw new InvalidConfigurationException(FrameworkResources.ConnectionStringNotFound((object) "Framework"));
      List<string> stringList = new List<string>();
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        stringList.Add(ServicingOperationConstants.ProvisionCollectionCreate);
        TeamFoundationDatabasePool databasePool = requestContext.GetService<TeamFoundationDatabaseManagementService>().GetDatabasePool(requestContext, DatabaseManagementConstants.DefaultPartitionPoolName);
        stringList.AddRange((IEnumerable<string>) databasePool.ServicingOperations.Split(';'));
        stringList.Add(ServicingOperationConstants.UpdatePartitionDatabaseProperties);
      }
      stringList.Add(ServicingOperationConstants.CreateCollection);
      stringList.Add(ServicingOperationConstants.CreateSplitDatabase_ServiceSpecific);
      stringList.Add(ServicingOperationConstants.CreateDataspaces_ServiceSpecific);
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        stringList.AddRange((IEnumerable<string>) OnPremiseServicingConstants.CollectionOperations);
      else
        stringList.Add(ServicingOperationConstants.CreateCollection_ServiceSpecific);
      stringList.Add(ServicingOperationConstants.FinishCreateCollection);
      ServicingJobData servicingJobData1 = new ServicingJobData(stringList.ToArray());
      servicingJobData1.ServicingHostId = collectionProperties.Id;
      servicingJobData1.JobTitle = FrameworkResources.CreateCollectionJobTitle((object) collectionProperties.Id);
      servicingJobData1.OperationClass = "CreateCollection";
      servicingJobData1.ServicingOptions = collectionIdCanAlreadyExist ? ServicingFlags.None : ServicingFlags.HostMustNotExist;
      servicingJobData1.ServicingTokens[ServicingTokenConstants.FinalHostState] = "Started";
      servicingJobData1.ServicingTokens[ServicingTokenConstants.DatabaseType] = TeamFoundationSqlResourceComponent.DatabaseTypeCollection;
      IDictionary<string, string> servicingTokens1 = servicingJobData1.ServicingTokens;
      string hostId = ServicingTokenConstants.HostId;
      Guid id = collectionProperties.Id;
      string str1 = id.ToString("D");
      servicingTokens1[hostId] = str1;
      servicingJobData1.ServicingTokens[ServicingTokenConstants.ParentHostId] = parentHostId.ToString("D");
      CollectionHostManagementService.SetCollectionCreationTime(servicingJobData1, DateTime.UtcNow);
      string str2 = requestContext.ServiceHost.DeploymentServiceHost.DatabaseProperties.ServiceLevel;
      if (str2.EndsWith(";", StringComparison.Ordinal))
        str2 = str2.Substring(0, str2.Length - 1);
      servicingJobData1.ServicingTokens[ServicingTokenConstants.ServiceLevel] = str2;
      ServicingJobData servicingJobData2 = servicingJobData1;
      TeamFoundationLockInfo[] foundationLockInfoArray = new TeamFoundationLockInfo[1];
      TeamFoundationLockInfo foundationLockInfo = new TeamFoundationLockInfo();
      foundationLockInfo.LockMode = TeamFoundationLockMode.Exclusive;
      id = collectionProperties.Id;
      foundationLockInfo.LockName = "Servicing-" + id.ToString();
      foundationLockInfo.LockTimeout = -1;
      foundationLockInfoArray[0] = foundationLockInfo;
      servicingJobData2.ServicingLocks = foundationLockInfoArray;
      if (servicingTokens != null)
      {
        foreach (KeyValuePair<string, string> servicingToken in (IEnumerable<KeyValuePair<string, string>>) servicingTokens)
          servicingJobData1.ServicingTokens[servicingToken.Key] = servicingToken.Value;
      }
      string str3 = (string) null;
      if (servicingJobData1.ServicingTokens.ContainsKey(ServicingTokenConstants.CollectionDatabaseName))
      {
        str3 = servicingJobData1.ServicingTokens[ServicingTokenConstants.CollectionDatabaseName];
      }
      else
      {
        if (collectionProperties.DefaultConnectionInfo != null && !string.IsNullOrEmpty(collectionProperties.DefaultConnectionInfo.InitialCatalog))
          str3 = collectionProperties.DefaultConnectionInfo.InitialCatalog;
        if (string.IsNullOrEmpty(str3) && collectionProperties.FrameworkConnectionInfo != null && !string.IsNullOrEmpty(collectionProperties.FrameworkConnectionInfo.InitialCatalog))
          str3 = collectionProperties.FrameworkConnectionInfo.InitialCatalog;
        if (string.IsNullOrEmpty(str3))
          str3 = "AzureDevOps_" + collectionProperties.Name;
        servicingJobData1.ServicingTokens.Add(ServicingTokenConstants.CollectionDatabaseName, str3);
      }
      if (!servicingJobData1.ServicingTokens.ContainsKey(ServicingTokenConstants.SqlServerInstance))
        servicingJobData1.ServicingTokens.Add(ServicingTokenConstants.SqlServerInstance, connectionStringBuilder.DataSource);
      if (requestingIdentity != null)
      {
        servicingJobData1.ServicingItems.Add(ServicingItemConstants.RequestingIdentity, (object) requestingIdentity.Descriptor);
        if (requestContext.ExecutionEnvironment.IsHostedDeployment)
          servicingJobData1.ServicingItems[ServicingItemConstants.RequestingIdentityObject] = (object) requestingIdentity;
      }
      servicingJobData1.ServicingItems.Add(ServicingItemConstants.CollectionProperties, (object) collectionProperties);
      servicingJobData1.ServicingTokens[ServicingTokenConstants.CollectionName] = collectionProperties.Name;
      IDictionary<string, string> servicingTokens2 = servicingJobData1.ServicingTokens;
      string instanceId = ServicingTokenConstants.InstanceId;
      id = collectionProperties.Id;
      string str4 = id.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture);
      servicingTokens2[instanceId] = str4;
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        connectionStringBuilder.InitialCatalog = str3;
        collectionProperties.DefaultConnectionInfo = SqlConnectionInfoFactory.Create(connectionStringBuilder.ConnectionString);
      }
      else
        servicingJobData1.ServicingTokens[ServicingTokenConstants.UseSharedService] = (requestContext.ServiceInstanceType() != ServiceInstanceTypes.SPS).ToString();
      return servicingJobData1;
    }

    private static string ValidateChildAppRelativePath(
      string suppliedPath,
      string argumentName,
      bool allowEmptyPath)
    {
      string str = suppliedPath;
      if (str != null)
        str = str.Trim();
      ArgumentUtility.CheckStringForNullOrEmpty(str, argumentName);
      try
      {
        if (!VirtualPathUtility.IsAppRelative(str))
          throw new ArgumentException(FrameworkResources.CollectionRequiresRelativePath((object) argumentName));
      }
      catch (HttpException ex)
      {
        throw new ArgumentException(FrameworkResources.CollectionRequiresRelativePath((object) argumentName), (Exception) ex);
      }
      string proposedVirtualPath = VirtualPathUtility.AppendTrailingSlash(VirtualPathUtility.ToAppRelative(str));
      if (allowEmptyPath && proposedVirtualPath.Equals("~/", StringComparison.Ordinal))
        return proposedVirtualPath;
      if (proposedVirtualPath.Length <= 2)
        throw new ArgumentException(FrameworkResources.CollectionRequiresSubDirectory((object) argumentName));
      foreach (string forbiddenChildPath in ForbiddenHostNames.ForbiddenChildPaths)
        CollectionHostManagementService.CheckVirtualPathsDontOverlap(forbiddenChildPath, proposedVirtualPath);
      foreach (object virtualDirectory in TeamFoundationHostManagementService.s_v1ServiceVirtualDirectories)
        CollectionHostManagementService.CheckVirtualPathsDontOverlap(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "~/{0}", virtualDirectory), proposedVirtualPath);
      return !proposedVirtualPath.Contains("..") ? proposedVirtualPath : throw new ArgumentException(FrameworkResources.InvalidVirtualPathError((object) proposedVirtualPath));
    }

    private static void CheckVirtualPathsDontOverlap(
      string reservedVirtualPath,
      string proposedVirtualPath)
    {
      if (proposedVirtualPath.StartsWith(reservedVirtualPath, StringComparison.OrdinalIgnoreCase) || reservedVirtualPath.StartsWith(proposedVirtualPath, StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(FrameworkResources.VirtualPathsConflictException((object) proposedVirtualPath));
    }

    internal void NormalizeAndValidateNewlyCreatedCollectionProperties(
      IVssRequestContext requestContext,
      Guid parentHostId,
      TeamProjectCollectionProperties collectionProperties,
      bool collectionIdCanAlreadyExist)
    {
      if (collectionProperties.Id == Guid.Empty)
        collectionProperties.Id = Guid.NewGuid();
      this.NormalizeAndValidateCollectionProperties(requestContext, parentHostId, collectionProperties, collectionIdCanAlreadyExist);
    }

    internal void NormalizeAndValidateCollectionProperties(
      IVssRequestContext requestContext,
      Guid parentHostId,
      TeamProjectCollectionProperties collectionProperties,
      bool collectionIdCanAlreadyExist)
    {
      requestContext.CheckDeploymentRequestContext();
      collectionProperties.Name = CollectionHostManagementService.ValidateCollectionName(collectionProperties.Name);
      collectionProperties.VirtualDirectory = CollectionHostManagementService.ValidateChildAppRelativePath(collectionProperties.VirtualDirectory, "collectionProperties.VirtualDirectory", requestContext.ExecutionEnvironment.IsHostedDeployment);
      CollectionHostManagementService.CheckVirtualPathsDontOverlap(CollectionHostManagementService.s_defaultResourceDirectory, collectionProperties.VirtualDirectory);
      requestContext.GetService<IInternalUrlHostResolutionService>();
      ITeamFoundationDatabaseManagementService service = requestContext.GetService<ITeamFoundationDatabaseManagementService>();
      TeamFoundationServiceHostProperties serviceHostProperties = requestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(requestContext, parentHostId, ServiceHostFilterFlags.IncludeChildren);
      if (serviceHostProperties == null || serviceHostProperties.Children == null)
        return;
      foreach (TeamFoundationServiceHostProperties child in serviceHostProperties.Children)
      {
        if (child.HostType == TeamFoundationHostType.ProjectCollection)
        {
          if (child.Id == collectionProperties.Id)
          {
            if (!collectionIdCanAlreadyExist)
              throw new ArgumentException(FrameworkResources.CollectionWithIdAlreadyExists((object) collectionProperties.Id));
          }
          else
          {
            if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && VssStringComparer.Hostname.Equals(child.Name, collectionProperties.Name))
              throw new ArgumentException(FrameworkResources.VirtualPathsConflictException((object) ("~/" + collectionProperties.Name + "/")));
            ITeamFoundationDatabaseProperties database = service.GetDatabase(requestContext, child.DatabaseId);
            SqlConnectionStringBuilder connectionStringBuilder1 = new SqlConnectionStringBuilder(database.SqlConnectionInfo == null ? string.Empty : database.SqlConnectionInfo.ConnectionString);
            SqlConnectionStringBuilder connectionStringBuilder2 = new SqlConnectionStringBuilder(collectionProperties.FrameworkConnectionInfo == null ? string.Empty : collectionProperties.FrameworkConnectionInfo.ConnectionString);
            if (StringComparer.OrdinalIgnoreCase.Equals(connectionStringBuilder1.InitialCatalog, connectionStringBuilder2.InitialCatalog) && StringComparer.OrdinalIgnoreCase.Equals(connectionStringBuilder1.DataSource, connectionStringBuilder2.DataSource))
              throw new ArgumentException(FrameworkResources.CollectionWithConnectionStringAlreadyExists());
          }
        }
      }
    }

    internal static string ValidateCollectionName(string collectionName)
    {
      if (collectionName != null)
        collectionName = collectionName.Trim();
      ArgumentUtility.CheckStringForNullOrEmpty(collectionName, nameof (collectionName));
      if (!ForbiddenHostNames.IsNameAllowed(collectionName, TeamFoundationHostType.ProjectCollection))
        throw new ArgumentException(FrameworkResources.CollectionNameReserved((object) collectionName));
      return !collectionName.Contains("..") ? collectionName : throw new ArgumentException(FrameworkResources.InvalidCollectionNameTwoDotError((object) collectionName));
    }

    internal static void ValidateVirtualDirectory(string virtualDirectory) => CollectionHostManagementService.ValidateChildAppRelativePath(virtualDirectory, "collectionProperties.VirtualDirectory", false);

    internal static void SetCollectionCreationTime(
      ServicingJobData servicingJobData,
      DateTime dateTime)
    {
      servicingJobData.ServicingTokens[ServicingTokenConstants.CollectionCreationTime] = dateTime.ToUniversalTime().ToString("O", (IFormatProvider) CultureInfo.InvariantCulture);
    }

    private IVssRequestContext CreateParentContext(
      IVssRequestContext requestContext,
      Guid parentHostId)
    {
      return requestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(requestContext, parentHostId, RequestContextType.ServicingContext, throwIfShutdown: false);
    }
  }
}
