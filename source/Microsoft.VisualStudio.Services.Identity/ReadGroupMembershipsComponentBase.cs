// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ReadGroupMembershipsComponentBase
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal abstract class ReadGroupMembershipsComponentBase : TeamFoundationSqlResourceComponent
  {
    public abstract ResultCollection ReadMemberships(
      IEnumerable<Guid> identityIds,
      QueryMembership childrenQuery,
      QueryMembership parentsQuery,
      bool includeRestricted,
      int? minInactivatedTime,
      Guid scopeId,
      bool returnVisibleIdentities,
      bool enableUseXtpProc = false,
      long minSequenceId = -1,
      bool inScopeMembershipsOnly = false);

    public IEnumerable<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData> ReadScopeVisiblity(
      IEnumerable<Guid> identityIds,
      Guid scopeId,
      bool enableUseXtpProc = false)
    {
      using (ResultCollection resultCollection = this.ReadMemberships(identityIds, QueryMembership.None, QueryMembership.None, false, new int?(), scopeId, true, enableUseXtpProc))
      {
        resultCollection.NextResult();
        resultCollection.NextResult();
        return (IEnumerable<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>) resultCollection.GetCurrent<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>().Items;
      }
    }

    protected class ScopeFilteredIdentitiesColumns : 
      ObjectBinder<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>
    {
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder Active = new SqlColumnBinder(nameof (Active));

      protected override ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData Bind() => new ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData()
      {
        IdentityId = this.Id.GetGuid((IDataReader) this.Reader),
        Active = this.Active.GetBoolean((IDataReader) this.Reader)
      };
    }

    internal class ScopeFilteredIdentityData
    {
      public Guid IdentityId { get; set; }

      public bool Active { get; set; }
    }

    protected class ParentColumns : ObjectBinder<GroupMembership>
    {
      private SqlColumnBinder QueriedId = new SqlColumnBinder(nameof (QueriedId));
      private SqlColumnBinder ParentId = new SqlColumnBinder(nameof (ParentId));
      private SqlColumnBinder Sid = new SqlColumnBinder(nameof (Sid));
      private SqlColumnBinder IdType = new SqlColumnBinder("Type");

      protected override GroupMembership Bind()
      {
        Guid guid1 = this.QueriedId.GetGuid((IDataReader) this.Reader);
        Guid guid2 = this.ParentId.GetGuid((IDataReader) this.Reader);
        string identifier = this.Sid.GetString((IDataReader) this.Reader, false);
        IdentityDescriptor identityDescriptor = new IdentityDescriptor(this.IdType.GetString((IDataReader) this.Reader, false), identifier);
        Guid id = guid2;
        IdentityDescriptor descriptor = identityDescriptor;
        return new GroupMembership(guid1, id, descriptor);
      }
    }

    protected class ChildrenColumns : ObjectBinder<GroupMembership>
    {
      private SqlColumnBinder QueriedId = new SqlColumnBinder(nameof (QueriedId));
      private SqlColumnBinder ChildId = new SqlColumnBinder(nameof (ChildId));
      private SqlColumnBinder Sid = new SqlColumnBinder(nameof (Sid));
      private SqlColumnBinder IdType = new SqlColumnBinder("Type");

      protected override GroupMembership Bind()
      {
        Guid guid1 = this.QueriedId.GetGuid((IDataReader) this.Reader);
        Guid guid2 = this.ChildId.GetGuid((IDataReader) this.Reader);
        string identifier = this.Sid.GetString((IDataReader) this.Reader, true);
        string identityType = this.IdType.GetString((IDataReader) this.Reader, true);
        IdentityDescriptor identityDescriptor = identifier != null ? new IdentityDescriptor(identityType, identifier) : (IdentityDescriptor) null;
        Guid id = guid2;
        IdentityDescriptor descriptor = identityDescriptor;
        return new GroupMembership(guid1, id, descriptor);
      }
    }

    internal class MinGroupSequenceIdException : IdentityServiceException
    {
      public MinGroupSequenceIdException(
        string currentGroupSequenceId,
        string expectedMinGroupSequenceId)
        : base(HostingResources.MinGroupSequenceIdError((object) currentGroupSequenceId, (object) expectedMinGroupSequenceId))
      {
      }

      public MinGroupSequenceIdException(string message, Exception innerException)
        : base(message, innerException)
      {
      }
    }

    internal class UpdateGroupScopeVisibilityException : IdentityServiceException
    {
      public UpdateGroupScopeVisibilityException(string scopeId, string updatedId)
        : base(HostingResources.UpdateGroupScopeVisibilityError((object) scopeId, (object) updatedId))
      {
      }

      public UpdateGroupScopeVisibilityException(string message, Exception innerException)
        : base(message, innerException)
      {
      }
    }

    internal class RestoreGroupScopeExecutionException : IdentityServiceException
    {
      public RestoreGroupScopeExecutionException(string scopeId, string failure, string status)
        : base(HostingResources.RestoreGroupScopeExecutionError((object) scopeId, (object) failure, (object) status))
      {
      }

      public RestoreGroupScopeExecutionException(string message, Exception innerException)
        : base(message, innerException)
      {
      }
    }
  }
}
