// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.BehaviorService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class BehaviorService : IBehaviorService, IVssFrameworkService
  {
    public static readonly string PortfolioBehaviorReferenceName = "System.PortfolioBacklogBehavior";
    public static readonly string RequirementBehaviorReferenceName = "System.RequirementBacklogBehavior";
    public static readonly string TaskBehaviorReferenceName = "System.TaskBacklogBehavior";
    public static readonly string OrderedBehaviorReferenceName = "System.OrderedBehavior";
    public static readonly string DefaultBehaviorColor = "FFFFFF";
    public static readonly int TaskBacklogRank = 10;
    public static readonly int RequirementBacklogRank = 20;
    public static readonly int BacklogLevelIncrements = 10;

    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public IReadOnlyCollection<Behavior> GetBehaviors(
      IVssRequestContext requestContext,
      Guid processId,
      bool includeParent = true,
      bool bypassCache = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      return requestContext.TraceBlock<IReadOnlyCollection<Behavior>>(911502, 911503, "WorkItemType", nameof (BehaviorService), nameof (GetBehaviors), (Func<IReadOnlyCollection<Behavior>>) (() => this.GetBehaviorsInternal(requestContext, processId, includeParent, bypassCache)));
    }

    public Behavior GetBehavior(
      IVssRequestContext requestContext,
      Guid processId,
      string behaviorReferenceName,
      bool includeParent = true,
      bool bypassCache = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(behaviorReferenceName, nameof (behaviorReferenceName));
      return requestContext.TraceBlock<Behavior>(911506, 911507, "WorkItemType", nameof (BehaviorService), nameof (GetBehavior), (Func<Behavior>) (() =>
      {
        Behavior behavior;
        if (!this.TryGetBehavior(requestContext, processId, behaviorReferenceName, out behavior, includeParent, bypassCache))
          throw new BehaviorDoesNotExistException(behaviorReferenceName, requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, processId).Name);
        return behavior;
      }));
    }

    public DeletedBehaviors GetBehaviorReferenceNamesDeletedSince(
      IVssRequestContext requestContext,
      DateTime deletedSince)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.TraceBlock<DeletedBehaviors>(911514, 911515, "WorkItemType", nameof (BehaviorService), "GetBehavior", (Func<DeletedBehaviors>) (() =>
      {
        DateTime asOf = DateTime.MinValue;
        List<Tuple<string, DateTime>> tupleList = (List<Tuple<string, DateTime>>) null;
        int typeletType = 2;
        using (WorkItemTypeExtensionComponent component = requestContext.CreateComponent<WorkItemTypeExtensionComponent>())
          tupleList = component.GetWorkItemTypeletsReferenceNamesDeletedSince(deletedSince, typeletType, out asOf);
        DeletedBehaviors namesDeletedSince = new DeletedBehaviors();
        namesDeletedSince.BehaviorsDeleted = (IDictionary<string, DateTime>) new Dictionary<string, DateTime>((IEqualityComparer<string>) TFStringComparer.BehaviorReferenceName);
        namesDeletedSince.AsOf = asOf;
        namesDeletedSince.DeletedSince = deletedSince;
        foreach (Tuple<string, DateTime> tuple in tupleList)
          namesDeletedSince.BehaviorsDeleted[tuple.Item1] = tuple.Item2;
        return namesDeletedSince;
      }));
    }

    public bool TryGetBehavior(
      IVssRequestContext requestContext,
      Guid processId,
      string behaviorReferenceName,
      out Behavior behavior,
      bool includeParent = true,
      bool bypassCache = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(behaviorReferenceName, nameof (behaviorReferenceName));
      try
      {
        requestContext.TraceEnter(911508, "WorkItemType", nameof (BehaviorService), nameof (TryGetBehavior));
        behavior = this.GetBehaviorsInternal(requestContext, processId, includeParent, bypassCache).FirstOrDefault<Behavior>((Func<Behavior, bool>) (b => TFStringComparer.BehaviorReferenceName.Equals(b.ReferenceName, behaviorReferenceName)));
        return behavior != null;
      }
      finally
      {
        requestContext.TraceLeave(911510, "WorkItemType", nameof (BehaviorService), nameof (TryGetBehavior));
      }
    }

    public IReadOnlyCollection<Behavior> GetPortfolioBehaviors(
      IVssRequestContext requestContext,
      Guid processId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      return (IReadOnlyCollection<Behavior>) requestContext.TraceBlock<List<Behavior>>(911504, 911505, "WorkItemType", nameof (BehaviorService), nameof (GetPortfolioBehaviors), (Func<List<Behavior>>) (() => this.GetBehaviorsInternal(requestContext, processId, true).Where<Behavior>((Func<Behavior, bool>) (b => TFStringComparer.BehaviorReferenceName.Equals(b.ParentBehaviorReferenceName, BehaviorService.PortfolioBehaviorReferenceName))).ToList<Behavior>()));
    }

    public IReadOnlyCollection<Behavior> GetBacklogBehaviors(
      IVssRequestContext requestContext,
      Guid processId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      return (IReadOnlyCollection<Behavior>) requestContext.TraceBlock<List<Behavior>>(911511, 911512, "WorkItemType", nameof (BehaviorService), nameof (GetBacklogBehaviors), (Func<List<Behavior>>) (() => this.GetBehaviorsInternal(requestContext, processId, true).Where<Behavior>((Func<Behavior, bool>) (b => this.IsOrderedBehavior(b))).ToList<Behavior>()));
    }

    public Behavior CreateBehavior(
      IVssRequestContext requestContext,
      Guid processId,
      string parentBehaviorRefName,
      string name,
      string color = null,
      string categoryReferenceName = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(name, nameof (name));
      return requestContext.TraceBlock<Behavior>(911508, 911509, "WorkItemType", nameof (BehaviorService), nameof (CreateBehavior), (Func<Behavior>) (() =>
      {
        if (!TFStringComparer.BehaviorReferenceName.Equals(parentBehaviorRefName, BehaviorService.PortfolioBehaviorReferenceName))
          throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.BehaviorCreationInvalidParent((object) parentBehaviorRefName, (object) BehaviorService.PortfolioBehaviorReferenceName));
        name = CommonWITUtils.RemoveASCIIControlCharactersAndTrim(name);
        CommonWITUtils.CheckValidName(name, 128);
        if (string.IsNullOrWhiteSpace(color))
          color = BehaviorService.DefaultBehaviorColor;
        CommonWITUtils.CheckColor(color);
        WorkItemTrackingProcessService.CheckEditPermission(requestContext, processId);
        ProcessDescriptor processDescriptor = requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, processId);
        string behaviorReferenceName = (string) null;
        behaviorReferenceName = categoryReferenceName == null ? CommonWITUtils.GenerateUniqueRefName() : categoryReferenceName;
        IReadOnlyCollection<Behavior> behaviorsInternal = this.GetBehaviorsInternal(requestContext, processId, true, true);
        if (behaviorsInternal.Any<Behavior>((Func<Behavior, bool>) (t => TFStringComparer.BehaviorReferenceName.Equals(t.ReferenceName, behaviorReferenceName))))
          throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.BehaviorReferenceNameInUse());
        if (behaviorsInternal.Any<Behavior>((Func<Behavior, bool>) (t => TFStringComparer.BehaviorName.Equals(t.Name, name))))
          throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.BehaviorNameInUse());
        List<Behavior> list1 = behaviorsInternal.Where<Behavior>((Func<Behavior, bool>) (b => TFStringComparer.BehaviorReferenceName.Equals(b.ParentBehaviorReferenceName, BehaviorService.PortfolioBehaviorReferenceName))).ToList<Behavior>();
        int portfolioBacklogLevels = requestContext.WitContext().TemplateValidatorConfiguration.MaxPortfolioBacklogLevels;
        if (list1.Count >= portfolioBacklogLevels)
          throw new PortfolioBehaviorLimitExceededException(portfolioBacklogLevels);
        int limitCount = portfolioBacklogLevels;
        if (processDescriptor.IsDerived)
          limitCount = portfolioBacklogLevels - list1.Count<Behavior>((Func<Behavior, bool>) (pb => pb.ProcessId == processDescriptor.Inherits || pb.Overridden));
        int num = BehaviorService.RequirementBacklogRank;
        List<int> list2 = list1.Select<Behavior, int>((Func<Behavior, int>) (b => b.Rank)).ToList<int>();
        if (list2.Any<int>())
          num = list2.Max();
        int rank = num + BehaviorService.BacklogLevelIncrements;
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        WorkItemTypeletRecord behavior1;
        using (WorkItemTypeExtensionComponent component = requestContext.CreateComponent<WorkItemTypeExtensionComponent>())
          behavior1 = component.CreateBehavior(Guid.NewGuid(), processId, name, behaviorReferenceName, BehaviorService.PortfolioBehaviorReferenceName, id, color, rank, false, limitCount);
        WorkItemTrackingFieldService service = requestContext.GetService<WorkItemTrackingFieldService>();
        Behavior behavior2 = Behavior.Create(requestContext, behavior1, service);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (BehaviorService), "CreatePortfolioBehavior", "CustomPortfolioBehaviorName", name);
        return behavior2;
      }));
    }

    public Behavior UpdateBehavior(
      IVssRequestContext requestContext,
      Guid processId,
      string behaviorReferenceName,
      string name,
      string color = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(behaviorReferenceName, nameof (behaviorReferenceName));
      return requestContext.TraceBlock<Behavior>(911510, 911511, "WorkItemType", nameof (BehaviorService), nameof (UpdateBehavior), (Func<Behavior>) (() =>
      {
        WorkItemTrackingProcessService.CheckEditPermission(requestContext, processId);
        if (string.IsNullOrWhiteSpace(name))
          throw new VssPropertyValidationException(nameof (name), Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.NullOrEmptyParameter((object) "Behavior name"));
        if (!string.IsNullOrWhiteSpace(color))
          CommonWITUtils.CheckColor(color);
        IReadOnlyCollection<Behavior> behaviorsInternal = this.GetBehaviorsInternal(requestContext, processId, true, true);
        Behavior behavior = behaviorsInternal.FirstOrDefault<Behavior>((Func<Behavior, bool>) (b => TFStringComparer.BehaviorReferenceName.Equals(b.ReferenceName, behaviorReferenceName)));
        if (behavior == null)
          throw new BehaviorDoesNotExistException(behaviorReferenceName, processId.ToString());
        if (string.IsNullOrWhiteSpace(color))
          color = behavior.Color;
        if (string.IsNullOrWhiteSpace(color))
          color = BehaviorService.DefaultBehaviorColor;
        if (name != null)
        {
          name = CommonWITUtils.RemoveASCIIControlCharactersAndTrim(name);
          CommonWITUtils.CheckValidName(name, 128);
          if (behaviorsInternal.Any<Behavior>((Func<Behavior, bool>) (t => TFStringComparer.BehaviorName.Equals(t.Name, name) && !TFStringComparer.BehaviorReferenceName.Equals(t.ReferenceName, behaviorReferenceName))))
            throw new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.BehaviorNameInUse());
        }
        bool overridden;
        Guid extensionId;
        if (behavior.ProcessId == processId)
        {
          overridden = behavior.Overridden;
          extensionId = behavior.Id;
        }
        else
        {
          overridden = true;
          extensionId = Guid.NewGuid();
        }
        requestContext.Trace(911530, TraceLevel.Info, "WorkItemType", nameof (BehaviorService), string.Format("ProcessId: {0}, Existing behavior processId: {1}, Behavior refname: {2}, Overridden: {3}, Id : {4}", (object) processId, (object) behavior.ProcessId, (object) behavior.ReferenceName, (object) overridden, (object) extensionId));
        WorkItemTypeletRecord record = (WorkItemTypeletRecord) null;
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        using (WorkItemTypeExtensionComponent component = WorkItemTypeExtensionComponent.CreateComponent(requestContext))
          record = component.UpdateBehavior(extensionId, processId, behavior.ReferenceName, name, color, overridden, id);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("behaviorRefName", behavior.ReferenceName);
        properties.Add("overridden", overridden);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (BehaviorService), nameof (UpdateBehavior), properties);
        WorkItemTrackingFieldService service = requestContext.GetService<WorkItemTrackingFieldService>();
        return Behavior.Create(requestContext, record, service);
      }));
    }

    public void DeleteBehavior(
      IVssRequestContext requestContext,
      Guid processId,
      string behaviorReferenceName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(behaviorReferenceName, nameof (behaviorReferenceName));
      requestContext.TraceBlock(911512, 911513, "WorkItemType", nameof (BehaviorService), "UpdateBehavior", (Action) (() =>
      {
        WorkItemTrackingProcessService.CheckEditPermission(requestContext, processId);
        Behavior behavior = this.GetBehaviorsInternal(requestContext, processId, true, true).FirstOrDefault<Behavior>((Func<Behavior, bool>) (b => TFStringComparer.BehaviorReferenceName.Equals(b.ReferenceName, behaviorReferenceName)));
        if (behavior == null)
          throw new BehaviorDoesNotExistException(behaviorReferenceName, processId.ToString());
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        if (processId != behavior.ProcessId)
        {
          using (WorkItemTypeExtensionComponent component = WorkItemTypeExtensionComponent.CreateComponent(requestContext))
            component.DeleteWorkItemTypeBehaviorReference(processId, Guid.Empty, behavior.ReferenceName, id);
        }
        else
        {
          using (WorkItemTypeExtensionComponent component = WorkItemTypeExtensionComponent.CreateComponent(requestContext))
            component.DeleteBehavior(behavior.Id, processId, behavior.ReferenceName, id);
        }
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (BehaviorService), nameof (DeleteBehavior), "behaviorRefName", behavior.ReferenceName);
      }));
    }

    public IDictionary<string, bool> IsSystemBehavior(
      IVssRequestContext requestContext,
      IReadOnlyCollection<string> behaviorReferenceNames)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IReadOnlyCollection<string>>(behaviorReferenceNames, nameof (behaviorReferenceNames));
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) behaviorReferenceNames, nameof (behaviorReferenceNames));
      return (IDictionary<string, bool>) behaviorReferenceNames.Distinct<string>((IEqualityComparer<string>) TFStringComparer.BehaviorReferenceName).ToDictionary<string, string, bool>((Func<string, string>) (k => k), (Func<string, bool>) (v => this.IsSystemBehavior(v)));
    }

    internal virtual IReadOnlyCollection<Behavior> GetBehaviorsInternal(
      IVssRequestContext requestContext,
      Guid processId,
      bool includeParent = false,
      bool bypassCache = false)
    {
      return (IReadOnlyCollection<Behavior>) requestContext.TraceBlock<List<Behavior>>(911500, 911501, "WorkItemType", nameof (BehaviorService), nameof (GetBehaviorsInternal), (Func<List<Behavior>>) (() =>
      {
        IProcessWorkItemTypeService service = requestContext.GetService<IProcessWorkItemTypeService>();
        IReadOnlyCollection<Behavior> typelets = service.GetTypelets<Behavior>(requestContext, processId, bypassCache);
        foreach (Behavior behavior in (IEnumerable<Behavior>) typelets)
          requestContext.Trace(911531, TraceLevel.Info, "WorkItemType", nameof (BehaviorService), string.Format("ProcessId: {0}, behavior processId: {1}, behavior refname: {2}, behavior name:: {3}", (object) processId, (object) behavior.ProcessId, (object) behavior.ReferenceName, (object) behavior.Name));
        Dictionary<string, Behavior> thisAndAncestorTypelets = typelets.ToDictionary<Behavior, string>((Func<Behavior, string>) (t => t.ReferenceName), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeReferenceName);
        if (includeParent)
        {
          Action<Behavior> action = (Action<Behavior>) (behavior =>
          {
            if (thisAndAncestorTypelets.ContainsKey(behavior.ReferenceName))
              return;
            thisAndAncestorTypelets.Add(behavior.ReferenceName, behavior);
          });
          ProcessDescriptor processDescriptor = requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, processId);
          if (processDescriptor.IsDerived)
          {
            foreach (Behavior typelet in (IEnumerable<Behavior>) service.GetTypelets<Behavior>(requestContext, processDescriptor.Inherits, bypassCache))
              action(typelet);
          }
          foreach (Behavior typelet in (IEnumerable<Behavior>) service.GetTypelets<Behavior>(requestContext, ProcessConstants.SystemProcessId, bypassCache))
            action(typelet);
        }
        List<Behavior> list = thisAndAncestorTypelets.Values.Select<Behavior, Behavior>((Func<Behavior, Behavior>) (b => Behavior.Create(b, b))).ToList<Behavior>();
        foreach (Behavior behavior in list)
        {
          behavior.ResolveTypeReference((IReadOnlyCollection<ProcessTypelet>) list);
          requestContext.Trace(911531, TraceLevel.Info, "WorkItemType", nameof (BehaviorService), string.Format("Behavior data after fixing reference ProcessId: {0}, behavior processId: {1}, behavior refname: {2}, behavior name:: {3}", (object) processId, (object) behavior.ProcessId, (object) behavior.ReferenceName, (object) behavior.Name));
        }
        return list;
      }));
    }

    private bool IsOrderedBehavior(Behavior behavior) => behavior.ParentBehavior == null ? TFStringComparer.BehaviorReferenceName.Equals(behavior.ReferenceName, BehaviorService.OrderedBehaviorReferenceName) : this.IsOrderedBehavior(behavior.ParentBehavior);

    private bool IsSystemBehavior(string behaviorReferenceName)
    {
      if (string.IsNullOrWhiteSpace(behaviorReferenceName))
        return false;
      return TFStringComparer.BehaviorReferenceName.Equals(behaviorReferenceName, BehaviorService.PortfolioBehaviorReferenceName) || TFStringComparer.BehaviorReferenceName.Equals(behaviorReferenceName, BehaviorService.RequirementBehaviorReferenceName) || TFStringComparer.BehaviorReferenceName.Equals(behaviorReferenceName, BehaviorService.TaskBehaviorReferenceName);
    }
  }
}
