// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineManagementResources
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  internal static class MachineManagementResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (MachineManagementResources), typeof (MachineManagementResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => MachineManagementResources.s_resMgr;

    private static string Get(string resourceName) => MachineManagementResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? MachineManagementResources.Get(resourceName) : MachineManagementResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) MachineManagementResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? MachineManagementResources.GetInt(resourceName) : (int) MachineManagementResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) MachineManagementResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? MachineManagementResources.GetBool(resourceName) : (bool) MachineManagementResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => MachineManagementResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = MachineManagementResources.Get(resourceName, culture);
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

    public static string DefaultImageAlreadyExistsException(object arg0) => MachineManagementResources.Format(nameof (DefaultImageAlreadyExistsException), arg0);

    public static string DefaultImageAlreadyExistsException(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (DefaultImageAlreadyExistsException), culture, arg0);

    public static string ImageNameAlreadyExistsException(object arg0) => MachineManagementResources.Format(nameof (ImageNameAlreadyExistsException), arg0);

    public static string ImageNameAlreadyExistsException(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (ImageNameAlreadyExistsException), culture, arg0);

    public static string ImageWithGivenNameDoesNotExistException(object arg0) => MachineManagementResources.Format(nameof (ImageWithGivenNameDoesNotExistException), arg0);

    public static string ImageWithGivenNameDoesNotExistException(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (ImageWithGivenNameDoesNotExistException), culture, arg0);

    public static string MachineImageDeleteException(object arg0) => MachineManagementResources.Format(nameof (MachineImageDeleteException), arg0);

    public static string MachineImageDeleteException(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (MachineImageDeleteException), culture, arg0);

    public static string MachineImageExistsException(object arg0, object arg1) => MachineManagementResources.Format(nameof (MachineImageExistsException), arg0, arg1);

    public static string MachineImageExistsException(object arg0, object arg1, CultureInfo culture) => MachineManagementResources.Format(nameof (MachineImageExistsException), culture, arg0, arg1);

    public static string MachineImageNotFoundException(object arg0) => MachineManagementResources.Format(nameof (MachineImageNotFoundException), arg0);

    public static string MachineImageNotFoundException(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (MachineImageNotFoundException), culture, arg0);

    public static string MachineInstanceExistsException(object arg0, object arg1) => MachineManagementResources.Format(nameof (MachineInstanceExistsException), arg0, arg1);

    public static string MachineInstanceExistsException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return MachineManagementResources.Format(nameof (MachineInstanceExistsException), culture, arg0, arg1);
    }

    public static string MachineInstanceNotFoundException(object arg0, object arg1) => MachineManagementResources.Format(nameof (MachineInstanceNotFoundException), arg0, arg1);

    public static string MachineInstanceNotFoundException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return MachineManagementResources.Format(nameof (MachineInstanceNotFoundException), culture, arg0, arg1);
    }

    public static string MachineOSImageDeleteException(object arg0, object arg1) => MachineManagementResources.Format(nameof (MachineOSImageDeleteException), arg0, arg1);

    public static string MachineOSImageDeleteException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return MachineManagementResources.Format(nameof (MachineOSImageDeleteException), culture, arg0, arg1);
    }

    public static string MachineOSImageDoesNotExistException(object arg0, object arg1) => MachineManagementResources.Format(nameof (MachineOSImageDoesNotExistException), arg0, arg1);

    public static string MachineOSImageDoesNotExistException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return MachineManagementResources.Format(nameof (MachineOSImageDoesNotExistException), culture, arg0, arg1);
    }

    public static string MachineOSImageExistsException(object arg0, object arg1) => MachineManagementResources.Format(nameof (MachineOSImageExistsException), arg0, arg1);

    public static string MachineOSImageExistsException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return MachineManagementResources.Format(nameof (MachineOSImageExistsException), culture, arg0, arg1);
    }

    public static string MachinePoolAliasExistsException(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return MachineManagementResources.Format(nameof (MachinePoolAliasExistsException), arg0, arg1, arg2, arg3);
    }

    public static string MachinePoolAliasExistsException(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return MachineManagementResources.Format(nameof (MachinePoolAliasExistsException), culture, arg0, arg1, arg2, arg3);
    }

    public static string MachinePoolDeleteException(object arg0) => MachineManagementResources.Format(nameof (MachinePoolDeleteException), arg0);

    public static string MachinePoolDeleteException(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (MachinePoolDeleteException), culture, arg0);

    public static string MachinePoolExistsException(object arg0) => MachineManagementResources.Format(nameof (MachinePoolExistsException), arg0);

    public static string MachinePoolExistsException(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (MachinePoolExistsException), culture, arg0);

    public static string MachinePoolNotFoundException(object arg0) => MachineManagementResources.Format(nameof (MachinePoolNotFoundException), arg0);

    public static string MachinePoolNotFoundException(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (MachinePoolNotFoundException), culture, arg0);

    public static string MachinePoolTypeDeleteException(object arg0) => MachineManagementResources.Format(nameof (MachinePoolTypeDeleteException), arg0);

    public static string MachinePoolTypeDeleteException(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (MachinePoolTypeDeleteException), culture, arg0);

    public static string MachinePoolTypeDoesNotExistException(object arg0) => MachineManagementResources.Format(nameof (MachinePoolTypeDoesNotExistException), arg0);

    public static string MachinePoolTypeDoesNotExistException(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (MachinePoolTypeDoesNotExistException), culture, arg0);

    public static string MachinePoolTypeExistsException(object arg0) => MachineManagementResources.Format(nameof (MachinePoolTypeExistsException), arg0);

    public static string MachinePoolTypeExistsException(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (MachinePoolTypeExistsException), culture, arg0);

    public static string MachineRequestAlreadyFinishedException(object arg0, object arg1) => MachineManagementResources.Format(nameof (MachineRequestAlreadyFinishedException), arg0, arg1);

    public static string MachineRequestAlreadyFinishedException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return MachineManagementResources.Format(nameof (MachineRequestAlreadyFinishedException), culture, arg0, arg1);
    }

    public static string MachineRequestAlreadyStartedException(object arg0, object arg1) => MachineManagementResources.Format(nameof (MachineRequestAlreadyStartedException), arg0, arg1);

    public static string MachineRequestAlreadyStartedException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return MachineManagementResources.Format(nameof (MachineRequestAlreadyStartedException), culture, arg0, arg1);
    }

    public static string MachineRequestDoesNotExistException(object arg0, object arg1) => MachineManagementResources.Format(nameof (MachineRequestDoesNotExistException), arg0, arg1);

    public static string MachineRequestDoesNotExistException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return MachineManagementResources.Format(nameof (MachineRequestDoesNotExistException), culture, arg0, arg1);
    }

    public static string MachineRequestInProgressException(object arg0, object arg1, object arg2) => MachineManagementResources.Format(nameof (MachineRequestInProgressException), arg0, arg1, arg2);

    public static string MachineRequestInProgressException(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return MachineManagementResources.Format(nameof (MachineRequestInProgressException), culture, arg0, arg1, arg2);
    }

    public static string MachineRequestTypeNotFoundException(object arg0) => MachineManagementResources.Format(nameof (MachineRequestTypeNotFoundException), arg0);

    public static string MachineRequestTypeNotFoundException(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (MachineRequestTypeNotFoundException), culture, arg0);

    public static string MultipleAliasesExistForSameMachinePoolException(object arg0, object arg1) => MachineManagementResources.Format(nameof (MultipleAliasesExistForSameMachinePoolException), arg0, arg1);

    public static string MultipleAliasesExistForSameMachinePoolException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return MachineManagementResources.Format(nameof (MultipleAliasesExistForSameMachinePoolException), culture, arg0, arg1);
    }

    public static string ResourceGroupAlreadyExistsException(object arg0) => MachineManagementResources.Format(nameof (ResourceGroupAlreadyExistsException), arg0);

    public static string ResourceGroupAlreadyExistsException(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (ResourceGroupAlreadyExistsException), culture, arg0);

    public static string ResourceGroupNotFoundException(object arg0) => MachineManagementResources.Format(nameof (ResourceGroupNotFoundException), arg0);

    public static string ResourceGroupNotFoundException(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (ResourceGroupNotFoundException), culture, arg0);

    public static string ServiceDefinitionDoesNotExist(object arg0, object arg1) => MachineManagementResources.Format(nameof (ServiceDefinitionDoesNotExist), arg0, arg1);

    public static string ServiceDefinitionDoesNotExist(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return MachineManagementResources.Format(nameof (ServiceDefinitionDoesNotExist), culture, arg0, arg1);
    }

    public static string StorageAccountAlreadyExistsException(object arg0) => MachineManagementResources.Format(nameof (StorageAccountAlreadyExistsException), arg0);

    public static string StorageAccountAlreadyExistsException(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (StorageAccountAlreadyExistsException), culture, arg0);

    public static string StorageAccountNotFoundException(object arg0) => MachineManagementResources.Format(nameof (StorageAccountNotFoundException), arg0);

    public static string StorageAccountNotFoundException(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (StorageAccountNotFoundException), culture, arg0);

    public static string VirtualMachineDiskAlreadyExistsException(object arg0) => MachineManagementResources.Format(nameof (VirtualMachineDiskAlreadyExistsException), arg0);

    public static string VirtualMachineDiskAlreadyExistsException(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (VirtualMachineDiskAlreadyExistsException), culture, arg0);

    public static string NoDefaultImageExistsException() => MachineManagementResources.Get(nameof (NoDefaultImageExistsException));

    public static string NoDefaultImageExistsException(CultureInfo culture) => MachineManagementResources.Get(nameof (NoDefaultImageExistsException), culture);

    public static string ProcessRequestAccessDeniedException() => MachineManagementResources.Get(nameof (ProcessRequestAccessDeniedException));

    public static string ProcessRequestAccessDeniedException(CultureInfo culture) => MachineManagementResources.Get(nameof (ProcessRequestAccessDeniedException), culture);

    public static string QueueMachineRequestAccessDeniedException() => MachineManagementResources.Get(nameof (QueueMachineRequestAccessDeniedException));

    public static string QueueMachineRequestAccessDeniedException(CultureInfo culture) => MachineManagementResources.Get(nameof (QueueMachineRequestAccessDeniedException), culture);

    public static string RegisterMachineAccessDeniedException() => MachineManagementResources.Get(nameof (RegisterMachineAccessDeniedException));

    public static string RegisterMachineAccessDeniedException(CultureInfo culture) => MachineManagementResources.Get(nameof (RegisterMachineAccessDeniedException), culture);

    public static string ImageLabelExistsException(object arg0) => MachineManagementResources.Format(nameof (ImageLabelExistsException), arg0);

    public static string ImageLabelExistsException(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (ImageLabelExistsException), culture, arg0);

    public static string ImageLabelDoesNotExistException(object arg0) => MachineManagementResources.Format(nameof (ImageLabelDoesNotExistException), arg0);

    public static string ImageLabelDoesNotExistException(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (ImageLabelDoesNotExistException), culture, arg0);

    public static string MachineRequestResourceDoesNotExistException(object arg0, object arg1) => MachineManagementResources.Format(nameof (MachineRequestResourceDoesNotExistException), arg0, arg1);

    public static string MachineRequestResourceDoesNotExistException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return MachineManagementResources.Format(nameof (MachineRequestResourceDoesNotExistException), culture, arg0, arg1);
    }

    public static string UpdateMachineInstanceAccessDeniedException() => MachineManagementResources.Get(nameof (UpdateMachineInstanceAccessDeniedException));

    public static string UpdateMachineInstanceAccessDeniedException(CultureInfo culture) => MachineManagementResources.Get(nameof (UpdateMachineInstanceAccessDeniedException), culture);

    public static string ManagePoolResourcesAccessDeniedException() => MachineManagementResources.Get(nameof (ManagePoolResourcesAccessDeniedException));

    public static string ManagePoolResourcesAccessDeniedException(CultureInfo culture) => MachineManagementResources.Get(nameof (ManagePoolResourcesAccessDeniedException), culture);

    public static string MessageIdAlreadyExistsException(object arg0) => MachineManagementResources.Format(nameof (MessageIdAlreadyExistsException), arg0);

    public static string MessageIdAlreadyExistsException(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (MessageIdAlreadyExistsException), culture, arg0);

    public static string CannotDeleteAndAddMetadataException() => MachineManagementResources.Get(nameof (CannotDeleteAndAddMetadataException));

    public static string CannotDeleteAndAddMetadataException(CultureInfo culture) => MachineManagementResources.Get(nameof (CannotDeleteAndAddMetadataException), culture);

    public static string MustAddBothCurrentAndPreviousImageVersionException() => MachineManagementResources.Get(nameof (MustAddBothCurrentAndPreviousImageVersionException));

    public static string MustAddBothCurrentAndPreviousImageVersionException(CultureInfo culture) => MachineManagementResources.Get(nameof (MustAddBothCurrentAndPreviousImageVersionException), culture);

    public static string MachineRequestTagNotFoundException(object arg0, object arg1) => MachineManagementResources.Format(nameof (MachineRequestTagNotFoundException), arg0, arg1);

    public static string MachineRequestTagNotFoundException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return MachineManagementResources.Format(nameof (MachineRequestTagNotFoundException), culture, arg0, arg1);
    }

    public static string NoPoolsFoundToRouteImageLabel(object arg0) => MachineManagementResources.Format(nameof (NoPoolsFoundToRouteImageLabel), arg0);

    public static string NoPoolsFoundToRouteImageLabel(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (NoPoolsFoundToRouteImageLabel), culture, arg0);

    public static string MachineRequestQueuePositionMessage() => MachineManagementResources.Get(nameof (MachineRequestQueuePositionMessage));

    public static string MachineRequestQueuePositionMessage(CultureInfo culture) => MachineManagementResources.Get(nameof (MachineRequestQueuePositionMessage), culture);

    public static string MachineRequestQueuePositionUnavailable() => MachineManagementResources.Get(nameof (MachineRequestQueuePositionUnavailable));

    public static string MachineRequestQueuePositionUnavailable(CultureInfo culture) => MachineManagementResources.Get(nameof (MachineRequestQueuePositionUnavailable), culture);

    public static string FinishRequestAccessDeniedException() => MachineManagementResources.Get(nameof (FinishRequestAccessDeniedException));

    public static string FinishRequestAccessDeniedException(CultureInfo culture) => MachineManagementResources.Get(nameof (FinishRequestAccessDeniedException), culture);

    public static string TooManyRequestsException(object arg0) => MachineManagementResources.Format(nameof (TooManyRequestsException), arg0);

    public static string TooManyRequestsException(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (TooManyRequestsException), culture, arg0);

    public static string AgentSpecAlreadyExists(object arg0) => MachineManagementResources.Format(nameof (AgentSpecAlreadyExists), arg0);

    public static string AgentSpecAlreadyExists(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (AgentSpecAlreadyExists), culture, arg0);

    public static string AgentSpecDoesNotExist(object arg0) => MachineManagementResources.Format(nameof (AgentSpecDoesNotExist), arg0);

    public static string AgentSpecDoesNotExist(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (AgentSpecDoesNotExist), culture, arg0);

    public static string ImageLabelEmptyException() => MachineManagementResources.Get(nameof (ImageLabelEmptyException));

    public static string ImageLabelEmptyException(CultureInfo culture) => MachineManagementResources.Get(nameof (ImageLabelEmptyException), culture);

    public static string NoSupportedImageLabelsFoundForNestedPoolException(object arg0) => MachineManagementResources.Format(nameof (NoSupportedImageLabelsFoundForNestedPoolException), arg0);

    public static string NoSupportedImageLabelsFoundForNestedPoolException(
      object arg0,
      CultureInfo culture)
    {
      return MachineManagementResources.Format(nameof (NoSupportedImageLabelsFoundForNestedPoolException), culture, arg0);
    }

    public static string MachineRequestOutcome(object arg0) => MachineManagementResources.Format(nameof (MachineRequestOutcome), arg0);

    public static string MachineRequestOutcome(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (MachineRequestOutcome), culture, arg0);

    public static string FailedToProvisionResources(object arg0) => MachineManagementResources.Format(nameof (FailedToProvisionResources), arg0);

    public static string FailedToProvisionResources(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (FailedToProvisionResources), culture, arg0);

    public static string FinishArbitraryRequestAccessDeniedException() => MachineManagementResources.Get(nameof (FinishArbitraryRequestAccessDeniedException));

    public static string FinishArbitraryRequestAccessDeniedException(CultureInfo culture) => MachineManagementResources.Get(nameof (FinishArbitraryRequestAccessDeniedException), culture);

    public static string TrustedHostShutDownException(object arg0) => MachineManagementResources.Format(nameof (TrustedHostShutDownException), arg0);

    public static string TrustedHostShutDownException(object arg0, CultureInfo culture) => MachineManagementResources.Format(nameof (TrustedHostShutDownException), culture, arg0);
  }
}
