// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.FunctionCoverage
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server2010
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class FunctionCoverage
  {
    internal int CoverageId { get; set; }

    internal int ModuleId { get; set; }

    internal int FunctionId { get; set; }

    [XmlAttribute]
    public string Namespace { get; set; }

    [XmlAttribute]
    public string Class { get; set; }

    [XmlAttribute]
    public string Name { get; set; }

    [XmlAttribute]
    public string SourceFile { get; set; }

    [XmlElement]
    [ClientProperty(ClientVisibility.Internal)]
    public CoverageStatistics Statistics { get; set; }
  }
}
