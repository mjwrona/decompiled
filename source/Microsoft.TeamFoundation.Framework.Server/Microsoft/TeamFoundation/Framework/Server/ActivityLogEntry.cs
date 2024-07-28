// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ActivityLogEntry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class ActivityLogEntry
  {
    private List<ActivityLogParameter> m_parameters;

    public int CommandId { get; internal set; }

    public string Application { get; set; }

    public string Command { get; set; }

    public int Status { get; set; }

    public DateTime StartTime { get; set; }

    public long DelayTime { get; set; }

    public long ExecutionTime { get; set; }

    public long TimeToFirstPage { get; set; }

    public string IdentityName { get; set; }

    public string IpAddress { get; set; }

    public Guid UniqueIdentifier { get; set; }

    public string UserAgent { get; set; }

    public string CommandIdentifier { get; set; }

    public int ExecutionCount { get; set; }

    public string AuthenticationType { get; set; }

    public string AgentId { get; set; }

    public int ResponseCode { get; set; }

    public List<ActivityLogParameter> Parameters
    {
      get
      {
        if (this.m_parameters == null)
          this.m_parameters = new List<ActivityLogParameter>();
        return this.m_parameters;
      }
    }
  }
}
