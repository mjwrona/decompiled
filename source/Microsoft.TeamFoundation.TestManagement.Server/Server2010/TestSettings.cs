// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.TestSettings
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server2010
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class TestSettings
  {
    private int m_id;
    private string m_name;
    private string m_areaPath;
    private XmlElement m_settings;
    private int m_revision;
    private bool m_isPublic;
    private bool m_isAutomated;

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
    public string Description { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public Guid CreatedBy { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public DateTime CreatedDate { get; set; }

    [XmlArray]
    [ClientProperty(ClientVisibility.Internal)]
    public TestSettingsMachineRole[] MachineRoles { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public bool IsPublic
    {
      get => this.m_isPublic;
      set => this.m_isPublic = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public bool IsAutomated
    {
      get => this.m_isAutomated;
      set => this.m_isAutomated = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string Settings
    {
      get => this.m_settings != null ? this.m_settings.OuterXml : (string) null;
      set
      {
        if (value != null)
          this.m_settings = XmlUtility.LoadXmlDocumentFromString(value).DocumentElement;
        else
          this.m_settings = (XmlElement) null;
      }
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string AreaPath
    {
      get => this.m_areaPath;
      set => this.m_areaPath = value;
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
    [DefaultValue(0)]
    public int Revision
    {
      get => this.m_revision;
      set => this.m_revision = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestSettings Id={0} Name={1}", (object) this.m_id, (object) this.m_name);
  }
}
