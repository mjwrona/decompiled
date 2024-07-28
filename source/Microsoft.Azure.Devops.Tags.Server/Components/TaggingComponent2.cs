// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.Components.TaggingComponent2
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Devops.Tags.Server.Components
{
  internal class TaggingComponent2 : TaggingComponent
  {
    internal override void CreateTagDefinition(TagDefinition tag)
    {
      this.PrepareStoredProcedure("prc_CreateTagDefinition");
      this.BindDataspace(tag.Scope);
      this.BindGuid("@tagId", tag.TagId);
      this.BindString("@tagName", tag.Name, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindBoolean("@includesAllArtifactKinds", tag.IncludesAllArtifactKinds);
      this.BindGuidTable("@applicableArtifactKinds", (IEnumerable<Guid>) ((object) tag.ApplicableKindIds ?? (object) Array.Empty<Guid>()));
      this.BindNullableGuid("@projectScope", tag.Scope);
      this.ExecuteNonQuery();
    }

    internal override TagDefinition GetTagDefinitionByName(string name, Guid scope)
    {
      this.PrepareStoredProcedure("prc_GetTagDefinitionByName");
      this.BindString("@name", name, 400, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindNullableGuid("@projectScope", scope);
      return this.ReadTagDefinitions(this.RequestContext, this.ProcedureName, this.ExecuteReader()).FirstOrDefault<TagDefinition>();
    }

    protected override TaggingComponent.TagDefinitionRowBinder GetTagDefinitionRowBinder() => (TaggingComponent.TagDefinitionRowBinder) new TaggingComponent2.TagDefinitionRowBinder2();

    internal class TagDefinitionRowBinder2 : TaggingComponent.TagDefinitionRowBinder
    {
      private SqlColumnBinder scopeColumn = new SqlColumnBinder("ProjectScope");

      protected override TaggingComponent.TagDefinitionRow Bind()
      {
        TaggingComponent.TagDefinitionRow tagDefinitionRow = base.Bind();
        tagDefinitionRow.Scope = this.scopeColumn.GetGuid((IDataReader) this.Reader, true);
        return tagDefinitionRow;
      }
    }
  }
}
