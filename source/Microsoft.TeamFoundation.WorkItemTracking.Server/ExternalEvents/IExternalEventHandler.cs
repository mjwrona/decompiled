// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalEvents.IExternalEventHandler
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalEvents
{
  [InheritedExport]
  public interface IExternalEventHandler
  {
    (bool SupportedEvent, bool SupportedHostType) SupportsEvent(
      IVssRequestContext requestContext,
      IDictionary<string, string> headers,
      string json);

    bool ValidateEvent(
      IVssRequestContext requestContext,
      IDictionary<string, string> headers,
      string json,
      EventSource eventSource);

    void HandleEvent(
      IVssRequestContext requestContext,
      IDictionary<string, string> headers,
      string json);
  }
}
