// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessValidator.WitProcessTemplateValidatorConfiguration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessValidator
{
  internal class WitProcessTemplateValidatorConfiguration : IWitProcessTemplateValidatorConfiguration
  {
    public const int DefaultMaxWorkItemTypesPerProcess = 64;
    public const int DefaultMaxCustomWorkItemTypesPerProcess = 64;
    public const int DefaultMaxFieldsPerCollection = 8192;
    public const int DefaultMaxFieldsPerProcessTemplate = 1024;
    public const int DefaultMaxCategoriesPerProcess = 32;
    public const int DefaultMaxGlobalListCountPerProcess = 256;
    public const int DefaultMaxGlobalListItemCountPerProcess = 1024;
    public const int DefaultMaxCustomLinkTypes = 8;
    public const int DefaultMaxStatesPerWorkItemType = 16;
    public const int DefaultMaxValuesInSingleRuleValuesList = 512;
    public const int DefaultMaxSyncNameChangesFieldsPerType = 64;
    public const int DefaultMaxFieldsInWorkItemType = 1024;
    public const int DefaultMaxCustomFieldsPerWorkItemType = 1024;
    public const int DefaultMaxRulesPerWorkItemType = 1024;
    public const int DefaultMaxPickListItemsPerList = 2048;
    public const int DefaultMaxPickListItemLength = 2048;
    public const int DefaultMaxCustomStatesPerWorkItemType = 32;
    public const int DefaultMaxPortfolioBacklogLevels = 5;
    public const int DefaultMaxPickListsPerCollection = 2048;

    public WitProcessTemplateValidatorConfiguration(
      IEqualityComparer<string> nameComparer,
      RegistryEntryCollection settings)
    {
      this.NameComparer = nameComparer;
      this.SystemLinkTypeWhiteList = (IEnumerable<LinkTypeDefinition>) new LinkTypeDefinition[4]
      {
        new LinkTypeDefinition("Microsoft.VSTS.TestCase.SharedParameterReferencedBy", "Referenced By", "References", "Dependency"),
        new LinkTypeDefinition("Microsoft.VSTS.TestCase.SharedStepReferencedBy", "Test Case", "Shared Steps", "Dependency"),
        new LinkTypeDefinition("Microsoft.VSTS.Common.TestedBy", "Tested By", "Tests", "Dependency"),
        new LinkTypeDefinition("Microsoft.VSTS.Common.Affects", "Affects", "Affected By", "Dependency")
      };
      this.SystemControlWhiteList = (IEnumerable<string>) new string[14]
      {
        "FieldControl",
        "DateTimeControl",
        "DeploymentsControl",
        "DevelopmentControl",
        "HtmlFieldControl",
        "LinksControl",
        "AttachmentsControl",
        "WorkItemClassificationControl",
        "WorkItemLogControl",
        "TestStepsControl",
        "AssociatedAutomationControl",
        "ParameterSetControl",
        "LabelControl",
        "WebpageControl"
      };
      this.BannedRules = (IEnumerable<string>) new string[4]
      {
        "MATCH",
        "CANNOTLOSEVALUE",
        "PROHIBITEDVALUES",
        "NOTSAMEAS"
      };
      this.SystemFieldWithSyncNameChanges = (IEnumerable<string>) new string[9]
      {
        "System.AssignedTo",
        "System.ChangedBy",
        "System.CreatedBy",
        "System.AuthorizedAs",
        "Microsoft.VSTS.Common.ActivatedBy",
        "Microsoft.VSTS.Common.ResolvedBy",
        "Microsoft.VSTS.Common.ClosedBy",
        "Microsoft.VSTS.CodeReview.AcceptedBy",
        "Microsoft.VSTS.Common.ReviewedBy"
      };
      this.RulesProhibitedFields = (IEnumerable<string>) new string[34]
      {
        "System.TeamProject",
        "System.NodeName",
        "System.Id",
        "System.Rev",
        "System.WorkItemType",
        "System.AreaPath",
        "System.AreaId",
        "System.IterationPath",
        "System.IterationId",
        "System.PersonId",
        "System.State",
        "System.Reason",
        "System.CreatedBy",
        "System.CreatedDate",
        "System.InProgressBy",
        "System.InProgressDate",
        "System.CompletedBy",
        "System.CompletedDate",
        "System.StateChangeDate",
        "System.History",
        "System.ChangedDate",
        "System.AuthorizedAs",
        "System.AuthorizedDate",
        "System.RevisedDate",
        "System.Watermark",
        "System.Priority",
        "System.IntegrationBuild",
        "System.AttachedFileCount",
        "System.ExternalLinkCount",
        "System.HyperlinkCount",
        "System.RelatedLinkCount",
        "System.CommentCount",
        "System.RemoteLinkCount",
        "System.Parent"
      };
      this.FieldRuleRestrictions = (IEnumerable<FieldRuleRestriction>) new FieldRuleRestriction[3]
      {
        new FieldRuleRestriction("System.Title", (IEnumerable<string>) new string[2]
        {
          "REQUIRED",
          "DEFAULT"
        }),
        new FieldRuleRestriction("System.AssignedTo", (IEnumerable<string>) new string[5]
        {
          "REQUIRED",
          "DEFAULT",
          "ALLOWEXISTINGVALUE",
          "VALIDUSER",
          "ALLOWEDVALUES"
        }),
        new FieldRuleRestriction("System.ChangedBy", (IEnumerable<string>) new string[2]
        {
          "ALLOWEXISTINGVALUE",
          "VALIDUSER"
        })
      };
      this.FeatureRequirements = (IEnumerable<FeatureRequirement>) new FeatureRequirement[13]
      {
        new FeatureRequirement("PortfolioBacklog", FeatureMetastateRequirement.Standard, (IEnumerable<string>) new string[1]
        {
          "Order"
        }),
        new FeatureRequirement("RequirementBacklog", FeatureMetastateRequirement.Standard, (IEnumerable<string>) new string[2]
        {
          "Order",
          "Effort"
        }),
        new FeatureRequirement("TaskBacklog", FeatureMetastateRequirement.Standard, (IEnumerable<string>) new string[2]
        {
          "RemainingWork",
          "Activity"
        }),
        new FeatureRequirement("FeedbackRequestWorkItems", FeatureMetastateRequirement.ProposedOptional, (IEnumerable<string>) new string[3]
        {
          "ApplicationType",
          "ApplicationLaunchInstructions",
          "ApplicationStartInformation"
        }),
        new FeatureRequirement("FeedbackResponseWorkItems", FeatureMetastateRequirement.ProposedOptional),
        new FeatureRequirement("BugWorkItems", FeatureMetastateRequirement.ResolvedAllowed, fields: (IEnumerable<string>) new string[1]
        {
          "Microsoft.VSTS.TCM.ReproSteps"
        }),
        new FeatureRequirement("TestPlanWorkItems", FeatureMetastateRequirement.ProposedOptional),
        new FeatureRequirement("TestSuiteWorkItems", FeatureMetastateRequirement.Standard, fields: (IEnumerable<string>) new string[4]
        {
          "Microsoft.VSTS.TCM.TestSuiteTypeId",
          "Microsoft.VSTS.TCM.TestSuiteType",
          "Microsoft.VSTS.TCM.QueryText",
          "Microsoft.VSTS.TCM.TestSuiteAudit"
        }),
        new FeatureRequirement("Microsoft.TestCaseCategory", fields: (IEnumerable<string>) new string[8]
        {
          "Microsoft.VSTS.TCM.Steps",
          "Microsoft.VSTS.TCM.AutomatedTestName",
          "Microsoft.VSTS.TCM.AutomatedTestStorage",
          "Microsoft.VSTS.TCM.AutomatedTestId",
          "Microsoft.VSTS.TCM.AutomatedTestType",
          "Microsoft.VSTS.TCM.AutomationStatus",
          "Microsoft.VSTS.TCM.Parameters",
          "Microsoft.VSTS.TCM.LocalDataSource"
        }, hardcodedCategory: "Microsoft.TestCaseCategory"),
        new FeatureRequirement("Microsoft.SharedStepsCategory", fields: (IEnumerable<string>) new string[2]
        {
          "Microsoft.VSTS.TCM.Steps",
          "Microsoft.VSTS.TCM.Parameters"
        }, hardcodedCategory: "Microsoft.SharedStepCategory"),
        new FeatureRequirement("Microsoft.SharedParameterCategory", fields: (IEnumerable<string>) new string[1]
        {
          "Microsoft.VSTS.TCM.Parameters"
        }, hardcodedCategory: "Microsoft.SharedParameterCategory"),
        new FeatureRequirement("Microsoft.CodeReviewRequestCategory", FeatureMetastateRequirement.ProposedOptional, hardcodedCategory: "Microsoft.CodeReviewRequestCategory", hardcodedMetastateMap: (IDictionary<string, string>) new Dictionary<string, string>()
        {
          {
            "InProgress",
            "Requested"
          },
          {
            "Complete",
            "Closed"
          }
        }),
        new FeatureRequirement("Microsoft.CodeReviewResponseCategory", FeatureMetastateRequirement.Standard, hardcodedCategory: "Microsoft.CodeReviewResponseCategory", hardcodedMetastateMap: (IDictionary<string, string>) new Dictionary<string, string>()
        {
          {
            "Proposed",
            "Requested"
          },
          {
            "InProgress",
            "Accepted"
          },
          {
            "Complete",
            "Closed"
          }
        })
      };
      this.DefaultSystemFields = (IEnumerable<ProcessFieldDefinition>) new ProcessFieldDefinition[10]
      {
        new ProcessFieldDefinition()
        {
          Name = "Authorized Date",
          ReferenceName = "System.AuthorizedDate",
          Type = InternalFieldType.DateTime
        },
        new ProcessFieldDefinition()
        {
          Name = "Watermark",
          ReferenceName = "System.Watermark",
          Type = InternalFieldType.Integer
        },
        new ProcessFieldDefinition()
        {
          Name = "Tags",
          ReferenceName = "System.Tags",
          Type = InternalFieldType.PlainText
        },
        new ProcessFieldDefinition()
        {
          Name = "Board Column",
          ReferenceName = "System.BoardColumn",
          Type = InternalFieldType.String
        },
        new ProcessFieldDefinition()
        {
          Name = "Board Column Done",
          ReferenceName = "System.BoardColumnDone",
          Type = InternalFieldType.Boolean
        },
        new ProcessFieldDefinition()
        {
          Name = "Board Lane",
          ReferenceName = "System.BoardLane",
          Type = InternalFieldType.String
        },
        new ProcessFieldDefinition()
        {
          Name = "Authorized As",
          ReferenceName = "System.AuthorizedAs",
          Type = InternalFieldType.String,
          SyncNameChanges = true
        },
        new ProcessFieldDefinition()
        {
          Name = "Comment Count",
          ReferenceName = "System.CommentCount",
          Type = InternalFieldType.Integer
        },
        new ProcessFieldDefinition()
        {
          Name = "Remote Link Count",
          ReferenceName = "System.RemoteLinkCount",
          Type = InternalFieldType.Integer
        },
        new ProcessFieldDefinition()
        {
          Name = "Parent",
          ReferenceName = "System.Parent",
          Type = InternalFieldType.Integer
        }
      };
      this.MaxWorkItemTypesPerProcess = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Validator/WorkItemTypesPerProcess", 64);
      this.MaxCustomWorkItemTypesPerProcess = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Validator/MaxCustomWorkItemTypesPerProcess", 64);
      this.MaxFieldsPerCollection = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Validator/MaxFieldsPerCollection", 8192);
      this.MaxFieldsPerProcessTemplate = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Validator/MaxFieldsPerProcessTemplate", 1024);
      this.MaxCategoriesPerProcess = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Validator/CategoriesPerProcess", 32);
      this.MaxGlobalListCountPerProcess = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Validator/GlobalListCountPerProcess", 256);
      this.MaxGlobalListItemCountPerProcess = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Validator/GlobaListItemCountPerProcess", 1024);
      this.MaxCustomLinkTypes = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Validator/CustomLinkTypes", 8);
      this.MaxStatesPerWorkItemType = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Validator/StatesPerWorkItemType", 16);
      this.MaxValuesInSingleRuleValuesList = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Validator/ValuesInSingleRuleValuesList", 512);
      this.MaxSyncNameChangesFieldsPerType = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Validator/SyncNameChangesFieldsPerType", 64);
      this.MaxFieldsInWorkItemType = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Validator/FieldsInWorkItemType", 1024);
      this.MaxCustomFieldsPerWorkItemType = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Validator/MaxCustomFieldsPerWorkItemType", 1024);
      this.MaxRulesPerWorkItemType = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Validator/RulesPerWorkItemType", 1024);
      this.MaxPickListItemsPerList = Math.Max(settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Validator/MaxPickListItemsPerList", 2048), this.MaxValuesInSingleRuleValuesList);
      this.MaxPickListItemLength = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Validator/MaxPickListItemLength", 2048);
      this.MaxCustomStatesPerWorkItemType = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Validator/MaxCustomStatesPerWorkItemType", 32);
      this.MaxPortfolioBacklogLevels = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Validator/MaxPortfolioBacklogLevels", 5);
      this.MaxPickListsPerCollection = settings.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/Validator/MaxPickListsPerCollection", 2048);
      this.GlobalListsPermitted = settings.GetValueFromPath<bool>("/Service/WorkItemTracking/Settings/Validator/GlobalListsPermitted", false);
      this.CustomLinkTypesPermitted = settings.GetValueFromPath<bool>("/Service/WorkItemTracking/Settings/Validator/CustomLinkTypesPermitted", false);
      this.AllRulesPermitted = settings.GetValueFromPath<bool>("/Service/WorkItemTracking/Settings/Validator/AllRulesPermitted", false);
      this.NoFieldRuleRestrictions = settings.GetValueFromPath<bool>("/Service/WorkItemTracking/Settings/Validator/NoFieldRuleRestrictions", false);
      this.CustomControlsPermitted = settings.GetValueFromPath<bool>("/Service/WorkItemTracking/Settings/Validator/CustomControlsPermitted", false);
      this.IdentitiesWithoutSyncNameChangesPermitted = settings.GetValueFromPath<bool>("/Service/WorkItemTracking/Settings/Validator/IdentitiesWithoutSyncNameChangesPermitted", false);
    }

    public IEnumerable<LinkTypeDefinition> SystemLinkTypeWhiteList { get; private set; }

    public IEnumerable<string> SystemControlWhiteList { get; private set; }

    public IEnumerable<string> BannedRules { get; private set; }

    public IEnumerable<string> RulesProhibitedFields { get; private set; }

    public IEnumerable<FieldRuleRestriction> FieldRuleRestrictions { get; private set; }

    public IEnumerable<FeatureRequirement> FeatureRequirements { get; private set; }

    public IEqualityComparer<string> NameComparer { get; private set; }

    public IEnumerable<ProcessFieldDefinition> DefaultSystemFields { get; private set; }

    public int MaxWorkItemTypesPerProcess { get; private set; }

    public int MaxCustomWorkItemTypesPerProcess { get; private set; }

    public int MaxCategoriesPerProcess { get; private set; }

    public int MaxFieldsPerCollection { get; }

    public int MaxFieldsPerProcessTemplate { get; }

    public int MaxGlobalListCountPerProcess { get; private set; }

    public int MaxGlobalListItemCountPerProcess { get; private set; }

    public int MaxCustomLinkTypes { get; private set; }

    public int MaxStatesPerWorkItemType { get; private set; }

    public int MaxValuesInSingleRuleValuesList { get; private set; }

    public int MaxSyncNameChangesFieldsPerType { get; private set; }

    public int MaxFieldsInWorkItemType { get; private set; }

    public int MaxCustomFieldsPerWorkItemType { get; private set; }

    public int MaxRulesPerWorkItemType { get; private set; }

    public int MaxPickListItemsPerList { get; private set; }

    public int MaxPickListItemLength { get; private set; }

    public int MaxCustomStatesPerWorkItemType { get; private set; }

    public int MaxPortfolioBacklogLevels { get; private set; }

    public int MaxPickListsPerCollection { get; private set; }

    public bool GlobalListsPermitted { get; private set; }

    public bool CustomLinkTypesPermitted { get; private set; }

    public bool AllRulesPermitted { get; private set; }

    public bool NoFieldRuleRestrictions { get; private set; }

    public IEnumerable<string> SystemFieldWithSyncNameChanges { get; private set; }

    public bool CustomControlsPermitted { get; private set; }

    public bool IdentitiesWithoutSyncNameChangesPermitted { get; private set; }
  }
}
