// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseArtifactSourceBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ReleaseArtifactSourceBinder : 
    ReleaseManagementObjectBinderBase<PipelineArtifactSource>
  {
    private SqlColumnBinder id = new SqlColumnBinder(nameof (Id));
    private SqlColumnBinder artifactSourceId = new SqlColumnBinder(nameof (ArtifactSourceId));
    private SqlColumnBinder artifactVersion = new SqlColumnBinder(nameof (ArtifactVersion));
    private SqlColumnBinder releaseId = new SqlColumnBinder(nameof (ReleaseId));
    private SqlColumnBinder artifactTypeId = new SqlColumnBinder(nameof (ArtifactTypeId));
    private SqlColumnBinder sourceData = new SqlColumnBinder(nameof (SourceData));

    public ReleaseArtifactSourceBinder(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override PipelineArtifactSource Bind()
    {
      string artifactTypeId = this.artifactTypeId.GetString((IDataReader) this.Reader, false);
      PipelineArtifactSource pipelineArtifactSource1 = new PipelineArtifactSource();
      pipelineArtifactSource1.Id = this.id.GetInt32((IDataReader) this.Reader);
      pipelineArtifactSource1.ArtifactSourceId = this.artifactSourceId.GetInt32((IDataReader) this.Reader);
      pipelineArtifactSource1.ArtifactVersion = this.artifactVersion.GetString((IDataReader) this.Reader, false);
      pipelineArtifactSource1.ReleaseId = this.releaseId.GetInt32((IDataReader) this.Reader);
      pipelineArtifactSource1.ArtifactTypeId = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.GetNormalizedArtifactTypeIdAfterDbRead(artifactTypeId);
      PipelineArtifactSource pipelineArtifactSource2 = pipelineArtifactSource1;
      IDictionary<string, InputValue> dictionary = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.FromString<IDictionary<string, InputValue>>(this.sourceData.GetString((IDataReader) this.Reader, false));
      if (dictionary != null)
      {
        foreach (KeyValuePair<string, InputValue> keyValuePair in (IEnumerable<KeyValuePair<string, InputValue>>) dictionary)
          pipelineArtifactSource2.SourceData[keyValuePair.Key] = keyValuePair.Value;
      }
      return pipelineArtifactSource2;
    }

    protected ref SqlColumnBinder Id => ref this.id;

    protected ref SqlColumnBinder ArtifactSourceId => ref this.ArtifactSourceId;

    protected ref SqlColumnBinder ArtifactVersion => ref this.artifactVersion;

    protected ref SqlColumnBinder ReleaseId => ref this.releaseId;

    protected ref SqlColumnBinder ArtifactTypeId => ref this.artifactTypeId;

    protected ref SqlColumnBinder SourceData => ref this.sourceData;
  }
}
