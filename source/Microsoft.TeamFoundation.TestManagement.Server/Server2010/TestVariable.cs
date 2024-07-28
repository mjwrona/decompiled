// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.TestVariable
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server2010
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class TestVariable
  {
    private int m_id;
    private string m_name;
    private string m_description;
    private int m_revision;
    private List<string> m_values;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int Id
    {
      get => this.m_id;
      set => this.m_id = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string Description
    {
      get => this.m_description;
      set => this.m_description = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int Revision
    {
      get => this.m_revision;
      set => this.m_revision = value;
    }

    [XmlArray]
    [XmlArrayItem(typeof (string))]
    [ClientProperty(ClientVisibility.Internal)]
    public List<string> Values
    {
      get
      {
        if (this.m_values == null)
          this.m_values = new List<string>();
        return this.m_values;
      }
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestVariable Id={0} Name={1} Description={2}", (object) this.m_id, (object) this.m_name, (object) this.m_description);
  }
}
