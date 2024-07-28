// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.AutoTriggerIssueBinder2
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class AutoTriggerIssueBinder2 : AutoTriggerIssueBinder
  {
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder sourceId = new SqlColumnBinder("SourceId");
    private SqlColumnBinder artifactVersionId = new SqlColumnBinder("ArtifactVersionId");
    private SqlColumnBinder triggerType = new SqlColumnBinder("TriggerType");
    private SqlColumnBinder artifactType = new SqlColumnBinder("ArtifactType");

    public AutoTriggerIssueBinder2(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override AutoTriggerIssue Bind()
    {
      switch ((ReleaseTriggerType) this.triggerType.GetByte((IDataReader) this.Reader))
      {
        case ReleaseTriggerType.ArtifactSource:
        case ReleaseTriggerType.SourceRepo:
        case ReleaseTriggerType.ContainerImage:
          ContinuousDeploymentTriggerIssue deploymentTriggerIssue = new ContinuousDeploymentTriggerIssue();
          deploymentTriggerIssue.ProjectId = this.SqlComponent.GetDataspaceIdentifier(this.dataspaceId.GetInt32((IDataReader) this.Reader));
          deploymentTriggerIssue.ArtifactVersionId = this.artifactVersionId.GetString((IDataReader) this.Reader, false);
          deploymentTriggerIssue.SourceId = this.sourceId.GetString((IDataReader) this.Reader, false);
          deploymentTriggerIssue.ReleaseDefinitionId = this.ReleaseDefinitionId.GetInt32((IDataReader) this.Reader);
          deploymentTriggerIssue.ReleaseDefinitionName = this.ReleaseDefinitionName.GetString((IDataReader) this.Reader, false);
          deploymentTriggerIssue.ArtifactType = this.artifactType.GetString((IDataReader) this.Reader, false);
          deploymentTriggerIssue.ReleaseTriggerType = (ReleaseTriggerType) this.triggerType.GetByte((IDataReader) this.Reader);
          deploymentTriggerIssue.IssueSource = (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IssueSource) this.IssueSource.GetByte((IDataReader) this.Reader);
          deploymentTriggerIssue.Issue = new Issue()
          {
            IssueType = (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.IssueType) this.IssueType.GetByte((IDataReader) this.Reader),
            Message = this.IssueMessage.GetString((IDataReader) this.Reader, false)
          };
          return (AutoTriggerIssue) deploymentTriggerIssue;
        default:
          return (AutoTriggerIssue) null;
      }
    }
  }
}
