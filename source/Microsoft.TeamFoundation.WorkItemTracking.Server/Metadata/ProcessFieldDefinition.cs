// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ProcessFieldDefinition
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class ProcessFieldDefinition : IEquatable<ProcessFieldDefinition>
  {
    public string ReferenceName { get; set; }

    public string Name { get; set; }

    public InternalFieldType Type { get; set; }

    public string HelpText { get; set; }

    public int ReportingType { get; set; }

    public int ReportingFormula { get; set; }

    public bool SyncNameChanges { get; set; }

    public string DefaultValue { get; set; }

    public bool IsRequired { get; set; }

    public bool IsReadOnly { get; set; }

    public Guid ProcessId { get; set; }

    public bool IsIdentity { get; set; }

    public bool IsLocked { get; set; }

    public ProcessFieldDefinition Clone() => this.MemberwiseClone() as ProcessFieldDefinition;

    public bool Equals(ProcessFieldDefinition other)
    {
      if (other == null)
        return false;
      return this == other || TFStringComparer.WorkItemFieldReferenceName.Equals(this.ReferenceName, other.ReferenceName);
    }

    public WorkItemFieldRule ConvertToFieldRule()
    {
      WorkItemFieldRule fieldRule = new WorkItemFieldRule()
      {
        Field = this.ReferenceName
      };
      if (!string.IsNullOrEmpty(this.DefaultValue))
      {
        WorkItemFieldRule workItemFieldRule = fieldRule;
        DefaultRule rule = new DefaultRule();
        rule.Value = this.DefaultValue;
        rule.ValueFrom = RuleValueFrom.Value;
        workItemFieldRule.AddRule<DefaultRule>(rule);
      }
      if (this.IsRequired)
        fieldRule.AddRule<RequiredRule>(new RequiredRule());
      if (this.IsReadOnly)
        fieldRule.AddRule<ReadOnlyRule>(new ReadOnlyRule());
      return fieldRule;
    }

    public ProcessFieldResult ConvertToFieldResult() => new ProcessFieldResult()
    {
      Name = this.Name,
      ReferenceName = this.ReferenceName,
      Type = this.Type,
      Description = this.HelpText,
      IsSystem = true,
      IsIdentity = this.IsIdentity
    };

    public override int GetHashCode()
    {
      string referenceName = this.ReferenceName;
      return referenceName == null ? 0 : referenceName.GetHashCode();
    }

    public static ProcessFieldDefinition CreateFromLegacyDeclaration(
      XElement fieldElement,
      XElement typeElement)
    {
      ArgumentUtility.CheckForNull<XElement>(fieldElement, nameof (fieldElement));
      ProcessFieldDefinition legacyDeclaration = new ProcessFieldDefinition();
      legacyDeclaration.Name = fieldElement.Attribute((XName) "name")?.Value;
      legacyDeclaration.ReferenceName = fieldElement.Attribute((XName) "refname")?.Value;
      InternalFieldType result1;
      Enum.TryParse<InternalFieldType>(fieldElement.Attribute((XName) "type")?.Value, true, out result1);
      legacyDeclaration.Type = result1;
      string str1 = fieldElement.Attribute((XName) "syncnamechanges")?.Value;
      bool result2;
      legacyDeclaration.SyncNameChanges = !string.IsNullOrEmpty(str1) && bool.TryParse(str1, out result2) && result2;
      string str2 = fieldElement.Attribute((XName) "reportable")?.Value;
      legacyDeclaration.ReportingType = string.IsNullOrEmpty(str2) ? 0 : FieldHelpers.ParseReportabilityString(str2);
      string str3 = fieldElement.Attribute((XName) "formula")?.Value;
      legacyDeclaration.ReportingFormula = string.IsNullOrEmpty(str3) ? 0 : FieldHelpers.ParseFormulaString(str3);
      legacyDeclaration.HelpText = fieldElement.Element((XName) "HELPTEXT")?.Value;
      legacyDeclaration.DefaultValue = fieldElement.Element((XName) "DEFAULT")?.Attribute((XName) "value")?.Value;
      legacyDeclaration.IsReadOnly = fieldElement.Element((XName) "READONLY") != null;
      legacyDeclaration.IsRequired = fieldElement.Element((XName) "REQUIRED") != null;
      legacyDeclaration.IsIdentity = legacyDeclaration.IsIdentityElement(fieldElement, typeElement);
      return legacyDeclaration;
    }

    private bool IsIdentityElement(XElement fieldElement, XElement typeElement)
    {
      string refName = fieldElement.Attribute((XName) "refname").Value;
      IEnumerable<XElement> collection = typeElement.Descendants((XName) "WORKFLOW").Descendants<XElement>((XName) "FIELD").Where<XElement>((Func<XElement, bool>) (e => e.Attribute((XName) "refname")?.Value == refName));
      List<XElement> source = new List<XElement>();
      source.Add(fieldElement);
      source.AddRange(collection);
      return source.Any<XElement>((Func<XElement, bool>) (f => f.Descendants((XName) "VALIDUSER").Any<XElement>() || ProcessFieldDefinition.ContainsIdentityListItem(f, "ALLOWEDVALUES") || ProcessFieldDefinition.ContainsIdentityListItem(f, "PROHIBITEDVALUES"))) || ((IEnumerable<string>) CoreFieldConstants.CoreIdentityFields).Contains<string>(refName) || this.SyncNameChanges;
    }

    private static bool ContainsIdentityListItem(XElement field, string nodeName) => field.Descendants((XName) nodeName).Any<XElement>() && field.Descendants((XName) nodeName).Any<XElement>((Func<XElement, bool>) (d => d.Descendants((XName) "LISTITEM").Any<XElement>((Func<XElement, bool>) (li => li.Attribute((XName) "value") != null && li.Attribute((XName) "value").Value.Contains("\\")))));
  }
}
