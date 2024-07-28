// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Location.RelativeToEnumCache
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Location;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebApi.Location
{
  internal static class RelativeToEnumCache
  {
    private static Dictionary<string, RelativeToSetting> s_relativeToEnums = new Dictionary<string, RelativeToSetting>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    static RelativeToEnumCache()
    {
      RelativeToEnumCache.s_relativeToEnums["Context"] = RelativeToSetting.Context;
      RelativeToEnumCache.s_relativeToEnums["FullyQualified"] = RelativeToSetting.FullyQualified;
      RelativeToEnumCache.s_relativeToEnums["WebApplication"] = RelativeToSetting.WebApplication;
    }

    internal static Dictionary<string, RelativeToSetting> GetRelativeToEnums() => RelativeToEnumCache.s_relativeToEnums;
  }
}
