// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.Components.TaggingComponent
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.Azure.Devops.Tags.Server.Components
{
  internal class TaggingComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[11]
    {
      (IComponentCreator) new ComponentCreator<TaggingComponent>(1, true),
      (IComponentCreator) new ComponentCreator<TaggingComponent2>(2),
      (IComponentCreator) new ComponentCreator<TaggingComponent3>(3),
      (IComponentCreator) new ComponentCreator<TaggingComponent4>(4),
      (IComponentCreator) new ComponentCreator<TaggingComponent5>(5),
      (IComponentCreator) new ComponentCreator<TaggingComponent6>(6),
      (IComponentCreator) new ComponentCreator<TaggingComponent7>(7),
      (IComponentCreator) new ComponentCreator<TaggingComponent8>(8),
      (IComponentCreator) new ComponentCreator<TaggingComponent9>(9),
      (IComponentCreator) new ComponentCreator<TaggingComponent10>(10),
      (IComponentCreator) new ComponentCreator<TaggingComponent11>(11)
    }, "Tagging");
    private static readonly Dictionary<int, SqlExceptionFactory> s_exceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        800085,
        new SqlExceptionFactory(typeof (DuplicateTagNameException))
      },
      {
        800086,
        new SqlExceptionFactory(typeof (InvalidTagNameException))
      }
    };
    protected const string s_area = "TeamFoundationTaggingService";
    protected const string s_layer = "Component";

    public TaggingComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    protected virtual void BindDataspace(Guid dataspaceIdentifier)
    {
    }

    internal virtual bool SupportsPermissions => false;

    internal virtual bool SupportsGetTaggedArtifactsFunctions => false;

    internal virtual void CleanUnusedTagDefinitions(DateTime cutoffTime) => throw new NotSupportedException();

    internal virtual void CleanUnusedTagDefinitions(DateTime cutoffTime, bool deleteUnusedTags) => throw new NotSupportedException();

    internal virtual void CreateTagDefinition(TagDefinition tag)
    {
      this.PrepareStoredProcedure("prc_CreateTagDefinition");
      this.BindGuid("@tagId", tag.TagId);
      this.BindString("@tagName", tag.Name, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindBoolean("@includesAllArtifactKinds", tag.IncludesAllArtifactKinds);
      this.BindGuidTable("@applicableArtifactKinds", (IEnumerable<Guid>) ((object) tag.ApplicableKindIds ?? (object) Array.Empty<Guid>()));
      this.ExecuteNonQuery();
    }

    internal virtual TagDefinition GetTagDefinitionById(Guid tagId)
    {
      this.PrepareStoredProcedure("prc_GetTagDefinitionById");
      this.BindGuid("@tagId", tagId);
      return this.ReadTagDefinitions(this.RequestContext, this.ProcedureName, this.ExecuteReader()).FirstOrDefault<TagDefinition>();
    }

    internal virtual IEnumerable<TagDefinition> GetTagDefinitionsById(IEnumerable<Guid> tagIds)
    {
      this.PrepareStoredProcedure("prc_GetTagDefinitionsById");
      this.BindGuidTable("@tagIds", tagIds);
      return this.ReadTagDefinitions(this.RequestContext, this.ProcedureName, this.ExecuteReader());
    }

    internal IEnumerable<TagDefinition> GetTagDefinitionsByTimestamp(byte[] timestamp)
    {
      this.PrepareStoredProcedure("prc_GetTagDefinitionsByTimestamp");
      this.BindBinary("@timestamp", timestamp, 8, SqlDbType.Binary);
      return this.ReadTagDefinitions(this.RequestContext, this.ProcedureName, this.ExecuteReader());
    }

    internal virtual TagDefinition GetTagDefinitionByName(string name, Guid scope)
    {
      this.PrepareStoredProcedure("prc_GetTagDefinitionByName");
      this.BindString("@name", name, 400, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      if (scope != Guid.Empty)
        throw new NotSupportedException();
      return this.ReadTagDefinitions(this.RequestContext, this.ProcedureName, this.ExecuteReader()).FirstOrDefault<TagDefinition>();
    }

    internal virtual TagDefinition UpdateTagDefinition(TagDefinition tag)
    {
      bool flag = tag.Status == TagDefinitionStatus.Inactive;
      if (flag)
      {
        this.PrepareStoredProcedure("prc_MarkTagDefinitionAsDeleted");
        this.BindGuid("@tagId", tag.TagId);
        this.ExecuteNonQuery();
      }
      this.PrepareStoredProcedure("prc_RenameTagDefinition");
      this.BindGuid("@tagId", tag.TagId);
      this.BindString("@newName", tag.Name, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindBoolean("@restoreStatusToNormal", !flag);
      return this.ReadTagDefinitions(this.RequestContext, this.ProcedureName, this.ExecuteReader()).FirstOrDefault<TagDefinition>();
    }

    protected virtual TaggingComponent.TagDefinitionRowBinder GetTagDefinitionRowBinder() => new TaggingComponent.TagDefinitionRowBinder();

    protected IEnumerable<TagDefinition> ReadTagDefinitions(
      IVssRequestContext requestContext,
      string storedProcedureName,
      SqlDataReader reader)
    {
      ResultCollection resultCollection = new ResultCollection((IDataReader) reader, storedProcedureName, requestContext);
      resultCollection.AddBinder<TaggingComponent.TagDefinitionRow>((ObjectBinder<TaggingComponent.TagDefinitionRow>) this.GetTagDefinitionRowBinder());
      resultCollection.AddBinder<TaggingComponent.ApplicabilityRow>((ObjectBinder<TaggingComponent.ApplicabilityRow>) new TaggingComponent.ApplicabilityRowBinder());
      Dictionary<Guid, TaggingComponent.TagDefinitionRow> dictionary = resultCollection.GetCurrent<TaggingComponent.TagDefinitionRow>().ToDictionary<TaggingComponent.TagDefinitionRow, Guid>((System.Func<TaggingComponent.TagDefinitionRow, Guid>) (row => row.TagId));
      if (!dictionary.Any<KeyValuePair<Guid, TaggingComponent.TagDefinitionRow>>())
        return Enumerable.Empty<TagDefinition>();
      resultCollection.NextResult();
      foreach (TaggingComponent.ApplicabilityRow applicabilityRow in resultCollection.GetCurrent<TaggingComponent.ApplicabilityRow>())
      {
        TaggingComponent.TagDefinitionRow tagDefinitionRow = (TaggingComponent.TagDefinitionRow) null;
        dictionary.TryGetValue(applicabilityRow.TagId, out tagDefinitionRow);
        if (tagDefinitionRow != null && !tagDefinitionRow.IncludesAllArtifactKinds)
          tagDefinitionRow.ApplicableKindIds.Add(applicabilityRow.ArtifactKindId);
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return (IEnumerable<TagDefinition>) dictionary.Values.Select<TaggingComponent.TagDefinitionRow, TagDefinition>(TaggingComponent.\u003C\u003EO.\u003C0\u003E__PopulateTagDefinition ?? (TaggingComponent.\u003C\u003EO.\u003C0\u003E__PopulateTagDefinition = new System.Func<TaggingComponent.TagDefinitionRow, TagDefinition>(TaggingComponent.PopulateTagDefinition))).ToArray<TagDefinition>();
    }

    protected static TagDefinition PopulateTagDefinition(TaggingComponent.TagDefinitionRow row) => row.IncludesAllArtifactKinds ? new TagDefinition(row.TagId, row.Name, (IEnumerable<Guid>) null, true, row.Scope, row.Status, row.LastUpdated) : new TagDefinition(row.TagId, row.Name, (IEnumerable<Guid>) row.ApplicableKindIds.ToList<Guid>().AsReadOnly(), false, row.Scope, row.Status, row.LastUpdated);

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) TaggingComponent.s_exceptionFactories;

    internal virtual void DeleteTagDefinitions(IEnumerable<Guid> tagIds) => throw new NotSupportedException();

    internal class ApplicabilityRow
    {
      internal Guid TagId { get; set; }

      internal Guid ArtifactKindId { get; set; }
    }

    internal class ApplicabilityRowBinder : ObjectBinder<TaggingComponent.ApplicabilityRow>
    {
      private SqlColumnBinder tagIdColumn = new SqlColumnBinder("TagId");
      private SqlColumnBinder artifactKindIdColumn = new SqlColumnBinder("ArtifactKindId");

      protected override TaggingComponent.ApplicabilityRow Bind() => new TaggingComponent.ApplicabilityRow()
      {
        TagId = this.tagIdColumn.GetGuid((IDataReader) this.Reader),
        ArtifactKindId = this.artifactKindIdColumn.GetGuid((IDataReader) this.Reader)
      };
    }

    internal class TagDefinitionRow
    {
      internal Guid TagId { get; set; }

      internal string Name { get; set; }

      internal ICollection<Guid> ApplicableKindIds { get; set; }

      internal bool IncludesAllArtifactKinds { get; set; }

      internal Guid Scope { get; set; }

      internal TagDefinitionStatus Status { get; set; }

      internal DateTime LastUpdated { get; set; }
    }

    internal class TagDefinitionRowBinder : ObjectBinder<TaggingComponent.TagDefinitionRow>
    {
      private SqlColumnBinder tagIdColumn = new SqlColumnBinder("TagId");
      private SqlColumnBinder nameColumn = new SqlColumnBinder("Name");
      private SqlColumnBinder includesAllArtifactKindsColumn = new SqlColumnBinder("IncludesAllArtifactKinds");
      private SqlColumnBinder statusColumn = new SqlColumnBinder("Status");
      private SqlColumnBinder lastUpdatedColumn = new SqlColumnBinder("LastUpdated");

      protected override TaggingComponent.TagDefinitionRow Bind() => new TaggingComponent.TagDefinitionRow()
      {
        TagId = this.tagIdColumn.GetGuid((IDataReader) this.Reader),
        Name = this.nameColumn.GetString((IDataReader) this.Reader, false),
        ApplicableKindIds = (ICollection<Guid>) new List<Guid>(),
        IncludesAllArtifactKinds = this.includesAllArtifactKindsColumn.GetBoolean((IDataReader) this.Reader),
        Status = (TagDefinitionStatus) this.statusColumn.GetInt32((IDataReader) this.Reader, 0, 0),
        LastUpdated = this.lastUpdatedColumn.GetDateTime((IDataReader) this.Reader, new DateTime())
      };
    }
  }
}
