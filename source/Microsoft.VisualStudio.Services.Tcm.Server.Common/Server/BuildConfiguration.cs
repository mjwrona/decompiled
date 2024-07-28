// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.BuildConfiguration
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [ClassVisibility(ClientVisibility.Internal)]
  public class BuildConfiguration : IBuildConfiguration
  {
    private const int c_queryBuildUrisBatchSize = 1000;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, UseClientDefinedProperty = true, PropertyName = "Id")]
    public int BuildConfigurationId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string TeamProjectName { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string BuildUri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string BuildFlavor { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string BuildPlatform { get; set; }

    public int BuildId { get; set; }

    internal string BuildNumber { get; set; }

    internal int BuildDefinitionId { get; set; }

    internal DateTime CreatedDate { get; set; }

    internal DateTime CompletedDate { get; set; }

    internal string RepositoryId { get; set; }

    internal string RepositoryType { get; set; }

    internal string BranchName { get; set; }

    internal string SourceVersion { get; set; }

    internal string BuildSystem { get; set; }

    internal string BuildQuality { get; set; }

    internal string BuildDefinitionName { get; set; }

    internal string TargetBranchName { get; set; }

    internal int OldBuildConfigurationId { get; set; }

    internal int MigrateBuildConfigurationId => this.OldBuildConfigurationId > 0 ? this.OldBuildConfigurationId : this.BuildConfigurationId;

    internal bool IsBuildMigrated => this.OldBuildConfigurationId > 0;

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Build Configuration Id={0} Build={1}", (object) this.BuildConfigurationId, (object) this.BuildUri);

    public BuildConfiguration Query(IVssRequestContext context, int buildConfigurationId)
    {
      using (PerfManager.Measure(context, "BusinessLayer", "BuildConfiguration.Query"))
      {
        Guid projectId;
        BuildConfiguration buildConfiguration;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          buildConfiguration = managementDatabase.QueryBuildConfigurationById(buildConfigurationId, out projectId);
        ProjectInfo project = context.GetService<IProjectService>().GetProject(context, projectId);
        buildConfiguration.TeamProjectName = project.Name;
        return buildConfiguration;
      }
    }

    public BuildConfiguration QueryWithPlatformAndFlavor(
      IVssRequestContext context,
      Guid projectId,
      int buildId,
      string platform,
      string flavor)
    {
      using (PerfManager.Measure(context, "BusinessLayer", "BuildConfiguration.QueryWithPlatformAndFlavor"))
      {
        ProjectInfo project = context.GetService<IProjectService>().GetProject(context, projectId);
        if (platform == null)
          platform = string.Empty;
        if (flavor == null)
          flavor = string.Empty;
        BuildConfiguration flavorAndPlatform;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          flavorAndPlatform = managementDatabase.GetBuildConfigurationIdFromFlavorAndPlatform(projectId, buildId, platform, flavor);
        flavorAndPlatform.TeamProjectName = project.Name;
        return flavorAndPlatform;
      }
    }

    public BuildConfiguration Query(
      IVssRequestContext context,
      Guid projectId,
      int buildConfigurationId)
    {
      using (PerfManager.Measure(context, "BusinessLayer", "BuildConfiguration.Query"))
      {
        BuildConfiguration buildConfiguration;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          buildConfiguration = managementDatabase.QueryBuildConfigurationById2(buildConfigurationId, projectId);
        ProjectInfo project = context.GetService<IProjectService>().GetProject(context, projectId);
        buildConfiguration.TeamProjectName = project.Name;
        return buildConfiguration;
      }
    }

    public IList<string> Query(
      IVssRequestContext context,
      Guid projectId,
      bool? queryDeletedBuild = false,
      int pageSize = 1000)
    {
      try
      {
        context.TraceEnter(0, "TestManagement", "BusinessLayer", "BuildConfiguration.Query");
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          return managementDatabase.QueryBuildsByProject(projectId, queryDeletedBuild, pageSize);
      }
      finally
      {
        context.TraceLeave(0, "TestManagement", "BusinessLayer", "BuildConfiguration.Query");
      }
    }
  }
}
