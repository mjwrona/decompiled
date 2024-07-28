// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.SRClient
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Resources;

namespace Microsoft.Azure.NotificationHubs
{
  internal class SRClient
  {
    private static ResourceManager resourceManager;
    private static CultureInfo resourceCulture;

    private SRClient()
    {
    }

    internal static ResourceManager ResourceManager
    {
      get
      {
        if (SRClient.resourceManager == null)
          SRClient.resourceManager = new ResourceManager("Microsoft.Azure.NotificationHubs.SRClient", typeof (SRClient).Assembly);
        return SRClient.resourceManager;
      }
    }

    [GeneratedCode("StrictResXFileCodeGenerator", "4.0.0.0")]
    internal static CultureInfo Culture
    {
      get => SRClient.resourceCulture;
      set => SRClient.resourceCulture = value;
    }

    internal static string MessageEntityNotOpened => SRClient.ResourceManager.GetString(nameof (MessageEntityNotOpened), SRClient.Culture);

    internal static string MessageEntityDisposed => SRClient.ResourceManager.GetString(nameof (MessageEntityDisposed), SRClient.Culture);

    internal static string EventDataDisposed => SRClient.ResourceManager.GetString(nameof (EventDataDisposed), SRClient.Culture);

    internal static string MessageBodyConsumed => SRClient.ResourceManager.GetString(nameof (MessageBodyConsumed), SRClient.Culture);

    internal static string InvalidXmlFormat => SRClient.ResourceManager.GetString(nameof (InvalidXmlFormat), SRClient.Culture);

    internal static string BufferAlreadyReclaimed => SRClient.ResourceManager.GetString(nameof (BufferAlreadyReclaimed), SRClient.Culture);

    internal static string CannotSendReceivedMessage => SRClient.ResourceManager.GetString(nameof (CannotSendReceivedMessage), SRClient.Culture);

    internal static string IsolationLevelNotSupported => SRClient.ResourceManager.GetString(nameof (IsolationLevelNotSupported), SRClient.Culture);

    internal static string MessageBodyNull => SRClient.ResourceManager.GetString(nameof (MessageBodyNull), SRClient.Culture);

    internal static string ReadNotSupported => SRClient.ResourceManager.GetString(nameof (ReadNotSupported), SRClient.Culture);

    internal static string SeekNotSupported => SRClient.ResourceManager.GetString(nameof (SeekNotSupported), SRClient.Culture);

    internal static string ValueMustBeNonNegative => SRClient.ResourceManager.GetString(nameof (ValueMustBeNonNegative), SRClient.Culture);

    internal static string CannotSerializeMessageWithPartiallyConsumedBodyStream => SRClient.ResourceManager.GetString(nameof (CannotSerializeMessageWithPartiallyConsumedBodyStream), SRClient.Culture);

    internal static string FailedToSerializeEntireBodyStream => SRClient.ResourceManager.GetString(nameof (FailedToSerializeEntireBodyStream), SRClient.Culture);

    internal static string FailedToDeSerializeEntireBodyStream => SRClient.ResourceManager.GetString(nameof (FailedToDeSerializeEntireBodyStream), SRClient.Culture);

    internal static string CannotSerializeSessionStateWithPartiallyConsumedStream => SRClient.ResourceManager.GetString(nameof (CannotSerializeSessionStateWithPartiallyConsumedStream), SRClient.Culture);

    internal static string FailedToSerializeEntireSessionStateStream => SRClient.ResourceManager.GetString(nameof (FailedToSerializeEntireSessionStateStream), SRClient.Culture);

    internal static string FailedToDeSerializeEntireSessionStateStream => SRClient.ResourceManager.GetString(nameof (FailedToDeSerializeEntireSessionStateStream), SRClient.Culture);

    internal static string SbmpTransport => SRClient.ResourceManager.GetString(nameof (SbmpTransport), SRClient.Culture);

    internal static string MultipleResourceManagersNotSupported => SRClient.ResourceManager.GetString(nameof (MultipleResourceManagersNotSupported), SRClient.Culture);

    internal static string ServerDidNotReply => SRClient.ResourceManager.GetString(nameof (ServerDidNotReply), SRClient.Culture);

    internal static string CannotCreateClientOnSubQueue => SRClient.ResourceManager.GetString(nameof (CannotCreateClientOnSubQueue), SRClient.Culture);

    internal static string InvalidOperationOnSessionBrowser => SRClient.ResourceManager.GetString(nameof (InvalidOperationOnSessionBrowser), SRClient.Culture);

    internal static string InvalidBatchFlushInterval => SRClient.ResourceManager.GetString(nameof (InvalidBatchFlushInterval), SRClient.Culture);

    internal static string FilterExpressionTooComplex => SRClient.ResourceManager.GetString(nameof (FilterExpressionTooComplex), SRClient.Culture);

    internal static string PeekLockModeRequired => SRClient.ResourceManager.GetString(nameof (PeekLockModeRequired), SRClient.Culture);

    internal static string ActionMustBeProcessed => SRClient.ResourceManager.GetString(nameof (ActionMustBeProcessed), SRClient.Culture);

    internal static string FilterMustBeProcessed => SRClient.ResourceManager.GetString(nameof (FilterMustBeProcessed), SRClient.Culture);

    internal static string EntityClosedOrAborted => SRClient.ResourceManager.GetString(nameof (EntityClosedOrAborted), SRClient.Culture);

    internal static string MessagingCommunicationError => SRClient.ResourceManager.GetString(nameof (MessagingCommunicationError), SRClient.Culture);

    internal static string BatchManagerAborted => SRClient.ResourceManager.GetString(nameof (BatchManagerAborted), SRClient.Culture);

    internal static string CannotCreateMessageSessionForSubQueue => SRClient.ResourceManager.GetString(nameof (CannotCreateMessageSessionForSubQueue), SRClient.Culture);

    internal static string UseOverloadWithBaseAddress => SRClient.ResourceManager.GetString(nameof (UseOverloadWithBaseAddress), SRClient.Culture);

    internal static string RuleCreationActionRequiresFilterTemplate => SRClient.ResourceManager.GetString(nameof (RuleCreationActionRequiresFilterTemplate), SRClient.Culture);

    internal static string EmptyPropertyInCorrelationFilter => SRClient.ResourceManager.GetString(nameof (EmptyPropertyInCorrelationFilter), SRClient.Culture);

    internal static string InvalidRefcountedCommunicationObject => SRClient.ResourceManager.GetString(nameof (InvalidRefcountedCommunicationObject), SRClient.Culture);

    internal static string InvalidStateMachineRefcountedCommunicationObject => SRClient.ResourceManager.GetString(nameof (InvalidStateMachineRefcountedCommunicationObject), SRClient.Culture);

    internal static string ConnectionStatusBehavior => SRClient.ResourceManager.GetString(nameof (ConnectionStatusBehavior), SRClient.Culture);

    internal static string StreamClosed => SRClient.ResourceManager.GetString(nameof (StreamClosed), SRClient.Culture);

    internal static string InvalidCallFaultException => SRClient.ResourceManager.GetString(nameof (InvalidCallFaultException), SRClient.Culture);

    internal static string MaximumAttemptsExceeded => SRClient.ResourceManager.GetString(nameof (MaximumAttemptsExceeded), SRClient.Culture);

    internal static string ConnectFailed => SRClient.ResourceManager.GetString(nameof (ConnectFailed), SRClient.Culture);

    internal static string InvalidID => SRClient.ResourceManager.GetString(nameof (InvalidID), SRClient.Culture);

    internal static string EndpointNotFound => SRClient.ResourceManager.GetString(nameof (EndpointNotFound), SRClient.Culture);

    internal static string DuplicateConnectionID => SRClient.ResourceManager.GetString(nameof (DuplicateConnectionID), SRClient.Culture);

    internal static string InvalidBufferSize => SRClient.ResourceManager.GetString(nameof (InvalidBufferSize), SRClient.Culture);

    internal static string ListenerLengthArgumentOutOfRange => SRClient.ResourceManager.GetString(nameof (ListenerLengthArgumentOutOfRange), SRClient.Culture);

    internal static string UnrecognizedCredentialType => SRClient.ResourceManager.GetString(nameof (UnrecognizedCredentialType), SRClient.Culture);

    internal static string LockedMessageInfo => SRClient.ResourceManager.GetString(nameof (LockedMessageInfo), SRClient.Culture);

    internal static string ReadOnlyPolicy => SRClient.ResourceManager.GetString(nameof (ReadOnlyPolicy), SRClient.Culture);

    internal static string EndpointNotFoundFault => SRClient.ResourceManager.GetString(nameof (EndpointNotFoundFault), SRClient.Culture);

    internal static string DuplicateConnectionIDFault => SRClient.ResourceManager.GetString(nameof (DuplicateConnectionIDFault), SRClient.Culture);

    internal static string InvalidConfiguration => SRClient.ResourceManager.GetString(nameof (InvalidConfiguration), SRClient.Culture);

    internal static string TransportSecurity => SRClient.ResourceManager.GetString(nameof (TransportSecurity), SRClient.Culture);

    internal static string HTTPAuthTokenNotSupportedException => SRClient.ResourceManager.GetString(nameof (HTTPAuthTokenNotSupportedException), SRClient.Culture);

    internal static string UnexpectedFormat => SRClient.ResourceManager.GetString(nameof (UnexpectedFormat), SRClient.Culture);

    internal static string InvalidChannelType => SRClient.ResourceManager.GetString(nameof (InvalidChannelType), SRClient.Culture);

    internal static string IncompatibleChannelListener => SRClient.ResourceManager.GetString(nameof (IncompatibleChannelListener), SRClient.Culture);

    internal static string NullSAMLs => SRClient.ResourceManager.GetString(nameof (NullSAMLs), SRClient.Culture);

    internal static string STSURIFormat => SRClient.ResourceManager.GetString(nameof (STSURIFormat), SRClient.Culture);

    internal static string NullIssuerName => SRClient.ResourceManager.GetString(nameof (NullIssuerName), SRClient.Culture);

    internal static string NullIssuerSecret => SRClient.ResourceManager.GetString(nameof (NullIssuerSecret), SRClient.Culture);

    internal static string InvalidIssuerSecret => SRClient.ResourceManager.GetString(nameof (InvalidIssuerSecret), SRClient.Culture);

    internal static string TokenExpiresOn => SRClient.ResourceManager.GetString(nameof (TokenExpiresOn), SRClient.Culture);

    internal static string InvalidEncoding => SRClient.ResourceManager.GetString(nameof (InvalidEncoding), SRClient.Culture);

    internal static string UnsupportedEncodingType => SRClient.ResourceManager.GetString(nameof (UnsupportedEncodingType), SRClient.Culture);

    internal static string NullSimpleWebToken => SRClient.ResourceManager.GetString(nameof (NullSimpleWebToken), SRClient.Culture);

    internal static string ArgumentOutOfRangeLessThanOne => SRClient.ResourceManager.GetString(nameof (ArgumentOutOfRangeLessThanOne), SRClient.Culture);

    internal static string BeginGetWebTokenNotSupported => SRClient.ResourceManager.GetString(nameof (BeginGetWebTokenNotSupported), SRClient.Culture);

    internal static string NullAppliesTo => SRClient.ResourceManager.GetString(nameof (NullAppliesTo), SRClient.Culture);

    internal static string NullHostname => SRClient.ResourceManager.GetString(nameof (NullHostname), SRClient.Culture);

    internal static string TimeoutExceeded => SRClient.ResourceManager.GetString(nameof (TimeoutExceeded), SRClient.Culture);

    internal static string AlreadyRunning => SRClient.ResourceManager.GetString(nameof (AlreadyRunning), SRClient.Culture);

    internal static string RelayCertificateNotFound => SRClient.ResourceManager.GetString(nameof (RelayCertificateNotFound), SRClient.Culture);

    internal static string MessageSizeExceeded => SRClient.ResourceManager.GetString(nameof (MessageSizeExceeded), SRClient.Culture);

    internal static string ExpectedBytesNotRead => SRClient.ResourceManager.GetString(nameof (ExpectedBytesNotRead), SRClient.Culture);

    internal static string HTTPConnectivityMode => SRClient.ResourceManager.GetString(nameof (HTTPConnectivityMode), SRClient.Culture);

    internal static string DownstreamConnection => SRClient.ResourceManager.GetString(nameof (DownstreamConnection), SRClient.Culture);

    internal static string ConnectionTermination => SRClient.ResourceManager.GetString(nameof (ConnectionTermination), SRClient.Culture);

    internal static string UpstreamConnection => SRClient.ResourceManager.GetString(nameof (UpstreamConnection), SRClient.Culture);

    internal static string URIEndpoint => SRClient.ResourceManager.GetString(nameof (URIEndpoint), SRClient.Culture);

    internal static string FaultyEndpointResponse => SRClient.ResourceManager.GetString(nameof (FaultyEndpointResponse), SRClient.Culture);

    internal static string FactoryEndpoint => SRClient.ResourceManager.GetString(nameof (FactoryEndpoint), SRClient.Culture);

    internal static string WebStreamShutdown => SRClient.ResourceManager.GetString(nameof (WebStreamShutdown), SRClient.Culture);

    internal static string ValueVisibility => SRClient.ResourceManager.GetString(nameof (ValueVisibility), SRClient.Culture);

    internal static string MultipleConnectionModeAssertions => SRClient.ResourceManager.GetString(nameof (MultipleConnectionModeAssertions), SRClient.Culture);

    internal static string ITokenProviderType => SRClient.ResourceManager.GetString(nameof (ITokenProviderType), SRClient.Culture);

    internal static string EnabledAutoFlowCreditIssuing => SRClient.ResourceManager.GetString(nameof (EnabledAutoFlowCreditIssuing), SRClient.Culture);

    internal static string MismatchedListSizeEncodedValueLength => SRClient.ResourceManager.GetString(nameof (MismatchedListSizeEncodedValueLength), SRClient.Culture);

    internal static string MessageListenerAlreadyRegistered => SRClient.ResourceManager.GetString(nameof (MessageListenerAlreadyRegistered), SRClient.Culture);

    internal static string CreditListenerAlreadyRegistered => SRClient.ResourceManager.GetString(nameof (CreditListenerAlreadyRegistered), SRClient.Culture);

    internal static string DispositionListenerAlreadyRegistered => SRClient.ResourceManager.GetString(nameof (DispositionListenerAlreadyRegistered), SRClient.Culture);

    internal static string DispositionListenerSetNotSupported => SRClient.ResourceManager.GetString(nameof (DispositionListenerSetNotSupported), SRClient.Culture);

    internal static string ServerCertificateAlreadySet => SRClient.ResourceManager.GetString(nameof (ServerCertificateAlreadySet), SRClient.Culture);

    internal static string ClientTargetHostAlreadySet => SRClient.ResourceManager.GetString(nameof (ClientTargetHostAlreadySet), SRClient.Culture);

    internal static string ClientTargetHostServerCertificateNotSet => SRClient.ResourceManager.GetString(nameof (ClientTargetHostServerCertificateNotSet), SRClient.Culture);

    internal static string TargetHostNotSet => SRClient.ResourceManager.GetString(nameof (TargetHostNotSet), SRClient.Culture);

    internal static string ServerCertificateNotSet => SRClient.ResourceManager.GetString(nameof (ServerCertificateNotSet), SRClient.Culture);

    internal static string AsyncResultInUse => SRClient.ResourceManager.GetString(nameof (AsyncResultInUse), SRClient.Culture);

    internal static string AsyncResultDifferent => SRClient.ResourceManager.GetString(nameof (AsyncResultDifferent), SRClient.Culture);

    internal static string AsyncResultNotInUse => SRClient.ResourceManager.GetString(nameof (AsyncResultNotInUse), SRClient.Culture);

    internal static string NullResourceDescription => SRClient.ResourceManager.GetString(nameof (NullResourceDescription), SRClient.Culture);

    internal static string NullResourceName => SRClient.ResourceManager.GetString(nameof (NullResourceName), SRClient.Culture);

    internal static string NullServiceNameSpace => SRClient.ResourceManager.GetString(nameof (NullServiceNameSpace), SRClient.Culture);

    internal static string PathSegmentASCIICharacters => SRClient.ResourceManager.GetString(nameof (PathSegmentASCIICharacters), SRClient.Culture);

    internal static string SystemTrackerHeaderMissing => SRClient.ResourceManager.GetString(nameof (SystemTrackerHeaderMissing), SRClient.Culture);

    internal static string SystemTrackerPropertyMissing => SRClient.ResourceManager.GetString(nameof (SystemTrackerPropertyMissing), SRClient.Culture);

    internal static string TrackingIDHeaderMissing => SRClient.ResourceManager.GetString(nameof (TrackingIDHeaderMissing), SRClient.Culture);

    internal static string TrackingIDPropertyMissing => SRClient.ResourceManager.GetString(nameof (TrackingIDPropertyMissing), SRClient.Culture);

    internal static string MessageLockLost => SRClient.ResourceManager.GetString(nameof (MessageLockLost), SRClient.Culture);

    internal static string SessionLockExpiredOnMessageSession => SRClient.ResourceManager.GetString(nameof (SessionLockExpiredOnMessageSession), SRClient.Culture);

    internal static string IOThreadTimerCannotAcceptMaxTimeSpan => SRClient.ResourceManager.GetString(nameof (IOThreadTimerCannotAcceptMaxTimeSpan), SRClient.Culture);

    internal static string ConnectFailedCommunicationException => SRClient.ResourceManager.GetString(nameof (ConnectFailedCommunicationException), SRClient.Culture);

    internal static string CreateSessionOnClosingConnection => SRClient.ResourceManager.GetString(nameof (CreateSessionOnClosingConnection), SRClient.Culture);

    internal static string ErroConvertingToChar => SRClient.ResourceManager.GetString(nameof (ErroConvertingToChar), SRClient.Culture);

    internal static string InvalidLengthofReceivedContent => SRClient.ResourceManager.GetString(nameof (InvalidLengthofReceivedContent), SRClient.Culture);

    internal static string InvalidReceivedContent => SRClient.ResourceManager.GetString(nameof (InvalidReceivedContent), SRClient.Culture);

    internal static string InvalidReceivedSessionId => SRClient.ResourceManager.GetString(nameof (InvalidReceivedSessionId), SRClient.Culture);

    internal static string MoreThanOneAddressCandidate => SRClient.ResourceManager.GetString(nameof (MoreThanOneAddressCandidate), SRClient.Culture);

    internal static string MoreThanOneIPEndPoint => SRClient.ResourceManager.GetString(nameof (MoreThanOneIPEndPoint), SRClient.Culture);

    internal static string NotSupportedTypeofChannel => SRClient.ResourceManager.GetString(nameof (NotSupportedTypeofChannel), SRClient.Culture);

    internal static string NoValidHostAddress => SRClient.ResourceManager.GetString(nameof (NoValidHostAddress), SRClient.Culture);

    internal static string NullRawDataInToken => SRClient.ResourceManager.GetString(nameof (NullRawDataInToken), SRClient.Culture);

    internal static string NullRoot => SRClient.ResourceManager.GetString(nameof (NullRoot), SRClient.Culture);

    internal static string OnMessageAlreadyCalled => SRClient.ResourceManager.GetString(nameof (OnMessageAlreadyCalled), SRClient.Culture);

    internal static string InternalServerError => SRClient.ResourceManager.GetString(nameof (InternalServerError), SRClient.Culture);

    internal static string InvalidCombinationOfManageRight => SRClient.ResourceManager.GetString(nameof (InvalidCombinationOfManageRight), SRClient.Culture);

    internal static string CannotHaveDuplicateAccessRights => SRClient.ResourceManager.GetString(nameof (CannotHaveDuplicateAccessRights), SRClient.Culture);

    internal static string NotSupportedXMLFormatAsBodyTemplate => SRClient.ResourceManager.GetString(nameof (NotSupportedXMLFormatAsBodyTemplate), SRClient.Culture);

    internal static string NotSupportedXMLFormatAsPayload => SRClient.ResourceManager.GetString(nameof (NotSupportedXMLFormatAsPayload), SRClient.Culture);

    internal static string ConnectionStringWithInvalidScheme => SRClient.ResourceManager.GetString(nameof (ConnectionStringWithInvalidScheme), SRClient.Culture);

    internal static string SetTokenScopeNotSupported => SRClient.ResourceManager.GetString(nameof (SetTokenScopeNotSupported), SRClient.Culture);

    internal static string CannotHaveDuplicateSAARule => SRClient.ResourceManager.GetString(nameof (CannotHaveDuplicateSAARule), SRClient.Culture);

    internal static string PairedNamespaceOnlyCallOnce => SRClient.ResourceManager.GetString(nameof (PairedNamespaceOnlyCallOnce), SRClient.Culture);

    internal static string PairedNamespaceMessagingFactoryAlreadyPaired => SRClient.ResourceManager.GetString(nameof (PairedNamespaceMessagingFactoryAlreadyPaired), SRClient.Culture);

    internal static string PairedNamespaceMessagingFactoryInOptionsAlreadyPaired => SRClient.ResourceManager.GetString(nameof (PairedNamespaceMessagingFactoryInOptionsAlreadyPaired), SRClient.Culture);

    internal static string PairedNamespaceMessagingFactoyCannotBeChanged => SRClient.ResourceManager.GetString(nameof (PairedNamespaceMessagingFactoyCannotBeChanged), SRClient.Culture);

    internal static string BacklogDeadletterDescriptionNoQueuePath => SRClient.ResourceManager.GetString(nameof (BacklogDeadletterDescriptionNoQueuePath), SRClient.Culture);

    internal static string BacklogDeadletterReasonNoQueuePath => SRClient.ResourceManager.GetString(nameof (BacklogDeadletterReasonNoQueuePath), SRClient.Culture);

    internal static string BacklogDeadletterReasonNotRetryable => SRClient.ResourceManager.GetString(nameof (BacklogDeadletterReasonNotRetryable), SRClient.Culture);

    internal static string SendAvailabilityNoTransferQueuesCreated => SRClient.ResourceManager.GetString(nameof (SendAvailabilityNoTransferQueuesCreated), SRClient.Culture);

    internal static string PairedNamespaceInvalidBacklogQueueCount => SRClient.ResourceManager.GetString(nameof (PairedNamespaceInvalidBacklogQueueCount), SRClient.Culture);

    internal static string PairedNamespacePrimaryAndSecondaryEqual => SRClient.ResourceManager.GetString(nameof (PairedNamespacePrimaryAndSecondaryEqual), SRClient.Culture);

    internal static string PairedNamespaceValidTimespanRange => SRClient.ResourceManager.GetString(nameof (PairedNamespaceValidTimespanRange), SRClient.Culture);

    internal static string PairedNamespacePrimaryEntityUnreachable => SRClient.ResourceManager.GetString(nameof (PairedNamespacePrimaryEntityUnreachable), SRClient.Culture);

    internal static string NullAsString => SRClient.ResourceManager.GetString(nameof (NullAsString), SRClient.Culture);

    internal static string PairedNamespacePropertyExtractionDlqReason => SRClient.ResourceManager.GetString(nameof (PairedNamespacePropertyExtractionDlqReason), SRClient.Culture);

    internal static string BodyIsNotSupportedExpression => SRClient.ResourceManager.GetString(nameof (BodyIsNotSupportedExpression), SRClient.Culture);

    internal static string ApnsCertificateExpired => SRClient.ResourceManager.GetString(nameof (ApnsCertificateExpired), SRClient.Culture);

    internal static string ApnsCertificateNotValid => SRClient.ResourceManager.GetString(nameof (ApnsCertificateNotValid), SRClient.Culture);

    internal static string ApnsCertificatePrivatekeyMissing => SRClient.ResourceManager.GetString(nameof (ApnsCertificatePrivatekeyMissing), SRClient.Culture);

    internal static string ApnsEndpointNotAllowed => SRClient.ResourceManager.GetString(nameof (ApnsEndpointNotAllowed), SRClient.Culture);

    internal static string ApnsPropertiesNotSpecified => SRClient.ResourceManager.GetString(nameof (ApnsPropertiesNotSpecified), SRClient.Culture);

    internal static string ApnsRequiredPropertiesError => SRClient.ResourceManager.GetString(nameof (ApnsRequiredPropertiesError), SRClient.Culture);

    internal static string CannotSpecifyExpirationTime => SRClient.ResourceManager.GetString(nameof (CannotSpecifyExpirationTime), SRClient.Culture);

    internal static string ChannelUriNullOrEmpty => SRClient.ResourceManager.GetString(nameof (ChannelUriNullOrEmpty), SRClient.Culture);

    internal static string DeviceTokenHexaDecimalDigitError => SRClient.ResourceManager.GetString(nameof (DeviceTokenHexaDecimalDigitError), SRClient.Culture);

    internal static string DeviceTokenIsEmpty => SRClient.ResourceManager.GetString(nameof (DeviceTokenIsEmpty), SRClient.Culture);

    internal static string EmptyExpiryValue => SRClient.ResourceManager.GetString(nameof (EmptyExpiryValue), SRClient.Culture);

    internal static string ExpiryDeserializationError => SRClient.ResourceManager.GetString(nameof (ExpiryDeserializationError), SRClient.Culture);

    internal static string FailedToDeserializeBodyTemplate => SRClient.ResourceManager.GetString(nameof (FailedToDeserializeBodyTemplate), SRClient.Culture);

    internal static string GcmEndpointNotSpecified => SRClient.ResourceManager.GetString(nameof (GcmEndpointNotSpecified), SRClient.Culture);

    internal static string GCMRegistrationInvalidId => SRClient.ResourceManager.GetString(nameof (GCMRegistrationInvalidId), SRClient.Culture);

    internal static string GcmRequiredProperties => SRClient.ResourceManager.GetString(nameof (GcmRequiredProperties), SRClient.Culture);

    internal static string GoogleApiKeyNotSpecified => SRClient.ResourceManager.GetString(nameof (GoogleApiKeyNotSpecified), SRClient.Culture);

    internal static string InvalidGcmEndpoint => SRClient.ResourceManager.GetString(nameof (InvalidGcmEndpoint), SRClient.Culture);

    internal static string NokiaXEndpointNotSpecified => SRClient.ResourceManager.GetString(nameof (NokiaXEndpointNotSpecified), SRClient.Culture);

    internal static string NokiaXRegistrationInvalidId => SRClient.ResourceManager.GetString(nameof (NokiaXRegistrationInvalidId), SRClient.Culture);

    internal static string NokiaXRequiredProperties => SRClient.ResourceManager.GetString(nameof (NokiaXRequiredProperties), SRClient.Culture);

    internal static string NokiaXAuthorizationKeyNotSpecified => SRClient.ResourceManager.GetString(nameof (NokiaXAuthorizationKeyNotSpecified), SRClient.Culture);

    internal static string InvalidNokiaXEndpoint => SRClient.ResourceManager.GetString(nameof (InvalidNokiaXEndpoint), SRClient.Culture);

    internal static string BaiduEndpointNotSpecified => SRClient.ResourceManager.GetString(nameof (BaiduEndpointNotSpecified), SRClient.Culture);

    internal static string BaiduRegistrationInvalidId => SRClient.ResourceManager.GetString(nameof (BaiduRegistrationInvalidId), SRClient.Culture);

    internal static string BaiduRequiredProperties => SRClient.ResourceManager.GetString(nameof (BaiduRequiredProperties), SRClient.Culture);

    internal static string BaiduApiKeyNotSpecified => SRClient.ResourceManager.GetString(nameof (BaiduApiKeyNotSpecified), SRClient.Culture);

    internal static string InvalidBaiduEndpoint => SRClient.ResourceManager.GetString(nameof (InvalidBaiduEndpoint), SRClient.Culture);

    internal static string InvalidWindowsLiveEndpoint => SRClient.ResourceManager.GetString(nameof (InvalidWindowsLiveEndpoint), SRClient.Culture);

    internal static string PackageSidAndSecretKeyAreRequired => SRClient.ResourceManager.GetString(nameof (PackageSidAndSecretKeyAreRequired), SRClient.Culture);

    internal static string PackageSidOrSecretKeyInvalid => SRClient.ResourceManager.GetString(nameof (PackageSidOrSecretKeyInvalid), SRClient.Culture);

    internal static string PropTokenNotAllowedInCompositeExpr => SRClient.ResourceManager.GetString(nameof (PropTokenNotAllowedInCompositeExpr), SRClient.Culture);

    internal static string NotSupportedXMLFormatAsBodyTemplateForMpns => SRClient.ResourceManager.GetString(nameof (NotSupportedXMLFormatAsBodyTemplateForMpns), SRClient.Culture);

    internal static string NotSupportedXMLFormatAsPayloadForMpns => SRClient.ResourceManager.GetString(nameof (NotSupportedXMLFormatAsPayloadForMpns), SRClient.Culture);

    internal static string InvalidMpnsCertificate => SRClient.ResourceManager.GetString(nameof (InvalidMpnsCertificate), SRClient.Culture);

    internal static string MpnsCertificateExpired => SRClient.ResourceManager.GetString(nameof (MpnsCertificateExpired), SRClient.Culture);

    internal static string MpnsCertificatePrivatekeyMissing => SRClient.ResourceManager.GetString(nameof (MpnsCertificatePrivatekeyMissing), SRClient.Culture);

    internal static string MpnsInvalidPropeties => SRClient.ResourceManager.GetString(nameof (MpnsInvalidPropeties), SRClient.Culture);

    internal static string MpnsRequiredPropertiesError => SRClient.ResourceManager.GetString(nameof (MpnsRequiredPropertiesError), SRClient.Culture);

    internal static string PartitionKeyMustBeEqualsToNonNullSessionId => SRClient.ResourceManager.GetString(nameof (PartitionKeyMustBeEqualsToNonNullSessionId), SRClient.Culture);

    internal static string UnsupportedBatchingDistinctPartitionKey => SRClient.ResourceManager.GetString(nameof (UnsupportedBatchingDistinctPartitionKey), SRClient.Culture);

    internal static string UnsupportedDeDupBatchingDistinctPartitionKey => SRClient.ResourceManager.GetString(nameof (UnsupportedDeDupBatchingDistinctPartitionKey), SRClient.Culture);

    internal static string TransactionPartitionKeyMissing => SRClient.ResourceManager.GetString(nameof (TransactionPartitionKeyMissing), SRClient.Culture);

    internal static string PartitionedEntityViaSenderNeedsViaPatitionKey => SRClient.ResourceManager.GetString(nameof (PartitionedEntityViaSenderNeedsViaPatitionKey), SRClient.Culture);

    internal static string MessagingPartitioningUnsupportedBatchingLockTockens => SRClient.ResourceManager.GetString(nameof (MessagingPartitioningUnsupportedBatchingLockTockens), SRClient.Culture);

    internal static string UnsupportedBatchingSequenceNumbersForDistinctPartitions => SRClient.ResourceManager.GetString(nameof (UnsupportedBatchingSequenceNumbersForDistinctPartitions), SRClient.Culture);

    internal static string MessagingPartitioningInvalidOperation => SRClient.ResourceManager.GetString(nameof (MessagingPartitioningInvalidOperation), SRClient.Culture);

    internal static string InvalidPayLoadFormat => SRClient.ResourceManager.GetString(nameof (InvalidPayLoadFormat), SRClient.Culture);

    internal static string SessionHandlerAlreadyRegistered => SRClient.ResourceManager.GetString(nameof (SessionHandlerAlreadyRegistered), SRClient.Culture);

    internal static string InvalidAdmAuthTokenUrl => SRClient.ResourceManager.GetString(nameof (InvalidAdmAuthTokenUrl), SRClient.Culture);

    internal static string ErrorNoCotent => SRClient.ResourceManager.GetString(nameof (ErrorNoCotent), SRClient.Culture);

    internal static string EventHubPathMismatch => SRClient.ResourceManager.GetString(nameof (EventHubPathMismatch), SRClient.Culture);

    internal static string CannotCreateReceiverWithDispatcher => SRClient.ResourceManager.GetString(nameof (CannotCreateReceiverWithDispatcher), SRClient.Culture);

    internal static string AdmRegistrationIdInvalid => SRClient.ResourceManager.GetString(nameof (AdmRegistrationIdInvalid), SRClient.Culture);

    internal static string InvalidAdmSendUrlTemplate => SRClient.ResourceManager.GetString(nameof (InvalidAdmSendUrlTemplate), SRClient.Culture);

    internal static string ReceiveContextNull => SRClient.ResourceManager.GetString(nameof (ReceiveContextNull), SRClient.Culture);

    internal static string TokenAudience => SRClient.ResourceManager.GetString(nameof (TokenAudience), SRClient.Culture);

    internal static string ObjectIsReadOnly => SRClient.ResourceManager.GetString(nameof (ObjectIsReadOnly), SRClient.Culture);

    internal static string CannotCheckpointWithCurrentConsumerGroup => SRClient.ResourceManager.GetString(nameof (CannotCheckpointWithCurrentConsumerGroup), SRClient.Culture);

    internal static string PartitionKeyMustBeEqualsToNonNullPublisher => SRClient.ResourceManager.GetString(nameof (PartitionKeyMustBeEqualsToNonNullPublisher), SRClient.Culture);

    internal static string PublisherMustBeEqualsToNonNullSessionId => SRClient.ResourceManager.GetString(nameof (PublisherMustBeEqualsToNonNullSessionId), SRClient.Culture);

    internal static string NotificationHubOperationNotAllowedForSKU => SRClient.ResourceManager.GetString(nameof (NotificationHubOperationNotAllowedForSKU), SRClient.Culture);

    internal static string UnknownRegistrationDescriptionType => SRClient.ResourceManager.GetString(nameof (UnknownRegistrationDescriptionType), SRClient.Culture);

    internal static string ExpressionHashInComposite => SRClient.ResourceManager.GetString(nameof (ExpressionHashInComposite), SRClient.Culture);

    internal static string EmptyPriorityValue => SRClient.ResourceManager.GetString(nameof (EmptyPriorityValue), SRClient.Culture);

    internal static string PriorityDeserializationError => SRClient.ResourceManager.GetString(nameof (PriorityDeserializationError), SRClient.Culture);

    internal static string CommunicationObjectFaulted(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (CommunicationObjectFaulted), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string EntityNameLengthExceedsLimit(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (EntityNameLengthExceedsLimit), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string TemplateNameLengthExceedsLimit(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (TemplateNameLengthExceedsLimit), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string LockTimeExceedsMaximumAllowed(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (LockTimeExceedsMaximumAllowed), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string DuplicateHistoryExpiryTimeExceedsMaximumAllowed(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (DuplicateHistoryExpiryTimeExceedsMaximumAllowed), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string SQLSyntaxError(object param0, object param1, object param2) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (SQLSyntaxError), SRClient.Culture), new object[3]
    {
      param0,
      param1,
      param2
    });

    internal static string SQLSyntaxErrorDetailed(
      object param0,
      object param1,
      object param2,
      object param3)
    {
      return string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (SQLSyntaxErrorDetailed), SRClient.Culture), param0, param1, param2, param3);
    }

    internal static string InvalidUriScheme(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (InvalidUriScheme), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string BufferedOutputStreamQuotaExceeded(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (BufferedOutputStreamQuotaExceeded), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string MessagingEntityAlreadyExists(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (MessagingEntityAlreadyExists), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string MessagingEntityCouldNotBeFound(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (MessagingEntityCouldNotBeFound), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string MessagingEndpointCommunicationError(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (MessagingEndpointCommunicationError), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string ChannelTypeNotSupported(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (ChannelTypeNotSupported), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string ConfigInvalidBindingConfigurationName(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (ConfigInvalidBindingConfigurationName), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string NoAddressesFound(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (NoAddressesFound), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string CannotUseSameMessageInstanceInMultipleOperations(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (CannotUseSameMessageInstanceInMultipleOperations), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string TooManyMessageProperties(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (TooManyMessageProperties), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string ExceededMessagePropertySizeLimit(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (ExceededMessagePropertySizeLimit), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string EntityNameNotFound(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (EntityNameNotFound), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string FailedToSerializeUnsupportedType(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (FailedToSerializeUnsupportedType), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string FailedToDeserializeUnsupportedProperty(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (FailedToDeserializeUnsupportedProperty), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string OverflowWhenAddingException(object param0, object param1, object param2) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (OverflowWhenAddingException), SRClient.Culture), new object[3]
    {
      param0,
      param1,
      param2
    });

    internal static string MessageAttributeSetMethodNotAccessible(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (MessageAttributeSetMethodNotAccessible), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string MessageAttributeGetMethodNotAccessible(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (MessageAttributeGetMethodNotAccessible), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string MessageGetPropertyNotFound(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (MessageGetPropertyNotFound), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string SqlFilterActionStatmentTooLong(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (SqlFilterActionStatmentTooLong), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string FilterActionTooManyStatements(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (FilterActionTooManyStatements), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string NoCorrelationForChannelMessageId(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (NoCorrelationForChannelMessageId), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string NoCorrelationResponseForChannelMessageId(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (NoCorrelationResponseForChannelMessageId), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string SentCorrelationMessage(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (SentCorrelationMessage), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string ReceivedCorrelationMessage(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (ReceivedCorrelationMessage), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string OperationRequestTimedOut(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (OperationRequestTimedOut), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string QueueProvisioningError(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (QueueProvisioningError), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string QueueUnProvisioningError(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (QueueUnProvisioningError), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string SubscriptionProvisioningError(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (SubscriptionProvisioningError), SRClient.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string TopicProvisioningError(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (TopicProvisioningError), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string TopicUnProvisioningError(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (TopicUnProvisioningError), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string IncompatibleQueueExport(object param0, object param1, object param2) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (IncompatibleQueueExport), SRClient.Culture), new object[3]
    {
      param0,
      param1,
      param2
    });

    internal static string IncompatibleTopicExport(object param0, object param1, object param2) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (IncompatibleTopicExport), SRClient.Culture), new object[3]
    {
      param0,
      param1,
      param2
    });

    internal static string InvalidSubQueueNameString(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (InvalidSubQueueNameString), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string CannotConvertFilterAction(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (CannotConvertFilterAction), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string CannotConvertFilterExpression(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (CannotConvertFilterExpression), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string InvalidAddressPath(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (InvalidAddressPath), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string InvalidEntityNameFormatWithSlash(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (InvalidEntityNameFormatWithSlash), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string InvalidCharacterInEntityName(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (InvalidCharacterInEntityName), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string ArgumentOutOfRange(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (ArgumentOutOfRange), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string SqlFilterStatmentTooLong(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (SqlFilterStatmentTooLong), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string MessageIdIsNullOrEmptyOrOverMaxValue(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (MessageIdIsNullOrEmptyOrOverMaxValue), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string SessionIdIsOverMaxValue(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (SessionIdIsOverMaxValue), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string InvalidManagementEntityType(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (InvalidManagementEntityType), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string TrackableExceptionMessageFormat(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (TrackableExceptionMessageFormat), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string TrackingIdAndTimestampFormat(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (TrackingIdAndTimestampFormat), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string SqlFilterActionCannotRemoveSystemProperty(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (SqlFilterActionCannotRemoveSystemProperty), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string FilterScopeNotSupported(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (FilterScopeNotSupported), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string NotSupportedCompatibilityLevel(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (NotSupportedCompatibilityLevel), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string MessageSetPropertyNotFound(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (MessageSetPropertyNotFound), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string SqlFilterReservedKeyword(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (SqlFilterReservedKeyword), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string DelimitedIdentifierNotTerminated(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (DelimitedIdentifierNotTerminated), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string StringLiteralNotTerminated(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (StringLiteralNotTerminated), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string FilterFunctionIncorrectNumberOfArguments(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (FilterFunctionIncorrectNumberOfArguments), SRClient.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string FilterUnknownFunctionName(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (FilterUnknownFunctionName), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string StringIsTooLong(object param0, object param1, object param2) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (StringIsTooLong), SRClient.Culture), new object[3]
    {
      param0,
      param1,
      param2
    });

    internal static string InvalidCharactersInEntityName(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (InvalidCharactersInEntityName), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string PropertyIsNullOrEmpty(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (PropertyIsNullOrEmpty), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string MessagingEntityIsDisabledException(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (MessagingEntityIsDisabledException), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string PropertyReferenceUsedWithoutInitializes(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (PropertyReferenceUsedWithoutInitializes), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string X509InUnTrustedStore(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (X509InUnTrustedStore), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string X509InvalidUsageTime(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (X509InvalidUsageTime), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string X509CRLCheckFailed(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (X509CRLCheckFailed), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string CannotFindTransactionResult(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (CannotFindTransactionResult), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string ExpectedTypeInvalidCastException(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (ExpectedTypeInvalidCastException), SRClient.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string InvalidElement(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (InvalidElement), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string UnsupportedChannelType(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (UnsupportedChannelType), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string UnsupportedConnectivityMode(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (UnsupportedConnectivityMode), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string MessageHeaderRetrieval(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (MessageHeaderRetrieval), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string ResponseHeaderRetrieval(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (ResponseHeaderRetrieval), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string XMLContentReadFault(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (XMLContentReadFault), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string IncorrectContentTypeFault(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (IncorrectContentTypeFault), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string UnableToReach(object param0, object param1, object param2) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (UnableToReach), SRClient.Culture), new object[3]
    {
      param0,
      param1,
      param2
    });

    internal static string MaxRedirectsExceeded(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (MaxRedirectsExceeded), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string InvalidDNSClaims(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (InvalidDNSClaims), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string BaseAddressScheme(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (BaseAddressScheme), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string UnsupportedAction(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (UnsupportedAction), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string MismatchServiceBusDomain(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (MismatchServiceBusDomain), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string UnsupportedServiceBusDomainPrefix(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (UnsupportedServiceBusDomainPrefix), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string UnexpectedSSL(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (UnexpectedSSL), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string InvalidFrameSize(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (InvalidFrameSize), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string FeatureNotSupported(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (FeatureNotSupported), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string BrokeredMessageApplicationProperties(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (BrokeredMessageApplicationProperties), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string NullEmptyRights(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (NullEmptyRights), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string UnsupportedRight(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (UnsupportedRight), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string UnsupportedGetClaim(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (UnsupportedGetClaim), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string InvalidSchemeValue(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (InvalidSchemeValue), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string InvalidServiceNameSpace(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (InvalidServiceNameSpace), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string InputURIPath(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (InputURIPath), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string UnexpedtedURIHostName(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (UnexpedtedURIHostName), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string URIServiceNameSpace(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (URIServiceNameSpace), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string OpenChannelFailed(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (OpenChannelFailed), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string ExtraParameterSpecifiedForSqlExpression(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (ExtraParameterSpecifiedForSqlExpression), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string ParameterNotSpecifiedForSqlExpression(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (ParameterNotSpecifiedForSqlExpression), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string BadUriFormat(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (BadUriFormat), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string NotSupportFrameCode(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (NotSupportFrameCode), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string MessagingEntityUpdateConflict(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (MessagingEntityUpdateConflict), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string NotSupportedPropertyType(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (NotSupportedPropertyType), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string AppSettingsConfigDuplicateSetting(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (AppSettingsConfigDuplicateSetting), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string AppSettingsConfigMissingSetting(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (AppSettingsConfigMissingSetting), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string AppSettingsConfigSettingInvalidKey(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (AppSettingsConfigSettingInvalidKey), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string AppSettingsConfigSettingInvalidValue(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (AppSettingsConfigSettingInvalidValue), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string AppSettingsCreateFactoryWithInvalidConnectionString(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (AppSettingsCreateFactoryWithInvalidConnectionString), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string AppSettingsCreateManagerWithInvalidConnectionString(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (AppSettingsCreateManagerWithInvalidConnectionString), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string AppSettingsConfigIncompleteSettingCombination(
      object param0,
      object param1)
    {
      return string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (AppSettingsConfigIncompleteSettingCombination), SRClient.Culture), new object[2]
      {
        param0,
        param1
      });
    }

    internal static string MessagingEntityMoved(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (MessagingEntityMoved), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string ArgumentInvalidCombination(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (ArgumentInvalidCombination), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string BrokeredMessageStreamNotCloneable(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (BrokeredMessageStreamNotCloneable), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string PropertyInvalidCombination(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (PropertyInvalidCombination), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string MessagingEntityIsDisabledForReceiveException(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (MessagingEntityIsDisabledForReceiveException), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string MessagingEntityIsDisabledForSendException(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (MessagingEntityIsDisabledForSendException), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string SqlSettingNotFound(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (SqlSettingNotFound), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string MaxConcurrentCallsMustBeGreaterThanZero(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (MaxConcurrentCallsMustBeGreaterThanZero), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string HttpServerAlreadyRunning(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (HttpServerAlreadyRunning), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string BacklogDeadletterDescriptionNotRetryable(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (BacklogDeadletterDescriptionNotRetryable), SRClient.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string PairedNamespacePropertyExtractionDlqDescription(
      object param0,
      object param1,
      object param2,
      object param3,
      object param4)
    {
      return string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (PairedNamespacePropertyExtractionDlqDescription), SRClient.Culture), param0, param1, param2, param3, param4);
    }

    internal static string ApnsCertificateNotUsable(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (ApnsCertificateNotUsable), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string InvalidToken(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (InvalidToken), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string LitteralMissing(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (LitteralMissing), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string MissingWNSHeader(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (MissingWNSHeader), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string PropertyLengthError(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (PropertyLengthError), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string PropertyNameError(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (PropertyNameError), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string TokenBeginError(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (TokenBeginError), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string UnsupportedChannelUri(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (UnsupportedChannelUri), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string UnsupportedExpression(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (UnsupportedExpression), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string WNSHeaderNullOrEmpty(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (WNSHeaderNullOrEmpty), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string FaultingPairedMessagingFactory(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (FaultingPairedMessagingFactory), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string MissingMpnsHeader(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (MissingMpnsHeader), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string MpnsCertificateError(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (MpnsCertificateError), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string MpnsHeaderIsNullOrEmpty(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (MpnsHeaderIsNullOrEmpty), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string UnknownApiVersion(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (UnknownApiVersion), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string PropertyOverMaxValue(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (PropertyOverMaxValue), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string SessionHandlerDoesNotHaveDefaultConstructor(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (SessionHandlerDoesNotHaveDefaultConstructor), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string SessionHandlerMissingInterfaces(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (SessionHandlerMissingInterfaces), SRClient.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string PropertyMustBeEqualOrLessThanOtherProperty(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (PropertyMustBeEqualOrLessThanOtherProperty), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string ValueMustBePositive(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (ValueMustBePositive), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string MessagingEntityGone(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (MessagingEntityGone), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string OnlyNPropertiesRequired(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (OnlyNPropertiesRequired), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string RequiredPropertiesNotSpecified(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (RequiredPropertiesNotSpecified), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string RequiredPropertyNotSpecified(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (RequiredPropertyNotSpecified), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string InvalidMethodWhilePeeking(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (InvalidMethodWhilePeeking), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string DominatingPropertyMustBeEqualsToNonNullDormantProperty(
      object param0,
      object param1)
    {
      return string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (DominatingPropertyMustBeEqualsToNonNullDormantProperty), SRClient.Culture), new object[2]
      {
        param0,
        param1
      });
    }

    internal static string InvalidEventHubCheckpointSettings(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (InvalidEventHubCheckpointSettings), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string EventHubUnsupportedOperation(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (EventHubUnsupportedOperation), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string EventHubUnsupportedTransport(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (EventHubUnsupportedTransport), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string MessagingEntityRequestConflict(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (MessagingEntityRequestConflict), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string PartitionInvalidPartitionKey(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (PartitionInvalidPartitionKey), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string CannotSendAnEmptyEvent(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (CannotSendAnEmptyEvent), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string ExpressionIsNotPositiveInteger(
      object param0,
      object param1,
      object param2)
    {
      return string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (ExpressionIsNotPositiveInteger), SRClient.Culture), new object[3]
      {
        param0,
        param1,
        param2
      });
    }

    internal static string ExpressionErrorParseDotFormat(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (ExpressionErrorParseDotFormat), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string ExpressionErrorParsePercentFormat(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (ExpressionErrorParsePercentFormat), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string ExpressionMissingClosingParentheses(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (ExpressionMissingClosingParentheses), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string ExpressionMissingDefaultEnd(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (ExpressionMissingDefaultEnd), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string ExpressionMissingProperty(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (ExpressionMissingProperty), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string ExpressionMissingOpenParentheses(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (ExpressionMissingOpenParentheses), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string ExpressionLiteralMissingClosingNotation(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (ExpressionLiteralMissingClosingNotation), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string ExpressionInvalidCompositionOperator(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (ExpressionInvalidCompositionOperator), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string PropertyTooLong(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (PropertyTooLong), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string PropertyNameIsBad(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (PropertyNameIsBad), SRClient.Culture), new object[1]
    {
      param0
    });

    internal static string ExpressionInvalidTokenType(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (ExpressionInvalidTokenType), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string ExpressionInvalidToken(object param0, object param1) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (ExpressionInvalidToken), SRClient.Culture), new object[2]
    {
      param0,
      param1
    });

    internal static string ExpressionMissingClosingParenthesesNoToken(object param0) => string.Format((IFormatProvider) SRClient.Culture, SRClient.ResourceManager.GetString(nameof (ExpressionMissingClosingParenthesesNoToken), SRClient.Culture), new object[1]
    {
      param0
    });
  }
}
