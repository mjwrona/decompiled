// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.ArtifactMetaDataSqlResourceComponent
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  public class ArtifactMetaDataSqlResourceComponent : DeploymentSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<ArtifactMetaDataSqlResourceComponent>(1),
      (IComponentCreator) new ComponentCreator<ArtifactMetaDataSqlResourceComponent2>(2),
      (IComponentCreator) new ComponentCreator<ArtifactMetaDataSqlResourceComponent3>(3)
    }, "DeploymentArtifactMetadata", "Deployment");

    public ArtifactMetaDataSqlResourceComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public void CreateNote(
      Guid scopeId,
      string name,
      NoteKind noteKind,
      Guid createdBy,
      int? fileId)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (CreateNote)))
      {
        this.PrepareStoredProcedure("Deployment.prc_CreateNote");
        this.BindGuid(nameof (scopeId), scopeId);
        this.BindString(nameof (name), name, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindShort("kind", (short) noteKind);
        this.BindGuid(nameof (createdBy), createdBy);
        this.BindNullableInt(nameof (fileId), fileId);
        this.ExecuteNonQuery();
      }
    }

    public NoteData GetNote(string name)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (GetNote)))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetNote");
        this.BindString(nameof (name), name, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<NoteData>((ObjectBinder<NoteData>) new NoteDataBinder());
          return resultCollection.GetCurrent<NoteData>().Items.FirstOrDefault<NoteData>();
        }
      }
    }

    public void DeleteNote(string name)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (DeleteNote)))
      {
        this.PrepareStoredProcedure("Deployment.prc_DeleteNote");
        this.BindString(nameof (name), name, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
    }

    public void CreateOccurrence(
      Guid scopeId,
      string noteName,
      string name,
      NoteKind kind,
      string resourceId,
      Guid createdBy,
      int? fileId,
      IList<string> tags)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (CreateOccurrence)))
      {
        this.PrepareStoredProcedure("Deployment.prc_CreateOccurrence");
        this.BindGuid(nameof (scopeId), scopeId);
        this.BindString(nameof (noteName), noteName, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString(nameof (name), name, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindShort(nameof (kind), (short) kind);
        this.BindString(nameof (resourceId), resourceId, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid(nameof (createdBy), createdBy);
        this.BindNullableInt(nameof (fileId), fileId);
        this.BindStringTable(nameof (tags), (IEnumerable<string>) tags);
        this.ExecuteNonQuery();
      }
    }

    public OccurrenceData GetOccurrence(string name)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (GetOccurrence)))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetOccurrence");
        this.BindString(nameof (name), name, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<OccurrenceData>((ObjectBinder<OccurrenceData>) new OccurrenceDataBinder());
          OccurrenceData occurrence = resultCollection.GetCurrent<OccurrenceData>().Items.FirstOrDefault<OccurrenceData>();
          resultCollection.NextResult();
          SqlColumnBinder tagColumnBinder = new SqlColumnBinder("Tag");
          resultCollection.AddBinder<string>((ObjectBinder<string>) new SimpleObjectBinder<string>((System.Func<IDataReader, string>) (reader => tagColumnBinder.GetString(reader, false))));
          ObjectBinder<string> current = resultCollection.GetCurrent<string>();
          if (occurrence != null)
            occurrence.Tags.AddRange<string, IList<string>>((IEnumerable<string>) current);
          return occurrence;
        }
      }
    }

    public List<OccurrenceData> GetNoteOccurrences(string noteName)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (GetNoteOccurrences)))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetOccurrencesByNote");
        this.BindString(nameof (noteName), noteName, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<OccurrenceData>((ObjectBinder<OccurrenceData>) new OccurrenceDataBinder());
          return resultCollection.GetCurrent<OccurrenceData>().Items;
        }
      }
    }

    public List<OccurrenceData> GetOccurrences(string resourceId)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (GetOccurrences)))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetOccurrencesByResource");
        this.BindString(nameof (resourceId), resourceId, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<OccurrenceData>((ObjectBinder<OccurrenceData>) new OccurrenceDataBinder());
          return resultCollection.GetCurrent<OccurrenceData>().Items;
        }
      }
    }

    public void CreateOccurrenceTag(string occurrenceName, string tag)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (CreateOccurrenceTag)))
      {
        this.PrepareStoredProcedure("Deployment.prc_CreateOccurrenceTag");
        this.BindString(nameof (occurrenceName), occurrenceName, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString(nameof (tag), tag, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
    }

    public void DeleteOccurrenceTag(string occurrenceName, string tag)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (DeleteOccurrenceTag)))
      {
        this.PrepareStoredProcedure("Deployment.prc_DeleteOccurrenceTag");
        this.BindString(nameof (occurrenceName), occurrenceName, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString(nameof (tag), tag, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
    }

    public virtual List<OccurrenceData> GetOccurrencesByResources(
      IEnumerable<string> resourceIds,
      NoteKind kind)
    {
      return new List<OccurrenceData>();
    }

    public virtual List<OccurrenceData> GetOccurrences(
      IEnumerable<string> resourceUris,
      IEnumerable<NoteKind> kinds)
    {
      throw new NotImplementedException();
    }

    public virtual IEnumerable<string> GetResourceIdsByTag(string tag) => throw new NotImplementedException();
  }
}
