// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ITeamFoundationWorkItemTrackingMetadataService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  [DefaultServiceImplementation(typeof (TeamFoundationWorkItemTrackingMetadataService))]
  public interface ITeamFoundationWorkItemTrackingMetadataService : IVssFrameworkService
  {
    ConstantRecord GetConstantRecord(
      IVssRequestContext requestContext,
      int constantId,
      WitReadReplicaContext? readReplicaContext = null);

    IEnumerable<ConstantRecord> GetConstantRecords(
      IVssRequestContext requestContext,
      IEnumerable<int> constantIds,
      bool includeInactiveConstants = false,
      WitReadReplicaContext? readReplicaContext = null);

    ConstantRecord GetConstantRecord(
      IVssRequestContext requestContext,
      Guid teamFoundationId,
      WitReadReplicaContext? readReplicaContext = null);

    IEnumerable<ConstantRecord> GetConstantRecords(
      IVssRequestContext requestContext,
      IEnumerable<Guid> teamFoundationIds,
      bool includeInactiveIdentities = false,
      WitReadReplicaContext? readReplicaContext = null);

    IEnumerable<ConstantRecord> GetConstantRecords(
      IVssRequestContext requestContext,
      IEnumerable<string> displayNames,
      bool includeInactiveIdentities = false,
      WitReadReplicaContext? readReplicaContext = null,
      bool includeInactiveNonIdentityConstants = true);

    IEnumerable<IdentityConstantRecord> SearchConstantIdentityRecords(
      IVssRequestContext requestContext,
      string searchTerm,
      SearchIdentityType identityType = SearchIdentityType.All);

    IEnumerable<PersonNameConstantRecord> GetConstantRecordsFromPersonNames(
      IVssRequestContext requestContext,
      IEnumerable<string> personNames);

    MetadataDBStamps GetMetadataTableTimestamps(IVssRequestContext requestContext);

    MetadataDBStamps GetMetadataTableTimestamps(
      IVssRequestContext requestContext,
      IEnumerable<MetadataTable> tableNames);

    bool SyncIdentity(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity, string source = null);

    bool SyncIdentities(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      string source = null);

    IEnumerable<ConstantsSearchRecord> SearchConstantsRecords(
      IVssRequestContext requestContext,
      IEnumerable<string> searchValues,
      IEnumerable<Guid> vsids,
      bool includeInactiveIdentities);

    IDictionary<ConstantSetReference, SetRecord[]> GetConstantSets(
      IVssRequestContext requestContext,
      IEnumerable<ConstantSetReference> setReferences);

    IDictionary<ConstantSetReference, SetRecord[]> GetDirectConstantSets(
      IVssRequestContext requestContext,
      IEnumerable<ConstantSetReference> setReferences);

    void SaveRuleSetRecordToSets(
      IVssRequestContext requestContext,
      IEnumerable<RuleSetRecord> ruleSetRecords);

    IEnumerable<GroupMembershipRecord> GetRuleDependentGroups(IVssRequestContext requestContext);

    IEnumerable<ForNotRuleGroupRecord> GetForNotRuleGroups(IVssRequestContext requestContext);

    void SetIdentityFieldBit(IVssRequestContext requestContext);

    IEnumerable<ConstantRecord> GetNonIdentityConstants(
      IVssRequestContext requestContext,
      IEnumerable<string> displayTextValues,
      bool includeInactiveNonIdentityConstants = true);

    IEnumerable<Guid> GetForceProcessADObjects(IVssRequestContext requestContext);

    void ForceSyncADObjects(
      IVssRequestContext requestContext,
      IEnumerable<ImsSyncIdentity> identities);

    IEnumerable<ConstantAuditEntry> GetDuplicateIdentityConstants(IVssRequestContext requestContext);

    bool TrySyncIdentitiesToConstants(
      IVssRequestContext requestContext,
      IList<string> missingIdentities,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> resolvedMissingIdentities);

    void CleanupConstants(
      IVssRequestContext requestContext,
      IReadOnlyCollection<int> usedIds,
      long constantsMetadataStamp);

    IDictionary<string, IList<GroupMemberEntry>> FindGroupMembersOfType(
      IVssRequestContext requestContext,
      IEnumerable<string> groupNames,
      GroupType groupType,
      int maxMemberForEachGroup = 100,
      int maxIteration = 50,
      WitReadReplicaContext? readReplicaContext = null);

    void ResetSequenceId(
      IVssRequestContext requestContext,
      int identitySequenceId,
      int groupSequenceId);

    void ForceRefreshClientMetadata(IVssRequestContext requestContext);
  }
}
