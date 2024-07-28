// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.CommonSettingsValidator
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class CommonSettingsValidator : SettingsValidator
  {
    public const string ApplicationTypeWebApp = "WebApp";
    public const string ApplicationTypeRemoteMachine = "RemoteMachine";
    public const string ApplicationTypeClientApp = "ClientApp";
    private const string c_typeAttributeName = "type";
    private const string c_formatAttributeName = "format";
    private const string c_referenceNameAttributeName = "refname";
    private const string c_categoryNameAttributeName = "category";
    private const string c_pluralAttributeName = "plural";
    private const string c_valueAttributeName = "value";
    private const string c_requirementWorkItemsElementName = "RequirementWorkItems";
    private const string c_taskWorkItemsElementName = "TaskWorkItems";
    private const string c_feedbackRequestItemsElementName = "FeedbackRequestWorkItems";
    private const string c_feedbackResponseItemsElementName = "FeedbackResponseWorkItems";
    private const string c_bugWorkItemsElementName = "BugWorkItems";
    private const string c_statesElementName = "States";
    private const string c_stateElementName = "State";
    private const string c_parentNamePluralElement = "ParentNamePlural";
    private const string c_typeFieldsElementName = "TypeFields";
    private const string c_typeFieldElementName = "TypeField";
    private static readonly string[] ApplicationTypes = new string[3]
    {
      "WebApp",
      "RemoteMachine",
      "ClientApp"
    };
    private CommonProjectConfiguration m_settings;
    private bool m_correctWarnings;
    private ISettingsValidatorDataProvider m_dataProvider;
    private IEnumerable<string> m_childWits;
    private IEnumerable<string> m_parentWits;
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
    private NodeDescription m_requirementWorkItemsNode = new NodeDescription()
    {
      Elements = new string[1]{ "RequirementWorkItems" }
    };
    private NodeDescription m_taskWorkItemsNode = new NodeDescription()
    {
      Elements = new string[1]{ "TaskWorkItems" }
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
    private NodeDescription m_taskStatesNode;
    private NodeDescription m_requirementStatesNode;
    private NodeDescription m_feedbackRequestStatesNode;
    private NodeDescription m_feedbackResponseStatesNode;
    private NodeDescription m_bugStatesNode;
    private Dictionary<FieldTypeEnum, NodeDescription> m_fieldNodes;

    public CommonSettingsValidator(CommonProjectConfiguration settings)
    {
      ArgumentUtility.CheckForNull<CommonProjectConfiguration>(settings, nameof (settings));
      this.m_settings = settings;
      this.m_taskStatesNode = this.m_taskWorkItemsNode.CreateChildNode("States");
      this.m_requirementStatesNode = this.m_requirementWorkItemsNode.CreateChildNode("States");
      this.m_feedbackRequestStatesNode = this.m_feedbackRequestWorkItemsNode.CreateChildNode("FeedbackRequestWorkItems");
      this.m_feedbackResponseStatesNode = this.m_feedbackResponseWorkItemsNode.CreateChildNode("FeedbackResponseWorkItems");
      this.m_bugStatesNode = this.m_bugWorkItemsNode.CreateChildNode("BugWorkItems");
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
    }

    public void Validate(
      IVssRequestContext requestContext,
      string projectUri,
      bool correctWarnings,
      CommonProjectConfiguration.OptionalFeatures featuresToValidate = CommonProjectConfiguration.OptionalFeatures.None)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      DefaultSettingsValidatorDataProvider dataProvider = new DefaultSettingsValidatorDataProvider(requestContext, projectUri);
      this.Validate(requestContext, (ISettingsValidatorDataProvider) dataProvider, correctWarnings, featuresToValidate);
    }

    public void Validate(
      IVssRequestContext requestContext,
      ISettingsValidatorDataProvider dataProvider,
      bool correctWarnings,
      CommonProjectConfiguration.OptionalFeatures featuresToValidate = CommonProjectConfiguration.OptionalFeatures.None)
    {
      ArgumentUtility.CheckForNull<ISettingsValidatorDataProvider>(dataProvider, nameof (dataProvider));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (this.m_settings.IsDefault)
        throw new MissingProjectSettingsException(Resources.Settings_MissingProjectSettings);
      Exception exception = (Exception) null;
      try
      {
        this.m_correctWarnings = correctWarnings;
        this.m_dataProvider = dataProvider;
        this.ValidateBasicStructure();
        if (this.HasErrors)
          return;
        this.ValidateWorkItemTypes();
        this.FixAllStateNameMismatches(requestContext);
        this.ValidateTeamField();
        this.ValidateEffortField();
        this.ValidateOrderByField();
        this.ValidateClosedDateField();
        if (this.ValidateRemainingWork())
          this.ValidateActivityField();
        this.ValidateTaskStates();
        this.ValidateProductBacklogStates();
        if ((featuresToValidate & CommonProjectConfiguration.OptionalFeatures.Feedback) != CommonProjectConfiguration.OptionalFeatures.None)
          this.ValidateFeedbackFeature();
        this.ValidateBugStates();
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
          requestContext.Trace(290001, TraceLevel.Error, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "Common project settings validation failed with {0}", (object) string.Join(",", (IEnumerable<string>) this.Errors));
          throw new InvalidProjectSettingsException((IEnumerable<string>) this.Errors, this.m_settings.GetType(), exception);
        }
      }
    }

    public void ValidateBasicStructure()
    {
      List<string> stringList = new List<string>();
      if (this.m_settings.TypeFields == null)
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
      }
      this.ValidateCategory(this.m_settings.RequirementWorkItems, this.m_requirementWorkItemsNode, true);
      this.ValidateCategory(this.m_settings.TaskWorkItems, this.m_taskWorkItemsNode, false);
      this.ValidateFieldsUnique();
      this.ValidateCategory(this.m_settings.FeedbackRequestWorkItems, this.m_feedbackRequestWorkItemsNode, false, true);
      this.ValidateCategory(this.m_settings.FeedbackResponseWorkItems, this.m_feedbackResponseWorkItemsNode, false, true);
      this.ValidateCategory(this.m_settings.BugWorkItems, this.m_bugWorkItemsNode, false, true);
      if (this.HasErrors)
        throw new InvalidProjectSettingsException((IEnumerable<string>) this.Errors, this.m_settings.GetType());
    }

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

    private void ValidateCategory(
      WorkItemCategory category,
      NodeDescription node,
      bool validatePlural,
      bool allowNull = false)
    {
      if (category == null)
      {
        if (allowNull)
          return;
        this.AddMissingElementError(node);
      }
      else
      {
        if (string.IsNullOrEmpty(category.CategoryName))
          this.AddMissingAttributeError(node, nameof (category));
        if (validatePlural && string.IsNullOrEmpty(category.PluralName))
          this.AddMissingAttributeError(node, "plural");
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

    private void ValidateWorkItemTypes()
    {
      if (!this.m_dataProvider.CategoryExists(this.m_settings.RequirementWorkItems.CategoryName))
        this.AddError(this.m_requirementWorkItemsNode.CreateChildNode("category", this.m_settings.RequirementWorkItems.CategoryName), Resources.Validation_InvalidCategoryName, (object) this.m_settings.RequirementWorkItems.CategoryName);
      if (!this.m_dataProvider.CategoryExists(this.m_settings.TaskWorkItems.CategoryName))
        this.AddError(this.m_taskWorkItemsNode.CreateChildNode("category", this.m_settings.TaskWorkItems.CategoryName), Resources.Validation_InvalidCategoryName, (object) this.m_settings.TaskWorkItems.CategoryName);
      this.m_parentWits = this.m_dataProvider.GetTypesInCategory(this.m_settings.RequirementWorkItems.CategoryName);
      this.m_childWits = this.m_dataProvider.GetTypesInCategory(this.m_settings.TaskWorkItems.CategoryName);
      IEnumerable<string> strings = this.m_parentWits.Intersect<string>(this.m_childWits);
      if (!strings.Any<string>())
        return;
      this.AddError(this.m_remainingWorkFieldNode, Resources.Validation_ParentCannotBeChild, (object) this.m_taskWorkItemsNode.GetXPathString(), (object) string.Join(", ", strings));
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

    private void ValidateTeamField()
    {
      IEnumerable<string> witsMissingField = this.GetWitsMissingField(this.m_parentWits, this.m_settings.TeamField.Name);
      if (witsMissingField.Any<string>())
      {
        this.AddError(this.m_teamFieldNode, Resources.Validation_FieldRequiredOnAllWits, (object) this.m_settings.GetField(FieldTypeEnum.Team).Name, (object) "RequirementWorkItems", (object) string.Join(", ", witsMissingField));
      }
      else
      {
        if (!this.m_parentWits.Any<string>())
          return;
        string name = this.m_settings.TeamField.Name;
        int num = this.m_dataProvider.FieldExists(this.m_parentWits.First<string>(), name) ? 1 : 0;
        bool flag1 = TFStringComparer.WorkItemFieldReferenceName.Equals(name, CoreFieldReferenceNames.AreaPath);
        bool flag2 = TFStringComparer.WorkItemFieldReferenceName.StartsWith(name, "System.");
        if (num != 0 && (flag1 || !flag2 && this.m_dataProvider.GetFieldType(this.m_parentWits.First<string>(), name) == InternalFieldType.String))
          return;
        this.AddError(this.m_teamFieldNode, Resources.Validation_TeamFieldInvalidType, (object) this.m_settings.TeamField.Name);
      }
    }

    private void ValidateEffortField()
    {
      IEnumerable<string> witsMissingField = this.GetWitsMissingField(this.m_parentWits, this.m_settings.EffortField.Name);
      if (witsMissingField.Any<string>())
      {
        this.AddError(this.m_requirementWorkItemsNode, Resources.Validation_FieldRequiredOnAllWits, (object) this.m_settings.EffortField.Name, (object) "RequirementWorkItems", (object) string.Join(", ", witsMissingField));
      }
      else
      {
        if (!this.m_parentWits.Any<string>() || this.FieldIsNumeric(this.m_dataProvider.GetFieldType(this.m_parentWits.First<string>(), this.m_settings.EffortField.Name)))
          return;
        this.AddError(this.m_effortFieldNode, Resources.Validation_InvalidType_Number, (object) this.m_settings.EffortField.Name);
      }
    }

    private void ValidateOrderByField()
    {
      IEnumerable<string> witsMissingField = this.GetWitsMissingField(this.m_parentWits, this.m_settings.OrderByField.Name);
      if (witsMissingField.Any<string>())
      {
        this.AddError(this.m_orderFieldNode, Resources.Validation_FieldRequiredOnAllWits, (object) this.m_settings.OrderByField.Name, (object) "RequirementWorkItems", (object) string.Join(", ", witsMissingField));
      }
      else
      {
        if (!this.m_parentWits.Any<string>() || this.FieldIsNumeric(this.m_dataProvider.GetFieldType(this.m_parentWits.First<string>(), this.m_settings.OrderByField.Name)))
          return;
        this.AddError(this.m_orderFieldNode, Resources.Validation_InvalidType_Number, (object) this.m_settings.OrderByField.Name);
      }
    }

    private void ValidateClosedDateField()
    {
      if (this.m_settings.ClosedDateField == null)
        return;
      IEnumerable<string> witsMissingField = this.GetWitsMissingField(this.m_parentWits, this.m_settings.ClosedDateField.Name);
      if (witsMissingField.Any<string>())
      {
        this.AddError(this.m_closedDateFieldNode, Resources.Validation_FieldRequiredOnAllWits, (object) this.m_settings.ClosedDateField.Name, (object) "RequirementWorkItems", (object) string.Join(", ", witsMissingField));
      }
      else
      {
        if (!this.m_parentWits.Any<string>() || this.FieldIsDate(this.m_dataProvider.GetFieldType(this.m_parentWits.First<string>(), this.m_settings.ClosedDateField.Name)))
          return;
        this.AddError(this.m_closedDateFieldNode, Resources.Validation_InvalidType_DateTime, (object) this.m_settings.ClosedDateField.Name);
      }
    }

    private bool ValidateRemainingWork()
    {
      bool flag = true;
      if (!this.m_childWits.Where<string>((Func<string, bool>) (wit => this.m_dataProvider.FieldExists(wit, this.m_settings.RemainingWorkField.Name))).Any<string>())
      {
        this.AddError(this.m_remainingWorkFieldNode, Resources.Validation_RemainingWorkInvalid, (object) this.m_settings.RemainingWorkField.Name, (object) "TaskWorkItems");
        flag = false;
      }
      return flag;
    }

    private void ValidateActivityField()
    {
      if (this.m_settings.ActivityField == null)
        return;
      IEnumerable<string> strings = this.m_childWits.Where<string>((Func<string, bool>) (wit => this.m_dataProvider.FieldExists(wit, this.m_settings.RemainingWorkField.Name) && !this.m_dataProvider.FieldExists(wit, this.m_settings.ActivityField.Name)));
      if (strings.Any<string>())
      {
        this.AddError(this.m_activityFieldNode, Resources.Validation_ActivityFieldNotDefined, (object) this.m_settings.ActivityField.Name, (object) this.m_taskWorkItemsNode.GetXPathString(), (object) this.m_remainingWorkFieldNode.GetXPathString(), (object) string.Join(", ", strings));
      }
      else
      {
        IEnumerable<string> source = this.m_childWits.Where<string>((Func<string, bool>) (wit => this.m_dataProvider.FieldExists(wit, this.m_settings.RemainingWorkField.Name) && this.m_dataProvider.FieldExists(wit, this.m_settings.ActivityField.Name)));
        if (!source.Any<string>())
        {
          this.AddError(this.m_activityFieldNode, Resources.Validation_ActivityFieldNotFound, (object) this.m_settings.ActivityField.Name, (object) this.m_taskWorkItemsNode.GetXPathString());
        }
        else
        {
          if (this.m_dataProvider.GetFieldType(source.First<string>(), this.m_settings.ActivityField.Name) == InternalFieldType.String)
            return;
          this.AddError(this.m_activityFieldNode, Resources.Validation_ActivityFieldNotString, (object) this.m_settings.ActivityField.Name);
        }
      }
    }

    private void ValidateTaskStates()
    {
      IEnumerable<string> invalidStates = this.GetInvalidStates(((IEnumerable<State>) this.m_settings.TaskWorkItems.States).Select<State, string>((Func<State, string>) (state => state.Value)), this.m_childWits);
      if (invalidStates.Any<string>())
        this.AddError(this.m_taskStatesNode, Resources.Validation_DisplayStatesInvalid, (object) this.m_taskWorkItemsNode.GetXPathString(), (object) string.Join(", ", invalidStates));
      ICollection<string> values = this.ValidateInitialWorkItemState(this.m_childWits, ((IEnumerable<State>) this.m_settings.TaskWorkItems.States).Select<State, string>((Func<State, string>) (state => state.Value)));
      if (values.Count <= 0)
        return;
      this.AddError(this.m_taskWorkItemsNode, Resources.Validation_WorkItemMissingInitialState, (object) string.Join(", ", (IEnumerable<string>) values));
    }

    private void ValidateProductBacklogStates()
    {
      IEnumerable<string> invalidStates = this.GetInvalidStates(((IEnumerable<State>) this.m_settings.RequirementWorkItems.States).Select<State, string>((Func<State, string>) (state => state.Value)), this.m_parentWits);
      if (invalidStates.Any<string>())
        this.AddError(this.m_requirementStatesNode, Resources.Validation_ProductBacklogStatesInvalid, (object) this.m_requirementWorkItemsNode.GetXPathString(), (object) string.Join(", ", invalidStates));
      ICollection<string> values = this.ValidateInitialWorkItemState(this.m_parentWits, ((IEnumerable<State>) this.m_settings.RequirementWorkItems.States).Select<State, string>((Func<State, string>) (state => state.Value)));
      if (values.Count <= 0)
        return;
      this.AddError(this.m_requirementWorkItemsNode, Resources.Validation_WorkItemMissingInitialState, (object) string.Join(", ", (IEnumerable<string>) values));
    }

    private void ValidateApplicationTypeField()
    {
      if (!this.CheckIfFeedbackExists())
        return;
      TypeField applicationTypeField = this.m_settings.ApplicationTypeField;
      if (applicationTypeField == null || applicationTypeField.TypeFieldValues == null || applicationTypeField.TypeFieldValues.Length != CommonSettingsValidator.ApplicationTypes.Length)
        this.AddError(this.m_applicationTypeFieldNode, Resources.FeedbackRequest_Error_WrongApplicationTypeConfiguration);
      foreach (string applicationType in CommonSettingsValidator.ApplicationTypes)
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
      IEnumerable<StateTypeEnum> source = ((IEnumerable<State>) this.m_settings.FeedbackRequestWorkItems.States).Select<State, StateTypeEnum>((Func<State, StateTypeEnum>) (s => s.Type));
      IEnumerable<string> strings = ((IEnumerable<State>) this.m_settings.FeedbackRequestWorkItems.States).Select<State, string>((Func<State, string>) (state => state.Value));
      if (!source.Contains<StateTypeEnum>(StateTypeEnum.InProgress) || !source.Contains<StateTypeEnum>(StateTypeEnum.Complete))
        this.AddError(this.m_feedbackRequestStatesNode, Resources.Validation_FeedbackRequestStatesNotComplete);
      IEnumerable<string> typesInCategory = this.m_dataProvider.GetTypesInCategory(this.m_settings.FeedbackRequestWorkItems.CategoryName);
      IEnumerable<string> invalidStates = this.GetInvalidStates(strings, typesInCategory);
      if (invalidStates.Any<string>())
        this.AddError(this.m_feedbackRequestStatesNode, Resources.Validation_FeedbackRequestStatesInvalid, (object) this.m_feedbackRequestStatesNode.GetXPathString(), (object) string.Join(", ", invalidStates));
      ICollection<string> values = this.ValidateInitialWorkItemState(typesInCategory, strings);
      if (values.Count <= 0)
        return;
      this.AddError(this.m_feedbackRequestWorkItemsNode, Resources.Validation_FeedbackWorkItem_MissingInitialState, (object) this.m_feedbackRequestWorkItemsNode.GetXPathString(), (object) string.Join(", ", (IEnumerable<string>) values));
    }

    private void ValidateFeedbackResponseStates()
    {
      if (!this.CheckIfFeedbackExists())
        return;
      IEnumerable<StateTypeEnum> source = ((IEnumerable<State>) this.m_settings.FeedbackResponseWorkItems.States).Select<State, StateTypeEnum>((Func<State, StateTypeEnum>) (s => s.Type));
      IEnumerable<string> strings = ((IEnumerable<State>) this.m_settings.FeedbackResponseWorkItems.States).Select<State, string>((Func<State, string>) (state => state.Value));
      if (!source.Contains<StateTypeEnum>(StateTypeEnum.InProgress) || !source.Contains<StateTypeEnum>(StateTypeEnum.Complete))
        this.AddError(this.m_feedbackResponseStatesNode, Resources.Validation_FeedbackResponseStatesNotComplete);
      IEnumerable<string> typesInCategory = this.m_dataProvider.GetTypesInCategory(this.m_settings.FeedbackResponseWorkItems.CategoryName);
      this.GetInvalidStates(strings, typesInCategory);
      IEnumerable<string> invalidStates = this.GetInvalidStates(strings, typesInCategory);
      if (invalidStates.Any<string>())
        this.AddError(this.m_feedbackResponseStatesNode, Resources.Validation_FeedbackResponseStatesInvalid, (object) this.m_feedbackResponseStatesNode.GetXPathString(), (object) string.Join(", ", invalidStates));
      ICollection<string> values = this.ValidateInitialWorkItemState(typesInCategory, strings);
      if (values.Count <= 0)
        return;
      this.AddError(this.m_feedbackResponseWorkItemsNode, Resources.Validation_FeedbackWorkItem_MissingInitialState, (object) this.m_feedbackResponseWorkItemsNode.GetXPathString(), (object) string.Join(", ", (IEnumerable<string>) values));
    }

    private void ValidateBugStates()
    {
      if (this.m_settings.BugWorkItems == null)
        return;
      IEnumerable<string> typesInCategory = this.m_dataProvider.GetTypesInCategory(this.m_settings.BugWorkItems.CategoryName);
      IEnumerable<string> invalidStates = this.GetInvalidStates(((IEnumerable<State>) this.m_settings.BugWorkItems.States).Select<State, string>((Func<State, string>) (state => state.Value)), typesInCategory);
      if (invalidStates.Any<string>())
        this.AddError(this.m_bugStatesNode, Resources.Validation_BugStatesInvalid, (object) this.m_bugWorkItemsNode.GetXPathString(), (object) string.Join(", ", invalidStates));
      ICollection<string> values = this.ValidateInitialWorkItemState(typesInCategory, ((IEnumerable<State>) this.m_settings.BugWorkItems.States).Select<State, string>((Func<State, string>) (state => state.Value)));
      if (values.Count <= 0)
        return;
      this.AddError(this.m_bugStatesNode, Resources.Validation_BugWorkItem_MissingInitialState, (object) this.m_bugWorkItemsNode.GetXPathString(), (object) string.Join(", ", (IEnumerable<string>) values));
    }

    private bool CheckIfFeedbackExists() => this.m_settings.FeedbackRequestWorkItems != null || this.m_settings.FeedbackResponseWorkItems != null || this.m_settings.FeedbackWorkItems != null || this.m_settings.ApplicationLaunchInstructions != null || this.m_settings.ApplicationStartInformation != null || this.m_settings.ApplicationTypeField != null;

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
      return states.Except<string>(wits.Select<string, IEnumerable<string>>((Func<string, IEnumerable<string>>) (wit => this.m_dataProvider.GetTypeStates(wit))).SelectMany<IEnumerable<string>, string>((Func<IEnumerable<string>, IEnumerable<string>>) (stateList => stateList)));
    }

    private void FixAllStateNameMismatches(IVssRequestContext requestContext)
    {
      if (this.m_settings.TaskWorkItems != null)
        this.FixStateNameMismatches(requestContext, this.m_childWits, (IEnumerable<State>) this.m_settings.TaskWorkItems.States);
      if (this.m_settings.RequirementWorkItems != null)
        this.FixStateNameMismatches(requestContext, this.m_parentWits, (IEnumerable<State>) this.m_settings.RequirementWorkItems.States);
      if (this.m_settings.BugWorkItems != null)
      {
        IEnumerable<string> typesInCategory = this.m_dataProvider.GetTypesInCategory(this.m_settings.BugWorkItems.CategoryName);
        this.FixStateNameMismatches(requestContext, typesInCategory, (IEnumerable<State>) this.m_settings.BugWorkItems.States);
      }
      if (this.m_settings.FeedbackRequestWorkItems != null)
      {
        IEnumerable<string> typesInCategory = this.m_dataProvider.GetTypesInCategory(this.m_settings.FeedbackRequestWorkItems.CategoryName);
        this.FixStateNameMismatches(requestContext, typesInCategory, (IEnumerable<State>) this.m_settings.FeedbackRequestWorkItems.States);
      }
      if (this.m_settings.FeedbackResponseWorkItems == null)
        return;
      IEnumerable<string> typesInCategory1 = this.m_dataProvider.GetTypesInCategory(this.m_settings.FeedbackResponseWorkItems.CategoryName);
      this.FixStateNameMismatches(requestContext, typesInCategory1, (IEnumerable<State>) this.m_settings.FeedbackResponseWorkItems.States);
    }

    private void FixStateNameMismatches(
      IVssRequestContext requestContext,
      IEnumerable<string> wits,
      IEnumerable<State> states)
    {
      if (wits == null || wits.Count<string>() == 0 || states == null || states.Count<State>() == 0)
        return;
      CultureInfo serverCulture = requestContext.GetService<WebAccessWorkItemService>().GetServerCulture(requestContext);
      IEnumerable<string> source = wits.Select<string, IEnumerable<string>>((Func<string, IEnumerable<string>>) (wit => this.m_dataProvider.GetTypeStates(wit))).Aggregate<IEnumerable<string>>((Func<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>>) ((set1, set2) => set1.Union<string>(set2)));
      foreach (State state1 in states)
      {
        State state = state1;
        string str = source.FirstOrDefault<string>((Func<string, bool>) (s => serverCulture.CompareInfo.Compare(s, state.Value, CompareOptions.IgnoreCase) == 0));
        if (!string.IsNullOrEmpty(str))
          state.Value = str;
      }
    }

    private IEnumerable<string> GetWitsMissingField(IEnumerable<string> wits, string field) => this.m_parentWits.Where<string>((Func<string, bool>) (wit => !this.m_dataProvider.FieldExists(wit, field)));

    private bool FieldIsNumeric(InternalFieldType fieldType) => fieldType == InternalFieldType.Integer || fieldType == InternalFieldType.Double;

    private bool FieldIsDate(InternalFieldType fieldType) => fieldType == InternalFieldType.DateTime;
  }
}
