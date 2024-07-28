// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.AgentReleaseArtifactBinder2
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
  public class AgentReleaseArtifactBinder2 : AgentReleaseArtifactBinder
  {
    private SqlColumnBinder alias = new SqlColumnBinder(nameof (Alias));

    protected override AgentArtifactDefinition Bind()
    {
      AgentArtifactDefinition artifactDefinition = new AgentArtifactDefinition();
      artifactDefinition.Name = this.Name.GetString((IDataReader) this.Reader, false);
      artifactDefinition.Alias = this.alias.GetString((IDataReader) this.Reader, false);
      artifactDefinition.Path = this.Path.GetString((IDataReader) this.Reader, false);
      artifactDefinition.ArtifactSourceId = this.ArtifactSourceId.GetInt32((IDataReader) this.Reader);
      artifactDefinition.ArtifactTypeId = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.GetNormalizedArtifactTypeIdAfterDbRead(this.ArtifactType.GetString((IDataReader) this.Reader, false));
      artifactDefinition.ArtifactVersion = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.FromString<InputValue>(this.ArtifactVersion.GetString((IDataReader) this.Reader, false)).Value;
      artifactDefinition.FillSourceData(this.SourceData.GetString((IDataReader) this.Reader, false));
      return artifactDefinition;
    }

    protected ref SqlColumnBinder Alias => ref this.alias;
  }
}
