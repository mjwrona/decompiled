// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.ProcessPermissions
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

namespace Microsoft.TeamFoundation.Framework.Common
{
  public static class ProcessPermissions
  {
    public const int Write = 0;
    public const int Edit = 1;
    public const int Delete = 2;
    public const int Create = 4;
    public const int AdministerProcessPermissions = 8;
    public const int Read = 16;
    public const int ReadRules = 32;
    public const int ManageProcess = 7;
    public const int AllPermissions = 15;
  }
}
