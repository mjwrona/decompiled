// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.TelemetryUtils
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  public class TelemetryUtils
  {
    public static void PublishUpdateColumnsData(
      IVssRequestContext requestContext,
      IEnumerable<BoardColumn> existingSettings,
      IEnumerable<BoardColumn> newSettings)
    {
      ArgumentUtility.CheckForNull<IEnumerable<BoardColumn>>(existingSettings, "existing boardColumns");
      ArgumentUtility.CheckForNull<IEnumerable<BoardColumn>>(newSettings, "new boardColumns");
      CustomerIntelligenceData details = new CustomerIntelligenceData();
      UpdateColumnSettingChanges columnSettingChanges = TelemetryUtils.GetUpdateColumnSettingChanges(existingSettings, newSettings);
      details.Add(BoardCustomerIntelligencePropertyName.TotalColumnCount, (double) columnSettingChanges.TotalColumnCount);
      details.Add(BoardCustomerIntelligencePropertyName.AddedColumnCount, (double) columnSettingChanges.NewColumnCount);
      details.Add(BoardCustomerIntelligencePropertyName.SplitColumnCount, (double) columnSettingChanges.SplitColumnCount);
      details.Add(BoardCustomerIntelligencePropertyName.DeletedColumnCount, (double) columnSettingChanges.DeletedColumnCount);
      details.Add(BoardCustomerIntelligencePropertyName.DescriptionColumnCount, (double) columnSettingChanges.DescriptionColumnCount);
      details.Add(BoardCustomerIntelligencePropertyName.AverageWipLimitInProgressColumn, (double) columnSettingChanges.AverageWipLimitInProgressColumn);
      details.Add(BoardCustomerIntelligencePropertyName.IsSplitColumnStateChanged, columnSettingChanges.IsSplitColumnStateChanged);
      details.Add(BoardCustomerIntelligencePropertyName.IsColumnNameChanged, columnSettingChanges.IsColumnNameChanged);
      details.Add(BoardCustomerIntelligencePropertyName.IsWIPLimitChanged, columnSettingChanges.IsWIPLimitChanged);
      details.Add(BoardCustomerIntelligencePropertyName.IsColumnDescriptionChanged, columnSettingChanges.IsDescriptionChanged);
      details.Add(BoardCustomerIntelligencePropertyName.IsColumnOrderChanged, columnSettingChanges.IsColumnOrderChanged);
      details.Add(BoardCustomerIntelligencePropertyName.IsColumnStateChanged, columnSettingChanges.IsColumnStateChanged);
      TelemetryUtils.Publish(requestContext, AgileCustomerIntelligenceFeature.UpdateBoardColumnSettings, details);
    }

    public static void PublishBoardBadgeIsPublicStatusChanged(
      IVssRequestContext requestContext,
      bool isPublic)
    {
      CustomerIntelligenceData details = new CustomerIntelligenceData();
      details.Add(BoardCustomerIntelligencePropertyName.BoardBadgeIsPublic, isPublic);
      TelemetryUtils.Publish(requestContext, AgileCustomerIntelligenceFeature.BoardBadgeIsPublicStatusChanged, details);
    }

    public static UpdateColumnSettingChanges GetUpdateColumnSettingChanges(
      IEnumerable<BoardColumn> existingSettings,
      IEnumerable<BoardColumn> newSettings)
    {
      UpdateColumnSettingChanges settingChanges = new UpdateColumnSettingChanges();
      settingChanges.TotalColumnCount = newSettings.Count<BoardColumn>();
      int sumOfWipLimit = 0;
      int inProgressColumnCount = 0;
      newSettings.ToList<BoardColumn>().ForEach((Action<BoardColumn>) (newCol =>
      {
        if (!newCol.Id.HasValue)
          ++settingChanges.NewColumnCount;
        if (newCol.IsSplit)
          ++settingChanges.SplitColumnCount;
        if (!string.IsNullOrEmpty(newCol.Description))
          ++settingChanges.DescriptionColumnCount;
        if (newCol.ColumnType != BoardColumnType.InProgress)
          return;
        sumOfWipLimit += newCol.ItemLimit;
        ++inProgressColumnCount;
      }));
      settingChanges.AverageWipLimitInProgressColumn = inProgressColumnCount > 0 ? sumOfWipLimit / inProgressColumnCount : 0;
      IEnumerable<\u003C\u003Ef__AnonymousType8<BoardColumn, BoardColumn>> source = newSettings.Join(existingSettings, (Func<BoardColumn, Guid?>) (newCol => newCol.Id), (Func<BoardColumn, Guid?>) (existingCol => existingCol.Id), (newCol, existingCol) => new
      {
        newCol = newCol,
        existingCol = existingCol
      });
      settingChanges.DeletedColumnCount = existingSettings.Count<BoardColumn>() - source.Count();
      source.ToList().ForEach(updatedCol =>
      {
        BoardColumn newCol = updatedCol.newCol;
        BoardColumn existingCol = updatedCol.existingCol;
        if (newCol.Name == null)
          newCol.Name = string.Empty;
        if (existingCol.Name == null)
          existingCol.Name = string.Empty;
        if (newCol.Description == null)
          newCol.Description = string.Empty;
        if (existingCol.Description == null)
          existingCol.Description = string.Empty;
        if (newCol.IsSplit != existingCol.IsSplit)
          settingChanges.IsSplitColumnStateChanged = true;
        if (!StringComparer.OrdinalIgnoreCase.Equals(newCol.Name.Trim(), existingCol.Name.Trim()))
          settingChanges.IsColumnNameChanged = true;
        if (newCol.ItemLimit != existingCol.ItemLimit)
          settingChanges.IsWIPLimitChanged = true;
        if (!StringComparer.OrdinalIgnoreCase.Equals(newCol.Description.Trim(), existingCol.Description.Trim()))
          settingChanges.IsDescriptionChanged = true;
        if (newCol.Order != existingCol.Order)
          settingChanges.IsColumnOrderChanged = true;
        if (newCol.StateMappings == null || existingCol.StateMappings == null)
          return;
        if (newCol.StateMappings.Count != existingCol.StateMappings.Count)
          settingChanges.IsColumnStateChanged = true;
        else
          existingCol.StateMappings.ToList<KeyValuePair<string, string>>().ForEach((Action<KeyValuePair<string, string>>) (mapping =>
          {
            if (newCol.StateMappings.ContainsKey(mapping.Key) && StringComparer.OrdinalIgnoreCase.Equals(newCol.StateMappings[mapping.Key].Trim(), mapping.Value.Trim()))
              return;
            settingChanges.IsColumnStateChanged = true;
          }));
      });
      return settingChanges;
    }

    private static void Publish(
      IVssRequestContext requestContext,
      string feature,
      CustomerIntelligenceData details)
    {
      try
      {
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, AgileCustomerIntelligenceArea.Agile, feature, details);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, AgileCustomerIntelligenceArea.Agile, string.Format("Telemetry Failed on: {0}", (object) feature), ex);
      }
    }
  }
}
