// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseTriggerBinder2
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
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ReleaseTriggerBinder2 : ReleaseTriggerBinder
  {
    private SqlColumnBinder alias = new SqlColumnBinder("Alias");
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");

    public ReleaseTriggerBinder2(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override ReleaseTriggerBase Bind()
    {
      ReleaseTriggerType releaseTriggerType = (ReleaseTriggerType) this.TriggerType.GetByte((IDataReader) this.Reader);
      string json = this.TriggerContent.GetString((IDataReader) this.Reader, string.Empty);
      Guid guid = this.dataspaceId.ColumnExists((IDataReader) this.Reader) ? this.SqlComponent.GetDataspaceIdentifier(this.dataspaceId.GetInt32((IDataReader) this.Reader)) : Guid.Empty;
      int int32 = this.ReleaseDefinitionId.GetInt32((IDataReader) this.Reader);
      string str = this.alias.GetString((IDataReader) this.Reader, string.Empty);
      switch (releaseTriggerType)
      {
        case ReleaseTriggerType.ArtifactSource:
          ArtifactSourceTrigger artifactSourceTrigger = new ArtifactSourceTrigger();
          artifactSourceTrigger.ReleaseDefinitionId = int32;
          artifactSourceTrigger.ProjectId = guid;
          artifactSourceTrigger.Alias = str;
          artifactSourceTrigger.TargetEnvironmentId = this.TargetEnvironmentId.GetInt32((IDataReader) this.Reader, 0, 0);
          artifactSourceTrigger.TriggerConditions = string.IsNullOrEmpty(json) ? (IList<ArtifactFilter>) null : JsonUtilities.Deserialize<IList<ArtifactFilter>>(json);
          return (ReleaseTriggerBase) artifactSourceTrigger;
        case ReleaseTriggerType.Schedule:
          ScheduledReleaseTrigger scheduledReleaseTrigger = new ScheduledReleaseTrigger();
          scheduledReleaseTrigger.ReleaseDefinitionId = int32;
          scheduledReleaseTrigger.ProjectId = guid;
          scheduledReleaseTrigger.TriggerContent = json;
          return (ReleaseTriggerBase) scheduledReleaseTrigger;
        case ReleaseTriggerType.SourceRepo:
          SourceRepoTrigger sourceRepoTrigger = (SourceRepoTrigger) null;
          if (!string.IsNullOrWhiteSpace(json))
            sourceRepoTrigger = JsonUtilities.Deserialize<SourceRepoTrigger>(json);
          if (sourceRepoTrigger == null)
            sourceRepoTrigger = new SourceRepoTrigger();
          sourceRepoTrigger.ReleaseDefinitionId = int32;
          sourceRepoTrigger.ProjectId = guid;
          sourceRepoTrigger.Alias = str;
          return (ReleaseTriggerBase) sourceRepoTrigger;
        case ReleaseTriggerType.ContainerImage:
          ContainerImageTrigger containerImageTrigger = (ContainerImageTrigger) null;
          if (!string.IsNullOrWhiteSpace(json))
            containerImageTrigger = JsonUtilities.Deserialize<ContainerImageTrigger>(json);
          if (containerImageTrigger == null)
            containerImageTrigger = new ContainerImageTrigger();
          containerImageTrigger.ReleaseDefinitionId = int32;
          containerImageTrigger.ProjectId = guid;
          containerImageTrigger.Alias = str;
          return (ReleaseTriggerBase) containerImageTrigger;
        case ReleaseTriggerType.Package:
          PackageTrigger packageTrigger = (PackageTrigger) null;
          if (!string.IsNullOrWhiteSpace(json))
            packageTrigger = JsonUtilities.Deserialize<PackageTrigger>(json);
          if (packageTrigger == null)
            packageTrigger = new PackageTrigger();
          packageTrigger.ReleaseDefinitionId = int32;
          packageTrigger.ProjectId = guid;
          packageTrigger.Alias = str;
          return (ReleaseTriggerBase) packageTrigger;
        case ReleaseTriggerType.PullRequest:
          PullRequestTrigger pullRequestTrigger = (PullRequestTrigger) null;
          if (!string.IsNullOrWhiteSpace(json))
            pullRequestTrigger = JsonUtilities.Deserialize<PullRequestTrigger>(json);
          if (pullRequestTrigger == null)
            pullRequestTrigger = new PullRequestTrigger();
          pullRequestTrigger.ReleaseDefinitionId = int32;
          pullRequestTrigger.ProjectId = guid;
          pullRequestTrigger.ArtifactAlias = str;
          return (ReleaseTriggerBase) pullRequestTrigger;
        default:
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidReleaseTriggerType, (object) this.TriggerType));
      }
    }
  }
}
