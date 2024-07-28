// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.TFCommonResources
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TFCommonResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (TFCommonResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => TFCommonResources.s_resMgr;

    private static string Get(string resourceName) => TFCommonResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? TFCommonResources.Get(resourceName) : TFCommonResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) TFCommonResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? TFCommonResources.GetInt(resourceName) : (int) TFCommonResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) TFCommonResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? TFCommonResources.GetBool(resourceName) : (bool) TFCommonResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => TFCommonResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = TFCommonResources.Get(resourceName, culture);
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

    public static string AccessToPathDenied(object arg0) => TFCommonResources.Format(nameof (AccessToPathDenied), arg0);

    public static string AccessToPathDenied(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (AccessToPathDenied), culture, arg0);

    public static string AssertionFailureHeader() => TFCommonResources.Get(nameof (AssertionFailureHeader));

    public static string AssertionFailureHeader(CultureInfo culture) => TFCommonResources.Get(nameof (AssertionFailureHeader), culture);

    public static string CannotDeleteDirectoryWithWritableFile(object arg0) => TFCommonResources.Format(nameof (CannotDeleteDirectoryWithWritableFile), arg0);

    public static string CannotDeleteDirectoryWithWritableFile(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (CannotDeleteDirectoryWithWritableFile), culture, arg0);

    public static string CannotDeleteDirectoryWithContents(object arg0) => TFCommonResources.Format(nameof (CannotDeleteDirectoryWithContents), arg0);

    public static string CannotDeleteDirectoryWithContents(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (CannotDeleteDirectoryWithContents), culture, arg0);

    public static string Category(object arg0) => TFCommonResources.Format(nameof (Category), arg0);

    public static string Category(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (Category), culture, arg0);

    public static string CommandFailedWithExitCode(object arg0, object arg1) => TFCommonResources.Format(nameof (CommandFailedWithExitCode), arg0, arg1);

    public static string CommandFailedWithExitCode(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (CommandFailedWithExitCode), culture, arg0, arg1);

    public static string EmptyStringNotAllowed() => TFCommonResources.Get(nameof (EmptyStringNotAllowed));

    public static string EmptyStringNotAllowed(CultureInfo culture) => TFCommonResources.Get(nameof (EmptyStringNotAllowed), culture);

    public static string EmptyGuidNotAllowed(object arg0) => TFCommonResources.Format(nameof (EmptyGuidNotAllowed), arg0);

    public static string EmptyGuidNotAllowed(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (EmptyGuidNotAllowed), culture, arg0);

    public static string Exception() => TFCommonResources.Get(nameof (Exception));

    public static string Exception(CultureInfo culture) => TFCommonResources.Get(nameof (Exception), culture);

    public static string ExceptionDataDictionaryReport(object arg0, object arg1) => TFCommonResources.Format(nameof (ExceptionDataDictionaryReport), arg0, arg1);

    public static string ExceptionDataDictionaryReport(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (ExceptionDataDictionaryReport), culture, arg0, arg1);
    }

    public static string ExceptionReport(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6,
      object arg7,
      object arg8,
      object arg9,
      object arg10)
    {
      return TFCommonResources.Format(nameof (ExceptionReport), arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
    }

    public static string ExceptionReport(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6,
      object arg7,
      object arg8,
      object arg9,
      object arg10,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (ExceptionReport), culture, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
    }

    public static string ExceptionReportNoDetails(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6,
      object arg7,
      object arg8,
      object arg9)
    {
      return TFCommonResources.Format(nameof (ExceptionReportNoDetails), arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
    }

    public static string ExceptionReportNoDetails(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6,
      object arg7,
      object arg8,
      object arg9,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (ExceptionReportNoDetails), culture, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
    }

    public static string ExceptionSource() => TFCommonResources.Get(nameof (ExceptionSource));

    public static string ExceptionSource(CultureInfo culture) => TFCommonResources.Get(nameof (ExceptionSource), culture);

    public static string ExceptionStackTrace(object arg0) => TFCommonResources.Format(nameof (ExceptionStackTrace), arg0);

    public static string ExceptionStackTrace(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (ExceptionStackTrace), culture, arg0);

    public static string FileInUse(object arg0) => TFCommonResources.Format(nameof (FileInUse), arg0);

    public static string FileInUse(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (FileInUse), culture, arg0);

    public static string FoundDirectoryExpectedFilePath(object arg0) => TFCommonResources.Format(nameof (FoundDirectoryExpectedFilePath), arg0);

    public static string FoundDirectoryExpectedFilePath(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (FoundDirectoryExpectedFilePath), culture, arg0);

    public static string HeadersAndContentsDontMatch(object arg0, object arg1) => TFCommonResources.Format(nameof (HeadersAndContentsDontMatch), arg0, arg1);

    public static string HeadersAndContentsDontMatch(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (HeadersAndContentsDontMatch), culture, arg0, arg1);

    public static string HttpStatusInfo(object arg0, object arg1) => TFCommonResources.Format(nameof (HttpStatusInfo), arg0, arg1);

    public static string HttpStatusInfo(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (HttpStatusInfo), culture, arg0, arg1);

    public static string IdentityNotFoundException(object arg0) => TFCommonResources.Format(nameof (IdentityNotFoundException), arg0);

    public static string IdentityNotFoundException(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (IdentityNotFoundException), culture, arg0);

    public static string InnerException() => TFCommonResources.Get(nameof (InnerException));

    public static string InnerException(CultureInfo culture) => TFCommonResources.Get(nameof (InnerException), culture);

    public static string InvalidComputerName(object arg0) => TFCommonResources.Format(nameof (InvalidComputerName), arg0);

    public static string InvalidComputerName(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidComputerName), culture, arg0);

    public static string InvalidComputerNameTooLong(object arg0, object arg1) => TFCommonResources.Format(nameof (InvalidComputerNameTooLong), arg0, arg1);

    public static string InvalidComputerNameTooLong(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidComputerNameTooLong), culture, arg0, arg1);

    public static string InvalidComputerNameInvalidCharacters(object arg0) => TFCommonResources.Format(nameof (InvalidComputerNameInvalidCharacters), arg0);

    public static string InvalidComputerNameInvalidCharacters(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidComputerNameInvalidCharacters), culture, arg0);

    public static string InvalidEnumerationValue(object arg0, object arg1) => TFCommonResources.Format(nameof (InvalidEnumerationValue), arg0, arg1);

    public static string InvalidEnumerationValue(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidEnumerationValue), culture, arg0, arg1);

    public static string InvalidPath(object arg0) => TFCommonResources.Format(nameof (InvalidPath), arg0);

    public static string InvalidPath(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidPath), culture, arg0);

    public static string InvalidPathDollarSign(object arg0) => TFCommonResources.Format(nameof (InvalidPathDollarSign), arg0);

    public static string InvalidPathDollarSign(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidPathDollarSign), culture, arg0);

    public static string InvalidPathTooLong(object arg0) => TFCommonResources.Format(nameof (InvalidPathTooLong), arg0);

    public static string InvalidPathTooLong(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidPathTooLong), culture, arg0);

    public static string InvalidServerPathTooLong(object arg0) => TFCommonResources.Format(nameof (InvalidServerPathTooLong), arg0);

    public static string InvalidServerPathTooLong(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidServerPathTooLong), culture, arg0);

    public static string InvalidPathTooLongVariable(object arg0, object arg1) => TFCommonResources.Format(nameof (InvalidPathTooLongVariable), arg0, arg1);

    public static string InvalidPathTooLongVariable(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidPathTooLongVariable), culture, arg0, arg1);

    public static string InvalidPathInvalidChar(object arg0, object arg1) => TFCommonResources.Format(nameof (InvalidPathInvalidChar), arg0, arg1);

    public static string InvalidPathInvalidChar(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidPathInvalidChar), culture, arg0, arg1);

    public static string InvalidPathInvalidCharacters(object arg0) => TFCommonResources.Format(nameof (InvalidPathInvalidCharacters), arg0);

    public static string InvalidPathInvalidCharacters(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidPathInvalidCharacters), culture, arg0);

    public static string InvalidPathInvalidCharactersAndWildcards(object arg0) => TFCommonResources.Format(nameof (InvalidPathInvalidCharactersAndWildcards), arg0);

    public static string InvalidPathInvalidCharactersAndWildcards(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidPathInvalidCharactersAndWildcards), culture, arg0);

    public static string InvalidPathTermination(object arg0) => TFCommonResources.Format(nameof (InvalidPathTermination), arg0);

    public static string InvalidPathTermination(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidPathTermination), culture, arg0);

    public static string InvalidServerResponse(object arg0) => TFCommonResources.Format(nameof (InvalidServerResponse), arg0);

    public static string InvalidServerResponse(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidServerResponse), culture, arg0);

    public static string InvalidTraceFile(object arg0, object arg1) => TFCommonResources.Format(nameof (InvalidTraceFile), arg0, arg1);

    public static string InvalidTraceFile(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidTraceFile), culture, arg0, arg1);

    public static string InvalidUserName(object arg0) => TFCommonResources.Format(nameof (InvalidUserName), arg0);

    public static string InvalidUserName(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidUserName), culture, arg0);

    public static string ListenerInitializeError(object arg0) => TFCommonResources.Format(nameof (ListenerInitializeError), arg0);

    public static string ListenerInitializeError(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (ListenerInitializeError), culture, arg0);

    public static string LogExceptionHeader(object arg0, object arg1) => TFCommonResources.Format(nameof (LogExceptionHeader), arg0, arg1);

    public static string LogExceptionHeader(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (LogExceptionHeader), culture, arg0, arg1);

    public static string LogExceptionDataDictionary(object arg0) => TFCommonResources.Format(nameof (LogExceptionDataDictionary), arg0);

    public static string LogExceptionDataDictionary(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (LogExceptionDataDictionary), culture, arg0);

    public static string Method(object arg0) => TFCommonResources.Format(nameof (Method), arg0);

    public static string Method(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (Method), culture, arg0);

    public static string AColonB(object arg0, object arg1) => TFCommonResources.Format(nameof (AColonB), arg0, arg1);

    public static string AColonB(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (AColonB), culture, arg0, arg1);

    public static string PathIsNotADirectory(object arg0) => TFCommonResources.Format(nameof (PathIsNotADirectory), arg0);

    public static string PathIsNotADirectory(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (PathIsNotADirectory), culture, arg0);

    public static string StackFrameHeader(object arg0, object arg1) => TFCommonResources.Format(nameof (StackFrameHeader), arg0, arg1);

    public static string StackFrameHeader(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (StackFrameHeader), culture, arg0, arg1);

    public static string StackFrameLineFormat(object arg0, object arg1) => TFCommonResources.Format(nameof (StackFrameLineFormat), arg0, arg1);

    public static string StackFrameLineFormat(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (StackFrameLineFormat), culture, arg0, arg1);

    public static string SqlExceptionError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6,
      object arg7,
      object arg8)
    {
      return TFCommonResources.Format(nameof (SqlExceptionError), arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
    }

    public static string SqlExceptionError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6,
      object arg7,
      object arg8,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (SqlExceptionError), culture, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
    }

    public static string SqlExceptionReport(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6)
    {
      return TFCommonResources.Format(nameof (SqlExceptionReport), arg0, arg1, arg2, arg3, arg4, arg5, arg6);
    }

    public static string SqlExceptionReport(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      object arg6,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (SqlExceptionReport), culture, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
    }

    public static string SoapExceptionReport(object arg0) => TFCommonResources.Format(nameof (SoapExceptionReport), arg0);

    public static string SoapExceptionReport(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (SoapExceptionReport), culture, arg0);

    public static string WebExceptionReport(object arg0, object arg1, object arg2) => TFCommonResources.Format(nameof (WebExceptionReport), arg0, arg1, arg2);

    public static string WebExceptionReport(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (WebExceptionReport), culture, arg0, arg1, arg2);
    }

    public static string WebExceptionReport_ResponseDisposed(object arg0) => TFCommonResources.Format(nameof (WebExceptionReport_ResponseDisposed), arg0);

    public static string WebExceptionReport_ResponseDisposed(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (WebExceptionReport_ResponseDisposed), culture, arg0);

    public static string StringLengthExceedsLimit() => TFCommonResources.Get(nameof (StringLengthExceedsLimit));

    public static string StringLengthExceedsLimit(CultureInfo culture) => TFCommonResources.Get(nameof (StringLengthExceedsLimit), culture);

    public static string StringPatternDidNotMatch(object arg0) => TFCommonResources.Format(nameof (StringPatternDidNotMatch), arg0);

    public static string StringPatternDidNotMatch(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (StringPatternDidNotMatch), culture, arg0);

    public static string TraceSettingArgumentException(object arg0) => TFCommonResources.Format(nameof (TraceSettingArgumentException), arg0);

    public static string TraceSettingArgumentException(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (TraceSettingArgumentException), culture, arg0);

    public static string TraceStartMessage(object arg0, object arg1) => TFCommonResources.Format(nameof (TraceStartMessage), arg0, arg1);

    public static string TraceStartMessage(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (TraceStartMessage), culture, arg0, arg1);

    public static string TraceStopMessage(object arg0, object arg1) => TFCommonResources.Format(nameof (TraceStopMessage), arg0, arg1);

    public static string TraceStopMessage(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (TraceStopMessage), culture, arg0, arg1);

    public static string TraceToggleException() => TFCommonResources.Get(nameof (TraceToggleException));

    public static string TraceToggleException(CultureInfo culture) => TFCommonResources.Get(nameof (TraceToggleException), culture);

    public static string Unauthorized(object arg0) => TFCommonResources.Format(nameof (Unauthorized), arg0);

    public static string Unauthorized(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (Unauthorized), culture, arg0);

    public static string UnableToContactService() => TFCommonResources.Get(nameof (UnableToContactService));

    public static string UnableToContactService(CultureInfo culture) => TFCommonResources.Get(nameof (UnableToContactService), culture);

    public static string UnableToRunApp(object arg0) => TFCommonResources.Format(nameof (UnableToRunApp), arg0);

    public static string UnableToRunApp(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (UnableToRunApp), culture, arg0);

    public static string UnableToRetrieveRegistrationInfo(object arg0) => TFCommonResources.Format(nameof (UnableToRetrieveRegistrationInfo), arg0);

    public static string UnableToRetrieveRegistrationInfo(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (UnableToRetrieveRegistrationInfo), culture, arg0);

    public static string UnauthorizedUnknownServer() => TFCommonResources.Get(nameof (UnauthorizedUnknownServer));

    public static string UnauthorizedUnknownServer(CultureInfo culture) => TFCommonResources.Get(nameof (UnauthorizedUnknownServer), culture);

    public static string UserNameAndPasswordRequiredForLoginTypeOAuth() => TFCommonResources.Get(nameof (UserNameAndPasswordRequiredForLoginTypeOAuth));

    public static string UserNameAndPasswordRequiredForLoginTypeOAuth(CultureInfo culture) => TFCommonResources.Get(nameof (UserNameAndPasswordRequiredForLoginTypeOAuth), culture);

    public static string UserNameAndPasswordRequiredForLoginTypeServiceIdentity() => TFCommonResources.Get(nameof (UserNameAndPasswordRequiredForLoginTypeServiceIdentity));

    public static string UserNameAndPasswordRequiredForLoginTypeServiceIdentity(CultureInfo culture) => TFCommonResources.Get(nameof (UserNameAndPasswordRequiredForLoginTypeServiceIdentity), culture);

    public static string WatsonMainPleaRegular() => TFCommonResources.Get(nameof (WatsonMainPleaRegular));

    public static string WatsonMainPleaRegular(CultureInfo culture) => TFCommonResources.Get(nameof (WatsonMainPleaRegular), culture);

    public static string WatsonMainIntroBold() => TFCommonResources.Get(nameof (WatsonMainIntroBold));

    public static string WatsonMainIntroBold(CultureInfo culture) => TFCommonResources.Get(nameof (WatsonMainIntroBold), culture);

    public static string WatsonMainIntroRegular() => TFCommonResources.Get(nameof (WatsonMainIntroRegular));

    public static string WatsonMainIntroRegular(CultureInfo culture) => TFCommonResources.Get(nameof (WatsonMainIntroRegular), culture);

    public static string WatsonMainPleaBold() => TFCommonResources.Get(nameof (WatsonMainPleaBold));

    public static string WatsonMainPleaBold(CultureInfo culture) => TFCommonResources.Get(nameof (WatsonMainPleaBold), culture);

    public static string WatsonEventDescription(object arg0) => TFCommonResources.Format(nameof (WatsonEventDescription), arg0);

    public static string WatsonEventDescription(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (WatsonEventDescription), culture, arg0);

    public static string WatsonReportBeingPrepared() => TFCommonResources.Get(nameof (WatsonReportBeingPrepared));

    public static string WatsonReportBeingPrepared(CultureInfo culture) => TFCommonResources.Get(nameof (WatsonReportBeingPrepared), culture);

    public static string WatsonReportException() => TFCommonResources.Get(nameof (WatsonReportException));

    public static string WatsonReportException(CultureInfo culture) => TFCommonResources.Get(nameof (WatsonReportException), culture);

    public static string WatsonReportInformation(object arg0) => TFCommonResources.Format(nameof (WatsonReportInformation), arg0);

    public static string WatsonReportInformation(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (WatsonReportInformation), culture, arg0);

    public static string WatsonReportNotFiled(object arg0) => TFCommonResources.Format(nameof (WatsonReportNotFiled), arg0);

    public static string WatsonReportNotFiled(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (WatsonReportNotFiled), culture, arg0);

    public static string WatsonReportReady() => TFCommonResources.Get(nameof (WatsonReportReady));

    public static string WatsonReportReady(CultureInfo culture) => TFCommonResources.Get(nameof (WatsonReportReady), culture);

    public static string WildcardsNotAllowed() => TFCommonResources.Get(nameof (WildcardsNotAllowed));

    public static string WildcardsNotAllowed(CultureInfo culture) => TFCommonResources.Get(nameof (WildcardsNotAllowed), culture);

    public static string VersionCheckFailed(object arg0) => TFCommonResources.Format(nameof (VersionCheckFailed), arg0);

    public static string VersionCheckFailed(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (VersionCheckFailed), culture, arg0);

    public static string RegistrationDataUnavailable(object arg0) => TFCommonResources.Format(nameof (RegistrationDataUnavailable), arg0);

    public static string RegistrationDataUnavailable(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (RegistrationDataUnavailable), culture, arg0);

    public static string VersionCheckFailedRange(object arg0, object arg1) => TFCommonResources.Format(nameof (VersionCheckFailedRange), arg0, arg1);

    public static string VersionCheckFailedRange(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (VersionCheckFailedRange), culture, arg0, arg1);

    public static string UnsupportedLinkFilter(object arg0) => TFCommonResources.Format(nameof (UnsupportedLinkFilter), arg0);

    public static string UnsupportedLinkFilter(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (UnsupportedLinkFilter), culture, arg0);

    public static string ServicesUnavailable(object arg0, object arg1) => TFCommonResources.Format(nameof (ServicesUnavailable), arg0, arg1);

    public static string ServicesUnavailable(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (ServicesUnavailable), culture, arg0, arg1);

    public static string ServicesUnavailableNoServer(object arg0) => TFCommonResources.Format(nameof (ServicesUnavailableNoServer), arg0);

    public static string ServicesUnavailableNoServer(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (ServicesUnavailableNoServer), culture, arg0);

    public static string DatabaseMissingErrorMessages(object arg0) => TFCommonResources.Format(nameof (DatabaseMissingErrorMessages), arg0);

    public static string DatabaseMissingErrorMessages(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (DatabaseMissingErrorMessages), culture, arg0);

    public static string AccessCheckExceptionTokenFormat(object arg0, object arg1, object arg2) => TFCommonResources.Format(nameof (AccessCheckExceptionTokenFormat), arg0, arg1, arg2);

    public static string AccessCheckExceptionTokenFormat(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (AccessCheckExceptionTokenFormat), culture, arg0, arg1, arg2);
    }

    public static string InvalidOperationOnNonHierarchicalNamespace() => TFCommonResources.Get(nameof (InvalidOperationOnNonHierarchicalNamespace));

    public static string InvalidOperationOnNonHierarchicalNamespace(CultureInfo culture) => TFCommonResources.Get(nameof (InvalidOperationOnNonHierarchicalNamespace), culture);

    public static string InvalidSecurityNamespaceGuid(object arg0) => TFCommonResources.Format(nameof (InvalidSecurityNamespaceGuid), arg0);

    public static string InvalidSecurityNamespaceGuid(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidSecurityNamespaceGuid), culture, arg0);

    public static string ReadOnlySecurityCollectionExceptionMessage() => TFCommonResources.Get(nameof (ReadOnlySecurityCollectionExceptionMessage));

    public static string ReadOnlySecurityCollectionExceptionMessage(CultureInfo culture) => TFCommonResources.Get(nameof (ReadOnlySecurityCollectionExceptionMessage), culture);

    public static string InvalidTokenSplitType() => TFCommonResources.Get(nameof (InvalidTokenSplitType));

    public static string InvalidTokenSplitType(CultureInfo culture) => TFCommonResources.Get(nameof (InvalidTokenSplitType), culture);

    public static string SecurityIdentityNotFoundMessage(object arg0) => TFCommonResources.Format(nameof (SecurityIdentityNotFoundMessage), arg0);

    public static string SecurityIdentityNotFoundMessage(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (SecurityIdentityNotFoundMessage), culture, arg0);

    public static string ServiceForFilterNotFoundWithIdentifier(object arg0, object arg1) => TFCommonResources.Format(nameof (ServiceForFilterNotFoundWithIdentifier), arg0, arg1);

    public static string ServiceForFilterNotFoundWithIdentifier(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (ServiceForFilterNotFoundWithIdentifier), culture, arg0, arg1);
    }

    public static string OperationNotSupportedMessage(object arg0, object arg1) => TFCommonResources.Format(nameof (OperationNotSupportedMessage), arg0, arg1);

    public static string OperationNotSupportedMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (OperationNotSupportedMessage), culture, arg0, arg1);
    }

    public static string ErrorReadingFile(object arg0, object arg1) => TFCommonResources.Format(nameof (ErrorReadingFile), arg0, arg1);

    public static string ErrorReadingFile(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (ErrorReadingFile), culture, arg0, arg1);

    public static string DuplicateLocationMappingMessage() => TFCommonResources.Get(nameof (DuplicateLocationMappingMessage));

    public static string DuplicateLocationMappingMessage(CultureInfo culture) => TFCommonResources.Get(nameof (DuplicateLocationMappingMessage), culture);

    public static string InvalidServiceTypeOnRegister(object arg0) => TFCommonResources.Format(nameof (InvalidServiceTypeOnRegister), arg0);

    public static string InvalidServiceTypeOnRegister(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidServiceTypeOnRegister), culture, arg0);

    public static string ServiceForFilterNotFound(object arg0) => TFCommonResources.Format(nameof (ServiceForFilterNotFound), arg0);

    public static string ServiceForFilterNotFound(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (ServiceForFilterNotFound), culture, arg0);

    public static string InvalidServiceDefinitionMissingMapping(object arg0, object arg1) => TFCommonResources.Format(nameof (InvalidServiceDefinitionMissingMapping), arg0, arg1);

    public static string InvalidServiceDefinitionMissingMapping(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (InvalidServiceDefinitionMissingMapping), culture, arg0, arg1);
    }

    public static string InvalidFullyQualifiedServiceDefinition(object arg0) => TFCommonResources.Format(nameof (InvalidFullyQualifiedServiceDefinition), arg0);

    public static string InvalidFullyQualifiedServiceDefinition(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidFullyQualifiedServiceDefinition), culture, arg0);

    public static string InvalidRelativeServiceDefinition(object arg0) => TFCommonResources.Format(nameof (InvalidRelativeServiceDefinition), arg0);

    public static string InvalidRelativeServiceDefinition(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidRelativeServiceDefinition), culture, arg0);

    public static string InvalidIdentifierOnUpdate() => TFCommonResources.Get(nameof (InvalidIdentifierOnUpdate));

    public static string InvalidIdentifierOnUpdate(CultureInfo culture) => TFCommonResources.Get(nameof (InvalidIdentifierOnUpdate), culture);

    public static string InvalidLocationServiceUrlRelativePath(object arg0, object arg1) => TFCommonResources.Format(nameof (InvalidLocationServiceUrlRelativePath), arg0, arg1);

    public static string InvalidLocationServiceUrlRelativePath(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (InvalidLocationServiceUrlRelativePath), culture, arg0, arg1);
    }

    public static string InvalidSecurityNamespaceDescriptionMessage() => TFCommonResources.Get(nameof (InvalidSecurityNamespaceDescriptionMessage));

    public static string InvalidSecurityNamespaceDescriptionMessage(CultureInfo culture) => TFCommonResources.Get(nameof (InvalidSecurityNamespaceDescriptionMessage), culture);

    public static string AccessMappingAlreadyRegistered(object arg0) => TFCommonResources.Format(nameof (AccessMappingAlreadyRegistered), arg0);

    public static string AccessMappingAlreadyRegistered(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (AccessMappingAlreadyRegistered), culture, arg0);

    public static string XmlNodeMissing(object arg0, object arg1) => TFCommonResources.Format(nameof (XmlNodeMissing), arg0, arg1);

    public static string XmlNodeMissing(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (XmlNodeMissing), culture, arg0, arg1);

    public static string XmlNodeEmpty(object arg0, object arg1) => TFCommonResources.Format(nameof (XmlNodeEmpty), arg0, arg1);

    public static string XmlNodeEmpty(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (XmlNodeEmpty), culture, arg0, arg1);

    public static string AccessMappingNotRegistered(object arg0) => TFCommonResources.Format(nameof (AccessMappingNotRegistered), arg0);

    public static string AccessMappingNotRegistered(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (AccessMappingNotRegistered), culture, arg0);

    public static string ServiceDefinitionWithNoLocations(object arg0) => TFCommonResources.Format(nameof (ServiceDefinitionWithNoLocations), arg0);

    public static string ServiceDefinitionWithNoLocations(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (ServiceDefinitionWithNoLocations), culture, arg0);

    public static string FullyQualifiedLocationParameter() => TFCommonResources.Get(nameof (FullyQualifiedLocationParameter));

    public static string FullyQualifiedLocationParameter(CultureInfo culture) => TFCommonResources.Get(nameof (FullyQualifiedLocationParameter), culture);

    public static string RelativeLocationMappingErrorMessage() => TFCommonResources.Get(nameof (RelativeLocationMappingErrorMessage));

    public static string RelativeLocationMappingErrorMessage(CultureInfo culture) => TFCommonResources.Get(nameof (RelativeLocationMappingErrorMessage), culture);

    public static string XmlAttributeEmpty(object arg0, object arg1) => TFCommonResources.Format(nameof (XmlAttributeEmpty), arg0, arg1);

    public static string XmlAttributeEmpty(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (XmlAttributeEmpty), culture, arg0, arg1);

    public static string XmlAttributeNull(object arg0, object arg1) => TFCommonResources.Format(nameof (XmlAttributeNull), arg0, arg1);

    public static string XmlAttributeNull(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (XmlAttributeNull), culture, arg0, arg1);

    public static string InvalidServiceDefinitionToolId() => TFCommonResources.Get(nameof (InvalidServiceDefinitionToolId));

    public static string InvalidServiceDefinitionToolId(CultureInfo culture) => TFCommonResources.Get(nameof (InvalidServiceDefinitionToolId), culture);

    public static string ConfigFileException() => TFCommonResources.Get(nameof (ConfigFileException));

    public static string ConfigFileException(CultureInfo culture) => TFCommonResources.Get(nameof (ConfigFileException), culture);

    public static string DuplicateRegistrationEntry(object arg0) => TFCommonResources.Format(nameof (DuplicateRegistrationEntry), arg0);

    public static string DuplicateRegistrationEntry(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (DuplicateRegistrationEntry), culture, arg0);

    public static string RegistrationEntryTypeMissing() => TFCommonResources.Get(nameof (RegistrationEntryTypeMissing));

    public static string RegistrationEntryTypeMissing(CultureInfo culture) => TFCommonResources.Get(nameof (RegistrationEntryTypeMissing), culture);

    public static string ServiceInterfacesDepricated() => TFCommonResources.Get(nameof (ServiceInterfacesDepricated));

    public static string ServiceInterfacesDepricated(CultureInfo culture) => TFCommonResources.Get(nameof (ServiceInterfacesDepricated), culture);

    public static string XmlRootNodeMissing(object arg0) => TFCommonResources.Format(nameof (XmlRootNodeMissing), arg0);

    public static string XmlRootNodeMissing(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (XmlRootNodeMissing), culture, arg0);

    public static string CorruptRegistrationData(object arg0, object arg1) => TFCommonResources.Format(nameof (CorruptRegistrationData), arg0, arg1);

    public static string CorruptRegistrationData(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (CorruptRegistrationData), culture, arg0, arg1);

    public static string UnableToRetrieveLocationInfo(object arg0) => TFCommonResources.Format(nameof (UnableToRetrieveLocationInfo), arg0);

    public static string UnableToRetrieveLocationInfo(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (UnableToRetrieveLocationInfo), culture, arg0);

    public static string ServiceDefinitionDoesNotExist(object arg0, object arg1) => TFCommonResources.Format(nameof (ServiceDefinitionDoesNotExist), arg0, arg1);

    public static string ServiceDefinitionDoesNotExist(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (ServiceDefinitionDoesNotExist), culture, arg0, arg1);
    }

    public static string ServiceDefinitionWithoutIdentifierDoesNotExist(object arg0) => TFCommonResources.Format(nameof (ServiceDefinitionWithoutIdentifierDoesNotExist), arg0);

    public static string ServiceDefinitionWithoutIdentifierDoesNotExist(
      object arg0,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (ServiceDefinitionWithoutIdentifierDoesNotExist), culture, arg0);
    }

    public static string InvalidFindServiceByTypeAndToolId(object arg0, object arg1, object arg2) => TFCommonResources.Format(nameof (InvalidFindServiceByTypeAndToolId), arg0, arg1, arg2);

    public static string InvalidFindServiceByTypeAndToolId(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (InvalidFindServiceByTypeAndToolId), culture, arg0, arg1, arg2);
    }

    public static string InvalidAccessMappingLocationServiceUrl() => TFCommonResources.Get(nameof (InvalidAccessMappingLocationServiceUrl));

    public static string InvalidAccessMappingLocationServiceUrl(CultureInfo culture) => TFCommonResources.Get(nameof (InvalidAccessMappingLocationServiceUrl), culture);

    public static string SingletonServiceDefinitionViolation(object arg0) => TFCommonResources.Format(nameof (SingletonServiceDefinitionViolation), arg0);

    public static string SingletonServiceDefinitionViolation(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (SingletonServiceDefinitionViolation), culture, arg0);

    public static string LocationMappingDoesNotExist(object arg0, object arg1, object arg2) => TFCommonResources.Format(nameof (LocationMappingDoesNotExist), arg0, arg1, arg2);

    public static string LocationMappingDoesNotExist(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (LocationMappingDoesNotExist), culture, arg0, arg1, arg2);
    }

    public static string BAD_ACCOUNT_NAME(object arg0) => TFCommonResources.Format(nameof (BAD_ACCOUNT_NAME), arg0);

    public static string BAD_ACCOUNT_NAME(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (BAD_ACCOUNT_NAME), culture, arg0);

    public static string BAD_DISPLAY_NAME(object arg0) => TFCommonResources.Format(nameof (BAD_DISPLAY_NAME), arg0);

    public static string BAD_DISPLAY_NAME(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (BAD_DISPLAY_NAME), culture, arg0);

    public static string BAD_GROUP_NAME(object arg0) => TFCommonResources.Format(nameof (BAD_GROUP_NAME), arg0);

    public static string BAD_GROUP_NAME(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (BAD_GROUP_NAME), culture, arg0);

    public static string BAD_GROUP_DESCRIPTION(object arg0) => TFCommonResources.Format(nameof (BAD_GROUP_DESCRIPTION), arg0);

    public static string BAD_GROUP_DESCRIPTION(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (BAD_GROUP_DESCRIPTION), culture, arg0);

    public static string BAD_GROUP_DESCRIPTION_TOO_LONG(object arg0) => TFCommonResources.Format(nameof (BAD_GROUP_DESCRIPTION_TOO_LONG), arg0);

    public static string BAD_GROUP_DESCRIPTION_TOO_LONG(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (BAD_GROUP_DESCRIPTION_TOO_LONG), culture, arg0);

    public static string BAD_SID(object arg0, object arg1) => TFCommonResources.Format(nameof (BAD_SID), arg0, arg1);

    public static string BAD_SID(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (BAD_SID), culture, arg0, arg1);

    public static string CannotCreateOrphanedResource() => TFCommonResources.Get(nameof (CannotCreateOrphanedResource));

    public static string CannotCreateOrphanedResource(CultureInfo culture) => TFCommonResources.Get(nameof (CannotCreateOrphanedResource), culture);

    public static string CatalogNodeDoesNotExist() => TFCommonResources.Get(nameof (CatalogNodeDoesNotExist));

    public static string CatalogNodeDoesNotExist(CultureInfo culture) => TFCommonResources.Get(nameof (CatalogNodeDoesNotExist), culture);

    public static string CatalogResourceDoesNotExist(object arg0) => TFCommonResources.Format(nameof (CatalogResourceDoesNotExist), arg0);

    public static string CatalogResourceDoesNotExist(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (CatalogResourceDoesNotExist), culture, arg0);

    public static string CatalogResourceTypeDoesNotExist(object arg0) => TFCommonResources.Format(nameof (CatalogResourceTypeDoesNotExist), arg0);

    public static string CatalogResourceTypeDoesNotExist(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (CatalogResourceTypeDoesNotExist), culture, arg0);

    public static string InvalidUnicodeCharacter(object arg0) => TFCommonResources.Format(nameof (InvalidUnicodeCharacter), arg0);

    public static string InvalidUnicodeCharacter(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidUnicodeCharacter), culture, arg0);

    public static string PropertyArgumentExceededMaximumSizeAllowed(object arg0, object arg1) => TFCommonResources.Format(nameof (PropertyArgumentExceededMaximumSizeAllowed), arg0, arg1);

    public static string PropertyArgumentExceededMaximumSizeAllowed(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (PropertyArgumentExceededMaximumSizeAllowed), culture, arg0, arg1);
    }

    public static string InvalidPropertyValueSize(object arg0, object arg1, object arg2) => TFCommonResources.Format(nameof (InvalidPropertyValueSize), arg0, arg1, arg2);

    public static string InvalidPropertyValueSize(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (InvalidPropertyValueSize), culture, arg0, arg1, arg2);
    }

    public static string PropertyInvalidVersionSpecification(object arg0) => TFCommonResources.Format(nameof (PropertyInvalidVersionSpecification), arg0);

    public static string PropertyInvalidVersionSpecification(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (PropertyInvalidVersionSpecification), culture, arg0);

    public static string UnsupportedPropertyType(object arg0) => TFCommonResources.Format(nameof (UnsupportedPropertyType), arg0);

    public static string UnsupportedPropertyType(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (UnsupportedPropertyType), culture, arg0);

    public static string MultipleExtensionNames() => TFCommonResources.Get(nameof (MultipleExtensionNames));

    public static string MultipleExtensionNames(CultureInfo culture) => TFCommonResources.Get(nameof (MultipleExtensionNames), culture);

    public static string ExtensionNameNullOrEmpty() => TFCommonResources.Get(nameof (ExtensionNameNullOrEmpty));

    public static string ExtensionNameNullOrEmpty(CultureInfo culture) => TFCommonResources.Get(nameof (ExtensionNameNullOrEmpty), culture);

    public static string DuplicateExtensionNameError(object arg0, object arg1, object arg2) => TFCommonResources.Format(nameof (DuplicateExtensionNameError), arg0, arg1, arg2);

    public static string DuplicateExtensionNameError(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (DuplicateExtensionNameError), culture, arg0, arg1, arg2);
    }

    public static string UnsupportedPropertyValue(object arg0, object arg1, object arg2) => TFCommonResources.Format(nameof (UnsupportedPropertyValue), arg0, arg1, arg2);

    public static string UnsupportedPropertyValue(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (UnsupportedPropertyValue), culture, arg0, arg1, arg2);
    }

    public static string SharePointServiceAccountsGroupDescription() => TFCommonResources.Get(nameof (SharePointServiceAccountsGroupDescription));

    public static string SharePointServiceAccountsGroupDescription(CultureInfo culture) => TFCommonResources.Get(nameof (SharePointServiceAccountsGroupDescription), culture);

    public static string FailedToMapPropertyKindToArtifactKind(object arg0) => TFCommonResources.Format(nameof (FailedToMapPropertyKindToArtifactKind), arg0);

    public static string FailedToMapPropertyKindToArtifactKind(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (FailedToMapPropertyKindToArtifactKind), culture, arg0);

    public static string SharePointServiceAccountsGroupName() => TFCommonResources.Get(nameof (SharePointServiceAccountsGroupName));

    public static string SharePointServiceAccountsGroupName(CultureInfo culture) => TFCommonResources.Get(nameof (SharePointServiceAccountsGroupName), culture);

    public static string IllegalIdentityException(object arg0) => TFCommonResources.Format(nameof (IllegalIdentityException), arg0);

    public static string IllegalIdentityException(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (IllegalIdentityException), culture, arg0);

    public static string Ellipsis(object arg0) => TFCommonResources.Format(nameof (Ellipsis), arg0);

    public static string Ellipsis(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (Ellipsis), culture, arg0);

    public static string SparseTreeTokenAlreadyExists() => TFCommonResources.Get(nameof (SparseTreeTokenAlreadyExists));

    public static string SparseTreeTokenAlreadyExists(CultureInfo culture) => TFCommonResources.Get(nameof (SparseTreeTokenAlreadyExists), culture);

    public static string SparseTreeNoEmptyStringToken() => TFCommonResources.Get(nameof (SparseTreeNoEmptyStringToken));

    public static string SparseTreeNoEmptyStringToken(CultureInfo culture) => TFCommonResources.Get(nameof (SparseTreeNoEmptyStringToken), culture);

    public static string InvalidCatalogNodePathAllowWildcard(object arg0) => TFCommonResources.Format(nameof (InvalidCatalogNodePathAllowWildcard), arg0);

    public static string InvalidCatalogNodePathAllowWildcard(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidCatalogNodePathAllowWildcard), culture, arg0);

    public static string InvalidCatalogNodePathNoWildcard(object arg0) => TFCommonResources.Format(nameof (InvalidCatalogNodePathNoWildcard), arg0);

    public static string InvalidCatalogNodePathNoWildcard(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidCatalogNodePathNoWildcard), culture, arg0);

    public static string ServicingStepNotExecuted(object arg0, object arg1, object arg2) => TFCommonResources.Format(nameof (ServicingStepNotExecuted), arg0, arg1, arg2);

    public static string ServicingStepNotExecuted(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (ServicingStepNotExecuted), culture, arg0, arg1, arg2);
    }

    public static string ExecutingServicingStep(object arg0, object arg1, object arg2) => TFCommonResources.Format(nameof (ExecutingServicingStep), arg0, arg1, arg2);

    public static string ExecutingServicingStep(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (ExecutingServicingStep), culture, arg0, arg1, arg2);
    }

    public static string ServicingStepFailed(object arg0, object arg1, object arg2) => TFCommonResources.Format(nameof (ServicingStepFailed), arg0, arg1, arg2);

    public static string ServicingStepFailed(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (ServicingStepFailed), culture, arg0, arg1, arg2);
    }

    public static string ServicingStepPassed(object arg0, object arg1, object arg2) => TFCommonResources.Format(nameof (ServicingStepPassed), arg0, arg1, arg2);

    public static string ServicingStepPassed(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (ServicingStepPassed), culture, arg0, arg1, arg2);
    }

    public static string ServicingStepPassedWithWarnings(object arg0, object arg1, object arg2) => TFCommonResources.Format(nameof (ServicingStepPassedWithWarnings), arg0, arg1, arg2);

    public static string ServicingStepPassedWithWarnings(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (ServicingStepPassedWithWarnings), culture, arg0, arg1, arg2);
    }

    public static string ServicingStepPassedWithSkipChildren(object arg0, object arg1, object arg2) => TFCommonResources.Format(nameof (ServicingStepPassedWithSkipChildren), arg0, arg1, arg2);

    public static string ServicingStepPassedWithSkipChildren(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (ServicingStepPassedWithSkipChildren), culture, arg0, arg1, arg2);
    }

    public static string ServicingStepStateChange(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return TFCommonResources.Format(nameof (ServicingStepStateChange), arg0, arg1, arg2, arg3);
    }

    public static string ServicingStepStateChange(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (ServicingStepStateChange), culture, arg0, arg1, arg2, arg3);
    }

    public static string ServicingStepValidated(object arg0, object arg1, object arg2) => TFCommonResources.Format(nameof (ServicingStepValidated), arg0, arg1, arg2);

    public static string ServicingStepValidated(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (ServicingStepValidated), culture, arg0, arg1, arg2);
    }

    public static string ServicingStepValidatedWithWarnings(object arg0, object arg1, object arg2) => TFCommonResources.Format(nameof (ServicingStepValidatedWithWarnings), arg0, arg1, arg2);

    public static string ServicingStepValidatedWithWarnings(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (ServicingStepValidatedWithWarnings), culture, arg0, arg1, arg2);
    }

    public static string ValidatingServicingStep(object arg0, object arg1, object arg2) => TFCommonResources.Format(nameof (ValidatingServicingStep), arg0, arg1, arg2);

    public static string ValidatingServicingStep(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (ValidatingServicingStep), culture, arg0, arg1, arg2);
    }

    public static string ServicingStepSkipped(object arg0, object arg1, object arg2) => TFCommonResources.Format(nameof (ServicingStepSkipped), arg0, arg1, arg2);

    public static string ServicingStepSkipped(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (ServicingStepSkipped), culture, arg0, arg1, arg2);
    }

    public static string CollectionDoesNotExist(object arg0) => TFCommonResources.Format(nameof (CollectionDoesNotExist), arg0);

    public static string CollectionDoesNotExist(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (CollectionDoesNotExist), culture, arg0);

    public static string InvalidRegistryValue(object arg0, object arg1) => TFCommonResources.Format(nameof (InvalidRegistryValue), arg0, arg1);

    public static string InvalidRegistryValue(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidRegistryValue), culture, arg0, arg1);

    public static string GlobalSecurityAdministerConfigurationPermission() => TFCommonResources.Get(nameof (GlobalSecurityAdministerConfigurationPermission));

    public static string GlobalSecurityAdministerConfigurationPermission(CultureInfo culture) => TFCommonResources.Get(nameof (GlobalSecurityAdministerConfigurationPermission), culture);

    public static string AccessCheckExceptionPrivilegeFormat(object arg0, object arg1) => TFCommonResources.Format(nameof (AccessCheckExceptionPrivilegeFormat), arg0, arg1);

    public static string AccessCheckExceptionPrivilegeFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (AccessCheckExceptionPrivilegeFormat), culture, arg0, arg1);
    }

    public static string SharePointServiceAccountsGroupNotFound(object arg0) => TFCommonResources.Format(nameof (SharePointServiceAccountsGroupNotFound), arg0);

    public static string SharePointServiceAccountsGroupNotFound(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (SharePointServiceAccountsGroupNotFound), culture, arg0);

    public static string FailedToAddSharePointServiceAccount(object arg0, object arg1) => TFCommonResources.Format(nameof (FailedToAddSharePointServiceAccount), arg0, arg1);

    public static string FailedToAddSharePointServiceAccount(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (FailedToAddSharePointServiceAccount), culture, arg0, arg1);
    }

    public static string FailedToAddSharePointServiceAccountWithException(
      object arg0,
      object arg1,
      object arg2)
    {
      return TFCommonResources.Format(nameof (FailedToAddSharePointServiceAccountWithException), arg0, arg1, arg2);
    }

    public static string FailedToAddSharePointServiceAccountWithException(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (FailedToAddSharePointServiceAccountWithException), culture, arg0, arg1, arg2);
    }

    public static string FailedToMapWssBackToTfsWithException(object arg0, object arg1) => TFCommonResources.Format(nameof (FailedToMapWssBackToTfsWithException), arg0, arg1);

    public static string FailedToMapWssBackToTfsWithException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (FailedToMapWssBackToTfsWithException), culture, arg0, arg1);
    }

    public static string FailedToAutoConfigureSharePointWebApplication(object arg0, object arg1) => TFCommonResources.Format(nameof (FailedToAutoConfigureSharePointWebApplication), arg0, arg1);

    public static string FailedToAutoConfigureSharePointWebApplication(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (FailedToAutoConfigureSharePointWebApplication), culture, arg0, arg1);
    }

    public static string InvalidPathMissingRoot(object arg0) => TFCommonResources.Format(nameof (InvalidPathMissingRoot), arg0);

    public static string InvalidPathMissingRoot(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidPathMissingRoot), culture, arg0);

    public static string TeamSystemWebAccessUrlIsMissing(object arg0) => TFCommonResources.Format(nameof (TeamSystemWebAccessUrlIsMissing), arg0);

    public static string TeamSystemWebAccessUrlIsMissing(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (TeamSystemWebAccessUrlIsMissing), culture, arg0);

    public static string ApplicationUrlIsNotInitialized() => TFCommonResources.Get(nameof (ApplicationUrlIsNotInitialized));

    public static string ApplicationUrlIsNotInitialized(CultureInfo culture) => TFCommonResources.Get(nameof (ApplicationUrlIsNotInitialized), culture);

    public static string CollectionIdIsNotInitialized() => TFCommonResources.Get(nameof (CollectionIdIsNotInitialized));

    public static string CollectionIdIsNotInitialized(CultureInfo culture) => TFCommonResources.Get(nameof (CollectionIdIsNotInitialized), culture);

    public static string FrameworkEventingServiceDescription() => TFCommonResources.Get(nameof (FrameworkEventingServiceDescription));

    public static string FrameworkEventingServiceDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkEventingServiceDescription), culture);

    public static string FrameworkIdentityServiceDescription() => TFCommonResources.Get(nameof (FrameworkIdentityServiceDescription));

    public static string FrameworkIdentityServiceDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkIdentityServiceDescription), culture);

    public static string FrameworkIdentityService2Description() => TFCommonResources.Get(nameof (FrameworkIdentityService2Description));

    public static string FrameworkIdentityService2Description(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkIdentityService2Description), culture);

    public static string FrameworkJobServiceDescription() => TFCommonResources.Get(nameof (FrameworkJobServiceDescription));

    public static string FrameworkJobServiceDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkJobServiceDescription), culture);

    public static string FrameworkLocationServiceDescription() => TFCommonResources.Get(nameof (FrameworkLocationServiceDescription));

    public static string FrameworkLocationServiceDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkLocationServiceDescription), culture);

    public static string FrameworkMessageQueueServiceDescription() => TFCommonResources.Get(nameof (FrameworkMessageQueueServiceDescription));

    public static string FrameworkMessageQueueServiceDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkMessageQueueServiceDescription), culture);

    public static string FrameworkPropertyServiceDescription() => TFCommonResources.Get(nameof (FrameworkPropertyServiceDescription));

    public static string FrameworkPropertyServiceDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkPropertyServiceDescription), culture);

    public static string FrameworkRegistryServiceDescription() => TFCommonResources.Get(nameof (FrameworkRegistryServiceDescription));

    public static string FrameworkRegistryServiceDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkRegistryServiceDescription), culture);

    public static string FrameworkSecurityServiceDescription() => TFCommonResources.Get(nameof (FrameworkSecurityServiceDescription));

    public static string FrameworkSecurityServiceDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkSecurityServiceDescription), culture);

    public static string FrameworkMethodologyServiceDescription() => TFCommonResources.Get(nameof (FrameworkMethodologyServiceDescription));

    public static string FrameworkMethodologyServiceDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkMethodologyServiceDescription), culture);

    public static string FrameworkMethodologyUploadServiceDescription() => TFCommonResources.Get(nameof (FrameworkMethodologyUploadServiceDescription));

    public static string FrameworkMethodologyUploadServiceDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkMethodologyUploadServiceDescription), culture);

    public static string FrameworkProcessTemplateServiceDescription() => TFCommonResources.Get(nameof (FrameworkProcessTemplateServiceDescription));

    public static string FrameworkProcessTemplateServiceDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkProcessTemplateServiceDescription), culture);

    public static string FrameworkStrongBoxServiceDescription() => TFCommonResources.Get(nameof (FrameworkStrongBoxServiceDescription));

    public static string FrameworkStrongBoxServiceDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkStrongBoxServiceDescription), culture);

    public static string FrameworkStrongBoxFileUploadServiceDescription() => TFCommonResources.Get(nameof (FrameworkStrongBoxFileUploadServiceDescription));

    public static string FrameworkStrongBoxFileUploadServiceDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkStrongBoxFileUploadServiceDescription), culture);

    public static string FrameworkStrongBoxFileDownloadServiceDescription() => TFCommonResources.Get(nameof (FrameworkStrongBoxFileDownloadServiceDescription));

    public static string FrameworkStrongBoxFileDownloadServiceDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkStrongBoxFileDownloadServiceDescription), culture);

    public static string CatalogServiceDescriptrion() => TFCommonResources.Get(nameof (CatalogServiceDescriptrion));

    public static string CatalogServiceDescriptrion(CultureInfo culture) => TFCommonResources.Get(nameof (CatalogServiceDescriptrion), culture);

    public static string CollectionManagementServiceDescription() => TFCommonResources.Get(nameof (CollectionManagementServiceDescription));

    public static string CollectionManagementServiceDescription(CultureInfo culture) => TFCommonResources.Get(nameof (CollectionManagementServiceDescription), culture);

    public static string FrameworkAdministrationServiceDescription() => TFCommonResources.Get(nameof (FrameworkAdministrationServiceDescription));

    public static string FrameworkAdministrationServiceDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkAdministrationServiceDescription), culture);

    public static string ServicingResourceUploadServiceDescription() => TFCommonResources.Get(nameof (ServicingResourceUploadServiceDescription));

    public static string ServicingResourceUploadServiceDescription(CultureInfo culture) => TFCommonResources.Get(nameof (ServicingResourceUploadServiceDescription), culture);

    public static string FrameworkAccessControlServiceDescription() => TFCommonResources.Get(nameof (FrameworkAccessControlServiceDescription));

    public static string FrameworkAccessControlServiceDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkAccessControlServiceDescription), culture);

    public static string AccessMappingIsDefaultMessage(object arg0) => TFCommonResources.Format(nameof (AccessMappingIsDefaultMessage), arg0);

    public static string AccessMappingIsDefaultMessage(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (AccessMappingIsDefaultMessage), culture, arg0);

    public static string FailedToAutoActivateSharePointWebApplicationFeature(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return TFCommonResources.Format(nameof (FailedToAutoActivateSharePointWebApplicationFeature), arg0, arg1, arg2, arg3);
    }

    public static string FailedToAutoActivateSharePointWebApplicationFeature(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (FailedToAutoActivateSharePointWebApplicationFeature), culture, arg0, arg1, arg2, arg3);
    }

    public static string NullArtifactUrlInUrlList() => TFCommonResources.Get(nameof (NullArtifactUrlInUrlList));

    public static string NullArtifactUrlInUrlList(CultureInfo culture) => TFCommonResources.Get(nameof (NullArtifactUrlInUrlList), culture);

    public static string WorkItemOnlyViewUsersGroupName() => TFCommonResources.Get(nameof (WorkItemOnlyViewUsersGroupName));

    public static string WorkItemOnlyViewUsersGroupName(CultureInfo culture) => TFCommonResources.Get(nameof (WorkItemOnlyViewUsersGroupName), culture);

    public static string WorkItemOnlyViewUsersGroupDescription() => TFCommonResources.Get(nameof (WorkItemOnlyViewUsersGroupDescription));

    public static string WorkItemOnlyViewUsersGroupDescription(CultureInfo culture) => TFCommonResources.Get(nameof (WorkItemOnlyViewUsersGroupDescription), culture);

    public static string CannotDetachFailedInitializeCollection(object arg0) => TFCommonResources.Format(nameof (CannotDetachFailedInitializeCollection), arg0);

    public static string CannotDetachFailedInitializeCollection(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (CannotDetachFailedInitializeCollection), culture, arg0);

    public static string FailureGetArtifactWithTool(object arg0) => TFCommonResources.Format(nameof (FailureGetArtifactWithTool), arg0);

    public static string FailureGetArtifactWithTool(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (FailureGetArtifactWithTool), culture, arg0);

    public static string MemoryCacheListKeyAlreadyPresent() => TFCommonResources.Get(nameof (MemoryCacheListKeyAlreadyPresent));

    public static string MemoryCacheListKeyAlreadyPresent(CultureInfo culture) => TFCommonResources.Get(nameof (MemoryCacheListKeyAlreadyPresent), culture);

    public static string UnableToEstablishConnection() => TFCommonResources.Get(nameof (UnableToEstablishConnection));

    public static string UnableToEstablishConnection(CultureInfo culture) => TFCommonResources.Get(nameof (UnableToEstablishConnection), culture);

    public static string EntityModel_AccessZoneNotFound(object arg0) => TFCommonResources.Format(nameof (EntityModel_AccessZoneNotFound), arg0);

    public static string EntityModel_AccessZoneNotFound(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (EntityModel_AccessZoneNotFound), culture, arg0);

    public static string EntityModel_AdministratorsMessage(object arg0) => TFCommonResources.Format(nameof (EntityModel_AdministratorsMessage), arg0);

    public static string EntityModel_AdministratorsMessage(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (EntityModel_AdministratorsMessage), culture, arg0);

    public static string EntityModel_CollectionOrganizationalNodeNotFound() => TFCommonResources.Get(nameof (EntityModel_CollectionOrganizationalNodeNotFound));

    public static string EntityModel_CollectionOrganizationalNodeNotFound(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_CollectionOrganizationalNodeNotFound), culture);

    public static string EntityModel_DefaultSiteCreationLocationNotConfigured() => TFCommonResources.Get(nameof (EntityModel_DefaultSiteCreationLocationNotConfigured));

    public static string EntityModel_DefaultSiteCreationLocationNotConfigured(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_DefaultSiteCreationLocationNotConfigured), culture);

    public static string EntityModel_DefaultSiteCreationLocationNotConfiguredSubTitle() => TFCommonResources.Get(nameof (EntityModel_DefaultSiteCreationLocationNotConfiguredSubTitle));

    public static string EntityModel_DefaultSiteCreationLocationNotConfiguredSubTitle(
      CultureInfo culture)
    {
      return TFCommonResources.Get(nameof (EntityModel_DefaultSiteCreationLocationNotConfiguredSubTitle), culture);
    }

    public static string EntityModel_DefaultZoneRequiresAllPaths(object arg0, object arg1) => TFCommonResources.Format(nameof (EntityModel_DefaultZoneRequiresAllPaths), arg0, arg1);

    public static string EntityModel_DefaultZoneRequiresAllPaths(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (EntityModel_DefaultZoneRequiresAllPaths), culture, arg0, arg1);
    }

    public static string EntityModel_HelpLink() => TFCommonResources.Get(nameof (EntityModel_HelpLink));

    public static string EntityModel_HelpLink(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_HelpLink), culture);

    public static string EntityModel_NoUriForCurrentZone(object arg0) => TFCommonResources.Format(nameof (EntityModel_NoUriForCurrentZone), arg0);

    public static string EntityModel_NoUriForCurrentZone(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (EntityModel_NoUriForCurrentZone), culture, arg0);

    public static string EntityModel_PortalOwnerInfo(object arg0, object arg1) => TFCommonResources.Format(nameof (EntityModel_PortalOwnerInfo), arg0, arg1);

    public static string EntityModel_PortalOwnerInfo(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (EntityModel_PortalOwnerInfo), culture, arg0, arg1);

    public static string EntityModel_PortalSiteNotConfigured() => TFCommonResources.Get(nameof (EntityModel_PortalSiteNotConfigured));

    public static string EntityModel_PortalSiteNotConfigured(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_PortalSiteNotConfigured), culture);

    public static string EntityModel_PortalSiteNotConfiguredSubTitle() => TFCommonResources.Get(nameof (EntityModel_PortalSiteNotConfiguredSubTitle));

    public static string EntityModel_PortalSiteNotConfiguredSubTitle(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_PortalSiteNotConfiguredSubTitle), culture);

    public static string EntityModel_ProcessGuidanceNotConfigured() => TFCommonResources.Get(nameof (EntityModel_ProcessGuidanceNotConfigured));

    public static string EntityModel_ProcessGuidanceNotConfigured(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_ProcessGuidanceNotConfigured), culture);

    public static string EntityModel_ProcessGuidanceNotConfiguredSubTitle() => TFCommonResources.Get(nameof (EntityModel_ProcessGuidanceNotConfiguredSubTitle));

    public static string EntityModel_ProcessGuidanceNotConfiguredSubTitle(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_ProcessGuidanceNotConfiguredSubTitle), culture);

    public static string EntityModel_ProjectIdNotFound(object arg0) => TFCommonResources.Format(nameof (EntityModel_ProjectIdNotFound), arg0);

    public static string EntityModel_ProjectIdNotFound(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (EntityModel_ProjectIdNotFound), culture, arg0);

    public static string EntityModel_ProjectUriNotFound(object arg0) => TFCommonResources.Format(nameof (EntityModel_ProjectUriNotFound), arg0);

    public static string EntityModel_ProjectUriNotFound(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (EntityModel_ProjectUriNotFound), culture, arg0);

    public static string EntityModel_ReadOnly() => TFCommonResources.Get(nameof (EntityModel_ReadOnly));

    public static string EntityModel_ReadOnly(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_ReadOnly), culture);

    public static string EntityModel_ReportingCubeInvalid() => TFCommonResources.Get(nameof (EntityModel_ReportingCubeInvalid));

    public static string EntityModel_ReportingCubeInvalid(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_ReportingCubeInvalid), culture);

    public static string EntityModel_ReportingNotConfigured() => TFCommonResources.Get(nameof (EntityModel_ReportingNotConfigured));

    public static string EntityModel_ReportingNotConfigured(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_ReportingNotConfigured), culture);

    public static string EntityModel_ReportingNotConfiguredSubTitle() => TFCommonResources.Get(nameof (EntityModel_ReportingNotConfiguredSubTitle));

    public static string EntityModel_ReportingNotConfiguredSubTitle(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_ReportingNotConfiguredSubTitle), culture);

    public static string EntityModel_ServiceDefinitionNotFound(object arg0) => TFCommonResources.Format(nameof (EntityModel_ServiceDefinitionNotFound), arg0);

    public static string EntityModel_ServiceDefinitionNotFound(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (EntityModel_ServiceDefinitionNotFound), culture, arg0);

    public static string EntityModel_WebAccessNotConfigured() => TFCommonResources.Get(nameof (EntityModel_WebAccessNotConfigured));

    public static string EntityModel_WebAccessNotConfigured(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_WebAccessNotConfigured), culture);

    public static string EntityModel_WebAccessNotConfiguredSubTitle() => TFCommonResources.Get(nameof (EntityModel_WebAccessNotConfiguredSubTitle));

    public static string EntityModel_WebAccessNotConfiguredSubTitle(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_WebAccessNotConfiguredSubTitle), culture);

    public static string EntityModel_BadBooleanFormat() => TFCommonResources.Get(nameof (EntityModel_BadBooleanFormat));

    public static string EntityModel_BadBooleanFormat(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_BadBooleanFormat), culture);

    public static string EntityModel_BadEnumFormat() => TFCommonResources.Get(nameof (EntityModel_BadEnumFormat));

    public static string EntityModel_BadEnumFormat(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_BadEnumFormat), culture);

    public static string EntityModel_BadGuidFormat() => TFCommonResources.Get(nameof (EntityModel_BadGuidFormat));

    public static string EntityModel_BadGuidFormat(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_BadGuidFormat), culture);

    public static string EntityModel_BadInt32Format() => TFCommonResources.Get(nameof (EntityModel_BadInt32Format));

    public static string EntityModel_BadInt32Format(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_BadInt32Format), culture);

    public static string EntityModel_DataDefinesUnsupportedSchema() => TFCommonResources.Get(nameof (EntityModel_DataDefinesUnsupportedSchema));

    public static string EntityModel_DataDefinesUnsupportedSchema(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_DataDefinesUnsupportedSchema), culture);

    public static string EntityModel_DataDoesNotDefineSchema() => TFCommonResources.Get(nameof (EntityModel_DataDoesNotDefineSchema));

    public static string EntityModel_DataDoesNotDefineSchema(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_DataDoesNotDefineSchema), culture);

    public static string EntityModel_IncompatibleType() => TFCommonResources.Get(nameof (EntityModel_IncompatibleType));

    public static string EntityModel_IncompatibleType(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_IncompatibleType), culture);

    public static string EntityModel_InvalidInstanceId() => TFCommonResources.Get(nameof (EntityModel_InvalidInstanceId));

    public static string EntityModel_InvalidInstanceId(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_InvalidInstanceId), culture);

    public static string EntityModel_InvalidObjectRefValue_BadFormat() => TFCommonResources.Get(nameof (EntityModel_InvalidObjectRefValue_BadFormat));

    public static string EntityModel_InvalidObjectRefValue_BadFormat(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_InvalidObjectRefValue_BadFormat), culture);

    public static string EntityModel_InvalidObjectRefValue_BadIdentifier() => TFCommonResources.Get(nameof (EntityModel_InvalidObjectRefValue_BadIdentifier));

    public static string EntityModel_InvalidObjectRefValue_BadIdentifier(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_InvalidObjectRefValue_BadIdentifier), culture);

    public static string EntityModel_InvalidObjectRefValue_UnknownIdentifier() => TFCommonResources.Get(nameof (EntityModel_InvalidObjectRefValue_UnknownIdentifier));

    public static string EntityModel_InvalidObjectRefValue_UnknownIdentifier(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_InvalidObjectRefValue_UnknownIdentifier), culture);

    public static string EntityModel_MustDeriveFromTfsObject() => TFCommonResources.Get(nameof (EntityModel_MustDeriveFromTfsObject));

    public static string EntityModel_MustDeriveFromTfsObject(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_MustDeriveFromTfsObject), culture);

    public static string EntityModel_SessionMissing() => TFCommonResources.Get(nameof (EntityModel_SessionMissing));

    public static string EntityModel_SessionMissing(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_SessionMissing), culture);

    public static string EntityModel_TooManyFieldValues() => TFCommonResources.Get(nameof (EntityModel_TooManyFieldValues));

    public static string EntityModel_TooManyFieldValues(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_TooManyFieldValues), culture);

    public static string EntityModel_UnknownType(object arg0) => TFCommonResources.Format(nameof (EntityModel_UnknownType), arg0);

    public static string EntityModel_UnknownType(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (EntityModel_UnknownType), culture, arg0);

    public static string EntityModel_AlreadyBoundToAnotherSession() => TFCommonResources.Get(nameof (EntityModel_AlreadyBoundToAnotherSession));

    public static string EntityModel_AlreadyBoundToAnotherSession(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_AlreadyBoundToAnotherSession), culture);

    public static string EntityModel_AlreadyHasParent() => TFCommonResources.Get(nameof (EntityModel_AlreadyHasParent));

    public static string EntityModel_AlreadyHasParent(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_AlreadyHasParent), culture);

    public static string EntityModel_CannotResolveUriWithoutSession() => TFCommonResources.Get(nameof (EntityModel_CannotResolveUriWithoutSession));

    public static string EntityModel_CannotResolveUriWithoutSession(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_CannotResolveUriWithoutSession), culture);

    public static string EntityModel_FieldFiltersNotSupported() => TFCommonResources.Get(nameof (EntityModel_FieldFiltersNotSupported));

    public static string EntityModel_FieldFiltersNotSupported(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_FieldFiltersNotSupported), culture);

    public static string EntityModel_ParentCannotChange() => TFCommonResources.Get(nameof (EntityModel_ParentCannotChange));

    public static string EntityModel_ParentCannotChange(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_ParentCannotChange), culture);

    public static string EntityModel_RefreshNotAvailable() => TFCommonResources.Get(nameof (EntityModel_RefreshNotAvailable));

    public static string EntityModel_RefreshNotAvailable(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_RefreshNotAvailable), culture);

    public static string EntityModel_SecurityNamespaceNotAvailable() => TFCommonResources.Get(nameof (EntityModel_SecurityNamespaceNotAvailable));

    public static string EntityModel_SecurityNamespaceNotAvailable(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_SecurityNamespaceNotAvailable), culture);

    public static string EntityModel_UnsupportedDerivedType() => TFCommonResources.Get(nameof (EntityModel_UnsupportedDerivedType));

    public static string EntityModel_UnsupportedDerivedType(CultureInfo culture) => TFCommonResources.Get(nameof (EntityModel_UnsupportedDerivedType), culture);

    public static string WebAccess_UnSupportedArtifactType(object arg0) => TFCommonResources.Format(nameof (WebAccess_UnSupportedArtifactType), arg0);

    public static string WebAccess_UnSupportedArtifactType(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (WebAccess_UnSupportedArtifactType), culture, arg0);

    public static string InvalidAsynchronousOperationParameter(object arg0) => TFCommonResources.Format(nameof (InvalidAsynchronousOperationParameter), arg0);

    public static string InvalidAsynchronousOperationParameter(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidAsynchronousOperationParameter), culture, arg0);

    public static string DequeueTimeout(object arg0) => TFCommonResources.Format(nameof (DequeueTimeout), arg0);

    public static string DequeueTimeout(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (DequeueTimeout), culture, arg0);

    public static string TeamByIdDoesNotExist(object arg0) => TFCommonResources.Format(nameof (TeamByIdDoesNotExist), arg0);

    public static string TeamByIdDoesNotExist(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (TeamByIdDoesNotExist), culture, arg0);

    public static string TeamByNameDoesNotExist(object arg0) => TFCommonResources.Format(nameof (TeamByNameDoesNotExist), arg0);

    public static string TeamByNameDoesNotExist(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (TeamByNameDoesNotExist), culture, arg0);

    public static string FrameworkIdentityManagementWebDescription() => TFCommonResources.Get(nameof (FrameworkIdentityManagementWebDescription));

    public static string FrameworkIdentityManagementWebDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkIdentityManagementWebDescription), culture);

    public static string FrameworkFileHandlerServiceDescription() => TFCommonResources.Get(nameof (FrameworkFileHandlerServiceDescription));

    public static string FrameworkFileHandlerServiceDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkFileHandlerServiceDescription), culture);

    public static string FrameworkDownloadServiceDescription() => TFCommonResources.Get(nameof (FrameworkDownloadServiceDescription));

    public static string FrameworkDownloadServiceDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkDownloadServiceDescription), culture);

    public static string FrameworkSecurityManagementWebDescription() => TFCommonResources.Get(nameof (FrameworkSecurityManagementWebDescription));

    public static string FrameworkSecurityManagementWebDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkSecurityManagementWebDescription), culture);

    public static string ProjectCollectionContributorsGroupName() => TFCommonResources.Get(nameof (ProjectCollectionContributorsGroupName));

    public static string ProjectCollectionContributorsGroupName(CultureInfo culture) => TFCommonResources.Get(nameof (ProjectCollectionContributorsGroupName), culture);

    public static string ProjectCollectionContributorsGroupDesc() => TFCommonResources.Get(nameof (ProjectCollectionContributorsGroupDesc));

    public static string ProjectCollectionContributorsGroupDesc(CultureInfo culture) => TFCommonResources.Get(nameof (ProjectCollectionContributorsGroupDesc), culture);

    public static string PublicAccessMappingDisplayName() => TFCommonResources.Get(nameof (PublicAccessMappingDisplayName));

    public static string PublicAccessMappingDisplayName(CultureInfo culture) => TFCommonResources.Get(nameof (PublicAccessMappingDisplayName), culture);

    public static string ServerAccessMappingDisplayName() => TFCommonResources.Get(nameof (ServerAccessMappingDisplayName));

    public static string ServerAccessMappingDisplayName(CultureInfo culture) => TFCommonResources.Get(nameof (ServerAccessMappingDisplayName), culture);

    public static string AlternateAccessMappingDisplayName() => TFCommonResources.Get(nameof (AlternateAccessMappingDisplayName));

    public static string AlternateAccessMappingDisplayName(CultureInfo culture) => TFCommonResources.Get(nameof (AlternateAccessMappingDisplayName), culture);

    public static string TextLoggerInfoLine(object arg0) => TFCommonResources.Format(nameof (TextLoggerInfoLine), arg0);

    public static string TextLoggerInfoLine(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (TextLoggerInfoLine), culture, arg0);

    public static string TextLoggerWarningLine(object arg0) => TFCommonResources.Format(nameof (TextLoggerWarningLine), arg0);

    public static string TextLoggerWarningLine(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (TextLoggerWarningLine), culture, arg0);

    public static string TextLoggerErrorLine(object arg0) => TFCommonResources.Format(nameof (TextLoggerErrorLine), arg0);

    public static string TextLoggerErrorLine(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (TextLoggerErrorLine), culture, arg0);

    public static string IdentityPropertyReadOnly(object arg0) => TFCommonResources.Format(nameof (IdentityPropertyReadOnly), arg0);

    public static string IdentityPropertyReadOnly(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (IdentityPropertyReadOnly), culture, arg0);

    public static string ServerIncompatible(object arg0) => TFCommonResources.Format(nameof (ServerIncompatible), arg0);

    public static string ServerIncompatible(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (ServerIncompatible), culture, arg0);

    public static string InvalidPropertyScope() => TFCommonResources.Get(nameof (InvalidPropertyScope));

    public static string InvalidPropertyScope(CultureInfo culture) => TFCommonResources.Get(nameof (InvalidPropertyScope), culture);

    public static string InvalidStringPropertyValueNullAllowed(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4)
    {
      return TFCommonResources.Format(nameof (InvalidStringPropertyValueNullAllowed), arg0, arg1, arg2, arg3, arg4);
    }

    public static string InvalidStringPropertyValueNullAllowed(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (InvalidStringPropertyValueNullAllowed), culture, arg0, arg1, arg2, arg3, arg4);
    }

    public static string InvalidStringPropertyValueNullForbidden(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4)
    {
      return TFCommonResources.Format(nameof (InvalidStringPropertyValueNullForbidden), arg0, arg1, arg2, arg3, arg4);
    }

    public static string InvalidStringPropertyValueNullForbidden(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (InvalidStringPropertyValueNullForbidden), culture, arg0, arg1, arg2, arg3, arg4);
    }

    public static string ValueTypeOutOfRange(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4)
    {
      return TFCommonResources.Format(nameof (ValueTypeOutOfRange), arg0, arg1, arg2, arg3, arg4);
    }

    public static string ValueTypeOutOfRange(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (ValueTypeOutOfRange), culture, arg0, arg1, arg2, arg3, arg4);
    }

    public static string PropertyValueOutOfRange(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return TFCommonResources.Format(nameof (PropertyValueOutOfRange), arg0, arg1, arg2, arg3);
    }

    public static string PropertyValueOutOfRange(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (PropertyValueOutOfRange), culture, arg0, arg1, arg2, arg3);
    }

    public static string UnsupportedPropertyValueType(object arg0, object arg1) => TFCommonResources.Format(nameof (UnsupportedPropertyValueType), arg0, arg1);

    public static string UnsupportedPropertyValueType(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (UnsupportedPropertyValueType), culture, arg0, arg1);
    }

    public static string DoubleValueOutOfRange(object arg0, object arg1) => TFCommonResources.Format(nameof (DoubleValueOutOfRange), arg0, arg1);

    public static string DoubleValueOutOfRange(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (DoubleValueOutOfRange), culture, arg0, arg1);

    public static string DateTimeKindMustBeSpecified() => TFCommonResources.Get(nameof (DateTimeKindMustBeSpecified));

    public static string DateTimeKindMustBeSpecified(CultureInfo culture) => TFCommonResources.Get(nameof (DateTimeKindMustBeSpecified), culture);

    public static string DateTimeAgoAMinute() => TFCommonResources.Get(nameof (DateTimeAgoAMinute));

    public static string DateTimeAgoAMinute(CultureInfo culture) => TFCommonResources.Get(nameof (DateTimeAgoAMinute), culture);

    public static string DateTimeAgoLessThanMinute() => TFCommonResources.Get(nameof (DateTimeAgoLessThanMinute));

    public static string DateTimeAgoLessThanMinute(CultureInfo culture) => TFCommonResources.Get(nameof (DateTimeAgoLessThanMinute), culture);

    public static string DateTimeAgoMinutes(object arg0) => TFCommonResources.Format(nameof (DateTimeAgoMinutes), arg0);

    public static string DateTimeAgoMinutes(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (DateTimeAgoMinutes), culture, arg0);

    public static string DateTimeAgoADay() => TFCommonResources.Get(nameof (DateTimeAgoADay));

    public static string DateTimeAgoADay(CultureInfo culture) => TFCommonResources.Get(nameof (DateTimeAgoADay), culture);

    public static string DateTimeAgoAMonth() => TFCommonResources.Get(nameof (DateTimeAgoAMonth));

    public static string DateTimeAgoAMonth(CultureInfo culture) => TFCommonResources.Get(nameof (DateTimeAgoAMonth), culture);

    public static string DateTimeAgoAnHour() => TFCommonResources.Get(nameof (DateTimeAgoAnHour));

    public static string DateTimeAgoAnHour(CultureInfo culture) => TFCommonResources.Get(nameof (DateTimeAgoAnHour), culture);

    public static string DateTimeAgoAWeek() => TFCommonResources.Get(nameof (DateTimeAgoAWeek));

    public static string DateTimeAgoAWeek(CultureInfo culture) => TFCommonResources.Get(nameof (DateTimeAgoAWeek), culture);

    public static string DateTimeAgoAYear() => TFCommonResources.Get(nameof (DateTimeAgoAYear));

    public static string DateTimeAgoAYear(CultureInfo culture) => TFCommonResources.Get(nameof (DateTimeAgoAYear), culture);

    public static string DateTimeAgoDays(object arg0) => TFCommonResources.Format(nameof (DateTimeAgoDays), arg0);

    public static string DateTimeAgoDays(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (DateTimeAgoDays), culture, arg0);

    public static string DateTimeAgoHours(object arg0) => TFCommonResources.Format(nameof (DateTimeAgoHours), arg0);

    public static string DateTimeAgoHours(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (DateTimeAgoHours), culture, arg0);

    public static string DateTimeAgoMonths(object arg0) => TFCommonResources.Format(nameof (DateTimeAgoMonths), arg0);

    public static string DateTimeAgoMonths(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (DateTimeAgoMonths), culture, arg0);

    public static string DateTimeAgoWeeks(object arg0) => TFCommonResources.Format(nameof (DateTimeAgoWeeks), arg0);

    public static string DateTimeAgoWeeks(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (DateTimeAgoWeeks), culture, arg0);

    public static string DateTimeAgoYears(object arg0) => TFCommonResources.Format(nameof (DateTimeAgoYears), arg0);

    public static string DateTimeAgoYears(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (DateTimeAgoYears), culture, arg0);

    public static string TimeStampTodayAt(object arg0) => TFCommonResources.Format(nameof (TimeStampTodayAt), arg0);

    public static string TimeStampTodayAt(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (TimeStampTodayAt), culture, arg0);

    public static string TimeStampYesterdayAt(object arg0) => TFCommonResources.Format(nameof (TimeStampYesterdayAt), arg0);

    public static string TimeStampYesterdayAt(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (TimeStampYesterdayAt), culture, arg0);

    public static string TimeStampDayAt(object arg0, object arg1) => TFCommonResources.Format(nameof (TimeStampDayAt), arg0, arg1);

    public static string TimeStampDayAt(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (TimeStampDayAt), culture, arg0, arg1);

    public static string TimeStampFullDate(object arg0, object arg1) => TFCommonResources.Format(nameof (TimeStampFullDate), arg0, arg1);

    public static string TimeStampFullDate(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (TimeStampFullDate), culture, arg0, arg1);

    public static string UnauthorizedUserForReauthentication() => TFCommonResources.Get(nameof (UnauthorizedUserForReauthentication));

    public static string UnauthorizedUserForReauthentication(CultureInfo culture) => TFCommonResources.Get(nameof (UnauthorizedUserForReauthentication), culture);

    public static string FrameworkAreasManagementWebDescription() => TFCommonResources.Get(nameof (FrameworkAreasManagementWebDescription));

    public static string FrameworkAreasManagementWebDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkAreasManagementWebDescription), culture);

    public static string FrameworkIterationsManagementWebDescription() => TFCommonResources.Get(nameof (FrameworkIterationsManagementWebDescription));

    public static string FrameworkIterationsManagementWebDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkIterationsManagementWebDescription), culture);

    public static string FrameworkNewTeamProjectWebDescription() => TFCommonResources.Get(nameof (FrameworkNewTeamProjectWebDescription));

    public static string FrameworkNewTeamProjectWebDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkNewTeamProjectWebDescription), culture);

    public static string InvalidPropertyName(object arg0) => TFCommonResources.Format(nameof (InvalidPropertyName), arg0);

    public static string InvalidPropertyName(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidPropertyName), culture, arg0);

    public static string PerformanceCounterCategoryNotRegistered(object arg0) => TFCommonResources.Format(nameof (PerformanceCounterCategoryNotRegistered), arg0);

    public static string PerformanceCounterCategoryNotRegistered(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (PerformanceCounterCategoryNotRegistered), culture, arg0);

    public static string FrameworkProjectAlertsWebDescription() => TFCommonResources.Get(nameof (FrameworkProjectAlertsWebDescription));

    public static string FrameworkProjectAlertsWebDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkProjectAlertsWebDescription), culture);

    public static string FrameworkTaskBoardWebDescription() => TFCommonResources.Get(nameof (FrameworkTaskBoardWebDescription));

    public static string FrameworkTaskBoardWebDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkTaskBoardWebDescription), culture);

    public static string GSS_ARGUMENT_EXCEPTION(object arg0) => TFCommonResources.Format(nameof (GSS_ARGUMENT_EXCEPTION), arg0);

    public static string GSS_ARGUMENT_EXCEPTION(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (GSS_ARGUMENT_EXCEPTION), culture, arg0);

    public static string GSS_BAD_ACCOUNT_NAME_ARRAY(object arg0) => TFCommonResources.Format(nameof (GSS_BAD_ACCOUNT_NAME_ARRAY), arg0);

    public static string GSS_BAD_ACCOUNT_NAME_ARRAY(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (GSS_BAD_ACCOUNT_NAME_ARRAY), culture, arg0);

    public static string GSS_BAD_ACTIONID(object arg0, object arg1) => TFCommonResources.Format(nameof (GSS_BAD_ACTIONID), arg0, arg1);

    public static string GSS_BAD_ACTIONID(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (GSS_BAD_ACTIONID), culture, arg0, arg1);

    public static string GSS_BAD_ACTIONID_ARRAY(object arg0) => TFCommonResources.Format(nameof (GSS_BAD_ACTIONID_ARRAY), arg0);

    public static string GSS_BAD_ACTIONID_ARRAY(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (GSS_BAD_ACTIONID_ARRAY), culture, arg0);

    public static string GSS_BAD_CLASSID_ACTIONID_PAIR_EXCEPTION(object arg0, object arg1) => TFCommonResources.Format(nameof (GSS_BAD_CLASSID_ACTIONID_PAIR_EXCEPTION), arg0, arg1);

    public static string GSS_BAD_CLASSID_ACTIONID_PAIR_EXCEPTION(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (GSS_BAD_CLASSID_ACTIONID_PAIR_EXCEPTION), culture, arg0, arg1);
    }

    public static string GSS_BAD_DISTINGUISHED_NAME(object arg0) => TFCommonResources.Format(nameof (GSS_BAD_DISTINGUISHED_NAME), arg0);

    public static string GSS_BAD_DISTINGUISHED_NAME(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (GSS_BAD_DISTINGUISHED_NAME), culture, arg0);

    public static string GSS_BAD_DISTINGUISHED_NAME_ARRAY(object arg0) => TFCommonResources.Format(nameof (GSS_BAD_DISTINGUISHED_NAME_ARRAY), arg0);

    public static string GSS_BAD_DISTINGUISHED_NAME_ARRAY(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (GSS_BAD_DISTINGUISHED_NAME_ARRAY), culture, arg0);

    public static string GSS_BAD_OBJECTID(object arg0, object arg1) => TFCommonResources.Format(nameof (GSS_BAD_OBJECTID), arg0, arg1);

    public static string GSS_BAD_OBJECTID(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (GSS_BAD_OBJECTID), culture, arg0, arg1);

    public static string GSS_BAD_OBJECTID_ARRAY(object arg0) => TFCommonResources.Format(nameof (GSS_BAD_OBJECTID_ARRAY), arg0);

    public static string GSS_BAD_OBJECTID_ARRAY(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (GSS_BAD_OBJECTID_ARRAY), culture, arg0);

    public static string GSS_BAD_OBJECT_CLASS_ID(object arg0, object arg1) => TFCommonResources.Format(nameof (GSS_BAD_OBJECT_CLASS_ID), arg0, arg1);

    public static string GSS_BAD_OBJECT_CLASS_ID(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (GSS_BAD_OBJECT_CLASS_ID), culture, arg0, arg1);

    public static string GSS_BAD_PARENTOBJECTID_SELFPARENT(object arg0, object arg1) => TFCommonResources.Format(nameof (GSS_BAD_PARENTOBJECTID_SELFPARENT), arg0, arg1);

    public static string GSS_BAD_PARENTOBJECTID_SELFPARENT(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (GSS_BAD_PARENTOBJECTID_SELFPARENT), culture, arg0, arg1);
    }

    public static string GSS_BAD_PROJECT_URI(object arg0, object arg1) => TFCommonResources.Format(nameof (GSS_BAD_PROJECT_URI), arg0, arg1);

    public static string GSS_BAD_PROJECT_URI(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (GSS_BAD_PROJECT_URI), culture, arg0, arg1);

    public static string GSS_BAD_PROJECT_URI_ARRAY(object arg0) => TFCommonResources.Format(nameof (GSS_BAD_PROJECT_URI_ARRAY), arg0);

    public static string GSS_BAD_PROJECT_URI_ARRAY(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (GSS_BAD_PROJECT_URI_ARRAY), culture, arg0);

    public static string GSS_BAD_SID_ARRAY(object arg0) => TFCommonResources.Format(nameof (GSS_BAD_SID_ARRAY), arg0);

    public static string GSS_BAD_SID_ARRAY(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (GSS_BAD_SID_ARRAY), culture, arg0);

    public static string NAMESPACE_CREATE_PROJECTS() => TFCommonResources.Get(nameof (NAMESPACE_CREATE_PROJECTS));

    public static string NAMESPACE_CREATE_PROJECTS(CultureInfo culture) => TFCommonResources.Get(nameof (NAMESPACE_CREATE_PROJECTS), culture);

    public static string NAMESPACE_DIAGNOSTIC_TRACE() => TFCommonResources.Get(nameof (NAMESPACE_DIAGNOSTIC_TRACE));

    public static string NAMESPACE_DIAGNOSTIC_TRACE(CultureInfo culture) => TFCommonResources.Get(nameof (NAMESPACE_DIAGNOSTIC_TRACE), culture);

    public static string NAMESPACE_GENERIC_READ() => TFCommonResources.Get(nameof (NAMESPACE_GENERIC_READ));

    public static string NAMESPACE_GENERIC_READ(CultureInfo culture) => TFCommonResources.Get(nameof (NAMESPACE_GENERIC_READ), culture);

    public static string NAMESPACE_GENERIC_WRITE() => TFCommonResources.Get(nameof (NAMESPACE_GENERIC_WRITE));

    public static string NAMESPACE_GENERIC_WRITE(CultureInfo culture) => TFCommonResources.Get(nameof (NAMESPACE_GENERIC_WRITE), culture);

    public static string NAMESPACE_MANAGE_LINK_TYPES() => TFCommonResources.Get(nameof (NAMESPACE_MANAGE_LINK_TYPES));

    public static string NAMESPACE_MANAGE_LINK_TYPES(CultureInfo culture) => TFCommonResources.Get(nameof (NAMESPACE_MANAGE_LINK_TYPES), culture);

    public static string NAMESPACE_MANAGE_TEMPLATE() => TFCommonResources.Get(nameof (NAMESPACE_MANAGE_TEMPLATE));

    public static string NAMESPACE_MANAGE_TEMPLATE(CultureInfo culture) => TFCommonResources.Get(nameof (NAMESPACE_MANAGE_TEMPLATE), culture);

    public static string NAMESPACE_MANAGE_TEST_CONTROLLERS() => TFCommonResources.Get(nameof (NAMESPACE_MANAGE_TEST_CONTROLLERS));

    public static string NAMESPACE_MANAGE_TEST_CONTROLLERS(CultureInfo culture) => TFCommonResources.Get(nameof (NAMESPACE_MANAGE_TEST_CONTROLLERS), culture);

    public static string NAMESPACE_SYNCHRONIZE_READ() => TFCommonResources.Get(nameof (NAMESPACE_SYNCHRONIZE_READ));

    public static string NAMESPACE_SYNCHRONIZE_READ(CultureInfo culture) => TFCommonResources.Get(nameof (NAMESPACE_SYNCHRONIZE_READ), culture);

    public static string NAMESPACE_TRIGGER_EVENT() => TFCommonResources.Get(nameof (NAMESPACE_TRIGGER_EVENT));

    public static string NAMESPACE_TRIGGER_EVENT(CultureInfo culture) => TFCommonResources.Get(nameof (NAMESPACE_TRIGGER_EVENT), culture);

    public static string PROJECT_ADMINISTER_BUILD() => TFCommonResources.Get(nameof (PROJECT_ADMINISTER_BUILD));

    public static string PROJECT_ADMINISTER_BUILD(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_ADMINISTER_BUILD), culture);

    public static string PROJECT_CHECK_IN() => TFCommonResources.Get(nameof (PROJECT_CHECK_IN));

    public static string PROJECT_CHECK_IN(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_CHECK_IN), culture);

    public static string PROJECT_DELETE() => TFCommonResources.Get(nameof (PROJECT_DELETE));

    public static string PROJECT_DELETE(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_DELETE), culture);

    public static string PROJECT_DELETE_TEST_RESULTS() => TFCommonResources.Get(nameof (PROJECT_DELETE_TEST_RESULTS));

    public static string PROJECT_DELETE_TEST_RESULTS(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_DELETE_TEST_RESULTS), culture);

    public static string PROJECT_EDIT_BUILD_STATUS() => TFCommonResources.Get(nameof (PROJECT_EDIT_BUILD_STATUS));

    public static string PROJECT_EDIT_BUILD_STATUS(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_EDIT_BUILD_STATUS), culture);

    public static string PROJECT_GENERIC_READ() => TFCommonResources.Get(nameof (PROJECT_GENERIC_READ));

    public static string PROJECT_GENERIC_READ(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_GENERIC_READ), culture);

    public static string PROJECT_GENERIC_WRITE() => TFCommonResources.Get(nameof (PROJECT_GENERIC_WRITE));

    public static string PROJECT_GENERIC_WRITE(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_GENERIC_WRITE), culture);

    public static string PROJECT_MANAGE_TEST_CONFIGURATIONS() => TFCommonResources.Get(nameof (PROJECT_MANAGE_TEST_CONFIGURATIONS));

    public static string PROJECT_MANAGE_TEST_CONFIGURATIONS(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_MANAGE_TEST_CONFIGURATIONS), culture);

    public static string PROJECT_MANAGE_TEST_ENVIRONMENTS() => TFCommonResources.Get(nameof (PROJECT_MANAGE_TEST_ENVIRONMENTS));

    public static string PROJECT_MANAGE_TEST_ENVIRONMENTS(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_MANAGE_TEST_ENVIRONMENTS), culture);

    public static string PROJECT_OVERRIDE_BUILD_CHECKIN_VALIDATION() => TFCommonResources.Get(nameof (PROJECT_OVERRIDE_BUILD_CHECKIN_VALIDATION));

    public static string PROJECT_OVERRIDE_BUILD_CHECKIN_VALIDATION(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_OVERRIDE_BUILD_CHECKIN_VALIDATION), culture);

    public static string PROJECT_PUBLISH_TEST_RESULTS() => TFCommonResources.Get(nameof (PROJECT_PUBLISH_TEST_RESULTS));

    public static string PROJECT_PUBLISH_TEST_RESULTS(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_PUBLISH_TEST_RESULTS), culture);

    public static string PROJECT_START_BUILD() => TFCommonResources.Get(nameof (PROJECT_START_BUILD));

    public static string PROJECT_START_BUILD(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_START_BUILD), culture);

    public static string PROJECT_UPDATE_BUILD() => TFCommonResources.Get(nameof (PROJECT_UPDATE_BUILD));

    public static string PROJECT_UPDATE_BUILD(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_UPDATE_BUILD), culture);

    public static string PROJECT_VIEW_TEST_RESULTS() => TFCommonResources.Get(nameof (PROJECT_VIEW_TEST_RESULTS));

    public static string PROJECT_VIEW_TEST_RESULTS(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_VIEW_TEST_RESULTS), culture);

    public static string CannotResolveServerHostUsingAD(object arg0, object arg1) => TFCommonResources.Format(nameof (CannotResolveServerHostUsingAD), arg0, arg1);

    public static string CannotResolveServerHostUsingAD(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (CannotResolveServerHostUsingAD), culture, arg0, arg1);
    }

    public static string MultipleIdentitiesFoundMessage(object arg0, object arg1) => TFCommonResources.Format(nameof (MultipleIdentitiesFoundMessage), arg0, arg1);

    public static string MultipleIdentitiesFoundMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (MultipleIdentitiesFoundMessage), culture, arg0, arg1);
    }

    public static string InvalidServerRequest(object arg0) => TFCommonResources.Format(nameof (InvalidServerRequest), arg0);

    public static string InvalidServerRequest(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidServerRequest), culture, arg0);

    public static string FrameworkConnectedServicesServiceDescription() => TFCommonResources.Get(nameof (FrameworkConnectedServicesServiceDescription));

    public static string FrameworkConnectedServicesServiceDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkConnectedServicesServiceDescription), culture);

    public static string JavaScriptSerializer_JSON_IllegalPrimitive(object arg0) => TFCommonResources.Format(nameof (JavaScriptSerializer_JSON_IllegalPrimitive), arg0);

    public static string JavaScriptSerializer_JSON_IllegalPrimitive(
      object arg0,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (JavaScriptSerializer_JSON_IllegalPrimitive), culture, arg0);
    }

    public static string JavaScriptSerializer_JSON_DepthLimitExceeded() => TFCommonResources.Get(nameof (JavaScriptSerializer_JSON_DepthLimitExceeded));

    public static string JavaScriptSerializer_JSON_DepthLimitExceeded(CultureInfo culture) => TFCommonResources.Get(nameof (JavaScriptSerializer_JSON_DepthLimitExceeded), culture);

    public static string JavaScriptSerializer_JSON_InvalidArrayStart() => TFCommonResources.Get(nameof (JavaScriptSerializer_JSON_InvalidArrayStart));

    public static string JavaScriptSerializer_JSON_InvalidArrayStart(CultureInfo culture) => TFCommonResources.Get(nameof (JavaScriptSerializer_JSON_InvalidArrayStart), culture);

    public static string JavaScriptSerializer_JSON_InvalidArrayExpectComma() => TFCommonResources.Get(nameof (JavaScriptSerializer_JSON_InvalidArrayExpectComma));

    public static string JavaScriptSerializer_JSON_InvalidArrayExpectComma(CultureInfo culture) => TFCommonResources.Get(nameof (JavaScriptSerializer_JSON_InvalidArrayExpectComma), culture);

    public static string JavaScriptSerializer_JSON_InvalidArrayExtraComma() => TFCommonResources.Get(nameof (JavaScriptSerializer_JSON_InvalidArrayExtraComma));

    public static string JavaScriptSerializer_JSON_InvalidArrayExtraComma(CultureInfo culture) => TFCommonResources.Get(nameof (JavaScriptSerializer_JSON_InvalidArrayExtraComma), culture);

    public static string JavaScriptSerializer_JSON_InvalidArrayEnd() => TFCommonResources.Get(nameof (JavaScriptSerializer_JSON_InvalidArrayEnd));

    public static string JavaScriptSerializer_JSON_InvalidArrayEnd(CultureInfo culture) => TFCommonResources.Get(nameof (JavaScriptSerializer_JSON_InvalidArrayEnd), culture);

    public static string JavaScriptSerializer_JSON_ExpectedOpenBrace() => TFCommonResources.Get(nameof (JavaScriptSerializer_JSON_ExpectedOpenBrace));

    public static string JavaScriptSerializer_JSON_ExpectedOpenBrace(CultureInfo culture) => TFCommonResources.Get(nameof (JavaScriptSerializer_JSON_ExpectedOpenBrace), culture);

    public static string JavaScriptSerializer_JSON_InvalidMemberName() => TFCommonResources.Get(nameof (JavaScriptSerializer_JSON_InvalidMemberName));

    public static string JavaScriptSerializer_JSON_InvalidMemberName(CultureInfo culture) => TFCommonResources.Get(nameof (JavaScriptSerializer_JSON_InvalidMemberName), culture);

    public static string JavaScriptSerializer_JSON_InvalidObject() => TFCommonResources.Get(nameof (JavaScriptSerializer_JSON_InvalidObject));

    public static string JavaScriptSerializer_JSON_InvalidObject(CultureInfo culture) => TFCommonResources.Get(nameof (JavaScriptSerializer_JSON_InvalidObject), culture);

    public static string JavaScriptSerializer_JSON_UnterminatedString() => TFCommonResources.Get(nameof (JavaScriptSerializer_JSON_UnterminatedString));

    public static string JavaScriptSerializer_JSON_UnterminatedString(CultureInfo culture) => TFCommonResources.Get(nameof (JavaScriptSerializer_JSON_UnterminatedString), culture);

    public static string JavaScriptSerializer_JSON_BadEscape() => TFCommonResources.Get(nameof (JavaScriptSerializer_JSON_BadEscape));

    public static string JavaScriptSerializer_JSON_BadEscape(CultureInfo culture) => TFCommonResources.Get(nameof (JavaScriptSerializer_JSON_BadEscape), culture);

    public static string JavaScriptSerializer_JSON_StringNotQuoted() => TFCommonResources.Get(nameof (JavaScriptSerializer_JSON_StringNotQuoted));

    public static string JavaScriptSerializer_JSON_StringNotQuoted(CultureInfo culture) => TFCommonResources.Get(nameof (JavaScriptSerializer_JSON_StringNotQuoted), culture);

    public static string FrameworkSignOutWebDescription() => TFCommonResources.Get(nameof (FrameworkSignOutWebDescription));

    public static string FrameworkSignOutWebDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkSignOutWebDescription), culture);

    public static string FrameworkFileContainerDescription() => TFCommonResources.Get(nameof (FrameworkFileContainerDescription));

    public static string FrameworkFileContainerDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkFileContainerDescription), culture);

    public static string InvalidSqlTimestampString(object arg0) => TFCommonResources.Format(nameof (InvalidSqlTimestampString), arg0);

    public static string InvalidSqlTimestampString(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidSqlTimestampString), culture, arg0);

    public static string ServiceChangeError(object arg0) => TFCommonResources.Format(nameof (ServiceChangeError), arg0);

    public static string ServiceChangeError(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (ServiceChangeError), culture, arg0);

    public static string ServiceControlManagerOpenError() => TFCommonResources.Get(nameof (ServiceControlManagerOpenError));

    public static string ServiceControlManagerOpenError(CultureInfo culture) => TFCommonResources.Get(nameof (ServiceControlManagerOpenError), culture);

    public static string ServiceOpenError(object arg0) => TFCommonResources.Format(nameof (ServiceOpenError), arg0);

    public static string ServiceOpenError(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (ServiceOpenError), culture, arg0);

    public static string BAD_ALIAS(object arg0) => TFCommonResources.Format(nameof (BAD_ALIAS), arg0);

    public static string BAD_ALIAS(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (BAD_ALIAS), culture, arg0);

    public static string BAD_ALIAS_NOT_ALPHANUM(object arg0) => TFCommonResources.Format(nameof (BAD_ALIAS_NOT_ALPHANUM), arg0);

    public static string BAD_ALIAS_NOT_ALPHANUM(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (BAD_ALIAS_NOT_ALPHANUM), culture, arg0);

    public static string FrameworkRoomsHubWebDescription() => TFCommonResources.Get(nameof (FrameworkRoomsHubWebDescription));

    public static string FrameworkRoomsHubWebDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkRoomsHubWebDescription), culture);

    public static string FrameworkUtilizationUsageSummaryDescription() => TFCommonResources.Get(nameof (FrameworkUtilizationUsageSummaryDescription));

    public static string FrameworkUtilizationUsageSummaryDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkUtilizationUsageSummaryDescription), culture);

    public static string FrameworkUtilizationUserUsageSummaryDescription() => TFCommonResources.Get(nameof (FrameworkUtilizationUserUsageSummaryDescription));

    public static string FrameworkUtilizationUserUsageSummaryDescription(CultureInfo culture) => TFCommonResources.Get(nameof (FrameworkUtilizationUserUsageSummaryDescription), culture);

    public static string LocationService() => TFCommonResources.Get(nameof (LocationService));

    public static string LocationService(CultureInfo culture) => TFCommonResources.Get(nameof (LocationService), culture);

    public static string AccountTenantExists() => TFCommonResources.Get(nameof (AccountTenantExists));

    public static string AccountTenantExists(CultureInfo culture) => TFCommonResources.Get(nameof (AccountTenantExists), culture);

    public static string InvalidInheritLevelServiceDefinition(object arg0) => TFCommonResources.Format(nameof (InvalidInheritLevelServiceDefinition), arg0);

    public static string InvalidInheritLevelServiceDefinition(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidInheritLevelServiceDefinition), culture, arg0);

    public static string InvalidLocationMappingsServiceDefinition() => TFCommonResources.Get(nameof (InvalidLocationMappingsServiceDefinition));

    public static string InvalidLocationMappingsServiceDefinition(CultureInfo culture) => TFCommonResources.Get(nameof (InvalidLocationMappingsServiceDefinition), culture);

    public static string InvalidInheritLevelRelativeServiceDefinition(object arg0, object arg1) => TFCommonResources.Format(nameof (InvalidInheritLevelRelativeServiceDefinition), arg0, arg1);

    public static string InvalidInheritLevelRelativeServiceDefinition(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TFCommonResources.Format(nameof (InvalidInheritLevelRelativeServiceDefinition), culture, arg0, arg1);
    }

    public static string InvalidGroupDescriptor(object arg0) => TFCommonResources.Format(nameof (InvalidGroupDescriptor), arg0);

    public static string InvalidGroupDescriptor(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (InvalidGroupDescriptor), culture, arg0);

    public static string PROCESS_PERMISSION_DELETE() => TFCommonResources.Get(nameof (PROCESS_PERMISSION_DELETE));

    public static string PROCESS_PERMISSION_DELETE(CultureInfo culture) => TFCommonResources.Get(nameof (PROCESS_PERMISSION_DELETE), culture);

    public static string PROCESS_PERMISSION_EDIT() => TFCommonResources.Get(nameof (PROCESS_PERMISSION_EDIT));

    public static string PROCESS_PERMISSION_EDIT(CultureInfo culture) => TFCommonResources.Get(nameof (PROCESS_PERMISSION_EDIT), culture);

    public static string PROCESS_PERMISSION_CREATE() => TFCommonResources.Get(nameof (PROCESS_PERMISSION_CREATE));

    public static string PROCESS_PERMISSION_CREATE(CultureInfo culture) => TFCommonResources.Get(nameof (PROCESS_PERMISSION_CREATE), culture);

    public static string PROCESS_PERMISSIONS_ADMINISTER() => TFCommonResources.Get(nameof (PROCESS_PERMISSIONS_ADMINISTER));

    public static string PROCESS_PERMISSIONS_ADMINISTER(CultureInfo culture) => TFCommonResources.Get(nameof (PROCESS_PERMISSIONS_ADMINISTER), culture);

    public static string PROCESS_PERMISSION_READ_RULES() => TFCommonResources.Get(nameof (PROCESS_PERMISSION_READ_RULES));

    public static string PROCESS_PERMISSION_READ_RULES(CultureInfo culture) => TFCommonResources.Get(nameof (PROCESS_PERMISSION_READ_RULES), culture);

    public static string PROJECT_WORK_ITEM_DELETE() => TFCommonResources.Get(nameof (PROJECT_WORK_ITEM_DELETE));

    public static string PROJECT_WORK_ITEM_DELETE(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_WORK_ITEM_DELETE), culture);

    public static string PROJECT_WORK_ITEM_MOVE() => TFCommonResources.Get(nameof (PROJECT_WORK_ITEM_MOVE));

    public static string PROJECT_WORK_ITEM_MOVE(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_WORK_ITEM_MOVE), culture);

    public static string PROJECT_WORK_ITEM_PERMANENTLY_DELETE() => TFCommonResources.Get(nameof (PROJECT_WORK_ITEM_PERMANENTLY_DELETE));

    public static string PROJECT_WORK_ITEM_PERMANENTLY_DELETE(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_WORK_ITEM_PERMANENTLY_DELETE), culture);

    public static string ErrorCreatingTempFile(object arg0) => TFCommonResources.Format(nameof (ErrorCreatingTempFile), arg0);

    public static string ErrorCreatingTempFile(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (ErrorCreatingTempFile), culture, arg0);

    public static string PROJECT_RENAME() => TFCommonResources.Get(nameof (PROJECT_RENAME));

    public static string PROJECT_RENAME(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_RENAME), culture);

    public static string XML_SCHEMAVALIDATION_READSCHEMAFAILED() => TFCommonResources.Get(nameof (XML_SCHEMAVALIDATION_READSCHEMAFAILED));

    public static string XML_SCHEMAVALIDATION_READSCHEMAFAILED(CultureInfo culture) => TFCommonResources.Get(nameof (XML_SCHEMAVALIDATION_READSCHEMAFAILED), culture);

    public static string XML_SCHEMAVALIDATION_FAILED(object arg0, object arg1) => TFCommonResources.Format(nameof (XML_SCHEMAVALIDATION_FAILED), arg0, arg1);

    public static string XML_SCHEMAVALIDATION_FAILED(object arg0, object arg1, CultureInfo culture) => TFCommonResources.Format(nameof (XML_SCHEMAVALIDATION_FAILED), culture, arg0, arg1);

    public static string BAD_DIRECTORY_ALIAS(object arg0) => TFCommonResources.Format(nameof (BAD_DIRECTORY_ALIAS), arg0);

    public static string BAD_DIRECTORY_ALIAS(object arg0, CultureInfo culture) => TFCommonResources.Format(nameof (BAD_DIRECTORY_ALIAS), culture, arg0);

    public static string InvalidHexString() => TFCommonResources.Get(nameof (InvalidHexString));

    public static string InvalidHexString(CultureInfo culture) => TFCommonResources.Get(nameof (InvalidHexString), culture);

    public static string PROJECT_MANAGE_PROPERTIES() => TFCommonResources.Get(nameof (PROJECT_MANAGE_PROPERTIES));

    public static string PROJECT_MANAGE_PROPERTIES(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_MANAGE_PROPERTIES), culture);

    public static string PROJECT_MANAGE_SYSTEM_PROPERTIES() => TFCommonResources.Get(nameof (PROJECT_MANAGE_SYSTEM_PROPERTIES));

    public static string PROJECT_MANAGE_SYSTEM_PROPERTIES(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_MANAGE_SYSTEM_PROPERTIES), culture);

    public static string PROJECT_BYPASS_PROPERTY_CACHE() => TFCommonResources.Get(nameof (PROJECT_BYPASS_PROPERTY_CACHE));

    public static string PROJECT_BYPASS_PROPERTY_CACHE(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_BYPASS_PROPERTY_CACHE), culture);

    public static string PROJECT_BYPASS_RULES() => TFCommonResources.Get(nameof (PROJECT_BYPASS_RULES));

    public static string PROJECT_BYPASS_RULES(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_BYPASS_RULES), culture);

    public static string PROJECT_SUPPRESS_NOTIFICATIONS() => TFCommonResources.Get(nameof (PROJECT_SUPPRESS_NOTIFICATIONS));

    public static string PROJECT_SUPPRESS_NOTIFICATIONS(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_SUPPRESS_NOTIFICATIONS), culture);

    public static string NAMESPACE_DELETE_FIELD() => TFCommonResources.Get(nameof (NAMESPACE_DELETE_FIELD));

    public static string NAMESPACE_DELETE_FIELD(CultureInfo culture) => TFCommonResources.Get(nameof (NAMESPACE_DELETE_FIELD), culture);

    public static string NAMESPACE_MANAGE_ENTERPRISE_POLICIES() => TFCommonResources.Get(nameof (NAMESPACE_MANAGE_ENTERPRISE_POLICIES));

    public static string NAMESPACE_MANAGE_ENTERPRISE_POLICIES(CultureInfo culture) => TFCommonResources.Get(nameof (NAMESPACE_MANAGE_ENTERPRISE_POLICIES), culture);

    public static string PROJECT_UPDATE_VISIBILITY() => TFCommonResources.Get(nameof (PROJECT_UPDATE_VISIBILITY));

    public static string PROJECT_UPDATE_VISIBILITY(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_UPDATE_VISIBILITY), culture);

    public static string PROCESS_PERMISSION_READ() => TFCommonResources.Get(nameof (PROCESS_PERMISSION_READ));

    public static string PROCESS_PERMISSION_READ(CultureInfo culture) => TFCommonResources.Get(nameof (PROCESS_PERMISSION_READ), culture);

    public static string MessageQueueAccessMappingDisplayName() => TFCommonResources.Get(nameof (MessageQueueAccessMappingDisplayName));

    public static string MessageQueueAccessMappingDisplayName(CultureInfo culture) => TFCommonResources.Get(nameof (MessageQueueAccessMappingDisplayName), culture);

    public static string PROJECT_CHANGE_PROCESS() => TFCommonResources.Get(nameof (PROJECT_CHANGE_PROCESS));

    public static string PROJECT_CHANGE_PROCESS(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_CHANGE_PROCESS), culture);

    public static string PROJECT_AGILETOOLS_BACKLOG() => TFCommonResources.Get(nameof (PROJECT_AGILETOOLS_BACKLOG));

    public static string PROJECT_AGILETOOLS_BACKLOG(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_AGILETOOLS_BACKLOG), culture);

    public static string PROJECT_AGILETOOLS_PLANS() => TFCommonResources.Get(nameof (PROJECT_AGILETOOLS_PLANS));

    public static string PROJECT_AGILETOOLS_PLANS(CultureInfo culture) => TFCommonResources.Get(nameof (PROJECT_AGILETOOLS_PLANS), culture);
  }
}
