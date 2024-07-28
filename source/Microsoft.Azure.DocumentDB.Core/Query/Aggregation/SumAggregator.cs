// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.Aggregation.SumAggregator
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.Documents.Query.Aggregation
{
  internal sealed class SumAggregator : IAggregator
  {
    private double globalSum;

    public void Aggregate(object localSum)
    {
      if (Undefined.Value.Equals(localSum))
        this.globalSum = double.NaN;
      else
        this.globalSum += Convert.ToDouble(localSum, (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public object GetResult() => double.IsNaN(this.globalSum) ? (object) Undefined.Value : (object) this.globalSum;
  }
}
