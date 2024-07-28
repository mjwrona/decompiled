// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.SecureFileBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class SecureFileBinder : ObjectBinder<SecureFile>
  {
    private SqlColumnBinder m_secureFileId = new SqlColumnBinder("Id");
    private SqlColumnBinder m_secureFileName = new SqlColumnBinder("Name");
    private SqlColumnBinder m_secureFileProperties = new SqlColumnBinder("Properties");
    private SqlColumnBinder m_createdBy = new SqlColumnBinder("CreatedBy");
    private SqlColumnBinder m_createdOn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder m_modifiedBy = new SqlColumnBinder("ModifiedBy");
    private SqlColumnBinder m_modifiedOn = new SqlColumnBinder("ModifiedOn");

    protected override SecureFile Bind() => new SecureFile()
    {
      Id = this.m_secureFileId.GetGuid((IDataReader) this.Reader),
      Name = this.m_secureFileName.GetString((IDataReader) this.Reader, false),
      CreatedBy = new IdentityRef()
      {
        Id = this.m_createdBy.GetGuid((IDataReader) this.Reader, false).ToString("D")
      },
      CreatedOn = this.m_createdOn.GetDateTime((IDataReader) this.Reader),
      ModifiedBy = new IdentityRef()
      {
        Id = this.m_modifiedBy.GetGuid((IDataReader) this.Reader, false).ToString("D")
      },
      ModifiedOn = this.m_modifiedOn.GetDateTime((IDataReader) this.Reader),
      Properties = JsonUtility.FromString<IDictionary<string, string>>(this.m_secureFileProperties.GetString((IDataReader) this.Reader, true))
    };
  }
}
