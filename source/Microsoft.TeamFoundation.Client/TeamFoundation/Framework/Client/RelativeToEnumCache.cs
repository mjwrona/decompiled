// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.RelativeToEnumCache
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Client
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
