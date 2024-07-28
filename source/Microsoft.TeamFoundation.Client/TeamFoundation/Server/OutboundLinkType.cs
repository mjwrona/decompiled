// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.OutboundLinkType
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server
{
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Registration/03")]
  public class OutboundLinkType
  {
    private string m_Name;
    private string m_TargetArtifactTypeTool;
    private string m_TargetArtifactTypeName;

    public OutboundLinkType()
    {
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

    internal Predicate<OutboundLinkType> EqualsByName() => (Predicate<OutboundLinkType>) (that => VssStringComparer.LinkName.Equals(this.Name, that.Name));
  }
}
