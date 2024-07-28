// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.ProjectProcessConfigurationComponent2
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class ProjectProcessConfigurationComponent2 : ProjectProcessConfigurationComponent
  {
    private static SqlMetaData[] typ_ProjectConfigurationWorkItemTypeTable2 = new SqlMetaData[6]
    {
      new SqlMetaData("WorkItemType", SqlDbType.TinyInt),
      new SqlMetaData("CategoryRefName", SqlDbType.NVarChar, 140L),
      new SqlMetaData("PluralDisplayName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("SingularDisplayName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("ParentCategoryRefName", SqlDbType.NVarChar, 70L),
      new SqlMetaData("WorkItemCountLimit", SqlDbType.Int)
    };
    private static SqlMetaData[] typ_ProjectConfigurationWorkItemStateTable2 = new SqlMetaData[4]
    {
      new SqlMetaData("WorkItemType", SqlDbType.TinyInt),
      new SqlMetaData("StateType", SqlDbType.TinyInt),
      new SqlMetaData("StateValue", SqlDbType.NVarChar, 256L),
      new SqlMetaData("CategoryRefName", SqlDbType.NVarChar, 70L)
    };
    private static SqlMetaData[] typ_ProjectConfigurationGridColumnTable2 = new SqlMetaData[4]
    {
      new SqlMetaData("FieldRefName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("DisplayWidth", SqlDbType.Int),
      new SqlMetaData("Order", SqlDbType.Int),
      new SqlMetaData("CategoryRefName", SqlDbType.NVarChar, 70L)
    };
    private static SqlMetaData[] typ_ProjectConfigurationAddPanelFieldsTable = new SqlMetaData[3]
    {
      new SqlMetaData("FieldRefName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Order", SqlDbType.Int),
      new SqlMetaData("CategoryRefName", SqlDbType.NVarChar, 70L)
    };
    private static SqlMetaData[] typ_ProjectConfigurationWorkItemColorsTable = new SqlMetaData[3]
    {
      new SqlMetaData("WorkItemTypeName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Primary", SqlDbType.Int),
      new SqlMetaData("Secondary", SqlDbType.Int)
    };

    internal override ProjectProcessConfiguration GetProjectProcessConfiguration(Guid projectId)
    {
      this.PrepareStoredProcedure("prc_GetProjectProcessConfiguration");
      this.BindDataspaceIdOrProjectId(projectId);
      ProjectProcessConfiguration settings = new ProjectProcessConfiguration();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TypeField>((ObjectBinder<TypeField>) new TypeFieldBinder());
        resultCollection.AddBinder<TypeFieldValueRow>((ObjectBinder<TypeFieldValueRow>) new TypeFieldValueRowBinder());
        resultCollection.AddBinder<ProjectProcessConfigurationComponent2.WorkItemTypeRow>((ObjectBinder<ProjectProcessConfigurationComponent2.WorkItemTypeRow>) new ProjectProcessConfigurationComponent2.WorkItemTypeRowBinder());
        resultCollection.AddBinder<ProjectProcessConfigurationComponent2.WorkItemStateRow>((ObjectBinder<ProjectProcessConfigurationComponent2.WorkItemStateRow>) new ProjectProcessConfigurationComponent2.WorkItemStateRowBinder());
        resultCollection.AddBinder<int>((ObjectBinder<int>) new Int32Binder());
        resultCollection.AddBinder<ProjectProcessConfigurationComponent2.GridColumnRow>((ObjectBinder<ProjectProcessConfigurationComponent2.GridColumnRow>) new ProjectProcessConfigurationComponent2.GridColumnRowBinder());
        resultCollection.AddBinder<ProjectProcessConfigurationComponent2.AddPanelFieldRow>((ObjectBinder<ProjectProcessConfigurationComponent2.AddPanelFieldRow>) new ProjectProcessConfigurationComponent2.AddPanelFieldRowBinder());
        resultCollection.AddBinder<ProjectProcessConfigurationComponent2.WorkItemColorsRow>((ObjectBinder<ProjectProcessConfigurationComponent2.WorkItemColorsRow>) new ProjectProcessConfigurationComponent2.WorkItemColorsRowBinder());
        settings.TypeFields = resultCollection.GetCurrent<TypeField>().Items.ToArray();
        resultCollection.NextResult();
        this.FillConfigurationTypeFieldValues(settings, resultCollection.GetCurrent<TypeFieldValueRow>().Items);
        resultCollection.NextResult();
        this.FillConfigurationCategories(settings, (IEnumerable<ProjectProcessConfigurationComponent2.WorkItemTypeRow>) resultCollection.GetCurrent<ProjectProcessConfigurationComponent2.WorkItemTypeRow>().Items);
        resultCollection.NextResult();
        this.FillConfigurationCategoryStates(settings, (IEnumerable<ProjectProcessConfigurationComponent2.WorkItemStateRow>) resultCollection.GetCurrent<ProjectProcessConfigurationComponent2.WorkItemStateRow>().Items);
        resultCollection.NextResult();
        settings.Weekends = resultCollection.GetCurrent<int>().Items.Select<int, DayOfWeek>((System.Func<int, DayOfWeek>) (i => (DayOfWeek) i)).ToArray<DayOfWeek>();
        resultCollection.NextResult();
        this.FillConfigurationGridColumns(settings, (IEnumerable<ProjectProcessConfigurationComponent2.GridColumnRow>) resultCollection.GetCurrent<ProjectProcessConfigurationComponent2.GridColumnRow>().Items);
        resultCollection.NextResult();
        this.FillConfigurationAddPanelFields(settings, (IEnumerable<ProjectProcessConfigurationComponent2.AddPanelFieldRow>) resultCollection.GetCurrent<ProjectProcessConfigurationComponent2.AddPanelFieldRow>().Items);
        resultCollection.NextResult();
        this.FillConfigurationWorkItemColors(settings, resultCollection.GetCurrent<ProjectProcessConfigurationComponent2.WorkItemColorsRow>().Items);
      }
      return settings;
    }

    internal override void SetProjectProcessConfiguration(
      Guid projectId,
      ProjectProcessConfiguration settings)
    {
      this.PrepareStoredProcedure("prc_SetProjectProcessConfiguration");
      this.BindDataspaceIdOrProjectId(projectId);
      this.BindProjectConfigurationTypeFieldTable("@typeFieldTable", (IEnumerable<TypeField>) settings.TypeFields);
      this.BindProjectConfigurationTypeFieldValueTable("@typeFieldValueTable", this.GetTypeFieldValueRows(settings));
      this.BindProjectConfigurationWorkItemTypeTable("@workItemTypeTable", this.GetTypeRows(settings));
      this.BindProjectConfigurationWorkItemStateTable("@workItemStateTable", this.GetStateRows(settings));
      this.BindInt32Table("@weekendTable", ((IEnumerable<DayOfWeek>) settings.Weekends).Select<DayOfWeek, int>((System.Func<DayOfWeek, int>) (weekEnum => (int) weekEnum)));
      this.BindProjectConfigurationGridColumnTable("@gridColumnTable", this.GetGridColumnRows(settings));
      this.BindProjectConfigurationAddPanelFieldsTable("@addPanelFieldsTable", this.GetAddPanelFieldRows(settings));
      this.BindProjectConfigurationWorkItemColorsTable("@colorTable", this.GetWorkItemColorRows(settings));
      this.ExecuteNonQuery();
    }

    internal override void DeleteProjectProcessConfiguration(Guid projectId)
    {
      this.PrepareStoredProcedure("prc_DeleteProjectProcessConfiguration");
      this.BindDataspaceIdOrProjectId(projectId);
      this.ExecuteNonQuery();
    }

    protected void FillConfigurationTypeFieldValues(
      ProjectProcessConfiguration settings,
      List<TypeFieldValueRow> rows)
    {
      foreach (IGrouping<FieldTypeEnum, TypeFieldValueRow> grouping in rows.GroupBy<TypeFieldValueRow, FieldTypeEnum>((System.Func<TypeFieldValueRow, FieldTypeEnum>) (row => row.FieldType)))
      {
        IGrouping<FieldTypeEnum, TypeFieldValueRow> g = grouping;
        TypeField typeField = ((IEnumerable<TypeField>) settings.TypeFields).Where<TypeField>((System.Func<TypeField, bool>) (f => f.Type == g.Key)).FirstOrDefault<TypeField>();
        if (typeField != null)
          typeField.TypeFieldValues = g.Select<TypeFieldValueRow, TypeFieldValue>((System.Func<TypeFieldValueRow, TypeFieldValue>) (item => new TypeFieldValue()
          {
            Type = item.Type,
            Value = item.Value
          })).ToArray<TypeFieldValue>();
      }
    }

    protected void FillConfigurationCategories(
      ProjectProcessConfiguration settings,
      IEnumerable<ProjectProcessConfigurationComponent2.WorkItemTypeRow> rows)
    {
      List<BacklogCategoryConfiguration> categoryConfigurationList = new List<BacklogCategoryConfiguration>();
      foreach (ProjectProcessConfigurationComponent2.WorkItemTypeRow row in rows)
      {
        switch (row.WorkItemType)
        {
          case WorkItemTypeEnum.Feedback:
            continue;
          case WorkItemTypeEnum.Requirement:
            settings.RequirementBacklog = this.CreateBacklogCategoryConfiguration(row);
            continue;
          case WorkItemTypeEnum.Task:
            settings.TaskBacklog = this.CreateBacklogCategoryConfiguration(row);
            continue;
          case WorkItemTypeEnum.Bug:
            settings.BugWorkItems = this.CreateCategoryConfiguration(row);
            continue;
          case WorkItemTypeEnum.FeedbackRequest:
            settings.FeedbackRequestWorkItems = this.CreateCategoryConfiguration(row);
            continue;
          case WorkItemTypeEnum.FeedbackResponse:
            settings.FeedbackResponseWorkItems = this.CreateCategoryConfiguration(row);
            continue;
          case WorkItemTypeEnum.Portfolio:
            categoryConfigurationList.Add(this.CreateBacklogCategoryConfiguration(row));
            continue;
          case WorkItemTypeEnum.Release:
            settings.ReleaseWorkItems = this.CreateCategoryConfiguration(row);
            continue;
          case WorkItemTypeEnum.Stage:
            settings.ReleaseStageWorkItems = this.CreateCategoryConfiguration(row);
            continue;
          case WorkItemTypeEnum.SignoffTask:
            settings.StageSignoffTaskWorkItems = this.CreateCategoryConfiguration(row);
            continue;
          case WorkItemTypeEnum.TestPlan:
            settings.TestPlanWorkItems = this.CreateCategoryConfiguration(row);
            continue;
          case WorkItemTypeEnum.TestSuite:
            settings.TestSuiteWorkItems = this.CreateCategoryConfiguration(row);
            continue;
          default:
            throw new ArgumentOutOfRangeException("WorkItemType", (object) row.WorkItemType, string.Empty);
        }
      }
      settings.PortfolioBacklogs = categoryConfigurationList.ToArray();
    }

    protected CategoryConfiguration CreateCategoryConfiguration(
      ProjectProcessConfigurationComponent2.WorkItemTypeRow row)
    {
      return new CategoryConfiguration()
      {
        CategoryReferenceName = row.CategoryRefName,
        PluralName = row.PluralDisplayName,
        SingularName = row.SingularDisplayName
      };
    }

    protected BacklogCategoryConfiguration CreateBacklogCategoryConfiguration(
      ProjectProcessConfigurationComponent2.WorkItemTypeRow row)
    {
      BacklogCategoryConfiguration categoryConfiguration = new BacklogCategoryConfiguration();
      categoryConfiguration.CategoryReferenceName = row.CategoryRefName;
      categoryConfiguration.PluralName = row.PluralDisplayName;
      categoryConfiguration.SingularName = row.SingularDisplayName;
      categoryConfiguration.ParentCategoryReferenceName = row.ParentCategoryRefName;
      categoryConfiguration.WorkItemCountLimit = row.WorkItemCountLimit;
      return categoryConfiguration;
    }

    protected void FillConfigurationCategoryStates(
      ProjectProcessConfiguration settings,
      IEnumerable<ProjectProcessConfigurationComponent2.WorkItemStateRow> rows)
    {
      foreach (IGrouping<WorkItemTypeEnum, ProjectProcessConfigurationComponent2.WorkItemStateRow> rows1 in rows.GroupBy<ProjectProcessConfigurationComponent2.WorkItemStateRow, WorkItemTypeEnum>((System.Func<ProjectProcessConfigurationComponent2.WorkItemStateRow, WorkItemTypeEnum>) (row => row.WorkItemType)))
      {
        switch (rows1.Key)
        {
          case WorkItemTypeEnum.Requirement:
            this.FillCategoryStates((CategoryConfiguration) settings.RequirementBacklog, (IEnumerable<ProjectProcessConfigurationComponent2.WorkItemStateRow>) rows1);
            continue;
          case WorkItemTypeEnum.Task:
            this.FillCategoryStates((CategoryConfiguration) settings.TaskBacklog, (IEnumerable<ProjectProcessConfigurationComponent2.WorkItemStateRow>) rows1);
            continue;
          case WorkItemTypeEnum.Bug:
            this.FillCategoryStates(settings.BugWorkItems, (IEnumerable<ProjectProcessConfigurationComponent2.WorkItemStateRow>) rows1);
            continue;
          case WorkItemTypeEnum.FeedbackRequest:
            this.FillCategoryStates(settings.FeedbackRequestWorkItems, (IEnumerable<ProjectProcessConfigurationComponent2.WorkItemStateRow>) rows1);
            continue;
          case WorkItemTypeEnum.FeedbackResponse:
            this.FillCategoryStates(settings.FeedbackResponseWorkItems, (IEnumerable<ProjectProcessConfigurationComponent2.WorkItemStateRow>) rows1);
            continue;
          case WorkItemTypeEnum.Portfolio:
            this.FillPortfolioBacklogCategoryStates(settings.PortfolioBacklogs, (IEnumerable<ProjectProcessConfigurationComponent2.WorkItemStateRow>) rows1);
            continue;
          case WorkItemTypeEnum.Release:
            this.FillCategoryStates(settings.ReleaseWorkItems, (IEnumerable<ProjectProcessConfigurationComponent2.WorkItemStateRow>) rows1);
            continue;
          case WorkItemTypeEnum.Stage:
            this.FillCategoryStates(settings.ReleaseStageWorkItems, (IEnumerable<ProjectProcessConfigurationComponent2.WorkItemStateRow>) rows1);
            continue;
          case WorkItemTypeEnum.SignoffTask:
            this.FillCategoryStates(settings.StageSignoffTaskWorkItems, (IEnumerable<ProjectProcessConfigurationComponent2.WorkItemStateRow>) rows1);
            continue;
          case WorkItemTypeEnum.TestPlan:
            this.FillCategoryStates(settings.TestPlanWorkItems, (IEnumerable<ProjectProcessConfigurationComponent2.WorkItemStateRow>) rows1);
            continue;
          case WorkItemTypeEnum.TestSuite:
            this.FillCategoryStates(settings.TestSuiteWorkItems, (IEnumerable<ProjectProcessConfigurationComponent2.WorkItemStateRow>) rows1);
            continue;
          default:
            continue;
        }
      }
    }

    protected void FillPortfolioBacklogCategoryStates(
      BacklogCategoryConfiguration[] backlogCategoryConfiguration,
      IEnumerable<ProjectProcessConfigurationComponent2.WorkItemStateRow> rows)
    {
      foreach (BacklogCategoryConfiguration categoryConfiguration in backlogCategoryConfiguration)
      {
        BacklogCategoryConfiguration category = categoryConfiguration;
        IEnumerable<ProjectProcessConfigurationComponent2.WorkItemStateRow> source = rows.Where<ProjectProcessConfigurationComponent2.WorkItemStateRow>((System.Func<ProjectProcessConfigurationComponent2.WorkItemStateRow, bool>) (s => TFStringComparer.WorkItemCategoryReferenceName.Equals(s.CategoryRefName, category.CategoryReferenceName)));
        category.States = source.Select<ProjectProcessConfigurationComponent2.WorkItemStateRow, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State>((System.Func<ProjectProcessConfigurationComponent2.WorkItemStateRow, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State>) (s => new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State()
        {
          Type = s.StateType,
          Value = s.StateValue
        })).ToArray<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State>();
      }
    }

    protected void FillCategoryStates(
      CategoryConfiguration category,
      IEnumerable<ProjectProcessConfigurationComponent2.WorkItemStateRow> rows)
    {
      ArgumentUtility.CheckForNull<CategoryConfiguration>(category, nameof (category));
      category.States = rows.Select<ProjectProcessConfigurationComponent2.WorkItemStateRow, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State>((System.Func<ProjectProcessConfigurationComponent2.WorkItemStateRow, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State>) (item => new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State()
      {
        Type = item.StateType,
        Value = item.StateValue
      })).ToArray<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State>();
    }

    protected void FillConfigurationGridColumns(
      ProjectProcessConfiguration settings,
      IEnumerable<ProjectProcessConfigurationComponent2.GridColumnRow> rows)
    {
      foreach (BacklogCategoryConfiguration allBacklogCategory in this.GetAllBacklogCategories(settings))
      {
        BacklogCategoryConfiguration category = allBacklogCategory;
        IEnumerable<ProjectProcessConfigurationComponent2.GridColumnRow> source = rows.Where<ProjectProcessConfigurationComponent2.GridColumnRow>((System.Func<ProjectProcessConfigurationComponent2.GridColumnRow, bool>) (r => TFStringComparer.WorkItemCategoryReferenceName.Equals(r.CategoryRefName, category.CategoryReferenceName)));
        category.Columns = source.Select<ProjectProcessConfigurationComponent2.GridColumnRow, Column>((System.Func<ProjectProcessConfigurationComponent2.GridColumnRow, Column>) (r => new Column()
        {
          ColumnWidth = r.DisplayWidth,
          FieldName = r.FieldRefName
        })).ToArray<Column>();
      }
    }

    protected void FillConfigurationAddPanelFields(
      ProjectProcessConfiguration settings,
      IEnumerable<ProjectProcessConfigurationComponent2.AddPanelFieldRow> rows)
    {
      foreach (BacklogCategoryConfiguration allBacklogCategory in this.GetAllBacklogCategories(settings))
      {
        BacklogCategoryConfiguration category = allBacklogCategory;
        IEnumerable<ProjectProcessConfigurationComponent2.AddPanelFieldRow> source = rows.Where<ProjectProcessConfigurationComponent2.AddPanelFieldRow>((System.Func<ProjectProcessConfigurationComponent2.AddPanelFieldRow, bool>) (s => TFStringComparer.WorkItemCategoryReferenceName.Equals(s.CategoryRefName, category.CategoryReferenceName)));
        category.AddPanel = new AddPanelConfiguration();
        category.AddPanel.Fields = source.Select<ProjectProcessConfigurationComponent2.AddPanelFieldRow, Field>((System.Func<ProjectProcessConfigurationComponent2.AddPanelFieldRow, Field>) (r => new Field()
        {
          Name = r.FieldRefName
        })).ToArray<Field>();
      }
    }

    protected void FillConfigurationWorkItemColors(
      ProjectProcessConfiguration settings,
      List<ProjectProcessConfigurationComponent2.WorkItemColorsRow> rows)
    {
      settings.WorkItemColors = rows.Select<ProjectProcessConfigurationComponent2.WorkItemColorsRow, WorkItemColor>((System.Func<ProjectProcessConfigurationComponent2.WorkItemColorsRow, WorkItemColor>) (r => new WorkItemColor()
      {
        WorkItemTypeName = r.WorkItemTypeName,
        PrimaryColor = r.Primary.ToString("X8"),
        SecondaryColor = r.Secondary.ToString("X8")
      })).ToArray<WorkItemColor>();
    }

    protected IEnumerable<BacklogCategoryConfiguration> GetAllBacklogCategories(
      ProjectProcessConfiguration settings)
    {
      List<BacklogCategoryConfiguration> backlogCategories = new List<BacklogCategoryConfiguration>();
      backlogCategories.Add(settings.TaskBacklog);
      backlogCategories.Add(settings.RequirementBacklog);
      backlogCategories.AddRange((IEnumerable<BacklogCategoryConfiguration>) settings.PortfolioBacklogs);
      return (IEnumerable<BacklogCategoryConfiguration>) backlogCategories;
    }

    protected IEnumerable<TypeFieldValueRow> GetTypeFieldValueRows(
      ProjectProcessConfiguration settings)
    {
      IList<TypeFieldValueRow> typeFieldValueRows = (IList<TypeFieldValueRow>) new List<TypeFieldValueRow>();
      foreach (TypeField typeField in settings.TypeFields)
      {
        TypeField field = typeField;
        if (field.TypeFieldValues != null)
        {
          foreach (TypeFieldValueRow typeFieldValueRow in ((IEnumerable<TypeFieldValue>) field.TypeFieldValues).Select<TypeFieldValue, TypeFieldValueRow>((System.Func<TypeFieldValue, TypeFieldValueRow>) (item => new TypeFieldValueRow()
          {
            FieldType = field.Type,
            Type = item.Type ?? "",
            Value = item.Value ?? ""
          })).ToArray<TypeFieldValueRow>())
            typeFieldValueRows.Add(typeFieldValueRow);
        }
      }
      return (IEnumerable<TypeFieldValueRow>) typeFieldValueRows;
    }

    protected IEnumerable<ProjectProcessConfigurationComponent2.WorkItemTypeRow> GetTypeRows(
      ProjectProcessConfiguration settings)
    {
      List<ProjectProcessConfigurationComponent2.WorkItemTypeRow> typeRows = new List<ProjectProcessConfigurationComponent2.WorkItemTypeRow>();
      CategoryConfiguration requestWorkItems = settings.FeedbackRequestWorkItems;
      if (requestWorkItems != null)
        typeRows.Add(new ProjectProcessConfigurationComponent2.WorkItemTypeRow(requestWorkItems, WorkItemTypeEnum.FeedbackRequest));
      CategoryConfiguration responseWorkItems = settings.FeedbackResponseWorkItems;
      if (responseWorkItems != null)
        typeRows.Add(new ProjectProcessConfigurationComponent2.WorkItemTypeRow(responseWorkItems, WorkItemTypeEnum.FeedbackResponse));
      CategoryConfiguration bugWorkItems = settings.BugWorkItems;
      if (bugWorkItems != null)
        typeRows.Add(new ProjectProcessConfigurationComponent2.WorkItemTypeRow(bugWorkItems, WorkItemTypeEnum.Bug));
      CategoryConfiguration testPlanWorkItems = settings.TestPlanWorkItems;
      if (testPlanWorkItems != null)
        typeRows.Add(new ProjectProcessConfigurationComponent2.WorkItemTypeRow(testPlanWorkItems, WorkItemTypeEnum.TestPlan));
      CategoryConfiguration testSuiteWorkItems = settings.TestSuiteWorkItems;
      if (testSuiteWorkItems != null)
        typeRows.Add(new ProjectProcessConfigurationComponent2.WorkItemTypeRow(testSuiteWorkItems, WorkItemTypeEnum.TestSuite));
      BacklogCategoryConfiguration taskBacklog = settings.TaskBacklog;
      if (taskBacklog != null && taskBacklog.CategoryReferenceName != null)
        typeRows.Add(new ProjectProcessConfigurationComponent2.WorkItemTypeRow()
        {
          WorkItemType = WorkItemTypeEnum.Task,
          CategoryRefName = taskBacklog.CategoryReferenceName,
          PluralDisplayName = taskBacklog.PluralName,
          SingularDisplayName = taskBacklog.SingularName,
          WorkItemCountLimit = taskBacklog.WorkItemCountLimit
        });
      BacklogCategoryConfiguration requirementBacklog = settings.RequirementBacklog;
      if (requirementBacklog != null && requirementBacklog.CategoryReferenceName != null)
        typeRows.Add(new ProjectProcessConfigurationComponent2.WorkItemTypeRow()
        {
          WorkItemType = WorkItemTypeEnum.Requirement,
          CategoryRefName = requirementBacklog.CategoryReferenceName,
          PluralDisplayName = requirementBacklog.PluralName,
          SingularDisplayName = requirementBacklog.SingularName,
          WorkItemCountLimit = requirementBacklog.WorkItemCountLimit
        });
      if (settings.PortfolioBacklogs != null)
      {
        foreach (BacklogCategoryConfiguration portfolioBacklog in settings.PortfolioBacklogs)
        {
          if (portfolioBacklog != null)
            typeRows.Add(new ProjectProcessConfigurationComponent2.WorkItemTypeRow()
            {
              WorkItemType = WorkItemTypeEnum.Portfolio,
              CategoryRefName = portfolioBacklog.CategoryReferenceName,
              PluralDisplayName = portfolioBacklog.PluralName,
              SingularDisplayName = portfolioBacklog.SingularName,
              ParentCategoryRefName = portfolioBacklog.ParentCategoryReferenceName,
              WorkItemCountLimit = portfolioBacklog.WorkItemCountLimit
            });
        }
      }
      CategoryConfiguration releaseWorkItems = settings.ReleaseWorkItems;
      if (releaseWorkItems != null)
        typeRows.Add(new ProjectProcessConfigurationComponent2.WorkItemTypeRow(releaseWorkItems, WorkItemTypeEnum.Release));
      CategoryConfiguration releaseStageWorkItems = settings.ReleaseStageWorkItems;
      if (releaseStageWorkItems != null)
        typeRows.Add(new ProjectProcessConfigurationComponent2.WorkItemTypeRow(releaseStageWorkItems, WorkItemTypeEnum.Stage));
      CategoryConfiguration signoffTaskWorkItems = settings.StageSignoffTaskWorkItems;
      if (signoffTaskWorkItems != null)
        typeRows.Add(new ProjectProcessConfigurationComponent2.WorkItemTypeRow(signoffTaskWorkItems, WorkItemTypeEnum.SignoffTask));
      return (IEnumerable<ProjectProcessConfigurationComponent2.WorkItemTypeRow>) typeRows;
    }

    protected ProjectProcessConfigurationComponent2.WorkItemTypeRow GetRowFromCategory(
      CategoryConfiguration category,
      WorkItemTypeEnum type)
    {
      return new ProjectProcessConfigurationComponent2.WorkItemTypeRow()
      {
        WorkItemType = type,
        CategoryRefName = category.CategoryReferenceName,
        PluralDisplayName = category.PluralName,
        SingularDisplayName = category.SingularName
      };
    }

    protected IEnumerable<ProjectProcessConfigurationComponent2.WorkItemStateRow> GetStateRows(
      ProjectProcessConfiguration settings)
    {
      foreach (ProjectProcessConfigurationComponent2.WorkItemStateRow stateRow in this.GetStateRows(settings.FeedbackRequestWorkItems, WorkItemTypeEnum.FeedbackRequest))
        yield return stateRow;
      foreach (ProjectProcessConfigurationComponent2.WorkItemStateRow stateRow in this.GetStateRows(settings.FeedbackResponseWorkItems, WorkItemTypeEnum.FeedbackResponse))
        yield return stateRow;
      foreach (ProjectProcessConfigurationComponent2.WorkItemStateRow stateRow in this.GetStateRows((CategoryConfiguration) settings.RequirementBacklog, WorkItemTypeEnum.Requirement))
        yield return stateRow;
      foreach (ProjectProcessConfigurationComponent2.WorkItemStateRow stateRow in this.GetStateRows((CategoryConfiguration) settings.TaskBacklog, WorkItemTypeEnum.Task))
        yield return stateRow;
      foreach (ProjectProcessConfigurationComponent2.WorkItemStateRow stateRow in this.GetStateRows(settings.BugWorkItems, WorkItemTypeEnum.Bug))
        yield return stateRow;
      foreach (ProjectProcessConfigurationComponent2.WorkItemStateRow stateRow in this.GetStateRows(settings.TestPlanWorkItems, WorkItemTypeEnum.TestPlan))
        yield return stateRow;
      foreach (ProjectProcessConfigurationComponent2.WorkItemStateRow stateRow in this.GetStateRows(settings.TestSuiteWorkItems, WorkItemTypeEnum.TestSuite))
        yield return stateRow;
      BacklogCategoryConfiguration[] categoryConfigurationArray = settings.PortfolioBacklogs;
      for (int index = 0; index < categoryConfigurationArray.Length; ++index)
      {
        foreach (ProjectProcessConfigurationComponent2.WorkItemStateRow stateRow in this.GetStateRows((CategoryConfiguration) categoryConfigurationArray[index], WorkItemTypeEnum.Portfolio))
          yield return stateRow;
      }
      categoryConfigurationArray = (BacklogCategoryConfiguration[]) null;
      foreach (ProjectProcessConfigurationComponent2.WorkItemStateRow stateRow in this.GetStateRows(settings.ReleaseWorkItems, WorkItemTypeEnum.Release))
        yield return stateRow;
      foreach (ProjectProcessConfigurationComponent2.WorkItemStateRow stateRow in this.GetStateRows(settings.ReleaseStageWorkItems, WorkItemTypeEnum.Stage))
        yield return stateRow;
      foreach (ProjectProcessConfigurationComponent2.WorkItemStateRow stateRow in this.GetStateRows(settings.StageSignoffTaskWorkItems, WorkItemTypeEnum.SignoffTask))
        yield return stateRow;
    }

    protected IEnumerable<ProjectProcessConfigurationComponent2.WorkItemStateRow> GetStateRows(
      CategoryConfiguration category,
      WorkItemTypeEnum workItemType)
    {
      return category != null && category.States != null && category.States.Length != 0 ? ((IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State>) category.States).Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State, ProjectProcessConfigurationComponent2.WorkItemStateRow>((System.Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.State, ProjectProcessConfigurationComponent2.WorkItemStateRow>) (state => new ProjectProcessConfigurationComponent2.WorkItemStateRow()
      {
        WorkItemType = workItemType,
        StateType = state.Type,
        StateValue = state.Value,
        CategoryRefName = category.CategoryReferenceName
      })) : Enumerable.Empty<ProjectProcessConfigurationComponent2.WorkItemStateRow>();
    }

    protected IEnumerable<ProjectProcessConfigurationComponent2.GridColumnRow> GetGridColumnRows(
      ProjectProcessConfiguration settings)
    {
      foreach (BacklogCategoryConfiguration allBacklogCategory in this.GetAllBacklogCategories(settings))
      {
        foreach (ProjectProcessConfigurationComponent2.GridColumnRow gridColumnRow in this.GetGridColumnRows(allBacklogCategory))
          yield return gridColumnRow;
      }
    }

    protected IEnumerable<ProjectProcessConfigurationComponent2.GridColumnRow> GetGridColumnRows(
      BacklogCategoryConfiguration backlogCategory)
    {
      return backlogCategory != null && backlogCategory.Columns != null && backlogCategory.Columns.Length != 0 ? ((IEnumerable<Column>) backlogCategory.Columns).Select<Column, ProjectProcessConfigurationComponent2.GridColumnRow>((Func<Column, int, ProjectProcessConfigurationComponent2.GridColumnRow>) ((item, index) => new ProjectProcessConfigurationComponent2.GridColumnRow()
      {
        FieldRefName = item.FieldName,
        DisplayWidth = item.ColumnWidth,
        Order = index,
        CategoryRefName = backlogCategory.CategoryReferenceName
      })) : Enumerable.Empty<ProjectProcessConfigurationComponent2.GridColumnRow>();
    }

    protected IEnumerable<ProjectProcessConfigurationComponent2.AddPanelFieldRow> GetAddPanelFieldRows(
      ProjectProcessConfiguration settings)
    {
      foreach (BacklogCategoryConfiguration allBacklogCategory in this.GetAllBacklogCategories(settings))
      {
        foreach (ProjectProcessConfigurationComponent2.AddPanelFieldRow addPanelFieldRow in this.GetAddPanelFieldRows(allBacklogCategory))
          yield return addPanelFieldRow;
      }
    }

    protected IEnumerable<ProjectProcessConfigurationComponent2.AddPanelFieldRow> GetAddPanelFieldRows(
      BacklogCategoryConfiguration backlogCategory)
    {
      return backlogCategory != null && backlogCategory.AddPanel != null && backlogCategory.AddPanel.Fields != null && backlogCategory.AddPanel.Fields.Length != 0 ? ((IEnumerable<Field>) backlogCategory.AddPanel.Fields).Select<Field, ProjectProcessConfigurationComponent2.AddPanelFieldRow>((Func<Field, int, ProjectProcessConfigurationComponent2.AddPanelFieldRow>) ((item, i) => new ProjectProcessConfigurationComponent2.AddPanelFieldRow()
      {
        FieldRefName = item.Name,
        Order = i,
        CategoryRefName = backlogCategory.CategoryReferenceName
      })) : Enumerable.Empty<ProjectProcessConfigurationComponent2.AddPanelFieldRow>();
    }

    protected IEnumerable<ProjectProcessConfigurationComponent2.WorkItemColorsRow> GetWorkItemColorRows(
      ProjectProcessConfiguration settings)
    {
      return settings.WorkItemColors != null && settings.WorkItemColors.Length != 0 ? ((IEnumerable<WorkItemColor>) settings.WorkItemColors).Select<WorkItemColor, ProjectProcessConfigurationComponent2.WorkItemColorsRow>((System.Func<WorkItemColor, ProjectProcessConfigurationComponent2.WorkItemColorsRow>) (c => new ProjectProcessConfigurationComponent2.WorkItemColorsRow()
      {
        WorkItemTypeName = c.WorkItemTypeName,
        Primary = Convert.ToInt32(c.PrimaryColor, 16),
        Secondary = Convert.ToInt32(c.SecondaryColor, 16)
      })) : Enumerable.Empty<ProjectProcessConfigurationComponent2.WorkItemColorsRow>();
    }

    protected virtual SqlParameter BindProjectConfigurationWorkItemTypeTable(
      string parameterName,
      IEnumerable<ProjectProcessConfigurationComponent2.WorkItemTypeRow> rows)
    {
      System.Func<ProjectProcessConfigurationComponent2.WorkItemTypeRow, SqlDataRecord> selector = (System.Func<ProjectProcessConfigurationComponent2.WorkItemTypeRow, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ProjectProcessConfigurationComponent2.typ_ProjectConfigurationWorkItemTypeTable2);
        ArgumentUtility.CheckForNull<ProjectProcessConfigurationComponent2.WorkItemTypeRow>(row, nameof (row));
        ArgumentUtility.CheckForNull<SqlDataRecord>(sqlDataRecord, "record");
        sqlDataRecord.SetByte(0, (byte) row.WorkItemType);
        sqlDataRecord.SetString(1, row.CategoryRefName);
        sqlDataRecord.SetNullableString(2, row.PluralDisplayName);
        sqlDataRecord.SetNullableString(3, row.SingularDisplayName);
        sqlDataRecord.SetNullableString(4, row.ParentCategoryRefName);
        sqlDataRecord.SetNullableInt32(5, new int?(row.WorkItemCountLimit));
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ProjectConfigurationWorkItemTypeTable2", rows.Select<ProjectProcessConfigurationComponent2.WorkItemTypeRow, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindProjectConfigurationWorkItemStateTable(
      string parameterName,
      IEnumerable<ProjectProcessConfigurationComponent2.WorkItemStateRow> rows)
    {
      return this.BindTable(parameterName, "typ_ProjectConfigurationWorkItemStateTable2", rows.Select<ProjectProcessConfigurationComponent2.WorkItemStateRow, SqlDataRecord>((System.Func<ProjectProcessConfigurationComponent2.WorkItemStateRow, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ProjectProcessConfigurationComponent2.typ_ProjectConfigurationWorkItemStateTable2);
        sqlDataRecord.SetByte(0, (byte) row.WorkItemType);
        sqlDataRecord.SetByte(1, (byte) row.StateType);
        sqlDataRecord.SetString(2, row.StateValue);
        sqlDataRecord.SetString(3, row.CategoryRefName);
        return sqlDataRecord;
      })));
    }

    protected virtual SqlParameter BindProjectConfigurationGridColumnTable(
      string parameterName,
      IEnumerable<ProjectProcessConfigurationComponent2.GridColumnRow> rows)
    {
      return this.BindTable(parameterName, "typ_ProjectConfigurationGridColumnTable2", rows.Select<ProjectProcessConfigurationComponent2.GridColumnRow, SqlDataRecord>((System.Func<ProjectProcessConfigurationComponent2.GridColumnRow, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ProjectProcessConfigurationComponent2.typ_ProjectConfigurationGridColumnTable2);
        sqlDataRecord.SetString(0, row.FieldRefName);
        sqlDataRecord.SetInt32(1, row.DisplayWidth);
        sqlDataRecord.SetInt32(2, row.Order);
        sqlDataRecord.SetString(3, row.CategoryRefName);
        return sqlDataRecord;
      })));
    }

    protected virtual SqlParameter BindProjectConfigurationAddPanelFieldsTable(
      string parameterName,
      IEnumerable<ProjectProcessConfigurationComponent2.AddPanelFieldRow> rows)
    {
      return this.BindTable(parameterName, "typ_ProjectConfigurationAddPanelFieldsTable", rows.Select<ProjectProcessConfigurationComponent2.AddPanelFieldRow, SqlDataRecord>((System.Func<ProjectProcessConfigurationComponent2.AddPanelFieldRow, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ProjectProcessConfigurationComponent2.typ_ProjectConfigurationAddPanelFieldsTable);
        sqlDataRecord.SetString(0, row.FieldRefName);
        sqlDataRecord.SetInt32(1, row.Order);
        sqlDataRecord.SetString(2, row.CategoryRefName);
        return sqlDataRecord;
      })));
    }

    protected virtual SqlParameter BindProjectConfigurationWorkItemColorsTable(
      string parameterName,
      IEnumerable<ProjectProcessConfigurationComponent2.WorkItemColorsRow> rows)
    {
      return this.BindTable(parameterName, "typ_ProjectConfigurationWorkItemColorsTable", rows.Select<ProjectProcessConfigurationComponent2.WorkItemColorsRow, SqlDataRecord>((System.Func<ProjectProcessConfigurationComponent2.WorkItemColorsRow, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ProjectProcessConfigurationComponent2.typ_ProjectConfigurationWorkItemColorsTable);
        sqlDataRecord.SetString(0, row.WorkItemTypeName);
        sqlDataRecord.SetInt32(1, row.Primary);
        sqlDataRecord.SetInt32(2, row.Secondary);
        return sqlDataRecord;
      })));
    }

    protected class AddPanelFieldRow
    {
      public string FieldRefName { get; set; }

      public int Order { get; set; }

      public string CategoryRefName { get; set; }
    }

    protected class AddPanelFieldRowBinder : 
      ObjectBinder<ProjectProcessConfigurationComponent2.AddPanelFieldRow>
    {
      private SqlColumnBinder FieldRefNameColumn = new SqlColumnBinder("FieldRefName");
      private SqlColumnBinder OrderColumn = new SqlColumnBinder("Order");
      private SqlColumnBinder CategoryRefNameColumn = new SqlColumnBinder("CategoryRefName");

      protected override ProjectProcessConfigurationComponent2.AddPanelFieldRow Bind() => new ProjectProcessConfigurationComponent2.AddPanelFieldRow()
      {
        FieldRefName = this.FieldRefNameColumn.GetString((IDataReader) this.Reader, false),
        Order = this.OrderColumn.GetInt32((IDataReader) this.Reader),
        CategoryRefName = this.CategoryRefNameColumn.GetString((IDataReader) this.Reader, false)
      };
    }

    protected class GridColumnRow
    {
      public string FieldRefName { get; set; }

      public int DisplayWidth { get; set; }

      public int Order { get; set; }

      public string CategoryRefName { get; set; }
    }

    protected class GridColumnRowBinder : 
      ObjectBinder<ProjectProcessConfigurationComponent2.GridColumnRow>
    {
      private SqlColumnBinder FieldRefNameColumn = new SqlColumnBinder("FieldRefName");
      private SqlColumnBinder DisplayWidthColumn = new SqlColumnBinder("DisplayWidth");
      private SqlColumnBinder OrderColumn = new SqlColumnBinder("Order");
      private SqlColumnBinder CategoryRefNameColumn = new SqlColumnBinder("CategoryRefName");

      protected override ProjectProcessConfigurationComponent2.GridColumnRow Bind() => new ProjectProcessConfigurationComponent2.GridColumnRow()
      {
        FieldRefName = this.FieldRefNameColumn.GetString((IDataReader) this.Reader, false),
        DisplayWidth = this.DisplayWidthColumn.GetInt32((IDataReader) this.Reader, 0),
        Order = this.OrderColumn.GetInt32((IDataReader) this.Reader),
        CategoryRefName = this.CategoryRefNameColumn.GetString((IDataReader) this.Reader, false)
      };
    }

    protected class WorkItemStateRow
    {
      public WorkItemTypeEnum WorkItemType { get; set; }

      public StateTypeEnum StateType { get; set; }

      public string StateValue { get; set; }

      public string CategoryRefName { get; set; }
    }

    protected class WorkItemStateRowBinder : 
      ObjectBinder<ProjectProcessConfigurationComponent2.WorkItemStateRow>
    {
      private SqlColumnBinder WorkItemTypeColumn = new SqlColumnBinder("WorkItemType");
      private SqlColumnBinder StateTypeColumn = new SqlColumnBinder("StateType");
      private SqlColumnBinder StateValueColumn = new SqlColumnBinder("StateValue");
      private SqlColumnBinder CategoryRefNameColumn = new SqlColumnBinder("CategoryRefName");

      protected override ProjectProcessConfigurationComponent2.WorkItemStateRow Bind() => new ProjectProcessConfigurationComponent2.WorkItemStateRow()
      {
        WorkItemType = (WorkItemTypeEnum) this.WorkItemTypeColumn.GetByte((IDataReader) this.Reader),
        StateType = (StateTypeEnum) this.StateTypeColumn.GetByte((IDataReader) this.Reader),
        StateValue = this.StateValueColumn.GetString((IDataReader) this.Reader, false),
        CategoryRefName = this.CategoryRefNameColumn.GetString((IDataReader) this.Reader, false)
      };
    }

    protected class WorkItemTypeRow
    {
      public WorkItemTypeRow()
      {
      }

      public WorkItemTypeRow(CategoryConfiguration category, WorkItemTypeEnum type)
      {
        this.WorkItemType = type;
        this.CategoryRefName = category.CategoryReferenceName;
        this.PluralDisplayName = category.PluralName;
        this.SingularDisplayName = category.SingularName;
      }

      public WorkItemTypeEnum WorkItemType { get; set; }

      public string CategoryRefName { get; set; }

      public string PluralDisplayName { get; set; }

      public string SingularDisplayName { get; set; }

      public string ParentCategoryRefName { get; set; }

      public int WorkItemCountLimit { get; set; }
    }

    protected class WorkItemTypeRowBinder : 
      ObjectBinder<ProjectProcessConfigurationComponent2.WorkItemTypeRow>
    {
      private SqlColumnBinder WorkItemTypeColumn = new SqlColumnBinder("WorkItemType");
      private SqlColumnBinder CategoryRefNameColumn = new SqlColumnBinder("CategoryRefName");
      private SqlColumnBinder PluralDisplayNameColumn = new SqlColumnBinder("PluralDisplayName");
      private SqlColumnBinder SingularDisplayNameColumn = new SqlColumnBinder("SingularDisplayName");
      private SqlColumnBinder ParentCategoryRefNameColumn = new SqlColumnBinder("ParentCategoryRefName");
      private SqlColumnBinder WorkItemCountLimit = new SqlColumnBinder(nameof (WorkItemCountLimit));

      protected override ProjectProcessConfigurationComponent2.WorkItemTypeRow Bind() => new ProjectProcessConfigurationComponent2.WorkItemTypeRow()
      {
        WorkItemType = (WorkItemTypeEnum) this.WorkItemTypeColumn.GetByte((IDataReader) this.Reader),
        CategoryRefName = this.CategoryRefNameColumn.GetString((IDataReader) this.Reader, false),
        PluralDisplayName = this.PluralDisplayNameColumn.GetString((IDataReader) this.Reader, true),
        SingularDisplayName = this.SingularDisplayNameColumn.GetString((IDataReader) this.Reader, true),
        ParentCategoryRefName = this.ParentCategoryRefNameColumn.GetString((IDataReader) this.Reader, true),
        WorkItemCountLimit = this.WorkItemCountLimit.GetInt32((IDataReader) this.Reader, 1000)
      };
    }

    protected class WorkItemColorsRow
    {
      public string WorkItemTypeName { get; set; }

      public int Primary { get; set; }

      public int Secondary { get; set; }
    }

    protected class WorkItemColorsRowBinder : 
      ObjectBinder<ProjectProcessConfigurationComponent2.WorkItemColorsRow>
    {
      private SqlColumnBinder WorkItemTypeNameColumn = new SqlColumnBinder("WorkItemTypeName");
      private SqlColumnBinder PrimaryColumn = new SqlColumnBinder("Primary");
      private SqlColumnBinder SecondaryColumn = new SqlColumnBinder("Secondary");

      protected override ProjectProcessConfigurationComponent2.WorkItemColorsRow Bind() => new ProjectProcessConfigurationComponent2.WorkItemColorsRow()
      {
        WorkItemTypeName = this.WorkItemTypeNameColumn.GetString((IDataReader) this.Reader, false),
        Primary = this.PrimaryColumn.GetInt32((IDataReader) this.Reader),
        Secondary = this.SecondaryColumn.GetInt32((IDataReader) this.Reader)
      };
    }
  }
}
