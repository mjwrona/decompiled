// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ReleaseDefinitionEnvironmentsService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Required use of types")]
  public class ReleaseDefinitionEnvironmentsService : ReleaseManagement2ServiceBase
  {
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Defaults are required.")]
    public IEnumerable<DefinitionEnvironmentReference> GetReleaseDefinitionEnvironments(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? taskGroupId = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string propertyFilters = null)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      IEnumerable<string> strings = ReleaseManagementArtifactPropertyKinds.AsPropertyFilters(propertyFilters);
      if ((strings == null || !strings.Any<string>()) && (!taskGroupId.HasValue || taskGroupId.Value == Guid.Empty))
        throw new InvalidRequestException(Resources.TaskGroupIdAndPropertyFilterCannotBeNullOrEmpty);
      List<int> definitionEnvironmentIds = new List<int>();
      if (strings != null && strings.Any<string>())
      {
        using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "ReleasesService.GetReleaseDefinitionEnvironments", 1976396))
        {
          using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, ReleaseManagementArtifactPropertyKinds.DefinitionEnvironment, strings, new Guid?(projectId)))
          {
            foreach (ArtifactPropertyValue current in properties.CurrentEnumerable<ArtifactPropertyValue>())
            {
              if (current != null && current.Spec != null && current.Spec.Id != null && current.Spec.Id.Length >= 4)
                definitionEnvironmentIds.Add(ReleaseManagementArtifactPropertyKinds.ToInt32(current.Spec.Id));
            }
          }
        }
      }
      using (ReleaseManagementTimer.Create(requestContext, "Service", "ReleaseDefinitionEnvironmentsService.GetReleaseDefinitionEnvironments", 1961226))
      {
        Func<ReleaseDefinitionEnvironmentsSqlComponent, IEnumerable<DefinitionEnvironmentReference>> action = (Func<ReleaseDefinitionEnvironmentsSqlComponent, IEnumerable<DefinitionEnvironmentReference>>) (component => component.GetReleaseDefinitionEnvironments(projectId, taskGroupId, (IEnumerable<int>) definitionEnvironmentIds));
        return requestContext.ExecuteWithinUsingWithComponent<ReleaseDefinitionEnvironmentsSqlComponent, IEnumerable<DefinitionEnvironmentReference>>(action);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf sdk needs it to be non-static")]
    public IEnumerable<PropertyValue> GetProperties(
      IVssRequestContext context,
      Guid projectId,
      int definitionEnvironmentId)
    {
      ArtifactSpec artifactSpec = new ArtifactSpec(ReleaseManagementArtifactPropertyKinds.DefinitionEnvironment, definitionEnvironmentId, 0, projectId);
      using (TeamFoundationDataReader properties = context.GetService<ITeamFoundationPropertyService>().GetProperties(context, artifactSpec, (IEnumerable<string>) null))
      {
        if (properties != null)
        {
          ArtifactPropertyValue artifactPropertyValue = properties.CurrentEnumerable<ArtifactPropertyValue>().FirstOrDefault<ArtifactPropertyValue>();
          if (artifactPropertyValue != null)
            return (IEnumerable<PropertyValue>) artifactPropertyValue.PropertyValues.ToList<PropertyValue>();
        }
      }
      return (IEnumerable<PropertyValue>) new List<PropertyValue>();
    }

    public XDocument GetBadgeForEnvironment(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      int environmentId,
      string branchName = null)
    {
      List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> list = requestContext.GetService<DeploymentsService>().ListDeployments(requestContext, projectId, definitionId, environmentId, 0, (string) null, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus.Undefined, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus.Succeeded | Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus.PartiallySucceeded | Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus.Failed, false, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder.IdDescending, 1, 0, new DateTime?(), new DateTime?(), createdFor: (string) null, branchName: branchName).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>();
      return !list.IsNullOrEmpty<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>() ? DeploymentBadgeHelper.GetSVGForEnvironment(requestContext, list[0]) : DeploymentBadgeHelper.GetSVGForEnvironment(requestContext, (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment) null);
    }
  }
}
