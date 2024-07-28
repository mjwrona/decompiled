// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ArtifactSourceBinder2
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
  public class ArtifactSourceBinder2 : ArtifactSourceBinder
  {
    private SqlColumnBinder isPrimary = new SqlColumnBinder("IsPrimary");

    protected override ArtifactSource Bind()
    {
      string typeIdAfterDbRead = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.GetNormalizedArtifactTypeIdAfterDbRead(this.ArtifactTypeId.GetString((IDataReader) this.Reader, false));
      ArtifactSource artifactSource = new ArtifactSource()
      {
        ArtifactTypeId = typeIdAfterDbRead,
        SourceId = this.SourceId.GetString((IDataReader) this.Reader, false),
        Alias = this.SourceAlias.GetString((IDataReader) this.Reader, false),
        IsPrimary = this.isPrimary.GetBoolean((IDataReader) this.Reader)
      };
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.FillSourceData(this.SourceData.GetString((IDataReader) this.Reader, false), artifactSource.SourceData);
      return artifactSource;
    }
  }
}
