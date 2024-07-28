// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecureTokenValidationFailureReason
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Flags]
  public enum SecureTokenValidationFailureReason
  {
    None = 0,
    TokenExpired = 1,
    ValidationKeyExpired = 2,
    InvalidToken = ValidationKeyExpired | TokenExpired, // 0x00000003
    IssuerMismatch = 4,
    AudienceMismatch = IssuerMismatch | TokenExpired, // 0x00000005
    Unknown = IssuerMismatch | ValidationKeyExpired, // 0x00000006
  }
}
