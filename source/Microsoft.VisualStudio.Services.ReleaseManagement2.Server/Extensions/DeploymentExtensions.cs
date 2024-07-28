// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.DeploymentExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  public static class DeploymentExtensions
  {
    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment ToContract(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment deployment,
      IVssRequestContext context,
      Guid projectId)
    {
      return ((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>) new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment[1]
      {
        deployment
      }).ToContract(context, projectId).FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>();
    }

    private static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment ToDeploymentContract(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment deployment,
      IVssRequestContext context,
      Guid projectId)
    {
      using (ReleaseManagementTimer.Create(context, "Service", "DeploymentExtensions.ToDeploymentContract", 1900006))
      {
        foreach (ArtifactSource linkedArtifact in (IEnumerable<ArtifactSource>) deployment.LinkedArtifacts)
          linkedArtifact.PopulateReleaseArtifact(context, projectId);
        return deployment.ConvertModelToContract(context, projectId);
      }
    }

    public static IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment> ToContract(
      this IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> deploymentsList,
      IVssRequestContext context,
      Guid projectId)
    {
      using (ReleaseManagementTimer.Create(context, "Service", "DeploymentExtensions.DeploymentModelToWebApi", 1900007))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return deploymentsList.ToContractImplementation(context, projectId, DeploymentExtensions.\u003C\u003EO.\u003C0\u003E__ToDeploymentContract ?? (DeploymentExtensions.\u003C\u003EO.\u003C0\u003E__ToDeploymentContract = new Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment, IVssRequestContext, Guid, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>(DeploymentExtensions.ToDeploymentContract)), DeploymentExtensions.\u003C\u003EO.\u003C1\u003E__PopulateIdentities ?? (DeploymentExtensions.\u003C\u003EO.\u003C1\u003E__PopulateIdentities = new Action<List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>, IVssRequestContext>(DeploymentExtensions.PopulateIdentities)));
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Testing requirements")]
    public static IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment> ToContractImplementation(
      this IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> deploymentList,
      IVssRequestContext context,
      Guid projectId,
      Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment, IVssRequestContext, Guid, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment> convertToWebApiDeployment,
      Action<List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>, IVssRequestContext> populateIdentities)
    {
      if (convertToWebApiDeployment == null)
        throw new ArgumentNullException(nameof (convertToWebApiDeployment));
      if (populateIdentities == null)
        throw new ArgumentNullException(nameof (populateIdentities));
      List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment> list = deploymentList.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>) (release => convertToWebApiDeployment(release, context, projectId))).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>();
      populateIdentities(list, context);
      return (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>) list;
    }

    public static IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment> ToContract(
      this IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment> deploymentList,
      IVssRequestContext context)
    {
      List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment> list = deploymentList.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>) (deployment => deployment.ToDeploymentContract(context, deployment.ProjectId))).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>();
      DeploymentExtensions.PopulateIdentities(list, context);
      return (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>) list;
    }

    private static void PopulateIdentities(List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment> deployments, IVssRequestContext context)
    {
      using (ReleaseManagementTimer.Create(context, "Service", "DeploymentExtensions.PopulateIdentities", 1961101))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        DeploymentExtensions.PopulateIdentitiesImplementation((IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>) deployments, context, DeploymentExtensions.\u003C\u003EO.\u003C2\u003E__GetIdentityHelper ?? (DeploymentExtensions.\u003C\u003EO.\u003C2\u003E__GetIdentityHelper = new Func<IVssRequestContext, IList<string>, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.IdentityHelper>(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.IdentityHelper.GetIdentityHelper)), DeploymentExtensions.\u003C\u003EO.\u003C3\u003E__PopulateIdentities ?? (DeploymentExtensions.\u003C\u003EO.\u003C3\u003E__PopulateIdentities = new Action<IVssRequestContext, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.IdentityHelper>(DeploymentIdentityHandler.PopulateIdentities)));
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Testing requirements")]
    public static void PopulateIdentitiesImplementation(
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment> deployments,
      IVssRequestContext context,
      Func<IVssRequestContext, IList<string>, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.IdentityHelper> getIdentityMap,
      Action<IVssRequestContext, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.IdentityHelper> updateReferences)
    {
      if (deployments == null)
        throw new ArgumentNullException(nameof (deployments));
      if (getIdentityMap == null)
        throw new ArgumentNullException(nameof (getIdentityMap));
      if (updateReferences == null)
        throw new ArgumentNullException(nameof (updateReferences));
      HashSet<string> source = new HashSet<string>();
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment deployment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>) deployments)
      {
        IList<string> teamFoundationIds = DeploymentIdentityHandler.GetTeamFoundationIds(deployment);
        source.UnionWith((IEnumerable<string>) teamFoundationIds);
      }
      Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.IdentityHelper identityMap = getIdentityMap(context, (IList<string>) source.ToList<string>());
      deployments.ToList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>().ForEach((Action<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>) (deployment => updateReferences(context, deployment, identityMap)));
    }
  }
}
