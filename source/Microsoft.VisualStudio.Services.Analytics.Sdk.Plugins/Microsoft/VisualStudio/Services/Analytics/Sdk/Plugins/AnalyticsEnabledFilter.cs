// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins.AnalyticsEnabledFilter
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3E9FDCC8-8891-4D47-89A2-C972B6459647
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins.dll

using Microsoft.TeamFoundation.Analytics.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Newtonsoft.Json.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins
{
  public class AnalyticsEnabledFilter : ContributionFilter<AnalyticsEnabledFilterProperties>
  {
    public override string Name => "AnalyticsEnabled";

    public override AnalyticsEnabledFilterProperties ParseProperties(JObject properties)
    {
      AnalyticsEnabledFilterProperties properties1 = new AnalyticsEnabledFilterProperties();
      bool flag1;
      if (properties.TryGetValue<bool>("inverse", out flag1))
        properties1.Inverse = flag1;
      bool flag2;
      if (properties.TryGetValue<bool>("treatPausedAsEnabled", out flag2))
        properties1.TreatPausedAsEnabled = flag2;
      return properties1;
    }

    public override bool ApplyConstraint(
      IVssRequestContext requestContext,
      AnalyticsEnabledFilterProperties properties,
      Contribution contribution)
    {
      bool flag = requestContext.GetService<IAnalyticsFeatureService>().IsAnalyticsEnabled(requestContext, properties.TreatPausedAsEnabled);
      return !properties.Inverse ? flag : !flag;
    }
  }
}
