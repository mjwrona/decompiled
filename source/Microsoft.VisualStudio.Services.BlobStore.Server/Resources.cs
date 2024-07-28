// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Resources
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.BlobStore.Server.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.BlobStore.Server.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.BlobStore.Server.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.BlobStore.Server.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.BlobStore.Server.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.BlobStore.Server.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.BlobStore.Server.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.BlobStore.Server.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Get(resourceName, culture);
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

    public static string ContentLengthTooLarge(object arg0) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Format(nameof (ContentLengthTooLarge), arg0);

    public static string ContentLengthTooLarge(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Format(nameof (ContentLengthTooLarge), culture, arg0);

    public static string ContentLengthUnavailable() => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Get(nameof (ContentLengthUnavailable));

    public static string ContentLengthUnavailable(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Get(nameof (ContentLengthUnavailable), culture);

    public static string DirectAccessForbidden() => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Get(nameof (DirectAccessForbidden));

    public static string DirectAccessForbidden(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Get(nameof (DirectAccessForbidden), culture);

    public static string FileLevelBlobIdRequired() => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Get(nameof (FileLevelBlobIdRequired));

    public static string FileLevelBlobIdRequired(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Get(nameof (FileLevelBlobIdRequired), culture);

    public static string IncorrectRequestBoby() => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Get(nameof (IncorrectRequestBoby));

    public static string IncorrectRequestBoby(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Get(nameof (IncorrectRequestBoby), culture);

    public static string ReferenceContentMissing() => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Get(nameof (ReferenceContentMissing));

    public static string ReferenceContentMissing(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Get(nameof (ReferenceContentMissing), culture);

    public static string SessionAccessForbidden() => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Get(nameof (SessionAccessForbidden));

    public static string SessionAccessForbidden(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Get(nameof (SessionAccessForbidden), culture);

    public static string InvalidProjectIdError(object arg0) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Format(nameof (InvalidProjectIdError), arg0);

    public static string InvalidProjectIdError(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Format(nameof (InvalidProjectIdError), culture, arg0);

    public static string ProjectDomainIdAlreadyExistsError(object arg0, object arg1) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Format(nameof (ProjectDomainIdAlreadyExistsError), arg0, arg1);

    public static string ProjectDomainIdAlreadyExistsError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Format(nameof (ProjectDomainIdAlreadyExistsError), culture, arg0, arg1);
    }

    public static string EnableFeatureFlagError(object arg0) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Format(nameof (EnableFeatureFlagError), arg0);

    public static string EnableFeatureFlagError(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Format(nameof (EnableFeatureFlagError), culture, arg0);

    public static string DomainDoesNotExistsError(object arg0) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Format(nameof (DomainDoesNotExistsError), arg0);

    public static string DomainDoesNotExistsError(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Format(nameof (DomainDoesNotExistsError), culture, arg0);

    public static string InvalidDomainIdError(object arg0) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Format(nameof (InvalidDomainIdError), arg0);

    public static string InvalidDomainIdError(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Format(nameof (InvalidDomainIdError), culture, arg0);

    public static string DuplicateDomainIdError(object arg0) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Format(nameof (DuplicateDomainIdError), arg0);

    public static string DuplicateDomainIdError(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Format(nameof (DuplicateDomainIdError), culture, arg0);

    public static string InvalidProjectDomainIdError(object arg0) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Format(nameof (InvalidProjectDomainIdError), arg0);

    public static string InvalidProjectDomainIdError(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Format(nameof (InvalidProjectDomainIdError), culture, arg0);

    public static string CreateProjectDomainError(object arg0, object arg1) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Format(nameof (CreateProjectDomainError), arg0, arg1);

    public static string CreateProjectDomainError(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Format(nameof (CreateProjectDomainError), culture, arg0, arg1);

    public static string ContentLengthMismatchError(object arg0, object arg1) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Format(nameof (ContentLengthMismatchError), arg0, arg1);

    public static string ContentLengthMismatchError(object arg0, object arg1, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Server.Resources.Format(nameof (ContentLengthMismatchError), culture, arg0, arg1);
  }
}
