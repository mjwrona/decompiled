// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.WebApiResources
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.WebApi
{
  internal static class WebApiResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (WebApiResources), typeof (WebApiResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => WebApiResources.s_resMgr;

    private static string Get(string resourceName) => WebApiResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? WebApiResources.Get(resourceName) : WebApiResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) WebApiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? WebApiResources.GetInt(resourceName) : (int) WebApiResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) WebApiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? WebApiResources.GetBool(resourceName) : (bool) WebApiResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => WebApiResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = WebApiResources.Get(resourceName, culture);
      if (args == null)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }

    public static string UnsupportedContentType(object arg0) => WebApiResources.Format(nameof (UnsupportedContentType), arg0);

    public static string UnsupportedContentType(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (UnsupportedContentType), culture, arg0);

    public static string DownloadCorrupted() => WebApiResources.Get(nameof (DownloadCorrupted));

    public static string DownloadCorrupted(CultureInfo culture) => WebApiResources.Get(nameof (DownloadCorrupted), culture);

    public static string SerializingPhrase() => WebApiResources.Get(nameof (SerializingPhrase));

    public static string SerializingPhrase(CultureInfo culture) => WebApiResources.Get(nameof (SerializingPhrase), culture);

    public static string DeserializationCorrupt() => WebApiResources.Get(nameof (DeserializationCorrupt));

    public static string DeserializationCorrupt(CultureInfo culture) => WebApiResources.Get(nameof (DeserializationCorrupt), culture);

    public static string ClientResourceVersionNotSupported(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return WebApiResources.Format(nameof (ClientResourceVersionNotSupported), arg0, arg1, arg2, arg3);
    }

    public static string ClientResourceVersionNotSupported(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return WebApiResources.Format(nameof (ClientResourceVersionNotSupported), culture, arg0, arg1, arg2, arg3);
    }

    public static string ResourceNotFoundOnServerMessage(object arg0, object arg1) => WebApiResources.Format(nameof (ResourceNotFoundOnServerMessage), arg0, arg1);

    public static string ResourceNotFoundOnServerMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WebApiResources.Format(nameof (ResourceNotFoundOnServerMessage), culture, arg0, arg1);
    }

    public static string ResourceNotRegisteredMessage(object arg0) => WebApiResources.Format(nameof (ResourceNotRegisteredMessage), arg0);

    public static string ResourceNotRegisteredMessage(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (ResourceNotRegisteredMessage), culture, arg0);

    public static string ContainerIdMustBeGreaterThanZero() => WebApiResources.Get(nameof (ContainerIdMustBeGreaterThanZero));

    public static string ContainerIdMustBeGreaterThanZero(CultureInfo culture) => WebApiResources.Get(nameof (ContainerIdMustBeGreaterThanZero), culture);

    public static string FullyQualifiedLocationParameter() => WebApiResources.Get(nameof (FullyQualifiedLocationParameter));

    public static string FullyQualifiedLocationParameter(CultureInfo culture) => WebApiResources.Get(nameof (FullyQualifiedLocationParameter), culture);

    public static string RelativeLocationMappingErrorMessage() => WebApiResources.Get(nameof (RelativeLocationMappingErrorMessage));

    public static string RelativeLocationMappingErrorMessage(CultureInfo culture) => WebApiResources.Get(nameof (RelativeLocationMappingErrorMessage), culture);

    public static string InvalidAccessMappingLocationServiceUrl() => WebApiResources.Get(nameof (InvalidAccessMappingLocationServiceUrl));

    public static string InvalidAccessMappingLocationServiceUrl(CultureInfo culture) => WebApiResources.Get(nameof (InvalidAccessMappingLocationServiceUrl), culture);

    public static string ServiceDefinitionDoesNotExist(object arg0, object arg1) => WebApiResources.Format(nameof (ServiceDefinitionDoesNotExist), arg0, arg1);

    public static string ServiceDefinitionDoesNotExist(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WebApiResources.Format(nameof (ServiceDefinitionDoesNotExist), culture, arg0, arg1);
    }

    public static string ServiceDefinitionWithNoLocations(object arg0) => WebApiResources.Format(nameof (ServiceDefinitionWithNoLocations), arg0);

    public static string ServiceDefinitionWithNoLocations(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (ServiceDefinitionWithNoLocations), culture, arg0);

    public static string JsonParseError(object arg0) => WebApiResources.Format(nameof (JsonParseError), arg0);

    public static string JsonParseError(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (JsonParseError), culture, arg0);

    public static string MissingRequiredParameterMessage(object arg0) => WebApiResources.Format(nameof (MissingRequiredParameterMessage), arg0);

    public static string MissingRequiredParameterMessage(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (MissingRequiredParameterMessage), culture, arg0);

    public static string ProxyAuthenticationRequired() => WebApiResources.Get(nameof (ProxyAuthenticationRequired));

    public static string ProxyAuthenticationRequired(CultureInfo culture) => WebApiResources.Get(nameof (ProxyAuthenticationRequired), culture);

    public static string InvalidApiVersionStringMessage(object arg0) => WebApiResources.Format(nameof (InvalidApiVersionStringMessage), arg0);

    public static string InvalidApiVersionStringMessage(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (InvalidApiVersionStringMessage), culture, arg0);

    public static string ApiResourceDuplicateIdMessage(object arg0) => WebApiResources.Format(nameof (ApiResourceDuplicateIdMessage), arg0);

    public static string ApiResourceDuplicateIdMessage(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (ApiResourceDuplicateIdMessage), culture, arg0);

    public static string ApiResourceDuplicateRouteNameMessage(object arg0) => WebApiResources.Format(nameof (ApiResourceDuplicateRouteNameMessage), arg0);

    public static string ApiResourceDuplicateRouteNameMessage(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (ApiResourceDuplicateRouteNameMessage), culture, arg0);

    public static string RequestContentTypeNotSupported(object arg0, object arg1, object arg2) => WebApiResources.Format(nameof (RequestContentTypeNotSupported), arg0, arg1, arg2);

    public static string RequestContentTypeNotSupported(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return WebApiResources.Format(nameof (RequestContentTypeNotSupported), culture, arg0, arg1, arg2);
    }

    public static string InvalidReferenceLinkFormat() => WebApiResources.Get(nameof (InvalidReferenceLinkFormat));

    public static string InvalidReferenceLinkFormat(CultureInfo culture) => WebApiResources.Get(nameof (InvalidReferenceLinkFormat), culture);

    public static string PreviewVersionNotSuppliedMessage(object arg0) => WebApiResources.Format(nameof (PreviewVersionNotSuppliedMessage), arg0);

    public static string PreviewVersionNotSuppliedMessage(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (PreviewVersionNotSuppliedMessage), culture, arg0);

    public static string VersionNotSuppliedMessage(object arg0) => WebApiResources.Format(nameof (VersionNotSuppliedMessage), arg0);

    public static string VersionNotSuppliedMessage(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (VersionNotSuppliedMessage), culture, arg0);

    public static string MustacheTemplateInvalidEndBlock(object arg0) => WebApiResources.Format(nameof (MustacheTemplateInvalidEndBlock), arg0);

    public static string MustacheTemplateInvalidEndBlock(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (MustacheTemplateInvalidEndBlock), culture, arg0);

    public static string MustacheTemplateMissingBlockHelper(object arg0, object arg1) => WebApiResources.Format(nameof (MustacheTemplateMissingBlockHelper), arg0, arg1);

    public static string MustacheTemplateMissingBlockHelper(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WebApiResources.Format(nameof (MustacheTemplateMissingBlockHelper), culture, arg0, arg1);
    }

    public static string MustacheTemplateMissingHelper(object arg0, object arg1) => WebApiResources.Format(nameof (MustacheTemplateMissingHelper), arg0, arg1);

    public static string MustacheTemplateMissingHelper(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WebApiResources.Format(nameof (MustacheTemplateMissingHelper), culture, arg0, arg1);
    }

    public static string MustacheTemplateNonMatchingEndBlock(object arg0, object arg1) => WebApiResources.Format(nameof (MustacheTemplateNonMatchingEndBlock), arg0, arg1);

    public static string MustacheTemplateNonMatchingEndBlock(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WebApiResources.Format(nameof (MustacheTemplateNonMatchingEndBlock), culture, arg0, arg1);
    }

    public static string MustacheTemplateBraceCountMismatch(object arg0) => WebApiResources.Format(nameof (MustacheTemplateBraceCountMismatch), arg0);

    public static string MustacheTemplateBraceCountMismatch(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (MustacheTemplateBraceCountMismatch), culture, arg0);

    public static string MustacheTemplateInvalidEndBraces(object arg0, object arg1) => WebApiResources.Format(nameof (MustacheTemplateInvalidEndBraces), arg0, arg1);

    public static string MustacheTemplateInvalidEndBraces(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WebApiResources.Format(nameof (MustacheTemplateInvalidEndBraces), culture, arg0, arg1);
    }

    public static string MustacheTemplateInvalidStartBraces(object arg0, object arg1, object arg2) => WebApiResources.Format(nameof (MustacheTemplateInvalidStartBraces), arg0, arg1, arg2);

    public static string MustacheTemplateInvalidStartBraces(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return WebApiResources.Format(nameof (MustacheTemplateInvalidStartBraces), culture, arg0, arg1, arg2);
    }

    public static string MustacheTemplateInvalidEscapedStringLiteral(object arg0, object arg1) => WebApiResources.Format(nameof (MustacheTemplateInvalidEscapedStringLiteral), arg0, arg1);

    public static string MustacheTemplateInvalidEscapedStringLiteral(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WebApiResources.Format(nameof (MustacheTemplateInvalidEscapedStringLiteral), culture, arg0, arg1);
    }

    public static string MustacheTemplateUnterminatedStringLiteral(object arg0, object arg1) => WebApiResources.Format(nameof (MustacheTemplateUnterminatedStringLiteral), arg0, arg1);

    public static string MustacheTemplateUnterminatedStringLiteral(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WebApiResources.Format(nameof (MustacheTemplateUnterminatedStringLiteral), culture, arg0, arg1);
    }

    public static string MustacheTemplateInvalidNumericLiteral(object arg0, object arg1) => WebApiResources.Format(nameof (MustacheTemplateInvalidNumericLiteral), arg0, arg1);

    public static string MustacheTemplateInvalidNumericLiteral(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WebApiResources.Format(nameof (MustacheTemplateInvalidNumericLiteral), culture, arg0, arg1);
    }

    public static string OperationNotFoundException(object arg0) => WebApiResources.Format(nameof (OperationNotFoundException), arg0);

    public static string OperationNotFoundException(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (OperationNotFoundException), culture, arg0);

    public static string OperationPluginNotFoundException(object arg0) => WebApiResources.Format(nameof (OperationPluginNotFoundException), arg0);

    public static string OperationPluginNotFoundException(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (OperationPluginNotFoundException), culture, arg0);

    public static string OperationPluginWithSameIdException(object arg0) => WebApiResources.Format(nameof (OperationPluginWithSameIdException), arg0);

    public static string OperationPluginWithSameIdException(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (OperationPluginWithSameIdException), culture, arg0);

    public static string OperationPluginNoPermission(object arg0, object arg1) => WebApiResources.Format(nameof (OperationPluginNoPermission), arg0, arg1);

    public static string OperationPluginNoPermission(object arg0, object arg1, CultureInfo culture) => WebApiResources.Format(nameof (OperationPluginNoPermission), culture, arg0, arg1);

    public static string OperationUpdateException(object arg0) => WebApiResources.Format(nameof (OperationUpdateException), arg0);

    public static string OperationUpdateException(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (OperationUpdateException), culture, arg0);

    public static string CollectionDoesNotExistException(object arg0) => WebApiResources.Format(nameof (CollectionDoesNotExistException), arg0);

    public static string CollectionDoesNotExistException(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (CollectionDoesNotExistException), culture, arg0);

    public static string MissingCloseInlineMessage() => WebApiResources.Get(nameof (MissingCloseInlineMessage));

    public static string MissingCloseInlineMessage(CultureInfo culture) => WebApiResources.Get(nameof (MissingCloseInlineMessage), culture);

    public static string MissingEndingBracesMessage(object arg0) => WebApiResources.Format(nameof (MissingEndingBracesMessage), arg0);

    public static string MissingEndingBracesMessage(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (MissingEndingBracesMessage), culture, arg0);

    public static string NestedInlinePartialsMessage() => WebApiResources.Get(nameof (NestedInlinePartialsMessage));

    public static string NestedInlinePartialsMessage(CultureInfo culture) => WebApiResources.Get(nameof (NestedInlinePartialsMessage), culture);

    public static string GetServiceArgumentError(object arg0) => WebApiResources.Format(nameof (GetServiceArgumentError), arg0);

    public static string GetServiceArgumentError(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (GetServiceArgumentError), culture, arg0);

    public static string ExtensibleServiceTypeNotRegistered(object arg0) => WebApiResources.Format(nameof (ExtensibleServiceTypeNotRegistered), arg0);

    public static string ExtensibleServiceTypeNotRegistered(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (ExtensibleServiceTypeNotRegistered), culture, arg0);

    public static string ExtensibleServiceTypeNotValid(object arg0, object arg1) => WebApiResources.Format(nameof (ExtensibleServiceTypeNotValid), arg0, arg1);

    public static string ExtensibleServiceTypeNotValid(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WebApiResources.Format(nameof (ExtensibleServiceTypeNotValid), culture, arg0, arg1);
    }

    public static string ServerDataProviderNotFound(object arg0) => WebApiResources.Format(nameof (ServerDataProviderNotFound), arg0);

    public static string ServerDataProviderNotFound(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (ServerDataProviderNotFound), culture, arg0);

    public static string ClientCertificateMissing(object arg0) => WebApiResources.Format(nameof (ClientCertificateMissing), arg0);

    public static string ClientCertificateMissing(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (ClientCertificateMissing), culture, arg0);

    public static string SmartCardMissing(object arg0) => WebApiResources.Format(nameof (SmartCardMissing), arg0);

    public static string SmartCardMissing(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (SmartCardMissing), culture, arg0);

    public static string ClientCertificateNoPermission(object arg0) => WebApiResources.Format(nameof (ClientCertificateNoPermission), arg0);

    public static string ClientCertificateNoPermission(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (ClientCertificateNoPermission), culture, arg0);

    public static string ClientCertificateErrorReadingStore(object arg0) => WebApiResources.Format(nameof (ClientCertificateErrorReadingStore), arg0);

    public static string ClientCertificateErrorReadingStore(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (ClientCertificateErrorReadingStore), culture, arg0);

    public static string CannotAuthenticateAsAnotherUser(object arg0, object arg1) => WebApiResources.Format(nameof (CannotAuthenticateAsAnotherUser), arg0, arg1);

    public static string CannotAuthenticateAsAnotherUser(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WebApiResources.Format(nameof (CannotAuthenticateAsAnotherUser), culture, arg0, arg1);
    }

    public static string MustacheTemplateInvalidPartialReference(object arg0) => WebApiResources.Format(nameof (MustacheTemplateInvalidPartialReference), arg0);

    public static string MustacheTemplateInvalidPartialReference(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (MustacheTemplateInvalidPartialReference), culture, arg0);

    public static string CannotGetUnattributedClient(object arg0) => WebApiResources.Format(nameof (CannotGetUnattributedClient), arg0);

    public static string CannotGetUnattributedClient(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (CannotGetUnattributedClient), culture, arg0);

    public static string UnknownEntityType(object arg0) => WebApiResources.Format(nameof (UnknownEntityType), arg0);

    public static string UnknownEntityType(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (UnknownEntityType), culture, arg0);

    public static string GraphGroupMissingRequiredFields() => WebApiResources.Get(nameof (GraphGroupMissingRequiredFields));

    public static string GraphGroupMissingRequiredFields(CultureInfo culture) => WebApiResources.Get(nameof (GraphGroupMissingRequiredFields), culture);

    public static string GraphUserMissingRequiredFields() => WebApiResources.Get(nameof (GraphUserMissingRequiredFields));

    public static string GraphUserMissingRequiredFields(CultureInfo culture) => WebApiResources.Get(nameof (GraphUserMissingRequiredFields), culture);

    public static string GraphServicePrincipalMissingRequiredFields() => WebApiResources.Get(nameof (GraphServicePrincipalMissingRequiredFields));

    public static string GraphServicePrincipalMissingRequiredFields(CultureInfo culture) => WebApiResources.Get(nameof (GraphServicePrincipalMissingRequiredFields), culture);

    public static string MustacheEvaluationResultLengthExceeded(object arg0) => WebApiResources.Format(nameof (MustacheEvaluationResultLengthExceeded), arg0);

    public static string MustacheEvaluationResultLengthExceeded(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (MustacheEvaluationResultLengthExceeded), culture, arg0);

    public static string MustacheTemplateInlinePartialsNotAllowed() => WebApiResources.Get(nameof (MustacheTemplateInlinePartialsNotAllowed));

    public static string MustacheTemplateInlinePartialsNotAllowed(CultureInfo culture) => WebApiResources.Get(nameof (MustacheTemplateInlinePartialsNotAllowed), culture);

    public static string MustacheTemplateMaxDepthExceeded(object arg0) => WebApiResources.Format(nameof (MustacheTemplateMaxDepthExceeded), arg0);

    public static string MustacheTemplateMaxDepthExceeded(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (MustacheTemplateMaxDepthExceeded), culture, arg0);

    public static string UnexpectedTokenType() => WebApiResources.Get(nameof (UnexpectedTokenType));

    public static string UnexpectedTokenType(CultureInfo culture) => WebApiResources.Get(nameof (UnexpectedTokenType), culture);

    public static string ApiVersionOutOfRange(object arg0, object arg1) => WebApiResources.Format(nameof (ApiVersionOutOfRange), arg0, arg1);

    public static string ApiVersionOutOfRange(object arg0, object arg1, CultureInfo culture) => WebApiResources.Format(nameof (ApiVersionOutOfRange), culture, arg0, arg1);

    public static string ApiVersionOutOfRangeForRoute(object arg0, object arg1) => WebApiResources.Format(nameof (ApiVersionOutOfRangeForRoute), arg0, arg1);

    public static string ApiVersionOutOfRangeForRoute(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WebApiResources.Format(nameof (ApiVersionOutOfRangeForRoute), culture, arg0, arg1);
    }

    public static string ApiVersionOutOfRangeForRoutes(object arg0, object arg1) => WebApiResources.Format(nameof (ApiVersionOutOfRangeForRoutes), arg0, arg1);

    public static string ApiVersionOutOfRangeForRoutes(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WebApiResources.Format(nameof (ApiVersionOutOfRangeForRoutes), culture, arg0, arg1);
    }

    public static string UnsafeCrossOriginRequest(object arg0) => WebApiResources.Format(nameof (UnsafeCrossOriginRequest), arg0);

    public static string UnsafeCrossOriginRequest(object arg0, CultureInfo culture) => WebApiResources.Format(nameof (UnsafeCrossOriginRequest), culture, arg0);
  }
}
