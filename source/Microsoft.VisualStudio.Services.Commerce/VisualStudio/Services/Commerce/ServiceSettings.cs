// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ServiceSettings
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class ServiceSettings
  {
    private IDictionary<string, ServiceSettings.ConfigQueryOrDefault> MethodQueryFallbackDeclaration;
    private string armApiVersion;
    private string registerApiVersion;
    private string classicAdministratorsVersion;
    private string roleAssignmentsVersion;
    private string roleDefinitionsVersion;
    private string armResourceGroupApiVersion;
    private string armResourceAccountExtensionApiVersion;
    private string armResourceAccountApiVersion;
    private string armAgreementApiVersion;
    private string rateCardApiVersion;
    private static readonly RegistryQuery RateCardRegistryQuery = (RegistryQuery) "/Service/Commerce/AzureRateCard/*";
    private static readonly RegistryQuery ResourceManagerRegistryQuery = (RegistryQuery) "/Service/Commerce/AzureResourceManager/*";

    internal IArmAdapterServiceConfigurationHelper ArmConfigHelper { get; set; }

    public ServiceSettings(
      IVssRequestContext requestContext,
      IArmAdapterServiceConfigurationHelper armAdapterServiceConfigurationHelper)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      this.ReadArmSettings(requestContext, service);
      this.ReadRateCardSettings(requestContext, service);
      this.ArmConfigHelper = armAdapterServiceConfigurationHelper;
      this.MethodQueryFallbackDeclaration = this.InitiliazeMethodConfig(this.ArmConfigHelper);
    }

    private void ReadArmSettings(
      IVssRequestContext requestContext,
      IVssRegistryService registryService)
    {
      RegistryEntryCollection registryEntryCollection = registryService.ReadEntries(requestContext, ServiceSettings.ResourceManagerRegistryQuery);
      this.ArmBaseUri = new Uri(registryEntryCollection.GetValueFromPath<string>("BaseUrl", "https://management.azure.com/"));
      this.ArmRequestTimeOut = registryEntryCollection.GetValueFromPath<int>("RequestTimeOut", 90000);
      this.armAgreementApiVersion = registryEntryCollection.GetValueFromPath<string>("AgreementApiVersion", "2015-06-01");
      this.armApiVersion = registryEntryCollection.GetValueFromPath<string>("ApiVersion", "2014-04-01-preview");
      this.armResourceAccountApiVersion = registryEntryCollection.GetValueFromPath<string>("ResourceApiVersion", "2014-04-01-preview");
      this.armResourceAccountExtensionApiVersion = this.armResourceAccountApiVersion;
      this.armResourceGroupApiVersion = registryEntryCollection.GetValueFromPath<string>("ResourceGroupApiVersion", "2016-02-01");
      this.classicAdministratorsVersion = this.armApiVersion;
      this.registerApiVersion = this.armApiVersion;
      this.roleAssignmentsVersion = this.armApiVersion;
      this.roleDefinitionsVersion = this.armApiVersion;
      if (this.ArmRequestTimeOut > 0)
        return;
      this.ArmRequestTimeOut = 90000;
    }

    private void ReadRateCardSettings(
      IVssRequestContext requestContext,
      IVssRegistryService registryService)
    {
      RegistryEntryCollection registryEntryCollection = registryService.ReadEntries(requestContext, ServiceSettings.RateCardRegistryQuery);
      this.RateCardBaseUri = new Uri(registryEntryCollection.GetValueFromPath<string>("BaseUrl", "https://management.azure.com/"));
      this.rateCardApiVersion = registryEntryCollection.GetValueFromPath<string>("ApiVersion", "2015-06-01-preview");
    }

    public Uri ArmBaseUri { get; private set; }

    public Uri RateCardBaseUri { get; private set; }

    public int ArmRequestTimeOut { get; private set; }

    public string ArmApiVersion(IVssRequestContext requestContext) => this.QueryConfigFrameworkOrFallback(requestContext, nameof (ArmApiVersion));

    public string RegisterApiVersion(IVssRequestContext requestContext) => this.QueryConfigFrameworkOrFallback(requestContext, nameof (RegisterApiVersion));

    public string ClassicAdministratorsVersion(IVssRequestContext requestContext) => this.QueryConfigFrameworkOrFallback(requestContext, nameof (ClassicAdministratorsVersion));

    public string RoleAssignmentsVersion(IVssRequestContext requestContext) => this.QueryConfigFrameworkOrFallback(requestContext, nameof (RoleAssignmentsVersion));

    public string RoleDefinitionsVersion(IVssRequestContext requestContext) => this.QueryConfigFrameworkOrFallback(requestContext, nameof (RoleDefinitionsVersion));

    public string ArmResourceGroupApiVersion(IVssRequestContext requestContext) => this.QueryConfigFrameworkOrFallback(requestContext, nameof (ArmResourceGroupApiVersion));

    public string ArmResourceAccountExtensionsApiVersion(IVssRequestContext requestContext) => this.QueryConfigFrameworkOrFallback(requestContext, nameof (ArmResourceAccountExtensionsApiVersion));

    public string ArmResourceAccountApiVersion(IVssRequestContext requestContext) => this.QueryConfigFrameworkOrFallback(requestContext, nameof (ArmResourceAccountApiVersion));

    public string ArmAgreementApiVersion(IVssRequestContext requestContext) => this.QueryConfigFrameworkOrFallback(requestContext, nameof (ArmAgreementApiVersion));

    public string RateCardApiVersion(IVssRequestContext requestContext) => this.QueryConfigFrameworkOrFallback(requestContext, nameof (RateCardApiVersion));

    private string QueryConfigFrameworkOrFallback(IVssRequestContext requestContext, [CallerMemberName] string method = null) => this.ArmConfigHelper.ArmApiEndpointVersionsUpdateEnabled(requestContext) ? this.MethodQueryFallbackDeclaration[method].QueryFunction(requestContext) : this.MethodQueryFallbackDeclaration[method].DefaultValue;

    private IDictionary<string, ServiceSettings.ConfigQueryOrDefault> InitiliazeMethodConfig(
      IArmAdapterServiceConfigurationHelper armConfigHelper)
    {
      return (IDictionary<string, ServiceSettings.ConfigQueryOrDefault>) new Dictionary<string, ServiceSettings.ConfigQueryOrDefault>()
      {
        {
          "ArmAgreementApiVersion",
          new ServiceSettings.ConfigQueryOrDefault(new Func<IVssRequestContext, string>(armConfigHelper.ArmAgreementApiVersion), this.armAgreementApiVersion)
        },
        {
          "ArmApiVersion",
          new ServiceSettings.ConfigQueryOrDefault(new Func<IVssRequestContext, string>(armConfigHelper.ArmApiVersion), this.armApiVersion)
        },
        {
          "ArmResourceAccountApiVersion",
          new ServiceSettings.ConfigQueryOrDefault(new Func<IVssRequestContext, string>(armConfigHelper.ArmResourceAccountApiVersion), this.armResourceAccountApiVersion)
        },
        {
          "ArmResourceAccountExtensionsApiVersion",
          new ServiceSettings.ConfigQueryOrDefault(new Func<IVssRequestContext, string>(armConfigHelper.ArmResourceAccountExtensionsApiVersion), this.armResourceAccountExtensionApiVersion)
        },
        {
          "ArmResourceGroupApiVersion",
          new ServiceSettings.ConfigQueryOrDefault(new Func<IVssRequestContext, string>(armConfigHelper.ArmResourceGroupApiVersion), this.armResourceGroupApiVersion)
        },
        {
          "ClassicAdministratorsVersion",
          new ServiceSettings.ConfigQueryOrDefault(new Func<IVssRequestContext, string>(armConfigHelper.ClassicAdministratorsVersion), this.classicAdministratorsVersion)
        },
        {
          "RateCardApiVersion",
          new ServiceSettings.ConfigQueryOrDefault(new Func<IVssRequestContext, string>(armConfigHelper.RateCardApiVersion), this.rateCardApiVersion)
        },
        {
          "RegisterApiVersion",
          new ServiceSettings.ConfigQueryOrDefault(new Func<IVssRequestContext, string>(armConfigHelper.RegisterApiVersion), this.registerApiVersion)
        },
        {
          "RoleAssignmentsVersion",
          new ServiceSettings.ConfigQueryOrDefault(new Func<IVssRequestContext, string>(armConfigHelper.RoleAssignmentsVersion), this.roleAssignmentsVersion)
        },
        {
          "RoleDefinitionsVersion",
          new ServiceSettings.ConfigQueryOrDefault(new Func<IVssRequestContext, string>(armConfigHelper.RoleDefinitionsVersion), this.roleDefinitionsVersion)
        }
      };
    }

    private struct ConfigQueryOrDefault
    {
      public ConfigQueryOrDefault(
        Func<IVssRequestContext, string> queryFunction,
        string defaultValue)
      {
        this.QueryFunction = queryFunction;
        this.DefaultValue = defaultValue;
      }

      public Func<IVssRequestContext, string> QueryFunction { get; private set; }

      public string DefaultValue { get; private set; }
    }
  }
}
