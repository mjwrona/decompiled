// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore.PsAccessCheckFlags
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore
{
  [Flags]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public enum PsAccessCheckFlags
  {
    AccessCheckComputeFieldProperties = 1,
    AccessCheckComputeFieldLists = 2,
    AccessCheckNoComputeAffectedFields = 4,
    AccessCheckUnconditionalOnly = 8,
    AccessCheckConditionalOnly = 16, // 0x00000010
    AccessAttemptMakeTrueAll = 32, // 0x00000020
    AccessAttemptMakeTruePartial = 64, // 0x00000040
    AccessApplyDefaults = 128, // 0x00000080
    AccessApplyAcceptMissingIf = 256, // 0x00000100
    AccessCheckSubtree = 512, // 0x00000200
    AccessNonRestrictiveAllowedValues = 1024, // 0x00000400
    AccessApplyGlobalSystemRulesOnly = 2048, // 0x00000800
  }
}
