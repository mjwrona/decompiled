// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.PermissionLevelAssignmentsExtensions
// Assembly: Microsoft.VisualStudio.Services.PermissionLevel, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 43771064-3FEF-4CA1-8A8B-671AEDB99122
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PermissionLevel.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.PermissionLevel.DataAccess;
using System;

namespace Microsoft.VisualStudio.Services.PermissionLevel
{
  public static class PermissionLevelAssignmentsExtensions
  {
    public static PermissionLevelAssignment ToPermissionLevelAssignment(
      this PermissionLevelAssignmentStoreItem permissionLevelAssignmentStoreItem,
      IVssRequestContext context)
    {
      if (permissionLevelAssignmentStoreItem == null)
        return (PermissionLevelAssignment) null;
      SubjectDescriptor subjectDescriptor = PermissionLevelAssignmentsExtensions.GetSubjectDescriptor(context, permissionLevelAssignmentStoreItem.SubjectId);
      return new PermissionLevelAssignment(permissionLevelAssignmentStoreItem.PermissionLevelDefinitionId, permissionLevelAssignmentStoreItem.ResourceId, subjectDescriptor);
    }

    public static PermissionLevelAssignmentStoreItem ToPermissionLevelAssignmentStoreItem(
      this PermissionLevelAssignment permissionLevelAssignment,
      IVssRequestContext context)
    {
      if (permissionLevelAssignment == null)
        return (PermissionLevelAssignmentStoreItem) null;
      Guid subjectId = PermissionLevelAssignmentsExtensions.GetSubjectId(context, permissionLevelAssignment.Subject);
      return new PermissionLevelAssignmentStoreItem(context.ServiceHost.InstanceId, permissionLevelAssignment.PermissionLevelDefinitionId, permissionLevelAssignment.ResourceId, subjectId);
    }

    public static Guid GetSubjectId(IVssRequestContext context, SubjectDescriptor subjectDescriptor)
    {
      context.CheckProjectCollectionRequestContext();
      return context.GetService<IGraphIdentifierConversionService>().GetStorageKeyByDescriptor(context, subjectDescriptor);
    }

    public static SubjectDescriptor GetSubjectDescriptor(IVssRequestContext context, Guid subjectId)
    {
      context.CheckProjectCollectionRequestContext();
      return context.GetService<IGraphIdentifierConversionService>().GetDescriptorByStorageKey(context, subjectId);
    }
  }
}
