// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.IterationReason
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [Flags]
  public enum IterationReason
  {
    Push = 0,
    ForcePush = 1,
    Create = 2,
    Rebase = 4,
    Unknown = 8,
    Retarget = 16, // 0x00000010
    ResolveConflicts = 32, // 0x00000020
  }
}
