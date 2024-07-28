// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Admin.AccountType
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System;

namespace Microsoft.TeamFoundation.Admin
{
  [Flags]
  public enum AccountType
  {
    None = 0,
    LocalSystem = 1,
    LocalService = 2,
    NetworkService = 4,
    User = 8,
    Computer = 16, // 0x00000010
    AcsServiceIdentity = 32, // 0x00000020
    ApplicationPoolIdentity = 64, // 0x00000040
  }
}
