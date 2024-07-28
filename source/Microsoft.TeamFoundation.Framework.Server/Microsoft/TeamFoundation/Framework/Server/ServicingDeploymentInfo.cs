// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingDeploymentInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ServicingDeploymentInfo
  {
    private Dictionary<string, ServiceLevel> m_ServiceLevelMap;

    public ServicingDeploymentInfo(string serviceLevels, DateTime startTime, DateTime? endTime)
    {
      this.ServiceLevels = serviceLevels;
      this.StartTime = startTime;
      this.EndTime = endTime;
    }

    public Dictionary<string, ServiceLevel> GetServiceLevelMap(IVssRequestContext requestContext)
    {
      if (this.m_ServiceLevelMap == null)
        this.m_ServiceLevelMap = ServiceLevel.CreateServiceLevelMap(requestContext, this.ServiceLevels);
      return this.m_ServiceLevelMap;
    }

    public string ServiceLevels { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }
  }
}
