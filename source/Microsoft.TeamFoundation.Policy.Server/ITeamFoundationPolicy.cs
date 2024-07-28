// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.ITeamFoundationPolicy
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Policy.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.Policy.Server
{
  [InheritedExport]
  public interface ITeamFoundationPolicy
  {
    Guid Id { get; }

    string DisplayName { get; }

    string Description { get; }

    string Area { get; }

    bool IsBypassable { get; }

    bool IsHidden { get; }

    bool AllowSyncPublishNotification { get; }

    PolicyConfigurationRecord Configuration { get; }

    object DeserializeSettings(string settingsString);

    bool CheckSettingsValidity(
      IVssRequestContext requestContext,
      Guid projectId,
      object settings,
      out string errorMessage);

    void CheckEditPoliciesPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      object settings);

    void CheckSupportedMatchKind(IVssRequestContext requestContext, object settings);

    void Initialize(
      IVssRequestContext requestContext,
      PolicyConfigurationRecord configuration,
      object settings);

    bool IsApplicableTo(IVssRequestContext requestContext, ITeamFoundationPolicyTarget target);

    string[] GetScopes(object settings);

    TeamFoundationPolicyEvaluationRecordContext ParseContext(
      IVssRequestContext requestContext,
      Guid projectId,
      JObject rawContext);

    void AppendClientTraceData(object settings, ref ClientTraceData eventData);

    PolicyNotificationResult Requeue(
      IVssRequestContext requestContext,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext,
      ITeamFoundationPolicyTarget target);
  }
}
