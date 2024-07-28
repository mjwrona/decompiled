// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ContributionType
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  [DataContract]
  public class ContributionType : ContributionBase
  {
    private bool m_hashCalculated;
    private int m_hashCode;

    [DataMember(Order = 20, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(Order = 50, EmitDefaultValue = false)]
    public bool Indexed { get; set; }

    [DataMember(Order = 100, EmitDefaultValue = false)]
    public Dictionary<string, ContributionPropertyDescription> Properties { get; set; }

    public override int GetHashCode()
    {
      int num1 = 0;
      if (!this.m_hashCalculated)
      {
        int num2 = num1 ^ base.GetHashCode();
        if (this.Name != null)
          num2 ^= this.Name.GetHashCode();
        int num3 = num2 ^ this.Indexed.GetHashCode();
        if (this.Properties != null)
        {
          foreach (string key in this.Properties.Keys)
          {
            num3 ^= key.GetHashCode();
            num3 ^= this.Properties[key].GetHashCode();
          }
        }
        this.m_hashCode = num3;
        this.m_hashCalculated = true;
      }
      return this.m_hashCode;
    }
  }
}
