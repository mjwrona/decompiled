// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OutboundLinkType
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Registration/03")]
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Public)]
  public class OutboundLinkType
  {
    private string m_Name;
    private string m_TargetArtifactTypeTool;
    private string m_TargetArtifactTypeName;

    public OutboundLinkType()
    {
    }

    public OutboundLinkType(string toolType, string artifactName)
    {
      this.ToolType = toolType;
      this.ArtifactName = artifactName;
    }

    public OutboundLinkType(
      string name,
      string targetArtifactTypeTool,
      string targetArtifactTypeName)
    {
      this.Name = name;
      this.TargetArtifactTypeTool = targetArtifactTypeTool;
      this.TargetArtifactTypeName = targetArtifactTypeName;
    }

    public string Name
    {
      get => this.m_Name;
      set => this.m_Name = value;
    }

    public string TargetArtifactTypeTool
    {
      get => this.m_TargetArtifactTypeTool;
      set => this.m_TargetArtifactTypeTool = value;
    }

    public string TargetArtifactTypeName
    {
      get => this.m_TargetArtifactTypeName;
      set => this.m_TargetArtifactTypeName = value;
    }

    public OutboundLinkType DeepClone() => new OutboundLinkType(this.Name, this.TargetArtifactTypeTool, this.TargetArtifactTypeName);

    [XmlIgnore]
    public string ToolType { get; internal set; }

    [XmlIgnore]
    public string ArtifactName { get; internal set; }

    internal Predicate<OutboundLinkType> EqualsByName() => (Predicate<OutboundLinkType>) (that => VssStringComparer.LinkName.Equals(this.Name, that.Name));
  }
}
