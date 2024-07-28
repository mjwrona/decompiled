// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseArtifactSourceBinder3
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
  public class ReleaseArtifactSourceBinder3 : ReleaseArtifactSourceBinder2
  {
    private SqlColumnBinder isPrimary = new SqlColumnBinder(nameof (IsPrimary));

    public ReleaseArtifactSourceBinder3(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override PipelineArtifactSource Bind()
    {
      string artifactTypeId = this.ArtifactTypeId.GetString((IDataReader) this.Reader, false);
      PipelineArtifactSource pipelineArtifactSource1 = new PipelineArtifactSource();
      pipelineArtifactSource1.Id = this.Id.GetInt32((IDataReader) this.Reader);
      pipelineArtifactSource1.ArtifactSourceId = this.ArtifactSourceId.GetInt32((IDataReader) this.Reader);
      pipelineArtifactSource1.ArtifactVersion = this.ArtifactVersion.GetString((IDataReader) this.Reader, false);
      pipelineArtifactSource1.ReleaseId = this.ReleaseId.GetInt32((IDataReader) this.Reader);
      pipelineArtifactSource1.ArtifactTypeId = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.GetNormalizedArtifactTypeIdAfterDbRead(artifactTypeId);
      pipelineArtifactSource1.Alias = this.SourceAlias.GetString((IDataReader) this.Reader, false);
      pipelineArtifactSource1.SourceBranch = this.Branch.GetString((IDataReader) this.Reader, string.Empty);
      pipelineArtifactSource1.IsPrimary = this.isPrimary.GetBoolean((IDataReader) this.Reader, false);
      PipelineArtifactSource pipelineArtifactSource2 = pipelineArtifactSource1;
      IDictionary<string, InputValue> dictionary = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.FromString<IDictionary<string, InputValue>>(this.SourceData.GetString((IDataReader) this.Reader, false));
      if (dictionary != null)
      {
        foreach (KeyValuePair<string, InputValue> keyValuePair in (IEnumerable<KeyValuePair<string, InputValue>>) dictionary)
          pipelineArtifactSource2.SourceData[keyValuePair.Key] = keyValuePair.Value;
      }
      return pipelineArtifactSource2;
    }

    protected ref SqlColumnBinder IsPrimary => ref this.isPrimary;
  }
}
