// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ITeamFoundationProcessService
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  [DefaultServiceImplementation(typeof (TeamFoundationProcessService))]
  public interface ITeamFoundationProcessService : IVssFrameworkService
  {
    ProcessDescriptor CreateOrUpdateLegacyProcess(
      IVssRequestContext requestContext,
      Stream zipContentStream);

    void UpdateProcessStatus(
      IVssRequestContext requestContext,
      Guid processTypeId,
      ProcessStatus processState);

    void UpdateProcessStatuses(
      IVssRequestContext requestContext,
      IEnumerable<Guid> typeIds,
      ProcessStatus processState);

    Guid GetSpecificProcessDescriptorIdByIntegerId(
      IVssRequestContext requestContext,
      int legacyProcessId);

    bool TryGetSpecificProcessDescriptorIdByIntegerId(
      IVssRequestContext requestContext,
      int legacyProcessId,
      out Guid processSpecificId);

    Guid GetSpecificProcessDescriptorIdByVersion(
      IVssRequestContext requestContext,
      Guid processTypeId,
      int majorVersion,
      int minorVersion);

    bool TryGetSpecificProcessDescriptorIdByVersion(
      IVssRequestContext requestContext,
      Guid processTypeId,
      int majorVersion,
      int minorVersion,
      out Guid processSpecificId);

    ProcessDescriptor GetSpecificProcessDescriptor(
      IVssRequestContext requestContext,
      Guid processSpecificId,
      bool fallThrough = true);

    bool TryGetSpecificProcessDescriptor(
      IVssRequestContext requestContext,
      Guid processSpecificId,
      out ProcessDescriptor descriptor,
      bool fallThrough = true);

    IReadOnlyCollection<ProcessDescriptor> GetProcessDescriptors(
      IVssRequestContext requestContext,
      bool fallThrough = true,
      bool bypassCache = false);

    Guid GetDefaultProcessTypeId(IVssRequestContext requestContext);

    ISet<Guid> GetDisabledProcessTypeIds(IVssRequestContext requestContext);

    IProcessTemplate GetLegacyProcess(
      IVssRequestContext requestContext,
      ProcessDescriptor descriptor);

    Stream GetLegacyProcessPackageContent(
      IVssRequestContext requestContext,
      ProcessDescriptor descriptor);

    Stream GetLegacyProcessPackageContent(IVssRequestContext requestContext, Guid processTypeId);

    Stream GetSpecificLegacyProcessPackageContent(
      IVssRequestContext requestContext,
      Guid processSpecificId);

    ProcessDescriptor GetProcessDescriptor(
      IVssRequestContext requestContext,
      Guid processTypeId,
      bool fallThrough = true,
      bool bypassCache = false);

    bool TryGetProcessDescriptor(
      IVssRequestContext requestContext,
      Guid processTypeId,
      out ProcessDescriptor descriptor,
      bool fallThrough = true,
      bool bypassCache = false);

    IReadOnlyCollection<ProjectProcessDescriptorMapping> GetProjectProcessDescriptorMappings(
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectIds = null,
      bool expectUnmappedProjects = false,
      bool expectInvalidProcessIds = false,
      ProjectState projectStateFilter = ProjectState.WellFormed);

    void SetProcessAsDefault(IVssRequestContext requestContext, Guid processTypeId);

    void DeleteProcess(IVssRequestContext requestContext, Guid processTypeId);

    void CheckProcessPermission(
      IVssRequestContext requestContext,
      ProcessDescriptor descriptor,
      int requestedPermission,
      bool checkDescriptorScope = true);

    bool HasProcessPermission(
      IVssRequestContext requestContext,
      int requestedPermission,
      ProcessDescriptor descriptor = null,
      bool checkDescriptorScope = true);

    bool HasRootCreatePermission(IVssRequestContext requestContext);

    void AddProcessPermission(
      IVssRequestContext requestContext,
      ProcessDescriptor descriptor,
      int requestedPermission = 15);

    IReadOnlyCollection<ProcessDescriptor> GetProcessDescriptorHistory(
      IVssRequestContext requestContext,
      Guid processTypeId);

    ProcessDescriptor UpdateProcessNameAndDescription(
      IVssRequestContext requestContext,
      Guid processTypeId,
      string name,
      string description);

    void UpdateProjectProcess(
      IVssRequestContext requestContext,
      ProjectInfo project,
      ProcessDescriptor targetProcess);

    bool IsProcessEnabled(IVssRequestContext requestContext);

    void CheckValidProcessName(string name);

    string GetSecurityToken(IVssRequestContext requestContext, ProcessDescriptor descriptor = null);
  }
}
