// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.LabelItemSpec
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class LabelItemSpec : IValidatable
  {
    private ItemSpec m_itemSpec;
    private VersionSpec m_version;
    private bool m_exclude;

    [XmlElement("ItemSpec")]
    public ItemSpec ItemSpec
    {
      get => this.m_itemSpec;
      set => this.m_itemSpec = value;
    }

    [XmlElement("Version")]
    public VersionSpec Version
    {
      get => this.m_version;
      set => this.m_version = value;
    }

    [XmlAttribute("ex")]
    [DefaultValue(false)]
    public bool Exclude
    {
      get => this.m_exclude;
      set => this.m_exclude = value;
    }

    void IValidatable.Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      versionControlRequestContext.Validation.check((IValidatable) this.m_itemSpec, "ItemSpec", false);
      versionControlRequestContext.Validation.check((IValidatable) this.m_version, "Version", false);
    }

    public override string ToString()
    {
      string str = string.Empty;
      if (this.ItemSpec != null)
        str = str + this.ItemSpec.Item + ";";
      if (this.Version != null)
        str += this.Version.ToString();
      return str;
    }
  }
}
