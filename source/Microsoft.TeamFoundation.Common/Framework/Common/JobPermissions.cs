// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.JobPermissions
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

namespace Microsoft.TeamFoundation.Framework.Common
{
  public static class JobPermissions
  {
    public const int Read = 1;
    public const int Queue = 2;
    public const int Update = 4;
    public const int AllPermissions = 7;
  }
}
