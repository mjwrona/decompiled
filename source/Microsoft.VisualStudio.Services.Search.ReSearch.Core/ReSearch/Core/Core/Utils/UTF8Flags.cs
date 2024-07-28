// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.ReSearch.Core.Core.Utils.UTF8Flags
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.ReSearch.Core.Core.Utils
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
