// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.Aggregation.MinMaxAggregator
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.Documents.Query.Aggregation
{
  internal sealed class MinMaxAggregator : IAggregator
  {
    private readonly bool isMinAggregation;
    private object globalMinMax;

    public MinMaxAggregator(bool isMinAggregation)
    {
      this.isMinAggregation = isMinAggregation;
      if (this.isMinAggregation)
        this.globalMinMax = (object) ItemComparer.MaxValue;
      else
        this.globalMinMax = (object) ItemComparer.MinValue;
    }

    public void Aggregate(object item)
    {
      if (this.globalMinMax == Undefined.Value)
        return;
      if (item == Undefined.Value)
      {
        this.globalMinMax = (object) Undefined.Value;
      }
      else
      {
        if (item is JObject jobject)
        {
          JToken jtoken1 = jobject["count"];
          if (jtoken1 != null)
          {
            if (jtoken1.ToObject<long>() == 0L)
              return;
            JToken jtoken2 = jobject["min"];
            JToken jtoken3 = jobject["max"];
            item = jtoken2 == null ? (jtoken3 == null ? (object) Undefined.Value : jtoken3.ToObject<object>()) : jtoken2.ToObject<object>();
          }
        }
        if (!ItemComparer.IsMinOrMax(this.globalMinMax) && (!ItemTypeHelper.IsPrimitive(item) || !ItemTypeHelper.IsPrimitive(this.globalMinMax)))
          this.globalMinMax = (object) Undefined.Value;
        else if (this.isMinAggregation)
        {
          if (ItemComparer.Instance.Compare(item, this.globalMinMax) >= 0)
            return;
          this.globalMinMax = item;
        }
        else
        {
          if (ItemComparer.Instance.Compare(item, this.globalMinMax) <= 0)
            return;
          this.globalMinMax = item;
        }
      }
    }

    public object GetResult() => this.globalMinMax == ItemComparer.MinValue || this.globalMinMax == ItemComparer.MaxValue ? (object) Undefined.Value : this.globalMinMax;
  }
}
