// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryComplexProperty
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Telemetry
{
  public class TelemetryComplexProperty
  {
    public object Value { get; }

    public TelemetryComplexProperty(object val)
    {
      val.RequiresArgumentNotNull<object>(nameof (val));
      this.Value = val;
    }

    [ExcludeFromCodeCoverage]
    public override string ToString() => string.Format("Complex({0})", this.Value);
  }
}
