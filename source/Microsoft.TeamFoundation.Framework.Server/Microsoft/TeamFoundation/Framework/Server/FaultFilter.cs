// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FaultFilter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [JsonObject(MemberSerialization.OptIn)]
  internal class FaultFilter
  {
    [JsonProperty("startTime")]
    private DateTime m_startTime;
    [JsonProperty("endTime")]
    private DateTime m_endTime;

    public FaultFilter()
    {
    }

    public FaultFilter(DateTime startTime, DateTime endTime)
    {
      this.m_startTime = startTime;
      this.m_endTime = endTime;
    }

    public bool ShouldExecute() => this.CheckTime();

    private bool CheckTime()
    {
      if (this.m_startTime != new DateTime() && this.m_endTime != new DateTime())
      {
        DateTime utcNow = DateTime.UtcNow;
        return utcNow >= this.m_startTime.ToUniversalTime() && utcNow < this.m_endTime.ToUniversalTime();
      }
      return this.m_startTime == new DateTime() && this.m_endTime == new DateTime();
    }
  }
}
