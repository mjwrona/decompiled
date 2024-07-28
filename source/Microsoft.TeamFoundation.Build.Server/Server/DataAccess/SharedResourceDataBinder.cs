// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.SharedResourceDataBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class SharedResourceDataBinder : BuildObjectBinder<SharedResourceData>
  {
    private SqlColumnBinder resourceNameColumn = new SqlColumnBinder("ResourceName");
    private SqlColumnBinder instanceIdColumn = new SqlColumnBinder("InstanceId");
    private SqlColumnBinder buildIdColumn = new SqlColumnBinder("BuildId");
    private SqlColumnBinder requestBuildUriColumn = new SqlColumnBinder("RequestBuildUri");
    private SqlColumnBinder messageQueueUrlColumn = new SqlColumnBinder("MessageQueueUrl");
    private SqlColumnBinder endpointUrlColumn = new SqlColumnBinder("EndpointUrl");
    private SqlColumnBinder lockedByColumn = new SqlColumnBinder("LockedBy");

    protected override SharedResourceData Bind()
    {
      string str1 = this.resourceNameColumn.GetString((IDataReader) this.Reader, false);
      string str2 = this.instanceIdColumn.GetString((IDataReader) this.Reader, false);
      int int32 = this.buildIdColumn.GetInt32((IDataReader) this.Reader, -1);
      string uriString = this.requestBuildUriColumn.GetString((IDataReader) this.Reader, true);
      string serverUrl = DBHelper.DBUrlToServerUrl(this.messageQueueUrlColumn.GetString((IDataReader) this.Reader, false));
      string str3 = this.lockedByColumn.ColumnExists((IDataReader) this.Reader) ? this.lockedByColumn.GetString((IDataReader) this.Reader, false) : (string) null;
      Uri uri = (Uri) null;
      if (string.IsNullOrEmpty(uriString))
      {
        if (int32 > 0)
          uri = new Uri(DBHelper.CreateArtifactUri("Build", int32));
      }
      else
        uri = new Uri(uriString);
      return new SharedResourceData()
      {
        BuildUri = uri,
        InstanceId = str2,
        ResourceName = str1,
        MessageQueueUrl = serverUrl,
        EndpointUrl = this.endpointUrlColumn.ColumnExists((IDataReader) this.Reader) ? DBHelper.DBUrlToServerUrl(this.endpointUrlColumn.GetString((IDataReader) this.Reader, false)) : (string) null,
        LockedBy = str3
      };
    }
  }
}
