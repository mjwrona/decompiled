// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildData
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [SoapType("BuildData")]
  [ClassVisibility(ClientVisibility.Internal)]
  public class BuildData
  {
    private string m_configurationFileUri;
    private string m_machineName;
    private string m_dropLocation;
    private string m_buildStatus;
    private DateTime m_startTime;
    private string m_configurationName;
    private string m_requestedBy;
    private string m_teamProject;
    private string m_buildNumber;
    private string m_buildQuality;
    private string m_buildURI;
    private DateTime m_finishTime;
    private string m_logLocation;
    private string m_lastChangedBy;
    private DateTime m_lastChangedOn;
    private int m_buildStatusId;
    private bool m_goodBuild;

    public string BuildTypeFileUri
    {
      get => this.m_configurationFileUri;
      set => this.m_configurationFileUri = value;
    }

    public string BuildMachine
    {
      get => this.m_machineName;
      set => this.m_machineName = value;
    }

    public string DropLocation
    {
      get => this.m_dropLocation;
      set => this.m_dropLocation = value;
    }

    public string BuildStatus
    {
      get => this.m_buildStatus;
      set => this.m_buildStatus = value;
    }

    public DateTime StartTime
    {
      get => this.m_startTime;
      set => this.m_startTime = value;
    }

    public string BuildType
    {
      get => this.m_configurationName;
      set => this.m_configurationName = value;
    }

    public string RequestedBy
    {
      get => this.m_requestedBy;
      set => this.m_requestedBy = value;
    }

    public string TeamProject
    {
      get => this.m_teamProject;
      set => this.m_teamProject = value;
    }

    public string BuildNumber
    {
      get => this.m_buildNumber;
      set => this.m_buildNumber = value;
    }

    public string BuildQuality
    {
      get => this.m_buildQuality;
      set => this.m_buildQuality = value;
    }

    public string BuildUri
    {
      get => this.m_buildURI;
      set => this.m_buildURI = value;
    }

    public DateTime FinishTime
    {
      get => this.m_finishTime;
      set => this.m_finishTime = value;
    }

    public string LogLocation
    {
      get => this.m_logLocation;
      set => this.m_logLocation = value;
    }

    public string LastChangedBy
    {
      get => this.m_lastChangedBy;
      set => this.m_lastChangedBy = value;
    }

    public DateTime LastChangedOn
    {
      get => this.m_lastChangedOn;
      set => this.m_lastChangedOn = value;
    }

    public int BuildStatusId
    {
      get => this.m_buildStatusId;
      set => this.m_buildStatusId = value;
    }

    public bool GoodBuild
    {
      get => this.m_goodBuild;
      set => this.m_goodBuild = value;
    }
  }
}
