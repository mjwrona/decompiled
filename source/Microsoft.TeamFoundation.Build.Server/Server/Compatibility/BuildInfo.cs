// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildInfo
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  internal struct BuildInfo
  {
    private Guid m_projectId;
    private string m_uri;
    private int m_queueId;

    public BuildInfo(Guid projectId, string uri, int queueId)
    {
      this.m_projectId = projectId;
      this.m_uri = uri;
      this.m_queueId = queueId;
    }

    public Guid ProjectId => this.m_projectId;

    public string Uri => this.m_uri;

    public int QueueId => this.m_queueId;
  }
}
