// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.LinkType
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server
{
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Registration/03")]
  public class LinkType
  {
    private string m_SourceTool;
    private string m_SourceArtifactType;
    private string m_LinkName;
    private string m_SinkTool;
    private string m_SinkArtifactType;

    public string SourceTool
    {
      get => this.m_SourceTool;
      set => this.m_SourceTool = value;
    }

    public string SourceArtifactType
    {
      get => this.m_SourceArtifactType;
      set => this.m_SourceArtifactType = value;
    }

    public string LinkName
    {
      get => this.m_LinkName;
      set => this.m_LinkName = value;
    }

    public string SinkTool
    {
      get => this.m_SinkTool;
      set => this.m_SinkTool = value;
    }

    public string SinkArtifactType
    {
      get => this.m_SinkArtifactType;
      set => this.m_SinkArtifactType = value;
    }

    internal Predicate<LinkType> EqualsByName() => (Predicate<LinkType>) (that => VssStringComparer.LinkName.Equals(this.LinkName, that.LinkName));
  }
}
