// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseManagementJobInfoSqlComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class ReleaseManagementJobInfoSqlComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<ReleaseManagementJobInfoSqlComponent>(1)
    }, "ReleaseManagementJobInfo", "ReleaseManagement");

    public ReleaseManagementJobInfo GetReleaseManagementJobInfo(
      Guid projectId,
      string jobName,
      bool createIfNotExists)
    {
      this.PrepareStoredProcedure("Release.prc_GetJobInfo", projectId);
      this.BindString("@jobName", jobName, 200, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindBoolean("@createIfNotExists", createIfNotExists);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseManagementJobInfo>((ObjectBinder<ReleaseManagementJobInfo>) new ReleaseManagementJobInfoBinder((ReleaseManagementSqlResourceComponentBase) this));
        return resultCollection.GetCurrent<ReleaseManagementJobInfo>().Items.FirstOrDefault<ReleaseManagementJobInfo>();
      }
    }
  }
}
