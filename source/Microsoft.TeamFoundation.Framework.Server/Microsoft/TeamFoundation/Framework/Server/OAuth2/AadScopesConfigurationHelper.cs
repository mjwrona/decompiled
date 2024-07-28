// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.AadScopesConfigurationHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  public class AadScopesConfigurationHelper : IAadScopesConfigurationHelper
  {
    public static IAadScopesConfigurationHelper Instance;
    private static readonly IConfigPrototype<bool> EnableAadScopesPrototype = ConfigPrototype.Create<bool>("Identity.OAuth2.EnableAadScopesAndRoles.M214", false);
    private readonly IConfigQueryable<bool> EnableAadScopesConfig;

    static AadScopesConfigurationHelper() => AadScopesConfigurationHelper.Instance = (IAadScopesConfigurationHelper) new AadScopesConfigurationHelper();

    public AadScopesConfigurationHelper(IConfigQueryable<bool> config) => this.EnableAadScopesConfig = config;

    private AadScopesConfigurationHelper()
      : this(ConfigProxy.Create<bool>(AadScopesConfigurationHelper.EnableAadScopesPrototype))
    {
    }

    public bool IsAadScopesEnabled(IVssRequestContext requestContext) => this.EnableAadScopesConfig.QueryByCtx<bool>(requestContext);
  }
}
