// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildAgentUpdate2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClientEnum(ClientVisibility.Internal)]
  [Flags]
  [XmlType("BuildAgentUpdate")]
  public enum BuildAgentUpdate2010
  {
    None = 0,
    Name = 1,
    Description = 2,
    ControllerUri = 4,
    BuildDirectory = 8,
    Status = 16, // 0x00000010
    StatusMessage = 32, // 0x00000020
    Tags = 64, // 0x00000040
    Enabled = 128, // 0x00000080
  }
}
