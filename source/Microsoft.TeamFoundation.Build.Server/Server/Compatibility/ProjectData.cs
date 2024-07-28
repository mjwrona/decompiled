// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.ProjectData
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [SoapType("ProjectData")]
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class ProjectData
  {
    private string m_projectFile;
    private string m_parentProjectFile;
    private string m_platformName;
    private string m_flavourName;
    private int m_compileErrors;
    private int m_compileWarnings;
    private int m_staticAnalysisErrors;
    private int m_staticAnalysisWarnings;

    public string ProjectFile
    {
      get => this.m_projectFile;
      set => this.m_projectFile = value;
    }

    public string ParentProjectFile
    {
      get => this.m_parentProjectFile;
      set => this.m_parentProjectFile = value;
    }

    public string PlatformName
    {
      get => this.m_platformName;
      set => this.m_platformName = value;
    }

    public string FlavourName
    {
      get => this.m_flavourName;
      set => this.m_flavourName = value;
    }

    public int CompileErrors
    {
      get => this.m_compileErrors;
      set => this.m_compileErrors = value;
    }

    public int CompileWarnings
    {
      get => this.m_compileWarnings;
      set => this.m_compileWarnings = value;
    }

    public int CodeAnalysisErrors
    {
      get => this.m_staticAnalysisErrors;
      set => this.m_staticAnalysisErrors = value;
    }

    public int CodeAnalysisWarnings
    {
      get => this.m_staticAnalysisWarnings;
      set => this.m_staticAnalysisWarnings = value;
    }
  }
}
