// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.ElasticPoolService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.TeamFoundation.DistributedTask.Azure.Models;
using Microsoft.TeamFoundation.DistributedTask.Server;
using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  internal class ElasticPoolService : IElasticPoolService, IVssFrameworkService
  {
    public async Task<ElasticPool> AddElasticPoolAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      string poolName)
    {
      ArgumentUtility.CheckForNull<ElasticPool>(elasticPool, nameof (elasticPool));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(poolName, nameof (poolName));
      elasticPool.Validate();
      ElasticHelpers.CheckViewAndOtherPermissionsForPool(requestContext, elasticPool.PoolId, 2);
      this.UpdateTaskAgentPoolOptions(requestContext, new NullableElasticPool(elasticPool));
      VirtualMachineScaleSet vmss = await this.TagElasticPoolAsync(requestContext, elasticPool, poolName);
      elasticPool.State = ElasticPoolState.New;
      elasticPool.OfflineSince = new DateTime?(DateTime.UtcNow);
      elasticPool.OrchestrationType = vmss.OrchestrationType();
      ElasticPool elasticPool1 = elasticPool;
      VirtualMachineScaleSet scaleSet = vmss;
      int num = scaleSet != null ? (int) scaleSet.OSType(new OperatingSystemType?(elasticPool.OsType)) : (int) elasticPool.OsType;
      elasticPool1.OsType = (OperatingSystemType) num;
      this.RecordCustomerEvent(requestContext, elasticPool, nameof (AddElasticPoolAsync));
      try
      {
        elasticPool = await requestContext.UsingElasticPoolComponent<ElasticPool>((Func<ElasticPoolComponent, Task<ElasticPool>>) (component => component.AddElasticPoolAsync(elasticPool)));
      }
      catch (Exception ex1)
      {
        requestContext.TraceException(ElasticPoolService.TraceLayer, ex1);
        try
        {
          await this.UntagElasticPoolAsync(requestContext, elasticPool, vmss);
        }
        catch (Exception ex2)
        {
          requestContext.TraceException(ElasticPoolService.TraceLayer, ex2);
        }
        throw;
      }
      if (elasticPool.OrchestrationType == OrchestrationType.Uniform)
        ElasticPoolConfigurationJob.EnableJob(requestContext, 0);
      else if (elasticPool.OrchestrationType == OrchestrationType.Flexible)
        FlexibleElasticPoolConfigurationJob.EnableJob(requestContext, 0);
      ElasticPool elasticPool2 = elasticPool;
      vmss = (VirtualMachineScaleSet) null;
      return elasticPool2;
    }

    public async Task<ElasticPoolCreationResult> CreateElasticPoolAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      string poolName,
      bool authorizeAllPipelines = false,
      bool autoProvisionProjectPools = false,
      Guid? projectId = null)
    {
      IDistributedTaskResourceService resourceService = requestContext.GetService<IDistributedTaskResourceService>();
      if (!await ElasticHelpers.CanReimageVMs(requestContext, elasticPool))
        throw new ElasticPoolVmRecycleNotAllowed("Recycle after each use allowed for scale sets with ephemeral disk only.");
      ElasticPoolCreationResult elasticPoolCreationResult = new ElasticPoolCreationResult();
      try
      {
        if (projectId.HasValue && projectId.Value != Guid.Empty)
        {
          elasticPoolCreationResult.agentQueue = new TaskAgentQueue()
          {
            Name = poolName
          };
          elasticPoolCreationResult.agentQueue = resourceService.AddAgentQueue(requestContext, projectId.Value, elasticPoolCreationResult.agentQueue, authorizeAllPipelines);
          elasticPoolCreationResult.agentPool = resourceService.GetAgentPool(requestContext, elasticPoolCreationResult.agentQueue.Pool.Id);
        }
        else
        {
          elasticPoolCreationResult.agentPool = new TaskAgentPool(poolName)
          {
            AutoProvision = new bool?(autoProvisionProjectPools)
          };
          if (requestContext.IsFeatureEnabled("DistributedTask.EnableGrantOrgLevelAccessPermissionToAllPipelinesInAgentPools") & authorizeAllPipelines)
            elasticPoolCreationResult.agentPool.Properties.Add("System.AutoAuthorize", (object) true);
          elasticPoolCreationResult.agentPool = resourceService.AddAgentPool(requestContext, elasticPoolCreationResult.agentPool);
        }
        if (elasticPoolCreationResult.agentPool == null || elasticPoolCreationResult.agentPool.Id == 0)
          throw new InvalidTaskAgentPoolException(TaskResources.ElasticPoolInvalidAgentPoolExceptionMessage());
        elasticPool.PoolId = elasticPoolCreationResult.agentPool.Id;
        ElasticPoolCreationResult poolCreationResult = elasticPoolCreationResult;
        poolCreationResult.elasticPool = await this.AddElasticPoolAsync(requestContext, elasticPool, elasticPoolCreationResult.agentPool.Name);
        poolCreationResult = (ElasticPoolCreationResult) null;
        elasticPoolCreationResult.agentPool = resourceService.GetAgentPool(requestContext, elasticPoolCreationResult.agentPool.Id);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "DistributedTask", nameof (ElasticPoolService), ex);
        int num = await this.TryDeleteElasticPoolAsync(requestContext, elasticPoolCreationResult, projectId) ? 1 : 0;
        throw;
      }
      ElasticPoolCreationResult elasticPoolAsync = elasticPoolCreationResult;
      resourceService = (IDistributedTaskResourceService) null;
      elasticPoolCreationResult = (ElasticPoolCreationResult) null;
      return elasticPoolAsync;
    }

    public async Task<ElasticPool> UpdateElasticPoolAsync(
      IVssRequestContext requestContext,
      NullableElasticPool poolChanges,
      bool validateReimageOption = false)
    {
      ArgumentUtility.CheckForNull<NullableElasticPool>(poolChanges, nameof (poolChanges));
      ElasticHelpers.CheckViewAndOtherPermissionsForPool(requestContext, poolChanges.PoolId, 2);
      ElasticPool poolBeforeChanges = await requestContext.UsingElasticPoolComponent<ElasticPool>((Func<ElasticPoolComponent, Task<ElasticPool>>) (component => component.GetElasticPoolAsync(poolChanges.PoolId)));
      if (poolBeforeChanges == null)
        throw new ElasticPoolDoesNotExistException(ElasticResources.ElasticPoolDoesNotExist((object) poolChanges.PoolId));
      int? nullable1 = poolChanges.MaxCapacity;
      int maxCapacity;
      if (!nullable1.HasValue)
      {
        maxCapacity = poolBeforeChanges.MaxCapacity;
      }
      else
      {
        nullable1 = poolChanges.MaxCapacity;
        maxCapacity = nullable1.Value;
      }
      int num1 = maxCapacity;
      if (num1 < 0)
        throw new ArgumentException(ElasticResources.ElasticPoolInvalidValue((object) "MaxCapacity"));
      nullable1 = poolChanges.DesiredIdle;
      if (nullable1.HasValue)
      {
        nullable1 = poolChanges.DesiredIdle;
        int num2 = num1;
        if (!(nullable1.GetValueOrDefault() > num2 & nullable1.HasValue))
        {
          nullable1 = poolChanges.DesiredIdle;
          int num3 = 0;
          if (!(nullable1.GetValueOrDefault() < num3 & nullable1.HasValue))
            goto label_12;
        }
        throw new ArgumentException(ElasticResources.ElasticPoolInvalidValue((object) "DesiredIdle"));
      }
label_12:
      nullable1 = poolChanges.MaxSavedNodeCount;
      if (nullable1.HasValue)
      {
        nullable1 = poolChanges.MaxSavedNodeCount;
        int num4 = num1;
        if (!(nullable1.GetValueOrDefault() > num4 & nullable1.HasValue))
        {
          nullable1 = poolChanges.MaxSavedNodeCount;
          int num5 = 0;
          if (!(nullable1.GetValueOrDefault() < num5 & nullable1.HasValue))
            goto label_16;
        }
        throw new ArgumentException(ElasticResources.ElasticPoolInvalidValue((object) "MaxSavedNodeCount"));
      }
label_16:
      ElasticPool poolMergedChanges = poolBeforeChanges.Clone();
      poolMergedChanges.MergeChanges(poolChanges);
      VirtualMachineScaleSet vmss = (VirtualMachineScaleSet) null;
      try
      {
        vmss = await requestContext.GetService<IAzureVirtualMachineScaleSetResourceServiceInternal>().GetScaleSetAsync(requestContext, poolMergedChanges);
        bool flag = validateReimageOption;
        if (flag)
          flag = !await ElasticHelpers.CanReimageVMs(requestContext, poolMergedChanges, vmss);
        if (flag)
          throw new ElasticPoolVmRecycleNotAllowed("Recycle after each use allowed for scale sets with ephemeral disk only.");
        poolChanges.OrchestrationType = new OrchestrationType?(vmss.OrchestrationType());
        NullableElasticPool nullableElasticPool = poolChanges;
        VirtualMachineScaleSet scaleSet = vmss;
        OperatingSystemType? nullable2 = scaleSet != null ? new OperatingSystemType?(scaleSet.OSType()) : poolChanges.OsType;
        nullableElasticPool.OsType = nullable2;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10015200, nameof (ElasticPoolService), ex);
      }
      this.UpdateTaskAgentPoolOptions(requestContext, poolChanges);
      if (poolChanges.ServiceEndpointId.HasValue)
      {
        Guid? serviceEndpointId1 = poolChanges.ServiceEndpointId;
        Guid serviceEndpointId2 = poolBeforeChanges.ServiceEndpointId;
        if ((serviceEndpointId1.HasValue ? (serviceEndpointId1.HasValue ? (serviceEndpointId1.GetValueOrDefault() != serviceEndpointId2 ? 1 : 0) : 0) : 1) != 0)
          goto label_30;
      }
      int num6;
      if (poolChanges.AzureId == null || !(poolBeforeChanges.AzureId != poolChanges.AzureId))
      {
        OperatingSystemType? osType1 = poolChanges.OsType;
        if (osType1.HasValue)
        {
          int osType2 = (int) poolBeforeChanges.OsType;
          osType1 = poolChanges.OsType;
          int valueOrDefault = (int) osType1.GetValueOrDefault();
          num6 = !(osType2 == valueOrDefault & osType1.HasValue) ? 1 : 0;
          goto label_31;
        }
        else
        {
          num6 = 0;
          goto label_31;
        }
      }
label_30:
      num6 = 1;
label_31:
      bool vmssUpdated = num6 != 0;
      IDistributedTaskResourceService dtrs = requestContext.GetService<IDistributedTaskResourceService>();
      TaskAgentPool pool = dtrs.GetAgentPool(requestContext, poolChanges.PoolId);
      this.RecordCustomerEvent(requestContext, poolChanges, nameof (UpdateElasticPoolAsync));
      if (vmssUpdated)
        vmss = await this.TagElasticPoolAsync(requestContext, poolMergedChanges, pool.Name, vmss);
      ElasticPool poolAfterChanges = await requestContext.UsingElasticPoolComponent<ElasticPool>((Func<ElasticPoolComponent, Task<ElasticPool>>) (component => component.UpdateElasticPoolAsync(poolChanges)));
      if (vmssUpdated || poolBeforeChanges.RecycleAfterEachUse != poolAfterChanges.RecycleAfterEachUse || poolBeforeChanges.OsType == OperatingSystemType.Windows && poolBeforeChanges.AgentInteractiveUI != poolAfterChanges.AgentInteractiveUI)
      {
        await this.UpdateExtensionAsync(requestContext, poolAfterChanges, pool.Name);
        requestContext.TraceInfo(10015200, nameof (ElasticPoolService), string.Format("Switching RecycleAfterEachUse from {0} to {1}", (object) poolBeforeChanges.RecycleAfterEachUse, (object) poolAfterChanges.RecycleAfterEachUse));
        foreach (TaskAgent agent in (IEnumerable<TaskAgent>) dtrs.GetAgents(requestContext, poolAfterChanges.PoolId))
        {
          requestContext.TraceInfo(10015200, nameof (ElasticPoolService), "Marking agent " + agent.Name + " as Disabled");
          agent.Enabled = new bool?(false);
          dtrs.UpdateAgent(requestContext, poolAfterChanges.PoolId, agent);
        }
        if (vmssUpdated)
        {
          requestContext.TraceInfo(10015200, nameof (ElasticPoolService), "Switching virtual machine scale sets from " + poolBeforeChanges.AzureId + " to " + poolAfterChanges.AzureId + ". Attempting to clean up the previous one.");
          try
          {
            await requestContext.GetService<IAzureVirtualMachineScaleSetResourceServiceInternal>().SetCapacityAsync(requestContext, poolBeforeChanges, 0);
            await this.UntagElasticPoolAsync(requestContext, poolBeforeChanges);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(10015200, nameof (ElasticPoolService), ex);
          }
        }
      }
      if (poolBeforeChanges.State == ElasticPoolState.Offline)
      {
        ElasticPoolState? state = poolChanges.State;
        if (state.HasValue)
        {
          state = poolChanges.State;
          ElasticPoolState elasticPoolState = ElasticPoolState.Online;
          if (state.GetValueOrDefault() == elasticPoolState & state.HasValue)
            goto label_52;
        }
      }
      OrchestrationType? orchestrationType1 = poolBeforeChanges?.OrchestrationType;
      OrchestrationType? orchestrationType2 = poolAfterChanges?.OrchestrationType;
      if (orchestrationType1.GetValueOrDefault() == orchestrationType2.GetValueOrDefault() & orchestrationType1.HasValue == orchestrationType2.HasValue)
        goto label_56;
label_52:
      ElasticPool elasticPool1 = poolAfterChanges;
      if ((elasticPool1 != null ? (elasticPool1.OrchestrationType == OrchestrationType.Uniform ? 1 : 0) : 0) != 0)
      {
        ElasticPoolConfigurationJob.EnableJob(requestContext, 10);
      }
      else
      {
        ElasticPool elasticPool2 = poolAfterChanges;
        if ((elasticPool2 != null ? (elasticPool2.OrchestrationType == OrchestrationType.Flexible ? 1 : 0) : 0) != 0)
          FlexibleElasticPoolConfigurationJob.EnableJob(requestContext, 10);
      }
label_56:
      ElasticPool elasticPool3 = poolAfterChanges;
      poolBeforeChanges = (ElasticPool) null;
      poolMergedChanges = (ElasticPool) null;
      vmss = (VirtualMachineScaleSet) null;
      dtrs = (IDistributedTaskResourceService) null;
      pool = (TaskAgentPool) null;
      poolAfterChanges = (ElasticPool) null;
      return elasticPool3;
    }

    public async Task DeleteElasticPoolAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      bool deleteVMs)
    {
      using (requestContext.TraceScope(ElasticPoolService.TraceLayer, nameof (DeleteElasticPoolAsync)))
      {
        ElasticHelpers.CheckViewAndOtherPermissionsForPool(requestContext, elasticPool.PoolId, 2);
        this.RecordCustomerEvent(requestContext, elasticPool, nameof (DeleteElasticPoolAsync));
        await requestContext.UsingElasticPoolComponent((Func<ElasticPoolComponent, Task>) (c => c.DeleteElasticPoolAsync(elasticPool.PoolId)));
        int maxAttempts = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/ElasticPool/CleanupAttempts", 2);
        int attempts = maxAttempts;
        if (deleteVMs)
        {
          do
          {
            try
            {
              await requestContext.GetService<IAzureVirtualMachineScaleSetResourceServiceInternal>().SetCapacityAsync(requestContext, elasticPool, 0);
              attempts = 0;
            }
            catch
            {
            }
          }
          while (attempts-- > 0);
        }
        attempts = maxAttempts;
        do
        {
          try
          {
            await this.UntagElasticPoolAsync(requestContext, elasticPool);
            attempts = 0;
          }
          catch
          {
          }
        }
        while (attempts-- > 0);
        attempts = maxAttempts;
        do
        {
          try
          {
            await requestContext.GetService<IAzureVirtualMachineScaleSetResourceServiceInternal>().DeleteExtensionAsync(requestContext, elasticPool, "Microsoft.Azure.DevOps.Pipelines.Agent");
            attempts = 0;
          }
          catch (Exception ex)
          {
          }
        }
        while (attempts-- > 0);
        if (requestContext.ExecutionEnvironment.IsDevFabricDeployment)
        {
          attempts = maxAttempts;
          do
          {
            try
            {
              await requestContext.GetService<IAzureVirtualMachineScaleSetResourceServiceInternal>().DeleteExtensionAsync(requestContext, elasticPool, "DevFabricRelay");
              attempts = 0;
            }
            catch (Exception ex)
            {
            }
          }
          while (attempts-- > 0);
        }
      }
    }

    public async Task<ElasticPool> GetElasticPoolAsync(
      IVssRequestContext requestContext,
      int poolId)
    {
      ElasticPool elasticPoolAsync;
      using (requestContext.TraceScope(ElasticPoolService.TraceLayer, nameof (GetElasticPoolAsync)))
      {
        ElasticHelpers.CheckViewAndOtherPermissionsForPool(requestContext, poolId);
        ElasticPool elasticPool = await requestContext.UsingElasticPoolComponent<ElasticPool>((Func<ElasticPoolComponent, Task<ElasticPool>>) (c => c.GetElasticPoolAsync(poolId)));
        if (elasticPool != null)
          requestContext.TraceInfo(10015200, nameof (ElasticPoolService), "Get ElasticPool {0} {1} SingleUse:{2}", (object) poolId, (object) elasticPool.AzureId, (object) elasticPool.RecycleAfterEachUse);
        else
          requestContext.TraceInfo(10015200, nameof (ElasticPoolService), "Get ElasticPool {0} returned null", (object) poolId);
        elasticPoolAsync = elasticPool;
      }
      return elasticPoolAsync;
    }

    public async Task<IReadOnlyList<ElasticPool>> GetElasticPoolsAsync(
      IVssRequestContext requestContext)
    {
      IReadOnlyList<ElasticPool> list;
      using (requestContext.TraceScope(ElasticPoolService.TraceLayer, nameof (GetElasticPoolsAsync)))
      {
        IReadOnlyList<ElasticPool> source = await requestContext.UsingElasticPoolComponent<IReadOnlyList<ElasticPool>>((Func<ElasticPoolComponent, Task<IReadOnlyList<ElasticPool>>>) (c => c.GetElasticPoolsAsync()));
        requestContext = requestContext.ToPoolRequestContext();
        IInternalDistributedTaskResourceService dtrs = requestContext.GetService<IInternalDistributedTaskResourceService>();
        Func<ElasticPool, bool> predicate = (Func<ElasticPool, bool>) (ep => dtrs.GetAgentPoolSecurity(requestContext, ep.PoolId).HasPoolPermission(requestContext, ep.PoolId, 1, true));
        list = (IReadOnlyList<ElasticPool>) source.Where<ElasticPool>(predicate).ToList<ElasticPool>();
      }
      return list;
    }

    public async Task<IReadOnlyList<ElasticPool>> GetElasticPoolsByTypeAsync(
      IVssRequestContext requestContext,
      OrchestrationType type)
    {
      IReadOnlyList<ElasticPool> list;
      using (requestContext.TraceScope(ElasticPoolService.TraceLayer, nameof (GetElasticPoolsByTypeAsync)))
      {
        IReadOnlyList<ElasticPool> source = await requestContext.UsingElasticPoolComponent<IReadOnlyList<ElasticPool>>((Func<ElasticPoolComponent, Task<IReadOnlyList<ElasticPool>>>) (c => c.GetElasticPoolsByTypeAsync(type)));
        requestContext = requestContext.ToPoolRequestContext();
        IInternalDistributedTaskResourceService dtrs = requestContext.GetService<IInternalDistributedTaskResourceService>();
        Func<ElasticPool, bool> predicate = (Func<ElasticPool, bool>) (ep =>
        {
          IAgentPoolSecurityProvider agentPoolSecurity = dtrs.GetAgentPoolSecurity(requestContext, ep.PoolId);
          return agentPoolSecurity != null && agentPoolSecurity.HasPoolPermission(requestContext, ep.PoolId, 1, true);
        });
        list = (IReadOnlyList<ElasticPool>) source.Where<ElasticPool>(predicate).ToList<ElasticPool>();
      }
      return list;
    }

    public async Task UpdateExtensionAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      string poolName)
    {
      string agentPoolToken = this.GenerateAgentPoolToken(requestContext, elasticPool);
      await requestContext.GetService<IAzureVirtualMachineScaleSetResourceServiceInternal>().InstallAgentExtensionAsync(requestContext, elasticPool, poolName, agentPoolToken);
    }

    public async Task UpdateElasticPoolTimeStampTagAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      VirtualMachineScaleSet scaleSet)
    {
      scaleSet.Tags["__AzureDevOpsElasticPoolTimeStamp"] = DateTime.UtcNow.ToString();
      VirtualMachineScaleSet virtualMachineScaleSet = await requestContext.GetService<IAzureVirtualMachineScaleSetResourceServiceInternal>().UpdateScaleSetAsync(requestContext, elasticPool, scaleSet);
    }

    public async Task<VirtualMachineScaleSet> TagElasticPoolAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      string poolName,
      VirtualMachineScaleSet scaleSet = null)
    {
      IAzureVirtualMachineScaleSetResourceServiceInternal ssrs = requestContext.GetService<IAzureVirtualMachineScaleSetResourceServiceInternal>();
      if (scaleSet == null)
        scaleSet = await ssrs.GetScaleSetAsync(requestContext, elasticPool);
      if (string.IsNullOrEmpty(poolName))
        poolName = requestContext.GetService<IDistributedTaskResourceService>().GetAgentPool(requestContext, elasticPool.PoolId).Name;
      if (scaleSet.Tags.ContainsKey("__AzureDevOpsElasticPool") && this.IsVmssTimestampTagValid(scaleSet.Tags["__AzureDevOpsElasticPoolTimeStamp"]))
        throw new VirtualMachineScaleSetAlreadyTaggedForUseException(poolName, scaleSet.Tags["__AzureDevOpsElasticPool"]);
      scaleSet.Tags.Remove("__AzureDevOpsElasticPool");
      scaleSet.Tags.Remove("__AzureDevOpsElasticPoolTimeStamp");
      scaleSet.Tags.Add("__AzureDevOpsElasticPool", poolName);
      scaleSet.Tags.Add("__AzureDevOpsElasticPoolTimeStamp", DateTime.UtcNow.ToString());
      VirtualMachineScaleSet virtualMachineScaleSet = await ssrs.UpdateScaleSetAsync(requestContext, elasticPool, scaleSet);
      ssrs = (IAzureVirtualMachineScaleSetResourceServiceInternal) null;
      return virtualMachineScaleSet;
    }

    private TaskAgentPool UpdateTaskAgentPoolOptions(
      IVssRequestContext requestContext,
      NullableElasticPool elasticPool)
    {
      using (requestContext.TraceScope(ElasticPoolService.TraceLayer, nameof (UpdateTaskAgentPoolOptions)))
      {
        IDistributedTaskResourceService service = requestContext.GetService<IDistributedTaskResourceService>();
        TaskAgentPool agentPool = service.GetAgentPool(requestContext, elasticPool.PoolId);
        if (agentPool == null)
        {
          requestContext.TraceInfo(10015200, nameof (ElasticPoolService), string.Format("Unable to get agentPool for elasticPool with id {0}", (object) elasticPool.PoolId));
          return (TaskAgentPool) null;
        }
        TaskAgentPoolOptions agentPoolOptions = agentPool.Options.GetValueOrDefault() | TaskAgentPoolOptions.ElasticPool;
        if (elasticPool.MaxSavedNodeCount.HasValue)
        {
          agentPoolOptions &= ~TaskAgentPoolOptions.PreserveAgentOnJobFailure;
          if (elasticPool.MaxSavedNodeCount.Value > 0)
            agentPoolOptions |= TaskAgentPoolOptions.PreserveAgentOnJobFailure;
        }
        if (elasticPool.RecycleAfterEachUse.HasValue)
        {
          agentPoolOptions &= ~TaskAgentPoolOptions.SingleUseAgents;
          if (elasticPool.RecycleAfterEachUse.Value)
            agentPoolOptions |= TaskAgentPoolOptions.SingleUseAgents;
        }
        IDistributedTaskResourceService taskResourceService = service;
        IVssRequestContext requestContext1 = requestContext;
        int poolId = elasticPool.PoolId;
        TaskAgentPool pool = new TaskAgentPool(agentPool.Name);
        pool.Id = elasticPool.PoolId;
        pool.Options = new TaskAgentPoolOptions?(agentPoolOptions);
        return taskResourceService.UpdateAgentPool(requestContext1, poolId, pool);
      }
    }

    private string GenerateAgentPoolToken(IVssRequestContext requestContext, ElasticPool pool)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (GenerateAgentPoolToken)))
      {
        requestContext = requestContext.ToPoolRequestContext();
        TaskAgentPoolData agentPool;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          agentPool = component.GetAgentPool(pool.PoolId);
        IVssRequestContext context = requestContext.Elevate();
        Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IInternalDistributedTaskResourceService>().ProvisionServiceIdentity(requestContext, agentPool, AgentPoolServiceAccountRoles.AgentPoolAdmin);
        string poolManageScope = DistributedTaskScopeHelper.GeneratePoolManageScope(pool.PoolId);
        string readIdentityRefs = DistributedTaskScopes.ReadIdentityRefs;
        string connectLocationService = DistributedTaskScopes.ConnectLocationService;
        string str1 = string.Join(" ", new string[4]
        {
          DistributedTaskScopes.FrameworkGlobalSecurity,
          connectLocationService,
          poolManageScope,
          readIdentityRefs
        });
        requestContext.TraceAlways(10015200, TraceLevel.Info, "DistributedTask", "ElasticPools", string.Format("PoolId:{0} Generating token with scopes: {1}", (object) pool.PoolId, (object) str1));
        IDelegatedAuthorizationService service = context.GetService<IDelegatedAuthorizationService>();
        IVssRequestContext requestContext1 = context;
        Guid? nullable1 = new Guid?(identity.Id);
        string displayName = identity.DisplayName;
        DateTime? nullable2 = new DateTime?(DateTime.UtcNow.Add(TimeSpan.FromHours(49.0)));
        string str2 = str1;
        Guid? clientId = new Guid?();
        Guid? userId = nullable1;
        string name = displayName;
        DateTime? validTo = nullable2;
        string scope = str2;
        Guid? authorizationId = new Guid?();
        Guid? accessId = new Guid?();
        SessionTokenResult sessionTokenResult = service.IssueSessionToken(requestContext1, clientId, userId, name, validTo, scope, authorizationId: authorizationId, accessId: accessId);
        string token = sessionTokenResult?.SessionToken?.Token;
        return !string.IsNullOrEmpty(token) ? token : throw new FailedToIssueAccessTokenException(string.Format("Unable to generate a token for service identity {0} ({1}). Error {2}", (object) identity.DisplayName, (object) identity.Id, (object) sessionTokenResult?.SessionTokenError));
      }
    }

    private async Task<bool> TryDeleteElasticPoolAsync(
      IVssRequestContext requestContext,
      ElasticPoolCreationResult elasticPoolCreationResult,
      Guid? projectId = null)
    {
      IDistributedTaskResourceService resourceService = requestContext.GetService<IDistributedTaskResourceService>();
      try
      {
        if (elasticPoolCreationResult.elasticPool != null)
          await this.DeleteElasticPoolAsync(requestContext, elasticPoolCreationResult.elasticPool, false);
        if (elasticPoolCreationResult.agentQueue != null && elasticPoolCreationResult.agentQueue.Id != 0 && projectId.HasValue)
          resourceService.DeleteAgentQueue(requestContext, projectId.Value, elasticPoolCreationResult.agentQueue.Id);
        if (elasticPoolCreationResult.agentPool != null && elasticPoolCreationResult.agentPool.Id != 0)
          resourceService.DeleteAgentPool(requestContext, elasticPoolCreationResult.agentPool.Id);
        return true;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "DistributedTask", nameof (ElasticPoolService), ex);
        return false;
      }
    }

    private bool IsVmssTimestampTagValid(string timeStampUTC) => DateTime.UtcNow.Subtract(Convert.ToDateTime(timeStampUTC)) < new TimeSpan(0, 240, 0);

    private async Task UntagElasticPoolAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool)
    {
      ArgumentUtility.CheckForNull<ElasticPool>(elasticPool, nameof (elasticPool));
      VirtualMachineScaleSet scaleSetAsync = await requestContext.GetService<IAzureVirtualMachineScaleSetResourceServiceInternal>().GetScaleSetAsync(requestContext, elasticPool);
      if (!scaleSetAsync.Tags.ContainsKey("__AzureDevOpsElasticPool"))
        return;
      await this.UntagElasticPoolAsync(requestContext, elasticPool, scaleSetAsync);
    }

    private async Task UntagElasticPoolAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      VirtualMachineScaleSet scaleSet)
    {
      if (scaleSet.Tags == null)
        return;
      scaleSet.Tags.Remove("__AzureDevOpsElasticPool");
      scaleSet.Tags.Remove("__AzureDevOpsElasticPoolTimeStamp");
      VirtualMachineScaleSet virtualMachineScaleSet = await requestContext.GetService<IAzureVirtualMachineScaleSetResourceServiceInternal>().UpdateScaleSetAsync(requestContext, elasticPool, scaleSet);
    }

    private void RecordCustomerEvent(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      string eventType)
    {
      this.RecordCustomerEvent(requestContext, new NullableElasticPool(elasticPool), eventType);
    }

    private void RecordCustomerEvent(
      IVssRequestContext requestContext,
      NullableElasticPool elasticPool,
      string eventType)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("EventType", eventType);
      properties.Add("PoolId", (double) elasticPool.PoolId);
      properties.Add("OSType", (object) (OperatingSystemType?) elasticPool?.OsType);
      properties.Add("OrchestrationType", (object) (OrchestrationType?) elasticPool?.OrchestrationType);
      properties.Add("MaxCapacity", (object) (int?) elasticPool?.MaxCapacity);
      properties.Add("DesiredIdle", (object) (int?) elasticPool?.DesiredIdle);
      properties.Add("RecycleAfterEachUse", (object) (bool?) elasticPool?.RecycleAfterEachUse);
      properties.Add("MaxSavedNodeCount", (object) (int?) elasticPool?.MaxSavedNodeCount);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "DistributedTask", "ElasticPools", properties);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    private static string TraceLayer => nameof (ElasticPoolService);
  }
}
