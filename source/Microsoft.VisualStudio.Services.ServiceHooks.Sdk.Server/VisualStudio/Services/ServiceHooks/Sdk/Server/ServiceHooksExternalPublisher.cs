// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.ServiceHooksExternalPublisher
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  public abstract class ServiceHooksExternalPublisher : ServiceHooksPublisher
  {
    public abstract void ValidateIncomingEventRequest(
      IVssRequestContext requestContext,
      HttpRequestMessage request,
      byte[] payload);

    public abstract string GetEventTypeFromRequest(HttpRequestMessage request, object payload);

    public abstract string GetChannelFromRequest(HttpRequestMessage request, object payload);

    protected internal static bool TryGetChannelFromUrl(string url, out string channelId)
    {
      string[] source = new string[2]
      {
        "channelId=",
        "channelId%3D"
      };
      channelId = ((IEnumerable<string>) source).Select<string, string>(new Func<string, string>(GetChannelIdWithLabel)).FirstOrDefault<string>((Func<string, bool>) (id => id != null));
      return channelId != null;

      string GetChannelIdWithLabel(string label)
      {
        int num = url.IndexOf(label, StringComparison.OrdinalIgnoreCase);
        if (num < 0 || num + label.Length + 36 > url.Length)
          return (string) null;
        string input = url.Substring(num + label.Length, 36);
        return !Guid.TryParse(input, out Guid _) ? (string) null : input;
      }
    }
  }
}
