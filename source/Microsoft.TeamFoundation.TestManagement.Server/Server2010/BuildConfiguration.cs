// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.BuildConfiguration
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server2010
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class BuildConfiguration
  {
    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal, PropertyName = "Id")]
    public int BuildConfigurationId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string TeamProjectName { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string BuildUri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string BuildFlavor { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string BuildPlatform { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Build Configuration Id={0} Build={1}", (object) this.BuildConfigurationId, (object) this.BuildUri);
  }
}
