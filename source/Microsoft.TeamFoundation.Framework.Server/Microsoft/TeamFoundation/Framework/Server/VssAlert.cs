// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssAlert
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class VssAlert
  {
    private int m_scopeIndex = -1;

    [XmlAttribute("enabled")]
    public bool Enabled { get; set; }

    [XmlAttribute("kpi")]
    public bool Kpi { get; set; }

    [XmlAttribute("stateful")]
    public bool Stateful { get; set; }

    [XmlAttribute("scopeIndex")]
    public int ScopeIndex
    {
      get => this.m_scopeIndex;
      set => this.m_scopeIndex = value;
    }

    [XmlAttribute("toAdditions")]
    public string ToAdditions { get; set; }
  }
}
