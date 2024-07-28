// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.Common.UTF8Flags
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F16CDF2D-8103-4EAE-A2A8-4FA5B1C1BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.CodeReview.Server.Common
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
