// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataAccess.CommandDataFactory
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Licensing.CosmosDataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing.DataAccess
{
  internal static class CommandDataFactory
  {
    public static RemoveUserCommandData CreateRemoveUserCommandData(Guid userId)
    {
      RemoveUserCommandData removeUserCommandData = new RemoveUserCommandData();
      removeUserCommandData.Data = (object) CommandArgumentsFactory.Create(userId);
      return removeUserCommandData;
    }

    public static UpdateUserStatusCommandData CreateUpdateUserCommandData(
      Guid userId,
      AccountUserStatus status,
      AssignmentSource assignmentSource)
    {
      UpdateUserStatusCommandData updateUserCommandData = new UpdateUserStatusCommandData();
      updateUserCommandData.Data = (object) CommandArgumentsFactory.Create(userId, status, assignmentSource);
      return updateUserCommandData;
    }

    public static AddUserCommandData CreateAddUserCommandData(
      Guid userId,
      AccountUserStatus status,
      License licenseIfAbsent,
      AssignmentSource assignmentSource)
    {
      AddUserCommandData addUserCommandData = new AddUserCommandData();
      addUserCommandData.Data = (object) CommandArgumentsFactory.Create(userId, status, licenseIfAbsent, assignmentSource);
      return addUserCommandData;
    }

    public static DeleteLicenseCommandData CreateDeleteLicenseCommandData(UserLicenseCosmosItem item)
    {
      DeleteLicenseCommandData licenseCommandData = new DeleteLicenseCommandData();
      licenseCommandData.Data = (object) CommandArgumentsFactory.Create(item.UserId, item.License, item.PreviousLicense, item.ExtensionLicenses);
      return licenseCommandData;
    }

    public static SeedLicensesCommandData CreateSeedLicensesCommandData(
      IEnumerable<Tuple<AccountUser, UserLicense>> entries)
    {
      SeedLicensesCommandData licensesCommandData = new SeedLicensesCommandData();
      licensesCommandData.Data = (object) entries.Select<Tuple<AccountUser, UserLicense>, SeedLicenseCommandArguments>((Func<Tuple<AccountUser, UserLicense>, SeedLicenseCommandArguments>) (entry => CommandArgumentsFactory.Create(entry.Item1.UserId, entry.Item1.UserStatus, entry.Item2.Source, entry.Item2.License))).ToList<SeedLicenseCommandArguments>();
      return licensesCommandData;
    }

    public static SetLicenseCommandData CreateSetLicenseCommandData(
      Guid userId,
      License license,
      AccountUserStatus statusIfAbsent,
      AssignmentSource assignmentSource)
    {
      SetLicenseCommandData licenseCommandData = new SetLicenseCommandData();
      licenseCommandData.Data = (object) CommandArgumentsFactory.Create(userId, license, statusIfAbsent, assignmentSource);
      return licenseCommandData;
    }

    public static ImportLicensesCommandData CreateImportLicensesCommandData(
      IList<UserLicense> userLicenses)
    {
      ImportLicensesCommandData licensesCommandData = new ImportLicensesCommandData();
      licensesCommandData.Data = (object) userLicenses.Select<UserLicense, FullLicenseCommandArguments>((Func<UserLicense, FullLicenseCommandArguments>) (entry => CommandArgumentsFactory.Create(entry.AccountId, entry.UserId, entry.Status, entry.Source, entry.License))).ToList<FullLicenseCommandArguments>();
      return licensesCommandData;
    }

    public static DeleteLicensesCommandData CreateDeleteLicensesCommandData(
      IList<UserLicense> userLicenses)
    {
      DeleteLicensesCommandData licensesCommandData = new DeleteLicensesCommandData();
      licensesCommandData.Data = (object) userLicenses.Select<UserLicense, SeedLicenseCommandArguments>((Func<UserLicense, SeedLicenseCommandArguments>) (entry => CommandArgumentsFactory.Create(entry.UserId, entry.Status, entry.Source, entry.License))).ToList<SeedLicenseCommandArguments>();
      return licensesCommandData;
    }

    public static TransferUserExtensionLicensesData CreateTransferUserExtensionLicensesCommandData(
      Guid oldUserId,
      Guid newUserId,
      string extensions)
    {
      TransferUserExtensionLicensesData licensesCommandData = new TransferUserExtensionLicensesData();
      licensesCommandData.Data = (object) CommandArgumentsFactory.Create(oldUserId, newUserId, extensions);
      return licensesCommandData;
    }

    public static UpsertLicenseCommandData CreateUpsertLicenseCommandData(UserLicenseCosmosItem item)
    {
      UpsertLicenseCommandData licenseCommandData = new UpsertLicenseCommandData();
      licenseCommandData.Data = (object) CommandArgumentsFactory.Create(item.UserId, item.License, item.PreviousLicense, item.ExtensionLicenses);
      return licenseCommandData;
    }

    public static UpdateLicenseCommandData CreateUpdateLicenseCommandData(UserLicenseCosmosItem item)
    {
      UpdateLicenseCommandData licenseCommandData = new UpdateLicenseCommandData();
      licenseCommandData.Data = (object) CommandArgumentsFactory.Create(item.UserId, item.License, item.PreviousLicense, item.ExtensionLicenses);
      return licenseCommandData;
    }
  }
}
