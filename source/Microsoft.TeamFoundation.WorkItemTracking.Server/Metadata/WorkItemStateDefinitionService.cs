// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemStateDefinitionService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Process;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.Events;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class WorkItemStateDefinitionService : IWorkItemStateDefinitionService, IVssFrameworkService
  {
    public static IReadOnlyDictionary<WorkItemStateCategory, string> DefaultStateCategoryColors = (IReadOnlyDictionary<WorkItemStateCategory, string>) new Dictionary<WorkItemStateCategory, string>()
    {
      {
        WorkItemStateCategory.Proposed,
        "b2b2b2"
      },
      {
        WorkItemStateCategory.InProgress,
        "007acc"
      },
      {
        WorkItemStateCategory.Resolved,
        "ff9d00"
      },
      {
        WorkItemStateCategory.Completed,
        "339933"
      },
      {
        WorkItemStateCategory.Removed,
        "ffffff"
      }
    };
    public static readonly HashSet<WorkItemStateCategory> UnchangeableStateCategories = new HashSet<WorkItemStateCategory>()
    {
      WorkItemStateCategory.Completed
    };
    public static readonly HashSet<WorkItemStateCategory> CompletelyHideableStateCategories = new HashSet<WorkItemStateCategory>()
    {
      WorkItemStateCategory.Resolved
    };
    private static IReadOnlyCollection<string> CombinedWorkFlowFieldReferenceNames = (IReadOnlyCollection<string>) new List<string>()
    {
      "System.State",
      "System.Reason",
      "Microsoft.VSTS.Common.ClosedDate",
      "Microsoft.VSTS.Common.ClosedBy",
      "Microsoft.VSTS.Common.ActivatedBy",
      "Microsoft.VSTS.Common.ActivatedDate",
      "Microsoft.VSTS.Common.ResolvedBy",
      "Microsoft.VSTS.Common.ResolvedDate",
      "Microsoft.VSTS.Common.StateChangeDate"
    };

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => requestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public WorkItemStateDefinition GetStateDefinition(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      Guid stateId,
      bool includeParent = false)
    {
      WorkItemStateDefinition itemStateDefinition = (WorkItemStateDefinition) null;
      IReadOnlyCollection<WorkItemStateDefinition> stateDefinitions = this.GetStateDefinitions(requestContext, processId, witReferenceName, includeParent);
      List<WorkItemStateDefinition> itemStateDefinitionList;
      if (stateDefinitions == null)
      {
        itemStateDefinitionList = (List<WorkItemStateDefinition>) null;
      }
      else
      {
        IEnumerable<WorkItemStateDefinition> source = stateDefinitions.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.Id == stateId));
        itemStateDefinitionList = source != null ? source.ToList<WorkItemStateDefinition>() : (List<WorkItemStateDefinition>) null;
      }
      List<WorkItemStateDefinition> source1 = itemStateDefinitionList;
      if (source1 != null && source1.Count == 1)
        itemStateDefinition = source1.First<WorkItemStateDefinition>();
      else if (source1 != null && source1.Count > 1)
        itemStateDefinition = source1.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => TFStringComparer.WorkItemTypeReferenceName.Equals(s.WorkItemTypeReferenceName, witReferenceName))).FirstOrDefault<WorkItemStateDefinition>();
      return itemStateDefinition != null ? itemStateDefinition : throw new WorkItemStateDefinitionNotFoundException(stateId.ToString());
    }

    public WorkItemStateDefinition GetStateDefinition(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      string stateName,
      bool includeParent = false)
    {
      return this.GetStateDefinitions(requestContext, processId, witReferenceName, includeParent).FirstOrDefault<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => TFStringComparer.WorkItemStateName.Equals(s.Name, stateName))) ?? throw new WorkItemStateDefinitionNotFoundException(stateName);
    }

    public IReadOnlyCollection<WorkItemStateDefinition> GetStateDefinitions(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      bool includeParent = false)
    {
      IReadOnlyCollection<WorkItemStateDefinition> source = this.CloneAndFixOrder((IList<WorkItemStateDefinition>) this.GetStateDefinitionsInternal(requestContext, processId, witReferenceName, true, true));
      return includeParent ? source : (IReadOnlyCollection<WorkItemStateDefinition>) source.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => TFStringComparer.WorkItemTypeReferenceName.Equals(witReferenceName, s.WorkItemTypeReferenceName))).ToList<WorkItemStateDefinition>();
    }

    public int GetStateDefinitionHash(IVssRequestContext requestContext, Guid processId)
    {
      int stateDefinitionHash = 0;
      if (requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, processId).IsDerived)
      {
        foreach (WorkItemStateDefinitionRecord itemStateDefinition in (IEnumerable<WorkItemStateDefinitionRecord>) requestContext.GetService<WorkItemStateDefinitionCacheService>().GetWorkItemStateDefinitions(requestContext, processId))
          stateDefinitionHash ^= itemStateDefinition.AuthorizedDate.GetHashCode();
      }
      return stateDefinitionHash;
    }

    public HashSet<string> GetStateNames(IVssRequestContext requestContext)
    {
      IReadOnlyCollection<ProcessDescriptor> processDescriptors = requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptors(requestContext);
      ILegacyWorkItemTrackingProcessService service1 = requestContext.GetService<ILegacyWorkItemTrackingProcessService>();
      WorkItemStateDefinitionCacheService service2 = requestContext.GetService<WorkItemStateDefinitionCacheService>();
      HashSet<string> stateNames = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
      foreach (ProcessDescriptor processDescriptor in (IEnumerable<ProcessDescriptor>) processDescriptors)
      {
        if (processDescriptor.IsDerived)
        {
          IEnumerable<string> other = service2.GetWorkItemStateDefinitions(requestContext, processDescriptor.TypeId).Select<WorkItemStateDefinitionRecord, string>((Func<WorkItemStateDefinitionRecord, string>) (sr => sr.Name));
          stateNames.UnionWith(other);
        }
        else
        {
          foreach (ProcessWorkItemTypeDefinition itemTypeDefinition in (IEnumerable<ProcessWorkItemTypeDefinition>) service1.GetProcessWorkDefinition(requestContext, processDescriptor.TypeId).WorkItemTypeDefinitions)
            stateNames.UnionWith((IEnumerable<string>) itemTypeDefinition.StateNames);
        }
      }
      return stateNames;
    }

    public HashSet<string> GetStateNames(
      IVssRequestContext requestContext,
      Guid processId,
      IReadOnlyCollection<WorkItemStateCategory> stateCategories)
    {
      ProcessDescriptor processDescriptor = requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, processId);
      HashSet<string> stateNames = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
      Guid? nullable = new Guid?();
      if (processDescriptor.IsDerived)
      {
        WorkItemStateCategory result;
        IEnumerable<string> other = requestContext.GetService<WorkItemStateDefinitionCacheService>().GetWorkItemStateDefinitions(requestContext, processDescriptor.TypeId).Where<WorkItemStateDefinitionRecord>((Func<WorkItemStateDefinitionRecord, bool>) (sr => Enum.TryParse<WorkItemStateCategory>(sr.StateCategory.ToString(), out result) && stateCategories.Contains<WorkItemStateCategory>(result))).Select<WorkItemStateDefinitionRecord, string>((Func<WorkItemStateDefinitionRecord, string>) (sr => sr.Name));
        stateNames.UnionWith(other);
        nullable = new Guid?(processDescriptor.Inherits);
      }
      else
        nullable = new Guid?(processId);
      if (nullable.HasValue)
      {
        foreach (ProcessWorkItemTypeDefinition itemTypeDefinition in (IEnumerable<ProcessWorkItemTypeDefinition>) requestContext.GetService<ILegacyWorkItemTrackingProcessService>().GetProcessWorkDefinition(requestContext, nullable.Value).WorkItemTypeDefinitions)
        {
          IEnumerable<string> other = itemTypeDefinition.States.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => stateCategories.Contains<WorkItemStateCategory>(s.StateCategory))).Select<WorkItemStateDefinition, string>((Func<WorkItemStateDefinition, string>) (s => s.Name));
          stateNames.UnionWith(other);
        }
      }
      return stateNames;
    }

    public IReadOnlyCollection<WorkItemStateDefinition> GetCombinedStateDefinitions(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      bool bypassCache = false)
    {
      List<WorkItemStateDefinition> definitionsInternal = this.GetStateDefinitionsInternal(requestContext, processId, witReferenceName, true, bypassCache);
      List<Guid> hiddenStateIds = definitionsInternal.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.Hidden)).Select<WorkItemStateDefinition, Guid>((Func<WorkItemStateDefinition, Guid>) (s => s.Id)).ToList<Guid>();
      return this.CloneAndFixOrder((IList<WorkItemStateDefinition>) definitionsInternal.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => !hiddenStateIds.Contains(s.Id))).ToList<WorkItemStateDefinition>());
    }

    public bool TryGetCombinedStateDefinitions(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      out IReadOnlyCollection<WorkItemStateDefinition> stateDefinitions)
    {
      try
      {
        stateDefinitions = this.GetCombinedStateDefinitions(requestContext, processId, witReferenceName, false);
        return true;
      }
      catch (ProcessWorkItemTypeDoesNotExistException ex)
      {
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("ExceptionType", ex.GetType().ToString());
        if (!string.IsNullOrWhiteSpace(ex.Message))
          properties.Add("ExceptionMessage", ex.Message);
        if (!string.IsNullOrWhiteSpace(ex.StackTrace))
          properties.Add("StackTrace", ex.StackTrace);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (WorkItemStateDefinitionService), nameof (TryGetCombinedStateDefinitions), properties);
        stateDefinitions = (IReadOnlyCollection<WorkItemStateDefinition>) null;
        return false;
      }
    }

    public IReadOnlyCollection<WorkItemStateDefinition> GetDeltaStateDefinitions(
      IVssRequestContext requestContext,
      ProcessWorkItemType wit)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ProcessWorkItemType>(wit, nameof (wit));
      return (IReadOnlyCollection<WorkItemStateDefinition>) requestContext.TraceBlock<List<WorkItemStateDefinition>>(910510, 910511, "WorkItemStateDefinition", nameof (WorkItemStateDefinitionService), nameof (GetDeltaStateDefinitions), (Func<List<WorkItemStateDefinition>>) (() => this.GetStateDefinitionsFromNewStorage(requestContext, wit)));
    }

    public WorkItemStateDefinition CreateStateDefinition(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      WorkItemStateDeclaration newState,
      bool allowModifyingAnyStateCategory = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witReferenceName, nameof (witReferenceName));
      WorkItemStateDefinitionService.CheckStateDeclaration(requestContext, newState);
      if (!allowModifyingAnyStateCategory)
        WorkItemStateDefinitionService.BlockChangesToUnchangeableStateCategories(newState.StateCategory);
      newState.WorkItemTypeReferenceName = witReferenceName;
      if (newState.Id.HasValue)
        throw new ArgumentException("Id");
      return requestContext.TraceBlock<WorkItemStateDefinition>(910502, 910503, "WorkItemStateDefinition", nameof (WorkItemStateDefinitionService), "AddStateToWorkflow", (Func<WorkItemStateDefinition>) (() =>
      {
        WorkItemTrackingProcessService.CheckEditPermission(requestContext, processId);
        ProcessWorkItemType typelet = requestContext.GetService<IProcessWorkItemTypeService>().GetTypelet<ProcessWorkItemType>(requestContext, processId, witReferenceName, true);
        IWorkItemTrackingProcessService service = requestContext.GetService<IWorkItemTrackingProcessService>();
        CustomerIntelligenceData properties1 = new CustomerIntelligenceData();
        if (typelet.IsDerived && !service.IsInheritedStateCustomizationAllowed(requestContext, processId))
          throw new WorkItemStateCustomizationNotSupportedException();
        List<WorkItemStateDefinition> definitionsInternal = this.GetStateDefinitionsInternal(requestContext, processId, witReferenceName, true, true);
        properties1.Add("existingStates", WorkItemStateDefinitionService.GetWorkItemStateDefinitionListString((IReadOnlyCollection<WorkItemStateDefinition>) definitionsInternal));
        definitionsInternal.Sort();
        properties1.Add("existingStatesAfterSort", WorkItemStateDefinitionService.GetWorkItemStateDefinitionListString((IReadOnlyCollection<WorkItemStateDefinition>) definitionsInternal));
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (WorkItemStateDefinitionService), nameof (CreateStateDefinition), properties1);
        int num = definitionsInternal.Count<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.Customization == CustomizationType.Inherited && s.Hidden));
        WorkItemStateDefinitionService.CheckStateLimit(requestContext, definitionsInternal.Count + 1 - num);
        if (definitionsInternal.Any<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => TFStringComparer.WorkItemStateName.Equals(s.Name, newState.Name))))
          throw new WorkItemStateNameAlreadyInUseException(newState.Name);
        IList<WorkItemStateDeclaration> source = this.MergeAndValidate(requestContext, processId, witReferenceName, (IReadOnlyCollection<WorkItemStateDefinition>) definitionsInternal, newState, (WorkItemStateDefinition) null);
        WorkItemStateDeclaration state = source.FirstOrDefault<WorkItemStateDeclaration>((Func<WorkItemStateDeclaration, bool>) (s => TFStringComparer.WorkItemStateName.Equals(s.Name, newState.Name)));
        source.Remove(state);
        Dictionary<Guid, int> dictionary = source.ToDictionary<WorkItemStateDeclaration, Guid, int>((Func<WorkItemStateDeclaration, Guid>) (s => s.Id.Value), (Func<WorkItemStateDeclaration, int>) (s => s.Order));
        this.EnsureBackcompatConstantsForStates(requestContext, witReferenceName, definitionsInternal.Select<WorkItemStateDefinition, string>((Func<WorkItemStateDefinition, string>) (s => s.Name)).Concat<string>((IEnumerable<string>) new string[1]
        {
          state.Name
        }));
        Guid userId = requestContext.GetUserId();
        using (WorkItemStateDefinitionComponent component = requestContext.CreateComponent<WorkItemStateDefinitionComponent>())
          component.CreateWorkItemStateDefinition(processId, typelet.Id, userId, state, (IDictionary<Guid, int>) dictionary);
        CustomerIntelligenceData properties2 = new CustomerIntelligenceData();
        properties2.Add("CreateNewState", newState.Name);
        properties2.Add("StateCategory", newState.StateCategory.ToString());
        properties2.Add("WorkItemType", typelet.Name);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (WorkItemStateDefinitionService), "Create", properties2);
        this.PublishStateDefinitionChangedEvent(requestContext, processId, StateDefinitionChangeType.CreateStateDefinition);
        return this.GetStateDefinition(requestContext, processId, witReferenceName, newState.Name, false);
      }));
    }

    public void DeleteStateDefinition(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      Guid stateId,
      bool allowModifyingAnyStateCategory = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witReferenceName, nameof (witReferenceName));
      ArgumentUtility.CheckForEmptyGuid(stateId, nameof (stateId));
      requestContext.TraceBlock(910504, 910505, "WorkItemStateDefinition", nameof (WorkItemStateDefinitionService), "RemoveStateFromWorkflow", (Action) (() =>
      {
        WorkItemTrackingProcessService.CheckEditPermission(requestContext, processId);
        ProcessWorkItemType typelet = requestContext.GetService<IProcessWorkItemTypeService>().GetTypelet<ProcessWorkItemType>(requestContext, processId, witReferenceName, true);
        IWorkItemTrackingProcessService service = requestContext.GetService<IWorkItemTrackingProcessService>();
        if (typelet.IsDerived && !service.IsInheritedStateCustomizationAllowed(requestContext, processId))
          throw new WorkItemStateCustomizationNotSupportedException();
        List<WorkItemStateDefinition> definitionsInternal = this.GetStateDefinitionsInternal(requestContext, processId, witReferenceName, true, true);
        WorkItemStateDefinition stateDefinition = this.GetStateDefinition(requestContext, processId, witReferenceName, stateId, false);
        if (stateDefinition.StateCategory == WorkItemStateCategory.Completed && stateDefinition.Customization == CustomizationType.Custom)
        {
          IEnumerable<WorkItemStateDefinition> source1 = definitionsInternal.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.StateCategory == WorkItemStateCategory.Completed && s.Customization != CustomizationType.System));
          IEnumerable<WorkItemStateDefinition> source2 = definitionsInternal.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.StateCategory == WorkItemStateCategory.Completed && s.Customization == CustomizationType.System));
          IEnumerable<WorkItemStateDefinition> source3 = source1.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.Customization == CustomizationType.Inherited && s.Hidden));
          if (source1.Count<WorkItemStateDefinition>() == 2 && source2.Count<WorkItemStateDefinition>() == 1 && source3.Count<WorkItemStateDefinition>() == 1)
          {
            WorkItemStateDefinition stateToRemove = source3.Single<WorkItemStateDefinition>();
            this.DeleteStateDefinitionInternal(requestContext, processId, witReferenceName, typelet, definitionsInternal, stateToRemove);
            definitionsInternal = this.GetStateDefinitionsInternal(requestContext, processId, witReferenceName, true, true);
            stateDefinition = this.GetStateDefinition(requestContext, processId, witReferenceName, stateId, false);
          }
        }
        this.DeleteStateDefinitionInternal(requestContext, processId, witReferenceName, typelet, definitionsInternal, stateDefinition);
      }));
    }

    private void DeleteStateDefinitionInternal(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      ProcessWorkItemType wit,
      List<WorkItemStateDefinition> existingStateDefinitions,
      WorkItemStateDefinition stateToRemove)
    {
      Dictionary<Guid, int> dictionary = this.MergeAndValidate(requestContext, processId, witReferenceName, (IReadOnlyCollection<WorkItemStateDefinition>) existingStateDefinitions, (WorkItemStateDeclaration) null, stateToRemove).ToDictionary<WorkItemStateDeclaration, Guid, int>((Func<WorkItemStateDeclaration, Guid>) (s => s.Id.Value), (Func<WorkItemStateDeclaration, int>) (s => s.Order));
      Guid userId = requestContext.GetUserId();
      using (WorkItemStateDefinitionComponent component = requestContext.CreateComponent<WorkItemStateDefinitionComponent>())
        component.DeleteWorkItemStateDefinition(processId, wit.Id, userId, stateToRemove.Id, (IDictionary<Guid, int>) dictionary);
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("RemoveState", stateToRemove.Name);
      properties.Add("StateCategory", stateToRemove.StateCategory.ToString());
      properties.Add("WorkItemType", wit.Name);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (WorkItemStateDefinitionService), "Remove", properties);
      this.PublishStateDefinitionChangedEvent(requestContext, processId, StateDefinitionChangeType.DeleteStateDefinition);
    }

    public WorkItemStateDefinition UpdateStateDefinition(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      Guid stateId,
      string color = null,
      WorkItemStateCategory? stateCategory = null,
      int? stateOrder = null,
      bool allowModifyingAnyStateCategory = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witReferenceName, nameof (witReferenceName));
      ArgumentUtility.CheckForEmptyGuid(stateId, nameof (stateId));
      if (color != null)
        CommonWITUtils.CheckColor(color);
      if (stateCategory.HasValue)
        ArgumentUtility.CheckGreaterThanZero((float) stateCategory.Value, nameof (stateCategory));
      return requestContext.TraceBlock<WorkItemStateDefinition>(910506, 910507, "WorkItemStateDefinition", nameof (WorkItemStateDefinitionService), "RemoveStateFromWorkflow", (Func<WorkItemStateDefinition>) (() =>
      {
        WorkItemTrackingProcessService.CheckEditPermission(requestContext, processId);
        IWorkItemTrackingProcessService service = requestContext.GetService<IWorkItemTrackingProcessService>();
        ProcessWorkItemType typelet = requestContext.GetService<IProcessWorkItemTypeService>().GetTypelet<ProcessWorkItemType>(requestContext, processId, witReferenceName, true);
        if (typelet.IsDerived && !service.IsInheritedStateCustomizationAllowed(requestContext, processId))
          throw new WorkItemStateCustomizationNotSupportedException();
        WorkItemStateDefinition stateDefinition = this.GetStateDefinition(requestContext, processId, witReferenceName, stateId, false);
        if (!allowModifyingAnyStateCategory && stateCategory.HasValue)
          WorkItemStateDefinitionService.BlockChangesToUnchangeableStateCategories(stateCategory.Value);
        if (stateDefinition.Hidden)
          throw new WorkItemStateCustomizationNotSupportedException();
        WorkItemStateDeclaration stateDeclaration = new WorkItemStateDeclaration()
        {
          Id = new Guid?(stateDefinition.Id),
          WorkItemTypeReferenceName = stateDefinition.WorkItemTypeReferenceName,
          Name = stateDefinition.Name,
          Hidden = stateDefinition.Hidden,
          Color = color ?? stateDefinition.Color
        };
        stateDeclaration.Order = !stateCategory.HasValue || stateOrder.HasValue ? (stateOrder.HasValue ? stateOrder.Value : stateDefinition.Order) : 0;
        stateDeclaration.StateCategory = (WorkItemStateCategory) ((int) stateCategory ?? (int) stateDefinition.StateCategory);
        bool orderChanged = stateDeclaration.Order != stateDefinition.Order || stateDeclaration.StateCategory != stateDefinition.StateCategory;
        Guid userId = requestContext.GetUserId();
        if (orderChanged)
        {
          IList<WorkItemStateDeclaration> source = this.MergeAndValidate(requestContext, processId, witReferenceName, (IReadOnlyCollection<WorkItemStateDefinition>) this.GetStateDefinitionsInternal(requestContext, processId, witReferenceName, true, true), stateDeclaration, stateDefinition);
          WorkItemStateDeclaration state = source.FirstOrDefault<WorkItemStateDeclaration>((Func<WorkItemStateDeclaration, bool>) (s =>
          {
            Guid? id = s.Id;
            Guid guid = stateId;
            if (!id.HasValue)
              return false;
            return !id.HasValue || id.GetValueOrDefault() == guid;
          }));
          source.Remove(state);
          Dictionary<Guid, int> dictionary = source.ToDictionary<WorkItemStateDeclaration, Guid, int>((Func<WorkItemStateDeclaration, Guid>) (s => s.Id.Value), (Func<WorkItemStateDeclaration, int>) (s => s.Order));
          using (WorkItemStateDefinitionComponent component = requestContext.CreateComponent<WorkItemStateDefinitionComponent>())
            component.UpdateWorkItemStateDefinition(processId, typelet.Id, userId, state, (IDictionary<Guid, int>) dictionary, orderChanged);
        }
        else
        {
          using (WorkItemStateDefinitionComponent component = requestContext.CreateComponent<WorkItemStateDefinitionComponent>())
            component.UpdateWorkItemStateDefinition(processId, typelet.Id, userId, stateDeclaration, (IDictionary<Guid, int>) null, orderChanged);
        }
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("UpdateState", stateDefinition.Name);
        properties.Add("StateCategory", stateDefinition.StateCategory.ToString());
        properties.Add("Color", stateDefinition.Color);
        properties.Add("Order", (double) stateDefinition.Order);
        properties.Add("WorkItemType", typelet.Name);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (WorkItemStateDefinitionService), "Update", properties);
        this.PublishStateDefinitionChangedEvent(requestContext, processId, StateDefinitionChangeType.UpdateStateDefinition);
        return this.GetStateDefinition(requestContext, processId, witReferenceName, stateId, false);
      }));
    }

    public WorkItemStateDefinition HideStateDefinition(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      Guid stateId,
      bool allowModifyingAnyStateCategory = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witReferenceName, nameof (witReferenceName));
      ArgumentUtility.CheckForEmptyGuid(stateId, nameof (stateId));
      return requestContext.TraceBlock<WorkItemStateDefinition>(910508, 910509, "WorkItemStateDefinition", nameof (WorkItemStateDefinitionService), nameof (HideStateDefinition), (Func<WorkItemStateDefinition>) (() =>
      {
        WorkItemTrackingProcessService.CheckEditPermission(requestContext, processId);
        ProcessWorkItemType typelet = requestContext.GetService<IProcessWorkItemTypeService>().GetTypelet<ProcessWorkItemType>(requestContext, processId, witReferenceName, true);
        IWorkItemTrackingProcessService service = requestContext.GetService<IWorkItemTrackingProcessService>();
        if (typelet.IsDerived && !service.IsInheritedStateCustomizationAllowed(requestContext, processId))
          throw new WorkItemStateCustomizationNotSupportedException();
        List<WorkItemStateDefinition> definitionsInternal = this.GetStateDefinitionsInternal(requestContext, processId, witReferenceName, true, true);
        WorkItemStateDefinition itemStateDefinition = definitionsInternal.FirstOrDefault<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.ProcessId != processId && s.Id == stateId));
        if (itemStateDefinition == null)
          throw new WorkItemStateDefinitionNotFoundException(stateId.ToString());
        if (definitionsInternal.Any<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.ProcessId == processId && s.Id == stateId)))
          throw new WorkItemStateDefinitionAlreadyExistsException(stateId.ToString());
        if (!allowModifyingAnyStateCategory)
          WorkItemStateDefinitionService.BlockChangesToUnchangeableStateCategories(itemStateDefinition.StateCategory);
        List<Guid> hiddenStateIds = definitionsInternal.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.Hidden)).Select<WorkItemStateDefinition, Guid>((Func<WorkItemStateDefinition, Guid>) (s => s.Id)).ToList<Guid>();
        WorkItemStateDefinitionService.CheckForOneActiveOneClosedState(definitionsInternal.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => !hiddenStateIds.Contains(s.Id))).ToList<WorkItemStateDefinition>().Select<WorkItemStateDefinition, WorkItemStateDeclaration>((Func<WorkItemStateDefinition, WorkItemStateDeclaration>) (s => WorkItemStateDeclaration.Create(s))).ToList<WorkItemStateDeclaration>());
        WorkItemStateDeclaration state = new WorkItemStateDeclaration()
        {
          Id = new Guid?(itemStateDefinition.Id),
          WorkItemTypeReferenceName = witReferenceName,
          Name = itemStateDefinition.Name,
          Color = itemStateDefinition.Color,
          StateCategory = itemStateDefinition.StateCategory,
          Order = itemStateDefinition.Order,
          Hidden = true
        };
        Guid userId = requestContext.GetUserId();
        using (WorkItemStateDefinitionComponent component = requestContext.CreateComponent<WorkItemStateDefinitionComponent>())
          component.HideParentStateDefinition(processId, typelet.Id, userId, state);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("HideState", itemStateDefinition.Name);
        properties.Add("StateCategory", itemStateDefinition.StateCategory.ToString());
        properties.Add("WorkItemType", typelet.Name);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (WorkItemStateDefinitionService), "Hide", properties);
        this.PublishStateDefinitionChangedEvent(requestContext, processId, StateDefinitionChangeType.HideStateDefinition);
        return this.GetStateDefinition(requestContext, processId, witReferenceName, stateId, false);
      }));
    }

    public IReadOnlyCollection<ProcessIdDateTimeWatermarkPair> GetChangedStateProcesses(
      IVssRequestContext requestContext,
      DateTime watermark)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (watermark));
      return requestContext.TraceBlock<IReadOnlyCollection<ProcessIdDateTimeWatermarkPair>>(910512, 910513, "WorkItemStateDefinition", "BehaviorService", nameof (GetChangedStateProcesses), (Func<IReadOnlyCollection<ProcessIdDateTimeWatermarkPair>>) (() =>
      {
        using (WorkItemStateDefinitionComponent component = requestContext.CreateComponent<WorkItemStateDefinitionComponent>())
          return component.GetChangedStateProcessesSinceWatermark(watermark);
      }));
    }

    public bool TryGetNextStateOnCheckinForInheritedProcess(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      string currentStateName,
      out WorkItemStateOnTransition nextState)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witReferenceName, nameof (witReferenceName));
      ArgumentUtility.CheckStringForNullOrEmpty(currentStateName, nameof (currentStateName));
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(nameof (processId), (object) processId);
      properties.Add("Workitemtype Reference Name", string.Format("[NonEmail: {0}]", (object) witReferenceName));
      properties.Add("Current State Name", currentStateName);
      nextState = new WorkItemStateOnTransition();
      try
      {
        ProcessDescriptor processDescriptor = requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, processId);
        if (!processDescriptor.IsCustom)
        {
          IReadOnlyCollection<WorkItemStateDefinition> stateDefinitions = this.GetCombinedStateDefinitions(requestContext, processId, witReferenceName, false);
          WorkItemStateDefinition itemStateDefinition1;
          if (stateDefinitions == null)
          {
            itemStateDefinition1 = (WorkItemStateDefinition) null;
          }
          else
          {
            IEnumerable<WorkItemStateDefinition> source = stateDefinitions.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => TFStringComparer.WorkItemStateName.Equals(s.Name, currentStateName)));
            itemStateDefinition1 = source != null ? source.FirstOrDefault<WorkItemStateDefinition>() : (WorkItemStateDefinition) null;
          }
          WorkItemStateDefinition itemStateDefinition2 = itemStateDefinition1;
          if (itemStateDefinition2 != null && (itemStateDefinition2.StateCategory == WorkItemStateCategory.InProgress || itemStateDefinition2.StateCategory == WorkItemStateCategory.Proposed))
          {
            properties.Add("Is derived process", processDescriptor.IsDerived);
            properties.Add("Process Inherits", (object) processDescriptor.Inherits);
            properties.Add("Current State Category", itemStateDefinition2.StateCategory.ToString());
            IEnumerable<WorkItemStateDefinition> hiddenStates = stateDefinitions != null ? stateDefinitions.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.Hidden)) : (IEnumerable<WorkItemStateDefinition>) null;
            IEnumerable<WorkItemStateDefinition> itemStateDefinitions1;
            if (stateDefinitions == null)
            {
              itemStateDefinitions1 = (IEnumerable<WorkItemStateDefinition>) null;
            }
            else
            {
              IEnumerable<WorkItemStateDefinition> source = stateDefinitions.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.StateCategory == WorkItemStateCategory.Resolved));
              itemStateDefinitions1 = source != null ? source.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => !hiddenStates.Any<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (hs => TFStringComparer.WorkItemStateName.Equals(hs.Name, s.Name))))) : (IEnumerable<WorkItemStateDefinition>) null;
            }
            IEnumerable<WorkItemStateDefinition> source1 = itemStateDefinitions1;
            if (source1 == null || source1.Count<WorkItemStateDefinition>() == 0)
            {
              IEnumerable<WorkItemStateDefinition> itemStateDefinitions2;
              if (stateDefinitions == null)
              {
                itemStateDefinitions2 = (IEnumerable<WorkItemStateDefinition>) null;
              }
              else
              {
                IEnumerable<WorkItemStateDefinition> source2 = stateDefinitions.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.StateCategory == WorkItemStateCategory.Completed));
                itemStateDefinitions2 = source2 != null ? source2.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => !hiddenStates.Any<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (hs => TFStringComparer.WorkItemStateName.Equals(hs.Name, s.Name))))) : (IEnumerable<WorkItemStateDefinition>) null;
              }
              source1 = itemStateDefinitions2;
            }
            IReadOnlyCollection<WorkItemStateDefinition> source3 = this.CloneAndFixOrder(source1 != null ? (IList<WorkItemStateDefinition>) source1.ToList<WorkItemStateDefinition>() : (IList<WorkItemStateDefinition>) null);
            if (source3 != null && source3.Count > 0)
              nextState.NextStateName = source3.First<WorkItemStateDefinition>().Name;
            properties.Add("Next State Name", nextState.NextStateName);
            properties.Add("Next State Error", nextState.ErrorMessage);
          }
          else if (itemStateDefinition2 != null)
          {
            if (itemStateDefinition2.StateCategory == WorkItemStateCategory.Resolved)
            {
              ComposedWorkItemType workItemType = requestContext.GetService<IProcessWorkItemTypeService>().GetWorkItemType(requestContext, processId, witReferenceName);
              nextState.ErrorMessage = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemNoAutomaticTransition((object) (workItemType?.Name ?? witReferenceName), (object) "{0}", (object) itemStateDefinition2.Name);
              nextState.ErrorCode = "VS1640135";
            }
          }
        }
      }
      catch (ProcessWorkItemTypeDoesNotExistException ex)
      {
        requestContext.TraceException(910501, "Services", nameof (WorkItemStateDefinitionService), (Exception) ex);
        return false;
      }
      catch (ProcessWorkItemTypeNotFoundException ex)
      {
        requestContext.TraceException(910501, "Services", nameof (WorkItemStateDefinitionService), (Exception) ex);
        return false;
      }
      properties.Add("Result ", nextState != null && nextState.NextStateName != null);
      service.Publish(requestContext, nameof (WorkItemStateDefinitionService), nameof (TryGetNextStateOnCheckinForInheritedProcess), properties);
      return nextState.NextStateName != null;
    }

    public WorkItemStateDefinition ReplaceCompletedStateDefinition(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      WorkItemStateDeclaration newState)
    {
      IEnumerable<WorkItemStateDefinition> source = this.GetCombinedStateDefinitions(requestContext, processId, witRefName, true).Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (x => x.StateCategory == WorkItemStateCategory.Completed));
      if (!source.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (x => x.Name == newState.Name && x.StateCategory == WorkItemStateCategory.Completed)).Any<WorkItemStateDefinition>() && this.CreateStateDefinition(requestContext, processId, witRefName, newState, true) == null)
        throw new CreateStateDefinitionFailedException();
      foreach (WorkItemStateDefinition itemStateDefinition in source.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (x => x.Name != newState.Name)))
      {
        if (itemStateDefinition.Customization == CustomizationType.Custom)
          this.DeleteStateDefinition(requestContext, processId, witRefName, itemStateDefinition.Id, true);
        else if (itemStateDefinition.Customization == CustomizationType.System)
          this.HideStateDefinition(requestContext, processId, witRefName, itemStateDefinition.Id, true);
      }
      return this.GetCombinedStateDefinitions(requestContext, processId, witRefName, false).Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (x => x.Name == newState.Name && x.StateCategory == WorkItemStateCategory.Completed)).First<WorkItemStateDefinition>();
    }

    internal void EnsureBackcompatConstantsForStates(
      IVssRequestContext requestContext,
      string witReferenceName,
      IEnumerable<string> states)
    {
      HashSet<string> source = new HashSet<string>();
      foreach (string state in states)
      {
        source.Add(state);
        source.Add(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateDefaultReason((object) state));
        source.Add(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateDefaultReasonOut((object) state));
      }
      source.Add(StatesConstants.AllowedValuesListHead(witReferenceName));
      using (WorkItemTypeExtensionComponent component = requestContext.CreateComponent<WorkItemTypeExtensionComponent>())
        component.EnsureConstantsForBackcompat((IList<string>) source.ToList<string>());
    }

    private List<WorkItemStateDefinition> GetStateDefinitionsInternal(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      bool includeParent = false,
      bool bypassCache = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrEmpty(witReferenceName, nameof (witReferenceName));
      return requestContext.TraceBlock<List<WorkItemStateDefinition>>(910500, 910501, "WorkItemStateDefinition", nameof (WorkItemStateDefinitionService), "GetStateDefinitions", (Func<List<WorkItemStateDefinition>>) (() =>
      {
        ITeamFoundationProcessService service = requestContext.GetService<ITeamFoundationProcessService>();
        ProcessDescriptor processDescriptor = service.GetProcessDescriptor(requestContext, processId);
        ComposedWorkItemType workItemType = requestContext.GetService<IProcessWorkItemTypeService>().GetWorkItemType(requestContext, processId, witReferenceName, bypassCache);
        bool flag = requestContext.IsFeatureEnabled("WebAccess.WorkItemTracking.WorkItemStateDefinitionUseTrace");
        StringBuilder stringBuilder = requestContext.IsTracing(910502, TraceLevel.Info, nameof (WorkItemStateDefinitionService), nameof (GetStateDefinitionsInternal)) ? new StringBuilder() : (StringBuilder) null;
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        stringBuilder?.Append(string.Format("ComposedWit IsDerived: {0} IsCustom: {1}", (object) workItemType.IsDerived, (object) workItemType.IsCustomType));
        properties.Add("ComposedWit", string.Format("IsDervied: {0}, IsCustom: {1}", (object) workItemType.IsDerived, (object) workItemType.IsCustomType));
        List<WorkItemStateDefinition> definitionsInternal1 = !workItemType.IsCustomType ? (!workItemType.IsDerived ? this.GetStateDefinitionsFromLegacyStorage(requestContext, workItemType.ProcessId, workItemType.ReferenceName) : this.GetStateDefinitionsFromNewStorage(requestContext, workItemType.DerivedWorkItemType, bypassCache)) : this.GetStateDefinitionsFromNewStorage(requestContext, workItemType.CustomWorkItemType, bypassCache);
        stringBuilder?.Append(" StatesBeforeSort " + WorkItemStateDefinitionService.GetWorkItemStateDefinitionListString((IReadOnlyCollection<WorkItemStateDefinition>) definitionsInternal1));
        properties.Add("statesBeforeSort", WorkItemStateDefinitionService.GetWorkItemStateDefinitionListString((IReadOnlyCollection<WorkItemStateDefinition>) definitionsInternal1));
        if (includeParent && workItemType.IsDerived)
        {
          List<WorkItemStateDefinition> definitionsInternal2 = this.ReadMerge((IReadOnlyCollection<WorkItemStateDefinition>) this.GetStateDefinitionsFromLegacyStorage(requestContext, service.GetProcessDescriptor(requestContext, processDescriptor.Inherits).TypeId, workItemType.ParentTypeRefName), (IReadOnlyCollection<WorkItemStateDefinition>) definitionsInternal1);
          if (flag)
          {
            stringBuilder?.Append(" MergedStates " + WorkItemStateDefinitionService.GetWorkItemStateDefinitionListString((IReadOnlyCollection<WorkItemStateDefinition>) definitionsInternal1));
            requestContext.Trace(910502, TraceLevel.Info, nameof (WorkItemStateDefinitionService), nameof (GetStateDefinitionsInternal), stringBuilder?.ToString());
            return definitionsInternal2;
          }
          properties.Add("mergedStates", WorkItemStateDefinitionService.GetWorkItemStateDefinitionListString((IReadOnlyCollection<WorkItemStateDefinition>) definitionsInternal1));
          requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (WorkItemStateDefinitionService), nameof (GetStateDefinitionsInternal), properties);
          return definitionsInternal2;
        }
        if (flag)
          requestContext.Trace(910502, TraceLevel.Info, nameof (WorkItemStateDefinitionService), nameof (GetStateDefinitionsInternal), stringBuilder?.ToString());
        else
          requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (WorkItemStateDefinitionService), nameof (GetStateDefinitionsInternal), properties);
        return definitionsInternal1;
      }));
    }

    private List<WorkItemStateDefinition> GetStateDefinitionsFromNewStorage(
      IVssRequestContext requestContext,
      ProcessWorkItemType wit,
      bool bypassCache = false)
    {
      if (!bypassCache)
      {
        List<WorkItemStateDefinition> list = requestContext.GetService<WorkItemStateDefinitionCacheService>().GetWorkItemStateDefinitions(requestContext, wit.ProcessId, wit).ToList<WorkItemStateDefinition>();
        list.Sort();
        return list;
      }
      IReadOnlyCollection<WorkItemStateDefinitionRecord> stateDefinitions;
      using (WorkItemStateDefinitionComponent component = requestContext.CreateComponent<WorkItemStateDefinitionComponent>())
        stateDefinitions = component.GetProcessStateDefinitions(wit.ProcessId);
      this.UpdateStateDefinitionsCache(requestContext, wit.ProcessId, (IEnumerable<WorkItemStateDefinitionRecord>) stateDefinitions);
      ReadOnlyDictionary<string, WorkItemOobState>.ValueCollection oobStates = requestContext.GetService<WorkItemTrackingOutOfBoxStatesCache>().GetOutOfBoxStates(requestContext)?.Values;
      List<WorkItemStateDefinition> list1 = stateDefinitions.Select<WorkItemStateDefinitionRecord, WorkItemStateDefinition>((Func<WorkItemStateDefinitionRecord, WorkItemStateDefinition>) (sr => WorkItemStateDefinition.Create(sr, wit.ReferenceName, (IEnumerable<WorkItemOobState>) oobStates))).Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (sr => sr.WorkItemTypeId == wit.Id)).ToList<WorkItemStateDefinition>();
      list1.Sort();
      return list1.ToList<WorkItemStateDefinition>();
    }

    private List<WorkItemStateDefinition> GetStateDefinitionsFromLegacyStorage(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName)
    {
      return (requestContext.GetService<ILegacyWorkItemTrackingProcessService>().GetProcessWorkDefinition(requestContext, processId).WorkItemTypeDefinitions.FirstOrDefault<ProcessWorkItemTypeDefinition>((Func<ProcessWorkItemTypeDefinition, bool>) (wit => TFStringComparer.WorkItemTypeReferenceName.Equals(wit.ReferenceName, witReferenceName))) ?? throw new ProcessWorkItemTypeDoesNotExistException(witReferenceName, processId.ToString())).States.ToList<WorkItemStateDefinition>();
    }

    private void UpdateStateDefinitionsCache(
      IVssRequestContext requestContext,
      Guid processId,
      IEnumerable<WorkItemStateDefinitionRecord> stateDefinitions)
    {
      WorkItemStateDefinitionCacheService service = requestContext.GetService<WorkItemStateDefinitionCacheService>();
      Dictionary<Guid, IReadOnlyCollection<WorkItemStateDefinitionRecord>> dictionary1 = stateDefinitions.GroupBy<WorkItemStateDefinitionRecord, Guid>((Func<WorkItemStateDefinitionRecord, Guid>) (s => s.WorkItemTypeId)).ToDictionary<IGrouping<Guid, WorkItemStateDefinitionRecord>, Guid, IReadOnlyCollection<WorkItemStateDefinitionRecord>>((Func<IGrouping<Guid, WorkItemStateDefinitionRecord>, Guid>) (g => g.Key), (Func<IGrouping<Guid, WorkItemStateDefinitionRecord>, IReadOnlyCollection<WorkItemStateDefinitionRecord>>) (g => (IReadOnlyCollection<WorkItemStateDefinitionRecord>) g.ToList<WorkItemStateDefinitionRecord>()));
      IVssRequestContext requestContext1 = requestContext;
      Guid key = processId;
      Dictionary<Guid, IReadOnlyCollection<WorkItemStateDefinitionRecord>> dictionary2 = dictionary1;
      service.Set(requestContext1, key, (IDictionary<Guid, IReadOnlyCollection<WorkItemStateDefinitionRecord>>) dictionary2);
    }

    internal IList<WorkItemStateDeclaration> MergeAndValidate(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      IReadOnlyCollection<WorkItemStateDefinition> existingStateDefinitions,
      WorkItemStateDeclaration newState,
      WorkItemStateDefinition removeState)
    {
      List<WorkItemStateDeclaration> stateDeclarationList = this.UpdateMerge(existingStateDefinitions, newState, removeState);
      IEnumerable<WorkItemStateCategory> parentStateCategories = existingStateDefinitions.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.ProcessId != processId)).Select<WorkItemStateDefinition, WorkItemStateCategory>((Func<WorkItemStateDefinition, WorkItemStateCategory>) (s => s.StateCategory)).Distinct<WorkItemStateCategory>();
      List<Guid?> hiddenStateIds = stateDeclarationList.Where<WorkItemStateDeclaration>((Func<WorkItemStateDeclaration, bool>) (s => s.Hidden)).Select<WorkItemStateDeclaration, Guid?>((Func<WorkItemStateDeclaration, Guid?>) (s => s.Id)).ToList<Guid?>();
      List<WorkItemStateDeclaration> list = stateDeclarationList.Where<WorkItemStateDeclaration>((Func<WorkItemStateDeclaration, bool>) (s => !hiddenStateIds.Contains(s.Id))).ToList<WorkItemStateDeclaration>();
      this.ValidateWorkFlow(requestContext, list, parentStateCategories);
      this.FixStateOrder(stateDeclarationList);
      return (IList<WorkItemStateDeclaration>) stateDeclarationList.Where<WorkItemStateDeclaration>((Func<WorkItemStateDeclaration, bool>) (s => TFStringComparer.WorkItemTypeReferenceName.Equals(s.WorkItemTypeReferenceName, witRefName))).ToList<WorkItemStateDeclaration>();
    }

    private static string GetWorkItemStateDefinitionListString(
      IReadOnlyCollection<WorkItemStateDefinition> workItemStateDefinitions)
    {
      StringBuilder stringBuilder = new StringBuilder(100 * workItemStateDefinitions.Count);
      foreach (WorkItemStateDefinition itemStateDefinition in (IEnumerable<WorkItemStateDefinition>) workItemStateDefinitions)
        stringBuilder.Append(string.Format("[Name: {0}, Order: {1}, StateCategory: {2}, WorkitemTypeReferenceName: {3}, Hidden: {4} \n", (object) itemStateDefinition?.Name, (object) itemStateDefinition?.Order, (object) itemStateDefinition?.StateCategory, (object) itemStateDefinition?.WorkItemTypeReferenceName, (object) itemStateDefinition?.Hidden));
      return stringBuilder.ToString();
    }

    private static string GetWorkItemStateDeclarationListString(
      List<WorkItemStateDeclaration> workItemStateDeclarations)
    {
      StringBuilder stringBuilder = new StringBuilder(100 * workItemStateDeclarations.Count);
      foreach (WorkItemStateDeclaration stateDeclaration in workItemStateDeclarations)
        stringBuilder.Append(WorkItemStateDefinitionService.GetWorkItemStateDeclarationString(stateDeclaration) + " , ");
      return stringBuilder.ToString();
    }

    private static string GetWorkItemStateDeclarationString(WorkItemStateDeclaration declaration) => string.Format("[Name: {0}, Order: + {1}, StateCategory: {2}, WorkItemTypeReferenceName: {3}, Hidden: {4} ]", (object) declaration?.Name, (object) declaration?.Order, (object) declaration?.StateCategory, (object) declaration?.WorkItemTypeReferenceName, (object) declaration?.Hidden);

    private List<WorkItemStateDeclaration> UpdateMerge(
      IReadOnlyCollection<WorkItemStateDefinition> existingStateDefinitions,
      WorkItemStateDeclaration newState,
      WorkItemStateDefinition removeState)
    {
      List<WorkItemStateDeclaration> list1 = existingStateDefinitions.Select<WorkItemStateDefinition, WorkItemStateDeclaration>((Func<WorkItemStateDefinition, WorkItemStateDeclaration>) (s => WorkItemStateDeclaration.Create(s))).ToList<WorkItemStateDeclaration>();
      if (removeState != null)
        list1 = list1.Where<WorkItemStateDeclaration>((Func<WorkItemStateDeclaration, bool>) (s =>
        {
          Guid? id1 = s.Id;
          Guid id2 = removeState.Id;
          return (id1.HasValue ? (id1.HasValue ? (id1.GetValueOrDefault() == id2 ? 1 : 0) : 1) : 0) == 0 || s.Hidden != removeState.Hidden;
        })).ToList<WorkItemStateDeclaration>();
      List<WorkItemStateDeclaration> list2 = list1.Where<WorkItemStateDeclaration>((Func<WorkItemStateDeclaration, bool>) (s => s.Hidden)).ToList<WorkItemStateDeclaration>();
      List<WorkItemStateDeclaration> list3 = list1.Where<WorkItemStateDeclaration>((Func<WorkItemStateDeclaration, bool>) (s => !s.Hidden)).ToList<WorkItemStateDeclaration>();
      if (newState != null)
      {
        if (newState.Order >= 1 && newState.Order <= list3.Count)
          list3.Insert(newState.Order - 1, newState);
        else if (newState.Order == 0)
        {
          for (int index = list3.Count - 1; index >= 0; --index)
          {
            if (list3[index].StateCategory <= newState.StateCategory)
            {
              list3.Insert(index + 1, newState);
              break;
            }
            if (index == 0)
              list3.Insert(0, newState);
          }
        }
        else
          list3.Insert(list3.Count, newState);
      }
      foreach (WorkItemStateDeclaration stateDeclaration in list2)
        list3.Insert(list3.Count, stateDeclaration);
      return list3;
    }

    private void FixStateOrder(List<WorkItemStateDeclaration> states)
    {
      int num = 0;
      WorkItemStateCategory? nullable = new WorkItemStateCategory?();
      for (int index = 0; index < states.Count; ++index)
      {
        WorkItemStateDeclaration state = states[index];
        if (!state.Hidden)
        {
          if (nullable.HasValue && nullable.Value == state.StateCategory)
            ++num;
          else
            num = 0;
          state.Order = num;
          nullable = new WorkItemStateCategory?(state.StateCategory);
        }
      }
    }

    private List<WorkItemStateDefinition> ReadMerge(
      IReadOnlyCollection<WorkItemStateDefinition> parentStates,
      IReadOnlyCollection<WorkItemStateDefinition> childStates)
    {
      List<WorkItemStateDefinition> itemStateDefinitionList = new List<WorkItemStateDefinition>();
      List<WorkItemStateDefinition> list1 = parentStates.ToList<WorkItemStateDefinition>();
      List<WorkItemStateDefinition> list2 = childStates.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => !s.Hidden)).ToList<WorkItemStateDefinition>();
      int index1 = 0;
      int index2 = 0;
      int index3 = 0;
      int num = 0;
      while (index1 < list1.Count && index2 < list2.Count)
      {
        WorkItemStateDefinition itemStateDefinition1 = list1[index1];
        WorkItemStateDefinition itemStateDefinition2 = list2[index2];
        if (itemStateDefinition2.StateCategory == itemStateDefinition1.StateCategory)
        {
          if (itemStateDefinition2.Order == num)
          {
            itemStateDefinitionList.Insert(index3, itemStateDefinition2);
            ++index2;
          }
          else
          {
            itemStateDefinitionList.Insert(index3, itemStateDefinition1);
            ++index1;
          }
          ++num;
        }
        else if (itemStateDefinition2.StateCategory < itemStateDefinition1.StateCategory)
        {
          num = 0;
          itemStateDefinitionList.Insert(index3, itemStateDefinition2);
          ++index2;
        }
        else
        {
          num = 0;
          itemStateDefinitionList.Insert(index3, itemStateDefinition1);
          ++index1;
        }
        ++index3;
      }
      while (index1 < list1.Count)
      {
        itemStateDefinitionList.Insert(index3, list1[index1]);
        ++index1;
        ++index3;
      }
      while (index2 < list2.Count)
      {
        itemStateDefinitionList.Insert(index3, list2[index2]);
        ++index2;
        ++index3;
      }
      foreach (WorkItemStateDefinition itemStateDefinition in childStates.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.Hidden)))
      {
        itemStateDefinitionList.Insert(index3, itemStateDefinition);
        ++index3;
      }
      return itemStateDefinitionList;
    }

    private IReadOnlyCollection<WorkItemStateDefinition> CloneAndFixOrder(
      IList<WorkItemStateDefinition> states)
    {
      List<WorkItemStateDefinition> itemStateDefinitionList = new List<WorkItemStateDefinition>();
      for (int index = 0; index < states.Count; ++index)
        itemStateDefinitionList.Insert(index, states[index].Clone(index + 1));
      return (IReadOnlyCollection<WorkItemStateDefinition>) itemStateDefinitionList;
    }

    private void PublishStateDefinitionChangedEvent(
      IVssRequestContext requestContext,
      Guid processId,
      StateDefinitionChangeType changeType)
    {
      try
      {
        StateDefinitionChangedEvent notificationEvent = new StateDefinitionChangedEvent()
        {
          ProcessId = processId,
          ChangeType = changeType
        };
        requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) notificationEvent);
        requestContext.Trace(910514, TraceLevel.Info, "WorkItemStateDefinition", nameof (WorkItemStateDefinitionService), "StateDefinitionChangedEvent fired for process : {0}", (object) processId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(910515, "WorkItemStateDefinition", nameof (WorkItemStateDefinitionService), ex);
      }
    }

    private static void CheckStateLimit(IVssRequestContext requestContext, int newCount)
    {
      int statesPerWorkItemType = requestContext.WitContext().TemplateValidatorConfiguration.MaxCustomStatesPerWorkItemType;
      if (newCount > statesPerWorkItemType)
        throw new WorkItemTypeStateLimitExceededException(statesPerWorkItemType);
    }

    private static void CheckStateDeclaration(
      IVssRequestContext requestContext,
      WorkItemStateDeclaration stateDeclaration)
    {
      ArgumentUtility.CheckForNull<WorkItemStateDeclaration>(stateDeclaration, nameof (stateDeclaration));
      ArgumentUtility.CheckStringForNullOrEmpty(stateDeclaration.Name, "Name");
      ArgumentUtility.CheckStringForNullOrEmpty(stateDeclaration.Color, "Color");
      ArgumentUtility.CheckGreaterThanZero((float) stateDeclaration.StateCategory, "StateCategory");
      stateDeclaration.Name = stateDeclaration.Name.Trim();
      CommonWITUtils.CheckValidName(stateDeclaration.Name, 128, WorkItemTypeMetadata.IllegalStateNameChars);
      CommonWITUtils.CheckColor(stateDeclaration.Color);
    }

    private void ValidateWorkFlow(
      IVssRequestContext requestContext,
      List<WorkItemStateDeclaration> states,
      IEnumerable<WorkItemStateCategory> parentStateCategories)
    {
      WorkItemStateDefinitionService.CheckForOneActiveOneClosedState(states);
      for (int index = 0; index < states.Count - 1; ++index)
      {
        if (states[index].StateCategory > states[index + 1].StateCategory)
          throw new WorkItemStateOrderInvalidException();
      }
    }

    private static void CheckForOneActiveOneClosedState(List<WorkItemStateDeclaration> states)
    {
      if (states.FirstOrDefault<WorkItemStateDeclaration>((Func<WorkItemStateDeclaration, bool>) (s => s.StateCategory == WorkItemStateCategory.Proposed || s.StateCategory == WorkItemStateCategory.InProgress)) == null | states.FirstOrDefault<WorkItemStateDeclaration>((Func<WorkItemStateDeclaration, bool>) (s => s.StateCategory == WorkItemStateCategory.Completed)) == null)
        throw new WorkItemTypeTwoStateRestrictionException();
    }

    private static void BlockChangesToUnchangeableStateCategories(
      WorkItemStateCategory stateCategory)
    {
      if (WorkItemStateDefinitionService.UnchangeableStateCategories.Contains(stateCategory))
        throw new WorkItemStateBlockCompletedCategoryChangesException();
    }

    public IEnumerable<WorkItemFieldRule> GenerateWorkFlowRules(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName)
    {
      IReadOnlyCollection<WorkItemStateDefinition> stateDefinitions = this.GetCombinedStateDefinitions(requestContext, processId, witReferenceName, false);
      CultureInfo culture = requestContext.ServiceHost.GetCulture(requestContext);
      return (IEnumerable<WorkItemFieldRule>) new List<WorkItemFieldRule>()
      {
        WorkItemStateDefinitionService.GenerateStateRule(stateDefinitions),
        WorkItemStateDefinitionService.GenerateReasonRule(stateDefinitions, culture)
      };
    }

    private static WorkItemFieldRule GenerateReasonRule(
      IReadOnlyCollection<WorkItemStateDefinition> states,
      CultureInfo culture)
    {
      WorkItemFieldRule reasonRule = new WorkItemFieldRule()
      {
        FieldId = 22,
        Field = "System.Reason"
      };
      WhenChangedRule whenChangedRule = new WhenChangedRule();
      whenChangedRule.Inverse = true;
      whenChangedRule.FieldId = 2;
      WhenChangedRule rule1 = whenChangedRule;
      rule1.AddRule<ReadOnlyRule>(new ReadOnlyRule());
      reasonRule.AddRule<WhenChangedRule>(rule1);
      foreach (WorkItemStateDefinition state in (IEnumerable<WorkItemStateDefinition>) states)
      {
        WhenRule whenRule1 = new WhenRule();
        whenRule1.FieldId = 2;
        whenRule1.Value = state.Name;
        WhenRule rule2 = whenRule1;
        WhenRule whenRule2 = rule2;
        CopyRule rule3 = new CopyRule();
        rule3.Value = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateDefaultReason((object) state.Name, culture);
        whenRule2.AddRule<CopyRule>(rule3);
        rule2.AddRule<RequiredRule>(new RequiredRule());
        reasonRule.AddRule<WhenRule>(rule2);
      }
      return reasonRule;
    }

    private static WorkItemFieldRule GenerateStateRule(
      IReadOnlyCollection<WorkItemStateDefinition> states)
    {
      WorkItemFieldRule stateRule = new WorkItemFieldRule()
      {
        FieldId = 2,
        Field = "System.State"
      };
      if (states.Any<WorkItemStateDefinition>())
      {
        HashSet<string> stringSet = new HashSet<string>(states.Select<WorkItemStateDefinition, string>((Func<WorkItemStateDefinition, string>) (s => s.Name)), (IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
        WorkItemFieldRule workItemFieldRule1 = stateRule;
        AllowedValuesRule rule1 = new AllowedValuesRule();
        rule1.Values = stringSet;
        workItemFieldRule1.AddRule<AllowedValuesRule>(rule1);
        WorkItemFieldRule workItemFieldRule2 = stateRule;
        DefaultRule rule2 = new DefaultRule();
        rule2.Value = states.FirstOrDefault<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.Order == 1)).Name;
        workItemFieldRule2.AddRule<DefaultRule>(rule2);
        WhenWasRule whenWasRule1 = new WhenWasRule();
        whenWasRule1.FieldId = 2;
        whenWasRule1.Value = string.Empty;
        WhenWasRule rule3 = whenWasRule1;
        WhenWasRule whenWasRule2 = rule3;
        AllowedValuesRule rule4 = new AllowedValuesRule();
        rule4.Values = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName)
        {
          states.FirstOrDefault<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.Order == 1)).Name
        };
        whenWasRule2.AddRule<AllowedValuesRule>(rule4);
        stateRule.AddRule<WhenWasRule>(rule3);
        foreach (WorkItemStateDefinition state in (IEnumerable<WorkItemStateDefinition>) states)
        {
          WhenWasRule whenWasRule3 = new WhenWasRule();
          whenWasRule3.FieldId = 2;
          whenWasRule3.Value = state.Name;
          WhenWasRule rule5 = whenWasRule3;
          WhenWasRule whenWasRule4 = rule5;
          AllowedValuesRule rule6 = new AllowedValuesRule();
          rule6.Values = stringSet;
          whenWasRule4.AddRule<AllowedValuesRule>(rule6);
          stateRule.AddRule<WhenWasRule>(rule5);
        }
      }
      return stateRule;
    }

    public IEnumerable<WorkItemFieldRule> GenerateStateCategoryTransitionRules(
      IVssRequestContext requestContext,
      WorkItemType workItemType)
    {
      if (!WorkItemStateDefinitionService.IsWitCustomized(workItemType.IsDerived, workItemType.IsCustomType))
        return Enumerable.Empty<WorkItemFieldRule>();
      ProcessDescriptor processDescriptor = requestContext.GetService<IWorkItemTrackingProcessService>().GetProjectProcessDescriptor(requestContext.Elevate(), workItemType.ProjectId);
      IEnumerable<WorkItemStateDefinition> stateDefinitions = (IEnumerable<WorkItemStateDefinition>) this.GetCombinedStateDefinitions(requestContext, processDescriptor.TypeId, workItemType.ReferenceName, false);
      IEnumerable<FieldEntry> fields = workItemType.GetFields(requestContext, true);
      return WorkItemStateDefinitionService.GenerateStateCategoryTransitionRules(requestContext, stateDefinitions, fields);
    }

    public IEnumerable<WorkItemFieldRule> GenerateStateCategoryTransitionRules(
      IVssRequestContext requestContext,
      ProcessWorkItemType processWit)
    {
      if (!WorkItemStateDefinitionService.IsWitCustomized(processWit.IsDerived, processWit.IsCustomType))
        return Enumerable.Empty<WorkItemFieldRule>();
      IEnumerable<WorkItemStateDefinition> stateDefinitions = (IEnumerable<WorkItemStateDefinition>) this.GetCombinedStateDefinitions(requestContext, processWit.ProcessId, processWit.ReferenceName, false);
      IEnumerable<FieldEntry> combinedFields = (IEnumerable<FieldEntry>) processWit.GetCombinedFields(requestContext);
      return WorkItemStateDefinitionService.GenerateStateCategoryTransitionRules(requestContext, stateDefinitions, combinedFields);
    }

    private static IEnumerable<WorkItemFieldRule> GenerateStateCategoryTransitionRules(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemStateDefinition> states,
      IEnumerable<FieldEntry> fields)
    {
      List<WorkItemFieldRule> categoryTransitionRules = new List<WorkItemFieldRule>();
      FieldEntry fieldEntry1 = fields.FirstOrDefault<FieldEntry>((Func<FieldEntry, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.ReferenceName, "Microsoft.VSTS.Common.ActivatedBy")));
      if (fieldEntry1 != null)
      {
        WorkItemFieldRule transitionByFieldRule = WorkItemStateDefinitionService.GenerateStateCategoryTransitionByFieldRule(states, fieldEntry1.FieldId, "Microsoft.VSTS.Common.ActivatedBy", WorkItemStateCategory.InProgress);
        categoryTransitionRules.Add(transitionByFieldRule);
      }
      FieldEntry fieldEntry2 = fields.FirstOrDefault<FieldEntry>((Func<FieldEntry, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.ReferenceName, "Microsoft.VSTS.Common.ActivatedDate")));
      if (fieldEntry2 != null)
      {
        WorkItemFieldRule transitionDateFieldRule = WorkItemStateDefinitionService.GenerateStateCategoryTransitionDateFieldRule(states, fieldEntry2.FieldId, "Microsoft.VSTS.Common.ActivatedDate", WorkItemStateCategory.InProgress);
        categoryTransitionRules.Add(transitionDateFieldRule);
      }
      FieldEntry fieldEntry3 = fields.FirstOrDefault<FieldEntry>((Func<FieldEntry, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.ReferenceName, "Microsoft.VSTS.Common.ResolvedBy")));
      if (fieldEntry3 != null)
      {
        WorkItemFieldRule transitionByFieldRule = WorkItemStateDefinitionService.GenerateStateCategoryTransitionByFieldRule(states, fieldEntry3.FieldId, "Microsoft.VSTS.Common.ResolvedBy", WorkItemStateCategory.Resolved);
        WorkItemStateDefinitionService.AddRulesForSettingResolvedFieldsWhenTransitionFromCustomStateToInProgressResolved(requestContext, states, transitionByFieldRule, ResolvedFieldType.By);
        categoryTransitionRules.Add(transitionByFieldRule);
      }
      FieldEntry fieldEntry4 = fields.FirstOrDefault<FieldEntry>((Func<FieldEntry, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.ReferenceName, "Microsoft.VSTS.Common.ResolvedDate")));
      if (fieldEntry4 != null)
      {
        WorkItemFieldRule transitionDateFieldRule = WorkItemStateDefinitionService.GenerateStateCategoryTransitionDateFieldRule(states, fieldEntry4.FieldId, "Microsoft.VSTS.Common.ResolvedDate", WorkItemStateCategory.Resolved);
        WorkItemStateDefinitionService.AddRulesForSettingResolvedFieldsWhenTransitionFromCustomStateToInProgressResolved(requestContext, states, transitionDateFieldRule, ResolvedFieldType.Date);
        categoryTransitionRules.Add(transitionDateFieldRule);
      }
      FieldEntry fieldEntry5 = fields.FirstOrDefault<FieldEntry>((Func<FieldEntry, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.ReferenceName, "Microsoft.VSTS.Common.ClosedBy")));
      if (fieldEntry5 != null)
      {
        WorkItemFieldRule transitionByFieldRule = WorkItemStateDefinitionService.GenerateStateCategoryTransitionByFieldRule(states, fieldEntry5.FieldId, "Microsoft.VSTS.Common.ClosedBy", WorkItemStateCategory.Completed);
        categoryTransitionRules.Add(transitionByFieldRule);
      }
      FieldEntry fieldEntry6 = fields.FirstOrDefault<FieldEntry>((Func<FieldEntry, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.ReferenceName, "Microsoft.VSTS.Common.ClosedDate")));
      if (fieldEntry6 != null)
      {
        WorkItemFieldRule transitionDateFieldRule = WorkItemStateDefinitionService.GenerateStateCategoryTransitionDateFieldRule(states, fieldEntry6.FieldId, "Microsoft.VSTS.Common.ClosedDate", WorkItemStateCategory.Completed);
        if (WorkItemTrackingFeatureFlags.IsSetClosedDateIfWITStateIsCompletedEnabled(requestContext))
          WorkItemStateDefinitionService.AddWhenChangedStateCategoryTransitionDateFieldRule(states, transitionDateFieldRule, WorkItemStateCategory.Completed);
        categoryTransitionRules.Add(transitionDateFieldRule);
      }
      return (IEnumerable<WorkItemFieldRule>) categoryTransitionRules;
    }

    private static WorkItemFieldRule GenerateStateCategoryTransitionByFieldRule(
      IEnumerable<WorkItemStateDefinition> states,
      int fieldId,
      string fieldRefName,
      WorkItemStateCategory destinationStateCategory)
    {
      WorkItemFieldRule transitionByFieldRule = new WorkItemFieldRule()
      {
        FieldId = fieldId,
        Field = fieldRefName
      };
      foreach (WorkItemStateDefinition itemStateDefinition in states.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.StateCategory < destinationStateCategory)))
      {
        WorkItemFieldRule workItemFieldRule = transitionByFieldRule;
        WhenRule whenRule1 = new WhenRule();
        whenRule1.FieldId = 2;
        whenRule1.Value = itemStateDefinition.Name;
        WhenRule whenRule2 = whenRule1;
        CopyRule[] copyRuleArray = new CopyRule[1];
        CopyRule copyRule = new CopyRule();
        copyRule.Value = string.Empty;
        copyRuleArray[0] = copyRule;
        WorkItemRule[] workItemRuleArray = (WorkItemRule[]) copyRuleArray;
        whenRule2.SubRules = workItemRuleArray;
        WhenRule rule = whenRule1;
        workItemFieldRule.AddRule<WhenRule>(rule);
      }
      foreach (WorkItemStateDefinition itemStateDefinition1 in states.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.StateCategory == destinationStateCategory)))
      {
        foreach (WorkItemStateDefinition itemStateDefinition2 in states.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.StateCategory != destinationStateCategory)))
        {
          WorkItemFieldRule workItemFieldRule = transitionByFieldRule;
          WhenRule whenRule3 = new WhenRule();
          whenRule3.FieldId = 2;
          whenRule3.Value = itemStateDefinition1.Name;
          WhenRule whenRule4 = whenRule3;
          WhenWasRule[] whenWasRuleArray = new WhenWasRule[1];
          WhenWasRule whenWasRule1 = new WhenWasRule();
          whenWasRule1.FieldId = 2;
          whenWasRule1.Value = itemStateDefinition2.Name;
          WhenWasRule whenWasRule2 = whenWasRule1;
          WorkItemRule[] workItemRuleArray1 = new WorkItemRule[3];
          CopyRule copyRule = new CopyRule();
          copyRule.ValueFrom = RuleValueFrom.CurrentUser;
          workItemRuleArray1[0] = (WorkItemRule) copyRule;
          workItemRuleArray1[1] = (WorkItemRule) new ValidUserRule();
          workItemRuleArray1[2] = (WorkItemRule) new AllowExistingValueRule();
          whenWasRule2.SubRules = workItemRuleArray1;
          whenWasRuleArray[0] = whenWasRule1;
          WorkItemRule[] workItemRuleArray2 = (WorkItemRule[]) whenWasRuleArray;
          whenRule4.SubRules = workItemRuleArray2;
          WhenRule rule = whenRule3;
          workItemFieldRule.AddRule<WhenRule>(rule);
        }
      }
      return transitionByFieldRule;
    }

    private static WorkItemFieldRule GenerateStateCategoryTransitionDateFieldRule(
      IEnumerable<WorkItemStateDefinition> states,
      int fieldId,
      string fieldRefName,
      WorkItemStateCategory destinationStateCategory)
    {
      WorkItemFieldRule transitionDateFieldRule = new WorkItemFieldRule()
      {
        FieldId = fieldId,
        Field = fieldRefName
      };
      foreach (WorkItemStateDefinition itemStateDefinition in states.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.StateCategory < destinationStateCategory)))
      {
        WorkItemFieldRule workItemFieldRule = transitionDateFieldRule;
        WhenRule whenRule1 = new WhenRule();
        whenRule1.FieldId = 2;
        whenRule1.Value = itemStateDefinition.Name;
        WhenRule whenRule2 = whenRule1;
        CopyRule[] copyRuleArray = new CopyRule[1];
        CopyRule copyRule = new CopyRule();
        copyRule.Value = string.Empty;
        copyRuleArray[0] = copyRule;
        WorkItemRule[] workItemRuleArray = (WorkItemRule[]) copyRuleArray;
        whenRule2.SubRules = workItemRuleArray;
        WhenRule rule = whenRule1;
        workItemFieldRule.AddRule<WhenRule>(rule);
      }
      foreach (WorkItemStateDefinition itemStateDefinition1 in states.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.StateCategory == destinationStateCategory)))
      {
        foreach (WorkItemStateDefinition itemStateDefinition2 in states.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.StateCategory != destinationStateCategory)))
        {
          WorkItemFieldRule workItemFieldRule = transitionDateFieldRule;
          WhenRule whenRule3 = new WhenRule();
          whenRule3.FieldId = 2;
          whenRule3.Value = itemStateDefinition1.Name;
          WhenRule whenRule4 = whenRule3;
          WhenWasRule[] whenWasRuleArray = new WhenWasRule[1];
          WhenWasRule whenWasRule = new WhenWasRule();
          whenWasRule.FieldId = 2;
          whenWasRule.Value = itemStateDefinition2.Name;
          whenWasRule.SubRules = new WorkItemRule[1]
          {
            (WorkItemRule) new ServerDefaultRule()
            {
              From = ServerDefaultType.ServerDateTime
            }
          };
          whenWasRuleArray[0] = whenWasRule;
          WorkItemRule[] workItemRuleArray = (WorkItemRule[]) whenWasRuleArray;
          whenRule4.SubRules = workItemRuleArray;
          WhenRule rule = whenRule3;
          workItemFieldRule.AddRule<WhenRule>(rule);
        }
      }
      return transitionDateFieldRule;
    }

    private static void AddWhenChangedStateCategoryTransitionDateFieldRule(
      IEnumerable<WorkItemStateDefinition> states,
      WorkItemFieldRule fieldRule,
      WorkItemStateCategory destinationStateCategory)
    {
      foreach (WorkItemStateDefinition itemStateDefinition in states.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.StateCategory == destinationStateCategory)))
      {
        WorkItemFieldRule workItemFieldRule = fieldRule;
        WhenRule whenRule1 = new WhenRule();
        whenRule1.FieldId = 2;
        whenRule1.Value = itemStateDefinition.Name;
        WhenRule whenRule2 = whenRule1;
        WhenChangedRule[] whenChangedRuleArray = new WhenChangedRule[1];
        WhenChangedRule whenChangedRule = new WhenChangedRule();
        whenChangedRule.FieldId = 2;
        whenChangedRule.SubRules = new WorkItemRule[1]
        {
          (WorkItemRule) new ServerDefaultRule()
          {
            From = ServerDefaultType.ServerDateTime
          }
        };
        whenChangedRuleArray[0] = whenChangedRule;
        WorkItemRule[] workItemRuleArray = (WorkItemRule[]) whenChangedRuleArray;
        whenRule2.SubRules = workItemRuleArray;
        WhenRule rule = whenRule1;
        workItemFieldRule.AddRule<WhenRule>(rule);
      }
    }

    private static void AddRulesForSettingResolvedFieldsWhenTransitionFromCustomStateToInProgressResolved(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemStateDefinition> states,
      WorkItemFieldRule fieldRule,
      ResolvedFieldType type)
    {
      string resolvedStateName = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.Resolved(requestContext.ServiceHost.GetCulture(requestContext));
      if (!states.Any<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.StateCategory == WorkItemStateCategory.InProgress && s.Customization == CustomizationType.System && s.Name == resolvedStateName)))
        return;
      WorkItemRule[] workItemRuleArray1;
      switch (type)
      {
        case ResolvedFieldType.By:
          WorkItemRule[] workItemRuleArray2 = new WorkItemRule[3];
          CopyRule copyRule = new CopyRule();
          copyRule.ValueFrom = RuleValueFrom.CurrentUser;
          workItemRuleArray2[0] = (WorkItemRule) copyRule;
          workItemRuleArray2[1] = (WorkItemRule) new ValidUserRule();
          workItemRuleArray2[2] = (WorkItemRule) new AllowExistingValueRule();
          workItemRuleArray1 = workItemRuleArray2;
          break;
        case ResolvedFieldType.Date:
          workItemRuleArray1 = new WorkItemRule[1]
          {
            (WorkItemRule) new ServerDefaultRule()
            {
              From = ServerDefaultType.ServerDateTime
            }
          };
          break;
        default:
          return;
      }
      List<WorkItemRule> workItemRuleList1 = new List<WorkItemRule>();
      WhenRule whenRule = new WhenRule();
      whenRule.FieldId = 2;
      whenRule.Value = resolvedStateName;
      whenRule.SubRules = workItemRuleArray1;
      workItemRuleList1.Add((WorkItemRule) whenRule);
      List<WorkItemRule> workItemRuleList2 = workItemRuleList1;
      foreach (WorkItemStateDefinition itemStateDefinition in states.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.StateCategory == WorkItemStateCategory.InProgress && s.Customization == CustomizationType.Custom)))
      {
        WorkItemFieldRule workItemFieldRule = fieldRule;
        WhenWasRule rule = new WhenWasRule();
        rule.FieldId = 2;
        rule.Value = itemStateDefinition.Name;
        rule.SubRules = workItemRuleList2.ToArray();
        workItemFieldRule.AddRule<WhenWasRule>(rule);
      }
    }

    public IEnumerable<WorkItemFieldRule> GenerateStateChangeDateFieldRules(
      IVssRequestContext requestContext)
    {
      FieldEntry field;
      if (!requestContext.WitContext().FieldDictionary.TryGetFieldByNameOrId("Microsoft.VSTS.Common.StateChangeDate", out field))
        return Enumerable.Empty<WorkItemFieldRule>();
      WorkItemFieldRule[] changeDateFieldRules = new WorkItemFieldRule[1];
      WorkItemFieldRule workItemFieldRule1 = new WorkItemFieldRule();
      workItemFieldRule1.FieldId = field.FieldId;
      workItemFieldRule1.Field = "Microsoft.VSTS.Common.StateChangeDate";
      WorkItemFieldRule workItemFieldRule2 = workItemFieldRule1;
      WhenChangedRule[] whenChangedRuleArray = new WhenChangedRule[2];
      WhenChangedRule whenChangedRule1 = new WhenChangedRule();
      whenChangedRule1.FieldId = 2;
      whenChangedRule1.Inverse = true;
      whenChangedRule1.SubRules = (WorkItemRule[]) new ReadOnlyRule[1]
      {
        new ReadOnlyRule()
      };
      whenChangedRuleArray[0] = whenChangedRule1;
      WhenChangedRule whenChangedRule2 = new WhenChangedRule();
      whenChangedRule2.FieldId = 2;
      whenChangedRule2.SubRules = (WorkItemRule[]) new ServerDefaultRule[1]
      {
        new ServerDefaultRule()
        {
          From = ServerDefaultType.ServerDateTime
        }
      };
      whenChangedRuleArray[1] = whenChangedRule2;
      WorkItemRule[] workItemRuleArray = (WorkItemRule[]) whenChangedRuleArray;
      workItemFieldRule2.SubRules = workItemRuleArray;
      changeDateFieldRules[0] = workItemFieldRule1;
      return (IEnumerable<WorkItemFieldRule>) changeDateFieldRules;
    }

    private static bool IsWitCustomized(bool witIsDerived, bool witIsCustomType) => witIsDerived | witIsCustomType;

    internal static IReadOnlyCollection<WorkItemStateDeclaration> GetDefaultStateModel(
      ProcessDescriptor processDescriptor,
      CultureInfo culture)
    {
      IReadOnlyCollection<WorkItemStateDeclaration> defaultStateModel = WorkItemStateDefinitionService.GetBasicDefaultStateModel(culture);
      if (processDescriptor.IsDerived)
      {
        if (processDescriptor.Inherits == ProcessTemplateTypeIdentifiers.MsfCmmiProcessImprovement)
          defaultStateModel = WorkItemStateDefinitionService.GetCmmiDefaultStateModel(culture);
        else if (processDescriptor.Inherits == ProcessTemplateTypeIdentifiers.VisualStudioScrum)
          defaultStateModel = WorkItemStateDefinitionService.GetScrumDefaultStateModel(culture);
        else if (processDescriptor.Inherits == ProcessTemplateTypeIdentifiers.MsfAgileSoftwareDevelopment)
          defaultStateModel = WorkItemStateDefinitionService.GetAgileDefaultStateModel(culture);
      }
      return defaultStateModel;
    }

    internal static IReadOnlyCollection<WorkItemStateDeclaration> GetBasicDefaultStateModel(
      CultureInfo culture)
    {
      return (IReadOnlyCollection<WorkItemStateDeclaration>) new WorkItemStateDeclaration[3]
      {
        new WorkItemStateDeclaration()
        {
          Name = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateToDo(culture),
          Color = "b2b2b2",
          Order = 0,
          StateCategory = WorkItemStateCategory.Proposed
        },
        new WorkItemStateDeclaration()
        {
          Name = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateDoing(culture),
          Color = "007acc",
          Order = 0,
          StateCategory = WorkItemStateCategory.InProgress
        },
        new WorkItemStateDeclaration()
        {
          Name = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateDone(culture),
          Color = "339933",
          Order = 0,
          StateCategory = WorkItemStateCategory.Completed
        }
      };
    }

    internal static IReadOnlyCollection<WorkItemStateDeclaration> GetAgileDefaultStateModel(
      CultureInfo culture)
    {
      return (IReadOnlyCollection<WorkItemStateDeclaration>) new WorkItemStateDeclaration[3]
      {
        new WorkItemStateDeclaration()
        {
          Name = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateNew(culture),
          Color = "b2b2b2",
          Order = 0,
          StateCategory = WorkItemStateCategory.Proposed
        },
        new WorkItemStateDeclaration()
        {
          Name = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateActive(culture),
          Color = "007acc",
          Order = 0,
          StateCategory = WorkItemStateCategory.InProgress
        },
        new WorkItemStateDeclaration()
        {
          Name = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateClosed(culture),
          Color = "339933",
          Order = 0,
          StateCategory = WorkItemStateCategory.Completed
        }
      };
    }

    internal static IReadOnlyCollection<WorkItemStateDeclaration> GetScrumDefaultStateModel(
      CultureInfo culture)
    {
      return (IReadOnlyCollection<WorkItemStateDeclaration>) new WorkItemStateDeclaration[3]
      {
        new WorkItemStateDeclaration()
        {
          Name = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateNew(culture),
          Color = "b2b2b2",
          Order = 0,
          StateCategory = WorkItemStateCategory.Proposed
        },
        new WorkItemStateDeclaration()
        {
          Name = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateCommitted(culture),
          Color = "007acc",
          Order = 0,
          StateCategory = WorkItemStateCategory.InProgress
        },
        new WorkItemStateDeclaration()
        {
          Name = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateDone(culture),
          Color = "339933",
          Order = 0,
          StateCategory = WorkItemStateCategory.Completed
        }
      };
    }

    internal static IReadOnlyCollection<WorkItemStateDeclaration> GetCmmiDefaultStateModel(
      CultureInfo culture)
    {
      return (IReadOnlyCollection<WorkItemStateDeclaration>) new WorkItemStateDeclaration[3]
      {
        new WorkItemStateDeclaration()
        {
          Name = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateProposed(culture),
          Color = "b2b2b2",
          Order = 0,
          StateCategory = WorkItemStateCategory.Proposed
        },
        new WorkItemStateDeclaration()
        {
          Name = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateActive(culture),
          Color = "007acc",
          Order = 0,
          StateCategory = WorkItemStateCategory.InProgress
        },
        new WorkItemStateDeclaration()
        {
          Name = Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.StateClosed(culture),
          Color = "339933",
          Order = 0,
          StateCategory = WorkItemStateCategory.Completed
        }
      };
    }

    internal static IReadOnlyCollection<string> CombinedWorkFlowFieldReferenceName => WorkItemStateDefinitionService.CombinedWorkFlowFieldReferenceNames;
  }
}
