// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FeatureAvailability.StageFeatureStates
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.FeatureAvailability.MessageBus;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server.FeatureAvailability
{
  internal class StageFeatureStates
  {
    public Dictionary<string, List<FeatureFlagSetting>> FeatureFlagsByStage { get; set; }

    public StageFeatureStates(
      FeatureFlagSettingsMessage featureFlagSettingsMessage)
    {
      this.FeatureFlagsByStage = featureFlagSettingsMessage.FeatureFlagsByStage;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("{");
      foreach (KeyValuePair<string, List<FeatureFlagSetting>> keyValuePair in this.FeatureFlagsByStage)
      {
        stringBuilder.Append("Stage : " + keyValuePair.Key);
        foreach (FeatureFlagSetting featureFlagSetting in keyValuePair.Value)
          stringBuilder.Append(string.Format("[FeatureName : {0} FeatureState: {1}]", (object) featureFlagSetting.FeatureFlagName, (object) featureFlagSetting.PreferredState));
      }
      stringBuilder.Append("}");
      return stringBuilder.ToString();
    }
  }
}
