// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.DataCollectorInformation
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server2010
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class DataCollectorInformation
  {
    private const string PropertyName_TypeUri = "__Reserved_TypeUri";
    private List<NameValuePair> m_properties;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public Guid Id { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string TypeUri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string Description { get; set; }

    [XmlElement]
    [ClientProperty(ClientVisibility.Internal)]
    public XmlNode DefaultConfiguration { get; set; }

    [XmlElement]
    [ClientProperty(ClientVisibility.Internal)]
    public XmlNode ConfigurationEditorConfiguration { get; set; }

    [XmlArray]
    [XmlArrayItem(Type = typeof (NameValuePair))]
    [ClientProperty(ClientVisibility.Internal)]
    public List<NameValuePair> Properties
    {
      get
      {
        if (this.m_properties == null)
          this.m_properties = new List<NameValuePair>();
        return this.m_properties;
      }
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DataCollectorInformation Name={0}", (object) this.TypeUri);
  }
}
