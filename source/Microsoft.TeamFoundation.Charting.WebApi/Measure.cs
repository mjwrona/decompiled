// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Charting.WebApi.Measure
// Assembly: Microsoft.TeamFoundation.Charting.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D43D663A-A882-4856-B0B7-D0E666C5CC4A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Charting.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Charting.WebApi
{
  [DataContract]
  public class Measure : SecuredChartObject
  {
    [DataMember]
    public string PropertyName { get; set; }

    [DataMember]
    public string Aggregation { get; set; }

    public bool IsPropertyNeeded() => !string.Equals("count", this.Aggregation);
  }
}
