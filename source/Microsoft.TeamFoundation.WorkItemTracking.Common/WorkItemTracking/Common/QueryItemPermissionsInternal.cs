// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.QueryItemPermissionsInternal
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [Flags]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public enum QueryItemPermissionsInternal
  {
    None = 0,
    Read = 1,
    Contribute = 2,
    Delete = 4,
    ManagePermissions = 8,
    FullControl = 16, // 0x00000010
    ReadContribute = Contribute | Read, // 0x00000003
    ReadContributeDelete = ReadContribute | Delete, // 0x00000007
    ReadManage = ManagePermissions | Read, // 0x00000009
    AllPermissions = ReadManage | FullControl | Delete | Contribute, // 0x0000001F
    CrossProjectExecution = 32, // 0x00000020
    RecordQueryExecutionInfo = 64, // 0x00000040
  }
}
