// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.SecurityUtility
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class SecurityUtility
  {
    public static void MergePermissions(
      int existingAllow,
      int existingDeny,
      int newAllow,
      int newDeny,
      int remove,
      out int updatedAllow,
      out int updatedDeny)
    {
      updatedAllow = (existingAllow & ~remove | newAllow) & ~newDeny;
      updatedDeny = (existingDeny & ~remove | newDeny) & ~updatedAllow;
    }
  }
}
