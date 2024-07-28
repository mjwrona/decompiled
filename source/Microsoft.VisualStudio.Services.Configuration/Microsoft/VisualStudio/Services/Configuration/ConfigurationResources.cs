// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.ConfigurationResources
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Configuration
{
  internal static class ConfigurationResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ConfigurationResources), typeof (ConfigurationResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ConfigurationResources.s_resMgr;

    private static string Get(string resourceName) => ConfigurationResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ConfigurationResources.Get(resourceName) : ConfigurationResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ConfigurationResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ConfigurationResources.GetInt(resourceName) : (int) ConfigurationResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ConfigurationResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ConfigurationResources.GetBool(resourceName) : (bool) ConfigurationResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ConfigurationResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ConfigurationResources.Get(resourceName, culture);
      if (args == null)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }

    public static string MakeCertFailed(object arg0) => ConfigurationResources.Format(nameof (MakeCertFailed), arg0);

    public static string MakeCertFailed(object arg0, CultureInfo culture) => ConfigurationResources.Format(nameof (MakeCertFailed), culture, arg0);

    public static string MakeCertFailedNoCert() => ConfigurationResources.Get(nameof (MakeCertFailedNoCert));

    public static string MakeCertFailedNoCert(CultureInfo culture) => ConfigurationResources.Get(nameof (MakeCertFailedNoCert), culture);

    public static string ProcessStarting(object arg0, object arg1) => ConfigurationResources.Format(nameof (ProcessStarting), arg0, arg1);

    public static string ProcessStarting(object arg0, object arg1, CultureInfo culture) => ConfigurationResources.Format(nameof (ProcessStarting), culture, arg0, arg1);

    public static string ProcessFinished(object arg0, object arg1, object arg2, object arg3) => ConfigurationResources.Format(nameof (ProcessFinished), arg0, arg1, arg2, arg3);

    public static string ProcessFinished(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ConfigurationResources.Format(nameof (ProcessFinished), culture, arg0, arg1, arg2, arg3);
    }

    public static string ResetIISFileNotFound() => ConfigurationResources.Get(nameof (ResetIISFileNotFound));

    public static string ResetIISFileNotFound(CultureInfo culture) => ConfigurationResources.Get(nameof (ResetIISFileNotFound), culture);

    public static string StartIIS() => ConfigurationResources.Get(nameof (StartIIS));

    public static string StartIIS(CultureInfo culture) => ConfigurationResources.Get(nameof (StartIIS), culture);

    public static string StopIIS() => ConfigurationResources.Get(nameof (StopIIS));

    public static string StopIIS(CultureInfo culture) => ConfigurationResources.Get(nameof (StopIIS), culture);

    public static string ResetIIS() => ConfigurationResources.Get(nameof (ResetIIS));

    public static string ResetIIS(CultureInfo culture) => ConfigurationResources.Get(nameof (ResetIIS), culture);

    public static string FailedToResetIIS() => ConfigurationResources.Get(nameof (FailedToResetIIS));

    public static string FailedToResetIIS(CultureInfo culture) => ConfigurationResources.Get(nameof (FailedToResetIIS), culture);

    public static string InvalidIISUserGroup(object arg0) => ConfigurationResources.Format(nameof (InvalidIISUserGroup), arg0);

    public static string InvalidIISUserGroup(object arg0, CultureInfo culture) => ConfigurationResources.Format(nameof (InvalidIISUserGroup), culture, arg0);

    public static string AccessDenied() => ConfigurationResources.Get(nameof (AccessDenied));

    public static string AccessDenied(CultureInfo culture) => ConfigurationResources.Get(nameof (AccessDenied), culture);

    public static string GroupNotExist() => ConfigurationResources.Get(nameof (GroupNotExist));

    public static string GroupNotExist(CultureInfo culture) => ConfigurationResources.Get(nameof (GroupNotExist), culture);

    public static string GroupAlreadyExists(object arg0) => ConfigurationResources.Format(nameof (GroupAlreadyExists), arg0);

    public static string GroupAlreadyExists(object arg0, CultureInfo culture) => ConfigurationResources.Format(nameof (GroupAlreadyExists), culture, arg0);

    public static string ErrorOperationWithReturnCode(object arg0, object arg1) => ConfigurationResources.Format(nameof (ErrorOperationWithReturnCode), arg0, arg1);

    public static string ErrorOperationWithReturnCode(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ConfigurationResources.Format(nameof (ErrorOperationWithReturnCode), culture, arg0, arg1);
    }

    public static string InvalidGroupName() => ConfigurationResources.Get(nameof (InvalidGroupName));

    public static string InvalidGroupName(CultureInfo culture) => ConfigurationResources.Get(nameof (InvalidGroupName), culture);

    public static string TfsBuiltInGroupsComment() => ConfigurationResources.Get(nameof (TfsBuiltInGroupsComment));

    public static string TfsBuiltInGroupsComment(CultureInfo culture) => ConfigurationResources.Get(nameof (TfsBuiltInGroupsComment), culture);

    public static string MemberNotExist() => ConfigurationResources.Get(nameof (MemberNotExist));

    public static string MemberNotExist(CultureInfo culture) => ConfigurationResources.Get(nameof (MemberNotExist), culture);

    public static string MemberAlreadyExistsInGroup() => ConfigurationResources.Get(nameof (MemberAlreadyExistsInGroup));

    public static string MemberAlreadyExistsInGroup(CultureInfo culture) => ConfigurationResources.Get(nameof (MemberAlreadyExistsInGroup), culture);

    public static string MemberNotInGroup(object arg0, object arg1) => ConfigurationResources.Format(nameof (MemberNotInGroup), arg0, arg1);

    public static string MemberNotInGroup(object arg0, object arg1, CultureInfo culture) => ConfigurationResources.Format(nameof (MemberNotInGroup), culture, arg0, arg1);

    public static string MissingPasswordForAccount(object arg0) => ConfigurationResources.Format(nameof (MissingPasswordForAccount), arg0);

    public static string MissingPasswordForAccount(object arg0, CultureInfo culture) => ConfigurationResources.Format(nameof (MissingPasswordForAccount), culture, arg0);

    public static string ServicingOperationNotValidDuringAppServicing(object arg0, object arg1) => ConfigurationResources.Format(nameof (ServicingOperationNotValidDuringAppServicing), arg0, arg1);

    public static string ServicingOperationNotValidDuringAppServicing(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ConfigurationResources.Format(nameof (ServicingOperationNotValidDuringAppServicing), culture, arg0, arg1);
    }

    public static string MediaIncorrectlyFormed(object arg0) => ConfigurationResources.Format(nameof (MediaIncorrectlyFormed), arg0);

    public static string MediaIncorrectlyFormed(object arg0, CultureInfo culture) => ConfigurationResources.Format(nameof (MediaIncorrectlyFormed), culture, arg0);

    public static string ServicingAssemblyRequired() => ConfigurationResources.Get(nameof (ServicingAssemblyRequired));

    public static string ServicingAssemblyRequired(CultureInfo culture) => ConfigurationResources.Get(nameof (ServicingAssemblyRequired), culture);

    public static string StartService(object arg0) => ConfigurationResources.Format(nameof (StartService), arg0);

    public static string StartService(object arg0, CultureInfo culture) => ConfigurationResources.Format(nameof (StartService), culture, arg0);

    public static string ServiceCantBeStopped(object arg0, object arg1) => ConfigurationResources.Format(nameof (ServiceCantBeStopped), arg0, arg1);

    public static string ServiceCantBeStopped(object arg0, object arg1, CultureInfo culture) => ConfigurationResources.Format(nameof (ServiceCantBeStopped), culture, arg0, arg1);

    public static string StopService(object arg0) => ConfigurationResources.Format(nameof (StopService), arg0);

    public static string StopService(object arg0, CultureInfo culture) => ConfigurationResources.Format(nameof (StopService), culture, arg0);

    public static string ServiceExecutableStillRunning(object arg0, object arg1) => ConfigurationResources.Format(nameof (ServiceExecutableStillRunning), arg0, arg1);

    public static string ServiceExecutableStillRunning(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ConfigurationResources.Format(nameof (ServiceExecutableStillRunning), culture, arg0, arg1);
    }

    public static string SkippingUninstallOfService(object arg0) => ConfigurationResources.Format(nameof (SkippingUninstallOfService), arg0);

    public static string SkippingUninstallOfService(object arg0, CultureInfo culture) => ConfigurationResources.Format(nameof (SkippingUninstallOfService), culture, arg0);

    public static string AttemptingToRemoveRecoveryActions(object arg0) => ConfigurationResources.Format(nameof (AttemptingToRemoveRecoveryActions), arg0);

    public static string AttemptingToRemoveRecoveryActions(object arg0, CultureInfo culture) => ConfigurationResources.Format(nameof (AttemptingToRemoveRecoveryActions), culture, arg0);

    public static string ProblemRemovingRecoveryActions(object arg0, object arg1) => ConfigurationResources.Format(nameof (ProblemRemovingRecoveryActions), arg0, arg1);

    public static string ProblemRemovingRecoveryActions(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ConfigurationResources.Format(nameof (ProblemRemovingRecoveryActions), culture, arg0, arg1);
    }

    public static string AttemptingToStopService(object arg0) => ConfigurationResources.Format(nameof (AttemptingToStopService), arg0);

    public static string AttemptingToStopService(object arg0, CultureInfo culture) => ConfigurationResources.Format(nameof (AttemptingToStopService), culture, arg0);

    public static string ProblemStoppingService(object arg0, object arg1) => ConfigurationResources.Format(nameof (ProblemStoppingService), arg0, arg1);

    public static string ProblemStoppingService(object arg0, object arg1, CultureInfo culture) => ConfigurationResources.Format(nameof (ProblemStoppingService), culture, arg0, arg1);

    public static string UninstallingService(object arg0) => ConfigurationResources.Format(nameof (UninstallingService), arg0);

    public static string UninstallingService(object arg0, CultureInfo culture) => ConfigurationResources.Format(nameof (UninstallingService), culture, arg0);

    public static string ServiceUninstallFailed(object arg0) => ConfigurationResources.Format(nameof (ServiceUninstallFailed), arg0);

    public static string ServiceUninstallFailed(object arg0, CultureInfo culture) => ConfigurationResources.Format(nameof (ServiceUninstallFailed), culture, arg0);

    public static string ServiceStillDefined(object arg0, object arg1, object arg2) => ConfigurationResources.Format(nameof (ServiceStillDefined), arg0, arg1, arg2);

    public static string ServiceStillDefined(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ConfigurationResources.Format(nameof (ServiceStillDefined), culture, arg0, arg1, arg2);
    }

    public static string FrameworkFileNotFound(object arg0, object arg1) => ConfigurationResources.Format(nameof (FrameworkFileNotFound), arg0, arg1);

    public static string FrameworkFileNotFound(object arg0, object arg1, CultureInfo culture) => ConfigurationResources.Format(nameof (FrameworkFileNotFound), culture, arg0, arg1);

    public static string FrameworkNotFoundByFilter(object arg0) => ConfigurationResources.Format(nameof (FrameworkNotFoundByFilter), arg0);

    public static string FrameworkNotFoundByFilter(object arg0, CultureInfo culture) => ConfigurationResources.Format(nameof (FrameworkNotFoundByFilter), culture, arg0);

    public static string FileAlreadyExists(object arg0) => ConfigurationResources.Format(nameof (FileAlreadyExists), arg0);

    public static string FileAlreadyExists(object arg0, CultureInfo culture) => ConfigurationResources.Format(nameof (FileAlreadyExists), culture, arg0);

    public static string FileDownloadedSignatureVerificationFailed() => ConfigurationResources.Get(nameof (FileDownloadedSignatureVerificationFailed));

    public static string FileDownloadedSignatureVerificationFailed(CultureInfo culture) => ConfigurationResources.Get(nameof (FileDownloadedSignatureVerificationFailed), culture);

    public static string FailedToDownloadSqlExpressInstaller(object arg0, object arg1) => ConfigurationResources.Format(nameof (FailedToDownloadSqlExpressInstaller), arg0, arg1);

    public static string FailedToDownloadSqlExpressInstaller(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ConfigurationResources.Format(nameof (FailedToDownloadSqlExpressInstaller), culture, arg0, arg1);
    }

    public static string FailedToDownloadFile(object arg0) => ConfigurationResources.Format(nameof (FailedToDownloadFile), arg0);

    public static string FailedToDownloadFile(object arg0, CultureInfo culture) => ConfigurationResources.Format(nameof (FailedToDownloadFile), culture, arg0);

    public static string SslServerCertificateBindingIsAlreadyInUse(object arg0, object arg1) => ConfigurationResources.Format(nameof (SslServerCertificateBindingIsAlreadyInUse), arg0, arg1);

    public static string SslServerCertificateBindingIsAlreadyInUse(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ConfigurationResources.Format(nameof (SslServerCertificateBindingIsAlreadyInUse), culture, arg0, arg1);
    }

    public static string FailedToCreateSslServerCertificateBinding(object arg0, object arg1) => ConfigurationResources.Format(nameof (FailedToCreateSslServerCertificateBinding), arg0, arg1);

    public static string FailedToCreateSslServerCertificateBinding(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ConfigurationResources.Format(nameof (FailedToCreateSslServerCertificateBinding), culture, arg0, arg1);
    }

    public static string FailedToDeleteSslServerCertificateBinding(object arg0, object arg1) => ConfigurationResources.Format(nameof (FailedToDeleteSslServerCertificateBinding), arg0, arg1);

    public static string FailedToDeleteSslServerCertificateBinding(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ConfigurationResources.Format(nameof (FailedToDeleteSslServerCertificateBinding), culture, arg0, arg1);
    }

    public static string FailedToDeleteSslSniServerCertificateBinding(object arg0, object arg1) => ConfigurationResources.Format(nameof (FailedToDeleteSslSniServerCertificateBinding), arg0, arg1);

    public static string FailedToDeleteSslSniServerCertificateBinding(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ConfigurationResources.Format(nameof (FailedToDeleteSslSniServerCertificateBinding), culture, arg0, arg1);
    }

    public static string SslSniServerCertificateBindingAlreadyInUse(object arg0, object arg1) => ConfigurationResources.Format(nameof (SslSniServerCertificateBindingAlreadyInUse), arg0, arg1);

    public static string SslSniServerCertificateBindingAlreadyInUse(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ConfigurationResources.Format(nameof (SslSniServerCertificateBindingAlreadyInUse), culture, arg0, arg1);
    }

    public static string FailedToCreateSslSniServerCertificateBinding(object arg0, object arg1) => ConfigurationResources.Format(nameof (FailedToCreateSslSniServerCertificateBinding), arg0, arg1);

    public static string FailedToCreateSslSniServerCertificateBinding(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ConfigurationResources.Format(nameof (FailedToCreateSslSniServerCertificateBinding), culture, arg0, arg1);
    }

    public static string ProcessStarted(object arg0, object arg1, object arg2) => ConfigurationResources.Format(nameof (ProcessStarted), arg0, arg1, arg2);

    public static string ProcessStarted(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ConfigurationResources.Format(nameof (ProcessStarted), culture, arg0, arg1, arg2);
    }
  }
}
