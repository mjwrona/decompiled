// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Utils.UTF8Flags
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Common.Utils
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
