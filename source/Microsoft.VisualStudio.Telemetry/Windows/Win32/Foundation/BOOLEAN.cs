// Decompiled with JetBrains decompiler
// Type: Windows.Win32.Foundation.BOOLEAN
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Diagnostics;

namespace Windows.Win32.Foundation
{
  [DebuggerDisplay("{Value}")]
  internal readonly struct BOOLEAN : IEquatable<BOOLEAN>
  {
    internal readonly byte Value;

    internal BOOLEAN(byte value) => this.Value = value;

    public static implicit operator byte(BOOLEAN value) => value.Value;

    public static explicit operator BOOLEAN(byte value) => new BOOLEAN(value);

    public static bool operator ==(BOOLEAN left, BOOLEAN right) => (int) left.Value == (int) right.Value;

    public static bool operator !=(BOOLEAN left, BOOLEAN right) => !(left == right);

    public bool Equals(BOOLEAN other) => (int) this.Value == (int) other.Value;

    public override bool Equals(object obj) => obj is BOOLEAN other && this.Equals(other);

    public override int GetHashCode() => this.Value.GetHashCode();
  }
}
