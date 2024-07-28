// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DefaultRuntimeSqlManagementHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DefaultRuntimeSqlManagementHelper : IRuntimeSqlManagementHelper
  {
    public void AddDatabaseToFailoverGroupAndEnableAZ(
      IVssRequestContext requestContext,
      string dataSource,
      string databaseName,
      bool enableAz,
      ITFLogger logger)
    {
    }

    public void AddDatabaseToFailoverGroupAndEnableAZ(
      Guid subscriptionId,
      string resourceManagerAadTenantId,
      string resourceManagementUrl,
      string resourceGroupName,
      string dataSource,
      string databaseName,
      ITFLogger logger,
      bool enableDatabaseFailoverGroup = true)
    {
    }

    public void DropUserDatabasesFromPartnerServer(
      Guid subscriptionId,
      string resourceManagerAadTenantId,
      string resourceManagementUrl,
      string resourceGroupName,
      string dataSource,
      ITFLogger logger)
    {
    }

    public void SetDatabaseZoneRedundancy(
      IVssRequestContext requestContext,
      string serverName,
      string databaseName,
      bool isDatabaseVcore,
      bool enable,
      ITFLogger logger)
    {
    }

    public bool GetDatabaseZoneRedundancy(
      IVssRequestContext requestContext,
      string serverName,
      string databaseName,
      ITFLogger logger)
    {
      return false;
    }

    public void UpgradeToHyperscale(
      IVssRequestContext deploymentRequestContext,
      ITeamFoundationDatabaseProperties databaseProperties,
      string currentServiceObjective,
      ITFLogger logger)
    {
    }
  }
}
