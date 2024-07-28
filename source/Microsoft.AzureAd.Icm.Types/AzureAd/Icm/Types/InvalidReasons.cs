// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.InvalidReasons
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;

namespace Microsoft.AzureAd.Icm.Types
{
  [Flags]
  public enum InvalidReasons
  {
    None = 0,
    CannotBeNull = 1,
    CannotBeEmpty = 2,
    IsInAnInvalidFormat = 4,
    IsOutOfRange = 8,
    IsUnexpected = 16, // 0x00000010
    ProvideAtLeastOne = 32, // 0x00000020
    CannotBeInPast = 64, // 0x00000040
    ExceededMaxLength = 128, // 0x00000080
    IsADuplicate = 256, // 0x00000100
    DoesNotExist = 512, // 0x00000200
    CannotBeLessThanStartDate = 1024, // 0x00000400
    IsDisqualified = 2048, // 0x00000800
    ContainsAnInvalidClaimType = 4096, // 0x00001000
    ContainsAnInvalidUPNClaim = 8192, // 0x00002000
    ContainsAnInvalidCertClaim = 16384, // 0x00004000
    ContainsUnsupportedSilo = 32768, // 0x00008000
    MissingPublicSilo = 65536, // 0x00010000
  }
}
