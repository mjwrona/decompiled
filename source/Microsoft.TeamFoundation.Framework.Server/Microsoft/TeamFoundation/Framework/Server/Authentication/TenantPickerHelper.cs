// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.TenantPickerHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Specialized;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  internal static class TenantPickerHelper
  {
    private const string HostTypeParam = "ht";

    public static bool HasTenantHint(IVssRequestContext requestContext)
    {
      if (!(requestContext is IWebRequestContextInternal))
        return false;
      return requestContext.RequestRestrictions().HasAnyLabel("SignedInPage") && TenantPickerHelper.HasTenantHint(AadAuthUrlUtility.ParseState(requestContext));
    }

    public static bool HasTenantHint(NameValueCollection stateParams)
    {
      string stateParam = stateParams["tenant"];
      return !string.IsNullOrWhiteSpace(stateParam) && !string.Equals(stateParam, "common", StringComparison.OrdinalIgnoreCase);
    }

    public static bool CheckSkipTenantPicker(IVssRequestContext requestContext)
    {
      NameValueCollection state = AadAuthUrlUtility.ParseState(requestContext);
      return HttpContext.Current.Request.Headers["X-TFS-FedAuthRedirect"] == "Suppress" || HttpContext.Current.Request.Path.EndsWith(".asmx", StringComparison.OrdinalIgnoreCase) || HttpContext.Current.Request.Path.EndsWith(".ashx", StringComparison.OrdinalIgnoreCase) || TenantPickerHelper.WasAtAccountLevel(state) || TenantPickerHelper.HasTenantHint(state);
    }

    public static bool WasAtAccountLevel(IVssRequestContext requestContext) => TenantPickerHelper.WasAtAccountLevel(AadAuthUrlUtility.ParseState(requestContext));

    public static bool WasAtAccountLevel(NameValueCollection stateParams)
    {
      string stateParam = stateParams["ht"];
      if (string.IsNullOrEmpty(stateParam))
        return false;
      int result = 1;
      int.TryParse(stateParam, out result);
      return (result & -3) == 0;
    }
  }
}
