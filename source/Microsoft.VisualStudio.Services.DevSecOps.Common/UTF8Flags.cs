// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Common.UTF8Flags
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 072F1303-F456-426E-A1CB-C0838641751B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.DevSecOps.Common
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
