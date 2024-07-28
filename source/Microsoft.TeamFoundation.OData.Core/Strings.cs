// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Strings
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData
{
  internal static class Strings
  {
    internal static string ExceptionUtils_ArgumentStringEmpty => TextRes.GetString(nameof (ExceptionUtils_ArgumentStringEmpty));

    internal static string ODataRequestMessage_AsyncNotAvailable => TextRes.GetString(nameof (ODataRequestMessage_AsyncNotAvailable));

    internal static string ODataRequestMessage_StreamTaskIsNull => TextRes.GetString(nameof (ODataRequestMessage_StreamTaskIsNull));

    internal static string ODataRequestMessage_MessageStreamIsNull => TextRes.GetString(nameof (ODataRequestMessage_MessageStreamIsNull));

    internal static string ODataResponseMessage_AsyncNotAvailable => TextRes.GetString(nameof (ODataResponseMessage_AsyncNotAvailable));

    internal static string ODataResponseMessage_StreamTaskIsNull => TextRes.GetString(nameof (ODataResponseMessage_StreamTaskIsNull));

    internal static string ODataResponseMessage_MessageStreamIsNull => TextRes.GetString(nameof (ODataResponseMessage_MessageStreamIsNull));

    internal static string AsyncBufferedStream_WriterDisposedWithoutFlush => TextRes.GetString(nameof (AsyncBufferedStream_WriterDisposedWithoutFlush));

    internal static string ODataFormat_AtomFormatObsoleted => TextRes.GetString(nameof (ODataFormat_AtomFormatObsoleted));

    internal static string ODataOutputContext_UnsupportedPayloadKindForFormat(object p0, object p1) => TextRes.GetString(nameof (ODataOutputContext_UnsupportedPayloadKindForFormat), p0, p1);

    internal static string ODataInputContext_UnsupportedPayloadKindForFormat(object p0, object p1) => TextRes.GetString(nameof (ODataInputContext_UnsupportedPayloadKindForFormat), p0, p1);

    internal static string ODataOutputContext_MetadataDocumentUriMissing => TextRes.GetString(nameof (ODataOutputContext_MetadataDocumentUriMissing));

    internal static string ODataJsonLightSerializer_RelativeUriUsedWithoutMetadataDocumentUriOrMetadata(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightSerializer_RelativeUriUsedWithoutMetadataDocumentUriOrMetadata), p0);
    }

    internal static string ODataWriter_RelativeUriUsedWithoutBaseUriSpecified(object p0) => TextRes.GetString(nameof (ODataWriter_RelativeUriUsedWithoutBaseUriSpecified), p0);

    internal static string ODataWriter_StreamPropertiesMustBePropertiesOfODataResource(object p0) => TextRes.GetString(nameof (ODataWriter_StreamPropertiesMustBePropertiesOfODataResource), p0);

    internal static string ODataWriterCore_InvalidStateTransition(object p0, object p1) => TextRes.GetString(nameof (ODataWriterCore_InvalidStateTransition), p0, p1);

    internal static string ODataWriterCore_InvalidTransitionFromStart(object p0, object p1) => TextRes.GetString(nameof (ODataWriterCore_InvalidTransitionFromStart), p0, p1);

    internal static string ODataWriterCore_InvalidTransitionFromResource(object p0, object p1) => TextRes.GetString(nameof (ODataWriterCore_InvalidTransitionFromResource), p0, p1);

    internal static string ODataWriterCore_InvalidTransitionFrom40DeletedResource(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataWriterCore_InvalidTransitionFrom40DeletedResource), p0, p1);
    }

    internal static string ODataWriterCore_InvalidTransitionFromNullResource(object p0, object p1) => TextRes.GetString(nameof (ODataWriterCore_InvalidTransitionFromNullResource), p0, p1);

    internal static string ODataWriterCore_InvalidTransitionFromResourceSet(object p0, object p1) => TextRes.GetString(nameof (ODataWriterCore_InvalidTransitionFromResourceSet), p0, p1);

    internal static string ODataWriterCore_InvalidTransitionFromExpandedLink(object p0, object p1) => TextRes.GetString(nameof (ODataWriterCore_InvalidTransitionFromExpandedLink), p0, p1);

    internal static string ODataWriterCore_InvalidTransitionFromCompleted(object p0, object p1) => TextRes.GetString(nameof (ODataWriterCore_InvalidTransitionFromCompleted), p0, p1);

    internal static string ODataWriterCore_InvalidTransitionFromError(object p0, object p1) => TextRes.GetString(nameof (ODataWriterCore_InvalidTransitionFromError), p0, p1);

    internal static string ODataJsonLightDeltaWriter_InvalidTransitionFromNestedResource(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightDeltaWriter_InvalidTransitionFromNestedResource), p0, p1);
    }

    internal static string ODataJsonLightDeltaWriter_InvalidTransitionToNestedResource(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightDeltaWriter_InvalidTransitionToNestedResource), p0, p1);
    }

    internal static string ODataJsonLightDeltaWriter_WriteStartExpandedResourceSetCalledInInvalidState(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightDeltaWriter_WriteStartExpandedResourceSetCalledInInvalidState), p0);
    }

    internal static string ODataWriterCore_WriteEndCalledInInvalidState(object p0) => TextRes.GetString(nameof (ODataWriterCore_WriteEndCalledInInvalidState), p0);

    internal static string ODataWriterCore_StreamNotDisposed => TextRes.GetString(nameof (ODataWriterCore_StreamNotDisposed));

    internal static string ODataWriterCore_DeltaResourceWithoutIdOrKeyProperties => TextRes.GetString(nameof (ODataWriterCore_DeltaResourceWithoutIdOrKeyProperties));

    internal static string ODataWriterCore_QueryCountInRequest => TextRes.GetString(nameof (ODataWriterCore_QueryCountInRequest));

    internal static string ODataWriterCore_QueryNextLinkInRequest => TextRes.GetString(nameof (ODataWriterCore_QueryNextLinkInRequest));

    internal static string ODataWriterCore_QueryDeltaLinkInRequest => TextRes.GetString(nameof (ODataWriterCore_QueryDeltaLinkInRequest));

    internal static string ODataWriterCore_CannotWriteDeltaWithResourceSetWriter => TextRes.GetString(nameof (ODataWriterCore_CannotWriteDeltaWithResourceSetWriter));

    internal static string ODataWriterCore_NestedContentNotAllowedIn40DeletedEntry => TextRes.GetString(nameof (ODataWriterCore_NestedContentNotAllowedIn40DeletedEntry));

    internal static string ODataWriterCore_CannotWriteTopLevelResourceSetWithResourceWriter => TextRes.GetString(nameof (ODataWriterCore_CannotWriteTopLevelResourceSetWithResourceWriter));

    internal static string ODataWriterCore_CannotWriteTopLevelResourceWithResourceSetWriter => TextRes.GetString(nameof (ODataWriterCore_CannotWriteTopLevelResourceWithResourceSetWriter));

    internal static string ODataWriterCore_SyncCallOnAsyncWriter => TextRes.GetString(nameof (ODataWriterCore_SyncCallOnAsyncWriter));

    internal static string ODataWriterCore_AsyncCallOnSyncWriter => TextRes.GetString(nameof (ODataWriterCore_AsyncCallOnSyncWriter));

    internal static string ODataWriterCore_EntityReferenceLinkWithoutNavigationLink => TextRes.GetString(nameof (ODataWriterCore_EntityReferenceLinkWithoutNavigationLink));

    internal static string ODataWriterCore_DeferredLinkInRequest => TextRes.GetString(nameof (ODataWriterCore_DeferredLinkInRequest));

    internal static string ODataWriterCore_MultipleItemsInNestedResourceInfoWithContent => TextRes.GetString(nameof (ODataWriterCore_MultipleItemsInNestedResourceInfoWithContent));

    internal static string ODataWriterCore_DeltaLinkNotSupportedOnExpandedResourceSet => TextRes.GetString(nameof (ODataWriterCore_DeltaLinkNotSupportedOnExpandedResourceSet));

    internal static string ODataWriterCore_PathInODataUriMustBeSetWhenWritingContainedElement => TextRes.GetString(nameof (ODataWriterCore_PathInODataUriMustBeSetWhenWritingContainedElement));

    internal static string DuplicatePropertyNamesNotAllowed(object p0) => TextRes.GetString(nameof (DuplicatePropertyNamesNotAllowed), p0);

    internal static string DuplicateAnnotationNotAllowed(object p0) => TextRes.GetString(nameof (DuplicateAnnotationNotAllowed), p0);

    internal static string DuplicateAnnotationForPropertyNotAllowed(object p0, object p1) => TextRes.GetString(nameof (DuplicateAnnotationForPropertyNotAllowed), p0, p1);

    internal static string DuplicateAnnotationForInstanceAnnotationNotAllowed(object p0, object p1) => TextRes.GetString(nameof (DuplicateAnnotationForInstanceAnnotationNotAllowed), p0, p1);

    internal static string PropertyAnnotationAfterTheProperty(object p0, object p1) => TextRes.GetString(nameof (PropertyAnnotationAfterTheProperty), p0, p1);

    internal static string AtomValueUtils_CannotConvertValueToAtomPrimitive(object p0) => TextRes.GetString(nameof (AtomValueUtils_CannotConvertValueToAtomPrimitive), p0);

    internal static string ODataJsonWriter_UnsupportedValueType(object p0) => TextRes.GetString(nameof (ODataJsonWriter_UnsupportedValueType), p0);

    internal static string ODataJsonWriter_UnsupportedValueInCollection => TextRes.GetString(nameof (ODataJsonWriter_UnsupportedValueInCollection));

    internal static string ODataException_GeneralError => TextRes.GetString(nameof (ODataException_GeneralError));

    internal static string ODataErrorException_GeneralError => TextRes.GetString(nameof (ODataErrorException_GeneralError));

    internal static string ODataUriParserException_GeneralError => TextRes.GetString(nameof (ODataUriParserException_GeneralError));

    internal static string ODataMessageWriter_WriterAlreadyUsed => TextRes.GetString(nameof (ODataMessageWriter_WriterAlreadyUsed));

    internal static string ODataMessageWriter_EntityReferenceLinksInRequestNotAllowed => TextRes.GetString(nameof (ODataMessageWriter_EntityReferenceLinksInRequestNotAllowed));

    internal static string ODataMessageWriter_ErrorPayloadInRequest => TextRes.GetString(nameof (ODataMessageWriter_ErrorPayloadInRequest));

    internal static string ODataMessageWriter_ServiceDocumentInRequest => TextRes.GetString(nameof (ODataMessageWriter_ServiceDocumentInRequest));

    internal static string ODataMessageWriter_MetadataDocumentInRequest => TextRes.GetString(nameof (ODataMessageWriter_MetadataDocumentInRequest));

    internal static string ODataMessageWriter_DeltaInRequest => TextRes.GetString(nameof (ODataMessageWriter_DeltaInRequest));

    internal static string ODataMessageWriter_AsyncInRequest => TextRes.GetString(nameof (ODataMessageWriter_AsyncInRequest));

    internal static string ODataMessageWriter_CannotWriteTopLevelNull => TextRes.GetString(nameof (ODataMessageWriter_CannotWriteTopLevelNull));

    internal static string ODataMessageWriter_CannotWriteNullInRawFormat => TextRes.GetString(nameof (ODataMessageWriter_CannotWriteNullInRawFormat));

    internal static string ODataMessageWriter_CannotSetHeadersWithInvalidPayloadKind(object p0) => TextRes.GetString(nameof (ODataMessageWriter_CannotSetHeadersWithInvalidPayloadKind), p0);

    internal static string ODataMessageWriter_IncompatiblePayloadKinds(object p0, object p1) => TextRes.GetString(nameof (ODataMessageWriter_IncompatiblePayloadKinds), p0, p1);

    internal static string ODataMessageWriter_CannotWriteStreamPropertyAsTopLevelProperty(object p0) => TextRes.GetString(nameof (ODataMessageWriter_CannotWriteStreamPropertyAsTopLevelProperty), p0);

    internal static string ODataMessageWriter_WriteErrorAlreadyCalled => TextRes.GetString(nameof (ODataMessageWriter_WriteErrorAlreadyCalled));

    internal static string ODataMessageWriter_CannotWriteInStreamErrorForRawValues => TextRes.GetString(nameof (ODataMessageWriter_CannotWriteInStreamErrorForRawValues));

    internal static string ODataMessageWriter_CannotWriteMetadataWithoutModel => TextRes.GetString(nameof (ODataMessageWriter_CannotWriteMetadataWithoutModel));

    internal static string ODataMessageWriter_CannotSpecifyOperationWithoutModel => TextRes.GetString(nameof (ODataMessageWriter_CannotSpecifyOperationWithoutModel));

    internal static string ODataMessageWriter_JsonPaddingOnInvalidContentType(object p0) => TextRes.GetString(nameof (ODataMessageWriter_JsonPaddingOnInvalidContentType), p0);

    internal static string ODataMessageWriter_NonCollectionType(object p0) => TextRes.GetString(nameof (ODataMessageWriter_NonCollectionType), p0);

    internal static string ODataMessageWriter_NotAllowedWriteTopLevelPropertyWithResourceValue(
      object p0)
    {
      return TextRes.GetString(nameof (ODataMessageWriter_NotAllowedWriteTopLevelPropertyWithResourceValue), p0);
    }

    internal static string ODataMessageWriterSettings_MessageWriterSettingsXmlCustomizationCallbacksMustBeSpecifiedBoth => TextRes.GetString(nameof (ODataMessageWriterSettings_MessageWriterSettingsXmlCustomizationCallbacksMustBeSpecifiedBoth));

    internal static string ODataCollectionWriterCore_InvalidTransitionFromStart(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataCollectionWriterCore_InvalidTransitionFromStart), p0, p1);
    }

    internal static string ODataCollectionWriterCore_InvalidTransitionFromCollection(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataCollectionWriterCore_InvalidTransitionFromCollection), p0, p1);
    }

    internal static string ODataCollectionWriterCore_InvalidTransitionFromItem(object p0, object p1) => TextRes.GetString(nameof (ODataCollectionWriterCore_InvalidTransitionFromItem), p0, p1);

    internal static string ODataCollectionWriterCore_WriteEndCalledInInvalidState(object p0) => TextRes.GetString(nameof (ODataCollectionWriterCore_WriteEndCalledInInvalidState), p0);

    internal static string ODataCollectionWriterCore_SyncCallOnAsyncWriter => TextRes.GetString(nameof (ODataCollectionWriterCore_SyncCallOnAsyncWriter));

    internal static string ODataCollectionWriterCore_AsyncCallOnSyncWriter => TextRes.GetString(nameof (ODataCollectionWriterCore_AsyncCallOnSyncWriter));

    internal static string ODataBatch_InvalidHttpMethodForChangeSetRequest(object p0) => TextRes.GetString(nameof (ODataBatch_InvalidHttpMethodForChangeSetRequest), p0);

    internal static string ODataBatchOperationHeaderDictionary_KeyNotFound(object p0) => TextRes.GetString(nameof (ODataBatchOperationHeaderDictionary_KeyNotFound), p0);

    internal static string ODataBatchOperationHeaderDictionary_DuplicateCaseInsensitiveKeys(
      object p0)
    {
      return TextRes.GetString(nameof (ODataBatchOperationHeaderDictionary_DuplicateCaseInsensitiveKeys), p0);
    }

    internal static string ODataParameterWriter_InStreamErrorNotSupported => TextRes.GetString(nameof (ODataParameterWriter_InStreamErrorNotSupported));

    internal static string ODataParameterWriter_CannotCreateParameterWriterOnResponseMessage => TextRes.GetString(nameof (ODataParameterWriter_CannotCreateParameterWriterOnResponseMessage));

    internal static string ODataParameterWriterCore_SyncCallOnAsyncWriter => TextRes.GetString(nameof (ODataParameterWriterCore_SyncCallOnAsyncWriter));

    internal static string ODataParameterWriterCore_AsyncCallOnSyncWriter => TextRes.GetString(nameof (ODataParameterWriterCore_AsyncCallOnSyncWriter));

    internal static string ODataParameterWriterCore_CannotWriteStart => TextRes.GetString(nameof (ODataParameterWriterCore_CannotWriteStart));

    internal static string ODataParameterWriterCore_CannotWriteParameter => TextRes.GetString(nameof (ODataParameterWriterCore_CannotWriteParameter));

    internal static string ODataParameterWriterCore_CannotWriteEnd => TextRes.GetString(nameof (ODataParameterWriterCore_CannotWriteEnd));

    internal static string ODataParameterWriterCore_CannotWriteInErrorOrCompletedState => TextRes.GetString(nameof (ODataParameterWriterCore_CannotWriteInErrorOrCompletedState));

    internal static string ODataParameterWriterCore_DuplicatedParameterNameNotAllowed(object p0) => TextRes.GetString(nameof (ODataParameterWriterCore_DuplicatedParameterNameNotAllowed), p0);

    internal static string ODataParameterWriterCore_CannotWriteValueOnNonValueTypeKind(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataParameterWriterCore_CannotWriteValueOnNonValueTypeKind), p0, p1);
    }

    internal static string ODataParameterWriterCore_CannotWriteValueOnNonSupportedValueType(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataParameterWriterCore_CannotWriteValueOnNonSupportedValueType), p0, p1);
    }

    internal static string ODataParameterWriterCore_CannotCreateCollectionWriterOnNonCollectionTypeKind(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataParameterWriterCore_CannotCreateCollectionWriterOnNonCollectionTypeKind), p0, p1);
    }

    internal static string ODataParameterWriterCore_CannotCreateResourceWriterOnNonEntityOrComplexTypeKind(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataParameterWriterCore_CannotCreateResourceWriterOnNonEntityOrComplexTypeKind), p0, p1);
    }

    internal static string ODataParameterWriterCore_CannotCreateResourceSetWriterOnNonStructuredCollectionTypeKind(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataParameterWriterCore_CannotCreateResourceSetWriterOnNonStructuredCollectionTypeKind), p0, p1);
    }

    internal static string ODataParameterWriterCore_ParameterNameNotFoundInOperation(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataParameterWriterCore_ParameterNameNotFoundInOperation), p0, p1);
    }

    internal static string ODataParameterWriterCore_MissingParameterInParameterPayload(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataParameterWriterCore_MissingParameterInParameterPayload), p0, p1);
    }

    internal static string ODataBatchWriter_FlushOrFlushAsyncCalledInStreamRequestedState => TextRes.GetString(nameof (ODataBatchWriter_FlushOrFlushAsyncCalledInStreamRequestedState));

    internal static string ODataBatchWriter_CannotCompleteBatchWithActiveChangeSet => TextRes.GetString(nameof (ODataBatchWriter_CannotCompleteBatchWithActiveChangeSet));

    internal static string ODataBatchWriter_CannotStartChangeSetWithActiveChangeSet => TextRes.GetString(nameof (ODataBatchWriter_CannotStartChangeSetWithActiveChangeSet));

    internal static string ODataBatchWriter_CannotCompleteChangeSetWithoutActiveChangeSet => TextRes.GetString(nameof (ODataBatchWriter_CannotCompleteChangeSetWithoutActiveChangeSet));

    internal static string ODataBatchWriter_InvalidTransitionFromStart => TextRes.GetString(nameof (ODataBatchWriter_InvalidTransitionFromStart));

    internal static string ODataBatchWriter_InvalidTransitionFromBatchStarted => TextRes.GetString(nameof (ODataBatchWriter_InvalidTransitionFromBatchStarted));

    internal static string ODataBatchWriter_InvalidTransitionFromChangeSetStarted => TextRes.GetString(nameof (ODataBatchWriter_InvalidTransitionFromChangeSetStarted));

    internal static string ODataBatchWriter_InvalidTransitionFromOperationCreated => TextRes.GetString(nameof (ODataBatchWriter_InvalidTransitionFromOperationCreated));

    internal static string ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested => TextRes.GetString(nameof (ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested));

    internal static string ODataBatchWriter_InvalidTransitionFromOperationContentStreamDisposed => TextRes.GetString(nameof (ODataBatchWriter_InvalidTransitionFromOperationContentStreamDisposed));

    internal static string ODataBatchWriter_InvalidTransitionFromChangeSetCompleted => TextRes.GetString(nameof (ODataBatchWriter_InvalidTransitionFromChangeSetCompleted));

    internal static string ODataBatchWriter_InvalidTransitionFromBatchCompleted => TextRes.GetString(nameof (ODataBatchWriter_InvalidTransitionFromBatchCompleted));

    internal static string ODataBatchWriter_CannotCreateRequestOperationWhenWritingResponse => TextRes.GetString(nameof (ODataBatchWriter_CannotCreateRequestOperationWhenWritingResponse));

    internal static string ODataBatchWriter_CannotCreateResponseOperationWhenWritingRequest => TextRes.GetString(nameof (ODataBatchWriter_CannotCreateResponseOperationWhenWritingRequest));

    internal static string ODataBatchWriter_MaxBatchSizeExceeded(object p0) => TextRes.GetString(nameof (ODataBatchWriter_MaxBatchSizeExceeded), p0);

    internal static string ODataBatchWriter_MaxChangeSetSizeExceeded(object p0) => TextRes.GetString(nameof (ODataBatchWriter_MaxChangeSetSizeExceeded), p0);

    internal static string ODataBatchWriter_SyncCallOnAsyncWriter => TextRes.GetString(nameof (ODataBatchWriter_SyncCallOnAsyncWriter));

    internal static string ODataBatchWriter_AsyncCallOnSyncWriter => TextRes.GetString(nameof (ODataBatchWriter_AsyncCallOnSyncWriter));

    internal static string ODataBatchWriter_DuplicateContentIDsNotAllowed(object p0) => TextRes.GetString(nameof (ODataBatchWriter_DuplicateContentIDsNotAllowed), p0);

    internal static string ODataBatchWriter_CannotWriteInStreamErrorForBatch => TextRes.GetString(nameof (ODataBatchWriter_CannotWriteInStreamErrorForBatch));

    internal static string ODataBatchUtils_RelativeUriUsedWithoutBaseUriSpecified(object p0) => TextRes.GetString(nameof (ODataBatchUtils_RelativeUriUsedWithoutBaseUriSpecified), p0);

    internal static string ODataBatchUtils_RelativeUriStartingWithDollarUsedWithoutBaseUriSpecified(
      object p0)
    {
      return TextRes.GetString(nameof (ODataBatchUtils_RelativeUriStartingWithDollarUsedWithoutBaseUriSpecified), p0);
    }

    internal static string ODataBatchOperationMessage_VerifyNotCompleted => TextRes.GetString(nameof (ODataBatchOperationMessage_VerifyNotCompleted));

    internal static string ODataBatchOperationStream_Disposed => TextRes.GetString(nameof (ODataBatchOperationStream_Disposed));

    internal static string ODataBatchReader_CannotCreateRequestOperationWhenReadingResponse => TextRes.GetString(nameof (ODataBatchReader_CannotCreateRequestOperationWhenReadingResponse));

    internal static string ODataBatchReader_CannotCreateResponseOperationWhenReadingRequest => TextRes.GetString(nameof (ODataBatchReader_CannotCreateResponseOperationWhenReadingRequest));

    internal static string ODataBatchReader_InvalidStateForCreateOperationRequestMessage(object p0) => TextRes.GetString(nameof (ODataBatchReader_InvalidStateForCreateOperationRequestMessage), p0);

    internal static string ODataBatchReader_OperationRequestMessageAlreadyCreated => TextRes.GetString(nameof (ODataBatchReader_OperationRequestMessageAlreadyCreated));

    internal static string ODataBatchReader_OperationResponseMessageAlreadyCreated => TextRes.GetString(nameof (ODataBatchReader_OperationResponseMessageAlreadyCreated));

    internal static string ODataBatchReader_InvalidStateForCreateOperationResponseMessage(object p0) => TextRes.GetString(nameof (ODataBatchReader_InvalidStateForCreateOperationResponseMessage), p0);

    internal static string ODataBatchReader_CannotUseReaderWhileOperationStreamActive => TextRes.GetString(nameof (ODataBatchReader_CannotUseReaderWhileOperationStreamActive));

    internal static string ODataBatchReader_SyncCallOnAsyncReader => TextRes.GetString(nameof (ODataBatchReader_SyncCallOnAsyncReader));

    internal static string ODataBatchReader_AsyncCallOnSyncReader => TextRes.GetString(nameof (ODataBatchReader_AsyncCallOnSyncReader));

    internal static string ODataBatchReader_ReadOrReadAsyncCalledInInvalidState(object p0) => TextRes.GetString(nameof (ODataBatchReader_ReadOrReadAsyncCalledInInvalidState), p0);

    internal static string ODataBatchReader_MaxBatchSizeExceeded(object p0) => TextRes.GetString(nameof (ODataBatchReader_MaxBatchSizeExceeded), p0);

    internal static string ODataBatchReader_MaxChangeSetSizeExceeded(object p0) => TextRes.GetString(nameof (ODataBatchReader_MaxChangeSetSizeExceeded), p0);

    internal static string ODataBatchReader_NoMessageWasCreatedForOperation => TextRes.GetString(nameof (ODataBatchReader_NoMessageWasCreatedForOperation));

    internal static string ODataBatchReader_ReaderModeNotInitilized => TextRes.GetString(nameof (ODataBatchReader_ReaderModeNotInitilized));

    internal static string ODataBatchReader_JsonBatchTopLevelPropertyMissing => TextRes.GetString(nameof (ODataBatchReader_JsonBatchTopLevelPropertyMissing));

    internal static string ODataBatchReader_DuplicateContentIDsNotAllowed(object p0) => TextRes.GetString(nameof (ODataBatchReader_DuplicateContentIDsNotAllowed), p0);

    internal static string ODataBatchReader_DuplicateAtomicityGroupIDsNotAllowed(object p0) => TextRes.GetString(nameof (ODataBatchReader_DuplicateAtomicityGroupIDsNotAllowed), p0);

    internal static string ODataBatchReader_RequestPropertyMissing(object p0) => TextRes.GetString(nameof (ODataBatchReader_RequestPropertyMissing), p0);

    internal static string ODataBatchReader_SameRequestIdAsAtomicityGroupIdNotAllowed(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataBatchReader_SameRequestIdAsAtomicityGroupIdNotAllowed), p0, p1);
    }

    internal static string ODataBatchReader_SelfReferenceDependsOnRequestIdNotAllowed(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataBatchReader_SelfReferenceDependsOnRequestIdNotAllowed), p0, p1);
    }

    internal static string ODataBatchReader_DependsOnRequestIdIsPartOfAtomicityGroupNotAllowed(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataBatchReader_DependsOnRequestIdIsPartOfAtomicityGroupNotAllowed), p0, p1);
    }

    internal static string ODataBatchReader_DependsOnIdNotFound(object p0, object p1) => TextRes.GetString(nameof (ODataBatchReader_DependsOnIdNotFound), p0, p1);

    internal static string ODataBatchReader_AbsoluteURINotMatchingBaseUri(object p0, object p1) => TextRes.GetString(nameof (ODataBatchReader_AbsoluteURINotMatchingBaseUri), p0, p1);

    internal static string ODataBatchReader_ReferenceIdNotIncludedInDependsOn(
      object p0,
      object p1,
      object p2)
    {
      return TextRes.GetString(nameof (ODataBatchReader_ReferenceIdNotIncludedInDependsOn), p0, p1, p2);
    }

    internal static string ODataBatch_GroupIdOrChangeSetIdCannotBeNull => TextRes.GetString(nameof (ODataBatch_GroupIdOrChangeSetIdCannotBeNull));

    internal static string ODataBatchReader_MessageIdPositionedIncorrectly(object p0, object p1) => TextRes.GetString(nameof (ODataBatchReader_MessageIdPositionedIncorrectly), p0, p1);

    internal static string ODataBatchReader_ReaderStreamChangesetBoundaryCannotBeNull => TextRes.GetString(nameof (ODataBatchReader_ReaderStreamChangesetBoundaryCannotBeNull));

    internal static string ODataBatchReaderStream_InvalidHeaderSpecified(object p0) => TextRes.GetString(nameof (ODataBatchReaderStream_InvalidHeaderSpecified), p0);

    internal static string ODataBatchReaderStream_InvalidRequestLine(object p0) => TextRes.GetString(nameof (ODataBatchReaderStream_InvalidRequestLine), p0);

    internal static string ODataBatchReaderStream_InvalidResponseLine(object p0) => TextRes.GetString(nameof (ODataBatchReaderStream_InvalidResponseLine), p0);

    internal static string ODataBatchReaderStream_InvalidHttpVersionSpecified(object p0, object p1) => TextRes.GetString(nameof (ODataBatchReaderStream_InvalidHttpVersionSpecified), p0, p1);

    internal static string ODataBatchReaderStream_NonIntegerHttpStatusCode(object p0) => TextRes.GetString(nameof (ODataBatchReaderStream_NonIntegerHttpStatusCode), p0);

    internal static string ODataBatchReaderStream_MissingContentTypeHeader => TextRes.GetString(nameof (ODataBatchReaderStream_MissingContentTypeHeader));

    internal static string ODataBatchReaderStream_MissingOrInvalidContentEncodingHeader(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataBatchReaderStream_MissingOrInvalidContentEncodingHeader), p0, p1);
    }

    internal static string ODataBatchReaderStream_InvalidContentTypeSpecified(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return TextRes.GetString(nameof (ODataBatchReaderStream_InvalidContentTypeSpecified), p0, p1, p2, p3);
    }

    internal static string ODataBatchReaderStream_InvalidContentLengthSpecified(object p0) => TextRes.GetString(nameof (ODataBatchReaderStream_InvalidContentLengthSpecified), p0);

    internal static string ODataBatchReaderStream_DuplicateHeaderFound(object p0) => TextRes.GetString(nameof (ODataBatchReaderStream_DuplicateHeaderFound), p0);

    internal static string ODataBatchReaderStream_NestedChangesetsAreNotSupported => TextRes.GetString(nameof (ODataBatchReaderStream_NestedChangesetsAreNotSupported));

    internal static string ODataBatchReaderStream_MultiByteEncodingsNotSupported(object p0) => TextRes.GetString(nameof (ODataBatchReaderStream_MultiByteEncodingsNotSupported), p0);

    internal static string ODataBatchReaderStream_UnexpectedEndOfInput => TextRes.GetString(nameof (ODataBatchReaderStream_UnexpectedEndOfInput));

    internal static string ODataBatchReaderStreamBuffer_BoundaryLineSecurityLimitReached(object p0) => TextRes.GetString(nameof (ODataBatchReaderStreamBuffer_BoundaryLineSecurityLimitReached), p0);

    internal static string ODataAsyncWriter_CannotCreateResponseWhenNotWritingResponse => TextRes.GetString(nameof (ODataAsyncWriter_CannotCreateResponseWhenNotWritingResponse));

    internal static string ODataAsyncWriter_CannotCreateResponseMoreThanOnce => TextRes.GetString(nameof (ODataAsyncWriter_CannotCreateResponseMoreThanOnce));

    internal static string ODataAsyncWriter_SyncCallOnAsyncWriter => TextRes.GetString(nameof (ODataAsyncWriter_SyncCallOnAsyncWriter));

    internal static string ODataAsyncWriter_AsyncCallOnSyncWriter => TextRes.GetString(nameof (ODataAsyncWriter_AsyncCallOnSyncWriter));

    internal static string ODataAsyncWriter_CannotWriteInStreamErrorForAsync => TextRes.GetString(nameof (ODataAsyncWriter_CannotWriteInStreamErrorForAsync));

    internal static string ODataAsyncReader_InvalidHeaderSpecified(object p0) => TextRes.GetString(nameof (ODataAsyncReader_InvalidHeaderSpecified), p0);

    internal static string ODataAsyncReader_CannotCreateResponseWhenNotReadingResponse => TextRes.GetString(nameof (ODataAsyncReader_CannotCreateResponseWhenNotReadingResponse));

    internal static string ODataAsyncReader_InvalidResponseLine(object p0) => TextRes.GetString(nameof (ODataAsyncReader_InvalidResponseLine), p0);

    internal static string ODataAsyncReader_InvalidHttpVersionSpecified(object p0, object p1) => TextRes.GetString(nameof (ODataAsyncReader_InvalidHttpVersionSpecified), p0, p1);

    internal static string ODataAsyncReader_NonIntegerHttpStatusCode(object p0) => TextRes.GetString(nameof (ODataAsyncReader_NonIntegerHttpStatusCode), p0);

    internal static string ODataAsyncReader_DuplicateHeaderFound(object p0) => TextRes.GetString(nameof (ODataAsyncReader_DuplicateHeaderFound), p0);

    internal static string ODataAsyncReader_MultiByteEncodingsNotSupported(object p0) => TextRes.GetString(nameof (ODataAsyncReader_MultiByteEncodingsNotSupported), p0);

    internal static string ODataAsyncReader_InvalidNewLineEncountered(object p0) => TextRes.GetString(nameof (ODataAsyncReader_InvalidNewLineEncountered), p0);

    internal static string ODataAsyncReader_UnexpectedEndOfInput => TextRes.GetString(nameof (ODataAsyncReader_UnexpectedEndOfInput));

    internal static string ODataAsyncReader_SyncCallOnAsyncReader => TextRes.GetString(nameof (ODataAsyncReader_SyncCallOnAsyncReader));

    internal static string ODataAsyncReader_AsyncCallOnSyncReader => TextRes.GetString(nameof (ODataAsyncReader_AsyncCallOnSyncReader));

    internal static string HttpUtils_MediaTypeUnspecified(object p0) => TextRes.GetString(nameof (HttpUtils_MediaTypeUnspecified), p0);

    internal static string HttpUtils_MediaTypeRequiresSlash(object p0) => TextRes.GetString(nameof (HttpUtils_MediaTypeRequiresSlash), p0);

    internal static string HttpUtils_MediaTypeRequiresSubType(object p0) => TextRes.GetString(nameof (HttpUtils_MediaTypeRequiresSubType), p0);

    internal static string HttpUtils_MediaTypeMissingParameterValue(object p0) => TextRes.GetString(nameof (HttpUtils_MediaTypeMissingParameterValue), p0);

    internal static string HttpUtils_MediaTypeMissingParameterName => TextRes.GetString(nameof (HttpUtils_MediaTypeMissingParameterName));

    internal static string HttpUtils_EscapeCharWithoutQuotes(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return TextRes.GetString(nameof (HttpUtils_EscapeCharWithoutQuotes), p0, p1, p2, p3);
    }

    internal static string HttpUtils_EscapeCharAtEnd(object p0, object p1, object p2, object p3) => TextRes.GetString(nameof (HttpUtils_EscapeCharAtEnd), p0, p1, p2, p3);

    internal static string HttpUtils_ClosingQuoteNotFound(object p0, object p1, object p2) => TextRes.GetString(nameof (HttpUtils_ClosingQuoteNotFound), p0, p1, p2);

    internal static string HttpUtils_InvalidCharacterInQuotedParameterValue(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return TextRes.GetString(nameof (HttpUtils_InvalidCharacterInQuotedParameterValue), p0, p1, p2, p3);
    }

    internal static string HttpUtils_ContentTypeMissing => TextRes.GetString(nameof (HttpUtils_ContentTypeMissing));

    internal static string HttpUtils_MediaTypeRequiresSemicolonBeforeParameter(object p0) => TextRes.GetString(nameof (HttpUtils_MediaTypeRequiresSemicolonBeforeParameter), p0);

    internal static string HttpUtils_InvalidQualityValueStartChar(object p0, object p1) => TextRes.GetString(nameof (HttpUtils_InvalidQualityValueStartChar), p0, p1);

    internal static string HttpUtils_InvalidQualityValue(object p0, object p1) => TextRes.GetString(nameof (HttpUtils_InvalidQualityValue), p0, p1);

    internal static string HttpUtils_CannotConvertCharToInt(object p0) => TextRes.GetString(nameof (HttpUtils_CannotConvertCharToInt), p0);

    internal static string HttpUtils_MissingSeparatorBetweenCharsets(object p0) => TextRes.GetString(nameof (HttpUtils_MissingSeparatorBetweenCharsets), p0);

    internal static string HttpUtils_InvalidSeparatorBetweenCharsets(object p0) => TextRes.GetString(nameof (HttpUtils_InvalidSeparatorBetweenCharsets), p0);

    internal static string HttpUtils_InvalidCharsetName(object p0) => TextRes.GetString(nameof (HttpUtils_InvalidCharsetName), p0);

    internal static string HttpUtils_UnexpectedEndOfQValue(object p0) => TextRes.GetString(nameof (HttpUtils_UnexpectedEndOfQValue), p0);

    internal static string HttpUtils_ExpectedLiteralNotFoundInString(
      object p0,
      object p1,
      object p2)
    {
      return TextRes.GetString(nameof (HttpUtils_ExpectedLiteralNotFoundInString), p0, p1, p2);
    }

    internal static string HttpUtils_InvalidHttpMethodString(object p0) => TextRes.GetString(nameof (HttpUtils_InvalidHttpMethodString), p0);

    internal static string HttpUtils_NoOrMoreThanOneContentTypeSpecified(object p0) => TextRes.GetString(nameof (HttpUtils_NoOrMoreThanOneContentTypeSpecified), p0);

    internal static string HttpHeaderValueLexer_UnrecognizedSeparator(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return TextRes.GetString(nameof (HttpHeaderValueLexer_UnrecognizedSeparator), p0, p1, p2, p3);
    }

    internal static string HttpHeaderValueLexer_TokenExpectedButFoundQuotedString(
      object p0,
      object p1,
      object p2)
    {
      return TextRes.GetString(nameof (HttpHeaderValueLexer_TokenExpectedButFoundQuotedString), p0, p1, p2);
    }

    internal static string HttpHeaderValueLexer_FailedToReadTokenOrQuotedString(
      object p0,
      object p1,
      object p2)
    {
      return TextRes.GetString(nameof (HttpHeaderValueLexer_FailedToReadTokenOrQuotedString), p0, p1, p2);
    }

    internal static string HttpHeaderValueLexer_InvalidSeparatorAfterQuotedString(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return TextRes.GetString(nameof (HttpHeaderValueLexer_InvalidSeparatorAfterQuotedString), p0, p1, p2, p3);
    }

    internal static string HttpHeaderValueLexer_EndOfFileAfterSeparator(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return TextRes.GetString(nameof (HttpHeaderValueLexer_EndOfFileAfterSeparator), p0, p1, p2, p3);
    }

    internal static string MediaType_EncodingNotSupported(object p0) => TextRes.GetString(nameof (MediaType_EncodingNotSupported), p0);

    internal static string MediaTypeUtils_DidNotFindMatchingMediaType(object p0, object p1) => TextRes.GetString(nameof (MediaTypeUtils_DidNotFindMatchingMediaType), p0, p1);

    internal static string MediaTypeUtils_CannotDetermineFormatFromContentType(object p0, object p1) => TextRes.GetString(nameof (MediaTypeUtils_CannotDetermineFormatFromContentType), p0, p1);

    internal static string MediaTypeUtils_NoOrMoreThanOneContentTypeSpecified(object p0) => TextRes.GetString(nameof (MediaTypeUtils_NoOrMoreThanOneContentTypeSpecified), p0);

    internal static string MediaTypeUtils_BoundaryMustBeSpecifiedForBatchPayloads(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (MediaTypeUtils_BoundaryMustBeSpecifiedForBatchPayloads), p0, p1);
    }

    internal static string ExpressionLexer_ExpectedLiteralToken(object p0) => TextRes.GetString(nameof (ExpressionLexer_ExpectedLiteralToken), p0);

    internal static string ODataUriUtils_ConvertToUriLiteralUnsupportedType(object p0) => TextRes.GetString(nameof (ODataUriUtils_ConvertToUriLiteralUnsupportedType), p0);

    internal static string ODataUriUtils_ConvertFromUriLiteralTypeRefWithoutModel => TextRes.GetString(nameof (ODataUriUtils_ConvertFromUriLiteralTypeRefWithoutModel));

    internal static string ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure), p0, p1);
    }

    internal static string ODataUriUtils_ConvertFromUriLiteralNullTypeVerificationFailure(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataUriUtils_ConvertFromUriLiteralNullTypeVerificationFailure), p0, p1);
    }

    internal static string ODataUriUtils_ConvertFromUriLiteralNullOnNonNullableType(object p0) => TextRes.GetString(nameof (ODataUriUtils_ConvertFromUriLiteralNullOnNonNullableType), p0);

    internal static string ODataUtils_CannotConvertValueToRawString(object p0) => TextRes.GetString(nameof (ODataUtils_CannotConvertValueToRawString), p0);

    internal static string ODataUtils_DidNotFindDefaultMediaType(object p0) => TextRes.GetString(nameof (ODataUtils_DidNotFindDefaultMediaType), p0);

    internal static string ODataUtils_UnsupportedVersionHeader(object p0) => TextRes.GetString(nameof (ODataUtils_UnsupportedVersionHeader), p0);

    internal static string ODataUtils_MaxProtocolVersionExceeded(object p0, object p1) => TextRes.GetString(nameof (ODataUtils_MaxProtocolVersionExceeded), p0, p1);

    internal static string ODataUtils_UnsupportedVersionNumber => TextRes.GetString(nameof (ODataUtils_UnsupportedVersionNumber));

    internal static string ODataUtils_ModelDoesNotHaveContainer => TextRes.GetString(nameof (ODataUtils_ModelDoesNotHaveContainer));

    internal static string ReaderUtils_EnumerableModified(object p0) => TextRes.GetString(nameof (ReaderUtils_EnumerableModified), p0);

    internal static string ReaderValidationUtils_NullValueForNonNullableType(object p0) => TextRes.GetString(nameof (ReaderValidationUtils_NullValueForNonNullableType), p0);

    internal static string ReaderValidationUtils_NullNamedValueForNonNullableType(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ReaderValidationUtils_NullNamedValueForNonNullableType), p0, p1);
    }

    internal static string ReaderValidationUtils_EntityReferenceLinkMissingUri => TextRes.GetString(nameof (ReaderValidationUtils_EntityReferenceLinkMissingUri));

    internal static string ReaderValidationUtils_ValueWithoutType => TextRes.GetString(nameof (ReaderValidationUtils_ValueWithoutType));

    internal static string ReaderValidationUtils_ResourceWithoutType => TextRes.GetString(nameof (ReaderValidationUtils_ResourceWithoutType));

    internal static string ReaderValidationUtils_CannotConvertPrimitiveValue(object p0, object p1) => TextRes.GetString(nameof (ReaderValidationUtils_CannotConvertPrimitiveValue), p0, p1);

    internal static string ReaderValidationUtils_MessageReaderSettingsBaseUriMustBeNullOrAbsolute(
      object p0)
    {
      return TextRes.GetString(nameof (ReaderValidationUtils_MessageReaderSettingsBaseUriMustBeNullOrAbsolute), p0);
    }

    internal static string ReaderValidationUtils_UndeclaredPropertyBehaviorKindSpecifiedOnRequest => TextRes.GetString(nameof (ReaderValidationUtils_UndeclaredPropertyBehaviorKindSpecifiedOnRequest));

    internal static string ReaderValidationUtils_ContextUriValidationInvalidExpectedEntitySet(
      object p0,
      object p1,
      object p2)
    {
      return TextRes.GetString(nameof (ReaderValidationUtils_ContextUriValidationInvalidExpectedEntitySet), p0, p1, p2);
    }

    internal static string ReaderValidationUtils_ContextUriValidationInvalidExpectedEntityType(
      object p0,
      object p1,
      object p2)
    {
      return TextRes.GetString(nameof (ReaderValidationUtils_ContextUriValidationInvalidExpectedEntityType), p0, p1, p2);
    }

    internal static string ReaderValidationUtils_ContextUriValidationNonMatchingPropertyNames(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return TextRes.GetString(nameof (ReaderValidationUtils_ContextUriValidationNonMatchingPropertyNames), p0, p1, p2, p3);
    }

    internal static string ReaderValidationUtils_ContextUriValidationNonMatchingDeclaringTypes(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return TextRes.GetString(nameof (ReaderValidationUtils_ContextUriValidationNonMatchingDeclaringTypes), p0, p1, p2, p3);
    }

    internal static string ReaderValidationUtils_NonMatchingPropertyNames(object p0, object p1) => TextRes.GetString(nameof (ReaderValidationUtils_NonMatchingPropertyNames), p0, p1);

    internal static string ReaderValidationUtils_TypeInContextUriDoesNotMatchExpectedType(
      object p0,
      object p1,
      object p2)
    {
      return TextRes.GetString(nameof (ReaderValidationUtils_TypeInContextUriDoesNotMatchExpectedType), p0, p1, p2);
    }

    internal static string ReaderValidationUtils_ContextUriDoesNotReferTypeAssignableToExpectedType(
      object p0,
      object p1,
      object p2)
    {
      return TextRes.GetString(nameof (ReaderValidationUtils_ContextUriDoesNotReferTypeAssignableToExpectedType), p0, p1, p2);
    }

    internal static string ReaderValidationUtils_ValueTypeNotAllowedInDerivedTypeConstraint(
      object p0,
      object p1,
      object p2)
    {
      return TextRes.GetString(nameof (ReaderValidationUtils_ValueTypeNotAllowedInDerivedTypeConstraint), p0, p1, p2);
    }

    internal static string ODataMessageReader_ReaderAlreadyUsed => TextRes.GetString(nameof (ODataMessageReader_ReaderAlreadyUsed));

    internal static string ODataMessageReader_ErrorPayloadInRequest => TextRes.GetString(nameof (ODataMessageReader_ErrorPayloadInRequest));

    internal static string ODataMessageReader_ServiceDocumentInRequest => TextRes.GetString(nameof (ODataMessageReader_ServiceDocumentInRequest));

    internal static string ODataMessageReader_MetadataDocumentInRequest => TextRes.GetString(nameof (ODataMessageReader_MetadataDocumentInRequest));

    internal static string ODataMessageReader_DeltaInRequest => TextRes.GetString(nameof (ODataMessageReader_DeltaInRequest));

    internal static string ODataMessageReader_ExpectedTypeSpecifiedWithoutMetadata(object p0) => TextRes.GetString(nameof (ODataMessageReader_ExpectedTypeSpecifiedWithoutMetadata), p0);

    internal static string ODataMessageReader_EntitySetSpecifiedWithoutMetadata(object p0) => TextRes.GetString(nameof (ODataMessageReader_EntitySetSpecifiedWithoutMetadata), p0);

    internal static string ODataMessageReader_OperationImportSpecifiedWithoutMetadata(object p0) => TextRes.GetString(nameof (ODataMessageReader_OperationImportSpecifiedWithoutMetadata), p0);

    internal static string ODataMessageReader_OperationSpecifiedWithoutMetadata(object p0) => TextRes.GetString(nameof (ODataMessageReader_OperationSpecifiedWithoutMetadata), p0);

    internal static string ODataMessageReader_ExpectedCollectionTypeWrongKind(object p0) => TextRes.GetString(nameof (ODataMessageReader_ExpectedCollectionTypeWrongKind), p0);

    internal static string ODataMessageReader_ExpectedPropertyTypeEntityCollectionKind => TextRes.GetString(nameof (ODataMessageReader_ExpectedPropertyTypeEntityCollectionKind));

    internal static string ODataMessageReader_ExpectedPropertyTypeEntityKind => TextRes.GetString(nameof (ODataMessageReader_ExpectedPropertyTypeEntityKind));

    internal static string ODataMessageReader_ExpectedPropertyTypeStream => TextRes.GetString(nameof (ODataMessageReader_ExpectedPropertyTypeStream));

    internal static string ODataMessageReader_ExpectedValueTypeWrongKind(object p0) => TextRes.GetString(nameof (ODataMessageReader_ExpectedValueTypeWrongKind), p0);

    internal static string ODataMessageReader_NoneOrEmptyContentTypeHeader => TextRes.GetString(nameof (ODataMessageReader_NoneOrEmptyContentTypeHeader));

    internal static string ODataMessageReader_WildcardInContentType(object p0) => TextRes.GetString(nameof (ODataMessageReader_WildcardInContentType), p0);

    internal static string ODataMessageReader_GetFormatCalledBeforeReadingStarted => TextRes.GetString(nameof (ODataMessageReader_GetFormatCalledBeforeReadingStarted));

    internal static string ODataMessageReader_DetectPayloadKindMultipleTimes => TextRes.GetString(nameof (ODataMessageReader_DetectPayloadKindMultipleTimes));

    internal static string ODataMessageReader_PayloadKindDetectionRunning => TextRes.GetString(nameof (ODataMessageReader_PayloadKindDetectionRunning));

    internal static string ODataMessageReader_PayloadKindDetectionInServerMode => TextRes.GetString(nameof (ODataMessageReader_PayloadKindDetectionInServerMode));

    internal static string ODataMessageReader_ParameterPayloadInResponse => TextRes.GetString(nameof (ODataMessageReader_ParameterPayloadInResponse));

    internal static string ODataMessageReader_SingletonNavigationPropertyForEntityReferenceLinks(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataMessageReader_SingletonNavigationPropertyForEntityReferenceLinks), p0, p1);
    }

    internal static string ODataAsyncResponseMessage_MustNotModifyMessage => TextRes.GetString(nameof (ODataAsyncResponseMessage_MustNotModifyMessage));

    internal static string ODataMessage_MustNotModifyMessage => TextRes.GetString(nameof (ODataMessage_MustNotModifyMessage));

    internal static string ODataReaderCore_SyncCallOnAsyncReader => TextRes.GetString(nameof (ODataReaderCore_SyncCallOnAsyncReader));

    internal static string ODataReaderCore_AsyncCallOnSyncReader => TextRes.GetString(nameof (ODataReaderCore_AsyncCallOnSyncReader));

    internal static string ODataReaderCore_ReadOrReadAsyncCalledInInvalidState(object p0) => TextRes.GetString(nameof (ODataReaderCore_ReadOrReadAsyncCalledInInvalidState), p0);

    internal static string ODataReaderCore_CreateReadStreamCalledInInvalidState => TextRes.GetString(nameof (ODataReaderCore_CreateReadStreamCalledInInvalidState));

    internal static string ODataReaderCore_CreateTextReaderCalledInInvalidState => TextRes.GetString(nameof (ODataReaderCore_CreateTextReaderCalledInInvalidState));

    internal static string ODataReaderCore_ReadCalledWithOpenStream => TextRes.GetString(nameof (ODataReaderCore_ReadCalledWithOpenStream));

    internal static string ODataReaderCore_NoReadCallsAllowed(object p0) => TextRes.GetString(nameof (ODataReaderCore_NoReadCallsAllowed), p0);

    internal static string ODataWriterCore_PropertyValueAlreadyWritten(object p0) => TextRes.GetString(nameof (ODataWriterCore_PropertyValueAlreadyWritten), p0);

    internal static string ODataJsonReader_CannotReadResourcesOfResourceSet(object p0) => TextRes.GetString(nameof (ODataJsonReader_CannotReadResourcesOfResourceSet), p0);

    internal static string ODataJsonReaderUtils_CannotConvertInt32(object p0) => TextRes.GetString(nameof (ODataJsonReaderUtils_CannotConvertInt32), p0);

    internal static string ODataJsonReaderUtils_CannotConvertDouble(object p0) => TextRes.GetString(nameof (ODataJsonReaderUtils_CannotConvertDouble), p0);

    internal static string ODataJsonReaderUtils_CannotConvertBoolean(object p0) => TextRes.GetString(nameof (ODataJsonReaderUtils_CannotConvertBoolean), p0);

    internal static string ODataJsonReaderUtils_CannotConvertDecimal(object p0) => TextRes.GetString(nameof (ODataJsonReaderUtils_CannotConvertDecimal), p0);

    internal static string ODataJsonReaderUtils_CannotConvertDateTime(object p0) => TextRes.GetString(nameof (ODataJsonReaderUtils_CannotConvertDateTime), p0);

    internal static string ODataJsonReaderUtils_CannotConvertDateTimeOffset(object p0) => TextRes.GetString(nameof (ODataJsonReaderUtils_CannotConvertDateTimeOffset), p0);

    internal static string ODataJsonReaderUtils_ConflictBetweenInputFormatAndParameter(object p0) => TextRes.GetString(nameof (ODataJsonReaderUtils_ConflictBetweenInputFormatAndParameter), p0);

    internal static string ODataJsonReaderUtils_MultipleErrorPropertiesWithSameName(object p0) => TextRes.GetString(nameof (ODataJsonReaderUtils_MultipleErrorPropertiesWithSameName), p0);

    internal static string ODataJsonLightResourceSerializer_ActionsAndFunctionsGroupMustSpecifyTarget(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceSerializer_ActionsAndFunctionsGroupMustSpecifyTarget), p0);
    }

    internal static string ODataJsonLightResourceSerializer_ActionsAndFunctionsGroupMustNotHaveDuplicateTarget(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceSerializer_ActionsAndFunctionsGroupMustNotHaveDuplicateTarget), p0, p1);
    }

    internal static string ODataJsonErrorDeserializer_TopLevelErrorWithInvalidProperty(object p0) => TextRes.GetString(nameof (ODataJsonErrorDeserializer_TopLevelErrorWithInvalidProperty), p0);

    internal static string ODataJsonErrorDeserializer_TopLevelErrorMessageValueWithInvalidProperty(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonErrorDeserializer_TopLevelErrorMessageValueWithInvalidProperty), p0);
    }

    internal static string ODataCollectionReaderCore_ReadOrReadAsyncCalledInInvalidState(object p0) => TextRes.GetString(nameof (ODataCollectionReaderCore_ReadOrReadAsyncCalledInInvalidState), p0);

    internal static string ODataCollectionReaderCore_SyncCallOnAsyncReader => TextRes.GetString(nameof (ODataCollectionReaderCore_SyncCallOnAsyncReader));

    internal static string ODataCollectionReaderCore_AsyncCallOnSyncReader => TextRes.GetString(nameof (ODataCollectionReaderCore_AsyncCallOnSyncReader));

    internal static string ODataCollectionReaderCore_ExpectedItemTypeSetInInvalidState(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataCollectionReaderCore_ExpectedItemTypeSetInInvalidState), p0, p1);
    }

    internal static string ODataParameterReaderCore_ReadOrReadAsyncCalledInInvalidState(object p0) => TextRes.GetString(nameof (ODataParameterReaderCore_ReadOrReadAsyncCalledInInvalidState), p0);

    internal static string ODataParameterReaderCore_SyncCallOnAsyncReader => TextRes.GetString(nameof (ODataParameterReaderCore_SyncCallOnAsyncReader));

    internal static string ODataParameterReaderCore_AsyncCallOnSyncReader => TextRes.GetString(nameof (ODataParameterReaderCore_AsyncCallOnSyncReader));

    internal static string ODataParameterReaderCore_SubReaderMustBeCreatedAndReadToCompletionBeforeTheNextReadOrReadAsyncCall(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataParameterReaderCore_SubReaderMustBeCreatedAndReadToCompletionBeforeTheNextReadOrReadAsyncCall), p0, p1);
    }

    internal static string ODataParameterReaderCore_SubReaderMustBeInCompletedStateBeforeTheNextReadOrReadAsyncCall(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataParameterReaderCore_SubReaderMustBeInCompletedStateBeforeTheNextReadOrReadAsyncCall), p0, p1);
    }

    internal static string ODataParameterReaderCore_InvalidCreateReaderMethodCalledForState(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataParameterReaderCore_InvalidCreateReaderMethodCalledForState), p0, p1);
    }

    internal static string ODataParameterReaderCore_CreateReaderAlreadyCalled(object p0, object p1) => TextRes.GetString(nameof (ODataParameterReaderCore_CreateReaderAlreadyCalled), p0, p1);

    internal static string ODataParameterReaderCore_ParameterNameNotInMetadata(object p0, object p1) => TextRes.GetString(nameof (ODataParameterReaderCore_ParameterNameNotInMetadata), p0, p1);

    internal static string ODataParameterReaderCore_DuplicateParametersInPayload(object p0) => TextRes.GetString(nameof (ODataParameterReaderCore_DuplicateParametersInPayload), p0);

    internal static string ODataParameterReaderCore_ParametersMissingInPayload(object p0, object p1) => TextRes.GetString(nameof (ODataParameterReaderCore_ParametersMissingInPayload), p0, p1);

    internal static string ValidationUtils_ActionsAndFunctionsMustSpecifyMetadata(object p0) => TextRes.GetString(nameof (ValidationUtils_ActionsAndFunctionsMustSpecifyMetadata), p0);

    internal static string ValidationUtils_ActionsAndFunctionsMustSpecifyTarget(object p0) => TextRes.GetString(nameof (ValidationUtils_ActionsAndFunctionsMustSpecifyTarget), p0);

    internal static string ValidationUtils_EnumerableContainsANullItem(object p0) => TextRes.GetString(nameof (ValidationUtils_EnumerableContainsANullItem), p0);

    internal static string ValidationUtils_AssociationLinkMustSpecifyName => TextRes.GetString(nameof (ValidationUtils_AssociationLinkMustSpecifyName));

    internal static string ValidationUtils_AssociationLinkMustSpecifyUrl => TextRes.GetString(nameof (ValidationUtils_AssociationLinkMustSpecifyUrl));

    internal static string ValidationUtils_TypeNameMustNotBeEmpty => TextRes.GetString(nameof (ValidationUtils_TypeNameMustNotBeEmpty));

    internal static string ValidationUtils_PropertyDoesNotExistOnType(object p0, object p1) => TextRes.GetString(nameof (ValidationUtils_PropertyDoesNotExistOnType), p0, p1);

    internal static string ValidationUtils_ResourceMustSpecifyUrl => TextRes.GetString(nameof (ValidationUtils_ResourceMustSpecifyUrl));

    internal static string ValidationUtils_ResourceMustSpecifyName(object p0) => TextRes.GetString(nameof (ValidationUtils_ResourceMustSpecifyName), p0);

    internal static string ValidationUtils_ServiceDocumentElementUrlMustNotBeNull => TextRes.GetString(nameof (ValidationUtils_ServiceDocumentElementUrlMustNotBeNull));

    internal static string ValidationUtils_NonPrimitiveTypeForPrimitiveValue(object p0) => TextRes.GetString(nameof (ValidationUtils_NonPrimitiveTypeForPrimitiveValue), p0);

    internal static string ValidationUtils_UnsupportedPrimitiveType(object p0) => TextRes.GetString(nameof (ValidationUtils_UnsupportedPrimitiveType), p0);

    internal static string ValidationUtils_IncompatiblePrimitiveItemType(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return TextRes.GetString(nameof (ValidationUtils_IncompatiblePrimitiveItemType), p0, p1, p2, p3);
    }

    internal static string ValidationUtils_NonNullableCollectionElementsMustNotBeNull => TextRes.GetString(nameof (ValidationUtils_NonNullableCollectionElementsMustNotBeNull));

    internal static string ValidationUtils_InvalidCollectionTypeName(object p0) => TextRes.GetString(nameof (ValidationUtils_InvalidCollectionTypeName), p0);

    internal static string ValidationUtils_UnrecognizedTypeName(object p0) => TextRes.GetString(nameof (ValidationUtils_UnrecognizedTypeName), p0);

    internal static string ValidationUtils_IncorrectTypeKind(object p0, object p1, object p2) => TextRes.GetString(nameof (ValidationUtils_IncorrectTypeKind), p0, p1, p2);

    internal static string ValidationUtils_IncorrectTypeKindNoTypeName(object p0, object p1) => TextRes.GetString(nameof (ValidationUtils_IncorrectTypeKindNoTypeName), p0, p1);

    internal static string ValidationUtils_IncorrectValueTypeKind(object p0, object p1) => TextRes.GetString(nameof (ValidationUtils_IncorrectValueTypeKind), p0, p1);

    internal static string ValidationUtils_LinkMustSpecifyName => TextRes.GetString(nameof (ValidationUtils_LinkMustSpecifyName));

    internal static string ValidationUtils_MismatchPropertyKindForStreamProperty(object p0) => TextRes.GetString(nameof (ValidationUtils_MismatchPropertyKindForStreamProperty), p0);

    internal static string ValidationUtils_NestedCollectionsAreNotSupported => TextRes.GetString(nameof (ValidationUtils_NestedCollectionsAreNotSupported));

    internal static string ValidationUtils_StreamReferenceValuesNotSupportedInCollections => TextRes.GetString(nameof (ValidationUtils_StreamReferenceValuesNotSupportedInCollections));

    internal static string ValidationUtils_IncompatibleType(object p0, object p1) => TextRes.GetString(nameof (ValidationUtils_IncompatibleType), p0, p1);

    internal static string ValidationUtils_OpenCollectionProperty(object p0) => TextRes.GetString(nameof (ValidationUtils_OpenCollectionProperty), p0);

    internal static string ValidationUtils_OpenStreamProperty(object p0) => TextRes.GetString(nameof (ValidationUtils_OpenStreamProperty), p0);

    internal static string ValidationUtils_InvalidCollectionTypeReference(object p0) => TextRes.GetString(nameof (ValidationUtils_InvalidCollectionTypeReference), p0);

    internal static string ValidationUtils_ResourceWithMediaResourceAndNonMLEType(object p0) => TextRes.GetString(nameof (ValidationUtils_ResourceWithMediaResourceAndNonMLEType), p0);

    internal static string ValidationUtils_ResourceWithoutMediaResourceAndMLEType(object p0) => TextRes.GetString(nameof (ValidationUtils_ResourceWithoutMediaResourceAndMLEType), p0);

    internal static string ValidationUtils_ResourceTypeNotAssignableToExpectedType(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ValidationUtils_ResourceTypeNotAssignableToExpectedType), p0, p1);
    }

    internal static string ValidationUtils_NavigationPropertyExpected(
      object p0,
      object p1,
      object p2)
    {
      return TextRes.GetString(nameof (ValidationUtils_NavigationPropertyExpected), p0, p1, p2);
    }

    internal static string ValidationUtils_InvalidBatchBoundaryDelimiterLength(object p0, object p1) => TextRes.GetString(nameof (ValidationUtils_InvalidBatchBoundaryDelimiterLength), p0, p1);

    internal static string ValidationUtils_RecursionDepthLimitReached(object p0) => TextRes.GetString(nameof (ValidationUtils_RecursionDepthLimitReached), p0);

    internal static string ValidationUtils_MaxDepthOfNestedEntriesExceeded(object p0) => TextRes.GetString(nameof (ValidationUtils_MaxDepthOfNestedEntriesExceeded), p0);

    internal static string ValidationUtils_NullCollectionItemForNonNullableType(object p0) => TextRes.GetString(nameof (ValidationUtils_NullCollectionItemForNonNullableType), p0);

    internal static string ValidationUtils_PropertiesMustNotContainReservedChars(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ValidationUtils_PropertiesMustNotContainReservedChars), p0, p1);
    }

    internal static string ValidationUtils_WorkspaceResourceMustNotContainNullItem => TextRes.GetString(nameof (ValidationUtils_WorkspaceResourceMustNotContainNullItem));

    internal static string ValidationUtils_InvalidMetadataReferenceProperty(object p0) => TextRes.GetString(nameof (ValidationUtils_InvalidMetadataReferenceProperty), p0);

    internal static string WriterValidationUtils_PropertyMustNotBeNull => TextRes.GetString(nameof (WriterValidationUtils_PropertyMustNotBeNull));

    internal static string WriterValidationUtils_PropertiesMustHaveNonEmptyName => TextRes.GetString(nameof (WriterValidationUtils_PropertiesMustHaveNonEmptyName));

    internal static string WriterValidationUtils_MissingTypeNameWithMetadata => TextRes.GetString(nameof (WriterValidationUtils_MissingTypeNameWithMetadata));

    internal static string WriterValidationUtils_NextPageLinkInRequest => TextRes.GetString(nameof (WriterValidationUtils_NextPageLinkInRequest));

    internal static string WriterValidationUtils_DefaultStreamWithContentTypeWithoutReadLink => TextRes.GetString(nameof (WriterValidationUtils_DefaultStreamWithContentTypeWithoutReadLink));

    internal static string WriterValidationUtils_DefaultStreamWithReadLinkWithoutContentType => TextRes.GetString(nameof (WriterValidationUtils_DefaultStreamWithReadLinkWithoutContentType));

    internal static string WriterValidationUtils_StreamReferenceValueMustHaveEditLinkOrReadLink => TextRes.GetString(nameof (WriterValidationUtils_StreamReferenceValueMustHaveEditLinkOrReadLink));

    internal static string WriterValidationUtils_StreamReferenceValueMustHaveEditLinkToHaveETag => TextRes.GetString(nameof (WriterValidationUtils_StreamReferenceValueMustHaveEditLinkToHaveETag));

    internal static string WriterValidationUtils_StreamReferenceValueEmptyContentType => TextRes.GetString(nameof (WriterValidationUtils_StreamReferenceValueEmptyContentType));

    internal static string WriterValidationUtils_EntriesMustHaveNonEmptyId => TextRes.GetString(nameof (WriterValidationUtils_EntriesMustHaveNonEmptyId));

    internal static string WriterValidationUtils_MessageWriterSettingsBaseUriMustBeNullOrAbsolute(
      object p0)
    {
      return TextRes.GetString(nameof (WriterValidationUtils_MessageWriterSettingsBaseUriMustBeNullOrAbsolute), p0);
    }

    internal static string WriterValidationUtils_EntityReferenceLinkUrlMustNotBeNull => TextRes.GetString(nameof (WriterValidationUtils_EntityReferenceLinkUrlMustNotBeNull));

    internal static string WriterValidationUtils_EntityReferenceLinksLinkMustNotBeNull => TextRes.GetString(nameof (WriterValidationUtils_EntityReferenceLinksLinkMustNotBeNull));

    internal static string WriterValidationUtils_NestedResourceTypeNotCompatibleWithParentPropertyType(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (WriterValidationUtils_NestedResourceTypeNotCompatibleWithParentPropertyType), p0, p1);
    }

    internal static string WriterValidationUtils_ExpandedLinkIsCollectionTrueWithResourceContent(
      object p0)
    {
      return TextRes.GetString(nameof (WriterValidationUtils_ExpandedLinkIsCollectionTrueWithResourceContent), p0);
    }

    internal static string WriterValidationUtils_ExpandedLinkIsCollectionFalseWithResourceSetContent(
      object p0)
    {
      return TextRes.GetString(nameof (WriterValidationUtils_ExpandedLinkIsCollectionFalseWithResourceSetContent), p0);
    }

    internal static string WriterValidationUtils_ExpandedLinkIsCollectionTrueWithResourceMetadata(
      object p0)
    {
      return TextRes.GetString(nameof (WriterValidationUtils_ExpandedLinkIsCollectionTrueWithResourceMetadata), p0);
    }

    internal static string WriterValidationUtils_ExpandedLinkIsCollectionFalseWithResourceSetMetadata(
      object p0)
    {
      return TextRes.GetString(nameof (WriterValidationUtils_ExpandedLinkIsCollectionFalseWithResourceSetMetadata), p0);
    }

    internal static string WriterValidationUtils_ExpandedLinkWithResourceSetPayloadAndResourceMetadata(
      object p0)
    {
      return TextRes.GetString(nameof (WriterValidationUtils_ExpandedLinkWithResourceSetPayloadAndResourceMetadata), p0);
    }

    internal static string WriterValidationUtils_ExpandedLinkWithResourcePayloadAndResourceSetMetadata(
      object p0)
    {
      return TextRes.GetString(nameof (WriterValidationUtils_ExpandedLinkWithResourcePayloadAndResourceSetMetadata), p0);
    }

    internal static string WriterValidationUtils_CollectionPropertiesMustNotHaveNullValue(object p0) => TextRes.GetString(nameof (WriterValidationUtils_CollectionPropertiesMustNotHaveNullValue), p0);

    internal static string WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue), p0, p1);
    }

    internal static string WriterValidationUtils_StreamPropertiesMustNotHaveNullValue(object p0) => TextRes.GetString(nameof (WriterValidationUtils_StreamPropertiesMustNotHaveNullValue), p0);

    internal static string WriterValidationUtils_OperationInRequest(object p0) => TextRes.GetString(nameof (WriterValidationUtils_OperationInRequest), p0);

    internal static string WriterValidationUtils_AssociationLinkInRequest(object p0) => TextRes.GetString(nameof (WriterValidationUtils_AssociationLinkInRequest), p0);

    internal static string WriterValidationUtils_StreamPropertyInRequest(object p0) => TextRes.GetString(nameof (WriterValidationUtils_StreamPropertyInRequest), p0);

    internal static string WriterValidationUtils_MessageWriterSettingsServiceDocumentUriMustBeNullOrAbsolute(
      object p0)
    {
      return TextRes.GetString(nameof (WriterValidationUtils_MessageWriterSettingsServiceDocumentUriMustBeNullOrAbsolute), p0);
    }

    internal static string WriterValidationUtils_NavigationLinkMustSpecifyUrl(object p0) => TextRes.GetString(nameof (WriterValidationUtils_NavigationLinkMustSpecifyUrl), p0);

    internal static string WriterValidationUtils_NestedResourceInfoMustSpecifyIsCollection(object p0) => TextRes.GetString(nameof (WriterValidationUtils_NestedResourceInfoMustSpecifyIsCollection), p0);

    internal static string WriterValidationUtils_MessageWriterSettingsJsonPaddingOnRequestMessage => TextRes.GetString(nameof (WriterValidationUtils_MessageWriterSettingsJsonPaddingOnRequestMessage));

    internal static string WriterValidationUtils_ValueTypeNotAllowedInDerivedTypeConstraint(
      object p0,
      object p1,
      object p2)
    {
      return TextRes.GetString(nameof (WriterValidationUtils_ValueTypeNotAllowedInDerivedTypeConstraint), p0, p1, p2);
    }

    internal static string XmlReaderExtension_InvalidNodeInStringValue(object p0) => TextRes.GetString(nameof (XmlReaderExtension_InvalidNodeInStringValue), p0);

    internal static string XmlReaderExtension_InvalidRootNode(object p0) => TextRes.GetString(nameof (XmlReaderExtension_InvalidRootNode), p0);

    internal static string ODataMetadataInputContext_ErrorReadingMetadata(object p0) => TextRes.GetString(nameof (ODataMetadataInputContext_ErrorReadingMetadata), p0);

    internal static string ODataMetadataOutputContext_ErrorWritingMetadata(object p0) => TextRes.GetString(nameof (ODataMetadataOutputContext_ErrorWritingMetadata), p0);

    internal static string ODataAtomDeserializer_RelativeUriUsedWithoutBaseUriSpecified(object p0) => TextRes.GetString(nameof (ODataAtomDeserializer_RelativeUriUsedWithoutBaseUriSpecified), p0);

    internal static string ODataAtomPropertyAndValueDeserializer_InvalidCollectionElement(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataAtomPropertyAndValueDeserializer_InvalidCollectionElement), p0, p1);
    }

    internal static string ODataAtomPropertyAndValueDeserializer_NavigationPropertyInProperties(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataAtomPropertyAndValueDeserializer_NavigationPropertyInProperties), p0, p1);
    }

    internal static string JsonLightInstanceAnnotationWriter_NullValueNotAllowedForInstanceAnnotation(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (JsonLightInstanceAnnotationWriter_NullValueNotAllowedForInstanceAnnotation), p0, p1);
    }

    internal static string EdmLibraryExtensions_OperationGroupReturningActionsAndFunctionsModelInvalid(
      object p0)
    {
      return TextRes.GetString(nameof (EdmLibraryExtensions_OperationGroupReturningActionsAndFunctionsModelInvalid), p0);
    }

    internal static string EdmLibraryExtensions_UnBoundOperationsFoundFromIEdmModelFindMethodIsInvalid(
      object p0)
    {
      return TextRes.GetString(nameof (EdmLibraryExtensions_UnBoundOperationsFoundFromIEdmModelFindMethodIsInvalid), p0);
    }

    internal static string EdmLibraryExtensions_NoParameterBoundOperationsFoundFromIEdmModelFindMethodIsInvalid(
      object p0)
    {
      return TextRes.GetString(nameof (EdmLibraryExtensions_NoParameterBoundOperationsFoundFromIEdmModelFindMethodIsInvalid), p0);
    }

    internal static string EdmLibraryExtensions_ValueOverflowForUnderlyingType(object p0, object p1) => TextRes.GetString(nameof (EdmLibraryExtensions_ValueOverflowForUnderlyingType), p0, p1);

    internal static string ODataAtomResourceDeserializer_ContentWithWrongType(object p0) => TextRes.GetString(nameof (ODataAtomResourceDeserializer_ContentWithWrongType), p0);

    internal static string ODataAtomErrorDeserializer_MultipleErrorElementsWithSameName(object p0) => TextRes.GetString(nameof (ODataAtomErrorDeserializer_MultipleErrorElementsWithSameName), p0);

    internal static string ODataAtomErrorDeserializer_MultipleInnerErrorElementsWithSameName(
      object p0)
    {
      return TextRes.GetString(nameof (ODataAtomErrorDeserializer_MultipleInnerErrorElementsWithSameName), p0);
    }

    internal static string CollectionWithoutExpectedTypeValidator_InvalidItemTypeKind(object p0) => TextRes.GetString(nameof (CollectionWithoutExpectedTypeValidator_InvalidItemTypeKind), p0);

    internal static string CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind), p0, p1);
    }

    internal static string CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName), p0, p1);
    }

    internal static string ResourceSetWithoutExpectedTypeValidator_IncompatibleTypes(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ResourceSetWithoutExpectedTypeValidator_IncompatibleTypes), p0, p1);
    }

    internal static string MessageStreamWrappingStream_ByteLimitExceeded(object p0, object p1) => TextRes.GetString(nameof (MessageStreamWrappingStream_ByteLimitExceeded), p0, p1);

    internal static string MetadataUtils_ResolveTypeName(object p0) => TextRes.GetString(nameof (MetadataUtils_ResolveTypeName), p0);

    internal static string MetadataUtils_CalculateBindableOperationsForType(object p0) => TextRes.GetString(nameof (MetadataUtils_CalculateBindableOperationsForType), p0);

    internal static string EdmValueUtils_UnsupportedPrimitiveType(object p0) => TextRes.GetString(nameof (EdmValueUtils_UnsupportedPrimitiveType), p0);

    internal static string EdmValueUtils_IncorrectPrimitiveTypeKind(
      object p0,
      object p1,
      object p2)
    {
      return TextRes.GetString(nameof (EdmValueUtils_IncorrectPrimitiveTypeKind), p0, p1, p2);
    }

    internal static string EdmValueUtils_IncorrectPrimitiveTypeKindNoTypeName(object p0, object p1) => TextRes.GetString(nameof (EdmValueUtils_IncorrectPrimitiveTypeKindNoTypeName), p0, p1);

    internal static string EdmValueUtils_CannotConvertTypeToClrValue(object p0) => TextRes.GetString(nameof (EdmValueUtils_CannotConvertTypeToClrValue), p0);

    internal static string ODataEdmStructuredValue_UndeclaredProperty(object p0, object p1) => TextRes.GetString(nameof (ODataEdmStructuredValue_UndeclaredProperty), p0, p1);

    internal static string ODataMetadataBuilder_MissingEntitySetUri(object p0) => TextRes.GetString(nameof (ODataMetadataBuilder_MissingEntitySetUri), p0);

    internal static string ODataMetadataBuilder_MissingSegmentForEntitySetUriSuffix(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataMetadataBuilder_MissingSegmentForEntitySetUriSuffix), p0, p1);
    }

    internal static string ODataMetadataBuilder_MissingEntityInstanceUri(object p0) => TextRes.GetString(nameof (ODataMetadataBuilder_MissingEntityInstanceUri), p0);

    internal static string ODataMetadataBuilder_MissingParentIdOrContextUrl => TextRes.GetString(nameof (ODataMetadataBuilder_MissingParentIdOrContextUrl));

    internal static string ODataMetadataBuilder_UnknownEntitySet(object p0) => TextRes.GetString(nameof (ODataMetadataBuilder_UnknownEntitySet), p0);

    internal static string ODataJsonLightInputContext_EntityTypeMustBeCompatibleWithEntitySetBaseType(
      object p0,
      object p1,
      object p2)
    {
      return TextRes.GetString(nameof (ODataJsonLightInputContext_EntityTypeMustBeCompatibleWithEntitySetBaseType), p0, p1, p2);
    }

    internal static string ODataJsonLightInputContext_PayloadKindDetectionForRequest => TextRes.GetString(nameof (ODataJsonLightInputContext_PayloadKindDetectionForRequest));

    internal static string ODataJsonLightInputContext_OperationCannotBeNullForCreateParameterReader(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightInputContext_OperationCannotBeNullForCreateParameterReader), p0);
    }

    internal static string ODataJsonLightInputContext_NoEntitySetForRequest => TextRes.GetString(nameof (ODataJsonLightInputContext_NoEntitySetForRequest));

    internal static string ODataJsonLightInputContext_ModelRequiredForReading => TextRes.GetString(nameof (ODataJsonLightInputContext_ModelRequiredForReading));

    internal static string ODataJsonLightInputContext_ItemTypeRequiredForCollectionReaderInRequests => TextRes.GetString(nameof (ODataJsonLightInputContext_ItemTypeRequiredForCollectionReaderInRequests));

    internal static string ODataJsonLightDeserializer_ContextLinkNotFoundAsFirstProperty => TextRes.GetString(nameof (ODataJsonLightDeserializer_ContextLinkNotFoundAsFirstProperty));

    internal static string ODataJsonLightDeserializer_OnlyODataTypeAnnotationCanTargetInstanceAnnotation(
      object p0,
      object p1,
      object p2)
    {
      return TextRes.GetString(nameof (ODataJsonLightDeserializer_OnlyODataTypeAnnotationCanTargetInstanceAnnotation), p0, p1, p2);
    }

    internal static string ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightDeserializer_AnnotationTargetingInstanceAnnotationWithoutValue), p0, p1);
    }

    internal static string ODataJsonLightWriter_EntityReferenceLinkAfterResourceSetInRequest => TextRes.GetString(nameof (ODataJsonLightWriter_EntityReferenceLinkAfterResourceSetInRequest));

    internal static string ODataJsonLightWriter_InstanceAnnotationNotSupportedOnExpandedResourceSet => TextRes.GetString(nameof (ODataJsonLightWriter_InstanceAnnotationNotSupportedOnExpandedResourceSet));

    internal static string ODataJsonLightPropertyAndValueSerializer_NoExpectedTypeOrTypeNameSpecifiedForResourceValueRequest => TextRes.GetString(nameof (ODataJsonLightPropertyAndValueSerializer_NoExpectedTypeOrTypeNameSpecifiedForResourceValueRequest));

    internal static string ODataJsonLightPropertyAndValueSerializer_NoExpectedTypeOrTypeNameSpecifiedForCollectionValueInRequest => TextRes.GetString(nameof (ODataJsonLightPropertyAndValueSerializer_NoExpectedTypeOrTypeNameSpecifiedForCollectionValueInRequest));

    internal static string ODataResourceTypeContext_MetadataOrSerializationInfoMissing => TextRes.GetString(nameof (ODataResourceTypeContext_MetadataOrSerializationInfoMissing));

    internal static string ODataResourceTypeContext_ODataResourceTypeNameMissing => TextRes.GetString(nameof (ODataResourceTypeContext_ODataResourceTypeNameMissing));

    internal static string ODataContextUriBuilder_ValidateDerivedType(object p0, object p1) => TextRes.GetString(nameof (ODataContextUriBuilder_ValidateDerivedType), p0, p1);

    internal static string ODataContextUriBuilder_TypeNameMissingForTopLevelCollection => TextRes.GetString(nameof (ODataContextUriBuilder_TypeNameMissingForTopLevelCollection));

    internal static string ODataContextUriBuilder_UnsupportedPayloadKind(object p0) => TextRes.GetString(nameof (ODataContextUriBuilder_UnsupportedPayloadKind), p0);

    internal static string ODataContextUriBuilder_StreamValueMustBePropertiesOfODataResource => TextRes.GetString(nameof (ODataContextUriBuilder_StreamValueMustBePropertiesOfODataResource));

    internal static string ODataContextUriBuilder_NavigationSourceOrTypeNameMissingForResourceOrResourceSet => TextRes.GetString(nameof (ODataContextUriBuilder_NavigationSourceOrTypeNameMissingForResourceOrResourceSet));

    internal static string ODataContextUriBuilder_ODataUriMissingForIndividualProperty => TextRes.GetString(nameof (ODataContextUriBuilder_ODataUriMissingForIndividualProperty));

    internal static string ODataContextUriBuilder_TypeNameMissingForProperty => TextRes.GetString(nameof (ODataContextUriBuilder_TypeNameMissingForProperty));

    internal static string ODataContextUriBuilder_ODataPathInvalidForContainedElement(object p0) => TextRes.GetString(nameof (ODataContextUriBuilder_ODataPathInvalidForContainedElement), p0);

    internal static string ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties), p0);
    }

    internal static string ODataJsonLightPropertyAndValueDeserializer_UnexpectedPropertyAnnotation(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightPropertyAndValueDeserializer_UnexpectedPropertyAnnotation), p0, p1);
    }

    internal static string ODataJsonLightPropertyAndValueDeserializer_UnexpectedODataPropertyAnnotation(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightPropertyAndValueDeserializer_UnexpectedODataPropertyAnnotation), p0);
    }

    internal static string ODataJsonLightPropertyAndValueDeserializer_UnexpectedProperty(object p0) => TextRes.GetString(nameof (ODataJsonLightPropertyAndValueDeserializer_UnexpectedProperty), p0);

    internal static string ODataJsonLightPropertyAndValueDeserializer_InvalidTopLevelPropertyPayload => TextRes.GetString(nameof (ODataJsonLightPropertyAndValueDeserializer_InvalidTopLevelPropertyPayload));

    internal static string ODataJsonLightPropertyAndValueDeserializer_InvalidTopLevelPropertyName(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightPropertyAndValueDeserializer_InvalidTopLevelPropertyName), p0, p1);
    }

    internal static string ODataJsonLightPropertyAndValueDeserializer_InvalidTypeName(object p0) => TextRes.GetString(nameof (ODataJsonLightPropertyAndValueDeserializer_InvalidTypeName), p0);

    internal static string ODataJsonLightPropertyAndValueDeserializer_TopLevelPropertyAnnotationWithoutProperty(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightPropertyAndValueDeserializer_TopLevelPropertyAnnotationWithoutProperty), p0);
    }

    internal static string ODataJsonLightPropertyAndValueDeserializer_ResourceValuePropertyAnnotationWithoutProperty(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightPropertyAndValueDeserializer_ResourceValuePropertyAnnotationWithoutProperty), p0);
    }

    internal static string ODataJsonLightPropertyAndValueDeserializer_ComplexValueWithPropertyTypeAnnotation(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightPropertyAndValueDeserializer_ComplexValueWithPropertyTypeAnnotation), p0);
    }

    internal static string ODataJsonLightPropertyAndValueDeserializer_ResourceTypeAnnotationNotFirst => TextRes.GetString(nameof (ODataJsonLightPropertyAndValueDeserializer_ResourceTypeAnnotationNotFirst));

    internal static string ODataJsonLightPropertyAndValueDeserializer_UnexpectedDataPropertyAnnotation(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightPropertyAndValueDeserializer_UnexpectedDataPropertyAnnotation), p0, p1);
    }

    internal static string ODataJsonLightPropertyAndValueDeserializer_TypePropertyAfterValueProperty(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightPropertyAndValueDeserializer_TypePropertyAfterValueProperty), p0, p1);
    }

    internal static string ODataJsonLightPropertyAndValueDeserializer_ODataTypeAnnotationInPrimitiveValue(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightPropertyAndValueDeserializer_ODataTypeAnnotationInPrimitiveValue), p0);
    }

    internal static string ODataJsonLightPropertyAndValueDeserializer_TopLevelPropertyWithPrimitiveNullValue(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightPropertyAndValueDeserializer_TopLevelPropertyWithPrimitiveNullValue), p0, p1);
    }

    internal static string ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty), p0);
    }

    internal static string ODataJsonLightPropertyAndValueDeserializer_NoPropertyAndAnnotationAllowedInNullPayload(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightPropertyAndValueDeserializer_NoPropertyAndAnnotationAllowedInNullPayload), p0);
    }

    internal static string ODataJsonLightPropertyAndValueDeserializer_CollectionTypeNotExpected(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightPropertyAndValueDeserializer_CollectionTypeNotExpected), p0);
    }

    internal static string ODataJsonLightPropertyAndValueDeserializer_CollectionTypeExpected(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightPropertyAndValueDeserializer_CollectionTypeExpected), p0);
    }

    internal static string ODataJsonReaderCoreUtils_CannotReadSpatialPropertyValue => TextRes.GetString(nameof (ODataJsonReaderCoreUtils_CannotReadSpatialPropertyValue));

    internal static string ODataJsonLightReader_UnexpectedPrimitiveValueForODataResource => TextRes.GetString(nameof (ODataJsonLightReader_UnexpectedPrimitiveValueForODataResource));

    internal static string ODataJsonLightReaderUtils_AnnotationWithNullValue(object p0) => TextRes.GetString(nameof (ODataJsonLightReaderUtils_AnnotationWithNullValue), p0);

    internal static string ODataJsonLightReaderUtils_InvalidValueForODataNullAnnotation(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightReaderUtils_InvalidValueForODataNullAnnotation), p0, p1);
    }

    internal static string JsonLightInstanceAnnotationWriter_DuplicateAnnotationNameInCollection(
      object p0)
    {
      return TextRes.GetString(nameof (JsonLightInstanceAnnotationWriter_DuplicateAnnotationNameInCollection), p0);
    }

    internal static string ODataJsonLightContextUriParser_NullMetadataDocumentUri => TextRes.GetString(nameof (ODataJsonLightContextUriParser_NullMetadataDocumentUri));

    internal static string ODataJsonLightContextUriParser_ContextUriDoesNotMatchExpectedPayloadKind(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightContextUriParser_ContextUriDoesNotMatchExpectedPayloadKind), p0, p1);
    }

    internal static string ODataJsonLightContextUriParser_InvalidEntitySetNameOrTypeName(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightContextUriParser_InvalidEntitySetNameOrTypeName), p0, p1);
    }

    internal static string ODataJsonLightContextUriParser_InvalidPayloadKindWithSelectQueryOption(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightContextUriParser_InvalidPayloadKindWithSelectQueryOption), p0);
    }

    internal static string ODataJsonLightContextUriParser_NoModel => TextRes.GetString(nameof (ODataJsonLightContextUriParser_NoModel));

    internal static string ODataJsonLightContextUriParser_InvalidContextUrl(object p0) => TextRes.GetString(nameof (ODataJsonLightContextUriParser_InvalidContextUrl), p0);

    internal static string ODataJsonLightContextUriParser_LastSegmentIsKeySegment(object p0) => TextRes.GetString(nameof (ODataJsonLightContextUriParser_LastSegmentIsKeySegment), p0);

    internal static string ODataJsonLightContextUriParser_TopLevelContextUrlShouldBeAbsolute(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightContextUriParser_TopLevelContextUrlShouldBeAbsolute), p0);
    }

    internal static string ODataJsonLightResourceDeserializer_DeltaRemovedAnnotationMustBeObject(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_DeltaRemovedAnnotationMustBeObject), p0);
    }

    internal static string ODataJsonLightResourceDeserializer_ResourceTypeAnnotationNotFirst => TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_ResourceTypeAnnotationNotFirst));

    internal static string ODataJsonLightResourceDeserializer_ResourceInstanceAnnotationPrecededByProperty(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_ResourceInstanceAnnotationPrecededByProperty), p0);
    }

    internal static string ODataJsonLightResourceDeserializer_UnexpectedDeletedEntryInResponsePayload => TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_UnexpectedDeletedEntryInResponsePayload));

    internal static string ODataJsonLightResourceDeserializer_CannotReadResourceSetContentStart(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_CannotReadResourceSetContentStart), p0);
    }

    internal static string ODataJsonLightResourceDeserializer_ExpectedResourceSetPropertyNotFound(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_ExpectedResourceSetPropertyNotFound), p0);
    }

    internal static string ODataJsonLightResourceDeserializer_InvalidNodeTypeForItemsInResourceSet(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_InvalidNodeTypeForItemsInResourceSet), p0);
    }

    internal static string ODataJsonLightResourceDeserializer_InvalidPropertyAnnotationInTopLevelResourceSet(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_InvalidPropertyAnnotationInTopLevelResourceSet), p0);
    }

    internal static string ODataJsonLightResourceDeserializer_InvalidPropertyInTopLevelResourceSet(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_InvalidPropertyInTopLevelResourceSet), p0, p1);
    }

    internal static string ODataJsonLightResourceDeserializer_PropertyWithoutValueWithWrongType(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_PropertyWithoutValueWithWrongType), p0, p1);
    }

    internal static string ODataJsonLightResourceDeserializer_OpenPropertyWithoutValue(object p0) => TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_OpenPropertyWithoutValue), p0);

    internal static string ODataJsonLightResourceDeserializer_StreamPropertyInRequest(object p0) => TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_StreamPropertyInRequest), p0);

    internal static string ODataJsonLightResourceDeserializer_UnexpectedStreamPropertyAnnotation(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_UnexpectedStreamPropertyAnnotation), p0, p1);
    }

    internal static string ODataJsonLightResourceDeserializer_StreamPropertyWithValue(object p0) => TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_StreamPropertyWithValue), p0);

    internal static string ODataJsonLightResourceDeserializer_UnexpectedDeferredLinkPropertyAnnotation(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_UnexpectedDeferredLinkPropertyAnnotation), p0, p1);
    }

    internal static string ODataJsonLightResourceDeserializer_CannotReadSingletonNestedResource(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_CannotReadSingletonNestedResource), p0, p1);
    }

    internal static string ODataJsonLightResourceDeserializer_CannotReadCollectionNestedResource(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_CannotReadCollectionNestedResource), p0, p1);
    }

    internal static string ODataJsonLightResourceDeserializer_CannotReadNestedResource(object p0) => TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_CannotReadNestedResource), p0);

    internal static string ODataJsonLightResourceDeserializer_UnexpectedExpandedSingletonNavigationLinkPropertyAnnotation(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_UnexpectedExpandedSingletonNavigationLinkPropertyAnnotation), p0, p1);
    }

    internal static string ODataJsonLightResourceDeserializer_UnexpectedExpandedCollectionNavigationLinkPropertyAnnotation(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_UnexpectedExpandedCollectionNavigationLinkPropertyAnnotation), p0, p1);
    }

    internal static string ODataJsonLightResourceDeserializer_UnexpectedComplexCollectionPropertyAnnotation(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_UnexpectedComplexCollectionPropertyAnnotation), p0, p1);
    }

    internal static string ODataJsonLightResourceDeserializer_DuplicateNestedResourceSetAnnotation(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_DuplicateNestedResourceSetAnnotation), p0, p1);
    }

    internal static string ODataJsonLightResourceDeserializer_UnexpectedPropertyAnnotationAfterExpandedResourceSet(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_UnexpectedPropertyAnnotationAfterExpandedResourceSet), p0, p1);
    }

    internal static string ODataJsonLightResourceDeserializer_UnexpectedNavigationLinkInRequestPropertyAnnotation(
      object p0,
      object p1,
      object p2)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_UnexpectedNavigationLinkInRequestPropertyAnnotation), p0, p1, p2);
    }

    internal static string ODataJsonLightResourceDeserializer_ArrayValueForSingletonBindPropertyAnnotation(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_ArrayValueForSingletonBindPropertyAnnotation), p0, p1);
    }

    internal static string ODataJsonLightResourceDeserializer_StringValueForCollectionBindPropertyAnnotation(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_StringValueForCollectionBindPropertyAnnotation), p0, p1);
    }

    internal static string ODataJsonLightResourceDeserializer_EmptyBindArray(object p0) => TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_EmptyBindArray), p0);

    internal static string ODataJsonLightResourceDeserializer_NavigationPropertyWithoutValueAndEntityReferenceLink(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_NavigationPropertyWithoutValueAndEntityReferenceLink), p0, p1);
    }

    internal static string ODataJsonLightResourceDeserializer_SingletonNavigationPropertyWithBindingAndValue(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_SingletonNavigationPropertyWithBindingAndValue), p0, p1);
    }

    internal static string ODataJsonLightResourceDeserializer_PropertyWithoutValueWithUnknownType(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_PropertyWithoutValueWithUnknownType), p0);
    }

    internal static string ODataJsonLightResourceDeserializer_OperationIsNotActionOrFunction(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_OperationIsNotActionOrFunction), p0);
    }

    internal static string ODataJsonLightResourceDeserializer_MultipleOptionalPropertiesInOperation(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_MultipleOptionalPropertiesInOperation), p0, p1);
    }

    internal static string ODataJsonLightResourceDeserializer_OperationMissingTargetProperty(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_OperationMissingTargetProperty), p0);
    }

    internal static string ODataJsonLightResourceDeserializer_MetadataReferencePropertyInRequest => TextRes.GetString(nameof (ODataJsonLightResourceDeserializer_MetadataReferencePropertyInRequest));

    internal static string ODataJsonLightValidationUtils_OperationPropertyCannotBeNull(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightValidationUtils_OperationPropertyCannotBeNull), p0, p1);
    }

    internal static string ODataJsonLightValidationUtils_OpenMetadataReferencePropertyNotSupported(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightValidationUtils_OpenMetadataReferencePropertyNotSupported), p0, p1);
    }

    internal static string ODataJsonLightDeserializer_RelativeUriUsedWithouODataMetadataAnnotation(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightDeserializer_RelativeUriUsedWithouODataMetadataAnnotation), p0, p1);
    }

    internal static string ODataJsonLightResourceMetadataContext_MetadataAnnotationMustBeInPayload(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightResourceMetadataContext_MetadataAnnotationMustBeInPayload), p0);
    }

    internal static string ODataJsonLightCollectionDeserializer_ExpectedCollectionPropertyNotFound(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightCollectionDeserializer_ExpectedCollectionPropertyNotFound), p0);
    }

    internal static string ODataJsonLightCollectionDeserializer_CannotReadCollectionContentStart(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightCollectionDeserializer_CannotReadCollectionContentStart), p0);
    }

    internal static string ODataJsonLightCollectionDeserializer_CannotReadCollectionEnd(object p0) => TextRes.GetString(nameof (ODataJsonLightCollectionDeserializer_CannotReadCollectionEnd), p0);

    internal static string ODataJsonLightCollectionDeserializer_InvalidCollectionTypeName(object p0) => TextRes.GetString(nameof (ODataJsonLightCollectionDeserializer_InvalidCollectionTypeName), p0);

    internal static string ODataJsonLightEntityReferenceLinkDeserializer_EntityReferenceLinkMustBeObjectValue(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightEntityReferenceLinkDeserializer_EntityReferenceLinkMustBeObjectValue), p0);
    }

    internal static string ODataJsonLightEntityReferenceLinkDeserializer_PropertyAnnotationForEntityReferenceLink(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightEntityReferenceLinkDeserializer_PropertyAnnotationForEntityReferenceLink), p0);
    }

    internal static string ODataJsonLightEntityReferenceLinkDeserializer_InvalidAnnotationInEntityReferenceLink(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightEntityReferenceLinkDeserializer_InvalidAnnotationInEntityReferenceLink), p0);
    }

    internal static string ODataJsonLightEntityReferenceLinkDeserializer_InvalidPropertyInEntityReferenceLink(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightEntityReferenceLinkDeserializer_InvalidPropertyInEntityReferenceLink), p0, p1);
    }

    internal static string ODataJsonLightEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty), p0);
    }

    internal static string ODataJsonLightEntityReferenceLinkDeserializer_MultipleUriPropertiesInEntityReferenceLink(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightEntityReferenceLinkDeserializer_MultipleUriPropertiesInEntityReferenceLink), p0);
    }

    internal static string ODataJsonLightEntityReferenceLinkDeserializer_EntityReferenceLinkUrlCannotBeNull(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightEntityReferenceLinkDeserializer_EntityReferenceLinkUrlCannotBeNull), p0);
    }

    internal static string ODataJsonLightEntityReferenceLinkDeserializer_PropertyAnnotationForEntityReferenceLinks => TextRes.GetString(nameof (ODataJsonLightEntityReferenceLinkDeserializer_PropertyAnnotationForEntityReferenceLinks));

    internal static string ODataJsonLightEntityReferenceLinkDeserializer_InvalidEntityReferenceLinksPropertyFound(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightEntityReferenceLinkDeserializer_InvalidEntityReferenceLinksPropertyFound), p0, p1);
    }

    internal static string ODataJsonLightEntityReferenceLinkDeserializer_InvalidPropertyAnnotationInEntityReferenceLinks(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightEntityReferenceLinkDeserializer_InvalidPropertyAnnotationInEntityReferenceLinks), p0);
    }

    internal static string ODataJsonLightEntityReferenceLinkDeserializer_ExpectedEntityReferenceLinksPropertyNotFound(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightEntityReferenceLinkDeserializer_ExpectedEntityReferenceLinksPropertyNotFound), p0);
    }

    internal static string ODataJsonOperationsDeserializerUtils_OperationPropertyCannotBeNull(
      object p0,
      object p1,
      object p2)
    {
      return TextRes.GetString(nameof (ODataJsonOperationsDeserializerUtils_OperationPropertyCannotBeNull), p0, p1, p2);
    }

    internal static string ODataJsonOperationsDeserializerUtils_OperationsPropertyMustHaveObjectValue(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonOperationsDeserializerUtils_OperationsPropertyMustHaveObjectValue), p0, p1);
    }

    internal static string ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInServiceDocument(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInServiceDocument), p0);
    }

    internal static string ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInServiceDocumentElement(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightServiceDocumentDeserializer_DuplicatePropertiesInServiceDocumentElement), p0);
    }

    internal static string ODataJsonLightServiceDocumentDeserializer_MissingValuePropertyInServiceDocument(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightServiceDocumentDeserializer_MissingValuePropertyInServiceDocument), p0);
    }

    internal static string ODataJsonLightServiceDocumentDeserializer_MissingRequiredPropertyInServiceDocumentElement(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightServiceDocumentDeserializer_MissingRequiredPropertyInServiceDocumentElement), p0);
    }

    internal static string ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationInServiceDocument(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationInServiceDocument), p0, p1);
    }

    internal static string ODataJsonLightServiceDocumentDeserializer_InstanceAnnotationInServiceDocument(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightServiceDocumentDeserializer_InstanceAnnotationInServiceDocument), p0, p1);
    }

    internal static string ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationInServiceDocumentElement(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationInServiceDocumentElement), p0);
    }

    internal static string ODataJsonLightServiceDocumentDeserializer_InstanceAnnotationInServiceDocumentElement(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightServiceDocumentDeserializer_InstanceAnnotationInServiceDocumentElement), p0);
    }

    internal static string ODataJsonLightServiceDocumentDeserializer_UnexpectedPropertyInServiceDocumentElement(
      object p0,
      object p1,
      object p2)
    {
      return TextRes.GetString(nameof (ODataJsonLightServiceDocumentDeserializer_UnexpectedPropertyInServiceDocumentElement), p0, p1, p2);
    }

    internal static string ODataJsonLightServiceDocumentDeserializer_UnexpectedPropertyInServiceDocument(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightServiceDocumentDeserializer_UnexpectedPropertyInServiceDocument), p0, p1);
    }

    internal static string ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationWithoutProperty(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightServiceDocumentDeserializer_PropertyAnnotationWithoutProperty), p0);
    }

    internal static string ODataJsonLightParameterDeserializer_PropertyAnnotationForParameters => TextRes.GetString(nameof (ODataJsonLightParameterDeserializer_PropertyAnnotationForParameters));

    internal static string ODataJsonLightParameterDeserializer_PropertyAnnotationWithoutPropertyForParameters(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightParameterDeserializer_PropertyAnnotationWithoutPropertyForParameters), p0);
    }

    internal static string ODataJsonLightParameterDeserializer_UnsupportedPrimitiveParameterType(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightParameterDeserializer_UnsupportedPrimitiveParameterType), p0, p1);
    }

    internal static string ODataJsonLightParameterDeserializer_NullCollectionExpected(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightParameterDeserializer_NullCollectionExpected), p0, p1);
    }

    internal static string ODataJsonLightParameterDeserializer_UnsupportedParameterTypeKind(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataJsonLightParameterDeserializer_UnsupportedParameterTypeKind), p0, p1);
    }

    internal static string SelectedPropertiesNode_StarSegmentNotLastSegment => TextRes.GetString(nameof (SelectedPropertiesNode_StarSegmentNotLastSegment));

    internal static string SelectedPropertiesNode_StarSegmentAfterTypeSegment => TextRes.GetString(nameof (SelectedPropertiesNode_StarSegmentAfterTypeSegment));

    internal static string ODataJsonLightErrorDeserializer_PropertyAnnotationNotAllowedInErrorPayload(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightErrorDeserializer_PropertyAnnotationNotAllowedInErrorPayload), p0);
    }

    internal static string ODataJsonLightErrorDeserializer_InstanceAnnotationNotAllowedInErrorPayload(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightErrorDeserializer_InstanceAnnotationNotAllowedInErrorPayload), p0);
    }

    internal static string ODataJsonLightErrorDeserializer_PropertyAnnotationWithoutPropertyForError(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightErrorDeserializer_PropertyAnnotationWithoutPropertyForError), p0);
    }

    internal static string ODataJsonLightErrorDeserializer_TopLevelErrorValueWithInvalidProperty(
      object p0)
    {
      return TextRes.GetString(nameof (ODataJsonLightErrorDeserializer_TopLevelErrorValueWithInvalidProperty), p0);
    }

    internal static string ODataConventionalUriBuilder_EntityTypeWithNoKeyProperties(object p0) => TextRes.GetString(nameof (ODataConventionalUriBuilder_EntityTypeWithNoKeyProperties), p0);

    internal static string ODataConventionalUriBuilder_NullKeyValue(object p0, object p1) => TextRes.GetString(nameof (ODataConventionalUriBuilder_NullKeyValue), p0, p1);

    internal static string ODataResourceMetadataContext_EntityTypeWithNoKeyProperties(object p0) => TextRes.GetString(nameof (ODataResourceMetadataContext_EntityTypeWithNoKeyProperties), p0);

    internal static string ODataResourceMetadataContext_NullKeyValue(object p0, object p1) => TextRes.GetString(nameof (ODataResourceMetadataContext_NullKeyValue), p0, p1);

    internal static string ODataResourceMetadataContext_KeyOrETagValuesMustBePrimitiveValues(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ODataResourceMetadataContext_KeyOrETagValuesMustBePrimitiveValues), p0, p1);
    }

    internal static string ODataResource_PropertyValueCannotBeODataResourceValue(object p0) => TextRes.GetString(nameof (ODataResource_PropertyValueCannotBeODataResourceValue), p0);

    internal static string EdmValueUtils_NonPrimitiveValue(object p0, object p1) => TextRes.GetString(nameof (EdmValueUtils_NonPrimitiveValue), p0, p1);

    internal static string EdmValueUtils_PropertyDoesntExist(object p0, object p1) => TextRes.GetString(nameof (EdmValueUtils_PropertyDoesntExist), p0, p1);

    internal static string ODataPrimitiveValue_CannotCreateODataPrimitiveValueFromNull => TextRes.GetString(nameof (ODataPrimitiveValue_CannotCreateODataPrimitiveValueFromNull));

    internal static string ODataPrimitiveValue_CannotCreateODataPrimitiveValueFromUnsupportedValueType(
      object p0)
    {
      return TextRes.GetString(nameof (ODataPrimitiveValue_CannotCreateODataPrimitiveValueFromUnsupportedValueType), p0);
    }

    internal static string ODataInstanceAnnotation_NeedPeriodInName(object p0) => TextRes.GetString(nameof (ODataInstanceAnnotation_NeedPeriodInName), p0);

    internal static string ODataInstanceAnnotation_ReservedNamesNotAllowed(object p0, object p1) => TextRes.GetString(nameof (ODataInstanceAnnotation_ReservedNamesNotAllowed), p0, p1);

    internal static string ODataInstanceAnnotation_BadTermName(object p0) => TextRes.GetString(nameof (ODataInstanceAnnotation_BadTermName), p0);

    internal static string ODataInstanceAnnotation_ValueCannotBeODataStreamReferenceValue => TextRes.GetString(nameof (ODataInstanceAnnotation_ValueCannotBeODataStreamReferenceValue));

    internal static string ODataJsonLightValueSerializer_MissingTypeNameOnCollection => TextRes.GetString(nameof (ODataJsonLightValueSerializer_MissingTypeNameOnCollection));

    internal static string ODataJsonLightValueSerializer_MissingRawValueOnUntyped => TextRes.GetString(nameof (ODataJsonLightValueSerializer_MissingRawValueOnUntyped));

    internal static string AtomInstanceAnnotation_MissingTermAttributeOnAnnotationElement => TextRes.GetString(nameof (AtomInstanceAnnotation_MissingTermAttributeOnAnnotationElement));

    internal static string AtomInstanceAnnotation_AttributeValueNotationUsedWithIncompatibleType(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (AtomInstanceAnnotation_AttributeValueNotationUsedWithIncompatibleType), p0, p1);
    }

    internal static string AtomInstanceAnnotation_AttributeValueNotationUsedOnNonEmptyElement(
      object p0)
    {
      return TextRes.GetString(nameof (AtomInstanceAnnotation_AttributeValueNotationUsedOnNonEmptyElement), p0);
    }

    internal static string AtomInstanceAnnotation_MultipleAttributeValueNotationAttributes => TextRes.GetString(nameof (AtomInstanceAnnotation_MultipleAttributeValueNotationAttributes));

    internal static string AnnotationFilterPattern_InvalidPatternMissingDot(object p0) => TextRes.GetString(nameof (AnnotationFilterPattern_InvalidPatternMissingDot), p0);

    internal static string AnnotationFilterPattern_InvalidPatternEmptySegment(object p0) => TextRes.GetString(nameof (AnnotationFilterPattern_InvalidPatternEmptySegment), p0);

    internal static string AnnotationFilterPattern_InvalidPatternWildCardInSegment(object p0) => TextRes.GetString(nameof (AnnotationFilterPattern_InvalidPatternWildCardInSegment), p0);

    internal static string AnnotationFilterPattern_InvalidPatternWildCardMustBeInLastSegment(
      object p0)
    {
      return TextRes.GetString(nameof (AnnotationFilterPattern_InvalidPatternWildCardMustBeInLastSegment), p0);
    }

    internal static string SyntacticTree_UriMustBeAbsolute(object p0) => TextRes.GetString(nameof (SyntacticTree_UriMustBeAbsolute), p0);

    internal static string SyntacticTree_MaxDepthInvalid => TextRes.GetString(nameof (SyntacticTree_MaxDepthInvalid));

    internal static string SyntacticTree_InvalidSkipQueryOptionValue(object p0) => TextRes.GetString(nameof (SyntacticTree_InvalidSkipQueryOptionValue), p0);

    internal static string SyntacticTree_InvalidTopQueryOptionValue(object p0) => TextRes.GetString(nameof (SyntacticTree_InvalidTopQueryOptionValue), p0);

    internal static string SyntacticTree_InvalidCountQueryOptionValue(object p0, object p1) => TextRes.GetString(nameof (SyntacticTree_InvalidCountQueryOptionValue), p0, p1);

    internal static string SyntacticTree_InvalidIndexQueryOptionValue(object p0) => TextRes.GetString(nameof (SyntacticTree_InvalidIndexQueryOptionValue), p0);

    internal static string QueryOptionUtils_QueryParameterMustBeSpecifiedOnce(object p0) => TextRes.GetString(nameof (QueryOptionUtils_QueryParameterMustBeSpecifiedOnce), p0);

    internal static string UriBuilder_NotSupportedClrLiteral(object p0) => TextRes.GetString(nameof (UriBuilder_NotSupportedClrLiteral), p0);

    internal static string UriBuilder_NotSupportedQueryToken(object p0) => TextRes.GetString(nameof (UriBuilder_NotSupportedQueryToken), p0);

    internal static string UriQueryExpressionParser_TooDeep => TextRes.GetString(nameof (UriQueryExpressionParser_TooDeep));

    internal static string UriQueryExpressionParser_ExpressionExpected(object p0, object p1) => TextRes.GetString(nameof (UriQueryExpressionParser_ExpressionExpected), p0, p1);

    internal static string UriQueryExpressionParser_OpenParenExpected(object p0, object p1) => TextRes.GetString(nameof (UriQueryExpressionParser_OpenParenExpected), p0, p1);

    internal static string UriQueryExpressionParser_CloseParenOrCommaExpected(object p0, object p1) => TextRes.GetString(nameof (UriQueryExpressionParser_CloseParenOrCommaExpected), p0, p1);

    internal static string UriQueryExpressionParser_CloseParenOrOperatorExpected(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (UriQueryExpressionParser_CloseParenOrOperatorExpected), p0, p1);
    }

    internal static string UriQueryExpressionParser_CannotCreateStarTokenFromNonStar(object p0) => TextRes.GetString(nameof (UriQueryExpressionParser_CannotCreateStarTokenFromNonStar), p0);

    internal static string UriQueryExpressionParser_RangeVariableAlreadyDeclared(object p0) => TextRes.GetString(nameof (UriQueryExpressionParser_RangeVariableAlreadyDeclared), p0);

    internal static string UriQueryExpressionParser_AsExpected(object p0, object p1) => TextRes.GetString(nameof (UriQueryExpressionParser_AsExpected), p0, p1);

    internal static string UriQueryExpressionParser_WithExpected(object p0, object p1) => TextRes.GetString(nameof (UriQueryExpressionParser_WithExpected), p0, p1);

    internal static string UriQueryExpressionParser_UnrecognizedWithMethod(
      object p0,
      object p1,
      object p2)
    {
      return TextRes.GetString(nameof (UriQueryExpressionParser_UnrecognizedWithMethod), p0, p1, p2);
    }

    internal static string UriQueryExpressionParser_PropertyPathExpected(object p0, object p1) => TextRes.GetString(nameof (UriQueryExpressionParser_PropertyPathExpected), p0, p1);

    internal static string UriQueryExpressionParser_KeywordOrIdentifierExpected(
      object p0,
      object p1,
      object p2)
    {
      return TextRes.GetString(nameof (UriQueryExpressionParser_KeywordOrIdentifierExpected), p0, p1, p2);
    }

    internal static string UriQueryExpressionParser_InnerMostExpandRequireFilter(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (UriQueryExpressionParser_InnerMostExpandRequireFilter), p0, p1);
    }

    internal static string UriQueryPathParser_RequestUriDoesNotHaveTheCorrectBaseUri(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (UriQueryPathParser_RequestUriDoesNotHaveTheCorrectBaseUri), p0, p1);
    }

    internal static string UriQueryPathParser_SyntaxError => TextRes.GetString(nameof (UriQueryPathParser_SyntaxError));

    internal static string UriQueryPathParser_TooManySegments => TextRes.GetString(nameof (UriQueryPathParser_TooManySegments));

    internal static string UriQueryPathParser_InvalidEscapeUri(object p0) => TextRes.GetString(nameof (UriQueryPathParser_InvalidEscapeUri), p0);

    internal static string UriUtils_DateTimeOffsetInvalidFormat(object p0) => TextRes.GetString(nameof (UriUtils_DateTimeOffsetInvalidFormat), p0);

    internal static string SelectionItemBinder_NonNavigationPathToken => TextRes.GetString(nameof (SelectionItemBinder_NonNavigationPathToken));

    internal static string MetadataBinder_UnsupportedQueryTokenKind(object p0) => TextRes.GetString(nameof (MetadataBinder_UnsupportedQueryTokenKind), p0);

    internal static string MetadataBinder_PropertyNotDeclared(object p0, object p1) => TextRes.GetString(nameof (MetadataBinder_PropertyNotDeclared), p0, p1);

    internal static string MetadataBinder_InvalidIdentifierInQueryOption(object p0) => TextRes.GetString(nameof (MetadataBinder_InvalidIdentifierInQueryOption), p0);

    internal static string MetadataBinder_PropertyNotDeclaredOrNotKeyInKeyValue(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (MetadataBinder_PropertyNotDeclaredOrNotKeyInKeyValue), p0, p1);
    }

    internal static string MetadataBinder_QualifiedFunctionNameWithParametersNotDeclared(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (MetadataBinder_QualifiedFunctionNameWithParametersNotDeclared), p0, p1);
    }

    internal static string MetadataBinder_UnnamedKeyValueOnTypeWithMultipleKeyProperties(object p0) => TextRes.GetString(nameof (MetadataBinder_UnnamedKeyValueOnTypeWithMultipleKeyProperties), p0);

    internal static string MetadataBinder_DuplicitKeyPropertyInKeyValues(object p0) => TextRes.GetString(nameof (MetadataBinder_DuplicitKeyPropertyInKeyValues), p0);

    internal static string MetadataBinder_NotAllKeyPropertiesSpecifiedInKeyValues(object p0) => TextRes.GetString(nameof (MetadataBinder_NotAllKeyPropertiesSpecifiedInKeyValues), p0);

    internal static string MetadataBinder_CannotConvertToType(object p0, object p1) => TextRes.GetString(nameof (MetadataBinder_CannotConvertToType), p0, p1);

    internal static string MetadataBinder_FilterExpressionNotSingleValue => TextRes.GetString(nameof (MetadataBinder_FilterExpressionNotSingleValue));

    internal static string MetadataBinder_OrderByExpressionNotSingleValue => TextRes.GetString(nameof (MetadataBinder_OrderByExpressionNotSingleValue));

    internal static string MetadataBinder_PropertyAccessWithoutParentParameter => TextRes.GetString(nameof (MetadataBinder_PropertyAccessWithoutParentParameter));

    internal static string MetadataBinder_BinaryOperatorOperandNotSingleValue(object p0) => TextRes.GetString(nameof (MetadataBinder_BinaryOperatorOperandNotSingleValue), p0);

    internal static string MetadataBinder_UnaryOperatorOperandNotSingleValue(object p0) => TextRes.GetString(nameof (MetadataBinder_UnaryOperatorOperandNotSingleValue), p0);

    internal static string MetadataBinder_LeftOperandNotSingleValue => TextRes.GetString(nameof (MetadataBinder_LeftOperandNotSingleValue));

    internal static string MetadataBinder_RightOperandNotCollectionValue => TextRes.GetString(nameof (MetadataBinder_RightOperandNotCollectionValue));

    internal static string MetadataBinder_PropertyAccessSourceNotSingleValue(object p0) => TextRes.GetString(nameof (MetadataBinder_PropertyAccessSourceNotSingleValue), p0);

    internal static string MetadataBinder_IncompatibleOperandsError(
      object p0,
      object p1,
      object p2)
    {
      return TextRes.GetString(nameof (MetadataBinder_IncompatibleOperandsError), p0, p1, p2);
    }

    internal static string MetadataBinder_IncompatibleOperandError(object p0, object p1) => TextRes.GetString(nameof (MetadataBinder_IncompatibleOperandError), p0, p1);

    internal static string MetadataBinder_UnknownFunction(object p0) => TextRes.GetString(nameof (MetadataBinder_UnknownFunction), p0);

    internal static string MetadataBinder_FunctionArgumentNotSingleValue(object p0) => TextRes.GetString(nameof (MetadataBinder_FunctionArgumentNotSingleValue), p0);

    internal static string MetadataBinder_NoApplicableFunctionFound(object p0, object p1) => TextRes.GetString(nameof (MetadataBinder_NoApplicableFunctionFound), p0, p1);

    internal static string MetadataBinder_BoundNodeCannotBeNull(object p0) => TextRes.GetString(nameof (MetadataBinder_BoundNodeCannotBeNull), p0);

    internal static string MetadataBinder_TopRequiresNonNegativeInteger(object p0) => TextRes.GetString(nameof (MetadataBinder_TopRequiresNonNegativeInteger), p0);

    internal static string MetadataBinder_SkipRequiresNonNegativeInteger(object p0) => TextRes.GetString(nameof (MetadataBinder_SkipRequiresNonNegativeInteger), p0);

    internal static string MetadataBinder_QueryOptionsBindStateCannotBeNull => TextRes.GetString(nameof (MetadataBinder_QueryOptionsBindStateCannotBeNull));

    internal static string MetadataBinder_QueryOptionsBindMethodCannotBeNull => TextRes.GetString(nameof (MetadataBinder_QueryOptionsBindMethodCannotBeNull));

    internal static string MetadataBinder_HierarchyNotFollowed(object p0, object p1) => TextRes.GetString(nameof (MetadataBinder_HierarchyNotFollowed), p0, p1);

    internal static string MetadataBinder_LambdaParentMustBeCollection => TextRes.GetString(nameof (MetadataBinder_LambdaParentMustBeCollection));

    internal static string MetadataBinder_ParameterNotInScope(object p0) => TextRes.GetString(nameof (MetadataBinder_ParameterNotInScope), p0);

    internal static string MetadataBinder_NavigationPropertyNotFollowingSingleEntityType => TextRes.GetString(nameof (MetadataBinder_NavigationPropertyNotFollowingSingleEntityType));

    internal static string MetadataBinder_AnyAllExpressionNotSingleValue => TextRes.GetString(nameof (MetadataBinder_AnyAllExpressionNotSingleValue));

    internal static string MetadataBinder_CastOrIsOfExpressionWithWrongNumberOfOperands(object p0) => TextRes.GetString(nameof (MetadataBinder_CastOrIsOfExpressionWithWrongNumberOfOperands), p0);

    internal static string MetadataBinder_CastOrIsOfFunctionWithoutATypeArgument => TextRes.GetString(nameof (MetadataBinder_CastOrIsOfFunctionWithoutATypeArgument));

    internal static string MetadataBinder_CastOrIsOfCollectionsNotSupported => TextRes.GetString(nameof (MetadataBinder_CastOrIsOfCollectionsNotSupported));

    internal static string MetadataBinder_CollectionOpenPropertiesNotSupportedInThisRelease => TextRes.GetString(nameof (MetadataBinder_CollectionOpenPropertiesNotSupportedInThisRelease));

    internal static string MetadataBinder_IllegalSegmentType(object p0) => TextRes.GetString(nameof (MetadataBinder_IllegalSegmentType), p0);

    internal static string MetadataBinder_QueryOptionNotApplicable(object p0) => TextRes.GetString(nameof (MetadataBinder_QueryOptionNotApplicable), p0);

    internal static string StringItemShouldBeQuoted(object p0) => TextRes.GetString(nameof (StringItemShouldBeQuoted), p0);

    internal static string StreamItemInvalidPrimitiveKind(object p0) => TextRes.GetString(nameof (StreamItemInvalidPrimitiveKind), p0);

    internal static string ApplyBinder_AggregateExpressionIncompatibleTypeForMethod(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ApplyBinder_AggregateExpressionIncompatibleTypeForMethod), p0, p1);
    }

    internal static string ApplyBinder_UnsupportedAggregateMethod(object p0) => TextRes.GetString(nameof (ApplyBinder_UnsupportedAggregateMethod), p0);

    internal static string ApplyBinder_UnsupportedAggregateKind(object p0) => TextRes.GetString(nameof (ApplyBinder_UnsupportedAggregateKind), p0);

    internal static string ApplyBinder_AggregateExpressionNotSingleValue(object p0) => TextRes.GetString(nameof (ApplyBinder_AggregateExpressionNotSingleValue), p0);

    internal static string ApplyBinder_GroupByPropertyNotPropertyAccessValue(object p0) => TextRes.GetString(nameof (ApplyBinder_GroupByPropertyNotPropertyAccessValue), p0);

    internal static string ApplyBinder_UnsupportedType(object p0) => TextRes.GetString(nameof (ApplyBinder_UnsupportedType), p0);

    internal static string ApplyBinder_UnsupportedGroupByChild(object p0) => TextRes.GetString(nameof (ApplyBinder_UnsupportedGroupByChild), p0);

    internal static string ApplyBinder_UnsupportedForEntitySetAggregation(object p0, object p1) => TextRes.GetString(nameof (ApplyBinder_UnsupportedForEntitySetAggregation), p0, p1);

    internal static string AggregateTransformationNode_UnsupportedAggregateExpressions => TextRes.GetString(nameof (AggregateTransformationNode_UnsupportedAggregateExpressions));

    internal static string FunctionCallBinder_CannotFindASuitableOverload(object p0, object p1) => TextRes.GetString(nameof (FunctionCallBinder_CannotFindASuitableOverload), p0, p1);

    internal static string FunctionCallBinder_UriFunctionMustHaveHaveNullParent(object p0) => TextRes.GetString(nameof (FunctionCallBinder_UriFunctionMustHaveHaveNullParent), p0);

    internal static string FunctionCallBinder_CallingFunctionOnOpenProperty(object p0) => TextRes.GetString(nameof (FunctionCallBinder_CallingFunctionOnOpenProperty), p0);

    internal static string FunctionCallParser_DuplicateParameterOrEntityKeyName => TextRes.GetString(nameof (FunctionCallParser_DuplicateParameterOrEntityKeyName));

    internal static string ODataUriParser_InvalidCount(object p0) => TextRes.GetString(nameof (ODataUriParser_InvalidCount), p0);

    internal static string CastBinder_ChildTypeIsNotEntity(object p0) => TextRes.GetString(nameof (CastBinder_ChildTypeIsNotEntity), p0);

    internal static string CastBinder_EnumOnlyCastToOrFromString => TextRes.GetString(nameof (CastBinder_EnumOnlyCastToOrFromString));

    internal static string Binder_IsNotValidEnumConstant(object p0) => TextRes.GetString(nameof (Binder_IsNotValidEnumConstant), p0);

    internal static string BatchReferenceSegment_InvalidContentID(object p0) => TextRes.GetString(nameof (BatchReferenceSegment_InvalidContentID), p0);

    internal static string SelectExpandBinder_UnknownPropertyType(object p0) => TextRes.GetString(nameof (SelectExpandBinder_UnknownPropertyType), p0);

    internal static string SelectExpandBinder_InvalidIdentifierAfterWildcard(object p0) => TextRes.GetString(nameof (SelectExpandBinder_InvalidIdentifierAfterWildcard), p0);

    internal static string SelectExpandBinder_InvalidQueryOptionNestedSelection(object p0) => TextRes.GetString(nameof (SelectExpandBinder_InvalidQueryOptionNestedSelection), p0);

    internal static string SelectExpandBinder_SystemTokenInSelect(object p0) => TextRes.GetString(nameof (SelectExpandBinder_SystemTokenInSelect), p0);

    internal static string SelectionItemBinder_NoExpandForSelectedProperty(object p0) => TextRes.GetString(nameof (SelectionItemBinder_NoExpandForSelectedProperty), p0);

    internal static string SelectExpandPathBinder_FollowNonTypeSegment(object p0) => TextRes.GetString(nameof (SelectExpandPathBinder_FollowNonTypeSegment), p0);

    internal static string SelectBinder_MultiLevelPathInSelect => TextRes.GetString(nameof (SelectBinder_MultiLevelPathInSelect));

    internal static string ExpandItemBinder_TraversingANonNormalizedTree => TextRes.GetString(nameof (ExpandItemBinder_TraversingANonNormalizedTree));

    internal static string ExpandItemBinder_CannotFindType(object p0) => TextRes.GetString(nameof (ExpandItemBinder_CannotFindType), p0);

    internal static string ExpandItemBinder_PropertyIsNotANavigationPropertyOrComplexProperty(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (ExpandItemBinder_PropertyIsNotANavigationPropertyOrComplexProperty), p0, p1);
    }

    internal static string ExpandItemBinder_TypeSegmentNotFollowedByPath => TextRes.GetString(nameof (ExpandItemBinder_TypeSegmentNotFollowedByPath));

    internal static string ExpandItemBinder_PathTooDeep => TextRes.GetString(nameof (ExpandItemBinder_PathTooDeep));

    internal static string ExpandItemBinder_TraversingMultipleNavPropsInTheSamePath => TextRes.GetString(nameof (ExpandItemBinder_TraversingMultipleNavPropsInTheSamePath));

    internal static string ExpandItemBinder_LevelsNotAllowedOnIncompatibleRelatedType(
      object p0,
      object p1,
      object p2)
    {
      return TextRes.GetString(nameof (ExpandItemBinder_LevelsNotAllowedOnIncompatibleRelatedType), p0, p1, p2);
    }

    internal static string ExpandItemBinder_InvaidSegmentInExpand(object p0) => TextRes.GetString(nameof (ExpandItemBinder_InvaidSegmentInExpand), p0);

    internal static string Nodes_CollectionNavigationNode_MustHaveSingleMultiplicity => TextRes.GetString(nameof (Nodes_CollectionNavigationNode_MustHaveSingleMultiplicity));

    internal static string Nodes_NonentityParameterQueryNodeWithEntityType(object p0) => TextRes.GetString(nameof (Nodes_NonentityParameterQueryNodeWithEntityType), p0);

    internal static string Nodes_CollectionNavigationNode_MustHaveManyMultiplicity => TextRes.GetString(nameof (Nodes_CollectionNavigationNode_MustHaveManyMultiplicity));

    internal static string Nodes_PropertyAccessShouldBeNonEntityProperty(object p0) => TextRes.GetString(nameof (Nodes_PropertyAccessShouldBeNonEntityProperty), p0);

    internal static string Nodes_PropertyAccessTypeShouldNotBeCollection(object p0) => TextRes.GetString(nameof (Nodes_PropertyAccessTypeShouldNotBeCollection), p0);

    internal static string Nodes_PropertyAccessTypeMustBeCollection(object p0) => TextRes.GetString(nameof (Nodes_PropertyAccessTypeMustBeCollection), p0);

    internal static string Nodes_NonStaticEntitySetExpressionsAreNotSupportedInThisRelease => TextRes.GetString(nameof (Nodes_NonStaticEntitySetExpressionsAreNotSupportedInThisRelease));

    internal static string Nodes_CollectionFunctionCallNode_ItemTypeMustBePrimitiveOrComplexOrEnum => TextRes.GetString(nameof (Nodes_CollectionFunctionCallNode_ItemTypeMustBePrimitiveOrComplexOrEnum));

    internal static string Nodes_EntityCollectionFunctionCallNode_ItemTypeMustBeAnEntity => TextRes.GetString(nameof (Nodes_EntityCollectionFunctionCallNode_ItemTypeMustBeAnEntity));

    internal static string Nodes_SingleValueFunctionCallNode_ItemTypeMustBePrimitiveOrComplexOrEnum => TextRes.GetString(nameof (Nodes_SingleValueFunctionCallNode_ItemTypeMustBePrimitiveOrComplexOrEnum));

    internal static string Nodes_InNode_CollectionItemTypeMustBeSameAsSingleItemType(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (Nodes_InNode_CollectionItemTypeMustBeSameAsSingleItemType), p0, p1);
    }

    internal static string ExpandTreeNormalizer_NonPathInPropertyChain => TextRes.GetString(nameof (ExpandTreeNormalizer_NonPathInPropertyChain));

    internal static string SelectTreeNormalizer_MultipleSelecTermWithSamePathFound(object p0) => TextRes.GetString(nameof (SelectTreeNormalizer_MultipleSelecTermWithSamePathFound), p0);

    internal static string UriExpandParser_TermIsNotValidForStar(object p0) => TextRes.GetString(nameof (UriExpandParser_TermIsNotValidForStar), p0);

    internal static string UriExpandParser_TermIsNotValidForStarRef(object p0) => TextRes.GetString(nameof (UriExpandParser_TermIsNotValidForStarRef), p0);

    internal static string UriExpandParser_ParentStructuredTypeIsNull(object p0) => TextRes.GetString(nameof (UriExpandParser_ParentStructuredTypeIsNull), p0);

    internal static string UriExpandParser_TermWithMultipleStarNotAllowed(object p0) => TextRes.GetString(nameof (UriExpandParser_TermWithMultipleStarNotAllowed), p0);

    internal static string UriSelectParser_TermIsNotValid(object p0) => TextRes.GetString(nameof (UriSelectParser_TermIsNotValid), p0);

    internal static string UriSelectParser_InvalidTopOption(object p0) => TextRes.GetString(nameof (UriSelectParser_InvalidTopOption), p0);

    internal static string UriSelectParser_InvalidSkipOption(object p0) => TextRes.GetString(nameof (UriSelectParser_InvalidSkipOption), p0);

    internal static string UriSelectParser_InvalidCountOption(object p0) => TextRes.GetString(nameof (UriSelectParser_InvalidCountOption), p0);

    internal static string UriSelectParser_InvalidLevelsOption(object p0) => TextRes.GetString(nameof (UriSelectParser_InvalidLevelsOption), p0);

    internal static string UriSelectParser_SystemTokenInSelectExpand(object p0, object p1) => TextRes.GetString(nameof (UriSelectParser_SystemTokenInSelectExpand), p0, p1);

    internal static string UriParser_MissingExpandOption(object p0) => TextRes.GetString(nameof (UriParser_MissingExpandOption), p0);

    internal static string UriParser_MissingSelectOption(object p0) => TextRes.GetString(nameof (UriParser_MissingSelectOption), p0);

    internal static string UriParser_RelativeUriMustBeRelative => TextRes.GetString(nameof (UriParser_RelativeUriMustBeRelative));

    internal static string UriParser_NeedServiceRootForThisOverload => TextRes.GetString(nameof (UriParser_NeedServiceRootForThisOverload));

    internal static string UriParser_UriMustBeAbsolute(object p0) => TextRes.GetString(nameof (UriParser_UriMustBeAbsolute), p0);

    internal static string UriParser_NegativeLimit => TextRes.GetString(nameof (UriParser_NegativeLimit));

    internal static string UriParser_ExpandCountExceeded(object p0, object p1) => TextRes.GetString(nameof (UriParser_ExpandCountExceeded), p0, p1);

    internal static string UriParser_ExpandDepthExceeded(object p0, object p1) => TextRes.GetString(nameof (UriParser_ExpandDepthExceeded), p0, p1);

    internal static string UriParser_TypeInvalidForSelectExpand(object p0) => TextRes.GetString(nameof (UriParser_TypeInvalidForSelectExpand), p0);

    internal static string UriParser_ContextHandlerCanNotBeNull(object p0) => TextRes.GetString(nameof (UriParser_ContextHandlerCanNotBeNull), p0);

    internal static string UriParserMetadata_MultipleMatchingPropertiesFound(object p0, object p1) => TextRes.GetString(nameof (UriParserMetadata_MultipleMatchingPropertiesFound), p0, p1);

    internal static string UriParserMetadata_MultipleMatchingNavigationSourcesFound(object p0) => TextRes.GetString(nameof (UriParserMetadata_MultipleMatchingNavigationSourcesFound), p0);

    internal static string UriParserMetadata_MultipleMatchingTypesFound(object p0) => TextRes.GetString(nameof (UriParserMetadata_MultipleMatchingTypesFound), p0);

    internal static string UriParserMetadata_MultipleMatchingKeysFound(object p0) => TextRes.GetString(nameof (UriParserMetadata_MultipleMatchingKeysFound), p0);

    internal static string UriParserMetadata_MultipleMatchingParametersFound(object p0) => TextRes.GetString(nameof (UriParserMetadata_MultipleMatchingParametersFound), p0);

    internal static string PathParser_EntityReferenceNotSupported(object p0) => TextRes.GetString(nameof (PathParser_EntityReferenceNotSupported), p0);

    internal static string PathParser_CannotUseValueOnCollection => TextRes.GetString(nameof (PathParser_CannotUseValueOnCollection));

    internal static string PathParser_TypeMustBeRelatedToSet(object p0, object p1, object p2) => TextRes.GetString(nameof (PathParser_TypeMustBeRelatedToSet), p0, p1, p2);

    internal static string PathParser_TypeCastOnlyAllowedAfterStructuralCollection(object p0) => TextRes.GetString(nameof (PathParser_TypeCastOnlyAllowedAfterStructuralCollection), p0);

    internal static string PathParser_TypeCastOnlyAllowedInDerivedTypeConstraint(
      object p0,
      object p1,
      object p2)
    {
      return TextRes.GetString(nameof (PathParser_TypeCastOnlyAllowedInDerivedTypeConstraint), p0, p1, p2);
    }

    internal static string ODataResourceSet_MustNotContainBothNextPageLinkAndDeltaLink => TextRes.GetString(nameof (ODataResourceSet_MustNotContainBothNextPageLinkAndDeltaLink));

    internal static string ODataExpandPath_OnlyLastSegmentMustBeNavigationProperty => TextRes.GetString(nameof (ODataExpandPath_OnlyLastSegmentMustBeNavigationProperty));

    internal static string ODataExpandPath_InvalidExpandPathSegment(object p0) => TextRes.GetString(nameof (ODataExpandPath_InvalidExpandPathSegment), p0);

    internal static string ODataSelectPath_CannotOnlyHaveTypeSegment => TextRes.GetString(nameof (ODataSelectPath_CannotOnlyHaveTypeSegment));

    internal static string ODataSelectPath_InvalidSelectPathSegmentType(object p0) => TextRes.GetString(nameof (ODataSelectPath_InvalidSelectPathSegmentType), p0);

    internal static string ODataSelectPath_OperationSegmentCanOnlyBeLastSegment => TextRes.GetString(nameof (ODataSelectPath_OperationSegmentCanOnlyBeLastSegment));

    internal static string ODataSelectPath_NavPropSegmentCanOnlyBeLastSegment => TextRes.GetString(nameof (ODataSelectPath_NavPropSegmentCanOnlyBeLastSegment));

    internal static string RequestUriProcessor_TargetEntitySetNotFound(object p0) => TextRes.GetString(nameof (RequestUriProcessor_TargetEntitySetNotFound), p0);

    internal static string RequestUriProcessor_FoundInvalidFunctionImport(object p0) => TextRes.GetString(nameof (RequestUriProcessor_FoundInvalidFunctionImport), p0);

    internal static string OperationSegment_ReturnTypeForMultipleOverloads => TextRes.GetString(nameof (OperationSegment_ReturnTypeForMultipleOverloads));

    internal static string OperationSegment_CannotReturnNull => TextRes.GetString(nameof (OperationSegment_CannotReturnNull));

    internal static string FunctionOverloadResolver_NoSingleMatchFound(object p0, object p1) => TextRes.GetString(nameof (FunctionOverloadResolver_NoSingleMatchFound), p0, p1);

    internal static string FunctionOverloadResolver_MultipleActionOverloads(object p0) => TextRes.GetString(nameof (FunctionOverloadResolver_MultipleActionOverloads), p0);

    internal static string FunctionOverloadResolver_MultipleActionImportOverloads(object p0) => TextRes.GetString(nameof (FunctionOverloadResolver_MultipleActionImportOverloads), p0);

    internal static string FunctionOverloadResolver_MultipleOperationImportOverloads(object p0) => TextRes.GetString(nameof (FunctionOverloadResolver_MultipleOperationImportOverloads), p0);

    internal static string FunctionOverloadResolver_MultipleOperationOverloads(object p0) => TextRes.GetString(nameof (FunctionOverloadResolver_MultipleOperationOverloads), p0);

    internal static string FunctionOverloadResolver_FoundInvalidOperation(object p0) => TextRes.GetString(nameof (FunctionOverloadResolver_FoundInvalidOperation), p0);

    internal static string FunctionOverloadResolver_FoundInvalidOperationImport(object p0) => TextRes.GetString(nameof (FunctionOverloadResolver_FoundInvalidOperationImport), p0);

    internal static string CustomUriFunctions_AddCustomUriFunction_BuiltInExistsNotAddingAsOverload(
      object p0)
    {
      return TextRes.GetString(nameof (CustomUriFunctions_AddCustomUriFunction_BuiltInExistsNotAddingAsOverload), p0);
    }

    internal static string CustomUriFunctions_AddCustomUriFunction_BuiltInExistsFullSignature(
      object p0)
    {
      return TextRes.GetString(nameof (CustomUriFunctions_AddCustomUriFunction_BuiltInExistsFullSignature), p0);
    }

    internal static string CustomUriFunctions_AddCustomUriFunction_CustomFunctionOverloadExists(
      object p0)
    {
      return TextRes.GetString(nameof (CustomUriFunctions_AddCustomUriFunction_CustomFunctionOverloadExists), p0);
    }

    internal static string RequestUriProcessor_InvalidValueForEntitySegment(object p0) => TextRes.GetString(nameof (RequestUriProcessor_InvalidValueForEntitySegment), p0);

    internal static string RequestUriProcessor_InvalidValueForKeySegment(object p0) => TextRes.GetString(nameof (RequestUriProcessor_InvalidValueForKeySegment), p0);

    internal static string RequestUriProcessor_CannotApplyFilterOnSingleEntities(object p0) => TextRes.GetString(nameof (RequestUriProcessor_CannotApplyFilterOnSingleEntities), p0);

    internal static string RequestUriProcessor_CannotApplyEachOnSingleEntities(object p0) => TextRes.GetString(nameof (RequestUriProcessor_CannotApplyEachOnSingleEntities), p0);

    internal static string RequestUriProcessor_FilterPathSegmentSyntaxError => TextRes.GetString(nameof (RequestUriProcessor_FilterPathSegmentSyntaxError));

    internal static string RequestUriProcessor_NoNavigationSourceFound(object p0) => TextRes.GetString(nameof (RequestUriProcessor_NoNavigationSourceFound), p0);

    internal static string RequestUriProcessor_OnlySingleOperationCanFollowEachPathSegment => TextRes.GetString(nameof (RequestUriProcessor_OnlySingleOperationCanFollowEachPathSegment));

    internal static string RequestUriProcessor_EmptySegmentInRequestUrl => TextRes.GetString(nameof (RequestUriProcessor_EmptySegmentInRequestUrl));

    internal static string RequestUriProcessor_SyntaxError => TextRes.GetString(nameof (RequestUriProcessor_SyntaxError));

    internal static string RequestUriProcessor_CountOnRoot => TextRes.GetString(nameof (RequestUriProcessor_CountOnRoot));

    internal static string RequestUriProcessor_FilterOnRoot => TextRes.GetString(nameof (RequestUriProcessor_FilterOnRoot));

    internal static string RequestUriProcessor_EachOnRoot => TextRes.GetString(nameof (RequestUriProcessor_EachOnRoot));

    internal static string RequestUriProcessor_RefOnRoot => TextRes.GetString(nameof (RequestUriProcessor_RefOnRoot));

    internal static string RequestUriProcessor_MustBeLeafSegment(object p0) => TextRes.GetString(nameof (RequestUriProcessor_MustBeLeafSegment), p0);

    internal static string RequestUriProcessor_LinkSegmentMustBeFollowedByEntitySegment(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (RequestUriProcessor_LinkSegmentMustBeFollowedByEntitySegment), p0, p1);
    }

    internal static string RequestUriProcessor_MissingSegmentAfterLink(object p0) => TextRes.GetString(nameof (RequestUriProcessor_MissingSegmentAfterLink), p0);

    internal static string RequestUriProcessor_CountNotSupported(object p0) => TextRes.GetString(nameof (RequestUriProcessor_CountNotSupported), p0);

    internal static string RequestUriProcessor_CannotQueryCollections(object p0) => TextRes.GetString(nameof (RequestUriProcessor_CannotQueryCollections), p0);

    internal static string RequestUriProcessor_SegmentDoesNotSupportKeyPredicates(object p0) => TextRes.GetString(nameof (RequestUriProcessor_SegmentDoesNotSupportKeyPredicates), p0);

    internal static string RequestUriProcessor_ValueSegmentAfterScalarPropertySegment(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (RequestUriProcessor_ValueSegmentAfterScalarPropertySegment), p0, p1);
    }

    internal static string RequestUriProcessor_InvalidTypeIdentifier_UnrelatedType(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (RequestUriProcessor_InvalidTypeIdentifier_UnrelatedType), p0, p1);
    }

    internal static string OpenNavigationPropertiesNotSupportedOnOpenTypes(object p0) => TextRes.GetString(nameof (OpenNavigationPropertiesNotSupportedOnOpenTypes), p0);

    internal static string BadRequest_ResourceCanBeCrossReferencedOnlyForBindOperation => TextRes.GetString(nameof (BadRequest_ResourceCanBeCrossReferencedOnlyForBindOperation));

    internal static string DataServiceConfiguration_ResponseVersionIsBiggerThanProtocolVersion(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (DataServiceConfiguration_ResponseVersionIsBiggerThanProtocolVersion), p0, p1);
    }

    internal static string BadRequest_KeyCountMismatch(object p0) => TextRes.GetString(nameof (BadRequest_KeyCountMismatch), p0);

    internal static string RequestUriProcessor_KeysMustBeNamed => TextRes.GetString(nameof (RequestUriProcessor_KeysMustBeNamed));

    internal static string RequestUriProcessor_ResourceNotFound(object p0) => TextRes.GetString(nameof (RequestUriProcessor_ResourceNotFound), p0);

    internal static string RequestUriProcessor_BatchedActionOnEntityCreatedInSameChangeset(object p0) => TextRes.GetString(nameof (RequestUriProcessor_BatchedActionOnEntityCreatedInSameChangeset), p0);

    internal static string RequestUriProcessor_Forbidden => TextRes.GetString(nameof (RequestUriProcessor_Forbidden));

    internal static string RequestUriProcessor_OperationSegmentBoundToANonEntityType => TextRes.GetString(nameof (RequestUriProcessor_OperationSegmentBoundToANonEntityType));

    internal static string RequestUriProcessor_NoBoundEscapeFunctionSupported(object p0) => TextRes.GetString(nameof (RequestUriProcessor_NoBoundEscapeFunctionSupported), p0);

    internal static string RequestUriProcessor_EscapeFunctionMustHaveOneStringParameter(object p0) => TextRes.GetString(nameof (RequestUriProcessor_EscapeFunctionMustHaveOneStringParameter), p0);

    internal static string General_InternalError(object p0) => TextRes.GetString(nameof (General_InternalError), p0);

    internal static string ExceptionUtils_CheckIntegerNotNegative(object p0) => TextRes.GetString(nameof (ExceptionUtils_CheckIntegerNotNegative), p0);

    internal static string ExceptionUtils_CheckIntegerPositive(object p0) => TextRes.GetString(nameof (ExceptionUtils_CheckIntegerPositive), p0);

    internal static string ExceptionUtils_CheckLongPositive(object p0) => TextRes.GetString(nameof (ExceptionUtils_CheckLongPositive), p0);

    internal static string ExceptionUtils_ArgumentStringNullOrEmpty => TextRes.GetString(nameof (ExceptionUtils_ArgumentStringNullOrEmpty));

    internal static string ExpressionToken_OnlyRefAllowWithStarInExpand => TextRes.GetString(nameof (ExpressionToken_OnlyRefAllowWithStarInExpand));

    internal static string ExpressionToken_NoPropAllowedAfterRef => TextRes.GetString(nameof (ExpressionToken_NoPropAllowedAfterRef));

    internal static string ExpressionToken_NoSegmentAllowedBeforeStarInExpand => TextRes.GetString(nameof (ExpressionToken_NoSegmentAllowedBeforeStarInExpand));

    internal static string ExpressionToken_IdentifierExpected(object p0) => TextRes.GetString(nameof (ExpressionToken_IdentifierExpected), p0);

    internal static string ExpressionLexer_UnterminatedStringLiteral(object p0, object p1) => TextRes.GetString(nameof (ExpressionLexer_UnterminatedStringLiteral), p0, p1);

    internal static string ExpressionLexer_InvalidCharacter(object p0, object p1, object p2) => TextRes.GetString(nameof (ExpressionLexer_InvalidCharacter), p0, p1, p2);

    internal static string ExpressionLexer_SyntaxError(object p0, object p1) => TextRes.GetString(nameof (ExpressionLexer_SyntaxError), p0, p1);

    internal static string ExpressionLexer_UnterminatedLiteral(object p0, object p1) => TextRes.GetString(nameof (ExpressionLexer_UnterminatedLiteral), p0, p1);

    internal static string ExpressionLexer_DigitExpected(object p0, object p1) => TextRes.GetString(nameof (ExpressionLexer_DigitExpected), p0, p1);

    internal static string ExpressionLexer_UnbalancedBracketExpression => TextRes.GetString(nameof (ExpressionLexer_UnbalancedBracketExpression));

    internal static string ExpressionLexer_InvalidNumericString(object p0) => TextRes.GetString(nameof (ExpressionLexer_InvalidNumericString), p0);

    internal static string ExpressionLexer_InvalidEscapeSequence(object p0, object p1, object p2) => TextRes.GetString(nameof (ExpressionLexer_InvalidEscapeSequence), p0, p1, p2);

    internal static string UriQueryExpressionParser_UnrecognizedLiteral(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return TextRes.GetString(nameof (UriQueryExpressionParser_UnrecognizedLiteral), p0, p1, p2, p3);
    }

    internal static string UriQueryExpressionParser_UnrecognizedLiteralWithReason(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4)
    {
      return TextRes.GetString(nameof (UriQueryExpressionParser_UnrecognizedLiteralWithReason), p0, p1, p2, p3, p4);
    }

    internal static string UriPrimitiveTypeParsers_FailedToParseTextToPrimitiveValue(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (UriPrimitiveTypeParsers_FailedToParseTextToPrimitiveValue), p0, p1);
    }

    internal static string UriPrimitiveTypeParsers_FailedToParseStringToGeography => TextRes.GetString(nameof (UriPrimitiveTypeParsers_FailedToParseStringToGeography));

    internal static string UriCustomTypeParsers_AddCustomUriTypeParserAlreadyExists => TextRes.GetString(nameof (UriCustomTypeParsers_AddCustomUriTypeParserAlreadyExists));

    internal static string UriCustomTypeParsers_AddCustomUriTypeParserEdmTypeExists(object p0) => TextRes.GetString(nameof (UriCustomTypeParsers_AddCustomUriTypeParserEdmTypeExists), p0);

    internal static string UriParserHelper_InvalidPrefixLiteral(object p0) => TextRes.GetString(nameof (UriParserHelper_InvalidPrefixLiteral), p0);

    internal static string CustomUriTypePrefixLiterals_AddCustomUriTypePrefixLiteralAlreadyExists(
      object p0)
    {
      return TextRes.GetString(nameof (CustomUriTypePrefixLiterals_AddCustomUriTypePrefixLiteralAlreadyExists), p0);
    }

    internal static string ValueParser_InvalidDuration(object p0) => TextRes.GetString(nameof (ValueParser_InvalidDuration), p0);

    internal static string PlatformHelper_DateTimeOffsetMustContainTimeZone(object p0) => TextRes.GetString(nameof (PlatformHelper_DateTimeOffsetMustContainTimeZone), p0);

    internal static string JsonReader_UnexpectedComma(object p0) => TextRes.GetString(nameof (JsonReader_UnexpectedComma), p0);

    internal static string JsonReader_ArrayClosureMismatch(object p0, object p1, object p2) => TextRes.GetString(nameof (JsonReader_ArrayClosureMismatch), p0, p1, p2);

    internal static string JsonReader_MultipleTopLevelValues => TextRes.GetString(nameof (JsonReader_MultipleTopLevelValues));

    internal static string JsonReader_EndOfInputWithOpenScope => TextRes.GetString(nameof (JsonReader_EndOfInputWithOpenScope));

    internal static string JsonReader_UnexpectedToken(object p0) => TextRes.GetString(nameof (JsonReader_UnexpectedToken), p0);

    internal static string JsonReader_UnrecognizedToken => TextRes.GetString(nameof (JsonReader_UnrecognizedToken));

    internal static string JsonReader_MissingColon(object p0) => TextRes.GetString(nameof (JsonReader_MissingColon), p0);

    internal static string JsonReader_UnrecognizedEscapeSequence(object p0) => TextRes.GetString(nameof (JsonReader_UnrecognizedEscapeSequence), p0);

    internal static string JsonReader_UnexpectedEndOfString => TextRes.GetString(nameof (JsonReader_UnexpectedEndOfString));

    internal static string JsonReader_InvalidNumberFormat(object p0) => TextRes.GetString(nameof (JsonReader_InvalidNumberFormat), p0);

    internal static string JsonReader_InvalidBinaryFormat(object p0) => TextRes.GetString(nameof (JsonReader_InvalidBinaryFormat), p0);

    internal static string JsonReader_MissingComma(object p0) => TextRes.GetString(nameof (JsonReader_MissingComma), p0);

    internal static string JsonReader_InvalidPropertyNameOrUnexpectedComma(object p0) => TextRes.GetString(nameof (JsonReader_InvalidPropertyNameOrUnexpectedComma), p0);

    internal static string JsonReader_MaxBufferReached => TextRes.GetString(nameof (JsonReader_MaxBufferReached));

    internal static string JsonReader_CannotAccessValueInStreamState => TextRes.GetString(nameof (JsonReader_CannotAccessValueInStreamState));

    internal static string JsonReader_CannotCallReadInStreamState => TextRes.GetString(nameof (JsonReader_CannotCallReadInStreamState));

    internal static string JsonReader_CannotCreateReadStream => TextRes.GetString(nameof (JsonReader_CannotCreateReadStream));

    internal static string JsonReader_CannotCreateTextReader => TextRes.GetString(nameof (JsonReader_CannotCreateTextReader));

    internal static string JsonReaderExtensions_UnexpectedNodeDetected(object p0, object p1) => TextRes.GetString(nameof (JsonReaderExtensions_UnexpectedNodeDetected), p0, p1);

    internal static string JsonReaderExtensions_UnexpectedNodeDetectedWithPropertyName(
      object p0,
      object p1,
      object p2)
    {
      return TextRes.GetString(nameof (JsonReaderExtensions_UnexpectedNodeDetectedWithPropertyName), p0, p1, p2);
    }

    internal static string JsonReaderExtensions_CannotReadPropertyValueAsString(
      object p0,
      object p1)
    {
      return TextRes.GetString(nameof (JsonReaderExtensions_CannotReadPropertyValueAsString), p0, p1);
    }

    internal static string JsonReaderExtensions_CannotReadValueAsString(object p0) => TextRes.GetString(nameof (JsonReaderExtensions_CannotReadValueAsString), p0);

    internal static string JsonReaderExtensions_CannotReadValueAsDouble(object p0) => TextRes.GetString(nameof (JsonReaderExtensions_CannotReadValueAsDouble), p0);

    internal static string JsonReaderExtensions_UnexpectedInstanceAnnotationName(object p0) => TextRes.GetString(nameof (JsonReaderExtensions_UnexpectedInstanceAnnotationName), p0);

    internal static string BufferUtils_InvalidBufferOrSize(object p0) => TextRes.GetString(nameof (BufferUtils_InvalidBufferOrSize), p0);

    internal static string ServiceProviderExtensions_NoServiceRegistered(object p0) => TextRes.GetString(nameof (ServiceProviderExtensions_NoServiceRegistered), p0);
  }
}
