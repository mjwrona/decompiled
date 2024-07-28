// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.PullRequestReleaseBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class PullRequestReleaseBinder : ReleaseManagementObjectBinderBase<PullRequestRelease>
  {
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder pullRequestId = new SqlColumnBinder("PullRequestId");
    private SqlColumnBinder releaseId = new SqlColumnBinder("ReleaseId");
    private SqlColumnBinder mergeCommitId = new SqlColumnBinder("MergeCommitId");
    private SqlColumnBinder iterationId = new SqlColumnBinder("IterationId");
    private SqlColumnBinder mergedAt = new SqlColumnBinder("MergedAt");
    private SqlColumnBinder isActive = new SqlColumnBinder("IsActive");

    public PullRequestReleaseBinder(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override PullRequestRelease Bind()
    {
      Guid guid = this.dataspaceId.ColumnExists((IDataReader) this.Reader) ? this.SqlComponent.GetDataspaceIdentifier(this.dataspaceId.GetInt32((IDataReader) this.Reader)) : Guid.Empty;
      return new PullRequestRelease()
      {
        ProjectId = guid,
        PullRequestId = this.pullRequestId.GetInt32((IDataReader) this.Reader),
        ReleaseId = this.releaseId.GetInt32((IDataReader) this.Reader),
        MergeCommitId = this.mergeCommitId.GetString((IDataReader) this.Reader, false),
        IterationId = this.iterationId.GetInt32((IDataReader) this.Reader),
        MergedAt = this.mergedAt.GetDateTime((IDataReader) this.Reader),
        IsActive = this.isActive.GetBoolean((IDataReader) this.Reader, true, true)
      };
    }
  }
}
