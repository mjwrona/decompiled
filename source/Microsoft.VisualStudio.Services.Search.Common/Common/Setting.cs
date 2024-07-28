// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Setting
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract]
  [JsonObject]
  [Serializable]
  public class Setting
  {
    [DataMember]
    [JsonProperty(PropertyName = "title")]
    public string Title { get; set; }

    [DataMember]
    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; }

    [DataMember]
    [JsonProperty(PropertyName = "routeId")]
    public string RouteId { get; set; }

    [DataMember]
    [JsonProperty(PropertyName = "routeParameterMapping")]
    public string RouteParameterMapping { get; set; }

    [DataMember]
    [JsonProperty(PropertyName = "icon")]
    public string Icon { get; set; }

    [DataMember]
    [JsonProperty(PropertyName = "scope")]
    public string Scope { get; set; }

    [DataMember]
    [JsonProperty(PropertyName = "tags")]
    public IEnumerable<string> Tags { get; set; }

    public static string GetSettingId(string title, string scope) => FormattableString.Invariant(FormattableStringFactory.Create("{0}-{1}", (object) scope.ToLowerInvariant(), (object) title.Replace(" ", string.Empty).ToLowerInvariant()));
  }
}
