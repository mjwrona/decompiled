// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.TestConfiguration
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server2010
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class TestConfiguration
  {
    private int m_id;
    private string m_name;
    private string m_areaPath;
    private string m_description;
    private byte m_state;
    private int m_revision;
    private List<NameValuePair> m_values;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    [DefaultValue(0)]
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
    public Guid LastUpdatedBy { get; set; }

    [XmlAttribute]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    [ClientProperty(ClientVisibility.Internal)]
    public DateTime LastUpdated { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string AreaPath
    {
      get => this.m_areaPath;
      set => this.m_areaPath = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public bool IsDefault { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    [DefaultValue(0)]
    public int Revision
    {
      get => this.m_revision;
      set => this.m_revision = value;
    }

    [XmlArray]
    [XmlArrayItem(typeof (NameValuePair))]
    [ClientProperty(ClientVisibility.Internal)]
    public List<NameValuePair> Values
    {
      get
      {
        if (this.m_values == null)
          this.m_values = new List<NameValuePair>();
        return this.m_values;
      }
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Internal)]
    public byte State
    {
      get => this.m_state;
      set => this.m_state = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestConfiguration Id={0} Name={1} Description={2}", (object) this.m_id, (object) this.m_name, (object) this.m_description);
  }
}
