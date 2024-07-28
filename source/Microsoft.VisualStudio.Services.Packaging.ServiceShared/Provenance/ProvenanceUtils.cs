// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance.ProvenanceUtils
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.WebApi.Exceptions;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance
{
  public static class ProvenanceUtils
  {
    public static string GetFeedId(
      IVssRequestContext requestContext,
      string feedId,
      string protocol)
    {
      requestContext.TraceEnter(20999999, protocol, nameof (ProvenanceUtils), nameof (GetFeedId));
      try
      {
        SessionId parsedSessionId;
        if (SessionId.TryParse(feedId, out parsedSessionId))
        {
          SessionKey sessionKey = new SessionKey(parsedSessionId, protocol);
          feedId = requestContext.GetService<ISessionMetadataService>().GetSessionMetadata(requestContext, sessionKey)?.Feed;
          if (feedId == null)
            throw new ProvenanceSessionNotFoundException(string.Format("Could not find {0} data for provenance session {1}", (object) sessionKey.Protocol, (object) sessionKey.SessionId));
          requestContext.Items["Packaging.Provenance.Session"] = (object) sessionKey;
          requestContext.AddPackagingTracesProperty("provenanceSession", (object) parsedSessionId);
        }
        return feedId;
      }
      finally
      {
        requestContext.TraceLeave(20999999, protocol, nameof (ProvenanceUtils), nameof (GetFeedId));
      }
    }

    public static Microsoft.VisualStudio.Services.Feed.WebApi.Provenance GetFeedProvenance(
      ProvenanceInfo provenanceInfo,
      Guid userId)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Provenance feedProvenance = new Microsoft.VisualStudio.Services.Feed.WebApi.Provenance()
      {
        PublisherUserIdentity = userId
      };
      if (provenanceInfo != null)
      {
        feedProvenance.UserAgent = provenanceInfo.UserAgent;
        feedProvenance.ProvenanceSource = provenanceInfo.ProvenanceSource;
        feedProvenance.Data = provenanceInfo.Data ?? (IDictionary<string, string>) new Dictionary<string, string>();
      }
      return feedProvenance;
    }

    public static bool TryGetSessionId(IVssRequestContext requestContext, out SessionId sessionId)
    {
      object obj;
      if (requestContext.Items.TryGetValue("Packaging.Provenance.Session", out obj) && obj is SessionKey sessionKey)
      {
        sessionId = sessionKey.SessionId;
        return true;
      }
      sessionId = SessionId.Empty;
      return false;
    }
  }
}
