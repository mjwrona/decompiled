// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RlsManagementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class RlsManagementService : IRlsManagementService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckDeploymentRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void EnableRls(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties,
      RlsOptions options)
    {
      this.Validate(requestContext, properties);
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        throw new InvalidOperationException("We should not enable RLS on-prem.");
      using (RlsManagementComponent component = this.CreateComponent(properties))
        component.EnableRls(options);
      requestContext.GetService<TeamFoundationDatabaseManagementService>().UpdateDatabaseProperties(requestContext, properties.DatabaseId, (Action<TeamFoundationDatabaseProperties>) (editableProperties => editableProperties.Flags = editableProperties.Flags | TeamFoundationDatabaseFlags.RlsEnabled | (options.HasFlag((Enum) RlsOptions.Dataspace) ? TeamFoundationDatabaseFlags.DataspaceRlsEnabled : TeamFoundationDatabaseFlags.None)));
    }

    public void DisableRls(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties)
    {
      this.Validate(requestContext, properties);
      using (RlsManagementComponent component = this.CreateComponent(properties))
        component.DisableRls();
      requestContext.GetService<TeamFoundationDatabaseManagementService>().UpdateDatabaseProperties(requestContext, properties.DatabaseId, (Action<TeamFoundationDatabaseProperties>) (editableProperties => editableProperties.Flags = editableProperties.Flags & ~TeamFoundationDatabaseFlags.RlsEnabled & ~TeamFoundationDatabaseFlags.DataspaceRlsEnabled));
    }

    private void Validate(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ITeamFoundationDatabaseProperties>(properties, nameof (properties));
      requestContext.CheckDeploymentRequestContext();
    }

    public bool IsRlsSupported(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties)
    {
      this.Validate(requestContext, properties);
      using (RlsManagementComponent component = this.CreateComponent(properties))
        return component.IsRlsSupported();
    }

    public bool IsRlsEnabled(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties)
    {
      this.Validate(requestContext, properties);
      using (RlsManagementComponent component = this.CreateComponent(properties))
        return component.IsRlsEnabled();
    }

    private RlsManagementComponent CreateComponent(ITeamFoundationDatabaseProperties properties) => properties.GetDboConnectionInfo().CreateComponentRaw<RlsManagementComponent>();
  }
}
