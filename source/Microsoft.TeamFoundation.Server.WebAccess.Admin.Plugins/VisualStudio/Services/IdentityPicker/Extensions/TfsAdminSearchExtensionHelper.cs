// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Extensions.TfsAdminSearchExtensionHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.IdentityPicker.Extensions
{
  internal sealed class TfsAdminSearchExtensionHelper
  {
    private const string NoServiceIdentities = "NoServiceIdentities";

    internal static bool IsAccountTenantBacked(IVssRequestContext requestContext) => requestContext != null && !requestContext.ExecutionEnvironment.IsOnPremisesDeployment && requestContext.IsOrganizationAadBacked();

    internal static void FilterSearchResponse(
      IVssRequestContext requestContext,
      IList<string> constraintList,
      SearchResponse response)
    {
      bool flag = false;
      if (constraintList == null || constraintList.Count == 0)
        return;
      if (constraintList.Contains("NoServiceIdentities"))
        flag = true;
      if (!flag)
        return;
      foreach (QueryTokenResult result1 in (IEnumerable<QueryTokenResult>) response.Results)
      {
        foreach (QueryTokenResult result2 in (IEnumerable<QueryTokenResult>) response.Results)
          result2.Identities = (IList<Microsoft.VisualStudio.Services.IdentityPicker.Identity>) result2.Identities.Where<Microsoft.VisualStudio.Services.IdentityPicker.Identity>((Func<Microsoft.VisualStudio.Services.IdentityPicker.Identity, bool>) (x => !TfsAdminSearchExtensionHelper.IsServiceIdentity(requestContext, x))).ToList<Microsoft.VisualStudio.Services.IdentityPicker.Identity>();
      }
    }

    private static bool IsServiceIdentity(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.IdentityPicker.Identity entity)
    {
      Guid result;
      if (!Guid.TryParse(!string.IsNullOrWhiteSpace(entity.LocalDirectory) ? entity.LocalId : entity.OriginId, out result) || result == Guid.Empty)
        return false;
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        result
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      return IdentityHelper.IsServiceIdentity(requestContext, (IReadOnlyVssIdentity) identity);
    }
  }
}
