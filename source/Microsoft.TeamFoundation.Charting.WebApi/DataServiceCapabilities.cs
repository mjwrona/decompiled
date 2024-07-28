// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Charting.WebApi.DataServiceCapabilities
// Assembly: Microsoft.TeamFoundation.Charting.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D43D663A-A882-4856-B0B7-D0E666C5CC4A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Charting.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Charting.WebApi
{
  [DataContract]
  public class DataServiceCapabilities
  {
    [DataMember]
    public string Scope { get; set; }

    [DataMember]
    public string PluralArtifactName { get; set; }

    [DataMember]
    public IEnumerable<NameLabelPair> NumericalAggregationFunctions { get; set; }

    [DataMember]
    public IEnumerable<NameLabelPair> HistoryRanges { get; set; }

    [DataMember]
    public IEnumerable<FieldInfo> Fields { get; set; }
  }
}
