// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.MathUtils
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;

namespace Microsoft.Azure.Documents
{
  internal static class MathUtils
  {
    public static int CeilingMultiple(int x, int n)
    {
      if (x <= 0)
        throw new ArgumentOutOfRangeException(nameof (x));
      if (n <= 0)
        throw new ArgumentOutOfRangeException(nameof (n));
      --x;
      return checked (x + n - unchecked (x % n));
    }
  }
}
