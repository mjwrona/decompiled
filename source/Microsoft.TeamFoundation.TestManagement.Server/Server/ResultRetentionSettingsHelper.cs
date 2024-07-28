// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ResultRetentionSettingsHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class ResultRetentionSettingsHelper : RestApiHelper
  {
    private GuidAndString _projectId;
    private Func<string, IdentityRef> _toIdentityRef;

    public ResultRetentionSettingsHelper(
      TestManagementRequestContext requestContext,
      GuidAndString projectId,
      Func<string, IdentityRef> toIdentityRef = null)
      : base(requestContext)
    {
      this._projectId = projectId;
      this._toIdentityRef = toIdentityRef;
    }

    public ResultRetentionSettings Get() => this.ExecuteAction<ResultRetentionSettings>("ResultRetentionSettingsHelper.Get", (Func<ResultRetentionSettings>) (() => this.ConvertIdentityReference(this.RequestContext.GetService<IResultRetentionSettingsService>().Get(this.TestManagementRequestContext, this._projectId))), 1015065, "TestManagement");

    public ResultRetentionSettings Update(ResultRetentionSettings settings)
    {
      this.ValidateSettings(settings);
      return this.ExecuteAction<ResultRetentionSettings>("ResultRetentionSettingsHelper.Update", (Func<ResultRetentionSettings>) (() =>
      {
        ResultRetentionSettings retentionSettings = this.ConvertIdentityReference(this.RequestContext.GetService<IResultRetentionSettingsService>().Update(this.TestManagementRequestContext, this._projectId, settings));
        this.PublishTelemetryData("UpdateResultRetentionSettings", settings);
        return retentionSettings;
      }), 1015065, "TestManagement");
    }

    private ResultRetentionSettings ConvertIdentityReference(
      ResultRetentionSettings resultRetentionSettings)
    {
      if (resultRetentionSettings.LastUpdatedBy != null)
        resultRetentionSettings.LastUpdatedBy = this._toIdentityRef == null ? this.ToIdentityRef(resultRetentionSettings.LastUpdatedBy.Id) : this._toIdentityRef(resultRetentionSettings.LastUpdatedBy.Id);
      return resultRetentionSettings;
    }

    private void ValidateSettings(ResultRetentionSettings settings)
    {
      ArgumentUtility.CheckForNull<ResultRetentionSettings>(settings, nameof (settings), "Test Results");
      this.ValidateDuration(settings.AutomatedResultsRetentionDuration);
      this.ValidateDuration(settings.ManualResultsRetentionDuration);
    }

    private void ValidateDuration(int duration)
    {
      if (duration != -1 && (duration < 1 || duration > 10000))
        throw new InvalidPropertyException(nameof (duration), ServerResources.ResultRetentionDurationValidationError);
    }

    private void PublishTelemetryData(string actionName, ResultRetentionSettings settings)
    {
      CustomerIntelligenceData cid = new CustomerIntelligenceData();
      cid.Add(actionName, (object) new ResultRetentionSettingsHelper.TelemetryData()
      {
        AutomatedResultsRetentionDuration = settings.AutomatedResultsRetentionDuration,
        ManualResultsRetentionDuration = settings.ManualResultsRetentionDuration
      });
      this.TelemetryLogger.PublishData(this.RequestContext, "ResultsRetentionSettings", cid);
    }

    private class TelemetryData
    {
      public int AutomatedResultsRetentionDuration;
      public int ManualResultsRetentionDuration;
    }
  }
}
