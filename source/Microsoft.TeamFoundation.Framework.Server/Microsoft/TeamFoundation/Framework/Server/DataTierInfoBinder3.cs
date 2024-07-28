// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataTierInfoBinder3
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Data;
using System.Security;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DataTierInfoBinder3 : DataTierInfoBinder2
  {
    protected SqlColumnBinder SigningKeyId = new SqlColumnBinder(nameof (SigningKeyId));

    internal DataTierInfoBinder3(ISqlConnectionInfo connectionInfo)
      : base(connectionInfo)
    {
    }

    protected override DataTierInfo Bind()
    {
      string connectionString = this.ConnectionString.GetString((IDataReader) this.Reader, false);
      DataTierState state = (DataTierState) Enum.ToObject(typeof (DataTierState), this.State.GetInt32((IDataReader) this.Reader));
      string tags = this.Tags.GetString((IDataReader) this.Reader, true);
      string userName = this.UserId.GetString((IDataReader) this.Reader, true);
      byte[] bytes = this.Password.GetBytes((IDataReader) this.Reader, true);
      ISqlConnectionInfo connectionInfo = (ISqlConnectionInfo) null;
      Guid guid = this.SigningKeyId.GetGuid((IDataReader) this.Reader);
      if (!SqlConnectionInfoFactory.TryCreate(connectionString, userName, bytes, out connectionInfo))
      {
        SecureString password = (SecureString) null;
        if (!string.IsNullOrEmpty(userName) && bytes != null && bytes.Length != 0)
          password = TeamFoundationSigningService.DecryptRaw(this.m_connectionInfo, guid, bytes, SigningAlgorithm.SHA256).ToSecureString();
        connectionInfo = SqlConnectionInfoFactory.Create(connectionString, userName, password, bytes);
      }
      return new DataTierInfo(connectionInfo, state, tags, guid);
    }
  }
}
