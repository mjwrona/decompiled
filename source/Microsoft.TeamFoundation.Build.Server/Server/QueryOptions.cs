// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.QueryOptions
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [Flags]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public enum QueryOptions
  {
    None = 0,
    Definitions = 1,
    Agents = 2,
    Workspaces = 5,
    Controllers = 8,
    Process = 17, // 0x00000011
    BatchedRequests = 32, // 0x00000020
    HistoricalBuilds = 64, // 0x00000040
    All = 127, // 0x0000007F
  }
}
