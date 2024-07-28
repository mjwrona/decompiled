// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.ProjectProcessConfigurationComponent3
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Settings.Legacy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class ProjectProcessConfigurationComponent3 : ProjectProcessConfigurationComponent2
  {
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
        resultCollection.AddBinder<KeyValuePair<string, string>>((ObjectBinder<KeyValuePair<string, string>>) new KeyValuePairStringTableBinder());
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
        resultCollection.NextResult();
        this.FillProjectConfigurationProperties(settings, (IEnumerable<KeyValuePair<string, string>>) resultCollection.GetCurrent<KeyValuePair<string, string>>().Items);
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
      this.BindKeyValuePairStringTableNullable("@propertyTable", this.GetProjectConfigurationProperties(settings));
      this.ExecuteNonQuery();
    }

    private void FillProjectConfigurationProperties(
      ProjectProcessConfiguration settings,
      IEnumerable<KeyValuePair<string, string>> pairs)
    {
      List<Property> propertyList = new List<Property>();
      if (pairs.Any<KeyValuePair<string, string>>())
        settings.Properties = Array.Empty<Property>();
      foreach (KeyValuePair<string, string> pair in pairs)
      {
        ProjectPropertiesEnum result;
        if (Enum.TryParse<ProjectPropertiesEnum>(pair.Key, out result))
        {
          switch (result)
          {
            case ProjectPropertiesEnum.ShowBugsOnBacklog:
            case ProjectPropertiesEnum.BugsBehavior:
            case ProjectPropertiesEnum.HiddenBacklogs:
            case ProjectPropertiesEnum.DuplicateWorkItemFlow:
            case ProjectPropertiesEnum.StateColors:
            case ProjectPropertiesEnum.WorkItemTypeIcons:
              propertyList.Add(new Property()
              {
                Name = pair.Key,
                Value = pair.Value
              });
              continue;
            default:
              continue;
          }
        }
      }
      settings.Properties = BugsBehaviorTranslator.TranslateProperties(propertyList.ToArray());
    }

    private IEnumerable<KeyValuePair<string, string>> GetProjectConfigurationProperties(
      ProjectProcessConfiguration settings)
    {
      if (settings.Properties == null)
        return Enumerable.Empty<KeyValuePair<string, string>>();
      settings.Properties = BugsBehaviorTranslator.TranslateProperties(settings.Properties);
      return ((IEnumerable<Property>) settings.Properties).Select<Property, KeyValuePair<string, string>>((System.Func<Property, KeyValuePair<string, string>>) (p => new KeyValuePair<string, string>(p.Name, p.Value)));
    }
  }
}
