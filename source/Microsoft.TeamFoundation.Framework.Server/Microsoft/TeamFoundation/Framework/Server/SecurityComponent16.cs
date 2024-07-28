// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityComponent16
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecurityComponent16 : SecurityComponent15
  {
    public override void DeleteSecurityNamespace(Guid namespaceId, bool deleteAces = true)
    {
      this.PrepareStoredProcedure("prc_DeleteSecurityNamespace");
      this.BindGuid("@namespaceGuid", namespaceId);
      this.BindGuid("@writerIdentifier", this.Author);
      this.BindBoolean("@deleteAces", deleteAces);
      this.ExecuteNonQuery();
    }

    public override IEnumerable<Guid> RemoveDataspacedACEs(
      IEnumerable<Tuple<Guid, string>> namespaces,
      Guid dataspaceIdentifier)
    {
      this.PrepareStoredProcedure("prc_RemoveACEsByDataspaceIdentifier");
      this.BindGuidStringTable("@namespaces", namespaces);
      this.BindGuid("@dataspaceIdentifier", dataspaceIdentifier);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_RemoveACEsByDataspaceIdentifier", this.RequestContext))
      {
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new SecurityComponent16.RemovedACEsNamespaceIdColumn());
        return (IEnumerable<Guid>) resultCollection.GetCurrent<Guid>().Items;
      }
    }

    protected class RemovedACEsNamespaceIdColumn : ObjectBinder<Guid>
    {
      private SqlColumnBinder namespaceIdColumn = new SqlColumnBinder("NamespaceId");

      protected override Guid Bind() => this.namespaceIdColumn.GetGuid((IDataReader) this.Reader);
    }
  }
}
