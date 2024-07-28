// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LogStoreContainerState
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [Flags]
  public enum LogStoreContainerState
  {
    IgnoreScan = 0,
    PendingCvcScan = 1,
    InProgressCvsScan = 2,
    CompleteCvsScan = 4,
    PendingAbuseScan = 8,
    CompleteSizeCalc = 16, // 0x00000010
    PendingSizeCalc = 32, // 0x00000020
    PendingGeoReplication = 64, // 0x00000040
  }
}
