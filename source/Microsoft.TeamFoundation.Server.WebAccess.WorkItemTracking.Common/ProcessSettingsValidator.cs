// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProcessSettingsValidator
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Settings;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Advanced)]
  public class ProcessSettingsValidator : SettingsValidator
  {
    public static Func<IVssRequestContext, ProjectProcessConfiguration, ISettingsValidatorDataProvider, bool, ProcessSettingsValidator> _Create = (Func<IVssRequestContext, ProjectProcessConfiguration, ISettingsValidatorDataProvider, bool, ProcessSettingsValidator>) ((requestContext, settings, dataProvider, correctWarnings) => new ProcessSettingsValidator(requestContext, settings, dataProvider, correctWarnings));
    private IVssRequestContext m_requestContext;
    private ProjectProcessConfiguration m_settings;
    private ISettingsValidatorDataProvider m_dataProvider;
    private bool m_correctWarnings;
    private const string ApplicationTypeWebApp = "WebApp";
    private const string ApplicationTypeRemoteMachine = "RemoteMachine";
    private const string ApplicationTypeClientApp = "ClientApp";
    private const string c_typeAttributeName = "type";
    private const string c_nameAttributeName = "name";
    private const string c_formatAttributeName = "format";
    private const string c_referenceNameAttributeName = "refname";
    private const string c_categoryNameAttributeName = "category";
    private const string c_pluralAttributeName = "pluralName";
    private const string c_singularAttributeName = "singularName";
    private const string c_parentAttributeName = "parent";
    private const string c_workItemCountLimitAttributeName = "workItemCountLimit";
    private const string c_valueAttributeName = "value";
    private const string c_colorsElementName = "WorkItemColor";
    private const string c_portfolioBacklogsElementName = "PortfolioBacklogs";
    private const string c_requirementBacklogElementName = "RequirementBacklog";
    private const string c_taskBacklogElementName = "TaskBacklog";
    private const string c_feedbackRequestItemsElementName = "FeedbackRequestWorkItems";
    private const string c_feedbackResponseItemsElementName = "FeedbackResponseWorkItems";
    private const string c_bugWorkItemsElementName = "BugWorkItems";
    private const string c_releaseWorkItemsElementName = "ReleaseWorkItems";
    private const string c_releaseStageWorkItemsElementName = "ReleaseStageWorkItems";
    private const string c_stageSignoffTaskWorkItemsElementName = "StageSignoffTaskWorkItems";
    private const string c_testPlanWorkItemsElementName = "TestPlanWorkItems";
    private const string c_testSuiteWorkItemsElementName = "TestSuiteWorkItems";
    private const string c_weekendsElementName = "Weekends";
    private const string c_statesElementName = "States";
    private const string c_stateElementName = "State";
    private const string c_parentNamePluralElement = "ParentNamePlural";
    private const string c_typeFieldsElementName = "TypeFields";
    private const string c_typeFieldElementName = "TypeField";
    private const string c_PropertiesElementName = "Properties";
    private const string c_columnsElementName = "Columns";
    private const string c_columnElementName = "Column";
    private const string c_addPanelElementName = "AddPanel";
    private const string c_columnWidthAttributeName = "ColumnWidth";
    private const string c_duplicateWorkItemFlowSupportedLinkType = "System.LinkTypes.Duplicate";
    private static readonly string[] ApplicationTypes = new string[3]
    {
      "WebApp",
      "RemoteMachine",
      "ClientApp"
    };
    private static readonly InternalFieldType[] ValidFieldTypesForAddPanel = new InternalFieldType[5]
    {
      InternalFieldType.String,
      InternalFieldType.Integer,
      InternalFieldType.Double,
      InternalFieldType.DateTime,
      InternalFieldType.TreePath
    };
    private NodeDescription m_typeFieldsNode = new NodeDescription()
    {
      Elements = new string[2]{ "TypeFields", "TypeField" }
    };
    private NodeDescription m_teamFieldNode = new NodeDescription()
    {
      Elements = new string[2]{ "TypeFields", "TypeField" },
      AttributeName = "type",
      AttributeValue = "Team"
    };
    private NodeDescription m_orderFieldNode = new NodeDescription()
    {
      Elements = new string[2]{ "TypeFields", "TypeField" },
      AttributeName = "type",
      AttributeValue = "Order"
    };
    private NodeDescription m_closedDateFieldNode = new NodeDescription()
    {
      Elements = new string[2]{ "TypeFields", "TypeField" },
      AttributeName = "type",
      AttributeValue = "ClosedDate"
    };
    private NodeDescription m_effortFieldNode = new NodeDescription()
    {
      Elements = new string[2]{ "TypeFields", "TypeField" },
      AttributeName = "type",
      AttributeValue = "Effort"
    };
    private NodeDescription m_remainingWorkFieldNode = new NodeDescription()
    {
      Elements = new string[2]{ "TypeFields", "TypeField" },
      AttributeName = "type",
      AttributeValue = "RemainingWork"
    };
    private NodeDescription m_activityFieldNode = new NodeDescription()
    {
      Elements = new string[2]{ "TypeFields", "TypeField" },
      AttributeName = "type",
      AttributeValue = "Activity"
    };
    private NodeDescription m_requestorFieldNode = new NodeDescription()
    {
      Elements = new string[2]{ "TypeFields", "TypeField" },
      AttributeName = "type",
      AttributeValue = "Requestor"
    };
    private NodeDescription m_applicationTypeFieldNode = new NodeDescription()
    {
      Elements = new string[2]{ "TypeFields", "TypeField" },
      AttributeName = "type",
      AttributeValue = "ApplicationType"
    };
    private NodeDescription m_applicationStartInformationFieldNode = new NodeDescription()
    {
      Elements = new string[2]{ "TypeFields", "TypeField" },
      AttributeName = "type",
      AttributeValue = "ApplicationStartInformation"
    };
    private NodeDescription m_applicationLaunchInstructionsFieldNode = new NodeDescription()
    {
      Elements = new string[2]{ "TypeFields", "TypeField" },
      AttributeName = "type",
      AttributeValue = "ApplicationLaunchInstructions"
    };
    private NodeDescription m_feedbackNotesFieldNode = new NodeDescription()
    {
      Elements = new string[2]{ "TypeFields", "TypeField" },
      AttributeName = "type",
      AttributeValue = "FeedbackNotes"
    };
    private NodeDescription m_colorsNode = new NodeDescription()
    {
      Elements = new string[1]{ "WorkItemColor" }
    };
    private NodeDescription m_propertiesNode = new NodeDescription()
    {
      Elements = new string[1]{ "Properties" }
    };
    private NodeDescription m_portfolioBacklogsNode = new NodeDescription()
    {
      Elements = new string[1]{ "PortfolioBacklogs" }
    };
    private NodeDescription m_requirementBacklogNode = new NodeDescription()
    {
      Elements = new string[1]{ "RequirementBacklog" }
    };
    private NodeDescription m_taskBacklogNode = new NodeDescription()
    {
      Elements = new string[1]{ "TaskBacklog" }
    };
    private NodeDescription m_feedbackRequestWorkItemsNode = new NodeDescription()
    {
      Elements = new string[1]{ "FeedbackRequestWorkItems" }
    };
    private NodeDescription m_feedbackResponseWorkItemsNode = new NodeDescription()
    {
      Elements = new string[1]
      {
        "FeedbackResponseWorkItems"
      }
    };
    private NodeDescription m_bugWorkItemsNode = new NodeDescription()
    {
      Elements = new string[1]{ "BugWorkItems" }
    };
    private NodeDescription m_releaseWorkItemsNode = new NodeDescription()
    {
      Elements = new string[1]{ "ReleaseWorkItems" }
    };
    private NodeDescription m_releaseStageWorkItemsNode = new NodeDescription()
    {
      Elements = new string[1]{ "ReleaseStageWorkItems" }
    };
    private NodeDescription m_stageSignoffTaskWorkItemsNode = new NodeDescription()
    {
      Elements = new string[1]
      {
        "StageSignoffTaskWorkItems"
      }
    };
    private NodeDescription m_testPlanWorkItemsNode = new NodeDescription()
    {
      Elements = new string[1]{ "TestPlanWorkItems" }
    };
    private NodeDescription m_testSuiteWorkItemsNode = new NodeDescription()
    {
      Elements = new string[1]{ "TestSuiteWorkItems" }
    };
    private NodeDescription m_weekendsNode = new NodeDescription()
    {
      Elements = new string[1]{ "Weekends" }
    };
    private NodeDescription m_feedbackRequestStatesNode;
    private NodeDescription m_feedbackResponseStatesNode;
    private NodeDescription m_bugStatesNode;
    private NodeDescription m_releaseStatesNode;
    private NodeDescription m_releaseStageStatesNode;
    private NodeDescription m_stageSignoffTaskStatesNode;
    private NodeDescription m_testPlanStatesNode;
    private NodeDescription m_testSuiteStatesNode;
    private Dictionary<FieldTypeEnum, NodeDescription> m_fieldNodes;
    private Dictionary<string, IEnumerable<string>> m_witStates;

    public static void Validate(
      IVssRequestContext requestContext,
      ProjectProcessConfiguration settings,
      string projectUri,
      bool correctWarnings)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      DefaultSettingsValidatorDataProvider dataProvider = new DefaultSettingsValidatorDataProvider(requestContext, projectUri);
      ProcessSettingsValidator.Validate(requestContext, settings, (ISettingsValidatorDataProvider) dataProvider, correctWarnings);
    }

    public static void Validate(
      IVssRequestContext requestContext,
      ProjectProcessConfiguration settings,
      ISettingsValidatorDataProvider dataProvider,
      bool correctWarnings)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ISettingsValidatorDataProvider>(dataProvider, nameof (dataProvider));
      ArgumentUtility.CheckForNull<ProjectProcessConfiguration>(settings, nameof (settings));
      if (settings.IsDefault)
        throw new MissingProjectSettingsException(Resources.Settings_MissingProjectSettings);
      ProcessSettingsValidator._Create(requestContext, settings, dataProvider, correctWarnings).Validate(requestContext, false);
    }

    public static void ValidateStructure(
      IVssRequestContext requestContext,
      ProjectProcessConfiguration settings)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ProjectProcessConfiguration>(settings, nameof (settings));
      if (settings.IsDefault)
        throw new MissingProjectSettingsException(Resources.Settings_MissingProjectSettings);
      ProcessSettingsValidator._Create(requestContext, settings, (ISettingsValidatorDataProvider) null, false).Validate(requestContext, true);
    }

    internal static void ValidateAndOrderPortfolioBacklogs(ProjectProcessConfiguration settings) => ProcessSettingsValidator._Create((IVssRequestContext) null, settings, (ISettingsValidatorDataProvider) null, true).ValidateAndOrderPortfolioBacklogsHierarchy();

    protected ProcessSettingsValidator()
    {
    }

    private ProcessSettingsValidator(
      IVssRequestContext requestContext,
      ProjectProcessConfiguration settings,
      ISettingsValidatorDataProvider dataProvider,
      bool correctWarnings)
    {
      this.m_requestContext = requestContext;
      this.m_settings = settings;
      this.m_dataProvider = dataProvider;
      this.m_correctWarnings = correctWarnings;
      this.m_feedbackRequestStatesNode = this.m_feedbackRequestWorkItemsNode.CreateChildNode("FeedbackRequestWorkItems");
      this.m_feedbackResponseStatesNode = this.m_feedbackResponseWorkItemsNode.CreateChildNode("FeedbackResponseWorkItems");
      this.m_bugStatesNode = this.m_bugWorkItemsNode.CreateChildNode("BugWorkItems");
      this.m_releaseStatesNode = this.m_releaseWorkItemsNode.CreateChildNode("ReleaseWorkItems");
      this.m_releaseStageStatesNode = this.m_releaseStageWorkItemsNode.CreateChildNode("ReleaseStageWorkItems");
      this.m_stageSignoffTaskStatesNode = this.m_stageSignoffTaskWorkItemsNode.CreateChildNode("StageSignoffTaskWorkItems");
      this.m_testPlanStatesNode = this.m_testPlanWorkItemsNode.CreateChildNode("TestPlanWorkItems");
      this.m_testSuiteStatesNode = this.m_testSuiteWorkItemsNode.CreateChildNode("TestSuiteWorkItems");
      this.m_fieldNodes = new Dictionary<FieldTypeEnum, NodeDescription>()
      {
        {
          FieldTypeEnum.Team,
          this.m_teamFieldNode
        },
        {
          FieldTypeEnum.Order,
          this.m_orderFieldNode
        },
        {
          FieldTypeEnum.ClosedDate,
          this.m_closedDateFieldNode
        },
        {
          FieldTypeEnum.Effort,
          this.m_effortFieldNode
        },
        {
          FieldTypeEnum.RemainingWork,
          this.m_remainingWorkFieldNode
        },
        {
          FieldTypeEnum.Activity,
          this.m_activityFieldNode
        },
        {
          FieldTypeEnum.Requestor,
          this.m_requestorFieldNode
        },
        {
          FieldTypeEnum.ApplicationType,
          this.m_applicationTypeFieldNode
        },
        {
          FieldTypeEnum.ApplicationStartInformation,
          this.m_applicationStartInformationFieldNode
        },
        {
          FieldTypeEnum.ApplicationLaunchInstructions,
          this.m_applicationLaunchInstructionsFieldNode
        },
        {
          FieldTypeEnum.FeedbackNotes,
          this.m_feedbackNotesFieldNode
        }
      };
      this.m_witStates = new Dictionary<string, IEnumerable<string>>();
    }

    public virtual void Validate(IVssRequestContext requestContext, bool validateStructureOnly)
    {
      PerformanceScenarioHelper scenarioHelper = new PerformanceScenarioHelper(requestContext, nameof (ProcessSettingsValidator), nameof (Validate));
      scenarioHelper.Add(nameof (validateStructureOnly), (object) validateStructureOnly);
      Exception exception = (Exception) null;
      try
      {
        this.ValidateBasicStructure(scenarioHelper);
        if (this.HasErrors || validateStructureOnly)
          return;
        this.ValidateContent(scenarioHelper);
      }
      catch (Exception ex)
      {
        exception = ex;
        this.m_requestContext.TraceException(290001, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, exception);
        this.Errors.Add(ex.Message);
      }
      finally
      {
        scenarioHelper.Add("hasErrors", (object) this.HasErrors);
        scenarioHelper.EndScenario();
        if (this.HasErrors)
        {
          this.m_requestContext.Trace(290001, TraceLevel.Error, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "project settings validation failed with {0}", (object) string.Join(",", (IEnumerable<string>) this.Errors));
          throw new InvalidProjectSettingsException((IEnumerable<string>) this.Errors, this.m_settings.GetType(), exception);
        }
      }
    }

    private NodeDescription GetPortfolioBacklogNode(BacklogCategoryConfiguration backlog) => new NodeDescription()
    {
      Elements = new string[1]
      {
        string.Format((IFormatProvider) CultureInfo.InvariantCulture, "PortfolioBacklog({0})", (object) backlog.CategoryReferenceName)
      }
    };

    private void ValidateBasicStructure(PerformanceScenarioHelper scenarioHelper)
    {
      using (scenarioHelper.Measure("ValidateColors"))
        this.ValidateColors();
      using (scenarioHelper.Measure("ValidateTypeFields"))
        this.ValidateTypeFields();
      using (scenarioHelper.Measure("ValidateCategoryStructures"))
        this.ValidateCategoryStructures();
      using (scenarioHelper.Measure("ValidateProperties"))
        this.ValidateProperties();
      using (scenarioHelper.Measure("ValidateWeekends"))
        this.ValidateWeekends();
    }

    private void ValidateColors()
    {
      if (this.m_settings.WorkItemColors == null)
        return;
      List<WorkItemColor> workItemColorList = new List<WorkItemColor>();
      foreach (WorkItemColor workItemColor in this.m_settings.WorkItemColors)
      {
        WorkItemColor color = workItemColor;
        if (string.IsNullOrEmpty(color.WorkItemTypeName))
          this.AddMissingAttributeError(this.m_colorsNode, "name");
        else if (this.m_dataProvider != null && this.m_dataProvider.WorkItemTypeExists(color.WorkItemTypeName))
        {
          if (workItemColorList.Exists((Predicate<WorkItemColor>) (c => TFStringComparer.WorkItemTypeName.Equals(color.WorkItemTypeName, c.WorkItemTypeName))))
            this.AddError(this.m_colorsNode, Resources.Validation_WorkItemColor_Duplicate, (object) color.WorkItemTypeName);
          else if (this.ValidateColorValueFormat(color))
            workItemColorList.Add(color);
        }
        else
          this.ValidateColorValueFormat(color);
      }
      if (!this.m_correctWarnings)
        return;
      this.m_settings.WorkItemColors = workItemColorList.ToArray();
    }

    private bool ValidateColorValueFormat(WorkItemColor color)
    {
      try
      {
        Convert.ToInt32(color.PrimaryColor, 16);
        Convert.ToInt32(color.SecondaryColor, 16);
        return true;
      }
      catch (Exception ex)
      {
        this.AddError(this.m_colorsNode, ex.Message);
      }
      return false;
    }

    private void ValidateProperties()
    {
      if (this.m_settings.Properties == null)
        return;
      List<Property> propertyList = new List<Property>();
      foreach (Property property1 in this.m_settings.Properties)
      {
        Property property = property1;
        if (string.IsNullOrEmpty(property.Name))
        {
          this.AddMissingAttributeError(this.m_propertiesNode, "name");
        }
        else
        {
          ProjectPropertiesEnum result;
          if (Enum.TryParse<ProjectPropertiesEnum>(property.Name, out result))
          {
            if (propertyList.Exists((Predicate<Property>) (p => p.Name.Equals(property.Name))))
            {
              this.AddError(this.m_propertiesNode, Resources.Validation_Propety_Duplicate, (object) property.Name);
            }
            else
            {
              propertyList.Add(property);
              switch (result)
              {
                case ProjectPropertiesEnum.ShowBugsOnBacklog:
                  if (!bool.TryParse(property.Value, out bool _))
                  {
                    this.AddError(this.m_propertiesNode, Resources.Validation_Property_NotBool, (object) property.Name);
                    continue;
                  }
                  continue;
                case ProjectPropertiesEnum.BugsBehavior:
                  if (!Enum.TryParse<BugsBehavior>(property.Value, out BugsBehavior _))
                  {
                    this.AddError(this.m_propertiesNode, Resources.Validation_BugsBehavior_Invalid, (object) property.Name);
                    continue;
                  }
                  continue;
                case ProjectPropertiesEnum.HiddenBacklogs:
                  if (!string.IsNullOrEmpty(property.Value) && this.m_settings.RequirementBacklog != null)
                  {
                    string[] strArray = new string[1]
                    {
                      this.m_settings.RequirementBacklog.CategoryReferenceName
                    };
                    if (this.m_settings.PortfolioBacklogs != null && this.m_settings.PortfolioBacklogs.Length != 0)
                      strArray = ((IEnumerable<string>) strArray).Union<string>(((IEnumerable<BacklogCategoryConfiguration>) this.m_settings.PortfolioBacklogs).Select<BacklogCategoryConfiguration, string>((Func<BacklogCategoryConfiguration, string>) (b => b.CategoryReferenceName))).ToArray<string>();
                    string[] array1 = ((IEnumerable<string>) property.Value.Split(new string[1]
                    {
                      ";"
                    }, StringSplitOptions.RemoveEmptyEntries)).Where<string>((Func<string, bool>) (x => !string.IsNullOrWhiteSpace(x))).Select<string, string>((Func<string, string>) (x => x.Trim())).ToArray<string>();
                    property.Value = string.Join(";", array1);
                    IEnumerable<string> strings = ((IEnumerable<string>) array1).Except<string>((IEnumerable<string>) strArray, (IEqualityComparer<string>) TFStringComparer.WorkItemCategoryReferenceName);
                    if (this.m_correctWarnings)
                    {
                      string[] array2 = ((IEnumerable<string>) array1).Except<string>(strings, (IEqualityComparer<string>) TFStringComparer.WorkItemCategoryReferenceName).ToArray<string>();
                      if (array2.Length == strArray.Length)
                        array2 = ((IEnumerable<string>) array2).Where<string>((Func<string, bool>) (c => !TFStringComparer.WorkItemCategoryReferenceName.Equals(c, this.m_settings.RequirementBacklog.CategoryReferenceName))).ToArray<string>();
                      property.Value = string.Join(";", array2);
                      continue;
                    }
                    if (strings.Any<string>() || array1.Length == strArray.Length)
                    {
                      this.AddError(this.m_propertiesNode, Resources.Validation_HiddenBacklogs_Invalid, (object) property.Name);
                      continue;
                    }
                    continue;
                  }
                  continue;
                case ProjectPropertiesEnum.DuplicateWorkItemFlow:
                  if (!string.IsNullOrEmpty(property.Value))
                  {
                    string str = property.Value;
                    bool flag1 = false;
                    try
                    {
                      DuplicateFlowConfiguration flowConfiguration = JsonConvert.DeserializeObject<DuplicateFlowConfiguration>(str);
                      if (string.IsNullOrEmpty(flowConfiguration.LinkTypeRefName) || string.IsNullOrEmpty(flowConfiguration.LinkTypePrimaryEndName) || string.IsNullOrEmpty(flowConfiguration.LinkTypeDuplicateEndName) || !TFStringComparer.WorkItemLinkTypeReferenceName.Equals(flowConfiguration.LinkTypeRefName, "System.LinkTypes.Duplicate") || flowConfiguration.WorkItemTypes == null || flowConfiguration.WorkItemTypes.Length == 0)
                      {
                        flag1 = true;
                        continue;
                      }
                      foreach (DuplicateFlowWorkItemType workItemType in flowConfiguration.WorkItemTypes)
                      {
                        if (!this.m_dataProvider.WorkItemTypeExists(workItemType.WorkItemTypeName))
                        {
                          flag1 = true;
                          break;
                        }
                        if (workItemType.ResolveAsDuplicate == null || workItemType.ResolveAsPrimary == null || workItemType.Unlink == null)
                        {
                          flag1 = true;
                          break;
                        }
                        bool flag2 = true;
                        foreach (DuplicateFlowWorkFieldValue flowWorkFieldValue in ((IEnumerable<DuplicateFlowWorkFieldValue>) workItemType.ResolveAsDuplicate).Concat<DuplicateFlowWorkFieldValue>((IEnumerable<DuplicateFlowWorkFieldValue>) workItemType.ResolveAsPrimary).Concat<DuplicateFlowWorkFieldValue>((IEnumerable<DuplicateFlowWorkFieldValue>) workItemType.Unlink))
                        {
                          if (string.IsNullOrEmpty(flowWorkFieldValue.ReferenceName) || flowWorkFieldValue.Value == null || !this.m_dataProvider.FieldExists(workItemType.WorkItemTypeName, flowWorkFieldValue.ReferenceName))
                          {
                            flag2 = false;
                            break;
                          }
                        }
                        if (!flag2)
                        {
                          flag1 = true;
                          break;
                        }
                      }
                      continue;
                    }
                    catch
                    {
                      flag1 = true;
                      continue;
                    }
                    finally
                    {
                      if (flag1)
                      {
                        if (!this.m_correctWarnings)
                        {
                          this.AddError(this.m_propertiesNode, Resources.Validation_DuplicateWorkItemFlow_Invalid, (object) property.Name);
                        }
                        else
                        {
                          this.m_requestContext.Trace(290001, TraceLevel.Error, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, Resources.Validation_DuplicateWorkItemFlow_Invalid);
                          property.Value = string.Empty;
                        }
                      }
                    }
                  }
                  else
                    continue;
                case ProjectPropertiesEnum.StateColors:
                  if (!string.IsNullOrEmpty(property.Value))
                  {
                    try
                    {
                      StateColorPropertyParser colorPropertyParser = new StateColorPropertyParser();
                      IDictionary<string, string> property2 = (IDictionary<string, string>) colorPropertyParser.ParseProperty(this.m_settings.StateColorPropertyValue);
                      property.Value = colorPropertyParser.CreatePropertyValue(property2);
                      continue;
                    }
                    catch (Exception ex)
                    {
                      if (!this.m_correctWarnings)
                      {
                        if (ex.Message.Length > 0)
                        {
                          this.AddError(this.m_propertiesNode, string.Format(Resources.Validation_StateColors_Invalid_With_Message, (object) property.Name, (object) ex.Message));
                          continue;
                        }
                        this.AddError(this.m_propertiesNode, Resources.Validation_StateColors_Invalid, (object) property.Name);
                        continue;
                      }
                      property.Value = string.Empty;
                      continue;
                    }
                  }
                  else
                    continue;
                case ProjectPropertiesEnum.WorkItemTypeIcons:
                  if (!string.IsNullOrEmpty(property.Value))
                  {
                    try
                    {
                      if (this.m_dataProvider != null)
                      {
                        KeyValuePair<string, string> keyValuePair = this.m_settings.WorkItemTypeIcons.FirstOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (p => !this.m_dataProvider.WorkItemTypeExists(p.Key)));
                        if (!string.IsNullOrEmpty(keyValuePair.Key))
                          throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.Validation_WorkItemTypeIcons_Type_Invalid((object) (keyValuePair.Key + "=" + keyValuePair.Value)));
                      }
                      property.Value = new WorkItemTypeIconPropertyParser().CreatePropertyValue((IDictionary<string, string>) this.m_settings.WorkItemTypeIcons);
                      continue;
                    }
                    catch (Exception ex)
                    {
                      if (!this.m_correctWarnings)
                      {
                        this.AddError(this.m_propertiesNode, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.Validation_WorkItemTypeIcons_Invalid_With_Message((object) property.Name, (object) ex.Message));
                        continue;
                      }
                      property.Value = string.Empty;
                      continue;
                    }
                  }
                  else
                    continue;
                default:
                  continue;
              }
            }
          }
          else
          {
            string str = string.Join(", ", Enum.GetNames(typeof (ProjectPropertiesEnum)));
            this.AddError(this.m_propertiesNode, Resources.Validation_Property_Unknown, (object) property.Name, (object) str);
          }
        }
      }
    }

    private void ValidateTypeFields()
    {
      if (this.m_settings.TypeFields == null || this.m_settings.TypeFields.Length == 0)
      {
        this.AddMissingElementError(new NodeDescription()
        {
          Elements = new string[1]{ "TypeFields" }
        });
      }
      else
      {
        this.ValidateField(this.m_settings.TeamField, this.m_teamFieldNode, false);
        this.ValidateField(this.m_settings.OrderByField, this.m_orderFieldNode, false);
        this.ValidateField(this.m_settings.ClosedDateField, this.m_closedDateFieldNode, false, true);
        this.ValidateField(this.m_settings.RemainingWorkField, this.m_remainingWorkFieldNode, true);
        this.ValidateField(this.m_settings.EffortField, this.m_effortFieldNode, false);
        this.ValidateField(this.m_settings.ApplicationTypeField, this.m_applicationTypeFieldNode, false, true);
        this.ValidateField(this.m_settings.ApplicationStartInformation, this.m_applicationStartInformationFieldNode, false, true);
        this.ValidateField(this.m_settings.ApplicationLaunchInstructions, this.m_applicationLaunchInstructionsFieldNode, false, true);
        this.ValidateField(this.m_settings.FeedbackNotes, this.m_feedbackNotesFieldNode, false, true);
        this.ValidateFieldsUnique();
      }
    }

    private void ValidateField(
      TypeField field,
      NodeDescription node,
      bool validateFormat,
      bool allowNull = false)
    {
      if (field == null)
      {
        if (allowNull)
          return;
        this.AddMissingElementError(node);
      }
      else
      {
        if (string.IsNullOrEmpty(field.Name))
          this.AddMissingAttributeError(node, "refname");
        if (!validateFormat)
          return;
        if (string.IsNullOrEmpty(field.Format))
        {
          this.AddMissingAttributeError(node, "format");
        }
        else
        {
          if (field.Format.Contains("{0}"))
            return;
          this.AddError(node, Resources.Validation_FormatMissingPlaceholder, (object) "format");
        }
      }
    }

    private void ValidateFieldsUnique()
    {
      Dictionary<string, bool> dictionary1 = new Dictionary<string, bool>(this.m_settings.TypeFields.Length, (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      Dictionary<FieldTypeEnum, bool> dictionary2 = new Dictionary<FieldTypeEnum, bool>(this.m_settings.TypeFields.Length);
      foreach (TypeField typeField in this.m_settings.TypeFields)
      {
        if (dictionary1.ContainsKey(typeField.Name))
          this.AddError(this.m_fieldNodes[typeField.Type], Resources.Validation_FieldReused, (object) typeField.Name, (object) this.m_typeFieldsNode.GetXPathString());
        if (dictionary2.ContainsKey(typeField.Type))
          this.AddError(this.m_fieldNodes[typeField.Type], Resources.Validation_FieldTypeDuplicated, (object) typeField.Type, (object) this.m_typeFieldsNode.GetXPathString());
        dictionary1[typeField.Name] = true;
        dictionary2[typeField.Type] = true;
      }
    }

    private void ValidateCategoryStructures()
    {
      if (this.m_settings.PortfolioBacklogs.Length > 5)
        this.AddError(this.m_portfolioBacklogsNode, Resources.Validation_PortfolioCountExceeded);
      foreach (BacklogCategoryConfiguration portfolioBacklog in this.m_settings.PortfolioBacklogs)
        this.ValidateBacklogStructure(portfolioBacklog, this.GetPortfolioBacklogNode(portfolioBacklog), true, true, true);
      this.ValidateBacklogStructure(this.m_settings.RequirementBacklog, this.m_requirementBacklogNode, true, false, true);
      if (string.IsNullOrWhiteSpace(this.m_settings.TaskBacklog.PluralName))
        this.m_settings.TaskBacklog.PluralName = Resources.DefaultTasksHubName;
      this.ValidateBacklogStructure(this.m_settings.TaskBacklog, this.m_taskBacklogNode, true, false, false);
      this.ValidateCategoryStructure(this.m_settings.FeedbackRequestWorkItems, this.m_feedbackRequestWorkItemsNode, false, false, true);
      this.ValidateCategoryStructure(this.m_settings.FeedbackResponseWorkItems, this.m_feedbackResponseWorkItemsNode, false, false, true);
      this.ValidateCategoryStructure(this.m_settings.BugWorkItems, this.m_bugWorkItemsNode, false, false, true);
      this.ValidateCategoryStructure(this.m_settings.ReleaseWorkItems, this.m_releaseWorkItemsNode, false, false, true);
      this.ValidateCategoryStructure(this.m_settings.ReleaseStageWorkItems, this.m_releaseStageWorkItemsNode, false, false, true);
      this.ValidateCategoryStructure(this.m_settings.StageSignoffTaskWorkItems, this.m_stageSignoffTaskWorkItemsNode, false, false, true);
      this.ValidateCategoryStructure(this.m_settings.TestPlanWorkItems, this.m_testPlanWorkItemsNode, false, false, true);
      this.ValidateCategoryStructure(this.m_settings.TestSuiteWorkItems, this.m_testSuiteWorkItemsNode, false, false, true);
    }

    private void ValidateBacklogStructure(
      BacklogCategoryConfiguration category,
      NodeDescription node,
      bool validatePlural,
      bool validateSingular,
      bool validateAddPanel)
    {
      this.ValidateCategoryStructure((CategoryConfiguration) category, node, validatePlural, validateSingular, false);
      if (category == null)
        return;
      NodeDescription childNode = node.CreateChildNode("Columns");
      if (childNode == null)
        this.AddMissingElementError(childNode);
      else if (category.Columns == null || category.Columns.Length == 0)
      {
        this.AddError(childNode, Resources.Validation_Columns_Required);
      }
      else
      {
        Dictionary<string, bool> dictionary = new Dictionary<string, bool>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
        foreach (Column column in category.Columns)
        {
          if (string.IsNullOrEmpty(column.FieldName))
            this.AddMissingAttributeError(childNode.CreateChildNode("Column"), "refname");
          if (dictionary.ContainsKey(column.FieldName))
            this.AddError(childNode.CreateChildNode("Column"), Resources.Validation_FieldReused, (object) column.FieldName, (object) childNode.GetXPathString());
          dictionary[column.FieldName] = true;
        }
      }
      if (category.WorkItemCountLimit != -1 && category.WorkItemCountLimit <= 0)
      {
        NodeDescription node1 = node.Clone();
        node1.AttributeName = "workItemCountLimit";
        node1.AttributeValue = category.WorkItemCountLimit.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        this.AddError(node1, Resources.Validation_InvalidWorkItemCountLimit);
      }
      if (validateAddPanel)
      {
        if (category.AddPanel != null && category.AddPanel.Fields != null && category.AddPanel.Fields.Length != 0)
          return;
        this.AddMissingElementError(node.CreateChildNode("AddPanel"));
      }
      else
      {
        if (category.AddPanel != null && category.AddPanel.Fields != null && category.AddPanel.Fields.Length != 0)
          return;
        BacklogCategoryConfiguration categoryConfiguration = category;
        AddPanelConfiguration panelConfiguration1 = new AddPanelConfiguration();
        panelConfiguration1.Fields = new Field[1]
        {
          new Field() { Name = CoreFieldReferenceNames.Title }
        };
        AddPanelConfiguration panelConfiguration2 = panelConfiguration1;
        categoryConfiguration.AddPanel = panelConfiguration2;
      }
    }

    private void ValidateCategoryStructure(
      CategoryConfiguration category,
      NodeDescription node,
      bool validatePlural,
      bool validateSingular,
      bool allowNull)
    {
      if (category == null)
      {
        if (allowNull)
          return;
        this.AddMissingElementError(node);
      }
      else
      {
        if (string.IsNullOrEmpty(category.CategoryReferenceName))
          this.AddMissingAttributeError(node, nameof (category));
        if (validatePlural && string.IsNullOrEmpty(category.PluralName))
          this.AddMissingAttributeError(node, "pluralName");
        else if (!string.IsNullOrEmpty(category.PluralName))
        {
          category.PluralName = category.PluralName.Trim();
          string pluralName = category.PluralName;
          if (!string.IsNullOrEmpty(pluralName) && !ProcessSettingsValidator.IsValidBacklogName(ref pluralName))
            this.AddError(node, Resources.PluralNameInvalid, (object) pluralName);
        }
        if (validateSingular && string.IsNullOrEmpty(category.SingularName))
          this.AddMissingAttributeError(node, "singularName");
        NodeDescription childNode = node.CreateChildNode("States");
        if (category.States == null)
          this.AddMissingElementError(childNode);
        else if (category.States.Length == 0)
        {
          this.AddError(childNode, Resources.Validation_States_Required);
        }
        else
        {
          Dictionary<string, bool> dictionary = new Dictionary<string, bool>(this.m_settings.TypeFields.Length, (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
          foreach (State state in category.States)
          {
            if (string.IsNullOrEmpty(state.Value))
              this.AddMissingAttributeError(childNode.CreateChildNode("State", "type", state.Type.ToString()), "value");
            if (dictionary.ContainsKey(state.Value))
              this.AddError(node, Resources.Validation_StateReused, (object) state.Value, (object) node.GetXPathString());
            dictionary[state.Value] = true;
          }
        }
      }
    }

    private static bool IsValidBacklogName(ref string pluralName)
    {
      if (string.IsNullOrWhiteSpace(pluralName))
        return false;
      pluralName = pluralName.Trim();
      return Microsoft.TeamFoundation.Core.WebApi.Internal.CssUtils.IsValidProjectName(pluralName);
    }

    private void ValidateContent(PerformanceScenarioHelper scenarioHelper)
    {
      using (scenarioHelper.Measure("ValidateWorkItemTypes"))
        this.ValidateWorkItemTypes();
      if (this.HasErrors)
        return;
      using (scenarioHelper.Measure("ValidateAndOrderPortfolioBacklogsHierarchy"))
        this.ValidateAndOrderPortfolioBacklogsHierarchy();
      if (this.HasErrors)
        return;
      using (scenarioHelper.Measure("FixAllStateNameMismatches"))
        this.FixAllStateNameMismatches();
      using (scenarioHelper.Measure("ValidateTeamField"))
        this.ValidateTeamField();
      using (scenarioHelper.Measure("ValidateEffortField"))
        this.ValidateEffortField();
      using (scenarioHelper.Measure("ValidateOrderByField"))
        this.ValidateOrderByField();
      using (scenarioHelper.Measure("ValidateClosedDateField"))
        this.ValidateClosedDateField();
      bool flag;
      using (scenarioHelper.Measure("ValidateRemainingWork"))
        flag = this.ValidateRemainingWork();
      if (flag)
      {
        using (scenarioHelper.Measure("ValidateActivityField"))
          this.ValidateActivityField();
      }
      using (scenarioHelper.Measure("ValidateAddPanels"))
        this.ValidateAddPanels();
      using (scenarioHelper.Measure("ValidateColumns"))
        this.ValidateColumns();
      using (scenarioHelper.Measure("ValidateBacklogsStates"))
        this.ValidateBacklogsStates();
      using (scenarioHelper.Measure("ValidateFeedbackFeature"))
        this.ValidateFeedbackFeature();
      using (scenarioHelper.Measure("ValidateBugStates"))
        this.ValidateBugStates();
      using (scenarioHelper.Measure("ValidateReleaseStates"))
        this.ValidateReleaseStates();
      using (scenarioHelper.Measure("ValidateReleaseStageStates"))
        this.ValidateReleaseStageStates();
      using (scenarioHelper.Measure("ValidateStageSignoffTaskStates"))
        this.ValidateStageSignoffTaskStates();
      using (scenarioHelper.Measure("ValidateTestPlanStates"))
        this.ValidateTestPlanStates();
      using (scenarioHelper.Measure("ValidateTestSuiteStates"))
        this.ValidateTestSuiteStates();
    }

    private IEnumerable<Tuple<BacklogCategoryConfiguration, NodeDescription>> AllBacklogPairs => ((IEnumerable<BacklogCategoryConfiguration>) this.m_settings.PortfolioBacklogs).Select<BacklogCategoryConfiguration, Tuple<BacklogCategoryConfiguration, NodeDescription>>((Func<BacklogCategoryConfiguration, Tuple<BacklogCategoryConfiguration, NodeDescription>>) (b => new Tuple<BacklogCategoryConfiguration, NodeDescription>(b, this.GetPortfolioBacklogNode(b)))).Concat<Tuple<BacklogCategoryConfiguration, NodeDescription>>((IEnumerable<Tuple<BacklogCategoryConfiguration, NodeDescription>>) new Tuple<BacklogCategoryConfiguration, NodeDescription>[2]
    {
      new Tuple<BacklogCategoryConfiguration, NodeDescription>(this.m_settings.RequirementBacklog, this.m_requirementBacklogNode),
      new Tuple<BacklogCategoryConfiguration, NodeDescription>(this.m_settings.TaskBacklog, this.m_taskBacklogNode)
    });

    private void ValidateAddPanels()
    {
      foreach (Tuple<BacklogCategoryConfiguration, NodeDescription> allBacklogPair in this.AllBacklogPairs)
      {
        BacklogCategoryConfiguration categoryConfiguration = allBacklogPair.Item1;
        NodeDescription nodeDescription = allBacklogPair.Item2;
        if (categoryConfiguration.AddPanel != null)
        {
          IEnumerable<string> wits = this.m_dataProvider.GetTypesInCategory(categoryConfiguration.CategoryReferenceName);
          IEnumerable<string> strings1 = categoryConfiguration.AddPanel.GetFieldNames().Where<string>((Func<string, bool>) (column => !wits.Any<string>((Func<string, bool>) (type => this.m_dataProvider.FieldExists(type, column)))));
          if (strings1.Any<string>())
          {
            if (this.m_correctWarnings)
              categoryConfiguration.AddPanel.Fields = categoryConfiguration.AddPanel.GetFieldNames().Except<string>(strings1).Select<string, Field>((Func<string, Field>) (c => new Field()
              {
                Name = c
              })).ToArray<Field>();
            else
              this.AddError(nodeDescription.CreateChildNode("AddPanel"), Resources.Validation_AddPanelColumnsInvalid, (object) string.Join(", ", strings1));
          }
          IEnumerable<string> strings2 = categoryConfiguration.AddPanel.GetFieldNames().GroupBy<string, string>((Func<string, string>) (column => column)).Where<IGrouping<string, string>>((Func<IGrouping<string, string>, bool>) (g => g.Count<string>() > 1)).Select<IGrouping<string, string>, string>((Func<IGrouping<string, string>, string>) (g => g.Key));
          if (strings2.Any<string>())
          {
            if (this.m_correctWarnings)
              categoryConfiguration.AddPanel.Fields = categoryConfiguration.AddPanel.GetFieldNames().Distinct<string>().Select<string, Field>((Func<string, Field>) (c => new Field()
              {
                Name = c
              })).ToArray<Field>();
            else
              this.AddError(nodeDescription.CreateChildNode("AddPanel"), Resources.Validation_AddPanelColumnsRepeated, (object) string.Join(", ", strings2));
          }
          IEnumerable<string> list = (IEnumerable<string>) categoryConfiguration.AddPanel.GetFieldNames().Where<string>((Func<string, bool>) (fRefName => wits.All<string>((Func<string, bool>) (wit => !this.IsValidAddPanelFieldForType(wit, fRefName))))).ToList<string>();
          if (list.Any<string>())
          {
            if (this.m_correctWarnings)
            {
              categoryConfiguration.AddPanel.Fields = categoryConfiguration.AddPanel.GetFieldNames().Except<string>(list).Select<string, Field>((Func<string, Field>) (c => new Field()
              {
                Name = c
              })).ToArray<Field>();
            }
            else
            {
              IEnumerable<string> values = ((IEnumerable<InternalFieldType>) ProcessSettingsValidator.ValidFieldTypesForAddPanel).Select<InternalFieldType, string>((Func<InternalFieldType, string>) (x => x.ToString()));
              this.AddError(nodeDescription.CreateChildNode("AddPanel"), Resources.Validation_AddPanelColumnsInvalidType, (object) string.Join(", ", values), (object) string.Join(", ", list));
            }
          }
        }
      }
    }

    private bool IsValidAddPanelFieldForType(string workItemTypeName, string fieldReferenceName) => this.m_dataProvider.FieldExists(workItemTypeName, fieldReferenceName) && ((IEnumerable<InternalFieldType>) ProcessSettingsValidator.ValidFieldTypesForAddPanel).Contains<InternalFieldType>(this.m_dataProvider.GetFieldType(workItemTypeName, fieldReferenceName));

    private void ValidateColumns()
    {
      foreach (Tuple<BacklogCategoryConfiguration, NodeDescription> allBacklogPair in this.AllBacklogPairs)
      {
        BacklogCategoryConfiguration categoryConfiguration = allBacklogPair.Item1;
        NodeDescription node = allBacklogPair.Item2;
        IEnumerable<string> strings = this.m_dataProvider.GetTypesInCategory(categoryConfiguration.CategoryReferenceName);
        if (categoryConfiguration == this.m_settings.TaskBacklog)
          strings = strings.Union<string>(this.m_dataProvider.GetTypesInCategory(this.m_settings.RequirementBacklog.CategoryReferenceName));
        categoryConfiguration.Columns = this.ValidateBacklogColumns(categoryConfiguration.Columns, node, strings);
      }
    }

    private Column[] ValidateBacklogColumns(
      Column[] columnsArray,
      NodeDescription node,
      IEnumerable<string> witTypes)
    {
      if (columnsArray != null)
      {
        IEnumerable<Column> columns1 = ((IEnumerable<Column>) columnsArray).Where<Column>((Func<Column, bool>) (column => !witTypes.Any<string>((Func<string, bool>) (wit => this.m_dataProvider.FieldExists(wit, column.FieldName)))));
        if (columns1.Any<Column>())
        {
          if (this.m_correctWarnings)
            columnsArray = ((IEnumerable<Column>) columnsArray).Except<Column>(columns1).ToArray<Column>();
          else
            this.AddError(node.CreateChildNode("Columns"), Resources.Validation_BacklogColumnInvalid, (object) string.Join(", ", columns1.Select<Column, string>((Func<Column, string>) (col => col.FieldName))));
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
            this.AddError(node.CreateChildNode("Columns"), Resources.Validation_BacklogColumnsRepeated, (object) string.Join(", ", strings));
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

    private void ValidateAndOrderPortfolioBacklogsHierarchy()
    {
      if (this.m_settings == null || this.m_settings.PortfolioBacklogs == null || this.m_settings.PortfolioBacklogs.Length == 0)
        return;
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.WorkItemCategoryName);
      bool flag = false;
      try
      {
        string str = (string) null;
        foreach (BacklogCategoryConfiguration portfolioBacklog in this.m_settings.PortfolioBacklogs)
        {
          if (string.IsNullOrEmpty(portfolioBacklog.ParentCategoryReferenceName))
          {
            if (!string.IsNullOrEmpty(str))
            {
              flag = true;
              return;
            }
            str = portfolioBacklog.CategoryReferenceName;
          }
          else
            dictionary[portfolioBacklog.ParentCategoryReferenceName] = portfolioBacklog.CategoryReferenceName;
        }
        if (string.IsNullOrEmpty(str))
        {
          flag = true;
        }
        else
        {
          List<string> source = new List<string>();
          source.Add(str);
          string key = str;
          while (source.Count < this.m_settings.PortfolioBacklogs.Length)
          {
            key = dictionary[key];
            if (source.Contains<string>(key, (IEqualityComparer<string>) TFStringComparer.WorkItemCategoryName))
            {
              flag = true;
              return;
            }
            source.Add(key);
          }
          if (dictionary.ContainsKey(key))
          {
            flag = true;
          }
          else
          {
            this.m_settings.PortfolioBacklogs = source.Select<string, BacklogCategoryConfiguration>((Func<string, BacklogCategoryConfiguration>) (categoryRefName => ((IEnumerable<BacklogCategoryConfiguration>) this.m_settings.PortfolioBacklogs).First<BacklogCategoryConfiguration>((Func<BacklogCategoryConfiguration, bool>) (b => TFStringComparer.WorkItemCategoryName.Equals(b.CategoryReferenceName, categoryRefName))))).ToArray<BacklogCategoryConfiguration>();
            if (this.m_settings.PortfolioBacklogs.Length != 0)
              this.m_settings.RequirementBacklog.ParentCategoryReferenceName = this.m_settings.PortfolioBacklogs[this.m_settings.PortfolioBacklogs.Length - 1].CategoryReferenceName;
            this.m_settings.TaskBacklog.ParentCategoryReferenceName = this.m_settings.RequirementBacklog.CategoryReferenceName;
          }
        }
      }
      catch
      {
        flag = true;
      }
      finally
      {
        if (flag)
          this.AddError(this.m_portfolioBacklogsNode, Resources.Validation_InvalidPortfolioHierarchy);
      }
    }

    private void ValidateWorkItemTypes()
    {
      Dictionary<string, string> typeMap = new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      Dictionary<string, string> dictionary1 = new Dictionary<string, string>((IEqualityComparer<string>) VssStringComparer.UrlPath);
      Dictionary<string, string> dictionary2 = new Dictionary<string, string>((IEqualityComparer<string>) VssStringComparer.UrlPath);
      foreach (Tuple<BacklogCategoryConfiguration, NodeDescription> allBacklogPair in this.AllBacklogPairs)
      {
        BacklogCategoryConfiguration backlog = allBacklogPair.Item1;
        NodeDescription node = allBacklogPair.Item2;
        WorkItemTypeCategory category = this.m_dataProvider.GetCategory(backlog.CategoryReferenceName);
        if (category == null)
        {
          this.AddError(node, Resources.Validation_InvalidCategoryName, (object) backlog.CategoryReferenceName);
        }
        else
        {
          if (this.m_correctWarnings)
            backlog.CategoryReferenceName = category.ReferenceName;
          this.ValidateWorkItemTypeMembership(typeMap, node, backlog);
          if (string.IsNullOrEmpty(backlog.SingularName))
            backlog.SingularName = this.m_dataProvider.GetDefaultTypeInCategory(backlog.CategoryReferenceName);
          if (dictionary1.ContainsKey(backlog.SingularName))
            this.AddError(node, Resources.Validation_UniqueBacklogName, (object) "singularName", (object) dictionary1[backlog.SingularName]);
          else
            dictionary1[backlog.SingularName] = node.GetXPathString();
          if (!string.IsNullOrEmpty(backlog.PluralName))
          {
            if (dictionary2.ContainsKey(backlog.PluralName))
              this.AddError(node, Resources.Validation_UniqueBacklogName, (object) "pluralName", (object) dictionary2[backlog.PluralName]);
            else
              dictionary2[backlog.PluralName] = node.GetXPathString();
          }
          if (!string.IsNullOrEmpty(backlog.ParentCategoryReferenceName) && !this.m_dataProvider.CategoryExists(backlog.ParentCategoryReferenceName))
            this.AddError(node, Resources.Validation_InvalidCategoryName, (object) backlog.ParentCategoryReferenceName);
        }
      }
      Tuple<CategoryConfiguration, NodeDescription>[] tupleArray = new Tuple<CategoryConfiguration, NodeDescription>[8]
      {
        new Tuple<CategoryConfiguration, NodeDescription>(this.m_settings.BugWorkItems, this.m_bugWorkItemsNode),
        new Tuple<CategoryConfiguration, NodeDescription>(this.m_settings.FeedbackRequestWorkItems, this.m_feedbackRequestWorkItemsNode),
        new Tuple<CategoryConfiguration, NodeDescription>(this.m_settings.FeedbackResponseWorkItems, this.m_feedbackResponseWorkItemsNode),
        new Tuple<CategoryConfiguration, NodeDescription>(this.m_settings.ReleaseWorkItems, this.m_releaseWorkItemsNode),
        new Tuple<CategoryConfiguration, NodeDescription>(this.m_settings.ReleaseStageWorkItems, this.m_releaseStageWorkItemsNode),
        new Tuple<CategoryConfiguration, NodeDescription>(this.m_settings.StageSignoffTaskWorkItems, this.m_stageSignoffTaskWorkItemsNode),
        new Tuple<CategoryConfiguration, NodeDescription>(this.m_settings.TestPlanWorkItems, this.m_testPlanWorkItemsNode),
        new Tuple<CategoryConfiguration, NodeDescription>(this.m_settings.TestSuiteWorkItems, this.m_testSuiteWorkItemsNode)
      };
      foreach (Tuple<CategoryConfiguration, NodeDescription> tuple in tupleArray)
      {
        CategoryConfiguration categoryConfiguration = tuple.Item1;
        NodeDescription node = tuple.Item2;
        if (categoryConfiguration != null)
        {
          if (!this.m_dataProvider.CategoryExists(categoryConfiguration.CategoryReferenceName))
          {
            this.AddError(node, Resources.Validation_InvalidCategoryName, (object) categoryConfiguration.CategoryReferenceName);
          }
          else
          {
            if (string.IsNullOrEmpty(categoryConfiguration.SingularName))
              categoryConfiguration.SingularName = this.m_dataProvider.GetDefaultTypeInCategory(categoryConfiguration.CategoryReferenceName);
            if (dictionary1.ContainsKey(categoryConfiguration.SingularName))
              this.AddError(node, Resources.Validation_UniqueBacklogName, (object) "singularName", (object) dictionary1[categoryConfiguration.SingularName]);
            else
              typeMap[categoryConfiguration.SingularName] = node.GetXPathString();
            if (!string.IsNullOrEmpty(categoryConfiguration.PluralName))
            {
              if (dictionary2.ContainsKey(categoryConfiguration.PluralName))
                this.AddError(node, Resources.Validation_UniqueBacklogName, (object) "pluralName", (object) dictionary2[categoryConfiguration.PluralName]);
              else
                typeMap[categoryConfiguration.PluralName] = node.GetXPathString();
            }
          }
        }
      }
    }

    private void ValidateWorkItemTypeMembership(
      Dictionary<string, string> typeMap,
      NodeDescription node,
      BacklogCategoryConfiguration backlog)
    {
      foreach (string key in this.m_dataProvider.GetTypesInCategory(backlog.CategoryReferenceName))
      {
        if (!typeMap.ContainsKey(key))
          typeMap[key] = node.GetXPathString();
        else
          this.AddError(node, Resources.Validation_IntersectingBacklogs, (object) key, (object) typeMap[key]);
      }
    }

    private void ValidateTeamField()
    {
      this.ValidateTeamFieldForBackLog();
      this.ValidateTeamFieldForTestPlan();
    }

    private void ValidateTeamFieldForBackLog()
    {
      foreach (BacklogCategoryConfiguration allBacklog in this.m_settings.AllBacklogs)
      {
        IEnumerable<string> typesInCategory = this.m_dataProvider.GetTypesInCategory(allBacklog.CategoryReferenceName);
        this.ValidateTeamField(allBacklog.CategoryReferenceName, typesInCategory);
      }
    }

    private void ValidateTeamFieldForTestPlan()
    {
      IEnumerable<string> typesInCategory = this.m_dataProvider.GetTypesInCategory("Microsoft.TestPlanCategory");
      if (typesInCategory == null)
        return;
      this.ValidateTeamField("Microsoft.TestPlanCategory", typesInCategory);
    }

    private void ValidateTeamField(
      string categoryReferenceName,
      IEnumerable<string> workItemTypesInCategory)
    {
      IEnumerable<string> witsMissingField = this.GetWitsMissingField(workItemTypesInCategory, this.m_settings.TeamField.Name);
      if (witsMissingField.Any<string>())
      {
        this.AddError(this.m_teamFieldNode, Resources.Validation_FieldRequiredOnAllWits, (object) this.m_settings.GetField(FieldTypeEnum.Team).Name, (object) categoryReferenceName, (object) string.Join(", ", witsMissingField));
      }
      else
      {
        if (!workItemTypesInCategory.Any<string>())
          return;
        string name = this.m_settings.TeamField.Name;
        int num = this.m_dataProvider.FieldExists(workItemTypesInCategory.First<string>(), name) ? 1 : 0;
        bool flag1 = TFStringComparer.WorkItemFieldReferenceName.Equals(name, CoreFieldReferenceNames.AreaPath);
        bool flag2 = FieldDefinition.IsSystemField(name);
        if (num != 0 && (flag1 || !flag2 && this.m_dataProvider.GetFieldType(workItemTypesInCategory.First<string>(), name) == InternalFieldType.String))
          return;
        this.AddError(this.m_teamFieldNode, Resources.Validation_TeamFieldInvalidType, (object) this.m_settings.TeamField.Name);
      }
    }

    private void ValidateEffortField()
    {
      IEnumerable<string> typesInCategory = this.m_dataProvider.GetTypesInCategory(this.m_settings.RequirementBacklog.CategoryReferenceName);
      IEnumerable<string> witsMissingField = this.GetWitsMissingField(typesInCategory, this.m_settings.EffortField.Name);
      if (witsMissingField.Any<string>())
      {
        this.AddError(this.m_requirementBacklogNode, Resources.Validation_FieldRequiredOnAllWits, (object) this.m_settings.EffortField.Name, (object) "RequirementBacklog", (object) string.Join(", ", witsMissingField));
      }
      else
      {
        if (!typesInCategory.Any<string>())
          return;
        if (!this.FieldIsNumeric(this.m_dataProvider.GetFieldType(typesInCategory.First<string>(), this.m_settings.EffortField.Name)))
          this.AddError(this.m_effortFieldNode, Resources.Validation_InvalidType_Number, (object) this.m_settings.EffortField.Name);
        if (!FieldDefinition.IsSystemField(this.m_settings.EffortField.Name))
          return;
        this.AddError(this.m_effortFieldNode, Resources.Validation_InvalidField_SystemField, (object) this.m_settings.EffortField.Name);
      }
    }

    private void ValidateOrderByField()
    {
      foreach (BacklogCategoryConfiguration categoryConfiguration in ((IEnumerable<BacklogCategoryConfiguration>) this.m_settings.PortfolioBacklogs).Concat<BacklogCategoryConfiguration>((IEnumerable<BacklogCategoryConfiguration>) new BacklogCategoryConfiguration[1]
      {
        this.m_settings.RequirementBacklog
      }))
      {
        IEnumerable<string> typesInCategory = this.m_dataProvider.GetTypesInCategory(categoryConfiguration.CategoryReferenceName);
        IEnumerable<string> witsMissingField = this.GetWitsMissingField(typesInCategory, this.m_settings.OrderByField.Name);
        if (witsMissingField.Any<string>())
          this.AddError(this.m_orderFieldNode, Resources.Validation_FieldRequiredOnAllWits, (object) this.m_settings.OrderByField.Name, (object) categoryConfiguration.CategoryReferenceName, (object) string.Join(", ", witsMissingField));
        else if (typesInCategory.Any<string>())
        {
          if (!this.FieldIsNumeric(this.m_dataProvider.GetFieldType(typesInCategory.First<string>(), this.m_settings.OrderByField.Name)))
            this.AddError(this.m_orderFieldNode, Resources.Validation_InvalidType_Number, (object) this.m_settings.OrderByField.Name);
          if (FieldDefinition.IsSystemField(this.m_settings.OrderByField.Name))
            this.AddError(this.m_orderFieldNode, Resources.Validation_InvalidField_SystemField, (object) this.m_settings.OrderByField.Name);
        }
      }
    }

    private void ValidateClosedDateField()
    {
      if (this.m_settings.ClosedDateField == null)
        return;
      string name = this.m_settings.ClosedDateField.Name;
      foreach (BacklogCategoryConfiguration categoryConfiguration in ((IEnumerable<BacklogCategoryConfiguration>) this.m_settings.PortfolioBacklogs).Concat<BacklogCategoryConfiguration>((IEnumerable<BacklogCategoryConfiguration>) new BacklogCategoryConfiguration[1]
      {
        this.m_settings.RequirementBacklog
      }))
      {
        IEnumerable<string> typesInCategory = this.m_dataProvider.GetTypesInCategory(categoryConfiguration.CategoryReferenceName);
        IEnumerable<string> witsMissingField = this.GetWitsMissingField(typesInCategory, name);
        if (witsMissingField.Any<string>())
          this.AddError(this.m_closedDateFieldNode, Resources.Validation_FieldRequiredOnAllWits, (object) this.m_settings.ClosedDateField.Name, (object) categoryConfiguration.CategoryReferenceName, (object) string.Join(", ", witsMissingField));
        else if (typesInCategory.Any<string>())
        {
          if (!this.FieldIsDate(this.m_dataProvider.GetFieldType(typesInCategory.First<string>(), this.m_settings.ClosedDateField.Name)))
            this.AddError(this.m_closedDateFieldNode, Resources.Validation_InvalidType_DateTime, (object) this.m_settings.ClosedDateField.Name);
          if (FieldDefinition.IsSystemField(name))
            this.AddError(this.m_closedDateFieldNode, Resources.Validation_InvalidField_SystemField, (object) name);
        }
      }
    }

    private bool ValidateRemainingWork()
    {
      bool flag = true;
      if (!this.m_dataProvider.GetTypesInCategory(this.m_settings.TaskBacklog.CategoryReferenceName).Where<string>((Func<string, bool>) (wit => this.m_dataProvider.FieldExists(wit, this.m_settings.RemainingWorkField.Name))).Any<string>())
      {
        this.AddError(this.m_remainingWorkFieldNode, Resources.Validation_RemainingWorkInvalid, (object) this.m_settings.RemainingWorkField.Name, (object) "TaskBacklog");
        flag = false;
      }
      return flag;
    }

    private void ValidateActivityField()
    {
      if (this.m_settings.ActivityField == null)
        return;
      IEnumerable<string> typesInCategory = this.m_dataProvider.GetTypesInCategory(this.m_settings.TaskBacklog.CategoryReferenceName);
      IEnumerable<string> strings = typesInCategory.Where<string>((Func<string, bool>) (wit => this.m_dataProvider.FieldExists(wit, this.m_settings.RemainingWorkField.Name) && !this.m_dataProvider.FieldExists(wit, this.m_settings.ActivityField.Name)));
      if (strings.Any<string>())
      {
        this.AddError(this.m_activityFieldNode, Resources.Validation_ActivityFieldNotDefined, (object) this.m_settings.ActivityField.Name, (object) this.m_taskBacklogNode.GetXPathString(), (object) this.m_remainingWorkFieldNode.GetXPathString(), (object) string.Join(", ", strings));
      }
      else
      {
        IEnumerable<string> source = typesInCategory.Where<string>((Func<string, bool>) (wit => this.m_dataProvider.FieldExists(wit, this.m_settings.RemainingWorkField.Name) && this.m_dataProvider.FieldExists(wit, this.m_settings.ActivityField.Name)));
        if (!source.Any<string>())
        {
          this.AddError(this.m_activityFieldNode, Resources.Validation_ActivityFieldNotFound, (object) this.m_settings.ActivityField.Name, (object) this.m_taskBacklogNode.GetXPathString());
        }
        else
        {
          if (this.m_dataProvider.GetFieldType(source.First<string>(), this.m_settings.ActivityField.Name) == InternalFieldType.String)
            return;
          this.AddError(this.m_activityFieldNode, Resources.Validation_ActivityFieldNotString, (object) this.m_settings.ActivityField.Name);
        }
      }
    }

    private void ValidateBacklogsStates()
    {
      foreach (Tuple<BacklogCategoryConfiguration, NodeDescription> allBacklogPair in this.AllBacklogPairs)
      {
        BacklogCategoryConfiguration categoryConfiguration = allBacklogPair.Item1;
        NodeDescription childNode = allBacklogPair.Item2.CreateChildNode("States");
        IEnumerable<string> typesInCategory = this.m_dataProvider.GetTypesInCategory(categoryConfiguration.CategoryReferenceName);
        IEnumerable<string> invalidStates = this.GetInvalidStates(((IEnumerable<State>) categoryConfiguration.States).Select<State, string>((Func<State, string>) (state => state.Value)), typesInCategory);
        if (invalidStates.Any<string>())
          this.AddError(childNode, Resources.Validation_DisplayStatesInvalid, (object) childNode.GetXPathString(), (object) string.Join(", ", invalidStates));
      }
    }

    private void ValidateApplicationTypeField()
    {
      if (!this.CheckIfFeedbackExists())
        return;
      TypeField applicationTypeField = this.m_settings.ApplicationTypeField;
      if (applicationTypeField == null || applicationTypeField.TypeFieldValues == null || applicationTypeField.TypeFieldValues.Length != ProcessSettingsValidator.ApplicationTypes.Length)
        this.AddError(this.m_applicationTypeFieldNode, Resources.FeedbackRequest_Error_WrongApplicationTypeConfiguration);
      foreach (string applicationType in ProcessSettingsValidator.ApplicationTypes)
      {
        string typeOfValue = applicationType;
        if (((IEnumerable<TypeFieldValue>) applicationTypeField.TypeFieldValues).Where<TypeFieldValue>((Func<TypeFieldValue, bool>) (v => v.Type == typeOfValue)).FirstOrDefault<TypeFieldValue>() == null)
          this.AddError(this.m_applicationTypeFieldNode, Resources.FeedbackRequest_Error_WrongApplicationTypeConfiguration);
      }
    }

    private void ValidateApplicationLaunchInstructions()
    {
      if (!this.CheckIfFeedbackExists() || this.m_settings.ApplicationLaunchInstructions != null)
        return;
      this.AddError(this.m_applicationLaunchInstructionsFieldNode, Resources.FeedbackRequest_Error_WrongApplicationLaunchInstructionConfiguration);
    }

    private void ValidateApplicationStartInformation()
    {
      if (!this.CheckIfFeedbackExists() || this.m_settings.ApplicationStartInformation != null)
        return;
      this.AddError(this.m_applicationStartInformationFieldNode, Resources.FeedbackRequest_Error_WrongApplicationStartInfoConfiguration);
    }

    private void ValidateFeedbackRequestStates()
    {
      if (!this.CheckIfFeedbackExists())
        return;
      if (this.m_settings.FeedbackRequestWorkItems == null)
      {
        this.AddMissingElementError(this.m_feedbackRequestWorkItemsNode);
      }
      else
      {
        IEnumerable<StateTypeEnum> source = ((IEnumerable<State>) this.m_settings.FeedbackRequestWorkItems.States).Select<State, StateTypeEnum>((Func<State, StateTypeEnum>) (s => s.Type));
        IEnumerable<string> strings = ((IEnumerable<State>) this.m_settings.FeedbackRequestWorkItems.States).Select<State, string>((Func<State, string>) (state => state.Value));
        if (!source.Contains<StateTypeEnum>(StateTypeEnum.InProgress) || !source.Contains<StateTypeEnum>(StateTypeEnum.Complete))
          this.AddError(this.m_feedbackRequestStatesNode, Resources.Validation_FeedbackRequestStatesNotComplete);
        IEnumerable<string> typesInCategory = this.m_dataProvider.GetTypesInCategory(this.m_settings.FeedbackRequestWorkItems.CategoryReferenceName);
        IEnumerable<string> invalidStates = this.GetInvalidStates(strings, typesInCategory);
        if (invalidStates.Any<string>())
          this.AddError(this.m_feedbackRequestStatesNode, Resources.Validation_FeedbackRequestStatesInvalid, (object) this.m_feedbackRequestStatesNode.GetXPathString(), (object) string.Join(", ", invalidStates));
        ICollection<string> values = this.ValidateInitialWorkItemState(typesInCategory, strings);
        if (values.Count <= 0)
          return;
        this.AddError(this.m_feedbackRequestWorkItemsNode, Resources.Validation_FeedbackWorkItem_MissingInitialState, (object) this.m_feedbackRequestWorkItemsNode.GetXPathString(), (object) string.Join(", ", (IEnumerable<string>) values));
      }
    }

    private void ValidateFeedbackResponseStates()
    {
      if (!this.CheckIfFeedbackExists())
        return;
      if (this.m_settings.FeedbackResponseWorkItems == null)
      {
        this.AddMissingElementError(this.m_feedbackResponseWorkItemsNode);
      }
      else
      {
        IEnumerable<StateTypeEnum> source = ((IEnumerable<State>) this.m_settings.FeedbackResponseWorkItems.States).Select<State, StateTypeEnum>((Func<State, StateTypeEnum>) (s => s.Type));
        IEnumerable<string> strings = ((IEnumerable<State>) this.m_settings.FeedbackResponseWorkItems.States).Select<State, string>((Func<State, string>) (state => state.Value));
        if (!source.Contains<StateTypeEnum>(StateTypeEnum.InProgress) || !source.Contains<StateTypeEnum>(StateTypeEnum.Complete))
          this.AddError(this.m_feedbackResponseStatesNode, Resources.Validation_FeedbackResponseStatesNotComplete);
        IEnumerable<string> typesInCategory = this.m_dataProvider.GetTypesInCategory(this.m_settings.FeedbackResponseWorkItems.CategoryReferenceName);
        IEnumerable<string> invalidStates = this.GetInvalidStates(strings, typesInCategory);
        if (invalidStates.Any<string>())
          this.AddError(this.m_feedbackResponseStatesNode, Resources.Validation_FeedbackResponseStatesInvalid, (object) this.m_feedbackResponseStatesNode.GetXPathString(), (object) string.Join(", ", invalidStates));
        ICollection<string> values = this.ValidateInitialWorkItemState(typesInCategory, strings);
        if (values.Count <= 0)
          return;
        this.AddError(this.m_feedbackResponseWorkItemsNode, Resources.Validation_FeedbackWorkItem_MissingInitialState, (object) this.m_feedbackResponseWorkItemsNode.GetXPathString(), (object) string.Join(", ", (IEnumerable<string>) values));
      }
    }

    private void ValidateTestPlanStates()
    {
      if (!this.CheckIfTestPlanExists())
        return;
      IEnumerable<StateTypeEnum> source = ((IEnumerable<State>) this.m_settings.TestPlanWorkItems.States).Select<State, StateTypeEnum>((Func<State, StateTypeEnum>) (s => s.Type)).Distinct<StateTypeEnum>();
      IEnumerable<string> states = ((IEnumerable<State>) this.m_settings.TestPlanWorkItems.States).Select<State, string>((Func<State, string>) (state => state.Value));
      if (!source.Contains<StateTypeEnum>(StateTypeEnum.InProgress) || !source.Contains<StateTypeEnum>(StateTypeEnum.Complete) || source.Count<StateTypeEnum>() != 2)
        this.AddError(this.m_testPlanStatesNode, Resources.Validation_TestPlanStatesNotComplete);
      IEnumerable<string> typesInCategory = this.m_dataProvider.GetTypesInCategory(this.m_settings.TestPlanWorkItems.CategoryReferenceName);
      IEnumerable<string> invalidStates = this.GetInvalidStates(states, typesInCategory);
      if (invalidStates.Any<string>())
        this.AddError(this.m_testPlanStatesNode, Resources.Validation_TestPlanStatesInvalid, (object) this.m_testPlanStatesNode.GetXPathString(), (object) string.Join(", ", invalidStates));
      IEnumerable<string> processConfigMapping = this.GetStatesWithoutProcessConfigMapping(states, typesInCategory);
      if (processConfigMapping.Any<string>())
        this.AddError(this.m_testPlanStatesNode, Resources.Validation_TestPlanWorkItem_StatesMissing, (object) string.Join(", ", processConfigMapping));
      IEnumerable<string> configuredStateNames = ((IEnumerable<State>) this.m_settings.TestPlanWorkItems.States).Where<State>((Func<State, bool>) (state => state.Type == StateTypeEnum.InProgress)).Select<State, string>((Func<State, string>) (state => state.Value));
      ICollection<string> strings = this.ValidateInitialWorkItemState(typesInCategory, configuredStateNames);
      if (!strings.Any<string>())
        return;
      this.AddError(this.m_testPlanStatesNode, Resources.Validation_TestPlanWorkItem_InvalidInitialStateMapping, (object) this.m_testPlanStatesNode.GetXPathString(), (object) string.Join(", ", (IEnumerable<string>) strings));
    }

    private void ValidateTestSuiteStates()
    {
      if (!this.CheckIfTestSuiteExists())
        return;
      IEnumerable<StateTypeEnum> source = ((IEnumerable<State>) this.m_settings.TestSuiteWorkItems.States).Select<State, StateTypeEnum>((Func<State, StateTypeEnum>) (s => s.Type)).Distinct<StateTypeEnum>();
      IEnumerable<string> states = ((IEnumerable<State>) this.m_settings.TestSuiteWorkItems.States).Select<State, string>((Func<State, string>) (state => state.Value));
      if (!source.Contains<StateTypeEnum>(StateTypeEnum.Proposed) || !source.Contains<StateTypeEnum>(StateTypeEnum.InProgress) || !source.Contains<StateTypeEnum>(StateTypeEnum.Complete) || source.Count<StateTypeEnum>() != 3)
        this.AddError(this.m_testSuiteStatesNode, Resources.Validation_TestSuiteStatesNotComplete);
      IEnumerable<string> typesInCategory = this.m_dataProvider.GetTypesInCategory(this.m_settings.TestSuiteWorkItems.CategoryReferenceName);
      IEnumerable<string> invalidStates = this.GetInvalidStates(states, typesInCategory);
      if (invalidStates.Any<string>())
        this.AddError(this.m_testSuiteStatesNode, Resources.Validation_TestSuiteStatesInvalid, (object) this.m_testSuiteStatesNode.GetXPathString(), (object) string.Join(", ", invalidStates));
      IEnumerable<string> processConfigMapping = this.GetStatesWithoutProcessConfigMapping(states, typesInCategory);
      if (processConfigMapping.Any<string>())
        this.AddError(this.m_testSuiteStatesNode, Resources.Validation_TestSuiteWorkItem_StatesMissing, (object) string.Join(", ", processConfigMapping));
      IEnumerable<string> configuredStateNames = ((IEnumerable<State>) this.m_settings.TestSuiteWorkItems.States).Where<State>((Func<State, bool>) (state => state.Type == StateTypeEnum.InProgress)).Select<State, string>((Func<State, string>) (state => state.Value));
      ICollection<string> strings = this.ValidateInitialWorkItemState(typesInCategory, configuredStateNames);
      if (!strings.Any<string>())
        return;
      this.AddError(this.m_testSuiteStatesNode, Resources.Validation_TestSuiteWorkItem_InvalidInitialStateMapping, (object) this.m_testSuiteStatesNode.GetXPathString(), (object) string.Join(", ", (IEnumerable<string>) strings));
    }

    private void ValidateWeekends()
    {
      if (this.m_settings.Weekends == null)
        return;
      foreach (IGrouping<DayOfWeek, DayOfWeek> source in ((IEnumerable<DayOfWeek>) this.m_settings.Weekends).GroupBy<DayOfWeek, DayOfWeek>((Func<DayOfWeek, DayOfWeek>) (dayEnum => dayEnum)).Where<IGrouping<DayOfWeek, DayOfWeek>>((Func<IGrouping<DayOfWeek, DayOfWeek>, bool>) (group => group.Count<DayOfWeek>() > 1)))
        this.AddError(this.m_weekendsNode, Resources.Validation_Weekends_Duplicate, (object) source.First<DayOfWeek>());
    }

    private void ValidateBugStates()
    {
      if (this.m_settings.BugWorkItems == null)
        return;
      IEnumerable<string> typesInCategory = this.m_dataProvider.GetTypesInCategory(this.m_settings.BugWorkItems.CategoryReferenceName);
      IEnumerable<string> invalidStates = this.GetInvalidStates(((IEnumerable<State>) this.m_settings.BugWorkItems.States).Select<State, string>((Func<State, string>) (state => state.Value)), typesInCategory);
      if (invalidStates.Any<string>())
        this.AddError(this.m_bugStatesNode, Resources.Validation_BugStatesInvalid, (object) this.m_bugWorkItemsNode.GetXPathString(), (object) string.Join(", ", invalidStates));
      ICollection<string> values = this.ValidateInitialWorkItemState(typesInCategory, ((IEnumerable<State>) this.m_settings.BugWorkItems.States).Select<State, string>((Func<State, string>) (state => state.Value)));
      if (values.Count <= 0)
        return;
      this.AddError(this.m_bugStatesNode, Resources.Validation_BugWorkItem_MissingInitialState, (object) this.m_bugWorkItemsNode.GetXPathString(), (object) string.Join(", ", (IEnumerable<string>) values));
    }

    private void ValidateReleaseStates()
    {
      if (this.m_settings.ReleaseWorkItems == null)
        return;
      IEnumerable<string> typesInCategory = this.m_dataProvider.GetTypesInCategory(this.m_settings.ReleaseWorkItems.CategoryReferenceName);
      IEnumerable<string> invalidStates = this.GetInvalidStates(((IEnumerable<State>) this.m_settings.ReleaseWorkItems.States).Select<State, string>((Func<State, string>) (state => state.Value)), typesInCategory);
      if (invalidStates.Any<string>())
        this.AddError(this.m_releaseStatesNode, Resources.Validation_ReleaseStatesInvalid, (object) this.m_releaseWorkItemsNode.GetXPathString(), (object) string.Join(", ", invalidStates));
      ICollection<string> values = this.ValidateInitialWorkItemState(typesInCategory, ((IEnumerable<State>) this.m_settings.ReleaseWorkItems.States).Select<State, string>((Func<State, string>) (state => state.Value)));
      if (values.Count <= 0)
        return;
      this.AddError(this.m_releaseStatesNode, Resources.Validation_ReleaseWorkItem_MissingInitialState, (object) this.m_releaseWorkItemsNode.GetXPathString(), (object) string.Join(", ", (IEnumerable<string>) values));
    }

    private void ValidateReleaseStageStates()
    {
      if (this.m_settings.ReleaseStageWorkItems == null)
        return;
      IEnumerable<string> typesInCategory = this.m_dataProvider.GetTypesInCategory(this.m_settings.ReleaseStageWorkItems.CategoryReferenceName);
      IEnumerable<string> invalidStates = this.GetInvalidStates(((IEnumerable<State>) this.m_settings.ReleaseStageWorkItems.States).Select<State, string>((Func<State, string>) (state => state.Value)), typesInCategory);
      if (invalidStates.Any<string>())
        this.AddError(this.m_releaseStageStatesNode, Resources.Validation_ReleaseStageStatesInvalid, (object) this.m_releaseStageWorkItemsNode.GetXPathString(), (object) string.Join(", ", invalidStates));
      ICollection<string> values = this.ValidateInitialWorkItemState(typesInCategory, ((IEnumerable<State>) this.m_settings.ReleaseStageWorkItems.States).Select<State, string>((Func<State, string>) (state => state.Value)));
      if (values.Count <= 0)
        return;
      this.AddError(this.m_releaseStageStatesNode, Resources.Validation_ReleaseStageWorkItem_MissingInitialState, (object) this.m_releaseStageWorkItemsNode.GetXPathString(), (object) string.Join(", ", (IEnumerable<string>) values));
    }

    private void ValidateStageSignoffTaskStates()
    {
      if (this.m_settings.StageSignoffTaskWorkItems == null)
        return;
      IEnumerable<string> typesInCategory = this.m_dataProvider.GetTypesInCategory(this.m_settings.StageSignoffTaskWorkItems.CategoryReferenceName);
      IEnumerable<string> invalidStates = this.GetInvalidStates(((IEnumerable<State>) this.m_settings.StageSignoffTaskWorkItems.States).Select<State, string>((Func<State, string>) (state => state.Value)), typesInCategory);
      if (invalidStates.Any<string>())
        this.AddError(this.m_stageSignoffTaskStatesNode, Resources.Validation_StageSignoffTaskStatesInvalid, (object) this.m_stageSignoffTaskWorkItemsNode.GetXPathString(), (object) string.Join(", ", invalidStates));
      ICollection<string> values = this.ValidateInitialWorkItemState(typesInCategory, ((IEnumerable<State>) this.m_settings.StageSignoffTaskWorkItems.States).Select<State, string>((Func<State, string>) (state => state.Value)));
      if (values.Count <= 0)
        return;
      this.AddError(this.m_stageSignoffTaskStatesNode, Resources.Validation_StageSignoffTaskWorkItem_MissingInitialState, (object) this.m_stageSignoffTaskWorkItemsNode.GetXPathString(), (object) string.Join(", ", (IEnumerable<string>) values));
    }

    private bool CheckIfFeedbackExists() => this.m_settings.FeedbackRequestWorkItems != null || this.m_settings.FeedbackResponseWorkItems != null || this.m_settings.FeedbackWorkItems != null || this.m_settings.ApplicationLaunchInstructions != null || this.m_settings.ApplicationStartInformation != null || this.m_settings.ApplicationTypeField != null;

    private bool CheckIfTestPlanExists() => this.m_settings.TestPlanWorkItems != null;

    private bool CheckIfTestSuiteExists() => this.m_settings.TestSuiteWorkItems != null;

    private ICollection<string> ValidateInitialWorkItemState(
      IEnumerable<string> workItemTypes,
      IEnumerable<string> configuredStateNames)
    {
      List<string> stringList = new List<string>();
      foreach (string workItemType in workItemTypes)
      {
        string wit = workItemType;
        if (!configuredStateNames.Any<string>((Func<string, bool>) (stateName => TFStringComparer.WorkItemStateName.Equals(stateName, this.m_dataProvider.GetTypeInitialState(wit)))))
          stringList.Add(wit);
      }
      return (ICollection<string>) stringList;
    }

    private IEnumerable<string> GetInvalidStates(
      IEnumerable<string> states,
      IEnumerable<string> wits)
    {
      return states.Except<string>(wits.Select<string, IEnumerable<string>>((Func<string, IEnumerable<string>>) (wit => this.GetTypeStates(wit))).SelectMany<IEnumerable<string>, string>((Func<IEnumerable<string>, IEnumerable<string>>) (stateList => stateList)));
    }

    private IEnumerable<string> GetStatesWithoutProcessConfigMapping(
      IEnumerable<string> states,
      IEnumerable<string> wits)
    {
      return wits.Select<string, IEnumerable<string>>((Func<string, IEnumerable<string>>) (wit => this.GetTypeStates(wit))).SelectMany<IEnumerable<string>, string>((Func<IEnumerable<string>, IEnumerable<string>>) (statesList => statesList)).Distinct<string>().Except<string>(states);
    }

    private IEnumerable<CategoryConfiguration> AllCategories => ((IEnumerable<CategoryConfiguration>) new CategoryConfiguration[8]
    {
      (CategoryConfiguration) this.m_settings.RequirementBacklog,
      (CategoryConfiguration) this.m_settings.TaskBacklog,
      this.m_settings.BugWorkItems,
      this.m_settings.FeedbackRequestWorkItems,
      this.m_settings.FeedbackResponseWorkItems,
      this.m_settings.ReleaseStageWorkItems,
      this.m_settings.ReleaseWorkItems,
      this.m_settings.StageSignoffTaskWorkItems
    }).Concat<CategoryConfiguration>((IEnumerable<CategoryConfiguration>) this.m_settings.PortfolioBacklogs);

    private void FixAllStateNameMismatches()
    {
      foreach (CategoryConfiguration allCategory in this.AllCategories)
      {
        if (allCategory != null)
          this.FixStateNameMismatches(this.m_requestContext, this.m_dataProvider.GetTypesInCategory(allCategory.CategoryReferenceName), (IEnumerable<State>) allCategory.States);
      }
    }

    private void FixStateNameMismatches(
      IVssRequestContext requestContext,
      IEnumerable<string> wits,
      IEnumerable<State> states)
    {
      if (wits == null || wits.Count<string>() == 0 || states == null || states.Count<State>() == 0)
        return;
      CultureInfo serverCulture = requestContext.GetService<WebAccessWorkItemService>().GetServerCulture(requestContext);
      IEnumerable<string> source = wits.Select<string, IEnumerable<string>>((Func<string, IEnumerable<string>>) (wit => this.GetTypeStates(wit))).Aggregate<IEnumerable<string>>((Func<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>>) ((set1, set2) => set1.Union<string>(set2)));
      foreach (State state1 in states)
      {
        State state = state1;
        string str = source.FirstOrDefault<string>((Func<string, bool>) (s => serverCulture.CompareInfo.Compare(s, state.Value, CompareOptions.IgnoreCase) == 0));
        if (!string.IsNullOrEmpty(str))
          state.Value = str;
      }
    }

    private IEnumerable<string> GetWitsMissingField(IEnumerable<string> wits, string field) => wits.Where<string>((Func<string, bool>) (wit => !this.m_dataProvider.FieldExists(wit, field)));

    private bool FieldIsNumeric(InternalFieldType fieldType) => fieldType == InternalFieldType.Integer || fieldType == InternalFieldType.Double;

    private bool FieldIsDate(InternalFieldType fieldType) => fieldType == InternalFieldType.DateTime;

    private void ValidateFeedbackFeature()
    {
      if (!this.CheckIfFeedbackExists())
        return;
      this.ValidateFeedbackRequestStates();
      this.ValidateFeedbackResponseStates();
      this.ValidateApplicationStartInformation();
      this.ValidateApplicationLaunchInstructions();
      this.ValidateApplicationTypeField();
    }

    private IEnumerable<string> GetTypeStates(string witName)
    {
      if (!this.m_witStates.ContainsKey(witName))
      {
        IEnumerable<string> typeStates = this.m_dataProvider.GetTypeStates(witName);
        this.m_witStates[witName] = (IEnumerable<string>) typeStates.ToList<string>();
      }
      return this.m_witStates[witName];
    }
  }
}
