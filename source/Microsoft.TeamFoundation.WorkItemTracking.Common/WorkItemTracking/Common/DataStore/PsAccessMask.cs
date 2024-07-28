// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore.PsAccessMask
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore
{
  [Flags]
  internal enum PsAccessMask
  {
    AccessMaskRead = 1,
    AccessMaskWrite = 2,
    AccessMaskAdmin = 4,
    AccessMaskAll = AccessMaskAdmin | AccessMaskWrite | AccessMaskRead, // 0x00000007
  }
}
