// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureRoleUtil
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public static class AzureRoleUtil
  {
    private static string s_tenantId;
    private static string s_servicePrincipalClientId;
    private static string s_servicePrincipalCertificateThumbprint;
    private static string s_azureSubscriptionId;
    private static string s_resourceGroupName;
    private static Lazy<VssRuntimeConfiguration> s_config = new Lazy<VssRuntimeConfiguration>(new Func<VssRuntimeConfiguration>(AzureRoleUtil.InitializeConfiguration));
    private static IRoleEnvironment s_env = (IRoleEnvironment) new VssRoleEnvironment();

    public static bool IsAvailable => !string.IsNullOrEmpty(AzureRoleUtil.s_env.DeploymentId);

    public static VssRuntimeConfiguration Configuration => AzureRoleUtil.s_config.Value;

    public static IRoleEnvironment Environment
    {
      get
      {
        if (!AzureRoleUtil.IsAvailable)
          throw new AzureRoleEnvironmentUnavailableException();
        return AzureRoleUtil.s_env;
      }
    }

    public static ArmClient GetArmClient(IVssRequestContext deploymentContext)
    {
      deploymentContext.CheckDeploymentRequestContext();
      if (AzureRoleUtil.s_tenantId == null)
        AzureRoleUtil.s_tenantId = AzureRoleUtil.GetOverridableConfigurationSetting("ResourceManagerAadTenantId");
      if (deploymentContext.IsFeatureEnabled("AzureDevOps.Services.ManagedIdentity.ManagementAccess"))
        return ArmClientFactory.ManagedIdentity((string) null).GetArmClient(AzureRoleUtil.s_tenantId);
      if (deploymentContext.IsFeatureEnabled("Microsoft.AzureDevOps.ResourceManagement.TenantIdFromSubscription"))
        AzureRoleUtil.s_tenantId = ((IEnumerable<TenantResource>) ArmClientFactory.ManagedIdentity((string) null).GetArmClient(AzureRoleUtil.s_tenantId).GetTenants()).First<TenantResource>().Data.TenantId.ToString();
      if (AzureRoleUtil.s_servicePrincipalClientId == null)
        AzureRoleUtil.s_servicePrincipalClientId = AzureRoleUtil.GetOverridableConfigurationSetting("RuntimeServicePrincipalClientId");
      if (AzureRoleUtil.s_servicePrincipalCertificateThumbprint == null)
        AzureRoleUtil.s_servicePrincipalCertificateThumbprint = AzureRoleUtil.GetOverridableConfigurationSetting("RuntimeServicePrincipalCertThumbprint");
      return ArmClientFactory.ServicePrincipal(AzureRoleUtil.s_servicePrincipalClientId, AzureRoleUtil.s_servicePrincipalCertificateThumbprint).GetArmClient(AzureRoleUtil.s_tenantId);
    }

    public static SubscriptionResource GetSubscriptionResource(IVssRequestContext deploymentContext)
    {
      if (AzureRoleUtil.s_azureSubscriptionId == null)
        AzureRoleUtil.s_azureSubscriptionId = AzureRoleUtil.GetOverridableConfigurationSetting("AzureSubscriptionId");
      ResourceIdentifier resourceIdentifier = SubscriptionResource.CreateResourceIdentifier(AzureRoleUtil.s_azureSubscriptionId);
      return AzureRoleUtil.GetArmClient(deploymentContext).GetSubscriptionResource(resourceIdentifier);
    }

    public static ResourceGroupResource GetResourceGroupResource(
      IVssRequestContext deploymentContext)
    {
      if (AzureRoleUtil.s_azureSubscriptionId == null)
        AzureRoleUtil.s_azureSubscriptionId = AzureRoleUtil.GetOverridableConfigurationSetting("AzureSubscriptionId");
      AzureRoleUtil.s_resourceGroupName = AzureRoleUtil.GetOverridableConfigurationSetting("HostedServiceName");
      ResourceIdentifier resourceIdentifier = ResourceGroupResource.CreateResourceIdentifier(AzureRoleUtil.s_azureSubscriptionId, AzureRoleUtil.s_resourceGroupName);
      return AzureRoleUtil.GetArmClient(deploymentContext).GetResourceGroupResource(resourceIdentifier);
    }

    public static string GetOverridableConfigurationSetting(string settingName)
    {
      string str = System.Environment.GetEnvironmentVariable(settingName);
      if (str == null)
        str = AzureRoleUtil.Configuration != null ? AzureRoleUtil.Configuration.GetStringSetting(settingName, (string) null) : throw new ApplicationException("Unable to load Azure role configuration and retrieve setting '" + settingName + "'");
      return str != null ? str : throw new ApplicationException("Unable to retrieve configuration setting '" + settingName + "'");
    }

    private static VssRuntimeConfiguration InitializeConfiguration()
    {
      string roleRoot = AzureRoleUtil.Environment.RoleRoot;
      if (roleRoot == null)
        return (VssRuntimeConfiguration) null;
      string path = Path.Combine(roleRoot, "config");
      string[] files = Directory.GetFiles(path, "*.config.xml");
      return files.Length != 0 ? new VssRuntimeConfiguration(files) : throw new ApplicationException("Could not find config files in the " + path + " directory.");
    }
  }
}
