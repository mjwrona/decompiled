// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.DataAccess.CheckConfigurationBinder
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Data;

namespace Microsoft.Azure.Pipelines.Checks.Server.DataAccess
{
  internal class CheckConfigurationBinder : ObjectBinder<CheckConfiguration>
  {
    private SqlColumnBinder m_checkConfigurationId = new SqlColumnBinder("AssignmentId");
    private SqlColumnBinder m_settingsId = new SqlColumnBinder("ConfigurationId");
    private SqlColumnBinder m_typeId = new SqlColumnBinder("TypeId");
    private SqlColumnBinder m_settings = new SqlColumnBinder("Config");
    private SqlColumnBinder m_scope = new SqlColumnBinder("Scope");
    private SqlColumnBinder m_createdBy = new SqlColumnBinder("CreatedBy");
    private SqlColumnBinder m_createdOn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder m_modifiedBy = new SqlColumnBinder("ModifiedBy");
    private SqlColumnBinder m_modifiedOn = new SqlColumnBinder("ModifiedOn");

    protected override CheckConfiguration Bind()
    {
      string settings = this.m_settings.GetString((IDataReader) this.Reader, false);
      Resource resource = new Resource();
      string toDeserialize = this.m_scope.GetString((IDataReader) this.Reader, false);
      try
      {
        resource = JsonUtility.FromString<Resource>(toDeserialize);
      }
      catch
      {
      }
      Guid guid = this.m_typeId.GetGuid((IDataReader) this.Reader);
      CheckType type = new CheckType() { Id = guid };
      CheckConfiguration configurationObject = type.CreateCheckConfigurationObject();
      configurationObject.Id = this.m_checkConfigurationId.GetInt32((IDataReader) this.Reader);
      configurationObject.SettingsId = this.m_settingsId.GetInt32((IDataReader) this.Reader);
      configurationObject.Type = type;
      configurationObject.Resource = resource;
      configurationObject.CreatedBy = new IdentityRef()
      {
        Id = this.m_createdBy.GetGuid((IDataReader) this.Reader, false).ToString("D")
      };
      configurationObject.CreatedOn = this.m_createdOn.GetDateTime((IDataReader) this.Reader);
      configurationObject.ModifiedBy = new IdentityRef()
      {
        Id = this.m_modifiedBy.GetGuid((IDataReader) this.Reader, false).ToString("D")
      };
      configurationObject.ModifiedOn = this.m_modifiedOn.GetDateTime((IDataReader) this.Reader);
      configurationObject.PopulateSettings(settings);
      return configurationObject;
    }
  }
}
