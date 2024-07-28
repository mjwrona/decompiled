// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ArmAdapterServiceConfigurationHelper
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class ArmAdapterServiceConfigurationHelper : IArmAdapterServiceConfigurationHelper
  {
    private static readonly IConfigPrototype<bool> ArmApiEndpointVersionsUpdateEnablePrototype = ConfigPrototype.Create<bool>("Commerce.ArmApiEndpointVersionsUpdateEnable", false);
    private static readonly IConfigPrototype<string> ArmAgreementApiVersionPrototype = ConfigPrototype.Create<string>("Commerce.ArmAgreementApiVersion", "2014-04-01-preview");
    private static readonly IConfigPrototype<string> ArmApiVersionPrototype = ConfigPrototype.Create<string>("Commerce.ArmApiVersion", "2014-04-01-preview");
    private static readonly IConfigPrototype<string> ArmResourceAccountApiVersionPrototype = ConfigPrototype.Create<string>("Commerce.ArmResourceAccountApiVersion", "2014-04-01-preview");
    private static readonly IConfigPrototype<string> ArmResourceAccountExtensionsApiVersionPrototype = ConfigPrototype.Create<string>("Commerce.ArmResourceAccountExtensionsApiVersion", "2014-04-01-preview");
    private static readonly IConfigPrototype<string> ArmResourceGroupApiVersionPrototype = ConfigPrototype.Create<string>("Commerce.ArmResourceGroupApiVersion", "2014-04-01-preview");
    private static readonly IConfigPrototype<string> ClassicAdministratorsVersionPrototype = ConfigPrototype.Create<string>("Commerce.ClassicAdministratorsVersion", "2014-04-01-preview");
    private static readonly IConfigPrototype<string> RateCardApiVersionPrototype = ConfigPrototype.Create<string>("Commerce.RateCardApiVersion", "2014-04-01-preview");
    private static readonly IConfigPrototype<string> RegisterApiVersionPrototype = ConfigPrototype.Create<string>("Commerce.RegisterApiVersion", "2014-04-01-preview");
    private static readonly IConfigPrototype<string> RoleAssignmentsVersionPrototype = ConfigPrototype.Create<string>("Commerce.RoleAssignmentsVersion", "2014-04-01-preview");
    private static readonly IConfigPrototype<string> RoleDefinitionsVersionPrototype = ConfigPrototype.Create<string>("Commerce.RoleDefinitionsVersion", "2014-04-01-preview");
    private readonly IConfigQueryable<bool> ArmApiEndpointVersionsUpdateEnableConfig;
    private readonly IConfigQueryable<string> ArmAgreementApiVersionConfig;
    private readonly IConfigQueryable<string> ArmApiVersionConfig;
    private readonly IConfigQueryable<string> ArmResourceAccountApiVersionConfig;
    private readonly IConfigQueryable<string> ArmResourceAccountExtensionsApiVersionConfig;
    private readonly IConfigQueryable<string> ArmResourceGroupApiVersionConfig;
    private readonly IConfigQueryable<string> ClassicAdministratorsVersionConfig;
    private readonly IConfigQueryable<string> RateCardApiVersionConfig;
    private readonly IConfigQueryable<string> RegisterApiVersionConfig;
    private readonly IConfigQueryable<string> RoleAssignmentsVersionConfig;
    private readonly IConfigQueryable<string> RoleDefinitionsVersionConfig;

    internal ArmAdapterServiceConfigurationHelper()
      : this(ConfigProxy.Create<bool>(ArmAdapterServiceConfigurationHelper.ArmApiEndpointVersionsUpdateEnablePrototype), ConfigProxy.Create<string>(ArmAdapterServiceConfigurationHelper.ArmAgreementApiVersionPrototype), ConfigProxy.Create<string>(ArmAdapterServiceConfigurationHelper.ArmApiVersionPrototype), ConfigProxy.Create<string>(ArmAdapterServiceConfigurationHelper.ArmResourceAccountApiVersionPrototype), ConfigProxy.Create<string>(ArmAdapterServiceConfigurationHelper.ArmResourceAccountExtensionsApiVersionPrototype), ConfigProxy.Create<string>(ArmAdapterServiceConfigurationHelper.ArmResourceGroupApiVersionPrototype), ConfigProxy.Create<string>(ArmAdapterServiceConfigurationHelper.ClassicAdministratorsVersionPrototype), ConfigProxy.Create<string>(ArmAdapterServiceConfigurationHelper.RateCardApiVersionPrototype), ConfigProxy.Create<string>(ArmAdapterServiceConfigurationHelper.RegisterApiVersionPrototype), ConfigProxy.Create<string>(ArmAdapterServiceConfigurationHelper.RoleAssignmentsVersionPrototype), ConfigProxy.Create<string>(ArmAdapterServiceConfigurationHelper.RoleDefinitionsVersionPrototype))
    {
    }

    internal ArmAdapterServiceConfigurationHelper(
      IConfigQueryable<bool> armApiEndpointVersionsUpdateEnableConfig,
      IConfigQueryable<string> armAgreementApiVersionConfig,
      IConfigQueryable<string> armApiVersionConfig,
      IConfigQueryable<string> armResourceAccountApiVersionConfig,
      IConfigQueryable<string> armResourceAccountExtensionsApiVersionConfig,
      IConfigQueryable<string> armResourceGroupApiVersionConfig,
      IConfigQueryable<string> classicAdministratorsVersionConfig,
      IConfigQueryable<string> rateCardApiVersionConfig,
      IConfigQueryable<string> registerApiVersionConfig,
      IConfigQueryable<string> roleAssignmentsVersionConfig,
      IConfigQueryable<string> roleDefinitionsVersionConfig)
    {
      this.ArmApiEndpointVersionsUpdateEnableConfig = armApiEndpointVersionsUpdateEnableConfig;
      this.ArmAgreementApiVersionConfig = armAgreementApiVersionConfig;
      this.ArmApiVersionConfig = armApiVersionConfig;
      this.ArmResourceAccountApiVersionConfig = armResourceAccountExtensionsApiVersionConfig;
      this.ArmResourceAccountApiVersionConfig = armResourceAccountApiVersionConfig;
      this.ArmResourceAccountExtensionsApiVersionConfig = armResourceAccountExtensionsApiVersionConfig;
      this.ArmResourceGroupApiVersionConfig = armResourceGroupApiVersionConfig;
      this.ClassicAdministratorsVersionConfig = classicAdministratorsVersionConfig;
      this.RateCardApiVersionConfig = rateCardApiVersionConfig;
      this.RegisterApiVersionConfig = registerApiVersionConfig;
      this.RoleAssignmentsVersionConfig = roleAssignmentsVersionConfig;
      this.RoleDefinitionsVersionConfig = roleDefinitionsVersionConfig;
    }

    public bool ArmApiEndpointVersionsUpdateEnabled(IVssRequestContext requestContext) => this.ArmApiEndpointVersionsUpdateEnableConfig.QueryByCtx<bool>(requestContext);

    public string ArmAgreementApiVersion(IVssRequestContext requestContext) => this.ArmAgreementApiVersionConfig.QueryByCtx<string>(requestContext);

    public string ArmApiVersion(IVssRequestContext requestContext) => this.ArmApiVersionConfig.QueryByCtx<string>(requestContext);

    public string ArmResourceAccountApiVersion(IVssRequestContext requestContext) => this.ArmResourceAccountApiVersionConfig.QueryByCtx<string>(requestContext);

    public string ArmResourceAccountExtensionsApiVersion(IVssRequestContext requestContext) => this.ArmResourceAccountExtensionsApiVersionConfig.QueryByCtx<string>(requestContext);

    public string ArmResourceGroupApiVersion(IVssRequestContext requestContext) => this.ArmResourceGroupApiVersionConfig.QueryByCtx<string>(requestContext);

    public string ClassicAdministratorsVersion(IVssRequestContext requestContext) => this.ClassicAdministratorsVersionConfig.QueryByCtx<string>(requestContext);

    public string RateCardApiVersion(IVssRequestContext requestContext) => this.RateCardApiVersionConfig.QueryByCtx<string>(requestContext);

    public string RegisterApiVersion(IVssRequestContext requestContext) => this.RegisterApiVersionConfig.QueryByCtx<string>(requestContext);

    public string RoleAssignmentsVersion(IVssRequestContext requestContext) => this.RoleAssignmentsVersionConfig.QueryByCtx<string>(requestContext);

    public string RoleDefinitionsVersion(IVssRequestContext requestContext) => this.RoleDefinitionsVersionConfig.QueryByCtx<string>(requestContext);
  }
}
