// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore.PsErrorEnum
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [CLSCompliant(false)]
  public enum PsErrorEnum : uint
  {
    ErrorServerItemAlreadyUpdated = 2147762176, // 0x80044000
    ErrorServerInternalUnknown = 2147762179, // 0x80044003
    ErrorServerActionFailed = 2147762180, // 0x80044004
    ErrorItemNotOpen = 2147762187, // 0x8004400B
    ErrorInvalidItem = 2147762189, // 0x8004400D
    ErrorItemIsNew = 2147762197, // 0x80044015
    ErrorHistory = 2147762202, // 0x8004401A
    ErrorNoDocument = 2147762203, // 0x8004401B
    ErrorChangesRejected = 2147762220, // 0x8004402C
    ErrorIgnoreField = 2147762221, // 0x8004402D
    ErrorCannotSortBy = 2147762222, // 0x8004402E
    ErrorFieldUsage = 2147762223, // 0x8004402F
    ErrorInvalidURL = 2147762229, // 0x80044035
    ErrorDuplicateFieldName = 2147762235, // 0x8004403B
    ErrorDuplicateNodeName = 2147762236, // 0x8004403C
    ErrorAmbiguousNameInAD = 2147762237, // 0x8004403D
    ErrorItemOpen = 2147762246, // 0x80044046
    ErrorItemOpenForEdit = 2147762248, // 0x80044048
    ErrorItemNotOpenForEdit = 2147762249, // 0x80044049
    ErrorAttachedFilesNotSupported = 2147762254, // 0x8004404E
    ErrorLinkedFilesNotSupported = 2147762255, // 0x8004404F
    ErrorQueryNestingLimit = 2147762256, // 0x80044050
    ErrorUpdateNestingLimit = 2147762257, // 0x80044051
    ErrorAttachmentAlreadyExists = 2147762263, // 0x80044057
    ErrorLinkAlreadyExists = 2147762264, // 0x80044058
    ErrorDuplicateLink = 2147762288, // 0x80044070
    ErrorLinkOrphaned = 2147762289, // 0x80044071
    ErrorPreCommit = 2147762292, // 0x80044074
    ErrorRequestNotCancelable = 2147762294, // 0x80044076
    ErrorCancelledByUser = 2147762295, // 0x80044077
    ErrorInvalidTreeNodeValue = 2147762297, // 0x80044079
    ErrorReadOnlyField = 2147762298, // 0x8004407A
    ErrorItemOrphaned = 2147762299, // 0x8004407B
    WebExceptionConnectFailed = 2147762301, // 0x8004407D
    WebExceptionCouldNotReachServer = 2147762302, // 0x8004407E
    WebExceptionTimeOut = 2147762303, // 0x8004407F
    WebExceptionAccessDenied = 2147762304, // 0x80044080
    BackendArgumentException = 2147762305, // 0x80044081
    MAP_DB_ErrorObjectNotFound = 2147762306, // 0x80044082
    MAP_DB_ErrorTransactionCount = 2147762307, // 0x80044083
    MAP_DB_ErrorDeadlock = 2147762308, // 0x80044084
    MAP_DB_ErrorColumnNotFound = 2147762309, // 0x80044085
    MAP_DB_ErrorDatabaseReadOnly = 2147762310, // 0x80044086
    MAP_DB_ErrorDatabaseCustomError = 2147762311, // 0x80044087
    MAP_DB_ErrorViewBindingError = 2147762312, // 0x80044088
    MAP_DB_ErrorColumnConversion = 2147762313, // 0x80044089
    MAP_DB_ErrorFullText = 2147762314, // 0x8004408A
    MAP_DB_ErrorLoginFailed = 2147762316, // 0x8004408C
    MAP_DB_ErrorConnectionForciblyClosed = 2147762317, // 0x8004408D
    MAP_DB_ErrorTimeOutOrServerNotResponding = 2147762318, // 0x8004408E
    MAP_DB_ErrorCannotOpenDatabase = 2147762319, // 0x8004408F
    MAP_DB_ErrorInsufficientMemory = 2147762320, // 0x80044090
    MAP_DB_ErrorServerPaused = 2147762321, // 0x80044091
    MAP_DB_ErrorSevereError = 2147762322, // 0x80044092
    MAP_DB_ErrorDatetimeShiftDetected = 2147762323, // 0x80044093
    MAP_DB_ErrorViewMoreColumnsError = 2147762324, // 0x80044094
    MAP_DB_ErrorViewMoreColumnNamesError = 2147762325, // 0x80044095
    MAP_DB_ErrorStoredProcedureNotFound = 2147762326, // 0x80044096
    MAP_DB_IndexAssociatedWithFieldError = 2147762327, // 0x80044097
  }
}
