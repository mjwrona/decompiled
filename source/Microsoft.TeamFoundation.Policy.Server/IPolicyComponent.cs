// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.IPolicyComponent
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Policy.Server
{
  internal interface IPolicyComponent : IDisposable
  {
    PolicyConfigurationRecord GetPolicyConfiguration(
      Guid projectId,
      int configurationId,
      int? revisionId);

    PolicyConfigurationRecord GetLatestPolicyConfiguration(Guid projectId, int configurationId);

    IList<PolicyConfigurationRecord> GetLatestPolicyConfigurationsByScope(
      Guid projectId,
      IEnumerable<string> scopes,
      Func<PolicyConfigurationRecord, IEnumerable<string>> determineScopes,
      int top,
      int firstConfigurationId,
      Guid? policyType = null,
      bool includeHidden = false,
      bool useVersion2 = false);

    IList<PolicyConfigurationRecord> GetLatestPolicyConfigurations(
      Guid projectId,
      int top,
      int firstConfigurationId,
      Guid? policyType = null);

    IDictionary<string, int> GetPolicyConfigurationsCountByScope(
      Guid projectId,
      IEnumerable<string> scopes);

    PolicyConfigurationRecord CreatePolicyConfiguration(
      PolicyConfigurationRecord newConfiguration,
      Func<PolicyConfigurationRecord, IEnumerable<string>> determineScopes);

    PolicyConfigurationRecord UpdatePolicyConfiguration(
      PolicyConfigurationRecord updatedConfiguration,
      Func<PolicyConfigurationRecord, IEnumerable<string>> determineScopes,
      IEnumerable<string> scopes,
      int expectedRevisionId);

    PolicyConfigurationRecord DeletePolicyConfiguration(
      Guid projectId,
      int policyConfigurationId,
      Guid modifiedById,
      Func<PolicyConfigurationRecord, IEnumerable<string>> determineScopes,
      IEnumerable<string> scopes,
      int expectedRevisionId);

    void UpdatePolicyEvaluationRecords(
      Guid projectId,
      ArtifactId artifact,
      IEnumerable<PolicyEvaluationRecord> updatedRecords,
      IEnumerable<int> idsOfRecordsToDelete);

    PolicyEvaluationRecord GetPolicyEvaluationRecord(Guid projectId, Guid evaluationRecordId);

    VirtualResultCollection<PolicyEvaluationRecord> GetPolicyEvaluationRecords(
      Guid projectId,
      int? policyConfigurationId,
      int? policyConfigurationRevision,
      ArtifactId artifactId,
      bool includeNotApplicable,
      int top,
      int skip);

    VirtualResultCollection<PolicyConfigurationRecord> GetPolicyConfigurationRevisions(
      Guid projectId,
      int configurationId,
      int top,
      int skip);

    int GetDataspaceId(Guid dataspaceIdentifier);
  }
}
