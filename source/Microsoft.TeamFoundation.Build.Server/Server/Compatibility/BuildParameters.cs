// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildParameters
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [SoapType("BuildParameters")]
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class BuildParameters
  {
    private string m_tfsServer;
    private string m_portfolioProject;
    private string m_configName;
    private string m_buildDirectory;
    private string m_buildNumber;
    private string m_lastBuildNumber;
    private string m_lastGoodBuildNumber;
    private string m_buildUri;
    private string m_buildMachine;
    private string m_requestor;
    private string m_dropLocation;

    public string TeamFoundationServer
    {
      get => this.m_tfsServer;
      set => this.m_tfsServer = value;
    }

    public string TeamProject
    {
      get => this.m_portfolioProject;
      set => this.m_portfolioProject = value;
    }

    public string BuildType
    {
      get => this.m_configName;
      set => this.m_configName = value;
    }

    public string BuildDirectory
    {
      get => this.m_buildDirectory;
      set => this.m_buildDirectory = value;
    }

    public string BuildNumber
    {
      get => this.m_buildNumber;
      set => this.m_buildNumber = value;
    }

    public string LastBuildNumber
    {
      get => this.m_lastBuildNumber;
      set => this.m_lastBuildNumber = value;
    }

    public string LastGoodBuildNumber
    {
      get => this.m_lastGoodBuildNumber;
      set => this.m_lastGoodBuildNumber = value;
    }

    public string BuildUri
    {
      get => this.m_buildUri;
      set => this.m_buildUri = value;
    }

    public string BuildMachine
    {
      get => this.m_buildMachine;
      set => this.m_buildMachine = value;
    }

    public string DropLocation
    {
      get => this.m_dropLocation;
      set => this.m_dropLocation = value;
    }

    public string RequestedBy
    {
      get => this.m_requestor;
      set => this.m_requestor = value;
    }
  }
}
