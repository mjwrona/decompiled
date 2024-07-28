// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProcessTemplateMetadata
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class ProcessTemplateMetadata : IProjectMetadata
  {
    private IProcessTemplate m_processTemplate;
    private XmlDocument m_witProcessData;
    private IVssRequestContext m_requestContext;
    private Dictionary<string, WorkItemTypeCategory> m_categories;
    private Dictionary<string, WorkItemTypeMetadata> m_wiTypes;

    public ProcessTemplateMetadata(
      IVssRequestContext requestContext,
      IProcessTemplate processTemplate)
    {
      requestContext.TraceEnter(1004002, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, "ProcessTemplateMetadata.Ctor");
      requestContext.Trace(1004010, TraceLevel.Verbose, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, "Loaded ProcessTemplateMetadata from template {0}", (object) processTemplate.Descriptor.Name);
      this.m_requestContext = requestContext;
      this.m_processTemplate = processTemplate;
      Stream resource = this.m_processTemplate.GetResource("WorkItem Tracking\\WorkItems.xml");
      if (resource != null)
      {
        using (StreamReader streamReader = new StreamReader(resource))
          this.m_witProcessData = XmlUtility.GetDocument(streamReader.ReadToEnd());
      }
      requestContext.TraceLeave(1004003, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, "ProcessTemplateMetadata.Ctor");
    }

    public string Name => this.m_processTemplate.Descriptor.Name;

    public WorkItemTypeCategory GetWorkItemTypeCategory(string referenceName) => this.WorkItemTypeCategories.ContainsKey(referenceName) ? this.WorkItemTypeCategories[referenceName] : (WorkItemTypeCategory) null;

    public WorkItemTypeMetadata GetWorkItemType(string name) => this.WorkItemTypes.ContainsKey(name) ? this.WorkItemTypes[name] : (WorkItemTypeMetadata) null;

    private Dictionary<string, WorkItemTypeMetadata> WorkItemTypes
    {
      get
      {
        if (this.m_wiTypes == null)
        {
          this.m_requestContext.TraceEnter(1004004, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, "WorkItemTypes.GetInitial");
          this.m_wiTypes = new Dictionary<string, WorkItemTypeMetadata>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
          if (this.m_witProcessData != null)
          {
            foreach (XmlNode selectNode in this.m_witProcessData.SelectNodes("//WORKITEMTYPES/WORKITEMTYPE"))
            {
              using (StreamReader streamReader = new StreamReader(this.m_processTemplate.GetResource(selectNode.Attributes["fileName"].Value)))
              {
                WorkItemTypeMetadata itemTypeMetadata = (WorkItemTypeMetadata) new ProcessTemplateWorkItemTypeMetadata(ProvisioningService.GetXml(streamReader.ReadToEnd(), InternalSchemaType.WorkItemType).DocumentElement, this.Name);
                this.m_wiTypes.Add(itemTypeMetadata.Name, itemTypeMetadata);
                this.m_requestContext.Trace(1004006, TraceLevel.Verbose, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, "Loaded WorkItemType {0}", (object) itemTypeMetadata.Name);
              }
            }
          }
          this.m_requestContext.TraceLeave(1004005, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, "WorkItemTypes.GetInitial");
        }
        return this.m_wiTypes;
      }
    }

    private Dictionary<string, WorkItemTypeCategory> WorkItemTypeCategories
    {
      get
      {
        if (this.m_categories == null)
        {
          this.m_requestContext.TraceEnter(1004007, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, "WorkItemTypeCategories.GetInitial");
          if (this.m_witProcessData == null)
          {
            this.m_categories = new Dictionary<string, WorkItemTypeCategory>((IEqualityComparer<string>) TFStringComparer.WorkItemCategoryReferenceName);
          }
          else
          {
            using (StreamReader streamReader = new StreamReader(this.m_processTemplate.GetResource(this.m_witProcessData.SelectSingleNode("//CATEGORIES").Attributes["fileName"].Value)))
            {
              List<ServerWitCategory> categoriesList = ProvisioningService.CreateCategoriesList(this.m_requestContext, 0, ProvisioningService.GetXml(streamReader.ReadToEnd(), InternalSchemaType.Categories), true);
              this.m_categories = new Dictionary<string, WorkItemTypeCategory>((IEqualityComparer<string>) TFStringComparer.WorkItemCategoryReferenceName);
              foreach (ServerWitCategory serverWitCategory in categoriesList)
              {
                WorkItemTypeCategory itemTypeCategory = new WorkItemTypeCategory(serverWitCategory.Name, serverWitCategory.ReferenceName, serverWitCategory.WorkItemTypes.Select<LegacyWorkItemType, string>((Func<LegacyWorkItemType, string>) (c => c.Name)), serverWitCategory.DefaultWorkItemType.Name);
                this.m_categories.Add(itemTypeCategory.ReferenceName, itemTypeCategory);
                this.m_requestContext.Trace(1004009, TraceLevel.Verbose, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, "Loaded Category {0}", (object) itemTypeCategory.ReferenceName);
              }
            }
          }
          this.m_requestContext.TraceLeave(1004008, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, "WorkItemTypeCategories.GetInitial");
        }
        return this.m_categories;
      }
    }

    public ProjectProcessConfiguration GetProcessConfiguration()
    {
      string configurationDefinition = this.GetConfigurationDefinition("ProjectConfiguration");
      if (!string.IsNullOrEmpty(configurationDefinition))
        return TeamFoundationSerializationUtility.Deserialize<ProjectProcessConfiguration>(configurationDefinition);
      this.m_requestContext.Trace(1004011, TraceLevel.Verbose, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, "No ProcessConfiguration found in process template");
      return (ProjectProcessConfiguration) null;
    }

    private string GetConfigurationDefinition(string configurationNodeName)
    {
      if (this.m_witProcessData == null)
        return (string) null;
      XmlNode xmlNode1 = this.m_witProcessData.SelectSingleNode("//PROCESSCONFIGURATION");
      if (xmlNode1 == null)
        return (string) null;
      XmlNode xmlNode2 = xmlNode1.SelectSingleNode("./" + configurationNodeName);
      if (xmlNode2 == null)
        return (string) null;
      using (StreamReader streamReader = new StreamReader(this.m_processTemplate.GetResource(xmlNode2.Attributes["fileName"].Value)))
        return streamReader.ReadToEnd();
    }
  }
}
