// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ContributionConstraint
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  [DataContract]
  public class ContributionConstraint
  {
    [DataMember(Order = 10, EmitDefaultValue = false)]
    public string Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public JObject Properties { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool Inverse { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int Group { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<string> Relationships { get; set; }

    public override int GetHashCode()
    {
      int num = 0;
      if (this.Name != null)
        num ^= this.Name.GetHashCode();
      if (this.Properties != null)
        num ^= this.Properties.ToString().GetHashCode();
      int hashCode = num ^ this.Inverse.GetHashCode() ^ this.Group.GetHashCode();
      if (this.Relationships != null)
      {
        foreach (string relationship in this.Relationships)
          hashCode ^= relationship.GetHashCode();
      }
      return hashCode;
    }
  }
}
