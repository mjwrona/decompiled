// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.ApprovalResources
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.Azure.Pipelines.Approval.Server
{
  internal static class ApprovalResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (ApprovalResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ApprovalResources.s_resMgr;

    private static string Get(string resourceName) => ApprovalResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ApprovalResources.Get(resourceName) : ApprovalResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ApprovalResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ApprovalResources.GetInt(resourceName) : (int) ApprovalResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ApprovalResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ApprovalResources.GetBool(resourceName) : (bool) ApprovalResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ApprovalResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ApprovalResources.Get(resourceName, culture);
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

    public static string InvalidApprovalConfigMinRequiredApproversInput() => ApprovalResources.Get(nameof (InvalidApprovalConfigMinRequiredApproversInput));

    public static string InvalidApprovalConfigMinRequiredApproversInput(CultureInfo culture) => ApprovalResources.Get(nameof (InvalidApprovalConfigMinRequiredApproversInput), culture);

    public static string InvalidApprovalsQueryParameters() => ApprovalResources.Get(nameof (InvalidApprovalsQueryParameters));

    public static string InvalidApprovalsQueryParameters(CultureInfo culture) => ApprovalResources.Get(nameof (InvalidApprovalsQueryParameters), culture);

    public static string InvalidApprovalId() => ApprovalResources.Get(nameof (InvalidApprovalId));

    public static string InvalidApprovalId(CultureInfo culture) => ApprovalResources.Get(nameof (InvalidApprovalId), culture);

    public static string ApprovalBlockedExceptionMessage(object arg0, object arg1) => ApprovalResources.Format(nameof (ApprovalBlockedExceptionMessage), arg0, arg1);

    public static string ApprovalBlockedExceptionMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ApprovalResources.Format(nameof (ApprovalBlockedExceptionMessage), culture, arg0, arg1);
    }

    public static string ApprovalExists(object arg0) => ApprovalResources.Format(nameof (ApprovalExists), arg0);

    public static string ApprovalExists(object arg0, CultureInfo culture) => ApprovalResources.Format(nameof (ApprovalExists), culture, arg0);

    public static string ApprovalNotFoundExceptionMessage() => ApprovalResources.Get(nameof (ApprovalNotFoundExceptionMessage));

    public static string ApprovalNotFoundExceptionMessage(CultureInfo culture) => ApprovalResources.Get(nameof (ApprovalNotFoundExceptionMessage), culture);

    public static string ApprovalParametersMaxLengthExceptionMessage(object arg0) => ApprovalResources.Format(nameof (ApprovalParametersMaxLengthExceptionMessage), arg0);

    public static string ApprovalParametersMaxLengthExceptionMessage(
      object arg0,
      CultureInfo culture)
    {
      return ApprovalResources.Format(nameof (ApprovalParametersMaxLengthExceptionMessage), culture, arg0);
    }

    public static string ApprovalUnauthorizedExceptionMessage(object arg0, object arg1) => ApprovalResources.Format(nameof (ApprovalUnauthorizedExceptionMessage), arg0, arg1);

    public static string ApprovalUnauthorizedExceptionMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ApprovalResources.Format(nameof (ApprovalUnauthorizedExceptionMessage), culture, arg0, arg1);
    }

    public static string ApprovalIdNotProvided() => ApprovalResources.Get(nameof (ApprovalIdNotProvided));

    public static string ApprovalIdNotProvided(CultureInfo culture) => ApprovalResources.Get(nameof (ApprovalIdNotProvided), culture);

    public static string InvalidApprovalConfigInSequenceMinRequiredApproversInput() => ApprovalResources.Get(nameof (InvalidApprovalConfigInSequenceMinRequiredApproversInput));

    public static string InvalidApprovalConfigInSequenceMinRequiredApproversInput(
      CultureInfo culture)
    {
      return ApprovalResources.Get(nameof (InvalidApprovalConfigInSequenceMinRequiredApproversInput), culture);
    }

    public static string GenericDatabaseUpdateError() => ApprovalResources.Get(nameof (GenericDatabaseUpdateError));

    public static string GenericDatabaseUpdateError(CultureInfo culture) => ApprovalResources.Get(nameof (GenericDatabaseUpdateError), culture);

    public static string ApprovalAlreadyCompletedExceptionMessage() => ApprovalResources.Get(nameof (ApprovalAlreadyCompletedExceptionMessage));

    public static string ApprovalAlreadyCompletedExceptionMessage(CultureInfo culture) => ApprovalResources.Get(nameof (ApprovalAlreadyCompletedExceptionMessage), culture);

    public static string DuplicateApprovalCreateRequests() => ApprovalResources.Get(nameof (DuplicateApprovalCreateRequests));

    public static string DuplicateApprovalCreateRequests(CultureInfo culture) => ApprovalResources.Get(nameof (DuplicateApprovalCreateRequests), culture);

    public static string InvalidIdentities(object arg0) => ApprovalResources.Format(nameof (InvalidIdentities), arg0);

    public static string InvalidIdentities(object arg0, CultureInfo culture) => ApprovalResources.Format(nameof (InvalidIdentities), culture, arg0);

    public static string StringValueCommaSeparator() => ApprovalResources.Get(nameof (StringValueCommaSeparator));

    public static string StringValueCommaSeparator(CultureInfo culture) => ApprovalResources.Get(nameof (StringValueCommaSeparator), culture);

    public static string InvalidQueryFilteringParameters() => ApprovalResources.Get(nameof (InvalidQueryFilteringParameters));

    public static string InvalidQueryFilteringParameters(CultureInfo culture) => ApprovalResources.Get(nameof (InvalidQueryFilteringParameters), culture);

    public static string ApprovalUpdateRequestFailed(object arg0) => ApprovalResources.Format(nameof (ApprovalUpdateRequestFailed), arg0);

    public static string ApprovalUpdateRequestFailed(object arg0, CultureInfo culture) => ApprovalResources.Format(nameof (ApprovalUpdateRequestFailed), culture, arg0);

    public static string ApprovalReassignRequestFailed(object arg0) => ApprovalResources.Format(nameof (ApprovalReassignRequestFailed), arg0);

    public static string ApprovalReassignRequestFailed(object arg0, CultureInfo culture) => ApprovalResources.Format(nameof (ApprovalReassignRequestFailed), culture, arg0);

    public static string ApprovalReassignUnauthorizedExceptionMessage(object arg0, object arg1) => ApprovalResources.Format(nameof (ApprovalReassignUnauthorizedExceptionMessage), arg0, arg1);

    public static string ApprovalReassignUnauthorizedExceptionMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ApprovalResources.Format(nameof (ApprovalReassignUnauthorizedExceptionMessage), culture, arg0, arg1);
    }

    public static string ApproverExists(object arg0, object arg1) => ApprovalResources.Format(nameof (ApproverExists), arg0, arg1);

    public static string ApproverExists(object arg0, object arg1, CultureInfo culture) => ApprovalResources.Format(nameof (ApproverExists), culture, arg0, arg1);

    public static string ApprovalReassignBlockedExceptionMessage(object arg0, object arg1) => ApprovalResources.Format(nameof (ApprovalReassignBlockedExceptionMessage), arg0, arg1);

    public static string ApprovalReassignBlockedExceptionMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ApprovalResources.Format(nameof (ApprovalReassignBlockedExceptionMessage), culture, arg0, arg1);
    }

    public static string InvalidApprovalReassignRequest(object arg0) => ApprovalResources.Format(nameof (InvalidApprovalReassignRequest), arg0);

    public static string InvalidApprovalReassignRequest(object arg0, CultureInfo culture) => ApprovalResources.Format(nameof (InvalidApprovalReassignRequest), culture, arg0);

    public static string InvalidApprovalOwner() => ApprovalResources.Get(nameof (InvalidApprovalOwner));

    public static string InvalidApprovalOwner(CultureInfo culture) => ApprovalResources.Get(nameof (InvalidApprovalOwner), culture);

    public static string ApprovalSkippedUnauthorizedExceptionMessage(object arg0, object arg1) => ApprovalResources.Format(nameof (ApprovalSkippedUnauthorizedExceptionMessage), arg0, arg1);

    public static string ApprovalSkippedUnauthorizedExceptionMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ApprovalResources.Format(nameof (ApprovalSkippedUnauthorizedExceptionMessage), culture, arg0, arg1);
    }

    public static string DeferredApprovalNotSupported() => ApprovalResources.Get(nameof (DeferredApprovalNotSupported));

    public static string DeferredApprovalNotSupported(CultureInfo culture) => ApprovalResources.Get(nameof (DeferredApprovalNotSupported), culture);

    public static string DeferredApprovalsMustHaveValidDeferredTo() => ApprovalResources.Get(nameof (DeferredApprovalsMustHaveValidDeferredTo));

    public static string DeferredApprovalsMustHaveValidDeferredTo(CultureInfo culture) => ApprovalResources.Get(nameof (DeferredApprovalsMustHaveValidDeferredTo), culture);
  }
}
