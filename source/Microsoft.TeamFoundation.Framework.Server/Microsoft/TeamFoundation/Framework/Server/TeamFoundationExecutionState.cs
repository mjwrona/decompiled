// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationExecutionState
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationExecutionState
  {
    private List<TeamFoundationServiceHostProperties> m_hostProperties;
    private List<TeamFoundationServiceHostProcess> m_hostProcesses;
    private List<TeamFoundationServiceHostInstance> m_hostInstances;

    internal TeamFoundationExecutionState(
      List<TeamFoundationServiceHostProperties> hostProperties,
      List<TeamFoundationServiceHostProcess> hostProcesses,
      List<TeamFoundationServiceHostInstance> hostInstances)
    {
      this.m_hostProperties = hostProperties;
      this.m_hostProcesses = hostProcesses;
      this.m_hostInstances = hostInstances;
    }

    public IEnumerable<TeamFoundationServiceHostProperties> HostProperties => (IEnumerable<TeamFoundationServiceHostProperties>) this.m_hostProperties;

    public IEnumerable<TeamFoundationServiceHostProcess> HostProcesses => (IEnumerable<TeamFoundationServiceHostProcess>) this.m_hostProcesses;

    public IEnumerable<TeamFoundationServiceHostInstance> HostInstances => (IEnumerable<TeamFoundationServiceHostInstance>) this.m_hostInstances;
  }
}
