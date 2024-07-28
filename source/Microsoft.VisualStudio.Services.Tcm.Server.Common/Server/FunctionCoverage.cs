// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.FunctionCoverage
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
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
    [ClientProperty(ClientVisibility.Private)]
    public CoverageStatistics Statistics { get; set; }
  }
}
