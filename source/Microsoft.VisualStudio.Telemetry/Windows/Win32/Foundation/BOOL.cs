// Decompiled with JetBrains decompiler
// Type: Windows.Win32.Foundation.BOOL
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.Runtime.CompilerServices;

namespace Windows.Win32.Foundation
{
  internal readonly struct BOOL
  {
    private readonly int value;

    internal int Value => this.value;

    internal BOOL(bool value) => this.value = (int) Unsafe.As<bool, sbyte>(ref value);

    internal BOOL(int value) => this.value = value;

    public static implicit operator bool(BOOL value)
    {
      sbyte source = checked ((sbyte) value.value);
      return Unsafe.As<sbyte, bool>(ref source);
    }

    public static implicit operator BOOL(bool value) => new BOOL(value);

    public static explicit operator BOOL(int value) => new BOOL(value);
  }
}
