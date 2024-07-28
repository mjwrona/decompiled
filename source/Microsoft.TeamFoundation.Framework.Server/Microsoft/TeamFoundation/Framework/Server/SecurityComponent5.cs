// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityComponent5
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecurityComponent5 : SecurityComponent4
  {
    public override ResultCollection RemoveIdentityACEs(
      Guid namespaceId,
      IEnumerable<Guid> teamFoundationIds)
    {
      this.PrepareStoredProcedure("prc_RemoveIdentityACEs");
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindGuidTable("@identityList", teamFoundationIds);
      this.BindGuid("@writerIdentifier", this.Author);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_RemoveIdentityACEs", this.RequestContext);
      resultCollection.AddBinder<int>((ObjectBinder<int>) new SecurityComponent.ChangeIdColumns());
      resultCollection.AddBinder<RemovedAccessControlEntry>((ObjectBinder<RemovedAccessControlEntry>) new SecurityComponent5.RemovedAccessControlEntryColumn());
      return resultCollection;
    }

    protected class RemovedAccessControlEntryColumn : ObjectBinder<RemovedAccessControlEntry>
    {
      private SqlColumnBinder tokenColumn = new SqlColumnBinder("SecurityToken");
      private SqlColumnBinder teamFoundationIdColumn = new SqlColumnBinder("TeamFoundationId");

      protected override RemovedAccessControlEntry Bind() => new RemovedAccessControlEntry(this.tokenColumn.GetString((IDataReader) this.Reader, true), this.teamFoundationIdColumn.GetGuid((IDataReader) this.Reader));
    }
  }
}
