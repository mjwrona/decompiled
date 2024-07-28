// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityDescriptorComparer
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class IdentityDescriptorComparer : 
    IComparer<IdentityDescriptor>,
    IEqualityComparer<IdentityDescriptor>
  {
    private IdentityDescriptorComparer()
    {
    }

    public int Compare(IdentityDescriptor x, IdentityDescriptor y)
    {
      if ((object) x == (object) y)
        return 0;
      if ((object) x == null && (object) y != null)
        return -1;
      if ((object) x != null && (object) y == null)
        return 1;
      int num = StringComparer.OrdinalIgnoreCase.Compare(x.IdentityType, y.IdentityType);
      if (num != 0 && (x.IsSystemServicePrincipalType() && y.IsClaimsIdentityType() || y.IsSystemServicePrincipalType() && x.IsClaimsIdentityType()))
        num = 0;
      if (num == 0)
        num = StringComparer.OrdinalIgnoreCase.Compare(x.Identifier, y.Identifier);
      return num;
    }

    public bool Equals(IdentityDescriptor x, IdentityDescriptor y) => (object) x == (object) y || this.Compare(x, y) == 0;

    public int GetHashCode(IdentityDescriptor obj)
    {
      int num = 7443;
      string str = obj.IdentityType;
      if (obj.IsSystemServicePrincipalType())
        str = "Microsoft.IdentityModel.Claims.ClaimsIdentity";
      return 524287 * (524287 * num + StringComparer.OrdinalIgnoreCase.GetHashCode(str)) + StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Identifier ?? string.Empty);
    }

    public static IdentityDescriptorComparer Instance { get; } = new IdentityDescriptorComparer();
  }
}
