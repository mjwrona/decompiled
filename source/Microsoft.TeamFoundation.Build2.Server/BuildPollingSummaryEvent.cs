// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildPollingSummaryEvent
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildPollingSummaryEvent
  {
    private Dictionary<string, string> m_ciData;

    public BuildPollingSummaryEvent(Dictionary<string, string> ciData) => this.m_ciData = ciData;

    public Dictionary<string, string> CIData => this.m_ciData;
  }
}
