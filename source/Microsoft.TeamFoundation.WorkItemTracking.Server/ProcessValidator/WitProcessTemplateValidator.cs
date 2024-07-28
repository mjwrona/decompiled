// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessValidator.WitProcessTemplateValidator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessValidator
{
  public class WitProcessTemplateValidator
  {
    private readonly IWitProcessTemplateValidatorConfiguration configuration;
    private readonly IEnumerable<WitProcessTemplateMetadata> existingTemplates;
    private readonly ISet<ProcessFieldDefinition> collectionFields;
    private readonly ISet<ProcessFieldDefinition> systemFields;
    private readonly IEnumerable<ProcessFieldDefinition> fieldsFromTableNotInTemplates;
    private readonly IEqualityComparer<ProcessFieldDefinition> fieldDefinitionComparer;
    private readonly IEqualityComparer<ProcessFieldDefinition> refNameOnlyFieldDefinitionComparer;
    private readonly IDictionary<string, bool> featureFlags;
    private static Dictionary<FeatureMetastateRequirement, IEnumerable<string>> legalMetastates = new Dictionary<FeatureMetastateRequirement, IEnumerable<string>>()
    {
      {
        FeatureMetastateRequirement.None,
        Enumerable.Empty<string>()
      },
      {
        FeatureMetastateRequirement.Standard,
        (IEnumerable<string>) new string[3]
        {
          "Proposed",
          "InProgress",
          "Complete"
        }
      },
      {
        FeatureMetastateRequirement.ProposedOptional,
        (IEnumerable<string>) new string[3]
        {
          "Proposed",
          "InProgress",
          "Complete"
        }
      },
      {
        FeatureMetastateRequirement.ResolvedAllowed,
        (IEnumerable<string>) new string[5]
        {
          "Proposed",
          "InProgress",
          "Resolved",
          "Complete",
          "Removed"
        }
      }
    };
    private static HashSet<string> leafNodeRules = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal)
    {
      "ALLOWEDVALUES",
      "ALLOWEXISTINGVALUE",
      "SUGGESTEDVALUES",
      "COPYDEFAULT",
      "EMPTY",
      "FROZEN",
      "READONLY",
      "REQUIRED",
      "SERVERDEFAULT",
      "VALIDUSER"
    };

    public WitProcessTemplateValidator(
      IWitProcessTemplateValidatorConfiguration configuration,
      IEnumerable<WitProcessTemplateMetadata> existingTemplates,
      ISet<ProcessFieldDefinition> collectionFields,
      IDictionary<string, bool> featureFlags = null)
    {
      this.configuration = configuration;
      this.existingTemplates = existingTemplates;
      this.collectionFields = collectionFields;
      this.featureFlags = featureFlags;
      this.fieldDefinitionComparer = (IEqualityComparer<ProcessFieldDefinition>) new FieldDefinitionComparer(configuration.NameComparer);
      this.refNameOnlyFieldDefinitionComparer = (IEqualityComparer<ProcessFieldDefinition>) new FieldDefinitionComparer(configuration.NameComparer, true);
      this.systemFields = (ISet<ProcessFieldDefinition>) existingTemplates.Where<WitProcessTemplateMetadata>((Func<WitProcessTemplateMetadata, bool>) (t => t.IsSystemTemplate)).Aggregate<WitProcessTemplateMetadata, HashSet<ProcessFieldDefinition>>(new HashSet<ProcessFieldDefinition>(this.fieldDefinitionComparer), (Func<HashSet<ProcessFieldDefinition>, WitProcessTemplateMetadata, HashSet<ProcessFieldDefinition>>) ((s, t) =>
      {
        s.UnionWith(t.DefinedFields);
        return s;
      }));
      this.systemFields.UnionWith(configuration.DefaultSystemFields);
      this.fieldsFromTableNotInTemplates = (IEnumerable<ProcessFieldDefinition>) collectionFields.Except<ProcessFieldDefinition>(existingTemplates.SelectMany<WitProcessTemplateMetadata, ProcessFieldDefinition>((Func<WitProcessTemplateMetadata, IEnumerable<ProcessFieldDefinition>>) (t => t.DefinedFields)), this.fieldDefinitionComparer).ToList<ProcessFieldDefinition>();
    }

    public ProcessTemplateValidatorResult ValidatePackage(
      IVssRequestContext requestContext,
      IProcessTemplatePackage package,
      Guid updatedTemplateVersionId,
      bool isTfsMigratorValidationContext = false)
    {
      return this.ValidatePackage(requestContext, package, updatedTemplateVersionId, out WitProcessTemplateValidator.LoadedTemplateState _, isTfsMigratorValidationContext);
    }

    public ProcessTemplateValidatorResult ValidatePackage(
      IVssRequestContext requestContext,
      IProcessTemplatePackage package,
      Guid updatedTemplateVersionId,
      out WitProcessTemplateValidator.LoadedTemplateState template,
      bool isTfsMigratorValidationContext = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IProcessTemplatePackage>(package, nameof (package));
      WitProcessTemplateMetadata previousTemplateVersion = this.existingTemplates.SingleOrDefault<WitProcessTemplateMetadata>((Func<WitProcessTemplateMetadata, bool>) (t => t.Id == updatedTemplateVersionId));
      List<ProcessTemplateValidatorMessage> validatorMessageList = new List<ProcessTemplateValidatorMessage>();
      WitProcessTemplateValidator.LoadedTemplateState loadedTemplate = this.LoadTemplate(package, validatorMessageList, false);
      template = loadedTemplate;
      if (validatorMessageList.Any<ProcessTemplateValidatorMessage>())
        return new ProcessTemplateValidatorResult((IEnumerable<ProcessTemplateValidatorMessage>) validatorMessageList);
      this.ValidateProcessName(requestContext, package.Name, validatorMessageList);
      if (validatorMessageList.Any<ProcessTemplateValidatorMessage>())
        return new ProcessTemplateValidatorResult((IEnumerable<ProcessTemplateValidatorMessage>) validatorMessageList);
      IEnumerable<ProcessTemplateValidatorMessage> collection1 = WitProcessTemplateValidator.ValidateProcessConfiguration(loadedTemplate.ProcessConfiguration);
      validatorMessageList.AddRange(collection1);
      if (validatorMessageList.Any<ProcessTemplateValidatorMessage>())
        return new ProcessTemplateValidatorResult((IEnumerable<ProcessTemplateValidatorMessage>) validatorMessageList);
      validatorMessageList.AddRange(this.ValidateAllTypesHaveRefName(loadedTemplate.TypeDefinitions));
      if (validatorMessageList.Any<ProcessTemplateValidatorMessage>())
        return new ProcessTemplateValidatorResult((IEnumerable<ProcessTemplateValidatorMessage>) validatorMessageList);
      IEnumerable<ProcessTemplateValidatorMessage> collection2 = WitProcessTemplateValidator.RunValidation((Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => (IEnumerable<ProcessTemplateValidatorMessage>) this.ValidateFieldDefinitionInternalConsistency(loadedTemplate.TypeDefinitions)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidatePortfolioBacklogs(loadedTemplate.FeatureElements, loadedTemplate.CategoryElements, loadedTemplate.ProcessConfiguration.FileName)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateAllTypesInCategoriesAreDefined(loadedTemplate.CategoryElements, loadedTemplate.CategoriesDocument.FileName, loadedTemplate.TypeDefinitions)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateAllFeaturesAreMappedToValidCategories(loadedTemplate.CategoryElements, loadedTemplate.FeatureElements, loadedTemplate.ProcessConfiguration, loadedTemplate.CategoriesDocument.FileName)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateLinkTypeDefinitionConsistency(loadedTemplate.LinkTypeDocuments)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateNoGlobalListsUsed(loadedTemplate.TypeDefinitions, isTfsMigratorValidationContext)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateNoCustomFormControls(loadedTemplate.TypeDefinitions)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateNoBannedRules(loadedTemplate.TypeDefinitions)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateNoFieldsInProtectedNamespace(loadedTemplate.TypeDefinitions)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateNoTypesInProtectedNamespace(loadedTemplate.TypeDefinitions)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateProperDefinitionOfProtectedFields(loadedTemplate.TypeDefinitions)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateConsistentFieldDefinitionsWithOtherTemplates(loadedTemplate.TypeDefinitions)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateConsistentSyncNameChangeUsageOnSystemFields(loadedTemplate.TypeDefinitions)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateUniqueWorkItemTypeNames(loadedTemplate.TypeDefinitions)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateUniqueDisplayNameForFields(requestContext, loadedTemplate.TypeDefinitions)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateNoRulesOnFieldsThatProhibitRules(loadedTemplate.TypeDefinitions)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateOnlyValidRulesOnFieldsThatLimitRules(loadedTemplate.TypeDefinitions)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateFeatureTypeFieldsInTypes(loadedTemplate.TypeDefinitions, loadedTemplate.FeatureElements, loadedTemplate.CategoryElements, loadedTemplate.ProcessConfiguration)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateTeamField(requestContext, loadedTemplate.ProcessConfiguration)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateFeatureFieldsInTypes(loadedTemplate.TypeDefinitions, loadedTemplate.FeatureElements, loadedTemplate.CategoryElements)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateFeatureMetastateMappingsInTypes(loadedTemplate.TypeDefinitions, loadedTemplate.FeatureElements, loadedTemplate.CategoryElements, loadedTemplate.ProcessConfiguration)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateTypeCountLimit(loadedTemplate.TypeDefinitions)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateTotalFieldCountLimit(loadedTemplate.TypeDefinitions)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateCategoryCountLimit(loadedTemplate.CategoryElements)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateGlobalListCountLimit(loadedTemplate.TypeDefinitions, isTfsMigratorValidationContext)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateGlobalListItemCountLimit(loadedTemplate.TypeDefinitions, isTfsMigratorValidationContext)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateCustomLinkTypeCountLimit(loadedTemplate.LinkTypeDocuments)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateStatesOnTypeCountLimit(loadedTemplate.TypeDefinitions)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateListItemsInRulesCountLimit(loadedTemplate.TypeDefinitions)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateSyncNameChangesCountLimit(loadedTemplate.TypeDefinitions)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateFieldCountLimit(loadedTemplate.TypeDefinitions)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateRuleOnTypeCountLimit(loadedTemplate.TypeDefinitions)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ScanForInvalidIdentityFields(loadedTemplate.TypeDefinitions)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateLocalization(loadedTemplate.TypeDefinitions, isTfsMigratorValidationContext)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateDeletedWits(requestContext, loadedTemplate.TypeDefinitions, previousTemplateVersion, isTfsMigratorValidationContext)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateRenamedWits(requestContext, loadedTemplate.TypeDefinitions, previousTemplateVersion, isTfsMigratorValidationContext)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateNarrowBreakSpaceForConstants(loadedTemplate.TypeDefinitions)));
      validatorMessageList.AddRange(collection2);
      List<ProcessTemplateFieldRenameData> fieldRenames = new List<ProcessTemplateFieldRenameData>();
      List<ProcessTemplateValidatorMessage> confirmationsNeeded = new List<ProcessTemplateValidatorMessage>();
      if (!validatorMessageList.Any<ProcessTemplateValidatorMessage>())
      {
        confirmationsNeeded.AddRange(WitProcessTemplateValidator.RunValidation((Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ScanForDeletedWorkItemTypes(loadedTemplate.TypeDefinitions, previousTemplateVersion)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ScanForRenamedWorkItemTypes(loadedTemplate.TypeDefinitions, previousTemplateVersion)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ScanForDeletedFields(loadedTemplate.TypeDefinitions, previousTemplateVersion)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ScanForRequiredFieldsOnLayout(loadedTemplate.TypeDefinitions)), (Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ValidateExistingAllowedValues(requestContext, loadedTemplate.TypeDefinitions, loadedTemplate.ProcessConfiguration, previousTemplateVersion))));
        IEnumerable<ProcessTemplateValidatorMessage> collection3 = WitProcessTemplateValidator.RunValidation((Func<IEnumerable<ProcessTemplateValidatorMessage>>) (() => this.ScanForFieldRenamesAcrossTemplates(loadedTemplate.TypeDefinitions, fieldRenames)));
        validatorMessageList.AddRange(collection3);
      }
      return new ProcessTemplateValidatorResult((IEnumerable<ProcessTemplateValidatorMessage>) validatorMessageList, (IEnumerable<ProcessTemplateValidatorMessage>) confirmationsNeeded, (IEnumerable<object>) fieldRenames);
    }

    private void ValidateProcessName(
      IVssRequestContext requestContext,
      string name,
      List<ProcessTemplateValidatorMessage> errors)
    {
      try
      {
        requestContext.GetService<ITeamFoundationProcessService>().CheckValidProcessName(name);
      }
      catch (ProcessServiceException ex)
      {
        errors.Add(new ProcessTemplateValidatorMessage(ex.Message, "ProcessTemplate.xml"));
      }
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateTeamField(
      IVssRequestContext requestContext,
      WitProcessTemplateValidator.LoadedFile processConfiguration)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      IEnumerable<ProcessTemplateValidatorMessage> validatorMessages = Enumerable.Empty<ProcessTemplateValidatorMessage>();
      XElement xelement = processConfiguration.Document.Root.Element((XName) "TypeFields");
      bool flag1 = requestContext.IsFeatureEnabled("Agile.ProcessSettingsValidator.AllowCustomTeamField");
      bool flag2;
      if (requestContext.Items != null && requestContext.Items.TryGetValue<bool>("Agile.ProcessSettingsValidator.AllowCustomTeamField", out flag2))
        flag1 |= flag2;
      return xelement == null | flag1 ? validatorMessages : (IEnumerable<ProcessTemplateValidatorMessage>) xelement.Elements().Where<XElement>((Func<XElement, bool>) (e => string.Equals(e.Name.LocalName, "TypeField", StringComparison.Ordinal))).Where<XElement>((Func<XElement, bool>) (e => e.Attribute((XName) "type").Value == "Team" && e.Attribute((XName) "refname").Value != "System.AreaPath")).Select<XElement, ProcessTemplateValidatorMessage>((Func<XElement, ProcessTemplateValidatorMessage>) (e =>
      {
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.InvalidTeamFieldValue((object) e.Attribute((XName) "type").Value, (object) "System.AreaPath");
        string fileName = processConfiguration.FileName;
        string invalidTeamField = WitProcessTemplateValidatorHelpLinks.ValidatorErrorInvalidTeamField;
        int? lineNumber = new int?();
        string helpLink = invalidTeamField;
        return new ProcessTemplateValidatorMessage(message, fileName, lineNumber, helpLink);
      })).ToList<ProcessTemplateValidatorMessage>();
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateLocalization(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions,
      bool isTfsMigratorValidationContext)
    {
      return isTfsMigratorValidationContext ? typeDefinitions.SelectMany(f => f.Document.AllFieldsInWIT().Select(e => new
      {
        TemplateField = new ProcessFieldDefinition()
        {
          Name = e.Attribute((XName) "name").Value,
          ReferenceName = e.Attribute((XName) "refname").Value,
          Type = this.ParseFieldType(e.Attribute((XName) "type").Value)
        },
        FileName = f.FileName,
        LineNumber = WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) e)
      }).Where(t => t.TemplateField.ReferenceName.Equals("System.IterationPath") && !t.TemplateField.Name.Equals("Iteration Path"))).Select(t => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorImproperDefinitionOfProtectedFieldOrFieldInDifferentLocale((object) t.TemplateField.ReferenceName, (object) "Iteration Path", (object) "TreePath", (object) t.TemplateField.Name, (object) t.TemplateField.Type), t.FileName, new int?(t.LineNumber), WitProcessTemplateValidatorHelpLinks.ValidatorImproperDefinitionOfProtectedField)) : Enumerable.Empty<ProcessTemplateValidatorMessage>();
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateDeletedWits(
      IVssRequestContext requestContext,
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions,
      WitProcessTemplateMetadata previousTemplateVersion,
      bool isTfsMigratorValidationContext)
    {
      if (((previousTemplateVersion == null || requestContext == null ? 1 : (!requestContext.ExecutionEnvironment.IsHostedDeployment ? 1 : 0)) | (isTfsMigratorValidationContext ? 1 : 0)) != 0)
        return Enumerable.Empty<ProcessTemplateValidatorMessage>();
      HashSet<string> definedTypes = new HashSet<string>(typeDefinitions.Select<WitProcessTemplateValidator.LoadedFile, string>((Func<WitProcessTemplateValidator.LoadedFile, string>) (f => f.Document.GetWITRefName())), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      IEnumerable<ProcessWorkItemTypeDefinition> itemTypeDefinitions = previousTemplateVersion.DefinedWorkItemTypes.Where<ProcessWorkItemTypeDefinition>((Func<ProcessWorkItemTypeDefinition, bool>) (t => !definedTypes.Contains(t.ReferenceName)));
      List<ProcessTemplateValidatorMessage> validatorMessageList = new List<ProcessTemplateValidatorMessage>();
      IWorkItemTypeService service = requestContext.GetService<IWorkItemTypeService>();
      foreach (ProcessWorkItemTypeDefinition itemTypeDefinition in itemTypeDefinitions)
      {
        if (service.HasAnyWorkItemsOfTypeForProcess(requestContext, previousTemplateVersion.Id, itemTypeDefinition.Name))
          validatorMessageList.Add(new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorWorkItemTypeDeleteWhileExistingWorkItems((object) itemTypeDefinition.Name), (string) null));
      }
      return (IEnumerable<ProcessTemplateValidatorMessage>) validatorMessageList;
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateRenamedWits(
      IVssRequestContext requestContext,
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions,
      WitProcessTemplateMetadata previousTemplateVersion,
      bool isTfsMigratorValidationContext)
    {
      if (isTfsMigratorValidationContext || requestContext == null || typeDefinitions == null || previousTemplateVersion == null || !requestContext.ExecutionEnvironment.IsHostedDeployment)
        return Enumerable.Empty<ProcessTemplateValidatorMessage>();
      Dictionary<string, string> dictionary1 = previousTemplateVersion.DefinedWorkItemTypes.ToDictionary<ProcessWorkItemTypeDefinition, string, string>((Func<ProcessWorkItemTypeDefinition, string>) (t => t.ReferenceName), (Func<ProcessWorkItemTypeDefinition, string>) (t => t.Name), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, string> dictionary2 = typeDefinitions.ToDictionary<WitProcessTemplateValidator.LoadedFile, string, string>((Func<WitProcessTemplateValidator.LoadedFile, string>) (t => t.Document.GetWITRefName()), (Func<WitProcessTemplateValidator.LoadedFile, string>) (t => t.Document.GetWITName()), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      IWorkItemTypeService service = requestContext.GetService<IWorkItemTypeService>();
      List<ProcessTemplateValidatorMessage> validatorMessageList = new List<ProcessTemplateValidatorMessage>();
      foreach (KeyValuePair<string, string> keyValuePair in dictionary1)
      {
        string str = keyValuePair.Value;
        string y;
        if (dictionary2.TryGetValue(keyValuePair.Key, out y) && !this.configuration.NameComparer.Equals(str, y) && service.HasAnyWorkItemsOfTypeForProcess(requestContext, previousTemplateVersion.Id, str))
          validatorMessageList.Add(new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorWorkItemTypeRenameWhileExistingWorkItems((object) str, (object) y), (string) null));
      }
      return (IEnumerable<ProcessTemplateValidatorMessage>) validatorMessageList;
    }

    public ProcessTemplateValidatorResult ValidatePackageOnPrem(
      IProcessTemplatePackage package,
      out WitProcessTemplateValidator.LoadedTemplateState template,
      bool IsOverridingExistingTemplate)
    {
      List<ProcessTemplateValidatorMessage> errors = new List<ProcessTemplateValidatorMessage>();
      WitProcessTemplateValidator.LoadedTemplateState loadedTemplateState = this.LoadTemplate(package, errors, IsOverridingExistingTemplate);
      template = loadedTemplateState;
      return new ProcessTemplateValidatorResult((IEnumerable<ProcessTemplateValidatorMessage>) errors);
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateConsistentSyncNameChangeUsageOnSystemFields(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions)
    {
      return typeDefinitions.SelectMany<WitProcessTemplateValidator.LoadedFile, ProcessTemplateValidatorMessage>((Func<WitProcessTemplateValidator.LoadedFile, IEnumerable<ProcessTemplateValidatorMessage>>) (f => f.Document.AllFieldsInWIT().Where<XElement>((Func<XElement, bool>) (e => e.Attribute((XName) "refname").Value.StartsWith("System.", StringComparison.OrdinalIgnoreCase))).Where<XElement>((Func<XElement, bool>) (e =>
      {
        if (this.configuration.SystemFieldWithSyncNameChanges.Contains<string>(e.Attribute((XName) "refname").Value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          return e.Attribute((XName) "syncnamechanges") == null;
        return e.Attribute((XName) "syncnamechanges") != null && e.Attribute((XName) "syncnamechanges").Value.Equals("true", StringComparison.OrdinalIgnoreCase);
      })).Select<XElement, ProcessTemplateValidatorMessage>((Func<XElement, ProcessTemplateValidatorMessage>) (e => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorInconsistentFieldDefinitionWithExistingTemplate((object) e.Attribute((XName) "refname").Value, (object) string.Join(", ", this.existingTemplates.Where<WitProcessTemplateMetadata>((Func<WitProcessTemplateMetadata, bool>) (t => t.IsSystemTemplate)).Select<WitProcessTemplateMetadata, string>((Func<WitProcessTemplateMetadata, string>) (t => t.Name)))), f.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) e)), WitProcessTemplateValidatorHelpLinks.ValidatorInconsistentFieldDefinitionWithExistingTemplate)))));
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateNoTypesInProtectedNamespace(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions)
    {
      return typeDefinitions.Where<WitProcessTemplateValidator.LoadedFile>((Func<WitProcessTemplateValidator.LoadedFile, bool>) (f => WitProcessTemplateValidator.WorkItemTypeReferenceName(f.Document) != null)).Where<WitProcessTemplateValidator.LoadedFile>((Func<WitProcessTemplateValidator.LoadedFile, bool>) (f => WitProcessTemplateValidator.WorkItemTypeReferenceName(f.Document).StartsWith("System.", StringComparison.OrdinalIgnoreCase))).Select<WitProcessTemplateValidator.LoadedFile, ProcessTemplateValidatorMessage>((Func<WitProcessTemplateValidator.LoadedFile, ProcessTemplateValidatorMessage>) (f => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorWorkItemTypeRefNameInProtectedNamespace((object) WitProcessTemplateValidator.WorkItemTypeReferenceName(f.Document), (object) "System"), f.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) f.Document.Root.Element((XName) "WORKITEMTYPE").Attribute((XName) "refname"))), WitProcessTemplateValidatorHelpLinks.ValidatorWorkItemTypeRefNameInProtectedNamespace)));
    }

    private static string WorkItemTypeReferenceName(XDocument typeDocument) => typeDocument.Root?.Element((XName) "WORKITEMTYPE")?.Attribute((XName) "refname")?.Value;

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateRuleOnTypeCountLimit(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions)
    {
      return typeDefinitions.Select(f => new
      {
        TypeDocument = f,
        LeafNodesCount = f.Document.Descendants().Where<XElement>((Func<XElement, bool>) (e => WitProcessTemplateValidator.leafNodeRules.Contains(e.Name.LocalName))).Count<XElement>()
      }).Where(g => g.LeafNodesCount > this.configuration.MaxRulesPerWorkItemType).Select(g =>
      {
        WitProcessTemplateValidator.LoadedFile typeDocument = g.TypeDocument;
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorLimitsTooManyRulesOnWIT((object) typeDocument.Document.GetWITName(), (object) g.LeafNodesCount, (object) this.configuration.MaxRulesPerWorkItemType);
        typeDocument = g.TypeDocument;
        string fileName = typeDocument.FileName;
        string tooManyRulesOnWit = WitProcessTemplateValidatorHelpLinks.ValidatorLimitsTooManyRulesOnWIT;
        int? lineNumber = new int?();
        string helpLink = tooManyRulesOnWit;
        return new ProcessTemplateValidatorMessage(message, fileName, lineNumber, helpLink);
      });
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateFieldCountLimit(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions)
    {
      return typeDefinitions.Select(f => new
      {
        Type = f,
        FieldCount = f.Document.AllFieldsInWIT().Count<XElement>()
      }).Where(g => g.FieldCount > this.configuration.MaxFieldsInWorkItemType).Select(g =>
      {
        WitProcessTemplateValidator.LoadedFile type = g.Type;
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorLimitsTooManyFieldsInWIT((object) type.Document.GetWITName(), (object) g.FieldCount, (object) this.configuration.MaxFieldsInWorkItemType);
        type = g.Type;
        string fileName = type.FileName;
        string tooManyFieldsInWit = WitProcessTemplateValidatorHelpLinks.ValidatorLimitsTooManyFieldsInWIT;
        int? lineNumber = new int?();
        string helpLink = tooManyFieldsInWit;
        return new ProcessTemplateValidatorMessage(message, fileName, lineNumber, helpLink);
      });
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateSyncNameChangesCountLimit(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions)
    {
      return typeDefinitions.Select(f => new
      {
        Type = f,
        SyncNameChangesCount = f.Document.AllFieldsInWIT().Where<XElement>((Func<XElement, bool>) (e => e.Attribute((XName) "syncnamechanges") != null && e.Attribute((XName) "syncnamechanges").Value.Equals("true", StringComparison.OrdinalIgnoreCase))).Count<XElement>()
      }).Where(g => g.SyncNameChangesCount > this.configuration.MaxSyncNameChangesFieldsPerType).Select(g =>
      {
        WitProcessTemplateValidator.LoadedFile type = g.Type;
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorLimitsTooManySyncNameChangesFieldsInType((object) type.Document.GetWITName(), (object) g.SyncNameChangesCount, (object) this.configuration.MaxSyncNameChangesFieldsPerType);
        type = g.Type;
        string fileName = type.FileName;
        string changesFieldsInType = WitProcessTemplateValidatorHelpLinks.ValidatorLimitsTooManySyncNameChangesFieldsInType;
        int? lineNumber = new int?();
        string helpLink = changesFieldsInType;
        return new ProcessTemplateValidatorMessage(message, fileName, lineNumber, helpLink);
      });
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateListItemsInRulesCountLimit(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions)
    {
      return typeDefinitions.SelectMany(f => f.Document.AllFieldsInWIT().Where<XElement>((Func<XElement, bool>) (e => e.Element((XName) "ALLOWEDVALUES") != null && e.Element((XName) "ALLOWEDVALUES").Elements((XName) "LISTITEM").Count<XElement>() > this.configuration.MaxValuesInSingleRuleValuesList)).Select(e => new
      {
        FieldElement = e,
        Type = f,
        ListItemCount = e.Element((XName) "ALLOWEDVALUES").Elements((XName) "LISTITEM").Count<XElement>()
      })).Select(g => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorLimitsTooManyValuesInValueList((object) g.FieldElement.Attribute((XName) "refname").Value, (object) g.Type.Document.GetWITName(), (object) g.ListItemCount, (object) this.configuration.MaxValuesInSingleRuleValuesList), g.Type.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) g.FieldElement)), WitProcessTemplateValidatorHelpLinks.ValidatorLimitsTooManyValuesInValueList));
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateStatesOnTypeCountLimit(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions)
    {
      return typeDefinitions.Select(f => new
      {
        TypeDocument = f,
        StateCount = f.Document.Descendants((XName) "STATES").Elements<XElement>().Count<XElement>()
      }).Where(g => g.StateCount > this.configuration.MaxStatesPerWorkItemType).Select(g =>
      {
        WitProcessTemplateValidator.LoadedFile typeDocument = g.TypeDocument;
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorLimitsTooManyStatesInWIT((object) typeDocument.Document.GetWITName(), (object) g.StateCount, (object) this.configuration.MaxStatesPerWorkItemType);
        typeDocument = g.TypeDocument;
        string fileName = typeDocument.FileName;
        typeDocument = g.TypeDocument;
        int? lineNumber = new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) typeDocument.Document.Descendants((XName) "STATES").Single<XElement>()));
        string tooManyStatesInWit = WitProcessTemplateValidatorHelpLinks.ValidatorLimitsTooManyStatesInWIT;
        return new ProcessTemplateValidatorMessage(message, fileName, lineNumber, tooManyStatesInWit);
      });
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateCustomLinkTypeCountLimit(
      List<WitProcessTemplateValidator.LoadedFile> linkTypes)
    {
      WitProcessTemplateValidator templateValidator = this;
      if (templateValidator.configuration.CustomLinkTypesPermitted)
      {
        // ISSUE: reference to a compiler-generated method
        int num = linkTypes.SelectMany(d => d.Document.Descendants((XName) "LinkType").Select(e => new
        {
          RefName = e.Attribute((XName) "ReferenceName").Value,
          FileName = d.FileName,
          LineNumber = WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) e)
        })).Where(new Func<\u003C\u003Ef__AnonymousType10<string, string, int>, bool>(templateValidator.\u003CValidateCustomLinkTypeCountLimit\u003Eb__27_1)).Count();
        if (num > templateValidator.configuration.MaxCustomLinkTypes)
        {
          string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorLimitsTooManyCustomLinkTypes((object) num, (object) templateValidator.configuration.MaxCustomLinkTypes);
          string manyCustomLinkTypes = WitProcessTemplateValidatorHelpLinks.ValidatorLimitsTooManyCustomLinkTypes;
          int? lineNumber = new int?();
          string helpLink = manyCustomLinkTypes;
          yield return new ProcessTemplateValidatorMessage(message, (string) null, lineNumber, helpLink);
        }
      }
    }

    private static bool IsSystemLinkType(string refName) => ValidationMethods.IsSystemReferenceName(refName);

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateGlobalListItemCountLimit(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions,
      bool isTfsMigratorValidationContext)
    {
      return !this.configuration.GlobalListsPermitted && !isTfsMigratorValidationContext ? Enumerable.Empty<ProcessTemplateValidatorMessage>() : typeDefinitions.SelectMany(f => f.Document.Descendants((XName) "GLOBALLISTS").Elements<XElement>((XName) "GLOBALLIST").Select(e => new
      {
        ListElement = e,
        File = f,
        ItemCount = e.Elements((XName) "LISTITEM").Count<XElement>()
      })).Where(g => g.ItemCount > this.configuration.MaxGlobalListItemCountPerProcess).Select(g => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorLimitsTooManyItemsInAGlobalList((object) g.ListElement.Attribute((XName) "name").Value, (object) g.ItemCount, (object) this.configuration.MaxGlobalListItemCountPerProcess), g.File.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) g.ListElement)), WitProcessTemplateValidatorHelpLinks.ValidatorLimitsTooManyItemsInAGlobalList));
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateGlobalListCountLimit(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions,
      bool isTfsMigratorValidationContext)
    {
      if (this.configuration.GlobalListsPermitted | isTfsMigratorValidationContext)
      {
        int num = typeDefinitions.SelectMany<WitProcessTemplateValidator.LoadedFile, XElement>((Func<WitProcessTemplateValidator.LoadedFile, IEnumerable<XElement>>) (f => f.Document.Descendants((XName) "GLOBALLISTS").Elements<XElement>((XName) "GLOBALLIST"))).Count<XElement>();
        if (num > this.configuration.MaxGlobalListCountPerProcess)
        {
          string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorLimitsTooManyGlobalLists((object) num, (object) this.configuration.MaxGlobalListCountPerProcess);
          string tooManyGlobalLists = WitProcessTemplateValidatorHelpLinks.ValidatorLimitsTooManyGlobalLists;
          int? lineNumber = new int?();
          string helpLink = tooManyGlobalLists;
          yield return new ProcessTemplateValidatorMessage(message, (string) null, lineNumber, helpLink);
        }
      }
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateCategoryCountLimit(
      Dictionary<string, XElement> categoryElements)
    {
      if (categoryElements.Count > this.configuration.MaxCategoriesPerProcess)
      {
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorLimitsTooManyCategories((object) categoryElements.Count, (object) this.configuration.MaxCategoriesPerProcess);
        string tooManyCategories = WitProcessTemplateValidatorHelpLinks.ValidatorLimitsTooManyCategories;
        int? lineNumber = new int?();
        string helpLink = tooManyCategories;
        yield return new ProcessTemplateValidatorMessage(message, (string) null, lineNumber, helpLink);
      }
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateTotalFieldCountLimit(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions)
    {
      WitProcessTemplateValidator templateValidator = this;
      // ISSUE: reference to a compiler-generated method
      int num = typeDefinitions.SelectMany<WitProcessTemplateValidator.LoadedFile, ProcessFieldDefinition>(new Func<WitProcessTemplateValidator.LoadedFile, IEnumerable<ProcessFieldDefinition>>(templateValidator.\u003CValidateTotalFieldCountLimit\u003Eb__32_0)).Distinct<ProcessFieldDefinition>(templateValidator.fieldDefinitionComparer).Count<ProcessFieldDefinition>();
      if (num > templateValidator.configuration.MaxFieldsPerProcessTemplate)
      {
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorLimitsTooManyFieldsTotal((object) num, (object) templateValidator.configuration.MaxFieldsPerProcessTemplate);
        string tooManyFieldsTotal = WitProcessTemplateValidatorHelpLinks.ValidatorLimitsTooManyFieldsTotal;
        int? lineNumber = new int?();
        string helpLink = tooManyFieldsTotal;
        yield return new ProcessTemplateValidatorMessage(message, (string) null, lineNumber, helpLink);
      }
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateTypeCountLimit(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions)
    {
      if (typeDefinitions.Count > this.configuration.MaxWorkItemTypesPerProcess)
      {
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorLimitsTooManyWITs((object) typeDefinitions.Count, (object) this.configuration.MaxWorkItemTypesPerProcess);
        string limitsTooManyWiTs = WitProcessTemplateValidatorHelpLinks.ValidatorLimitsTooManyWITs;
        int? lineNumber = new int?();
        string helpLink = limitsTooManyWiTs;
        yield return new ProcessTemplateValidatorMessage(message, (string) null, lineNumber, helpLink);
      }
    }

    private ILookup<string, string> FeaturesForType(
      List<XElement> featureElements,
      Dictionary<string, XElement> categoryElements)
    {
      return featureElements.Where<XElement>((Func<XElement, bool>) (e => e.Attribute((XName) "category") != null && categoryElements.ContainsKey(e.Attribute((XName) "category").Value))).SelectMany(e => categoryElements[e.Attribute((XName) "category").Value].Elements().Select(ce => new
      {
        Feature = e.Name.LocalName,
        Type = ce.Attribute((XName) "name").Value
      })).ToLookup(ft => ft.Type, ft => ft.Feature, this.configuration.NameComparer);
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateFeatureFieldsInTypes(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions,
      List<XElement> featureElements,
      Dictionary<string, XElement> categoryElements)
    {
      ILookup<string, string> featuresForType = this.FeaturesForType(featureElements, categoryElements);
      return typeDefinitions.SelectMany<WitProcessTemplateValidator.LoadedFile, ProcessTemplateValidatorMessage>((Func<WitProcessTemplateValidator.LoadedFile, IEnumerable<ProcessTemplateValidatorMessage>>) (e => featuresForType[e.Document.GetWITName()].SelectMany<string, FeatureRequirement>((Func<string, IEnumerable<FeatureRequirement>>) (f => this.configuration.FeatureRequirements.Where<FeatureRequirement>((Func<FeatureRequirement, bool>) (fr => fr.FeatureName.Equals(f, StringComparison.Ordinal))))).SelectMany(fr => fr.Fields.Select(f => new
      {
        Feature = fr.FeatureName,
        Field = f
      })).Where(f => e.Document.AllFieldsInWIT().SingleOrDefault<XElement>((Func<XElement, bool>) (fe => TFStringComparer.WorkItemFieldReferenceName.Equals(fe.Attribute((XName) "refname").Value, f.Field))) == null).Select(f =>
      {
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorFeatureMissingField((object) f.Field, (object) f.Feature, (object) e.Document.GetWITName());
        string fileName = e.FileName;
        string featureMissingField = WitProcessTemplateValidatorHelpLinks.ValidatorFeatureMissingField;
        int? lineNumber = new int?();
        string helpLink = featureMissingField;
        return new ProcessTemplateValidatorMessage(message, fileName, lineNumber, helpLink);
      })));
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateAllFeaturesAreMappedToValidCategories(
      Dictionary<string, XElement> categoryElements,
      List<XElement> featureElements,
      WitProcessTemplateValidator.LoadedFile processConfigurationFile,
      string categoriesPath)
    {
      IEnumerable<ProcessTemplateValidatorMessage> second = featureElements.Where<XElement>((Func<XElement, bool>) (e => e.Attribute((XName) "category") != null && !categoryElements.ContainsKey(e.Attribute((XName) "category").Value) && e.Attribute((XName) "virtualFeature") != null)).Select<XElement, ProcessTemplateValidatorMessage>((Func<XElement, ProcessTemplateValidatorMessage>) (e =>
      {
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorFeatureMissingHardcodedCategory((object) e.Attribute((XName) "category").Value);
        string file = categoriesPath;
        string hardcodedCategory = WitProcessTemplateValidatorHelpLinks.ValidatorFeatureMissingHardcodedCategory;
        int? lineNumber = new int?();
        string helpLink = hardcodedCategory;
        return new ProcessTemplateValidatorMessage(message, file, lineNumber, helpLink);
      }));
      return featureElements.Where<XElement>((Func<XElement, bool>) (e => e.Attribute((XName) "category") != null && !categoryElements.ContainsKey(e.Attribute((XName) "category").Value) && e.Attribute((XName) "virtualFeature") == null)).Select<XElement, ProcessTemplateValidatorMessage>((Func<XElement, ProcessTemplateValidatorMessage>) (e => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorFeatureUnknownCategoryForFeature((object) e.Name.LocalName, (object) e.Attribute((XName) "category").Value), processConfigurationFile.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) e)), WitProcessTemplateValidatorHelpLinks.ValidatorFeatureUnknownCategoryForFeature))).Concat<ProcessTemplateValidatorMessage>(second);
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateFeatureMetastateMappingsInTypes(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions,
      List<XElement> featureElements,
      Dictionary<string, XElement> categoryElements,
      WitProcessTemplateValidator.LoadedFile processConfig)
    {
      List<ProcessTemplateValidatorMessage> validatorMessageList = new List<ProcessTemplateValidatorMessage>();
      Dictionary<string, FeatureMetastateRequirement> metastatesForFeature = this.configuration.FeatureRequirements.ToDictionary<FeatureRequirement, string, FeatureMetastateRequirement>((Func<FeatureRequirement, string>) (fr => fr.FeatureName), (Func<FeatureRequirement, FeatureMetastateRequirement>) (fr => fr.Metastate), (IEqualityComparer<string>) StringComparer.Ordinal);
      List<XElement> list = featureElements.Where<XElement>((Func<XElement, bool>) (e => e.Attribute((XName) "category") != null && categoryElements.ContainsKey(e.Attribute((XName) "category").Value) && metastatesForFeature.ContainsKey(e.Name.LocalName) && metastatesForFeature[e.Name.LocalName] != 0)).ToList<XElement>();
      validatorMessageList.AddRange(list.Where<XElement>((Func<XElement, bool>) (e => metastatesForFeature[e.Name.LocalName] == FeatureMetastateRequirement.Standard && e.Element((XName) "States").Elements().Attributes((XName) "type").Select<XAttribute, string>((Func<XAttribute, string>) (a => a.Value)).Where<string>((Func<string, bool>) (s => s.Equals("Proposed", StringComparison.Ordinal))).Count<string>() < 1)).Select<XElement, ProcessTemplateValidatorMessage>((Func<XElement, ProcessTemplateValidatorMessage>) (e => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorFeatureMissingMappingInConfig((object) "Proposed", (object) e.Name.LocalName), processConfig.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) e)), WitProcessTemplateValidatorHelpLinks.ValidatorFeatureMissingMappingInConfig))));
      validatorMessageList.AddRange(list.Where<XElement>((Func<XElement, bool>) (e => e.Element((XName) "States").Elements().Attributes((XName) "type").Select<XAttribute, string>((Func<XAttribute, string>) (a => a.Value)).Where<string>((Func<string, bool>) (s => s.Equals("Complete", StringComparison.Ordinal))).Count<string>() < 1)).Select<XElement, ProcessTemplateValidatorMessage>((Func<XElement, ProcessTemplateValidatorMessage>) (e => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorFeatureMissingMappingInConfig((object) "Complete", (object) e.Name.LocalName), processConfig.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) e)), WitProcessTemplateValidatorHelpLinks.ValidatorFeatureMissingMappingInConfig))));
      validatorMessageList.AddRange(list.SelectMany(e => e.Element((XName) "States").Elements().Attributes((XName) "type").Where<XAttribute>((Func<XAttribute, bool>) (a => !WitProcessTemplateValidator.legalMetastates[metastatesForFeature[e.Name.LocalName]].Contains<string>(a.Value, (IEqualityComparer<string>) StringComparer.Ordinal))).Select(a => new
      {
        FeatureElement = e,
        BadMetastateAttribute = a
      })).Select(es => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorFeatureUnknownMetastate((object) es.BadMetastateAttribute.Value, (object) es.FeatureElement.Name.LocalName), processConfig.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) es.BadMetastateAttribute)), WitProcessTemplateValidatorHelpLinks.ValidatorFeatureUnknownMetastate)));
      validatorMessageList.AddRange(list.SelectMany(e => e.Element((XName) "States").Elements().Attributes((XName) "value").GroupBy<XAttribute, string>((Func<XAttribute, string>) (a => a.Value), this.configuration.NameComparer).Where<IGrouping<string, XAttribute>>((Func<IGrouping<string, XAttribute>, bool>) (g => g.Count<XAttribute>() > 1)).Select(g => new
      {
        MultiMappedState = g.Key,
        FeatureElement = e
      })).Select(sf => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorFeatureStateMappedToMultipleMetastates((object) sf.MultiMappedState, (object) sf.FeatureElement.Name.LocalName), processConfig.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) sf.FeatureElement)), WitProcessTemplateValidatorHelpLinks.ValidatorFeatureStateMappedToMultipleMetastates)));
      IEnumerable<\u003C\u003Ef__AnonymousType16<XElement, ILookup<string, string>, FeatureMetastateRequirement, WitProcessTemplateValidator.LoadedFile>> source = list.Where<XElement>((Func<XElement, bool>) (e => e.Attribute((XName) "virtualFeature") == null)).SelectMany(e => categoryElements[e.Attribute((XName) "category").Value].Elements().Attributes((XName) "name").Select<XAttribute, string>((Func<XAttribute, string>) (a => a.Value)).Select(s => new
      {
        Feature = e,
        MetastateMap = e.Element((XName) "States").Elements().Select(se => new
        {
          State = se.Attribute((XName) "value").Value,
          Type = se.Attribute((XName) "type").Value
        }).ToLookup(se => se.Type, se => se.State, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase),
        Metastates = metastatesForFeature[e.Name.LocalName],
        Type = typeDefinitions.FirstOrDefault<WitProcessTemplateValidator.LoadedFile>((Func<WitProcessTemplateValidator.LoadedFile, bool>) (t => this.configuration.NameComparer.Equals(t.Document.GetWITName(), s)))
      })).Where(fm => fm.Type.Document != null);
      validatorMessageList.AddRange(source.Where(fm => fm.Metastates == FeatureMetastateRequirement.Standard && fm.MetastateMap["Proposed"].Any<string>() && fm.Type.Document.Descendants((XName) "STATES").Elements<XElement>().Attributes((XName) "value").Select<XAttribute, string>((Func<XAttribute, string>) (a => a.Value)).Intersect<string>(fm.MetastateMap["Proposed"], (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Count<string>() < 1).Select(fm =>
      {
        WitProcessTemplateValidator.LoadedFile type = fm.Type;
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorFeatureMissingMappedStateInType((object) type.Document.GetWITName(), (object) "Proposed", (object) fm.Feature.Name.LocalName);
        type = fm.Type;
        string fileName = type.FileName;
        string mappedStateInType = WitProcessTemplateValidatorHelpLinks.ValidatorFeatureMissingMappedStateInType;
        int? lineNumber = new int?();
        string helpLink = mappedStateInType;
        return new ProcessTemplateValidatorMessage(message, fileName, lineNumber, helpLink);
      }));
      validatorMessageList.AddRange(source.Where(fm => fm.MetastateMap["Complete"].Any<string>() && fm.Type.Document.Descendants((XName) "STATES").Elements<XElement>().Attributes((XName) "value").Select<XAttribute, string>((Func<XAttribute, string>) (a => a.Value)).Intersect<string>(fm.MetastateMap["Complete"], (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Count<string>() != 1).Select(fm =>
      {
        WitProcessTemplateValidator.LoadedFile type = fm.Type;
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorFeatureMissingMappedStateInType((object) type.Document.GetWITName(), (object) "Complete", (object) fm.Feature.Name.LocalName);
        type = fm.Type;
        string fileName = type.FileName;
        string mappedStateInType = WitProcessTemplateValidatorHelpLinks.ValidatorFeatureMissingMappedStateInType;
        int? lineNumber = new int?();
        string helpLink = mappedStateInType;
        return new ProcessTemplateValidatorMessage(message, fileName, lineNumber, helpLink);
      }));
      IEnumerable<\u003C\u003Ef__AnonymousType19<XElement, string, string>> outer = list.Where<XElement>((Func<XElement, bool>) (f => f.Attribute((XName) "virtualFeature") != null)).SelectMany(e => categoryElements[e.Attribute((XName) "category").Value].Elements().Select(ce => new
      {
        Feature = e,
        Type = ce.Attribute((XName) "name").Value,
        States = e.Element((XName) "States").Elements().Attributes((XName) "value").Select<XAttribute, string>((Func<XAttribute, string>) (a => a.Value))
      })).SelectMany(fts => fts.States.Select(s => new
      {
        Feature = fts.Feature,
        Type = fts.Type,
        State = s
      }));
      IEnumerable<\u003C\u003Ef__AnonymousType20<string, string, WitProcessTemplateValidator.LoadedFile>> inner = typeDefinitions.SelectMany(f => f.Document.Descendants((XName) "STATES").Elements<XElement>().Attributes((XName) "value").Select<XAttribute, string>((Func<XAttribute, string>) (a => a.Value)).Select(s => new
      {
        State = s,
        Type = f.Document.GetWITName(),
        File = f
      }));
      ILookup<string, string> fileNamesForType = typeDefinitions.ToLookup<WitProcessTemplateValidator.LoadedFile, string, string>((Func<WitProcessTemplateValidator.LoadedFile, string>) (f => f.Document.GetWITName()), (Func<WitProcessTemplateValidator.LoadedFile, string>) (f => f.FileName));
      validatorMessageList.AddRange(outer.GroupJoin(inner, rs => rs.Type, sit => sit.Type, (rs, sits) => new
      {
        RequiredState = rs.State,
        Type = rs.Type,
        Feature = rs.Feature,
        MatchedTypeStates = sits.Select(sit => sit.State)
      }, this.configuration.NameComparer).Where(g => !g.MatchedTypeStates.Contains<string>(g.RequiredState, this.configuration.NameComparer) && fileNamesForType.Contains(g.Type)).Select(g =>
      {
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorFeatureStateMissingInType((object) g.Type, (object) g.RequiredState, (object) g.Feature.Name.LocalName);
        string file = fileNamesForType[g.Type].First<string>();
        string stateMissingInType = WitProcessTemplateValidatorHelpLinks.ValidatorFeatureStateMissingInType;
        int? lineNumber = new int?();
        string helpLink = stateMissingInType;
        return new ProcessTemplateValidatorMessage(message, file, lineNumber, helpLink);
      }));
      return (IEnumerable<ProcessTemplateValidatorMessage>) validatorMessageList;
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateFeatureTypeFieldsInTypes(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions,
      List<XElement> featureElements,
      Dictionary<string, XElement> categoryElements,
      WitProcessTemplateValidator.LoadedFile processConfiguration)
    {
      ILookup<string, string> featuresForType = this.FeaturesForType(featureElements, categoryElements);
      Dictionary<string, string> refNameForTypeField = processConfiguration.Document.Descendants((XName) "TypeField").ToDictionary<XElement, string, string>((Func<XElement, string>) (e => e.Attribute((XName) "type").Value), (Func<XElement, string>) (e => e.Attribute((XName) "refname").Value), (IEqualityComparer<string>) StringComparer.Ordinal);
      Dictionary<string, List<string>> allowedValuesForTypeField = processConfiguration.Document.Descendants((XName) "TypeFieldValues").ToDictionary<XElement, string, List<string>>((Func<XElement, string>) (e => e.Parent.Attribute((XName) "type").Value), (Func<XElement, List<string>>) (e => e.Elements().Select<XElement, string>((Func<XElement, string>) (ve => ve.Attribute((XName) "value").Value)).ToList<string>()), (IEqualityComparer<string>) StringComparer.Ordinal);
      return typeDefinitions.SelectMany<WitProcessTemplateValidator.LoadedFile, ProcessTemplateValidatorMessage>((Func<WitProcessTemplateValidator.LoadedFile, IEnumerable<ProcessTemplateValidatorMessage>>) (e => featuresForType[e.Document.GetWITName()].SelectMany<string, FeatureRequirement>((Func<string, IEnumerable<FeatureRequirement>>) (f => this.configuration.FeatureRequirements.Where<FeatureRequirement>((Func<FeatureRequirement, bool>) (fr => fr.FeatureName.Equals(f, StringComparison.Ordinal))))).SelectMany(fr => fr.TypeFields.Select(tf => new
      {
        Feature = fr.FeatureName,
        TypeField = tf
      })).Where(tf => e.Document.AllFieldsInWIT().SingleOrDefault<XElement>((Func<XElement, bool>) (fe => TFStringComparer.WorkItemFieldReferenceName.Equals(fe.Attribute((XName) "refname").Value, refNameForTypeField[tf.TypeField]))) == null).Select(tf =>
      {
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorFeatureMissingTypeField((object) e.Document.GetWITName(), (object) tf.TypeField, (object) refNameForTypeField[tf.TypeField], (object) tf.Feature);
        string fileName = e.FileName;
        string missingTypeField = WitProcessTemplateValidatorHelpLinks.ValidatorFeatureMissingTypeField;
        int? lineNumber = new int?();
        string helpLink = missingTypeField;
        return new ProcessTemplateValidatorMessage(message, fileName, lineNumber, helpLink);
      }))).Concat<ProcessTemplateValidatorMessage>(typeDefinitions.SelectMany<WitProcessTemplateValidator.LoadedFile, ProcessTemplateValidatorMessage>((Func<WitProcessTemplateValidator.LoadedFile, IEnumerable<ProcessTemplateValidatorMessage>>) (e => featuresForType[e.Document.GetWITName()].SelectMany<string, FeatureRequirement>((Func<string, IEnumerable<FeatureRequirement>>) (f => this.configuration.FeatureRequirements.Where<FeatureRequirement>((Func<FeatureRequirement, bool>) (fr => fr.FeatureName.Equals(f, StringComparison.Ordinal))))).SelectMany(fr => fr.TypeFields.Select(tf => new
      {
        Feature = fr.FeatureName,
        TypeField = tf
      })).Select(tf => new
      {
        BadFieldElement = e.Document.AllFieldsInWIT().SingleOrDefault<XElement>((Func<XElement, bool>) (fe =>
        {
          if (!TFStringComparer.WorkItemFieldReferenceName.Equals(fe.Attribute((XName) "refname").Value, refNameForTypeField[tf.TypeField]) || !allowedValuesForTypeField.ContainsKey(tf.TypeField))
            return false;
          return fe.Element((XName) "ALLOWEDVALUES") == null || allowedValuesForTypeField[tf.TypeField].Any<string>((Func<string, bool>) (v => !fe.Element((XName) "ALLOWEDVALUES").Elements((XName) "LISTITEM").Attributes((XName) "value").Select<XAttribute, string>((Func<XAttribute, string>) (a => a.Value)).Contains<string>(v, this.configuration.NameComparer)));
        })),
        Feature = tf.Feature,
        TypeField = tf.TypeField
      }).Where(tf => tf.BadFieldElement != null).Select(tf => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorFeatureMissingAllowedValuesOnField((object) e.Document.GetWITName(), (object) refNameForTypeField[tf.TypeField], (object) tf.Feature), e.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) tf.BadFieldElement)), WitProcessTemplateValidatorHelpLinks.ValidatorFeatureMissingAllowedValuesOnField)))));
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateOnlyValidRulesOnFieldsThatLimitRules(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions)
    {
      if (this.configuration.NoFieldRuleRestrictions)
        return Enumerable.Empty<ProcessTemplateValidatorMessage>();
      Dictionary<string, IEnumerable<string>> restrictedFields = this.configuration.FieldRuleRestrictions.ToDictionary<FieldRuleRestriction, string, IEnumerable<string>>((Func<FieldRuleRestriction, string>) (r => r.FieldRefName), (Func<FieldRuleRestriction, IEnumerable<string>>) (r => r.AllowedRules), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      return typeDefinitions.SelectMany(f => f.Document.AllFieldsInWIT().Where<XElement>((Func<XElement, bool>) (e =>
      {
        // ISSUE: variable of a compiler-generated type
        WitProcessTemplateValidator.\u003C\u003Ec__DisplayClass39_0 cDisplayClass390 = this;
        XElement e1 = e;
        // ISSUE: reference to a compiler-generated field
        return restrictedFields.ContainsKey(e1.Attribute((XName) "refname").Value) && e1.Elements().Where<XElement>((Func<XElement, bool>) (c => !c.Name.LocalName.Equals("HELPTEXT", StringComparison.Ordinal) && !cDisplayClass390.restrictedFields[e1.Attribute((XName) "refname").Value].Contains<string>(c.Name.LocalName, (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName))).Any<XElement>();
      })).Select(e => new
      {
        FieldElement = e,
        FileName = f.FileName
      })).Select(f => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorBlockedRulesOnFieldThatLimitsRules((object) f.FieldElement.Attribute((XName) "refname").Value, (object) string.Join(", ", restrictedFields[f.FieldElement.Attribute((XName) "refname").Value])), f.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) f.FieldElement)), WitProcessTemplateValidatorHelpLinks.ValidatorBlockedRulesOnFieldThatLimitsRules));
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateNoRulesOnFieldsThatProhibitRules(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions)
    {
      return typeDefinitions.SelectMany(f => f.Document.AllFieldsInWIT().Where<XElement>((Func<XElement, bool>) (e => this.configuration.RulesProhibitedFields.Contains<string>(e.Attribute((XName) "refname").Value, (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName) && e.Elements().Where<XElement>((Func<XElement, bool>) (c => !c.Name.LocalName.Equals("HELPTEXT", StringComparison.Ordinal))).Any<XElement>())).Select(e => new
      {
        FieldElement = e,
        FileName = f.FileName
      })).Select(f => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorRulesOnFieldThatProhibitsRules((object) f.FieldElement.Attribute((XName) "refname").Value), f.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) f.FieldElement)), WitProcessTemplateValidatorHelpLinks.ValidatorRulesOnFieldThatProhibitsRules));
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidatePortfolioBacklogs(
      List<XElement> featureElements,
      Dictionary<string, XElement> categoryElements,
      string processConfigurationPath)
    {
      List<ProcessTemplateValidatorMessage> validatorMessageList1 = new List<ProcessTemplateValidatorMessage>();
      List<XElement> list = featureElements.Where<XElement>((Func<XElement, bool>) (e => e.Name.LocalName.Equals("PortfolioBacklog", StringComparison.Ordinal))).ToList<XElement>();
      if (list.Count > 5)
      {
        List<ProcessTemplateValidatorMessage> validatorMessageList2 = validatorMessageList1;
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorTooManyPortfolioBacklogs();
        string file = processConfigurationPath;
        string portfolioBacklogs = WitProcessTemplateValidatorHelpLinks.ValidatorTooManyPortfolioBacklogs;
        int? lineNumber = new int?();
        string helpLink = portfolioBacklogs;
        ProcessTemplateValidatorMessage validatorMessage = new ProcessTemplateValidatorMessage(message, file, lineNumber, helpLink);
        validatorMessageList2.Add(validatorMessage);
      }
      IEnumerable<ProcessTemplateValidatorMessage> collection1 = list.GroupBy<XElement, bool>((Func<XElement, bool>) (e => e.Attribute((XName) "parent") == null)).Where<IGrouping<bool, XElement>>((Func<IGrouping<bool, XElement>, bool>) (g => g.Key && g.Count<XElement>() > 1)).Select<IGrouping<bool, XElement>, ProcessTemplateValidatorMessage>((Func<IGrouping<bool, XElement>, ProcessTemplateValidatorMessage>) (g =>
      {
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorPortfolioBacklogMultipleRoots((object) string.Join(", ", g.Select<XElement, string>((Func<XElement, string>) (e => e.Attribute((XName) "category").Value))));
        string file = processConfigurationPath;
        string backlogMultipleRoots = WitProcessTemplateValidatorHelpLinks.ValidatorPortfolioBacklogMultipleRoots;
        int? lineNumber = new int?();
        string helpLink = backlogMultipleRoots;
        return new ProcessTemplateValidatorMessage(message, file, lineNumber, helpLink);
      }));
      validatorMessageList1.AddRange(collection1);
      IEnumerable<ProcessTemplateValidatorMessage> collection2 = list.GroupBy<XElement, string>((Func<XElement, string>) (e => e.Attribute((XName) "parent") == null ? (string) null : e.Attribute((XName) "parent").Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Where<IGrouping<string, XElement>>((Func<IGrouping<string, XElement>, bool>) (g => g.Key != null && g.Count<XElement>() > 1)).Select<IGrouping<string, XElement>, ProcessTemplateValidatorMessage>((Func<IGrouping<string, XElement>, ProcessTemplateValidatorMessage>) (g =>
      {
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorPortfilioBacklogsMultipleChildren((object) g.Key, (object) string.Join(", ", g.Select<XElement, string>((Func<XElement, string>) (e => e.Attribute((XName) "category").Value))));
        string file = processConfigurationPath;
        string multipleChildren = WitProcessTemplateValidatorHelpLinks.ValidatorPortfilioBacklogsMultipleChildren;
        int? lineNumber = new int?();
        string helpLink = multipleChildren;
        return new ProcessTemplateValidatorMessage(message, file, lineNumber, helpLink);
      }));
      validatorMessageList1.AddRange(collection2);
      HashSet<string> definedBacklogNames = new HashSet<string>(list.Select<XElement, string>((Func<XElement, string>) (e => e.Attribute((XName) "category").Value)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      IEnumerable<ProcessTemplateValidatorMessage> collection3 = list.Where<XElement>((Func<XElement, bool>) (e => e.Attribute((XName) "parent") != null && !definedBacklogNames.Contains(e.Attribute((XName) "parent").Value))).Select<XElement, ProcessTemplateValidatorMessage>((Func<XElement, ProcessTemplateValidatorMessage>) (e => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorPortfolioBacklogMissingParent((object) e.Attribute((XName) "category").Value, (object) e.Attribute((XName) "parent").Value), processConfigurationPath, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) e)), WitProcessTemplateValidatorHelpLinks.ValidatorPortfolioBacklogMissingParent)));
      validatorMessageList1.AddRange(collection3);
      return (IEnumerable<ProcessTemplateValidatorMessage>) validatorMessageList1;
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateAllTypesInCategoriesAreDefined(
      Dictionary<string, XElement> categoryElements,
      string categoryDocumentPath,
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions)
    {
      IEnumerable<string> typeNames = typeDefinitions.Select<WitProcessTemplateValidator.LoadedFile, string>((Func<WitProcessTemplateValidator.LoadedFile, string>) (f => f.Document.GetWITName()));
      return categoryElements.SelectMany(kvp => kvp.Value.Elements().Select(e => new
      {
        CategoryName = kvp.Key,
        TypeName = e.Attribute((XName) "name").Value,
        TypeElement = e
      })).Where(t => !typeNames.Contains<string>(t.TypeName, this.configuration.NameComparer)).Select(t => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorUnknownWorkItemTypeInCategory((object) t.TypeName, (object) t.CategoryName), categoryDocumentPath, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) t.TypeElement)), WitProcessTemplateValidatorHelpLinks.ValidatorUnknownWorkItemTypeInCategory));
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ScanForDeletedFields(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions,
      WitProcessTemplateMetadata previousTemplateVersion)
    {
      if (previousTemplateVersion == null)
        return Enumerable.Empty<ProcessTemplateValidatorMessage>();
      IEnumerable<\u003C\u003Ef__AnonymousType4<ProcessFieldDefinition, string, int>> inner = typeDefinitions.SelectMany(f => f.Document.AllFieldsInWIT().Where<XElement>((Func<XElement, bool>) (e => !e.Attribute((XName) "refname").Value.StartsWith("System.", StringComparison.OrdinalIgnoreCase))).Select(e => new
      {
        TemplateField = new ProcessFieldDefinition()
        {
          Name = e.Attribute((XName) "name").Value,
          ReferenceName = e.Attribute((XName) "refname").Value,
          Type = this.ParseFieldType(e.Attribute((XName) "type").Value)
        },
        FileName = f.FileName,
        LineNumber = WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) e)
      }));
      HashSet<ProcessFieldDefinition> customFieldsFromOtherTemplates = new HashSet<ProcessFieldDefinition>(this.existingTemplates.Where<WitProcessTemplateMetadata>((Func<WitProcessTemplateMetadata, bool>) (t => t.Id != previousTemplateVersion.Id)).SelectMany<WitProcessTemplateMetadata, ProcessFieldDefinition>((Func<WitProcessTemplateMetadata, IEnumerable<ProcessFieldDefinition>>) (t => t.DefinedFields)).Where<ProcessFieldDefinition>((Func<ProcessFieldDefinition, bool>) (f => !f.ReferenceName.StartsWith("System.", StringComparison.OrdinalIgnoreCase))), this.refNameOnlyFieldDefinitionComparer);
      return previousTemplateVersion.DefinedFields.Where<ProcessFieldDefinition>((Func<ProcessFieldDefinition, bool>) (fd => !fd.ReferenceName.StartsWith("System.", StringComparison.OrdinalIgnoreCase))).GroupJoin(inner, (Func<ProcessFieldDefinition, ProcessFieldDefinition>) (p => p), n => n.TemplateField, (p, ns) => new
      {
        Previous = p,
        New = ns
      }, this.refNameOnlyFieldDefinitionComparer).Where(g => !g.New.Any() && !customFieldsFromOtherTemplates.Contains(g.Previous)).Select(g =>
      {
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorFieldDelete((object) g.Previous.ReferenceName);
        string validatorFieldDelete = WitProcessTemplateValidatorHelpLinks.ValidatorFieldDelete;
        int? lineNumber = new int?();
        string helpLink = validatorFieldDelete;
        return new ProcessTemplateValidatorMessage(message, (string) null, lineNumber, helpLink);
      });
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ScanForDeletedWorkItemTypes(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions,
      WitProcessTemplateMetadata previousTemplateVersion)
    {
      if (previousTemplateVersion == null)
        return Enumerable.Empty<ProcessTemplateValidatorMessage>();
      HashSet<string> definedTypes = new HashSet<string>(typeDefinitions.Select<WitProcessTemplateValidator.LoadedFile, string>((Func<WitProcessTemplateValidator.LoadedFile, string>) (f => f.Document.GetWITRefName())), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      return previousTemplateVersion.DefinedWorkItemTypes.Where<ProcessWorkItemTypeDefinition>((Func<ProcessWorkItemTypeDefinition, bool>) (t => !definedTypes.Contains(t.ReferenceName))).Select<ProcessWorkItemTypeDefinition, ProcessTemplateValidatorMessage>((Func<ProcessWorkItemTypeDefinition, ProcessTemplateValidatorMessage>) (t =>
      {
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorWorkItemTypeDelete((object) t.Name);
        string workItemTypeDelete = WitProcessTemplateValidatorHelpLinks.ValidatorWorkItemTypeDelete;
        int? lineNumber = new int?();
        string helpLink = workItemTypeDelete;
        return new ProcessTemplateValidatorMessage(message, (string) null, lineNumber, helpLink);
      }));
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ScanForRenamedWorkItemTypes(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions,
      WitProcessTemplateMetadata previousTemplateVersion)
    {
      return previousTemplateVersion == null ? Enumerable.Empty<ProcessTemplateValidatorMessage>() : typeDefinitions.Join(previousTemplateVersion.DefinedWorkItemTypes, (Func<WitProcessTemplateValidator.LoadedFile, string>) (f => f.Document.GetWITRefName()), (Func<ProcessWorkItemTypeDefinition, string>) (p => p.ReferenceName), (f, p) => new
      {
        NewType = f,
        PreviousType = p
      }, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Where(t => !this.configuration.NameComparer.Equals(t.NewType.Document.GetWITName(), t.PreviousType.Name)).Select(t =>
      {
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorWorkItemTypeRename((object) t.PreviousType.ReferenceName, (object) t.NewType.Document.GetWITName(), (object) t.PreviousType.Name);
        WitProcessTemplateValidator.LoadedFile newType = t.NewType;
        string fileName = newType.FileName;
        newType = t.NewType;
        int? lineNumber = new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) newType.Document.Root.Element((XName) "WORKITEMTYPE").Attribute((XName) "name")));
        string workItemTypeRename = WitProcessTemplateValidatorHelpLinks.ValidatorWorkItemTypeRename;
        return new ProcessTemplateValidatorMessage(message, fileName, lineNumber, workItemTypeRename);
      });
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateProperDefinitionOfProtectedFields(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions)
    {
      return typeDefinitions.SelectMany(f => f.Document.AllFieldsInWIT().Select(e => new
      {
        TemplateField = new ProcessFieldDefinition()
        {
          Name = e.Attribute((XName) "name").Value,
          ReferenceName = e.Attribute((XName) "refname").Value,
          Type = this.ParseFieldType(e.Attribute((XName) "type").Value)
        },
        FileName = f.FileName,
        LineNumber = WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) e)
      }).Where(t => this.systemFields.Contains<ProcessFieldDefinition>(t.TemplateField, this.refNameOnlyFieldDefinitionComparer) && !this.systemFields.Contains(t.TemplateField))).Select(t => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorImproperDefinitionOfProtectedField((object) t.TemplateField.ReferenceName, (object) this.systemFields.Single<ProcessFieldDefinition>((Func<ProcessFieldDefinition, bool>) (f => f.ReferenceName.Equals(t.TemplateField.ReferenceName, StringComparison.OrdinalIgnoreCase))).Name, (object) this.systemFields.Single<ProcessFieldDefinition>((Func<ProcessFieldDefinition, bool>) (f => f.ReferenceName.Equals(t.TemplateField.ReferenceName, StringComparison.OrdinalIgnoreCase))).Type, (object) t.TemplateField.Name, (object) t.TemplateField.Type), t.FileName, new int?(t.LineNumber), WitProcessTemplateValidatorHelpLinks.ValidatorImproperDefinitionOfProtectedField));
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateUniqueDisplayNameForFields(
      IVssRequestContext requestContext,
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions)
    {
      IEnumerable<\u003C\u003Ef__AnonymousType24<XElement, string>> datas = typeDefinitions.SelectMany(f => f.Document.AllFieldsInWIT().Select(e => new
      {
        FieldElement = e,
        FileName = f.FileName
      }));
      IEnumerable<ProcessTemplateValidatorMessage> second = datas.Join(this.fieldsFromTableNotInTemplates, t => t.FieldElement.Attribute((XName) "name").Value, (Func<ProcessFieldDefinition, string>) (f => f.Name), (t, f) => new
      {
        TemplateField = t,
        TableField = f
      }, this.configuration.NameComparer).Where(t => !TFStringComparer.WorkItemFieldReferenceName.Equals(t.TemplateField.FieldElement.Attribute((XName) "refname").Value, t.TableField.ReferenceName)).Select(e => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorMultipleUsesOfDisplayNameInFieldTable((object) e.TemplateField.FieldElement.Attribute((XName) "refname").Value, (object) e.TemplateField.FieldElement.Attribute((XName) "name").Value, (object) e.TableField.ReferenceName), e.TemplateField.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) e.TemplateField.FieldElement)), WitProcessTemplateValidatorHelpLinks.ValidatorMultipleUsesOfDisplayNameInFieldTable));
      if (WorkItemTrackingFeatureFlags.IsValidateUniqueDisplayNameFixEnabled(requestContext))
      {
        IReadOnlyCollection<ProcessDescriptor> processDescriptors = requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptors(requestContext);
        ProcessFieldService processFieldService = requestContext.GetService<ProcessFieldService>();
        Dictionary<string, \u003C\u003Ef__AnonymousType31<\u003C\u003Ef__AnonymousType30<string, string>, List<string>>> usedFields = processDescriptors.SelectMany(t => processFieldService.GetFields(requestContext, t.TypeId).Select(f => new
        {
          Field = new
          {
            Name = f.Name,
            ReferenceName = f.ReferenceName
          },
          TemplateName = t.Name
        })).Concat(this.existingTemplates.SelectMany(t => t.DefinedFields.Select(f => new
        {
          Field = new
          {
            Name = f.Name,
            ReferenceName = f.ReferenceName
          },
          TemplateName = t.Name
        }))).GroupBy(t => t.Field.Name, this.configuration.NameComparer).ToDictionary(g => g.Key, g => new
        {
          Field = g.First().Field,
          Templates = g.Select(t => t.TemplateName).Distinct<string>().ToList<string>()
        }, this.configuration.NameComparer);
        return datas.Where(e => usedFields.ContainsKey(e.FieldElement.Attribute((XName) "name").Value) && !TFStringComparer.WorkItemFieldReferenceName.Equals(usedFields[e.FieldElement.Attribute((XName) "name").Value].Field.ReferenceName, e.FieldElement.Attribute((XName) "refname").Value)).Select(e => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorMultipleUsesOfDisplayNameAcrossTemplates((object) e.FieldElement.Attribute((XName) "refname").Value, (object) e.FieldElement.Attribute((XName) "name").Value, (object) usedFields[e.FieldElement.Attribute((XName) "name").Value].Field.ReferenceName, (object) string.Join(", ", (IEnumerable<string>) usedFields[e.FieldElement.Attribute((XName) "name").Value].Templates)), e.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) e.FieldElement)), WitProcessTemplateValidatorHelpLinks.ValidatorMultipleUsesOfDisplayNameAcrossTemplates)).Concat<ProcessTemplateValidatorMessage>(second);
      }
      Dictionary<string, \u003C\u003Ef__AnonymousType31<ProcessFieldDefinition, List<WitProcessTemplateMetadata>>> usedFields1 = this.existingTemplates.SelectMany(t => t.DefinedFields.Select(f => new
      {
        Field = f,
        Template = t
      })).GroupBy(t => t.Field.Name, this.configuration.NameComparer).ToDictionary(g => g.Key, g => new
      {
        Field = g.First().Field,
        Templates = g.Select(t => t.Template).ToList<WitProcessTemplateMetadata>()
      }, this.configuration.NameComparer);
      return datas.Where(e => usedFields1.ContainsKey(e.FieldElement.Attribute((XName) "name").Value) && !TFStringComparer.WorkItemFieldReferenceName.Equals(usedFields1[e.FieldElement.Attribute((XName) "name").Value].Field.ReferenceName, e.FieldElement.Attribute((XName) "refname").Value)).Select(e => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorMultipleUsesOfDisplayNameAcrossTemplates((object) e.FieldElement.Attribute((XName) "refname").Value, (object) e.FieldElement.Attribute((XName) "name").Value, (object) usedFields1[e.FieldElement.Attribute((XName) "name").Value].Field.ReferenceName, (object) string.Join(", ", usedFields1[e.FieldElement.Attribute((XName) "name").Value].Templates.Select<WitProcessTemplateMetadata, string>((Func<WitProcessTemplateMetadata, string>) (t => t.Name)))), e.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) e.FieldElement)), WitProcessTemplateValidatorHelpLinks.ValidatorMultipleUsesOfDisplayNameAcrossTemplates)).Concat<ProcessTemplateValidatorMessage>(second);
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateExistingAllowedValues(
      IVssRequestContext requestContext,
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions,
      WitProcessTemplateValidator.LoadedFile processConfiguration,
      WitProcessTemplateMetadata previousTemplateVersion)
    {
      try
      {
        if (!WorkItemTrackingFeatureFlags.IsValidateAllowedValuesOfExistingFieldsPresenceEnabled(requestContext) || !WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext))
          return Enumerable.Empty<ProcessTemplateValidatorMessage>();
        IWorkItemPickListService service1 = requestContext.GetService<IWorkItemPickListService>();
        WorkItemTrackingFieldService service2 = requestContext.GetService<WorkItemTrackingFieldService>();
        IEnumerable<string> second1 = previousTemplateVersion == null ? Enumerable.Empty<string>() : previousTemplateVersion.DefinedFields.Select<ProcessFieldDefinition, string>((Func<ProcessFieldDefinition, string>) (f => f.ReferenceName));
        HashSet<string> excludedFields = processConfiguration.Document.Descendants((XName) "TypeFieldValues").Select<XElement, string>((Func<XElement, string>) (e => e.Parent.Attribute((XName) "refname").Value)).Concat<string>(second1).ToHashSet<string>();
        IVssRequestContext requestContext1 = requestContext;
        bool? checkFreshness = new bool?();
        IEnumerable<FieldEntry> inner = service2.GetAllFields(requestContext1, checkFreshness).Where<FieldEntry>((Func<FieldEntry, bool>) (f => !excludedFields.Contains<string>(f.ReferenceName, (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName) && f.IsPicklist));
        IEnumerable<\u003C\u003Ef__AnonymousType33<\u003C\u003Ef__AnonymousType24<XElement, string>, FieldEntry>> source = typeDefinitions.SelectMany(f => f.Document.AllFieldsInWIT().Select(e => new
        {
          FieldElement = e,
          FileName = f.FileName
        })).Where(f => f.FieldElement.Attribute((XName) "type").Value.Equals("String") || f.FieldElement.Attribute((XName) "type").Value.Equals("Integer")).Join(inner, t => t.FieldElement.Attribute((XName) "refname").Value, (Func<FieldEntry, string>) (f => f.ReferenceName), (t, f) => new
        {
          TemplateField = t,
          UsedField = f
        }, (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
        if (!source.Any())
          return Enumerable.Empty<ProcessTemplateValidatorMessage>();
        Dictionary<string, IEnumerable<string>> globalListDefinitions = (Dictionary<string, IEnumerable<string>>) null;
        if (this.configuration.GlobalListsPermitted)
        {
          globalListDefinitions = typeDefinitions.SelectMany<WitProcessTemplateValidator.LoadedFile, XElement>((Func<WitProcessTemplateValidator.LoadedFile, IEnumerable<XElement>>) (f => f.Document.Descendants((XName) "GLOBALLISTS").Elements<XElement>((XName) "GLOBALLIST"))).ToDictionary<XElement, string, IEnumerable<string>>((Func<XElement, string>) (l => l.Attribute((XName) "name").Value), (Func<XElement, IEnumerable<string>>) (l => l.Elements((XName) "LISTITEM").Attributes((XName) "value").Select<XAttribute, string>((Func<XAttribute, string>) (a => a.Value))));
          ConstantsSearchSession constantsSearchSession = new ConstantsSearchSession(requestContext);
          Dictionary<int, string> globalListsFromDB = constantsSearchSession.ExpandNonIdentityConstantSet("299f07ef-6201-41b3-90fc-03eeb3977587").SelectMany<KeyValuePair<ConstantSetReference, SetRecord[]>, SetRecord>((Func<KeyValuePair<ConstantSetReference, SetRecord[]>, IEnumerable<SetRecord>>) (x => (IEnumerable<SetRecord>) x.Value)).ToDictionary<SetRecord, int, string>((Func<SetRecord, int>) (x => x.ItemId), (Func<SetRecord, string>) (x => x.Item.Remove(0, 1)));
          Dictionary<string, IEnumerable<string>> dictionary = constantsSearchSession.ExpandConstantSets(globalListsFromDB.Keys.ToArray<int>()).Where<KeyValuePair<ConstantSetReference, SetRecord[]>>((Func<KeyValuePair<ConstantSetReference, SetRecord[]>, bool>) (x => !globalListDefinitions.ContainsKey(globalListsFromDB.GetValueOrDefault<int, string>(x.Key.Id, (string) null)))).ToDictionary<KeyValuePair<ConstantSetReference, SetRecord[]>, string, IEnumerable<string>>((Func<KeyValuePair<ConstantSetReference, SetRecord[]>, string>) (kvp => globalListsFromDB.GetValueOrDefault<int, string>(kvp.Key.Id, (string) null)), (Func<KeyValuePair<ConstantSetReference, SetRecord[]>, IEnumerable<string>>) (kvp => ((IEnumerable<SetRecord>) kvp.Value).Select<SetRecord, string>((Func<SetRecord, string>) (x => x.Item))));
          globalListDefinitions = globalListDefinitions.Concat<KeyValuePair<string, IEnumerable<string>>>((IEnumerable<KeyValuePair<string, IEnumerable<string>>>) dictionary).ToDictionary<KeyValuePair<string, IEnumerable<string>>, string, IEnumerable<string>>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (x => x.Key), (Func<KeyValuePair<string, IEnumerable<string>>, IEnumerable<string>>) (x => x.Value));
        }
        List<ProcessTemplateValidatorMessage> validatorMessageList = new List<ProcessTemplateValidatorMessage>();
        foreach (var data in source)
        {
          XElement xelement = data.TemplateField.FieldElement.Element((XName) "ALLOWEDVALUES");
          IEnumerable<string> strings = xelement != null ? xelement.Elements((XName) "LISTITEM").Attributes((XName) "value").Select<XAttribute, string>((Func<XAttribute, string>) (a => a.Value)) : (IEnumerable<string>) null;
          if (this.configuration.GlobalListsPermitted && globalListDefinitions != null)
          {
            IEnumerable<string> second2 = data.TemplateField.FieldElement.Descendants((XName) "GLOBALLIST").SelectMany<XElement, string>((Func<XElement, IEnumerable<string>>) (l => globalListDefinitions.GetValueOrDefault<string, IEnumerable<string>>(l.Attribute((XName) "name").Value, (IEnumerable<string>) null)));
            strings = strings.Concat<string>(second2);
          }
          FieldEntry usedField = data.UsedField;
          if (strings != null && strings.Count<string>() > 0)
          {
            HashSet<string> usedFieldAllowedValues = service1.GetList(requestContext, usedField.PickListId.Value).Items.Select<WorkItemPickListMember, string>((Func<WorkItemPickListMember, string>) (i => i.Value)).ToHashSet<string>();
            if (usedFieldAllowedValues.Count != strings.Count<string>() || strings.Any<string>((Func<string, bool>) (v => !usedFieldAllowedValues.Contains(v))))
              validatorMessageList.Add(new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.FieldAllowedValuesMismatch((object) usedField.FieldType, (object) usedField.ReferenceName), data.TemplateField.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) data.TemplateField.FieldElement))));
          }
          else
          {
            service1.GetList(requestContext, usedField.PickListId.Value).Items.Select<WorkItemPickListMember, string>((Func<WorkItemPickListMember, string>) (i => i.Value));
            validatorMessageList.Add(new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.FieldAllowedValuesMismatch((object) usedField.FieldType, (object) usedField.ReferenceName), data.TemplateField.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) data.TemplateField.FieldElement))));
          }
        }
        return (IEnumerable<ProcessTemplateValidatorMessage>) validatorMessageList;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(902300, nameof (WitProcessTemplateValidator), nameof (ValidateExistingAllowedValues), ex);
        return Enumerable.Empty<ProcessTemplateValidatorMessage>();
      }
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateAllTypesHaveRefName(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions)
    {
      return typeDefinitions.Where<WitProcessTemplateValidator.LoadedFile>((Func<WitProcessTemplateValidator.LoadedFile, bool>) (f => f.Document.Root.Element((XName) "WORKITEMTYPE").Attribute((XName) "refname") == null)).Select<WitProcessTemplateValidator.LoadedFile, ProcessTemplateValidatorMessage>((Func<WitProcessTemplateValidator.LoadedFile, ProcessTemplateValidatorMessage>) (f => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorMissingRefNameOnWorkItemType((object) f.Document.GetWITName()), f.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) f.Document.Root.Element((XName) "WORKITEMTYPE"))), WitProcessTemplateValidatorHelpLinks.ValidatorMissingRefNameOnWorkItemType)));
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateUniqueWorkItemTypeNames(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions)
    {
      IEnumerable<ProcessTemplateValidatorMessage> second = typeDefinitions.Select<WitProcessTemplateValidator.LoadedFile, XAttribute>((Func<WitProcessTemplateValidator.LoadedFile, XAttribute>) (f => f.Document.Root.Element((XName) "WORKITEMTYPE").Attribute((XName) "refname"))).Where<XAttribute>((Func<XAttribute, bool>) (a => a != null)).GroupBy<XAttribute, string, int>((Func<XAttribute, string>) (s => s.Value), (Func<XAttribute, int>) (s => 1), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName).Where<IGrouping<string, int>>((Func<IGrouping<string, int>, bool>) (g => g.Count<int>() > 1)).Select<IGrouping<string, int>, ProcessTemplateValidatorMessage>((Func<IGrouping<string, int>, ProcessTemplateValidatorMessage>) (g =>
      {
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorMultipleUsesOfWorkItemTypeRefName((object) g.Key);
        string workItemTypeRefName = WitProcessTemplateValidatorHelpLinks.ValidatorMultipleUsesOfWorkItemTypeRefName;
        int? lineNumber = new int?();
        string helpLink = workItemTypeRefName;
        return new ProcessTemplateValidatorMessage(message, (string) null, lineNumber, helpLink);
      }));
      return typeDefinitions.Select<WitProcessTemplateValidator.LoadedFile, string>((Func<WitProcessTemplateValidator.LoadedFile, string>) (f => f.Document.GetWITName())).GroupBy<string, string, int>((Func<string, string>) (s => s), (Func<string, int>) (s => 1), this.configuration.NameComparer).Where<IGrouping<string, int>>((Func<IGrouping<string, int>, bool>) (g => g.Count<int>() > 1)).Select<IGrouping<string, int>, ProcessTemplateValidatorMessage>((Func<IGrouping<string, int>, ProcessTemplateValidatorMessage>) (g =>
      {
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorMultipleUsesOfWorkItemTypeName((object) g.Key);
        string workItemTypeName = WitProcessTemplateValidatorHelpLinks.ValidatorMultipleUsesOfWorkItemTypeName;
        int? lineNumber = new int?();
        string helpLink = workItemTypeName;
        return new ProcessTemplateValidatorMessage(message, (string) null, lineNumber, helpLink);
      })).Concat<ProcessTemplateValidatorMessage>(second);
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ScanForFieldRenamesAcrossTemplates(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions,
      List<ProcessTemplateFieldRenameData> fieldRenames)
    {
      Dictionary<string, \u003C\u003Ef__AnonymousType31<ProcessFieldDefinition, List<WitProcessTemplateMetadata>>> usedFields = this.existingTemplates.SelectMany(t => t.DefinedFields.Select(f => new
      {
        Field = f,
        Template = t
      })).GroupBy(t => t.Field.ReferenceName, (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName).ToDictionary(g => g.Key, g => new
      {
        Field = g.First().Field,
        Templates = g.Select(t => t.Template).ToList<WitProcessTemplateMetadata>()
      }, (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      List<\u003C\u003Ef__AnonymousType34<ProcessTemplateValidatorMessage, ProcessTemplateFieldRenameData>> list = typeDefinitions.SelectMany(f => f.Document.AllFieldsInWIT().Where<XElement>((Func<XElement, bool>) (e => !e.Attribute((XName) "refname").Value.StartsWith("System.", StringComparison.OrdinalIgnoreCase))).Select(e => new
      {
        FieldElement = e,
        FileName = f.FileName
      })).Where(e => usedFields.ContainsKey(e.FieldElement.Attribute((XName) "refname").Value) && !this.configuration.NameComparer.Equals(usedFields[e.FieldElement.Attribute((XName) "refname").Value].Field.Name, e.FieldElement.Attribute((XName) "name").Value)).Select(e => new
      {
        Warning = new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorRenameOfFieldInExistingTemplatesError((object) e.FieldElement.Attribute((XName) "refname").Value, (object) e.FieldElement.Attribute((XName) "name").Value, (object) usedFields[e.FieldElement.Attribute((XName) "refname").Value].Field.Name, (object) string.Join(", ", usedFields[e.FieldElement.Attribute((XName) "refname").Value].Templates.Select<WitProcessTemplateMetadata, string>((Func<WitProcessTemplateMetadata, string>) (t => t.Name)))), e.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) e.FieldElement)), WitProcessTemplateValidatorHelpLinks.ValidatorRenameOfFieldInExistingTemplates),
        RenameData = new ProcessTemplateFieldRenameData(e.FieldElement.Attribute((XName) "refname").Value, e.FieldElement.Attribute((XName) "name").Value, (IEnumerable<Guid>) usedFields[e.FieldElement.Attribute((XName) "refname").Value].Templates.Select<WitProcessTemplateMetadata, Guid>((Func<WitProcessTemplateMetadata, Guid>) (t => t.Id)).ToList<Guid>())
      }).ToList();
      fieldRenames?.AddRange(list.Select(r => r.RenameData));
      return list.Select(r => r.Warning);
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateConsistentFieldDefinitionsWithOtherTemplates(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions)
    {
      Dictionary<string, \u003C\u003Ef__AnonymousType31<ProcessFieldDefinition, List<WitProcessTemplateMetadata>>> usedFields = this.existingTemplates.SelectMany(t => t.DefinedFields.Select(f => new
      {
        Field = f,
        Template = t
      })).GroupBy(t => t.Field.ReferenceName, (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName).ToDictionary(g => g.Key, g => new
      {
        Field = g.First().Field,
        Templates = g.Select(t => t.Template).ToList<WitProcessTemplateMetadata>()
      }, (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      IEnumerable<\u003C\u003Ef__AnonymousType24<XElement, string>> datas = typeDefinitions.SelectMany(f => f.Document.AllFieldsInWIT().Select(e => new
      {
        FieldElement = e,
        FileName = f.FileName
      }));
      IEnumerable<ProcessTemplateValidatorMessage> second = datas.Join(this.fieldsFromTableNotInTemplates, t => t.FieldElement.Attribute((XName) "refname").Value, (Func<ProcessFieldDefinition, string>) (f => f.ReferenceName), (t, f) => new
      {
        TemplateField = t,
        TableField = f
      }, (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName).Where(t => !this.ParseFieldType(t.TemplateField.FieldElement.Attribute((XName) "type").Value).Equals((object) t.TableField.Type)).Select(e => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorInconsistentFieldDefinitionWithFieldTable((object) e.TemplateField.FieldElement.Attribute((XName) "refname").Value, (object) e.TemplateField.FieldElement.Attribute((XName) "type").Value, (object) e.TableField.Type), e.TemplateField.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) e.TemplateField.FieldElement)), WitProcessTemplateValidatorHelpLinks.ValidatorInconsistentFieldDefinitionWithFieldTable));
      return datas.Where(e => usedFields.ContainsKey(e.FieldElement.Attribute((XName) "refname").Value) && usedFields[e.FieldElement.Attribute((XName) "refname").Value].Field.Type != this.ParseFieldType(e.FieldElement.Attribute((XName) "type").Value)).Select(e => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorInconsistentFieldDefinitionWithExistingTemplate((object) e.FieldElement.Attribute((XName) "refname").Value, (object) string.Join(", ", usedFields[e.FieldElement.Attribute((XName) "refname").Value].Templates.Select<WitProcessTemplateMetadata, string>((Func<WitProcessTemplateMetadata, string>) (t => t.Name)).Distinct<string>(this.configuration.NameComparer))), e.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) e.FieldElement)), WitProcessTemplateValidatorHelpLinks.ValidatorInconsistentFieldDefinitionWithExistingTemplate)).Concat<ProcessTemplateValidatorMessage>(second);
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateNoFieldsInProtectedNamespace(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions)
    {
      return typeDefinitions.SelectMany(f => f.Document.AllFieldsInWIT().Where<XElement>((Func<XElement, bool>) (e => e.Attribute((XName) "refname").Value.StartsWith("System.", StringComparison.OrdinalIgnoreCase))).Select(e => new
      {
        Field = new ProcessFieldDefinition()
        {
          Name = e.Attribute((XName) "name").Value,
          ReferenceName = e.Attribute((XName) "refname").Value,
          Type = this.ParseFieldType(e.Attribute((XName) "type").Value)
        },
        FileName = f.FileName,
        LineNumber = WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) e)
      }).Where(t => !this.systemFields.Contains<ProcessFieldDefinition>(t.Field, this.refNameOnlyFieldDefinitionComparer))).Select(t => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorCustomFieldInProtectedNamespace((object) t.Field.ReferenceName, (object) "System"), t.FileName, new int?(t.LineNumber), WitProcessTemplateValidatorHelpLinks.ValidatorCustomFieldInProtectedNamespace));
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateNarrowBreakSpaceForConstants(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions)
    {
      return typeDefinitions.SelectMany<WitProcessTemplateValidator.LoadedFile, ProcessTemplateValidatorMessage>((Func<WitProcessTemplateValidator.LoadedFile, IEnumerable<ProcessTemplateValidatorMessage>>) (d => d.Document.Root.Descendants((XName) "HELPTEXT").Where<XElement>((Func<XElement, bool>) (a => a.ToString().Contains<char>(' '))).Select<XElement, ProcessTemplateValidatorMessage>((Func<XElement, ProcessTemplateValidatorMessage>) (a => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.validateNarrowBreakSpaceForConstants((object) a.ToString()), d.FileName))))).Concat<ProcessTemplateValidatorMessage>(typeDefinitions.SelectMany<WitProcessTemplateValidator.LoadedFile, ProcessTemplateValidatorMessage>((Func<WitProcessTemplateValidator.LoadedFile, IEnumerable<ProcessTemplateValidatorMessage>>) (d => d.Document.Root.Descendants((XName) "LISTITEM").Where<XElement>((Func<XElement, bool>) (a => a.ToString().Contains<char>(' '))).Select<XElement, ProcessTemplateValidatorMessage>((Func<XElement, ProcessTemplateValidatorMessage>) (a => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.validateNarrowBreakSpaceForConstants((object) a.ToString()), d.FileName))))));
    }

    private static IEnumerable<ProcessTemplateValidatorMessage> ValidateProcessConfiguration(
      WitProcessTemplateValidator.LoadedFile processConfiguration)
    {
      return ((IEnumerable<string>) new string[4]
      {
        "PortfolioBacklogs",
        "PortfolioBacklog",
        "RequirementBacklog",
        "TaskBacklog"
      }).Except<string>(processConfiguration.Document.Root.Elements().Select<XElement, string>((Func<XElement, string>) (e => e.Name.LocalName)).Concat<string>(processConfiguration.Document.Descendants((XName) "PortfolioBacklog").Select<XElement, string>((Func<XElement, string>) (e => e.Name.LocalName)).Distinct<string>((IEqualityComparer<string>) StringComparer.Ordinal))).Select<string, ProcessTemplateValidatorMessage>((Func<string, ProcessTemplateValidatorMessage>) (s =>
      {
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorMissingFeatureElementInProcessConfiguration((object) s);
        string fileName = processConfiguration.FileName;
        string processConfiguration1 = WitProcessTemplateValidatorHelpLinks.ValidatorMissingFeatureElementInProcessConfiguration;
        int? lineNumber = new int?();
        string helpLink = processConfiguration1;
        return new ProcessTemplateValidatorMessage(message, fileName, lineNumber, helpLink);
      })).Concat<ProcessTemplateValidatorMessage>(((IEnumerable<string>) new string[8]
      {
        "Team",
        "RemainingWork",
        "Order",
        "Effort",
        "Activity",
        "ApplicationStartInformation",
        "ApplicationLaunchInstructions",
        "ApplicationType"
      }).Except<string>(processConfiguration.Document.Descendants((XName) "TypeField").Select<XElement, string>((Func<XElement, string>) (e => e.Attribute((XName) "type").Value))).Select<string, ProcessTemplateValidatorMessage>((Func<string, ProcessTemplateValidatorMessage>) (s =>
      {
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorMissingTypeFieldDefinition((object) s);
        string fileName = processConfiguration.FileName;
        string typeFieldDefinition = WitProcessTemplateValidatorHelpLinks.ValidatorMissingTypeFieldDefinition;
        int? lineNumber = new int?();
        string helpLink = typeFieldDefinition;
        return new ProcessTemplateValidatorMessage(message, fileName, lineNumber, helpLink);
      })));
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateNoBannedRules(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions)
    {
      return this.configuration.AllRulesPermitted ? Enumerable.Empty<ProcessTemplateValidatorMessage>() : typeDefinitions.SelectMany<WitProcessTemplateValidator.LoadedFile, ProcessTemplateValidatorMessage>((Func<WitProcessTemplateValidator.LoadedFile, IEnumerable<ProcessTemplateValidatorMessage>>) (f => f.Document.Descendants().Where<XElement>((Func<XElement, bool>) (e => this.configuration.BannedRules.Contains<string>(e.Name.LocalName, (IEqualityComparer<string>) StringComparer.Ordinal))).Select<XElement, ProcessTemplateValidatorMessage>((Func<XElement, ProcessTemplateValidatorMessage>) (e => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorBannedRuleUsed((object) e.Name.LocalName), f.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) e)), WitProcessTemplateValidatorHelpLinks.ValidatorBannedRuleUsed)))));
    }

    private static IEnumerable<ProcessTemplateValidatorMessage> RunValidation(
      params Func<IEnumerable<ProcessTemplateValidatorMessage>>[] validationSteps)
    {
      return ((IEnumerable<Func<IEnumerable<ProcessTemplateValidatorMessage>>>) validationSteps).SelectMany<Func<IEnumerable<ProcessTemplateValidatorMessage>>, ProcessTemplateValidatorMessage>((Func<Func<IEnumerable<ProcessTemplateValidatorMessage>>, IEnumerable<ProcessTemplateValidatorMessage>>) (v => v()));
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateNoGlobalListsUsed(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions,
      bool isTfsMigratorValidationContext)
    {
      return this.configuration.GlobalListsPermitted | isTfsMigratorValidationContext ? Enumerable.Empty<ProcessTemplateValidatorMessage>() : typeDefinitions.SelectMany<WitProcessTemplateValidator.LoadedFile, ProcessTemplateValidatorMessage>((Func<WitProcessTemplateValidator.LoadedFile, IEnumerable<ProcessTemplateValidatorMessage>>) (f => f.Document.Descendants((XName) "GLOBALLISTS").Elements<XElement>((XName) "GLOBALLIST").Select<XElement, ProcessTemplateValidatorMessage>((Func<XElement, ProcessTemplateValidatorMessage>) (e => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorCannotDefineGlobalLists((object) e.Attribute((XName) "name").Value, (object) f.Document.GetWITName()), f.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) e)), WitProcessTemplateValidatorHelpLinks.ValidatorCannotDefineGlobalLists))))).Concat<ProcessTemplateValidatorMessage>(typeDefinitions.SelectMany<WitProcessTemplateValidator.LoadedFile, ProcessTemplateValidatorMessage>((Func<WitProcessTemplateValidator.LoadedFile, IEnumerable<ProcessTemplateValidatorMessage>>) (f => f.Document.Descendants((XName) "GLOBALLIST").Where<XElement>((Func<XElement, bool>) (e => !e.Parent.Name.LocalName.Equals("GLOBALLISTS", StringComparison.Ordinal))).Select<XElement, ProcessTemplateValidatorMessage>((Func<XElement, ProcessTemplateValidatorMessage>) (e => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorCannotReferenceAGlobalList((object) f.Document.GetWITName(), (object) e.Attribute((XName) "name").Value), f.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) e)), WitProcessTemplateValidatorHelpLinks.ValidatorCannotReferenceAGlobalList))))));
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateNoCustomFormControls(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions)
    {
      if (this.configuration.CustomControlsPermitted)
        return Enumerable.Empty<ProcessTemplateValidatorMessage>();
      List<ProcessTemplateValidatorMessage> validatorMessageList = new List<ProcessTemplateValidatorMessage>();
      validatorMessageList.AddRange(typeDefinitions.SelectMany<WitProcessTemplateValidator.LoadedFile, ProcessTemplateValidatorMessage>((Func<WitProcessTemplateValidator.LoadedFile, IEnumerable<ProcessTemplateValidatorMessage>>) (f => f.Document.Descendants((XName) "Control").Where<XElement>((Func<XElement, bool>) (e => !this.configuration.SystemControlWhiteList.Contains<string>(e.Attribute((XName) "Type").Value, (IEqualityComparer<string>) StringComparer.Ordinal))).Select<XElement, ProcessTemplateValidatorMessage>((Func<XElement, ProcessTemplateValidatorMessage>) (e => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorNoCustomFormControls((object) e.Attribute((XName) "Type").Value, (object) f.Document.GetWITName()), f.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) e)), WitProcessTemplateValidatorHelpLinks.ValidatorNoCustomFormControls))))));
      return (IEnumerable<ProcessTemplateValidatorMessage>) validatorMessageList;
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ValidateLinkTypeDefinitionConsistency(
      List<WitProcessTemplateValidator.LoadedFile> linkTypes)
    {
      List<ProcessTemplateValidatorMessage> errors = new List<ProcessTemplateValidatorMessage>();
      Dictionary<string, LinkTypeDefinition> dictionary = this.configuration.SystemLinkTypeWhiteList.ToDictionary<LinkTypeDefinition, string>((Func<LinkTypeDefinition, string>) (x => x.ReferenceName), (IEqualityComparer<string>) TFStringComparer.WorkItemLinkTypeReferenceName);
      List<\u003C\u003Ef__AnonymousType36<string, string, string, string, string, int>> list = linkTypes.SelectMany(d => d.Document.Descendants((XName) "LinkType").Select(l =>
      {
        XAttribute xattribute1 = l.Attribute((XName) "ReferenceName");
        if (xattribute1 == null)
          return null;
        XAttribute xattribute2 = l.Attribute((XName) "Topology");
        if (xattribute2 == null)
          errors.Add(new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorLinkTypeAttributeNotExist((object) "Topology"), d.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) l)), WitProcessTemplateValidatorHelpLinks.ValidatorLinkTypeAttributeNotExist));
        XAttribute xattribute3 = l.Attribute((XName) "ForwardName");
        if (xattribute3 == null)
          errors.Add(new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorLinkTypeAttributeNotExist((object) "ForwardName"), d.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) l)), WitProcessTemplateValidatorHelpLinks.ValidatorLinkTypeAttributeNotExist));
        XAttribute xattribute4 = l.Attribute((XName) "ReverseName");
        return new
        {
          ReferenceName = xattribute1.Value,
          ForwardName = xattribute3?.Value,
          ReverseName = xattribute4?.Value,
          Topology = xattribute2?.Value,
          FileName = d.FileName,
          LineNumber = WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) l)
        };
      })).Where(t => t != null).ToList();
      foreach (IGrouping<string, \u003C\u003Ef__AnonymousType36<string, string, string, string, string, int>> grouping in list.GroupBy(t => t.ReferenceName, (IEqualityComparer<string>) TFStringComparer.WorkItemLinkTypeReferenceName))
      {
        IGrouping<string, \u003C\u003Ef__AnonymousType36<string, string, string, string, string, int>> refNameGroup = grouping;
        if (refNameGroup.Count() != 1)
        {
          List<ProcessTemplateValidatorMessage> validatorMessageList = errors;
          string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorLinkTypeDefinitionNotUnique((object) refNameGroup.Key);
          string definitionNotUnique = WitProcessTemplateValidatorHelpLinks.ValidatorLinkTypeDefinitionNotUnique;
          int? lineNumber = new int?();
          string helpLink = definitionNotUnique;
          ProcessTemplateValidatorMessage validatorMessage = new ProcessTemplateValidatorMessage(message, (string) null, lineNumber, helpLink);
          validatorMessageList.Add(validatorMessage);
        }
        LinkTypeDefinition systemLinkType;
        if (dictionary.TryGetValue(refNameGroup.Key, out systemLinkType))
          errors.AddRange(refNameGroup.Where(x => systemLinkType.ForwardName != x.ForwardName || systemLinkType.ReverseName != x.ReverseName || systemLinkType.Topology != x.Topology).Select(x => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorImproperDefinitionOfProtectedLinkType((object) refNameGroup.Key, (object) systemLinkType.ForwardName, (object) systemLinkType.ReverseName, (object) systemLinkType.Topology), x.FileName, new int?(x.LineNumber), WitProcessTemplateValidatorHelpLinks.ValidatorImproperDefinitionOfProtectedLinkType)));
        else if (!this.configuration.CustomLinkTypesPermitted)
          errors.AddRange(refNameGroup.Select(x => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorNoCustomLinkTypes((object) x.ReferenceName), x.FileName, new int?(x.LineNumber), WitProcessTemplateValidatorHelpLinks.ValidatorNoCustomLinkTypes)));
        else if (WitProcessTemplateValidator.IsSystemLinkType(refNameGroup.Key))
          errors.AddRange(refNameGroup.Select(x => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorCustomLinkTypeInProtectedNamespace((object) refNameGroup.Key, (object) "System"), x.FileName, new int?(x.LineNumber), WitProcessTemplateValidatorHelpLinks.ValidatorCustomLinkTypeInProtectedNamespace)));
      }
      errors.AddRange(list.Where(x => !string.IsNullOrEmpty(x.ForwardName)).Select(x => new
      {
        Key = x.ForwardName,
        Value = x
      }).Union(list.Where(x => !string.IsNullOrEmpty(x.ReverseName) && !this.configuration.NameComparer.Equals(x.ReverseName, x.ForwardName)).Select(x => new
      {
        Key = x.ReverseName,
        Value = x
      })).GroupBy(x => x.Key, x => x.Value, this.configuration.NameComparer).Where<IGrouping<string, \u003C\u003Ef__AnonymousType36<string, string, string, string, string, int>>>(x => x.Count() > 1).Select<IGrouping<string, \u003C\u003Ef__AnonymousType36<string, string, string, string, string, int>>, ProcessTemplateValidatorMessage>(x =>
      {
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorMultipleUsesOfLinkTypeDisplayName((object) x.Key, (object) string.Join(", ", x.Select(t => t.ReferenceName).Distinct<string>((IEqualityComparer<string>) TFStringComparer.WorkItemLinkTypeReferenceName)));
        string linkTypeDisplayName = WitProcessTemplateValidatorHelpLinks.ValidatorMultipleUsesOfLinkTypeDisplayName;
        int? lineNumber = new int?();
        string helpLink = linkTypeDisplayName;
        return new ProcessTemplateValidatorMessage(message, (string) null, lineNumber, helpLink);
      }));
      return (IEnumerable<ProcessTemplateValidatorMessage>) errors;
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ScanForInvalidIdentityFields(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions)
    {
      return this.configuration.IdentitiesWithoutSyncNameChangesPermitted ? Enumerable.Empty<ProcessTemplateValidatorMessage>() : typeDefinitions.Select(file => new
      {
        Type = file,
        Fields = file.Document.AllFieldsInWIT().Where<XElement>((Func<XElement, bool>) (e =>
        {
          if (e.Attribute((XName) "syncnamechanges") == null)
            return true;
          return e.Attribute((XName) "syncnamechanges") != null && e.Attribute((XName) "syncnamechanges").Value.Equals("false", StringComparison.OrdinalIgnoreCase);
        })).GroupJoin(file.Document.AllFieldsInWorkflow(), (Func<XElement, string>) (field => field.Attribute((XName) "refname")?.Value), (Func<XElement, string>) (workflowField => workflowField.Attribute((XName) "refname")?.Value), (field, workflowField) => new
        {
          TypeField = field,
          WorkflowFields = workflowField.DefaultIfEmpty<XElement>()
        }, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).SelectMany(fields =>
        {
          List<XElement> xelementList = new List<XElement>();
          xelementList.Add(fields.TypeField);
          if (fields.WorkflowFields != null)
            xelementList.AddRange(fields.WorkflowFields.Where<XElement>((Func<XElement, bool>) (f => f != null)));
          return (IEnumerable<XElement>) xelementList;
        })
      }).Select(fieldAndType => new
      {
        Type = fieldAndType.Type,
        Fields = fieldAndType.Fields.Where<XElement>((Func<XElement, bool>) (field => WitProcessTemplateValidator.ContainsIdentityRule(field)))
      }).Where(fieldAndType => fieldAndType.Fields.Any<XElement>()).SelectMany(fieldAndType => fieldAndType.Fields.Select<XElement, ProcessTemplateValidatorMessage>((Func<XElement, ProcessTemplateValidatorMessage>) (f => new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorUnsupportedIdentityFieldUsage((object) f.Attribute((XName) "refname").Value, (object) fieldAndType.Type.Document.GetWITName()), fieldAndType.Type.FileName, new int?(WitProcessTemplateValidator.GetLineNumberForXmlItem((XObject) f))))));
    }

    private IEnumerable<ProcessTemplateValidatorMessage> ScanForRequiredFieldsOnLayout(
      List<WitProcessTemplateValidator.LoadedFile> typeDefinitions)
    {
      List<ProcessTemplateValidatorMessage> validatorMessages = new List<ProcessTemplateValidatorMessage>();
      foreach (WitProcessTemplateValidator.LoadedFile typeDefinition1 in typeDefinitions)
      {
        WitProcessTemplateValidator.LoadedFile typeDefinition = typeDefinition1;
        XmlElement xmlElement1 = typeDefinition.Document.Root.Element((XName) "WORKITEMTYPE").Element((XName) "FIELDS").ToXmlElement();
        XmlElement xmlElement2 = typeDefinition.Document.Root.Element((XName) "WORKITEMTYPE").Element((XName) "WORKFLOW").ToXmlElement();
        XmlElement xmlElement3 = typeDefinition.Document.Root.Element((XName) "WORKITEMTYPE").Element((XName) "FORM").ToXmlElement();
        XmlElement workflowElement = xmlElement2;
        XmlElement formElement = xmlElement3;
        Action<string> requiredFieldNotInBothLayoutsErrorAction = (Action<string>) (fieldName =>
        {
          List<ProcessTemplateValidatorMessage> validatorMessageList = validatorMessages;
          string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorRequiredFieldsNotOnLayout((object) typeDefinition.Document.GetWITName(), (object) fieldName);
          string fileName = typeDefinition.FileName;
          string fieldsNotInLayout = WitProcessTemplateValidatorHelpLinks.ValidatorRequiredFieldsNotInLayout;
          int? lineNumber = new int?();
          string helpLink = fieldsNotInLayout;
          ProcessTemplateValidatorMessage validatorMessage = new ProcessTemplateValidatorMessage(message, fileName, lineNumber, helpLink);
          validatorMessageList.Add(validatorMessage);
        });
        WebLayoutXmlHelper.ValidateRequiredFieldsOnLayout(xmlElement1, workflowElement, formElement, requiredFieldNotInBothLayoutsErrorAction);
      }
      return (IEnumerable<ProcessTemplateValidatorMessage>) validatorMessages;
    }

    private static bool ContainsIdentityRule(XElement field) => field.Descendants((XName) "VALIDUSER").Any<XElement>() || WitProcessTemplateValidator.ContainsIdentityListItem(field, "ALLOWEDVALUES") || WitProcessTemplateValidator.ContainsIdentityListItem(field, "PROHIBITEDVALUES");

    private static bool ContainsIdentityListItem(XElement field, string nodeName) => field.Descendants((XName) nodeName).Any<XElement>() && field.Descendants((XName) nodeName).Any<XElement>((Func<XElement, bool>) (d => d.Descendants((XName) "LISTITEM").Any<XElement>((Func<XElement, bool>) (li => li.Attribute((XName) "value") != null && li.Attribute((XName) "value").Value.Contains("\\")))));

    private List<ProcessTemplateValidatorMessage> ValidateFieldDefinitionInternalConsistency(
      List<WitProcessTemplateValidator.LoadedFile> typeDefintionDocuments)
    {
      List<ProcessTemplateValidatorMessage> validatorMessageList1 = new List<ProcessTemplateValidatorMessage>();
      foreach (IGrouping<string, \u003C\u003Ef__AnonymousType40<XElement, string>> source in typeDefintionDocuments.SelectMany(d => d.Document.AllFieldsInWIT().Select(e => new
      {
        FieldElement = e,
        TypeName = d.Document.GetWITName()
      })).GroupBy(t => t.FieldElement.Attribute((XName) "refname").Value, (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName))
      {
        if (source.Select(t => t.FieldElement.Attribute((XName) "name").Value).Distinct<string>(this.configuration.NameComparer).Count<string>() != 1)
        {
          List<ProcessTemplateValidatorMessage> validatorMessageList2 = validatorMessageList1;
          string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorInconsistentFieldDefinitionInTemplate((object) source.Key, (object) string.Join(", ", source.Select(t => t.TypeName)));
          string definitionInTemplate = WitProcessTemplateValidatorHelpLinks.ValidatorInconsistentFieldDefinitionInTemplate;
          int? lineNumber = new int?();
          string helpLink = definitionInTemplate;
          ProcessTemplateValidatorMessage validatorMessage = new ProcessTemplateValidatorMessage(message, (string) null, lineNumber, helpLink);
          validatorMessageList2.Add(validatorMessage);
        }
        if (source.Select(t => t.FieldElement.Attribute((XName) "type").Value).Distinct<string>((IEqualityComparer<string>) StringComparer.Ordinal).Count<string>() != 1)
        {
          List<ProcessTemplateValidatorMessage> validatorMessageList3 = validatorMessageList1;
          string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorInconsistentFieldDefinitionInTemplate((object) source.Key, (object) string.Join(", ", source.Select(t => t.TypeName)));
          string definitionInTemplate = WitProcessTemplateValidatorHelpLinks.ValidatorInconsistentFieldDefinitionInTemplate;
          int? lineNumber = new int?();
          string helpLink = definitionInTemplate;
          ProcessTemplateValidatorMessage validatorMessage = new ProcessTemplateValidatorMessage(message, (string) null, lineNumber, helpLink);
          validatorMessageList3.Add(validatorMessage);
        }
        if (source.Select(t => t.FieldElement.Attribute((XName) "syncnamechanges") == null ? "false" : t.FieldElement.Attribute((XName) "syncnamechanges").Value).Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Count<string>() != 1)
        {
          List<ProcessTemplateValidatorMessage> validatorMessageList4 = validatorMessageList1;
          string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorInconsistentFieldDefinitionInTemplate((object) source.Key, (object) string.Join(", ", source.Select(t => t.TypeName)));
          string definitionInTemplate = WitProcessTemplateValidatorHelpLinks.ValidatorInconsistentFieldDefinitionInTemplate;
          int? lineNumber = new int?();
          string helpLink = definitionInTemplate;
          ProcessTemplateValidatorMessage validatorMessage = new ProcessTemplateValidatorMessage(message, (string) null, lineNumber, helpLink);
          validatorMessageList4.Add(validatorMessage);
        }
      }
      foreach (IGrouping<string, \u003C\u003Ef__AnonymousType40<XElement, string>> source in typeDefintionDocuments.SelectMany(d => d.Document.AllFieldsInWIT().Select(e => new
      {
        FieldElement = e,
        TypeName = d.Document.GetWITName()
      })).GroupBy(t => t.FieldElement.Attribute((XName) "name").Value, this.configuration.NameComparer))
      {
        if (source.Select(t => t.FieldElement.Attribute((XName) "refname").Value).Distinct<string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName).Count<string>() != 1)
        {
          List<ProcessTemplateValidatorMessage> validatorMessageList5 = validatorMessageList1;
          string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorMultipleUsesOfDisplayNameInTemplate((object) source.Key, (object) string.Join(", ", source.Select(t => t.TypeName)));
          string displayNameInTemplate = WitProcessTemplateValidatorHelpLinks.ValidatorMultipleUsesOfDisplayNameInTemplate;
          int? lineNumber = new int?();
          string helpLink = displayNameInTemplate;
          ProcessTemplateValidatorMessage validatorMessage = new ProcessTemplateValidatorMessage(message, (string) null, lineNumber, helpLink);
          validatorMessageList5.Add(validatorMessage);
        }
      }
      return validatorMessageList1;
    }

    private WitProcessTemplateValidator.LoadedTemplateState LoadTemplate(
      IProcessTemplatePackage package,
      List<ProcessTemplateValidatorMessage> errors,
      bool allowLegacyAgileSetting)
    {
      XDocument document1;
      ProcessTemplateValidatorMessage validatorMessage1 = WitProcessTemplateValidator.SafeFileLoad(package, "ProcessTemplate.xml", out document1);
      if (validatorMessage1 != null)
      {
        errors.Add(validatorMessage1);
        return new WitProcessTemplateValidator.LoadedTemplateState();
      }
      XElement xelement1 = document1.Descendants((XName) "group").SingleOrDefault<XElement>((Func<XElement, bool>) (e => e.Attribute((XName) "id").Value.Equals("WorkItemTracking", StringComparison.Ordinal)));
      if (xelement1 == null)
      {
        List<ProcessTemplateValidatorMessage> validatorMessageList = errors;
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorNoWorkItemTrackingGroupInProcessTemplate();
        string inProcessTemplate = WitProcessTemplateValidatorHelpLinks.ValidatorNoWorkItemTrackingGroupInProcessTemplate;
        int? lineNumber = new int?();
        string helpLink = inProcessTemplate;
        ProcessTemplateValidatorMessage validatorMessage2 = new ProcessTemplateValidatorMessage(message, "ProcessTemplate.xml", lineNumber, helpLink);
        validatorMessageList.Add(validatorMessage2);
        return new WitProcessTemplateValidator.LoadedTemplateState();
      }
      string str = xelement1.Element((XName) "taskList").Attribute((XName) "filename").Value;
      XDocument document2;
      ProcessTemplateValidatorMessage validatorMessage3 = WitProcessTemplateValidator.SafeFileLoad(package, str, out document2);
      if (validatorMessage3 != null)
      {
        errors.Add(validatorMessage3);
        return new WitProcessTemplateValidator.LoadedTemplateState();
      }
      IEnumerable<string> paths1 = document2.Descendants((XName) "WORKITEMTYPE").Attributes((XName) "fileName").Select<XAttribute, string>((Func<XAttribute, string>) (a => a.Value));
      List<WitProcessTemplateValidator.LoadedFile> loadedFileList1 = new List<WitProcessTemplateValidator.LoadedFile>();
      IProcessTemplatePackage package1 = package;
      List<WitProcessTemplateValidator.LoadedFile> documentList1 = loadedFileList1;
      List<ProcessTemplateValidatorMessage> errors1 = errors;
      WitProcessTemplateValidator.LoadDocuments(paths1, package1, documentList1, errors1);
      Func<IEnumerable<WitProcessTemplateValidator.LoadedFile>, InternalSchemaType, Action<object, ValidationEventArgs, WitProcessTemplateValidator.LoadedFile>, IEnumerable<WitProcessTemplateValidator.LoadedFile>> func = (Func<IEnumerable<WitProcessTemplateValidator.LoadedFile>, InternalSchemaType, Action<object, ValidationEventArgs, WitProcessTemplateValidator.LoadedFile>, IEnumerable<WitProcessTemplateValidator.LoadedFile>>) ((input, schema, action) =>
      {
        XmlSchemaSet schemaSet = new XmlSchemaSet();
        InternalSchemas.InitSchemaSet(schema, schemaSet);
        return (IEnumerable<WitProcessTemplateValidator.LoadedFile>) input.Select<WitProcessTemplateValidator.LoadedFile, WitProcessTemplateValidator.LoadedFile>((Func<WitProcessTemplateValidator.LoadedFile, WitProcessTemplateValidator.LoadedFile>) (x =>
        {
          bool isValid = true;
          x.Document.Validate(schemaSet, (ValidationEventHandler) ((s, ea) =>
          {
            isValid = false;
            action(s, ea, x);
          }));
          if (isValid)
            return x;
          return new WitProcessTemplateValidator.LoadedFile()
          {
            Document = x.Document,
            FileName = x.FileName,
            IsValid = isValid
          };
        })).ToList<WitProcessTemplateValidator.LoadedFile>();
      });
      List<WitProcessTemplateValidator.LoadedFile> list1 = func((IEnumerable<WitProcessTemplateValidator.LoadedFile>) loadedFileList1, InternalSchemaType.WorkItemType, (Action<object, ValidationEventArgs, WitProcessTemplateValidator.LoadedFile>) ((s, ea, lf) => errors.Add(WitProcessTemplateValidator.CreateTypeSchemaError(s, ea, lf)))).ToList<WitProcessTemplateValidator.LoadedFile>();
      IEnumerable<string> strings1 = document2.Descendants((XName) "CATEGORIES").Attributes((XName) "fileName").Select<XAttribute, string>((Func<XAttribute, string>) (a => a.Value));
      if (strings1.Count<string>() > 1)
      {
        List<ProcessTemplateValidatorMessage> validatorMessageList = errors;
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorMultipleCategoriesDocuments();
        string file = str;
        string categoriesDocuments = WitProcessTemplateValidatorHelpLinks.ValidatorMultipleCategoriesDocuments;
        int? lineNumber = new int?();
        string helpLink = categoriesDocuments;
        ProcessTemplateValidatorMessage validatorMessage4 = new ProcessTemplateValidatorMessage(message, file, lineNumber, helpLink);
        validatorMessageList.Add(validatorMessage4);
        return new WitProcessTemplateValidator.LoadedTemplateState();
      }
      List<WitProcessTemplateValidator.LoadedFile> documentList2 = new List<WitProcessTemplateValidator.LoadedFile>();
      WitProcessTemplateValidator.LoadDocuments(strings1, package, documentList2, errors);
      IEnumerable<string> strings2 = document2.Descendants((XName) "PROCESSCONFIGURATION").Elements<XElement>((XName) "ProjectConfiguration").Attributes((XName) "fileName").Select<XAttribute, string>((Func<XAttribute, string>) (a => a.Value));
      List<WitProcessTemplateValidator.LoadedFile> loadedFileList2 = new List<WitProcessTemplateValidator.LoadedFile>();
      bool flag = true;
      if (strings2.Count<string>() > 1)
      {
        List<ProcessTemplateValidatorMessage> validatorMessageList = errors;
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorMultipleProcessConfigurationDocuments();
        string file = str;
        string configurationDocuments = WitProcessTemplateValidatorHelpLinks.ValidatorMultipleProcessConfigurationDocuments;
        int? lineNumber = new int?();
        string helpLink = configurationDocuments;
        ProcessTemplateValidatorMessage validatorMessage5 = new ProcessTemplateValidatorMessage(message, file, lineNumber, helpLink);
        validatorMessageList.Add(validatorMessage5);
        return new WitProcessTemplateValidator.LoadedTemplateState();
      }
      if (strings2.Count<string>() < 1)
      {
        if (allowLegacyAgileSetting)
        {
          flag = false;
        }
        else
        {
          List<ProcessTemplateValidatorMessage> validatorMessageList = errors;
          string configurationNotFound1 = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorPathToProcessConfigurationNotFound();
          string file = str;
          string configurationNotFound2 = WitProcessTemplateValidatorHelpLinks.ValidatorPathToProcessConfigurationNotFound;
          int? lineNumber = new int?();
          string helpLink = configurationNotFound2;
          ProcessTemplateValidatorMessage validatorMessage6 = new ProcessTemplateValidatorMessage(configurationNotFound1, file, lineNumber, helpLink);
          validatorMessageList.Add(validatorMessage6);
          return new WitProcessTemplateValidator.LoadedTemplateState();
        }
      }
      List<XElement> featureList = new List<XElement>();
      WitProcessTemplateValidator.LoadedFile loadedFile1 = new WitProcessTemplateValidator.LoadedFile();
      WitProcessTemplateValidator.LoadedFile loadedFile2 = new WitProcessTemplateValidator.LoadedFile();
      WitProcessTemplateValidator.LoadedFile loadedFile3 = new WitProcessTemplateValidator.LoadedFile();
      if (flag)
      {
        WitProcessTemplateValidator.LoadDocuments(strings2, package, loadedFileList2, errors);
        if (errors.Any<ProcessTemplateValidatorMessage>())
          return new WitProcessTemplateValidator.LoadedTemplateState();
        loadedFile1 = loadedFileList2.Single<WitProcessTemplateValidator.LoadedFile>();
        XElement xelement2 = loadedFile1.Document.Root.Element((XName) "PortfolioBacklogs");
        IEnumerable<XElement> second = xelement2 != null ? xelement2.Elements() : Enumerable.Empty<XElement>();
        string[] nonFeatureElements = new string[5]
        {
          "TypeFields",
          "PortfolioBacklogs",
          "Weekends",
          "WorkItemColors",
          "Properties"
        };
        featureList = loadedFile1.Document.Root.Elements().Where<XElement>((Func<XElement, bool>) (e => !((IEnumerable<string>) nonFeatureElements).Contains<string>(e.Name.LocalName, (IEqualityComparer<string>) StringComparer.Ordinal))).Concat<XElement>(second).ToList<XElement>();
        this.EnsureTCMFeaturesAreIncluded(featureList);
        this.AddHardcodedFeatures(featureList);
        loadedFile3.IsValid = false;
        loadedFile2.IsValid = false;
      }
      else
      {
        loadedFile1.IsValid = false;
        IEnumerable<XElement> source = document2.Descendants((XName) "PROCESSCONFIGURATION");
        if (source.Count<XElement>() > 0)
        {
          IEnumerable<string> strings3 = source.Elements<XElement>((XName) "AgileConfiguration").Attributes((XName) "fileName").Select<XAttribute, string>((Func<XAttribute, string>) (a => a.Value));
          IEnumerable<string> strings4 = source.Elements<XElement>((XName) "CommonConfiguration").Attributes((XName) "fileName").Select<XAttribute, string>((Func<XAttribute, string>) (a => a.Value));
          List<WitProcessTemplateValidator.LoadedFile> loadedFileList3 = new List<WitProcessTemplateValidator.LoadedFile>();
          List<WitProcessTemplateValidator.LoadedFile> loadedFileList4 = new List<WitProcessTemplateValidator.LoadedFile>();
          if (strings3.Count<string>() != 1 || strings4.Count<string>() != 1)
          {
            errors.Add(new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorInvalidNumberOfAgileCommonConfigurationDocuments(), str));
            return new WitProcessTemplateValidator.LoadedTemplateState();
          }
          WitProcessTemplateValidator.LoadDocuments(strings3, package, loadedFileList3, errors);
          if (errors.Any<ProcessTemplateValidatorMessage>())
            return new WitProcessTemplateValidator.LoadedTemplateState();
          WitProcessTemplateValidator.LoadDocuments(strings4, package, loadedFileList4, errors);
          if (errors.Any<ProcessTemplateValidatorMessage>())
            return new WitProcessTemplateValidator.LoadedTemplateState();
          loadedFile2 = loadedFileList3.Single<WitProcessTemplateValidator.LoadedFile>();
          loadedFile3 = loadedFileList4.Single<WitProcessTemplateValidator.LoadedFile>();
        }
      }
      WitProcessTemplateValidator.LoadedFile loadedFile4 = func((IEnumerable<WitProcessTemplateValidator.LoadedFile>) documentList2, InternalSchemaType.Categories, (Action<object, ValidationEventArgs, WitProcessTemplateValidator.LoadedFile>) ((s, ea, lf) => errors.Add(new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorSchemaValidationFailure((object) ea.Message), lf.FileName, new int?(ea.Exception.LineNumber), WitProcessTemplateValidatorHelpLinks.ValidatorSchemaValidationFailure)))).FirstOrDefault<WitProcessTemplateValidator.LoadedFile>();
      Dictionary<string, XElement> dictionary = loadedFile4.Document.Descendants((XName) "CATEGORY").ToDictionary<XElement, string>((Func<XElement, string>) (e => e.Attribute((XName) "refname").Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      IEnumerable<string> paths2 = document2.Descendants((XName) "LINKTYPE").Attributes((XName) "fileName").Select<XAttribute, string>((Func<XAttribute, string>) (a => a.Value));
      List<WitProcessTemplateValidator.LoadedFile> loadedFileList5 = new List<WitProcessTemplateValidator.LoadedFile>();
      IProcessTemplatePackage package2 = package;
      List<WitProcessTemplateValidator.LoadedFile> documentList3 = loadedFileList5;
      List<ProcessTemplateValidatorMessage> errors2 = errors;
      WitProcessTemplateValidator.LoadDocuments(paths2, package2, documentList3, errors2);
      List<WitProcessTemplateValidator.LoadedFile> list2 = func((IEnumerable<WitProcessTemplateValidator.LoadedFile>) loadedFileList5, InternalSchemaType.WorkItemLinkType, (Action<object, ValidationEventArgs, WitProcessTemplateValidator.LoadedFile>) ((s, ea, lf) => errors.Add(new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorSchemaValidationFailure((object) ea.Message), lf.FileName, new int?(ea.Exception.LineNumber), WitProcessTemplateValidatorHelpLinks.ValidatorSchemaValidationFailure)))).ToList<WitProcessTemplateValidator.LoadedFile>();
      return new WitProcessTemplateValidator.LoadedTemplateState()
      {
        CategoryElements = dictionary,
        FeatureElements = featureList,
        LinkTypeDocuments = list2,
        ProcessConfiguration = loadedFile1,
        TypeDefinitions = list1,
        CategoriesDocument = loadedFile4,
        LegacyAgileProjectConfiguration = loadedFile2,
        LegacyCommonProjectConfiguration = loadedFile3
      };
    }

    private static ProcessTemplateValidatorMessage CreateTypeSchemaError(
      object sender,
      ValidationEventArgs ea,
      WitProcessTemplateValidator.LoadedFile document)
    {
      return sender is XAttribute xattribute && xattribute.Name.LocalName.Equals("refname", StringComparison.Ordinal) && !string.IsNullOrWhiteSpace(xattribute.Value) ? new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorWorkItemTypeRefNameInvalid((object) xattribute.Value), document.FileName, new int?(ea.Exception.LineNumber), WitProcessTemplateValidatorHelpLinks.ValidatorWorkItemTypeRefNameInvalid) : new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorSchemaValidationFailure((object) ea.Message), document.FileName, new int?(ea.Exception.LineNumber), WitProcessTemplateValidatorHelpLinks.ValidatorSchemaValidationFailure);
    }

    private void AddHardcodedFeatures(List<XElement> featureList) => featureList.AddRange(this.configuration.FeatureRequirements.Where<FeatureRequirement>((Func<FeatureRequirement, bool>) (fr => fr.HardCodedCategory != null)).Select<FeatureRequirement, XElement>((Func<FeatureRequirement, XElement>) (fr => this.CreateVirtualFeatureElement(fr.HardCodedCategory, fr.HardCodedCategory, fr.HardCodedMetastateMap.ToArray<KeyValuePair<string, string>>()))));

    private void EnsureTCMFeaturesAreIncluded(List<XElement> featureList)
    {
      if (featureList.SingleOrDefault<XElement>((Func<XElement, bool>) (e => e.Name.LocalName.Equals("TestPlanWorkItems", StringComparison.Ordinal))) == null)
        featureList.Add(this.CreateVirtualFeatureElement("TestPlanWorkItems", "Microsoft.TestPlanCategory", new KeyValuePair<string, string>("InProgress", "Active"), new KeyValuePair<string, string>("Complete", "Inactive")));
      if (featureList.SingleOrDefault<XElement>((Func<XElement, bool>) (e => e.Name.LocalName.Equals("TestSuiteWorkItems", StringComparison.Ordinal))) != null)
        return;
      featureList.Add(this.CreateVirtualFeatureElement("TestSuiteWorkItems", "Microsoft.TestSuiteCategory", new KeyValuePair<string, string>("Proposed", "In Planning"), new KeyValuePair<string, string>("InProgress", "In Progress"), new KeyValuePair<string, string>("Complete", "Completed")));
    }

    private InternalFieldType ParseFieldType(string value)
    {
      InternalFieldType result;
      Enum.TryParse<InternalFieldType>(value, true, out result);
      return result;
    }

    private XElement CreateVirtualFeatureElement(
      string featureName,
      string categoryName,
      params KeyValuePair<string, string>[] stateMap)
    {
      return new XElement((XName) featureName, new object[3]
      {
        (object) new XAttribute((XName) "category", (object) categoryName),
        (object) new XAttribute((XName) "virtualFeature", (object) true),
        (object) new XElement((XName) "States", (object) ((IEnumerable<KeyValuePair<string, string>>) stateMap).Select<KeyValuePair<string, string>, XElement>((Func<KeyValuePair<string, string>, XElement>) (kvp => new XElement((XName) "State", new object[2]
        {
          (object) new XAttribute((XName) "type", (object) kvp.Key),
          (object) new XAttribute((XName) "value", (object) kvp.Value)
        }))))
      });
    }

    private static void LoadDocuments(
      IEnumerable<string> paths,
      IProcessTemplatePackage package,
      List<WitProcessTemplateValidator.LoadedFile> documentList,
      List<ProcessTemplateValidatorMessage> errors)
    {
      foreach (string path in paths)
      {
        XDocument document;
        ProcessTemplateValidatorMessage validatorMessage = WitProcessTemplateValidator.SafeFileLoad(package, path, out document);
        if (validatorMessage != null)
          errors.Add(validatorMessage);
        else
          documentList.Add(new WitProcessTemplateValidator.LoadedFile()
          {
            Document = document,
            FileName = path,
            IsValid = true
          });
      }
    }

    private static ProcessTemplateValidatorMessage SafeFileLoad(
      IProcessTemplatePackage package,
      string path,
      out XDocument document)
    {
      document = (XDocument) null;
      try
      {
        document = package.GetDocument(path);
        return (ProcessTemplateValidatorMessage) null;
      }
      catch (ProcessTemplateFileNotFoundException ex)
      {
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorMissingFileInPackage();
        string file = path;
        string missingFileInPackage = WitProcessTemplateValidatorHelpLinks.ValidatorMissingFileInPackage;
        int? lineNumber = new int?();
        string helpLink = missingFileInPackage;
        return new ProcessTemplateValidatorMessage(message, file, lineNumber, helpLink);
      }
      catch (XmlException ex)
      {
        return new ProcessTemplateValidatorMessage(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorParsingXmlFails((object) ex.Message), path, new int?(ex.LineNumber), WitProcessTemplateValidatorHelpLinks.ValidatorParsingXmlFails);
      }
      catch (ProcessTemplateInvalidXmlException ex)
      {
        string message = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ValidatorParsingXmlFails((object) ex.Message);
        string file = path;
        XmlException xmlError = ex.XmlError;
        int? lineNumber = new int?(xmlError != null ? xmlError.LineNumber : 0);
        string validatorParsingXmlFails = WitProcessTemplateValidatorHelpLinks.ValidatorParsingXmlFails;
        return new ProcessTemplateValidatorMessage(message, file, lineNumber, validatorParsingXmlFails);
      }
    }

    private static ProcessTemplateValidatorResult FatalTopLevelError(
      ProcessTemplateValidatorMessage message)
    {
      return new ProcessTemplateValidatorResult((IEnumerable<ProcessTemplateValidatorMessage>) new ProcessTemplateValidatorMessage[1]
      {
        message
      });
    }

    private static int GetLineNumberForXmlItem(XObject node) => ((IXmlLineInfo) node).LineNumber;

    public struct LoadedTemplateState
    {
      public List<WitProcessTemplateValidator.LoadedFile> TypeDefinitions;
      public List<XElement> FeatureElements;
      public Dictionary<string, XElement> CategoryElements;
      public WitProcessTemplateValidator.LoadedFile ProcessConfiguration;
      public List<WitProcessTemplateValidator.LoadedFile> LinkTypeDocuments;
      public WitProcessTemplateValidator.LoadedFile CategoriesDocument;
      public WitProcessTemplateValidator.LoadedFile LegacyAgileProjectConfiguration;
      public WitProcessTemplateValidator.LoadedFile LegacyCommonProjectConfiguration;
    }

    public struct LoadedFile
    {
      public string FileName { get; internal set; }

      public XDocument Document { get; internal set; }

      public bool IsValid { get; internal set; }
    }
  }
}
