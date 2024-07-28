// Decompiled with JetBrains decompiler
// Type: Windows.Win32.Foundation.HANDLE
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Diagnostics;

namespace Windows.Win32.Foundation
{
  [DebuggerDisplay("{Value}")]
  internal readonly struct HANDLE : IEquatable<HANDLE>
  {
    internal readonly IntPtr Value;

    internal HANDLE(IntPtr value) => this.Value = value;

    internal bool IsNull => this.Value == IntPtr.Zero;

    public static implicit operator IntPtr(HANDLE value) => value.Value;

    public static explicit operator HANDLE(IntPtr value) => new HANDLE(value);

    public static bool operator ==(HANDLE left, HANDLE right) => left.Value == right.Value;

    public static bool operator !=(HANDLE left, HANDLE right) => !(left == right);

    public bool Equals(HANDLE other) => this.Value == other.Value;

    public override bool Equals(object obj) => obj is HANDLE other && this.Equals(other);

    public override int GetHashCode() => this.Value.GetHashCode();
  }
}
