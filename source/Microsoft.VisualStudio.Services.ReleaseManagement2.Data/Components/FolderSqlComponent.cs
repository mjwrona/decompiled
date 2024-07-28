// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.FolderSqlComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class FolderSqlComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<FolderSqlComponent>(1),
      (IComponentCreator) new ComponentCreator<FolderSqlComponent2>(2)
    }, "ReleaseManagementFolder", "ReleaseManagement");

    public FolderSqlComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public Folder AddFolder(Guid projectId, string path, Folder folder, Guid requestedBy)
    {
      if (folder == null)
        return (Folder) null;
      this.PrepareStoredProcedure("Release.prc_AddFolder", projectId);
      this.BindString("@path", PathHelper.UserToDBPath(path), 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@description", folder.Description, 2048, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid("@requestedBy", requestedBy);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Folder>((ObjectBinder<Folder>) new FolderBinder());
        return resultCollection.GetCurrent<Folder>().Items.FirstOrDefault<Folder>();
      }
    }

    public IList<Folder> GetFolders(Guid projectId, string path, FolderPathQueryOrder queryOrder)
    {
      this.PrepareStoredProcedure("Release.prc_GetFolders", projectId);
      this.BindString("@path", PathHelper.UserToDBPath(path), 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("@queryOrder", (int) queryOrder);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Folder>((ObjectBinder<Folder>) new FolderBinder());
        return (IList<Folder>) resultCollection.GetCurrent<Folder>().Items;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for user")]
    public virtual Folder UpdateFolder(
      Guid projectId,
      string path,
      Folder folder,
      Guid requestedBy,
      string originalSecurityToken = null,
      string newSecurityToken = null)
    {
      if (folder == null)
        return (Folder) null;
      this.PrepareStoredProcedure("Release.prc_UpdateFolder", projectId);
      this.BindString("@oldPath", PathHelper.UserToDBPath(path), 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@newPath", PathHelper.UserToDBPath(folder.Path), 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@description", folder.Description, 2048, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid("@requestedBy", requestedBy);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Folder>((ObjectBinder<Folder>) new FolderBinder());
        return resultCollection.GetCurrent<Folder>().Items.FirstOrDefault<Folder>();
      }
    }

    public void DeleteFolder(Guid projectId, string path, Guid requestedBy)
    {
      this.PrepareStoredProcedure("Release.prc_DeleteFolder", projectId);
      this.BindString("@path", PathHelper.UserToDBPath(path), 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("@requestedBy", requestedBy);
      this.ExecuteNonQuery();
    }
  }
}
