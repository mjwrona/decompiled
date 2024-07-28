// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.ColumnSettingsHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class ColumnSettingsHelper : TestHelperBase
  {
    private static List<TestPointGridDisplayColumn> s_requiredPagedColumns;
    private WorkItemTrackingFieldService m_fieldTypes;
    private Dictionary<string, ColumnSettingsHelper.TestPointProperties> m_testPointPropertiesMap = new Dictionary<string, ColumnSettingsHelper.TestPointProperties>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, ColumnSettingModel> m_defaultColumnsDictionary = new Dictionary<string, ColumnSettingModel>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public ColumnSettingsHelper(TestManagerRequestContext testContext)
      : base(testContext)
    {
      this.InitializeTestPointFieldDictionary();
      this.InitializeDefaultColumnDictionary();
    }

    public ColumnSettingsHelper(IVssRequestContext tfsRequestContext)
      : base(tfsRequestContext)
    {
      this.InitializeTestPointFieldDictionary();
      this.InitializeDefaultColumnDictionary();
    }

    public ColumnSettingModel[] GetDefaultColumns() => this.m_defaultColumnsDictionary.Values.ToArray<ColumnSettingModel>();

    internal List<ColumnSettingModel> GetFixedColumns() => new List<ColumnSettingModel>()
    {
      this.m_defaultColumnsDictionary[TestPointReferenceNames.Outcome],
      this.m_defaultColumnsDictionary[TestPointReferenceNames.Order],
      this.m_defaultColumnsDictionary[CoreFieldReferenceNames.Id],
      this.m_defaultColumnsDictionary[CoreFieldReferenceNames.Title]
    };

    public List<TestPointGridDisplayColumn> GetDisplayColumns(IList<ColumnSettingModel> columns)
    {
      if (this.m_fieldTypes == null)
        this.m_fieldTypes = this.TfsRequestContext.GetService<WorkItemTrackingFieldService>();
      List<TestPointGridDisplayColumn> displayColumns = new List<TestPointGridDisplayColumn>();
      foreach (ColumnSettingModel column in (IEnumerable<ColumnSettingModel>) columns)
      {
        FieldEntry field = (FieldEntry) null;
        if (!this.m_testPointPropertiesMap.ContainsKey(column.RefName))
        {
          if (this.m_fieldTypes.TryGetField(this.TfsRequestContext, column.RefName, out field))
          {
            List<TestPointGridDisplayColumn> gridDisplayColumnList = displayColumns;
            TestPointGridDisplayColumn gridDisplayColumn = new TestPointGridDisplayColumn();
            gridDisplayColumn.Name = column.RefName;
            gridDisplayColumn.Text = field.Name;
            gridDisplayColumn.FieldId = field.FieldId;
            gridDisplayColumn.CanSortBy = field.CanSortBy;
            gridDisplayColumn.Width = column.Width;
            gridDisplayColumn.Index = this.GetIndex(column.RefName);
            gridDisplayColumn.Type = field.SystemType.ToString();
            gridDisplayColumn.IsIdentity = field.IsIdentity;
            gridDisplayColumnList.Add(gridDisplayColumn);
          }
        }
        else
        {
          string refName = column.RefName;
          List<TestPointGridDisplayColumn> gridDisplayColumnList = displayColumns;
          TestPointGridDisplayColumn gridDisplayColumn = new TestPointGridDisplayColumn();
          gridDisplayColumn.Name = refName;
          gridDisplayColumn.Text = this.m_testPointPropertiesMap[refName].Text;
          gridDisplayColumn.FieldId = this.m_testPointPropertiesMap[refName].FieldId;
          gridDisplayColumn.CanSortBy = true;
          gridDisplayColumn.Width = column.Width;
          gridDisplayColumn.Index = this.GetIndex(refName);
          gridDisplayColumn.Type = this.GetFieldType(refName);
          gridDisplayColumn.IsIdentity = this.m_testPointPropertiesMap[refName].IsIdentity;
          gridDisplayColumnList.Add(gridDisplayColumn);
        }
      }
      return displayColumns;
    }

    internal List<string> GetWorkItemFields(IEnumerable<TestPointGridDisplayColumn> columns)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (TestPointGridDisplayColumn column in columns)
      {
        if (!this.m_testPointPropertiesMap.ContainsKey(column.Name) && !dictionary.ContainsKey(column.Name))
          dictionary.Add(column.Name, column.Name);
      }
      return dictionary.Values.ToList<string>();
    }

    internal List<TestPointGridDisplayColumn> GetPagedColumns(
      TestPointGridDisplayColumn[] displayColumns)
    {
      List<TestPointGridDisplayColumn> pagedColumns = new List<TestPointGridDisplayColumn>();
      if (displayColumns != null && displayColumns.Length != 0)
        pagedColumns.AddRange((IEnumerable<TestPointGridDisplayColumn>) displayColumns);
      ColumnSettingsHelper.s_requiredPagedColumns.ForEach((Action<TestPointGridDisplayColumn>) (column =>
      {
        if (pagedColumns.Any<TestPointGridDisplayColumn>((Func<TestPointGridDisplayColumn, bool>) (pagedColumn => string.Equals(pagedColumn.Index, column.Index, StringComparison.OrdinalIgnoreCase))))
          return;
        pagedColumns.Add(column);
      }));
      return pagedColumns.Distinct<TestPointGridDisplayColumn>().ToList<TestPointGridDisplayColumn>();
    }

    internal ColumnSortOrderModel GetSortOrder(
      IList<TestPointGridDisplayColumn> displayColumns,
      ColumnSortOrderModel sortOrder)
    {
      foreach (TestPointGridDisplayColumn displayColumn in (IEnumerable<TestPointGridDisplayColumn>) displayColumns)
      {
        if (string.Equals(displayColumn.Index, sortOrder.Index, StringComparison.OrdinalIgnoreCase))
          return sortOrder;
      }
      return ColumnSortOrderModel.Default;
    }

    private string GetFieldType(string refName) => this.m_testPointPropertiesMap.ContainsKey(refName) ? this.m_testPointPropertiesMap[refName].Type.ToString() : typeof (string).ToString();

    private string GetIndex(string refName) => this.m_testPointPropertiesMap.ContainsKey(refName) ? this.m_testPointPropertiesMap[refName].Index : refName;

    private void InitializeTestPointFieldDictionary()
    {
      this.m_testPointPropertiesMap.Add(TestPointReferenceNames.Tester, new ColumnSettingsHelper.TestPointProperties()
      {
        ReferenceName = TestPointReferenceNames.Tester,
        FieldId = 302,
        Index = "tester",
        Text = TestManagementResources.TestPointGridColumnTester,
        Type = typeof (string),
        IsIdentity = true
      });
      this.m_testPointPropertiesMap.Add(TestPointReferenceNames.AssignedTo, new ColumnSettingsHelper.TestPointProperties()
      {
        ReferenceName = TestPointReferenceNames.Tester,
        FieldId = 303,
        Index = "assignedTo",
        Text = string.Empty,
        Type = typeof (string),
        IsIdentity = true
      });
      this.m_testPointPropertiesMap.Add(TestPointReferenceNames.ConfigurationId, new ColumnSettingsHelper.TestPointProperties()
      {
        ReferenceName = TestPointReferenceNames.ConfigurationId,
        FieldId = 304,
        Index = "configurationId",
        Text = string.Empty,
        Type = typeof (int),
        IsIdentity = false
      });
      this.m_testPointPropertiesMap.Add(TestPointReferenceNames.ConfigurationName, new ColumnSettingsHelper.TestPointProperties()
      {
        ReferenceName = TestPointReferenceNames.ConfigurationName,
        FieldId = 305,
        Index = "configurationName",
        Text = TestManagementResources.TestPointGridColumnConfiguration,
        Type = typeof (string),
        IsIdentity = false
      });
      this.m_testPointPropertiesMap.Add(TestPointReferenceNames.SuiteName, new ColumnSettingsHelper.TestPointProperties()
      {
        ReferenceName = TestPointReferenceNames.SuiteName,
        FieldId = 307,
        Index = TestPointIndexes.SuiteName,
        Text = TestManagementResources.TestPointGridColumnSuiteName,
        Type = typeof (string),
        IsIdentity = false
      });
      this.m_testPointPropertiesMap.Add(TestPointReferenceNames.SuiteId, new ColumnSettingsHelper.TestPointProperties()
      {
        ReferenceName = TestPointReferenceNames.SuiteId,
        FieldId = 311,
        Index = TestPointIndexes.SuiteId,
        Text = TestManagementResources.TestPointGridColumnSuiteId,
        Type = typeof (int),
        IsIdentity = false
      });
      this.m_testPointPropertiesMap.Add(TestPointReferenceNames.LastRunBy, new ColumnSettingsHelper.TestPointProperties()
      {
        ReferenceName = TestPointReferenceNames.LastRunBy,
        FieldId = 309,
        Index = TestPointIndexes.LastRunBy,
        Text = TestManagementResources.TestPointGridColumnLastRunBy,
        Type = typeof (string),
        IsIdentity = true
      });
      this.m_testPointPropertiesMap.Add(TestPointReferenceNames.LastRunDuration, new ColumnSettingsHelper.TestPointProperties()
      {
        ReferenceName = TestPointReferenceNames.LastRunDuration,
        FieldId = 308,
        Index = TestPointIndexes.LastRunDuration,
        Text = TestManagementResources.TestPointGridColumnLastRunDuration,
        Type = typeof (long),
        IsIdentity = false
      });
      this.m_testPointPropertiesMap.Add(TestPointReferenceNames.Build, new ColumnSettingsHelper.TestPointProperties()
      {
        ReferenceName = TestPointReferenceNames.Build,
        FieldId = 310,
        Index = TestPointIndexes.Build,
        Text = TestManagementResources.TestPointGridColumnBuild,
        Type = typeof (string),
        IsIdentity = false
      });
      this.m_testPointPropertiesMap.Add(TestPointReferenceNames.Outcome, new ColumnSettingsHelper.TestPointProperties()
      {
        ReferenceName = TestPointReferenceNames.Outcome,
        FieldId = 306,
        Index = TestPointIndexes.Outcome,
        Text = TestManagementResources.TestPointGridColumnOutcome,
        Type = typeof (string),
        IsIdentity = false
      });
      this.m_testPointPropertiesMap.Add(TestPointReferenceNames.TestCaseId, new ColumnSettingsHelper.TestPointProperties()
      {
        ReferenceName = TestPointReferenceNames.TestCaseId,
        FieldId = 301,
        Index = TestPointIndexes.TestCaseId,
        Text = string.Empty,
        Type = typeof (int),
        IsIdentity = false
      });
      this.m_testPointPropertiesMap.Add(TestPointReferenceNames.TestPointId, new ColumnSettingsHelper.TestPointProperties()
      {
        ReferenceName = TestPointReferenceNames.TestPointId,
        FieldId = 300,
        Index = TestPointIndexes.TestPointId,
        Text = string.Empty,
        Type = typeof (int),
        IsIdentity = false
      });
      this.m_testPointPropertiesMap.Add(TestPointReferenceNames.Order, new ColumnSettingsHelper.TestPointProperties()
      {
        ReferenceName = TestPointReferenceNames.Order,
        FieldId = 312,
        Index = TestPointIndexes.Order,
        Text = TestManagementResources.TestPointGridColumnOrder,
        Type = typeof (int),
        IsIdentity = false
      });
    }

    private void InitializeDefaultColumnDictionary()
    {
      this.m_defaultColumnsDictionary.Add(TestPointReferenceNames.Outcome, new ColumnSettingModel()
      {
        RefName = TestPointReferenceNames.Outcome,
        Width = WITServerResources.GetInt("ResultListDefaultStringColumnWidth")
      });
      this.m_defaultColumnsDictionary.Add(CoreFieldReferenceNames.Id, new ColumnSettingModel()
      {
        RefName = CoreFieldReferenceNames.Id,
        Width = WITServerResources.GetInt("ResultListDefaultIdColumnWidth")
      });
      this.m_defaultColumnsDictionary.Add(TestPointReferenceNames.Order, new ColumnSettingModel()
      {
        RefName = TestPointReferenceNames.Order,
        Width = WITServerResources.GetInt("ResultListDefaultIdColumnWidth")
      });
      this.m_defaultColumnsDictionary.Add(CoreFieldReferenceNames.Title, new ColumnSettingModel()
      {
        RefName = CoreFieldReferenceNames.Title,
        Width = WITServerResources.GetInt("ResultListDefaultTitleColumnWidth")
      });
      this.m_defaultColumnsDictionary.Add(TestPointReferenceNames.ConfigurationName, new ColumnSettingModel()
      {
        RefName = TestPointReferenceNames.ConfigurationName,
        Width = WITServerResources.GetInt("ResultListDefaultStringColumnWidth")
      });
      this.m_defaultColumnsDictionary.Add(TestPointReferenceNames.Tester, new ColumnSettingModel()
      {
        RefName = TestPointReferenceNames.Tester,
        Width = WITServerResources.GetInt("ResultListDefaultStringColumnWidth")
      });
    }

    static ColumnSettingsHelper()
    {
      List<TestPointGridDisplayColumn> gridDisplayColumnList = new List<TestPointGridDisplayColumn>();
      TestPointGridDisplayColumn gridDisplayColumn1 = new TestPointGridDisplayColumn();
      gridDisplayColumn1.FieldId = 80;
      gridDisplayColumn1.Type = typeof (string).FullName;
      gridDisplayColumn1.Name = CoreFieldReferenceNames.Tags;
      gridDisplayColumn1.Index = CoreFieldReferenceNames.Tags;
      gridDisplayColumnList.Add(gridDisplayColumn1);
      TestPointGridDisplayColumn gridDisplayColumn2 = new TestPointGridDisplayColumn();
      gridDisplayColumn2.FieldId = 300;
      gridDisplayColumn2.Type = typeof (int).FullName;
      gridDisplayColumn2.Name = TestPointReferenceNames.TestPointId;
      gridDisplayColumn2.Index = TestPointIndexes.TestPointId;
      gridDisplayColumnList.Add(gridDisplayColumn2);
      TestPointGridDisplayColumn gridDisplayColumn3 = new TestPointGridDisplayColumn();
      gridDisplayColumn3.FieldId = 2;
      gridDisplayColumn3.Type = typeof (string).FullName;
      gridDisplayColumn3.Name = CoreFieldReferenceNames.State;
      gridDisplayColumn3.Index = CoreFieldReferenceNames.State;
      gridDisplayColumnList.Add(gridDisplayColumn3);
      TestPointGridDisplayColumn gridDisplayColumn4 = new TestPointGridDisplayColumn();
      gridDisplayColumn4.FieldId = 24;
      gridDisplayColumn4.Type = typeof (string).FullName;
      gridDisplayColumn4.Name = CoreFieldReferenceNames.AssignedTo;
      gridDisplayColumn4.Index = CoreFieldReferenceNames.AssignedTo;
      gridDisplayColumn4.IsIdentity = true;
      gridDisplayColumnList.Add(gridDisplayColumn4);
      TestPointGridDisplayColumn gridDisplayColumn5 = new TestPointGridDisplayColumn();
      gridDisplayColumn5.FieldId = 25;
      gridDisplayColumn5.Type = typeof (string).FullName;
      gridDisplayColumn5.Name = CoreFieldReferenceNames.WorkItemType;
      gridDisplayColumn5.Index = CoreFieldReferenceNames.WorkItemType;
      gridDisplayColumnList.Add(gridDisplayColumn5);
      TestPointGridDisplayColumn gridDisplayColumn6 = new TestPointGridDisplayColumn();
      gridDisplayColumn6.FieldId = -42;
      gridDisplayColumn6.Type = typeof (string).FullName;
      gridDisplayColumn6.Name = CoreFieldReferenceNames.TeamProject;
      gridDisplayColumn6.Index = CoreFieldReferenceNames.TeamProject;
      gridDisplayColumnList.Add(gridDisplayColumn6);
      ColumnSettingsHelper.s_requiredPagedColumns = gridDisplayColumnList;
    }

    private class TestPointProperties
    {
      public string ReferenceName { get; set; }

      public int FieldId { get; set; }

      public string Index { get; set; }

      public string Text { get; set; }

      public Type Type { get; set; }

      public bool IsIdentity { get; set; }
    }
  }
}
