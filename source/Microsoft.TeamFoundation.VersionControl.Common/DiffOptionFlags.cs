// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.DiffOptionFlags
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using System;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  [Flags]
  public enum DiffOptionFlags
  {
    None = 0,
    IgnoreEndOfLineDifference = 1,
    IgnoreCase = 2,
    IgnoreWhiteSpace = 4,
    IgnoreKanaType = 8,
    IgnoreSymbols = 16, // 0x00000010
    IgnoreNonSpace = 32, // 0x00000020
    IgnoreWidth = 64, // 0x00000040
    EnablePreambleHandling = 128, // 0x00000080
    IgnoreLeadingAndTrailingWhiteSpace = 256, // 0x00000100
    ScanFullFileForEncodingDetection = 512, // 0x00000200
    ThrowIfDetectedEncodingMismatch = 1024, // 0x00000400
    IgnoreEndOfFileEndOfLineDifference = 2048, // 0x00000800
  }
}
