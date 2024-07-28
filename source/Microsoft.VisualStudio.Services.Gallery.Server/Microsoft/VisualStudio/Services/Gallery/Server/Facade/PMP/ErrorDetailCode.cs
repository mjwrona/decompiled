// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.ErrorDetailCode
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP
{
  public enum ErrorDetailCode
  {
    InvalidValue = 1000, // 0x000003E8
    LimitExceeded = 1001, // 0x000003E9
    ZipSlipValidationFailed = 1002, // 0x000003EA
    ZipBombValidationFailed = 1003, // 0x000003EB
    DuplicateEntryValidationFailed = 1004, // 0x000003EC
    FutureModifyDateValidationFailed = 1005, // 0x000003ED
  }
}
