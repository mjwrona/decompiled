// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.FileContainerResources
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.WebApi
{
  internal static class FileContainerResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (FileContainerResources), typeof (FileContainerResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => FileContainerResources.s_resMgr;

    private static string Get(string resourceName) => FileContainerResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? FileContainerResources.Get(resourceName) : FileContainerResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) FileContainerResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? FileContainerResources.GetInt(resourceName) : (int) FileContainerResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) FileContainerResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? FileContainerResources.GetBool(resourceName) : (bool) FileContainerResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => FileContainerResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = FileContainerResources.Get(resourceName, culture);
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

    public static string ArtifactUriNotSupportedException(object arg0) => FileContainerResources.Format(nameof (ArtifactUriNotSupportedException), arg0);

    public static string ArtifactUriNotSupportedException(object arg0, CultureInfo culture) => FileContainerResources.Format(nameof (ArtifactUriNotSupportedException), culture, arg0);

    public static string ContainerNotFoundException(object arg0) => FileContainerResources.Format(nameof (ContainerNotFoundException), arg0);

    public static string ContainerNotFoundException(object arg0, CultureInfo culture) => FileContainerResources.Format(nameof (ContainerNotFoundException), culture, arg0);

    public static string ContainerItemNotFoundException(object arg0, object arg1) => FileContainerResources.Format(nameof (ContainerItemNotFoundException), arg0, arg1);

    public static string ContainerItemNotFoundException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FileContainerResources.Format(nameof (ContainerItemNotFoundException), culture, arg0, arg1);
    }

    public static string ContainerItemWithDifferentTypeExists(object arg0, object arg1) => FileContainerResources.Format(nameof (ContainerItemWithDifferentTypeExists), arg0, arg1);

    public static string ContainerItemWithDifferentTypeExists(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FileContainerResources.Format(nameof (ContainerItemWithDifferentTypeExists), culture, arg0, arg1);
    }

    public static string PendingUploadNotFoundException(object arg0) => FileContainerResources.Format(nameof (PendingUploadNotFoundException), arg0);

    public static string PendingUploadNotFoundException(object arg0, CultureInfo culture) => FileContainerResources.Format(nameof (PendingUploadNotFoundException), culture, arg0);

    public static string ContainerItemDoesNotExist(object arg0, object arg1) => FileContainerResources.Format(nameof (ContainerItemDoesNotExist), arg0, arg1);

    public static string ContainerItemDoesNotExist(object arg0, object arg1, CultureInfo culture) => FileContainerResources.Format(nameof (ContainerItemDoesNotExist), culture, arg0, arg1);

    public static string ContainerItemCopySourcePendingUpload(object arg0) => FileContainerResources.Format(nameof (ContainerItemCopySourcePendingUpload), arg0);

    public static string ContainerItemCopySourcePendingUpload(object arg0, CultureInfo culture) => FileContainerResources.Format(nameof (ContainerItemCopySourcePendingUpload), culture, arg0);

    public static string ContainerItemCopyTargetChildOfSource(object arg0, object arg1) => FileContainerResources.Format(nameof (ContainerItemCopyTargetChildOfSource), arg0, arg1);

    public static string ContainerItemCopyTargetChildOfSource(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return FileContainerResources.Format(nameof (ContainerItemCopyTargetChildOfSource), culture, arg0, arg1);
    }

    public static string ContainerItemCopyDuplicateTargets(object arg0) => FileContainerResources.Format(nameof (ContainerItemCopyDuplicateTargets), arg0);

    public static string ContainerItemCopyDuplicateTargets(object arg0, CultureInfo culture) => FileContainerResources.Format(nameof (ContainerItemCopyDuplicateTargets), culture, arg0);

    public static string ContainerAlreadyExists(object arg0) => FileContainerResources.Format(nameof (ContainerAlreadyExists), arg0);

    public static string ContainerAlreadyExists(object arg0, CultureInfo culture) => FileContainerResources.Format(nameof (ContainerAlreadyExists), culture, arg0);

    public static string UnexpectedContentType(object arg0, object arg1) => FileContainerResources.Format(nameof (UnexpectedContentType), arg0, arg1);

    public static string UnexpectedContentType(object arg0, object arg1, CultureInfo culture) => FileContainerResources.Format(nameof (UnexpectedContentType), culture, arg0, arg1);

    public static string NoContentReturned() => FileContainerResources.Get(nameof (NoContentReturned));

    public static string NoContentReturned(CultureInfo culture) => FileContainerResources.Get(nameof (NoContentReturned), culture);

    public static string GzipNotSupportedOnServer() => FileContainerResources.Get(nameof (GzipNotSupportedOnServer));

    public static string GzipNotSupportedOnServer(CultureInfo culture) => FileContainerResources.Get(nameof (GzipNotSupportedOnServer), culture);

    public static string BadCompression() => FileContainerResources.Get(nameof (BadCompression));

    public static string BadCompression(CultureInfo culture) => FileContainerResources.Get(nameof (BadCompression), culture);

    public static string ChunksizeWrongWithContentId(object arg0) => FileContainerResources.Format(nameof (ChunksizeWrongWithContentId), arg0);

    public static string ChunksizeWrongWithContentId(object arg0, CultureInfo culture) => FileContainerResources.Format(nameof (ChunksizeWrongWithContentId), culture, arg0);

    public static string ContentIdCollision(object arg0, object arg1, object arg2, object arg3) => FileContainerResources.Format(nameof (ContentIdCollision), arg0, arg1, arg2, arg3);

    public static string ContentIdCollision(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return FileContainerResources.Format(nameof (ContentIdCollision), culture, arg0, arg1, arg2, arg3);
    }
  }
}
