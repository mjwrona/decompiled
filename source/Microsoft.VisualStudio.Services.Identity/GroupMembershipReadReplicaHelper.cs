// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupMembershipReadReplicaHelper
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class GroupMembershipReadReplicaHelper
  {
    private const string Area = "IdentityService";
    private const string Layer = "GroupMembershipReadReplicaHelper";
    public const string SQLReadOnlyReplicationConfig = "GroupManagement.SQLReadOnlyReplicationConfig";
    private static readonly IConfigPrototype<GroupMembershipReadReplicaHelper.ReadOnlyReplicaSettings> configPrototype = ConfigPrototype.Create<GroupMembershipReadReplicaHelper.ReadOnlyReplicaSettings>("GroupManagement.SQLReadOnlyReplicationConfig", new GroupMembershipReadReplicaHelper.ReadOnlyReplicaSettings());
    private readonly IConfigQueryable<GroupMembershipReadReplicaHelper.ReadOnlyReplicaSettings> config;

    public GroupMembershipReadReplicaHelper()
      : this(ConfigProxy.Create<GroupMembershipReadReplicaHelper.ReadOnlyReplicaSettings>(GroupMembershipReadReplicaHelper.configPrototype))
    {
    }

    public GroupMembershipReadReplicaHelper(
      IConfigQueryable<GroupMembershipReadReplicaHelper.ReadOnlyReplicaSettings> config)
    {
      this.config = config;
    }

    internal bool CanReadFromReplica(
      IVssRequestContext context,
      QueryMembership childrenQuery,
      QueryMembership parentsQuery,
      SequenceContext minSequenceContext)
    {
      GroupMembershipReadReplicaHelper.ReadOnlyReplicaSettings settings = this.config.QueryByCtx<GroupMembershipReadReplicaHelper.ReadOnlyReplicaSettings>(context);
      bool flag1 = minSequenceContext != null;
      bool flag2 = !context.ExecutionEnvironment.IsOnPremisesDeployment;
      bool flag3 = !context.ServiceHost.Is(TeamFoundationHostType.Deployment);
      bool componentEnabled = settings.ReadGroupMembershipComponentEnabled;
      bool flag4 = this.ReadOnlyReplicaReadEnabledForFlow(context);
      bool flag5 = flag1 & flag2 & flag3 & componentEnabled & flag4;
      bool flag6 = this.CanReadFromReplicaForQuery(parentsQuery, true, settings);
      bool flag7 = this.CanReadFromReplicaForQuery(childrenQuery, false, settings);
      bool flag8 = DateTimeOffset.UtcNow.Ticks % 100L < (long) settings.PercentOfRequestsToROReplica;
      context.Trace(2432351, TraceLevel.Info, "IdentityService", nameof (GroupMembershipReadReplicaHelper), string.Format("canReadFromReplica={0}; ", (object) flag5) + string.Format("sequenceContextIsNotNull={0};", (object) flag1) + string.Format("isHostedEnvironment={0};", (object) flag2) + string.Format("isCollectionLevel={0};", (object) flag3) + string.Format("isReadGroupMembershipComponentEnabled={0};", (object) componentEnabled) + string.Format("isReadOnlyReplicaEnabledForFlow={0};", (object) flag4) + string.Format("canReadFromReplicaForParentQuery={0};", (object) flag6) + string.Format("canReadFromReplicaForChildrenQuery={0};", (object) flag7) + string.Format("willTrafficGoToReadReplica={0}", (object) flag8));
      return ((!flag5 ? 0 : (flag6 | flag7 ? 1 : 0)) & (flag8 ? 1 : 0)) != 0;
    }

    private bool CanReadFromReplicaForQuery(
      QueryMembership query,
      bool parentQuery,
      GroupMembershipReadReplicaHelper.ReadOnlyReplicaSettings settings)
    {
      bool membershipNoneEnabled = settings.QueryMembershipNoneEnabled;
      bool membershipDirectEnabled = settings.QueryMembershipDirectEnabled;
      bool expandedUpEnabled = settings.QueryMembershipExpandedUpEnabled;
      bool expandedDownEnabled = settings.QueryMembershipExpandedDownEnabled;
      bool flag = false;
      switch (query)
      {
        case QueryMembership.None:
          flag = membershipNoneEnabled;
          break;
        case QueryMembership.Direct:
          flag = membershipDirectEnabled;
          break;
        case QueryMembership.Expanded:
          flag = parentQuery ? expandedUpEnabled : expandedDownEnabled;
          break;
        case QueryMembership.ExpandedUp:
          flag = expandedUpEnabled;
          break;
        case QueryMembership.ExpandedDown:
          flag = expandedDownEnabled;
          break;
      }
      return flag;
    }

    private bool ReadOnlyReplicaReadEnabledForFlow(IVssRequestContext context)
    {
      bool flag;
      return context.TryGetItem<bool>(RequestContextItemsKeys.ReadOnlyReplicaReadEnabled, out flag) && flag;
    }

    internal class ReadOnlyReplicaSettings
    {
      public bool QueryMembershipNoneEnabled { get; set; }

      public bool QueryMembershipDirectEnabled { get; set; }

      public bool QueryMembershipExpandedUpEnabled { get; set; }

      public bool QueryMembershipExpandedDownEnabled { get; set; }

      public bool ReadGroupMembershipComponentEnabled { get; set; }

      public int PercentOfRequestsToROReplica { get; set; }
    }
  }
}
