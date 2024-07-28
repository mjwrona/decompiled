// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Coverage
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  [XmlInclude(typeof (BuildCoverage))]
  [XmlInclude(typeof (TestRunCoverage))]
  public class Coverage
  {
    private List<ModuleCoverage> m_modules = new List<ModuleCoverage>();

    [XmlAttribute]
    internal int Id { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public byte State { get; set; }

    [XmlElement]
    public string LastError { get; set; }

    [XmlArray]
    [ClientProperty(ClientVisibility.Private)]
    public List<ModuleCoverage> Modules => this.m_modules;

    protected void AssignIds()
    {
      int num1 = 0;
      foreach (ModuleCoverage module in this.Modules)
      {
        int num2 = 0;
        if (module.ModuleId == 0)
        {
          ++num1;
          module.ModuleId = num1;
        }
        foreach (FunctionCoverage function in module.Functions)
        {
          ++num2;
          function.FunctionId = num2;
          function.ModuleId = num1;
        }
      }
    }
  }
}
