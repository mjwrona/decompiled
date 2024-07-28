// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.KpiMetric
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class KpiMetric
  {
    public KpiMetric() => this.TimeStamp = DateTime.UtcNow;

    public KpiMetric(string name, double value)
      : this()
    {
      this.Name = name;
      this.Value = value;
    }

    public string Name { get; set; }

    public double Value { get; set; }

    public DateTime TimeStamp { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Name: {0}; Value {1}; TimeStamp: {2}", (object) this.Name, (object) this.Value, (object) this.TimeStamp);
  }
}
