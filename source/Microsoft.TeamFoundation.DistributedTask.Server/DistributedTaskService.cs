// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DistributedTaskService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.FileContainer;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class DistributedTaskService : 
    IDistributedTaskService,
    IVssFrameworkService,
    IInternalDistributedTaskService
  {
    private readonly IAgentPoolSecurityProvider Security;
    private const string c_layer = "DistributedTaskService";
    private const int BufferSize = 32768;
    private static readonly TimeSpan c_slowThreshold = TimeSpan.FromSeconds(10.0);

    internal DistributedTaskService()
      : this((IAgentPoolSecurityProvider) new AutomationPoolSecurityProvider())
    {
    }

    internal DistributedTaskService(IAgentPoolSecurityProvider security) => this.Security = security;

    public Stream GetTaskDefinition(
      IVssRequestContext requestContext,
      Guid taskId,
      TaskVersion version,
      out CompressionType compressionType)
    {
      using (new MethodScope(requestContext, nameof (DistributedTaskService), nameof (GetTaskDefinition)))
        return this.GetTaskDefinitionContent(requestContext, taskId, version, (Func<TaskDefinitionData, string>) (x => x.FilePath), out compressionType);
    }

    public Stream GetTaskDefinitionIcon(
      IVssRequestContext requestContext,
      Guid taskId,
      TaskVersion version,
      out CompressionType compressionType)
    {
      using (new MethodScope(requestContext, nameof (DistributedTaskService), nameof (GetTaskDefinitionIcon)))
        return this.GetTaskDefinitionContent(requestContext, taskId, version, (Func<TaskDefinitionData, string>) (x => x.IconPath), out compressionType);
    }

    public IList<TaskDefinition> GetTaskDefinitions(
      IVssRequestContext requestContext,
      Guid? taskId = null,
      TaskVersion version = null,
      IEnumerable<string> visibility = null,
      bool scopeLocal = false,
      bool allVersions = false)
    {
      return (IList<TaskDefinition>) this.GetTaskDefinitionsInternal(requestContext, taskId, version, scopeLocal, allVersions).Get(visibility != null ? visibility.ToArray<string>() : (string[]) null, allVersions).Select<TaskDefinitionData, TaskDefinition>((Func<TaskDefinitionData, TaskDefinition>) (x => x.GetDefinition().Clone())).ToList<TaskDefinition>();
    }

    public void DeleteTaskDefinition(IVssRequestContext requestContext, Guid taskId)
    {
      using (new MethodScope(requestContext, nameof (DistributedTaskService), nameof (DeleteTaskDefinition)))
      {
        ArgumentUtility.CheckForEmptyGuid(taskId, nameof (taskId));
        this.ValidatePermissions(requestContext);
        TaskDefinitionData taskDefinitionData = this.GetTaskDefinitionsInternal(requestContext, new Guid?(taskId), scopeLocal: true).FirstOrDefault<TaskDefinitionData>();
        if (taskDefinitionData == null)
          return;
        using (TaskDefinitionComponent component = requestContext.CreateComponent<TaskDefinitionComponent>())
          component.DeleteTaskDefinition(taskId);
        this.InvalidateCache(requestContext);
        this.PublishTasksChangedEvent(requestContext, (IList<Guid>) new Guid[1]
        {
          taskDefinitionData.Id
        });
        ITeamFoundationFileContainerService service = requestContext.GetService<ITeamFoundationFileContainerService>();
        long taskContainerId = service.GetTaskContainerId(requestContext);
        string definitionFolder = TaskContainer.GetTaskDefinitionFolder(taskId);
        service.DeleteItems(requestContext.Elevate(), taskContainerId, (IList<string>) new string[1]
        {
          definitionFolder
        }, Guid.Empty);
      }
    }

    public async Task<bool> UploadTaskDefinitionAsync(
      IVssRequestContext requestContext,
      TaskContribution contribution,
      bool isChangedEventCritical = false)
    {
      DistributedTaskService distributedTaskService = this;
      using (new MethodScope(requestContext, nameof (DistributedTaskService), nameof (UploadTaskDefinitionAsync)))
      {
        bool updateContributionVersion = !string.IsNullOrEmpty(contribution.Definition.ContributionIdentifier);
        (bool UpdateTask, bool UpdateContributionVersion) tuple = distributedTaskService.CheckForDuplicateTaskDefinition(requestContext, contribution.Definition, updateContributionVersion);
        TaskDefinitionComponent taskComponent;
        if (tuple.UpdateTask)
        {
          try
          {
            distributedTaskService.ValidatePermissions(requestContext);
          }
          catch (AccessDeniedException ex)
          {
            throw new AccessDeniedException(TaskResources.UploadTaskDefinitionAccessDenied(), (Exception) ex);
          }
          requestContext.TraceAlways(10015090, TraceLevel.Info, "DistributedTask", "ContributionBuildTask", string.Format("Updating task definition {0}({1}) to version {2}. Contribution version is  {3}", (object) contribution.Definition.Id, (object) contribution.Definition.Name, (object) contribution.Definition.Version.ToString(), (object) contribution.Definition.ContributionVersion));
          string zipPath = (string) null;
          string iconPath = (string) null;
          long containerId;
          using (TaskPackageResources resources = await contribution.GetPackageResourcesAsync())
          {
            if (resources.Definition.BuildConfigMapping.Count > 0 && !DistributedTaskService.TryValidateBuildConfigMappingThrowOnInvalid(requestContext, resources.Definition))
              return false;
            ITeamFoundationFileContainerService service = requestContext.GetService<ITeamFoundationFileContainerService>();
            containerId = service.GetTaskContainerId(requestContext);
            zipPath = TaskContainer.GetTaskDefinitionResourcePath(resources.Definition, "task.zip");
            service.UploadFile(requestContext.Elevate(), containerId, zipPath, resources.PackageStream);
            if (resources.Icon != null)
            {
              iconPath = TaskContainer.GetTaskDefinitionResourcePath(resources.Definition, resources.Icon.Name);
              service.UploadFile(requestContext.Elevate(), containerId, iconPath, resources.Icon.Stream, resources.Icon.Length);
            }
            byte[] metadataDocument = JsonUtility.Serialize((object) resources.Definition);
            bool overwrite = requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && requestContext.IsServicingContext;
            taskComponent = requestContext.CreateComponent<TaskDefinitionComponent>();
            try
            {
              await taskComponent.AddTaskDefinitionAsync(resources.Definition.Id, resources.Definition.Version, resources.Definition.Name, resources.Definition.FriendlyName, metadataDocument, overwrite, resources.Definition.ContributionIdentifier, resources.Definition.ContributionVersion, contributionUpdatedOn: new DateTime?(DateTime.Now));
            }
            finally
            {
              taskComponent?.Dispose();
            }
            taskComponent = (TaskDefinitionComponent) null;
          }
          taskComponent = requestContext.CreateComponent<TaskDefinitionComponent>();
          try
          {
            await taskComponent.UpdateTaskDefinitionAsync(contribution.Definition.Id, contribution.Definition.Version, containerId, zipPath, iconPath);
          }
          finally
          {
            taskComponent?.Dispose();
          }
          taskComponent = (TaskDefinitionComponent) null;
          distributedTaskService.InvalidateCache(requestContext);
          distributedTaskService.PublishTasksChangedEvent(requestContext, (IList<Guid>) new Guid[1]
          {
            contribution.Definition.Id
          }, (isChangedEventCritical ? 1 : 0) != 0);
          return true;
        }
        if (tuple.UpdateContributionVersion)
        {
          requestContext.TraceAlways(10015088, TraceLevel.Info, "DistributedTask", "ContributionBuildTask", string.Format("Updating contribution version for task definition {0} to contribution version {1}", (object) contribution.Definition.Id, (object) contribution.Definition.ContributionVersion));
          taskComponent = requestContext.CreateComponent<TaskDefinitionComponent>();
          try
          {
            await taskComponent.UpdateContributionVersionAsync(contribution.Definition.Id, contribution.Definition.ContributionVersion);
          }
          finally
          {
            taskComponent?.Dispose();
          }
          taskComponent = (TaskDefinitionComponent) null;
          return true;
        }
        requestContext.TraceAlways(10015089, TraceLevel.Info, "DistributedTask", "ContributionBuildTask", string.Format("Not updating task definition or contribution version for task definition {0} (version = {1}). Contribution version is {2}", (object) contribution.Definition.Id, (object) contribution.Definition.Version.ToString(), (object) contribution.Definition.ContributionVersion));
        return false;
      }
    }

    internal static bool TryValidateBuildConfigMappingThrowOnInvalid(
      IVssRequestContext requestContext,
      TaskDefinition definition)
    {
      if (definition.BuildConfigMapping.Count > 100)
      {
        requestContext.TraceAlways(10015090, TraceLevel.Error, "DistributedTask", "ContributionBuildTask", string.Format("taskId={0} has has more than {1} mappings. skipping task.", (object) definition.Id, (object) 100));
        return false;
      }
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) definition.BuildConfigMapping)
      {
        if (keyValuePair.Key.Length > 100)
        {
          requestContext.TraceAlways(10015090, TraceLevel.Error, "DistributedTask", "ContributionBuildTask", string.Format("taskId={0} has a build config mapping whose key is more than {1} characters. skipping task.", (object) definition.Id, (object) 100));
          return false;
        }
        try
        {
          TaskVersion taskVersion = new TaskVersion(keyValuePair.Value);
        }
        catch (Exception ex)
        {
          requestContext.TraceAlways(10015090, TraceLevel.Error, "DistributedTask", "ContributionBuildTask", string.Format("Exception parsing supplied version for taskId=={0} x.Value=={1}: e={2}; skipping task upload", (object) definition.Id, (object) keyValuePair.Value, (object) ex));
          return false;
        }
      }
      return true;
    }

    public async Task DeleteTaskDefinitionsForExtensionAsync(
      IVssRequestContext requestContext,
      ExtensionIdentifier extensionId)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (DistributedTaskService), nameof (DeleteTaskDefinitionsForExtensionAsync));
      try
      {
        ArgumentUtility.CheckForNull<ExtensionIdentifier>(extensionId, nameof (extensionId));
        this.ValidatePermissions(requestContext);
        IList<Guid> taskIds;
        using (TaskDefinitionComponent taskComponent = requestContext.CreateComponent<TaskDefinitionComponent>())
          taskIds = await taskComponent.DeleteTaskDefinitionsForExtensionAsync(extensionId.ToString());
        this.InvalidateCache(requestContext);
        this.PublishTasksChangedEvent(requestContext, taskIds);
        ITeamFoundationFileContainerService service = requestContext.GetService<ITeamFoundationFileContainerService>();
        long taskContainerId = service.GetTaskContainerId(requestContext);
        try
        {
          foreach (Guid taskId in (IEnumerable<Guid>) taskIds)
          {
            string definitionFolder = TaskContainer.GetTaskDefinitionFolder(taskId);
            service.DeleteItems(requestContext.Elevate(), taskContainerId, (IList<string>) new string[1]
            {
              definitionFolder
            }, Guid.Empty);
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException("ResourceService", ex);
        }
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    public async Task ValidateInstallAsync(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (DistributedTaskService), nameof (ValidateInstallAsync));
      try
      {
        ArgumentUtility.CheckStringForNullOrEmpty(publisherName, nameof (publisherName));
        ArgumentUtility.CheckStringForNullOrEmpty(extensionName, nameof (extensionName));
        ArgumentUtility.CheckStringForNullOrEmpty(version, nameof (version));
        ExtensionIdentifier extensionId = new ExtensionIdentifier(publisherName, extensionName);
        using (TaskContributionPackage contributionPackage = await requestContext.GetService<ITaskContributionLoaderService>().LoadAsync(requestContext, extensionId, version))
        {
          foreach (TaskContribution task in (IEnumerable<TaskContribution>) contributionPackage.Tasks)
            this.CheckForDuplicateTaskDefinition(requestContext, task.Definition, false);
        }
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    private (bool UpdateTask, bool UpdateContributionVersion) CheckForDuplicateTaskDefinition(
      IVssRequestContext requestContext,
      TaskDefinition incomingVersion,
      bool updateContributionVersion)
    {
      if (requestContext.IsServicingContext && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return (true, false);
      using (new MethodScope(requestContext, nameof (DistributedTaskService), nameof (CheckForDuplicateTaskDefinition)))
      {
        bool flag1 = false;
        bool flag2 = false;
        bool flag3 = false;
        TaskDefinitionData taskDefinitionData1 = (TaskDefinitionData) null;
        foreach (TaskDefinitionData taskDefinitionData2 in (IEnumerable<TaskDefinitionData>) this.GetTaskDefinitionsInternal(requestContext, new Guid?(incomingVersion.Id)))
        {
          if (taskDefinitionData2.HostType == TeamFoundationHostType.Deployment)
            flag3 = true;
          if (taskDefinitionData1 == null || taskDefinitionData2.Version.CompareTo(taskDefinitionData1.Version) > 0)
            taskDefinitionData1 = taskDefinitionData2;
        }
        TaskDefinitionData taskDefinitionData3 = this.GetTaskDefinitionsInternal(requestContext, new Guid?(incomingVersion.Id), incomingVersion.Version, bypassCache: true).FirstOrDefault<TaskDefinitionData>();
        if (taskDefinitionData1 != null && !taskDefinitionData1.HostType.Equals((object) TeamFoundationHostType.Application))
        {
          string contributionIdentifier1 = taskDefinitionData1.ContributionIdentifier;
          if (contributionIdentifier1 == null)
          {
            if (incomingVersion.ContributionIdentifier != null)
            {
              if (flag3)
                throw new TaskDefinitionExistsException(TaskResources.TaskDefinitionCannotBeReplaced((object) taskDefinitionData1.Id));
              if (taskDefinitionData3 != null)
                throw new TaskDefinitionExistsException(TaskResources.TaskDefinitionAlreadyExists((object) taskDefinitionData3.Id, (object) taskDefinitionData3.Name, (object) taskDefinitionData3.Version));
              flag1 = true;
            }
            else
            {
              if (taskDefinitionData3 != null)
                throw new TaskDefinitionExistsException(TaskResources.TaskDefinitionAlreadyExists((object) taskDefinitionData3.Id, (object) taskDefinitionData3.Name, (object) taskDefinitionData3.Version));
              flag1 = true;
            }
          }
          else
          {
            ContributionIdentifier contributionIdentifier2 = new ContributionIdentifier(contributionIdentifier1);
            if (incomingVersion.ContributionIdentifier != null)
            {
              if (!string.Equals(incomingVersion.ContributionIdentifier, contributionIdentifier1, StringComparison.OrdinalIgnoreCase))
              {
                if (!updateContributionVersion)
                  throw new TaskDefinitionExistsException(TaskResources.TaskDefinitionAlreadyUploadedForExtension((object) taskDefinitionData1.Id, (object) taskDefinitionData1.Name, (object) taskDefinitionData1.Version, (object) contributionIdentifier2.ExtensionName));
                flag1 = false;
                flag2 = true;
                requestContext.TraceError(10015091, "ContributionBuildTask", string.Format("A task definition with id '{0}' and version '{1}' already exists for contribution ({2}, v{3}). An attempt was made to install the same task at version '{4}' with contribution ({5}, v{6}).", (object) taskDefinitionData1.Id, (object) taskDefinitionData1.Version, (object) contributionIdentifier1, (object) taskDefinitionData1.ContributionVersion, (object) incomingVersion.Version, (object) incomingVersion.ContributionIdentifier, (object) incomingVersion.ContributionVersion));
              }
              else
              {
                Version version1 = new Version(taskDefinitionData1.ContributionVersion);
                Version version2 = new Version(incomingVersion.ContributionVersion);
                if (version2 < version1)
                  requestContext.TraceError(10015100, "ContributionBuildTask", "Invalid attempt to update contribution " + incomingVersion.ContributionIdentifier + " from version " + taskDefinitionData1.ContributionVersion + " to " + incomingVersion.ContributionVersion + ". This indicates an error that should be investigated. Skipping...");
                else if (taskDefinitionData3 == null)
                  flag1 = true;
                else if (version2 > version1)
                {
                  flag2 = true;
                  requestContext.TraceInfo(10015099, "ContributionBuildTask", string.Format("Updating contribution info for task ({0}, v{1}) from ({2}, v{3}) to({4}, v{5}).", (object) taskDefinitionData3.Id, (object) taskDefinitionData3.Version, (object) taskDefinitionData3.ContributionIdentifier, (object) taskDefinitionData3.ContributionVersion, (object) incomingVersion.ContributionIdentifier, (object) incomingVersion.ContributionVersion));
                }
              }
            }
            else
            {
              if (taskDefinitionData3 != null)
                throw new TaskDefinitionExistsException(TaskResources.TaskDefinitionAlreadyExists((object) taskDefinitionData3.Id, (object) taskDefinitionData3.Name, (object) taskDefinitionData3.Version));
              flag1 = true;
            }
          }
        }
        else
          flag1 = true;
        return (flag1, flag2);
      }
    }

    internal async Task AddTaskDefinitionHistoryAsync(
      IVssRequestContext requestContext,
      TaskDefinitionStatus taskDefinitionStatus,
      Guid? taskId = null,
      TaskVersion version = null,
      Dictionary<string, string> contributions = null)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (DistributedTaskService), nameof (AddTaskDefinitionHistoryAsync));
      try
      {
        using (TaskDefinitionComponent taskComponent = requestContext.CreateComponent<TaskDefinitionComponent>())
          await taskComponent.AddTaskDefinitionHistoryAsync(taskDefinitionStatus, taskId, version, contributions);
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    internal IList<TaskDefinitionData> UpdateContributionInstallationAsComplete(
      IVssRequestContext requestContext,
      Dictionary<string, string> contributions)
    {
      using (new MethodScope(requestContext, nameof (DistributedTaskService), nameof (UpdateContributionInstallationAsComplete)))
      {
        IList<TaskDefinitionData> source = (IList<TaskDefinitionData>) new List<TaskDefinitionData>();
        using (TaskDefinitionComponent component = requestContext.CreateComponent<TaskDefinitionComponent>())
          source = component.UpdateContributionInstallComplete((IDictionary<string, string>) contributions);
        this.InvalidateCache(requestContext);
        this.PublishTasksChangedEvent(requestContext, (IList<Guid>) source.Select<TaskDefinitionData, Guid>((Func<TaskDefinitionData, Guid>) (c => c.Id)).ToList<Guid>());
        return source;
      }
    }

    internal async Task<IList<TaskDefinitionData>> GetTaskDefinitionsForExtensionAsync(
      IVssRequestContext requestContext,
      string extensionIdentifier)
    {
      IList<TaskDefinitionData> forExtensionAsync;
      using (new MethodScope(requestContext, nameof (DistributedTaskService), nameof (GetTaskDefinitionsForExtensionAsync)))
      {
        using (TaskDefinitionComponent taskComponent = requestContext.CreateComponent<TaskDefinitionComponent>())
          forExtensionAsync = await taskComponent.GetTaskDefinitionsForExtensionAsync(extensionIdentifier);
      }
      return forExtensionAsync;
    }

    internal TaskDefinitionDataResult GetTaskDefinitionsInternal(
      IVssRequestContext requestContext,
      Guid? taskId = null,
      TaskVersion version = null,
      bool scopeLocal = false,
      bool allVersions = false,
      bool bypassCache = false)
    {
      int num1 = requestContext.IsFeatureEnabled("DistributedTask.TaskLockdownAllowed") ? 1 : 0;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      RegistryQuery registryQuery;
      int num2;
      if (num1 != 0)
      {
        IVssRegistryService registryService = service;
        IVssRequestContext requestContext1 = requestContext;
        registryQuery = (RegistryQuery) "/Service/DistributedTask/Settings/DisableInBoxTasks";
        ref RegistryQuery local = ref registryQuery;
        num2 = registryService.GetValue<bool>(requestContext1, in local, false) ? 1 : 0;
      }
      else
        num2 = 0;
      bool flag = num2 != 0;
      int num3;
      if (num1 != 0)
      {
        IVssRegistryService registryService = service;
        IVssRequestContext requestContext2 = requestContext;
        registryQuery = (RegistryQuery) "/Service/DistributedTask/Settings/DisableMarketplaceTasks";
        ref RegistryQuery local = ref registryQuery;
        num3 = registryService.GetValue<bool>(requestContext2, in local, false) ? 1 : 0;
      }
      else
        num3 = 0;
      Func<TaskDefinitionData, bool> predicate = num3 == 0 ? (Func<TaskDefinitionData, bool>) (task => true) : (Func<TaskDefinitionData, bool>) (task => string.IsNullOrEmpty(task.ContributionIdentifier));
      using (new MethodScope(requestContext, nameof (DistributedTaskService), nameof (GetTaskDefinitionsInternal)))
      {
        TaskDefinitionDataResult definitionsInternal;
        if (version == (TaskVersion) null & allVersions | bypassCache)
        {
          definitionsInternal = new TaskDefinitionDataResult(new int?(1));
          if (!scopeLocal && !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && !flag)
          {
            IVssRequestContext deploymentContext = requestContext.To(TeamFoundationHostType.Deployment);
            using (TaskDefinitionComponent component = deploymentContext.CreateComponent<TaskDefinitionComponent>())
            {
              using (deploymentContext.TraceSlowCall(DistributedTaskService.c_slowThreshold, nameof (DistributedTaskService), "GetTaskDefinitions({0},{1},true) took more than: {2}", (object) taskId, (object) version, (object) DistributedTaskService.c_slowThreshold))
                definitionsInternal.Add(component.GetTaskDefinitions(taskId, version, allVersions).Select<TaskDefinitionData, TaskDefinitionData>((Func<TaskDefinitionData, TaskDefinitionData>) (x =>
                {
                  x.SetHostType(deploymentContext);
                  return x;
                })));
            }
          }
          if (!requestContext.ServiceHost.Equals((object) TeamFoundationHostType.Application))
          {
            using (TaskDefinitionComponent component = requestContext.CreateComponent<TaskDefinitionComponent>())
            {
              using (requestContext.TraceSlowCall(DistributedTaskService.c_slowThreshold, nameof (DistributedTaskService), "GetTaskDefinitions({0},{1},true) took more than: {2}", (object) taskId, (object) version, (object) DistributedTaskService.c_slowThreshold))
                definitionsInternal.Add(component.GetTaskDefinitions(taskId, version, allVersions).Where<TaskDefinitionData>(predicate).Select<TaskDefinitionData, TaskDefinitionData>((Func<TaskDefinitionData, TaskDefinitionData>) (x =>
                {
                  x.SetHostType(requestContext);
                  return x;
                })));
            }
          }
        }
        else
        {
          TaskDefinitionDataResult definitionDataResult = (TaskDefinitionDataResult) null;
          IVssRequestContext deploymentContext = (IVssRequestContext) null;
          if (!scopeLocal && !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && !flag)
          {
            deploymentContext = requestContext.To(TeamFoundationHostType.Deployment);
            definitionDataResult = deploymentContext.GetService<ITaskDefinitionDataCacheService>().GetTasks(deploymentContext);
          }
          TaskDefinitionDataResult tasks = requestContext.GetService<ITaskDefinitionDataCacheService>().GetTasks(requestContext);
          if (taskId.HasValue && version != (TaskVersion) null)
          {
            TaskDefinitionData definition = (definitionDataResult != null ? definitionDataResult.Get(new Guid?(taskId.Value), version).FirstOrDefault<TaskDefinitionData>() : (TaskDefinitionData) null) ?? tasks.Get(new Guid?(taskId.Value), version).Where<TaskDefinitionData>(predicate).FirstOrDefault<TaskDefinitionData>();
            if (definition == null)
            {
              if (deploymentContext != null && !flag)
              {
                using (TaskDefinitionComponent component = deploymentContext.CreateComponent<TaskDefinitionComponent>())
                {
                  definition = component.GetTaskDefinitions(taskId, version, false).Select<TaskDefinitionData, TaskDefinitionData>((Func<TaskDefinitionData, TaskDefinitionData>) (x =>
                  {
                    x.SetHostType(deploymentContext);
                    return x;
                  })).FirstOrDefault<TaskDefinitionData>();
                  if (definition != null)
                    definitionDataResult.Add(definition);
                }
              }
              if (definition == null)
              {
                using (TaskDefinitionComponent component = requestContext.CreateComponent<TaskDefinitionComponent>())
                {
                  definition = component.GetTaskDefinitions(taskId, version, false).Select<TaskDefinitionData, TaskDefinitionData>((Func<TaskDefinitionData, TaskDefinitionData>) (x =>
                  {
                    x.SetHostType(requestContext);
                    return x;
                  })).Where<TaskDefinitionData>(predicate).FirstOrDefault<TaskDefinitionData>();
                  if (definition != null)
                    tasks.Add(definition);
                }
              }
            }
            definitionsInternal = new TaskDefinitionDataResult(new int?(1));
            if (definition != null)
              definitionsInternal.Add(definition);
          }
          else
          {
            definitionsInternal = new TaskDefinitionDataResult(new int?(1));
            if (definitionDataResult != null && !flag)
              definitionsInternal.Add(definitionDataResult.Get(taskId, (TaskVersion) null));
            definitionsInternal.Add(tasks.Get(taskId, (TaskVersion) null).Where<TaskDefinitionData>(predicate));
          }
        }
        return definitionsInternal;
      }
    }

    private void ValidatePermissions(IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      this.Security.CheckPoolPermission(requestContext, 2);
    }

    private Stream GetTaskDefinitionContent(
      IVssRequestContext requestContext,
      Guid taskId,
      TaskVersion version,
      Func<TaskDefinitionData, string> getPath,
      out CompressionType compressionType)
    {
      using (new MethodScope(requestContext, nameof (DistributedTaskService), nameof (GetTaskDefinitionContent)))
      {
        ArgumentUtility.CheckForEmptyGuid(taskId, nameof (taskId));
        TaskDefinitionData taskDefinitionData = this.GetTaskDefinitionsInternal(requestContext, new Guid?(taskId), version).FirstOrDefault<TaskDefinitionData>((Func<TaskDefinitionData, bool>) (x => x.ContainerId.HasValue));
        if (taskDefinitionData == null)
          throw new TaskDefinitionNotFoundException(TaskResources.TaskDefinitionNotFound((object) taskId, (object) version.ToString()));
        string itemPath = getPath(taskDefinitionData);
        if (requestContext.ServiceHost.Is(taskDefinitionData.HostType))
          return this.GetItemContent(requestContext, taskDefinitionData.ContainerId.Value, itemPath, out compressionType);
        IVssRequestContext vssRequestContext = requestContext.To(taskDefinitionData.HostType);
        return vssRequestContext.GetService<DistributedTaskService>().GetItemContent(vssRequestContext, taskDefinitionData.ContainerId.Value, itemPath, out compressionType);
      }
    }

    private Stream GetItemContent(
      IVssRequestContext requestContext,
      long containerId,
      string itemPath,
      out CompressionType compressionType)
    {
      using (new MethodScope(requestContext, nameof (DistributedTaskService), nameof (GetItemContent)))
      {
        compressionType = CompressionType.None;
        FileContainerItem fileContainerItem = requestContext.GetService<ITeamFoundationFileContainerService>().QueryItems(requestContext.Elevate(), containerId, itemPath, Guid.Empty).FirstOrDefault<FileContainerItem>();
        if (fileContainerItem != null && fileContainerItem.FileId != 0)
          return requestContext.GetService<ITeamFoundationFileService>().RetrieveFile(requestContext.Elevate(), (long) fileContainerItem.FileId, false, out byte[] _, out long _, out compressionType);
        requestContext.TraceWarning(10015028, "DistributedTask", "Couldn't get item content in path {0} for container {2}", (object) itemPath, (object) containerId);
        return (Stream) null;
      }
    }

    private void OnTasksChanged(IVssRequestContext requestContext, NotificationEventArgs args) => this.InvalidateCache(requestContext);

    private void PublishTasksChangedEvent(
      IVssRequestContext requestContext,
      IList<Guid> taskIds,
      bool isChangedEventCritical = false)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      using (new MethodScope(requestContext, nameof (DistributedTaskService), nameof (PublishTasksChangedEvent)))
      {
        try
        {
          IVssRequestContext vssRequestContext = requestContext.Elevate();
          ServiceEvent serviceEvent = new ServiceEvent()
          {
            EventType = "MS.TF.DistributedTask.TasksChanged",
            Publisher = new Microsoft.VisualStudio.Services.WebApi.Publisher()
            {
              Name = "Tfs",
              ServiceOwnerId = ServiceInstanceTypes.TFS
            },
            Resource = (object) new TaskChangeEvent(),
            ResourceVersion = "2.0",
            ResourceContainers = this.GetResourceContainers(requestContext)
          };
          vssRequestContext.GetService<IMessageBusPublisherService>().Publish(vssRequestContext, "Microsoft.TeamFoundation.DistributedTask.Server", (object[]) new ServiceEvent[1]
          {
            serviceEvent
          });
          requestContext.TraceInfo(10015118, "DistributedTask", "Published task changed event. Task ids: {0}", taskIds == null ? (object) "In box and marketplace tasks (task lock down)" : (object) string.Join<Guid>(", ", (IEnumerable<Guid>) taskIds));
        }
        catch (Exception ex)
        {
          requestContext.TraceException(0, "TaskService", ex);
          if (!isChangedEventCritical)
            return;
          throw;
        }
      }
    }

    private Dictionary<string, object> GetResourceContainers(IVssRequestContext requestContext)
    {
      Dictionary<string, object> resourceContainers = new Dictionary<string, object>();
      IVssServiceHost organizationServiceHost = requestContext.ServiceHost.OrganizationServiceHost;
      if (organizationServiceHost != null)
        resourceContainers["Account"] = (object) organizationServiceHost.InstanceId;
      return resourceContainers;
    }

    private void InvalidateCache(IVssRequestContext requestContext) => requestContext.GetService<ITaskDefinitionDataCacheService>().Invalidate(requestContext);

    async Task IInternalDistributedTaskService.AddTaskDefinitionAsync(
      IVssRequestContext requestContext,
      TaskDefinition taskDefinition,
      bool overwrite)
    {
      DistributedTaskService distributedTaskService = this;
      MethodScope methodScope = new MethodScope(requestContext, nameof (DistributedTaskService), "AddTaskDefinitionAsync");
      try
      {
        TaskDefinitionValidation.ValidateTaskDefinition(requestContext, taskDefinition);
        distributedTaskService.Security.CheckPoolPermission(requestContext, 2);
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && distributedTaskService.GetTaskDefinitionsInternal(requestContext, new Guid?(taskDefinition.Id)).Any<TaskDefinitionData>((Func<TaskDefinitionData, bool>) (x => x.IsDeployment)))
          throw new TaskDefinitionExistsException(TaskResources.TaskDefinitionCannotBeReplaced((object) taskDefinition.Id));
        byte[] metadataDocument = JsonUtility.Serialize((object) taskDefinition);
        using (TaskDefinitionComponent taskComponent = requestContext.CreateComponent<TaskDefinitionComponent>())
          await taskComponent.AddTaskDefinitionAsync(taskDefinition.Id, taskDefinition.Version, taskDefinition.Name, taskDefinition.FriendlyName, metadataDocument, overwrite, contributionUpdatedOn: new DateTime?(DateTime.Now));
        distributedTaskService.InvalidateCache(requestContext);
        distributedTaskService.PublishTasksChangedEvent(requestContext, (IList<Guid>) new Guid[1]
        {
          taskDefinition.Id
        });
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    async Task IInternalDistributedTaskService.UploadTaskDefinitionAsync(
      IVssRequestContext requestContext,
      Guid taskId,
      TaskVersion version,
      Stream fileStream,
      Stream iconStream,
      long iconStreamLength,
      Stream helpStream,
      long helpStreamLength)
    {
      DistributedTaskService distributedTaskService = this;
      MethodScope methodScope = new MethodScope(requestContext, nameof (DistributedTaskService), "UploadTaskDefinitionAsync");
      try
      {
        ArgumentUtility.CheckForNull<Stream>(fileStream, nameof (fileStream));
        ArgumentUtility.CheckForEmptyGuid(taskId, nameof (taskId));
        distributedTaskService.Security.CheckPoolPermission(requestContext, 2);
        TaskDefinitionData taskDefinitionData = distributedTaskService.GetTaskDefinitionsInternal(requestContext, new Guid?(taskId), version, bypassCache: true).FirstOrDefault<TaskDefinitionData>();
        if (taskDefinitionData == null)
          throw new TaskDefinitionNotFoundException(TaskResources.TaskDefinitionNotFound((object) taskId, (object) version));
        if (taskDefinitionData.ContainerId.HasValue)
          throw new TaskDefinitionExistsException(TaskResources.TaskDefinitionAlreadyUploaded((object) taskId, (object) taskDefinitionData.Name, (object) version));
        if (taskDefinitionData.IsDeployment && !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || !taskDefinitionData.IsDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          throw new TaskDefinitionHostContextMismatchException(TaskResources.TaskDefinitionHostContextMismatch((object) taskDefinitionData.IsDeployment, (object) requestContext.ServiceHost.HostType));
        ITeamFoundationFileContainerService service = requestContext.GetService<ITeamFoundationFileContainerService>();
        long taskContainerId = service.GetTaskContainerId(requestContext);
        string filePath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Tasks/{0:N}/{1}/task.zip", (object) taskId, (object) version);
        FileContainerItem fileContainerItem = service.UploadFile(requestContext, taskContainerId, filePath, fileStream);
        string str = (string) null;
        if (iconStream != null)
        {
          str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Tasks/{0:N}/{1}/{2}", (object) taskId, (object) version, (object) "icon.png");
          service.UploadFile(requestContext, taskContainerId, str, iconStream, iconStreamLength);
        }
        using (TaskDefinitionComponent taskComponent = requestContext.CreateComponent<TaskDefinitionComponent>())
          await taskComponent.UpdateTaskDefinitionAsync(taskDefinitionData.Id, taskDefinitionData.Version, fileContainerItem.ContainerId, fileContainerItem.Path, str);
        distributedTaskService.InvalidateCache(requestContext);
        distributedTaskService.PublishTasksChangedEvent(requestContext, (IList<Guid>) new Guid[1]
        {
          taskId
        });
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    private void OnTaskLockDownSettingChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.TraceInfo(nameof (DistributedTaskService), "Task lock down feature flag or registry settings were changed, Invalidating cache entries. Event Data {0}", (object) changedEntries);
      this.PublishTasksChangedEvent(requestContext, (IList<Guid>) null);
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(requestContext, "DistributedTask", SqlNotificationEventIds.TasksChanged, new SqlNotificationHandler(this.OnTasksChanged), true);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      service.RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnTaskLockDownSettingChanged), new RegistryQuery("/Service/DistributedTask/Settings/DisableInBoxTasks"));
      service.RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnTaskLockDownSettingChanged), new RegistryQuery("/Service/DistributedTask/Settings/DisableMarketplaceTasks"));
      RegistryQuery filter = RegistryKeys.TaskLockDownAllowedAvailabilityState;
      service.RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnTaskLockDownSettingChanged), in filter);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
      requestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(requestContext, "DistributedTask", SqlNotificationEventIds.TasksChanged, new SqlNotificationHandler(this.OnTasksChanged), false);
      requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnTaskLockDownSettingChanged));
    }
  }
}
