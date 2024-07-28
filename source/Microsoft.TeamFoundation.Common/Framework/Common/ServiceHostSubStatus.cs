// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.ServiceHostSubStatus
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Common
{
  public enum ServiceHostSubStatus
  {
    [XmlEnum("-1")] Unchanged = -1, // 0xFFFFFFFF
    [XmlEnum("0")] None = 0,
    [XmlEnum("1")] Creating = 1,
    [XmlEnum("2")] Servicing = 2,
    [XmlEnum("3")] Deleting = 3,
    [XmlEnum("4")] Detaching = 4,
    [XmlEnum("5")] Importing = 5,
    [XmlEnum("6")] Reparenting = 6,
    [XmlEnum("7")] Migrating = 7,
    [XmlEnum("8")] Moving = 8,
    [XmlEnum("9")] UserRequested = 9,
    [XmlEnum("10")] Testing = 10, // 0x0000000A
    [XmlEnum("11")] UpgradeDuringImport = 11, // 0x0000000B
    [XmlEnum("12")] Idle = 12, // 0x0000000C
    [XmlEnum("13")] Propagate = 13, // 0x0000000D
  }
}
