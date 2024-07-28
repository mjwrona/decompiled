// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataAccess.Binders
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Account;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Licensing.DataAccess
{
  internal static class Binders
  {
    internal class PreviousUserLicenseRowBinder : ObjectBinder<UserLicense>
    {
      protected SqlColumnBinder m_scopeId = new SqlColumnBinder("ScopeId");
      protected SqlColumnBinder m_userIdColumn = new SqlColumnBinder("UserId");
      protected SqlColumnBinder m_statusColumn = new SqlColumnBinder("Status");
      protected SqlColumnBinder m_sourceColumn = new SqlColumnBinder("Source");
      protected SqlColumnBinder m_licenseColumn = new SqlColumnBinder("License");
      protected Guid m_accountId;

      internal PreviousUserLicenseRowBinder()
      {
      }

      protected override UserLicense Bind()
      {
        Guid guid1 = this.m_scopeId.GetGuid((IDataReader) this.Reader);
        Guid guid2 = this.m_userIdColumn.GetGuid((IDataReader) this.Reader);
        AccountUserStatus accountUserStatus = (AccountUserStatus) this.m_statusColumn.GetByte((IDataReader) this.Reader);
        LicensingSource licensingSource = (LicensingSource) this.m_sourceColumn.GetByte((IDataReader) this.Reader);
        byte num = this.m_licenseColumn.GetByte((IDataReader) this.Reader);
        return new UserLicense()
        {
          AccountId = guid1,
          UserId = guid2,
          Status = accountUserStatus,
          Source = licensingSource,
          License = (int) num,
          AssignmentSource = AssignmentSource.Unknown,
          AssignmentDate = (DateTimeOffset) DateTime.Now,
          DateCreated = (DateTimeOffset) DateTime.Now,
          LastUpdated = (DateTimeOffset) DateTime.Now
        };
      }
    }

    internal class UserLicenseRowBinder : ObjectBinder<UserLicense>
    {
      protected SqlColumnBinder m_userIdColumn = new SqlColumnBinder("UserId");
      protected SqlColumnBinder m_statusColumn = new SqlColumnBinder("Status");
      protected SqlColumnBinder m_sourceColumn = new SqlColumnBinder("Source");
      protected SqlColumnBinder m_licenseColumn = new SqlColumnBinder("License");
      protected SqlColumnBinder m_assignmentDateColumn = new SqlColumnBinder("AssignmentDate");
      protected SqlColumnBinder m_dateCreatedColumn = new SqlColumnBinder("DateCreated");
      protected SqlColumnBinder m_lastUpdatedColumn = new SqlColumnBinder("LastUpdated");
      protected Guid m_accountId;

      protected UserLicenseRowBinder()
      {
      }

      internal UserLicenseRowBinder(Guid accountId)
        : this()
      {
        this.m_accountId = accountId;
      }

      protected override UserLicense Bind()
      {
        Guid guid = this.m_userIdColumn.GetGuid((IDataReader) this.Reader);
        AccountUserStatus accountUserStatus = (AccountUserStatus) this.m_statusColumn.GetByte((IDataReader) this.Reader);
        LicensingSource licensingSource = (LicensingSource) this.m_sourceColumn.GetByte((IDataReader) this.Reader);
        byte num = this.m_licenseColumn.GetByte((IDataReader) this.Reader);
        DateTime dateTime1 = this.m_assignmentDateColumn.GetDateTime((IDataReader) this.Reader);
        DateTime dateTime2 = this.m_dateCreatedColumn.GetDateTime((IDataReader) this.Reader);
        DateTime dateTime3 = this.m_lastUpdatedColumn.GetDateTime((IDataReader) this.Reader);
        return new UserLicense()
        {
          AccountId = this.m_accountId,
          UserId = guid,
          Status = accountUserStatus,
          Source = licensingSource,
          License = (int) num,
          AssignmentDate = (DateTimeOffset) dateTime1,
          DateCreated = (DateTimeOffset) dateTime2,
          LastUpdated = (DateTimeOffset) dateTime3
        };
      }
    }

    internal class UserLicenseRowBinderV2 : Binders.UserLicenseRowBinder
    {
      protected SqlColumnBinder m_lastaccessedColumn = new SqlColumnBinder("LastAccessed");

      protected UserLicenseRowBinderV2()
      {
      }

      internal UserLicenseRowBinderV2(Guid accountId)
        : base(accountId)
      {
      }

      protected override UserLicense Bind()
      {
        UserLicense userLicense = base.Bind();
        userLicense.LastAccessed = (DateTimeOffset) this.m_lastaccessedColumn.GetDateTime((IDataReader) this.Reader).ToUniversalTime();
        return userLicense;
      }
    }

    internal class UserLicenseRowBinderV3 : Binders.UserLicenseRowBinderV2
    {
      protected SqlColumnBinder m_scopeId = new SqlColumnBinder("ScopeId");

      protected override UserLicense Bind()
      {
        UserLicense userLicense = base.Bind();
        userLicense.AccountId = this.m_scopeId.GetGuid((IDataReader) this.Reader);
        return userLicense;
      }
    }

    internal class UserLicenseRowBinderV4 : Binders.UserLicenseRowBinderV3
    {
      protected SqlColumnBinder m_assignmentSourceColumn = new SqlColumnBinder("AssignmentSource");

      protected override UserLicense Bind()
      {
        UserLicense userLicense = base.Bind();
        userLicense.AssignmentSource = (AssignmentSource) this.m_assignmentSourceColumn.GetByte((IDataReader) this.Reader, (byte) 1);
        return userLicense;
      }
    }

    internal class UserLicenseRowBinderV5 : Binders.UserLicenseRowBinderV4
    {
      protected SqlColumnBinder m_originColumn = new SqlColumnBinder("Origin");

      protected override UserLicense Bind()
      {
        UserLicense userLicense = base.Bind();
        userLicense.Origin = (LicensingOrigin) this.m_originColumn.GetByte((IDataReader) this.Reader, (byte) 0);
        return userLicense;
      }
    }

    internal class LicenseEventRowBinder : ObjectBinder<LicenseEventRow>
    {
      private SqlColumnBinder m_eventIdColumn = new SqlColumnBinder("EventId");
      private SqlColumnBinder m_eventTypeFamilyColumn = new SqlColumnBinder("EventTypeFamily");
      private SqlColumnBinder m_eventTypeDescriptorColumn = new SqlColumnBinder("EventTypeDescriptor");
      private SqlColumnBinder m_eventDataColumn = new SqlColumnBinder("EventData");
      private SqlColumnBinder m_timeCreatedColumn = new SqlColumnBinder("TimeCreated");

      protected override LicenseEventRow Bind()
      {
        int int32 = this.m_eventIdColumn.GetInt32((IDataReader) this.Reader);
        string str1 = this.m_eventTypeFamilyColumn.GetString((IDataReader) this.Reader, false);
        string str2 = this.m_eventTypeDescriptorColumn.GetString((IDataReader) this.Reader, true);
        string str3 = this.m_eventDataColumn.GetString((IDataReader) this.Reader, true);
        DateTime dateTime = this.m_timeCreatedColumn.GetDateTime((IDataReader) this.Reader);
        return new LicenseEventRow()
        {
          EventId = int32,
          EventTypeFamily = str1,
          EventTypeDescriptor = str2,
          EventData = str3,
          TimeCreated = dateTime
        };
      }
    }

    internal class AccountLicenseCountBinder : ObjectBinder<AccountLicenseCount>
    {
      private SqlColumnBinder m_sourceColumn = new SqlColumnBinder("Source");
      private SqlColumnBinder m_licenseColumn = new SqlColumnBinder("License");
      private SqlColumnBinder m_countColumn = new SqlColumnBinder("Count");

      protected override AccountLicenseCount Bind()
      {
        LicensingSource source = (LicensingSource) this.m_sourceColumn.GetByte((IDataReader) this.Reader);
        byte license = this.m_licenseColumn.GetByte((IDataReader) this.Reader);
        int int32 = this.m_countColumn.GetInt32((IDataReader) this.Reader);
        return new AccountLicenseCount()
        {
          License = new AccountUserLicense(source, (int) license),
          Count = int32
        };
      }
    }

    internal class UserLicenseCountBinder : ObjectBinder<UserLicenseCount>
    {
      private SqlColumnBinder m_sourceColumn = new SqlColumnBinder("Source");
      private SqlColumnBinder m_licenseColumn = new SqlColumnBinder("License");
      private SqlColumnBinder m_statusColumn = new SqlColumnBinder("Status");
      private SqlColumnBinder m_countColumn = new SqlColumnBinder("Count");

      protected override UserLicenseCount Bind()
      {
        LicensingSource source = (LicensingSource) this.m_sourceColumn.GetByte((IDataReader) this.Reader);
        byte license = this.m_licenseColumn.GetByte((IDataReader) this.Reader);
        AccountUserStatus accountUserStatus = (AccountUserStatus) this.m_statusColumn.GetByte((IDataReader) this.Reader);
        int int32 = this.m_countColumn.GetInt32((IDataReader) this.Reader);
        UserLicenseCount userLicenseCount = new UserLicenseCount();
        userLicenseCount.License = new AccountUserLicense(source, (int) license);
        userLicenseCount.UserStatus = accountUserStatus;
        userLicenseCount.Count = int32;
        return userLicenseCount;
      }
    }

    internal class AccountUserRowBinder : ObjectBinder<AccountUser>
    {
      protected SqlColumnBinder m_userIdColumn = new SqlColumnBinder("UserId");
      protected SqlColumnBinder m_statusColumn = new SqlColumnBinder("Status");
      protected SqlColumnBinder m_lastUpdatedColumn = new SqlColumnBinder("LastUpdated");
      protected Guid m_accountId;

      protected AccountUserRowBinder()
      {
      }

      internal AccountUserRowBinder(Guid accountId)
        : this()
      {
        this.m_accountId = accountId;
      }

      protected override AccountUser Bind()
      {
        Guid guid = this.m_userIdColumn.GetGuid((IDataReader) this.Reader);
        AccountUserStatus accountUserStatus = (AccountUserStatus) this.m_statusColumn.GetByte((IDataReader) this.Reader);
        DateTime dateTime = this.m_lastUpdatedColumn.GetDateTime((IDataReader) this.Reader);
        return new AccountUser(this.m_accountId, guid)
        {
          UserStatus = accountUserStatus,
          LastUpdated = (DateTimeOffset) dateTime
        };
      }
    }

    internal class LicensingScope : ObjectBinder<Guid>
    {
      protected SqlColumnBinder m_scopeId = new SqlColumnBinder("ScopeId");

      protected override Guid Bind() => this.m_scopeId.GetGuid((IDataReader) this.Reader);
    }

    internal class MigratedUser : ObjectBinder<Guid>
    {
      protected SqlColumnBinder m_count = new SqlColumnBinder("UserId");

      protected override Guid Bind() => this.m_count.GetGuid((IDataReader) this.Reader);
    }
  }
}
