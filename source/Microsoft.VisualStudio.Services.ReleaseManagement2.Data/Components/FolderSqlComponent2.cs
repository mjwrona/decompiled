// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.FolderSqlComponent2
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class FolderSqlComponent2 : FolderSqlComponent
  {
    public override Folder UpdateFolder(
      Guid projectId,
      string path,
      Folder folder,
      Guid requestedBy,
      string originalSecurityToken,
      string newSecurityToken)
    {
      if (folder == null)
        return (Folder) null;
      this.PrepareStoredProcedure("Release.prc_UpdateFolder", projectId);
      this.BindString("@oldPath", PathHelper.UserToDBPath(path), 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@newPath", PathHelper.UserToDBPath(folder.Path), 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@description", folder.Description, 2048, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid("@requestedBy", requestedBy);
      this.BindString("@originalSecurityToken", originalSecurityToken, 465, false, SqlDbType.NVarChar);
      this.BindString("@newSecurityToken", newSecurityToken, 465, false, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Folder>((ObjectBinder<Folder>) new FolderBinder());
        return resultCollection.GetCurrent<Folder>().Items.FirstOrDefault<Folder>();
      }
    }
  }
}
