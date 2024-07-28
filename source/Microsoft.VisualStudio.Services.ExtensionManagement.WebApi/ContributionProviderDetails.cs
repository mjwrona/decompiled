// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ContributionProviderDetails
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  [DataContract]
  public class ContributionProviderDetails
  {
    private int? m_hashCode;

    public ContributionProviderDetails() => this.Properties = new Dictionary<string, string>();

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string DisplayName { get; set; }

    [DataMember]
    public string Version { get; set; }

    [DataMember]
    public Dictionary<string, string> Properties { get; private set; }

    public override int GetHashCode()
    {
      if (!this.m_hashCode.HasValue)
      {
        this.m_hashCode = new int?(0);
        int? hashCode1;
        if (!string.IsNullOrEmpty(this.Name))
        {
          hashCode1 = this.m_hashCode;
          int hashCode2 = this.Name.GetHashCode();
          this.m_hashCode = hashCode1.HasValue ? new int?(hashCode1.GetValueOrDefault() ^ hashCode2) : new int?();
        }
        if (!string.IsNullOrEmpty(this.DisplayName))
        {
          hashCode1 = this.m_hashCode;
          int hashCode3 = this.DisplayName.GetHashCode();
          this.m_hashCode = hashCode1.HasValue ? new int?(hashCode1.GetValueOrDefault() ^ hashCode3) : new int?();
        }
        if (this.Properties != null)
        {
          foreach (string key in this.Properties.Keys)
          {
            hashCode1 = this.m_hashCode;
            int hashCode4 = key.GetHashCode();
            this.m_hashCode = hashCode1.HasValue ? new int?(hashCode1.GetValueOrDefault() ^ hashCode4) : new int?();
            string property = this.Properties[key];
            if (!string.IsNullOrEmpty(property))
            {
              hashCode1 = this.m_hashCode;
              int hashCode5 = property.GetHashCode();
              this.m_hashCode = hashCode1.HasValue ? new int?(hashCode1.GetValueOrDefault() ^ hashCode5) : new int?();
            }
          }
        }
      }
      return this.m_hashCode.Value;
    }
  }
}
