// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.AzureVirtualMachineScaleSetResourceService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.DistributedTask.Azure;
using Microsoft.TeamFoundation.DistributedTask.Azure.Models;
using Microsoft.TeamFoundation.DistributedTask.AzureSdk;
using Microsoft.TeamFoundation.DistributedTask.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.DevFabricRelay;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  internal class AzureVirtualMachineScaleSetResourceService : 
    ResourceServiceBase<IAzureVirtualMachineScaleSetClient>,
    IAzureVirtualMachineScaleSetResourceServiceInternal,
    IAzureVirtualMachineScaleSetResourceService,
    IVssFrameworkService
  {
    public async Task<VirtualMachineScaleSet> GetScaleSetAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool)
    {
      VirtualMachineScaleSet async = await this.Client(requestContext, elasticPool.ServiceEndpointId, elasticPool.ServiceEndpointScope).GetAsync(requestContext.ActivityId, elasticPool.AzureId);
      if (async.Tags == null)
        async.Tags = (IDictionary<string, string>) new Dictionary<string, string>();
      return async;
    }

    public async Task<VirtualMachineScaleSet> UpdateScaleSetAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      VirtualMachineScaleSet scaleSet)
    {
      return await this.Client(requestContext, elasticPool.ServiceEndpointId, elasticPool.ServiceEndpointScope).UpdateAsync(requestContext.ActivityId, elasticPool.AzureId, scaleSet);
    }

    public async Task SetCapacityAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      int newCapacity)
    {
      IAzureVirtualMachineScaleSetClient machineScaleSetClient = this.Client(requestContext, elasticPool.ServiceEndpointId, elasticPool.ServiceEndpointScope);
      Guid activityId = requestContext.ActivityId;
      string azureId = elasticPool.AzureId;
      VirtualMachineScaleSet scaleSet = new VirtualMachineScaleSet();
      scaleSet.Sku = new Sku() { Capacity = newCapacity };
      CancellationToken cancellationToken = new CancellationToken();
      VirtualMachineScaleSet virtualMachineScaleSet = await machineScaleSetClient.UpdateAsync(activityId, azureId, scaleSet, cancellationToken);
    }

    public async Task<IReadOnlyList<VirtualMachineScaleSet>> ListScaleSetsInSubscriptionAsync(
      IVssRequestContext requestContext,
      Guid serviceEndpointId,
      Guid serviceEndpointScope)
    {
      ArgumentUtility.CheckForEmptyGuid(serviceEndpointId, nameof (serviceEndpointId));
      ArgumentUtility.CheckForEmptyGuid(serviceEndpointScope, nameof (serviceEndpointScope));
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpointAsync = await requestContext.GetService<IServiceEndpointService2>().GetServiceEndpointAsync(requestContext, serviceEndpointScope, serviceEndpointId);
      if (serviceEndpointAsync == null)
        throw new ServiceEndpointDoesNotExistException(serviceEndpointId, serviceEndpointScope);
      Guid subscriptionId = serviceEndpointAsync.Data.ContainsKey("subscriptionId") && !string.IsNullOrEmpty(serviceEndpointAsync.Data["subscriptionId"]) ? new Guid(serviceEndpointAsync.Data["subscriptionId"]) : throw new InvalidServiceEndpointException(string.Format("Service endpoint {0} does not contain an Azure Subscription Id", (object) serviceEndpointAsync.Name));
      return (IReadOnlyList<VirtualMachineScaleSet>) (await this.Client(requestContext, serviceEndpointId, serviceEndpointScope).ListAsync(requestContext.ActivityId, subscriptionId)).ToArray<VirtualMachineScaleSet>();
    }

    public async Task<IReadOnlyList<VirtualMachineScaleSetVM>> ListVMInstancesAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool)
    {
      return (IReadOnlyList<VirtualMachineScaleSetVM>) (await this.Client(requestContext, elasticPool.ServiceEndpointId, elasticPool.ServiceEndpointScope).ListVMInstancesAsync(requestContext.ActivityId, elasticPool.AzureId)).ToArray<VirtualMachineScaleSetVM>();
    }

    public async Task<IReadOnlyList<VirtualMachineScaleSetVM>> ListVMsAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool)
    {
      return (IReadOnlyList<VirtualMachineScaleSetVM>) (await this.Client(requestContext, elasticPool.ServiceEndpointId, elasticPool.ServiceEndpointScope).ListVMsAsync(requestContext.ActivityId, elasticPool.AzureId)).ToArray<VirtualMachineScaleSetVM>();
    }

    public async Task DeleteMachinesAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      IEnumerable<string> ids,
      bool force)
    {
      IAzureVirtualMachineScaleSetClient machineScaleSetClient = this.Client(requestContext, elasticPool.ServiceEndpointId, elasticPool.ServiceEndpointScope);
      MachineInstances machineInstances = new MachineInstances();
      machineInstances.InstanceIds = ids.ToArray<string>();
      Guid activityId = requestContext.ActivityId;
      string azureId = elasticPool.AzureId;
      MachineInstances instances = machineInstances;
      int num = force ? 1 : 0;
      CancellationToken cancellationToken = new CancellationToken();
      await machineScaleSetClient.DeleteInstancesAsync(activityId, azureId, instances, num != 0, cancellationToken);
    }

    public async Task InstallAgentExtensionAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      string poolName,
      string token)
    {
      string accountUrl = requestContext.GetService<ILocationService>().GetAccessMapping(requestContext, "PublicAccessMapping").AccessPoint;
      Dictionary<string, string> packageDownloadUrls = requestContext.GetService<IPackageMetadataService>().GetLatestPackageDownloadUrls(requestContext, TaskAgentConstants.AgentPackageType);
      string windowsAgentUrl = packageDownloadUrls[TaskAgentConstants.CoreV2WindowsPlatformName];
      string linuxAgentUrl = packageDownloadUrls[TaskAgentConstants.CoreV2LinuxPlatformName];
      IVssRegistryService registry = requestContext.GetService<IVssRegistryService>();
      StringBuilder sb = new StringBuilder();
      IAzureVirtualMachineScaleSetClient client = this.Client(requestContext, elasticPool.ServiceEndpointId, elasticPool.ServiceEndpointScope);
      if (requestContext.ExecutionEnvironment.IsDevFabricDeployment)
      {
        IVssRequestContext vssRequestContext = requestContext.Elevate().To(TeamFoundationHostType.Deployment);
        IDevFabricRelayService service = vssRequestContext.GetService<IDevFabricRelayService>();
        if (service.Settings.Enabled)
        {
          VirtualMachineScaleSetExtension extension1 = new VirtualMachineScaleSetExtension()
          {
            Name = "DevFabricRelay"
          };
          List<string> stringList = new List<string>();
          if (elasticPool.OsType == OperatingSystemType.Windows)
          {
            DevFabricRelayInstallData windowsInstallData = service.GetWindowsInstallData(vssRequestContext);
            stringList.Add(windowsInstallData.InstallScriptUri);
            stringList.Add(windowsInstallData.PackageUri);
            extension1.Properties = new VirtualMachineScaleSetExtensionProperties()
            {
              Publisher = "Microsoft.Compute",
              Type = "CustomScriptExtension",
              TypeHandlerVersion = "1.9",
              AutoUpgradeMinorVersion = false,
              Settings = (ExtensionSettings) new CustomScriptExtensionSettings(),
              ProtectedSettings = (ExtensionSettings) new CustomScriptExtensionSettings()
              {
                FileUris = stringList.ToArray(),
                CommandToExecute = windowsInstallData.InstallCommand
              }
            };
          }
          else
          {
            DevFabricRelayInstallData linuxInstallData = service.GetLinuxInstallData(vssRequestContext);
            stringList.Add(linuxInstallData.InstallScriptUri);
            stringList.Add(linuxInstallData.PackageUri);
            extension1.Properties = new VirtualMachineScaleSetExtensionProperties()
            {
              Publisher = "Microsoft.Azure.Extensions",
              Type = "CustomScript",
              TypeHandlerVersion = "2.0",
              AutoUpgradeMinorVersion = false,
              Settings = (ExtensionSettings) new CustomScriptExtensionSettings(),
              ProtectedSettings = (ExtensionSettings) new CustomScriptExtensionSettings()
              {
                FileUris = stringList.ToArray(),
                CommandToExecute = linuxInstallData.InstallCommand
              }
            };
          }
          try
          {
            await client.CreateExtensionAsync(requestContext.ActivityId, elasticPool.AzureId, extension1);
          }
          catch (Exception ex)
          {
            sb.AppendLine("Exception creating DevFabricRelay: " + ex.Message);
          }
        }
      }
      IVssRegistryService registryService1 = registry;
      IVssRequestContext requestContext1 = requestContext;
      RegistryQuery registryQuery = (RegistryQuery) "/Service/DistributedTask/ElasticPool/ExtensionPublisher";
      ref RegistryQuery local1 = ref registryQuery;
      string str1 = registryService1.GetValue<string>(requestContext1, in local1, "Microsoft.VisualStudio.Services");
      IVssRegistryService registryService2 = registry;
      IVssRequestContext requestContext2 = requestContext;
      registryQuery = (RegistryQuery) string.Format("/Service/DistributedTask/ElasticPool/ExtensionPublisher/{0}", (object) elasticPool.PoolId);
      ref RegistryQuery local2 = ref registryQuery;
      string defaultValue1 = str1;
      string str2 = registryService2.GetValue<string>(requestContext2, in local2, defaultValue1);
      string str3;
      string str4;
      string format;
      string str5;
      string str6;
      string str7;
      string str8;
      string str9;
      if (elasticPool.OsType == OperatingSystemType.Windows)
      {
        IVssRegistryService registryService3 = registry;
        IVssRequestContext requestContext3 = requestContext;
        registryQuery = (RegistryQuery) "/Service/DistributedTask/ElasticPool/WindowsExtensionVersion";
        ref RegistryQuery local3 = ref registryQuery;
        string str10 = registryService3.GetValue<string>(requestContext3, in local3, "1.31");
        IVssRegistryService registryService4 = registry;
        IVssRequestContext requestContext4 = requestContext;
        registryQuery = (RegistryQuery) "/Service/DistributedTask/ElasticPool/WindowsScriptVersion";
        ref RegistryQuery local4 = ref registryQuery;
        string str11 = registryService4.GetValue<string>(requestContext4, in local4, "17");
        IVssRegistryService registryService5 = registry;
        IVssRequestContext requestContext5 = requestContext;
        registryQuery = (RegistryQuery) "/Service/DistributedTask/ElasticPool/WindowsScriptUrl";
        ref RegistryQuery local5 = ref registryQuery;
        string str12 = registryService5.GetValue<string>(requestContext5, in local5, "https://vstsagenttools.blob.core.windows.net/tools/ElasticPools/Windows/{0}/enableagent.ps1");
        IVssRegistryService registryService6 = registry;
        IVssRequestContext requestContext6 = requestContext;
        registryQuery = (RegistryQuery) "/Service/DistributedTask/ElasticPool/WindowsAgentUrl";
        ref RegistryQuery local6 = ref registryQuery;
        string defaultValue2 = windowsAgentUrl;
        string str13 = registryService6.GetValue<string>(requestContext6, in local6, defaultValue2);
        IVssRegistryService registryService7 = registry;
        IVssRequestContext requestContext7 = requestContext;
        registryQuery = (RegistryQuery) string.Format("/Service/DistributedTask/ElasticPool/WindowsExtensionVersion/{0}", (object) elasticPool.PoolId);
        ref RegistryQuery local7 = ref registryQuery;
        string defaultValue3 = str10;
        str3 = registryService7.GetValue<string>(requestContext7, in local7, defaultValue3);
        IVssRegistryService registryService8 = registry;
        IVssRequestContext requestContext8 = requestContext;
        registryQuery = (RegistryQuery) string.Format("/Service/DistributedTask/ElasticPool/WindowsScriptVersion/{0}", (object) elasticPool.PoolId);
        ref RegistryQuery local8 = ref registryQuery;
        string defaultValue4 = str11;
        str4 = registryService8.GetValue<string>(requestContext8, in local8, defaultValue4);
        IVssRegistryService registryService9 = registry;
        IVssRequestContext requestContext9 = requestContext;
        registryQuery = (RegistryQuery) string.Format("/Service/DistributedTask/ElasticPool/WindowsScriptUrl/{0}", (object) elasticPool.PoolId);
        ref RegistryQuery local9 = ref registryQuery;
        string defaultValue5 = str12;
        format = registryService9.GetValue<string>(requestContext9, in local9, defaultValue5);
        IVssRegistryService registryService10 = registry;
        IVssRequestContext requestContext10 = requestContext;
        registryQuery = (RegistryQuery) string.Format("/Service/DistributedTask/ElasticPool/WindowsAgentUrl/{0}", (object) elasticPool.PoolId);
        ref RegistryQuery local10 = ref registryQuery;
        string defaultValue6 = str13;
        str5 = registryService10.GetValue<string>(requestContext10, in local10, defaultValue6);
        str6 = elasticPool.RecycleAfterEachUse ? "--once" : "''";
        str7 = "TeamServicesAgent";
        str8 = "C:\\agent";
        str9 = "-url " + accountUrl + " -pool '" + poolName + "' -token " + token + " -runArgs " + str6 + " " + (elasticPool.AgentInteractiveUI ? "-interactive" : (string) null);
      }
      else
      {
        IVssRegistryService registryService11 = registry;
        IVssRequestContext requestContext11 = requestContext;
        registryQuery = (RegistryQuery) "/Service/DistributedTask/ElasticPool/LinuxExtensionVersion";
        ref RegistryQuery local11 = ref registryQuery;
        string str14 = registryService11.GetValue<string>(requestContext11, in local11, "1.23");
        IVssRegistryService registryService12 = registry;
        IVssRequestContext requestContext12 = requestContext;
        registryQuery = (RegistryQuery) "/Service/DistributedTask/ElasticPool/LinuxScriptVersion";
        ref RegistryQuery local12 = ref registryQuery;
        string str15 = registryService12.GetValue<string>(requestContext12, in local12, "15");
        IVssRegistryService registryService13 = registry;
        IVssRequestContext requestContext13 = requestContext;
        registryQuery = (RegistryQuery) "/Service/DistributedTask/ElasticPool/LinuxScriptUrl";
        ref RegistryQuery local13 = ref registryQuery;
        string str16 = registryService13.GetValue<string>(requestContext13, in local13, "https://vstsagenttools.blob.core.windows.net/tools/ElasticPools/Linux/{0}/enableagent.sh");
        IVssRegistryService registryService14 = registry;
        IVssRequestContext requestContext14 = requestContext;
        registryQuery = (RegistryQuery) "/Service/DistributedTask/ElasticPool/LinuxAgentUrl";
        ref RegistryQuery local14 = ref registryQuery;
        string defaultValue7 = linuxAgentUrl;
        string str17 = registryService14.GetValue<string>(requestContext14, in local14, defaultValue7);
        IVssRegistryService registryService15 = registry;
        IVssRequestContext requestContext15 = requestContext;
        registryQuery = (RegistryQuery) string.Format("/Service/DistributedTask/ElasticPool/LinuxExtensionVersion/{0}", (object) elasticPool.PoolId);
        ref RegistryQuery local15 = ref registryQuery;
        string defaultValue8 = str14;
        str3 = registryService15.GetValue<string>(requestContext15, in local15, defaultValue8);
        IVssRegistryService registryService16 = registry;
        IVssRequestContext requestContext16 = requestContext;
        registryQuery = (RegistryQuery) string.Format("/Service/DistributedTask/ElasticPool/LinuxScriptVersion/{0}", (object) elasticPool.PoolId);
        ref RegistryQuery local16 = ref registryQuery;
        string defaultValue9 = str15;
        str4 = registryService16.GetValue<string>(requestContext16, in local16, defaultValue9);
        IVssRegistryService registryService17 = registry;
        IVssRequestContext requestContext17 = requestContext;
        registryQuery = (RegistryQuery) string.Format("/Service/DistributedTask/ElasticPool/LinuxScriptUrl/{0}", (object) elasticPool.PoolId);
        ref RegistryQuery local17 = ref registryQuery;
        string defaultValue10 = str16;
        format = registryService17.GetValue<string>(requestContext17, in local17, defaultValue10);
        IVssRegistryService registryService18 = registry;
        IVssRequestContext requestContext18 = requestContext;
        registryQuery = (RegistryQuery) string.Format("/Service/DistributedTask/ElasticPool/LinuxAgentUrl/{0}", (object) elasticPool.PoolId);
        ref RegistryQuery local18 = ref registryQuery;
        string defaultValue11 = str17;
        str5 = registryService18.GetValue<string>(requestContext18, in local18, defaultValue11);
        str6 = elasticPool.RecycleAfterEachUse ? "--once" : (string) null;
        str7 = "TeamServicesAgentLinux";
        str8 = "/agent";
        str9 = accountUrl + " '" + poolName + "' " + token + " " + str6;
      }
      VirtualMachineScaleSetExtension extension = new VirtualMachineScaleSetExtension()
      {
        Name = "Microsoft.Azure.DevOps.Pipelines.Agent",
        Properties = new VirtualMachineScaleSetExtensionProperties()
        {
          Publisher = str2,
          Type = str7,
          TypeHandlerVersion = str3,
          AutoUpgradeMinorVersion = false,
          Settings = (ExtensionSettings) new PipelinesExtensionSettings()
          {
            IsPipelinesAgent = true,
            AgentDownloadUrl = str5,
            AgentFolder = str8,
            EnableScriptDownloadUrl = string.Format(format, (object) str4)
          },
          ProtectedSettings = (ExtensionSettings) new PipelinesExtensionSettings()
          {
            EnableScriptParameters = str9
          }
        }
      };
      PipelinesExtensionSettings settings = (PipelinesExtensionSettings) extension.Properties.Settings;
      sb.AppendLine(string.Format("PipelinesExtensionSettings Url:{0}, PoolName:{1}, AgentUrl:{2}, Folder:{3}, EnableUrl:{4}, RunArgs:{5}, InteractiveUI:{6}", (object) accountUrl, (object) poolName, (object) settings.AgentDownloadUrl, (object) settings.AgentFolder, (object) settings.EnableScriptDownloadUrl, (object) str6, (object) elasticPool.AgentInteractiveUI));
      foreach (VirtualMachineScaleSetExtension scaleSetExtension in await client.ListExtensionsAsync(requestContext.ActivityId, elasticPool.AzureId))
      {
        if ((string.Equals(scaleSetExtension?.Properties?.Type, "CustomScript", StringComparison.OrdinalIgnoreCase) || string.Equals(scaleSetExtension?.Properties?.Type, "CustomScriptExtension", StringComparison.OrdinalIgnoreCase)) && !string.IsNullOrEmpty(scaleSetExtension?.Name))
        {
          sb.AppendLine("Found CustomScriptExtension: " + scaleSetExtension.Name);
          extension.Properties.ProvisionAfterExtensions = new string[1]
          {
            scaleSetExtension.Name
          };
          break;
        }
      }
      try
      {
        await client.CreateExtensionAsync(requestContext.ActivityId, elasticPool.AzureId, extension);
      }
      catch (Exception ex)
      {
        sb.AppendLine(ex.Message);
      }
      requestContext.TraceAlways(10015200, TraceLevel.Info, "DistributedTask", "ElasticPools", sb.ToString());
      accountUrl = (string) null;
      windowsAgentUrl = (string) null;
      linuxAgentUrl = (string) null;
      registry = (IVssRegistryService) null;
      sb = (StringBuilder) null;
      client = (IAzureVirtualMachineScaleSetClient) null;
      extension = (VirtualMachineScaleSetExtension) null;
    }

    public async Task ReimageAllMachinesAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      HashSet<string> instanceIds)
    {
      IAzureVirtualMachineScaleSetClient machineScaleSetClient = this.Client(requestContext, elasticPool.ServiceEndpointId, elasticPool.ServiceEndpointScope);
      MachineInstances machineInstances = new MachineInstances()
      {
        InstanceIds = instanceIds.ToArray<string>()
      };
      Guid activityId = requestContext.ActivityId;
      string azureId = elasticPool.AzureId;
      MachineInstances instances = machineInstances;
      CancellationToken cancellationToken = new CancellationToken();
      await machineScaleSetClient.ReimageAllInstancesAsync(activityId, azureId, instances, cancellationToken);
    }

    public async Task ReimageMachinesAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      HashSet<string> instanceIds)
    {
      IAzureVirtualMachineScaleSetClient machineScaleSetClient = this.Client(requestContext, elasticPool.ServiceEndpointId, elasticPool.ServiceEndpointScope);
      MachineInstances machineInstances = new MachineInstances()
      {
        InstanceIds = instanceIds.ToArray<string>()
      };
      Guid activityId = requestContext.ActivityId;
      string azureId = elasticPool.AzureId;
      MachineInstances instances = machineInstances;
      CancellationToken cancellationToken = new CancellationToken();
      await machineScaleSetClient.ReimageInstancesAsync(activityId, azureId, instances, cancellationToken);
    }

    public async Task<VirtualMachineScaleSetVMInstanceView> GetInstanceViewAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      string instanceId)
    {
      return await this.Client(requestContext, elasticPool.ServiceEndpointId, elasticPool.ServiceEndpointScope).GetInstanceViewAsync(requestContext.ActivityId, elasticPool.AzureId, instanceId);
    }

    public async Task UpgradeMachinesAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      HashSet<string> instanceIds)
    {
      IAzureVirtualMachineScaleSetClient machineScaleSetClient = this.Client(requestContext, elasticPool.ServiceEndpointId, elasticPool.ServiceEndpointScope);
      MachineInstances machineInstances = new MachineInstances()
      {
        InstanceIds = instanceIds.ToArray<string>()
      };
      Guid activityId = requestContext.ActivityId;
      string azureId = elasticPool.AzureId;
      MachineInstances instances = machineInstances;
      CancellationToken cancellationToken = new CancellationToken();
      await machineScaleSetClient.UpgradeInstancesAsync(activityId, azureId, instances, cancellationToken);
    }

    public async Task DeleteExtensionAsync(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      string extensionName)
    {
      await this.Client(requestContext, elasticPool.ServiceEndpointId, elasticPool.ServiceEndpointScope).DeleteExtensionAsync(requestContext.ActivityId, elasticPool.AzureId, extensionName);
    }

    private string LinuxEscapeString(string text)
    {
      char ch1 = '\\';
      char[] chArray = new char[6]
      {
        '"',
        ',',
        '&',
        '<',
        '>',
        ';'
      };
      StringBuilder stringBuilder = new StringBuilder(text);
      foreach (char ch2 in chArray)
        stringBuilder.Replace(ch2.ToString(), string.Format(string.Format("{0}{1}", (object) ch1, (object) ch2)));
      return stringBuilder.ToString();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => this.DisableTracing(systemRequestContext);

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.EnableTracing(systemRequestContext);

    protected override void ClientFactory(
      IVssRequestContext requestContext,
      out IAzureVirtualMachineScaleSetClient client)
    {
      string apiVersion = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Service/DistributedTask/ElasticPool/AzureVMSSClientApiVersion", AzureVirtualMachineScaleSetClient.DefaultApiVersion);
      bool validateHttpClientQueryParams = requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolValidateAzureHttpClientApiVersionQueryParam");
      client = (IAzureVirtualMachineScaleSetClient) new AzureVirtualMachineScaleSetClient(ResourceServiceBase<IAzureVirtualMachineScaleSetClient>.GetAzureResourceManagerUrl(), this.GetMessageHandler(requestContext, AzureHttpClientBase.DefaultAuthenticationUrl), false, new TimeSpan(), apiVersion, validateHttpClientQueryParams);
    }

    private IAzureVirtualMachineScaleSetClient Client(
      IVssRequestContext requestContext,
      Guid serviceEndpointId,
      Guid serviceEndpointScope)
    {
      return this.Client(requestContext, serviceEndpointId, serviceEndpointScope, nameof (AzureVirtualMachineScaleSetResourceService), "IAzureVirtualMachineScaleSetClient");
    }
  }
}
