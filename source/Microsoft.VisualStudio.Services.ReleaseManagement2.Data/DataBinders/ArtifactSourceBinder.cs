// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ArtifactSourceBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ArtifactSourceBinder : ObjectBinder<ArtifactSource>
  {
    private SqlColumnBinder sourceAlias = new SqlColumnBinder("Alias");
    private SqlColumnBinder artifactTypeIdColumn = new SqlColumnBinder(nameof (ArtifactTypeId));
    private SqlColumnBinder sourceData = new SqlColumnBinder(nameof (SourceData));
    private SqlColumnBinder sourceId = new SqlColumnBinder(nameof (SourceId));

    protected override ArtifactSource Bind()
    {
      string typeIdAfterDbRead = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.GetNormalizedArtifactTypeIdAfterDbRead(this.artifactTypeIdColumn.GetString((IDataReader) this.Reader, false));
      ArtifactSource artifactSource = new ArtifactSource()
      {
        ArtifactTypeId = typeIdAfterDbRead,
        SourceId = this.sourceId.GetString((IDataReader) this.Reader, false),
        Alias = this.sourceAlias.GetString((IDataReader) this.Reader, false)
      };
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.FillSourceData(this.sourceData.GetString((IDataReader) this.Reader, false), artifactSource.SourceData);
      return artifactSource;
    }

    protected ref SqlColumnBinder SourceAlias => ref this.sourceAlias;

    protected ref SqlColumnBinder ArtifactTypeId => ref this.artifactTypeIdColumn;

    protected ref SqlColumnBinder SourceData => ref this.sourceData;

    protected ref SqlColumnBinder SourceId => ref this.sourceId;
  }
}
