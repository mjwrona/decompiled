// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.ArtifactId
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

namespace Microsoft.VisualStudio.Services.Common
{
  public class ArtifactId
  {
    private string m_VisualStudioServerNamespace;
    private string m_Tool;
    private string m_ArtifactType;
    private string m_ToolSpecificId;

    public ArtifactId()
    {
    }

    public ArtifactId(string tool, string artifactType, string specificId)
    {
      this.m_Tool = tool;
      this.m_ArtifactType = artifactType;
      this.m_ToolSpecificId = specificId;
    }

    public string VisualStudioServerNamespace
    {
      get => this.m_VisualStudioServerNamespace;
      set => this.m_VisualStudioServerNamespace = value;
    }

    public string Tool
    {
      get => this.m_Tool;
      set => this.m_Tool = value;
    }

    public string ArtifactType
    {
      get => this.m_ArtifactType;
      set => this.m_ArtifactType = value;
    }

    public string ToolSpecificId
    {
      get => this.m_ToolSpecificId;
      set => this.m_ToolSpecificId = value;
    }

    public new string ToString() => "< Namespace: " + this.m_VisualStudioServerNamespace + " | Tool: " + this.m_Tool + " | Artifact Type: " + this.m_ArtifactType + " | Tool Specific ID: " + this.m_ToolSpecificId + " >";
  }
}
