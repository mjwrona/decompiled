// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.CheckInOptions2
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using System;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  [Flags]
  public enum CheckInOptions2
  {
    None = 0,
    ValidateCheckInOwner = 1,
    SuppressEvent = 2,
    DeleteShelveset = 4,
    OverrideGatedCheckIn = 8,
    QueueBuildForGatedCheckIn = 16, // 0x00000010
    AllContentUploaded = 32, // 0x00000020
    AllowUnchangedContent = 64, // 0x00000040
    NoAutoResolve = 128, // 0x00000080
    NoConflictsCheckForGatedCheckIn = 256, // 0x00000100
  }
}
