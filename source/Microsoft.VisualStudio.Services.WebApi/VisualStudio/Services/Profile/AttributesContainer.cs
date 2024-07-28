// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Profile.AttributesContainer
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Profile
{
  [DataContract]
  public class AttributesContainer : IVersioned, ICloneable
  {
    private string m_containerName;

    public AttributesContainer(string containerName)
      : this()
    {
      this.ContainerName = containerName;
    }

    public AttributesContainer() => this.Attributes = (IDictionary<string, ProfileAttribute>) new Dictionary<string, ProfileAttribute>((IEqualityComparer<string>) VssStringComparer.AttributesDescriptor);

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string ContainerName
    {
      get => this.m_containerName;
      set
      {
        ProfileArgumentValidation.ValidateContainerName(value);
        this.m_containerName = value;
      }
    }

    public object Clone()
    {
      AttributesContainer attributesContainer = (AttributesContainer) this.MemberwiseClone();
      attributesContainer.Attributes = this.Attributes != null ? (IDictionary<string, ProfileAttribute>) this.Attributes.ToDictionary<KeyValuePair<string, ProfileAttribute>, string, ProfileAttribute>((Func<KeyValuePair<string, ProfileAttribute>, string>) (x => x.Key), (Func<KeyValuePair<string, ProfileAttribute>, ProfileAttribute>) (x => (ProfileAttribute) x.Value.Clone())) : (IDictionary<string, ProfileAttribute>) null;
      return (object) attributesContainer;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IDictionary<string, ProfileAttribute> Attributes { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public int Revision { get; set; }
  }
}
