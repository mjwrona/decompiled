// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SasRequestPermissions
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [DataContract]
  [Flags]
  public enum SasRequestPermissions
  {
    None = 0,
    Read = 1,
    Write = 2,
    Delete = 4,
    List = 8,
    Add = 16, // 0x00000010
    Create = 32, // 0x00000020
    Update = 64, // 0x00000040
    ProcessMessages = 128, // 0x00000080
    Tag = 256, // 0x00000100
    Filter = 512, // 0x00000200
    Verify = 32768, // 0x00008000
    All = Verify | Filter | Tag | ProcessMessages | Update | Create | Add | List | Delete | Write | Read, // 0x000083FF
  }
}
