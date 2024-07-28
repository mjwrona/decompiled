// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildServiceHost2010Binder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  internal sealed class BuildServiceHost2010Binder : BuildObjectBinder<BuildServiceHost2010>
  {
    private SqlColumnBinder serviceHostId = new SqlColumnBinder("ServiceHostId");
    private SqlColumnBinder displayName = new SqlColumnBinder("DisplayName");
    private SqlColumnBinder baseUrl = new SqlColumnBinder("BaseUrl");
    private SqlColumnBinder messageQueueUrl = new SqlColumnBinder("MessageQueueUrl");
    private SqlColumnBinder requireClientCertificates = new SqlColumnBinder("RequireClientCertificates");
    private SqlColumnBinder status = new SqlColumnBinder("Status");
    private SqlColumnBinder statusChangedOn = new SqlColumnBinder("StatusChangedOn");
    private SqlColumnBinder m_isVirtual = new SqlColumnBinder("IsElastic");

    public BuildServiceHost2010Binder()
    {
    }

    public BuildServiceHost2010Binder(SqlDataReader dataReader, string procedureName)
      : base(dataReader, procedureName)
    {
    }

    protected override BuildServiceHost2010 Bind()
    {
      BuildServiceHost2010 buildServiceHost2010 = new BuildServiceHost2010();
      buildServiceHost2010.Uri = this.serviceHostId.GetArtifactUriFromInt32(this.Reader, "ServiceHost", false);
      buildServiceHost2010.Name = this.displayName.GetBuildItem(this.Reader, false);
      buildServiceHost2010.BaseUrl = DBHelper.DBUrlToServerUrl(this.baseUrl.GetString((IDataReader) this.Reader, false));
      this.messageQueueUrl.GetString((IDataReader) this.Reader, false);
      buildServiceHost2010.RequireClientCertificates = this.requireClientCertificates.GetBoolean((IDataReader) this.Reader);
      int num = (int) this.status.GetByte((IDataReader) this.Reader);
      this.statusChangedOn.GetDateTime((IDataReader) this.Reader);
      buildServiceHost2010.IsVirtual = this.m_isVirtual.ColumnExists((IDataReader) this.Reader) && this.m_isVirtual.GetBoolean((IDataReader) this.Reader);
      return buildServiceHost2010;
    }
  }
}
