// Decompiled with JetBrains decompiler
// Type: Windows.Win32.Foundation.HINSTANCE
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Diagnostics;

namespace Windows.Win32.Foundation
{
  [DebuggerDisplay("{Value}")]
  internal readonly struct HINSTANCE : IEquatable<HINSTANCE>
  {
    internal readonly IntPtr Value;

    internal HINSTANCE(IntPtr value) => this.Value = value;

    internal bool IsNull => this.Value == IntPtr.Zero;

    public static implicit operator IntPtr(HINSTANCE value) => value.Value;

    public static explicit operator HINSTANCE(IntPtr value) => new HINSTANCE(value);

    public static bool operator ==(HINSTANCE left, HINSTANCE right) => left.Value == right.Value;

    public static bool operator !=(HINSTANCE left, HINSTANCE right) => !(left == right);

    public bool Equals(HINSTANCE other) => this.Value == other.Value;

    public override bool Equals(object obj) => obj is HINSTANCE other && this.Equals(other);

    public override int GetHashCode() => this.Value.GetHashCode();
  }
}
