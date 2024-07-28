// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.HostedTaskAgentExtension
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.AzComm.SharedContracts;
using Microsoft.VisualStudio.Services.AzComm.WebApi.Contracts;
using Microsoft.VisualStudio.Services.AzComm.WebApi.HttpClients;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.MachineManagement.Framework.Server;
using Microsoft.VisualStudio.Services.MachineManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal sealed class HostedTaskAgentExtension : ITaskAgentExtension
  {
    private const string s_layer = "HostedTaskAgentExtension";
    private const string c_initialLeaseTimeRegistryPath = "/Service/DistributedTask/InitialHostedAgentLeaseMinutes";
    private const string c_extendedLeaseTimeRegistryPath = "/Service/DistributedTask/ExtendedHostedAgentLeaseMinutes";
    private const string c_imageLabelMappingsRegistryPath = "/Service/DistributedTask/AgentPoolToImageLabelMappings";
    private const string c_imageBasedBuildTimeoutRegistryPath = "/Service/DistributedTask/BuildTimeout/";
    private const string c_hostedAgentMinutesMeterName = "Build";
    private const int c_minimumMaxParallelismAllowed = 1;

    public async Task<(int MaxParallelism, int RequestTimeout)> CheckBillingResourcesAsync(
      IVssRequestContext requestContext,
      int poolId,
      Guid scopeId,
      Guid planId,
      string parallelismTag,
      bool throwException = true)
    {
      HostedPoolSettings hostedPoolSettings = await this.CheckHostedBillingResourcesAsync(requestContext, poolId, parallelismTag, throwException, scopeId, planId);
      return (hostedPoolSettings.MaxParallelism, hostedPoolSettings.RequestTimeout);
    }

    public async Task<HostedPoolSettings> CheckHostedBillingResourcesAsync(
      IVssRequestContext requestContext,
      int poolId,
      string parallelismTag,
      bool throwException = true,
      Guid scopeId = default (Guid),
      Guid planId = default (Guid))
    {
      TaskAgentPool agentPoolAsync = await requestContext.GetService<IDistributedTaskResourceService>().GetAgentPoolAsync(requestContext, poolId);
      if (this.IsMobileCenterMacBuild(requestContext, agentPoolAsync.Name))
        return new HostedPoolSettings()
        {
          MaxParallelism = agentPoolAsync.Size,
          RequestTimeout = 0
        };
      HostedPoolSettings poolSettings = DistributedTaskHostedPoolHelper.GetPoolSettings(requestContext, agentPoolAsync.Size, scopeId, planId, parallelismTag);
      DistributedTaskHostedPoolHelper.ResizePoolsIfNecessary(requestContext, poolSettings, true);
      if (!poolSettings.HasPremiumAgents)
      {
        bool flag;
        try
        {
          flag = this.HasAgentMinutesAvailable(requestContext);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(nameof (HostedTaskAgentExtension), ex);
          poolSettings.RequestTimeout = 360;
          return poolSettings;
        }
        if (!flag)
        {
          string message = TaskResources.AccountDoesNotHaveUsageLeft((object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) (requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, Guid.Empty, AccessMappingConstants.PublicAccessMappingMoniker) + TaskResources.ResourceLimitsLocation())));
          if (throwException)
            throw new TaskAgentJobFailedNotEnoughSubscriptionResourcesException(message);
        }
      }
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      poolSettings.RequestTimeout = service.GetValue<int>(requestContext, (RegistryQuery) ("/Service/DistributedTask/BuildTimeout/" + agentPoolAsync.Name), poolSettings.RequestTimeout);
      return poolSettings;
    }

    private bool HasAgentMinutesAvailable(IVssRequestContext requestContext)
    {
      MeterUsage2GetResponse usage2GetResponse = requestContext.To(TeamFoundationHostType.Deployment).GetClient<MeterUsage2HttpClient>().GetMeterUsageAsync(requestContext.ServiceHost.InstanceId, AzCommMeterIds.BuildMinutesMeterId).SyncResult<MeterUsage2GetResponse>();
      return usage2GetResponse == null || usage2GetResponse.CurrentQuantity < usage2GetResponse.FreeQuantity;
    }

    public async Task FilterCapabilitiesAsync(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgent agent)
    {
      TaskAgentPool agentPoolAsync = await requestContext.GetService<IDistributedTaskResourceService>().GetAgentPoolAsync(requestContext, poolId);
      bool flag1 = agentPoolAsync.Name.IsLinuxPoolName();
      bool flag2 = agentPoolAsync.Name.IsVS2015PoolName();
      bool flag3 = agentPoolAsync.Name.IsVS2017PoolName();
      bool flag4 = agentPoolAsync.Name.IsMacPoolName();
      bool hasValue = agentPoolAsync.AgentCloudId.HasValue;
      PackageVersion agentPackageVersion = requestContext.GetRecommendedAgentPackageVersion();
      if (agentPackageVersion != null)
        agent.Version = agentPackageVersion.ToString();
      if (flag1)
      {
        agent.SystemCapabilities.Clear();
        agent.SystemCapabilities["Agent.OS"] = "linux";
        agent.SystemCapabilities["ant"] = "/usr/bin/ant";
        agent.SystemCapabilities["curl"] = "/usr/bin/curl";
        agent.SystemCapabilities["git"] = "/usr/bin/git";
        agent.SystemCapabilities["gulp"] = "/usr/local/bin/gulp";
        agent.SystemCapabilities["java"] = "/usr/bin/java";
        agent.SystemCapabilities["JDK"] = "/usr/bin/javac";
        agent.SystemCapabilities["make"] = "/usr/bin/make";
        agent.SystemCapabilities["maven"] = "/usr/bin/mvn";
        agent.SystemCapabilities["node.js"] = "/usr/bin/nodejs";
        agent.SystemCapabilities["npm"] = "/usr/local/bin/npm";
        agent.SystemCapabilities["python"] = "/usr/bin/python";
        agent.SystemCapabilities["python3"] = "/usr/bin/python3";
        agent.SystemCapabilities["sh"] = "/bin/sh";
      }
      else if (flag2)
      {
        agent.SystemCapabilities.Clear();
        agent.SystemCapabilities["Agent.OS"] = "Windows_NT";
        agent.SystemCapabilities["AndroidSDK"] = "C:\\Program Files (x86)\\Android\\android-sdk";
        agent.SystemCapabilities["AndroidSDK_10"] = "C:\\Program Files (x86)\\Android\\android-sdk\\platforms\\android-10";
        agent.SystemCapabilities["AndroidSDK_15"] = "C:\\Program Files (x86)\\Android\\android-sdk\\platforms\\android-15";
        agent.SystemCapabilities["AndroidSDK_16"] = "C:\\Program Files (x86)\\Android\\android-sdk\\platforms\\android-16";
        agent.SystemCapabilities["AndroidSDK_17"] = "C:\\Program Files (x86)\\Android\\android-sdk\\platforms\\android-17";
        agent.SystemCapabilities["AndroidSDK_18"] = "C:\\Program Files (x86)\\Android\\android-sdk\\platforms\\android-18";
        agent.SystemCapabilities["AndroidSDK_19"] = "C:\\Program Files (x86)\\Android\\android-sdk\\platforms\\android-19";
        agent.SystemCapabilities["AndroidSDK_20"] = "C:\\Program Files (x86)\\Android\\android-sdk\\platforms\\android-20";
        agent.SystemCapabilities["AndroidSDK_21"] = "C:\\Program Files (x86)\\Android\\android-sdk\\platforms\\android-21";
        agent.SystemCapabilities["AndroidSDK_22"] = "C:\\Program Files (x86)\\Android\\android-sdk\\platforms\\android-22";
        agent.SystemCapabilities["AndroidSDK_23"] = "C:\\Program Files (x86)\\Android\\android-sdk\\platforms\\android-23";
        agent.SystemCapabilities["AndroidSDK_24"] = "C:\\Program Files (x86)\\Android\\android-sdk\\platforms\\android-24";
        agent.SystemCapabilities["AndroidSDK_25"] = "C:\\Program Files (x86)\\Android\\android-sdk\\platforms\\android-25";
        agent.SystemCapabilities["AndroidSDK_8"] = "C:\\Program Files (x86)\\Android\\android-sdk\\platforms\\android-8";
        agent.SystemCapabilities["ant"] = "C:\\apache_ant\\bin";
        agent.SystemCapabilities["AzurePS"] = "3.2.0";
        agent.SystemCapabilities["bower"] = "C:\\NPM\\Modules\\bower.cmd";
        agent.SystemCapabilities["ChocolateyInstall"] = "C:\\ProgramData\\chocolatey";
        agent.SystemCapabilities["CMAKE_HOME"] = "C:\\Program Files\\CMake\\bin";
        agent.SystemCapabilities["Cmd"] = "C:\\Windows\\system32\\cmd.exe";
        agent.SystemCapabilities["DotNetFramework"] = "C:\\Windows\\Microsoft.NET\\Framework64\\v4.0.30319";
        agent.SystemCapabilities["DotNetFramework_4.6.2"] = "C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319";
        agent.SystemCapabilities["DotNetFramework_4.6.2_x64"] = "C:\\Windows\\Microsoft.NET\\Framework64\\v4.0.30319";
        agent.SystemCapabilities["grunt"] = "C:\\NPM\\Modules\\grunt.cmd";
        agent.SystemCapabilities["gulp"] = "C:\\NPM\\Modules\\gulp.cmd";
        agent.SystemCapabilities["java"] = "C:\\Program Files\\Java\\jre1.8.0_131";
        agent.SystemCapabilities["java_6_x64"] = "C:\\Program Files\\Java\\jre6";
        agent.SystemCapabilities["java_7_x64"] = "C:\\Program Files\\Java\\jre7";
        agent.SystemCapabilities["java_8_x64"] = "C:\\Program Files\\Java\\jre1.8.0_131";
        agent.SystemCapabilities["jdk"] = "C:\\Program Files\\Java\\jdk1.8.0_131";
        agent.SystemCapabilities["jdk_6"] = "C:\\Program Files (x86)\\Java\\jdk1.6.0_45";
        agent.SystemCapabilities["jdk_6_x64"] = "C:\\Program Files\\Java\\jdk1.6.0_45";
        agent.SystemCapabilities["jdk_7"] = "C:\\Program Files (x86)\\Java\\jdk1.7.0_75";
        agent.SystemCapabilities["jdk_7_x64"] = "C:\\Program Files\\Java\\jdk1.7.0_75";
        agent.SystemCapabilities["jdk_8"] = "C:\\Program Files (x86)\\Java\\jdk1.8.0_131";
        agent.SystemCapabilities["jdk_8_x64"] = "C:\\Program Files\\Java\\jdk1.8.0_131";
        agent.SystemCapabilities["maven"] = "C:\\apache-maven-3.5.0";
        agent.SystemCapabilities["MSBuild"] = "C:\\Program Files (x86)\\MSBuild\\14.0\\bin\\";
        agent.SystemCapabilities["MSBuild_12.0"] = "C:\\Program Files (x86)\\MSBuild\\12.0\\bin\\";
        agent.SystemCapabilities["MSBuild_12.0_x64"] = "C:\\Program Files (x86)\\MSBuild\\12.0\\bin\\amd64\\";
        agent.SystemCapabilities["MSBuild_14.0"] = "C:\\Program Files (x86)\\MSBuild\\14.0\\bin\\";
        agent.SystemCapabilities["MSBuild_14.0_x64"] = "C:\\Program Files (x86)\\MSBuild\\14.0\\bin\\amd64\\";
        agent.SystemCapabilities["MSBuild_4.0"] = "C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\";
        agent.SystemCapabilities["MSBuild_4.0_x64"] = "C:\\Windows\\Microsoft.NET\\Framework64\\v4.0.30319\\";
        agent.SystemCapabilities["MSBuild_x64"] = "C:\\Program Files (x86)\\MSBuild\\14.0\\bin\\amd64\\";
        agent.SystemCapabilities["node.js"] = "C:\\Program Files\\nodejs\\node.exe";
        agent.SystemCapabilities["npm"] = "C:\\Program Files\\nodejs\\npm.cmd";
        agent.SystemCapabilities["SqlPackage"] = "C:\\Program Files\\Microsoft SQL Server\\140\\DAC\\bin\\SqlPackage.exe";
        agent.SystemCapabilities["VisualStudio"] = "C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\";
        agent.SystemCapabilities["VisualStudio_14.0"] = "C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\";
        agent.SystemCapabilities["VisualStudio_IDE"] = "C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\Common7\\IDE\\";
        agent.SystemCapabilities["VisualStudio_IDE_14.0"] = "C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\Common7\\IDE\\";
        agent.SystemCapabilities["VSTest"] = "C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\Common7\\IDE\\CommonExtensions\\Microsoft\\TestWindow";
        agent.SystemCapabilities["VSTest_14.0"] = "C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\Common7\\IDE\\CommonExtensions\\Microsoft\\TestWindow";
        agent.SystemCapabilities["Xamarin.Android"] = "7.0.2.37";
      }
      else if (flag3)
      {
        agent.SystemCapabilities.Clear();
        agent.SystemCapabilities["AndroidSDK"] = "C:\\Program Files (x86)\\Android\\android-sdk";
        agent.SystemCapabilities["ant"] = "C:\\java\\ant\\apache-ant-1.9.7";
        agent.SystemCapabilities["AzurePS"] = "3.6.0";
        agent.SystemCapabilities["Cmd"] = "C:\\Windows\\system32\\cmd.exe";
        agent.SystemCapabilities["curl"] = "C:\\Program Files\\Git\\mingw64\\bin\\curl.exe";
        agent.SystemCapabilities["DotNetFramework"] = "C:\\Windows\\Microsoft.NET\\Framework64\\v4.0.30319";
        agent.SystemCapabilities["GTK_BASEPATH"] = "C:\\Program Files (x86)\\GtkSharp\\2.12\\";
        agent.SystemCapabilities["java"] = "C:\\Program Files (x86)\\Java\\jdk1.8.0_112";
        agent.SystemCapabilities["jdk"] = "C:\\Program Files (x86)\\Java\\jdk1.8.0_112";
        agent.SystemCapabilities["jdk_8"] = "C:\\Program Files (x86)\\Java\\jdk1.8.0_112";
        agent.SystemCapabilities["maven"] = "C:\\java\\maven\\apache-maven-3.2.2";
        agent.SystemCapabilities["MSBuild"] = "C:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\Enterprise\\MSBuild\\15.0\\Bin\\";
        agent.SystemCapabilities["node.js"] = "C:\\Program Files\\nodejs\\node.exe";
        agent.SystemCapabilities["npm"] = "C:\\Program Files\\nodejs\\npm.cmd";
        agent.SystemCapabilities["SqlPackage"] = "C:\\Program Files\\Microsoft SQL Server\\140\\DAC\\bin\\SqlPackage.exe";
        agent.SystemCapabilities["VisualStudio"] = "C:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\Enterprise\\";
        agent.SystemCapabilities["VSTest"] = "C:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\Enterprise\\Common7\\IDE\\CommonExtensions\\Microsoft\\TestWindow";
        agent.SystemCapabilities["Xamarin.Android"] = "7.2.0.7";
      }
      else if (flag4)
      {
        agent.SystemCapabilities.Clear();
        agent.SystemCapabilities["Agent.OS"] = "darwin";
        agent.SystemCapabilities["__CF_USER_TEXT_ENCODING"] = "0x1F5:0x0:0x0";
        agent.SystemCapabilities["ANDROID_HOME"] = "/Users/ci/Library/Android/sdk";
        agent.SystemCapabilities["ANDROID_NDK_HOME"] = "/Users/ci/Library/Android/sdk/ndk-bundle";
        agent.SystemCapabilities["AndroidSDK"] = "/Users/ci/Library/Android/sdk/tools/android";
        agent.SystemCapabilities["Apple_PubSub_Socket_Render"] = "/private/tmp/com.apple.launchd.JLBkIzGlMq/Render";
        agent.SystemCapabilities["bundler"] = "/usr/local/bin/bundle";
        agent.SystemCapabilities["clang"] = "/usr/bin/clang";
        agent.SystemCapabilities["curl"] = "/usr/bin/curl";
        agent.SystemCapabilities["git"] = "/usr/local/bin/git";
        agent.SystemCapabilities["java"] = "/usr/bin/java";
        agent.SystemCapabilities["JAVA_HOME"] = "/Library/Java/JavaVirtualMachines/jdk1.8.0_131.jdk/Contents/Home";
        agent.SystemCapabilities["JDK"] = "/usr/bin/javac";
        agent.SystemCapabilities["LC_CTYPE"] = "en_US.UTF-8";
        agent.SystemCapabilities["make"] = "/usr/bin/make";
        agent.SystemCapabilities["maven"] = "/usr/local/bin/mvn";
        agent.SystemCapabilities["MSBuild"] = "/Library/Frameworks/Mono.framework/Versions/Current/Commands/xbuild";
        agent.SystemCapabilities["node.js"] = "/usr/local/bin/node";
        agent.SystemCapabilities["npm"] = "/usr/local/bin/npm";
        agent.SystemCapabilities["NUNIT_BASE_PATH"] = "/Library/Developer/nunit";
        agent.SystemCapabilities["NUNIT3_PATH"] = "/Library/Developer/nunit/3.6.0";
        agent.SystemCapabilities["python"] = "/usr/bin/python";
        agent.SystemCapabilities["rake"] = "/usr/local/bin/rake";
        agent.SystemCapabilities["RCT_NO_LAUNCH_PACKAGER"] = "1";
        agent.SystemCapabilities["ruby"] = "/usr/local/bin/ruby";
        agent.SystemCapabilities["sh"] = "/bin/sh";
        agent.SystemCapabilities["SHELL"] = "/bin/bash";
        agent.SystemCapabilities["subversion"] = "/usr/bin/svn";
        agent.SystemCapabilities["svn"] = "/usr/bin/svn";
        agent.SystemCapabilities["Xamarin.Android"] = "/Library/Frameworks/Xamarin.Android.framework/Commands/generator";
        agent.SystemCapabilities["Xamarin.iOS"] = "/Applications/Visual Studio.app/Contents/MacOS/vstool";
        agent.SystemCapabilities["xcode"] = "/Applications/Xcode_8.3.3.app/Contents/Developer";
        agent.SystemCapabilities["XPC_FLAGS"] = "0x0";
        agent.SystemCapabilities["XPC_SERVICE_NAME"] = "0";
      }
      else if (hasValue)
      {
        agent.SystemCapabilities.Clear();
        agent.SystemCapabilities["__CF_USER_TEXT_ENCODING"] = "";
        agent.SystemCapabilities["ANDROID_HOME"] = "";
        agent.SystemCapabilities["ANDROID_NDK_HOME"] = "";
        agent.SystemCapabilities["AndroidSDK"] = "";
        agent.SystemCapabilities["AndroidSDK_10"] = "";
        agent.SystemCapabilities["AndroidSDK_15"] = "";
        agent.SystemCapabilities["AndroidSDK_16"] = "";
        agent.SystemCapabilities["AndroidSDK_17"] = "";
        agent.SystemCapabilities["AndroidSDK_18"] = "";
        agent.SystemCapabilities["AndroidSDK_19"] = "";
        agent.SystemCapabilities["AndroidSDK_20"] = "";
        agent.SystemCapabilities["AndroidSDK_21"] = "";
        agent.SystemCapabilities["AndroidSDK_22"] = "";
        agent.SystemCapabilities["AndroidSDK_23"] = "";
        agent.SystemCapabilities["AndroidSDK_24"] = "";
        agent.SystemCapabilities["AndroidSDK_25"] = "";
        agent.SystemCapabilities["AndroidSDK_8"] = "";
        agent.SystemCapabilities["ant"] = "";
        agent.SystemCapabilities["Apple_PubSub_Socket_Render"] = "";
        agent.SystemCapabilities["AzurePS"] = "";
        agent.SystemCapabilities["bower"] = "";
        agent.SystemCapabilities["bundler"] = "";
        agent.SystemCapabilities["ChocolateyInstall"] = "";
        agent.SystemCapabilities["clang"] = "";
        agent.SystemCapabilities["CMAKE_HOME"] = "";
        agent.SystemCapabilities["Cmd"] = "";
        agent.SystemCapabilities["curl"] = "";
        agent.SystemCapabilities["DotNetFramework"] = "";
        agent.SystemCapabilities["DotNetFramework_4.6.2"] = "";
        agent.SystemCapabilities["DotNetFramework_4.6.2_x64"] = "";
        agent.SystemCapabilities["git"] = "";
        agent.SystemCapabilities["grunt"] = "";
        agent.SystemCapabilities["GTK_BASEPATH"] = "";
        agent.SystemCapabilities["gulp"] = "";
        agent.SystemCapabilities["java"] = "";
        agent.SystemCapabilities["JAVA_HOME"] = "";
        agent.SystemCapabilities["java_6_x64"] = "";
        agent.SystemCapabilities["java_7_x64"] = "";
        agent.SystemCapabilities["java_8_x64"] = "";
        agent.SystemCapabilities["jdk"] = "";
        agent.SystemCapabilities["JDK"] = "";
        agent.SystemCapabilities["jdk_6"] = "";
        agent.SystemCapabilities["jdk_6_x64"] = "";
        agent.SystemCapabilities["jdk_7"] = "";
        agent.SystemCapabilities["jdk_7_x64"] = "";
        agent.SystemCapabilities["jdk_8"] = "";
        agent.SystemCapabilities["jdk_8_x64"] = "";
        agent.SystemCapabilities["LC_CTYPE"] = "";
        agent.SystemCapabilities["make"] = "";
        agent.SystemCapabilities["maven"] = "";
        agent.SystemCapabilities["MSBuild"] = "";
        agent.SystemCapabilities["MSBuild_12.0"] = "";
        agent.SystemCapabilities["MSBuild_12.0_x64"] = "";
        agent.SystemCapabilities["MSBuild_14.0"] = "";
        agent.SystemCapabilities["MSBuild_14.0_x64"] = "";
        agent.SystemCapabilities["MSBuild_4.0"] = "";
        agent.SystemCapabilities["MSBuild_4.0_x64"] = "";
        agent.SystemCapabilities["MSBuild_x64"] = "";
        agent.SystemCapabilities["node.js"] = "";
        agent.SystemCapabilities["npm"] = "";
        agent.SystemCapabilities["NUNIT_BASE_PATH"] = "";
        agent.SystemCapabilities["NUNIT3_PATH"] = "";
        agent.SystemCapabilities["python"] = "";
        agent.SystemCapabilities["python3"] = "";
        agent.SystemCapabilities["rake"] = "";
        agent.SystemCapabilities["RCT_NO_LAUNCH_PACKAGER"] = "";
        agent.SystemCapabilities["ruby"] = "";
        agent.SystemCapabilities["sh"] = "";
        agent.SystemCapabilities["SHELL"] = "";
        agent.SystemCapabilities["subversion"] = "";
        agent.SystemCapabilities["SqlPackage"] = "";
        agent.SystemCapabilities["svn"] = "";
        agent.SystemCapabilities["VisualStudio"] = "";
        agent.SystemCapabilities["VisualStudio_14.0"] = "";
        agent.SystemCapabilities["VisualStudio_IDE"] = "";
        agent.SystemCapabilities["VisualStudio_IDE_14.0"] = "";
        agent.SystemCapabilities["VSTest"] = "";
        agent.SystemCapabilities["VSTest_14.0"] = "";
        agent.SystemCapabilities["Xamarin.Android"] = "";
        agent.SystemCapabilities["Xamarin.iOS"] = "";
        agent.SystemCapabilities["xcode"] = "";
        agent.SystemCapabilities["XPC_FLAGS"] = "";
        agent.SystemCapabilities["XPC_SERVICE_NAME"] = "";
      }
      else
      {
        agent.SystemCapabilities.Clear();
        agent.SystemCapabilities["AndroidSDK"] = "C:\\java\\androidsdk\\android-sdk";
        agent.SystemCapabilities["ant"] = "C:\\java\\ant\\apache-ant-1.9.7";
        agent.SystemCapabilities["AzurePS"] = "1.0.0";
        agent.SystemCapabilities["bower"] = "C:\\NPM\\Modules\\bower.cmd";
        agent.SystemCapabilities["Cmd"] = "C:\\Windows\\system32\\cmd.exe";
        agent.SystemCapabilities["curl"] = "C:\\Program Files (x86)\\Git\\bin\\curl.exe";
        agent.SystemCapabilities["DotNetFramework"] = "C:\\Windows\\Microsoft.NET\\Framework64\\v4.0.30319";
        agent.SystemCapabilities["grunt"] = "C:\\NPM\\Modules\\grunt.cmd";
        agent.SystemCapabilities["gulp"] = "C:\\NPM\\Modules\\gulp.cmd";
        agent.SystemCapabilities["java"] = "C:\\Program Files\\Java\\jre1.8.0_102";
        agent.SystemCapabilities["JDK"] = "C:\\Program Files\\Java\\jdk1.8.0_102";
        agent.SystemCapabilities["maven"] = "C:\\java\\maven\\apache-maven-3.2.2";
        agent.SystemCapabilities["MSBuild"] = "C:\\Program Files (x86)\\MSBuild\\14.0\\bin";
        agent.SystemCapabilities["node.js"] = "C:\\Program Files\\nodejs\\node.exe";
        agent.SystemCapabilities["npm"] = "C:\\Program Files\\nodejs\\npm.cmd";
        agent.SystemCapabilities["SqlPackage"] = "C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\Common7\\IDE\\Extensions\\Microsoft\\SQLDB\\DAC\\120\\sqlpackage.exe";
        agent.SystemCapabilities["VisualStudio"] = "C:\\Program Files (x86)\\Microsoft Visual Studio 14.0";
        agent.SystemCapabilities["VSTest"] = "C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\Common7\\IDE\\CommonExtensions\\Microsoft\\TestWindow";
        agent.SystemCapabilities["Xamarin.Android"] = "5.1.4.16";
      }
      agent.PopulateImplicitCapabilities();
    }

    public async Task JobAssignedAsync(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest jobRequest,
      bool isAgentCloudBacked)
    {
      DistributedTaskResourceService resourceService = requestContext.GetService<DistributedTaskResourceService>();
      if (!isAgentCloudBacked)
      {
        IVssRequestContext deploymentContext = requestContext.To(TeamFoundationHostType.Deployment);
        IVssRegistryService registryService = deploymentContext.GetService<IVssRegistryService>();
        int num1 = registryService.GetValue<int>(deploymentContext, (RegistryQuery) "/Service/DistributedTask/InitialHostedAgentLeaseMinutes", 5);
        TaskAgentJobRequest taskAgentJobRequest1 = await resourceService.UpdateAgentRequestAsync(requestContext.Elevate(), poolId, jobRequest.RequestId, new DateTime?(DateTime.UtcNow.AddMinutes((double) num1)), new DateTime?(), new DateTime?(), new TaskResult?());
        string key1 = "PoolType";
        string key2 = "PoolName";
        if (!jobRequest.ServiceOwner.Equals(ServiceInstanceTypes.TFS))
        {
          key1 = "OrchestrationPoolType";
          key2 = "OrchestrationPoolName";
        }
        TaskAgentPool pool = resourceService.GetAgentPool(requestContext, poolId, (IList<string>) new string[2]
        {
          key1,
          key2
        }, TaskAgentPoolActionFilter.None);
        string poolType = pool.Properties.GetValue<string>(key1, (string) null);
        string poolName = pool.Properties.GetValue<string>(key2, (string) null);
        IMachineManagementService machineManagement = requestContext.GetService<IMachineManagementService>();
        if (!machineManagement.IsUsingSeparateService())
        {
          if (string.IsNullOrEmpty(poolType))
          {
            resourceService = (DistributedTaskResourceService) null;
            return;
          }
          if (string.IsNullOrEmpty(poolName))
          {
            resourceService = (DistributedTaskResourceService) null;
            return;
          }
        }
        bool isScheduled = this.IsJobScheduled(jobRequest);
        string machineImageLabel = HostedTaskAgentExtension.GetMachineImageLabel(requestContext, pool.Name);
        if (string.IsNullOrEmpty(machineImageLabel))
          throw new MachineImageLabelDoesNotExistException(TaskResources.ImageLabelNotFound((object) poolName));
        PackageVersion agentVersion = requestContext.GetRecommendedAgentPackageVersion();
        string parallelismTag = jobRequest.GetParallelismTag();
        (int, int) valueTuple = await this.CheckBillingResourcesAsync(requestContext, poolId, jobRequest.ScopeId, jobRequest.PlanId, parallelismTag);
        string str1 = requestContext.BuildHyperlink();
        MachineRequest machineRequest = new MachineRequest(poolType, poolName, requestContext.ServiceHost.InstanceId, agentVersion.ToString())
        {
          Inputs = {
            {
              "AgentId",
              jobRequest.ReservedAgent.Id.ToString()
            },
            {
              "AgentName",
              jobRequest.ReservedAgent.Name
            },
            {
              "DefinitionId",
              jobRequest.Definition?.Id.ToString() ?? string.Empty
            },
            {
              "JobId",
              jobRequest.JobId.ToString("D")
            },
            {
              "JobRequestId",
              jobRequest.RequestId.ToString("D")
            },
            {
              "PlanHostId",
              jobRequest.HostId.ToString("D")
            },
            {
              "PlanId",
              jobRequest.PlanId.ToString("D")
            },
            {
              "PlanType",
              jobRequest.PlanType
            },
            {
              "PoolId",
              poolId.ToString()
            },
            {
              "PoolName",
              pool.Name
            },
            {
              "ScopeIdentifier",
              jobRequest.ScopeId.ToString("D")
            },
            {
              "ServerUrl",
              str1
            },
            {
              "ServiceOwner",
              jobRequest.ServiceOwner.ToString("D")
            },
            {
              "PerformanceMetrics",
              jobRequest.GetPerformanceMetrics()
            }
          },
          IsScheduled = new bool?(isScheduled),
          MachineImageLabel = machineImageLabel,
          RequestType = "DistributedTask"
        };
        string str2 = string.Equals(parallelismTag, "Public", StringComparison.OrdinalIgnoreCase) ? "PublicProject" : "PrivateProject";
        machineRequest.Tags.Add(str2);
        if (isScheduled)
          machineRequest.Tags.Add("Scheduled");
        int num2 = valueTuple.Item1;
        machineRequest.MaxParallelism = new int?(num2 > 0 ? num2 : 1);
        if (valueTuple.Item2 > 0)
          machineRequest.Timeout = new TimeSpan?(TimeSpan.FromMinutes((double) valueTuple.Item2));
        machineManagement.QueueRequest(requestContext, machineRequest);
        int num3 = registryService.GetValue<int>(deploymentContext, (RegistryQuery) "/Service/DistributedTask/ExtendedHostedAgentLeaseMinutes", 15);
        TaskAgentJobRequest taskAgentJobRequest2 = await resourceService.UpdateAgentRequestAsync(requestContext.Elevate(), poolId, jobRequest.RequestId, new DateTime?(DateTime.UtcNow.AddMinutes((double) num3)), new DateTime?(), new DateTime?(), new TaskResult?());
        deploymentContext = (IVssRequestContext) null;
        registryService = (IVssRegistryService) null;
        pool = (TaskAgentPool) null;
        poolType = (string) null;
        poolName = (string) null;
        machineManagement = (IMachineManagementService) null;
        machineImageLabel = (string) null;
        agentVersion = (PackageVersion) null;
        parallelismTag = (string) null;
      }
      else
        await resourceService.RaiseEventAsync<JobAssignedEvent>(requestContext.Elevate(), jobRequest.ServiceOwner, jobRequest.HostId, jobRequest.ScopeId, jobRequest.PlanType, jobRequest.PlanId, new JobAssignedEvent(jobRequest.JobId, jobRequest));
      resourceService.GetTaskAgentPoolExtension(requestContext, poolId).AgentRequestAssigned(requestContext, poolId, jobRequest);
      resourceService = (DistributedTaskResourceService) null;
    }

    public void JobCompleted(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest jobRequest,
      bool isAgentCloudBacked)
    {
      if (jobRequest.ReservedAgent == null || isAgentCloudBacked)
        return;
      requestContext.GetService<IDistributedTaskResourceService>().ClearAgentInformation(requestContext.Elevate(), poolId, jobRequest.ReservedAgent.Id);
    }

    public Task<Stream> GetAgentPoolMetadataAsync(
      IVssRequestContext requestContext,
      string poolName,
      int? poolMetadataFileId)
    {
      string machineImageLabel = HostedTaskAgentExtension.GetMachineImageLabel(requestContext, poolName);
      if (string.IsNullOrEmpty(machineImageLabel))
        return Task.FromResult<Stream>((Stream) null);
      MachineManagementHttpClient client = requestContext.Elevate().GetClient<MachineManagementHttpClient>();
      try
      {
        return client.GetMachineImageLabelMetadataAsync(machineImageLabel);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(nameof (HostedTaskAgentExtension), ex);
        return Task.FromResult<Stream>((Stream) null);
      }
    }

    private bool IsJobScheduled(TaskAgentJobRequest jobRequest)
    {
      string str;
      bool result;
      return jobRequest.Data.TryGetValue(TaskAgentRequestConstants.IsScheduledKey, out str) && bool.TryParse(str, out result) && result;
    }

    internal static string GetMachineImageLabel(IVssRequestContext requestContext, string poolName)
    {
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        string str1 = vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, (RegistryQuery) "/Service/DistributedTask/AgentPoolToImageLabelMappings", (string) null);
        requestContext.TraceInfo(10015260, nameof (HostedTaskAgentExtension), "GetMachineImageLabel for PoolName: {0} Mappings: {1}.", (object) poolName, (object) str1);
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(str1))
        {
          string str2 = str1;
          char[] chArray1 = new char[1]{ ',' };
          foreach (string str3 in str2.Split(chArray1))
          {
            char[] chArray2 = new char[1]{ '=' };
            string[] strArray = str3.Split(chArray2);
            dictionary.Add(strArray[0], strArray[1]);
          }
        }
        if (dictionary.ContainsKey(poolName))
          return dictionary[poolName];
      }
      catch (Exception ex)
      {
        requestContext.TraceException(nameof (HostedTaskAgentExtension), ex);
      }
      int num = poolName.IsLinuxPoolName() ? 1 : 0;
      bool flag1 = poolName.IsVS2015PoolName();
      bool flag2 = poolName.IsVS2017PoolName();
      bool flag3 = poolName.IsVS2019PoolName();
      bool flag4 = poolName.IsMacPoolName();
      bool flag5 = poolName.IsWindowsContainerPoolName();
      bool flag6 = poolName.IsHostedUbuntu16Name();
      if (num != 0)
        return "Linux";
      if (flag1)
        return "VS2015";
      if (flag2)
        return "VS2017";
      if (flag3)
        return "windows-2019-vs2019";
      if (flag5)
        return "WINCON";
      if (flag4)
      {
        bool flag7 = poolName.Contains("High Sierra");
        return poolName.Contains("INT") ? (!flag7 ? "Mac_INT" : "Mac_INT1013") : (poolName.Contains("Staging") ? (!flag7 ? "Mac_Staging" : "Mac_Staging1013") : (poolName.Contains("Prod") ? (!flag7 ? "Mac_Prod" : "Mac_Prod1013") : (!flag7 ? "MacOS" : "MacOS-1013")));
      }
      if (flag6)
        return "Ubuntu16";
      return poolName.Equals("Hosted", StringComparison.OrdinalIgnoreCase) ? "DefaultHosted" : string.Empty;
    }

    private bool IsMobileCenterMacBuild(IVssRequestContext requestContext, string poolName)
    {
      try
      {
        if (poolName.IsMacPoolName())
          return requestContext.TryGetIsMobileCenter();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(nameof (HostedTaskAgentExtension), ex);
      }
      return false;
    }
  }
}
