// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.IWorkItemTrackingProcessService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  [DefaultServiceImplementation(typeof (WorkItemTrackingProcessService))]
  public interface IWorkItemTrackingProcessService : IVssFrameworkService
  {
    bool TryGetAllowedValues(
      IVssRequestContext requestContext,
      Guid processId,
      int fieldId,
      IEnumerable<string> workItemTypeNames,
      out IReadOnlyCollection<string> allowedValues,
      bool bypassCustomProcessCache = true);

    void DeleteProcess(IVssRequestContext requestContext, Guid processId);

    bool IsInheritedStateCustomizationAllowed(IVssRequestContext requestContext, Guid processId);

    Guid CloneHostedXmlProcessToInherited(
      IVssRequestContext requestContext,
      Guid sourceTemplateId,
      string ProcessName,
      Guid TargetProcessType,
      string processDescription = null,
      Dictionary<string, Dictionary<string, string>> workItemTypeNameToStateNameToStateColors = null);

    ProcessDescriptor CreateInheritedProcess(
      IVssRequestContext requestContext,
      string name,
      string referenceName,
      string description,
      Guid parentProcessTypeId);

    IReadOnlyCollection<ProcessMigrationResult> MigrateProjectsProcess(
      IVssRequestContext requestContext,
      IEnumerable<ProcessMigrationModel> migratingProjects);

    ProjectProcessDescriptorMapping GetProjectProcessDescriptorMapping(
      IVssRequestContext requestContext,
      Guid projectId,
      bool expectUnmappedProject = false);

    IReadOnlyCollection<ProjectProcessDescriptorMapping> GetProjectProcessDescriptorMappings(
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectIds = null,
      bool expectUnmappedProjects = false,
      bool expectInvalidProcessIds = false,
      ProjectState projectStateFilter = ProjectState.WellFormed);

    ProcessDescriptor UpdateProcessNameAndDescription(
      IVssRequestContext requestContext,
      Guid processTypeId,
      string name,
      string description);

    void UpdateProjectProcess(
      IVssRequestContext requestContext,
      ProjectInfo project,
      ProcessDescriptor targetProcess);

    bool HasDeleteFieldPermission(IVssRequestContext requestContext);

    bool HasSetFieldLockPermission(IVssRequestContext requestContext);

    void EnableDisableProcess(
      IVssRequestContext requestContext,
      Guid processTypeId,
      bool isEnabled);

    ProcessDescriptor GetProjectProcessDescriptor(IVssRequestContext requestContext, Guid projectId);

    bool TryGetProjectProcessDescriptor(
      IVssRequestContext requestContext,
      Guid projectId,
      out ProcessDescriptor processDescriptor);

    bool TryGetLatestProjectProcessDescriptor(
      IVssRequestContext requestContext,
      Guid projectId,
      out ProcessDescriptor processDescriptor);

    void RestoreProcess(IVssRequestContext requestContext, Guid processTypeId);
  }
}
