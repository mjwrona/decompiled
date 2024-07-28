// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Definition
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public class Definition
  {
    [JsonProperty("DisplayText")]
    public string DisplayText { get; set; }

    [JsonProperty("LocalizedValues")]
    public object LocalizedValues { get; set; }

    [JsonProperty("plans")]
    public List<OfferPlan> Plans { get; set; }
  }
}
