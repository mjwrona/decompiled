// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.TestResolutionState
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server2010
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class TestResolutionState
  {
    private int m_id;
    private string m_name;
    private string m_teamProject;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    [DefaultValue(0)]
    public int Id
    {
      get => this.m_id;
      set => this.m_id = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string TeamProject
    {
      get => this.m_teamProject;
      set => this.m_teamProject = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestResolutionState Id={0} Name={1}", (object) this.m_id, (object) this.m_name);
  }
}
