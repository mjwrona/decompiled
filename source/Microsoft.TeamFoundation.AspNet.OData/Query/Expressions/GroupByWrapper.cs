// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Expressions.GroupByWrapper
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Query.Expressions
{
  [JsonConverter(typeof (DynamicTypeWrapperConverter))]
  internal class GroupByWrapper : DynamicTypeWrapper
  {
    protected Dictionary<string, object> _values;
    protected static readonly IPropertyMapper DefaultPropertyMapper = (IPropertyMapper) new IdentityPropertyMapper();

    public virtual AggregationPropertyContainer GroupByContainer { get; set; }

    public virtual AggregationPropertyContainer Container { get; set; }

    public override Dictionary<string, object> Values
    {
      get
      {
        this.EnsureValues();
        return this._values;
      }
    }

    public override bool Equals(object obj)
    {
      if (!(obj is GroupByWrapper groupByWrapper))
        return false;
      Dictionary<string, object> values1 = this.Values;
      Dictionary<string, object> values2 = groupByWrapper.Values;
      return values1.Count<KeyValuePair<string, object>>() == values2.Count<KeyValuePair<string, object>>() && !values1.Except<KeyValuePair<string, object>>((IEnumerable<KeyValuePair<string, object>>) values2).Any<KeyValuePair<string, object>>();
    }

    public override int GetHashCode()
    {
      this.EnsureValues();
      long hashCode = 1870403278;
      foreach (object obj in this.Values.Values)
        hashCode = hashCode * -1521134295L + (obj == null ? 0L : (long) obj.GetHashCode());
      return (int) hashCode;
    }

    protected virtual void EnsureValues()
    {
      if (this._values != null)
        return;
      this._values = this.GroupByContainer == null ? new Dictionary<string, object>() : this.GroupByContainer.ToDictionary(GroupByWrapper.DefaultPropertyMapper);
      if (this.Container == null)
        return;
      this._values.MergeWithReplace<string, object>(this.Container.ToDictionary(GroupByWrapper.DefaultPropertyMapper));
    }
  }
}
