// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.DeploymentIssueBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class DeploymentIssueBinder : ObjectBinder<DeploymentIssue>
  {
    private SqlColumnBinder releaseId = new SqlColumnBinder("ReleaseId");
    private SqlColumnBinder deploymentId = new SqlColumnBinder("DeploymentId");
    private SqlColumnBinder issue = new SqlColumnBinder("IssueText");
    private SqlColumnBinder issueType = new SqlColumnBinder("IssueType");

    protected override DeploymentIssue Bind()
    {
      string str = this.issue.GetString((IDataReader) this.Reader, false);
      byte num = this.issueType.GetByte((IDataReader) this.Reader);
      return new DeploymentIssue(this.releaseId.GetInt32((IDataReader) this.Reader), this.deploymentId.GetInt32((IDataReader) this.Reader), new Issue()
      {
        Message = str,
        IssueType = (IssueType) num
      });
    }
  }
}
