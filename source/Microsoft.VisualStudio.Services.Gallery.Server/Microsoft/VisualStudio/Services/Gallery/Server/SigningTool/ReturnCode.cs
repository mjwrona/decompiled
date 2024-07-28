// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SigningTool.ReturnCode
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

namespace Microsoft.VisualStudio.Services.Gallery.Server.SigningTool
{
  public enum ReturnCode
  {
    None = 0,
    RequiredArgumentMissing = 1,
    InvalidArgument = 2,
    PackageIsUnreadable = 3,
    UnhandledException = 4,
    SignatureManifestIsMissing = 5,
    SignatureManifestIsUnreadable = 6,
    SignatureIsMissing = 7,
    SignatureIsUnreadable = 8,
    CertificateIsUnreadable = 9,
    SignatureArchiveIsUnreadable = 10, // 0x0000000A
    FileAlreadyExists = 11, // 0x0000000B
    Success = 100, // 0x00000064
    PackageIntegrityCheckFailed = 101, // 0x00000065
    SignatureIsInvalid = 102, // 0x00000066
    SignatureManifestIsInvalid = 103, // 0x00000067
    SignatureIntegrityCheckFailed = 104, // 0x00000068
    EntryIsMissing = 105, // 0x00000069
    EntryIsTampered = 106, // 0x0000006A
    Untrusted = 107, // 0x0000006B
    CertificateRevoked = 108, // 0x0000006C
    SignatureIsNotValid = 109, // 0x0000006D
    UnknownError = 110, // 0x0000006E
  }
}
