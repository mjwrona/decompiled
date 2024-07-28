// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.StatementProcessingErrorCode
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

namespace Microsoft.Cloud.Metrics.Client.Metrics
{
  public enum StatementProcessingErrorCode
  {
    StatementCompilationFailure = 0,
    LiteralCannotBeParsed = 1001, // 0x000003E9
    UnexpectedTsDeclarationSyntax = 1002, // 0x000003EA
    UnexpectedProjectOperationSyntax = 1003, // 0x000003EB
    UnexpectedTopOperationSyntax = 1004, // 0x000003EC
    UnexpectedZoomOperationSyntax = 1005, // 0x000003ED
    UnexpectedSummarizeOperationSyntax = 1006, // 0x000003EE
    UnexpectedWhereOperationSyntax = 1007, // 0x000003EF
    UnexpectedJoinOperationSyntax = 1008, // 0x000003F0
    UnexpectedQueryStatementSyntax = 1009, // 0x000003F1
    EmptyQueryStatementSyntax = 1010, // 0x000003F2
    DuplicateDefinitionSyntax = 1011, // 0x000003F3
    UnexpectedInOperationSyntax = 1012, // 0x000003F4
    UnexpectedStartsWithOperationSyntax = 1013, // 0x000003F5
    UnexpectedContainsOperationSyntax = 1014, // 0x000003F6
    UnexpectedIifFunctionSyntax = 1015, // 0x000003F7
    UnexpectedIsNullFunctionSyntax = 1016, // 0x000003F8
    UnexpectedForeachOperatorSyntax = 1017, // 0x000003F9
    AmbiguousSamplingTypeOrDimension = 2000, // 0x000007D0
    NameDoesNotExist = 2001, // 0x000007D1
    DuplicateIdentifier = 2002, // 0x000007D2
    JoinSamplingTypesConflict = 2003, // 0x000007D3
    IncorrectProjectedExpression = 2004, // 0x000007D4
    NamedSamplingTypeAggregationExpected = 2005, // 0x000007D5
    IncorrectSummarizedDimension = 2006, // 0x000007D6
    ScalarOperatorIncompatibleTypes = 2007, // 0x000007D7
    UnexpectedExpressionDataType = 2008, // 0x000007D8
    MetricDoesNotExist = 2009, // 0x000007D9
    DimensionDoesNotExist = 2010, // 0x000007DA
    NoMatchingPerAggregateForSamplingType = 2011, // 0x000007DB
    DuplicateDimension = 2013, // 0x000007DD
    DuplicateSamplingType = 2014, // 0x000007DE
    NoSamplingTypesDefined = 2015, // 0x000007DF
    IncorrectAggregationFunction = 2018, // 0x000007E2
    IncorrectTopNRecordsNumber = 2019, // 0x000007E3
    SamplingTypeIsExpected = 2020, // 0x000007E4
    SamplingTypeExpressionExpected = 2023, // 0x000007E7
    UnexpectedZoomTimeSpan = 2024, // 0x000007E8
    IncorrectZoomTimeSpan = 2025, // 0x000007E9
    JoinIncompatibleDimensions = 2026, // 0x000007EA
    ZoomOverNonDefaultResolution = 2027, // 0x000007EB
    JoinIncompatibleResolutions = 2028, // 0x000007EC
    NoMatchingPerAggregateForDimnesions = 2029, // 0x000007ED
    PreaggregateDoesNotExist = 2030, // 0x000007EE
    MetricNamespaceUndefined = 2031, // 0x000007EF
    InOperatorIncompatibleTypes = 2032, // 0x000007F0
    InOperatorRightSideEmpty = 2033, // 0x000007F1
    UnexpectedIteratorSourceDataType = 2034, // 0x000007F2
    IifOperatorIncompatibleTypes = 2035, // 0x000007F3
    EqualityWithNullIsNotAllowed = 2036, // 0x000007F4
    OperationDisabled = 2037, // 0x000007F5
    AccountDoesNotExist = 2038, // 0x000007F6
    GeneralRuntimeMessage = 5000, // 0x00001388
    NoRawSamplingType = 5001, // 0x00001389
    HintsRetrievalFailed = 5002, // 0x0000138A
    QueryThrottled = 5004, // 0x0000138C
    BuildQueryFailed = 5005, // 0x0000138D
    QueryRequestTypeNotSupported = 5007, // 0x0000138F
    DataCorruptionDetected = 5008, // 0x00001390
    NaNValueInSamplingType = 5009, // 0x00001391
    WrongResultVersion = 5010, // 0x00001392
    NoOutPutMetric = 5011, // 0x00001393
    MStoreTooLargeHistogram = 5012, // 0x00001394
    MStoreTooManyBuckets = 5013, // 0x00001395
    NoSeriesReturnedFromDataSource = 5014, // 0x00001396
    IncorrectArgumentInQueryRequest = 5101, // 0x000013ED
    BuildQueryRequestFailed = 5102, // 0x000013EE
    InvalidInputSamplingType = 5103, // 0x000013EF
    LargeInputSeriesForQueryTask = 6001, // 0x00001771
    ExecutionTimeExceedsSLA = 6002, // 0x00001772
    PartialDataReturned = 6003, // 0x00001773
    QueryTimeOut = 6004, // 0x00001774
    InvalidDistinctCountAggregation = 6005, // 0x00001775
    Information = 7000, // 0x00001B58
  }
}
