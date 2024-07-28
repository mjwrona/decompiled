// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Navigation.HeaderActionHelpers
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;

namespace Microsoft.TeamFoundation.Server.WebAccess.Navigation
{
  public static class HeaderActionHelpers
  {
    public static bool HasClaim(IVssRequestContext requestContext, string requiredClaim) => requestContext.GetService<IContributionClaimService>().HasClaim(requestContext, requiredClaim);
  }
}
