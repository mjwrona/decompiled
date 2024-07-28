// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.DiscussionWebAPIResources
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 443E2621-CB19-4319-96B1-AE621A0F5B5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi
{
  internal static class DiscussionWebAPIResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (DiscussionWebAPIResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => DiscussionWebAPIResources.s_resMgr;

    private static string Get(string resourceName) => DiscussionWebAPIResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? DiscussionWebAPIResources.Get(resourceName) : DiscussionWebAPIResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) DiscussionWebAPIResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? DiscussionWebAPIResources.GetInt(resourceName) : (int) DiscussionWebAPIResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) DiscussionWebAPIResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? DiscussionWebAPIResources.GetBool(resourceName) : (bool) DiscussionWebAPIResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => DiscussionWebAPIResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = DiscussionWebAPIResources.Get(resourceName, culture);
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

    public static string CommentAuthorCannotbeNull(object arg0) => DiscussionWebAPIResources.Format(nameof (CommentAuthorCannotbeNull), arg0);

    public static string CommentAuthorCannotbeNull(object arg0, CultureInfo culture) => DiscussionWebAPIResources.Format(nameof (CommentAuthorCannotbeNull), culture, arg0);
  }
}
