// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildServiceHostBinder2
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class BuildServiceHostBinder2 : BuildObjectBinder<BuildServiceHost>
  {
    private SqlColumnBinder serviceHostId = new SqlColumnBinder("ServiceHostId");
    private SqlColumnBinder displayName = new SqlColumnBinder("DisplayName");
    private SqlColumnBinder baseUrl = new SqlColumnBinder("BaseUrl");
    private SqlColumnBinder messageQueueUrl = new SqlColumnBinder("MessageQueueUrl");
    private SqlColumnBinder requireClientCertificates = new SqlColumnBinder("RequireClientCertificates");
    private SqlColumnBinder status = new SqlColumnBinder("Status");
    private SqlColumnBinder statusChangedOn = new SqlColumnBinder("StatusChangedOn");
    private SqlColumnBinder acquiredOn = new SqlColumnBinder("AcquiredOn");
    private SqlColumnBinder connectedOn = new SqlColumnBinder("ConnectedOn");
    private SqlColumnBinder isElastic = new SqlColumnBinder("IsElastic");
    private SqlColumnBinder isAllocationPending = new SqlColumnBinder("IsAllocationPending");
    private SqlColumnBinder serviceIdentityId = new SqlColumnBinder("ServiceIdentityId");
    private SqlColumnBinder ownerSessionId = new SqlColumnBinder("OwnerSessionId");

    protected override BuildServiceHost Bind() => new BuildServiceHost()
    {
      Uri = this.serviceHostId.GetArtifactUriFromInt32(this.Reader, "ServiceHost", false),
      Name = this.displayName.GetBuildItem(this.Reader, false),
      BaseUrl = DBHelper.DBUrlToServerUrl(this.baseUrl.GetString((IDataReader) this.Reader, false)),
      MessageQueueUrl = DBHelper.DBUrlToServerUrl(this.messageQueueUrl.GetString((IDataReader) this.Reader, false)),
      RequireClientCertificates = this.requireClientCertificates.GetBoolean((IDataReader) this.Reader),
      Status = (ServiceHostStatus) this.status.GetByte((IDataReader) this.Reader),
      StatusChangedOn = this.statusChangedOn.GetDateTime((IDataReader) this.Reader),
      AcquiredOn = this.acquiredOn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue),
      ConnectedOn = this.connectedOn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue),
      IsVirtual = this.isElastic.GetBoolean((IDataReader) this.Reader),
      IsAllocationPending = this.isAllocationPending.GetBoolean((IDataReader) this.Reader),
      ServiceIdentityId = this.serviceIdentityId.GetGuid((IDataReader) this.Reader, true),
      OwnerSessionId = this.ownerSessionId.GetGuid((IDataReader) this.Reader, true)
    };
  }
}
