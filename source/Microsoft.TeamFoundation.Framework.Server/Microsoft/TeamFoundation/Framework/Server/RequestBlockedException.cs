// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RequestBlockedException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Net;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class RequestBlockedException : RequestFilterException
  {
    public string MessageHtml { get; }

    public RequestBlockedException(string resourceName, string friendlyNamespaceName, int linkId)
      : base(FrameworkResources.RequestBlockedWithFwlink((object) resourceName, (object) friendlyNamespaceName, (object) linkId.ToString()), (HttpStatusCode) 429)
    {
    }

    public RequestBlockedException(string message, string messageHtml)
      : base(message, (HttpStatusCode) 429)
    {
      this.MessageHtml = messageHtml;
    }
  }
}
