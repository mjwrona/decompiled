// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Charting.WebApi.TransformOptions
// Assembly: Microsoft.TeamFoundation.Charting.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D43D663A-A882-4856-B0B7-D0E666C5CC4A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Charting.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Charting.WebApi
{
  [DataContract]
  public class TransformOptions : SecuredChartObject, ICloneable
  {
    public object Clone()
    {
      TransformOptions transformOptions = this.MemberwiseClone() as TransformOptions;
      transformOptions.OrderBy = new OrderBy()
      {
        Direction = this.OrderBy.Direction,
        PropertyName = this.OrderBy.PropertyName
      };
      transformOptions.Measure = new Measure()
      {
        Aggregation = this.Measure.Aggregation,
        PropertyName = this.Measure.PropertyName
      };
      return (object) transformOptions;
    }

    [DataMember]
    public Guid? TransformId { get; set; }

    [DataMember]
    public string Filter { get; set; }

    [DataMember]
    public string HistoryRange { get; set; }

    [DataMember]
    public string GroupBy { get; set; }

    [DataMember]
    public string[] FilteredGroups { get; set; }

    [DataMember]
    public bool IsRowColFilterAllowedForChartType { get; set; }

    [DataMember]
    public string[] RowFilteredGroups { get; set; }

    [DataMember]
    public string[] ColumnFilteredGroups { get; set; }

    [DataMember]
    public string Series { get; set; }

    [DataMember]
    public OrderBy OrderBy { get; set; }

    [DataMember]
    public Measure Measure { get; set; }

    [DataMember]
    public IDictionary FilterContext { get; set; }

    [DataMember(IsRequired = false)]
    public bool GetFilteredQuery { get; set; }

    protected override void UpdateSecuredObjectOfChildren(ISecuredObject securedObject)
    {
      this.OrderBy.SetSecuredObject(securedObject);
      this.Measure.SetSecuredObject(securedObject);
    }

    public bool IsSemanticEqual(TransformOptions target)
    {
      Guid? transformId1 = this.TransformId;
      Guid? transformId2 = target.TransformId;
      return (transformId1.HasValue == transformId2.HasValue ? (transformId1.HasValue ? (transformId1.GetValueOrDefault() == transformId2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && this.Filter == target.Filter && this.HistoryRange == target.HistoryRange && this.GroupBy == target.GroupBy && this.Series == target.Series && this.OrderBy.Direction == target.OrderBy.Direction && this.OrderBy.PropertyName == target.OrderBy.PropertyName && this.Measure.Aggregation == target.Measure.Aggregation && this.Measure.PropertyName == target.Measure.PropertyName && this.FilterContext == target.FilterContext;
    }
  }
}
