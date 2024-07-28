// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Application.CustomSqlError
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

namespace Microsoft.VisualStudio.Services.Licensing.Application
{
  internal static class CustomSqlError
  {
    public const int GenericWrapperCode = 50000;
    public const int GetUserLicenses = 1060101;
    public const int UpsertUserLicense = 1060102;
    public const int UpsertUserStatus = 1060103;
    public const int DeleteUserLicense = 1060104;
    public const int UserLicenseNotFound = 1060105;
    public const int GetUserLicensesAcrossPartitions = 1060106;
    public const int InsertLicensingEvent = 1060107;
    public const int UpdateUserLastAccessed = 1060108;
    public const int CreateLicenseScopeFailed = 1060109;
    public const int LicenseScopeNotFound = 1060110;
    public const int UpsertPreviousUserLicense = 1060111;
    public const int UpsertUserExtensionLicense = 1060201;
    public const int UserExtensionLicenseNotFound = 1060202;
    public const int UserExtensionLicenseUpdateException = 1060203;
    public const int UserExtensionLicenseExists = 1060204;
    public const int UpdateExtensionsAssignedToUser = 1060205;
    public const int UserExtensionLicenseCopyException = 1060206;
    public const int GetUserExtensionLicenses = 1060207;
    public const int DeleteUserLicenseByCollection = 1060301;
    public const int ImportScopeFailed = 1060401;
    public const int DeleteScopeFailed = 1060402;
    public const int MAX_SQL_ERROR = 1060402;
  }
}
