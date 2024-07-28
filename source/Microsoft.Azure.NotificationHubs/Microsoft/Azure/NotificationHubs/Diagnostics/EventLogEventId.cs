// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Diagnostics.EventLogEventId
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs.Diagnostics
{
  internal enum EventLogEventId : uint
  {
    ServiceAuthorizationSuccess = 1074135041, // 0x40060001
    MessageAuthenticationSuccess = 1074135043, // 0x40060003
    SecurityNegotiationSuccess = 1074135045, // 0x40060005
    TransportAuthenticationSuccess = 1074135047, // 0x40060007
    ImpersonationSuccess = 1074135049, // 0x40060009
    FailedToSetupTracing = 3221291108, // 0xC0010064
    FailedToInitializeTraceSource = 3221291109, // 0xC0010065
    FailFast = 3221291110, // 0xC0010066
    FailFastException = 3221291111, // 0xC0010067
    FailedToTraceEvent = 3221291112, // 0xC0010068
    FailedToTraceEventWithException = 3221291113, // 0xC0010069
    InvariantAssertionFailed = 3221291114, // 0xC001006A
    PiiLoggingOn = 3221291115, // 0xC001006B
    PiiLoggingNotAllowed = 3221291116, // 0xC001006C
    WebHostUnhandledException = 3221356545, // 0xC0020001
    WebHostHttpError = 3221356546, // 0xC0020002
    WebHostFailedToProcessRequest = 3221356547, // 0xC0020003
    WebHostFailedToListen = 3221356548, // 0xC0020004
    FailedToLogMessage = 3221356549, // 0xC0020005
    RemovedBadFilter = 3221356550, // 0xC0020006
    FailedToCreateMessageLoggingTraceSource = 3221356551, // 0xC0020007
    MessageLoggingOn = 3221356552, // 0xC0020008
    MessageLoggingOff = 3221356553, // 0xC0020009
    FailedToLoadPerformanceCounter = 3221356554, // 0xC002000A
    FailedToRemovePerformanceCounter = 3221356555, // 0xC002000B
    WmiGetObjectFailed = 3221356556, // 0xC002000C
    WmiPutInstanceFailed = 3221356557, // 0xC002000D
    WmiDeleteInstanceFailed = 3221356558, // 0xC002000E
    WmiCreateInstanceFailed = 3221356559, // 0xC002000F
    WmiExecQueryFailed = 3221356560, // 0xC0020010
    WmiExecMethodFailed = 3221356561, // 0xC0020011
    WmiRegistrationFailed = 3221356562, // 0xC0020012
    WmiUnregistrationFailed = 3221356563, // 0xC0020013
    WmiAdminTypeMismatch = 3221356564, // 0xC0020014
    WmiPropertyMissing = 3221356565, // 0xC0020015
    ComPlusServiceHostStartingServiceError = 3221356566, // 0xC0020016
    ComPlusDllHostInitializerStartingError = 3221356567, // 0xC0020017
    ComPlusTLBImportError = 3221356568, // 0xC0020018
    ComPlusInvokingMethodFailed = 3221356569, // 0xC0020019
    ComPlusInstanceCreationError = 3221356570, // 0xC002001A
    ComPlusInvokingMethodFailedMismatchedTransactions = 3221356571, // 0xC002001B
    UnhandledStateMachineExceptionRecordDescription = 3221422081, // 0xC0030001
    FatalUnexpectedStateMachineEvent = 3221422082, // 0xC0030002
    ParticipantRecoveryLogEntryCorrupt = 3221422083, // 0xC0030003
    CoordinatorRecoveryLogEntryCorrupt = 3221422084, // 0xC0030004
    CoordinatorRecoveryLogEntryCreationFailure = 3221422085, // 0xC0030005
    ParticipantRecoveryLogEntryCreationFailure = 3221422086, // 0xC0030006
    ProtocolInitializationFailure = 3221422087, // 0xC0030007
    ProtocolStartFailure = 3221422088, // 0xC0030008
    ProtocolRecoveryBeginningFailure = 3221422089, // 0xC0030009
    ProtocolRecoveryCompleteFailure = 3221422090, // 0xC003000A
    TransactionBridgeRecoveryFailure = 3221422091, // 0xC003000B
    ProtocolStopFailure = 3221422092, // 0xC003000C
    NonFatalUnexpectedStateMachineEvent = 3221422093, // 0xC003000D
    PerformanceCounterInitializationFailure = 3221422094, // 0xC003000E
    ProtocolRecoveryComplete = 3221422095, // 0xC003000F
    ProtocolStopped = 3221422096, // 0xC0030010
    ThumbPrintNotFound = 3221422097, // 0xC0030011
    ThumbPrintNotValidated = 3221422098, // 0xC0030012
    SslNoPrivateKey = 3221422099, // 0xC0030013
    SslNoAccessiblePrivateKey = 3221422100, // 0xC0030014
    MissingNecessaryKeyUsage = 3221422101, // 0xC0030015
    MissingNecessaryEnhancedKeyUsage = 3221422102, // 0xC0030016
    StartErrorPublish = 3221487617, // 0xC0040001
    BindingError = 3221487618, // 0xC0040002
    LAFailedToListenForApp = 3221487619, // 0xC0040003
    UnknownListenerAdapterError = 3221487620, // 0xC0040004
    WasDisconnected = 3221487621, // 0xC0040005
    WasConnectionTimedout = 3221487622, // 0xC0040006
    ServiceStartFailed = 3221487623, // 0xC0040007
    MessageQueueDuplicatedSocketLeak = 3221487624, // 0xC0040008
    MessageQueueDuplicatedPipeLeak = 3221487625, // 0xC0040009
    SharingUnhandledException = 3221487626, // 0xC004000A
    ServiceAuthorizationFailure = 3221618690, // 0xC0060002
    MessageAuthenticationFailure = 3221618692, // 0xC0060004
    SecurityNegotiationFailure = 3221618694, // 0xC0060006
    TransportAuthenticationFailure = 3221618696, // 0xC0060008
    ImpersonationFailure = 3221618698, // 0xC006000A
  }
}
