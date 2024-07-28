// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.AuthorizationNamespacePermissions
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;

namespace Microsoft.TeamFoundation.Server
{
  public static class AuthorizationNamespacePermissions
  {
    public static readonly int GenericRead = TeamProjectCollectionPermissions.GenericRead;
    public static readonly int GenericWrite = TeamProjectCollectionPermissions.GenericWrite;
    public static readonly int CreateProjects = TeamProjectCollectionPermissions.CreateProjects;
    public static readonly int AdministerWarehouse = TeamProjectCollectionPermissions.AdministerWarehouse;
    public static readonly int TriggerEvent = TeamProjectCollectionPermissions.TriggerEvent;
    public static readonly int ManageTemplate = TeamProjectCollectionPermissions.ManageTemplate;
    public static readonly int DiagnosticTrace = TeamProjectCollectionPermissions.DiagnosticTrace;
    public static readonly int SynchronizeRead = TeamProjectCollectionPermissions.SynchronizeRead;
    public static readonly int ManageLinkTypes = 256;
    public static readonly int ManageTestControllers = TeamProjectCollectionPermissions.ManageTestControllers;
    public static readonly int AllPermissions = TeamProjectCollectionPermissions.AllPermissions;
  }
}
