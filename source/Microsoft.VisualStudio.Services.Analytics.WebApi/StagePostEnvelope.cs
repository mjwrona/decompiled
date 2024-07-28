// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WebApi.StagePostEnvelope
// Assembly: Microsoft.VisualStudio.Services.Analytics.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F858B048-FFE8-4E5F-8EBC-9B25DAC23DF8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Collections;

namespace Microsoft.VisualStudio.Services.Analytics.WebApi
{
  public class StagePostEnvelope
  {
    internal const string ContentVersionProperty = "contentVersion";
    internal const string StageVersionProperty = "stageVersion";
    internal const string FromWatermarkProperty = "fromWatermark";
    internal const string ToWatermarkProperty = "toWatermark";
    internal const string IsCurrentProperty = "isCurrent";
    internal const string SyncDateProperty = "syncDate";
    internal const string KeysOnlyProperty = "keysOnly";
    internal const string ReplaceProperty = "replace";
    internal const string RecordsProperty = "records";
    internal const string MergeRecordsProperty = "mergeRecords";

    [JsonProperty("contentVersion")]
    public int ContentVersion { get; set; }

    [JsonProperty("stageVersion")]
    public int StageVersion { get; set; }

    [JsonProperty("fromWatermark")]
    public string FromWatermark { get; set; }

    [JsonProperty("toWatermark")]
    public string ToWatermark { get; set; }

    [JsonProperty("isCurrent")]
    public bool IsCurrent { get; set; } = true;

    [JsonProperty("syncDate")]
    public DateTime? SyncDate { get; set; }

    [JsonProperty("keysOnly", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool KeysOnly { get; set; }

    [JsonProperty("replace", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public bool Replace { get; set; }

    [JsonProperty("records")]
    public IList Records { get; set; }

    [JsonIgnore]
    public IList MergeRecords
    {
      get => this.Records;
      set => this.Records = value;
    }
  }
}
