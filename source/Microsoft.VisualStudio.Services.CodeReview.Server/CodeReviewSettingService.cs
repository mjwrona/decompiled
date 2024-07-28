// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CodeReviewSettingService
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal class CodeReviewSettingService : 
    CodeReviewServiceBase,
    ICodeReviewSettingService,
    IVssFrameworkService
  {
    private const string c_reviewSettingsPropertyName = "reviewSettings";

    public ReviewSettings GetReviewSettings(IVssRequestContext requestContext, Guid projectId) => this.ExecuteAndTrace<ReviewSettings>(requestContext, 1383611, 1383612, 1383613, (Func<ReviewSettings>) (() =>
    {
      requestContext.Trace(1383614, TraceLevel.Verbose, this.Area, this.Layer, "Getting review settings: project id: '{0}'", (object) projectId);
      ReviewSettings settings = this.Convert(requestContext, ArtifactPropertyKinds.GetProperties(requestContext, ArtifactPropertyKinds.MakeArtifactSpec(ServerConstants.ReviewSettingPropertyKind, ArtifactPropertyKinds.GetReviewSettingMoniker(projectId))));
      settings.AddReferenceLinks(requestContext, projectId);
      return settings;
    }), nameof (GetReviewSettings));

    public ReviewSettings SaveReviewSettings(
      IVssRequestContext requestContext,
      Guid projectId,
      ReviewSettings settings)
    {
      return this.ExecuteAndTrace<ReviewSettings>(requestContext, 1383601, 1383602, 1383603, (Func<ReviewSettings>) (() =>
      {
        if (!this.IsUserProjectAdmin(requestContext, projectId))
          throw new CodeReviewInsufficientPermissions(CodeReviewResources.ReviewSettingsSaveInsufficientPermissions());
        this.TraceSaveReviewSettingsInfo(requestContext, projectId, settings);
        ReviewSettings settings1 = this.Convert(requestContext, ArtifactPropertyKinds.PatchProperties(requestContext, ArtifactPropertyKinds.MakeArtifactSpec(ServerConstants.ReviewSettingPropertyKind, ArtifactPropertyKinds.GetReviewSettingMoniker(projectId)), this.Convert(settings)));
        settings1.AddReferenceLinks(requestContext, projectId);
        return settings1;
      }), nameof (SaveReviewSettings));
    }

    private PropertiesCollection Convert(ReviewSettings settings)
    {
      PropertiesCollection propertiesCollection = new PropertiesCollection();
      if (settings != null)
        propertiesCollection.Add("reviewSettings", (object) JsonConvert.SerializeObject((object) settings));
      return propertiesCollection;
    }

    private ReviewSettings Convert(
      IVssRequestContext requestContext,
      PropertiesCollection properties)
    {
      ReviewSettings reviewSettings = new ReviewSettings();
      if (properties != null)
      {
        if (properties.Count == 1)
        {
          try
          {
            reviewSettings = JsonConvert.DeserializeObject<ReviewSettings>(properties.First<KeyValuePair<string, object>>().Value.ToString());
          }
          catch (Exception ex)
          {
            requestContext.Trace(1383605, TraceLevel.Verbose, this.Area, this.Layer, "Exception parsing review settings '{0}': '{1}'", (object) properties.First<KeyValuePair<string, object>>().Value.ToString(), (object) ex.Message);
          }
        }
      }
      return reviewSettings;
    }

    private void TraceSaveReviewSettingsInfo(
      IVssRequestContext requestContext,
      Guid projectId,
      ReviewSettings settings)
    {
      if (!requestContext.IsTracing(1383604, TraceLevel.Verbose, this.Area, this.Layer))
        return;
      StringBuilder stringBuilder = new StringBuilder();
      if (settings.GlobalSettings != null)
        stringBuilder.Append(string.Format("global settings: '{0}'", (object) settings.GlobalSettings.ToString()));
      if (settings.Settings != null && settings.Settings.Count > 0)
      {
        stringBuilder.Append("settings: ");
        foreach (KeyValuePair<string, JObject> setting in (IEnumerable<KeyValuePair<string, JObject>>) settings.Settings)
          stringBuilder.Append(string.Format("{0}:{1},", (object) setting.Key, (object) setting.Value.ToString()));
      }
      requestContext.Trace(1383604, TraceLevel.Verbose, this.Area, this.Layer, "Save review settings: project id: '{0}', {1}", (object) stringBuilder.ToString());
    }
  }
}
