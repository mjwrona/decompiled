// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.CaptureLength
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Common
{
  public class CaptureLength : Capture<int>
  {
    public CaptureLength()
      : this(0)
    {
    }

    public CaptureLength(int value)
      : base(value, CaptureLength.\u003C\u003EO.\u003C0\u003E__IsValueValid ?? (CaptureLength.\u003C\u003EO.\u003C0\u003E__IsValueValid = new Predicate<int>(CaptureLength.IsValueValid)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    public static CaptureLength Create(int value) => new CaptureLength(value);

    private static bool IsValueValid(int value) => value >= 0;
  }
}
