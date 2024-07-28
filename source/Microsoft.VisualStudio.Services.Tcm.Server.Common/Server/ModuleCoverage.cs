// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ModuleCoverage
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class ModuleCoverage
  {
    private List<FunctionCoverage> m_functions = new List<FunctionCoverage>();

    internal int CoverageId { get; set; }

    internal int ModuleId { get; set; }

    [XmlAttribute]
    public Guid Signature { get; set; }

    [XmlAttribute]
    public int SignatureAge { get; set; }

    [XmlAttribute]
    public string Name { get; set; }

    [XmlAttribute]
    public string CoverageFileUrl { get; set; }

    [XmlAttribute]
    public int BlockCount { get; set; }

    [XmlElement]
    public byte[] BlockData { get; set; }

    [ClientProperty(ClientVisibility.Private)]
    public CoverageStatistics Statistics { get; set; }

    [XmlArray]
    [ClientProperty(ClientVisibility.Private)]
    public List<FunctionCoverage> Functions => this.m_functions;
  }
}
