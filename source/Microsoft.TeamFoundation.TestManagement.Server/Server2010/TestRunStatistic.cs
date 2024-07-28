// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.TestRunStatistic
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server2010
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class TestRunStatistic
  {
    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public byte State { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public byte Outcome { get; set; }

    [XmlElement]
    [ClientProperty(ClientVisibility.Internal)]
    public TestResolutionState ResolutionState { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int Count { get; set; }
  }
}
