// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ProcessWorkItemTypeDefinition
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class ProcessWorkItemTypeDefinition : 
    BaseWorkItemType,
    IEquatable<ProcessWorkItemTypeDefinition>
  {
    private List<ProcessFieldDefinition> m_fields = new List<ProcessFieldDefinition>();

    public IReadOnlyCollection<ProcessFieldDefinition> FieldDefinitions
    {
      get => (IReadOnlyCollection<ProcessFieldDefinition>) this.m_fields;
      set
      {
        this.m_fields.Clear();
        if (value == null)
          return;
        this.m_fields.AddRange((IEnumerable<ProcessFieldDefinition>) value);
      }
    }

    public virtual Layout Form { get; set; }

    public Guid ProcessId { get; set; }

    public string FormXml { get; set; }

    public IList<string> StateNames { get; set; }

    public IEnumerable<ProcessWorkItemTypeActionDefinition> Actions { get; set; }

    public IEnumerable<WorkItemStateDefinition> States { get; set; }

    public string InitialState { get; set; }

    public static ProcessWorkItemTypeDefinition CreateFromLegacyDeclaration(XDocument typeDocument)
    {
      ArgumentUtility.CheckForNull<XDocument>(typeDocument, nameof (typeDocument));
      XElement typeElement = typeDocument.Root?.Element((XName) "WORKITEMTYPE");
      ProcessWorkItemTypeDefinition legacyDeclaration = new ProcessWorkItemTypeDefinition();
      if (typeElement != null)
      {
        legacyDeclaration.Name = typeElement.Attribute((XName) "name")?.Value;
        legacyDeclaration.ReferenceName = typeElement.Attribute((XName) "refname")?.Value;
        IEnumerable<XElement> source1 = typeElement.Element((XName) "FIELDS")?.Elements((XName) "FIELD");
        if (source1 != null)
          legacyDeclaration.m_fields.AddRange(source1.Select<XElement, ProcessFieldDefinition>((Func<XElement, ProcessFieldDefinition>) (field => ProcessFieldDefinition.CreateFromLegacyDeclaration(field, typeElement))));
        ProcessWorkItemTypeDefinition itemTypeDefinition = legacyDeclaration;
        XElement xelement1 = typeElement.Element((XName) "WORKFLOW");
        List<string> stringList;
        if (xelement1 == null)
        {
          stringList = (List<string>) null;
        }
        else
        {
          IEnumerable<XElement> source2 = xelement1.Elements((XName) "STATES");
          if (source2 == null)
          {
            stringList = (List<string>) null;
          }
          else
          {
            IEnumerable<XElement> source3 = source2.Elements<XElement>((XName) "STATE");
            if (source3 == null)
            {
              stringList = (List<string>) null;
            }
            else
            {
              IEnumerable<XAttribute> source4 = source3.Attributes((XName) "value");
              if (source4 == null)
              {
                stringList = (List<string>) null;
              }
              else
              {
                IEnumerable<string> source5 = source4.Select<XAttribute, string>((Func<XAttribute, string>) (a => a.Value));
                stringList = source5 != null ? source5.ToList<string>() : (List<string>) null;
              }
            }
          }
        }
        if (stringList == null)
          stringList = new List<string>();
        itemTypeDefinition.StateNames = (IList<string>) stringList;
        legacyDeclaration.FormXml = typeElement.Element((XName) "FORM")?.ToString();
        legacyDeclaration.Description = typeElement.Element((XName) "DESCRIPTION")?.Value;
        IEnumerable<XElement> source6 = typeElement.Element((XName) "WORKFLOW")?.Elements((XName) "TRANSITIONS");
        List<ProcessWorkItemTypeActionDefinition> actionDefinitionList = new List<ProcessWorkItemTypeActionDefinition>();
        if (source6 != null)
        {
          foreach (XElement element in source6.Elements<XElement>((XName) "TRANSITION"))
          {
            string fromState = element.Attribute((XName) "from")?.Value;
            string toState = element.Attribute((XName) "to")?.Value;
            IEnumerable<XElement> source7 = element.Element((XName) "ACTIONS")?.Elements((XName) "ACTION");
            if (source7 != null && source7.Any<XElement>())
            {
              foreach (XElement xelement2 in source7)
              {
                ProcessWorkItemTypeActionDefinition actionDefinition = new ProcessWorkItemTypeActionDefinition(xelement2.Attribute((XName) "value")?.Value, legacyDeclaration.Name, fromState, toState);
                actionDefinitionList.Add(actionDefinition);
              }
            }
            if (fromState == "")
              legacyDeclaration.InitialState = element.Attribute((XName) "to").Value;
          }
        }
        legacyDeclaration.Actions = (IEnumerable<ProcessWorkItemTypeActionDefinition>) actionDefinitionList;
      }
      return legacyDeclaration;
    }

    public bool Equals(ProcessWorkItemTypeDefinition other) => other != null && TFStringComparer.WorkItemTypeName.Equals(this.Name, other.Name) && TFStringComparer.WorkItemTypeReferenceName.Equals(this.ReferenceName, other.ReferenceName);

    public override bool Equals(object obj) => this.Equals(obj as ProcessWorkItemTypeDefinition);

    public override int GetHashCode()
    {
      string name = this.Name;
      int hashCode1 = name != null ? name.GetHashCode() : 0;
      int num = (hashCode1 << 5) + hashCode1;
      string referenceName = this.ReferenceName;
      int hashCode2 = referenceName != null ? referenceName.GetHashCode() : 0;
      return num ^ hashCode2;
    }
  }
}
