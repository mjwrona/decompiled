// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.TeamProjectCollectionPermissions
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

namespace Microsoft.TeamFoundation.Framework.Common
{
  public static class TeamProjectCollectionPermissions
  {
    public static readonly int GenericRead = 1;
    public static readonly int GenericWrite = 2;
    public static readonly int CreateProjects = 4;
    public static readonly int AdministerWarehouse = 8;
    public static readonly int TriggerEvent = 16;
    public static readonly int ManageTemplate = 32;
    public static readonly int DiagnosticTrace = 64;
    public static readonly int SynchronizeRead = 128;
    public static readonly int ManageTestControllers = 512;
    public static readonly int DeleteField = 1024;
    public static readonly int ManageEnterprisePolicies = 2048;
    public static readonly int AllPermissions = TeamProjectCollectionPermissions.GenericRead | TeamProjectCollectionPermissions.GenericWrite | TeamProjectCollectionPermissions.CreateProjects | TeamProjectCollectionPermissions.AdministerWarehouse | TeamProjectCollectionPermissions.TriggerEvent | TeamProjectCollectionPermissions.ManageTemplate | TeamProjectCollectionPermissions.DiagnosticTrace | TeamProjectCollectionPermissions.SynchronizeRead | TeamProjectCollectionPermissions.ManageTestControllers;
  }
}
