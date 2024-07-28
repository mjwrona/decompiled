// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseHistorySqlComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class ReleaseHistorySqlComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<ReleaseHistorySqlComponent>(1),
      (IComponentCreator) new ComponentCreator<ReleaseHistorySqlComponent2>(2),
      (IComponentCreator) new ComponentCreator<ReleaseHistorySqlComponent3>(3),
      (IComponentCreator) new ComponentCreator<ReleaseHistorySqlComponent4>(4)
    }, "ReleaseManagementReleaseHistory", "ReleaseManagement");
    private static readonly Microsoft.SqlServer.Server.SqlMetaData[] SqlMetaData = new Microsoft.SqlServer.Server.SqlMetaData[2]
    {
      new Microsoft.SqlServer.Server.SqlMetaData("Id1", SqlDbType.Int),
      new Microsoft.SqlServer.Server.SqlMetaData("Id2", SqlDbType.Int)
    };

    public virtual void SaveRevision(Guid projectId, ReleaseRevision releaseRevision)
    {
    }

    public virtual void UpdateRevision(
      Guid projectId,
      int releaseId,
      int definitionSnapshotRevision,
      ReleaseHistoryChangeTypes changeType,
      int fileId)
    {
    }

    public virtual IEnumerable<ReleaseRevision> GetHistory(Guid projectId, int releaseId) => (IEnumerable<ReleaseRevision>) null;

    public virtual IEnumerable<ReleaseRevision> GetHistory(
      Guid projectId,
      int releaseId,
      int environmentId,
      int attempt)
    {
      return (IEnumerable<ReleaseRevision>) null;
    }

    public virtual ReleaseRevision GetRevision(
      Guid projectId,
      int releaseId,
      int definitionSnapshotRevision)
    {
      return (ReleaseRevision) null;
    }

    public virtual IEnumerable<ReleaseRevision> GetReleaseHistoryForReleases(
      Guid projectId,
      IEnumerable<int> releaseIds)
    {
      this.PrepareStoredProcedure("Release.prc_GetReleaseHistoryForGivenReleases", projectId);
      this.BindInt32Table(nameof (releaseIds), releaseIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseRevision>((ObjectBinder<ReleaseRevision>) new ReleaseHistoryBinderMinimal());
        return (IEnumerable<ReleaseRevision>) resultCollection.GetCurrent<ReleaseRevision>().Items;
      }
    }

    public virtual void DeleteReleaseHistoryForgivenReleaseAndSnapshot(
      Guid projectId,
      IEnumerable<int> releaseIds,
      IEnumerable<int> definitionSnapshotRevisions)
    {
      this.PrepareStoredProcedure("Release.prc_DeleteReleaseHistory", projectId);
      this.BindTable("releaseAndSnapshotIds", "Release.typ_Int32Int32Table", ReleaseHistorySqlComponent.GetSqlRecords(releaseIds, definitionSnapshotRevisions));
      this.ExecuteNonQuery();
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "The base declartion declares the variable name as t which is not understandable")]
    private static IEnumerable<SqlDataRecord> GetSqlRecords(
      IEnumerable<int> releaseIds,
      IEnumerable<int> definitionSnapshotRevisions)
    {
      int length = releaseIds.Count<int>();
      for (int recordIndex = 0; recordIndex < length; ++recordIndex)
      {
        int ordinal = 0;
        SqlDataRecord sqlRecord = new SqlDataRecord(ReleaseHistorySqlComponent.SqlMetaData);
        sqlRecord.SetInt32(ordinal, releaseIds.ElementAt<int>(recordIndex));
        int num;
        sqlRecord.SetInt32(num = ordinal + 1, definitionSnapshotRevisions.ElementAt<int>(recordIndex));
        yield return sqlRecord;
      }
    }
  }
}
