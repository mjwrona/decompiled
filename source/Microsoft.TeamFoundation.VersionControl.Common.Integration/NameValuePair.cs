// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.NameValuePair
// Assembly: Microsoft.TeamFoundation.VersionControl.Common.Integration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2A9D748-4CEE-4498-9785-584B91A44F85
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.Integration.dll

using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  public class NameValuePair
  {
    private string m_name;
    private string m_value;

    public NameValuePair()
    {
    }

    public NameValuePair(string name, string value)
    {
      this.Name = name;
      this.Value = value;
    }

    [XmlAttribute("name")]
    public string Name
    {
      get => string.IsNullOrEmpty(this.m_name) ? string.Empty : this.m_name;
      set => this.m_name = value;
    }

    [XmlAttribute("val")]
    public string Value
    {
      get => string.IsNullOrEmpty(this.m_value) ? string.Empty : this.m_value;
      set => this.m_value = value;
    }
  }
}
