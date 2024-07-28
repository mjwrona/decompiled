// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.CaptureSize
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Common
{
  public class CaptureSize : Capture<long>
  {
    public CaptureSize()
      : this(0L)
    {
    }

    public CaptureSize(long value)
      : base(value, CaptureSize.\u003C\u003EO.\u003C0\u003E__IsValueValid ?? (CaptureSize.\u003C\u003EO.\u003C0\u003E__IsValueValid = new Predicate<long>(CaptureSize.IsValueValid)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    public static CaptureSize Create(long value) => new CaptureSize(value);

    private static bool IsValueValid(long value) => value >= 0L;
  }
}
