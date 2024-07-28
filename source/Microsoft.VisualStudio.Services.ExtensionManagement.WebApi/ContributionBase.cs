// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ContributionBase
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  [DataContract]
  public abstract class ContributionBase
  {
    [DataMember(Order = 10, EmitDefaultValue = false)]
    public string Id { get; set; }

    [DataMember(Order = 30, EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(Order = 40, EmitDefaultValue = false)]
    public List<string> VisibleTo { get; set; }

    public override int GetHashCode()
    {
      int hashCode = 0;
      if (this.Id != null)
        hashCode ^= this.Id.GetHashCode();
      if (this.Description != null)
        hashCode ^= this.Description.GetHashCode();
      if (this.VisibleTo != null)
      {
        foreach (string str in this.VisibleTo)
          hashCode ^= str.GetHashCode();
      }
      return hashCode;
    }
  }
}
