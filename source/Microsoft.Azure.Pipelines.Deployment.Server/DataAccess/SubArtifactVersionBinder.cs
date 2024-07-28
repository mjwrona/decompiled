// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.SubArtifactVersionBinder
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  internal class SubArtifactVersionBinder : ObjectBinder<SubArtifactDataRow>
  {
    private SqlColumnBinder m_Id = new SqlColumnBinder("SubArtifactVersionId");
    private SqlColumnBinder m_SubArtifactName = new SqlColumnBinder("SubArtifactName");

    protected override SubArtifactDataRow Bind() => new SubArtifactDataRow()
    {
      SubArtifactVersionId = this.m_Id.GetInt32((IDataReader) this.Reader),
      SubArtifactName = this.m_SubArtifactName.GetString((IDataReader) this.Reader, false)
    };
  }
}
