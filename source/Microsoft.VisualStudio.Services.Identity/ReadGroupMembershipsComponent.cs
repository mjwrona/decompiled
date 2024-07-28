// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ReadGroupMembershipsComponent
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class ReadGroupMembershipsComponent : ReadGroupMembershipsComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[14]
    {
      (IComponentCreator) new ComponentCreator<ReadGroupMembershipsComponent>(1),
      (IComponentCreator) new ComponentCreator<ReadGroupMembershipComponent2>(13),
      (IComponentCreator) new ComponentCreator<ReadGroupMembershipComponent2>(14),
      (IComponentCreator) new ComponentCreator<ReadGroupMembershipComponent2>(15),
      (IComponentCreator) new ComponentCreator<ReadGroupMembershipComponent3>(16),
      (IComponentCreator) new ComponentCreator<ReadGroupMembershipComponent4>(17),
      (IComponentCreator) new ComponentCreator<ReadGroupMembershipComponent4>(18),
      (IComponentCreator) new ComponentCreator<ReadGroupMembershipComponent4>(19),
      (IComponentCreator) new ComponentCreator<ReadGroupMembershipComponent4>(20),
      (IComponentCreator) new ComponentCreator<ReadGroupMembershipComponent4>(21),
      (IComponentCreator) new ComponentCreator<ReadGroupMembershipComponent4>(22),
      (IComponentCreator) new ComponentCreator<ReadGroupMembershipComponent4>(23),
      (IComponentCreator) new ComponentCreator<ReadGroupMembershipComponent5>(24),
      (IComponentCreator) new ComponentCreator<ReadGroupMembershipComponent6>(25)
    }, "Group");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    private static readonly string s_componentName = nameof (ReadGroupMembershipsComponent);

    static ReadGroupMembershipsComponent() => ReadGroupMembershipsComponent.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        400025,
        new SqlExceptionFactory(typeof (GroupScopeDoesNotExistException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new GroupScopeDoesNotExistException(sqEr.ExtractString("project_uri"))))
      },
      {
        400105,
        new SqlExceptionFactory(typeof (ReadGroupMembershipsComponentBase.MinGroupSequenceIdException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ReadGroupMembershipsComponentBase.MinGroupSequenceIdException(sqEr.ExtractString("current_groupSequenceId"), sqEr.ExtractString("min_groupSequenceId"))))
      }
    };

    public ReadGroupMembershipsComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) ReadGroupMembershipsComponent.s_sqlExceptionFactories;

    protected override void SetupComponentCircuitBreaker(
      IVssRequestContext requestContext,
      string databaseName,
      ApplicationIntent applicationIntent)
    {
      string key = ReadGroupMembershipsComponent.s_componentName + "-" + databaseName + "-" + DatabaseResourceComponent.ToString(applicationIntent);
      this.ComponentLevelCommandSetter.AndCommandKey((Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) key);
      this.ComponentLevelCircuitBreakerProperties = (ICommandProperties) new CommandPropertiesRegistry(requestContext, (Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) key, this.ComponentLevelCommandSetter.CommandPropertiesDefaults);
    }

    public override ResultCollection ReadMemberships(
      IEnumerable<Guid> identityIds,
      QueryMembership childrenQuery,
      QueryMembership parentsQuery,
      bool includeRestricted,
      int? minInactivatedTime,
      Guid scopeId,
      bool returnVisibleIdentities,
      bool useXtpProc = false,
      long minSequenceId = -1,
      bool inScopeMembershipsOnly = false)
    {
      try
      {
        this.TraceEnter(4703111, nameof (ReadMemberships));
        if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
          this.PrepareStoredProcedure("prc_ReadGroupMembership2");
        else
          this.PrepareStoredProcedure("prc_ReadGroupMembership");
        this.BindGuidTable("@identityIds", identityIds);
        this.BindInt("@childrenQuery", (int) childrenQuery);
        this.BindInt("@parentsQuery", (int) parentsQuery);
        this.BindBoolean("@includeRestricted", includeRestricted);
        this.BindNullableInt("@minInactivatedTime", minInactivatedTime.GetValueOrDefault(), 0);
        this.BindGuid("@scopeId", scopeId);
        this.BindBoolean("@returnVisibleIdentities", returnVisibleIdentities);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new ReadGroupMembershipsComponentBase.ParentColumns());
        resultCollection.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new ReadGroupMembershipsComponentBase.ChildrenColumns());
        if (returnVisibleIdentities)
          resultCollection.AddBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>((ObjectBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>) new ReadGroupMembershipsComponentBase.ScopeFilteredIdentitiesColumns());
        return resultCollection;
      }
      finally
      {
        this.TraceLeave(4703118, nameof (ReadMemberships));
      }
    }
  }
}
