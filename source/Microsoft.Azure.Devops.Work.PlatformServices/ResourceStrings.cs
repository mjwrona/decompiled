// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.PlatformServices.ResourceStrings
// Assembly: Microsoft.Azure.Devops.Work.PlatformServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7C8E511A-CB9A-4327-9803-A1164853E0F0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Work.PlatformServices.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.Azure.Devops.Work.PlatformServices
{
  internal static class ResourceStrings
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ResourceStrings), typeof (ResourceStrings).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ResourceStrings.s_resMgr;

    private static string Get(string resourceName) => ResourceStrings.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ResourceStrings.Get(resourceName) : ResourceStrings.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ResourceStrings.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ResourceStrings.GetInt(resourceName) : (int) ResourceStrings.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ResourceStrings.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ResourceStrings.GetBool(resourceName) : (bool) ResourceStrings.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ResourceStrings.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ResourceStrings.Get(resourceName, culture);
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

    public static string InvalidRelationType(object arg0) => ResourceStrings.Format(nameof (InvalidRelationType), arg0);

    public static string InvalidRelationType(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (InvalidRelationType), culture, arg0);

    public static string InvalidWiql() => ResourceStrings.Get(nameof (InvalidWiql));

    public static string InvalidWiql(CultureInfo culture) => ResourceStrings.Get(nameof (InvalidWiql), culture);

    public static string WorkItemPatchDocument_AmbiguousRelation(
      object arg0,
      object arg1,
      object arg2)
    {
      return ResourceStrings.Format(nameof (WorkItemPatchDocument_AmbiguousRelation), arg0, arg1, arg2);
    }

    public static string WorkItemPatchDocument_AmbiguousRelation(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (WorkItemPatchDocument_AmbiguousRelation), culture, arg0, arg1, arg2);
    }

    public static string WorkItemPatchDocument_CannotChangeRelationType() => ResourceStrings.Get(nameof (WorkItemPatchDocument_CannotChangeRelationType));

    public static string WorkItemPatchDocument_CannotChangeRelationType(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemPatchDocument_CannotChangeRelationType), culture);

    public static string WorkItemPatchDocument_DuplicateWorkItemId(object arg0) => ResourceStrings.Format(nameof (WorkItemPatchDocument_DuplicateWorkItemId), arg0);

    public static string WorkItemPatchDocument_DuplicateWorkItemId(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (WorkItemPatchDocument_DuplicateWorkItemId), culture, arg0);

    public static string WorkItemPatchDocument_IndexOutOfRange(object arg0) => ResourceStrings.Format(nameof (WorkItemPatchDocument_IndexOutOfRange), arg0);

    public static string WorkItemPatchDocument_IndexOutOfRange(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (WorkItemPatchDocument_IndexOutOfRange), culture, arg0);

    public static string WorkItemPatchDocument_InvalidPath(object arg0) => ResourceStrings.Format(nameof (WorkItemPatchDocument_InvalidPath), arg0);

    public static string WorkItemPatchDocument_InvalidPath(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (WorkItemPatchDocument_InvalidPath), culture, arg0);

    public static string WorkItemPatchDocument_InvalidRelationForAGivenWorkItem() => ResourceStrings.Get(nameof (WorkItemPatchDocument_InvalidRelationForAGivenWorkItem));

    public static string WorkItemPatchDocument_InvalidRelationForAGivenWorkItem(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemPatchDocument_InvalidRelationForAGivenWorkItem), culture);

    public static string WorkItemPatchDocument_InvalidRelationPath(object arg0, object arg1) => ResourceStrings.Format(nameof (WorkItemPatchDocument_InvalidRelationPath), arg0, arg1);

    public static string WorkItemPatchDocument_InvalidRelationPath(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (WorkItemPatchDocument_InvalidRelationPath), culture, arg0, arg1);
    }

    public static string WorkItemPatchDocument_OperationNotSuppported(object arg0) => ResourceStrings.Format(nameof (WorkItemPatchDocument_OperationNotSuppported), arg0);

    public static string WorkItemPatchDocument_OperationNotSuppported(
      object arg0,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (WorkItemPatchDocument_OperationNotSuppported), culture, arg0);
    }

    public static string WorkItemPatchDocument_TestFailed(object arg0, object arg1, object arg2) => ResourceStrings.Format(nameof (WorkItemPatchDocument_TestFailed), arg0, arg1, arg2);

    public static string WorkItemPatchDocument_TestFailed(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (WorkItemPatchDocument_TestFailed), culture, arg0, arg1, arg2);
    }

    public static string WorkItemPatchDocument_TestNotSupportedForRelations() => ResourceStrings.Get(nameof (WorkItemPatchDocument_TestNotSupportedForRelations));

    public static string WorkItemPatchDocument_TestNotSupportedForRelations(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemPatchDocument_TestNotSupportedForRelations), culture);

    public static string WorkItemPatchDoesNotSupportEmptyPath() => ResourceStrings.Get(nameof (WorkItemPatchDoesNotSupportEmptyPath));

    public static string WorkItemPatchDoesNotSupportEmptyPath(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemPatchDoesNotSupportEmptyPath), culture);

    public static string WorkItemPatchDoesNotSupportPatchingTopLevelProperties(object arg0) => ResourceStrings.Format(nameof (WorkItemPatchDoesNotSupportPatchingTopLevelProperties), arg0);

    public static string WorkItemPatchDoesNotSupportPatchingTopLevelProperties(
      object arg0,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (WorkItemPatchDoesNotSupportPatchingTopLevelProperties), culture, arg0);
    }

    public static string WorkItemPatchDoesNotSupportReplacingAttachedFile() => ResourceStrings.Get(nameof (WorkItemPatchDoesNotSupportReplacingAttachedFile));

    public static string WorkItemPatchDoesNotSupportReplacingAttachedFile(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemPatchDoesNotSupportReplacingAttachedFile), culture);
  }
}
