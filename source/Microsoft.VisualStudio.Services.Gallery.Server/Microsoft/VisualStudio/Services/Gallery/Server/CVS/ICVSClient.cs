// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CVS.ICVSClient
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.Ops.Cvs.Client.DataContracts;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server.CVS
{
  internal interface ICVSClient
  {
    void Unload(IVssRequestContext requestContext);

    CVSScanResponse GetScanStatus(IVssRequestContext requestContext, string jobId);

    CVSScanResponse SubmitScan(
      IVssRequestContext requestContext,
      ContentType contentType,
      string path,
      string callbackURL,
      string externalId,
      IdentityPuid publisherPuid,
      DateTime? PublishedTime,
      bool isSynchronous);
  }
}
