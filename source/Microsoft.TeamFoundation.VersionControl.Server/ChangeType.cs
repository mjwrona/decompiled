// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ChangeType
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Flags]
  public enum ChangeType
  {
    [ClientValue(1)] None = 0,
    [ClientValue(2)] Add = 1,
    [ClientValue(4)] Edit = 2,
    [ClientValue(8)] Encoding = 4,
    [ClientValue(16)] Rename = 8,
    [ClientValue(32)] Delete = 16, // 0x00000010
    [ClientValue(64)] Undelete = 32, // 0x00000020
    [ClientValue(128)] Branch = 64, // 0x00000040
    [ClientValue(256)] Merge = 128, // 0x00000080
    [ClientValue(512)] Lock = 256, // 0x00000100
    [ClientValue(1024)] Rollback = 512, // 0x00000200
    [ClientValue(2048)] SourceRename = 1024, // 0x00000400
    [XmlIgnore] TargetRename = 2048, // 0x00000800
    [ClientValue(8192)] Property = 4096, // 0x00001000
    [XmlIgnore] All = Property | TargetRename | SourceRename | Rollback | Lock | Merge | Branch | Undelete | Delete | Rename | Encoding | Edit | Add, // 0x00001FFF
  }
}
