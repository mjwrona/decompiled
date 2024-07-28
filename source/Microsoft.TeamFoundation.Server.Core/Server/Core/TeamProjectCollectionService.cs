// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamProjectCollectionService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Security.Principal;

namespace Microsoft.TeamFoundation.Server.Core
{
  public sealed class TeamProjectCollectionService : 
    IInternalTeamProjectCollectionService,
    IVssFrameworkService
  {
    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckOrganizationRequestContext();

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public ServicingJobDetail QueueDeleteCollection(
      IVssRequestContext requestContext,
      Guid collectionId)
    {
      requestContext.CheckOrganizationRequestContext();
      this.CheckReadCollectionPermission(requestContext, collectionId);
      this.CheckDeleteCollectionPermission(requestContext, collectionId);
      ServicingJobData collectionJobData = this.GenerateDeleteCollectionJobData(requestContext, collectionId);
      return requestContext.GetService<TeamFoundationServicingService>().QueueServicingJob(requestContext, collectionJobData, JobPriorityClass.Normal, JobPriorityLevel.Lowest, new Guid?());
    }

    public ServicingJobDetail QueueAttachCollection(
      IVssRequestContext requestContext,
      TeamProjectCollectionProperties collectionProperties,
      IDictionary<string, string> servicingTokens,
      bool cloneCollection)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<TeamProjectCollectionProperties>(collectionProperties, nameof (collectionProperties));
      if (collectionProperties.Name != null)
      {
        ArgumentUtility.CheckStringLength(collectionProperties.Name, "Name", 123 - "AzureDevOps_".Length, 1);
        ArgumentUtility.CheckStringForInvalidCharacters(collectionProperties.Name, "Name", DatabaseNamingConstants.IllegalDatabaseNameCharacters);
        if (!CssUtils.IsValidProjectName(collectionProperties.Name))
          throw new ArgumentException(Resources.InvalidCollectionName());
        CollectionHostManagementService.ValidateCollectionName(collectionProperties.Name);
      }
      if (collectionProperties.VirtualDirectory != null)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(collectionProperties.VirtualDirectory, "VirtualDirectory");
        CollectionHostManagementService.ValidateVirtualDirectory(collectionProperties.VirtualDirectory);
      }
      string connectionString = collectionProperties.DefaultConnectionString;
      ArgumentUtility.CheckStringForNullOrEmpty(connectionString, "DefaultConnectionString");
      collectionProperties.FrameworkConnectionInfo = SqlConnectionInfoFactory.Create(connectionString);
      try
      {
        SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(collectionProperties.FrameworkConnectionInfo.ConnectionString);
        ArgumentUtility.CheckStringLength(connectionStringBuilder.InitialCatalog, "Initial Catalog", 123);
        ArgumentUtility.CheckStringForInvalidCharacters(connectionStringBuilder.InitialCatalog, "Initial Catalog", DatabaseNamingConstants.IllegalDatabaseNameCharacters);
      }
      catch (ArgumentException ex)
      {
        throw new ArgumentException(FrameworkResources.ArgumentPropertyIsInvalid((object) "FrameworkConnectionString", (object) ex.Message), nameof (collectionProperties), (Exception) ex);
      }
      this.CheckCreateCollectionPermission(requestContext);
      List<string> stringList = new List<string>();
      if (servicingTokens == null)
        servicingTokens = (IDictionary<string, string>) new Dictionary<string, string>();
      bool collectionNeedsUpgrade;
      ServiceLevel collectionServiceLevel;
      TeamProjectCollectionService.ValidateServiceLevelsForAttach(requestContext.FrameworkConnectionInfo, collectionProperties.FrameworkConnectionInfo, out collectionNeedsUpgrade, out collectionServiceLevel);
      servicingTokens[ServicingTokenConstants.ServiceLevelFrom] = collectionServiceLevel.ToString();
      if (collectionNeedsUpgrade)
      {
        stringList.Add(ServicingOperationConstants.PrepareAttachCollection);
        stringList.Add(ServicingOperationConstants.BringToCurrentServiceLevelAndAttach);
        servicingTokens["ServicingAction"] = "Upgrade";
      }
      else
      {
        stringList.Add(ServicingOperationConstants.PrepareAttachCollection);
        stringList.Add(ServicingOperationConstants.AttachCollection);
        stringList.Add(ServicingOperationConstants.UpdatePartitionDatabaseProperties);
      }
      using (ExtendedAttributeComponent componentRaw = collectionProperties.FrameworkConnectionInfo.CreateComponentRaw<ExtendedAttributeComponent>())
      {
        if (!StringComparer.OrdinalIgnoreCase.Equals(componentRaw.ReadDatabaseAttribute(CollectionMoveConstants.SnapshotStateExtendedProperty), CollectionMoveConstants.SnapshotStateComplete))
          throw new AttachCollectionException(FrameworkResources.DatabaseMustHaveSnapshotAttributeToAttach());
      }
      if (collectionProperties.Id == Guid.Empty)
        collectionProperties.Id = TeamProjectCollectionService.DetermineAttachCollectionId(collectionProperties.FrameworkConnectionInfo, cloneCollection);
      if (requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationHostManagementService>().QueryServiceHostProperties(requestContext.To(TeamFoundationHostType.Deployment), collectionProperties.Id, ServiceHostFilterFlags.None) != null)
        throw new AttachCollectionException(FrameworkResources.CollectionInstanceIdAlreadyExists((object) collectionProperties.Id));
      ServicingJobData servicingJobData = new ServicingJobData(stringList.ToArray());
      servicingJobData.ServicingHostId = collectionProperties.Id;
      servicingJobData.JobTitle = FrameworkResources.AttachCollectionJobTitle((object) collectionProperties.Id);
      servicingJobData.OperationClass = "AttachCollection";
      servicingJobData.ServicingOptions = ServicingFlags.HostMustNotExist;
      servicingJobData.ServicingTokens[ServicingTokenConstants.FinalHostState] = "Started";
      servicingJobData.ServicingLocks = new TeamFoundationLockInfo[1]
      {
        new TeamFoundationLockInfo()
        {
          LockMode = TeamFoundationLockMode.Exclusive,
          LockName = "Servicing-" + collectionProperties.Id.ToString(),
          LockTimeout = -1
        }
      };
      this.AddAttachCommonTokens(requestContext, collectionProperties, servicingTokens, servicingJobData);
      servicingJobData.ServicingItems[ServicingItemConstants.ConnectionInfo] = (object) new SqlConnectionInfoWrapper(requestContext, collectionProperties.FrameworkConnectionInfo);
      return requestContext.GetService<TeamFoundationServicingService>().QueueServicingJob(requestContext, servicingJobData, JobPriorityClass.AboveNormal, JobPriorityLevel.Normal, new Guid?());
    }

    public ServicingJobDetail QueueUpdateCollection(
      IVssRequestContext requestContext,
      TeamProjectCollectionProperties updatedCollectionProperties,
      IDictionary<string, string> servicingTokens)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<TeamProjectCollectionProperties>(updatedCollectionProperties, nameof (updatedCollectionProperties));
      ArgumentUtility.CheckStringLength(updatedCollectionProperties.Name, "Name", 123 - "AzureDevOps_".Length, 1);
      ArgumentUtility.CheckStringForInvalidCharacters(updatedCollectionProperties.Name, "Name", DatabaseNamingConstants.IllegalDatabaseNameCharacters);
      ArgumentUtility.CheckStringForNullOrEmpty(updatedCollectionProperties.VirtualDirectory, "VirtualDirectory");
      if (!CssUtils.IsValidProjectName(updatedCollectionProperties.Name))
        throw new ArgumentException(Resources.InvalidCollectionName());
      CollectionHostManagementService.ValidateCollectionName(updatedCollectionProperties.Name);
      CollectionHostManagementService.ValidateVirtualDirectory(updatedCollectionProperties.VirtualDirectory);
      string connectionString = updatedCollectionProperties.DefaultConnectionString;
      try
      {
        SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
        ArgumentUtility.CheckStringLength(connectionStringBuilder.InitialCatalog, "Initial Catalog", 123);
        ArgumentUtility.CheckStringForInvalidCharacters(connectionStringBuilder.InitialCatalog, "Initial Catalog", DatabaseNamingConstants.IllegalDatabaseNameCharacters);
      }
      catch (ArgumentException ex)
      {
        throw new ArgumentException(FrameworkResources.ArgumentPropertyIsInvalid((object) "FrameworkConnectionString", (object) ex.Message), nameof (updatedCollectionProperties), (Exception) ex);
      }
      updatedCollectionProperties.FrameworkConnectionInfo = SqlConnectionInfoFactory.Create(connectionString);
      requestContext.CheckOrganizationRequestContext();
      this.CheckManageCollectionPermission(requestContext, updatedCollectionProperties.Id);
      TeamProjectCollectionProperties collectionProperties = requestContext.GetService<ITeamProjectCollectionPropertiesService>().GetCollectionProperties(requestContext, updatedCollectionProperties.Id, ServiceHostFilterFlags.None);
      collectionProperties.DefaultConnectionInfo = collectionProperties.FrameworkConnectionInfo;
      ServicingJobData servicingJobData = new ServicingJobData(new string[1]
      {
        ServicingOperationConstants.UpdateCollectionProperties
      });
      if (servicingTokens != null)
      {
        foreach (KeyValuePair<string, string> servicingToken in (IEnumerable<KeyValuePair<string, string>>) servicingTokens)
          servicingJobData.ServicingTokens.Add(servicingToken.Key, servicingToken.Value);
      }
      servicingJobData.ServicingHostId = updatedCollectionProperties.Id;
      servicingJobData.JobTitle = FrameworkResources.UpdateCollectionPropertiesTitle((object) updatedCollectionProperties.Id);
      servicingJobData.OperationClass = "UpdateCollectionProperties";
      servicingJobData.ServicingOptions = ServicingFlags.HostMustExist;
      if (!servicingJobData.ServicingTokens.ContainsKey(ServicingTokenConstants.FinalHostState))
        servicingJobData.ServicingTokens[ServicingTokenConstants.FinalHostState] = collectionProperties.State.ToString();
      servicingJobData.ServicingTokens[ServicingTokenConstants.InstanceId] = updatedCollectionProperties.Id.ToString("D");
      servicingJobData.ServicingLocks = new TeamFoundationLockInfo[1]
      {
        new TeamFoundationLockInfo()
        {
          LockMode = TeamFoundationLockMode.Exclusive,
          LockName = "Servicing-" + updatedCollectionProperties.Id.ToString(),
          LockTimeout = -1
        }
      };
      if (updatedCollectionProperties.FrameworkConnectionInfo != null)
        servicingJobData.ServicingItems[ServicingItemConstants.ConnectionInfo] = (object) new SqlConnectionInfoWrapper(requestContext, updatedCollectionProperties.FrameworkConnectionInfo);
      servicingJobData.ServicingItems[ServicingItemConstants.CollectionProperties] = (object) updatedCollectionProperties;
      servicingJobData.ServicingItems[ServicingItemConstants.InitialCollectionProperties] = (object) collectionProperties;
      servicingJobData.ServicingTokens[ServicingTokenConstants.DatabaseId] = collectionProperties.DatabaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      servicingJobData.ServicingItems[ServicingItemConstants.CollectionDatabaseReachable] = (object) (bool) (updatedCollectionProperties.FrameworkConnectionInfo == null ? 1 : (StringComparer.Ordinal.Equals((object) updatedCollectionProperties.FrameworkConnectionInfo, (object) collectionProperties.FrameworkConnectionInfo) ? 1 : 0));
      return requestContext.GetService<TeamFoundationServicingService>().QueueServicingJob(requestContext, servicingJobData, JobPriorityClass.High, JobPriorityLevel.Normal, new Guid?());
    }

    internal static Guid DetermineAttachCollectionId(
      ISqlConnectionInfo frameworkConnectionInfo,
      bool cloneCollection)
    {
      Guid x = Guid.Empty;
      using (DatabasePartitionComponent component = DatabasePartitionComponent.CreateComponent(frameworkConnectionInfo))
        x = component.QueryOnlyPartition().ServiceHostId;
      if (!cloneCollection)
        return x;
      using (ExtendedAttributeComponent componentRaw = frameworkConnectionInfo.CreateComponentRaw<ExtendedAttributeComponent>())
      {
        string str = componentRaw.ReadDatabaseAttribute(CollectionMoveConstants.AttachCloneCollectionId);
        Guid attachCollectionId = string.IsNullOrEmpty(str) || StringComparer.OrdinalIgnoreCase.Equals((object) x, (object) str) ? Guid.NewGuid() : new Guid(str);
        componentRaw.WriteDatabaseAttribute(CollectionMoveConstants.AttachCloneCollectionId, attachCollectionId.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture));
        return attachCollectionId;
      }
    }

    public ServicingJobDetail QueueCreateCollection(
      IVssRequestContext requestContext,
      TeamProjectCollectionProperties collectionProperties,
      IDictionary<string, string> servicingTokens,
      ISqlConnectionInfo dataTierConnectionInfo = null,
      JobPriorityClass priorityClass = JobPriorityClass.High,
      JobPriorityLevel priorityLevel = JobPriorityLevel.Normal)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<TeamProjectCollectionProperties>(collectionProperties, nameof (collectionProperties));
      ArgumentUtility.CheckStringLength(collectionProperties.Name, "Name", 123 - "AzureDevOps_".Length, 1);
      ArgumentUtility.CheckStringForInvalidCharacters(collectionProperties.Name, "Name", DatabaseNamingConstants.IllegalDatabaseNameCharacters);
      if (!CssUtils.IsValidProjectName(collectionProperties.Name))
        throw new ArgumentException(Resources.InvalidCollectionName());
      CollectionHostManagementService.ValidateCollectionName(collectionProperties.Name);
      requestContext.CheckOrganizationRequestContext();
      this.CheckCreateCollectionPermission(requestContext);
      if (dataTierConnectionInfo == null)
        dataTierConnectionInfo = requestContext.FrameworkConnectionInfo;
      if (dataTierConnectionInfo == null)
        dataTierConnectionInfo = collectionProperties.DefaultConnectionInfo;
      Microsoft.VisualStudio.Services.Identity.Identity requestingIdentity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      IdentityDescriptor identityDescriptor = this.GetRequestingIdentityDescriptor(requestContext);
      if (identityDescriptor != (IdentityDescriptor) null)
        requestingIdentity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          identityDescriptor
        }, QueryMembership.None, (IEnumerable<string>) null)[0];
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<CollectionHostManagementService>().QueueCreateCollectionHost(vssRequestContext, requestContext.ServiceHost.InstanceId, collectionProperties, servicingTokens, dataTierConnectionInfo, priorityClass, priorityLevel, requestingIdentity);
    }

    public ServicingJobDetail QueueDetachCollection(
      IVssRequestContext requestContext,
      Guid collectionId,
      IDictionary<string, string> servicingTokens,
      string collectionStoppedMessage,
      out string detachedConnectionString)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(collectionId, nameof (collectionId));
      this.CheckManageCollectionPermission(requestContext, collectionId);
      this.CheckDeleteCollectionPermission(requestContext, collectionId);
      this.CheckCollectionIsDetachable(requestContext, collectionId);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      TeamProjectCollectionProperties collectionProperties = requestContext.GetService<ITeamProjectCollectionPropertiesService>().GetCollectionProperties(requestContext, collectionId, ServiceHostFilterFlags.None);
      TeamFoundationServiceHostProperties serviceHostProperties = vssRequestContext.GetService<TeamFoundationHostManagementService>().QueryServiceHostProperties(vssRequestContext, collectionProperties.Id);
      TeamFoundationDatabaseManagementService service = vssRequestContext.GetService<TeamFoundationDatabaseManagementService>();
      detachedConnectionString = service.GetSqlConnectionInfo(vssRequestContext, serviceHostProperties.DatabaseId).ConnectionString;
      ServicingJobData servicingJobData = new ServicingJobData(new string[2]
      {
        ServicingOperationConstants.Snapshot,
        ServicingOperationConstants.Delete
      });
      servicingJobData.ServicingHostId = collectionProperties.Id;
      servicingJobData.JobTitle = FrameworkResources.DetachCollectionJobTitle((object) collectionProperties.Id);
      servicingJobData.OperationClass = "DetachCollection";
      servicingJobData.ServicingOptions = ServicingFlags.RequiresStoppedHost;
      servicingJobData.ServicingLocks = new TeamFoundationLockInfo[1]
      {
        new TeamFoundationLockInfo()
        {
          LockMode = TeamFoundationLockMode.Exclusive,
          LockName = "Servicing-" + collectionProperties.Id.ToString(),
          LockTimeout = -1
        }
      };
      ((IInternalTeamProjectCollectionService) this).AddCommonTokensAndItems(requestContext, servicingJobData, collectionProperties, true);
      servicingJobData.ServicingItems[ServicingItemConstants.ConnectionInfo] = (object) new SqlConnectionInfoWrapper(requestContext, collectionProperties.FrameworkConnectionInfo);
      servicingJobData.ServicingTokens.Add(ServicingTokenConstants.StopHostReason, collectionStoppedMessage);
      return requestContext.GetService<TeamFoundationServicingService>().QueueServicingJob(requestContext, servicingJobData, JobPriorityClass.AboveNormal, JobPriorityLevel.Normal, new Guid?());
    }

    public void CheckCollectionIsDetachable(IVssRequestContext requestContext, Guid collectionId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(collectionId, nameof (collectionId));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      TeamFoundationHostManagementService service1 = vssRequestContext.GetService<TeamFoundationHostManagementService>();
      TeamFoundationDatabaseManagementService service2 = vssRequestContext.GetService<TeamFoundationDatabaseManagementService>();
      IVssRequestContext requestContext1 = vssRequestContext;
      Guid hostId = collectionId;
      TeamFoundationServiceHostProperties serviceHostProperties = service1.QueryServiceHostProperties(requestContext1, hostId);
      ISqlConnectionInfo sqlConnectionInfo = service2.GetSqlConnectionInfo(vssRequestContext, serviceHostProperties.DatabaseId);
      if (serviceHostProperties == null)
        throw new CollectionDoesNotExistException(collectionId);
      TeamProjectCollectionService.ValidateServiceLevelsForDetach(vssRequestContext.FrameworkConnectionInfo, sqlConnectionInfo, serviceHostProperties.Name);
    }

    public ServicingJobDetail QueueServiceCollection(
      IVssRequestContext requestContext,
      TeamProjectCollectionProperties collection,
      bool stopHostNow,
      params string[] servicingOperations)
    {
      return this.QueueServiceCollections(requestContext, (IEnumerable<TeamProjectCollectionProperties>) new TeamProjectCollectionProperties[1]
      {
        collection
      }, (stopHostNow ? 1 : 0) != 0, servicingOperations)[0];
    }

    public List<ServicingJobDetail> QueueServiceCollections(
      IVssRequestContext requestContext,
      IEnumerable<TeamProjectCollectionProperties> collections,
      bool stopHostsNow,
      params string[] servicingOperations)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      TeamFoundationServicingService service1 = requestContext.GetService<TeamFoundationServicingService>();
      TeamFoundationHostManagementService service2 = requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationHostManagementService>();
      List<TeamFoundationJobDefinition> foundationJobDefinitionList = new List<TeamFoundationJobDefinition>();
      List<ServicingJobData> servicingJobsData = new List<ServicingJobData>();
      foreach (TeamProjectCollectionProperties collection in collections)
      {
        ServicingJobData servicingJobData = new ServicingJobData(servicingOperations);
        servicingJobData.ServicingHostId = collection.Id;
        servicingJobData.JobTitle = FrameworkResources.ServiceCollectionJobTitle((object) collection.Id);
        servicingJobData.OperationClass = "ApplyPatch";
        servicingJobData.ServicingOptions = ServicingFlags.RequiresStoppedHost;
        servicingJobData.ServicingLocks = new TeamFoundationLockInfo[1]
        {
          new TeamFoundationLockInfo()
          {
            LockMode = TeamFoundationLockMode.Exclusive,
            LockName = "Servicing-" + collection.Id.ToString(),
            LockTimeout = -1
          }
        };
        foreach (KeyValue<string, string> keyValue in collection.ServicingTokensValue)
          servicingJobData.ServicingTokens[keyValue.Key] = keyValue.Value;
        if (collection.State == TeamFoundationServiceHostStatus.Started)
          servicingJobData.ServicingTokens[ServicingTokenConstants.FinalHostState] = "Started";
        if (stopHostsNow && service2.QueryServiceHostProperties(requestContext.To(TeamFoundationHostType.Deployment), collection.Id, ServiceHostFilterFlags.None).Status == TeamFoundationServiceHostStatus.Started)
          service2.StopHost(requestContext.To(TeamFoundationHostType.Deployment), collection.Id, ServiceHostSubStatus.Servicing, FrameworkResources.ProjectCollectionServicingInProgress(), TimeSpan.Zero);
        using (ExtendedAttributeComponent componentRaw = collection.FrameworkConnectionInfo.CreateComponentRaw<ExtendedAttributeComponent>())
        {
          string str = componentRaw.ReadServiceLevelStamp();
          servicingJobData.ServicingTokens[ServicingTokenConstants.ServiceLevelFrom] = str;
        }
        ((IInternalTeamProjectCollectionService) this).AddCommonTokensAndItems(requestContext, servicingJobData, collection, requestContext.ExecutionEnvironment.IsOnPremisesDeployment);
        servicingJobsData.Add(servicingJobData);
      }
      return service1.QueueServicingJobs(requestContext, (IList<ServicingJobData>) servicingJobsData);
    }

    internal static void ValidateServiceLevelsForAttach(
      ISqlConnectionInfo configDbConnectionInfo,
      ISqlConnectionInfo collectionConnectionInfo,
      out bool collectionNeedsUpgrade)
    {
      TeamProjectCollectionService.ValidateServiceLevelsForAttach(configDbConnectionInfo, collectionConnectionInfo, out collectionNeedsUpgrade, out ServiceLevel _);
    }

    private void AddAttachCommonTokens(
      IVssRequestContext requestContext,
      TeamProjectCollectionProperties collectionProperties,
      IDictionary<string, string> servicingTokens,
      ServicingJobData servicingJobData)
    {
      if (servicingTokens != null)
      {
        foreach (KeyValuePair<string, string> servicingToken in (IEnumerable<KeyValuePair<string, string>>) servicingTokens)
          servicingJobData.ServicingTokens.Add(servicingToken.Key, servicingToken.Value);
      }
      ((IInternalTeamProjectCollectionService) this).AddCommonTokensAndItems(requestContext, servicingJobData, collectionProperties, true);
      servicingJobData.ServicingTokens[ServicingTokenConstants.ParentHostId] = requestContext.ServiceHost.InstanceId.ToString("D");
    }

    void IInternalTeamProjectCollectionService.AddCommonTokensAndItems(
      IVssRequestContext requestContext,
      ServicingJobData servicingJobData,
      TeamProjectCollectionProperties collectionProperties,
      bool addRequestingIdentityItem)
    {
      servicingJobData.ServicingItems.Add(ServicingItemConstants.CollectionProperties, (object) collectionProperties);
      if (addRequestingIdentityItem)
        servicingJobData.ServicingItems.Add(ServicingItemConstants.RequestingIdentity, (object) this.GetRequestingIdentityDescriptor(requestContext));
      servicingJobData.ServicingTokens[ServicingTokenConstants.CollectionName] = collectionProperties.Name;
      servicingJobData.ServicingTokens[ServicingTokenConstants.InstanceId] = collectionProperties.Id.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture);
      servicingJobData.ServicingTokens[ServicingTokenConstants.HostId] = collectionProperties.Id.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture);
      servicingJobData.ServicingTokens[ServicingTokenConstants.DatabaseId] = collectionProperties.DatabaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    private IdentityDescriptor GetRequestingIdentityDescriptor(IVssRequestContext requestContext)
    {
      IdentityDescriptor identityDescriptor;
      if (requestContext.UserContext == (IdentityDescriptor) null || IdentityDescriptorComparer.Instance.Equals(requestContext.UserContext, requestContext.ServiceHost.SystemDescriptor()))
      {
        if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          throw new UnauthorizedRequestException();
        identityDescriptor = IdentityHelper.CreateWindowsDescriptor(WindowsIdentity.GetCurrent().User);
      }
      else
        identityDescriptor = requestContext.UserContext;
      return identityDescriptor;
    }

    private static void ValidateServiceLevelsForAttach(
      ISqlConnectionInfo configDbConnectionInfo,
      ISqlConnectionInfo collectionConnectionInfo,
      out bool collectionNeedsUpgrade,
      out ServiceLevel collectionServiceLevel)
    {
      using (ExtendedAttributeComponent componentRaw1 = configDbConnectionInfo.CreateComponentRaw<ExtendedAttributeComponent>())
      {
        using (ExtendedAttributeComponent componentRaw2 = collectionConnectionInfo.CreateComponentRaw<ExtendedAttributeComponent>())
        {
          collectionNeedsUpgrade = false;
          collectionServiceLevel = new ServiceLevel(componentRaw2.ReadServiceLevelStamp());
          ServiceLevel serviceLevel = new ServiceLevel(componentRaw1.ReadServiceLevelStamp());
          if (collectionServiceLevel > serviceLevel)
            throw new AttachCollectionException(FrameworkResources.AttachCollectionServiceLevelNewer((object) collectionServiceLevel, (object) serviceLevel));
          if (!(collectionServiceLevel < serviceLevel))
            return;
          string message;
          if (!ServicingUtils.IsUpgradeAllowedForCollectionDatabase(collectionServiceLevel, out message))
            throw new AttachCollectionException(FrameworkResources.CannotAttachCollection((object) message));
          collectionNeedsUpgrade = true;
        }
      }
    }

    private void CheckCreateCollectionPermission(IVssRequestContext requestContext) => requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.CollectionManagementNamespaceId).CheckPermission(requestContext, FrameworkSecurity.CollectionManagementNamespaceToken, 1, false);

    private void CheckDeleteCollectionPermission(
      IVssRequestContext requestContext,
      Guid collectionId)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.CollectionManagementNamespaceId);
      string str = FrameworkSecurity.CollectionManagementNamespaceToken + (object) FrameworkSecurity.CollectionManagementPathSeparator + collectionId.ToString();
      IVssRequestContext requestContext1 = requestContext;
      string token = str;
      securityNamespace.CheckPermission(requestContext1, token, 2);
    }

    private bool CheckReadCollectionPermission(IVssRequestContext requestContext, Guid collectionId)
    {
      if (requestContext.IsSystemContext)
        return true;
      TeamFoundationHostManagementService service = requestContext.GetService<TeamFoundationHostManagementService>();
      bool flag = false;
      try
      {
        using (IVssRequestContext vssRequestContext = service.BeginRequest(requestContext, collectionId, RequestContextType.UserContext, true, true))
          flag = vssRequestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 1);
      }
      catch (HostDoesNotExistException ex)
      {
      }
      return flag ? flag : throw new CollectionDoesNotExistException(collectionId);
    }

    private void CheckManageCollectionPermission(
      IVssRequestContext requestContext,
      Guid collectionId)
    {
      if (requestContext.IsSystemContext)
        return;
      TeamFoundationHostManagementService service = requestContext.GetService<TeamFoundationHostManagementService>();
      try
      {
        using (IVssRequestContext vssRequestContext = service.BeginRequest(requestContext, collectionId, RequestContextType.UserContext, true, true))
        {
          IVssSecurityNamespace securityNamespace = vssRequestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId);
          if (!securityNamespace.HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 1))
            throw new CollectionDoesNotExistException(collectionId);
          if (securityNamespace.HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 2, false))
            return;
          securityNamespace.NamespaceExtension.ThrowAccessDeniedException(vssRequestContext, securityNamespace, requestContext.GetUserIdentity() ?? throw new IdentityNotFoundException(requestContext.UserContext), FrameworkSecurity.FrameworkNamespaceToken, 2);
        }
      }
      catch (HostDoesNotExistException ex)
      {
        throw new CollectionDoesNotExistException(collectionId);
      }
    }

    private static void ValidateServiceLevelsForDetach(
      ISqlConnectionInfo configDbConnectionInfo,
      ISqlConnectionInfo collectionConnectionInfo,
      string collectionName)
    {
      using (ExtendedAttributeComponent componentRaw1 = configDbConnectionInfo.CreateComponentRaw<ExtendedAttributeComponent>())
      {
        using (ExtendedAttributeComponent componentRaw2 = collectionConnectionInfo.CreateComponentRaw<ExtendedAttributeComponent>())
        {
          ServiceLevel serviceLevel1 = string.IsNullOrEmpty(componentRaw2.ReadServiceLevelToStamp()) ? new ServiceLevel(componentRaw2.ReadServiceLevelStamp()) : throw new DetachCollectionException(FrameworkResources.CollectionNotDetachableServicingFailed((object) collectionName));
          ServiceLevel serviceLevel2 = new ServiceLevel(componentRaw1.ReadServiceLevelStamp());
          if (ServiceLevel.CompareMajorVersions(serviceLevel1.MajorVersion, serviceLevel2.MajorVersion) != 0)
            throw new DetachCollectionException(FrameworkResources.DetachCollectionServiceLevelDiffer((object) collectionName, (object) serviceLevel1, (object) serviceLevel2));
          if (ServiceLevel.CompareMilestones(serviceLevel1.Milestone, serviceLevel2.Milestone) != 0)
            throw new DetachCollectionException(FrameworkResources.DetachCollectionServiceLevelDiffer((object) collectionName, (object) serviceLevel1, (object) serviceLevel2));
        }
      }
    }

    private ServicingJobData GenerateDeleteCollectionJobData(
      IVssRequestContext requestContext,
      Guid collectionId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(collectionId, "hostId");
      TeamProjectCollectionProperties collectionProperties = requestContext.GetService<ITeamProjectCollectionPropertiesService>().GetCollectionProperties(requestContext, collectionId, ServiceHostFilterFlags.None);
      ServicingJobData servicingJobData = new ServicingJobData(new string[1]
      {
        ServicingOperationConstants.Delete
      });
      servicingJobData.ServicingHostId = collectionProperties.Id;
      servicingJobData.JobTitle = FrameworkResources.DeleteCollectionJobTitle((object) collectionProperties.Id);
      servicingJobData.OperationClass = "DeleteCollection";
      servicingJobData.ServicingOptions = ServicingFlags.RequiresStoppedHost;
      servicingJobData.ServicingLocks = new TeamFoundationLockInfo[1]
      {
        new TeamFoundationLockInfo()
        {
          LockMode = TeamFoundationLockMode.Exclusive,
          LockName = "Servicing-" + collectionProperties.Id.ToString(),
          LockTimeout = -1
        }
      };
      ((IInternalTeamProjectCollectionService) this).AddCommonTokensAndItems(requestContext, servicingJobData, collectionProperties, true);
      try
      {
        servicingJobData.ServicingItems[ServicingItemConstants.ConnectionInfo] = (object) new SqlConnectionInfoWrapper(requestContext, collectionProperties.FrameworkConnectionInfo);
        DatabasePartitionComponent.GetPartitionId(collectionProperties.FrameworkConnectionInfo, collectionProperties.Id);
        servicingJobData.ServicingItems[ServicingItemConstants.CollectionDatabaseReachable] = (object) true;
      }
      catch (Exception ex)
      {
        servicingJobData.ServicingItems[ServicingItemConstants.CollectionDatabaseReachable] = (object) false;
      }
      servicingJobData.ServicingTokens.Add(ServicingTokenConstants.StopHostReason, FrameworkResources.ProjectCollectionDeleteInProgress());
      return servicingJobData;
    }
  }
}
