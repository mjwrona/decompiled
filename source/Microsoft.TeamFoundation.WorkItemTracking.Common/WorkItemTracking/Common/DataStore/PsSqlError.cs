// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore.PsSqlError
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public enum PsSqlError
  {
    ErrorTimeOutOrServerNotResponding = -2, // 0xFFFFFFFE
    ErrorSevereError = 0,
    ScalarVariableNotDeclaredError = 137, // 0x00000089
    ErrorColumnNotFound = 207, // 0x000000CF
    ErrorObjectNotFound = 208, // 0x000000D0
    ErrorColumnConversion = 245, // 0x000000F5
    ErrorTransactionCount = 266, // 0x0000010A
    ErrorInsufficientMemory = 701, // 0x000002BD
    ErrorDeadlock = 1205, // 0x000004B5
    ErrorStoredProcedureNotFound = 2812, // 0x00000AFC
    ErrorDatabaseReadOnly = 3906, // 0x00000F42
    ErrorCannotOpenDatabase = 4060, // 0x00000FDC
    ErrorViewBindingError = 4413, // 0x0000113D
    ErrorViewMoreColumnsError = 4501, // 0x00001195
    ErrorViewMoreColumnNamesError = 4502, // 0x00001196
    IndexAssociatedWithFieldError = 5074, // 0x000013D2
    ErrorFullTextBegin = 7600, // 0x00001DB0
    ErrorFullTextEnd = 7700, // 0x00001E14
    ErrorInsufficientMemoryTimeout = 8645, // 0x000021C5
    ErrorStatisticsStreamCorrupt = 9105, // 0x00002391
    FullTextServiceDisabled = 9954, // 0x000026E2
    FullTextServiceNotResponsive = 9955, // 0x000026E3
    FullTextServiceLogonFailure = 9956, // 0x000026E4
    FullTextCatalogCorrupt = 9957, // 0x000026E5
    FullTextCatalogMissing = 9958, // 0x000026E6
    ErrorConnectionForciblyClosed = 10054, // 0x00002746
    CannotReachServer = 11001, // 0x00002AF9
    ErrorServerPaused = 17142, // 0x000042F6
    ErrorLoginFailed = 18452, // 0x00004814
    ErrorFullTextBegin2 = 30000, // 0x00007530
    ErrorFullTextEnd2 = 30100, // 0x00007594
  }
}
