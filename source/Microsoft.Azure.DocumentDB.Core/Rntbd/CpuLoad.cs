// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.CpuLoad
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal struct CpuLoad
  {
    public DateTime Timestamp;
    public float Value;

    public CpuLoad(DateTime timestamp, float value)
    {
      if ((double) value < 0.0 || (double) value > 100.0)
        throw new ArgumentOutOfRangeException(nameof (value), (object) value, "Valid CPU load values must be between 0.0 and 100.0");
      this.Timestamp = timestamp;
      this.Value = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0:O} {1:F3})", (object) this.Timestamp, (object) this.Value);
  }
}
