// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.CodeReviewWebAPIResources
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  internal static class CodeReviewWebAPIResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (CodeReviewWebAPIResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => CodeReviewWebAPIResources.s_resMgr;

    private static string Get(string resourceName) => CodeReviewWebAPIResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? CodeReviewWebAPIResources.Get(resourceName) : CodeReviewWebAPIResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) CodeReviewWebAPIResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? CodeReviewWebAPIResources.GetInt(resourceName) : (int) CodeReviewWebAPIResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) CodeReviewWebAPIResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? CodeReviewWebAPIResources.GetBool(resourceName) : (bool) CodeReviewWebAPIResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => CodeReviewWebAPIResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = CodeReviewWebAPIResources.Get(resourceName, culture);
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

    public static string ApprovedReviewerState() => CodeReviewWebAPIResources.Get(nameof (ApprovedReviewerState));

    public static string ApprovedReviewerState(CultureInfo culture) => CodeReviewWebAPIResources.Get(nameof (ApprovedReviewerState), culture);

    public static string ApprovedWithCommentsReviewerState() => CodeReviewWebAPIResources.Get(nameof (ApprovedWithCommentsReviewerState));

    public static string ApprovedWithCommentsReviewerState(CultureInfo culture) => CodeReviewWebAPIResources.Get(nameof (ApprovedWithCommentsReviewerState), culture);

    public static string ChangeTrackingIdAlreadySetException(object arg0) => CodeReviewWebAPIResources.Format(nameof (ChangeTrackingIdAlreadySetException), arg0);

    public static string ChangeTrackingIdAlreadySetException(object arg0, CultureInfo culture) => CodeReviewWebAPIResources.Format(nameof (ChangeTrackingIdAlreadySetException), culture, arg0);

    public static string CodeNotReadyYetReviewerState() => CodeReviewWebAPIResources.Get(nameof (CodeNotReadyYetReviewerState));

    public static string CodeNotReadyYetReviewerState(CultureInfo culture) => CodeReviewWebAPIResources.Get(nameof (CodeNotReadyYetReviewerState), culture);

    public static string DeclinedReviewerState() => CodeReviewWebAPIResources.Get(nameof (DeclinedReviewerState));

    public static string DeclinedReviewerState(CultureInfo culture) => CodeReviewWebAPIResources.Get(nameof (DeclinedReviewerState), culture);

    public static string DownloadDirectoryNotFound(object arg0) => CodeReviewWebAPIResources.Format(nameof (DownloadDirectoryNotFound), arg0);

    public static string DownloadDirectoryNotFound(object arg0, CultureInfo culture) => CodeReviewWebAPIResources.Format(nameof (DownloadDirectoryNotFound), culture, arg0);

    public static string InvalidChangeTypeException(object arg0, object arg1) => CodeReviewWebAPIResources.Format(nameof (InvalidChangeTypeException), arg0, arg1);

    public static string InvalidChangeTypeException(object arg0, object arg1, CultureInfo culture) => CodeReviewWebAPIResources.Format(nameof (InvalidChangeTypeException), culture, arg0, arg1);

    public static string InvalidCodeReviewArtifactUri(object arg0) => CodeReviewWebAPIResources.Format(nameof (InvalidCodeReviewArtifactUri), arg0);

    public static string InvalidCodeReviewArtifactUri(object arg0, CultureInfo culture) => CodeReviewWebAPIResources.Format(nameof (InvalidCodeReviewArtifactUri), culture, arg0);

    public static string InvalidReviewerStateId(object arg0) => CodeReviewWebAPIResources.Format(nameof (InvalidReviewerStateId), arg0);

    public static string InvalidReviewerStateId(object arg0, CultureInfo culture) => CodeReviewWebAPIResources.Format(nameof (InvalidReviewerStateId), culture, arg0);

    public static string IterationListNotCompleteException() => CodeReviewWebAPIResources.Get(nameof (IterationListNotCompleteException));

    public static string IterationListNotCompleteException(CultureInfo culture) => CodeReviewWebAPIResources.Get(nameof (IterationListNotCompleteException), culture);

    public static string NoResponseReviewerState() => CodeReviewWebAPIResources.Get(nameof (NoResponseReviewerState));

    public static string NoResponseReviewerState(CultureInfo culture) => CodeReviewWebAPIResources.Get(nameof (NoResponseReviewerState), culture);

    public static string ProjectIdMustBeSpecified(object arg0) => CodeReviewWebAPIResources.Format(nameof (ProjectIdMustBeSpecified), arg0);

    public static string ProjectIdMustBeSpecified(object arg0, CultureInfo culture) => CodeReviewWebAPIResources.Format(nameof (ProjectIdMustBeSpecified), culture, arg0);

    public static string RejectedReviewerState() => CodeReviewWebAPIResources.Get(nameof (RejectedReviewerState));

    public static string RejectedReviewerState(CultureInfo culture) => CodeReviewWebAPIResources.Get(nameof (RejectedReviewerState), culture);
  }
}
