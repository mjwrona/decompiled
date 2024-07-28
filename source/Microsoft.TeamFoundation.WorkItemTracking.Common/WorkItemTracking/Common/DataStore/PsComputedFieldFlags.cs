// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore.PsComputedFieldFlags
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore
{
  [Flags]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public enum PsComputedFieldFlags
  {
    ComputedFieldFlagEmpty = 1,
    ComputedFieldFlagSameAsBefore = 2,
    ComputedFieldFlagList = 4,
    ComputedFieldFlagPattern = 8,
    ComputedFieldFlagIncludeOldValue = 16, // 0x00000010
    ComputedFieldFlagKeepOld = 32, // 0x00000020
    ComputedFieldFlagKeepEmpty = 64, // 0x00000040
    ComputedFieldFlagCanBeAddedToList = 128, // 0x00000080
    ComputedFieldFlagFullNameInList = 256, // 0x00000100
    ComputedFieldFlagUnderlying = 512, // 0x00000200
    ComputedFieldFlagValueInOtherField = 1024, // 0x00000400
    ComputedFieldFlagDomainUsers = 2048, // 0x00000800
    ComputedFieldFlagDomainGroups = 4096, // 0x00001000
    ComputedFieldFlagIncompleteSet = 8192, // 0x00002000
    ComputedFieldFlagEmptyOrSameAsBefore = 16384, // 0x00004000
  }
}
