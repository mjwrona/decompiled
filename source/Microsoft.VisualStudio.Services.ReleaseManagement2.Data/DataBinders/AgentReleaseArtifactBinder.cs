// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.AgentReleaseArtifactBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class AgentReleaseArtifactBinder : ObjectBinder<AgentArtifactDefinition>
  {
    private SqlColumnBinder artifactType = new SqlColumnBinder("ArtifactTypeId");
    private SqlColumnBinder name = new SqlColumnBinder(nameof (Name));
    private SqlColumnBinder path = new SqlColumnBinder(nameof (Path));
    private SqlColumnBinder artifactVersion = new SqlColumnBinder(nameof (ArtifactVersion));
    private SqlColumnBinder artifactSourceId = new SqlColumnBinder(nameof (ArtifactSourceId));
    private SqlColumnBinder sourceData = new SqlColumnBinder(nameof (SourceData));

    protected override AgentArtifactDefinition Bind()
    {
      AgentArtifactDefinition artifactDefinition = new AgentArtifactDefinition();
      artifactDefinition.Name = this.name.GetString((IDataReader) this.Reader, false);
      artifactDefinition.Path = this.path.GetString((IDataReader) this.Reader, false);
      artifactDefinition.ArtifactSourceId = this.artifactSourceId.GetInt32((IDataReader) this.Reader);
      artifactDefinition.ArtifactTypeId = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.GetNormalizedArtifactTypeIdAfterDbRead(this.artifactType.GetString((IDataReader) this.Reader, false));
      artifactDefinition.ArtifactVersion = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.FromString<InputValue>(this.artifactVersion.GetString((IDataReader) this.Reader, false)).Value;
      artifactDefinition.FillSourceData(this.sourceData.GetString((IDataReader) this.Reader, false));
      return artifactDefinition;
    }

    protected ref SqlColumnBinder ArtifactSourceId => ref this.artifactSourceId;

    protected ref SqlColumnBinder ArtifactType => ref this.artifactType;

    protected ref SqlColumnBinder ArtifactVersion => ref this.artifactVersion;

    protected ref SqlColumnBinder Name => ref this.name;

    protected ref SqlColumnBinder Path => ref this.path;

    protected ref SqlColumnBinder SourceData => ref this.sourceData;
  }
}
