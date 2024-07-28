// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Packaging.WorkItemTrackingProcessTemplatePackage
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Packaging
{
  public class WorkItemTrackingProcessTemplatePackage
  {
    public const string GlobalListXmlFile = "WorkItem Tracking\\GlobalList.xml";
    private ProcessTemplatePackage processTemplatePackage;
    private XDocument processTemplateXml;

    public WorkItemTrackingProcessTemplatePackage(
      ProcessTemplatePackage templatePackage,
      Action<string> logError)
    {
      this.processTemplatePackage = templatePackage;
      this.processTemplateXml = this.processTemplatePackage.GetGroupTaskDocument("WorkItemTracking", logError);
    }

    public IReadOnlyCollection<LinkTypeDeclaration> ReadLinkTypes(Action<string> logError)
    {
      XElement root = this.processTemplateXml.Root;
      LinkTypeDeclaration[] linkTypeDeclarationArray;
      if (root == null)
      {
        linkTypeDeclarationArray = (LinkTypeDeclaration[]) null;
      }
      else
      {
        XElement xelement = root.Elements((XName) "task").FirstOrDefault<XElement>((Func<XElement, bool>) (e => StringComparer.OrdinalIgnoreCase.Equals(e.Attribute((XName) "id")?.Value, "LinkTypes")));
        linkTypeDeclarationArray = xelement != null ? xelement.Descendants((XName) "LINKTYPE").Select<XElement, string>((Func<XElement, string>) (e => Utilities.RequireAttribute(e, (XName) "fileName", logError))).Select<string, XDocument>((Func<string, XDocument>) (fileName => this.processTemplatePackage.GetDocument(fileName, logError))).SelectMany<XDocument, XElement>((Func<XDocument, IEnumerable<XElement>>) (document => document.Descendants((XName) "LinkType"))).Select<XElement, LinkTypeDeclaration>((Func<XElement, LinkTypeDeclaration>) (xmlLinkType => new LinkTypeDeclaration(xmlLinkType))).ToArray<LinkTypeDeclaration>() : (LinkTypeDeclaration[]) null;
      }
      return (IReadOnlyCollection<LinkTypeDeclaration>) linkTypeDeclarationArray ?? (IReadOnlyCollection<LinkTypeDeclaration>) Array.Empty<LinkTypeDeclaration>();
    }

    public IReadOnlyCollection<WorkItemTypeCategoryDeclaration> ReadWorkItemTypeCategories(
      Action<string> logError)
    {
      XElement root = this.processTemplateXml.Root;
      WorkItemTypeCategoryDeclaration[] categoryDeclarationArray;
      if (root == null)
      {
        categoryDeclarationArray = (WorkItemTypeCategoryDeclaration[]) null;
      }
      else
      {
        XElement xelement = root.Elements((XName) "task").FirstOrDefault<XElement>((Func<XElement, bool>) (e => StringComparer.OrdinalIgnoreCase.Equals(e.Attribute((XName) "id")?.Value, "Categories")));
        if (xelement == null)
        {
          categoryDeclarationArray = (WorkItemTypeCategoryDeclaration[]) null;
        }
        else
        {
          IEnumerable<string> source1 = xelement.Descendants((XName) "CATEGORIES").Select<XElement, string>((Func<XElement, string>) (e => Utilities.RequireAttribute(e, (XName) "fileName", logError)));
          if (source1 == null)
          {
            categoryDeclarationArray = (WorkItemTypeCategoryDeclaration[]) null;
          }
          else
          {
            IEnumerable<XDocument> source2 = source1.Select<string, XDocument>((Func<string, XDocument>) (fileName => this.processTemplatePackage.GetDocument(fileName, logError)));
            if (source2 == null)
            {
              categoryDeclarationArray = (WorkItemTypeCategoryDeclaration[]) null;
            }
            else
            {
              IEnumerable<XElement> source3 = source2.SelectMany<XDocument, XElement>((Func<XDocument, IEnumerable<XElement>>) (document => document.Descendants((XName) "CATEGORY")));
              if (source3 == null)
              {
                categoryDeclarationArray = (WorkItemTypeCategoryDeclaration[]) null;
              }
              else
              {
                IEnumerable<WorkItemTypeCategoryDeclaration> source4 = source3.Select<XElement, WorkItemTypeCategoryDeclaration>((Func<XElement, WorkItemTypeCategoryDeclaration>) (xmlCategory => new WorkItemTypeCategoryDeclaration(xmlCategory)));
                categoryDeclarationArray = source4 != null ? source4.ToArray<WorkItemTypeCategoryDeclaration>() : (WorkItemTypeCategoryDeclaration[]) null;
              }
            }
          }
        }
      }
      return (IReadOnlyCollection<WorkItemTypeCategoryDeclaration>) categoryDeclarationArray ?? (IReadOnlyCollection<WorkItemTypeCategoryDeclaration>) Array.Empty<WorkItemTypeCategoryDeclaration>();
    }

    public IReadOnlyCollection<WorkItemTypeDeclaration> ReadWorkItemTypes(Action<string> logError)
    {
      XElement root = this.processTemplateXml.Root;
      WorkItemTypeDeclaration[] itemTypeDeclarationArray;
      if (root == null)
      {
        itemTypeDeclarationArray = (WorkItemTypeDeclaration[]) null;
      }
      else
      {
        XElement xelement = root.Elements((XName) "task").FirstOrDefault<XElement>((Func<XElement, bool>) (e => StringComparer.OrdinalIgnoreCase.Equals(e.Attribute((XName) "id")?.Value, "WITs")));
        if (xelement == null)
        {
          itemTypeDeclarationArray = (WorkItemTypeDeclaration[]) null;
        }
        else
        {
          IEnumerable<string> source1 = xelement.Descendants((XName) "WORKITEMTYPE").Select<XElement, string>((Func<XElement, string>) (e => Utilities.RequireAttribute(e, (XName) "fileName", logError)));
          if (source1 == null)
          {
            itemTypeDeclarationArray = (WorkItemTypeDeclaration[]) null;
          }
          else
          {
            IEnumerable<XDocument> source2 = source1.Select<string, XDocument>((Func<string, XDocument>) (fileName => this.processTemplatePackage.GetDocument(fileName, logError)));
            if (source2 == null)
            {
              itemTypeDeclarationArray = (WorkItemTypeDeclaration[]) null;
            }
            else
            {
              IEnumerable<XElement> source3 = source2.Select<XDocument, XElement>((Func<XDocument, XElement>) (document => document.Root.Element((XName) "WORKITEMTYPE")));
              if (source3 == null)
              {
                itemTypeDeclarationArray = (WorkItemTypeDeclaration[]) null;
              }
              else
              {
                IEnumerable<WorkItemTypeDeclaration> source4 = source3.Select<XElement, WorkItemTypeDeclaration>((Func<XElement, WorkItemTypeDeclaration>) (xmlWorkItemType => new WorkItemTypeDeclaration(xmlWorkItemType, logError)));
                itemTypeDeclarationArray = source4 != null ? source4.ToArray<WorkItemTypeDeclaration>() : (WorkItemTypeDeclaration[]) null;
              }
            }
          }
        }
      }
      return (IReadOnlyCollection<WorkItemTypeDeclaration>) itemTypeDeclarationArray ?? (IReadOnlyCollection<WorkItemTypeDeclaration>) Array.Empty<WorkItemTypeDeclaration>();
    }

    public ProcessConfigurationDeclaration ReadProcessConfiguration(Action<string> logError)
    {
      XElement root = this.processTemplateXml.Root;
      XElement xelement1;
      if (root == null)
      {
        xelement1 = (XElement) null;
      }
      else
      {
        XElement xelement2 = root.Elements((XName) "task").FirstOrDefault<XElement>((Func<XElement, bool>) (e => StringComparer.OrdinalIgnoreCase.Equals(e.Attribute((XName) "id")?.Value, "ProcessConfiguration")));
        if (xelement2 == null)
        {
          xelement1 = (XElement) null;
        }
        else
        {
          IEnumerable<XElement> source = xelement2.Descendants((XName) "ProjectConfiguration");
          xelement1 = source != null ? source.FirstOrDefault<XElement>() : (XElement) null;
        }
      }
      XElement element = xelement1;
      if (element == null)
        return (ProcessConfigurationDeclaration) null;
      XDocument document = this.processTemplatePackage.GetDocument(Utilities.RequireAttribute(element, (XName) "fileName", logError), logError);
      using (XmlReader reader = document.Root.CreateReader())
        return (ProcessConfigurationDeclaration) new XmlSerializer(typeof (ProcessConfigurationDeclaration)).Deserialize(reader);
    }

    public List<GlobalListDeclaration> ReadGlobalLists(Action<string> logError)
    {
      XDocument document = this.processTemplatePackage.GetDocument("WorkItem Tracking\\GlobalList.xml", logError);
      return document == null ? new List<GlobalListDeclaration>() : GlobalListDeclaration.ReadGlobalLists(document.Root, logError);
    }
  }
}
