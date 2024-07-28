// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.ChangedEvent
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  public abstract class ChangedEvent
  {
    [XmlAttribute(DataType = "anyURI")]
    public string TeamProjectCollectionUrl { get; set; }

    [XmlAttribute(DataType = "anyURI")]
    public string Uri { get; set; }

    [XmlAttribute]
    public string Name { get; set; }

    [XmlAttribute]
    public string ResourceType { get; set; }

    [XmlAttribute]
    public string ChangedBy { get; set; }

    [XmlAttribute]
    public string ChangedTime { get; set; }

    [XmlAttribute]
    public ChangedType ChangedType { get; set; }

    [XmlElement(Namespace = "")]
    public string Title { get; set; }

    public string TimeZone { get; set; }

    public string TimeZoneOffset { get; set; }

    [Obsolete]
    public string Subscriber { get; set; }

    public List<PropertyChange> PropertyChanges { get; set; }
  }
}
