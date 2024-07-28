// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ContentValidationRegions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class ContentValidationRegions
  {
    private static readonly ISet<string> s_enabledRegions = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "CUS",
      "EUS",
      "EUS2",
      "NCUS",
      "SCUS",
      "WCUS",
      "WUS",
      "WUS2"
    };

    public static bool IsEnabled(string region) => ContentValidationRegions.s_enabledRegions.Contains(region);
  }
}
