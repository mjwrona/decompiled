// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.Internal.DiffResources
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.VersionControl.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class DiffResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (DiffResources), typeof (DiffResources).GetTypeInfo().Assembly);
    public const string VssDiffChange = "VssDiffChange";
    public const string VssDiffChangeTo = "VssDiffChangeTo";
    public const string VssDiffInsert = "VssDiffInsert";
    public const string VssDiffDelete = "VssDiffDelete";
    public const string UnexpectedEndOfStream = "UnexpectedEndOfStream";
    public const string IncompleteCharacter = "IncompleteCharacter";
    public const string IncompleteSurrogatePair = "IncompleteSurrogatePair";
    public const string ReversedSurrogatePair = "ReversedSurrogatePair";
    public const string InvalidCodePage = "InvalidCodePage";
    public const string InternalCodeError = "InternalCodeError";
    public const string ArgOutOfRange = "ArgOutOfRange";
    public const string ArgumentError_TokenizerTypesMustMatch = "ArgumentError_TokenizerTypesMustMatch";
    public const string TokenizersMustBeInitialized = "TokenizersMustBeInitialized";
    public const string ArgumentError_TokenizerCodePagesMustMatch = "ArgumentError_TokenizerCodePagesMustMatch";
    public const string ArgumentError_TokenStreamsMustSeek = "ArgumentError_TokenStreamsMustSeek";
    public const string InvalidDiffOperation = "InvalidDiffOperation";
    public const string WriteOnceProperty = "WriteOnceProperty";
    public const string CouldNotRereadEntireToken = "CouldNotRereadEntireToken";

    public static ResourceManager Manager => DiffResources.s_resMgr;

    public static string Get(string resourceName) => DiffResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    public static string Get(string resourceName, CultureInfo culture) => culture == null ? DiffResources.Get(resourceName) : DiffResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) DiffResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? DiffResources.GetInt(resourceName) : (int) DiffResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) DiffResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? DiffResources.GetBool(resourceName) : (bool) DiffResources.s_resMgr.GetObject(resourceName, culture);

    public static string Format(string resourceName, params object[] args) => DiffResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    public static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = DiffResources.Get(resourceName, culture);
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
  }
}
