// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.TaggingPermissions
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

namespace Microsoft.TeamFoundation.Framework.Common
{
  public static class TaggingPermissions
  {
    public static readonly int Enumerate = 1;
    public static readonly int Create = 2;
    public static readonly int Update = 4;
    public static readonly int Delete = 8;
    public static readonly int AllPermissions = TaggingPermissions.Enumerate | TaggingPermissions.Create | TaggingPermissions.Update | TaggingPermissions.Delete;
  }
}
