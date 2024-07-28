// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.Components.TaggingComponent11
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.Azure.Devops.Tags.Server.Components
{
  internal class TaggingComponent11 : TaggingComponent10
  {
    internal void CreateTagDefinitionExtendedCharSupport(TagDefinition tag)
    {
      this.PrepareStoredProcedure("prc_CreateTagDefinitionExtendedCharSupport");
      this.BindDataspace(tag.Scope);
      this.BindGuid("@tagId", tag.TagId);
      this.BindString("@tagName", tag.Name, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindBoolean("@includesAllArtifactKinds", tag.IncludesAllArtifactKinds);
      this.BindGuidTable("@applicableArtifactKinds", (IEnumerable<Guid>) ((object) tag.ApplicableKindIds ?? (object) Array.Empty<Guid>()));
      this.BindNullableGuid("@projectScope", tag.Scope);
      this.BindInt("@tagStatus", (int) tag.Status);
      this.ExecuteNonQuery();
    }
  }
}
