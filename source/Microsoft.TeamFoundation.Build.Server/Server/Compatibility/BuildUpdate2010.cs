// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildUpdate2010
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
  [XmlType("BuildUpdate")]
  public enum BuildUpdate2010
  {
    None = 0,
    BuildNumber = 1,
    DropLocation = 2,
    LabelName = 4,
    LogLocation = 8,
    [XmlIgnore] FinishTime = 16, // 0x00000010
    Status = 32, // 0x00000020
    Quality = 64, // 0x00000040
    CompilationStatus = 128, // 0x00000080
    TestStatus = 256, // 0x00000100
    KeepForever = 512, // 0x00000200
    [XmlIgnore, Obsolete("This field has been deprecated. Please remove all references.", false)] ConfigurationFolderUri = 1024, // 0x00000400
    SourceGetVersion = 2048, // 0x00000800
  }
}
