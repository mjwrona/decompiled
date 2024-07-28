// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.PipelineWebHookBinder
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  public class PipelineWebHookBinder : ObjectBinder<WebHook>
  {
    protected SqlColumnBinder m_webHookId = new SqlColumnBinder("WebHookId");
    protected SqlColumnBinder m_artifactType = new SqlColumnBinder("ArtifactType");
    protected SqlColumnBinder m_uniqueResourceIdentifier = new SqlColumnBinder("UniqueResourceIdentifier");
    protected SqlColumnBinder m_connectionId = new SqlColumnBinder("ConnectionId");

    protected override WebHook Bind() => new WebHook()
    {
      WebHookId = this.m_webHookId.GetGuid((IDataReader) this.Reader, false),
      ArtifactType = this.m_artifactType.GetString((IDataReader) this.Reader, false),
      UniqueArtifactIdentifier = this.m_uniqueResourceIdentifier.GetString((IDataReader) this.Reader, false),
      ConnectionId = this.m_connectionId.GetGuid((IDataReader) this.Reader, false)
    };
  }
}
