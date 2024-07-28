// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.CommandLine.ExitCode
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Client.CommandLine
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public enum ExitCode
  {
    Unknown = -1, // 0xFFFFFFFF
    Success = 0,
    PartialSuccess = 1,
    UnrecognizedCommand = 2,
    NotAttempted = 3,
    SuccessRebootRequired = 4,
    Failure = 100, // 0x00000064
  }
}
