// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ContributionPropertyDescription
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  [DataContract]
  public class ContributionPropertyDescription
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public bool Required { get; set; }

    [DataMember]
    public ContributionPropertyType Type { get; set; }

    public override int GetHashCode()
    {
      int num = 0;
      if (this.Name != null)
        num ^= this.Name.GetHashCode();
      if (this.Description != null)
        num ^= this.Description.GetHashCode();
      return num ^ this.Required.GetHashCode() ^ this.Type.GetHashCode();
    }
  }
}
