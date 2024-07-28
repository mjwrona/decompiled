// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildArtifactBinder5
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BuildArtifactBinder5 : BuildObjectBinder<BuildArtifact>
  {
    private SqlColumnBinder m_buildId = new SqlColumnBinder("BuildId");
    private SqlColumnBinder m_artifactId = new SqlColumnBinder("ArtifactId");
    private SqlColumnBinder m_artifactName = new SqlColumnBinder("ArtifactName");
    private SqlColumnBinder m_artifactSource = new SqlColumnBinder("ArtifactSource");
    private SqlColumnBinder m_artifactType = new SqlColumnBinder("ArtifactType");
    private SqlColumnBinder m_data = new SqlColumnBinder("Data");
    private SqlColumnBinder m_properties = new SqlColumnBinder("Metadata");

    public BuildArtifactBinder5(IVssRequestContext requestContext)
      : base(requestContext)
    {
    }

    protected override BuildArtifact Bind() => new BuildArtifact()
    {
      BuildId = this.m_buildId.GetInt32((IDataReader) this.Reader),
      Id = this.m_artifactId.GetInt32((IDataReader) this.Reader),
      Name = this.m_artifactName.GetString((IDataReader) this.Reader, false),
      Source = this.m_artifactSource.GetString((IDataReader) this.Reader, true),
      Resource = this.GetArtifactResource(this.m_artifactType.GetString((IDataReader) this.Reader, true), this.m_data.GetString((IDataReader) this.Reader, true), this.m_properties.GetString((IDataReader) this.Reader, true))
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
