// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.DownloadTaskTraceabilityErrorBinder
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Utilities;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  internal class DownloadTaskTraceabilityErrorBinder : ObjectBinder<DownloadTaskTraceabilityError>
  {
    private SqlColumnBinder m_Status = new SqlColumnBinder("Status");

    protected override DownloadTaskTraceabilityError Bind()
    {
      int int32 = this.m_Status.GetInt32((IDataReader) this.Reader, -1, -1);
      return Enum.IsDefined(typeof (DownloadTaskTraceabilityError), (object) int32) ? (DownloadTaskTraceabilityError) int32 : DownloadTaskTraceabilityError.UnknownError;
    }
  }
}
