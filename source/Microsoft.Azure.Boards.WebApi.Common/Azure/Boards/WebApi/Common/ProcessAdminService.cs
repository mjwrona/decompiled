// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.ProcessAdminService
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Promote;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormExtensions;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Process;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessValidator;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.Azure.Boards.WebApi.Common
{
  public class ProcessAdminService : IProcessAdminService, IVssFrameworkService
  {
    private const string c_Area = "ProcessService";
    private const string c_Layer = "IVssFrameworkService";
    private static readonly HashSet<string> s_hiddenFields = new HashSet<string>()
    {
      "System.AreaLevel1",
      "System.AreaLevel2",
      "System.AreaLevel3",
      "System.AreaLevel4",
      "System.AreaLevel5",
      "System.AreaLevel6",
      "System.AreaLevel7",
      "System.AttachedFiles",
      "System.AuthorizedAs",
      "System.AuthorizedDate",
      "System.BISLinks",
      "System.BoardColumn",
      "System.BoardColumnDone",
      "System.BoardLane",
      "System.CommentCount",
      "System.InAdminOnlyTreeFlag",
      "System.InDeletedTreeFlag",
      "System.IsDeleted",
      "System.IterationLevel1",
      "System.IterationLevel2",
      "System.IterationLevel3",
      "System.IterationLevel4",
      "System.IterationLevel5",
      "System.IterationLevel6",
      "System.IterationLevel7",
      "System.LinkedFiles",
      "System.Links.LinkType",
      "System.NodeType",
      "System.PersonId",
      "System.ProjectId",
      "System.RelatedLinks",
      "System.RemoteLinkCount",
      "System.Tags",
      "System.TFServer",
      "System.Watermark",
      "System.WorkItemForm",
      "System.WorkItemFormId"
    };

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public IEnumerable<ProcessDescriptorModel> GetProcesses(IVssRequestContext requestContext) => (IEnumerable<ProcessDescriptorModel>) requestContext.TraceBlock<List<ProcessDescriptorModel>>(1300501, 1300502, "ProcessService", "IVssFrameworkService", nameof (GetProcesses), (Func<List<ProcessDescriptorModel>>) (() =>
    {
      List<ProcessDescriptorModel> source1 = new List<ProcessDescriptorModel>();
      ITeamFoundationProcessService service1 = requestContext.GetService<ITeamFoundationProcessService>();
      IReadOnlyCollection<ProcessDescriptor> processDescriptors = service1.GetProcessDescriptors(requestContext);
      ISet<Guid> disabledProcessTypeIds = service1.GetDisabledProcessTypeIds(requestContext);
      Guid defaultProcessTypeId = service1.GetDefaultProcessTypeId(requestContext);
      IProjectService service2 = requestContext.GetService<IProjectService>();
      ILegacyProjectPropertiesReaderService service3 = requestContext.GetService<ILegacyProjectPropertiesReaderService>();
      IVssRequestContext requestContext1 = requestContext.Elevate();
      IEnumerable<ProjectInfo> projects = service2.GetProjects(requestContext1);
      Dictionary<Guid, IEnumerable<ProjectInfo>> dictionary = service3.PopulateProperties(projects, requestContext.Elevate(), ProcessTemplateIdPropertyNames.ProcessTemplateType).ToList<ProjectInfo>().Where<ProjectInfo>((Func<ProjectInfo, bool>) (p => p.Properties.Any<ProjectProperty>() && Guid.TryParse((string) p.Properties[0].Value, out Guid _))).GroupBy<ProjectInfo, Guid>((Func<ProjectInfo, Guid>) (p => p.GetProjectTemplateTypeId())).ToDictionary<IGrouping<Guid, ProjectInfo>, Guid, IEnumerable<ProjectInfo>>((Func<IGrouping<Guid, ProjectInfo>, Guid>) (group => group.Key), (Func<IGrouping<Guid, ProjectInfo>, IEnumerable<ProjectInfo>>) (group => group.AsEnumerable<ProjectInfo>()));
      foreach (ProcessDescriptor descriptor in (IEnumerable<ProcessDescriptor>) processDescriptors)
      {
        bool flag1 = service1.HasProcessPermission(requestContext, 1, descriptor, false);
        bool flag2 = service1.HasProcessPermission(requestContext, 2, descriptor);
        bool flag3 = service1.HasProcessPermission(requestContext, 4, descriptor, false);
        IEnumerable<ProjectInfo> source2 = (IEnumerable<ProjectInfo>) null;
        int num = 0;
        if (dictionary.TryGetValue(descriptor.TypeId, out source2))
          num = source2.Count<ProjectInfo>();
        source1.Add(new ProcessDescriptorModel()
        {
          TemplateTypeId = descriptor.TypeId,
          Id = descriptor.RowId,
          Name = descriptor.Name,
          ReferenceName = descriptor.ReferenceName,
          Version = string.Format("{0}.{1}", (object) descriptor.Version.Major, (object) descriptor.Version.Minor),
          Description = descriptor.Description,
          IsDefault = descriptor.TypeId == defaultProcessTypeId,
          IsEnabled = !disabledProcessTypeIds.Contains(descriptor.TypeId),
          IsSystemTemplate = descriptor.Scope == ProcessScope.Deployment,
          IsInherited = descriptor.IsDerived,
          Status = descriptor.ProcessStatus,
          SubscribedProjectCount = num,
          Inherits = descriptor.Inherits,
          DerivedProcessCount = 0,
          EditPermission = flag1,
          DeletePermission = flag2,
          CreatePermission = flag3
        });
      }
      source1.Sort();
      if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext))
      {
        for (int index = 0; index < source1.Count; ++index)
        {
          ProcessDescriptorModel currentIteratedProcess = source1[index];
          if (!(currentIteratedProcess.Inherits == Guid.Empty))
          {
            ProcessDescriptorModel processDescriptorModel = source1.First<ProcessDescriptorModel>((Func<ProcessDescriptorModel, bool>) (e => e.TemplateTypeId == currentIteratedProcess.Inherits));
            ++processDescriptorModel.DerivedProcessCount;
            int num = source1.IndexOf(processDescriptorModel);
            source1.RemoveAt(index);
            source1.Insert(num + processDescriptorModel.DerivedProcessCount, currentIteratedProcess);
          }
        }
      }
      CustomerIntelligenceService service4 = requestContext.GetService<CustomerIntelligenceService>();
      service4.Publish(requestContext, "ProcessService", "Get", "CustomProcessCount", (double) source1.Where<ProcessDescriptorModel>((Func<ProcessDescriptorModel, bool>) (p => !p.IsSystemTemplate)).Count<ProcessDescriptorModel>());
      service4.Publish(requestContext, "ProcessService", "Get", "ProcessToProjectCount", string.Join(",", source1.Select<ProcessDescriptorModel, string>((Func<ProcessDescriptorModel, string>) (p => string.Format("{0}:{1} : {2}", (object) p.TemplateTypeId, (object) p.Name, (object) p.SubscribedProjectCount)))));
      return source1;
    }));

    public void DeleteProcess(IVssRequestContext requestContext, Guid templateTypeId)
    {
      requestContext.TraceEnter(1300504, "ProcessService", "IVssFrameworkService", nameof (DeleteProcess));
      try
      {
        requestContext.Trace(1300505, TraceLevel.Info, "ProcessService", "IVssFrameworkService", string.Format("Delete Process with processTeamplate id: {0}", (object) templateTypeId));
        requestContext.GetService<ITeamFoundationProcessService>().DeleteProcess(requestContext, templateTypeId);
      }
      finally
      {
        requestContext.TraceLeave(1300550, "ProcessService", "IVssFrameworkService", nameof (DeleteProcess));
      }
    }

    public ProcessUpdateResultModel UpdateProcess(
      IVssRequestContext requestContext,
      Stream content,
      bool bypassWarnings)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Stream>(content, nameof (content));
      return requestContext.TraceBlock<ProcessUpdateResultModel>(1300551, 1300600, "ProcessService", "IVssFrameworkService", nameof (UpdateProcess), (Func<ProcessUpdateResultModel>) (() =>
      {
        requestContext.GetService<IProcessTemplateValidatorService>().ValidateTemplateFileSizeLimit(requestContext, content);
        content = ZipArchiveProcessTemplatePackage.FixupZipSingleRootFolder(content);
        Guid typeId;
        using (ZipArchiveProcessTemplatePackage processTemplatePackage = new ZipArchiveProcessTemplatePackage(content, true, true))
          typeId = processTemplatePackage.TypeId;
        content.Seek(0L, SeekOrigin.Begin);
        ProcessDescriptor descriptor;
        if (requestContext.GetService<ITeamFoundationProcessService>().TryGetProcessDescriptor(requestContext, typeId, out descriptor))
        {
          if (descriptor.Scope == ProcessScope.Deployment)
            throw new ProcessServiceDeploymentTemplateUpdateBlockedException();
          if (descriptor.ProcessStatus == ProcessStatus.Updating)
            throw new ProcessServiceUpdateBlockedBecauseProcessIsUpdatingException(descriptor.Name);
          if (descriptor.ProcessStatus == ProcessStatus.Deleting)
            throw new ProcessServiceUpdateBlockedBecauseProcessIsDeletingException(descriptor.Name);
        }
        ProcessTemplateValidatorResult templateValidatorResult = this.Validate(requestContext, typeId, content);
        ProcessUpdateResultModel currentResult = new ProcessUpdateResultModel()
        {
          ProcessTemplateValidatorResult = templateValidatorResult,
          TemplateTypeId = typeId
        };
        bool flag = !templateValidatorResult.Errors.Any<ProcessTemplateValidatorMessage>() && (!templateValidatorResult.ConfirmationsNeeded.Any<ProcessTemplateValidatorMessage>() || bypassWarnings);
        if (flag)
        {
          this.UploadProcessInternal(requestContext, typeId, content, templateValidatorResult.Details.OfType<ProcessTemplateFieldRenameData>(), currentResult);
          requestContext.Trace(1300553, TraceLevel.Info, "ProcessService", "IVssFrameworkService", string.Format("PromoteJobId :{0}", (object) currentResult.PromoteJobId));
        }
        string str = requestContext.ExecutionEnvironment.IsHostedDeployment ? "https://go.microsoft.com/fwlink?LinkID=613784" : "https://go.microsoft.com/fwlink?LinkID=852639";
        foreach (ProcessTemplateValidatorMessage error in (IEnumerable<ProcessTemplateValidatorMessage>) currentResult.ProcessTemplateValidatorResult.Errors)
        {
          if (error.HelpLink == null)
            error.HelpLink = str;
        }
        foreach (ProcessTemplateValidatorMessage validatorMessage in (IEnumerable<ProcessTemplateValidatorMessage>) currentResult.ProcessTemplateValidatorResult.ConfirmationsNeeded)
        {
          if (validatorMessage.HelpLink == null)
            validatorMessage.HelpLink = str;
        }
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "ProcessService", "Update", "SuccessRate", string.Format("{0} : {1}", (object) typeId, (object) flag));
        return currentResult;
      }));
    }

    public ProcessUpdateProgressModel MonitorUpdateProgress(
      IVssRequestContext requestContext,
      Guid promoteJobId)
    {
      TeamFoundationJobDefinition foundationJobDefinition = requestContext.GetService<ITeamFoundationJobService>().QueryJobDefinition(requestContext, promoteJobId);
      if (foundationJobDefinition == null)
        return (ProcessUpdateProgressModel) null;
      TeamProjectPromoteJobData projectPromoteJobData = TeamFoundationSerializationUtility.Deserialize<TeamProjectPromoteJobData>(foundationJobDefinition.Data);
      return new ProcessUpdateProgressModel()
      {
        IsSuccessful = projectPromoteJobData.IsSuccessful(),
        TotalProjectsCount = projectPromoteJobData.Projects.Count,
        RemainingRetries = projectPromoteJobData.RemainingRetries,
        ProcessedProjectsCount = projectPromoteJobData.Projects.Count - projectPromoteJobData.Projects.Where<PromoteProjectInfo>((Func<PromoteProjectInfo, bool>) (p => p.State == ProjectPromoteState.NotProcessed)).Count<PromoteProjectInfo>(),
        FailedProject = projectPromoteJobData.Projects.FirstOrDefault<PromoteProjectInfo>((Func<PromoteProjectInfo, bool>) (p => p.State == ProjectPromoteState.Failed)),
        Complete = projectPromoteJobData.Projects.Where<PromoteProjectInfo>((Func<PromoteProjectInfo, bool>) (p => p.State == ProjectPromoteState.Promoted)).Count<PromoteProjectInfo>(),
        Remaining = projectPromoteJobData.Projects.Where<PromoteProjectInfo>((Func<PromoteProjectInfo, bool>) (p => p.State == ProjectPromoteState.NotProcessed)).Count<PromoteProjectInfo>()
      };
    }

    public bool FillFieldDescription(
      ProcessFieldResult fieldResult,
      Field fieldObj,
      UsageProperties usageProperties)
    {
      bool flag = false;
      if (!string.IsNullOrWhiteSpace(fieldResult.Description))
      {
        if (string.IsNullOrWhiteSpace(fieldObj.Description))
          fieldObj.Description = fieldResult.Description;
        else if (fieldObj.Description != fieldResult.Description)
        {
          usageProperties.HelpText = fieldResult.Description;
          flag = true;
        }
      }
      return flag;
    }

    public bool FillFieldUsageProperties(
      IVssRequestContext requestContext,
      ProcessFieldResult fieldResult,
      IDictionary<string, WorkItemFieldRule> fieldRules,
      Field fieldObj,
      UsageProperties usageProperties)
    {
      bool flag1 = false;
      WorkItemFieldRule workItemFieldRule;
      if (fieldRules.TryGetValue(fieldResult.ReferenceName, out workItemFieldRule) && workItemFieldRule.SubRules != null && ((IEnumerable<WorkItemRule>) workItemFieldRule.SubRules).Any<WorkItemRule>())
      {
        IEnumerable<WorkItemRule> unconditionalRules = workItemFieldRule.GetAllUnconditionalRules();
        IdentityDefaultRule identityDefaultRule = unconditionalRules.OfType<IdentityDefaultRule>().FirstOrDefault<IdentityDefaultRule>();
        DefaultRule defaultRule = unconditionalRules.OfType<DefaultRule>().FirstOrDefault<DefaultRule>();
        if (identityDefaultRule != null)
        {
          UsageProperties usageProperties1 = usageProperties;
          IdentityDefault identityDefault = new IdentityDefault();
          identityDefault.Vsid = identityDefaultRule.Vsid;
          identityDefault.Value = identityDefaultRule.Value;
          usageProperties1.Default = (Default) identityDefault;
          if (defaultRule.ValueFrom == RuleValueFrom.CurrentUser)
            usageProperties.Default = new Default()
            {
              Value = "$currentUser"
            };
        }
        else if (defaultRule != null)
        {
          if (defaultRule.ValueFrom == RuleValueFrom.Clock)
            usageProperties.Default = new Default()
            {
              Value = "$currentDateTime"
            };
          else if (defaultRule.ValueFrom == RuleValueFrom.CurrentUser)
            usageProperties.Default = new Default()
            {
              Value = "$currentUser"
            };
          else
            usageProperties.Default = new Default()
            {
              Value = defaultRule.Value
            };
          flag1 = true;
        }
        if (unconditionalRules.OfType<RequiredRule>().Any<RequiredRule>())
        {
          usageProperties.IsRequired = true;
          flag1 = true;
        }
        if (unconditionalRules.OfType<ReadOnlyRule>().Any<ReadOnlyRule>())
        {
          usageProperties.IsReadOnly = true;
          flag1 = true;
        }
        if (unconditionalRules.OfType<AllowedValuesRule>().Any<AllowedValuesRule>())
        {
          AllowedValuesRule allowedValuesRule = unconditionalRules.OfType<AllowedValuesRule>().First<AllowedValuesRule>();
          if (allowedValuesRule.Values != null && allowedValuesRule.Values.Any<string>())
          {
            usageProperties.AllowedValues = allowedValuesRule.Values.ToArray<string>();
            flag1 = true;
          }
          if (fieldObj.Type == "Identity")
          {
            bool flag2 = allowedValuesRule.Sets != null && ((IEnumerable<ConstantSetReference>) allowedValuesRule.Sets).Any<ConstantSetReference>() && ((IEnumerable<ConstantSetReference>) allowedValuesRule.Sets).First<ConstantSetReference>().Id == -1;
            usageProperties.AllowGroups = flag2;
            flag1 = true;
          }
        }
      }
      IReadOnlyCollection<string> values;
      if ((usageProperties.AllowedValues == null || !((IEnumerable<string>) usageProperties.AllowedValues).Any<string>()) && OOBFieldValues.TryGetAllowedValues(requestContext, fieldObj.Id, out values))
      {
        usageProperties.AllowedValues = values.ToArray<string>();
        flag1 = true;
      }
      return flag1;
    }

    public void PopulateWorkItemTypeFields(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor,
      ComposedWorkItemType composedType,
      IDictionary<string, Field> fields)
    {
      bool flag1 = WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext);
      Dictionary<string, WorkItemFieldRule> dictionary = composedType.GetFieldRules(requestContext).ToDictionary<WorkItemFieldRule, string>((Func<WorkItemFieldRule, string>) (rule => rule.Field));
      HashSet<string> stringSet1 = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      IReadOnlyCollection<WorkItemFieldRule> rules = (IReadOnlyCollection<WorkItemFieldRule>) new List<WorkItemFieldRule>();
      if (composedType.IsDerived)
      {
        foreach (string str in composedType.ParentWorkItemType.FieldDefinitions.Select<ProcessFieldDefinition, string>((Func<ProcessFieldDefinition, string>) (f => f.ReferenceName)))
          stringSet1.Add(str);
        IWorkItemRulesService service = requestContext.GetService<IWorkItemRulesService>();
        if (flag1 && !processDescriptor.IsCustom && !service.TryGetOutOfBoxRules(requestContext, processDescriptor, composedType.ParentTypeRefName, out rules))
        {
          rules = (IReadOnlyCollection<WorkItemFieldRule>) new List<WorkItemFieldRule>();
          CustomerIntelligenceData properties = new CustomerIntelligenceData();
          properties.Add("Composed Type Name", composedType.Name);
          properties.Add("Composed Type ReferenceName", composedType.ReferenceName);
          properties.Add("Composed Type ParentTypeRefName", composedType.ParentTypeRefName);
          properties.Add("Composed Type IsCustomType", composedType.IsCustomType);
          properties.Add("Composed Type IsDerived", composedType.IsDerived);
          requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (ProcessAdminService), nameof (PopulateWorkItemTypeFields), properties);
        }
      }
      List<ProcessFieldResult> list = composedType.GetLegacyFields(requestContext).ToList<ProcessFieldResult>();
      if (flag1)
      {
        HashSet<string> stringSet2 = new HashSet<string>(list.Select<ProcessFieldResult, string>((Func<ProcessFieldResult, string>) (f => f.ReferenceName)).Distinct<string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
        foreach (ProcessFieldDefinition boxFieldDefinition in (IEnumerable<ProcessFieldDefinition>) requestContext.GetService<IProcessFieldService>().GetAllOutOfBoxFieldDefinitions(requestContext))
        {
          if (!stringSet2.Contains(boxFieldDefinition.ReferenceName))
          {
            list.Add(boxFieldDefinition.ConvertToFieldResult());
            stringSet2.Add(boxFieldDefinition.ReferenceName);
          }
        }
      }
      IProcessWorkItemTypeService service1 = requestContext.GetService<IProcessWorkItemTypeService>();
      foreach (ProcessFieldResult fieldResult in list)
      {
        Field fieldObj;
        if (!fields.TryGetValue(fieldResult.ReferenceName, out fieldObj))
        {
          string str = fieldResult.IsIdentity ? "Identity" : fieldResult.Type.ToString();
          fieldObj = new Field()
          {
            Id = fieldResult.ReferenceName,
            Name = fieldResult.Name,
            Type = str,
            Description = fieldResult.Description,
            Usages = new List<FieldUsage>(),
            PickListId = fieldResult.PickListId
          };
          fields[fieldResult.ReferenceName] = fieldObj;
        }
        FieldUsage fieldUsage = new FieldUsage()
        {
          WorkItemTypeId = composedType.ReferenceName,
          IsInherited = stringSet1.Contains(fieldResult.ReferenceName),
          IsSystem = fieldResult.IsSystem,
          IsBehaviorField = fieldResult.IsBehaviorField
        };
        UsageProperties usageProperties = new UsageProperties();
        bool flag2 = !ValidationMethods.IsSystemReferenceName(fieldResult.ReferenceName);
        if (flag1)
        {
          FieldEntry fieldEntry;
          if (requestContext.WitContext().FieldDictionary.TryGetField(fieldResult.ReferenceName, out fieldEntry))
          {
            fieldUsage.CanEditFieldProperties = service1.CanEditFieldPropertiesWithinScopeOfWorkItemType(requestContext, processDescriptor, fieldEntry, composedType);
            if (composedType.IsDerived)
            {
              WorkItemFieldRule workItemFieldRule = rules.FirstOrDefault<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (r => r.FieldId == fieldEntry.FieldId));
              usageProperties.IsRequiredInParent = workItemFieldRule != null && workItemFieldRule.GetUnconditionalRules<RequiredRule>().Any<RequiredRule>();
              usageProperties.IsInheritedIdentity = workItemFieldRule != null && fieldEntry.IsIdentity && workItemFieldRule.GetUnconditionalRules<AllowedValuesRule>().Any<AllowedValuesRule>();
            }
            else if (fieldEntry.IsIdentity)
              usageProperties.IsInheritedIdentity = true;
          }
        }
        if (this.FillFieldDescription(fieldResult, fieldObj, usageProperties) | this.FillFieldUsageProperties(requestContext, fieldResult, (IDictionary<string, WorkItemFieldRule>) dictionary, fieldObj, usageProperties))
        {
          fieldUsage.Properties = usageProperties;
          flag2 = true;
        }
        if (flag2)
          fieldObj.Usages.Add(fieldUsage);
      }
    }

    public ProcessFieldUsageInfo GetProcessFieldUsages(
      IVssRequestContext requestContext,
      Guid processTypeId)
    {
      return requestContext.TraceBlock<ProcessFieldUsageInfo>(1300574, 1300575, "ProcessService", "IVssFrameworkService", nameof (GetProcessFieldUsages), (Func<ProcessFieldUsageInfo>) (() =>
      {
        IProcessFieldService service = requestContext.GetService<IProcessFieldService>();
        IEnumerable<FieldEntry> allFields = requestContext.GetService<WorkItemTrackingFieldService>().GetAllFields(requestContext);
        HashSet<string> hiddenFields;
        if (WorkItemTrackingFeatureFlags.IsProcessAdminServiceFieldPrefixLegacyBehaviorEnabled(requestContext))
        {
          IReadOnlyCollection<ProcessFieldDefinition> outOfBoxFields = service.GetAllOutOfBoxFieldDefinitions(requestContext);
          IEnumerable<FieldEntry> source = allFields.Where<FieldEntry>((Func<FieldEntry, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.StartsWith(f.ReferenceName, ProcessFieldService.SystemFieldPrefix) || TFStringComparer.WorkItemFieldReferenceName.StartsWith(f.ReferenceName, ProcessFieldService.MicrosoftVSTSFieldPrefix)));
          hiddenFields = source != null ? source.Where<FieldEntry>((Func<FieldEntry, bool>) (sf => !outOfBoxFields.Any<ProcessFieldDefinition>((Func<ProcessFieldDefinition, bool>) (oobf => TFStringComparer.WorkItemFieldReferenceName.Equals(oobf.ReferenceName, sf.ReferenceName))))).ToHashSet<FieldEntry, string>((Func<FieldEntry, string>) (item => item.ReferenceName)) : (HashSet<string>) null;
        }
        else
          hiddenFields = ProcessAdminService.s_hiddenFields;
        Dictionary<string, Field> dictionary = allFields.Where<FieldEntry>((Func<FieldEntry, bool>) (af => !hiddenFields.Contains(af.ReferenceName))).Where<FieldEntry>((Func<FieldEntry, bool>) (f => (f.Usage & InternalFieldUsages.WorkItemTypeExtension) == InternalFieldUsages.None)).ToList<FieldEntry>().ToDictionary<FieldEntry, string, Field>((Func<FieldEntry, string>) (f => f.ReferenceName), (Func<FieldEntry, Field>) (f =>
        {
          string str = f.IsIdentity ? "Identity" : f.FieldType.ToString();
          return new Field()
          {
            Id = f.ReferenceName,
            Name = f.Name,
            Type = str,
            Description = f.Description,
            Usages = new List<FieldUsage>(),
            PickListId = f.PickListId
          };
        }), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
        List<WorkItemType> source1 = new List<WorkItemType>();
        IReadOnlyCollection<ComposedWorkItemType> allWorkItemTypes = requestContext.GetService<ProcessWorkItemTypeService>().GetAllWorkItemTypes(requestContext, processTypeId, true, false);
        ProcessDescriptor processDescriptor = requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, processTypeId);
        foreach (ComposedWorkItemType composedType in (IEnumerable<ComposedWorkItemType>) allWorkItemTypes)
        {
          if (!((IEnumerable<string>) Customization.WorkItemTypesBlockedFromCustomization).Contains<string>(composedType.ReferenceName, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeReferenceName))
          {
            WorkItemType workItemType = new WorkItemType()
            {
              Id = composedType.ReferenceName,
              Name = composedType.Name,
              Description = composedType.Description,
              ParentWorkItemTypeId = composedType.ParentTypeRefName,
              Color = composedType.Color,
              Layout = composedType.GetFormLayout(requestContext),
              IsCustomType = composedType.IsCustomType,
              IsDisabled = composedType.IsDisabled
            };
            source1.Add(workItemType);
            this.PopulateWorkItemTypeFields(requestContext, processDescriptor, composedType, (IDictionary<string, Field>) dictionary);
          }
        }
        StringComparer serverStringComparer = requestContext.WitContext().ServerSettings.ServerStringComparer;
        return new ProcessFieldUsageInfo()
        {
          Fields = (IEnumerable<Field>) dictionary.Values.OrderBy<Field, string>((Func<Field, string>) (x => x.Name), (IComparer<string>) serverStringComparer).ToList<Field>(),
          WorkItemTypes = (IEnumerable<WorkItemType>) source1.OrderBy<WorkItemType, string>((Func<WorkItemType, string>) (x => x.Name), (IComparer<string>) serverStringComparer).ToList<WorkItemType>()
        };
      }));
    }

    private bool HasSubscribedProjects(IVssRequestContext collectionContext, Guid templateTypeId)
    {
      IProjectService service1 = collectionContext.GetService<IProjectService>();
      ILegacyProjectPropertiesReaderService service2 = collectionContext.GetService<ILegacyProjectPropertiesReaderService>();
      IVssRequestContext requestContext = collectionContext.Elevate();
      IEnumerable<ProjectInfo> projects = service1.GetProjects(requestContext);
      IEnumerable<ProjectInfo> list = (IEnumerable<ProjectInfo>) service2.PopulateProperties(projects, collectionContext.Elevate(), ProcessTemplateIdPropertyNames.ProcessTemplateType).ToList<ProjectInfo>();
      ISet<ProjectState> projectStateSet = (ISet<ProjectState>) new HashSet<ProjectState>()
      {
        ProjectState.CreatePending,
        ProjectState.New,
        ProjectState.WellFormed
      };
      if (list != null)
      {
        foreach (ProjectInfo project in list)
        {
          if (projectStateSet.Contains(project.State) && project.GetProjectTemplateTypeId() == templateTypeId)
            return true;
        }
      }
      return false;
    }

    private void UploadProcessInternal(
      IVssRequestContext requestContext,
      Guid updatedTemplateTypeId,
      Stream content,
      IEnumerable<ProcessTemplateFieldRenameData> renamedFields,
      ProcessUpdateResultModel currentResult)
    {
      Guid templateToPromote = Guid.Empty;
      IEnumerable<Guid> guids1 = Enumerable.Empty<Guid>();
      Guid guid1 = Guid.Empty;
      ITeamFoundationProcessService service = requestContext.GetService<ITeamFoundationProcessService>();
      try
      {
        content = ProcessAdminService.UpdateTemplateContentWitDefaultPlugins(content, requestContext.ExecutionEnvironment.IsHostedDeployment);
        ISet<Guid> guidSet = (ISet<Guid>) new HashSet<Guid>()
        {
          updatedTemplateTypeId
        };
        if (renamedFields.Any<ProcessTemplateFieldRenameData>())
        {
          foreach (ProcessTemplateFieldRenameData renamedField in renamedFields)
          {
            foreach (Guid guid2 in renamedField.TemplatesToUpdate)
              guidSet.Add(guid2);
          }
        }
        requestContext.Trace(1300571, TraceLevel.Info, "ProcessService", "IVssFrameworkService", string.Join(",", guidSet.Select<Guid, string>((Func<Guid, string>) (t => t.ToString()))));
        bool isExisitingTemplate = service.TryGetProcessDescriptor(requestContext, updatedTemplateTypeId, out ProcessDescriptor _);
        guids1 = guidSet.Where<Guid>((Func<Guid, bool>) (t => t != updatedTemplateTypeId || t == updatedTemplateTypeId & isExisitingTemplate));
        ProcessTemplateValidatorResult templateValidatorResult = this.Validate(requestContext, updatedTemplateTypeId, content);
        if ((!templateValidatorResult.Errors.Any<ProcessTemplateValidatorMessage>() ? 1 : (templateValidatorResult.ConfirmationsNeeded.Count != currentResult.ProcessTemplateValidatorResult.ConfirmationsNeeded.Count ? 1 : 0)) == 0)
        {
          currentResult.ProcessTemplateValidatorResult = templateValidatorResult;
        }
        else
        {
          bool flag = WorkItemTrackingFeatureFlags.AreXMLProcessesEnabled(requestContext);
          if (flag)
          {
            templateToPromote = this.PickTemplateToPromote(requestContext, updatedTemplateTypeId, guidSet);
            requestContext.Trace(1300572, TraceLevel.Info, "ProcessService", "IVssFrameworkService", string.Format("Template that will be promoted: {0}", (object) templateToPromote));
          }
          if (guids1.Any<Guid>())
            this.UpdateProcessStatuses(requestContext, service, guids1, ProcessStatus.Updating, true);
          foreach (Guid affectedTemplate in (IEnumerable<Guid>) guidSet)
          {
            if (affectedTemplate == updatedTemplateTypeId)
              service.CreateOrUpdateLegacyProcess(requestContext, content);
            else
              this.UpdateAffectedTemplateFieldsContent(requestContext, renamedFields, affectedTemplate);
          }
          if (!flag)
            return;
          if (templateToPromote == updatedTemplateTypeId)
            guid1 = this.QueuePromote(requestContext, templateToPromote);
          else if (templateToPromote != Guid.Empty)
            this.RenameFields(requestContext, renamedFields);
          currentResult.PromoteJobId = guid1;
        }
      }
      finally
      {
        IEnumerable<Guid> guids2 = !(templateToPromote == updatedTemplateTypeId) || !(guid1 != Guid.Empty) ? guids1 : guids1.Where<Guid>((Func<Guid, bool>) (t => t != templateToPromote));
        if (guids2.Any<Guid>())
          this.UpdateProcessStatuses(requestContext, service, guids2, ProcessStatus.Ready, false);
      }
    }

    private void UpdateProcessStatuses(
      IVssRequestContext requestContext,
      ITeamFoundationProcessService processService,
      IEnumerable<Guid> processTypeIds,
      ProcessStatus processState,
      bool throwProcessServiceExceptions)
    {
      try
      {
        processService.UpdateProcessStatuses(requestContext, processTypeIds, processState);
      }
      catch (ProcessServiceException ex)
      {
        requestContext.TraceException(1300573, nameof (ProcessAdminService), nameof (UpdateProcessStatuses), (Exception) ex);
        if (!throwProcessServiceExceptions)
          return;
        throw;
      }
    }

    private static Stream UpdateTemplateContentWitDefaultPlugins(Stream content, bool isHosted)
    {
      MemoryStream destination = new MemoryStream();
      content.CopyTo((Stream) destination);
      content.Close();
      using (ZipArchiveProcessTemplatePackage processTemplatePackage = new ZipArchiveProcessTemplatePackage((Stream) destination, true, true, true))
        processTemplatePackage.ReplacePluginsWithDefaults(isHosted: new bool?(isHosted));
      destination.Seek(0L, SeekOrigin.Begin);
      return (Stream) destination;
    }

    private void UpdateAffectedTemplateFieldsContent(
      IVssRequestContext collectionContext,
      IEnumerable<ProcessTemplateFieldRenameData> renamedFields,
      Guid affectedTemplate)
    {
      ITeamFoundationProcessService service = collectionContext.GetService<ITeamFoundationProcessService>();
      ProcessFieldDefinition[] array = renamedFields.Where<ProcessTemplateFieldRenameData>((Func<ProcessTemplateFieldRenameData, bool>) (c => c.TemplatesToUpdate.Contains<Guid>(affectedTemplate))).Select<ProcessTemplateFieldRenameData, ProcessFieldDefinition>((Func<ProcessTemplateFieldRenameData, ProcessFieldDefinition>) (c => new ProcessFieldDefinition()
      {
        Name = c.NewName,
        ReferenceName = c.RefName
      })).ToArray<ProcessFieldDefinition>();
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (Stream processPackageContent = service.GetLegacyProcessPackageContent(collectionContext, affectedTemplate))
          processPackageContent.CopyTo((Stream) memoryStream);
        memoryStream.Seek(0L, SeekOrigin.Begin);
        bool flag;
        using (ZipArchiveProcessTemplatePackage package = new ZipArchiveProcessTemplatePackage((Stream) memoryStream, true, true, true))
        {
          Lazy<IEnumerable<Contribution>> formContributions = new Lazy<IEnumerable<Contribution>>((Func<IEnumerable<Contribution>>) (() => FormExtensionsUtility.GetFilteredContributions(collectionContext)));
          flag = new LegacyWorkItemTrackingProcessPackage(collectionContext, (IProcessTemplatePackage) package, formContributions, (Layout) null).RenameFields((IEnumerable<ProcessFieldDefinition>) array);
        }
        if (!flag)
          return;
        memoryStream.Seek(0L, SeekOrigin.Begin);
        service.CreateOrUpdateLegacyProcess(collectionContext, (Stream) memoryStream);
      }
    }

    private void RenameFields(
      IVssRequestContext collectionContext,
      IEnumerable<ProcessTemplateFieldRenameData> renamedFields)
    {
      collectionContext.GetService<TeamFoundationProjectPromoteService>().RenameFields(collectionContext.Elevate(), (IEnumerable<KeyValuePair<string, string>>) renamedFields.Select<ProcessTemplateFieldRenameData, KeyValuePair<string, string>>((Func<ProcessTemplateFieldRenameData, KeyValuePair<string, string>>) (rf => new KeyValuePair<string, string>(rf.RefName, rf.NewName))).ToList<KeyValuePair<string, string>>());
    }

    private Guid PickTemplateToPromote(
      IVssRequestContext collectionContext,
      Guid updatedTemplateTypeId,
      ISet<Guid> affectedTemplates)
    {
      Guid promote = Guid.Empty;
      if (this.HasSubscribedProjects(collectionContext, updatedTemplateTypeId))
        return updatedTemplateTypeId;
      foreach (Guid affectedTemplate in (IEnumerable<Guid>) affectedTemplates)
      {
        if (affectedTemplate != updatedTemplateTypeId && this.HasSubscribedProjects(collectionContext, affectedTemplate))
        {
          promote = affectedTemplate;
          break;
        }
      }
      return promote;
    }

    private Guid QueuePromote(IVssRequestContext collectionContext, Guid templateId)
    {
      collectionContext.GetService<TeamFoundationProjectPromoteService>().QueuePromoteJob(collectionContext, templateId, false, new Guid?());
      return templateId;
    }

    public virtual ProcessTemplateValidatorResult Validate(
      IVssRequestContext collectionContext,
      Guid templateTypeId,
      Stream content)
    {
      ProcessTemplateValidatorResult templateValidatorResult = collectionContext.GetService<IProcessTemplateValidatorService>().Validate(collectionContext, content);
      string message = string.Join(",", templateValidatorResult.Errors.Select<ProcessTemplateValidatorMessage, string>((Func<ProcessTemplateValidatorMessage, string>) (e => string.Format("{0}:{1}", (object) e.File, (object) e.LineNumber))));
      collectionContext.Trace(1300561, TraceLevel.Info, "ProcessService", "IVssFrameworkService", message);
      collectionContext.GetService<CustomerIntelligenceService>().Publish(collectionContext, "ProcessService", "Update", "Errors", string.Format("{0} : {1}", (object) templateTypeId, (object) message));
      collectionContext.Trace(1300562, TraceLevel.Info, "ProcessService", "IVssFrameworkService", string.Join(",", templateValidatorResult.ConfirmationsNeeded.Select<ProcessTemplateValidatorMessage, string>((Func<ProcessTemplateValidatorMessage, string>) (w => string.Format("{0}:{1}:{2}", (object) w.File, (object) w.LineNumber, (object) w.Message)))));
      collectionContext.Trace(1300563, TraceLevel.Info, "ProcessService", "IVssFrameworkService", string.Join(",", templateValidatorResult.Details.OfType<ProcessTemplateFieldRenameData>().Select<ProcessTemplateFieldRenameData, string>((Func<ProcessTemplateFieldRenameData, string>) (r => string.Format("{0}:{1}", (object) r.RefName, (object) r.NewName)))));
      return templateValidatorResult;
    }

    public static void ExtractNameAndType(Stream content, out string name, out Guid type)
    {
      using (content = ZipArchiveProcessTemplatePackage.FixupZipSingleRootFolder(content))
      {
        using (ZipArchiveProcessTemplatePackage processTemplatePackage = new ZipArchiveProcessTemplatePackage(content))
        {
          name = processTemplatePackage.Name;
          type = processTemplatePackage.TypeId;
        }
      }
    }

    public enum ProcessUpdateProgressState
    {
      Created,
      InProgress,
      Success,
      Failure,
    }
  }
}
