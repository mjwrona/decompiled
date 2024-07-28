// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.ICheckConfigurationService
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Checks.Server
{
  [DefaultServiceImplementation(typeof (CheckConfigurationService))]
  public interface ICheckConfigurationService : IVssFrameworkService
  {
    CheckConfiguration AddCheckConfiguration(
      IVssRequestContext requestContext,
      Guid projectId,
      CheckConfiguration checkConfiguration);

    IList<CheckConfiguration> AddCheckConfigurations(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<CheckConfiguration> checkConfigurations);

    CheckConfiguration GetCheckConfiguration(
      IVssRequestContext requestContext,
      Guid projectId,
      int id,
      CheckConfigurationExpandParameter expand = CheckConfigurationExpandParameter.None);

    List<CheckConfiguration> GetCheckConfigurationsOnResource(
      IVssRequestContext requestContext,
      Guid projectId,
      Resource resource,
      bool includeDisabledChecks,
      CheckConfigurationExpandParameter expand = CheckConfigurationExpandParameter.None);

    List<CheckConfiguration> GetCheckConfigurationsOnResources(
      IVssRequestContext requestContext,
      Guid projectId,
      List<Resource> resources,
      bool includeDisabledChecks,
      CheckConfigurationExpandParameter expand = CheckConfigurationExpandParameter.None,
      bool includeDeletedChecks = false);

    List<CheckConfiguration> GetCheckConfigurationsByIdVersion(
      IVssRequestContext requestContext,
      Guid projectId,
      List<CheckConfigurationRef> checkParams,
      CheckConfigurationExpandParameter expand = CheckConfigurationExpandParameter.None,
      bool includeDeletedChecks = false);

    CheckConfiguration UpdateCheckConfiguration(
      IVssRequestContext requestContext,
      Guid projectId,
      int assignmentId,
      CheckConfiguration checkConfiguration);

    void DeleteCheckConfiguration(IVssRequestContext requestContext, Guid projectId, int id);
  }
}
