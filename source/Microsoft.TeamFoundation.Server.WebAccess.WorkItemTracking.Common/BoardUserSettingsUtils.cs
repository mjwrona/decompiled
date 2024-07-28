// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardUserSettingsUtils
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.Settings;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class BoardUserSettingsUtils : IBoardUserSettingsUtils
  {
    private const string BoardUserSettingsRootKeyFormat = "/KanbanBoard.UserSettings/{0}/";
    private const string AutoRefreshStateKeyFormat = "/KanbanBoard.UserSettings/{0}/autoRefreshState";
    private const string CurrentBoardFilterKeyFormat = "/KanbanBoard.UserSettings/{0}/CurrentBoardFilter";
    private static object _lock = new object();
    private static IBoardUserSettingsUtils _instance = (IBoardUserSettingsUtils) null;

    private BoardUserSettingsUtils()
    {
    }

    public static IBoardUserSettingsUtils Instance
    {
      get
      {
        if (BoardUserSettingsUtils._instance == null)
        {
          lock (BoardUserSettingsUtils._lock)
          {
            if (BoardUserSettingsUtils._instance == null)
              BoardUserSettingsUtils._instance = (IBoardUserSettingsUtils) new BoardUserSettingsUtils();
          }
        }
        return BoardUserSettingsUtils._instance;
      }
    }

    public virtual void SetBoardUserSettings(
      IVssRequestContext requestContext,
      Guid projectGuid,
      WebApiTeam team,
      Guid boardId,
      Dictionary<string, string> userSettings)
    {
      try
      {
        using (ISettingsProvider userWebSettings = this.GetUserWebSettings(requestContext, projectGuid, team))
        {
          string str;
          bool result;
          if (!userSettings.TryGetValue("autoRefreshState", out str) || !bool.TryParse(str, out result))
            return;
          userWebSettings.SetSetting<bool>(this.GetAutoRefreshSettingKey(boardId), result);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(240313, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, ex);
        throw;
      }
    }

    public virtual BoardUserSettings GetBoardUserSettings(
      IVssRequestContext requestContext,
      Guid projectGuid,
      WebApiTeam team,
      Guid boardId)
    {
      BoardUserSettings boardUserSettings = new BoardUserSettings()
      {
        AutoBoardRefreshState = true,
        CurrentBoardFilter = (string) null
      };
      try
      {
        using (ISettingsProvider userWebSettings = this.GetUserWebSettings(requestContext, projectGuid, team))
        {
          IDictionary<string, object> dictionary = userWebSettings.QueryEntries(this.GetBoardUserSettingsKeyPattern(boardId));
          object obj;
          bool result;
          if (dictionary.TryGetValue(this.GetAutoRefreshSettingKey(boardId), out obj) && bool.TryParse((string) obj, out result))
            boardUserSettings.AutoBoardRefreshState = result;
          object json;
          if (dictionary.TryGetValue(this.GetCurrentBoardFilterSettingKey(boardId), out json))
          {
            SavedFilterSettings savedFilterSettings;
            try
            {
              savedFilterSettings = JsonUtilities.Deserialize<SavedFilterSettings>((string) json, true);
            }
            catch (Exception ex)
            {
              savedFilterSettings = new SavedFilterSettings();
              savedFilterSettings.QueryText = (string) json;
              savedFilterSettings.ParentWorkItemIds = (IEnumerable<int>) new List<int>();
              requestContext.Trace(290559, TraceLevel.Error, "Agile", TfsTraceLayers.BusinessLogic, "In Registry, filter setting is saved in old format as {0}. So Json deserialization of filter settings failed during initial boardload with error: {1}", (object) savedFilterSettings.QueryText, (object) ex.Message);
            }
            boardUserSettings.CurrentBoardFilter = savedFilterSettings.QueryText;
            boardUserSettings.ParentWorkItemIds = savedFilterSettings.ParentWorkItemIds;
          }
        }
        return boardUserSettings;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(240314, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, ex);
      }
      return boardUserSettings;
    }

    public virtual ISettingsProvider GetUserWebSettings(
      IVssRequestContext requestContext,
      Guid projectId,
      WebApiTeam team)
    {
      return WebSettings.GetWebSettings(requestContext, projectId, team, WebSettingsScope.User);
    }

    private string GetAutoRefreshSettingKey(Guid boardId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/KanbanBoard.UserSettings/{0}/autoRefreshState", (object) boardId);

    private string GetCurrentBoardFilterSettingKey(Guid boardId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/KanbanBoard.UserSettings/{0}/CurrentBoardFilter", (object) boardId);

    private string GetBoardUserSettingsKeyPattern(Guid boardId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/KanbanBoard.UserSettings/{0}/", (object) boardId) + "*";
  }
}
