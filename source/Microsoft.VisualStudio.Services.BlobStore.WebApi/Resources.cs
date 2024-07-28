// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Get(resourceName, culture);
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

    public static string BlobNotFoundException(object arg0) => Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Format(nameof (BlobNotFoundException), arg0);

    public static string BlobNotFoundException(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Format(nameof (BlobNotFoundException), culture, arg0);

    public static string ClientToolNoMatchingReleaseFound() => Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Get(nameof (ClientToolNoMatchingReleaseFound));

    public static string ClientToolNoMatchingReleaseFound(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Get(nameof (ClientToolNoMatchingReleaseFound), culture);

    public static string ClientToolNotFound(object arg0, object arg1, object arg2) => Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Format(nameof (ClientToolNotFound), arg0, arg1, arg2);

    public static string ClientToolNotFound(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Format(nameof (ClientToolNotFound), culture, arg0, arg1, arg2);
    }

    public static string DedupInconsistentAttributeException(object arg0) => Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Format(nameof (DedupInconsistentAttributeException), arg0);

    public static string DedupInconsistentAttributeException(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Format(nameof (DedupInconsistentAttributeException), culture, arg0);

    public static string DedupNotFoundException(object arg0) => Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Format(nameof (DedupNotFoundException), arg0);

    public static string DedupNotFoundException(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Format(nameof (DedupNotFoundException), culture, arg0);

    public static string EmptyDirectoryNotSupported() => Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Get(nameof (EmptyDirectoryNotSupported));

    public static string EmptyDirectoryNotSupported(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Get(nameof (EmptyDirectoryNotSupported), culture);

    public static string Error_UnknownDatabaseErrorOcurred(object arg0) => Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Format(nameof (Error_UnknownDatabaseErrorOcurred), arg0);

    public static string Error_UnknownDatabaseErrorOcurred(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Format(nameof (Error_UnknownDatabaseErrorOcurred), culture, arg0);

    public static string InvalidPath() => Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Get(nameof (InvalidPath));

    public static string InvalidPath(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Get(nameof (InvalidPath), culture);

    public static string RemainingBytesError() => Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Get(nameof (RemainingBytesError));

    public static string RemainingBytesError(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Get(nameof (RemainingBytesError), culture);

    public static string UnsupportedRuntime(object arg0) => Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Format(nameof (UnsupportedRuntime), arg0);

    public static string UnsupportedRuntime(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Format(nameof (UnsupportedRuntime), culture, arg0);

    public static string UploadFailed() => Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Get(nameof (UploadFailed));

    public static string UploadFailed(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.WebApi.Resources.Get(nameof (UploadFailed), culture);
  }
}
