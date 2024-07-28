// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ApplicationExtensionLicensingComponent
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class ApplicationExtensionLicensingComponent : 
    BaseLicenseComponent,
    IExtensionLicensingComponent,
    IDisposable
  {
    private static Dictionary<int, SqlExceptionFactory> sqlExceptionFactories;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[10]
    {
      (IComponentCreator) new ComponentCreator<ApplicationExtensionLicensingComponent1>(1),
      (IComponentCreator) new ComponentCreator<ApplicationExtensionLicensingComponent2>(2),
      (IComponentCreator) new ComponentCreator<ApplicationExtensionLicensingComponent3>(3),
      (IComponentCreator) new ComponentCreator<ApplicationExtensionLicensingComponent4>(4),
      (IComponentCreator) new ComponentCreator<ApplicationExtensionLicensingComponent5>(5),
      (IComponentCreator) new ComponentCreator<ApplicationExtensionLicensingComponent6>(6),
      (IComponentCreator) new ComponentCreator<ApplicationExtensionLicensingComponent7>(7),
      (IComponentCreator) new ComponentCreator<ApplicationExtensionLicensingComponent8>(8),
      (IComponentCreator) new ComponentCreator<ApplicationExtensionLicensingComponent9>(9),
      (IComponentCreator) new ComponentCreator<ApplicationExtensionLicensingComponent10>(10)
    }, "ExtensionLicensing");

    static ApplicationExtensionLicensingComponent() => ApplicationExtensionLicensingComponent.sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        1060202,
        new SqlExceptionFactory(typeof (FailedToGetExtensionLicenseStatus))
      },
      {
        1060204,
        new SqlExceptionFactory(typeof (FailedToUpsertExtensionLicenseStatus))
      },
      {
        1060203,
        new SqlExceptionFactory(typeof (FailedToUpsertExtensionLicenseStatus))
      },
      {
        1060201,
        new SqlExceptionFactory(typeof (FailedToUpsertExtensionLicenseStatus))
      },
      {
        1060205,
        new SqlExceptionFactory(typeof (FailedToSynchronizeUserExtensions))
      },
      {
        1060206,
        new SqlExceptionFactory(typeof (FailedToCopyExtensionLicense))
      },
      {
        1060301,
        new SqlExceptionFactory(typeof (FailedToDeleteUserExtensionLicense))
      },
      {
        1060207,
        new SqlExceptionFactory(typeof (FailedToGetUserExtensionLicenses))
      },
      {
        1060110,
        new SqlExceptionFactory(typeof (LicenseScopeNotFoundException))
      }
    };

    [ExcludeFromCodeCoverage]
    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) ApplicationExtensionLicensingComponent.sqlExceptionFactories;

    protected virtual string TraceLayer => nameof (ApplicationExtensionLicensingComponent);

    public ApplicationExtensionLicensingComponent()
    {
      this.ContainerErrorCode = 50000;
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    public virtual IList<UserExtensionLicense> GetUserExtensionLicenses(
      Guid scopeId,
      Guid userId,
      UserExtensionLicenseStatus status)
    {
      throw new NotImplementedException();
    }

    public virtual IList<UserExtensionLicense> GetUserExtensionLicenses(Guid scopeId) => throw new ServiceVersionNotSupportedException("ExtensionLicensing", this.Version, 7);

    public virtual IDictionary<Guid, IList<ExtensionSource>> GetExtensionsForUsersBatch(
      Guid scopeId,
      IList<Guid> userIds)
    {
      throw new ServiceVersionNotSupportedException("ExtensionLicensing", this.Version, 6);
    }

    public virtual IList<AccountExtensionCount> GetAccountExtensionCount(
      Guid scopeId,
      UserExtensionLicenseStatus status)
    {
      throw new ServiceVersionNotSupportedException();
    }

    internal virtual void AssignExtensionLicenseToUser(
      Guid scopeId,
      Guid userId,
      string extensionId,
      LicensingSource source,
      AssignmentSource assignmentSource)
    {
      throw new NotImplementedException();
    }

    internal virtual void AssignExtensionLicenseToUser(
      Guid scopeId,
      Guid userId,
      string extensionId,
      LicensingSource source,
      AssignmentSource assignmentSource,
      Guid collectionId)
    {
      this.AssignExtensionLicenseToUser(scopeId, userId, extensionId, source, assignmentSource);
    }

    internal virtual void UpdateUserStatus(
      Guid scopeId,
      Guid userId,
      string extensionId,
      UserExtensionLicenseStatus status,
      LicensingSource source,
      AssignmentSource assignmentSource)
    {
      throw new NotImplementedException();
    }

    internal virtual void UpdateUserStatus(
      Guid scopeId,
      Guid userId,
      string extensionId,
      UserExtensionLicenseStatus status,
      LicensingSource source,
      AssignmentSource assignmentSource,
      Guid CollectionId)
    {
      this.UpdateUserStatus(scopeId, userId, extensionId, status, source, assignmentSource);
    }

    public virtual int GetExtensionUsageCountInAccount(Guid scopeId, string extensionId) => throw new NotImplementedException();

    internal virtual void AssignExtensionLicenseToUserBatch(
      Guid scopeId,
      IEnumerable<Guid> userIds,
      string extensionId,
      LicensingSource source,
      AssignmentSource assignmentSource)
    {
      throw new NotImplementedException();
    }

    public virtual void AssignExtensionLicenseToUserBatch(
      Guid scopeId,
      IEnumerable<Guid> userIds,
      string extensionId,
      LicensingSource source,
      AssignmentSource assignmentSource,
      Guid collectionId)
    {
      this.AssignExtensionLicenseToUserBatch(scopeId, userIds, extensionId, source, assignmentSource);
    }

    internal virtual void UpdateUserStatusBatch(
      Guid scopeId,
      IEnumerable<Guid> userIds,
      string extensionId,
      UserExtensionLicenseStatus status,
      LicensingSource source,
      AssignmentSource assignmentSource)
    {
      throw new NotImplementedException();
    }

    public virtual void UpdateUserStatusBatch(
      Guid scopeId,
      IEnumerable<Guid> userIds,
      string extensionId,
      UserExtensionLicenseStatus status,
      LicensingSource source,
      AssignmentSource assignmentSource,
      Guid CollectionId)
    {
      this.UpdateUserStatusBatch(scopeId, userIds, extensionId, status, source, assignmentSource);
    }

    internal virtual void UpdateExtensionsAssignedToUserBatch(
      Guid scopeId,
      Guid userIds,
      IEnumerable<string> extensionIds,
      LicensingSource source)
    {
      throw new NotImplementedException();
    }

    public virtual IList<UserExtensionLicense> FilterUsersWithExtensionBatch(
      Guid scopeId,
      IEnumerable<Guid> userIds,
      string extensionId)
    {
      throw new NotImplementedException();
    }

    internal virtual int UpdateExtensionsAssignedToUserBatchWithCount(
      Guid scopeId,
      Guid userId,
      IEnumerable<string> extensionIds,
      LicensingSource source,
      AssignmentSource assignmentSource)
    {
      throw new NotImplementedException();
    }

    public virtual int UpdateExtensionsAssignedToUserBatchWithCount(
      Guid scopeId,
      Guid userId,
      IEnumerable<string> extensionIds,
      LicensingSource source,
      AssignmentSource assignmentSource,
      Guid collectionId)
    {
      return this.UpdateExtensionsAssignedToUserBatchWithCount(scopeId, userId, extensionIds, source, assignmentSource);
    }

    public virtual void TransferUserExtensionLicenses(
      Guid scopeId,
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap)
    {
      throw new NotImplementedException();
    }
  }
}
