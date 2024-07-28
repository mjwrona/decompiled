// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DeleteHostResourceOptions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Flags]
  public enum DeleteHostResourceOptions
  {
    None = 0,
    MarkForDeletion = 1,
    DeleteImmediately = 2,
    EnsureInstanceManagementCleanupSuccessful = 4,
    SkipInstanceManagementCleanup = 8,
    SkipVirtualParentCleanup = 16, // 0x00000010
    SkipMarkingBlobsForDeletion = 32, // 0x00000020
    OverrideInstanceCheck = 64, // 0x00000040
    SkipSubStatusUpdate = 128, // 0x00000080
  }
}
