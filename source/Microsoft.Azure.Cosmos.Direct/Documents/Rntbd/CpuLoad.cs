// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.CpuLoad
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

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
      this.Timestamp = timestamp;
      this.Value = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0:O} {1:F3})", (object) this.Timestamp, (object) this.Value);
  }
}
