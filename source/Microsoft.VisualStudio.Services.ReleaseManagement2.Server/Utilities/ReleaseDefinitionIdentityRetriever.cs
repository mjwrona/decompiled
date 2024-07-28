// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.ReleaseDefinitionIdentityRetriever
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public class ReleaseDefinitionIdentityRetriever
  {
    private readonly Func<IVssRequestContext, Guid> getRequestorId;
    private readonly Func<IVssRequestContext, IList<string>, IdentityHelper> getIdentityHelper;

    public ReleaseDefinitionIdentityRetriever()
      : this((Func<IVssRequestContext, Guid>) (context => context.GetUserId(true)), ReleaseDefinitionIdentityRetriever.\u003C\u003EO.\u003C0\u003E__GetIdentityHelper ?? (ReleaseDefinitionIdentityRetriever.\u003C\u003EO.\u003C0\u003E__GetIdentityHelper = new Func<IVssRequestContext, IList<string>, IdentityHelper>(IdentityHelper.GetIdentityHelper)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is the intended design")]
    protected ReleaseDefinitionIdentityRetriever(
      Func<IVssRequestContext, Guid> getRequestorId,
      Func<IVssRequestContext, IList<string>, IdentityHelper> getIdentityHelper)
    {
      this.getRequestorId = getRequestorId;
      this.getIdentityHelper = getIdentityHelper;
    }

    public void PopulateServerDefinition(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition serverDefinition,
      IVssRequestContext context)
    {
      if (serverDefinition == null)
        throw new ArgumentNullException(nameof (serverDefinition));
      Guid guid = this.getRequestorId(context);
      serverDefinition.CreatedBy = serverDefinition.ModifiedBy = guid;
      foreach (DefinitionEnvironment definitionEnvironment in serverDefinition.Environments.Where<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (e => e.OwnerId == Guid.Empty)))
        definitionEnvironment.OwnerId = guid;
    }

    public void PopulateWebApiDefinitions(
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition> webApiDefinitions,
      IVssRequestContext context)
    {
      if (webApiDefinitions == null)
        throw new ArgumentNullException(nameof (webApiDefinitions));
      using (ReleaseManagementTimer.Create(context, "Service", "RDIdentityRetriever.PopulateIdentities", 1971058))
      {
        IList<string> teamFoundationIds = ReleaseDefinitionIdentityRetriever.GetUniqueTeamFoundationIds(webApiDefinitions);
        IdentityHelper identityHelper = this.getIdentityHelper(context, teamFoundationIds);
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition webApiDefinition in webApiDefinitions)
          ReleaseDefinitionIdentityRetriever.PopulateWebApiDefinitionIdentities(webApiDefinition, identityHelper);
      }
    }

    public void PopulateWebApiDefinition(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition webApiDefinition,
      IVssRequestContext context)
    {
      IList<string> stringList = webApiDefinition != null ? ReleaseDefinitionIdentityRetriever.GetUniqueTeamFoundationIds(webApiDefinition) : throw new ArgumentNullException(nameof (webApiDefinition));
      IdentityHelper identityHelper = this.getIdentityHelper(context, stringList);
      ReleaseDefinitionIdentityRetriever.PopulateWebApiDefinitionIdentities(webApiDefinition, identityHelper);
    }

    private static void PopulateWebApiDefinitionIdentities(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition webApiDefinition,
      IdentityHelper identityHelper)
    {
      webApiDefinition.CreatedBy = identityHelper.GetIdentity(webApiDefinition.CreatedBy);
      webApiDefinition.ModifiedBy = identityHelper.GetIdentity(webApiDefinition.ModifiedBy);
      if (webApiDefinition.LastRelease != null)
        webApiDefinition.LastRelease.CreatedBy = identityHelper.GetIdentity(webApiDefinition.LastRelease.CreatedBy);
      foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) webApiDefinition.Environments)
      {
        environment.Owner = identityHelper.GetIdentity(environment.Owner);
        foreach (ReleaseDefinitionApprovalStep definitionApprovalStep in webApiDefinition.Environments.SelectMany<ReleaseDefinitionEnvironment, ReleaseDefinitionApprovalStep>((Func<ReleaseDefinitionEnvironment, IEnumerable<ReleaseDefinitionApprovalStep>>) (e => (IEnumerable<ReleaseDefinitionApprovalStep>) e.PreDeployApprovals.Approvals)))
          definitionApprovalStep.Approver = identityHelper.GetIdentity(definitionApprovalStep.Approver);
        foreach (ReleaseDefinitionApprovalStep definitionApprovalStep in webApiDefinition.Environments.SelectMany<ReleaseDefinitionEnvironment, ReleaseDefinitionApprovalStep>((Func<ReleaseDefinitionEnvironment, IEnumerable<ReleaseDefinitionApprovalStep>>) (e => (IEnumerable<ReleaseDefinitionApprovalStep>) e.PostDeployApprovals.Approvals)))
          definitionApprovalStep.Approver = identityHelper.GetIdentity(definitionApprovalStep.Approver);
      }
    }

    private static IList<string> GetUniqueTeamFoundationIds(
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition> webApiDefinitions)
    {
      List<IdentityRef> source = new List<IdentityRef>();
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition webApiDefinition in webApiDefinitions)
        source.AddRange(ReleaseDefinitionIdentityRetriever.GetTeamFoundationIds(webApiDefinition));
      return (IList<string>) source.Select<IdentityRef, string>((Func<IdentityRef, string>) (identity => identity.Id)).Distinct<string>().ToList<string>();
    }

    private static IList<string> GetUniqueTeamFoundationIds(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition webApiDefinition) => (IList<string>) ReleaseDefinitionIdentityRetriever.GetTeamFoundationIds(webApiDefinition).Select<IdentityRef, string>((Func<IdentityRef, string>) (identity => identity.Id)).Distinct<string>().ToList<string>();

    private static IEnumerable<IdentityRef> GetTeamFoundationIds(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition webApiDefinition)
    {
      List<IdentityRef> first = new List<IdentityRef>()
      {
        webApiDefinition.CreatedBy,
        webApiDefinition.ModifiedBy
      };
      if (webApiDefinition.LastRelease != null)
        first.Add(webApiDefinition.LastRelease.CreatedBy);
      List<IdentityRef> list = webApiDefinition.Environments.Select<ReleaseDefinitionEnvironment, IdentityRef>((Func<ReleaseDefinitionEnvironment, IdentityRef>) (e => e.Owner)).ToList<IdentityRef>();
      IEnumerable<IdentityRef> second1 = webApiDefinition.Environments.SelectMany<ReleaseDefinitionEnvironment, IdentityRef>((Func<ReleaseDefinitionEnvironment, IEnumerable<IdentityRef>>) (e => e.PreDeployApprovals.Approvals.Select<ReleaseDefinitionApprovalStep, IdentityRef>((Func<ReleaseDefinitionApprovalStep, IdentityRef>) (s => s.Approver))));
      IEnumerable<IdentityRef> second2 = webApiDefinition.Environments.SelectMany<ReleaseDefinitionEnvironment, IdentityRef>((Func<ReleaseDefinitionEnvironment, IEnumerable<IdentityRef>>) (e => e.PostDeployApprovals.Approvals.Select<ReleaseDefinitionApprovalStep, IdentityRef>((Func<ReleaseDefinitionApprovalStep, IdentityRef>) (s => s.Approver))));
      return first.Union<IdentityRef>((IEnumerable<IdentityRef>) list).Union<IdentityRef>(second1).Union<IdentityRef>(second2).Where<IdentityRef>((Func<IdentityRef, bool>) (id => id != null));
    }
  }
}
