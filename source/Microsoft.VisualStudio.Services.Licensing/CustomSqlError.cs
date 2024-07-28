// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.CustomSqlError
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal static class CustomSqlError
  {
    public const int GenericWrapperCode = 50000;
    public const int InvalidMachineId = 1060001;
    public const int GetLicensingVSTrialExpirationsFail = 1060002;
    public const int GetLicensingVSTrialUserExpirationFail = 1060003;
    public const int GetLicensingVSTrialMachineExpirationFail = 1060004;
    public const int GetLicensingAccountEntitlementsForAccount = 1060005;
    public const int GetLicensingAccountEntitlementsForUser = 1060006;
    public const int AssignLicensingAccountEntitlement = 1060007;
    public const int DeleteLicensingAccountEntitlement = 1060008;
    public const int UpsertExtensionLicenseRegistration = 1060009;
    public const int MAX_SQL_ERROR = 1060009;
  }
}
