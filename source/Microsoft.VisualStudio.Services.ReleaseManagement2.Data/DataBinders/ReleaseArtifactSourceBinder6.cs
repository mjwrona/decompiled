// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseArtifactSourceBinder6
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Cannot avoid this as this is a sql binder")]
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ReleaseArtifactSourceBinder6 : ReleaseArtifactSourceBinder5
  {
    private SqlColumnBinder artifactVersionCreatedOn = new SqlColumnBinder("ArtifactVersionCreatedOn");
    private SqlColumnBinder isRetained = new SqlColumnBinder("IsRetained");
    private SqlColumnBinder releaseDefinitionId = new SqlColumnBinder("ReleaseDefinitionId");
    private SqlColumnBinder createdOn = new SqlColumnBinder("CreatedOn");

    public ReleaseArtifactSourceBinder6(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override PipelineArtifactSource Bind()
    {
      string artifactTypeId = this.ArtifactTypeId.GetString((IDataReader) this.Reader, false);
      Guid guid = this.DataspaceId.ColumnExists((IDataReader) this.Reader) ? this.SqlComponent.GetDataspaceIdentifier(this.DataspaceId.GetInt32((IDataReader) this.Reader)) : Guid.Empty;
      PipelineArtifactSource pipelineArtifactSource1 = new PipelineArtifactSource();
      pipelineArtifactSource1.Id = this.Id.GetInt32((IDataReader) this.Reader);
      pipelineArtifactSource1.ProjectId = guid;
      pipelineArtifactSource1.SourceId = this.SourceId.GetString((IDataReader) this.Reader, false);
      pipelineArtifactSource1.ReleaseId = this.ReleaseId.GetInt32((IDataReader) this.Reader);
      pipelineArtifactSource1.ArtifactTypeId = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.GetNormalizedArtifactTypeIdAfterDbRead(artifactTypeId);
      pipelineArtifactSource1.Alias = this.SourceAlias.GetString((IDataReader) this.Reader, false);
      pipelineArtifactSource1.SourceBranch = this.Branch.GetString((IDataReader) this.Reader, string.Empty);
      pipelineArtifactSource1.IsPrimary = this.IsPrimary.GetBoolean((IDataReader) this.Reader, false);
      pipelineArtifactSource1.ArtifactVersion = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) new InputValue()
      {
        Value = this.ArtifactVersionId.GetString((IDataReader) this.Reader, true),
        DisplayValue = this.ArtifactVersionName.GetString((IDataReader) this.Reader, false),
        Data = (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "artifactVersionCreatedOn",
            (object) this.artifactVersionCreatedOn.GetDateTime((IDataReader) this.Reader)
          }
        }
      });
      pipelineArtifactSource1.IsRetained = this.isRetained.ColumnExists((IDataReader) this.Reader) && this.isRetained.GetBoolean((IDataReader) this.Reader, false);
      pipelineArtifactSource1.ReleaseDefinitionId = this.releaseDefinitionId.GetInt32((IDataReader) this.Reader, 0, 0);
      pipelineArtifactSource1.CreatedOn = this.createdOn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue);
      PipelineArtifactSource pipelineArtifactSource2 = pipelineArtifactSource1;
      string str = this.SourceData.GetString((IDataReader) this.Reader, false);
      try
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.FillSourceData(str, pipelineArtifactSource2.SourceData);
      }
      catch (Exception ex) when (ex is JsonReaderException || ex is JsonSerializationException)
      {
        if (!ReleaseArtifactSourceBinder6.TryParseSourceData(str, pipelineArtifactSource2.SourceData))
          throw;
      }
      return pipelineArtifactSource2;
    }

    private static bool TryParseSourceData(
      string sourceData,
      Dictionary<string, InputValue> targetDictionary)
    {
      JToken jtoken1 = JsonUtilities.DeserializeTruncatedJson(sourceData);
      bool sourceData1 = false;
      foreach (JProperty jproperty in jtoken1.Children().OfType<JProperty>())
      {
        JToken jtoken2 = jproperty.Value;
        InputValue inputValue = new InputValue()
        {
          Value = jtoken2.Value<string>((object) "value"),
          DisplayValue = jtoken2.Value<string>((object) "displayValue")
        };
        targetDictionary.Add(jproperty.Name, inputValue);
        if (!sourceData1)
          sourceData1 = true;
      }
      return sourceData1;
    }
  }
}
