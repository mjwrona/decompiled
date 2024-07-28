// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.ScheduleDays2010
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
  [XmlType("ScheduleDays")]
  public enum ScheduleDays2010
  {
    None = 0,
    Monday = 1,
    Tuesday = 2,
    Wednesday = 4,
    Thursday = 8,
    Friday = 16, // 0x00000010
    Saturday = 32, // 0x00000020
    Sunday = 64, // 0x00000040
    All = Sunday | Saturday | Friday | Thursday | Wednesday | Tuesday | Monday, // 0x0000007F
  }
}
