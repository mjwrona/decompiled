// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.FeatureFlagFeedSettingDefinition
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Organization;

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  public class FeatureFlagFeedSettingDefinition
  {
    public string FeatureName { get; }

    public string InternalFeatureFlagName { get; }

    public FeatureFlagFeedSettingDefinition(string featureName, string internalFeatureFlagName = null)
    {
      this.FeatureName = featureName;
      this.InternalFeatureFlagName = internalFeatureFlagName;
    }

    public bool IsEnabled(IVssRequestContext requestContext)
    {
      if (requestContext.IsFeatureEnabled(this.FeatureName))
        return true;
      return requestContext.IsMicrosoftTenant() && requestContext.IsFeatureEnabled(this.InternalFeatureFlagName);
    }
  }
}
