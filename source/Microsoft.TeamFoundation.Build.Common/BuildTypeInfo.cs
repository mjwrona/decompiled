// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.BuildTypeInfo
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Build.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class BuildTypeInfo
  {
    private string m_name;
    private string m_description;
    private string m_buildMachine;
    private string m_buildDir;
    private string m_dropLocation;

    public string Name => this.m_name;

    public string Description => this.m_description;

    public string BuildMachine => this.m_buildMachine;

    public string BuildDir => this.m_buildDir;

    public string DropLocation => this.m_dropLocation;

    public BuildTypeInfo(
      string name,
      string description,
      string buildMachine,
      string buildDirectory,
      string dropLocation)
    {
      this.m_name = name;
      this.m_description = description;
      this.m_buildMachine = buildMachine;
      this.m_buildDir = buildDirectory;
      this.m_dropLocation = dropLocation;
    }

    public BuildTypeInfo(string name)
    {
      this.m_name = name;
      this.m_description = string.Empty;
      this.m_buildMachine = string.Empty;
      this.m_buildDir = string.Empty;
      this.m_dropLocation = string.Empty;
    }
  }
}
