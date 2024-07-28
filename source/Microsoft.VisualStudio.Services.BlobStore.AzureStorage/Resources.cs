// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources
// Assembly: Microsoft.VisualStudio.Services.BlobStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8BF1D977-E244-4825-BEA6-8BA4E1DDDB84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.AzureStorage.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.BlobStore.AzureStorage
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Get(resourceName, culture);
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

    public static string AzureBlobContainerCreationFailed(object arg0) => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Format(nameof (AzureBlobContainerCreationFailed), arg0);

    public static string AzureBlobContainerCreationFailed(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Format(nameof (AzureBlobContainerCreationFailed), culture, arg0);

    public static string BlobExistenceCheckFailed(object arg0) => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Format(nameof (BlobExistenceCheckFailed), arg0);

    public static string BlobExistenceCheckFailed(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Format(nameof (BlobExistenceCheckFailed), culture, arg0);

    public static string BlobExistenceCheckMatchingIdentifiersFailed() => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Get(nameof (BlobExistenceCheckMatchingIdentifiersFailed));

    public static string BlobExistenceCheckMatchingIdentifiersFailed(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Get(nameof (BlobExistenceCheckMatchingIdentifiersFailed), culture);

    public static string BlobIdentifierMustBeDefined() => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Get(nameof (BlobIdentifierMustBeDefined));

    public static string BlobIdentifierMustBeDefined(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Get(nameof (BlobIdentifierMustBeDefined), culture);

    public static string CannotMarkNewerKeepUntilInProduction() => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Get(nameof (CannotMarkNewerKeepUntilInProduction));

    public static string CannotMarkNewerKeepUntilInProduction(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Get(nameof (CannotMarkNewerKeepUntilInProduction), culture);

    public static string ContentLengthExceedsMaximumAllowable(object arg0) => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Format(nameof (ContentLengthExceedsMaximumAllowable), arg0);

    public static string ContentLengthExceedsMaximumAllowable(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Format(nameof (ContentLengthExceedsMaximumAllowable), culture, arg0);

    public static string DeleteBlobFailed(object arg0) => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Format(nameof (DeleteBlobFailed), arg0);

    public static string DeleteBlobFailed(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Format(nameof (DeleteBlobFailed), culture, arg0);

    public static string DeleteBlobMatchingIdentifiersFailed() => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Get(nameof (DeleteBlobMatchingIdentifiersFailed));

    public static string DeleteBlobMatchingIdentifiersFailed(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Get(nameof (DeleteBlobMatchingIdentifiersFailed), culture);

    public static string GetBlobFailed(object arg0) => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Format(nameof (GetBlobFailed), arg0);

    public static string GetBlobFailed(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Format(nameof (GetBlobFailed), culture, arg0);

    public static string PlusInHeaders() => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Get(nameof (PlusInHeaders));

    public static string PlusInHeaders(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Get(nameof (PlusInHeaders), culture);

    public static string PutBlobFailed(object arg0) => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Format(nameof (PutBlobFailed), arg0);

    public static string PutBlobFailed(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Format(nameof (PutBlobFailed), culture, arg0);

    public static string ResourceContentionTimeout(object arg0) => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Format(nameof (ResourceContentionTimeout), arg0);

    public static string ResourceContentionTimeout(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Format(nameof (ResourceContentionTimeout), culture, arg0);

    public static string UriGenerationTimeout() => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Get(nameof (UriGenerationTimeout));

    public static string UriGenerationTimeout(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.AzureStorage.Resources.Get(nameof (UriGenerationTimeout), culture);
  }
}
