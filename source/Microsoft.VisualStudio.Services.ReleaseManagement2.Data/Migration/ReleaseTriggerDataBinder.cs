// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Migration.ReleaseTriggerDataBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Migration
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Suffix is the correct term here")]
  public class ReleaseTriggerDataBinder : ObjectBinder<ReleaseTriggerData>
  {
    private SqlColumnBinder releaseDefinitionId = new SqlColumnBinder("ReleaseDefinitionId");
    private SqlColumnBinder triggerType = new SqlColumnBinder("TriggerType");
    private SqlColumnBinder triggerEntityId = new SqlColumnBinder("TriggerEntityId");
    private SqlColumnBinder alias = new SqlColumnBinder("Alias");
    private SqlColumnBinder targetEnvironmentId = new SqlColumnBinder("TargetEnvironmentId");
    private SqlColumnBinder triggerContent = new SqlColumnBinder("TriggerContent");
    private SqlColumnBinder releaseDefinitionRevision = new SqlColumnBinder("Revision");

    protected override ReleaseTriggerData Bind() => new ReleaseTriggerData()
    {
      ReleaseDefinitionId = this.releaseDefinitionId.GetInt32((IDataReader) this.Reader),
      TriggerType = (ReleaseTriggerType) this.triggerType.GetByte((IDataReader) this.Reader),
      TriggerEntityId = this.triggerEntityId.GetNullableInt32((IDataReader) this.Reader),
      Alias = this.alias.GetString((IDataReader) this.Reader, false),
      TargetEnvironmentId = this.targetEnvironmentId.GetInt32((IDataReader) this.Reader),
      TriggerContent = this.triggerContent.GetString((IDataReader) this.Reader, true),
      ReleaseDefinitionRevision = this.releaseDefinitionRevision.GetInt32((IDataReader) this.Reader)
    };
  }
}
