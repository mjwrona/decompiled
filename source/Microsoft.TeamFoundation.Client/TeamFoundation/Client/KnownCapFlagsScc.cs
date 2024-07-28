// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.KnownCapFlagsScc
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class KnownCapFlagsScc
  {
    public const string TFVC = "1";
    public const string Git = "2";

    public static TeamFoundationSourceControlType Convert(string capFlagsScc)
    {
      if (!string.Equals(capFlagsScc, "2", StringComparison.OrdinalIgnoreCase) && !string.Equals(capFlagsScc, "1", StringComparison.OrdinalIgnoreCase))
        TeamFoundationTrace.Verbose(TraceKeywordSets.TeamExplorer, "KnownCapFlagsScc.Convert: Unrecognized capFlagsScc value for conversion: " + capFlagsScc);
      return !string.Equals(capFlagsScc, "2", StringComparison.OrdinalIgnoreCase) ? TeamFoundationSourceControlType.TFVC : TeamFoundationSourceControlType.Git;
    }
  }
}
