// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseArtifactDefinitionBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ReleaseArtifactDefinitionBinder : ObjectBinder<ReleaseArtifactDefinition>
  {
    private SqlColumnBinder id = new SqlColumnBinder("Id");
    private SqlColumnBinder name = new SqlColumnBinder("Name");
    private SqlColumnBinder description = new SqlColumnBinder("Description");
    private SqlColumnBinder path = new SqlColumnBinder("Path");
    private SqlColumnBinder releaseArtifactSourceId = new SqlColumnBinder("ReleaseArtifactSourceId");

    protected override ReleaseArtifactDefinition Bind() => new ReleaseArtifactDefinition()
    {
      Name = this.name.GetString((IDataReader) this.Reader, false),
      Description = this.description.GetString((IDataReader) this.Reader, false),
      Path = this.path.GetString((IDataReader) this.Reader, false),
      ArtifactSourceId = this.releaseArtifactSourceId.GetInt32((IDataReader) this.Reader),
      ArtifactDefinitionId = this.id.GetInt32((IDataReader) this.Reader)
    };
  }
}
