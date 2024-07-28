// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OrgAccessConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class OrgAccessConstants
  {
    internal static readonly Guid OrgUserSubjectId = new Guid("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC");
    internal static readonly SecuritySubjectEntry OrgUserSubject = new SecuritySubjectEntry(OrgAccessConstants.OrgUserSubjectId, SecuritySubjectType.WellKnownGroup, OrgAccessConstants.OrgUserSubjectId.ToString(), "Organization User Subject");
    public const string UseTenantAsBoundaryForOidProjectVisibilityFF = "Microsoft.VisualStudio.Services.Identity.UseTenantAsBoundaryForOidProjectVisibility";
    public const string MaterializeEnterpriseUserAADGroupInAdoFF = "Microsoft.VisualStudio.Services.Identity.MaterializeEnterpriseUserAADGroupInADO";
  }
}
