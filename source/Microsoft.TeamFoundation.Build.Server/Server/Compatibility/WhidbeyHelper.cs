// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.WhidbeyHelper
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server.DataAccess;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  internal static class WhidbeyHelper
  {
    internal static string AddBuild(
      IVssRequestContext requestContext,
      string teamProject,
      BuildData data)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (AddBuild));
      if (!TFStringComparer.TeamProjectName.Equals(teamProject, data.TeamProject))
        throw new ArgumentException(ResourceStrings.InvalidTeamProject());
      if (BuildCommonUtil.IsDefaultDateTime(data.StartTime))
      {
        data.StartTime = DateTime.Now;
        requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Build start time set '{0}'", (object) data.StartTime);
      }
      TeamFoundationIdentity requestedBy = requestContext.GetService<TeamFoundationIdentityService>().ReadRequestIdentity(requestContext);
      TeamFoundationIdentity requestedFor;
      if (string.IsNullOrEmpty(data.RequestedBy))
      {
        requestedFor = requestedBy;
        requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Set requested for identity as requested by identity '{0}'", (object) requestedFor);
      }
      else
      {
        requestedFor = Microsoft.TeamFoundation.Build.Server.Validation.ResolveIdentity(requestContext, data.RequestedBy);
        requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Set requested for identity '{0}'", (object) requestedFor);
        if (requestedFor == null)
        {
          requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Identity not found for '{0}'", (object) data.RequestedBy);
          throw new ArgumentException(ResourceStrings.InvalidIdentityNotFound((object) data.RequestedBy));
        }
      }
      BuildDefinitionSpec spec = new BuildDefinitionSpec(BuildPath.Root(data.TeamProject, data.BuildType));
      BuildControllerSpec controllerSpec = new BuildControllerSpec("*", data.BuildMachine, false);
      TeamFoundationBuildService service1 = requestContext.GetService<TeamFoundationBuildService>();
      IProjectService service2 = requestContext.GetService<IProjectService>();
      TeamFoundationBuildResourceService service3 = requestContext.GetService<TeamFoundationBuildResourceService>();
      BuildDefinitionQueryResult definitionQueryResult = service1.QueryBuildDefinitions(requestContext, spec);
      BuildDefinition buildDefinition = definitionQueryResult.Definitions.FirstOrDefault<BuildDefinition>();
      if (buildDefinition == null)
      {
        string definitionUri = BuildPath.Root(data.TeamProject, data.BuildType);
        requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Definition '{0}' not found", (object) definitionUri);
        throw new BuildDefinitionDoesNotExistException(definitionUri);
      }
      service1.BuildHost.SecurityManager.CheckBuildPermission(requestContext, buildDefinition, BuildPermissions.UpdateBuildInformation);
      BuildController buildController = service3.QueryBuildControllers(requestContext, controllerSpec).Controllers.FirstOrDefault<BuildController>();
      if (buildController == null)
      {
        requestContext.Trace(0, TraceLevel.Error, "Build", "Service", "Controller '{0}' not found", (object) controllerSpec.ServiceHostName);
        throw new BuildControllerDoesNotExistException(controllerSpec.ServiceHostName);
      }
      string sourceGetVersion = (string) null;
      if (!string.IsNullOrEmpty(data.BuildTypeFileUri) && LinkingUtilities.IsUriWellFormed(data.BuildTypeFileUri))
      {
        Microsoft.TeamFoundation.VersionControl.Server.Item versionedItem = requestContext.GetService<TeamFoundationVersionControlService>().GetVersionedItem(requestContext, data.BuildTypeFileUri);
        sourceGetVersion = new ChangesetVersionSpec(versionedItem.ChangesetId).ToDBString(requestContext);
        requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Read build type filed '{0}' at source version '{1}'", (object) versionedItem.ServerItem, (object) sourceGetVersion);
      }
      if (string.IsNullOrEmpty(sourceGetVersion))
      {
        sourceGetVersion = LatestVersionSpec.DisplayString;
        requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Used latest version spec '{0}'", (object) sourceGetVersion);
      }
      string resId = BuildQuality.TryConvertBuildQualityToResId(data.BuildQuality);
      using (BuildComponent component = requestContext.CreateComponent<BuildComponent>("Build"))
      {
        try
        {
          BuildDetail buildDetail = component.AddBuild(buildController.Uri, buildDefinition.Uri, buildDefinition.TeamProject.Id, data.BuildNumber, data.DropLocation, data.StartTime, sourceGetVersion, resId, WhidbeyHelper.ToBuildStatus(data.BuildStatus), requestedBy, requestedFor);
          requestContext.TraceEnter(0, "Build", "Service", nameof (AddBuild));
          return buildDetail.Uri;
        }
        catch (BuildQualityDoesNotExistException ex)
        {
          requestContext.TraceException(0, "Build", "Service", (Exception) ex);
          service1.BuildHost.SecurityManager.CheckBuildPermission(requestContext, (IEnumerable<BuildDefinition>) definitionQueryResult.Definitions, BuildPermissions.ManageBuildQualities);
          component.AddBuildQualities(service2.GetTeamProjectFromGuidOrName(requestContext, teamProject), (IList<string>) new string[1]
          {
            resId
          });
          BuildDetail buildDetail = component.AddBuild(buildController.Uri, buildDefinition.Uri, buildDefinition.TeamProject.Id, data.BuildNumber, data.DropLocation, data.StartTime, sourceGetVersion, resId, WhidbeyHelper.ToBuildStatus(data.BuildStatus), requestedBy, requestedFor);
          requestContext.TraceEnter(0, "Build", "Service", nameof (AddBuild));
          return buildDetail.Uri;
        }
      }
    }

    internal static void CheckBuildData(string argumentName, BuildData buildData, bool allowNull)
    {
      ArgumentValidation.Check(argumentName, (object) buildData, allowNull);
      if (buildData == null)
        return;
      ArgumentValidation.Check("BuildStatus", buildData.BuildStatus, false, (string) null);
      ArgumentValidation.Check("BuildQuality", buildData.BuildQuality, true, (string) null);
      ArgumentValidation.CheckBuildType("BuildType", buildData.BuildType, false);
      ArgumentValidation.CheckBuildNumber("BuildNumber", buildData.BuildNumber, true);
      ArgumentValidation.CheckBuildMachine("BuildMachine", buildData.BuildMachine, false);
      string dropLocation = buildData.DropLocation;
      ArgumentValidation.CheckDropLocation("DropLocation", ref dropLocation, true, (string) null);
      buildData.DropLocation = dropLocation;
    }

    internal static Microsoft.TeamFoundation.Build.Server.BuildStatus ToBuildStatus(
      string buildStatus)
    {
      if (string.Equals(buildStatus, BuildTypeResource.Status_Succeeded(), StringComparison.OrdinalIgnoreCase) || string.Equals(buildStatus, BuildTypeResource.Status_V1_Succeeded(), StringComparison.OrdinalIgnoreCase))
        return Microsoft.TeamFoundation.Build.Server.BuildStatus.Succeeded;
      if (string.Equals(buildStatus, BuildTypeResource.Status_Stopped(), StringComparison.OrdinalIgnoreCase))
        return Microsoft.TeamFoundation.Build.Server.BuildStatus.Stopped;
      if (string.Equals(buildStatus, BuildTypeResource.Status_Failed(), StringComparison.OrdinalIgnoreCase))
        return Microsoft.TeamFoundation.Build.Server.BuildStatus.Failed;
      return string.Equals(buildStatus, BuildTypeResource.Status_InProgress(), StringComparison.OrdinalIgnoreCase) ? Microsoft.TeamFoundation.Build.Server.BuildStatus.InProgress : Microsoft.TeamFoundation.Build.Server.BuildStatus.Succeeded;
    }
  }
}
