// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Process.LegacyWorkItemTrackingProcessService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormExtensions;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Process
{
  public class LegacyWorkItemTrackingProcessService : 
    BaseTeamFoundationWorkItemTrackingService,
    ILegacyWorkItemTrackingProcessService,
    IVssFrameworkService
  {
    public virtual ProcessWorkDefinition GetProcessWorkDefinition(
      IVssRequestContext requestContext,
      Guid processTypeId)
    {
      return requestContext.TraceBlock<ProcessWorkDefinition>(10005107, 10005108, "ProcessTemplate", nameof (LegacyWorkItemTrackingProcessService), nameof (GetProcessWorkDefinition), (Func<ProcessWorkDefinition>) (() => this.GetProcessWorkDefinition(requestContext, requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, processTypeId))));
    }

    public virtual ProcessWorkDefinition GetProcessWorkDefinition(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor)
    {
      return requestContext.TraceBlock<ProcessWorkDefinition>(10005109, 10005110, "ProcessTemplate", nameof (LegacyWorkItemTrackingProcessService), nameof (GetProcessWorkDefinition), (Func<ProcessWorkDefinition>) (() =>
      {
        if (processDescriptor.IsDerived)
          throw new InvalidOperationException("Derived process does not have legacy package don't call this api for derived process, gets its parent package");
        return processDescriptor.Scope == ProcessScope.Deployment ? this.GetProcessWorkDefinitionInternal(requestContext.To(TeamFoundationHostType.Deployment), processDescriptor) : this.GetProcessWorkDefinitionInternal(requestContext, processDescriptor);
      }));
    }

    private ProcessWorkDefinition GetProcessWorkDefinitionInternal(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor)
    {
      return requestContext.TraceBlock<ProcessWorkDefinition>(10005111, 10005112, "ProcessTemplate", nameof (LegacyWorkItemTrackingProcessService), nameof (GetProcessWorkDefinitionInternal), (Func<ProcessWorkDefinition>) (() =>
      {
        LegacyWorkItemTrackingProcessWorkDefinitionCache service1 = requestContext.GetService<LegacyWorkItemTrackingProcessWorkDefinitionCache>();
        ProcessWorkDefinition definitionInternal1;
        if (service1.TryGetValue(requestContext, processDescriptor.RowId, out definitionInternal1))
          return definitionInternal1;
        ITeamFoundationProcessService service2 = requestContext.GetService<ITeamFoundationProcessService>();
        List<ProcessWorkItemTypeDefinition> workItemTypeDefinitions = new List<ProcessWorkItemTypeDefinition>();
        Layout oobLayout = (Layout) null;
        if (!processDescriptor.IsSystem)
          oobLayout = WellKnownProcessLayout.GetAgileBugLayout(requestContext);
        IVssRequestContext requestContext1 = requestContext;
        ProcessDescriptor descriptor = processDescriptor;
        ProcessBacklogs backlogs;
        IReadOnlyDictionary<string, CategoryWorkItemTypes> itemTypeCategories;
        IReadOnlyDictionary<string, OobLinkType> typesByReferenceName;
        using (Stream processPackageContent = service2.GetLegacyProcessPackageContent(requestContext1, descriptor))
        {
          using (ZipArchiveProcessTemplatePackage package = new ZipArchiveProcessTemplatePackage(processPackageContent))
          {
            Lazy<IEnumerable<Contribution>> formContributions = new Lazy<IEnumerable<Contribution>>((Func<IEnumerable<Contribution>>) (() => FormExtensionsUtility.GetFilteredContributions(requestContext)));
            LegacyWorkItemTrackingProcessPackage trackingProcessPackage = new LegacyWorkItemTrackingProcessPackage(requestContext, (IProcessTemplatePackage) package, formContributions, oobLayout, requestContext.RequestTracer, LegacyWorkItemTrackingProcessService.CanParseState(requestContext, processDescriptor));
            foreach (ProcessWorkItemTypeDefinition typeDefinition in (IEnumerable<ProcessWorkItemTypeDefinition>) trackingProcessPackage.TypeDefinitions)
            {
              if (typeDefinition.Form != null && processDescriptor.IsSystem)
                typeDefinition.Form.GetDescendants<LayoutNode>().ToList<LayoutNode>().ForEach((Action<LayoutNode>) (node => node.FromInheritedLayout = true));
              workItemTypeDefinitions.Add(typeDefinition);
            }
            backlogs = trackingProcessPackage.Backlogs;
            itemTypeCategories = trackingProcessPackage.WorkItemTypesByWorkItemTypeCategories;
            typesByReferenceName = trackingProcessPackage.LinkTypesByReferenceName;
          }
        }
        ProcessWorkDefinition definitionInternal2 = new ProcessWorkDefinition((IReadOnlyCollection<ProcessWorkItemTypeDefinition>) workItemTypeDefinitions, backlogs, itemTypeCategories, typesByReferenceName);
        if (workItemTypeDefinitions.Count == 0 || itemTypeCategories.Count == 0)
          requestContext.Trace(10005115, TraceLevel.Error, "ProcessTemplate", nameof (LegacyWorkItemTrackingProcessService), string.Format("LegacyWorkItemTrackingProcessWorkDefinitionCache is going to be corrupted. WitCollection_Count: {0}, workItemTypeCategories_Count: {1}", (object) workItemTypeDefinitions.Count, (object) itemTypeCategories.Count));
        service1.Set(requestContext, processDescriptor.RowId, definitionInternal2);
        return definitionInternal2;
      }));
    }

    private static bool CanParseState(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor)
    {
      return processDescriptor.IsSystem;
    }
  }
}
