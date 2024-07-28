// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.ColumnPreferences
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Settings;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ColumnPreferences : IEnumerable<Column>, IEnumerable
  {
    private const string c_productBacklogColumnSettingsName = "ProductBacklogColumnOptions";
    private const string c_iterationBacklogColumnSettingsName = "IterationBacklogColumnOptions";
    private List<Column> m_columns = new List<Column>();
    private IUserSettings m_userSettings;
    private Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn[] m_defaultColumns;
    private string m_legacyIUserSettingsKey;
    private string m_newISettingsServiceKey;

    protected ColumnPreferences()
    {
    }

    public ColumnPreferences(
      IVssRequestContext requestContext,
      BacklogLevelConfiguration backlogLevelConfig,
      IUserSettings userSettings,
      ColumnPreferenceScope columnPreferenceScope)
    {
      ArgumentUtility.CheckForNull<IUserSettings>(userSettings, nameof (userSettings));
      ArgumentUtility.CheckForNull<BacklogLevelConfiguration>(backlogLevelConfig, nameof (backlogLevelConfig));
      this.m_userSettings = userSettings;
      (this.m_legacyIUserSettingsKey, this.m_newISettingsServiceKey) = this.GetColumnOptionsKey(backlogLevelConfig.Id, columnPreferenceScope);
      this.m_defaultColumns = backlogLevelConfig.ColumnFields;
      this.Initialize(requestContext, this.m_defaultColumns);
    }

    public virtual string PreferencesKey => this.m_legacyIUserSettingsKey;

    public virtual IDictionary<string, int> GetColumnWidthMap()
    {
      Dictionary<string, int> columnWidthMap = new Dictionary<string, int>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      if (this.m_columns != null)
      {
        foreach (Column column in this.m_columns)
        {
          if (!columnWidthMap.ContainsKey(column.FieldName))
            columnWidthMap[column.FieldName] = column.ColumnWidth;
        }
      }
      return (IDictionary<string, int>) columnWidthMap;
    }

    public virtual void EnsureColumnIsPresent(
      string fieldName,
      int width = 100,
      bool notAField = false,
      bool rollup = false,
      RollupCalculation rollupCalculation = null)
    {
      if (this.m_columns.Exists((Predicate<Column>) (col => TFStringComparer.WorkItemFieldReferenceName.Equals(col.FieldName, fieldName))))
        return;
      this.m_columns.Add(new Column(fieldName, width, notAField, rollup, rollupCalculation));
    }

    public virtual void Save(IVssRequestContext requestContext)
    {
      string json;
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer"))
        json = Microsoft.TeamFoundation.Server.WebAccess.JsonExtensions.ToJson<List<Column>>(this.m_columns, (JsonConverter) new JsonConverterColumnConverter());
      else
        json = Microsoft.TeamFoundation.Server.WebAccess.JsonExtensions.ToJson<List<Column>>(this.m_columns, (JavaScriptConverter) new JsonColumnConverter());
      this.m_userSettings.SetValue<string>(this.m_legacyIUserSettingsKey, json);
    }

    public virtual bool Remove(Column column) => this.m_columns.Remove(column);

    public static void EnsureValid(
      IVssRequestContext requestContext,
      ColumnPreferences columnPreferences,
      IFieldTypeDictionary fields)
    {
      ColumnPreferences.Implementation.Instance.EnsureValid(requestContext, columnPreferences, fields);
    }

    private void Initialize(IVssRequestContext requestContext, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn[] defaultColumns)
    {
      if (this.TryLoadColumnFromSettings(requestContext))
        return;
      this.AddColumnsWithdoutDuplicate((IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn>) defaultColumns);
    }

    private void AddColumnsWithdoutDuplicate(IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn> columns)
    {
      foreach (Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn column in columns)
        this.EnsureColumnIsPresent(column.ColumnReferenceName, column.Width, column.NotAField, column.Rollup, column.RollupCalculation);
    }

    private (string, string) GetColumnOptionsKey(
      string backlogLevelId,
      ColumnPreferenceScope columnPreferenceScope)
    {
      string str1;
      string str2;
      switch (columnPreferenceScope)
      {
        case ColumnPreferenceScope.SprintBacklog:
          str1 = "IterationBacklogColumnOptions";
          str2 = "Agile/SprintsHub/IterationBacklogColumnOptions";
          break;
        case ColumnPreferenceScope.ProductBacklog:
          str1 = "ProductBacklogColumnOptions";
          str2 = string.Format("Agile/BacklogsHub/ColumnOptions/{0}", (object) "ProductBacklogColumnOptions");
          break;
        case ColumnPreferenceScope.PortfolioBacklog:
          str1 = backlogLevelId;
          str2 = string.Format("Agile/BacklogsHub/ColumnOptions/{0}", (object) backlogLevelId);
          break;
        default:
          throw new InvalidArgumentValueException(nameof (columnPreferenceScope), "Unknown columnPreferenceScope: " + columnPreferenceScope.ToString());
      }
      return (str1, str2);
    }

    private bool TryLoadColumnFromSettings(IVssRequestContext requestContext)
    {
      try
      {
        this.m_columns.Clear();
        string json = this.LoadAndMoveColumnSettings(requestContext);
        if (json == null)
          return false;
        IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn> backlogColumn1;
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer"))
          backlogColumn1 = this.ConvertColumnsToBacklogColumn((IEnumerable<Column>) Microsoft.TeamFoundation.Server.WebAccess.JsonExtensions.FromJson<Column[]>(json, (JsonConverter) new JsonConverterColumnConverter()));
        else
          backlogColumn1 = this.ConvertColumnsToBacklogColumn((IEnumerable<Column>) Microsoft.TeamFoundation.Server.WebAccess.JsonExtensions.FromJson<Column[]>(json, -1, (JavaScriptConverter) new JsonColumnConverter()));
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn backlogColumn2 = backlogColumn1.Where<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn, bool>) (c => c.NotAField && TFStringComparer.WorkItemFieldReferenceName.Equals(c.ColumnReferenceName, "System.Backlog.Parent"))).FirstOrDefault<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn>();
        if (backlogColumn2 != null && requestContext.GetService<WorkItemTrackingFieldService>().GetFieldById(requestContext, -35) != null)
        {
          backlogColumn2.ColumnReferenceName = CoreFieldReferenceNames.Parent;
          backlogColumn2.NotAField = false;
          this.Save(requestContext);
        }
        this.AddColumnsWithdoutDuplicate(backlogColumn1);
        return true;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
        return false;
      }
    }

    private string LoadAndMoveColumnSettings(IVssRequestContext requestContext)
    {
      ISettingsService service = requestContext.GetService<ISettingsService>();
      IWebTeamContext webTeamContext = requestContext.GetWebTeamContext();
      string str = service.GetValue<string>(requestContext, SettingsUserScope.User, "WebTeam", webTeamContext.Team.Identity.Id.ToString(), this.m_newISettingsServiceKey, (string) null);
      if (string.IsNullOrEmpty(str))
      {
        str = this.m_userSettings.GetValue<string>(this.m_legacyIUserSettingsKey);
        if (!string.IsNullOrEmpty(str))
        {
          service.SetValue(requestContext, SettingsUserScope.User, "WebTeam", webTeamContext.Team.Identity.Id.ToString(), this.m_newISettingsServiceKey, (object) str);
          this.m_userSettings.SetValue<string>(this.m_legacyIUserSettingsKey, (string) null);
        }
      }
      return str;
    }

    private IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn> ConvertColumnsToBacklogColumn(
      IEnumerable<Column> columns)
    {
      return (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn>) columns.Select<Column, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn>((Func<Column, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn>) (c => new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn()
      {
        ColumnReferenceName = c.FieldName,
        Width = c.ColumnWidth,
        NotAField = c.NotAField,
        Rollup = c.Rollup,
        RollupCalculation = c.RollupCalculation
      })).ToArray<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn>();
    }

    public IEnumerator<Column> GetEnumerator() => (IEnumerator<Column>) this.m_columns.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.m_columns.GetEnumerator();

    public class Implementation
    {
      private static ColumnPreferences.Implementation s_instance;

      protected Implementation()
      {
      }

      public static ColumnPreferences.Implementation Instance
      {
        get
        {
          if (ColumnPreferences.Implementation.s_instance == null)
            ColumnPreferences.Implementation.s_instance = new ColumnPreferences.Implementation();
          return ColumnPreferences.Implementation.s_instance;
        }
        internal set => ColumnPreferences.Implementation.s_instance = value;
      }

      public virtual void EnsureValid(
        IVssRequestContext requestContext,
        ColumnPreferences columnPreferences,
        IFieldTypeDictionary fields)
      {
        Column[] array = columnPreferences.Where<Column>((Func<Column, bool>) (column =>
        {
          if (column.NotAField || column.Rollup)
            return false;
          FieldEntry field = (FieldEntry) null;
          return !fields.TryGetField(column.FieldName, out field) || !field.IsQueryable;
        })).ToArray<Column>();
        if (!((IEnumerable<Column>) array).Any<Column>())
          return;
        foreach (Column column in array)
          columnPreferences.Remove(column);
        TeamFoundationTrace.Info("Removing invalid columns '{0}' from user column preferences", (object) string.Join("', '", ((IEnumerable<Column>) array).Select<Column, string>((Func<Column, string>) (column => column.FieldName))));
        columnPreferences.Save(requestContext);
      }
    }
  }
}
