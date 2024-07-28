// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyEvaluationRecordBinder
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Policy.Server
{
  internal class PolicyEvaluationRecordBinder : ObjectBinder<PolicyEvaluationRecord>
  {
    internal SqlColumnBinder m_policyTypeId = new SqlColumnBinder("PolicyType");
    internal SqlColumnBinder m_policyConfigurationId = new SqlColumnBinder("PolicyConfigurationId");
    internal SqlColumnBinder m_versionId = new SqlColumnBinder("PolicyConfigurationVersion");
    internal SqlColumnBinder m_evaluationId = new SqlColumnBinder("PolicyEvaluationId");
    internal SqlColumnBinder m_artifactId = new SqlColumnBinder("ArtifactId");
    internal SqlColumnBinder m_startedDate = new SqlColumnBinder("StartedDate");
    internal SqlColumnBinder m_completedDate = new SqlColumnBinder("CompletedDate");
    internal SqlColumnBinder m_status = new SqlColumnBinder("Status");
    internal SqlColumnBinder m_context = new SqlColumnBinder("Context");
    internal SqlColumnBinder m_createdById = new SqlColumnBinder("CreatedById");
    internal SqlColumnBinder m_createdDate = new SqlColumnBinder("DateCreated");
    internal SqlColumnBinder m_isEnabled = new SqlColumnBinder("IsEnabled");
    internal SqlColumnBinder m_isBlocking = new SqlColumnBinder("IsBlocking");
    internal SqlColumnBinder m_isDeleted = new SqlColumnBinder("IsDeleted");
    internal SqlColumnBinder m_settings = new SqlColumnBinder("Settings");
    internal SqlColumnBinder m_isEnterpriseManaged = new SqlColumnBinder("IsEnterpriseManaged");
    private readonly ISecuredObject m_securedObject;

    public PolicyEvaluationRecordBinder()
      : this((ISecuredObject) null)
    {
    }

    public PolicyEvaluationRecordBinder(ISecuredObject securedObject) => this.m_securedObject = securedObject;

    protected override PolicyEvaluationRecord Bind()
    {
      string json = this.m_context.GetString((IDataReader) this.Reader, true);
      PolicyEvaluationRecord evaluationRecord1 = new PolicyEvaluationRecord(this.m_securedObject);
      PolicyConfiguration policyConfiguration = new PolicyConfiguration(this.m_securedObject);
      IdentityRef identityRef;
      if (!this.m_createdById.ColumnExists((IDataReader) this.Reader))
      {
        identityRef = (IdentityRef) null;
      }
      else
      {
        identityRef = new IdentityRef();
        identityRef.Id = this.m_createdById.GetGuid((IDataReader) this.Reader).ToString();
      }
      policyConfiguration.CreatedBy = identityRef;
      policyConfiguration.CreatedDate = this.m_createdDate.ColumnExists((IDataReader) this.Reader) ? this.m_createdDate.GetDateTime((IDataReader) this.Reader) : DateTime.MinValue;
      policyConfiguration.Id = this.m_policyConfigurationId.GetInt32((IDataReader) this.Reader, 0);
      policyConfiguration.Revision = this.m_versionId.GetInt32((IDataReader) this.Reader, 0);
      policyConfiguration.IsEnabled = !this.m_isEnabled.ColumnExists((IDataReader) this.Reader) || this.m_isEnabled.GetBoolean((IDataReader) this.Reader);
      policyConfiguration.IsBlocking = !this.m_isBlocking.ColumnExists((IDataReader) this.Reader) || this.m_isBlocking.GetBoolean((IDataReader) this.Reader);
      policyConfiguration.IsDeleted = this.m_isDeleted.ColumnExists((IDataReader) this.Reader) && this.m_isDeleted.GetBoolean((IDataReader) this.Reader);
      policyConfiguration.Settings = this.m_settings.ColumnExists((IDataReader) this.Reader) ? JObject.Parse(this.m_settings.GetString((IDataReader) this.Reader, false)) : new JObject();
      policyConfiguration.IsEnterpriseManaged = this.m_isEnterpriseManaged.ColumnExists((IDataReader) this.Reader) && this.m_isEnterpriseManaged.GetBoolean((IDataReader) this.Reader);
      policyConfiguration.Type = new PolicyTypeRef(this.m_securedObject)
      {
        Id = this.m_policyTypeId.GetGuid((IDataReader) this.Reader, false)
      };
      evaluationRecord1.Configuration = policyConfiguration;
      evaluationRecord1.ArtifactId = this.m_artifactId.GetString((IDataReader) this.Reader, false);
      evaluationRecord1.EvaluationId = this.m_evaluationId.ColumnExists((IDataReader) this.Reader) ? this.m_evaluationId.GetGuid((IDataReader) this.Reader, true) : Guid.Empty;
      evaluationRecord1.StartedDate = this.m_startedDate.GetDateTime((IDataReader) this.Reader);
      evaluationRecord1.CompletedDate = new DateTime?(this.m_completedDate.GetDateTime((IDataReader) this.Reader));
      evaluationRecord1.Status = new PolicyEvaluationStatus?((PolicyEvaluationStatus) this.m_status.GetByte((IDataReader) this.Reader, (byte) 0));
      evaluationRecord1.Context = json != null ? JObject.Parse(json) : (JObject) null;
      PolicyEvaluationRecord evaluationRecord2 = evaluationRecord1;
      if (evaluationRecord2.CompletedDate.Value == new DateTime())
        evaluationRecord2.CompletedDate = new DateTime?();
      return evaluationRecord2;
    }
  }
}
