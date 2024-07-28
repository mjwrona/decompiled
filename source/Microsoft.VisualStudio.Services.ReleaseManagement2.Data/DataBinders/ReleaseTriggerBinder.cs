// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseTriggerBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ReleaseTriggerBinder : ReleaseManagementObjectBinderBase<ReleaseTriggerBase>
  {
    private SqlColumnBinder triggerType = new SqlColumnBinder(nameof (TriggerType));
    private SqlColumnBinder artifactSourceId = new SqlColumnBinder(nameof (TriggerEntityId));
    private SqlColumnBinder releaseDefinitionId = new SqlColumnBinder(nameof (ReleaseDefinitionId));
    private SqlColumnBinder targetEnvironmentId = new SqlColumnBinder(nameof (TargetEnvironmentId));
    private SqlColumnBinder triggerContent = new SqlColumnBinder(nameof (TriggerContent));

    public ReleaseTriggerBinder(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override ReleaseTriggerBase Bind()
    {
      switch ((ReleaseTriggerType) this.triggerType.GetByte((IDataReader) this.Reader))
      {
        case ReleaseTriggerType.ArtifactSource:
          ArtifactSourceTrigger artifactSourceTrigger = new ArtifactSourceTrigger();
          artifactSourceTrigger.ReleaseDefinitionId = this.releaseDefinitionId.GetInt32((IDataReader) this.Reader);
          artifactSourceTrigger.ArtifactSourceId = this.artifactSourceId.GetInt32((IDataReader) this.Reader);
          artifactSourceTrigger.TargetEnvironmentId = this.targetEnvironmentId.GetInt32((IDataReader) this.Reader, 0, 0);
          return (ReleaseTriggerBase) artifactSourceTrigger;
        case ReleaseTriggerType.Schedule:
          ScheduledReleaseTrigger scheduledReleaseTrigger = new ScheduledReleaseTrigger();
          scheduledReleaseTrigger.ReleaseDefinitionId = this.releaseDefinitionId.GetInt32((IDataReader) this.Reader);
          scheduledReleaseTrigger.TriggerContent = this.triggerContent.GetString((IDataReader) this.Reader, string.Empty);
          return (ReleaseTriggerBase) scheduledReleaseTrigger;
        default:
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidReleaseTriggerType, (object) this.triggerType));
      }
    }

    protected ref SqlColumnBinder TriggerType => ref this.triggerType;

    protected ref SqlColumnBinder TriggerEntityId => ref this.TriggerEntityId;

    protected ref SqlColumnBinder ReleaseDefinitionId => ref this.releaseDefinitionId;

    protected ref SqlColumnBinder TargetEnvironmentId => ref this.targetEnvironmentId;

    protected ref SqlColumnBinder TriggerContent => ref this.triggerContent;
  }
}
