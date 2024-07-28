// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentitySynchronizer
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentitySynchronizer
  {
    private const int c_SyncWholeGroupThreshold = 350;
    private readonly IDictionary<string, IIdentityProvider> m_syncAgents;
    private readonly PlatformIdentityStore m_identityStore;
    private readonly int m_errorLimit;
    private readonly ITFLogger m_logger;
    private readonly SyncErrors m_syncErrors;
    private int m_totalErrors;
    private Queue<Microsoft.VisualStudio.Services.Identity.Identity> m_groupsToSync;
    private HashSet<IdentityDescriptor> m_syncedGroups;
    private HashSet<IdentityDescriptor> m_knownGroups;
    private List<Microsoft.VisualStudio.Services.Identity.Identity> m_newGroups;
    private const string c_area = "Identity";
    private const string c_layer = "IdentitySynchronizer";
    private const string c_disableLogErrorsSummaryFeatureFlag = "VisualStudio.Services.Identity.SyncErrors.DisableLogErrorsSummary";

    public IdentitySynchronizer(
      IDictionary<string, IIdentityProvider> syncAgents,
      PlatformIdentityStore identityStore,
      int errorLimit,
      ITFLogger logger)
    {
      this.m_syncAgents = syncAgents;
      this.m_identityStore = identityStore;
      this.m_errorLimit = errorLimit;
      this.m_logger = logger ?? (ITFLogger) new ServerTraceLogger();
      this.m_syncErrors = new SyncErrors(logger);
    }

    public virtual void SyncIdentity(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity rootIdentity,
      bool recursive)
    {
      this.SyncIdentity(requestContext, rootIdentity, recursive, true);
    }

    internal void SyncIdentity(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity rootIdentity,
      bool recursive,
      bool logErrors)
    {
      IdentityServiceBase.Trace(TraceLevel.Info, "Starting Sync of {0}, recursion: {1}", (object) rootIdentity.DisplayName, (object) recursive);
      if (!rootIdentity.IsContainer)
      {
        this.SyncOneUser(requestContext, rootIdentity);
      }
      else
      {
        this.m_totalErrors = 0;
        this.m_syncedGroups = new HashSet<IdentityDescriptor>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
        this.m_groupsToSync = new Queue<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (recursive)
        {
          this.m_groupsToSync.Enqueue(rootIdentity);
          while (this.m_groupsToSync.Count > 0)
          {
            Microsoft.VisualStudio.Services.Identity.Identity groupToSync = this.m_groupsToSync.Dequeue();
            if (!this.m_syncedGroups.Contains(groupToSync.Descriptor))
            {
              this.m_syncedGroups.Add(groupToSync.Descriptor);
              this.SyncOneGroup(requestContext, groupToSync);
            }
          }
        }
        else
          this.SyncOneGroup(requestContext, rootIdentity);
        if (logErrors)
          this.LogErrorsSummary(requestContext);
      }
      IdentityServiceBase.Trace(TraceLevel.Info, "Finished Sync {0}", (object) rootIdentity.DisplayName);
    }

    public virtual void SyncTree(IVssRequestContext requestContext)
    {
      IdentityServiceBase.Trace(TraceLevel.Info, "{0} Starting Sync of full tree", (object) DateTime.Now);
      Microsoft.VisualStudio.Services.Identity.Identity rootIdentity = this.ReadIdentityFromDatabase(requestContext, this.m_identityStore.Domain.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.EveryoneGroup), false);
      this.m_identityStore.UpdateSyncQueue(requestContext, this.m_identityStore.Domain, (IList<Guid>) new Guid[1]
      {
        Guid.Empty
      }, (IList<Tuple<Guid, bool, bool>>) null);
      this.SyncIdentity(requestContext, rootIdentity, true);
      IdentityServiceBase.Trace(TraceLevel.Info, "Finished Sync of full tree");
    }

    protected virtual void SyncOneGroup(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity groupToSync)
    {
      requestContext.RequestContextInternal().CheckCanceled();
      this.m_syncErrors.Initialize(groupToSync.DisplayName);
      IdentityServiceBase.Trace(TraceLevel.Info, "Now syncing {0}", (object) groupToSync.DisplayName);
      try
      {
        IdentitySyncHelper syncHelper = new IdentitySyncHelper(requestContext, this.m_identityStore, groupToSync);
        string property = groupToSync.GetProperty<string>("DN", string.Empty);
        this.m_syncAgents[groupToSync.Descriptor.IdentityType].SyncMembers(requestContext, groupToSync.Descriptor, (IIdentitySyncHelper) syncHelper, this.m_syncAgents, property, this.m_syncErrors);
        syncHelper.Submit(out bool _);
        foreach (Microsoft.VisualStudio.Services.Identity.Identity nestedGroup in syncHelper.NestedGroups)
          this.m_groupsToSync.Enqueue(nestedGroup);
        IdentityServiceBase.Trace(TraceLevel.Info, "Finished syncing {0}", (object) groupToSync.DisplayName);
      }
      catch (Exception ex)
      {
        this.m_syncErrors.Add(groupToSync.DisplayName, ex);
        IdentityServiceBase.Trace(TraceLevel.Info, "Error syncing {0}", (object) groupToSync.DisplayName);
        Microsoft.VisualStudio.Services.Identity.Identity identity = this.ReadIdentityFromDatabase(requestContext, groupToSync.Descriptor, true);
        this.EnqueueMemberGroups(requestContext, identity);
      }
      this.LogErrors(requestContext);
      this.m_totalErrors += this.m_syncErrors.Count;
      if (this.m_totalErrors > this.m_errorLimit)
        throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Aborting sync due to too many errors (exceeded {0})", (object) this.m_errorLimit));
    }

    public virtual bool SyncFromQueue(
      IVssRequestContext requestContext,
      out Microsoft.VisualStudio.Services.Identity.Identity idSynced,
      out bool log)
    {
      IdentityServiceBase.Trace(TraceLevel.Info, "Starting Sync from queue");
      SyncQueueData syncQueueData = this.m_identityStore.ReadSyncQueue(requestContext, this.m_identityStore.Domain, 1).FirstOrDefault<SyncQueueData>();
      if (syncQueueData != null)
      {
        log = syncQueueData.Log;
        idSynced = this.m_identityStore.ReadIdentities(requestContext, this.m_identityStore.Domain, (IList<Guid>) new Guid[1]
        {
          syncQueueData.IdentityId
        }, QueryMembership.None, false, (IEnumerable<string>) null, bypassCache: true, filterResults: false)[0];
        this.SyncIdentity(requestContext, idSynced, syncQueueData.Recursive);
        this.m_identityStore.UpdateSyncQueue(requestContext, this.m_identityStore.Domain, (IList<Guid>) new Guid[1]
        {
          syncQueueData.IdentityId
        }, (IList<Tuple<Guid, bool, bool>>) null);
        return true;
      }
      idSynced = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      log = false;
      return false;
    }

    public virtual void SyncTreeMembership(IVssRequestContext requestContext)
    {
      IdentityServiceBase.Trace(TraceLevel.Info, "{0} Starting Membership Sync of full tree", (object) DateTime.Now);
      IdentityServiceBase.Trace(TraceLevel.Info, "Sync whole group threshold {0}", (object) 350);
      this.m_totalErrors = 0;
      this.m_syncedGroups = new HashSet<IdentityDescriptor>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      this.m_groupsToSync = new Queue<Microsoft.VisualStudio.Services.Identity.Identity>();
      this.m_newGroups = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (this.m_knownGroups == null)
      {
        IEnumerable<IdentityDescriptor> allIdentityGroups = (IEnumerable<IdentityDescriptor>) this.m_identityStore.GetAllIdentityGroups(requestContext, this.m_identityStore.Domain);
        if (allIdentityGroups == null)
        {
          IdentityServiceBase.Trace(TraceLevel.Warning, "GetAllIdentityGroups returned null IEnumerable<IdentityDescriptor> for domain {0}", (object) this.m_identityStore.Domain.Name);
          this.m_knownGroups = new HashSet<IdentityDescriptor>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
        }
        else
          this.m_knownGroups = new HashSet<IdentityDescriptor>(allIdentityGroups, (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      }
      Microsoft.VisualStudio.Services.Identity.Identity identity = this.ReadIdentityFromDatabase(requestContext, this.m_identityStore.Domain.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.EveryoneGroup), false);
      if (identity == null)
        IdentityServiceBase.Trace(TraceLevel.Error, "ReadIdentityFromDatabase returned null for the 'Everyone' group");
      else if ((IdentityDescriptor) null == identity.Descriptor)
        IdentityServiceBase.Trace(TraceLevel.Error, "ReadIdentityFromDatabase returned a null Descriptor for the 'Everyone' group");
      else
        this.m_groupsToSync.Enqueue(identity);
      while (this.m_groupsToSync.Count > 0)
      {
        Microsoft.VisualStudio.Services.Identity.Identity groupToSync = this.m_groupsToSync.Dequeue();
        if (!this.m_syncedGroups.Contains(groupToSync.Descriptor))
        {
          this.m_syncedGroups.Add(groupToSync.Descriptor);
          this.SyncOneGroupMembership(requestContext, groupToSync);
        }
      }
      foreach (Microsoft.VisualStudio.Services.Identity.Identity newGroup in this.m_newGroups)
        this.SyncIdentity(requestContext, newGroup, true, false);
      this.LogErrorsSummary(requestContext);
      IdentityServiceBase.Trace(TraceLevel.Info, "Finished IMS Membership Sync");
    }

    protected virtual void SyncOneGroupMembership(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity groupToSync)
    {
      requestContext.RequestContextInternal().CheckCanceled();
      this.m_syncErrors.Initialize(groupToSync.DisplayName);
      IdentityServiceBase.Trace(TraceLevel.Info, "Begin Membership sync for {0}", (object) groupToSync.DisplayName);
      try
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = this.ReadIdentityFromSource(requestContext, groupToSync, true);
        IdentityServiceBase.Trace(TraceLevel.Info, "Finished reading from source for {0}", (object) groupToSync.DisplayName);
        this.EnqueueMemberGroups(requestContext, identity1);
        if (!IdentityValidation.IsTeamFoundationType(groupToSync.Descriptor))
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity2 = this.ReadIdentityFromDatabase(requestContext, groupToSync.Descriptor, true);
          ICollection<IdentityDescriptor> identityDescriptors1 = identity1 != null ? identity1.Members : (ICollection<IdentityDescriptor>) Array.Empty<IdentityDescriptor>();
          ICollection<IdentityDescriptor> identityDescriptors2 = identity2 != null ? identity2.Members : (ICollection<IdentityDescriptor>) Array.Empty<IdentityDescriptor>();
          HashSet<IdentityDescriptor> identityDescriptorSet = new HashSet<IdentityDescriptor>((IEnumerable<IdentityDescriptor>) identityDescriptors1, (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
          identityDescriptorSet.ExceptWith((IEnumerable<IdentityDescriptor>) identityDescriptors2);
          HashSet<IdentityDescriptor> source1 = new HashSet<IdentityDescriptor>((IEnumerable<IdentityDescriptor>) identityDescriptors2, (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
          source1.ExceptWith((IEnumerable<IdentityDescriptor>) identityDescriptors1);
          List<Microsoft.VisualStudio.Services.Identity.Identity> source2 = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
          if (identityDescriptorSet.Count > 0)
          {
            foreach (IdentityDescriptor descriptor in identityDescriptorSet)
            {
              Microsoft.VisualStudio.Services.Identity.Identity identity3 = this.ReadIdentityFromSource(requestContext, descriptor, false);
              if (identity3 != null && identity3.IsActive)
              {
                source2.Add(identity3);
                if (identity3.IsContainer)
                  this.m_newGroups.Add(identity3);
              }
            }
            this.m_identityStore.UpdateIdentities(requestContext, this.m_identityStore.Domain, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) source2, false);
          }
          if (source2.Count <= 0)
          {
            if (source1.Count <= 0)
              goto label_16;
          }
          IdentityServiceBase.Trace(TraceLevel.Info, "Now submitting group {0}", (object) groupToSync.DisplayName);
          List<Tuple<IdentityDescriptor, IdentityDescriptor, bool>> updates = new List<Tuple<IdentityDescriptor, IdentityDescriptor, bool>>(identityDescriptorSet.Count + source1.Count);
          updates.AddRange(source2.Select<Microsoft.VisualStudio.Services.Identity.Identity, Tuple<IdentityDescriptor, IdentityDescriptor, bool>>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Tuple<IdentityDescriptor, IdentityDescriptor, bool>>) (member => new Tuple<IdentityDescriptor, IdentityDescriptor, bool>(groupToSync.Descriptor, member.Descriptor, true))));
          updates.AddRange(source1.Select<IdentityDescriptor, Tuple<IdentityDescriptor, IdentityDescriptor, bool>>((Func<IdentityDescriptor, Tuple<IdentityDescriptor, IdentityDescriptor, bool>>) (member => new Tuple<IdentityDescriptor, IdentityDescriptor, bool>(groupToSync.Descriptor, member, false))));
          this.m_identityStore.UpdateIdentityMembership(requestContext, this.m_identityStore.Domain, true, (IList<Tuple<IdentityDescriptor, IdentityDescriptor, bool>>) updates);
          IdentityServiceBase.Trace(TraceLevel.Info, "Finished processing membership changes in {0}", (object) groupToSync.DisplayName);
        }
      }
      catch (Exception ex)
      {
        this.m_syncErrors.Add(groupToSync.DisplayName, ex);
        Microsoft.VisualStudio.Services.Identity.Identity identity = this.ReadIdentityFromDatabase(requestContext, groupToSync.Descriptor, true);
        this.EnqueueMemberGroups(requestContext, identity);
      }
label_16:
      this.LogErrors(requestContext);
      IdentityServiceBase.Trace(TraceLevel.Info, "Finished Membership sync for {0}", (object) groupToSync.DisplayName);
      this.m_totalErrors += this.m_syncErrors.Count;
      if (this.m_totalErrors > this.m_errorLimit)
        throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Aborting sync due to too many errors (exceeded {0})", (object) this.m_errorLimit));
    }

    public virtual void SyncPropertiesPartial(
      IVssRequestContext requestContext,
      DateTime syncCycleStart,
      TimeSpan syncCycleDuration,
      int syncsPerCycle)
    {
      List<IdentityDescriptor> groupsToSync = this.m_identityStore.GetGroupsToSync(requestContext, this.m_identityStore.Domain, syncCycleStart, out int _);
      if (groupsToSync.Count <= 0)
        return;
      Stopwatch stopwatch = new Stopwatch();
      TimeSpan timeSpan = syncCycleDuration - (DateTime.UtcNow - syncCycleStart);
      int num = (int) ((timeSpan < new TimeSpan() ? new TimeSpan() : timeSpan).Ticks * (long) syncsPerCycle / syncCycleDuration.Ticks) + 1;
      int count = (groupsToSync.Count - 1) / num + 1;
      requestContext.Trace(9251, TraceLevel.Info, "Identity", nameof (IdentitySynchronizer), string.Format("Sync start: {0}, sync duration: {1}, syncs left: {2}. ", (object) syncCycleStart, (object) syncCycleDuration, (object) num) + string.Format("Groups left to sync: {0}, groups targeted in this sync {1}.", (object) groupsToSync.Count, (object) count));
      foreach (IdentityDescriptor descriptor in groupsToSync.Take<IdentityDescriptor>(count))
      {
        stopwatch.Restart();
        Microsoft.VisualStudio.Services.Identity.Identity rootIdentity = this.ReadIdentityFromDatabase(requestContext, descriptor, false);
        if (rootIdentity != null && rootIdentity.IsActive)
        {
          this.SyncIdentity(requestContext, rootIdentity, false, false);
          requestContext.Trace(9252, TraceLevel.Info, "Identity", nameof (IdentitySynchronizer), string.Format("Synced {0} ({1}). Time to sync group: {2}.", (object) rootIdentity.DisplayName, (object) rootIdentity.Id, (object) stopwatch.Elapsed));
        }
      }
      this.LogErrorsSummary(requestContext);
    }

    protected virtual void SyncOneUser(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity idToSync)
    {
      try
      {
        this.m_identityStore.UpdateIdentities(requestContext, this.m_identityStore.Domain, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
        {
          idToSync
        }, false);
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException((IVssRequestContext) null, nameof (SyncOneUser), ex, TeamFoundationEventId.ActiveDirectorySyncErrors, EventLogEntryType.Error);
        this.m_logger.Error(ex);
      }
    }

    protected virtual Microsoft.VisualStudio.Services.Identity.Identity ReadIdentityFromSource(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      bool includeMemberships)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity;
      if (this.m_syncAgents[descriptor.IdentityType].TrySyncIdentity(requestContext, descriptor, includeMemberships, (string) null, this.m_syncErrors, out identity))
        return identity;
      throw new NotSupportedException();
    }

    protected virtual Microsoft.VisualStudio.Services.Identity.Identity ReadIdentityFromSource(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      bool includeMemberships)
    {
      string property = identity.GetProperty<string>("DN", string.Empty);
      Microsoft.VisualStudio.Services.Identity.Identity identity1;
      if (this.m_syncAgents[identity.Descriptor.IdentityType].TrySyncIdentity(requestContext, identity.Descriptor, includeMemberships, property, this.m_syncErrors, out identity1))
        return identity1;
      throw new NotSupportedException();
    }

    protected virtual Microsoft.VisualStudio.Services.Identity.Identity ReadIdentityFromDatabase(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      bool includeMemberships,
      bool includeInactivatedMembers = true)
    {
      return this.m_identityStore.ReadIdentityFromDatabase(requestContext, this.m_identityStore.Domain, descriptor, QueryMembership.None, includeMemberships ? QueryMembership.Direct : QueryMembership.None, true, includeInactivatedMembers, true, (SequenceContext) null);
    }

    protected virtual void EnqueueMemberGroups(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (identity?.Members == null)
        return;
      foreach (IdentityDescriptor member in (IEnumerable<IdentityDescriptor>) identity.Members)
      {
        if (IdentityValidation.IsTeamFoundationType(member) || this.m_knownGroups != null && this.m_knownGroups.Contains(member))
          this.m_groupsToSync.Enqueue(this.ReadIdentityFromDatabase(requestContext, member, false));
      }
    }

    private void LogErrors(IVssRequestContext requestContext)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.SyncErrors.DisableLogErrorsSummary"))
        requestContext.Trace(9250, TraceLevel.Info, "Identity", nameof (IdentitySynchronizer), "The feature flag VisualStudio.Services.Identity.SyncErrors.DisableLogErrorsSummary is disabled.");
      else
        this.m_syncErrors.LogErrors();
    }

    private void LogErrorsSummary(IVssRequestContext requestContext)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.SyncErrors.DisableLogErrorsSummary"))
        requestContext.Trace(9255, TraceLevel.Info, "Identity", nameof (IdentitySynchronizer), "The feature flag VisualStudio.Services.Identity.SyncErrors.DisableLogErrorsSummary is enabled.");
      else
        this.m_syncErrors.LogErrorsSummary();
    }

    public Dictionary<string, SyncCounter> SyncCounters { get; } = new Dictionary<string, SyncCounter>();
  }
}
