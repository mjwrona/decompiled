// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Tracing
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.IdentityPicker
{
  public static class Tracing
  {
    public const string Area = "VSS.IdentityPicker";
    public const string Layer = "Framework";
    internal const int BaseTraceOffset = 10030000;
    internal const int IdentityStart = 0;
    internal const int Identity_FactoryCreate_Exception = 8;
    internal const int OperationStart = 100;
    internal const int IdentityOperation_Validate_Enter = 701;
    internal const int IdentityOperation_Validate_Exception = 708;
    internal const int IdentityOperation_Validate_Leave = 709;
    internal const int IdentityOperation_Process_Enter = 711;
    internal const int IdentityOperation_Process_Exception = 718;
    internal const int IdentityOperation_Process_Leave = 719;
    internal const int AbstractSearchRequest_Validate_Enter = 101;
    internal const int AbstractSearchRequest_Validate_Exception = 108;
    internal const int AbstractSearchRequest_Validate_Leave = 109;
    internal const int SearchRequest_Validate_Enter = 111;
    internal const int SearchRequest_Validate_Exception = 118;
    internal const int SearchRequest_Validate_Leave = 119;
    internal const int SearchRequest_Process_Enter = 121;
    internal const int SearchRequest_ParseTokens_Exception = 125;
    internal const int SearchRequest_Process_Exception = 128;
    internal const int SearchRequest_Process_Leave = 129;
    internal const int GetAvatarRequest_Validate_Enter = 131;
    internal const int GetAvatarRequest_Validate_Exception = 138;
    internal const int GetAvatarRequest_Validate_Leave = 139;
    internal const int GetAvatarRequest_Process_Enter = 141;
    internal const int GetAvatarRequest_Process_Exception = 148;
    internal const int GetAvatarRequest_Process_Leave = 149;
    internal const int GetMruRequest_Validate_Enter = 151;
    internal const int GetMruRequest_Validate_Exception = 158;
    internal const int GetMruRequest_Validate_Leave = 159;
    internal const int GetMruRequest_Process_Enter = 161;
    internal const int GetMruRequest_Process_Exception = 168;
    internal const int GetMruRequest_Process_Leave = 169;
    internal const int PatchMruRequest_Validate_Enter = 171;
    internal const int PatchMruRequest_Validate_Exception = 178;
    internal const int PatchMruRequest_Validate_Leave = 179;
    internal const int PatchMruRequest_Process_Enter = 181;
    internal const int PatchMruRequest_Process_Exception = 188;
    internal const int PatchMruRequest_Process_Leave = 189;
    internal const int GetConnectionsRequest_Validate_Enter = 191;
    internal const int GetConnectionsRequest_Validate_Exception = 198;
    internal const int GetConnectionsRequest_Validate_Leave = 199;
    internal const int GetConnectionsRequest_Process_Enter = 201;
    internal const int GetConnectionsRequest_Process_Exception = 208;
    internal const int GetConnectionsRequest_Process_Leave = 209;
    internal const int DdsAdapter_GetIdentitiesEmails_Enter = 501;
    internal const int DdsAdapter_GetIdentitiesEmails_ExceptionAggregate = 507;
    internal const int DdsAdapter_GetIdentitiesEmails_Exception = 508;
    internal const int DdsAdapter_GetIdentitiesEmails_Leave = 509;
    internal const int DdsAdapter_GetIdentitiesPrefix_Enter = 511;
    internal const int DdsAdapter_GetIdentitiesPrefix_ExceptionAggregate = 517;
    internal const int DdsAdapter_GetIdentitiesPrefix_Exception = 518;
    internal const int DdsAdapter_GetIdentitiesPrefix_Leave = 519;
    internal const int DdsAdapter_GetIdentityImage_Enter = 521;
    internal const int IdentityPicker_ImageRetrieval_Exception = 527;
    internal const int IdentityPicker_ImageNotAvailable_Exception = 528;
    internal const int DdsAdapter_GetIdentityImage_Leave = 529;
    internal const int MruAdapter_GetIdentitiesMru_Enter = 531;
    internal const int MruAdapter_GetIdentitiesMru_Exception = 538;
    internal const int MruAdapter_GetIdentitiesMru_Leave = 539;
    internal const int MruAdapter_AddIdentitiesMru_Enter = 541;
    internal const int MruAdapter_AddIdentitiesMru_Exception = 548;
    internal const int MruAdapter_AddIdentitiesMru_Leave = 549;
    internal const int MruAdapter_RemoveIdentitiesMru_Enter = 551;
    internal const int MruAdapter_RemoveIdentitiesMru_Exception = 558;
    internal const int MruAdapter_RemoveIdentitiesMru_Leave = 559;
    internal const int DdsAdapter_GetIdentitiesEntityIds_Enter = 561;
    internal const int DdsAdapter_GetIdentitiesEntityIds_ExceptionAggregate = 567;
    internal const int DdsAdapter_GetIdentitiesEntityIds_Exception = 568;
    internal const int DdsAdapter_GetIdentitiesEntityIds_Leave = 569;
    internal const int DdsAdapter_GetIdentitiesSamAccountNames_Enter = 621;
    internal const int DdsAdapter_GetIdentitiesSamAccountNames_ExceptionAggregate = 627;
    internal const int DdsAdapter_GetIdentitiesSamAccountNames_Exception = 628;
    internal const int DdsAdapter_GetIdentitiesSamAccountNames_Leave = 629;
    internal const int DdsAdapter_RequestAccountNotFound = 571;
    internal const int DdsAdapter_AccountNotAADBacked = 572;
    internal const int DdsAdapter_GuestUserEnabled = 573;
    internal const int DdsAdapter_GuestUserException = 574;
    internal const int IOperationRequest_Validate_Exception = 584;
    internal const int DdsAdapter_GetIdentitiesDirectoryIds_Enter = 591;
    internal const int DdsAdapter_GetIdentitiesDirectoryIds_ExceptionAggregate = 597;
    internal const int DdsAdapter_GetIdentitiesDirectoryIds_Exception = 598;
    internal const int DdsAdapter_GetIdentitiesDirectoryIds_Leave = 599;
    internal const int DdsAdapter_GetIdentities_Enter = 801;
    internal const int DdsAdapter_GetIdentities_ExceptionAggregate = 807;
    internal const int DdsAdapter_GetIdentities_Exception = 808;
    internal const int DdsAdapter_GetIdentities_Leave = 809;
    internal const int FrameworkIdentityPickerService_ServiceStart_Enter = 601;
    internal const int FrameworkIdentityPickerService_ServiceStart_Exception = 608;
    internal const int FrameworkIdentityPickerService_LoadExtensions_NullReturn_Info = 609;
    internal const int FrameworkIdentityPickerService_ServiceStart_Leave = 609;
    internal const int FrameworkIdentityPickerService_Invoke_Enter = 611;
    internal const int FrameworkIdentityPickerService_Invoke_Exception = 618;
    internal const int FrameworkIdentityPickerService_Invoke_Leave = 619;
    internal const int IdentityOperationHelper_MatchPrefix_Regex_Exception = 638;

    internal static void TraceException(
      IVssRequestContext requestContext,
      int traceOffset,
      Exception ex)
    {
      requestContext.TraceException(traceOffset + 10030000, "VSS.IdentityPicker", "Framework", ex);
    }

    internal static void TraceEnter(
      IVssRequestContext requestContext,
      int traceOffset,
      string message)
    {
      requestContext.TraceEnter(traceOffset + 10030000, "VSS.IdentityPicker", "Framework", message);
    }

    internal static void TraceLeave(
      IVssRequestContext requestContext,
      int traceOffset,
      string message)
    {
      requestContext.TraceEnter(traceOffset + 10030000, "VSS.IdentityPicker", "Framework", message);
    }

    internal static void TraceError(
      IVssRequestContext requestContext,
      int traceOffset,
      string message)
    {
      requestContext.Trace(traceOffset + 10030000, TraceLevel.Error, "VSS.IdentityPicker", "Framework", message);
    }

    internal static void TraceInfo(
      IVssRequestContext requestContext,
      int traceOffset,
      string format,
      params object[] args)
    {
      VssRequestContextExtensions.Trace(requestContext, traceOffset + 10030000, TraceLevel.Info, "VSS.IdentityPicker", "Framework", format, args);
    }
  }
}
