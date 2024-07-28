// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Internal.ClientResources
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Client.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ClientResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (ClientResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ClientResources.s_resMgr;

    private static string Get(string resourceName) => ClientResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ClientResources.Get(resourceName) : ClientResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ClientResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ClientResources.GetInt(resourceName) : (int) ClientResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ClientResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ClientResources.GetBool(resourceName) : (bool) ClientResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ClientResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ClientResources.Get(resourceName, culture);
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

    public static string AnswerAll() => ClientResources.Get(nameof (AnswerAll));

    public static string AnswerAll(CultureInfo culture) => ClientResources.Get(nameof (AnswerAll), culture);

    public static string AnswerNo() => ClientResources.Get(nameof (AnswerNo));

    public static string AnswerNo(CultureInfo culture) => ClientResources.Get(nameof (AnswerNo), culture);

    public static string AnswerYes() => ClientResources.Get(nameof (AnswerYes));

    public static string AnswerYes(CultureInfo culture) => ClientResources.Get(nameof (AnswerYes), culture);

    public static string AnswerAllFull() => ClientResources.Get(nameof (AnswerAllFull));

    public static string AnswerAllFull(CultureInfo culture) => ClientResources.Get(nameof (AnswerAllFull), culture);

    public static string AnswerNoFull() => ClientResources.Get(nameof (AnswerNoFull));

    public static string AnswerNoFull(CultureInfo culture) => ClientResources.Get(nameof (AnswerNoFull), culture);

    public static string AnswerYesFull() => ClientResources.Get(nameof (AnswerYesFull));

    public static string AnswerYesFull(CultureInfo culture) => ClientResources.Get(nameof (AnswerYesFull), culture);

    public static string Yes() => ClientResources.Get(nameof (Yes));

    public static string Yes(CultureInfo culture) => ClientResources.Get(nameof (Yes), culture);

    public static string No() => ClientResources.Get(nameof (No));

    public static string No(CultureInfo culture) => ClientResources.Get(nameof (No), culture);

    public static string BasicAuthenticationRequiresSsl() => ClientResources.Get(nameof (BasicAuthenticationRequiresSsl));

    public static string BasicAuthenticationRequiresSsl(CultureInfo culture) => ClientResources.Get(nameof (BasicAuthenticationRequiresSsl), culture);

    public static string CannotGetTempFileName(object arg0) => ClientResources.Format(nameof (CannotGetTempFileName), arg0);

    public static string CannotGetTempFileName(object arg0, CultureInfo culture) => ClientResources.Format(nameof (CannotGetTempFileName), culture, arg0);

    public static string CannotModifyBuiltinService(object arg0) => ClientResources.Format(nameof (CannotModifyBuiltinService), arg0);

    public static string CannotModifyBuiltinService(object arg0, CultureInfo culture) => ClientResources.Format(nameof (CannotModifyBuiltinService), culture, arg0);

    public static string ComboTreeHelpText() => ClientResources.Get(nameof (ComboTreeHelpText));

    public static string ComboTreeHelpText(CultureInfo culture) => ClientResources.Get(nameof (ComboTreeHelpText), culture);

    public static string CommandCanceled() => ClientResources.Get(nameof (CommandCanceled));

    public static string CommandCanceled(CultureInfo culture) => ClientResources.Get(nameof (CommandCanceled), culture);

    public static string CommandFileLineTooLongWarning(object arg0) => ClientResources.Format(nameof (CommandFileLineTooLongWarning), arg0);

    public static string CommandFileLineTooLongWarning(object arg0, CultureInfo culture) => ClientResources.Format(nameof (CommandFileLineTooLongWarning), culture, arg0);

    public static string ContentsOfFileTooBig(object arg0, object arg1) => ClientResources.Format(nameof (ContentsOfFileTooBig), arg0, arg1);

    public static string ContentsOfFileTooBig(object arg0, object arg1, CultureInfo culture) => ClientResources.Format(nameof (ContentsOfFileTooBig), culture, arg0, arg1);

    public static string CouldNotParseParameter(object arg0, object arg1) => ClientResources.Format(nameof (CouldNotParseParameter), arg0, arg1);

    public static string CouldNotParseParameter(object arg0, object arg1, CultureInfo culture) => ClientResources.Format(nameof (CouldNotParseParameter), culture, arg0, arg1);

    public static string DoesNotAcceptAValue(object arg0) => ClientResources.Format(nameof (DoesNotAcceptAValue), arg0);

    public static string DoesNotAcceptAValue(object arg0, CultureInfo culture) => ClientResources.Format(nameof (DoesNotAcceptAValue), culture, arg0);

    public static string DuplicateOption(object arg0) => ClientResources.Format(nameof (DuplicateOption), arg0);

    public static string DuplicateOption(object arg0, CultureInfo culture) => ClientResources.Format(nameof (DuplicateOption), culture, arg0);

    public static string EmptyStringNotAllowed() => ClientResources.Get(nameof (EmptyStringNotAllowed));

    public static string EmptyStringNotAllowed(CultureInfo culture) => ClientResources.Get(nameof (EmptyStringNotAllowed), culture);

    public static string ErrorTitle() => ClientResources.Get(nameof (ErrorTitle));

    public static string ErrorTitle(CultureInfo culture) => ClientResources.Get(nameof (ErrorTitle), culture);

    public static string ExceptionCaption() => ClientResources.Get(nameof (ExceptionCaption));

    public static string ExceptionCaption(CultureInfo culture) => ClientResources.Get(nameof (ExceptionCaption), culture);

    public static string ExitCode(object arg0) => ClientResources.Format(nameof (ExitCode), arg0);

    public static string ExitCode(object arg0, CultureInfo culture) => ClientResources.Format(nameof (ExitCode), culture, arg0);

    public static string ServerOptionIsDeprecated(object arg0) => ClientResources.Format(nameof (ServerOptionIsDeprecated), arg0);

    public static string ServerOptionIsDeprecated(object arg0, CultureInfo culture) => ClientResources.Format(nameof (ServerOptionIsDeprecated), culture, arg0);

    public static string ExtraCommaInOption(object arg0) => ClientResources.Format(nameof (ExtraCommaInOption), arg0);

    public static string ExtraCommaInOption(object arg0, CultureInfo culture) => ClientResources.Format(nameof (ExtraCommaInOption), culture, arg0);

    public static string InvalidOptionCombination(object arg0, object arg1) => ClientResources.Format(nameof (InvalidOptionCombination), arg0, arg1);

    public static string InvalidOptionCombination(object arg0, object arg1, CultureInfo culture) => ClientResources.Format(nameof (InvalidOptionCombination), culture, arg0, arg1);

    public static string MissingAssociatedOption(object arg0, object arg1) => ClientResources.Format(nameof (MissingAssociatedOption), arg0, arg1);

    public static string MissingAssociatedOption(object arg0, object arg1, CultureInfo culture) => ClientResources.Format(nameof (MissingAssociatedOption), culture, arg0, arg1);

    public static string InvalidOptionValue(object arg0, object arg1) => ClientResources.Format(nameof (InvalidOptionValue), arg0, arg1);

    public static string InvalidOptionValue(object arg0, object arg1, CultureInfo culture) => ClientResources.Format(nameof (InvalidOptionValue), culture, arg0, arg1);

    public static string InvalidResponse() => ClientResources.Get(nameof (InvalidResponse));

    public static string InvalidResponse(CultureInfo culture) => ClientResources.Get(nameof (InvalidResponse), culture);

    public static string MissingOptionValue(object arg0) => ClientResources.Format(nameof (MissingOptionValue), arg0);

    public static string MissingOptionValue(object arg0, CultureInfo culture) => ClientResources.Format(nameof (MissingOptionValue), culture, arg0);

    public static string NoRootFolder() => ClientResources.Get(nameof (NoRootFolder));

    public static string NoRootFolder(CultureInfo culture) => ClientResources.Get(nameof (NoRootFolder), culture);

    public static string PressEnterKey() => ClientResources.Get(nameof (PressEnterKey));

    public static string PressEnterKey(CultureInfo culture) => ClientResources.Get(nameof (PressEnterKey), culture);

    public static string QuestionYNSuffix() => ClientResources.Get(nameof (QuestionYNSuffix));

    public static string QuestionYNSuffix(CultureInfo culture) => ClientResources.Get(nameof (QuestionYNSuffix), culture);

    public static string QuestionYNASuffix() => ClientResources.Get(nameof (QuestionYNASuffix));

    public static string QuestionYNASuffix(CultureInfo culture) => ClientResources.Get(nameof (QuestionYNASuffix), culture);

    public static string ServerNameNotValid(object arg0) => ClientResources.Format(nameof (ServerNameNotValid), arg0);

    public static string ServerNameNotValid(object arg0, CultureInfo culture) => ClientResources.Format(nameof (ServerNameNotValid), culture, arg0);

    public static string ServiceNotRegistered(object arg0) => ClientResources.Format(nameof (ServiceNotRegistered), arg0);

    public static string ServiceNotRegistered(object arg0, CultureInfo culture) => ClientResources.Format(nameof (ServiceNotRegistered), culture, arg0);

    public static string StandardErrorDlgTitle() => ClientResources.Get(nameof (StandardErrorDlgTitle));

    public static string StandardErrorDlgTitle(CultureInfo culture) => ClientResources.Get(nameof (StandardErrorDlgTitle), culture);

    public static string ThreadingMustBeSTA() => ClientResources.Get(nameof (ThreadingMustBeSTA));

    public static string ThreadingMustBeSTA(CultureInfo culture) => ClientResources.Get(nameof (ThreadingMustBeSTA), culture);

    public static string UICredProvider_MessageText() => ClientResources.Get(nameof (UICredProvider_MessageText));

    public static string UICredProvider_MessageText(CultureInfo culture) => ClientResources.Get(nameof (UICredProvider_MessageText), culture);

    public static string UICredProvider_TitleText(object arg0) => ClientResources.Format(nameof (UICredProvider_TitleText), arg0);

    public static string UICredProvider_TitleText(object arg0, CultureInfo culture) => ClientResources.Format(nameof (UICredProvider_TitleText), culture, arg0);

    public static string UnrecognizedOption(object arg0) => ClientResources.Format(nameof (UnrecognizedOption), arg0);

    public static string UnrecognizedOption(object arg0, CultureInfo culture) => ClientResources.Format(nameof (UnrecognizedOption), culture, arg0);

    public static string ValueLengthExceedsMaxForOption(object arg0, object arg1) => ClientResources.Format(nameof (ValueLengthExceedsMaxForOption), arg0, arg1);

    public static string ValueLengthExceedsMaxForOption(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ClientResources.Format(nameof (ValueLengthExceedsMaxForOption), culture, arg0, arg1);
    }

    public static string WrongNumberOfArgsForBuiltIn(object arg0) => ClientResources.Format(nameof (WrongNumberOfArgsForBuiltIn), arg0);

    public static string WrongNumberOfArgsForBuiltIn(object arg0, CultureInfo culture) => ClientResources.Format(nameof (WrongNumberOfArgsForBuiltIn), culture, arg0);

    public static string InformationBarImages() => ClientResources.Get(nameof (InformationBarImages));

    public static string InformationBarImages(CultureInfo culture) => ClientResources.Get(nameof (InformationBarImages), culture);

    public static string FileTypeFilter() => ClientResources.Get(nameof (FileTypeFilter));

    public static string FileTypeFilter(CultureInfo culture) => ClientResources.Get(nameof (FileTypeFilter), culture);

    public static string ClientCertificateMissing(object arg0) => ClientResources.Format(nameof (ClientCertificateMissing), arg0);

    public static string ClientCertificateMissing(object arg0, CultureInfo culture) => ClientResources.Format(nameof (ClientCertificateMissing), culture, arg0);

    public static string SmartCardMissing(object arg0) => ClientResources.Format(nameof (SmartCardMissing), arg0);

    public static string SmartCardMissing(object arg0, CultureInfo culture) => ClientResources.Format(nameof (SmartCardMissing), culture, arg0);

    public static string ClientCertificateNoPermission(object arg0) => ClientResources.Format(nameof (ClientCertificateNoPermission), arg0);

    public static string ClientCertificateNoPermission(object arg0, CultureInfo culture) => ClientResources.Format(nameof (ClientCertificateNoPermission), culture, arg0);

    public static string ClientCertificateErrorReadingStore(object arg0) => ClientResources.Format(nameof (ClientCertificateErrorReadingStore), arg0);

    public static string ClientCertificateErrorReadingStore(object arg0, CultureInfo culture) => ClientResources.Format(nameof (ClientCertificateErrorReadingStore), culture, arg0);

    public static string ClientCertificatesEnabledDisabled(object arg0) => ClientResources.Format(nameof (ClientCertificatesEnabledDisabled), arg0);

    public static string ClientCertificatesEnabledDisabled(object arg0, CultureInfo culture) => ClientResources.Format(nameof (ClientCertificatesEnabledDisabled), culture, arg0);

    public static string ClientCertificatesUsingAny() => ClientResources.Get(nameof (ClientCertificatesUsingAny));

    public static string ClientCertificatesUsingAny(CultureInfo culture) => ClientResources.Get(nameof (ClientCertificatesUsingAny), culture);

    public static string ClientCertificatesUsingSpecific() => ClientResources.Get(nameof (ClientCertificatesUsingSpecific));

    public static string ClientCertificatesUsingSpecific(CultureInfo culture) => ClientResources.Get(nameof (ClientCertificatesUsingSpecific), culture);

    public static string ClientCertificatesAffectedByMachineSettings() => ClientResources.Get(nameof (ClientCertificatesAffectedByMachineSettings));

    public static string ClientCertificatesAffectedByMachineSettings(CultureInfo culture) => ClientResources.Get(nameof (ClientCertificatesAffectedByMachineSettings), culture);

    public static string NullArtifactUrlInUrlList() => ClientResources.Get(nameof (NullArtifactUrlInUrlList));

    public static string NullArtifactUrlInUrlList(CultureInfo culture) => ClientResources.Get(nameof (NullArtifactUrlInUrlList), culture);

    public static string MissingOption(object arg0) => ClientResources.Format(nameof (MissingOption), arg0);

    public static string MissingOption(object arg0, CultureInfo culture) => ClientResources.Format(nameof (MissingOption), culture, arg0);

    public static string ProcessIdentifierFormat(object arg0, object arg1, object arg2) => ClientResources.Format(nameof (ProcessIdentifierFormat), arg0, arg1, arg2);

    public static string ProcessIdentifierFormat(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ClientResources.Format(nameof (ProcessIdentifierFormat), culture, arg0, arg1, arg2);
    }

    public static string TfsConnectDialogTitleConnectToServer() => ClientResources.Get(nameof (TfsConnectDialogTitleConnectToServer));

    public static string TfsConnectDialogTitleConnectToServer(CultureInfo culture) => ClientResources.Get(nameof (TfsConnectDialogTitleConnectToServer), culture);

    public static string TfsConnectDialogSelectServerToViewCollections() => ClientResources.Get(nameof (TfsConnectDialogSelectServerToViewCollections));

    public static string TfsConnectDialogSelectServerToViewCollections(CultureInfo culture) => ClientResources.Get(nameof (TfsConnectDialogSelectServerToViewCollections), culture);

    public static string TfsConnectDialogSelectCollectionToViewProjects() => ClientResources.Get(nameof (TfsConnectDialogSelectCollectionToViewProjects));

    public static string TfsConnectDialogSelectCollectionToViewProjects(CultureInfo culture) => ClientResources.Get(nameof (TfsConnectDialogSelectCollectionToViewProjects), culture);

    public static string TfsConnectDialogWorking() => ClientResources.Get(nameof (TfsConnectDialogWorking));

    public static string TfsConnectDialogWorking(CultureInfo culture) => ClientResources.Get(nameof (TfsConnectDialogWorking), culture);

    public static string TfsConnectDialogLegacyServer(object arg0) => ClientResources.Format(nameof (TfsConnectDialogLegacyServer), arg0);

    public static string TfsConnectDialogLegacyServer(object arg0, CultureInfo culture) => ClientResources.Format(nameof (TfsConnectDialogLegacyServer), culture, arg0);

    public static string TfsConnectDialogRefresh() => ClientResources.Get(nameof (TfsConnectDialogRefresh));

    public static string TfsConnectDialogRefresh(CultureInfo culture) => ClientResources.Get(nameof (TfsConnectDialogRefresh), culture);

    public static string TfsConnectDialogActionLinkSeparator() => ClientResources.Get(nameof (TfsConnectDialogActionLinkSeparator));

    public static string TfsConnectDialogActionLinkSeparator(CultureInfo culture) => ClientResources.Get(nameof (TfsConnectDialogActionLinkSeparator), culture);

    public static string TfsConnectDialogSupplyCredentials() => ClientResources.Get(nameof (TfsConnectDialogSupplyCredentials));

    public static string TfsConnectDialogSupplyCredentials(CultureInfo culture) => ClientResources.Get(nameof (TfsConnectDialogSupplyCredentials), culture);

    public static string DomainNameAlreadyExists(object arg0) => ClientResources.Format(nameof (DomainNameAlreadyExists), arg0);

    public static string DomainNameAlreadyExists(object arg0, CultureInfo culture) => ClientResources.Format(nameof (DomainNameAlreadyExists), culture, arg0);

    public static string DomainNameAlreadyExistsTitle() => ClientResources.Get(nameof (DomainNameAlreadyExistsTitle));

    public static string DomainNameAlreadyExistsTitle(CultureInfo culture) => ClientResources.Get(nameof (DomainNameAlreadyExistsTitle), culture);

    public static string RemoveServer(object arg0) => ClientResources.Format(nameof (RemoveServer), arg0);

    public static string RemoveServer(object arg0, CultureInfo culture) => ClientResources.Format(nameof (RemoveServer), culture, arg0);

    public static string RemoveServerTitle() => ClientResources.Get(nameof (RemoveServerTitle));

    public static string RemoveServerTitle(CultureInfo culture) => ClientResources.Get(nameof (RemoveServerTitle), culture);

    public static string ServerNameEmpty() => ClientResources.Get(nameof (ServerNameEmpty));

    public static string ServerNameEmpty(CultureInfo culture) => ClientResources.Get(nameof (ServerNameEmpty), culture);

    public static string InvalidServerName() => ClientResources.Get(nameof (InvalidServerName));

    public static string InvalidServerName(CultureInfo culture) => ClientResources.Get(nameof (InvalidServerName), culture);

    public static string InvalidPortNumber() => ClientResources.Get(nameof (InvalidPortNumber));

    public static string InvalidPortNumber(CultureInfo culture) => ClientResources.Get(nameof (InvalidPortNumber), culture);

    public static string ConnectToTfs_NoPermission(object arg0, object arg1) => ClientResources.Format(nameof (ConnectToTfs_NoPermission), arg0, arg1);

    public static string ConnectToTfs_NoPermission(object arg0, object arg1, CultureInfo culture) => ClientResources.Format(nameof (ConnectToTfs_NoPermission), culture, arg0, arg1);

    public static string ConnectToTfs_AccessCheck(object arg0, object arg1) => ClientResources.Format(nameof (ConnectToTfs_AccessCheck), arg0, arg1);

    public static string ConnectToTfs_AccessCheck(object arg0, object arg1, CultureInfo culture) => ClientResources.Format(nameof (ConnectToTfs_AccessCheck), culture, arg0, arg1);

    public static string ConnectToTfs_UnableToConnect(object arg0) => ClientResources.Format(nameof (ConnectToTfs_UnableToConnect), arg0);

    public static string ConnectToTfs_UnableToConnect(object arg0, CultureInfo culture) => ClientResources.Format(nameof (ConnectToTfs_UnableToConnect), culture, arg0);

    public static string ConnectToTfs_UnExpected(object arg0) => ClientResources.Format(nameof (ConnectToTfs_UnExpected), arg0);

    public static string ConnectToTfs_UnExpected(object arg0, CultureInfo culture) => ClientResources.Format(nameof (ConnectToTfs_UnExpected), culture, arg0);

    public static string ConnectToTfs_AddServer_UnableToConnect(object arg0, object arg1) => ClientResources.Format(nameof (ConnectToTfs_AddServer_UnableToConnect), arg0, arg1);

    public static string ConnectToTfs_AddServer_UnableToConnect(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ClientResources.Format(nameof (ConnectToTfs_AddServer_UnableToConnect), culture, arg0, arg1);
    }

    public static string ConnectToTfs_AddServer_UnableToConnect_WithTechnicalInfo(
      object arg0,
      object arg1,
      object arg2)
    {
      return ClientResources.Format(nameof (ConnectToTfs_AddServer_UnableToConnect_WithTechnicalInfo), arg0, arg1, arg2);
    }

    public static string ConnectToTfs_AddServer_UnableToConnect_WithTechnicalInfo(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ClientResources.Format(nameof (ConnectToTfs_AddServer_UnableToConnect_WithTechnicalInfo), culture, arg0, arg1, arg2);
    }

    public static string ConnectToTfs_Unknown(object arg0, object arg1) => ClientResources.Format(nameof (ConnectToTfs_Unknown), arg0, arg1);

    public static string ConnectToTfs_Unknown(object arg0, object arg1, CultureInfo culture) => ClientResources.Format(nameof (ConnectToTfs_Unknown), culture, arg0, arg1);

    public static string ConnectToTfs_TrustFailure(object arg0) => ClientResources.Format(nameof (ConnectToTfs_TrustFailure), arg0);

    public static string ConnectToTfs_TrustFailure(object arg0, CultureInfo culture) => ClientResources.Format(nameof (ConnectToTfs_TrustFailure), culture, arg0);

    public static string ConnectDialogImages() => ClientResources.Get(nameof (ConnectDialogImages));

    public static string ConnectDialogImages(CultureInfo culture) => ClientResources.Get(nameof (ConnectDialogImages), culture);

    public static string TFHost_ErrorWritingTEConfigFile() => ClientResources.Get(nameof (TFHost_ErrorWritingTEConfigFile));

    public static string TFHost_ErrorWritingTEConfigFile(CultureInfo culture) => ClientResources.Get(nameof (TFHost_ErrorWritingTEConfigFile), culture);

    public static string RegisteredServersCannotFindCollection(object arg0) => ClientResources.Format(nameof (RegisteredServersCannotFindCollection), arg0);

    public static string RegisteredServersCannotFindCollection(object arg0, CultureInfo culture) => ClientResources.Format(nameof (RegisteredServersCannotFindCollection), culture, arg0);

    public static string AddDomainDialogAddingCollection() => ClientResources.Get(nameof (AddDomainDialogAddingCollection));

    public static string AddDomainDialogAddingCollection(CultureInfo culture) => ClientResources.Get(nameof (AddDomainDialogAddingCollection), culture);

    public static string AddDomainDialog_InvalidServer() => ClientResources.Get(nameof (AddDomainDialog_InvalidServer));

    public static string AddDomainDialog_InvalidServer(CultureInfo culture) => ClientResources.Get(nameof (AddDomainDialog_InvalidServer), culture);

    public static string AddDomainDialog_InvalidPort() => ClientResources.Get(nameof (AddDomainDialog_InvalidPort));

    public static string AddDomainDialog_InvalidPort(CultureInfo culture) => ClientResources.Get(nameof (AddDomainDialog_InvalidPort), culture);

    public static string ErrorCaption() => ClientResources.Get(nameof (ErrorCaption));

    public static string ErrorCaption(CultureInfo culture) => ClientResources.Get(nameof (ErrorCaption), culture);

    public static string InformationCaption() => ClientResources.Get(nameof (InformationCaption));

    public static string InformationCaption(CultureInfo culture) => ClientResources.Get(nameof (InformationCaption), culture);

    public static string WarningCaption() => ClientResources.Get(nameof (WarningCaption));

    public static string WarningCaption(CultureInfo culture) => ClientResources.Get(nameof (WarningCaption), culture);

    public static string UnregisteredCollectionInvalidMethodError() => ClientResources.Get(nameof (UnregisteredCollectionInvalidMethodError));

    public static string UnregisteredCollectionInvalidMethodError(CultureInfo culture) => ClientResources.Get(nameof (UnregisteredCollectionInvalidMethodError), culture);

    public static string InvalidConnectionString(object arg0) => ClientResources.Format(nameof (InvalidConnectionString), arg0);

    public static string InvalidConnectionString(object arg0, CultureInfo culture) => ClientResources.Format(nameof (InvalidConnectionString), culture, arg0);

    public static string MissingConnectionString(object arg0) => ClientResources.Format(nameof (MissingConnectionString), arg0);

    public static string MissingConnectionString(object arg0, CultureInfo culture) => ClientResources.Format(nameof (MissingConnectionString), culture, arg0);

    public static string CollectionServicingJobDidNotSucceed() => ClientResources.Get(nameof (CollectionServicingJobDidNotSucceed));

    public static string CollectionServicingJobDidNotSucceed(CultureInfo culture) => ClientResources.Get(nameof (CollectionServicingJobDidNotSucceed), culture);

    public static string OperationNotSuportedPreFramework() => ClientResources.Get(nameof (OperationNotSuportedPreFramework));

    public static string OperationNotSuportedPreFramework(CultureInfo culture) => ClientResources.Get(nameof (OperationNotSuportedPreFramework), culture);

    public static string ServicingOperationConfigDoesNotDefineOperationNameError() => ClientResources.Get(nameof (ServicingOperationConfigDoesNotDefineOperationNameError));

    public static string ServicingOperationConfigDoesNotDefineOperationNameError(CultureInfo culture) => ClientResources.Get(nameof (ServicingOperationConfigDoesNotDefineOperationNameError), culture);

    public static string WaitForCollectionServicingTimeout() => ClientResources.Get(nameof (WaitForCollectionServicingTimeout));

    public static string WaitForCollectionServicingTimeout(CultureInfo culture) => ClientResources.Get(nameof (WaitForCollectionServicingTimeout), culture);

    public static string RegisteredInstancedDuplicateInstances() => ClientResources.Get(nameof (RegisteredInstancedDuplicateInstances));

    public static string RegisteredInstancedDuplicateInstances(CultureInfo culture) => ClientResources.Get(nameof (RegisteredInstancedDuplicateInstances), culture);

    public static string DialogAuthenticate_Waiting(object arg0) => ClientResources.Format(nameof (DialogAuthenticate_Waiting), arg0);

    public static string DialogAuthenticate_Waiting(object arg0, CultureInfo culture) => ClientResources.Format(nameof (DialogAuthenticate_Waiting), culture, arg0);

    public static string DialogAuthenticate_Done() => ClientResources.Get(nameof (DialogAuthenticate_Done));

    public static string DialogAuthenticate_Done(CultureInfo culture) => ClientResources.Get(nameof (DialogAuthenticate_Done), culture);

    public static string TfsmqInvalidChannelType(object arg0) => ClientResources.Format(nameof (TfsmqInvalidChannelType), arg0);

    public static string TfsmqInvalidChannelType(object arg0, CultureInfo culture) => ClientResources.Format(nameof (TfsmqInvalidChannelType), culture, arg0);

    public static string TfsmqTransportNoProjectCollection() => ClientResources.Get(nameof (TfsmqTransportNoProjectCollection));

    public static string TfsmqTransportNoProjectCollection(CultureInfo culture) => ClientResources.Get(nameof (TfsmqTransportNoProjectCollection), culture);

    public static string TfsmqInvalidOperationOneWayOnly(object arg0, object arg1) => ClientResources.Format(nameof (TfsmqInvalidOperationOneWayOnly), arg0, arg1);

    public static string TfsmqInvalidOperationOneWayOnly(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ClientResources.Format(nameof (TfsmqInvalidOperationOneWayOnly), culture, arg0, arg1);
    }

    public static string TfsmqReceiveTimeout(object arg0) => ClientResources.Format(nameof (TfsmqReceiveTimeout), arg0);

    public static string TfsmqReceiveTimeout(object arg0, CultureInfo culture) => ClientResources.Format(nameof (TfsmqReceiveTimeout), culture, arg0);

    public static string TfsmqInvalidServerConfiguration(object arg0, object arg1) => ClientResources.Format(nameof (TfsmqInvalidServerConfiguration), arg0, arg1);

    public static string TfsmqInvalidServerConfiguration(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ClientResources.Format(nameof (TfsmqInvalidServerConfiguration), culture, arg0, arg1);
    }

    public static string TfsmqAcceptChannelTimeout(object arg0) => ClientResources.Format(nameof (TfsmqAcceptChannelTimeout), arg0);

    public static string TfsmqAcceptChannelTimeout(object arg0, CultureInfo culture) => ClientResources.Format(nameof (TfsmqAcceptChannelTimeout), culture, arg0);

    public static string TfsmqNotSupportedOnProjectCollection(object arg0) => ClientResources.Format(nameof (TfsmqNotSupportedOnProjectCollection), arg0);

    public static string TfsmqNotSupportedOnProjectCollection(object arg0, CultureInfo culture) => ClientResources.Format(nameof (TfsmqNotSupportedOnProjectCollection), culture, arg0);

    public static string TfsmqInvalidUri(object arg0) => ClientResources.Format(nameof (TfsmqInvalidUri), arg0);

    public static string TfsmqInvalidUri(object arg0, CultureInfo culture) => ClientResources.Format(nameof (TfsmqInvalidUri), culture, arg0);

    public static string MultipleIdentitiesFoundMessage(object arg0, object arg1) => ClientResources.Format(nameof (MultipleIdentitiesFoundMessage), arg0, arg1);

    public static string MultipleIdentitiesFoundMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ClientResources.Format(nameof (MultipleIdentitiesFoundMessage), culture, arg0, arg1);
    }

    public static string ArgumentNotSupportedClass(object arg0) => ClientResources.Format(nameof (ArgumentNotSupportedClass), arg0);

    public static string ArgumentNotSupportedClass(object arg0, CultureInfo culture) => ClientResources.Format(nameof (ArgumentNotSupportedClass), culture, arg0);

    public static string TeamProjectDeleter_CouldNotConnect(object arg0) => ClientResources.Format(nameof (TeamProjectDeleter_CouldNotConnect), arg0);

    public static string TeamProjectDeleter_CouldNotConnect(object arg0, CultureInfo culture) => ClientResources.Format(nameof (TeamProjectDeleter_CouldNotConnect), culture, arg0);

    public static string TeamProjectDeleter_DeletingFrom(object arg0) => ClientResources.Format(nameof (TeamProjectDeleter_DeletingFrom), arg0);

    public static string TeamProjectDeleter_DeletingFrom(object arg0, CultureInfo culture) => ClientResources.Format(nameof (TeamProjectDeleter_DeletingFrom), culture, arg0);

    public static string TeamProjectDeleter_DeletingFromRosetta() => ClientResources.Get(nameof (TeamProjectDeleter_DeletingFromRosetta));

    public static string TeamProjectDeleter_DeletingFromRosetta(CultureInfo culture) => ClientResources.Get(nameof (TeamProjectDeleter_DeletingFromRosetta), culture);

    public static string TeamProjectDeleter_DetachingFromWss() => ClientResources.Get(nameof (TeamProjectDeleter_DetachingFromWss));

    public static string TeamProjectDeleter_DetachingFromWss(CultureInfo culture) => ClientResources.Get(nameof (TeamProjectDeleter_DetachingFromWss), culture);

    public static string TeamProjectDeleter_DeletingFromWss() => ClientResources.Get(nameof (TeamProjectDeleter_DeletingFromWss));

    public static string TeamProjectDeleter_DeletingFromWss(CultureInfo culture) => ClientResources.Get(nameof (TeamProjectDeleter_DeletingFromWss), culture);

    public static string TeamProjectDeleter_Done() => ClientResources.Get(nameof (TeamProjectDeleter_Done));

    public static string TeamProjectDeleter_Done(CultureInfo culture) => ClientResources.Get(nameof (TeamProjectDeleter_Done), culture);

    public static string TeamProjectDeleter_EleadNotFound() => ClientResources.Get(nameof (TeamProjectDeleter_EleadNotFound));

    public static string TeamProjectDeleter_EleadNotFound(CultureInfo culture) => ClientResources.Get(nameof (TeamProjectDeleter_EleadNotFound), culture);

    public static string TeamProjectDeleter_Failure() => ClientResources.Get(nameof (TeamProjectDeleter_Failure));

    public static string TeamProjectDeleter_Failure(CultureInfo culture) => ClientResources.Get(nameof (TeamProjectDeleter_Failure), culture);

    public static string TeamProjectDeleter_ProjectNotFound(object arg0) => ClientResources.Format(nameof (TeamProjectDeleter_ProjectNotFound), arg0);

    public static string TeamProjectDeleter_ProjectNotFound(object arg0, CultureInfo culture) => ClientResources.Format(nameof (TeamProjectDeleter_ProjectNotFound), culture, arg0);

    public static string TeamProjectDeleter_RosettaNotFound() => ClientResources.Get(nameof (TeamProjectDeleter_RosettaNotFound));

    public static string TeamProjectDeleter_RosettaNotFound(CultureInfo culture) => ClientResources.Get(nameof (TeamProjectDeleter_RosettaNotFound), culture);

    public static string TeamProjectDeleter_WssNotFound() => ClientResources.Get(nameof (TeamProjectDeleter_WssNotFound));

    public static string TeamProjectDeleter_WssNotFound(CultureInfo culture) => ClientResources.Get(nameof (TeamProjectDeleter_WssNotFound), culture);

    public static string TeamProjectDeleter_DomainError() => ClientResources.Get(nameof (TeamProjectDeleter_DomainError));

    public static string TeamProjectDeleter_DomainError(CultureInfo culture) => ClientResources.Get(nameof (TeamProjectDeleter_DomainError), culture);

    public static string TeamProjectDeleter_TryUsingForce() => ClientResources.Get(nameof (TeamProjectDeleter_TryUsingForce));

    public static string TeamProjectDeleter_TryUsingForce(CultureInfo culture) => ClientResources.Get(nameof (TeamProjectDeleter_TryUsingForce), culture);

    public static string TeamProjectDeleter_MissingName() => ClientResources.Get(nameof (TeamProjectDeleter_MissingName));

    public static string TeamProjectDeleter_MissingName(CultureInfo culture) => ClientResources.Get(nameof (TeamProjectDeleter_MissingName), culture);

    public static string TeamProjectDeleter_MissingDomain(object arg0) => ClientResources.Format(nameof (TeamProjectDeleter_MissingDomain), arg0);

    public static string TeamProjectDeleter_MissingDomain(object arg0, CultureInfo culture) => ClientResources.Format(nameof (TeamProjectDeleter_MissingDomain), culture, arg0);

    public static string TeamProjectDeleter_FailureInstructions() => ClientResources.Get(nameof (TeamProjectDeleter_FailureInstructions));

    public static string TeamProjectDeleter_FailureInstructions(CultureInfo culture) => ClientResources.Get(nameof (TeamProjectDeleter_FailureInstructions), culture);

    public static string ToolNames_VersionControl() => ClientResources.Get(nameof (ToolNames_VersionControl));

    public static string ToolNames_VersionControl(CultureInfo culture) => ClientResources.Get(nameof (ToolNames_VersionControl), culture);

    public static string ToolNames_WorkItemTracking() => ClientResources.Get(nameof (ToolNames_WorkItemTracking));

    public static string ToolNames_WorkItemTracking(CultureInfo culture) => ClientResources.Get(nameof (ToolNames_WorkItemTracking), culture);

    public static string ToolNames_TeamFoundation() => ClientResources.Get(nameof (ToolNames_TeamFoundation));

    public static string ToolNames_TeamFoundation(CultureInfo culture) => ClientResources.Get(nameof (ToolNames_TeamFoundation), culture);

    public static string ToolIdMalformed(object arg0) => ClientResources.Format(nameof (ToolIdMalformed), arg0);

    public static string ToolIdMalformed(object arg0, CultureInfo culture) => ClientResources.Format(nameof (ToolIdMalformed), culture, arg0);

    public static string NoServiceInterfaceByName(object arg0, object arg1) => ClientResources.Format(nameof (NoServiceInterfaceByName), arg0, arg1);

    public static string NoServiceInterfaceByName(object arg0, object arg1, CultureInfo culture) => ClientResources.Format(nameof (NoServiceInterfaceByName), culture, arg0, arg1);

    public static string NullServiceUrl(object arg0, object arg1) => ClientResources.Format(nameof (NullServiceUrl), arg0, arg1);

    public static string NullServiceUrl(object arg0, object arg1, CultureInfo culture) => ClientResources.Format(nameof (NullServiceUrl), culture, arg0, arg1);

    public static string MoreThanOneServiceInstance(object arg0) => ClientResources.Format(nameof (MoreThanOneServiceInstance), arg0);

    public static string MoreThanOneServiceInstance(object arg0, CultureInfo culture) => ClientResources.Format(nameof (MoreThanOneServiceInstance), culture, arg0);

    public static string NullOrEmptyServiceInterface(object arg0, object arg1) => ClientResources.Format(nameof (NullOrEmptyServiceInterface), arg0, arg1);

    public static string NullOrEmptyServiceInterface(object arg0, object arg1, CultureInfo culture) => ClientResources.Format(nameof (NullOrEmptyServiceInterface), culture, arg0, arg1);

    public static string NoRegistrationEntries(object arg0) => ClientResources.Format(nameof (NoRegistrationEntries), arg0);

    public static string NoRegistrationEntries(object arg0, CultureInfo culture) => ClientResources.Format(nameof (NoRegistrationEntries), culture, arg0);

    public static string NoServiceInterfaces(object arg0) => ClientResources.Format(nameof (NoServiceInterfaces), arg0);

    public static string NoServiceInterfaces(object arg0, CultureInfo culture) => ClientResources.Format(nameof (NoServiceInterfaces), culture, arg0);

    public static string NodeDoesNotExistMessage(object arg0, object arg1) => ClientResources.Format(nameof (NodeDoesNotExistMessage), arg0, arg1);

    public static string NodeDoesNotExistMessage(object arg0, object arg1, CultureInfo culture) => ClientResources.Format(nameof (NodeDoesNotExistMessage), culture, arg0, arg1);

    public static string RegistrationInInconsistentStateMessage() => ClientResources.Get(nameof (RegistrationInInconsistentStateMessage));

    public static string RegistrationInInconsistentStateMessage(CultureInfo culture) => ClientResources.Get(nameof (RegistrationInInconsistentStateMessage), culture);

    public static string TypeCannotSerializeMessage(object arg0) => ClientResources.Format(nameof (TypeCannotSerializeMessage), arg0);

    public static string TypeCannotSerializeMessage(object arg0, CultureInfo culture) => ClientResources.Format(nameof (TypeCannotSerializeMessage), culture, arg0);

    public static string ManageTfsListApplicationInstanceNotSupported() => ClientResources.Get(nameof (ManageTfsListApplicationInstanceNotSupported));

    public static string ManageTfsListApplicationInstanceNotSupported(CultureInfo culture) => ClientResources.Get(nameof (ManageTfsListApplicationInstanceNotSupported), culture);

    public static string GenericTeamFoundationCaption() => ClientResources.Get(nameof (GenericTeamFoundationCaption));

    public static string GenericTeamFoundationCaption(CultureInfo culture) => ClientResources.Get(nameof (GenericTeamFoundationCaption), culture);

    public static string MicrosoftVisualStudioCaption() => ClientResources.Get(nameof (MicrosoftVisualStudioCaption));

    public static string MicrosoftVisualStudioCaption(CultureInfo culture) => ClientResources.Get(nameof (MicrosoftVisualStudioCaption), culture);

    public static string EnterValidPath() => ClientResources.Get(nameof (EnterValidPath));

    public static string EnterValidPath(CultureInfo culture) => ClientResources.Get(nameof (EnterValidPath), culture);

    public static string HttpRequestTimeout(object arg0) => ClientResources.Format(nameof (HttpRequestTimeout), arg0);

    public static string HttpRequestTimeout(object arg0, CultureInfo culture) => ClientResources.Format(nameof (HttpRequestTimeout), culture, arg0);

    public static string LocalMetadataTableMutexTimeout() => ClientResources.Get(nameof (LocalMetadataTableMutexTimeout));

    public static string LocalMetadataTableMutexTimeout(CultureInfo culture) => ClientResources.Get(nameof (LocalMetadataTableMutexTimeout), culture);

    public static string ACSAuthenticationError(object arg0) => ClientResources.Format(nameof (ACSAuthenticationError), arg0);

    public static string ACSAuthenticationError(object arg0, CultureInfo culture) => ClientResources.Format(nameof (ACSAuthenticationError), culture, arg0);

    public static string ConfigurationErrorsEncountered(object arg0) => ClientResources.Format(nameof (ConfigurationErrorsEncountered), arg0);

    public static string ConfigurationErrorsEncountered(object arg0, CultureInfo culture) => ClientResources.Format(nameof (ConfigurationErrorsEncountered), culture, arg0);

    public static string FileNotFound(object arg0) => ClientResources.Format(nameof (FileNotFound), arg0);

    public static string FileNotFound(object arg0, CultureInfo culture) => ClientResources.Format(nameof (FileNotFound), culture, arg0);

    public static string UnsafeFileOpenPrompt() => ClientResources.Get(nameof (UnsafeFileOpenPrompt));

    public static string UnsafeFileOpenPrompt(CultureInfo culture) => ClientResources.Get(nameof (UnsafeFileOpenPrompt), culture);

    public static string HttpClientOpenTimeout(object arg0) => ClientResources.Format(nameof (HttpClientOpenTimeout), arg0);

    public static string HttpClientOpenTimeout(object arg0, CultureInfo culture) => ClientResources.Format(nameof (HttpClientOpenTimeout), culture, arg0);

    public static string HttpClientAlreadyOpened() => ClientResources.Get(nameof (HttpClientAlreadyOpened));

    public static string HttpClientAlreadyOpened(CultureInfo culture) => ClientResources.Get(nameof (HttpClientAlreadyOpened), culture);

    public static string SimpleWebCredentialsMissing() => ClientResources.Get(nameof (SimpleWebCredentialsMissing));

    public static string SimpleWebCredentialsMissing(CultureInfo culture) => ClientResources.Get(nameof (SimpleWebCredentialsMissing), culture);

    public static string FederatedCredentialWithNoIssuedTokenProvider() => ClientResources.Get(nameof (FederatedCredentialWithNoIssuedTokenProvider));

    public static string FederatedCredentialWithNoIssuedTokenProvider(CultureInfo culture) => ClientResources.Get(nameof (FederatedCredentialWithNoIssuedTokenProvider), culture);

    public static string TokenProviderReturnInvalidCurrentToken() => ClientResources.Get(nameof (TokenProviderReturnInvalidCurrentToken));

    public static string TokenProviderReturnInvalidCurrentToken(CultureInfo culture) => ClientResources.Get(nameof (TokenProviderReturnInvalidCurrentToken), culture);

    public static string TfsConnectDialogSwitchUser() => ClientResources.Get(nameof (TfsConnectDialogSwitchUser));

    public static string TfsConnectDialogSwitchUser(CultureInfo culture) => ClientResources.Get(nameof (TfsConnectDialogSwitchUser), culture);

    public static string TfsConnectDialogUserLabelFormat(object arg0, object arg1) => ClientResources.Format(nameof (TfsConnectDialogUserLabelFormat), arg0, arg1);

    public static string TfsConnectDialogUserLabelFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ClientResources.Format(nameof (TfsConnectDialogUserLabelFormat), culture, arg0, arg1);
    }

    public static string TruncatedStringFormat(object arg0) => ClientResources.Format(nameof (TruncatedStringFormat), arg0);

    public static string TruncatedStringFormat(object arg0, CultureInfo culture) => ClientResources.Format(nameof (TruncatedStringFormat), culture, arg0);

    public static string CannotAuthenticateAsAnotherUser(object arg0, object arg1) => ClientResources.Format(nameof (CannotAuthenticateAsAnotherUser), arg0, arg1);

    public static string CannotAuthenticateAsAnotherUser(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ClientResources.Format(nameof (CannotAuthenticateAsAnotherUser), culture, arg0, arg1);
    }

    public static string ConnectionUserChangedWarning(object arg0) => ClientResources.Format(nameof (ConnectionUserChangedWarning), arg0);

    public static string ConnectionUserChangedWarning(object arg0, CultureInfo culture) => ClientResources.Format(nameof (ConnectionUserChangedWarning), culture, arg0);

    public static string SwitchUserNotSupportedOnPremise() => ClientResources.Get(nameof (SwitchUserNotSupportedOnPremise));

    public static string SwitchUserNotSupportedOnPremise(CultureInfo culture) => ClientResources.Get(nameof (SwitchUserNotSupportedOnPremise), culture);

    public static string SwitchUserRequiresFederatedAuthentication() => ClientResources.Get(nameof (SwitchUserRequiresFederatedAuthentication));

    public static string SwitchUserRequiresFederatedAuthentication(CultureInfo culture) => ClientResources.Get(nameof (SwitchUserRequiresFederatedAuthentication), culture);

    public static string DeleteVolatileCacheDirectoryError(object arg0, object arg1) => ClientResources.Format(nameof (DeleteVolatileCacheDirectoryError), arg0, arg1);

    public static string DeleteVolatileCacheDirectoryError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ClientResources.Format(nameof (DeleteVolatileCacheDirectoryError), culture, arg0, arg1);
    }

    public static string ServerNotSupported(object arg0) => ClientResources.Format(nameof (ServerNotSupported), arg0);

    public static string ServerNotSupported(object arg0, CultureInfo culture) => ClientResources.Format(nameof (ServerNotSupported), culture, arg0);

    public static string JobDefinitionPropertiesCannotSet() => ClientResources.Get(nameof (JobDefinitionPropertiesCannotSet));

    public static string JobDefinitionPropertiesCannotSet(CultureInfo culture) => ClientResources.Get(nameof (JobDefinitionPropertiesCannotSet), culture);

    public static string InvalidProxyUrl(object arg0) => ClientResources.Format(nameof (InvalidProxyUrl), arg0);

    public static string InvalidProxyUrl(object arg0, CultureInfo culture) => ClientResources.Format(nameof (InvalidProxyUrl), culture, arg0);

    public static string TeamProjectDeleter_OperationCancelledAtTime(object arg0) => ClientResources.Format(nameof (TeamProjectDeleter_OperationCancelledAtTime), arg0);

    public static string TeamProjectDeleter_OperationCancelledAtTime(
      object arg0,
      CultureInfo culture)
    {
      return ClientResources.Format(nameof (TeamProjectDeleter_OperationCancelledAtTime), culture, arg0);
    }

    public static string TeamProjectDeleter_OperationFailedAtTime(object arg0) => ClientResources.Format(nameof (TeamProjectDeleter_OperationFailedAtTime), arg0);

    public static string TeamProjectDeleter_OperationFailedAtTime(object arg0, CultureInfo culture) => ClientResources.Format(nameof (TeamProjectDeleter_OperationFailedAtTime), culture, arg0);

    public static string TeamProjectDeleter_OperationInProgressAtTime(object arg0) => ClientResources.Format(nameof (TeamProjectDeleter_OperationInProgressAtTime), arg0);

    public static string TeamProjectDeleter_OperationInProgressAtTime(
      object arg0,
      CultureInfo culture)
    {
      return ClientResources.Format(nameof (TeamProjectDeleter_OperationInProgressAtTime), culture, arg0);
    }

    public static string TeamProjectDeleter_OperationQueuedAtTime(object arg0) => ClientResources.Format(nameof (TeamProjectDeleter_OperationQueuedAtTime), arg0);

    public static string TeamProjectDeleter_OperationQueuedAtTime(object arg0, CultureInfo culture) => ClientResources.Format(nameof (TeamProjectDeleter_OperationQueuedAtTime), culture, arg0);

    public static string TeamProjectDeleter_OperationSucceededAtTime(object arg0) => ClientResources.Format(nameof (TeamProjectDeleter_OperationSucceededAtTime), arg0);

    public static string TeamProjectDeleter_OperationSucceededAtTime(
      object arg0,
      CultureInfo culture)
    {
      return ClientResources.Format(nameof (TeamProjectDeleter_OperationSucceededAtTime), culture, arg0);
    }

    public static string TeamProjectDeleter_DeletingFromTfs() => ClientResources.Get(nameof (TeamProjectDeleter_DeletingFromTfs));

    public static string TeamProjectDeleter_DeletingFromTfs(CultureInfo culture) => ClientResources.Get(nameof (TeamProjectDeleter_DeletingFromTfs), culture);

    public static string TeamProjectDeleter_OperationHasId(object arg0) => ClientResources.Format(nameof (TeamProjectDeleter_OperationHasId), arg0);

    public static string TeamProjectDeleter_OperationHasId(object arg0, CultureInfo culture) => ClientResources.Format(nameof (TeamProjectDeleter_OperationHasId), culture, arg0);

    public static string TeamProjectDeleter_ProjectServerNotFound() => ClientResources.Get(nameof (TeamProjectDeleter_ProjectServerNotFound));

    public static string TeamProjectDeleter_ProjectServerNotFound(CultureInfo culture) => ClientResources.Get(nameof (TeamProjectDeleter_ProjectServerNotFound), culture);

    public static string ToolNames_Build() => ClientResources.Get(nameof (ToolNames_Build));

    public static string ToolNames_Build(CultureInfo culture) => ClientResources.Get(nameof (ToolNames_Build), culture);

    public static string ToolNames_ProjectServer() => ClientResources.Get(nameof (ToolNames_ProjectServer));

    public static string ToolNames_ProjectServer(CultureInfo culture) => ClientResources.Get(nameof (ToolNames_ProjectServer), culture);

    public static string ToolNames_TeamProjects() => ClientResources.Get(nameof (ToolNames_TeamProjects));

    public static string ToolNames_TeamProjects(CultureInfo culture) => ClientResources.Get(nameof (ToolNames_TeamProjects), culture);

    public static string ToolNames_WSS() => ClientResources.Get(nameof (ToolNames_WSS));

    public static string ToolNames_WSS(CultureInfo culture) => ClientResources.Get(nameof (ToolNames_WSS), culture);

    public static string TeamProjectDeleter_InitializationException(object arg0) => ClientResources.Format(nameof (TeamProjectDeleter_InitializationException), arg0);

    public static string TeamProjectDeleter_InitializationException(
      object arg0,
      CultureInfo culture)
    {
      return ClientResources.Format(nameof (TeamProjectDeleter_InitializationException), culture, arg0);
    }

    public static string TeamProjectDeleter_ProjectNotFoundOnService() => ClientResources.Get(nameof (TeamProjectDeleter_ProjectNotFoundOnService));

    public static string TeamProjectDeleter_ProjectNotFoundOnService(CultureInfo culture) => ClientResources.Get(nameof (TeamProjectDeleter_ProjectNotFoundOnService), culture);

    public static string ProcessModelIsNotCorrect(object arg0) => ClientResources.Format(nameof (ProcessModelIsNotCorrect), arg0);

    public static string ProcessModelIsNotCorrect(object arg0, CultureInfo culture) => ClientResources.Format(nameof (ProcessModelIsNotCorrect), culture, arg0);

    public static string InternetExplorerNotFound(object arg0) => ClientResources.Format(nameof (InternetExplorerNotFound), arg0);

    public static string InternetExplorerNotFound(object arg0, CultureInfo culture) => ClientResources.Format(nameof (InternetExplorerNotFound), culture, arg0);

    public static string TeamProjectDeleter_SoftDeleteNotSupported() => ClientResources.Get(nameof (TeamProjectDeleter_SoftDeleteNotSupported));

    public static string TeamProjectDeleter_SoftDeleteNotSupported(CultureInfo culture) => ClientResources.Get(nameof (TeamProjectDeleter_SoftDeleteNotSupported), culture);
  }
}
