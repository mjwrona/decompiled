// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.BrowserNotSupportedException
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  public class BrowserNotSupportedException : TeamFoundationServiceException
  {
    public BrowserNotSupportedException()
      : this(WebFrameworkResources.NotSupportedBrowser())
    {
    }

    public BrowserNotSupportedException(string message)
      : base(message)
    {
    }

    public BrowserNotSupportedException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public BrowserNotSupportedException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public BrowserNotSupportedException(HttpStatusCode httpStatusCode)
      : base(WebFrameworkResources.NotSupportedBrowser(), httpStatusCode)
    {
    }

    public string DetailedMessage { get; set; }
  }
}
