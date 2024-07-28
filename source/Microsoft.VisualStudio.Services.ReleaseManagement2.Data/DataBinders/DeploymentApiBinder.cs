// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.DeploymentApiBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class DeploymentApiBinder : ReleaseManagementObjectBinderBase<Deployment>
  {
    private SqlColumnBinder releaseDefinitionId = new SqlColumnBinder("ReleaseDefinitionId");
    private SqlColumnBinder definitionName = new SqlColumnBinder("DefinitionName");
    private SqlColumnBinder releaseId = new SqlColumnBinder("ReleaseId");
    private SqlColumnBinder releaseName = new SqlColumnBinder("ReleaseName");
    private SqlColumnBinder definitionEnvironmentId = new SqlColumnBinder("DefinitionEnvironmentId");
    private SqlColumnBinder releaseEnvironmentId = new SqlColumnBinder("ReleaseEnvironmentId");
    private SqlColumnBinder releaseEnvironmentName = new SqlColumnBinder("ReleaseEnvironmentName");
    private SqlColumnBinder id = new SqlColumnBinder("Id");
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder attempt = new SqlColumnBinder("Attempt");
    private SqlColumnBinder deploymentReason = new SqlColumnBinder("Reason");
    private SqlColumnBinder deploymentStatus = new SqlColumnBinder("DeploymentStatus");
    private SqlColumnBinder operationStatus = new SqlColumnBinder("OperationStatus");
    private SqlColumnBinder requestedBy = new SqlColumnBinder("RequestedBy");
    private SqlColumnBinder startedTime = new SqlColumnBinder("StartedTime");
    private SqlColumnBinder modifiedOn = new SqlColumnBinder("LastModifiedOn");
    private SqlColumnBinder modifiedBy = new SqlColumnBinder("LastModifiedBy");
    private SqlColumnBinder conditions = new SqlColumnBinder("Conditions");
    private SqlColumnBinder scheduledDeploymentTime = new SqlColumnBinder("ScheduledDeploymentTime");

    public DeploymentApiBinder(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override Deployment Bind()
    {
      Guid guid = this.dataspaceId.ColumnExists((IDataReader) this.Reader) ? this.SqlComponent.GetDataspaceIdentifier(this.dataspaceId.GetInt32((IDataReader) this.Reader)) : Guid.Empty;
      DateTime dateTime = this.scheduledDeploymentTime.GetDateTime((IDataReader) this.Reader, SqlDateTime.MinValue.Value);
      string truncatedJson = this.conditions.GetString((IDataReader) this.Reader, (string) null);
      Deployment deployment = new Deployment()
      {
        Id = this.id.GetInt32((IDataReader) this.Reader),
        ProjectId = guid,
        ReleaseDefinitionId = this.releaseDefinitionId.GetInt32((IDataReader) this.Reader),
        ReleaseDefinitionName = this.definitionName.GetString((IDataReader) this.Reader, false),
        ReleaseId = this.releaseId.GetInt32((IDataReader) this.Reader),
        ReleaseName = this.releaseName.GetString((IDataReader) this.Reader, false),
        DefinitionEnvironmentId = this.definitionEnvironmentId.GetInt32((IDataReader) this.Reader),
        ReleaseEnvironmentId = this.releaseEnvironmentId.GetInt32((IDataReader) this.Reader),
        ReleaseEnvironmentName = this.releaseEnvironmentName.GetString((IDataReader) this.Reader, false),
        Attempt = this.attempt.GetInt32((IDataReader) this.Reader),
        RequestedBy = this.requestedBy.GetGuid((IDataReader) this.Reader),
        Reason = (DeploymentReason) this.deploymentReason.GetByte((IDataReader) this.Reader),
        LastModifiedOn = this.modifiedOn.GetDateTime((IDataReader) this.Reader),
        Status = (DeploymentStatus) this.deploymentStatus.GetByte((IDataReader) this.Reader),
        OperationStatus = (DeploymentOperationStatus) this.operationStatus.GetInt32((IDataReader) this.Reader),
        LastModifiedBy = this.modifiedBy.GetGuid((IDataReader) this.Reader)
      };
      this.FillQueuedOn(deployment);
      DateTime? nullable = dateTime == DateTime.MinValue ? new DateTime?() : new DateTime?(dateTime);
      if (nullable.HasValue)
        deployment.ScheduledDeploymentTime = nullable.Value;
      if (truncatedJson != null)
      {
        try
        {
          deployment.Conditions = JsonConvert.DeserializeObject<IList<ReleaseCondition>>(truncatedJson);
        }
        catch (Exception ex) when (ex is JsonReaderException || ex is JsonSerializationException)
        {
          deployment.Conditions = (IList<ReleaseCondition>) DeploymentApiBinder.ParseInvalidJson(truncatedJson);
        }
      }
      else
        deployment.Conditions = (IList<ReleaseCondition>) null;
      return deployment;
    }

    protected virtual void FillQueuedOn(Deployment deployment)
    {
      if (deployment == null)
        throw new ArgumentNullException(nameof (deployment));
      deployment.QueuedOn = this.startedTime.GetDateTime((IDataReader) this.Reader);
      deployment.StartedOn = deployment.QueuedOn;
    }

    private static List<ReleaseCondition> ParseInvalidJson(string truncatedJson) => JsonUtilities.DeserializeTruncatedJson(truncatedJson).ToObject<List<ReleaseCondition>>();
  }
}
