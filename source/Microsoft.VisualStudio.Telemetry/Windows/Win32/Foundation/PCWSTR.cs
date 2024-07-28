// Decompiled with JetBrains decompiler
// Type: Windows.Win32.Foundation.PCWSTR
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;

namespace Windows.Win32.Foundation
{
  [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay}")]
  internal readonly struct PCWSTR : IEquatable<PCWSTR>
  {
    internal readonly unsafe char* Value;

    internal unsafe PCWSTR(char* value) => this.Value = value;

    public static unsafe explicit operator char*(PCWSTR value) => value.Value;

    public static unsafe implicit operator PCWSTR(char* value) => new PCWSTR(value);

    public static unsafe implicit operator PCWSTR(PWSTR value) => new PCWSTR(value.Value);

    public unsafe bool Equals(PCWSTR other) => this.Value == other.Value;

    public override bool Equals(object obj) => obj is PCWSTR other && this.Equals(other);

    public override unsafe int GetHashCode() => (int) this.Value;

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

    internal unsafe ReadOnlySpan<char> AsSpan() => (IntPtr) this.Value != IntPtr.Zero ? new ReadOnlySpan<char>((void*) this.Value, this.Length) : new ReadOnlySpan<char>();

    private string DebuggerDisplay => this.ToString();
  }
}
