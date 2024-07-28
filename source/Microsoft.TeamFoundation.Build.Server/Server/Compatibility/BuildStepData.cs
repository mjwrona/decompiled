// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildStepData
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [SoapType("BuildStepData")]
  [ClassVisibility(ClientVisibility.Internal)]
  public class BuildStepData
  {
    private long m_buildStepId;
    private long m_parentBuildStepId;
    private string m_buildstepName;
    private string m_buildstepMessage;
    private DateTime m_startTime;
    private DateTime m_finishTime;
    private BuildStepStatus m_status;

    public long BuildStepId
    {
      get => this.m_buildStepId;
      set => this.m_buildStepId = value;
    }

    public long ParentBuildStepId
    {
      get => this.m_parentBuildStepId;
      set => this.m_parentBuildStepId = value;
    }

    public string BuildStepName
    {
      get => this.m_buildstepName;
      set => this.m_buildstepName = value;
    }

    public string BuildStepMessage
    {
      get => this.m_buildstepMessage;
      set => this.m_buildstepMessage = value;
    }

    public DateTime StartTime
    {
      get => this.m_startTime;
      set => this.m_startTime = value;
    }

    public DateTime FinishTime
    {
      get => this.m_finishTime;
      set => this.m_finishTime = value;
    }

    public BuildStepStatus Status
    {
      get => this.m_status;
      set => this.m_status = value;
    }
  }
}
