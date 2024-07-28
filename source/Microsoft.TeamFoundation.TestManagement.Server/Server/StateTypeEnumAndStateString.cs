// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.StateTypeEnumAndStateString
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class StateTypeEnumAndStateString
  {
    public StateTypeEnumAndStateString()
    {
    }

    public StateTypeEnumAndStateString(byte stateType, string state)
    {
      this.StateType = stateType;
      this.State = state;
    }

    [XmlAttribute]
    public byte StateType { get; set; }

    [XmlAttribute]
    public string State { get; set; }
  }
}
