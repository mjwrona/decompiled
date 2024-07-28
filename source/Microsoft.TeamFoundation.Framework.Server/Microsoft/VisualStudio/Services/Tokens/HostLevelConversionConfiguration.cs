// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tokens.HostLevelConversionConfiguration
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;

namespace Microsoft.VisualStudio.Services.Tokens
{
  public class HostLevelConversionConfiguration : IHostLevelConversionConfiguration
  {
    public static readonly IHostLevelConversionConfiguration Instance;
    private static readonly IConfigPrototype<bool> EnableExchangeTokenFlowHostLevelConvertPrototype = ConfigPrototype.Create<bool>("EnableExchangeTokenFlowHostLevelConvert.M227", false);
    private readonly IConfigQueryable<bool> EnableExchangeTokenFlowHostLevelConvertConfig;
    private static readonly IConfigPrototype<bool> EnableSessionTokenFlowHostLevelConvertPrototype = ConfigPrototype.Create<bool>("EnableSessionTokenFlowHostLevelConvert.M227", false);
    private readonly IConfigQueryable<bool> EnableSessionTokenFlowHostLevelConvertConfig;
    private static readonly IConfigPrototype<bool> EnableTokenAdminFlowHostLevelConvertPrototype = ConfigPrototype.Create<bool>("EnableTokenAdminFlowHostLevelConvert.M228", false);
    private readonly IConfigQueryable<bool> EnableTokenAdminFlowHostLevelConvertConfig;

    static HostLevelConversionConfiguration() => HostLevelConversionConfiguration.Instance = (IHostLevelConversionConfiguration) new HostLevelConversionConfiguration();

    public HostLevelConversionConfiguration(
      IConfigQueryable<bool> exchangeConfig,
      IConfigQueryable<bool> sessionConfig,
      IConfigQueryable<bool> adminConfig)
    {
      this.EnableExchangeTokenFlowHostLevelConvertConfig = exchangeConfig;
      this.EnableSessionTokenFlowHostLevelConvertConfig = sessionConfig;
      this.EnableTokenAdminFlowHostLevelConvertConfig = adminConfig;
    }

    private HostLevelConversionConfiguration()
      : this(ConfigProxy.Create<bool>(HostLevelConversionConfiguration.EnableExchangeTokenFlowHostLevelConvertPrototype), ConfigProxy.Create<bool>(HostLevelConversionConfiguration.EnableSessionTokenFlowHostLevelConvertPrototype), ConfigProxy.Create<bool>(HostLevelConversionConfiguration.EnableTokenAdminFlowHostLevelConvertPrototype))
    {
    }

    public bool IsEnabledForExchangeTokenFlow(IVssRequestContext requestContext) => this.EnableExchangeTokenFlowHostLevelConvertConfig.QueryByCtx<bool>(requestContext);

    public bool IsEnabledForSessionTokenFlow(IVssRequestContext requestContext) => this.EnableSessionTokenFlowHostLevelConvertConfig.QueryByCtx<bool>(requestContext);

    public bool IsEnabledForTokenAdminFlow(IVssRequestContext requestContext) => this.EnableTokenAdminFlowHostLevelConvertConfig.QueryByCtx<bool>(requestContext);
  }
}
