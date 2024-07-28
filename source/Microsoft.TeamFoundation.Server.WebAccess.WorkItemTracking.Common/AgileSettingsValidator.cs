// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.AgileSettingsValidator
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class AgileSettingsValidator : SettingsValidator
  {
    private const string c_columnsElementName = "Columns";
    private const string c_columnElementName = "Column";
    private const string c_productBacklogAddPanelElementName = "AddPanel";
    private const string c_columnWidthAttributeName = "ColumnWidth";
    private const string c_referenceNameAttributeName = "refname";
    private const string c_iterationBacklogElementName = "IterationBacklog";
    private const string c_productBacklogElementName = "ProductBacklog";
    private const string c_requirementWorkItemsElementName = "RequirementWorkItems";
    private const string c_taskWorkItemsElementName = "TaskWorkItems";
    private const string c_workItemCountLimitAttributeName = "workItemCountLimit";
    private AgileProjectConfiguration m_settings;
    private bool m_correctWarnings;
    private ISettingsValidatorDataProvider m_dataProvider;
    private IEnumerable<string> m_parentWits;
    private IEnumerable<string> m_allWits;
    private NodeDescription m_workItemCountLimitNode = new NodeDescription()
    {
      Elements = new string[1]{ "IterationBacklog" },
      AttributeName = "workItemCountLimit",
      AttributeValue = ""
    };
    private NodeDescription m_productBacklogNode = new NodeDescription()
    {
      Elements = new string[1]{ "ProductBacklog" }
    };
    private NodeDescription m_iterationBacklogNode = new NodeDescription()
    {
      Elements = new string[1]{ "IterationBacklog" }
    };
    private NodeDescription m_requirementWorkItemsNode = new NodeDescription()
    {
      Elements = new string[1]{ "RequirementWorkItems" }
    };
    private NodeDescription m_taskWorkItemsNode = new NodeDescription()
    {
      Elements = new string[1]{ "TaskWorkItems" }
    };
    private NodeDescription m_addPanelNode = new NodeDescription();

    public AgileSettingsValidator(AgileProjectConfiguration agileSettings)
    {
      ArgumentUtility.CheckForNull<AgileProjectConfiguration>(agileSettings, nameof (agileSettings));
      this.m_settings = agileSettings;
      this.m_addPanelNode = this.m_productBacklogNode.CreateChildNode("AddPanel");
    }

    public void Validate(
      IVssRequestContext requestContext,
      string projectUri,
      bool correctWarnings)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectUri, nameof (projectUri));
      if (this.m_settings.IsDefault)
        throw new MissingProjectSettingsException(Resources.Settings_MissingProjectSettings);
      requestContext.GetService<ProjectConfigurationService>();
      CommonProjectConfiguration commonSettings = LegacySettingsConverter.GetCommonSettings(requestContext, projectUri, false);
      commonSettings.Validate(requestContext, projectUri, true);
      DefaultSettingsValidatorDataProvider dataProvider = new DefaultSettingsValidatorDataProvider(requestContext, projectUri);
      this.Validate(requestContext, (ISettingsValidatorDataProvider) dataProvider, commonSettings, correctWarnings);
    }

    public void Validate(
      IVssRequestContext requestContext,
      ISettingsValidatorDataProvider dataProvider,
      CommonProjectConfiguration commonSettings,
      bool correctWarnings)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ISettingsValidatorDataProvider>(dataProvider, nameof (dataProvider));
      ArgumentUtility.CheckForNull<CommonProjectConfiguration>(commonSettings, nameof (commonSettings));
      Exception exception = (Exception) null;
      try
      {
        this.m_dataProvider = dataProvider;
        this.m_correctWarnings = correctWarnings;
        if (commonSettings == null || commonSettings.IsDefault)
          this.Errors.Add(Resources.CommonSettingsMissing);
        else if (commonSettings.RequirementWorkItems == null || string.IsNullOrEmpty(commonSettings.RequirementWorkItems.CategoryName) || commonSettings.TaskWorkItems == null || string.IsNullOrEmpty(commonSettings.TaskWorkItems.CategoryName))
        {
          this.Errors.Add(Resources.CommonSettingsInvalid);
        }
        else
        {
          this.m_parentWits = this.m_dataProvider.GetTypesInCategory(commonSettings.RequirementWorkItems.CategoryName);
          this.m_allWits = this.m_dataProvider.GetTypesInCategory(commonSettings.TaskWorkItems.CategoryName);
          this.m_allWits = this.m_allWits.Union<string>(this.m_parentWits);
          this.ValidateBasicStructure();
          this.ValidateProductBacklog();
          this.ValidateIterationBacklog();
        }
      }
      catch (Exception ex)
      {
        exception = ex;
        requestContext.TraceException(290001, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, exception);
        this.Errors.Add(ex.Message);
      }
      finally
      {
        if (this.HasErrors)
        {
          requestContext.Trace(290001, TraceLevel.Error, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "Agile project settings validation failed with {0}", (object) string.Join(",", (IEnumerable<string>) this.Errors));
          throw new InvalidProjectSettingsException((IEnumerable<string>) this.Errors, this.m_settings.GetType(), exception);
        }
      }
    }

    public void ValidateBasicStructure()
    {
      this.ValidateBacklogStructure((BacklogConfiguration) this.m_settings.ProductBacklog, this.m_productBacklogNode);
      this.ValidateBacklogStructure((BacklogConfiguration) this.m_settings.IterationBacklog, this.m_iterationBacklogNode);
      if (this.HasErrors)
        throw new InvalidProjectSettingsException((IEnumerable<string>) this.Errors, this.m_settings.GetType());
    }

    private void ValidateBacklogStructure(BacklogConfiguration backlog, NodeDescription node)
    {
      if (backlog == null)
      {
        this.AddMissingElementError(node);
      }
      else
      {
        NodeDescription childNode = node.CreateChildNode("Columns");
        if (childNode == null)
          this.AddMissingElementError(childNode);
        else if (backlog.Columns == null || backlog.Columns.Length == 0)
        {
          this.AddError(childNode, Resources.Validation_Columns_Required);
        }
        else
        {
          Dictionary<string, bool> dictionary = new Dictionary<string, bool>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
          foreach (Column column in backlog.Columns)
          {
            if (string.IsNullOrEmpty(column.FieldName))
              this.AddMissingAttributeError(childNode.CreateChildNode("Column"), "refname");
            if (dictionary.ContainsKey(column.FieldName))
              this.AddError(childNode.CreateChildNode("Column"), Resources.Validation_FieldReused, (object) column.FieldName, (object) childNode.GetXPathString());
            dictionary[column.FieldName] = true;
          }
        }
      }
    }

    private void ValidateProductBacklog()
    {
      this.m_settings.ProductBacklog.Columns = this.ValidateBacklogColumns(this.m_settings.ProductBacklog.Columns, this.m_productBacklogNode, this.m_parentWits, this.m_requirementWorkItemsNode);
      this.ValidateProductBacklogAddPanel();
    }

    private void ValidateIterationBacklog()
    {
      this.m_settings.IterationBacklog.Columns = this.ValidateBacklogColumns(this.m_settings.IterationBacklog.Columns, this.m_iterationBacklogNode, this.m_allWits, this.m_taskWorkItemsNode);
      if (this.m_settings.IterationBacklog == null || this.m_settings.IterationBacklog.WorkItemCountLimit == -1 || this.m_settings.IterationBacklog.WorkItemCountLimit > 0)
        return;
      this.m_workItemCountLimitNode.AttributeValue = this.m_settings.IterationBacklog.WorkItemCountLimit.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.AddError(this.m_workItemCountLimitNode, Resources.Validation_InvalidWorkItemCountLimit);
    }

    private Column[] ValidateBacklogColumns(
      Column[] columnsArray,
      NodeDescription node,
      IEnumerable<string> witTypes,
      NodeDescription fieldContainerNode)
    {
      if (columnsArray != null)
      {
        IEnumerable<Column> columns1 = ((IEnumerable<Column>) columnsArray).Where<Column>((Func<Column, bool>) (column => !witTypes.Any<string>((Func<string, bool>) (wit => this.m_dataProvider.FieldExists(wit, column.FieldName)))));
        if (columns1.Any<Column>())
        {
          if (this.m_correctWarnings)
            columnsArray = ((IEnumerable<Column>) columnsArray).Except<Column>(columns1).ToArray<Column>();
          else
            this.AddError(node, Resources.Validation_BacklogColumnInvalid, (object) fieldContainerNode.GetXPathString(), (object) string.Join(", ", columns1.Select<Column, string>((Func<Column, string>) (col => col.FieldName))));
        }
        IEnumerable<string> strings = ((IEnumerable<Column>) columnsArray).GroupBy<Column, string>((Func<Column, string>) (column => column.FieldName)).Where<IGrouping<string, Column>>((Func<IGrouping<string, Column>, bool>) (g => g.Count<Column>() > 1)).Select<IGrouping<string, Column>, string>((Func<IGrouping<string, Column>, string>) (g => g.Key));
        if (strings.Any<string>())
        {
          if (this.m_correctWarnings)
          {
            Dictionary<string, Column> source = new Dictionary<string, Column>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
            foreach (Column columns2 in columnsArray)
              source[columns2.FieldName] = columns2;
            columnsArray = source.Select<KeyValuePair<string, Column>, Column>((Func<KeyValuePair<string, Column>, Column>) (keyValue => keyValue.Value)).ToArray<Column>();
          }
          else
            this.AddError(node, Resources.Validation_BacklogColumnsRepeated, (object) string.Join(", ", strings));
        }
        foreach (Column column in ((IEnumerable<Column>) columnsArray).Where<Column>((Func<Column, bool>) (column => column.ColumnWidth <= 0)))
        {
          if (this.m_correctWarnings)
            column.ColumnWidth = 100;
          else
            this.AddError(node, Resources.Validation_InvalidBacklogColumnWidths, (object) node.CreateChildNode("Column", "ColumnWidth", column.ColumnWidth.ToString((IFormatProvider) CultureInfo.InvariantCulture)), (object) column.FieldName);
        }
      }
      return columnsArray;
    }

    private void ValidateProductBacklogAddPanel()
    {
      if (this.m_settings.ProductBacklog.AddPanel == null)
      {
        this.AddMissingElementError(this.m_addPanelNode);
      }
      else
      {
        IEnumerable<string> strings1 = this.m_settings.ProductBacklog.AddPanel.GetFieldNames().Where<string>((Func<string, bool>) (column => !this.m_parentWits.Any<string>((Func<string, bool>) (type => this.m_dataProvider.FieldExists(type, column)))));
        if (strings1.Any<string>())
        {
          if (this.m_correctWarnings)
            this.m_settings.ProductBacklog.AddPanel.Fields = this.m_settings.ProductBacklog.AddPanel.GetFieldNames().Except<string>(strings1).Select<string, Field>((Func<string, Field>) (c => new Field()
            {
              Name = c
            })).ToArray<Field>();
          else
            this.AddError(this.m_addPanelNode, Resources.Validation_AddPanelColumnsInvalid, (object) this.m_requirementWorkItemsNode.GetXPathString(), (object) string.Join(", ", strings1));
        }
        IEnumerable<string> strings2 = this.m_settings.ProductBacklog.AddPanel.GetFieldNames().GroupBy<string, string>((Func<string, string>) (column => column)).Where<IGrouping<string, string>>((Func<IGrouping<string, string>, bool>) (g => g.Count<string>() > 1)).Select<IGrouping<string, string>, string>((Func<IGrouping<string, string>, string>) (g => g.Key));
        if (!strings2.Any<string>())
          return;
        if (this.m_correctWarnings)
          this.m_settings.ProductBacklog.AddPanel.Fields = this.m_settings.ProductBacklog.AddPanel.GetFieldNames().Distinct<string>().Select<string, Field>((Func<string, Field>) (c => new Field()
          {
            Name = c
          })).ToArray<Field>();
        else
          this.AddError(this.m_addPanelNode, Resources.Validation_AddPanelColumnsRepeated, (object) string.Join(", ", strings2));
      }
    }
  }
}
