// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.VariableGroupBinder
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
  internal sealed class VariableGroupBinder : ObjectBinder<VariableGroup>
  {
    private SqlColumnBinder m_groupId = new SqlColumnBinder("Id");
    private SqlColumnBinder m_groupName = new SqlColumnBinder("Name");
    private SqlColumnBinder m_description = new SqlColumnBinder("Description");
    private SqlColumnBinder m_createdBy = new SqlColumnBinder("CreatedBy");
    private SqlColumnBinder m_createdOn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder m_modifiedBy = new SqlColumnBinder("ModifiedBy");
    private SqlColumnBinder m_modifiedOn = new SqlColumnBinder("ModifiedOn");
    private SqlColumnBinder m_variables = new SqlColumnBinder("Variables");

    protected override VariableGroup Bind() => new VariableGroup()
    {
      Type = "Vsts",
      Id = this.m_groupId.GetInt32((IDataReader) this.Reader),
      Name = this.m_groupName.GetString((IDataReader) this.Reader, false),
      Description = this.m_description.GetString((IDataReader) this.Reader, true),
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
      Variables = JsonUtility.FromString<IDictionary<string, VariableValue>>(this.m_variables.GetString((IDataReader) this.Reader, false))
    };
  }
}
