// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.GroupLicensingRule.ExtensionRule
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.GroupLicensingRule
{
  [DataContract]
  public class ExtensionRule : IEquatable<ExtensionRule>
  {
    [DataMember]
    public string ExtensionId { get; set; }

    [DataMember]
    public GroupLicensingRuleStatus Status { get; set; }

    public ExtensionRule()
    {
    }

    public ExtensionRule(string extensionId) => this.ExtensionId = extensionId;

    public override bool Equals(object obj) => this.Equals(obj as ExtensionRule);

    public bool Equals(ExtensionRule other) => other != (ExtensionRule) null && this.ExtensionId.Equals(other.ExtensionId);

    public override int GetHashCode() => this.ExtensionId.GetHashCode();

    public static bool Equals(ExtensionRule left, ExtensionRule right)
    {
      if ((object) left == null)
        return (object) right == null;
      return (object) right != null && left.Equals(right);
    }

    public static bool operator ==(ExtensionRule left, ExtensionRule right) => ExtensionRule.Equals(left, right);

    public static bool operator !=(ExtensionRule left, ExtensionRule right) => !ExtensionRule.Equals(left, right);
  }
}
