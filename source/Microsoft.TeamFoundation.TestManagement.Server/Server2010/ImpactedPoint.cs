// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.ImpactedPoint
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server2010
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class ImpactedPoint
  {
    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string BuildUri { get; set; }

    [XmlAttribute]
    public int TestCaseId { get; set; }

    [XmlAttribute]
    public int PointId { get; set; }

    [XmlAttribute]
    public byte Confidence { get; set; }

    [XmlAttribute]
    public byte State { get; set; }

    [XmlAttribute]
    public string SuiteName { get; set; }
  }
}
