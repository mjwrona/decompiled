// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupComponent13
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class GroupComponent13 : GroupComponent12
  {
    public override ResultCollection ReadSnapshot(
      Guid scopeId,
      bool readAllIdentities,
      bool includeInactiveMemberships = false)
    {
      try
      {
        this.TraceEnter(4703130, nameof (ReadSnapshot));
        this.PrepareStoredProcedure("prc_ReadGroupSnapshot");
        this.BindGuid("@scopeId", scopeId);
        this.BindBoolean("@readAllIdentities", readAllIdentities);
        this.BindBoolean("@includeInactiveMemberships", includeInactiveMemberships);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<IdentityScope>((ObjectBinder<IdentityScope>) new GroupComponent.IdentityScopeColumns2());
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.Identity.Identity>((ObjectBinder<Microsoft.VisualStudio.Services.Identity.Identity>) this.GetGroupIdentityColumns());
        resultCollection.AddBinder<GroupMembership>((ObjectBinder<GroupMembership>) new GroupComponent.ChildrenColumns2());
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new GroupComponent.IdentityIdColumns());
        return resultCollection;
      }
      finally
      {
        this.TraceLeave(4703139, nameof (ReadSnapshot));
      }
    }
  }
}
