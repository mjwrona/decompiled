// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.WebServiceHelpers
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  internal static class WebServiceHelpers
  {
    public static void CheckWITProvisionPermission(
      IVssRequestContext requestContext,
      string projectUri)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectUri, nameof (projectUri));
      string toolSpecificId = LinkingUtilities.DecodeUri(projectUri).ToolSpecificId;
      requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, WitProvisionSecurity.NamespaceId).CheckPermission(requestContext, "$/" + toolSpecificId, 1, false);
    }
  }
}
