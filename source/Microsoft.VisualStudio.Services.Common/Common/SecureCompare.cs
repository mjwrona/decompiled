// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.SecureCompare
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class SecureCompare
  {
    public static bool TimeInvariantEquals(byte[] lhs, byte[] rhs)
    {
      if (lhs.Length != rhs.Length)
        return false;
      int num = 0;
      for (int index = 0; index < lhs.Length; ++index)
        num |= (int) lhs[index] ^ (int) rhs[index];
      return num == 0;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public new static bool Equals(object lhs, object rhs) => throw new NotImplementedException("This is not the secure equals method! Use `TimeInvariantEquals` instead.");
  }
}
