// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricsStreaming.RuleOnboardingAllowDefinition
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Cloud.Metrics.Client.MetricsStreaming
{
  public class RuleOnboardingAllowDefinition
  {
    private const string IdDelimiter = "-";

    [JsonProperty(PropertyName = "id")]
    public string Id => string.Format("{0}{1}{2}", (object) this.Type, (object) "-", (object) this.Name);

    public string Name { get; set; }

    public RuleOnboardingAllowDefinition.ItemType Type { get; set; }

    public List<string> Values { get; set; }

    public enum ItemType
    {
      FIELD,
      CUSTOM_ATTRIBUTES,
    }
  }
}
