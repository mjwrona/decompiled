// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Internal.CommonResources
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class CommonResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (CommonResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => CommonResources.s_resMgr;

    private static string Get(string resourceName) => CommonResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? CommonResources.Get(resourceName) : CommonResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) CommonResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? CommonResources.GetInt(resourceName) : (int) CommonResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) CommonResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? CommonResources.GetBool(resourceName) : (bool) CommonResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => CommonResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = CommonResources.Get(resourceName, culture);
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

    public static string EmptyCollectionNotAllowed() => CommonResources.Get(nameof (EmptyCollectionNotAllowed));

    public static string EmptyCollectionNotAllowed(CultureInfo culture) => CommonResources.Get(nameof (EmptyCollectionNotAllowed), culture);

    public static string EmptyStringNotAllowed() => CommonResources.Get(nameof (EmptyStringNotAllowed));

    public static string EmptyStringNotAllowed(CultureInfo culture) => CommonResources.Get(nameof (EmptyStringNotAllowed), culture);

    public static string StringLengthNotAllowed(object arg0, object arg1, object arg2) => CommonResources.Format(nameof (StringLengthNotAllowed), arg0, arg1, arg2);

    public static string StringLengthNotAllowed(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return CommonResources.Format(nameof (StringLengthNotAllowed), culture, arg0, arg1, arg2);
    }

    public static string EmptyGuidNotAllowed(object arg0) => CommonResources.Format(nameof (EmptyGuidNotAllowed), arg0);

    public static string EmptyGuidNotAllowed(object arg0, CultureInfo culture) => CommonResources.Format(nameof (EmptyGuidNotAllowed), culture, arg0);

    public static string InvalidPropertyName(object arg0) => CommonResources.Format(nameof (InvalidPropertyName), arg0);

    public static string InvalidPropertyName(object arg0, CultureInfo culture) => CommonResources.Format(nameof (InvalidPropertyName), culture, arg0);

    public static string InvalidPropertyValueSize(object arg0, object arg1, object arg2) => CommonResources.Format(nameof (InvalidPropertyValueSize), arg0, arg1, arg2);

    public static string InvalidPropertyValueSize(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return CommonResources.Format(nameof (InvalidPropertyValueSize), culture, arg0, arg1, arg2);
    }

    public static string DateTimeKindMustBeSpecified() => CommonResources.Get(nameof (DateTimeKindMustBeSpecified));

    public static string DateTimeKindMustBeSpecified(CultureInfo culture) => CommonResources.Get(nameof (DateTimeKindMustBeSpecified), culture);

    public static string PropertyArgumentExceededMaximumSizeAllowed(object arg0, object arg1) => CommonResources.Format(nameof (PropertyArgumentExceededMaximumSizeAllowed), arg0, arg1);

    public static string PropertyArgumentExceededMaximumSizeAllowed(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return CommonResources.Format(nameof (PropertyArgumentExceededMaximumSizeAllowed), culture, arg0, arg1);
    }

    public static string InvalidStringPropertyValueNullAllowed(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4)
    {
      return CommonResources.Format(nameof (InvalidStringPropertyValueNullAllowed), arg0, arg1, arg2, arg3, arg4);
    }

    public static string InvalidStringPropertyValueNullAllowed(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      CultureInfo culture)
    {
      return CommonResources.Format(nameof (InvalidStringPropertyValueNullAllowed), culture, arg0, arg1, arg2, arg3, arg4);
    }

    public static string InvalidStringPropertyValueNullForbidden(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4)
    {
      return CommonResources.Format(nameof (InvalidStringPropertyValueNullForbidden), arg0, arg1, arg2, arg3, arg4);
    }

    public static string InvalidStringPropertyValueNullForbidden(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      CultureInfo culture)
    {
      return CommonResources.Format(nameof (InvalidStringPropertyValueNullForbidden), culture, arg0, arg1, arg2, arg3, arg4);
    }

    public static string ValueTypeOutOfRange(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4)
    {
      return CommonResources.Format(nameof (ValueTypeOutOfRange), arg0, arg1, arg2, arg3, arg4);
    }

    public static string ValueTypeOutOfRange(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      CultureInfo culture)
    {
      return CommonResources.Format(nameof (ValueTypeOutOfRange), culture, arg0, arg1, arg2, arg3, arg4);
    }

    public static string VssPropertyValueOutOfRange(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return CommonResources.Format(nameof (VssPropertyValueOutOfRange), arg0, arg1, arg2, arg3);
    }

    public static string VssPropertyValueOutOfRange(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return CommonResources.Format(nameof (VssPropertyValueOutOfRange), culture, arg0, arg1, arg2, arg3);
    }

    public static string VssInvalidUnicodeCharacter(object arg0) => CommonResources.Format(nameof (VssInvalidUnicodeCharacter), arg0);

    public static string VssInvalidUnicodeCharacter(object arg0, CultureInfo culture) => CommonResources.Format(nameof (VssInvalidUnicodeCharacter), culture, arg0);

    public static string ErrorReadingFile(object arg0, object arg1) => CommonResources.Format(nameof (ErrorReadingFile), arg0, arg1);

    public static string ErrorReadingFile(object arg0, object arg1, CultureInfo culture) => CommonResources.Format(nameof (ErrorReadingFile), culture, arg0, arg1);

    public static string IllegalBase64String() => CommonResources.Get(nameof (IllegalBase64String));

    public static string IllegalBase64String(CultureInfo culture) => CommonResources.Get(nameof (IllegalBase64String), culture);

    public static string CannotPromptIfNonInteractive() => CommonResources.Get(nameof (CannotPromptIfNonInteractive));

    public static string CannotPromptIfNonInteractive(CultureInfo culture) => CommonResources.Get(nameof (CannotPromptIfNonInteractive), culture);

    public static string StringContainsInvalidCharacters(object arg0) => CommonResources.Format(nameof (StringContainsInvalidCharacters), arg0);

    public static string StringContainsInvalidCharacters(object arg0, CultureInfo culture) => CommonResources.Format(nameof (StringContainsInvalidCharacters), culture, arg0);

    public static string DoubleValueOutOfRange(object arg0, object arg1) => CommonResources.Format(nameof (DoubleValueOutOfRange), arg0, arg1);

    public static string DoubleValueOutOfRange(object arg0, object arg1, CultureInfo culture) => CommonResources.Format(nameof (DoubleValueOutOfRange), culture, arg0, arg1);

    public static string HttpRequestTimeout(object arg0) => CommonResources.Format(nameof (HttpRequestTimeout), arg0);

    public static string HttpRequestTimeout(object arg0, CultureInfo culture) => CommonResources.Format(nameof (HttpRequestTimeout), culture, arg0);

    public static string VssUnauthorized(object arg0) => CommonResources.Format(nameof (VssUnauthorized), arg0);

    public static string VssUnauthorized(object arg0, CultureInfo culture) => CommonResources.Format(nameof (VssUnauthorized), culture, arg0);

    public static string VssUnauthorizedUnknownServer() => CommonResources.Get(nameof (VssUnauthorizedUnknownServer));

    public static string VssUnauthorizedUnknownServer(CultureInfo culture) => CommonResources.Get(nameof (VssUnauthorizedUnknownServer), culture);

    public static string XmlAttributeEmpty(object arg0, object arg1) => CommonResources.Format(nameof (XmlAttributeEmpty), arg0, arg1);

    public static string XmlAttributeEmpty(object arg0, object arg1, CultureInfo culture) => CommonResources.Format(nameof (XmlAttributeEmpty), culture, arg0, arg1);

    public static string XmlAttributeNull(object arg0, object arg1) => CommonResources.Format(nameof (XmlAttributeNull), arg0, arg1);

    public static string XmlAttributeNull(object arg0, object arg1, CultureInfo culture) => CommonResources.Format(nameof (XmlAttributeNull), culture, arg0, arg1);

    public static string XmlNodeEmpty(object arg0, object arg1) => CommonResources.Format(nameof (XmlNodeEmpty), arg0, arg1);

    public static string XmlNodeEmpty(object arg0, object arg1, CultureInfo culture) => CommonResources.Format(nameof (XmlNodeEmpty), culture, arg0, arg1);

    public static string XmlNodeMissing(object arg0, object arg1) => CommonResources.Format(nameof (XmlNodeMissing), arg0, arg1);

    public static string XmlNodeMissing(object arg0, object arg1, CultureInfo culture) => CommonResources.Format(nameof (XmlNodeMissing), culture, arg0, arg1);

    public static string VssUnsupportedPropertyValueType(object arg0, object arg1) => CommonResources.Format(nameof (VssUnsupportedPropertyValueType), arg0, arg1);

    public static string VssUnsupportedPropertyValueType(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return CommonResources.Format(nameof (VssUnsupportedPropertyValueType), culture, arg0, arg1);
    }

    public static string ErrorDependencyOptionNotProvided(object arg0, object arg1) => CommonResources.Format(nameof (ErrorDependencyOptionNotProvided), arg0, arg1);

    public static string ErrorDependencyOptionNotProvided(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return CommonResources.Format(nameof (ErrorDependencyOptionNotProvided), culture, arg0, arg1);
    }

    public static string ErrorInvalidEnumValueTypeConversion(object arg0) => CommonResources.Format(nameof (ErrorInvalidEnumValueTypeConversion), arg0);

    public static string ErrorInvalidEnumValueTypeConversion(object arg0, CultureInfo culture) => CommonResources.Format(nameof (ErrorInvalidEnumValueTypeConversion), culture, arg0);

    public static string ErrorInvalidResponseFileOption(object arg0) => CommonResources.Format(nameof (ErrorInvalidResponseFileOption), arg0);

    public static string ErrorInvalidResponseFileOption(object arg0, CultureInfo culture) => CommonResources.Format(nameof (ErrorInvalidResponseFileOption), culture, arg0);

    public static string ErrorInvalidValueTypeConversion(object arg0, object arg1) => CommonResources.Format(nameof (ErrorInvalidValueTypeConversion), arg0, arg1);

    public static string ErrorInvalidValueTypeConversion(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return CommonResources.Format(nameof (ErrorInvalidValueTypeConversion), culture, arg0, arg1);
    }

    public static string ErrorOptionArgumentsNotDefined() => CommonResources.Get(nameof (ErrorOptionArgumentsNotDefined));

    public static string ErrorOptionArgumentsNotDefined(CultureInfo culture) => CommonResources.Get(nameof (ErrorOptionArgumentsNotDefined), culture);

    public static string ErrorOptionMultiplesNotAllowed(object arg0) => CommonResources.Format(nameof (ErrorOptionMultiplesNotAllowed), arg0);

    public static string ErrorOptionMultiplesNotAllowed(object arg0, CultureInfo culture) => CommonResources.Format(nameof (ErrorOptionMultiplesNotAllowed), culture, arg0);

    public static string ErrorOptionMustExist(object arg0) => CommonResources.Format(nameof (ErrorOptionMustExist), arg0);

    public static string ErrorOptionMustExist(object arg0, CultureInfo culture) => CommonResources.Format(nameof (ErrorOptionMustExist), culture, arg0);

    public static string ErrorOptionNotRecognized(object arg0) => CommonResources.Format(nameof (ErrorOptionNotRecognized), arg0);

    public static string ErrorOptionNotRecognized(object arg0, CultureInfo culture) => CommonResources.Format(nameof (ErrorOptionNotRecognized), culture, arg0);

    public static string ErrorOptionRequired(object arg0) => CommonResources.Format(nameof (ErrorOptionRequired), arg0);

    public static string ErrorOptionRequired(object arg0, CultureInfo culture) => CommonResources.Format(nameof (ErrorOptionRequired), culture, arg0);

    public static string ErrorOptionRequiresValue(object arg0) => CommonResources.Format(nameof (ErrorOptionRequiresValue), arg0);

    public static string ErrorOptionRequiresValue(object arg0, CultureInfo culture) => CommonResources.Format(nameof (ErrorOptionRequiresValue), culture, arg0);

    public static string ErrorOptionRunsDoNotSupportValues() => CommonResources.Get(nameof (ErrorOptionRunsDoNotSupportValues));

    public static string ErrorOptionRunsDoNotSupportValues(CultureInfo culture) => CommonResources.Get(nameof (ErrorOptionRunsDoNotSupportValues), culture);

    public static string ErrorOptionsAreMutuallyExclusive(object arg0) => CommonResources.Format(nameof (ErrorOptionsAreMutuallyExclusive), arg0);

    public static string ErrorOptionsAreMutuallyExclusive(object arg0, CultureInfo culture) => CommonResources.Format(nameof (ErrorOptionsAreMutuallyExclusive), culture, arg0);

    public static string ErrorOptionsAreMutuallyInclusive(object arg0) => CommonResources.Format(nameof (ErrorOptionsAreMutuallyInclusive), arg0);

    public static string ErrorOptionsAreMutuallyInclusive(object arg0, CultureInfo culture) => CommonResources.Format(nameof (ErrorOptionsAreMutuallyInclusive), culture, arg0);

    public static string ErrorOptionValueConverterNotFound(object arg0) => CommonResources.Format(nameof (ErrorOptionValueConverterNotFound), arg0);

    public static string ErrorOptionValueConverterNotFound(object arg0, CultureInfo culture) => CommonResources.Format(nameof (ErrorOptionValueConverterNotFound), culture, arg0);

    public static string ErrorOptionValueNotAllowed(object arg0) => CommonResources.Format(nameof (ErrorOptionValueNotAllowed), arg0);

    public static string ErrorOptionValueNotAllowed(object arg0, CultureInfo culture) => CommonResources.Format(nameof (ErrorOptionValueNotAllowed), culture, arg0);

    public static string ErrorOptionValuesDoNotMatchExpected(object arg0, object arg1) => CommonResources.Format(nameof (ErrorOptionValuesDoNotMatchExpected), arg0, arg1);

    public static string ErrorOptionValuesDoNotMatchExpected(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return CommonResources.Format(nameof (ErrorOptionValuesDoNotMatchExpected), culture, arg0, arg1);
    }

    public static string ErrorPositionalArgumentsNotAllowed() => CommonResources.Get(nameof (ErrorPositionalArgumentsNotAllowed));

    public static string ErrorPositionalArgumentsNotAllowed(CultureInfo culture) => CommonResources.Get(nameof (ErrorPositionalArgumentsNotAllowed), culture);

    public static string ErrorRequiredOptionDoesNotExist(object arg0) => CommonResources.Format(nameof (ErrorRequiredOptionDoesNotExist), arg0);

    public static string ErrorRequiredOptionDoesNotExist(object arg0, CultureInfo culture) => CommonResources.Format(nameof (ErrorRequiredOptionDoesNotExist), culture, arg0);

    public static string ErrorResponseFileNotFound(object arg0) => CommonResources.Format(nameof (ErrorResponseFileNotFound), arg0);

    public static string ErrorResponseFileNotFound(object arg0, CultureInfo culture) => CommonResources.Format(nameof (ErrorResponseFileNotFound), culture, arg0);

    public static string ErrorResponseFileOptionNotSupported() => CommonResources.Get(nameof (ErrorResponseFileOptionNotSupported));

    public static string ErrorResponseFileOptionNotSupported(CultureInfo culture) => CommonResources.Get(nameof (ErrorResponseFileOptionNotSupported), culture);

    public static string ErrorValueCannotBeConvertedToEnum(object arg0, object arg1) => CommonResources.Format(nameof (ErrorValueCannotBeConvertedToEnum), arg0, arg1);

    public static string ErrorValueCannotBeConvertedToEnum(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return CommonResources.Format(nameof (ErrorValueCannotBeConvertedToEnum), culture, arg0, arg1);
    }

    public static string OperationHandlerNotFound(object arg0) => CommonResources.Format(nameof (OperationHandlerNotFound), arg0);

    public static string OperationHandlerNotFound(object arg0, CultureInfo culture) => CommonResources.Format(nameof (OperationHandlerNotFound), culture, arg0);

    public static string ErrorInvalidValueConverterOrNoDefaultFound(object arg0) => CommonResources.Format(nameof (ErrorInvalidValueConverterOrNoDefaultFound), arg0);

    public static string ErrorInvalidValueConverterOrNoDefaultFound(
      object arg0,
      CultureInfo culture)
    {
      return CommonResources.Format(nameof (ErrorInvalidValueConverterOrNoDefaultFound), culture, arg0);
    }

    public static string ErrorOperationHandlerConstructorNotFound(object arg0) => CommonResources.Format(nameof (ErrorOperationHandlerConstructorNotFound), arg0);

    public static string ErrorOperationHandlerConstructorNotFound(object arg0, CultureInfo culture) => CommonResources.Format(nameof (ErrorOperationHandlerConstructorNotFound), culture, arg0);

    public static string ErrorOperationHandlerNotFound() => CommonResources.Get(nameof (ErrorOperationHandlerNotFound));

    public static string ErrorOperationHandlerNotFound(CultureInfo culture) => CommonResources.Get(nameof (ErrorOperationHandlerNotFound), culture);

    public static string ErrorDuplicateDefaultOperationModeHandlerFound() => CommonResources.Get(nameof (ErrorDuplicateDefaultOperationModeHandlerFound));

    public static string ErrorDuplicateDefaultOperationModeHandlerFound(CultureInfo culture) => CommonResources.Get(nameof (ErrorDuplicateDefaultOperationModeHandlerFound), culture);

    public static string ErrorDuplicateOperationModeHandlerFound() => CommonResources.Get(nameof (ErrorDuplicateOperationModeHandlerFound));

    public static string ErrorDuplicateOperationModeHandlerFound(CultureInfo culture) => CommonResources.Get(nameof (ErrorDuplicateOperationModeHandlerFound), culture);

    public static string ErrorInvalidValueConverterDataType(object arg0, object arg1) => CommonResources.Format(nameof (ErrorInvalidValueConverterDataType), arg0, arg1);

    public static string ErrorInvalidValueConverterDataType(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return CommonResources.Format(nameof (ErrorInvalidValueConverterDataType), culture, arg0, arg1);
    }

    public static string ErrorMembersContainingPositionalsRequireCollection() => CommonResources.Get(nameof (ErrorMembersContainingPositionalsRequireCollection));

    public static string ErrorMembersContainingPositionalsRequireCollection(CultureInfo culture) => CommonResources.Get(nameof (ErrorMembersContainingPositionalsRequireCollection), culture);

    public static string ErrorDuplicatePositionalOptionAttributes(object arg0) => CommonResources.Format(nameof (ErrorDuplicatePositionalOptionAttributes), arg0);

    public static string ErrorDuplicatePositionalOptionAttributes(object arg0, CultureInfo culture) => CommonResources.Format(nameof (ErrorDuplicatePositionalOptionAttributes), culture, arg0);

    public static string ErrorOptionsAllowingMultiplesRequireCollection(object arg0) => CommonResources.Format(nameof (ErrorOptionsAllowingMultiplesRequireCollection), arg0);

    public static string ErrorOptionsAllowingMultiplesRequireCollection(
      object arg0,
      CultureInfo culture)
    {
      return CommonResources.Format(nameof (ErrorOptionsAllowingMultiplesRequireCollection), culture, arg0);
    }

    public static string ErrorOptionNotFound(object arg0) => CommonResources.Format(nameof (ErrorOptionNotFound), arg0);

    public static string ErrorOptionNotFound(object arg0, CultureInfo culture) => CommonResources.Format(nameof (ErrorOptionNotFound), culture, arg0);

    public static string ErrorOptionFlagRequiresBooleanMember(object arg0) => CommonResources.Format(nameof (ErrorOptionFlagRequiresBooleanMember), arg0);

    public static string ErrorOptionFlagRequiresBooleanMember(object arg0, CultureInfo culture) => CommonResources.Format(nameof (ErrorOptionFlagRequiresBooleanMember), culture, arg0);

    public static string ContentIdCalculationBlockSizeError(object arg0) => CommonResources.Format(nameof (ContentIdCalculationBlockSizeError), arg0);

    public static string ContentIdCalculationBlockSizeError(object arg0, CultureInfo culture) => CommonResources.Format(nameof (ContentIdCalculationBlockSizeError), culture, arg0);

    public static string BasicAuthenticationRequiresSsl() => CommonResources.Get(nameof (BasicAuthenticationRequiresSsl));

    public static string BasicAuthenticationRequiresSsl(CultureInfo culture) => CommonResources.Get(nameof (BasicAuthenticationRequiresSsl), culture);

    public static string ValueOutOfRange(object arg0, object arg1, object arg2, object arg3) => CommonResources.Format(nameof (ValueOutOfRange), arg0, arg1, arg2, arg3);

    public static string ValueOutOfRange(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return CommonResources.Format(nameof (ValueOutOfRange), culture, arg0, arg1, arg2, arg3);
    }

    public static string OutOfRange(object arg0) => CommonResources.Format(nameof (OutOfRange), arg0);

    public static string OutOfRange(object arg0, CultureInfo culture) => CommonResources.Format(nameof (OutOfRange), culture, arg0);

    public static string ValueMustBeGreaterThanZero() => CommonResources.Get(nameof (ValueMustBeGreaterThanZero));

    public static string ValueMustBeGreaterThanZero(CultureInfo culture) => CommonResources.Get(nameof (ValueMustBeGreaterThanZero), culture);

    public static string NullValueNecessary(object arg0) => CommonResources.Format(nameof (NullValueNecessary), arg0);

    public static string NullValueNecessary(object arg0, CultureInfo culture) => CommonResources.Format(nameof (NullValueNecessary), culture, arg0);

    public static string LowercaseStringRequired(object arg0) => CommonResources.Format(nameof (LowercaseStringRequired), arg0);

    public static string LowercaseStringRequired(object arg0, CultureInfo culture) => CommonResources.Format(nameof (LowercaseStringRequired), culture, arg0);

    public static string UppercaseStringRequired(object arg0) => CommonResources.Format(nameof (UppercaseStringRequired), arg0);

    public static string UppercaseStringRequired(object arg0, CultureInfo culture) => CommonResources.Format(nameof (UppercaseStringRequired), culture, arg0);

    public static string EmptyArrayNotAllowed() => CommonResources.Get(nameof (EmptyArrayNotAllowed));

    public static string EmptyArrayNotAllowed(CultureInfo culture) => CommonResources.Get(nameof (EmptyArrayNotAllowed), culture);

    public static string EmptyOrWhiteSpaceStringNotAllowed() => CommonResources.Get(nameof (EmptyOrWhiteSpaceStringNotAllowed));

    public static string EmptyOrWhiteSpaceStringNotAllowed(CultureInfo culture) => CommonResources.Get(nameof (EmptyOrWhiteSpaceStringNotAllowed), culture);

    public static string StringLengthNotMatch(object arg0) => CommonResources.Format(nameof (StringLengthNotMatch), arg0);

    public static string StringLengthNotMatch(object arg0, CultureInfo culture) => CommonResources.Format(nameof (StringLengthNotMatch), culture, arg0);

    public static string BothStringsCannotBeNull(object arg0, object arg1) => CommonResources.Format(nameof (BothStringsCannotBeNull), arg0, arg1);

    public static string BothStringsCannotBeNull(object arg0, object arg1, CultureInfo culture) => CommonResources.Format(nameof (BothStringsCannotBeNull), culture, arg0, arg1);

    public static string WhiteSpaceNotAllowed() => CommonResources.Get(nameof (WhiteSpaceNotAllowed));

    public static string WhiteSpaceNotAllowed(CultureInfo culture) => CommonResources.Get(nameof (WhiteSpaceNotAllowed), culture);

    public static string UnexpectedType(object arg0, object arg1) => CommonResources.Format(nameof (UnexpectedType), arg0, arg1);

    public static string UnexpectedType(object arg0, object arg1, CultureInfo culture) => CommonResources.Format(nameof (UnexpectedType), culture, arg0, arg1);

    public static string InvalidEmailAddressError() => CommonResources.Get(nameof (InvalidEmailAddressError));

    public static string InvalidEmailAddressError(CultureInfo culture) => CommonResources.Get(nameof (InvalidEmailAddressError), culture);

    public static string AbsoluteVirtualPathNotAllowed(object arg0) => CommonResources.Format(nameof (AbsoluteVirtualPathNotAllowed), arg0);

    public static string AbsoluteVirtualPathNotAllowed(object arg0, CultureInfo culture) => CommonResources.Format(nameof (AbsoluteVirtualPathNotAllowed), culture, arg0);

    public static string UriUtility_AbsoluteUriRequired(object arg0) => CommonResources.Format(nameof (UriUtility_AbsoluteUriRequired), arg0);

    public static string UriUtility_AbsoluteUriRequired(object arg0, CultureInfo culture) => CommonResources.Format(nameof (UriUtility_AbsoluteUriRequired), culture, arg0);

    public static string UriUtility_RelativePathInvalid(object arg0) => CommonResources.Format(nameof (UriUtility_RelativePathInvalid), arg0);

    public static string UriUtility_RelativePathInvalid(object arg0, CultureInfo culture) => CommonResources.Format(nameof (UriUtility_RelativePathInvalid), culture, arg0);

    public static string UriUtility_UriNotAllowed(object arg0) => CommonResources.Format(nameof (UriUtility_UriNotAllowed), arg0);

    public static string UriUtility_UriNotAllowed(object arg0, CultureInfo culture) => CommonResources.Format(nameof (UriUtility_UriNotAllowed), culture, arg0);

    public static string UriUtility_MustBeAuthorityOnlyUri(object arg0, object arg1) => CommonResources.Format(nameof (UriUtility_MustBeAuthorityOnlyUri), arg0, arg1);

    public static string UriUtility_MustBeAuthorityOnlyUri(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return CommonResources.Format(nameof (UriUtility_MustBeAuthorityOnlyUri), culture, arg0, arg1);
    }

    public static string UrlNotValid() => CommonResources.Get(nameof (UrlNotValid));

    public static string UrlNotValid(CultureInfo culture) => CommonResources.Get(nameof (UrlNotValid), culture);

    public static string MalformedArtifactId(object arg0) => CommonResources.Format(nameof (MalformedArtifactId), arg0);

    public static string MalformedArtifactId(object arg0, CultureInfo culture) => CommonResources.Format(nameof (MalformedArtifactId), culture, arg0);

    public static string MalformedUri(object arg0) => CommonResources.Format(nameof (MalformedUri), arg0);

    public static string MalformedUri(object arg0, CultureInfo culture) => CommonResources.Format(nameof (MalformedUri), culture, arg0);

    public static string MalformedUrl(object arg0) => CommonResources.Format(nameof (MalformedUrl), arg0);

    public static string MalformedUrl(object arg0, CultureInfo culture) => CommonResources.Format(nameof (MalformedUrl), culture, arg0);

    public static string NullArtifactUrl() => CommonResources.Get(nameof (NullArtifactUrl));

    public static string NullArtifactUrl(CultureInfo culture) => CommonResources.Get(nameof (NullArtifactUrl), culture);

    public static string FailureGetArtifact() => CommonResources.Get(nameof (FailureGetArtifact));

    public static string FailureGetArtifact(CultureInfo culture) => CommonResources.Get(nameof (FailureGetArtifact), culture);

    public static string NullArtifactUriRoot() => CommonResources.Get(nameof (NullArtifactUriRoot));

    public static string NullArtifactUriRoot(CultureInfo culture) => CommonResources.Get(nameof (NullArtifactUriRoot), culture);

    public static string UnknownTypeForSerialization(object arg0) => CommonResources.Format(nameof (UnknownTypeForSerialization), arg0);

    public static string UnknownTypeForSerialization(object arg0, CultureInfo culture) => CommonResources.Format(nameof (UnknownTypeForSerialization), culture, arg0);

    public static string StringContainsIllegalChars() => CommonResources.Get(nameof (StringContainsIllegalChars));

    public static string StringContainsIllegalChars(CultureInfo culture) => CommonResources.Get(nameof (StringContainsIllegalChars), culture);

    public static string ValueEqualsToInfinity() => CommonResources.Get(nameof (ValueEqualsToInfinity));

    public static string ValueEqualsToInfinity(CultureInfo culture) => CommonResources.Get(nameof (ValueEqualsToInfinity), culture);

    public static string SingleBitRequired(object arg0) => CommonResources.Format(nameof (SingleBitRequired), arg0);

    public static string SingleBitRequired(object arg0, CultureInfo culture) => CommonResources.Format(nameof (SingleBitRequired), culture, arg0);

    public static string InvalidEnumArgument(object arg0, object arg1, object arg2) => CommonResources.Format(nameof (InvalidEnumArgument), arg0, arg1, arg2);

    public static string InvalidEnumArgument(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return CommonResources.Format(nameof (InvalidEnumArgument), culture, arg0, arg1, arg2);
    }

    public static string ConflictingPathSeparatorForVssFileStorage(
      object arg0,
      object arg1,
      object arg2)
    {
      return CommonResources.Format(nameof (ConflictingPathSeparatorForVssFileStorage), arg0, arg1, arg2);
    }

    public static string ConflictingPathSeparatorForVssFileStorage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return CommonResources.Format(nameof (ConflictingPathSeparatorForVssFileStorage), culture, arg0, arg1, arg2);
    }

    public static string ConflictingStringComparerForVssFileStorage(
      object arg0,
      object arg1,
      object arg2)
    {
      return CommonResources.Format(nameof (ConflictingStringComparerForVssFileStorage), arg0, arg1, arg2);
    }

    public static string ConflictingStringComparerForVssFileStorage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return CommonResources.Format(nameof (ConflictingStringComparerForVssFileStorage), culture, arg0, arg1, arg2);
    }

    public static string InvalidClientStoragePath(object arg0, object arg1) => CommonResources.Format(nameof (InvalidClientStoragePath), arg0, arg1);

    public static string InvalidClientStoragePath(object arg0, object arg1, CultureInfo culture) => CommonResources.Format(nameof (InvalidClientStoragePath), culture, arg0, arg1);

    public static string CollectionSizeLimitExceeded(object arg0, object arg1) => CommonResources.Format(nameof (CollectionSizeLimitExceeded), arg0, arg1);

    public static string CollectionSizeLimitExceeded(object arg0, object arg1, CultureInfo culture) => CommonResources.Format(nameof (CollectionSizeLimitExceeded), culture, arg0, arg1);

    public static string DefaultValueNotAllowed(object arg0) => CommonResources.Format(nameof (DefaultValueNotAllowed), arg0);

    public static string DefaultValueNotAllowed(object arg0, CultureInfo culture) => CommonResources.Format(nameof (DefaultValueNotAllowed), culture, arg0);

    public static string NullElementNotAllowedInCollection() => CommonResources.Get(nameof (NullElementNotAllowedInCollection));

    public static string NullElementNotAllowedInCollection(CultureInfo culture) => CommonResources.Get(nameof (NullElementNotAllowedInCollection), culture);

    public static string InvalidUriError(object arg0) => CommonResources.Format(nameof (InvalidUriError), arg0);

    public static string InvalidUriError(object arg0, CultureInfo culture) => CommonResources.Format(nameof (InvalidUriError), culture, arg0);

    public static string SubjectDescriptorEmpty(object arg0) => CommonResources.Format(nameof (SubjectDescriptorEmpty), arg0);

    public static string SubjectDescriptorEmpty(object arg0, CultureInfo culture) => CommonResources.Format(nameof (SubjectDescriptorEmpty), culture, arg0);

    public static string EUIILeakException(object arg0) => CommonResources.Format(nameof (EUIILeakException), arg0);

    public static string EUIILeakException(object arg0, CultureInfo culture) => CommonResources.Format(nameof (EUIILeakException), culture, arg0);

    public static string UnexpectedBoolValue(object arg0, object arg1, object arg2) => CommonResources.Format(nameof (UnexpectedBoolValue), arg0, arg1, arg2);

    public static string UnexpectedBoolValue(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return CommonResources.Format(nameof (UnexpectedBoolValue), culture, arg0, arg1, arg2);
    }

    public static string AvatarUtils_ContentMissingMessage() => CommonResources.Get(nameof (AvatarUtils_ContentMissingMessage));

    public static string AvatarUtils_ContentMissingMessage(CultureInfo culture) => CommonResources.Get(nameof (AvatarUtils_ContentMissingMessage), culture);

    public static string AvatarUtils_ImageParseError() => CommonResources.Get(nameof (AvatarUtils_ImageParseError));

    public static string AvatarUtils_ImageParseError(CultureInfo culture) => CommonResources.Get(nameof (AvatarUtils_ImageParseError), culture);

    public static string AvatarUtils_ImageTooLargeMessage(object arg0) => CommonResources.Format(nameof (AvatarUtils_ImageTooLargeMessage), arg0);

    public static string AvatarUtils_ImageTooLargeMessage(object arg0, CultureInfo culture) => CommonResources.Format(nameof (AvatarUtils_ImageTooLargeMessage), culture, arg0);

    public static string AvatarUtils_ImageZeroBytesMessage() => CommonResources.Get(nameof (AvatarUtils_ImageZeroBytesMessage));

    public static string AvatarUtils_ImageZeroBytesMessage(CultureInfo culture) => CommonResources.Get(nameof (AvatarUtils_ImageZeroBytesMessage), culture);
  }
}
