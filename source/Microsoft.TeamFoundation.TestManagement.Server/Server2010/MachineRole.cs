// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.MachineRole
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server2010
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class MachineRole
  {
    internal MachineRole()
    {
    }

    private MachineRole(Guid id, string name)
    {
      this.Id = id;
      this.Name = name;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public Guid Id { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string Name { get; set; }
  }
}
