// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.Coverage
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server2010
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlInclude(typeof (BuildCoverage))]
  [XmlInclude(typeof (TestRunCoverage))]
  public class Coverage
  {
    private List<ModuleCoverage> m_modules = new List<ModuleCoverage>();

    [XmlAttribute]
    internal int Id { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public byte State { get; set; }

    [XmlElement]
    public string LastError { get; set; }

    [XmlArray]
    [ClientProperty(ClientVisibility.Internal)]
    public List<ModuleCoverage> Modules => this.m_modules;
  }
}
