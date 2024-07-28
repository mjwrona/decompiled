// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyConfigurationBinder
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Policy.Server
{
  internal class PolicyConfigurationBinder : ObjectBinder<PolicyConfigurationRecord>
  {
    internal SqlColumnBinder m_policyConfigurationId = new SqlColumnBinder("PolicyConfigurationId");
    internal SqlColumnBinder m_versionId = new SqlColumnBinder("VersionId");
    internal SqlColumnBinder m_typeId = new SqlColumnBinder("PolicyType");
    internal SqlColumnBinder m_scope = new SqlColumnBinder("Scope");
    internal SqlColumnBinder m_isEnabled = new SqlColumnBinder("IsEnabled");
    internal SqlColumnBinder m_isBlocking = new SqlColumnBinder("IsBlocking");
    internal SqlColumnBinder m_settings = new SqlColumnBinder("Settings");
    internal SqlColumnBinder m_creatorId = new SqlColumnBinder("CreatedById");
    internal SqlColumnBinder m_creationDate = new SqlColumnBinder("DateCreated");
    internal SqlColumnBinder m_isDeleted = new SqlColumnBinder("IsDeleted");
    internal SqlColumnBinder m_isEnterpriseManaged = new SqlColumnBinder("IsEnterpriseManaged");
    private Guid m_projectId;

    internal PolicyConfigurationBinder(Guid projectId) => this.m_projectId = projectId;

    protected override PolicyConfigurationRecord Bind()
    {
      int int32_1 = this.m_versionId.GetInt32((IDataReader) this.Reader);
      int int32_2 = this.m_policyConfigurationId.GetInt32((IDataReader) this.Reader);
      Guid guid1 = this.m_typeId.GetGuid((IDataReader) this.Reader);
      Guid projectId = this.m_projectId;
      Guid guid2 = this.m_creatorId.GetGuid((IDataReader) this.Reader);
      DateTime dateTime = this.m_creationDate.GetDateTime((IDataReader) this.Reader);
      int num1 = this.m_isEnabled.GetBoolean((IDataReader) this.Reader) ? 1 : 0;
      int num2 = this.m_isBlocking.GetBoolean((IDataReader) this.Reader) ? 1 : 0;
      bool boolean = this.m_isDeleted.GetBoolean((IDataReader) this.Reader);
      string str = this.m_settings.GetString((IDataReader) this.Reader, false);
      int num3 = this.m_isEnterpriseManaged.ColumnExists((IDataReader) this.Reader) ? (this.m_isEnterpriseManaged.GetBoolean((IDataReader) this.Reader) ? 1 : 0) : 0;
      string settings = str;
      Guid creatorId = guid2;
      DateTime creationDate = dateTime;
      int num4 = boolean ? 1 : 0;
      return new PolicyConfigurationRecord(int32_1, int32_2, guid1, projectId, num1 != 0, num2 != 0, num3 != 0, settings, creatorId, creationDate, num4 != 0);
    }
  }
}
