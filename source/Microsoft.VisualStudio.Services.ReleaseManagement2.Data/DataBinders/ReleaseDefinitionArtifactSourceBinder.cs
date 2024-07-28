// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseDefinitionArtifactSourceBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ReleaseDefinitionArtifactSourceBinder : 
    ReleaseManagementObjectBinderBase<ReleaseDefinitionArtifactSourceMap>
  {
    private SqlColumnBinder releaseDefinitionId = new SqlColumnBinder(nameof (ReleaseDefinitionId));
    private SqlColumnBinder artifactSourceId = new SqlColumnBinder(nameof (ArtifactSourceId));
    private SqlColumnBinder alias = new SqlColumnBinder(nameof (Alias));

    public ReleaseDefinitionArtifactSourceBinder(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override ReleaseDefinitionArtifactSourceMap Bind() => new ReleaseDefinitionArtifactSourceMap()
    {
      ReleaseDefinitionId = this.releaseDefinitionId.GetInt32((IDataReader) this.Reader),
      ArtifactSourceId = this.artifactSourceId.GetInt32((IDataReader) this.Reader),
      Alias = this.alias.GetString((IDataReader) this.Reader, true)
    };

    protected ref SqlColumnBinder ReleaseDefinitionId => ref this.releaseDefinitionId;

    protected ref SqlColumnBinder ArtifactSourceId => ref this.artifactSourceId;

    protected ref SqlColumnBinder Alias => ref this.alias;
  }
}
