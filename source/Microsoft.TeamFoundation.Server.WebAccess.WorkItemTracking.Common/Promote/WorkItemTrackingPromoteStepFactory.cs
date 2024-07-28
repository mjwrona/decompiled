// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Promote.WorkItemTrackingPromoteStepFactory
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Promote
{
  public class WorkItemTrackingPromoteStepFactory : IProjectPromoteStepFactory
  {
    private static IProjectPromoteStepFactory s_instance;
    private const string c_TraceArea = "TeamProjectPromote";
    private const string c_TraceLayer = "WorkItemTrackingPromoteStep";

    private WorkItemTrackingPromoteStepFactory()
    {
    }

    public static IProjectPromoteStepFactory Instance
    {
      get
      {
        if (WorkItemTrackingPromoteStepFactory.s_instance == null)
          WorkItemTrackingPromoteStepFactory.s_instance = (IProjectPromoteStepFactory) new WorkItemTrackingPromoteStepFactory();
        return WorkItemTrackingPromoteStepFactory.s_instance;
      }
    }

    public IEnumerable<IProjectPromoteStep> GenerateSteps(
      IVssRequestContext requestContext,
      IProcessTemplate source,
      IProcessTemplate destination,
      StringBuilder log)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IProcessTemplate>(destination, nameof (destination));
      ArgumentUtility.CheckForNull<StringBuilder>(log, nameof (log));
      XmlDocument sourceProcessData = (XmlDocument) null;
      if (source != null)
        sourceProcessData = ProcessTemplateHelper.GetWorkItemTrackingProcessData(requestContext, source);
      XmlDocument trackingProcessData = ProcessTemplateHelper.GetWorkItemTrackingProcessData(requestContext, destination);
      if (trackingProcessData == null)
      {
        log.AppendLine("Empty work item tracking process data is not supported.");
        throw new InvalidOperationException();
      }
      List<IProjectPromoteStep> allSteps = new List<IProjectPromoteStep>();
      allSteps.AddRange((IEnumerable<IProjectPromoteStep>) WorkItemTrackingPromoteStepFactory.GenerateLinkTypePromoteSteps(requestContext, source, sourceProcessData, destination, trackingProcessData).OrderBy<WorkItemTrackingPromoteStepFactory.LinkTypePromoteStep, string>((Func<WorkItemTrackingPromoteStepFactory.LinkTypePromoteStep, string>) (x => x.OrderKey)));
      List<IProjectPromoteStep> list = WorkItemTrackingPromoteStepFactory.GenerateWorkItemTypeSteps(requestContext, source, sourceProcessData, destination, trackingProcessData, log).ToList<IProjectPromoteStep>();
      this.OrderAndAddRenameSteps(requestContext, allSteps, (IEnumerable<IProjectPromoteStep>) list);
      allSteps.AddRange((IEnumerable<IProjectPromoteStep>) list.OfType<WorkItemTrackingPromoteStepFactory.WorkItemGlobalLists>());
      allSteps.AddRange((IEnumerable<IProjectPromoteStep>) list.OfType<WorkItemTrackingPromoteStepFactory.WorkItemTypeInsertion>().OrderBy<WorkItemTrackingPromoteStepFactory.WorkItemTypeInsertion, string>((Func<WorkItemTrackingPromoteStepFactory.WorkItemTypeInsertion, string>) (x => x.OrderKey)));
      if (destination?.Descriptor == null || destination.Descriptor.IsCustom)
      {
        IProjectPromoteStep categoryStep = WorkItemTrackingPromoteStepFactory.GenerateCategoryStep(requestContext, source, sourceProcessData, destination, trackingProcessData, (IEnumerable<IProjectPromoteStep>) list);
        if (categoryStep != null)
          allSteps.Add(categoryStep);
      }
      allSteps.AddRange((IEnumerable<IProjectPromoteStep>) list.OfType<WorkItemTrackingPromoteStepFactory.WorkItemTypeDeletion>().OrderBy<WorkItemTrackingPromoteStepFactory.WorkItemTypeDeletion, string>((Func<WorkItemTrackingPromoteStepFactory.WorkItemTypeDeletion, string>) (x => x.OrderKey)));
      return (IEnumerable<IProjectPromoteStep>) allSteps;
    }

    public void ValidateSteps(
      IVssRequestContext requestContext,
      IEnumerable<IProjectPromoteStep> steps,
      Guid projectId,
      StringBuilder log)
    {
      IEnumerable<string> source = steps.OfType<WorkItemTrackingPromoteStepFactory.WorkItemTypeDeletion>().Select<WorkItemTrackingPromoteStepFactory.WorkItemTypeDeletion, string>((Func<WorkItemTrackingPromoteStepFactory.WorkItemTypeDeletion, string>) (s => s.Name)).Concat<string>(steps.OfType<WorkItemTrackingPromoteStepFactory.WorkItemTypeRename>().Select<WorkItemTrackingPromoteStepFactory.WorkItemTypeRename, string>((Func<WorkItemTrackingPromoteStepFactory.WorkItemTypeRename, string>) (s => s.Name))).Distinct<string>();
      if (!source.Any<string>())
        return;
      IWorkItemTypeService service = requestContext.GetService<IWorkItemTypeService>();
      foreach (string workItemTypeName in source)
      {
        if (service.HasAnyWorkItemsOfTypeForProject(requestContext, projectId, workItemTypeName))
        {
          log.AppendLine(string.Format("Promote for project {0} intended to delete or rename work items of type {1}, but 1 or more work items of that type still exist in the project! For safety, promote will fail before any steps are performed.", (object) projectId, (object) workItemTypeName));
          throw new InvalidOperationException();
        }
      }
    }

    private static IEnumerable<WorkItemTrackingPromoteStepFactory.LinkTypePromoteStep> GenerateLinkTypePromoteSteps(
      IVssRequestContext requestContext,
      IProcessTemplate sourceTemplate,
      XmlDocument sourceProcessData,
      IProcessTemplate destinationTemplate,
      XmlDocument destinationProcessData)
    {
      Dictionary<string, string> existingLinkTypes = new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      if (sourceProcessData != null && sourceTemplate != null)
      {
        requestContext.Trace(1000028, TraceLevel.Verbose, "TeamProjectPromote", "WorkItemTrackingPromoteStep", "Collecting existing link type definitions");
        foreach (XmlNode selectNode in sourceProcessData.SelectNodes("//LINKTYPES/LINKTYPE"))
        {
          string resource = WorkItemTrackingPromoteStepFactory.GetResource(sourceTemplate, selectNode);
          requestContext.Trace(1000029, TraceLevel.Verbose, "TeamProjectPromote", "WorkItemTrackingPromoteStep", "Existing link type definition: {0}", (object) resource);
          foreach (KeyValuePair<string, string> linkTypeDefinition in WorkItemTrackingPromoteStepFactory.GetLinkTypeDefinitions(resource))
            existingLinkTypes[linkTypeDefinition.Key] = linkTypeDefinition.Value;
        }
      }
      foreach (XmlNode selectNode in destinationProcessData.SelectNodes("//LINKTYPES/LINKTYPE"))
      {
        string resource = WorkItemTrackingPromoteStepFactory.GetResource(destinationTemplate, selectNode);
        requestContext.Trace(1000030, TraceLevel.Verbose, "TeamProjectPromote", "WorkItemTrackingPromoteStep", "Destination link type definition: {0}", (object) resource);
        foreach (KeyValuePair<string, string> linkTypeDefinition in WorkItemTrackingPromoteStepFactory.GetLinkTypeDefinitions(resource))
        {
          string xml1;
          if (!existingLinkTypes.TryGetValue(linkTypeDefinition.Key, out xml1) || !XmlUtility.CompareXmlDocuments(xml1, linkTypeDefinition.Value))
          {
            requestContext.Trace(1000031, TraceLevel.Verbose, "TeamProjectPromote", "WorkItemTrackingPromoteStep", "Link type updated/added: {0}", (object) linkTypeDefinition.Key);
            yield return new WorkItemTrackingPromoteStepFactory.LinkTypePromoteStep(linkTypeDefinition.Value, linkTypeDefinition.Key);
          }
        }
      }
    }

    private static IEnumerable<IProjectPromoteStep> GenerateWorkItemTypeSteps(
      IVssRequestContext requestContext,
      IProcessTemplate sourceTemplate,
      XmlDocument sourceProcessData,
      IProcessTemplate destinationTemplate,
      XmlDocument destinationProcessData,
      StringBuilder log)
    {
      StringComparer serverStringComparer = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).ServerStringComparer;
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      HashSet<string> stringSet1 = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      HashSet<string> stringSet2 = new HashSet<string>((IEqualityComparer<string>) serverStringComparer);
      List<IProjectPromoteStep> workItemTypeSteps = new List<IProjectPromoteStep>();
      WorkItemTrackingPromoteStepFactory.WorkItemGlobalLists globalListStep = new WorkItemTrackingPromoteStepFactory.WorkItemGlobalLists();
      bool? flag1 = new bool?();
      if (sourceProcessData != null && sourceTemplate != null)
      {
        requestContext.Trace(1000020, TraceLevel.Verbose, "TeamProjectPromote", "WorkItemTrackingPromoteStep", "Collect existing work item types");
        foreach (XmlNode selectNode in sourceProcessData.SelectNodes("//WORKITEMTYPES/WORKITEMTYPE"))
        {
          string resource = WorkItemTrackingPromoteStepFactory.GetResource(sourceTemplate, selectNode);
          requestContext.Trace(1000021, TraceLevel.Verbose, "TeamProjectPromote", "WorkItemTrackingPromoteStep", "Existing work item type definition: {0}", (object) resource);
          string attribute = WorkItemTrackingPromoteStepFactory.GetAttribute(resource, "//WORKITEMTYPE/@refname");
          WorkItemTrackingPromoteStepFactory.CheckFlag(ref flag1, attribute != null);
          if (attribute == null)
            attribute = selectNode.Attributes["fileName"].Value;
          dictionary[attribute] = resource;
        }
      }
      requestContext.Trace(1000022, TraceLevel.Verbose, "TeamProjectPromote", "WorkItemTrackingPromoteStep", "Collect destination work item types");
      bool flag2 = !flag1.HasValue || flag1.Value;
      flag1 = new bool?();
      foreach (XmlNode selectNode in destinationProcessData.SelectNodes("//WORKITEMTYPES/WORKITEMTYPE"))
      {
        string resource = WorkItemTrackingPromoteStepFactory.GetResource(destinationTemplate, selectNode);
        requestContext.Trace(1000023, TraceLevel.Verbose, "TeamProjectPromote", "WorkItemTrackingPromoteStep", "Destination work item type definition: {0}", (object) resource);
        string attribute1 = WorkItemTrackingPromoteStepFactory.GetAttribute(resource, "//WORKITEMTYPE/@refname");
        WorkItemTrackingPromoteStepFactory.CheckFlag(ref flag1, attribute1 != null);
        if (attribute1 == null || !flag2)
          attribute1 = selectNode.Attributes["fileName"].Value;
        stringSet1.Add(attribute1);
        stringSet2.Add(WorkItemTrackingPromoteStepFactory.GetAttribute(resource, "//WORKITEMTYPE/@name"));
        string existingDefinition;
        if (!dictionary.TryGetValue(attribute1, out existingDefinition))
        {
          log.AppendLine("New type refname: " + attribute1);
          WorkItemTrackingPromoteStepFactory.UpdateGlobalListStepAndRemoveGlobalLists(globalListStep, ref resource);
          workItemTypeSteps.Add((IProjectPromoteStep) new WorkItemTrackingPromoteStepFactory.WorkItemTypeInsertion(resource, attribute1));
        }
        else
        {
          WorkItemTrackingPromoteStepFactory.UpdateGlobalListStepAndRemoveGlobalLists(globalListStep, ref existingDefinition, ref resource);
          if (!XmlUtility.CompareXmlDocuments(existingDefinition, resource))
          {
            if (flag1.Value)
            {
              string attribute2 = WorkItemTrackingPromoteStepFactory.GetAttribute(existingDefinition, "//WORKITEMTYPE/@name");
              string attribute3 = WorkItemTrackingPromoteStepFactory.GetAttribute(resource, "//WORKITEMTYPE/@name");
              if (!serverStringComparer.Equals(attribute2, attribute3))
              {
                log.AppendLine("Renamed type refname: " + attribute1 + ", oldName: " + attribute2 + ", newName: " + attribute3);
                workItemTypeSteps.Add((IProjectPromoteStep) new WorkItemTrackingPromoteStepFactory.WorkItemTypeRename(attribute2, attribute3, attribute1));
              }
            }
            log.AppendLine("Updated type refname: " + attribute1);
            workItemTypeSteps.Add((IProjectPromoteStep) new WorkItemTrackingPromoteStepFactory.WorkItemTypeInsertion(resource, attribute1));
          }
        }
      }
      if (flag1.Value)
      {
        foreach (KeyValuePair<string, string> keyValuePair in dictionary)
        {
          if (!stringSet1.Contains(keyValuePair.Key))
          {
            string name = WorkItemTrackingPromoteStepFactory.GetAttribute(keyValuePair.Value, "//WORKITEMTYPE/@name");
            if (stringSet2.Contains(name))
            {
              string newName = name + "_ToDelete_" + ProcessTemplateHelper.GetProcessTemplateTypeString(requestContext, destinationTemplate).Replace('.', '_');
              log.AppendLine("Rename type refname: " + keyValuePair.Key + ", oldName: " + name + ", newName: " + newName);
              workItemTypeSteps.Add((IProjectPromoteStep) new WorkItemTrackingPromoteStepFactory.WorkItemTypeRename(name, newName));
              name = newName;
            }
            else
              log.AppendLine("Delete type refname: " + keyValuePair.Key + ", name: " + name);
            workItemTypeSteps.Add((IProjectPromoteStep) new WorkItemTrackingPromoteStepFactory.WorkItemTypeDeletion(name));
          }
        }
      }
      if (globalListStep.ListDefinitions.Any<XElement>())
        workItemTypeSteps.Add((IProjectPromoteStep) globalListStep);
      return (IEnumerable<IProjectPromoteStep>) workItemTypeSteps;
    }

    private static void UpdateGlobalListStepAndRemoveGlobalLists(
      WorkItemTrackingPromoteStepFactory.WorkItemGlobalLists globalListStep,
      ref string existingDefinition,
      ref string newDefinition)
    {
      XElement definition1 = XElement.Parse(existingDefinition);
      XElement globalLists1;
      bool globalLists2 = WorkItemTrackingPromoteStepFactory.TryExtractGlobalLists(definition1, out globalLists1);
      XElement definition2 = XElement.Parse(newDefinition);
      XElement globalLists3;
      if (WorkItemTrackingPromoteStepFactory.TryExtractGlobalLists(definition2, out globalLists3))
      {
        if (!globalLists2 || !XmlUtility.CompareXmlDocuments(globalLists1.ToString(), globalLists3.ToString()))
        {
          IEnumerable<XElement> listDefinitions = globalLists3.Elements((XName) "GLOBALLIST");
          globalListStep.AddListDefinitions(listDefinitions);
        }
        globalLists3.Remove();
        newDefinition = definition2.ToString();
      }
      if (!globalLists2)
        return;
      globalLists1.Remove();
      existingDefinition = definition1.ToString();
    }

    private static void UpdateGlobalListStepAndRemoveGlobalLists(
      WorkItemTrackingPromoteStepFactory.WorkItemGlobalLists globalListStep,
      ref string definition)
    {
      XElement definition1 = XElement.Parse(definition);
      XElement globalLists;
      if (!WorkItemTrackingPromoteStepFactory.TryExtractGlobalLists(definition1, out globalLists))
        return;
      IEnumerable<XElement> listDefinitions = globalLists.Elements((XName) "GLOBALLIST");
      globalListStep.AddListDefinitions(listDefinitions);
      globalLists.Remove();
      definition = definition1.ToString();
    }

    private static bool TryExtractGlobalLists(XElement definition, out XElement globalLists)
    {
      globalLists = definition.Descendants((XName) "GLOBALLISTS").FirstOrDefault<XElement>();
      return globalLists != null;
    }

    private void OrderAndAddRenameSteps(
      IVssRequestContext requestContext,
      List<IProjectPromoteStep> allSteps,
      IEnumerable<IProjectPromoteStep> typeSteps)
    {
      StringComparer serverStringComparer = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).ServerStringComparer;
      Dictionary<string, WorkItemTrackingPromoteStepFactory.WorkItemTypeRename> dictionary = typeSteps.OfType<WorkItemTrackingPromoteStepFactory.WorkItemTypeRename>().ToDictionary<WorkItemTrackingPromoteStepFactory.WorkItemTypeRename, string>((Func<WorkItemTrackingPromoteStepFactory.WorkItemTypeRename, string>) (x => x.Name), (IEqualityComparer<string>) serverStringComparer);
      while (dictionary.Any<KeyValuePair<string, WorkItemTrackingPromoteStepFactory.WorkItemTypeRename>>())
      {
        foreach (KeyValuePair<string, WorkItemTrackingPromoteStepFactory.WorkItemTypeRename> keyValuePair in dictionary.OrderBy<KeyValuePair<string, WorkItemTrackingPromoteStepFactory.WorkItemTypeRename>, string>((Func<KeyValuePair<string, WorkItemTrackingPromoteStepFactory.WorkItemTypeRename>, string>) (x => x.Value.OrderKey)).ToList<KeyValuePair<string, WorkItemTrackingPromoteStepFactory.WorkItemTypeRename>>())
        {
          if (!dictionary.ContainsKey(keyValuePair.Value.NewName))
          {
            allSteps.Add((IProjectPromoteStep) keyValuePair.Value);
            dictionary.Remove(keyValuePair.Key);
          }
        }
      }
    }

    private static void CheckFlag(ref bool? flag, bool value)
    {
      if (!flag.HasValue)
        flag = new bool?(value);
      else if (flag.Value != value)
        throw new InvalidOperationException();
    }

    private static IEnumerable<KeyValuePair<string, string>> GetLinkTypeDefinitions(
      string fileContent)
    {
      foreach (XElement element in XDocument.Parse(fileContent).Root.Elements((XName) "LinkType"))
        yield return new KeyValuePair<string, string>(element.Attribute((XName) "ReferenceName").Value, new XElement((XName) "LinkTypes", (object) element).ToString());
    }

    private static string GetAttribute(string definition, string xpath) => XmlUtility.GetDocument(definition).DocumentElement.SelectSingleNode(xpath)?.Value;

    private static IProjectPromoteStep GenerateCategoryStep(
      IVssRequestContext requestContext,
      IProcessTemplate sourceTemplate,
      XmlDocument sourceProcessData,
      IProcessTemplate destinationTemplate,
      XmlDocument destinationProcessData,
      IEnumerable<IProjectPromoteStep> existingSteps)
    {
      string xml1 = (string) null;
      if (sourceProcessData != null && sourceTemplate != null)
        xml1 = WorkItemTrackingPromoteStepFactory.GetResource(sourceTemplate, sourceProcessData.SelectSingleNode("//CATEGORIES"));
      string resource = WorkItemTrackingPromoteStepFactory.GetResource(destinationTemplate, destinationProcessData.SelectSingleNode("//CATEGORIES"));
      return !XmlUtility.CompareXmlDocuments(xml1, resource) || existingSteps.OfType<WorkItemTrackingPromoteStepFactory.WorkItemTypeRename>().Any<WorkItemTrackingPromoteStepFactory.WorkItemTypeRename>() ? (IProjectPromoteStep) new WorkItemTrackingPromoteStepFactory.WorkItemTypeCategoryPromoteStep(resource) : (IProjectPromoteStep) null;
    }

    private static string GetResource(IProcessTemplate template, XmlNode node)
    {
      string resourceName = node.Attributes["fileName"].Value;
      using (StreamReader streamReader = new StreamReader(template.GetResource(resourceName)))
        return streamReader.ReadToEnd();
    }

    internal abstract class WorkItemTrackingProjectStep : IProjectPromoteStep
    {
      public abstract void Execute(
        IVssRequestContext requestContext,
        ProjectInfo project,
        StringBuilder log);

      protected int ResolveProjectId(IVssRequestContext requestContext, Guid projectId) => requestContext.GetService<WorkItemTrackingTreeService>().GetTreeNode(requestContext, projectId, projectId).Id;

      public string OrderKey { get; protected set; }
    }

    private class LinkTypePromoteStep : 
      WorkItemTrackingPromoteStepFactory.WorkItemTrackingProjectStep
    {
      public LinkTypePromoteStep(string definition, string orderKey)
      {
        this.Definition = definition;
        this.OrderKey = orderKey;
      }

      public string Definition { get; private set; }

      public override void Execute(
        IVssRequestContext requestContext,
        ProjectInfo project,
        StringBuilder log)
      {
        log.AppendLine("Importing link type. Order Key {this.OrderKey}.");
        requestContext.GetService<IProvisioningService>().ImportWorkItemLinkType(requestContext, this.Definition, true);
      }
    }

    private class WorkItemTypeCategoryPromoteStep : 
      WorkItemTrackingPromoteStepFactory.WorkItemTrackingProjectStep
    {
      public WorkItemTypeCategoryPromoteStep(string definition) => this.Definition = definition;

      public string Definition { get; private set; }

      public override void Execute(
        IVssRequestContext requestContext,
        ProjectInfo project,
        StringBuilder log)
      {
        log.AppendLine("Importing work item type category.");
        requestContext.GetService<IProvisioningService>().ImportCategories(requestContext, this.ResolveProjectId(requestContext, project.Id), this.Definition, false);
      }
    }

    internal class WorkItemTypeInsertion : 
      WorkItemTrackingPromoteStepFactory.WorkItemTrackingProjectStep
    {
      public WorkItemTypeInsertion(string definition, string orderKey)
      {
        this.Definition = definition;
        this.OrderKey = orderKey;
      }

      public string Definition { private set; get; }

      public override void Execute(
        IVssRequestContext requestContext,
        ProjectInfo project,
        StringBuilder log)
      {
        log.AppendLine("Importing work item type. Order Key " + this.OrderKey + ".");
        requestContext.GetService<IProvisioningService>().ImportWorkItemType(requestContext, this.ResolveProjectId(requestContext, project.Id), (string) null, this.Definition, isPromote: true);
      }
    }

    internal class WorkItemTypeInsertions : 
      WorkItemTrackingPromoteStepFactory.WorkItemTrackingProjectStep
    {
      public WorkItemTypeInsertions(
        IEnumerable<WorkItemTrackingPromoteStepFactory.WorkItemTypeInsertion> workItemTypeInsertions)
      {
        this.Definitions = workItemTypeInsertions.OrderBy<WorkItemTrackingPromoteStepFactory.WorkItemTypeInsertion, string>((Func<WorkItemTrackingPromoteStepFactory.WorkItemTypeInsertion, string>) (x => x.OrderKey)).Select<WorkItemTrackingPromoteStepFactory.WorkItemTypeInsertion, string>((Func<WorkItemTrackingPromoteStepFactory.WorkItemTypeInsertion, string>) (i => i.Definition)).ToList<string>();
      }

      public List<string> Definitions { private set; get; }

      public override void Execute(
        IVssRequestContext requestContext,
        ProjectInfo project,
        StringBuilder log)
      {
        log.AppendLine("Importing work item types.");
        requestContext.GetService<IProvisioningService>().ImportWorkItemTypes(requestContext, this.ResolveProjectId(requestContext, project.Id), (string) null, (IEnumerable<string>) this.Definitions, overwrite: true, isPromote: true);
      }
    }

    private class WorkItemTypeRename : WorkItemTrackingPromoteStepFactory.WorkItemTrackingProjectStep
    {
      public WorkItemTypeRename(string name, string newName, string orderKey = null)
      {
        this.Name = name;
        this.NewName = newName;
        this.OrderKey = orderKey ?? name;
      }

      public string Name { private set; get; }

      public string NewName { private set; get; }

      public override void Execute(
        IVssRequestContext requestContext,
        ProjectInfo project,
        StringBuilder log)
      {
        log.AppendLine("Rename work item type from " + this.Name + " to " + this.NewName + ". Order Key " + this.OrderKey + ".");
        requestContext.GetService<IProvisioningService>().RenameWorkItemType(requestContext, project.Id, this.Name, this.NewName);
      }
    }

    private class WorkItemTypeDeletion : 
      WorkItemTrackingPromoteStepFactory.WorkItemTrackingProjectStep
    {
      public WorkItemTypeDeletion(string name, string orderKey = null)
      {
        this.Name = name;
        this.OrderKey = orderKey ?? name;
      }

      public string Name { private set; get; }

      public override void Execute(
        IVssRequestContext requestContext,
        ProjectInfo project,
        StringBuilder log)
      {
        log.AppendLine("Deleting work item type: " + this.Name + ". Order Key " + this.OrderKey + ".");
        IWorkItemTypeService service1 = requestContext.GetService<IWorkItemTypeService>();
        IProvisioningService service2 = requestContext.GetService<IProvisioningService>();
        IVssRequestContext requestContext1 = requestContext;
        Guid id = project.Id;
        foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType workItemType in (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType>) service1.GetWorkItemTypes(requestContext1, id))
        {
          if (workItemType.Name == this.Name)
            service2.DestroyWorkItemType(requestContext, this.Name, this.ResolveProjectId(requestContext, project.Id));
        }
      }
    }

    internal class WorkItemGlobalLists : 
      WorkItemTrackingPromoteStepFactory.WorkItemTrackingProjectStep
    {
      private List<XElement> listDefinitions = new List<XElement>();

      public IReadOnlyCollection<XElement> ListDefinitions => (IReadOnlyCollection<XElement>) this.listDefinitions;

      public void AddListDefinitions(IEnumerable<XElement> listDefinitions) => this.listDefinitions.AddRange(listDefinitions);

      public override void Execute(
        IVssRequestContext requestContext,
        ProjectInfo project,
        StringBuilder log)
      {
        List<XElement> unprocessedListDefinitions = this.MarkAndGetUnprocessedListDefinitions(requestContext);
        if (unprocessedListDefinitions.Count == 0)
        {
          log.AppendLine("No global lists to import.");
        }
        else
        {
          log.AppendLine(string.Format("Importing {0} global lists.", (object) unprocessedListDefinitions.Count));
          string definition = new XElement((XName) "GLOBALWORKFLOW", (object) new XElement((XName) "GLOBALLISTS", (object) unprocessedListDefinitions)).ToString();
          requestContext.GetService<IProvisioningService>().ImportGlobalWorkflow(requestContext, this.ResolveProjectId(requestContext, project.Id), definition);
        }
      }

      private List<XElement> MarkAndGetUnprocessedListDefinitions(IVssRequestContext requestContext)
      {
        ISet<string> seenGlobalListNames = requestContext.GetSeenGlobalListNames();
        List<XElement> unprocessedListDefinitions = new List<XElement>();
        foreach (XElement listDefinition in this.listDefinitions)
        {
          string str = listDefinition.Attribute((XName) "name")?.Value;
          if (!string.IsNullOrWhiteSpace(str) && !seenGlobalListNames.Contains(str))
          {
            unprocessedListDefinitions.Add(listDefinition);
            seenGlobalListNames.Add(str);
          }
        }
        return unprocessedListDefinitions;
      }
    }
  }
}
