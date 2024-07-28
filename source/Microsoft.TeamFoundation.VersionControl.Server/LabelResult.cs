// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.LabelResult
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class LabelResult
  {
    private string m_label;
    private string m_scope;
    private LabelResultStatus m_status;
    private int m_labelId;

    public LabelResult()
    {
    }

    public LabelResult(string labelName, string scope, LabelResultStatus status)
    {
      this.Label = labelName;
      this.Scope = scope;
      this.Status = status;
    }

    public LabelResult(string labelName, string scope, int labelId, LabelResultStatus status)
      : this(labelName, scope, status)
    {
      this.LabelId = labelId;
    }

    [XmlAttribute("label")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string Label
    {
      get => this.m_label;
      set => this.m_label = value;
    }

    [XmlAttribute("scope")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string Scope
    {
      get => this.m_scope;
      set => this.m_scope = value;
    }

    [XmlAttribute("status")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public LabelResultStatus Status
    {
      get => this.m_status;
      set => this.m_status = value;
    }

    [XmlIgnore]
    public int LabelId
    {
      get => this.m_labelId;
      set => this.m_labelId = value;
    }
  }
}
