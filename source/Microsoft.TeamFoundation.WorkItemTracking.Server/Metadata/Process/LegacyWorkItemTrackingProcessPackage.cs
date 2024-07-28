// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Process.LegacyWorkItemTrackingProcessPackage
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Process
{
  internal class LegacyWorkItemTrackingProcessPackage
  {
    private IVssRequestContext m_requestContext;
    private IProcessTemplatePackage m_package;
    private ITraceRequest m_tracer;
    private Lazy<IEnumerable<Contribution>> m_formContributions;
    private LayoutHelper m_layoutHelper;
    private Layout m_oobLayout;
    private XDocument m_witConfigDocument;
    private XDocument m_witProcessConfigDocument;
    private XDocument m_witCategoriesDocument;
    private IReadOnlyDictionary<string, XDocument> m_workItemTypeDocumentsByPath;
    private IReadOnlyDictionary<string, XDocument> m_linkTypeDocumentsByPath;
    private IReadOnlyDictionary<string, OobLinkType> m_linkTypesByReferenceName;
    private IReadOnlyDictionary<string, IReadOnlyDictionary<string, WorkItemStateCategory>> m_stateCategoryByStateByWorkItemCategory;
    private IReadOnlyDictionary<string, string> m_workItemTypeCategoriesByWorkItemType;
    private IReadOnlyDictionary<string, CategoryWorkItemTypes> m_workItemTypesByWorkItemTypeCategories;
    private static readonly string XmlCompletedValue = "Complete";
    private List<ProcessWorkItemTypeDefinition> m_types;
    private const int c_backlogRankSkips = 10;
    private ProcessBacklogs m_backlogs;
    private BugsBehavior? m_bugsBehavior;
    private IReadOnlyDictionary<string, string> m_properties;
    private Dictionary<string, string> m_workItemTypeColors;
    private Dictionary<string, string> m_workItemTypeIcons;
    private Dictionary<FieldTypeEnum, string> m_typeFields;
    private bool m_parseState;

    public LegacyWorkItemTrackingProcessPackage(
      IVssRequestContext requestContext,
      IProcessTemplatePackage package,
      Lazy<IEnumerable<Contribution>> formContributions,
      Layout oobLayout,
      ITraceRequest tracer = null,
      bool parseState = false)
    {
      this.m_requestContext = requestContext;
      this.m_package = package;
      this.m_tracer = tracer;
      this.m_parseState = parseState;
      this.m_formContributions = formContributions;
      this.m_layoutHelper = new LayoutHelper(oobLayout);
      this.m_oobLayout = oobLayout;
    }

    internal virtual List<ProcessWorkItemTypeDefinition> Types => this.TypeDocuments.Values.Select<XDocument, ProcessWorkItemTypeDefinition>((Func<XDocument, ProcessWorkItemTypeDefinition>) (tdoc => ProcessWorkItemTypeDefinition.CreateFromLegacyDeclaration(tdoc))).ToList<ProcessWorkItemTypeDefinition>();

    internal virtual Layout ConvertToNewLayout(
      FormTransformer transformer,
      Lazy<Dictionary<string, string>> lazyFieldNameMap,
      string formXml,
      string typeName,
      string processName)
    {
      XmlReaderSettings settings = new XmlReaderSettings();
      settings.DtdProcessing = DtdProcessing.Prohibit;
      settings.XmlResolver = (XmlResolver) null;
      XmlDocument xmlDocument = new XmlDocument();
      using (StringReader input = new StringReader(formXml))
      {
        using (XmlReader reader = XmlReader.Create((TextReader) input, settings))
          xmlDocument.Load(reader);
      }
      WebLayoutXmlHelper.GeneratePageAndGroupIds(processName, typeName, this.m_layoutHelper.GetOobFirstPageLabel(), (XmlElement) xmlDocument.FirstChild, (Action<string>) null, (Action<string>) null, (Action<string>) null, (Action<string, string>) null);
      string str;
      return transformer.ConvertToNewFormLayout(xmlDocument.OuterXml, this.m_package.Name, typeName, this.m_formContributions, this.m_oobLayout, (ControlLabelResolver) (controlId => lazyFieldNameMap.Value.TryGetValue(controlId, out str) ? str : (string) null));
    }

    public IReadOnlyCollection<ProcessWorkItemTypeDefinition> TypeDefinitions
    {
      get
      {
        if (this.m_types == null)
        {
          List<ProcessWorkItemTypeDefinition> types = this.Types;
          IDictionary<string, string> typeColorsByName = this.WorkItemTypeColorsByName;
          FormTransformer transformer = new FormTransformer(this.m_tracer);
          Lazy<Dictionary<string, string>> lazyFieldNameMap = new Lazy<Dictionary<string, string>>((Func<Dictionary<string, string>>) (() =>
          {
            Dictionary<string, string> typeDefinitions = new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
            foreach (ProcessWorkItemTypeDefinition itemTypeDefinition in types)
            {
              foreach (ProcessFieldDefinition fieldDefinition in (IEnumerable<ProcessFieldDefinition>) itemTypeDefinition.FieldDefinitions)
                typeDefinitions[fieldDefinition.ReferenceName] = fieldDefinition.Name;
            }
            return typeDefinitions;
          }), false);
          foreach (ProcessWorkItemTypeDefinition typeDefinition in types)
          {
            typeDefinition.ProcessId = this.m_package.TypeId;
            string str1;
            if (typeDefinition.ReferenceName != null && typeColorsByName.TryGetValue(typeDefinition.ReferenceName, out str1) || typeColorsByName.TryGetValue(typeDefinition.Name, out str1))
              typeDefinition.Color = str1;
            string str2;
            if (typeDefinition.ReferenceName != null && this.WorkItemTypeIcons.TryGetValue(typeDefinition.ReferenceName, out str2) || this.WorkItemTypeIcons.TryGetValue(typeDefinition.Name, out str2))
              typeDefinition.Icon = str2;
            typeDefinition.States = !this.m_parseState ? Enumerable.Empty<WorkItemStateDefinition>() : this.GenerateWorkItemStateDefinition(typeDefinition);
            Layout newLayout = this.ConvertToNewLayout(transformer, lazyFieldNameMap, typeDefinition.FormXml, typeDefinition.Name, this.m_package.Name);
            typeDefinition.Form = newLayout;
          }
          this.m_types = types;
        }
        return (IReadOnlyCollection<ProcessWorkItemTypeDefinition>) this.m_types;
      }
    }

    public ProcessBacklogs Backlogs
    {
      get
      {
        if (this.m_backlogs == null)
        {
          this.m_backlogs = new ProcessBacklogs();
          ProcessBacklogDefinition backlogDefinition1 = this.ReadProcessBacklogDefinition(this.ProcessConfigDocument.Descendants((XName) "RequirementBacklog").FirstOrDefault<XElement>());
          backlogDefinition1.Rank = 20;
          this.m_backlogs.RequirementBacklog = backlogDefinition1;
          ProcessBacklogDefinition backlogDefinition2 = this.ReadProcessBacklogDefinition(this.ProcessConfigDocument.Descendants((XName) "TaskBacklog").FirstOrDefault<XElement>());
          backlogDefinition2.Rank = 10;
          this.m_backlogs.TaskBacklog = backlogDefinition2;
          List<ProcessBacklogDefinition> backlogDefinitionList = new List<ProcessBacklogDefinition>();
          Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
          Dictionary<string, ProcessBacklogDefinition> dictionary2 = new Dictionary<string, ProcessBacklogDefinition>();
          string key1 = (string) null;
          foreach (XElement descendant in this.ProcessConfigDocument.Descendants((XName) "PortfolioBacklog"))
          {
            ProcessBacklogDefinition backlogDefinition3 = this.ReadProcessBacklogDefinition(descendant);
            string key2 = descendant.Attribute((XName) "parent")?.Value;
            if (!string.IsNullOrWhiteSpace(backlogDefinition3.CategoryReferenceName))
            {
              dictionary2[backlogDefinition3.CategoryReferenceName] = backlogDefinition3;
              if (string.IsNullOrWhiteSpace(key2))
                key1 = backlogDefinition3.CategoryReferenceName;
              else
                dictionary1[key2] = backlogDefinition3.CategoryReferenceName;
            }
          }
          if (!string.IsNullOrWhiteSpace(key1))
          {
            int num = 30 + dictionary1.Count * 10;
            ProcessBacklogDefinition backlogDefinition4 = dictionary2[key1];
            backlogDefinition4.Rank = num;
            backlogDefinitionList.Add(backlogDefinition4);
            string key3;
            for (string key4 = key1; backlogDefinitionList.Count < dictionary1.Count + 1 && dictionary1.TryGetValue(key4, out key3); key4 = key3)
            {
              ProcessBacklogDefinition backlogDefinition5 = dictionary2[key3];
              backlogDefinition5.Rank = num - 10;
              backlogDefinitionList.Add(backlogDefinition5);
            }
          }
          this.m_backlogs.PortfolioBacklogs = (IReadOnlyCollection<ProcessBacklogDefinition>) backlogDefinitionList;
          IReadOnlyCollection<string> strings = (IReadOnlyCollection<string>) new List<string>();
          string behaviorCategory = this.GetBugBehaviorCategory();
          CategoryWorkItemTypes categoryWorkItemTypes;
          if (!string.IsNullOrWhiteSpace(behaviorCategory) && this.WorkItemTypesByWorkItemTypeCategories.TryGetValue(behaviorCategory, out categoryWorkItemTypes))
          {
            this.m_backlogs.BugWorkItems = categoryWorkItemTypes.WorkItemTypeNames;
            this.m_backlogs.BugsBehavior = this.BugsBehavior;
          }
          else
          {
            this.m_backlogs.BugWorkItems = (IReadOnlyCollection<string>) new List<string>();
            this.m_backlogs.BugsBehavior = BugsBehavior.Off;
          }
          List<string> stringList = new List<string>();
          string str;
          if (this.Properties.TryGetValue("HiddenBacklogs", out str) && !string.IsNullOrWhiteSpace(str))
            stringList = ((IEnumerable<string>) str.Split(new string[1]
            {
              ";"
            }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (x => x.Trim())).Where<string>((Func<string, bool>) (x => !string.IsNullOrWhiteSpace(x))).Distinct<string>((IEqualityComparer<string>) TFStringComparer.WorkItemCategoryReferenceName).ToList<string>();
          this.m_backlogs.HiddenBacklogs = (IReadOnlyCollection<string>) stringList;
        }
        return this.m_backlogs;
      }
    }

    public IReadOnlyDictionary<FieldTypeEnum, string> TypeFields
    {
      get
      {
        if (this.m_typeFields != null)
          return (IReadOnlyDictionary<FieldTypeEnum, string>) this.m_typeFields;
        Dictionary<FieldTypeEnum, string> dictionary = new Dictionary<FieldTypeEnum, string>();
        IEnumerable<XElement> source = this.ProcessConfigDocument?.Descendants((XName) nameof (TypeFields));
        foreach (XElement descendant in source != null ? source.Descendants<XElement>((XName) "TypeField") : (IEnumerable<XElement>) null)
        {
          string str1 = descendant.Attribute((XName) "refname")?.Value;
          string str2 = descendant.Attribute((XName) "type")?.Value;
          FieldTypeEnum result;
          if (!string.IsNullOrEmpty(str1) && !string.IsNullOrEmpty(str2) && Enum.TryParse<FieldTypeEnum>(str2, out result))
            dictionary[result] = str1;
        }
        this.m_typeFields = dictionary;
        return (IReadOnlyDictionary<FieldTypeEnum, string>) new ReadOnlyDictionary<FieldTypeEnum, string>((IDictionary<FieldTypeEnum, string>) this.m_typeFields);
      }
    }

    public IReadOnlyDictionary<string, string> Properties
    {
      get
      {
        if (this.m_properties == null)
        {
          IEnumerable<XElement> source = this.ProcessConfigDocument.Descendants((XName) nameof (Properties)).FirstOrDefault<XElement>()?.Descendants((XName) "Property");
          Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) VssStringComparer.PropertyName);
          if (source != null && source.Any<XElement>())
          {
            foreach (XElement xelement in source)
            {
              string key = xelement.Attribute((XName) "name").Value;
              string str = xelement.Attribute((XName) "value").Value;
              dictionary.Add(key, str);
            }
          }
          this.m_properties = (IReadOnlyDictionary<string, string>) dictionary;
        }
        return this.m_properties;
      }
    }

    public BugsBehavior BugsBehavior
    {
      get
      {
        if (!this.m_bugsBehavior.HasValue)
        {
          BugsBehavior result = BugsBehavior.Off;
          string str;
          this.m_bugsBehavior = !this.Properties.TryGetValue(nameof (BugsBehavior), out str) || !Enum.TryParse<BugsBehavior>(str, out result) ? new BugsBehavior?(BugsBehavior.Off) : new BugsBehavior?(result);
        }
        return this.m_bugsBehavior.Value;
      }
    }

    private ProcessBacklogDefinition ReadProcessBacklogDefinition(XElement backlogElement)
    {
      string key = backlogElement?.Attribute((XName) "category")?.Value;
      string str1 = backlogElement?.Attribute((XName) "pluralName")?.Value;
      string str2 = backlogElement?.Attribute((XName) "singularName")?.Value;
      string s = backlogElement?.Attribute((XName) "workItemCountLimit")?.Value;
      int result1 = 0;
      if (string.IsNullOrEmpty(s) || !int.TryParse(s, out result1))
        result1 = 1000;
      ProcessBacklogDefinition backlogDefinition = new ProcessBacklogDefinition()
      {
        CategoryReferenceName = key,
        PluralName = str1,
        SingularName = str2,
        WorkItemCountLimit = result1
      };
      CategoryWorkItemTypes categoryWorkItemTypes;
      if (!string.IsNullOrWhiteSpace(key) && this.WorkItemTypesByWorkItemTypeCategories.TryGetValue(key, out categoryWorkItemTypes))
      {
        backlogDefinition.WorkItemTypesInCategory = categoryWorkItemTypes.WorkItemTypeNames;
        backlogDefinition.DefaultWorkItemTypeName = categoryWorkItemTypes.DefaultWorkItemTypeName;
      }
      backlogDefinition.AddPanelFields = (IReadOnlyCollection<string>) new List<string>();
      IEnumerable<XElement> xelements;
      if (backlogElement == null)
      {
        xelements = (IEnumerable<XElement>) null;
      }
      else
      {
        XElement xelement = backlogElement.Descendants((XName) "AddPanel").FirstOrDefault<XElement>();
        xelements = xelement != null ? xelement.Descendants((XName) "Fields").FirstOrDefault<XElement>()?.Descendants((XName) "Field") : (IEnumerable<XElement>) null;
      }
      IEnumerable<XElement> source1 = xelements;
      if (source1 != null)
        backlogDefinition.AddPanelFields = (IReadOnlyCollection<string>) source1.Select<XElement, string>((Func<XElement, string>) (apf => apf.Attribute((XName) "refname")?.Value)).Where<string>((Func<string, bool>) (v => !string.IsNullOrEmpty(v))).ToList<string>();
      backlogDefinition.Columns = (IReadOnlyCollection<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.BacklogColumn>) new List<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.BacklogColumn>();
      IEnumerable<XElement> source2 = backlogElement != null ? backlogElement.Descendants((XName) "Columns").FirstOrDefault<XElement>()?.Descendants((XName) "Column") : (IEnumerable<XElement>) null;
      if (source2 != null)
        backlogDefinition.Columns = (IReadOnlyCollection<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.BacklogColumn>) source2.Select<XElement, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.BacklogColumn>((Func<XElement, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.BacklogColumn>) (col =>
        {
          string str3 = col.Attribute((XName) "refname")?.Value;
          if (string.IsNullOrEmpty(str3))
            return (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.BacklogColumn) null;
          int result2;
          if (!int.TryParse(col.Attribute((XName) "width")?.Value, out result2))
            result2 = 100;
          return new Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.BacklogColumn()
          {
            ColumnReferenceName = str3,
            Width = result2
          };
        })).Where<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.BacklogColumn>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.BacklogColumn, bool>) (v => v != null)).ToList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.BacklogColumn>();
      return backlogDefinition;
    }

    public XDocument ConfigDocument
    {
      get
      {
        if (this.m_witConfigDocument == null)
        {
          string packagePath = this.m_package.RootDocument.Descendants((XName) "group").FirstOrDefault<XElement>((Func<XElement, bool>) (g => StringComparer.OrdinalIgnoreCase.Equals(g.Attribute((XName) "id")?.Value, "WorkItemTracking")))?.Element((XName) "taskList")?.Attribute((XName) "filename")?.Value;
          this.m_witConfigDocument = string.IsNullOrEmpty(packagePath) ? new XDocument() : this.m_package.GetDocument(packagePath);
        }
        return this.m_witConfigDocument;
      }
    }

    public XDocument CategoriesDocument
    {
      get
      {
        if (this.m_witCategoriesDocument == null)
        {
          string packagePath = this.ConfigDocument.Descendants((XName) "task").FirstOrDefault<XElement>((Func<XElement, bool>) (g => StringComparer.OrdinalIgnoreCase.Equals(g.Attribute((XName) "id")?.Value, "Categories")))?.Element((XName) "taskXml")?.Element((XName) "CATEGORIES")?.Attribute((XName) "fileName")?.Value;
          this.m_witCategoriesDocument = string.IsNullOrEmpty(packagePath) ? new XDocument() : this.m_package.GetDocument(packagePath);
        }
        return this.m_witCategoriesDocument;
      }
    }

    public XDocument ProcessConfigDocument
    {
      get
      {
        if (this.m_witProcessConfigDocument == null)
        {
          string packagePath = this.ConfigDocument.Descendants((XName) "taskXml").Descendants<XElement>((XName) "ProjectConfiguration").FirstOrDefault<XElement>()?.Attribute((XName) "fileName")?.Value;
          this.m_witProcessConfigDocument = !string.IsNullOrEmpty(packagePath) ? this.m_package.GetDocument(packagePath) : new XDocument();
        }
        return this.m_witProcessConfigDocument;
      }
    }

    private IReadOnlyDictionary<string, XDocument> TypeDocuments
    {
      get
      {
        if (this.m_workItemTypeDocumentsByPath == null)
        {
          Dictionary<string, XDocument> dictionary = new Dictionary<string, XDocument>((IEqualityComparer<string>) VssStringComparer.FilePath);
          foreach (string str in this.ConfigDocument.Descendants((XName) "WORKITEMTYPE").Attributes((XName) "fileName").Select<XAttribute, string>((Func<XAttribute, string>) (a => a.Value)).ToArray<string>())
            dictionary[str] = this.m_package.GetDocument(str);
          this.m_workItemTypeDocumentsByPath = (IReadOnlyDictionary<string, XDocument>) dictionary;
        }
        return this.m_workItemTypeDocumentsByPath;
      }
    }

    internal virtual IDictionary<string, string> WorkItemTypeColorsByName
    {
      get
      {
        if (this.m_workItemTypeColors == null)
        {
          this.m_workItemTypeColors = new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
          foreach (XElement descendant in this.ProcessConfigDocument.Descendants((XName) "WorkItemColor"))
          {
            string key = descendant.Attribute((XName) "name")?.Value;
            if (!string.IsNullOrEmpty(key))
            {
              string str = descendant.Attribute((XName) "primary")?.Value ?? descendant.Attribute((XName) "secondary")?.Value;
              if (!string.IsNullOrEmpty(str))
                this.m_workItemTypeColors[key] = str.Substring(2);
            }
          }
        }
        return (IDictionary<string, string>) this.m_workItemTypeColors;
      }
    }

    internal virtual IDictionary<string, string> WorkItemTypeIcons
    {
      get
      {
        if (this.m_workItemTypeIcons == null)
        {
          string propertyValue;
          this.m_workItemTypeIcons = !this.Properties.TryGetValue(nameof (WorkItemTypeIcons), out propertyValue) ? new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName) : new WorkItemTypeIconPropertyParser().ParseProperty(propertyValue);
        }
        return (IDictionary<string, string>) this.m_workItemTypeIcons;
      }
    }

    public bool RenameFields(IEnumerable<ProcessFieldDefinition> renamedFields)
    {
      Dictionary<string, ProcessFieldDefinition> renamedFieldsMap = renamedFields.ToDedupedDictionary<ProcessFieldDefinition, string, ProcessFieldDefinition>((Func<ProcessFieldDefinition, string>) (f => f.ReferenceName), (Func<ProcessFieldDefinition, ProcessFieldDefinition>) (f => f), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      KeyValuePair<string, XDocument>[] array = this.TypeDocuments.Where<KeyValuePair<string, XDocument>>((Func<KeyValuePair<string, XDocument>, bool>) (pair =>
      {
        bool flag = false;
        IEnumerable<XElement> xelements = pair.Value.Root.Element((XName) "WORKITEMTYPE")?.Element((XName) "FIELDS")?.Elements();
        if (xelements != null)
        {
          foreach (XElement xelement in xelements)
          {
            string key = xelement.Attribute((XName) "refname")?.Value;
            ProcessFieldDefinition processFieldDefinition;
            if (!string.IsNullOrEmpty(key) && renamedFieldsMap.TryGetValue(key, out processFieldDefinition))
            {
              xelement.Attribute((XName) "name").SetValue((object) processFieldDefinition.Name);
              flag = true;
            }
          }
        }
        return flag;
      })).ToArray<KeyValuePair<string, XDocument>>();
      foreach (KeyValuePair<string, XDocument> keyValuePair in array)
        this.m_package.UpdateDocument(keyValuePair.Key, keyValuePair.Value);
      if (array.Length == 0)
        return false;
      this.m_types = (List<ProcessWorkItemTypeDefinition>) null;
      return true;
    }

    internal virtual IReadOnlyDictionary<string, IReadOnlyDictionary<string, WorkItemStateCategory>> StateCategoryByStateByWorkItemCategory
    {
      get
      {
        if (this.m_stateCategoryByStateByWorkItemCategory == null)
        {
          Dictionary<string, IReadOnlyDictionary<string, WorkItemStateCategory>> dictionary1 = new Dictionary<string, IReadOnlyDictionary<string, WorkItemStateCategory>>();
          foreach (XElement descendant in this.ProcessConfigDocument.Descendants((XName) "States"))
          {
            string key1 = descendant.Parent.Attribute((XName) "category")?.Value;
            IEnumerable<XElement> source = descendant.Elements((XName) "State");
            Dictionary<string, WorkItemStateCategory> dictionary2 = new Dictionary<string, WorkItemStateCategory>();
            foreach (XElement xelement in source)
            {
              string key2 = xelement.Attribute((XName) "value")?.Value;
              string workItemStateCategoryName = xelement.Attribute((XName) "type")?.Value;
              if (!string.IsNullOrEmpty(workItemStateCategoryName))
                dictionary2[key2] = this.ParseWorkItemStateCategory(workItemStateCategoryName);
            }
            if (!string.IsNullOrEmpty(key1) && source.Any<XElement>())
              dictionary1[key1] = (IReadOnlyDictionary<string, WorkItemStateCategory>) new ReadOnlyDictionary<string, WorkItemStateCategory>((IDictionary<string, WorkItemStateCategory>) dictionary2);
          }
          this.m_stateCategoryByStateByWorkItemCategory = (IReadOnlyDictionary<string, IReadOnlyDictionary<string, WorkItemStateCategory>>) new ReadOnlyDictionary<string, IReadOnlyDictionary<string, WorkItemStateCategory>>((IDictionary<string, IReadOnlyDictionary<string, WorkItemStateCategory>>) dictionary1);
        }
        return this.m_stateCategoryByStateByWorkItemCategory;
      }
    }

    internal virtual IReadOnlyDictionary<string, OobLinkType> LinkTypesByReferenceName
    {
      get
      {
        if (this.m_linkTypesByReferenceName != null)
          return this.m_linkTypesByReferenceName;
        if (this.m_linkTypeDocumentsByPath == null)
        {
          Dictionary<string, XDocument> dictionary = new Dictionary<string, XDocument>((IEqualityComparer<string>) VssStringComparer.FilePath);
          foreach (string str in this.ConfigDocument.Descendants((XName) "LINKTYPE").Attributes((XName) "fileName").Select<XAttribute, string>((Func<XAttribute, string>) (a => a.Value)).ToArray<string>())
            dictionary[str] = this.m_package.GetDocument(str);
          this.m_linkTypeDocumentsByPath = (IReadOnlyDictionary<string, XDocument>) dictionary;
        }
        Dictionary<string, OobLinkType> dictionary1 = new Dictionary<string, OobLinkType>();
        foreach (KeyValuePair<string, XDocument> keyValuePair in (IEnumerable<KeyValuePair<string, XDocument>>) this.m_linkTypeDocumentsByPath)
        {
          foreach (XElement descendant in keyValuePair.Value.Descendants((XName) "LinkType"))
          {
            string str = descendant.Attribute((XName) "ReferenceName").Value;
            string forwardName = descendant.Attribute((XName) "ForwardName").Value;
            string reverseName = descendant.Attribute((XName) "ReverseName").Value;
            OobLinkType oobLinkType = new OobLinkType(str, forwardName, reverseName);
            if (!this.m_requestContext.IsFeatureEnabled("WorkItemTracking.Server.DisableLinkTypeDuplicatesIgnore"))
            {
              if (!dictionary1.TryAdd<string, OobLinkType>(str, oobLinkType))
                this.m_requestContext.Trace(900000, TraceLevel.Error, "Dictionaries", "MetadataService", "A duplicate of " + str + " linktype in " + keyValuePair.Key + " was found!");
            }
            else
              dictionary1.Add(str, oobLinkType);
          }
        }
        this.m_linkTypesByReferenceName = (IReadOnlyDictionary<string, OobLinkType>) dictionary1;
        return this.m_linkTypesByReferenceName;
      }
    }

    internal virtual IReadOnlyDictionary<string, string> WorkItemTypeCategoriesByWorkItemType
    {
      get
      {
        if (this.m_workItemTypeCategoriesByWorkItemType == null)
        {
          Dictionary<string, string> dictionary = new Dictionary<string, string>();
          foreach (XElement descendant in this.CategoriesDocument.Descendants((XName) "CATEGORY"))
          {
            string str = descendant.Attribute((XName) "refname")?.Value;
            if (!string.IsNullOrEmpty(str))
            {
              foreach (XAttribute attribute in descendant.Elements((XName) "DEFAULTWORKITEMTYPE").Concat<XElement>(descendant.Elements((XName) "WORKITEMTYPE")).Attributes((XName) "name"))
                dictionary[attribute.Value] = str;
            }
          }
          this.m_workItemTypeCategoriesByWorkItemType = (IReadOnlyDictionary<string, string>) new ReadOnlyDictionary<string, string>((IDictionary<string, string>) dictionary);
        }
        return this.m_workItemTypeCategoriesByWorkItemType;
      }
    }

    internal IReadOnlyDictionary<string, CategoryWorkItemTypes> WorkItemTypesByWorkItemTypeCategories
    {
      get
      {
        if (this.m_workItemTypesByWorkItemTypeCategories == null)
        {
          Dictionary<string, CategoryWorkItemTypes> dictionary = new Dictionary<string, CategoryWorkItemTypes>((IEqualityComparer<string>) TFStringComparer.WorkItemCategoryReferenceName);
          foreach (XElement descendant in this.CategoriesDocument.Descendants((XName) "CATEGORY"))
          {
            string name = descendant.Attribute((XName) "name")?.Value;
            string key = descendant.Attribute((XName) "refname")?.Value;
            if (!string.IsNullOrEmpty(key))
            {
              XAttribute xattribute = descendant.Elements((XName) "DEFAULTWORKITEMTYPE").Attributes((XName) "name").FirstOrDefault<XAttribute>();
              IEnumerable<XAttribute> source = descendant.Elements((XName) "WORKITEMTYPE").Attributes((XName) "name");
              CategoryWorkItemTypes categoryWorkItemTypes = new CategoryWorkItemTypes(name);
              HashSet<string> collection = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
              if (xattribute != null)
              {
                collection.Add(xattribute.Value);
                categoryWorkItemTypes.DefaultWorkItemTypeName = xattribute.Value;
              }
              collection.AddRange<string, HashSet<string>>(source.Select<XAttribute, string>((Func<XAttribute, string>) (wit => wit.Value)));
              categoryWorkItemTypes.WorkItemTypeNames = (IReadOnlyCollection<string>) collection;
              categoryWorkItemTypes.CategoryReferenceName = key;
              dictionary[key] = categoryWorkItemTypes;
            }
          }
          this.m_workItemTypesByWorkItemTypeCategories = (IReadOnlyDictionary<string, CategoryWorkItemTypes>) new ReadOnlyDictionary<string, CategoryWorkItemTypes>((IDictionary<string, CategoryWorkItemTypes>) dictionary);
        }
        return this.m_workItemTypesByWorkItemTypeCategories;
      }
    }

    private WorkItemStateCategory ParseWorkItemStateCategory(string workItemStateCategoryName)
    {
      if (workItemStateCategoryName == LegacyWorkItemTrackingProcessPackage.XmlCompletedValue)
        return WorkItemStateCategory.Completed;
      WorkItemStateCategory result = WorkItemStateCategory.Removed;
      Enum.TryParse<WorkItemStateCategory>(workItemStateCategoryName, out result);
      return result;
    }

    private IEnumerable<WorkItemStateDefinition> GenerateWorkItemStateDefinition(
      ProcessWorkItemTypeDefinition typeDefinition)
    {
      List<WorkItemStateDefinition> itemStateDefinition = new List<WorkItemStateDefinition>(typeDefinition.StateNames.Count);
      ReadOnlyDictionary<string, WorkItemOobState> s_stateGuidMap = this.m_requestContext.GetService<WorkItemTrackingOutOfBoxStatesCache>().GetOutOfBoxStates(this.m_requestContext);
      foreach (string stateName in (IEnumerable<string>) typeDefinition.StateNames)
      {
        if (!s_stateGuidMap.TryGetValue(stateName, out WorkItemOobState _))
          throw new WorkItemStateDefinitionNotFoundException(stateName);
      }
      typeDefinition.StateNames = (IList<string>) typeDefinition.StateNames.OrderBy<string, WorkItemStateCategory>((Func<string, WorkItemStateCategory>) (t => s_stateGuidMap[t].StateCategory)).ToList<string>();
      string empty = string.Empty;
      this.WorkItemTypeCategoriesByWorkItemType.TryGetValue(typeDefinition.Name, out empty);
      IReadOnlyDictionary<string, WorkItemStateCategory> readOnlyDictionary = (IReadOnlyDictionary<string, WorkItemStateCategory>) null;
      if (empty != null)
        this.StateCategoryByStateByWorkItemCategory.TryGetValue(empty, out readOnlyDictionary);
      int num1 = 0;
      foreach (string stateName in (IEnumerable<string>) typeDefinition.StateNames)
      {
        WorkItemOobState workItemOobState = s_stateGuidMap[stateName];
        WorkItemStateCategory itemStateCategory = WorkItemStateCategory.Removed;
        if (readOnlyDictionary == null || !readOnlyDictionary.TryGetValue(stateName, out itemStateCategory))
          itemStateCategory = workItemOobState.StateCategory;
        string stateCategoryColor = WorkItemStateDefinitionService.DefaultStateCategoryColors[itemStateCategory];
        int num2 = Math.Max((int) itemStateCategory, num1 + 1);
        num1 = num2;
        itemStateDefinition.Add(WorkItemStateDefinition.Create(new WorkItemStateDefinitionRecord()
        {
          Color = stateCategoryColor,
          Name = stateName,
          ProcessId = typeDefinition.ProcessId,
          StateCategory = (int) itemStateCategory,
          StateId = workItemOobState.Id,
          StateOrder = num2 % (int) itemStateCategory
        }, typeDefinition.ReferenceName, (IEnumerable<WorkItemOobState>) s_stateGuidMap.Values));
      }
      itemStateDefinition.Sort();
      return (IEnumerable<WorkItemStateDefinition>) itemStateDefinition;
    }

    private string GetBugBehaviorCategory() => this.ProcessConfigDocument.Descendants((XName) "BugWorkItems").FirstOrDefault<XElement>()?.Attribute((XName) "category")?.Value;
  }
}
