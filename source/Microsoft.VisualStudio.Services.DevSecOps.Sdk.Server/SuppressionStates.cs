// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.SuppressionStates
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server
{
  [Flags]
  public enum SuppressionStates
  {
    NotSuppressed = 0,
    FilteredByNoiseReducingRegex = 1,
    InvalidatedByValidator = 2,
    SuppressedInSource = 4,
    SuppressedByUserSidecarFile = 8,
    SuppressedByBuiltInSidecarFile = 16, // 0x00000010
    MatchesHashValue = 32, // 0x00000020
    MatchesPlaceholder = 64, // 0x00000040
    MatchesFileLocation = 128, // 0x00000080
    MatchesDirectoryLocation = 256, // 0x00000100
    MatchesRuleId = 512, // 0x00000200
    FilteredDueToMultipleMatchesPerRuleOnSameLine = 1024, // 0x00000400
  }
}
