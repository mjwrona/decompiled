// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.TeamFoundationRequestContextExtensionMethods
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal static class TeamFoundationRequestContextExtensionMethods
  {
    internal static bool IsClientOm(this IVssRequestContext requestContext)
    {
      bool flag = false;
      object obj;
      if (requestContext.Items.TryGetValue(nameof (IsClientOm), out obj))
        flag = (bool) obj;
      return flag;
    }

    internal static IdentityDisplayType GetIdentityDisplayType(
      this IVssRequestContext requestContext)
    {
      int num = requestContext.IsClientOm() ? 1 : 0;
      bool flag = false;
      object obj;
      if (requestContext.Items.TryGetValue("UseLegacyIdentityString", out obj))
        flag = (bool) obj;
      if (num == 0)
        return IdentityDisplayType.ComboDisplayName;
      return !flag ? IdentityDisplayType.ComboDisplayNameWhenNeeded : IdentityDisplayType.DisplayName;
    }
  }
}
