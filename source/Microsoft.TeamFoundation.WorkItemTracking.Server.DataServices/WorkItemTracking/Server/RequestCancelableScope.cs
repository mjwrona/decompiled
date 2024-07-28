// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.RequestCancelableScope
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class RequestCancelableScope : IDisposable
  {
    private Guid m_requestId;
    private bool m_disposed;
    private static readonly Dictionary<Guid, IVssRequestContext> s_contexts = new Dictionary<Guid, IVssRequestContext>();

    public RequestCancelableScope(Guid requestId, IVssRequestContext tfsRequestContext)
    {
      ArgumentUtility.CheckForEmptyGuid(requestId, nameof (requestId));
      this.m_requestId = requestId;
      lock (RequestCancelableScope.s_contexts)
      {
        if (RequestCancelableScope.s_contexts.ContainsKey(this.m_requestId))
          throw new LegacyValidationException(ResourceStrings.Get("RequestAlreadyActive"), 600171);
        RequestCancelableScope.s_contexts.Add(this.m_requestId, tfsRequestContext);
      }
    }

    public static IVssRequestContext GetContext(string requestId)
    {
      Guid key = RequestCancelableScope.ValidateRequestId(requestId);
      IVssRequestContext context = (IVssRequestContext) null;
      lock (RequestCancelableScope.s_contexts)
        RequestCancelableScope.s_contexts.TryGetValue(key, out context);
      return context;
    }

    public static bool TryParseRequestId(string requestId, out Guid requestIdGuid)
    {
      requestIdGuid = new Guid();
      if (requestId == null)
        return false;
      string str = requestId.Trim();
      int length = "uuid:".Length;
      return str.Length > length && !(str.Substring(0, length) != "uuid:") && Guid.TryParse(str.Substring(length), out requestIdGuid) && requestIdGuid != Guid.Empty;
    }

    private static Guid ValidateRequestId(string requestId)
    {
      Guid requestIdGuid;
      if (!RequestCancelableScope.TryParseRequestId(requestId, out requestIdGuid))
        throw new SoapException(ResourceStrings.Get("InvalidRequestId"), Soap12FaultCodes.SenderFaultCode, new SoapFaultSubCode(Soap12FaultCodes.RpcBadArgumentsFaultCode, WorkItemTrackingFaultCodes.InvalidRequestId));
      return requestIdGuid;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (this.m_disposed)
        return;
      lock (RequestCancelableScope.s_contexts)
        RequestCancelableScope.s_contexts.Remove(this.m_requestId);
      this.m_disposed = true;
    }
  }
}
