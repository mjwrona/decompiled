// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildArtifactBinderHelper6
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class BuildArtifactBinderHelper6
  {
    private SqlColumnBinder m_buildId = new SqlColumnBinder("BuildId");
    private SqlColumnBinder m_artifactId = new SqlColumnBinder("ArtifactId");
    private SqlColumnBinder m_artifactName = new SqlColumnBinder("ArtifactName");
    private SqlColumnBinder m_artifactSource = new SqlColumnBinder("ArtifactSource");
    private SqlColumnBinder m_artifactSourceCreatedOn = new SqlColumnBinder("ArtifactSourceCreatedOn");
    private SqlColumnBinder m_artifactType = new SqlColumnBinder("ArtifactType");
    private SqlColumnBinder m_data = new SqlColumnBinder("Data");
    private SqlColumnBinder m_properties = new SqlColumnBinder("Metadata");

    public BuildArtifact Bind(SqlDataReader Reader) => new BuildArtifact()
    {
      BuildId = this.m_buildId.GetInt32((IDataReader) Reader),
      Id = this.m_artifactId.GetInt32((IDataReader) Reader),
      Name = this.m_artifactName.GetString((IDataReader) Reader, false),
      Source = this.m_artifactSource.GetString((IDataReader) Reader, true),
      Resource = this.GetArtifactResource(this.m_artifactType.GetString((IDataReader) Reader, true), this.m_data.GetString((IDataReader) Reader, true), this.m_properties.GetString((IDataReader) Reader, true)),
      SourceCreatedDate = this.m_artifactSourceCreatedOn.GetDateTime((IDataReader) Reader)
    };

    private BuildArtifactResource GetArtifactResource(
      string artifactType,
      string data,
      string propertyString)
    {
      BuildArtifactResource resource = (BuildArtifactResource) null;
      if (!string.IsNullOrEmpty(data))
      {
        resource = new BuildArtifactResource()
        {
          Data = data,
          Type = artifactType,
          Properties = JsonUtility.FromString<Dictionary<string, string>>(propertyString)
        };
        if (string.IsNullOrEmpty(resource.Type))
          resource.TryInferType();
      }
      return resource;
    }
  }
}
