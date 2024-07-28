// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RMResources
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Documents
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class RMResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    internal RMResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (RMResources.resourceMan == null)
          RMResources.resourceMan = new ResourceManager("Microsoft.Azure.Documents.RMResources", typeof (RMResources).GetAssembly());
        return RMResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => RMResources.resourceCulture;
      set => RMResources.resourceCulture = value;
    }

    internal static string ApiTypeForbidden => RMResources.ResourceManager.GetString(nameof (ApiTypeForbidden), RMResources.resourceCulture);

    internal static string ArgumentRequired => RMResources.ResourceManager.GetString(nameof (ArgumentRequired), RMResources.resourceCulture);

    internal static string AutoScaleSettingChangeWithUserAuthIsDisallowed => RMResources.ResourceManager.GetString(nameof (AutoScaleSettingChangeWithUserAuthIsDisallowed), RMResources.resourceCulture);

    internal static string BadClientMongo => RMResources.ResourceManager.GetString(nameof (BadClientMongo), RMResources.resourceCulture);

    internal static string BadGateway => RMResources.ResourceManager.GetString(nameof (BadGateway), RMResources.resourceCulture);

    internal static string BadRequest => RMResources.ResourceManager.GetString(nameof (BadRequest), RMResources.resourceCulture);

    internal static string BadUrl => RMResources.ResourceManager.GetString(nameof (BadUrl), RMResources.resourceCulture);

    internal static string CannotOfflineWriteRegionWithNoReadRegions => RMResources.ResourceManager.GetString(nameof (CannotOfflineWriteRegionWithNoReadRegions), RMResources.resourceCulture);

    internal static string CannotSpecifyPKRangeForNonPartitionedResource => RMResources.ResourceManager.GetString(nameof (CannotSpecifyPKRangeForNonPartitionedResource), RMResources.resourceCulture);

    internal static string ChangeFeedOptionsStartTimeWithUnspecifiedDateTimeKind => RMResources.ResourceManager.GetString(nameof (ChangeFeedOptionsStartTimeWithUnspecifiedDateTimeKind), RMResources.resourceCulture);

    internal static string ChannelClosed => RMResources.ResourceManager.GetString(nameof (ChannelClosed), RMResources.resourceCulture);

    internal static string ChannelMultiplexerClosedTransportError => RMResources.ResourceManager.GetString(nameof (ChannelMultiplexerClosedTransportError), RMResources.resourceCulture);

    internal static string ChannelOpenFailedTransportError => RMResources.ResourceManager.GetString(nameof (ChannelOpenFailedTransportError), RMResources.resourceCulture);

    internal static string ChannelOpenTimeoutTransportError => RMResources.ResourceManager.GetString(nameof (ChannelOpenTimeoutTransportError), RMResources.resourceCulture);

    internal static string ClientCpuOverload => RMResources.ResourceManager.GetString(nameof (ClientCpuOverload), RMResources.resourceCulture);

    internal static string ClientUnavailable => RMResources.ResourceManager.GetString(nameof (ClientUnavailable), RMResources.resourceCulture);

    internal static string CollectionCreateTopologyConflict => RMResources.ResourceManager.GetString(nameof (CollectionCreateTopologyConflict), RMResources.resourceCulture);

    internal static string CollectionThroughputCannotBeMoreThan => RMResources.ResourceManager.GetString(nameof (CollectionThroughputCannotBeMoreThan), RMResources.resourceCulture);

    internal static string ConnectFailedTransportError => RMResources.ResourceManager.GetString(nameof (ConnectFailedTransportError), RMResources.resourceCulture);

    internal static string ConnectionBrokenTransportError => RMResources.ResourceManager.GetString(nameof (ConnectionBrokenTransportError), RMResources.resourceCulture);

    internal static string ConnectTimeoutTransportError => RMResources.ResourceManager.GetString(nameof (ConnectTimeoutTransportError), RMResources.resourceCulture);

    internal static string CorrelationIDNotFoundInResponse => RMResources.ResourceManager.GetString(nameof (CorrelationIDNotFoundInResponse), RMResources.resourceCulture);

    internal static string CorsAllowedOriginsEmptyList => RMResources.ResourceManager.GetString(nameof (CorsAllowedOriginsEmptyList), RMResources.resourceCulture);

    internal static string CorsAllowedOriginsInvalidPath => RMResources.ResourceManager.GetString(nameof (CorsAllowedOriginsInvalidPath), RMResources.resourceCulture);

    internal static string CorsAllowedOriginsMalformedUri => RMResources.ResourceManager.GetString(nameof (CorsAllowedOriginsMalformedUri), RMResources.resourceCulture);

    internal static string CorsAllowedOriginsWildcardsNotSupported => RMResources.ResourceManager.GetString(nameof (CorsAllowedOriginsWildcardsNotSupported), RMResources.resourceCulture);

    internal static string CorsTooManyRules => RMResources.ResourceManager.GetString(nameof (CorsTooManyRules), RMResources.resourceCulture);

    internal static string CrossPartitionContinuationAndIndex => RMResources.ResourceManager.GetString(nameof (CrossPartitionContinuationAndIndex), RMResources.resourceCulture);

    internal static string CrossPartitionQueryDisabled => RMResources.ResourceManager.GetString(nameof (CrossPartitionQueryDisabled), RMResources.resourceCulture);

    internal static string DatabaseAccountNotFound => RMResources.ResourceManager.GetString(nameof (DatabaseAccountNotFound), RMResources.resourceCulture);

    internal static string DatabaseCreateTopologyConflict => RMResources.ResourceManager.GetString(nameof (DatabaseCreateTopologyConflict), RMResources.resourceCulture);

    internal static string DateTimeConverterInvalidDateTime => RMResources.ResourceManager.GetString(nameof (DateTimeConverterInvalidDateTime), RMResources.resourceCulture);

    internal static string DateTimeConverterInvalidReaderValue => RMResources.ResourceManager.GetString(nameof (DateTimeConverterInvalidReaderValue), RMResources.resourceCulture);

    internal static string DateTimeConveterInvalidReaderDoubleValue => RMResources.ResourceManager.GetString(nameof (DateTimeConveterInvalidReaderDoubleValue), RMResources.resourceCulture);

    internal static string DeserializationError => RMResources.ResourceManager.GetString(nameof (DeserializationError), RMResources.resourceCulture);

    internal static string DnsResolutionFailedTransportError => RMResources.ResourceManager.GetString(nameof (DnsResolutionFailedTransportError), RMResources.resourceCulture);

    internal static string DnsResolutionTimeoutTransportError => RMResources.ResourceManager.GetString(nameof (DnsResolutionTimeoutTransportError), RMResources.resourceCulture);

    internal static string DocumentQueryExecutionContextIsDone => RMResources.ResourceManager.GetString(nameof (DocumentQueryExecutionContextIsDone), RMResources.resourceCulture);

    internal static string DocumentServiceUnavailable => RMResources.ResourceManager.GetString(nameof (DocumentServiceUnavailable), RMResources.resourceCulture);

    internal static string DuplicateCorrelationIdGenerated => RMResources.ResourceManager.GetString(nameof (DuplicateCorrelationIdGenerated), RMResources.resourceCulture);

    internal static string EmptyVirtualNetworkResourceGuid => RMResources.ResourceManager.GetString(nameof (EmptyVirtualNetworkResourceGuid), RMResources.resourceCulture);

    internal static string EmptyVirtualNetworkRulesSpecified => RMResources.ResourceManager.GetString(nameof (EmptyVirtualNetworkRulesSpecified), RMResources.resourceCulture);

    internal static string EnableMultipleWriteLocationsAndStrongConsistencyNotSupported => RMResources.ResourceManager.GetString(nameof (EnableMultipleWriteLocationsAndStrongConsistencyNotSupported), RMResources.resourceCulture);

    internal static string EnableMultipleWriteLocationsBeforeAddingRegion => RMResources.ResourceManager.GetString(nameof (EnableMultipleWriteLocationsBeforeAddingRegion), RMResources.resourceCulture);

    internal static string EnableMultipleWriteLocationsNotModified => RMResources.ResourceManager.GetString(nameof (EnableMultipleWriteLocationsNotModified), RMResources.resourceCulture);

    internal static string EndpointNotFound => RMResources.ResourceManager.GetString(nameof (EndpointNotFound), RMResources.resourceCulture);

    internal static string EntityAlreadyExists => RMResources.ResourceManager.GetString(nameof (EntityAlreadyExists), RMResources.resourceCulture);

    internal static string ExceptionMessage => RMResources.ResourceManager.GetString(nameof (ExceptionMessage), RMResources.resourceCulture);

    internal static string ExceptionMessageAddIpAddress => RMResources.ResourceManager.GetString(nameof (ExceptionMessageAddIpAddress), RMResources.resourceCulture);

    internal static string ExceptionMessageAddRequestUri => RMResources.ResourceManager.GetString(nameof (ExceptionMessageAddRequestUri), RMResources.resourceCulture);

    internal static string FeatureNotSupportedForMultiRegionAccount => RMResources.ResourceManager.GetString(nameof (FeatureNotSupportedForMultiRegionAccount), RMResources.resourceCulture);

    internal static string FeatureNotSupportedInRegion => RMResources.ResourceManager.GetString(nameof (FeatureNotSupportedInRegion), RMResources.resourceCulture);

    internal static string FeatureNotSupportedOnSubscription => RMResources.ResourceManager.GetString(nameof (FeatureNotSupportedOnSubscription), RMResources.resourceCulture);

    internal static string FederationEntityNotFound => RMResources.ResourceManager.GetString(nameof (FederationEntityNotFound), RMResources.resourceCulture);

    internal static string Forbidden => RMResources.ResourceManager.GetString(nameof (Forbidden), RMResources.resourceCulture);

    internal static string ForbiddenClientIpAddress => RMResources.ResourceManager.GetString(nameof (ForbiddenClientIpAddress), RMResources.resourceCulture);

    internal static string GatewayTimedout => RMResources.ResourceManager.GetString(nameof (GatewayTimedout), RMResources.resourceCulture);

    internal static string GlobalAndWriteRegionMisMatch => RMResources.ResourceManager.GetString(nameof (GlobalAndWriteRegionMisMatch), RMResources.resourceCulture);

    internal static string GlobalStrongWriteBarrierNotMet => RMResources.ResourceManager.GetString(nameof (GlobalStrongWriteBarrierNotMet), RMResources.resourceCulture);

    internal static string Gone => RMResources.ResourceManager.GetString(nameof (Gone), RMResources.resourceCulture);

    internal static string IdGenerationFailed => RMResources.ResourceManager.GetString(nameof (IdGenerationFailed), RMResources.resourceCulture);

    internal static string IncompleteRoutingMap => RMResources.ResourceManager.GetString(nameof (IncompleteRoutingMap), RMResources.resourceCulture);

    internal static string InsufficientPermissions => RMResources.ResourceManager.GetString(nameof (InsufficientPermissions), RMResources.resourceCulture);

    internal static string InsufficientResourceTokens => RMResources.ResourceManager.GetString(nameof (InsufficientResourceTokens), RMResources.resourceCulture);

    internal static string InternalServerError => RMResources.ResourceManager.GetString(nameof (InternalServerError), RMResources.resourceCulture);

    internal static string InvalidAPIVersion => RMResources.ResourceManager.GetString(nameof (InvalidAPIVersion), RMResources.resourceCulture);

    internal static string InvalidAPIVersionForFeature => RMResources.ResourceManager.GetString(nameof (InvalidAPIVersionForFeature), RMResources.resourceCulture);

    internal static string InvalidAudienceKind => RMResources.ResourceManager.GetString(nameof (InvalidAudienceKind), RMResources.resourceCulture);

    internal static string InvalidAudienceResourceType => RMResources.ResourceManager.GetString(nameof (InvalidAudienceResourceType), RMResources.resourceCulture);

    internal static string InvalidAuthHeaderFormat => RMResources.ResourceManager.GetString(nameof (InvalidAuthHeaderFormat), RMResources.resourceCulture);

    internal static string InvalidBackendResponse => RMResources.ResourceManager.GetString(nameof (InvalidBackendResponse), RMResources.resourceCulture);

    internal static string InvalidCapabilityCombination => RMResources.ResourceManager.GetString(nameof (InvalidCapabilityCombination), RMResources.resourceCulture);

    internal static string InvalidCharacterInResourceName => RMResources.ResourceManager.GetString(nameof (InvalidCharacterInResourceName), RMResources.resourceCulture);

    internal static string InvalidConflictResolutionMode => RMResources.ResourceManager.GetString(nameof (InvalidConflictResolutionMode), RMResources.resourceCulture);

    internal static string InvalidConsistencyLevel => RMResources.ResourceManager.GetString(nameof (InvalidConsistencyLevel), RMResources.resourceCulture);

    internal static string InvalidContinuationToken => RMResources.ResourceManager.GetString(nameof (InvalidContinuationToken), RMResources.resourceCulture);

    internal static string InvalidDatabase => RMResources.ResourceManager.GetString(nameof (InvalidDatabase), RMResources.resourceCulture);

    internal static string InvalidDateHeader => RMResources.ResourceManager.GetString(nameof (InvalidDateHeader), RMResources.resourceCulture);

    internal static string InvalidDocumentCollection => RMResources.ResourceManager.GetString(nameof (InvalidDocumentCollection), RMResources.resourceCulture);

    internal static string InvalidEnableMultipleWriteLocations => RMResources.ResourceManager.GetString(nameof (InvalidEnableMultipleWriteLocations), RMResources.resourceCulture);

    internal static string InvalidEnumValue => RMResources.ResourceManager.GetString(nameof (InvalidEnumValue), RMResources.resourceCulture);

    internal static string InvalidFailoverPriority => RMResources.ResourceManager.GetString(nameof (InvalidFailoverPriority), RMResources.resourceCulture);

    internal static string InvalidHeaderValue => RMResources.ResourceManager.GetString(nameof (InvalidHeaderValue), RMResources.resourceCulture);

    internal static string InvalidIndexKindValue => RMResources.ResourceManager.GetString(nameof (InvalidIndexKindValue), RMResources.resourceCulture);

    internal static string InvalidIndexSpecFormat => RMResources.ResourceManager.GetString(nameof (InvalidIndexSpecFormat), RMResources.resourceCulture);

    internal static string InvalidIndexTransformationProgressValues => RMResources.ResourceManager.GetString(nameof (InvalidIndexTransformationProgressValues), RMResources.resourceCulture);

    internal static string InvalidLocations => RMResources.ResourceManager.GetString(nameof (InvalidLocations), RMResources.resourceCulture);

    internal static string InvalidPrivateLinkServiceConnections => RMResources.ResourceManager.GetString(nameof (InvalidPrivateLinkServiceConnections), RMResources.resourceCulture);

    internal static string InvalidPrivateLinkServiceProxies => RMResources.ResourceManager.GetString(nameof (InvalidPrivateLinkServiceProxies), RMResources.resourceCulture);

    internal static string InvalidGroupIdCount => RMResources.ResourceManager.GetString(nameof (InvalidGroupIdCount), RMResources.resourceCulture);

    internal static string InvalidGroupId => RMResources.ResourceManager.GetString(nameof (InvalidGroupId), RMResources.resourceCulture);

    internal static string InvalidMaxStalenessInterval => RMResources.ResourceManager.GetString(nameof (InvalidMaxStalenessInterval), RMResources.resourceCulture);

    internal static string InvalidMaxStalenessPrefix => RMResources.ResourceManager.GetString(nameof (InvalidMaxStalenessPrefix), RMResources.resourceCulture);

    internal static string InvalidNonPartitionedOfferThroughput => RMResources.ResourceManager.GetString(nameof (InvalidNonPartitionedOfferThroughput), RMResources.resourceCulture);

    internal static string InsufficientPartitionedDataForOfferThroughput => RMResources.ResourceManager.GetString(nameof (InsufficientPartitionedDataForOfferThroughput), RMResources.resourceCulture);

    internal static string InvalidOfferIsAutoScaleEnabled => RMResources.ResourceManager.GetString(nameof (InvalidOfferIsAutoScaleEnabled), RMResources.resourceCulture);

    internal static string InvalidOfferAutoScaleMode => RMResources.ResourceManager.GetString(nameof (InvalidOfferAutoScaleMode), RMResources.resourceCulture);

    internal static string OfferAutopilotNotSupportedForNonPartitionedCollections => RMResources.ResourceManager.GetString(nameof (OfferAutopilotNotSupportedForNonPartitionedCollections), RMResources.resourceCulture);

    internal static string OfferAutopilotNotSupportedOnSharedThroughputDatabase => RMResources.ResourceManager.GetString(nameof (OfferAutopilotNotSupportedOnSharedThroughputDatabase), RMResources.resourceCulture);

    internal static string InvalidOfferIsRUPerMinuteThroughputEnabled => RMResources.ResourceManager.GetString(nameof (InvalidOfferIsRUPerMinuteThroughputEnabled), RMResources.resourceCulture);

    internal static string InvalidOfferThroughput => RMResources.ResourceManager.GetString(nameof (InvalidOfferThroughput), RMResources.resourceCulture);

    internal static string InvalidOfferType => RMResources.ResourceManager.GetString(nameof (InvalidOfferType), RMResources.resourceCulture);

    internal static string InvalidOfferV2Input => RMResources.ResourceManager.GetString(nameof (InvalidOfferV2Input), RMResources.resourceCulture);

    internal static string InvalidOwnerResourceType => RMResources.ResourceManager.GetString(nameof (InvalidOwnerResourceType), RMResources.resourceCulture);

    internal static string InvalidPageSize => RMResources.ResourceManager.GetString(nameof (InvalidPageSize), RMResources.resourceCulture);

    internal static string InvalidPartitionKey => RMResources.ResourceManager.GetString(nameof (InvalidPartitionKey), RMResources.resourceCulture);

    internal static string InvalidPartitionKeyRangeIdHeader => RMResources.ResourceManager.GetString(nameof (InvalidPartitionKeyRangeIdHeader), RMResources.resourceCulture);

    internal static string InvalidPermissionMode => RMResources.ResourceManager.GetString(nameof (InvalidPermissionMode), RMResources.resourceCulture);

    internal static string InvalidProxyCommand => RMResources.ResourceManager.GetString(nameof (InvalidProxyCommand), RMResources.resourceCulture);

    internal static string InvalidQuery => RMResources.ResourceManager.GetString(nameof (InvalidQuery), RMResources.resourceCulture);

    internal static string InvalidQueryValue => RMResources.ResourceManager.GetString(nameof (InvalidQueryValue), RMResources.resourceCulture);

    internal static string InvalidRegionsInSessionToken => RMResources.ResourceManager.GetString(nameof (InvalidRegionsInSessionToken), RMResources.resourceCulture);

    internal static string InvalidReplicationAndConsistencyCombination => RMResources.ResourceManager.GetString(nameof (InvalidReplicationAndConsistencyCombination), RMResources.resourceCulture);

    internal static string InvalidResourceID => RMResources.ResourceManager.GetString(nameof (InvalidResourceID), RMResources.resourceCulture);

    internal static string InvalidResourceIdBatchSize => RMResources.ResourceManager.GetString(nameof (InvalidResourceIdBatchSize), RMResources.resourceCulture);

    internal static string InvalidResourceKind => RMResources.ResourceManager.GetString(nameof (InvalidResourceKind), RMResources.resourceCulture);

    internal static string InvalidResourceType => RMResources.ResourceManager.GetString(nameof (InvalidResourceType), RMResources.resourceCulture);

    internal static string InvalidResourceUrlPath => RMResources.ResourceManager.GetString(nameof (InvalidResourceUrlPath), RMResources.resourceCulture);

    internal static string InvalidResourceUrlQuery => RMResources.ResourceManager.GetString(nameof (InvalidResourceUrlQuery), RMResources.resourceCulture);

    internal static string InvalidResponseContinuationTokenLimit => RMResources.ResourceManager.GetString(nameof (InvalidResponseContinuationTokenLimit), RMResources.resourceCulture);

    internal static string InvalidScriptResource => RMResources.ResourceManager.GetString(nameof (InvalidScriptResource), RMResources.resourceCulture);

    internal static string InvalidSessionToken => RMResources.ResourceManager.GetString(nameof (InvalidSessionToken), RMResources.resourceCulture);

    internal static string InvalidSpaceEndingInResourceName => RMResources.ResourceManager.GetString(nameof (InvalidSpaceEndingInResourceName), RMResources.resourceCulture);

    internal static string InvalidStalenessPolicy => RMResources.ResourceManager.GetString(nameof (InvalidStalenessPolicy), RMResources.resourceCulture);

    internal static string InvalidStorageServiceMediaIndex => RMResources.ResourceManager.GetString(nameof (InvalidStorageServiceMediaIndex), RMResources.resourceCulture);

    internal static string InvalidSwitchOffCanEnableMultipleWriteLocations => RMResources.ResourceManager.GetString(nameof (InvalidSwitchOffCanEnableMultipleWriteLocations), RMResources.resourceCulture);

    internal static string InvalidSwitchOnCanEnableMultipleWriteLocations => RMResources.ResourceManager.GetString(nameof (InvalidSwitchOnCanEnableMultipleWriteLocations), RMResources.resourceCulture);

    internal static string InvalidTarget => RMResources.ResourceManager.GetString(nameof (InvalidTarget), RMResources.resourceCulture);

    internal static string InvalidTokenTimeRange => RMResources.ResourceManager.GetString(nameof (InvalidTokenTimeRange), RMResources.resourceCulture);

    internal static string InvalidUrl => RMResources.ResourceManager.GetString(nameof (InvalidUrl), RMResources.resourceCulture);

    internal static string InvalidUseSystemKey => RMResources.ResourceManager.GetString(nameof (InvalidUseSystemKey), RMResources.resourceCulture);

    internal static string InvalidVersionFormat => RMResources.ResourceManager.GetString(nameof (InvalidVersionFormat), RMResources.resourceCulture);

    internal static string IpAddressBlockedByPolicy => RMResources.ResourceManager.GetString(nameof (IpAddressBlockedByPolicy), RMResources.resourceCulture);

    internal static string IsForceDeleteFederationAllowed => RMResources.ResourceManager.GetString(nameof (IsForceDeleteFederationAllowed), RMResources.resourceCulture);

    internal static string JsonArrayNotStarted => RMResources.ResourceManager.GetString(nameof (JsonArrayNotStarted), RMResources.resourceCulture);

    internal static string JsonInvalidEscapedCharacter => RMResources.ResourceManager.GetString(nameof (JsonInvalidEscapedCharacter), RMResources.resourceCulture);

    internal static string JsonInvalidNumber => RMResources.ResourceManager.GetString(nameof (JsonInvalidNumber), RMResources.resourceCulture);

    internal static string JsonInvalidParameter => RMResources.ResourceManager.GetString(nameof (JsonInvalidParameter), RMResources.resourceCulture);

    internal static string JsonInvalidStringCharacter => RMResources.ResourceManager.GetString(nameof (JsonInvalidStringCharacter), RMResources.resourceCulture);

    internal static string JsonInvalidToken => RMResources.ResourceManager.GetString(nameof (JsonInvalidToken), RMResources.resourceCulture);

    internal static string JsonInvalidUnicodeEscape => RMResources.ResourceManager.GetString(nameof (JsonInvalidUnicodeEscape), RMResources.resourceCulture);

    internal static string JsonMaxNestingExceeded => RMResources.ResourceManager.GetString(nameof (JsonMaxNestingExceeded), RMResources.resourceCulture);

    internal static string JsonMissingClosingQuote => RMResources.ResourceManager.GetString(nameof (JsonMissingClosingQuote), RMResources.resourceCulture);

    internal static string JsonMissingEndArray => RMResources.ResourceManager.GetString(nameof (JsonMissingEndArray), RMResources.resourceCulture);

    internal static string JsonMissingEndObject => RMResources.ResourceManager.GetString(nameof (JsonMissingEndObject), RMResources.resourceCulture);

    internal static string JsonMissingNameSeparator => RMResources.ResourceManager.GetString(nameof (JsonMissingNameSeparator), RMResources.resourceCulture);

    internal static string JsonMissingProperty => RMResources.ResourceManager.GetString(nameof (JsonMissingProperty), RMResources.resourceCulture);

    internal static string JsonNotComplete => RMResources.ResourceManager.GetString(nameof (JsonNotComplete), RMResources.resourceCulture);

    internal static string JsonNotFieldnameToken => RMResources.ResourceManager.GetString(nameof (JsonNotFieldnameToken), RMResources.resourceCulture);

    internal static string JsonNotNumberToken => RMResources.ResourceManager.GetString(nameof (JsonNotNumberToken), RMResources.resourceCulture);

    internal static string JsonNotStringToken => RMResources.ResourceManager.GetString(nameof (JsonNotStringToken), RMResources.resourceCulture);

    internal static string JsonNumberOutOfRange => RMResources.ResourceManager.GetString(nameof (JsonNumberOutOfRange), RMResources.resourceCulture);

    internal static string JsonNumberTooLong => RMResources.ResourceManager.GetString(nameof (JsonNumberTooLong), RMResources.resourceCulture);

    internal static string JsonObjectNotStarted => RMResources.ResourceManager.GetString(nameof (JsonObjectNotStarted), RMResources.resourceCulture);

    internal static string JsonPropertyAlreadyAdded => RMResources.ResourceManager.GetString(nameof (JsonPropertyAlreadyAdded), RMResources.resourceCulture);

    internal static string JsonPropertyArrayOrObjectNotStarted => RMResources.ResourceManager.GetString(nameof (JsonPropertyArrayOrObjectNotStarted), RMResources.resourceCulture);

    internal static string JsonUnexpectedEndArray => RMResources.ResourceManager.GetString(nameof (JsonUnexpectedEndArray), RMResources.resourceCulture);

    internal static string JsonUnexpectedEndObject => RMResources.ResourceManager.GetString(nameof (JsonUnexpectedEndObject), RMResources.resourceCulture);

    internal static string JsonUnexpectedNameSeparator => RMResources.ResourceManager.GetString(nameof (JsonUnexpectedNameSeparator), RMResources.resourceCulture);

    internal static string JsonUnexpectedToken => RMResources.ResourceManager.GetString(nameof (JsonUnexpectedToken), RMResources.resourceCulture);

    internal static string JsonUnexpectedValueSeparator => RMResources.ResourceManager.GetString(nameof (JsonUnexpectedValueSeparator), RMResources.resourceCulture);

    internal static string Locked => RMResources.ResourceManager.GetString(nameof (Locked), RMResources.resourceCulture);

    internal static string MaximumRULimitExceeded => RMResources.ResourceManager.GetString(nameof (MaximumRULimitExceeded), RMResources.resourceCulture);

    internal static string MessageIdHeaderMissing => RMResources.ResourceManager.GetString(nameof (MessageIdHeaderMissing), RMResources.resourceCulture);

    internal static string MethodNotAllowed => RMResources.ResourceManager.GetString(nameof (MethodNotAllowed), RMResources.resourceCulture);

    internal static string MismatchToken => RMResources.ResourceManager.GetString(nameof (MismatchToken), RMResources.resourceCulture);

    internal static string MissingAuthHeader => RMResources.ResourceManager.GetString(nameof (MissingAuthHeader), RMResources.resourceCulture);

    internal static string MissingDateForAuthorization => RMResources.ResourceManager.GetString(nameof (MissingDateForAuthorization), RMResources.resourceCulture);

    internal static string MissingPartitionKeyValue => RMResources.ResourceManager.GetString(nameof (MissingPartitionKeyValue), RMResources.resourceCulture);

    internal static string MissingProperty => RMResources.ResourceManager.GetString(nameof (MissingProperty), RMResources.resourceCulture);

    internal static string MissingRequiredHeader => RMResources.ResourceManager.GetString(nameof (MissingRequiredHeader), RMResources.resourceCulture);

    internal static string MissingRequiredQuery => RMResources.ResourceManager.GetString(nameof (MissingRequiredQuery), RMResources.resourceCulture);

    internal static string MoreThanOneBackupIntervalCapability => RMResources.ResourceManager.GetString(nameof (MoreThanOneBackupIntervalCapability), RMResources.resourceCulture);

    internal static string MoreThanOneBackupRetentionCapability => RMResources.ResourceManager.GetString(nameof (MoreThanOneBackupRetentionCapability), RMResources.resourceCulture);

    internal static string MustHaveNonZeroPreferredRegionWhenAutomaticFailoverDisabled => RMResources.ResourceManager.GetString(nameof (MustHaveNonZeroPreferredRegionWhenAutomaticFailoverDisabled), RMResources.resourceCulture);

    internal static string NamingPropertyNotFound => RMResources.ResourceManager.GetString(nameof (NamingPropertyNotFound), RMResources.resourceCulture);

    internal static string NegativeInteger => RMResources.ResourceManager.GetString(nameof (NegativeInteger), RMResources.resourceCulture);

    internal static string networks_xsd => RMResources.ResourceManager.GetString("networks.xsd", RMResources.resourceCulture);

    internal static string NoGraftPoint => RMResources.ResourceManager.GetString(nameof (NoGraftPoint), RMResources.resourceCulture);

    internal static string NotFound => RMResources.ResourceManager.GetString(nameof (NotFound), RMResources.resourceCulture);

    internal static string OfferReplaceTopologyConflict => RMResources.ResourceManager.GetString(nameof (OfferReplaceTopologyConflict), RMResources.resourceCulture);

    internal static string OfferReplaceWithSpecifiedVersionsNotSupported => RMResources.ResourceManager.GetString(nameof (OfferReplaceWithSpecifiedVersionsNotSupported), RMResources.resourceCulture);

    internal static string OfferTypeAndThroughputCannotBeSpecifiedBoth => RMResources.ResourceManager.GetString(nameof (OfferTypeAndThroughputCannotBeSpecifiedBoth), RMResources.resourceCulture);

    internal static string OfferThroughputAndAutoPilotSettingsCannotBeSpecifiedBoth => RMResources.ResourceManager.GetString(nameof (OfferThroughputAndAutoPilotSettingsCannotBeSpecifiedBoth), RMResources.resourceCulture);

    internal static string AutoPilotTierAndAutoPilotSettingsCannotBeSpecifiedBoth => RMResources.ResourceManager.GetString(nameof (AutoPilotTierAndAutoPilotSettingsCannotBeSpecifiedBoth), RMResources.resourceCulture);

    internal static string AutopilotAutoUpgradeUnsupportedNonPartitionedCollection => RMResources.ResourceManager.GetString(nameof (AutopilotAutoUpgradeUnsupportedNonPartitionedCollection), RMResources.resourceCulture);

    internal static string OperationRequestedStatusIsInvalid => RMResources.ResourceManager.GetString(nameof (OperationRequestedStatusIsInvalid), RMResources.resourceCulture);

    internal static string PartitionIsFull => RMResources.ResourceManager.GetString(nameof (PartitionIsFull), RMResources.resourceCulture);

    internal static string PartitionKeyAndEffectivePartitionKeyBothSpecified => RMResources.ResourceManager.GetString(nameof (PartitionKeyAndEffectivePartitionKeyBothSpecified), RMResources.resourceCulture);

    internal static string PartitionKeyAndPartitionKeyRangeRangeIdBothSpecified => RMResources.ResourceManager.GetString(nameof (PartitionKeyAndPartitionKeyRangeRangeIdBothSpecified), RMResources.resourceCulture);

    internal static string PartitionKeyMismatch => RMResources.ResourceManager.GetString(nameof (PartitionKeyMismatch), RMResources.resourceCulture);

    internal static string PartitionKeyRangeIdAbsentInContext => RMResources.ResourceManager.GetString(nameof (PartitionKeyRangeIdAbsentInContext), RMResources.resourceCulture);

    internal static string PartitionKeyRangeIdOrPartitionKeyMustBeSpecified => RMResources.ResourceManager.GetString(nameof (PartitionKeyRangeIdOrPartitionKeyMustBeSpecified), RMResources.resourceCulture);

    internal static string PartitionKeyRangeNotFound => RMResources.ResourceManager.GetString(nameof (PartitionKeyRangeNotFound), RMResources.resourceCulture);

    internal static string PositiveInteger => RMResources.ResourceManager.GetString(nameof (PositiveInteger), RMResources.resourceCulture);

    internal static string PreconditionFailed => RMResources.ResourceManager.GetString(nameof (PreconditionFailed), RMResources.resourceCulture);

    internal static string PrimaryNotFound => RMResources.ResourceManager.GetString(nameof (PrimaryNotFound), RMResources.resourceCulture);

    internal static string PropertyCannotBeNull => RMResources.ResourceManager.GetString(nameof (PropertyCannotBeNull), RMResources.resourceCulture);

    internal static string PropertyNotFound => RMResources.ResourceManager.GetString(nameof (PropertyNotFound), RMResources.resourceCulture);

    internal static string ProvisionLimit => RMResources.ResourceManager.GetString(nameof (ProvisionLimit), RMResources.resourceCulture);

    internal static string ReadQuorumNotMet => RMResources.ResourceManager.GetString(nameof (ReadQuorumNotMet), RMResources.resourceCulture);

    internal static string ReadSessionNotAvailable => RMResources.ResourceManager.GetString(nameof (ReadSessionNotAvailable), RMResources.resourceCulture);

    internal static string ReceiveFailedTransportError => RMResources.ResourceManager.GetString(nameof (ReceiveFailedTransportError), RMResources.resourceCulture);

    internal static string ReceiveStreamClosedTransportError => RMResources.ResourceManager.GetString(nameof (ReceiveStreamClosedTransportError), RMResources.resourceCulture);

    internal static string ReceiveTimeoutTransportError => RMResources.ResourceManager.GetString(nameof (ReceiveTimeoutTransportError), RMResources.resourceCulture);

    internal static string RemoveWriteRegionNotSupported => RMResources.ResourceManager.GetString(nameof (RemoveWriteRegionNotSupported), RMResources.resourceCulture);

    internal static string ReplicaAtIndexNotAvailable => RMResources.ResourceManager.GetString(nameof (ReplicaAtIndexNotAvailable), RMResources.resourceCulture);

    internal static string RequestConsistencyLevelNotSupported => RMResources.ResourceManager.GetString(nameof (RequestConsistencyLevelNotSupported), RMResources.resourceCulture);

    internal static string RequestEntityTooLarge => RMResources.ResourceManager.GetString(nameof (RequestEntityTooLarge), RMResources.resourceCulture);

    internal static string RequestTimeout => RMResources.ResourceManager.GetString(nameof (RequestTimeout), RMResources.resourceCulture);

    internal static string RequestTimeoutTransportError => RMResources.ResourceManager.GetString(nameof (RequestTimeoutTransportError), RMResources.resourceCulture);

    internal static string RequestTooLarge => RMResources.ResourceManager.GetString(nameof (RequestTooLarge), RMResources.resourceCulture);

    internal static string ResourceIdNotValid => RMResources.ResourceManager.GetString(nameof (ResourceIdNotValid), RMResources.resourceCulture);

    internal static string ResourceIdPolicyNotSupported => RMResources.ResourceManager.GetString(nameof (ResourceIdPolicyNotSupported), RMResources.resourceCulture);

    internal static string ResourceTypeNotSupported => RMResources.ResourceManager.GetString(nameof (ResourceTypeNotSupported), RMResources.resourceCulture);

    internal static string RetryWith => RMResources.ResourceManager.GetString(nameof (RetryWith), RMResources.resourceCulture);

    internal static string ScriptRenameInMultiplePartitionsIsNotSupported => RMResources.ResourceManager.GetString(nameof (ScriptRenameInMultiplePartitionsIsNotSupported), RMResources.resourceCulture);

    internal static string SecondariesNotFound => RMResources.ResourceManager.GetString(nameof (SecondariesNotFound), RMResources.resourceCulture);

    internal static string SendFailedTransportError => RMResources.ResourceManager.GetString(nameof (SendFailedTransportError), RMResources.resourceCulture);

    internal static string SendLockTimeoutTransportError => RMResources.ResourceManager.GetString(nameof (SendLockTimeoutTransportError), RMResources.resourceCulture);

    internal static string SendTimeoutTransportError => RMResources.ResourceManager.GetString(nameof (SendTimeoutTransportError), RMResources.resourceCulture);

    internal static string ServerResponseBodyTooLargeError => RMResources.ResourceManager.GetString(nameof (ServerResponseBodyTooLargeError), RMResources.resourceCulture);

    internal static string ServerResponseHeaderTooLargeError => RMResources.ResourceManager.GetString(nameof (ServerResponseHeaderTooLargeError), RMResources.resourceCulture);

    internal static string ServerResponseInvalidHeaderLengthError => RMResources.ResourceManager.GetString(nameof (ServerResponseInvalidHeaderLengthError), RMResources.resourceCulture);

    internal static string ServerResponseTransportRequestIdMissingError => RMResources.ResourceManager.GetString(nameof (ServerResponseTransportRequestIdMissingError), RMResources.resourceCulture);

    internal static string ServiceNotFound => RMResources.ResourceManager.GetString(nameof (ServiceNotFound), RMResources.resourceCulture);

    internal static string ServiceReservedBitsOutOfRange => RMResources.ResourceManager.GetString(nameof (ServiceReservedBitsOutOfRange), RMResources.resourceCulture);

    internal static string ServiceUnavailable => RMResources.ResourceManager.GetString(nameof (ServiceUnavailable), RMResources.resourceCulture);

    internal static string ServiceWithResourceIdNotFound => RMResources.ResourceManager.GetString(nameof (ServiceWithResourceIdNotFound), RMResources.resourceCulture);

    internal static string SpatialBoundingBoxInvalidCoordinates => RMResources.ResourceManager.GetString(nameof (SpatialBoundingBoxInvalidCoordinates), RMResources.resourceCulture);

    internal static string SpatialExtensionMethodsNotImplemented => RMResources.ResourceManager.GetString(nameof (SpatialExtensionMethodsNotImplemented), RMResources.resourceCulture);

    internal static string SpatialFailedToDeserializeCrs => RMResources.ResourceManager.GetString(nameof (SpatialFailedToDeserializeCrs), RMResources.resourceCulture);

    internal static string SpatialInvalidGeometryType => RMResources.ResourceManager.GetString(nameof (SpatialInvalidGeometryType), RMResources.resourceCulture);

    internal static string SpatialInvalidPosition => RMResources.ResourceManager.GetString(nameof (SpatialInvalidPosition), RMResources.resourceCulture);

    internal static string SslNegotiationFailedTransportError => RMResources.ResourceManager.GetString(nameof (SslNegotiationFailedTransportError), RMResources.resourceCulture);

    internal static string SslNegotiationTimeoutTransportError => RMResources.ResourceManager.GetString(nameof (SslNegotiationTimeoutTransportError), RMResources.resourceCulture);

    internal static string StarSlashArgumentError => RMResources.ResourceManager.GetString(nameof (StarSlashArgumentError), RMResources.resourceCulture);

    internal static string StorageAnalyticsNotEnabled => RMResources.ResourceManager.GetString(nameof (StorageAnalyticsNotEnabled), RMResources.resourceCulture);

    internal static string StringArgumentNullOrEmpty => RMResources.ResourceManager.GetString(nameof (StringArgumentNullOrEmpty), RMResources.resourceCulture);

    internal static string TooFewPartitionKeyComponents => RMResources.ResourceManager.GetString(nameof (TooFewPartitionKeyComponents), RMResources.resourceCulture);

    internal static string TooManyPartitionKeyComponents => RMResources.ResourceManager.GetString(nameof (TooManyPartitionKeyComponents), RMResources.resourceCulture);

    internal static string TooManyRequests => RMResources.ResourceManager.GetString(nameof (TooManyRequests), RMResources.resourceCulture);

    internal static string TransportExceptionMessage => RMResources.ResourceManager.GetString(nameof (TransportExceptionMessage), RMResources.resourceCulture);

    internal static string TransportNegotiationTimeoutTransportError => RMResources.ResourceManager.GetString(nameof (TransportNegotiationTimeoutTransportError), RMResources.resourceCulture);

    internal static string UnableToDeserializePartitionKeyValue => RMResources.ResourceManager.GetString(nameof (UnableToDeserializePartitionKeyValue), RMResources.resourceCulture);

    internal static string UnableToFindFreeConnection => RMResources.ResourceManager.GetString(nameof (UnableToFindFreeConnection), RMResources.resourceCulture);

    internal static string Unauthorized => RMResources.ResourceManager.GetString(nameof (Unauthorized), RMResources.resourceCulture);

    internal static string UnauthorizedOfferReplaceRequest => RMResources.ResourceManager.GetString(nameof (UnauthorizedOfferReplaceRequest), RMResources.resourceCulture);

    internal static string UnauthorizedRequestForAutoScale => RMResources.ResourceManager.GetString(nameof (UnauthorizedRequestForAutoScale), RMResources.resourceCulture);

    internal static string UnexpectedConsistencyLevel => RMResources.ResourceManager.GetString(nameof (UnexpectedConsistencyLevel), RMResources.resourceCulture);

    internal static string UnexpectedJsonSerializationFormat => RMResources.ResourceManager.GetString(nameof (UnexpectedJsonSerializationFormat), RMResources.resourceCulture);

    internal static string UnexpectedJsonTokenType => RMResources.ResourceManager.GetString(nameof (UnexpectedJsonTokenType), RMResources.resourceCulture);

    internal static string UnexpectedOfferVersion => RMResources.ResourceManager.GetString(nameof (UnexpectedOfferVersion), RMResources.resourceCulture);

    internal static string UnexpectedOperationTypeForRoutingRequest => RMResources.ResourceManager.GetString(nameof (UnexpectedOperationTypeForRoutingRequest), RMResources.resourceCulture);

    internal static string UnexpectedOperator => RMResources.ResourceManager.GetString(nameof (UnexpectedOperator), RMResources.resourceCulture);

    internal static string UnexpectedPartitionKeyRangeId => RMResources.ResourceManager.GetString(nameof (UnexpectedPartitionKeyRangeId), RMResources.resourceCulture);

    internal static string UnExpectedResourceKindToReEncrypt => RMResources.ResourceManager.GetString(nameof (UnExpectedResourceKindToReEncrypt), RMResources.resourceCulture);

    internal static string UnexpectedResourceType => RMResources.ResourceManager.GetString(nameof (UnexpectedResourceType), RMResources.resourceCulture);

    internal static string UnknownResourceKind => RMResources.ResourceManager.GetString(nameof (UnknownResourceKind), RMResources.resourceCulture);

    internal static string UnknownResourceType => RMResources.ResourceManager.GetString(nameof (UnknownResourceType), RMResources.resourceCulture);

    internal static string UnknownTransportError => RMResources.ResourceManager.GetString(nameof (UnknownTransportError), RMResources.resourceCulture);

    internal static string UnorderedDistinctQueryContinuationToken => RMResources.ResourceManager.GetString(nameof (UnorderedDistinctQueryContinuationToken), RMResources.resourceCulture);

    internal static string UnsupportedCapabilityForKind => RMResources.ResourceManager.GetString(nameof (UnsupportedCapabilityForKind), RMResources.resourceCulture);

    internal static string UnsupportedCrossPartitionOrderByQueryOnMixedTypes => RMResources.ResourceManager.GetString(nameof (UnsupportedCrossPartitionOrderByQueryOnMixedTypes), RMResources.resourceCulture);

    internal static string UnsupportedCrossPartitionQuery => RMResources.ResourceManager.GetString(nameof (UnsupportedCrossPartitionQuery), RMResources.resourceCulture);

    internal static string UnsupportedCrossPartitionQueryWithAggregate => RMResources.ResourceManager.GetString(nameof (UnsupportedCrossPartitionQueryWithAggregate), RMResources.resourceCulture);

    internal static string UnsupportedEntityType => RMResources.ResourceManager.GetString(nameof (UnsupportedEntityType), RMResources.resourceCulture);

    internal static string UnsupportedHints => RMResources.ResourceManager.GetString(nameof (UnsupportedHints), RMResources.resourceCulture);

    internal static string UnsupportedKeyType => RMResources.ResourceManager.GetString(nameof (UnsupportedKeyType), RMResources.resourceCulture);

    internal static string UnSupportedOfferThroughput => RMResources.ResourceManager.GetString(nameof (UnSupportedOfferThroughput), RMResources.resourceCulture);

    internal static string UnSupportedOfferThroughputWithTwoRanges => RMResources.ResourceManager.GetString(nameof (UnSupportedOfferThroughputWithTwoRanges), RMResources.resourceCulture);

    internal static string UnsupportedOfferTypeWithV2Offer => RMResources.ResourceManager.GetString(nameof (UnsupportedOfferTypeWithV2Offer), RMResources.resourceCulture);

    internal static string UnsupportedOfferVersion => RMResources.ResourceManager.GetString(nameof (UnsupportedOfferVersion), RMResources.resourceCulture);

    internal static string UnsupportedPartitionKeyComponentValue => RMResources.ResourceManager.GetString(nameof (UnsupportedPartitionKeyComponentValue), RMResources.resourceCulture);

    internal static string UnsupportedProgram => RMResources.ResourceManager.GetString(nameof (UnsupportedProgram), RMResources.resourceCulture);

    internal static string UnsupportedProtocol => RMResources.ResourceManager.GetString(nameof (UnsupportedProtocol), RMResources.resourceCulture);

    internal static string UnsupportedQueryWithFullResultAggregate => RMResources.ResourceManager.GetString(nameof (UnsupportedQueryWithFullResultAggregate), RMResources.resourceCulture);

    internal static string UnsupportedRegion => RMResources.ResourceManager.GetString(nameof (UnsupportedRegion), RMResources.resourceCulture);

    internal static string UnsupportedSparkClusterRegion => RMResources.ResourceManager.GetString(nameof (UnsupportedSparkClusterRegion), RMResources.resourceCulture);

    internal static string UnsupportedRollbackKind => RMResources.ResourceManager.GetString(nameof (UnsupportedRollbackKind), RMResources.resourceCulture);

    internal static string UnsupportedRootPolicyChange => RMResources.ResourceManager.GetString(nameof (UnsupportedRootPolicyChange), RMResources.resourceCulture);

    internal static string UnsupportedSystemKeyKind => RMResources.ResourceManager.GetString(nameof (UnsupportedSystemKeyKind), RMResources.resourceCulture);

    internal static string UnsupportedTokenType => RMResources.ResourceManager.GetString(nameof (UnsupportedTokenType), RMResources.resourceCulture);

    internal static string UnsupportedV1OfferVersion => RMResources.ResourceManager.GetString(nameof (UnsupportedV1OfferVersion), RMResources.resourceCulture);

    internal static string UpsertsForScriptsWithMultiplePartitionsAreNotSupported => RMResources.ResourceManager.GetString(nameof (UpsertsForScriptsWithMultiplePartitionsAreNotSupported), RMResources.resourceCulture);

    internal static string WriteRegionAutomaticFailoverNotEnabled => RMResources.ResourceManager.GetString(nameof (WriteRegionAutomaticFailoverNotEnabled), RMResources.resourceCulture);

    internal static string WriteRegionDoesNotExist => RMResources.ResourceManager.GetString(nameof (WriteRegionDoesNotExist), RMResources.resourceCulture);

    internal static string ZoneRedundantAccountsNotSupportedInLocation => RMResources.ResourceManager.GetString(nameof (ZoneRedundantAccountsNotSupportedInLocation), RMResources.resourceCulture);

    internal static string ConnectionIsBusy => RMResources.ResourceManager.GetString(nameof (ConnectionIsBusy), RMResources.resourceCulture);

    internal static string InvalidGremlinPartitionKey => RMResources.ResourceManager.GetString(nameof (InvalidGremlinPartitionKey), RMResources.resourceCulture);

    internal static string DataPlaneOperationNotAllowed => RMResources.ResourceManager.GetString(nameof (DataPlaneOperationNotAllowed), RMResources.resourceCulture);

    internal static string CollectionCreateInProgress => RMResources.ResourceManager.GetString(nameof (CollectionCreateInProgress), RMResources.resourceCulture);
  }
}
