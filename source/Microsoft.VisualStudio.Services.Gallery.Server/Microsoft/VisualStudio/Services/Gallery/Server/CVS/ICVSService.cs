// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CVS.ICVSService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.Ops.Cvs.Client.DataContracts;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server.CVS
{
  [DefaultServiceImplementation(typeof (CVSService))]
  internal interface ICVSService : IVssFrameworkService
  {
    ScanItem SubmitStreamForScan(
      IVssRequestContext requestContext,
      Guid scanId,
      Stream stream,
      string itemDescription,
      ContentType cvsContentType,
      string externalId,
      IdentityPuid publisherPuid,
      DateTime? updateTime,
      bool isSynchronous);

    ScanItem SubmitItemForScan(
      IVssRequestContext requestContext,
      Guid scanId,
      Guid itemId,
      string itemDescription,
      ContentType cvsContentType,
      string externalId,
      IdentityPuid publisherPuid,
      DateTime? updateTime,
      bool isSynchronous);

    CVSScanResponse GetItemScanStatus(IVssRequestContext requestContext, string jobId);

    ScanItem UpdateItemScanStatus(
      IVssRequestContext requestContext,
      Guid scanId,
      Guid itemId,
      CVSScanResponse response);

    IEnumerable<ScanItem> GetAllScanItems(IVssRequestContext requestContext, Guid scanId);

    void DeleteScanItemsByScanId(IVssRequestContext requestContext, Guid scanId);

    bool IsScanTimeOut(IVssRequestContext requestContext, DateTime startTime);

    TimeSpan GetRecheckTimespan(IVssRequestContext requestContext);

    long GetRetryCount();

    DateTime GetViolationTimeOutDate(DateTime contactDate);

    bool IsCallbackEnabled();

    IEnumerable<ScanViolationItem> GetScanViolations(
      IVssRequestContext requestContext,
      IEnumerable<Guid> extensionIds = null);

    int DeleteScanViolations(IVssRequestContext requestContext, IEnumerable<Guid> extensionIds = null);

    void UpdateScanViolation(IVssRequestContext requestContext, ScanViolationItem scanViolation);

    bool IsViolationNotifiableToMarketplace(IVssRequestContext requestContext, DateTime contactTime);
  }
}
