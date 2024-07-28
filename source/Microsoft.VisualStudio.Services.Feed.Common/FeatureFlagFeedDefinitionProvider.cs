// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.FeatureFlagFeedDefinitionProvider
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  public static class FeatureFlagFeedDefinitionProvider
  {
    public static FeatureFlagFeedSettingDefinition Cargo = new FeatureFlagFeedSettingDefinition("Packaging.Feed.Cargo.EnableUI", "Packaging.Feed.Cargo.EnableUI.MSFT");
    public static FeatureFlagFeedSettingDefinition Conda = new FeatureFlagFeedSettingDefinition("Packaging.Feed.Conda.EnableUI", "Packaging.Feed.Conda.EnableUI.MSFT");

    public static FeatureFlagFeedSettingDefinition Get(string protocol)
    {
      FeatureFlagFeedSettingDefinition settingDefinition;
      switch (protocol.ToLowerInvariant())
      {
        case "cargo":
          settingDefinition = FeatureFlagFeedDefinitionProvider.Cargo;
          break;
        case "conda":
          settingDefinition = FeatureFlagFeedDefinitionProvider.Conda;
          break;
        default:
          settingDefinition = (FeatureFlagFeedSettingDefinition) null;
          break;
      }
      return settingDefinition;
    }
  }
}
