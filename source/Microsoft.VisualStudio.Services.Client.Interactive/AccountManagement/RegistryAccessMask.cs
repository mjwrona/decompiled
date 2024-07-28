// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.RegistryAccessMask
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  public enum RegistryAccessMask : uint
  {
    QueryValue = 1,
    SetValue = 2,
    CreateSubKey = 4,
    EnumerateSubKeys = 8,
    Notify = 16, // 0x00000010
    CreateLink = 32, // 0x00000020
    Wow6464Key = 256, // 0x00000100
    Wow6432Key = 512, // 0x00000200
    Write = 131078, // 0x00020006
    Execute = 131097, // 0x00020019
    Read = 131097, // 0x00020019
    WriteDac = 262144, // 0x00040000
    AllAccess = 983103, // 0x000F003F
  }
}
