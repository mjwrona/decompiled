// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.ArtifactMetaDataSqlResourceComponent2
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  public class ArtifactMetaDataSqlResourceComponent2 : ArtifactMetaDataSqlResourceComponent
  {
    public override List<OccurrenceData> GetOccurrencesByResources(
      IEnumerable<string> resourceIds,
      NoteKind kind)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (GetOccurrencesByResources)))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetOccurrencesByResources");
        this.BindStringTable(nameof (resourceIds), resourceIds);
        this.BindShort(nameof (kind), (short) kind);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<OccurrenceData>((ObjectBinder<OccurrenceData>) new OccurrenceDataBinder());
          return resultCollection.GetCurrent<OccurrenceData>().Items;
        }
      }
    }
  }
}
