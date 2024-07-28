// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IRlsManagementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (RlsManagementService))]
  internal interface IRlsManagementService : IVssFrameworkService
  {
    bool IsRlsSupported(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties);

    bool IsRlsEnabled(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties);

    void EnableRls(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties,
      RlsOptions options);

    void DisableRls(IVssRequestContext requestContext, ITeamFoundationDatabaseProperties properties);
  }
}
