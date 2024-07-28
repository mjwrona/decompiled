// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.ServiceConnectionAuditConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  public static class ServiceConnectionAuditConstants
  {
    private static readonly string LibraryArea = "Library.";
    public static readonly string ServiceConnectionCreated = ServiceConnectionAuditConstants.LibraryArea + nameof (ServiceConnectionCreated);
    public static readonly string ServiceConnectionCreatedForMultipleProjects = ServiceConnectionAuditConstants.LibraryArea + nameof (ServiceConnectionCreatedForMultipleProjects);
    public static readonly string ServiceConnectionDeleted = ServiceConnectionAuditConstants.LibraryArea + nameof (ServiceConnectionDeleted);
    public static readonly string ServiceConnectionDeletedFromMultipleProjects = ServiceConnectionAuditConstants.LibraryArea + nameof (ServiceConnectionDeletedFromMultipleProjects);
    public static readonly string ServiceConnectionModified = ServiceConnectionAuditConstants.LibraryArea + nameof (ServiceConnectionModified);
    public static readonly string ServiceConnectionForProjectModified = ServiceConnectionAuditConstants.LibraryArea + nameof (ServiceConnectionForProjectModified);
    public static readonly string ServiceConnectionShared = ServiceConnectionAuditConstants.LibraryArea + nameof (ServiceConnectionShared);
    public static readonly string ServiceConnectionSharedWithMultipleProjects = ServiceConnectionAuditConstants.LibraryArea + nameof (ServiceConnectionSharedWithMultipleProjects);
    public static readonly string ServiceConnectionExecuted = ServiceConnectionAuditConstants.LibraryArea + nameof (ServiceConnectionExecuted);
    public static readonly string ServiceConnectionPropertyChanged = ServiceConnectionAuditConstants.LibraryArea + nameof (ServiceConnectionPropertyChanged);
    public const string ServiceConnectionId = "EndpointId";
    public const string ServiceConnectionIsDisabled = "IsDisabled";
    public const string ServiceConnectionType = "Type";
  }
}
