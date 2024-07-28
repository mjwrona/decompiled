// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Channels.ITfsRequestListener
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;
using System.Net;

namespace Microsoft.TeamFoundation.Client.Channels
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface ITfsRequestListener
  {
    void AfterReceiveReply(long requestId, string methodName, HttpWebResponse response);

    void BeforeSendRequest(long requestId, string methodName, HttpWebRequest request);

    long BeginRequest();

    void EndRequest(long requestId, Exception exception);
  }
}
