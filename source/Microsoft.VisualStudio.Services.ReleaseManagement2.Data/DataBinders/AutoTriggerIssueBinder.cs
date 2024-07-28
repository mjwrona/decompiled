// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.AutoTriggerIssueBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class AutoTriggerIssueBinder : ReleaseManagementObjectBinderBase<AutoTriggerIssue>
  {
    private SqlColumnBinder buildId = new SqlColumnBinder("BuildId");
    private SqlColumnBinder releaseDefinitionId = new SqlColumnBinder(nameof (ReleaseDefinitionId));
    private SqlColumnBinder releaseDefinitionName = new SqlColumnBinder(nameof (ReleaseDefinitionName));
    private SqlColumnBinder issueSource = new SqlColumnBinder(nameof (IssueSource));
    private SqlColumnBinder issueType = new SqlColumnBinder(nameof (IssueType));
    private SqlColumnBinder issueMessage = new SqlColumnBinder(nameof (IssueMessage));

    public AutoTriggerIssueBinder(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override AutoTriggerIssue Bind()
    {
      ContinuousDeploymentTriggerIssue deploymentTriggerIssue = new ContinuousDeploymentTriggerIssue();
      deploymentTriggerIssue.ArtifactVersionId = this.buildId.GetInt32((IDataReader) this.Reader).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      deploymentTriggerIssue.ReleaseDefinitionId = this.releaseDefinitionId.GetInt32((IDataReader) this.Reader);
      deploymentTriggerIssue.ReleaseDefinitionName = this.releaseDefinitionName.GetString((IDataReader) this.Reader, false);
      deploymentTriggerIssue.ArtifactType = "Build";
      deploymentTriggerIssue.IssueSource = (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IssueSource) this.issueSource.GetByte((IDataReader) this.Reader);
      deploymentTriggerIssue.Issue = new Issue()
      {
        IssueType = (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.IssueType) this.issueType.GetByte((IDataReader) this.Reader),
        Message = this.issueMessage.GetString((IDataReader) this.Reader, false)
      };
      return (AutoTriggerIssue) deploymentTriggerIssue;
    }

    protected ref SqlColumnBinder ReleaseDefinitionId => ref this.releaseDefinitionId;

    protected ref SqlColumnBinder ReleaseDefinitionName => ref this.releaseDefinitionName;

    protected ref SqlColumnBinder IssueSource => ref this.issueSource;

    protected ref SqlColumnBinder IssueType => ref this.issueType;

    protected ref SqlColumnBinder IssueMessage => ref this.issueMessage;
  }
}
