// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.GroupLicensingRule
{
  [DataContract]
  public class GroupLicensingRule : IEquatable<Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule>
  {
    [DataMember]
    public SubjectDescriptor SubjectDescriptor { get; set; }

    [DataMember]
    public LicenseRule LicenseRule { get; set; }

    [DataMember]
    public IEnumerable<ExtensionRule> ExtensionRules { get; set; }

    public GroupLicensingRuleStatus Status => this.ExtensionRules.Select<ExtensionRule, GroupLicensingRuleStatus>((Func<ExtensionRule, GroupLicensingRuleStatus>) (x => x.Status)).Union<GroupLicensingRuleStatus>((IEnumerable<GroupLicensingRuleStatus>) new GroupLicensingRuleStatus[1]
    {
      this.LicenseRule.Status
    }).HighestSeverity();

    public GroupLicensingRule() => this.ExtensionRules = (IEnumerable<ExtensionRule>) new List<ExtensionRule>();

    public Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule Clone()
    {
      Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule groupLicensingRule = new Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule();
      groupLicensingRule.SubjectDescriptor = this.SubjectDescriptor;
      groupLicensingRule.LicenseRule = new LicenseRule(this.LicenseRule?.License);
      IEnumerable<ExtensionRule> extensionRules = this.ExtensionRules;
      groupLicensingRule.ExtensionRules = (extensionRules != null ? extensionRules.Select<ExtensionRule, ExtensionRule>((Func<ExtensionRule, ExtensionRule>) (x => new ExtensionRule(x.ExtensionId))) : (IEnumerable<ExtensionRule>) null) ?? (IEnumerable<ExtensionRule>) new List<ExtensionRule>();
      return groupLicensingRule;
    }

    public override bool Equals(object obj) => this.Equals(obj as Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule);

    public bool Equals(Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule other) => other != (Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule) null && this.SubjectDescriptor.Equals(other.SubjectDescriptor) && this.LicenseRule.Equals(other.LicenseRule) && this.ExtensionRules.OrderBy<ExtensionRule, string>((Func<ExtensionRule, string>) (e => e.ExtensionId)).SequenceEqual<ExtensionRule>((IEnumerable<ExtensionRule>) other.ExtensionRules.OrderBy<ExtensionRule, string>((Func<ExtensionRule, string>) (e => e.ExtensionId)));

    public override int GetHashCode() => this.SubjectDescriptor.GetHashCode();

    public static bool Equals(Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule left, Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule right)
    {
      if ((object) left == null)
        return (object) right == null;
      return (object) right != null && left.Equals(right);
    }

    public static bool operator ==(Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule left, Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule right) => Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule.Equals(left, right);

    public static bool operator !=(Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule left, Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule right) => !Microsoft.VisualStudio.Services.GroupLicensingRule.GroupLicensingRule.Equals(left, right);
  }
}
