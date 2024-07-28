// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.Configuration.DisabledContributionConfigurationHelper
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.Configuration
{
  public class DisabledContributionConfigurationHelper : IDisabledContributionConfigurationHelper
  {
    public const string DisableContributionsAtRuntime = "WebPlatform.DisableContributionsAtRuntime";
    private static readonly IConfigPrototype<HashSet<string>> DisabledContributionsPrototype = ConfigPrototype.Create<HashSet<string>>("WebPlatform.DisabledContributionIds", new HashSet<string>(0));
    public static IDisabledContributionConfigurationHelper Instance = (IDisabledContributionConfigurationHelper) new DisabledContributionConfigurationHelper();
    private readonly IConfigQueryable<HashSet<string>> DisabledContributionsConfig;

    private DisabledContributionConfigurationHelper()
      : this(ConfigProxy.Create<HashSet<string>>(DisabledContributionConfigurationHelper.DisabledContributionsPrototype))
    {
    }

    public DisabledContributionConfigurationHelper(
      IConfigQueryable<HashSet<string>> disabedContributionsConfig)
    {
      this.DisabledContributionsConfig = disabedContributionsConfig;
    }

    public bool IsContributionDisabled(IVssRequestContext requestContext, string contributionId) => this.DisabledContributionsConfig.QueryByCtx<HashSet<string>>(requestContext).Contains(contributionId);
  }
}
