// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.InheritanceChange
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class InheritanceChange : SecurityChange
  {
    private bool m_inherit = true;

    [DefaultValue(true)]
    [XmlAttribute("inherit")]
    public bool Inherit
    {
      get => this.m_inherit;
      set => this.m_inherit = value;
    }

    public override string ToString() => "Inherit: " + (object) this.Inherit;
  }
}
