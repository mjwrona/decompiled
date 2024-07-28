// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Diff.UTF8Flags
// Assembly: Microsoft.TeamFoundation.Diff, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F647AACF-6EF1-4C0C-AB27-20317A054A39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Diff.dll

using System;

namespace Microsoft.TeamFoundation.Diff
{
  [Flags]
  internal enum UTF8Flags
  {
    NONE = 0,
    NONASCII = 1,
    UCS4 = 2,
    OVERLONG = 4,
    TRAIL_NO_COUNT = 8388864, // 0x00800100
    COUNT_NO_TRAIL = 8389120, // 0x00800200
    UCS4OUTOFRANGE = 8389632, // 0x00800400
  }
}
