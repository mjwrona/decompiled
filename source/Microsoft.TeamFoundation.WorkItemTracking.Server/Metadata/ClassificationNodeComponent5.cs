// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ClassificationNodeComponent5
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class ClassificationNodeComponent5 : ClassificationNodeComponent4
  {
    public override IEnumerable<ClassificationNodeUpdateError> CreateClassificationNodes(
      Guid projectId,
      IEnumerable<ClassificationNodeUpdate> nodes,
      Guid requestIdentityId,
      out int nodeSequenceId)
    {
      this.PrepareStoredProcedure("prc_CreateClassificationNodes");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindClassificationNodeTable("@nodes", nodes);
      this.BindGuid("@changerTfId", requestIdentityId);
      this.BindGuid("@eventAuthor", this.Author);
      using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ClassificationNodeUpdateError>((ObjectBinder<ClassificationNodeUpdateError>) new ClassificationNodeComponent.ClassificationNodeUpdateErrorBinder());
        resultCollection.AddBinder<int>((ObjectBinder<int>) new ClassificationNodeComponent5.SequenceIdColumns());
        List<ClassificationNodeUpdateError> items = resultCollection.GetCurrent<ClassificationNodeUpdateError>().Items;
        resultCollection.NextResult();
        nodeSequenceId = resultCollection.GetCurrent<int>().Items.FirstOrDefault<int>();
        return (IEnumerable<ClassificationNodeUpdateError>) items;
      }
    }

    public override ClassificationNodeUpdateError UpdateClassificationNode(
      Guid projectId,
      Guid changerTfId,
      int nodeId,
      string nodeName,
      int? parentId,
      DateTime? startDate,
      DateTime? finishDate,
      bool setDates,
      int timeout,
      bool forceUpdate,
      out int nodeSequenceId)
    {
      this.PrepareStoredProcedure("prc_UpdateClassificationNode");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@nodeId", nodeId);
      this.BindString("@name", nodeName, (int) byte.MaxValue, true, SqlDbType.NVarChar);
      if (parentId.HasValue)
        this.BindInt("@parentId", parentId.Value);
      else
        this.BindNullValue("@parentId", SqlDbType.Int);
      this.BindIterationDate("@startDate", startDate);
      this.BindIterationDate("@finishDate", finishDate);
      this.BindBoolean("@areDatesChanged", setDates);
      this.BindGuid("@changerTfId", changerTfId);
      this.BindGuid("@eventAuthor", this.Author);
      this.BindInt("@timeout", timeout);
      this.BindBoolean("@forceUpdate", forceUpdate);
      using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ClassificationNodeUpdateError>((ObjectBinder<ClassificationNodeUpdateError>) new ClassificationNodeComponent.ClassificationNodeUpdateErrorBinder());
        resultCollection.AddBinder<int>((ObjectBinder<int>) new ClassificationNodeComponent5.SequenceIdColumns());
        ClassificationNodeUpdateError classificationNodeUpdateError = resultCollection.GetCurrent<ClassificationNodeUpdateError>().Items.FirstOrDefault<ClassificationNodeUpdateError>();
        resultCollection.NextResult();
        nodeSequenceId = resultCollection.GetCurrent<int>().Items.FirstOrDefault<int>();
        return classificationNodeUpdateError;
      }
    }

    public override IEnumerable<ClassificationNodeUpdateError> DeleteClassificationNodes(
      Guid projectId,
      Guid changerTfId,
      IEnumerable<ClassificationNodeUpdate> nodes,
      int timeout,
      out int nodeSequenceId)
    {
      this.PrepareStoredProcedure("prc_DeleteClassificationNodes");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindClassificationNodeTable("@nodes", nodes);
      this.BindGuid("@changerTfId", changerTfId);
      this.BindGuid("@eventAuthor", this.Author);
      this.BindInt("@timeout", timeout);
      using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ClassificationNodeUpdateError>((ObjectBinder<ClassificationNodeUpdateError>) new ClassificationNodeComponent.ClassificationNodeUpdateErrorBinder());
        resultCollection.AddBinder<int>((ObjectBinder<int>) new ClassificationNodeComponent5.SequenceIdColumns());
        List<ClassificationNodeUpdateError> items = resultCollection.GetCurrent<ClassificationNodeUpdateError>().Items;
        resultCollection.NextResult();
        nodeSequenceId = resultCollection.GetCurrent<int>().Items.FirstOrDefault<int>();
        return (IEnumerable<ClassificationNodeUpdateError>) items;
      }
    }

    internal class SequenceIdColumns : ObjectBinder<int>
    {
      private SqlColumnBinder sequenceId = new SqlColumnBinder(nameof (sequenceId));

      protected override int Bind() => this.sequenceId.GetInt32((IDataReader) this.Reader, 0);
    }
  }
}
