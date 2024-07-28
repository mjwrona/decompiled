// Decompiled with JetBrains decompiler
// Type: Windows.Win32.Foundation.PWSTR
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Diagnostics;

namespace Windows.Win32.Foundation
{
  [DebuggerDisplay("{Value}")]
  internal readonly struct PWSTR : IEquatable<PWSTR>
  {
    internal readonly unsafe char* Value;

    internal unsafe PWSTR(char* value) => this.Value = value;

    public static unsafe implicit operator char*(PWSTR value) => value.Value;

    public static unsafe implicit operator PWSTR(char* value) => new PWSTR(value);

    public static unsafe bool operator ==(PWSTR left, PWSTR right) => left.Value == right.Value;

    public static bool operator !=(PWSTR left, PWSTR right) => !(left == right);

    public unsafe bool Equals(PWSTR other) => this.Value == other.Value;

    public override bool Equals(object obj) => obj is PWSTR other && this.Equals(other);

    public override unsafe int GetHashCode() => checked ((int) (UIntPtr) this.Value);

    internal unsafe int Length
    {
      get
      {
        char* chPtr = this.Value;
        if ((IntPtr) chPtr == IntPtr.Zero)
          return 0;
        while (*chPtr != char.MinValue)
          ++chPtr;
        return checked ((int) (chPtr - this.Value));
      }
    }

    public override unsafe string ToString() => (IntPtr) this.Value != IntPtr.Zero ? new string(this.Value) : (string) null;

    internal unsafe Span<char> AsSpan() => (IntPtr) this.Value != IntPtr.Zero ? new Span<char>((void*) this.Value, this.Length) : new Span<char>();
  }
}
