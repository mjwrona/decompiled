// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.ModuleCoverage
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server2010
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
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
    public int BlockCount { get; set; }

    [XmlElement]
    public byte[] BlockData { get; set; }

    [ClientProperty(ClientVisibility.Internal)]
    public CoverageStatistics Statistics { get; set; }

    [XmlArray]
    [ClientProperty(ClientVisibility.Internal)]
    public List<FunctionCoverage> Functions => this.m_functions;
  }
}
