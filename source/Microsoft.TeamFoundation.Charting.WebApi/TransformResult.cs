// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Charting.WebApi.TransformResult
// Assembly: Microsoft.TeamFoundation.Charting.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D43D663A-A882-4856-B0B7-D0E666C5CC4A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Charting.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Charting.WebApi
{
  [DataContract]
  public class TransformResult : SecuredChartObject
  {
    [DataMember]
    public IEnumerable<AggregatedRecord> Records { get; set; }

    [DataMember]
    public TransformOptions Options { get; set; }

    [DataMember]
    public bool AreHistoryOptionsUnmodified { get; set; }

    protected override void UpdateSecuredObjectOfChildren(ISecuredObject securedObject)
    {
      TransformOptions transformOptions = this.Options.Clone() as TransformOptions;
      transformOptions.SetSecuredObject(securedObject);
      this.Options = transformOptions;
      List<AggregatedRecord> aggregatedRecordList = new List<AggregatedRecord>(this.Records);
      foreach (SecuredChartObject securedChartObject in aggregatedRecordList)
        securedChartObject.SetSecuredObject(securedObject);
      this.Records = (IEnumerable<AggregatedRecord>) aggregatedRecordList;
    }
  }
}
