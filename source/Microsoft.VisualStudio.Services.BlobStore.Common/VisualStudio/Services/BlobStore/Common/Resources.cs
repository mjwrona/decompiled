// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.Resources
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.VisualStudio.Services.BlobStore.Common.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(resourceName) : Microsoft.VisualStudio.Services.BlobStore.Common.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.VisualStudio.Services.BlobStore.Common.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.BlobStore.Common.Resources.GetInt(resourceName) : (int) Microsoft.VisualStudio.Services.BlobStore.Common.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.VisualStudio.Services.BlobStore.Common.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.VisualStudio.Services.BlobStore.Common.Resources.GetBool(resourceName) : (bool) Microsoft.VisualStudio.Services.BlobStore.Common.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(resourceName, culture);
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

    public static string InvalidProjectDomainInputError() => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (InvalidProjectDomainInputError));

    public static string InvalidProjectDomainInputError(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (InvalidProjectDomainInputError), culture);

    public static string DomainIdLengthError() => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (DomainIdLengthError));

    public static string DomainIdLengthError(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (DomainIdLengthError), culture);

    public static string DomainIdNullError() => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (DomainIdNullError));

    public static string DomainIdNullError(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (DomainIdNullError), culture);

    public static string DomainIdParseError() => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (DomainIdParseError));

    public static string DomainIdParseError(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (DomainIdParseError), culture);

    public static string InvalidContentHashValue(object arg0) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Format(nameof (InvalidContentHashValue), arg0);

    public static string InvalidContentHashValue(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Format(nameof (InvalidContentHashValue), culture, arg0);

    public static string InvalidFinalBlockContentLength() => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (InvalidFinalBlockContentLength));

    public static string InvalidFinalBlockContentLength(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (InvalidFinalBlockContentLength), culture);

    public static string InvalidHashLength(object arg0) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Format(nameof (InvalidHashLength), arg0);

    public static string InvalidHashLength(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Format(nameof (InvalidHashLength), culture, arg0);

    public static string InvalidPartialBlock() => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (InvalidPartialBlock));

    public static string InvalidPartialBlock(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (InvalidPartialBlock), culture);

    public static string InvalidPartialContentBlockLength() => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (InvalidPartialContentBlockLength));

    public static string InvalidPartialContentBlockLength(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (InvalidPartialContentBlockLength), culture);

    public static string NoChunksFound() => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (NoChunksFound));

    public static string NoChunksFound(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (NoChunksFound), culture);

    public static string SymLinkExceptionMessage() => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (SymLinkExceptionMessage));

    public static string SymLinkExceptionMessage(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (SymLinkExceptionMessage), culture);

    public static string ProjectDomainProjectIsInvalid(object arg0) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Format(nameof (ProjectDomainProjectIsInvalid), arg0);

    public static string ProjectDomainProjectIsInvalid(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Format(nameof (ProjectDomainProjectIsInvalid), culture, arg0);

    public static string EmptyPartitionKey() => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (EmptyPartitionKey));

    public static string EmptyPartitionKey(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (EmptyPartitionKey), culture);

    public static string InvalidPartitionKey() => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (InvalidPartitionKey));

    public static string InvalidPartitionKey(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (InvalidPartitionKey), culture);

    public static string UnknownDomainIdError(object arg0) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Format(nameof (UnknownDomainIdError), arg0);

    public static string UnknownDomainIdError(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Format(nameof (UnknownDomainIdError), culture, arg0);

    public static string InvalidPhysicalDomainIdError(object arg0) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Format(nameof (InvalidPhysicalDomainIdError), arg0);

    public static string InvalidPhysicalDomainIdError(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Format(nameof (InvalidPhysicalDomainIdError), culture, arg0);

    public static string InvalidProjectDomainIdFormatError(object arg0) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Format(nameof (InvalidProjectDomainIdFormatError), arg0);

    public static string InvalidProjectDomainIdFormatError(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Format(nameof (InvalidProjectDomainIdFormatError), culture, arg0);

    public static string InvalidProjectIdError(object arg0) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Format(nameof (InvalidProjectIdError), arg0);

    public static string InvalidProjectIdError(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Format(nameof (InvalidProjectIdError), culture, arg0);

    public static string MismatchedChunkedStatusToHashTypeException(object arg0, object arg1) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Format(nameof (MismatchedChunkedStatusToHashTypeException), arg0, arg1);

    public static string MismatchedChunkedStatusToHashTypeException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Format(nameof (MismatchedChunkedStatusToHashTypeException), culture, arg0, arg1);
    }

    public static string VsoHashAlgorithmNotFinalized() => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (VsoHashAlgorithmNotFinalized));

    public static string VsoHashAlgorithmNotFinalized(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (VsoHashAlgorithmNotFinalized), culture);

    public static string InvalidShardSetIdFormatError(object arg0) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Format(nameof (InvalidShardSetIdFormatError), arg0);

    public static string InvalidShardSetIdFormatError(object arg0, CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Format(nameof (InvalidShardSetIdFormatError), culture, arg0);

    public static string EmptyGuidShardSetIdError() => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (EmptyGuidShardSetIdError));

    public static string EmptyGuidShardSetIdError(CultureInfo culture) => Microsoft.VisualStudio.Services.BlobStore.Common.Resources.Get(nameof (EmptyGuidShardSetIdError), culture);
  }
}
