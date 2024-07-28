// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.GraphResources
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.WebApi
{
  internal static class GraphResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (GraphResources), typeof (GraphResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => GraphResources.s_resMgr;

    private static string Get(string resourceName) => GraphResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? GraphResources.Get(resourceName) : GraphResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) GraphResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? GraphResources.GetInt(resourceName) : (int) GraphResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) GraphResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? GraphResources.GetBool(resourceName) : (bool) GraphResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => GraphResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = GraphResources.Get(resourceName, culture);
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

    public static string CannotEditChildrenOfNonGroup(object arg0) => GraphResources.Format(nameof (CannotEditChildrenOfNonGroup), arg0);

    public static string CannotEditChildrenOfNonGroup(object arg0, CultureInfo culture) => GraphResources.Format(nameof (CannotEditChildrenOfNonGroup), culture, arg0);

    public static string EmptySubjectDescriptorNotAllowed(object arg0) => GraphResources.Format(nameof (EmptySubjectDescriptorNotAllowed), arg0);

    public static string EmptySubjectDescriptorNotAllowed(object arg0, CultureInfo culture) => GraphResources.Format(nameof (EmptySubjectDescriptorNotAllowed), culture, arg0);

    public static string WellKnownSidNotAllowed(object arg0) => GraphResources.Format(nameof (WellKnownSidNotAllowed), arg0);

    public static string WellKnownSidNotAllowed(object arg0, CultureInfo culture) => GraphResources.Format(nameof (WellKnownSidNotAllowed), culture, arg0);

    public static string GraphMembershipNotFound(object arg0, object arg1) => GraphResources.Format(nameof (GraphMembershipNotFound), arg0, arg1);

    public static string GraphMembershipNotFound(object arg0, object arg1, CultureInfo culture) => GraphResources.Format(nameof (GraphMembershipNotFound), culture, arg0, arg1);

    public static string GraphSubjectNotFound(object arg0) => GraphResources.Format(nameof (GraphSubjectNotFound), arg0);

    public static string GraphSubjectNotFound(object arg0, CultureInfo culture) => GraphResources.Format(nameof (GraphSubjectNotFound), culture, arg0);

    public static string InvalidGraphLegacyDescriptor(object arg0) => GraphResources.Format(nameof (InvalidGraphLegacyDescriptor), arg0);

    public static string InvalidGraphLegacyDescriptor(object arg0, CultureInfo culture) => GraphResources.Format(nameof (InvalidGraphLegacyDescriptor), culture, arg0);

    public static string InvalidGraphMemberCuid(object arg0) => GraphResources.Format(nameof (InvalidGraphMemberCuid), arg0);

    public static string InvalidGraphMemberCuid(object arg0, CultureInfo culture) => GraphResources.Format(nameof (InvalidGraphMemberCuid), culture, arg0);

    public static string InvalidGraphMemberStorageKey(object arg0) => GraphResources.Format(nameof (InvalidGraphMemberStorageKey), arg0);

    public static string InvalidGraphMemberStorageKey(object arg0, CultureInfo culture) => GraphResources.Format(nameof (InvalidGraphMemberStorageKey), culture, arg0);

    public static string InvalidGraphSubjectDescriptor(object arg0) => GraphResources.Format(nameof (InvalidGraphSubjectDescriptor), arg0);

    public static string InvalidGraphSubjectDescriptor(object arg0, CultureInfo culture) => GraphResources.Format(nameof (InvalidGraphSubjectDescriptor), culture, arg0);

    public static string StorageKeyNotFound(object arg0) => GraphResources.Format(nameof (StorageKeyNotFound), arg0);

    public static string StorageKeyNotFound(object arg0, CultureInfo culture) => GraphResources.Format(nameof (StorageKeyNotFound), culture, arg0);

    public static string SubjectDescriptorNotFoundWithIdentityDescriptor(object arg0) => GraphResources.Format(nameof (SubjectDescriptorNotFoundWithIdentityDescriptor), arg0);

    public static string SubjectDescriptorNotFoundWithIdentityDescriptor(
      object arg0,
      CultureInfo culture)
    {
      return GraphResources.Format(nameof (SubjectDescriptorNotFoundWithIdentityDescriptor), culture, arg0);
    }

    public static string SubjectDescriptorNotFoundWithStorageKey(object arg0) => GraphResources.Format(nameof (SubjectDescriptorNotFoundWithStorageKey), arg0);

    public static string SubjectDescriptorNotFoundWithStorageKey(object arg0, CultureInfo culture) => GraphResources.Format(nameof (SubjectDescriptorNotFoundWithStorageKey), culture, arg0);

    public static string IdentifierLengthOutOfRange() => GraphResources.Get(nameof (IdentifierLengthOutOfRange));

    public static string IdentifierLengthOutOfRange(CultureInfo culture) => GraphResources.Get(nameof (IdentifierLengthOutOfRange), culture);

    public static string SubjectTypeLengthOutOfRange() => GraphResources.Get(nameof (SubjectTypeLengthOutOfRange));

    public static string SubjectTypeLengthOutOfRange(CultureInfo culture) => GraphResources.Get(nameof (SubjectTypeLengthOutOfRange), culture);
  }
}
