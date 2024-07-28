// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.BuildSecurity
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Build.Common
{
  [GenerateSpecificConstants(null)]
  public static class BuildSecurity
  {
    public static readonly Guid AdministrationNamespaceId = new Guid("302ACACA-B667-436d-A946-87133492041C");
    public const string BuildNamespaceIdString = "33344D9C-FC72-4d6f-ABA5-FA317101A7E9";
    [GenerateConstant(null)]
    public static readonly Guid BuildNamespaceId = new Guid("33344D9C-FC72-4d6f-ABA5-FA317101A7E9");
    public static readonly string PrivilegesToken = "BuildPrivileges";
    public static readonly char NamespaceSeparator = Security.NamespaceSeparator;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string[] GetPermissionStrings(Guid namespaceId, int permissions)
    {
      List<string> stringList = new List<string>();
      if (namespaceId == BuildSecurity.AdministrationNamespaceId)
      {
        if ((permissions & AdministrationPermissions.ManageBuildResources) != 0)
          stringList.Add(BuildTypeResource.ManageBuildResourcesPermission());
        if ((permissions & AdministrationPermissions.ViewBuildResources) != 0)
          stringList.Add(BuildTypeResource.ViewBuildResourcesPermission());
        if ((permissions & AdministrationPermissions.UseBuildResources) != 0)
          stringList.Add(BuildTypeResource.UseBuildResourcesPermission());
        if ((permissions & AdministrationPermissions.AdministerBuildResourcePermissions) != 0)
          stringList.Add(BuildTypeResource.AdministerBuildResourcePermissionsPermission());
        if ((permissions & AdministrationPermissions.ManagePipelinePolicies) != 0)
          stringList.Add(BuildTypeResource.ManagePipelinePoliciesPermission());
      }
      else if (namespaceId == BuildSecurity.BuildNamespaceId)
      {
        if ((permissions & BuildPermissions.DeleteBuilds) != 0)
          stringList.Add(BuildTypeResource.DeleteBuildsPermission());
        if ((permissions & BuildPermissions.DestroyBuilds) != 0)
          stringList.Add(BuildTypeResource.DestroyBuildsPermission());
        if ((permissions & BuildPermissions.EditBuildQuality) != 0)
          stringList.Add(BuildTypeResource.EditBuildQualityPermission());
        if ((permissions & BuildPermissions.ManageBuildQualities) != 0)
          stringList.Add(BuildTypeResource.ManageBuildQualitiesPermission());
        if ((permissions & BuildPermissions.RetainIndefinitely) != 0)
          stringList.Add(BuildTypeResource.RetainIndefinitelyPermission());
        if ((permissions & BuildPermissions.ViewBuilds) != 0)
          stringList.Add(BuildTypeResource.ViewBuildsPermission());
        if ((permissions & BuildPermissions.ManageBuildQueue) != 0)
          stringList.Add(BuildTypeResource.ManageBuildQueuePermission());
        if ((permissions & BuildPermissions.QueueBuilds) != 0)
          stringList.Add(BuildTypeResource.QueueBuildsPermission());
        if ((permissions & BuildPermissions.StopBuilds) != 0)
          stringList.Add(BuildTypeResource.StopBuildsPermission());
        if ((permissions & BuildPermissions.DeleteBuildDefinition) != 0)
          stringList.Add(BuildTypeResource.DeleteBuildDefinitionPermission());
        if ((permissions & BuildPermissions.EditBuildDefinition) != 0)
          stringList.Add(BuildTypeResource.EditBuildDefinitionPermission());
        if ((permissions & BuildPermissions.ViewBuildDefinition) != 0)
          stringList.Add(BuildTypeResource.ViewBuildDefinitionPermission());
        if ((permissions & BuildPermissions.UpdateBuildInformation) != 0)
          stringList.Add(BuildTypeResource.UpdateBuildInformationPermission());
        if ((permissions & BuildPermissions.OverrideBuildCheckInValidation) != 0)
          stringList.Add(BuildTypeResource.OverrideBuildCheckInPermission());
        if ((permissions & BuildPermissions.AdministerBuildPermissions) != 0)
          stringList.Add(BuildTypeResource.AdministerBuildPermissionsPermission());
        if ((permissions & BuildPermissions.EditPipelineQueueConfigurationPermission) != 0)
          stringList.Add(BuildTypeResource.EditPipelineQueueConfigurationPermission());
      }
      return stringList.ToArray();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetSecurityTokenPath(string path) => Security.GetSecurityTokenPath(path);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static int ConvertAuthPermissionsToBuildPermissions(int authPermissions)
    {
      int buildPermissions = 0;
      if ((authPermissions & AuthorizationProjectPermissions.GenericRead) != 0)
        buildPermissions = buildPermissions | BuildPermissions.ViewBuilds | BuildPermissions.ViewBuildDefinition;
      if ((authPermissions & AuthorizationProjectPermissions.EditBuildStatus) != 0)
        buildPermissions = buildPermissions | BuildPermissions.EditBuildQuality | BuildPermissions.RetainIndefinitely;
      if ((authPermissions & AuthorizationProjectPermissions.AdministerBuild) != 0)
        buildPermissions = buildPermissions | BuildPermissions.AdministerBuildPermissions | BuildPermissions.DeleteBuilds | BuildPermissions.ManageBuildQualities | BuildPermissions.DestroyBuilds | BuildPermissions.EditBuildDefinition | BuildPermissions.DeleteBuildDefinition | BuildPermissions.ManageBuildQueue | BuildPermissions.StopBuilds;
      if ((authPermissions & AuthorizationProjectPermissions.StartBuild) != 0)
        buildPermissions |= BuildPermissions.QueueBuilds;
      if ((authPermissions & AuthorizationProjectPermissions.UpdateBuild) != 0)
        buildPermissions = buildPermissions | BuildPermissions.UpdateBuildInformation | BuildPermissions.OverrideBuildCheckInValidation;
      return buildPermissions;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static int ConvertBuildPermissionsToAuthPermissions(int buildPermissions)
    {
      int authPermissions = 0;
      if ((buildPermissions & BuildPermissions.ViewBuilds) != 0 && (buildPermissions & BuildPermissions.ViewBuildDefinition) != 0)
        authPermissions |= AuthorizationProjectPermissions.GenericRead;
      if ((buildPermissions & BuildPermissions.EditBuildQuality) != 0 && (buildPermissions & BuildPermissions.RetainIndefinitely) != 0)
        authPermissions |= AuthorizationProjectPermissions.EditBuildStatus;
      if ((buildPermissions & BuildPermissions.AdministerBuildPermissions) != 0 && (buildPermissions & BuildPermissions.DeleteBuilds) != 0 && (buildPermissions & BuildPermissions.ManageBuildQualities) != 0 && (buildPermissions & BuildPermissions.DestroyBuilds) != 0 && (buildPermissions & BuildPermissions.EditBuildDefinition) != 0 && (buildPermissions & BuildPermissions.DeleteBuildDefinition) != 0 && (buildPermissions & BuildPermissions.ManageBuildQueue) != 0 && (buildPermissions & BuildPermissions.StopBuilds) != 0)
        authPermissions |= AuthorizationProjectPermissions.AdministerBuild;
      if ((buildPermissions & BuildPermissions.QueueBuilds) != 0)
        authPermissions |= AuthorizationProjectPermissions.StartBuild;
      if ((buildPermissions & BuildPermissions.UpdateBuildInformation) != 0 && (buildPermissions & BuildPermissions.OverrideBuildCheckInValidation) != 0)
        authPermissions |= AuthorizationProjectPermissions.UpdateBuild;
      return authPermissions;
    }
  }
}
