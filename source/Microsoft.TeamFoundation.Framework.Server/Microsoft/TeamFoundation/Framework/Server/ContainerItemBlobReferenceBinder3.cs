// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ContainerItemBlobReferenceBinder3
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.FileContainer;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ContainerItemBlobReferenceBinder3 : ContainerItemBlobReferenceBinder2
  {
    private SqlColumnBinder ArtifactIdColumn = new SqlColumnBinder("ArtifactId");
    private SqlColumnBinder ArtifactHashColumn = new SqlColumnBinder("ArtifactHash");
    private SqlColumnBinder ArtifactTypeColumn = new SqlColumnBinder("CompressionType");
    private SqlColumnBinder DataspaceIdColumn = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder SessionIdColumn = new SqlColumnBinder("SessionId");
    private readonly TeamFoundationSqlResourceComponent teamFoundationSqlResourceComponent;

    public ContainerItemBlobReferenceBinder3(
      TeamFoundationSqlResourceComponent teamFoundationSqlResourceComponent)
      : base(teamFoundationSqlResourceComponent)
    {
      this.teamFoundationSqlResourceComponent = teamFoundationSqlResourceComponent;
    }

    protected override ContainerItemBlobReference Bind() => new ContainerItemBlobReference()
    {
      ArtifactId = this.ArtifactIdColumn.GetInt64((IDataReader) this.Reader),
      ArtifactHash = this.ArtifactHashColumn.GetString((IDataReader) this.Reader, true),
      CompressionType = (BlobCompressionType) this.ArtifactTypeColumn.GetByte((IDataReader) this.Reader),
      SessionId = this.SessionIdColumn.GetGuid((IDataReader) this.Reader, true),
      ScopeIdentifier = this.teamFoundationSqlResourceComponent.GetDataspaceIdentifier(this.DataspaceIdColumn.GetInt32((IDataReader) this.Reader))
    };
  }
}
