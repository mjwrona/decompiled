// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ExtensionBinders
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal static class ExtensionBinders
  {
    internal class UserExtensionLicenseRowBinder : ObjectBinder<UserExtensionLicense>
    {
      protected SqlColumnBinder m_userIdColumn = new SqlColumnBinder("UserId");
      protected SqlColumnBinder m_extensionIdColumn = new SqlColumnBinder("ExtensionId");
      protected SqlColumnBinder m_statusColumn = new SqlColumnBinder("Status");
      protected SqlColumnBinder m_sourceColumn = new SqlColumnBinder("Source");
      protected SqlColumnBinder m_assignmentDateColumn = new SqlColumnBinder("AssignmentDate");
      protected SqlColumnBinder m_collectionIdColumn = new SqlColumnBinder("CollectionId");
      protected Guid m_accountId;

      internal UserExtensionLicenseRowBinder()
      {
      }

      internal UserExtensionLicenseRowBinder(Guid accountId)
        : this()
      {
        this.m_accountId = accountId;
      }

      protected override UserExtensionLicense Bind() => this.GetUserExtensionFromReader((DbDataReader) this.Reader);

      internal UserExtensionLicense GetUserExtensionFromReader(DbDataReader reader)
      {
        Guid guid1 = this.m_userIdColumn.GetGuid((IDataReader) reader);
        string str = this.m_extensionIdColumn.GetString((IDataReader) reader, false);
        UserExtensionLicenseStatus extensionLicenseStatus = (UserExtensionLicenseStatus) this.m_statusColumn.GetByte((IDataReader) reader);
        LicensingSource licensingSource = (LicensingSource) this.m_sourceColumn.GetByte((IDataReader) reader);
        DateTime dateTime = this.m_assignmentDateColumn.GetDateTime((IDataReader) reader);
        Guid guid2 = this.m_collectionIdColumn.GetGuid((IDataReader) reader, true, Guid.Empty);
        return new UserExtensionLicense()
        {
          AccountId = this.m_accountId,
          UserId = guid1,
          ExtensionId = str,
          Status = extensionLicenseStatus,
          Source = licensingSource,
          AssignmentSource = AssignmentSource.Unknown,
          AssignmentDate = (DateTimeOffset) dateTime,
          CollectionId = guid2
        };
      }
    }

    internal class UserExtensionLicenseRowBinderV2 : ExtensionBinders.UserExtensionLicenseRowBinder
    {
      protected SqlColumnBinder m_assignmentSourceColumn = new SqlColumnBinder("AssignmentSource");

      internal UserExtensionLicenseRowBinderV2()
      {
      }

      internal UserExtensionLicenseRowBinderV2(Guid accountId)
        : base(accountId)
      {
      }

      protected override UserExtensionLicense Bind()
      {
        UserExtensionLicense extensionLicense = base.Bind();
        extensionLicense.AssignmentSource = (AssignmentSource) this.m_assignmentSourceColumn.GetByte((IDataReader) this.Reader, (byte) 1);
        return extensionLicense;
      }
    }

    internal class UserExtensionLicenseRowBinderV3 : ExtensionBinders.UserExtensionLicenseRowBinderV2
    {
      protected SqlColumnBinder m_lastUpdatedColumn = new SqlColumnBinder("LastUpdated");

      internal UserExtensionLicenseRowBinderV3()
      {
      }

      internal UserExtensionLicenseRowBinderV3(Guid accountId)
        : base(accountId)
      {
      }

      protected override UserExtensionLicense Bind()
      {
        UserExtensionLicense extensionLicense = base.Bind();
        extensionLicense.LastUpdated = this.m_lastUpdatedColumn.GetNullableDateTime((IDataReader) this.Reader);
        return extensionLicense;
      }
    }

    internal class AccountExtensionsAssignedRowBinder : ObjectBinder<AccountExtensionCount>
    {
      private SqlColumnBinder m_extensionIdColumn = new SqlColumnBinder("ExtensionId");
      private SqlColumnBinder m_source = new SqlColumnBinder("Source");
      private SqlColumnBinder m_assignedColumn = new SqlColumnBinder("Assigned");

      protected override AccountExtensionCount Bind() => this.GetAccountExtensionCountFromReader((DbDataReader) this.Reader);

      internal AccountExtensionCount GetAccountExtensionCountFromReader(DbDataReader reader)
      {
        string str = this.m_extensionIdColumn.GetString((IDataReader) reader, false);
        int int32 = this.m_assignedColumn.GetInt32((IDataReader) reader);
        byte num = this.m_source.GetByte((IDataReader) reader, (byte) 1, (byte) 1);
        return new AccountExtensionCount()
        {
          ExtensionId = str,
          Assigned = int32,
          Source = (LicensingSource) num
        };
      }
    }

    internal class UserExtensionsRowBinder : ObjectBinder<KeyValuePair<Guid, ExtensionSource>>
    {
      private SqlColumnBinder m_userIdColumn = new SqlColumnBinder("UserId");
      private SqlColumnBinder m_extensionIdColumn = new SqlColumnBinder("ExtensionId");
      private SqlColumnBinder m_source = new SqlColumnBinder("Source");

      internal UserExtensionsRowBinder()
      {
      }

      protected override KeyValuePair<Guid, ExtensionSource> Bind() => this.GetUserExtensionFromReader((DbDataReader) this.Reader);

      internal KeyValuePair<Guid, ExtensionSource> GetUserExtensionFromReader(DbDataReader reader)
      {
        Guid guid = this.m_userIdColumn.GetGuid((IDataReader) reader);
        string str = this.m_extensionIdColumn.GetString((IDataReader) reader, true);
        byte num = this.m_source.GetByte((IDataReader) reader, (byte) 1, (byte) 1);
        return new KeyValuePair<Guid, ExtensionSource>(guid, new ExtensionSource()
        {
          ExtensionGalleryId = str,
          LicensingSource = (LicensingSource) num,
          AssignmentSource = AssignmentSource.Unknown
        });
      }
    }

    internal class UserExtensionsRowBinderV2 : ExtensionBinders.UserExtensionsRowBinder
    {
      protected SqlColumnBinder m_assignmentSourceColumn = new SqlColumnBinder("AssignmentSource");

      protected override KeyValuePair<Guid, ExtensionSource> Bind()
      {
        KeyValuePair<Guid, ExtensionSource> keyValuePair = base.Bind();
        keyValuePair.Value.AssignmentSource = (AssignmentSource) this.m_assignmentSourceColumn.GetByte((IDataReader) this.Reader, (byte) 1);
        return keyValuePair;
      }
    }

    internal class UserExtensionLicenseBinder : ObjectBinder<UserExtensionLicense>
    {
      private SqlColumnBinder m_internalScopeId = new SqlColumnBinder("InternalScopeId");
      private SqlColumnBinder m_userId = new SqlColumnBinder("UserId");
      private SqlColumnBinder m_extensionId = new SqlColumnBinder("ExtensionId");
      private SqlColumnBinder m_source = new SqlColumnBinder("Source");
      private SqlColumnBinder m_status = new SqlColumnBinder("Status");
      private SqlColumnBinder m_collectionId = new SqlColumnBinder("CollectionId");
      private SqlColumnBinder m_assignmentDate = new SqlColumnBinder("AssignmentDate");

      internal UserExtensionLicenseBinder()
      {
      }

      protected override UserExtensionLicense Bind() => new UserExtensionLicense()
      {
        InternalScopeId = this.m_internalScopeId.GetInt32((IDataReader) this.Reader),
        UserId = this.m_userId.GetGuid((IDataReader) this.Reader),
        ExtensionId = this.m_extensionId.GetString((IDataReader) this.Reader, false),
        Source = (LicensingSource) this.m_source.GetByte((IDataReader) this.Reader),
        Status = (UserExtensionLicenseStatus) this.m_status.GetByte((IDataReader) this.Reader),
        CollectionId = this.m_collectionId.GetGuid((IDataReader) this.Reader),
        AssignmentDate = (DateTimeOffset) this.m_assignmentDate.GetDateTime((IDataReader) this.Reader)
      };
    }
  }
}
