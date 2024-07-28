// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryHashedProperty
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;

namespace Microsoft.VisualStudio.Telemetry
{
  public class TelemetryHashedProperty
  {
    public string StringValue { get; }

    public object RawValue { get; }

    public TelemetryHashedProperty(object val)
    {
      val.RequiresArgumentNotNull<object>(nameof (val));
      this.RawValue = val;
      this.StringValue = TypeTools.ConvertToString(val);
    }

    public override string ToString() => string.Format("Hashed({0})", this.RawValue);
  }
}
