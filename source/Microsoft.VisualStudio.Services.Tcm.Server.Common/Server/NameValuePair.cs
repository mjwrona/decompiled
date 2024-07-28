// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.NameValuePair
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Public)]
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

    [XmlAttribute]
    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    [XmlAttribute]
    public string Value
    {
      get => this.m_value;
      set => this.m_value = value;
    }
  }
}
