// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.IdentityDescriptorComparer
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public class IdentityDescriptorComparer : 
    IComparer<IdentityDescriptor>,
    IEqualityComparer<IdentityDescriptor>
  {
    private static IdentityDescriptorComparer s_instance = new IdentityDescriptorComparer();

    private IdentityDescriptorComparer()
    {
    }

    public int Compare(IdentityDescriptor x, IdentityDescriptor y)
    {
      if (x == y)
        return 0;
      if (x == null && y != null)
        return -1;
      if (x != null && y == null)
        return 1;
      int num;
      return (num = VssStringComparer.IdentityDescriptor.Compare(x.Identifier, y.Identifier)) != 0 ? num : VssStringComparer.IdentityDescriptor.Compare(x.IdentityType, y.IdentityType);
    }

    public bool Equals(IdentityDescriptor x, IdentityDescriptor y) => this.Compare(x, y) == 0;

    public int GetHashCode(IdentityDescriptor obj) => obj.Identifier.GetHashCode();

    public static IdentityDescriptorComparer Instance => IdentityDescriptorComparer.s_instance;
  }
}
