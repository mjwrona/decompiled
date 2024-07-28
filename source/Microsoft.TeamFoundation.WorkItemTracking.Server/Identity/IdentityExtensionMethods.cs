// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Identity.IdentityExtensionMethods
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Identity
{
  internal static class IdentityExtensionMethods
  {
    internal static string GetLegacyDistinctDisplayName(this Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      string disambiguationPart = IdentityExtensionMethods.GetDisambiguationPart(identity);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} <{1}>", (object) identity.DisplayName, (object) disambiguationPart);
    }

    private static string GetDisambiguationPart(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      string property1 = identity.GetProperty<string>("Account", string.Empty);
      if ((identity.IsExternalUser || IdentityExtensionMethods.IsMsaDomain(identity)) && !identity.IsContainer)
        return property1;
      Guid property2 = identity.GetProperty<Guid>("LocalScopeId", Guid.Empty);
      string property3 = identity.GetProperty<string>("Domain", string.Empty);
      Guid empty = Guid.Empty;
      if (property2 != empty)
        return string.Format("{0}{1}", (object) "id:", (object) identity.Id);
      return string.IsNullOrEmpty(property3) ? property1 : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}", (object) property3, (object) property1);
    }

    private static bool IsMsaDomain(Microsoft.VisualStudio.Services.Identity.Identity identity) => string.Compare(identity.GetProperty<string>("Domain", string.Empty), "Windows Live ID", StringComparison.OrdinalIgnoreCase) == 0;
  }
}
