// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Profile.AttributeDescriptorComparer
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Profile
{
  public class AttributeDescriptorComparer : 
    IComparer<AttributeDescriptor>,
    IEqualityComparer<AttributeDescriptor>
  {
    private static AttributeDescriptorComparer s_instance = new AttributeDescriptorComparer();

    private AttributeDescriptorComparer()
    {
    }

    public int Compare(AttributeDescriptor x, AttributeDescriptor y)
    {
      if (x == y)
        return 0;
      if (x == null && y != null)
        return -1;
      return x != null && y == null ? 1 : x.CompareTo(y);
    }

    public bool Equals(AttributeDescriptor x, AttributeDescriptor y) => this.Compare(x, y) == 0;

    public int GetHashCode(AttributeDescriptor obj) => obj.GetHashCode();

    public static AttributeDescriptorComparer Instance => AttributeDescriptorComparer.s_instance;
  }
}
