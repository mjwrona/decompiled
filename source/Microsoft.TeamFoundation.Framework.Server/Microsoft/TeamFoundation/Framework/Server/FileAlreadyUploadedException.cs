// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileAlreadyUploadedException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class FileAlreadyUploadedException : TeamFoundationServiceException
  {
    public FileAlreadyUploadedException(long fileId)
      : base(FrameworkResources.FileAlreadyUploadedException((object) fileId))
    {
      this.EventId = TeamFoundationEventId.FileAlreadyUploadedException;
    }

    public FileAlreadyUploadedException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractLong(sqlError, "fileId"))
    {
      this.EventId = TeamFoundationEventId.FileAlreadyUploadedException;
    }
  }
}
