// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataAccess.CommandArgumentsFactory
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.VisualStudio.Services.Account;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing.DataAccess
{
  internal static class CommandArgumentsFactory
  {
    public static UserLicensingCommandArguments Create(Guid userId) => new UserLicensingCommandArguments()
    {
      UserId = userId
    };

    public static SetUserStatusCommandArguments Create(
      Guid userId,
      AccountUserStatus status,
      AssignmentSource assignmentSource)
    {
      SetUserStatusCommandArguments commandArguments = new SetUserStatusCommandArguments();
      commandArguments.UserId = userId;
      commandArguments.Status = status;
      commandArguments.AssignmentSource = assignmentSource;
      return commandArguments;
    }

    public static AddUserCommandArguments Create(
      Guid userId,
      AccountUserStatus status,
      License licenseIfAbsent,
      AssignmentSource assignmentSource)
    {
      AddUserCommandArguments commandArguments = new AddUserCommandArguments();
      commandArguments.UserId = userId;
      commandArguments.Status = status;
      commandArguments.LicenseIfAbsent = licenseIfAbsent;
      commandArguments.AssignmentSource = assignmentSource;
      return commandArguments;
    }

    public static SetLicenseCommandArguments Create(
      Guid userId,
      License license,
      AccountUserStatus statusIfAbsent,
      AssignmentSource assignmentSource)
    {
      SetLicenseCommandArguments commandArguments = new SetLicenseCommandArguments();
      commandArguments.UserId = userId;
      commandArguments.License = license;
      commandArguments.StatusIfAbsent = statusIfAbsent;
      commandArguments.AssignmentSource = assignmentSource;
      return commandArguments;
    }

    public static SeedLicenseCommandArguments Create(
      Guid userId,
      AccountUserStatus status,
      LicensingSource source,
      int license)
    {
      SeedLicenseCommandArguments commandArguments = new SeedLicenseCommandArguments();
      commandArguments.UserId = userId;
      commandArguments.Source = source;
      commandArguments.License = license;
      commandArguments.Status = status;
      return commandArguments;
    }

    public static FullLicenseCommandArguments Create(
      Guid accountId,
      Guid userId,
      AccountUserStatus status,
      LicensingSource source,
      int license)
    {
      return new FullLicenseCommandArguments()
      {
        AccountId = accountId,
        UserId = userId,
        Source = source,
        License = license,
        Status = status
      };
    }

    public static TransferUserExtensionLicensesCommandArguments Create(
      Guid oldUserId,
      Guid newUserId,
      string extensions)
    {
      return new TransferUserExtensionLicensesCommandArguments()
      {
        OldUserId = oldUserId,
        NewUserId = newUserId,
        Extensions = extensions
      };
    }

    public static LicenseDocumentCommandArguments Create(
      Guid userId,
      UserLicense license,
      UserLicense previousLicense,
      List<UserExtensionLicense> extensionLicenses)
    {
      return new LicenseDocumentCommandArguments()
      {
        UserId = userId,
        License = license,
        PreviousLicense = previousLicense,
        ExtensionLicenses = extensionLicenses
      };
    }
  }
}
