// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CreateBranchObjectException
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public class CreateBranchObjectException : ServerException
  {
    private string m_message;

    public CreateBranchObjectException(string message)
      : base(message)
    {
      this.m_message = message;
    }

    public CreateBranchObjectException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : base(string.Empty)
    {
      string dataspaceServerItem1 = ServerException.ExtractDataspaceServerItem(requestContext, sqlError, "item");
      foreach (int num in TeamFoundationServiceException.ExtractInts(sqlError, "error"))
      {
        switch (num)
        {
          case 500170:
            this.m_message = Resources.Format("BranchParentCycle", (object) dataspaceServerItem1);
            break;
          case 500171:
            this.m_message = Resources.Format("BranchParentDoesNotExist", (object) dataspaceServerItem1);
            break;
          case 500172:
            this.m_message = Resources.Format("BranchParentNoMergeRelationship", (object) dataspaceServerItem1);
            break;
          case 500173:
            this.m_message = Resources.Format("BranchParentMaximumDepthExceeded", (object) ServerException.ExtractDataspaceServerItem(requestContext, sqlError, "oitem"));
            break;
          case 500174:
            this.m_message = Resources.Format("CannotUpdateBranchDoesNotExist", (object) dataspaceServerItem1);
            break;
          case 500175:
            this.m_message = Resources.Format("CannotCreateBranchAlreadyExists", (object) dataspaceServerItem1);
            break;
          case 500178:
            string dataspaceServerItem2 = ServerException.ExtractDataspaceServerItem(requestContext, sqlError, "oitem");
            this.m_message = Resources.Format("BranchObjectParentChildAlreadyExists", (object) dataspaceServerItem1, (object) dataspaceServerItem2);
            break;
          case 500188:
            this.m_message = Resources.Format("BranchObjectNotRootOfOperationException", (object) dataspaceServerItem1);
            break;
          case 500232:
            this.m_message = Resources.Format("BranchParentNotFound", (object) dataspaceServerItem1);
            break;
        }
        if (!string.IsNullOrEmpty(this.m_message))
          break;
      }
    }

    public override string Message => this.m_message;
  }
}
