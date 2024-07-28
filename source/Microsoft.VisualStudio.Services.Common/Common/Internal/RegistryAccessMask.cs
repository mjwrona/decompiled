// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Internal.RegistryAccessMask
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Common.Internal
{
  [Flags]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [CLSCompliant(false)]
  public enum RegistryAccessMask : uint
  {
    AllAccess = 983103, // 0x000F003F
    CreateLink = 32, // 0x00000020
    CreateSubKey = 4,
    EnumerateSubKeys = 8,
    Execute = 131097, // 0x00020019
    Notify = 16, // 0x00000010
    QueryValue = 1,
    Read = 131097, // 0x00020019
    SetValue = 2,
    Wow6432Key = 512, // 0x00000200
    Wow6464Key = 256, // 0x00000100
    Write = 131078, // 0x00020006
    WriteDac = 262144, // 0x00040000
  }
}
