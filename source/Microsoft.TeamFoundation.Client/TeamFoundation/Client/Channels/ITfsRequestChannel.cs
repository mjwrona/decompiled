// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Channels.ITfsRequestChannel
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.TeamFoundation.Client.Channels
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface ITfsRequestChannel
  {
    Uri Uri { get; }

    VssCredentials Credentials { get; }

    CultureInfo Culture { get; }

    Guid SessionId { get; }

    TfsRequestSettings Settings { get; }

    TfsHttpClientState State { get; }

    void Abort();

    IAsyncResult BeginRequest(TfsMessage message, AsyncCallback callback, object state);

    IAsyncResult BeginRequest(
      TfsMessage message,
      TimeSpan timeout,
      AsyncCallback callback,
      object state);

    TfsMessage EndRequest(IAsyncResult result);

    TfsMessage Request(TfsMessage message);

    TfsMessage Request(TfsMessage message, TimeSpan timeout);
  }
}
