// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.RedeployTriggerDeploymentGroupEventBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "As per Vssf")]
  public class RedeployTriggerDeploymentGroupEventBinder : 
    ObjectBinder<RedeployTriggerDeploymentGroupEvent>
  {
    private SqlColumnBinder id = new SqlColumnBinder("Id");
    private SqlColumnBinder eventType = new SqlColumnBinder("EventType");
    private SqlColumnBinder deploymentGroupId = new SqlColumnBinder("DeploymentGroupId");
    private SqlColumnBinder targetId = new SqlColumnBinder("TargetId");
    private SqlColumnBinder tags = new SqlColumnBinder("Tags");
    private SqlColumnBinder modifiedOn = new SqlColumnBinder("ModifiedOn");
    private SqlColumnBinder environmentId = new SqlColumnBinder("EnvironmentId");
    private SqlColumnBinder isReadyForProcessing = new SqlColumnBinder("IsReadyForProcessing");
    private SqlColumnBinder metaInfo = new SqlColumnBinder("MetaInfo");
    private IVssRequestContext requestContext;
    private readonly string deserializeError = "REDEPLOYTRIGGER_TAGINFO_DESERIALIZE_ERROR - DeserializeError for tagInfo - {0}, error details - {1}";

    public RedeployTriggerDeploymentGroupEventBinder(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override RedeployTriggerDeploymentGroupEvent Bind() => new RedeployTriggerDeploymentGroupEvent()
    {
      Id = this.id.GetInt64((IDataReader) this.Reader),
      EventType = this.eventType.GetString((IDataReader) this.Reader, false),
      DeploymentGroupId = this.deploymentGroupId.GetInt32((IDataReader) this.Reader),
      TargetIds = (IList<int>) new List<int>()
      {
        this.targetId.GetInt32((IDataReader) this.Reader)
      },
      TagsInfo = this.ConvertStringToRedeployTriggerEventTagsInfo(this.tags.GetString((IDataReader) this.Reader, true)),
      ModifiedOn = this.modifiedOn.GetDateTime((IDataReader) this.Reader),
      IsReadyForProcessing = this.isReadyForProcessing.GetBoolean((IDataReader) this.Reader),
      EnvironmentId = this.environmentId.GetInt32((IDataReader) this.Reader),
      MetaInfo = this.metaInfo.GetString((IDataReader) this.Reader, true)
    };

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Binder should not throw")]
    private RedeployTriggerEventTagsInfo ConvertStringToRedeployTriggerEventTagsInfo(string value)
    {
      RedeployTriggerEventTagsInfo triggerEventTagsInfo = new RedeployTriggerEventTagsInfo();
      if (string.IsNullOrEmpty(value))
        return triggerEventTagsInfo;
      try
      {
        return ServerModelUtility.FromString<RedeployTriggerEventTagsInfo>(value);
      }
      catch (Exception ex)
      {
        this.requestContext.Trace(1972122, TraceLevel.Error, "ReleaseManagementService", "JobLayer", string.Format((IFormatProvider) CultureInfo.InvariantCulture, this.deserializeError, (object) value, (object) ex.Message));
        return triggerEventTagsInfo;
      }
    }
  }
}
