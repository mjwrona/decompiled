// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.ArtifactMetaDataSqlResourceComponent3
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  public class ArtifactMetaDataSqlResourceComponent3 : ArtifactMetaDataSqlResourceComponent2
  {
    public override List<OccurrenceData> GetOccurrences(
      IEnumerable<string> resourceUris,
      IEnumerable<NoteKind> kinds)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (GetOccurrences)))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetOccurrencesByResourcesWithKinds");
        this.BindStringTable(nameof (resourceUris), resourceUris);
        this.BindTinyIntTable(nameof (kinds), kinds.Select<NoteKind, byte>((System.Func<NoteKind, byte>) (k => (byte) k)));
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<OccurrenceData>((ObjectBinder<OccurrenceData>) new OccurrenceDataBinder());
          return resultCollection.GetCurrent<OccurrenceData>().Items;
        }
      }
    }

    public override IEnumerable<string> GetResourceIdsByTag(string tag)
    {
      using (new DeploymentSqlComponentBase.SqlMethodScope((DeploymentSqlComponentBase) this, nameof (GetResourceIdsByTag)))
      {
        this.PrepareStoredProcedure("Deployment.prc_GetResourceIdsByTag");
        this.BindString(nameof (tag), tag, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<string>((ObjectBinder<string>) new ResourceIdDataBinder());
          return (IEnumerable<string>) resultCollection.GetCurrent<string>().Items;
        }
      }
    }
  }
}
