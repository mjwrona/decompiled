// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Profile.AttributeDescriptor
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Profile
{
  public class AttributeDescriptor : IComparable<AttributeDescriptor>, ICloneable
  {
    private string m_attributeName;
    private string m_containerName;

    public AttributeDescriptor(string containerName, string attributeName)
    {
      this.AttributeName = attributeName;
      this.ContainerName = containerName;
    }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string AttributeName
    {
      get => this.m_attributeName;
      set
      {
        ProfileArgumentValidation.ValidateAttributeName(value);
        this.m_attributeName = value;
      }
    }

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

    public int CompareTo(AttributeDescriptor obj)
    {
      if (this == obj)
        return 0;
      if (obj == null)
        return 1;
      int num;
      return (num = VssStringComparer.AttributesDescriptor.Compare(this.AttributeName, obj.AttributeName)) != 0 ? num : VssStringComparer.AttributesDescriptor.Compare(this.ContainerName, obj.ContainerName);
    }

    public override bool Equals(object obj) => obj != null && !(this.GetType() != obj.GetType()) && this.CompareTo((AttributeDescriptor) obj) == 0;

    public override int GetHashCode() => this.ContainerName.GetHashCode() + this.AttributeName.GetHashCode();

    public object Clone() => (object) new AttributeDescriptor(this.ContainerName, this.AttributeName);

    public override string ToString() => this.ContainerName + ";" + this.AttributeName;
  }
}
