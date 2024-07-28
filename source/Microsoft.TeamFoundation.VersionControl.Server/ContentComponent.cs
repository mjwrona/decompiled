// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ContentComponent
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ContentComponent : VersionControlSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<ContentComponent>(1, true),
      (IComponentCreator) new ComponentCreator<ContentComponent2>(2),
      (IComponentCreator) new ComponentCreator<ContentComponent3>(3),
      (IComponentCreator) new ComponentCreator<ContentComponent4>(4)
    }, "VCContent");

    public virtual PreUploadFileResult PreUploadFile(Workspace workspace, string targetServerItem)
    {
      this.PrepareStoredProcedure("prc_PreUploadFile");
      this.BindGuid("@teamFoundationId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindServerItem("@targetServerItem", targetServerItem, true);
      return new PreUploadFileResult((int) this.ExecuteScalar());
    }

    public virtual void PostUploadFile(
      Workspace workspace,
      string targetServerItem,
      int fileId,
      bool isLastChunk)
    {
      this.PrepareStoredProcedure("prc_PostUploadFile");
      this.BindGuid("@teamFoundationId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindServerItem("@targetServerItem", targetServerItem, true);
      this.BindInt("@fileId", fileId);
      this.BindBoolean("@isLastChunk", isLastChunk);
      this.ExecuteNonQuery();
    }
  }
}
