// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TuningRecommendation
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TuningRecommendation
  {
    public string Name { get; set; }

    public string Type { get; set; }

    public string Reason { get; set; }

    public DateTime ValidSince { get; set; }

    public DateTime LastRefresh { get; set; }

    public string State { get; set; }

    public bool IsExecutableAction { get; set; }

    public bool IsRevertableAction { get; set; }

    public TuningAction ExecuteAction { get; set; }

    public TuningAction RevertAction { get; set; }

    public int Score { get; set; }

    public string Details { get; set; }
  }
}
