// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DeleteOptions
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [Flags]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public enum DeleteOptions
  {
    None = 0,
    DropLocation = 1,
    TestResults = 2,
    Label = 4,
    Details = 8,
    Symbols = 16, // 0x00000010
    All = Symbols | Details | Label | TestResults | DropLocation, // 0x0000001F
  }
}
