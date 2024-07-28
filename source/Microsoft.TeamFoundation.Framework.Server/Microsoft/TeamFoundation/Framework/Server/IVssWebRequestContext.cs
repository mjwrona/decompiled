// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IVssWebRequestContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public interface IVssWebRequestContext : IVssRequestContext, IDisposable
  {
    string AuthenticationType { get; }

    string Command { get; }

    string HttpMethod { get; }

    string RawUrl { get; }

    string RemotePort { get; }

    string RemoteIPAddress { get; }

    string RemoteComputer { get; }

    Uri RequestUri { get; }

    IUrlTracer RequestUriForTracing { get; }

    string UniqueAgentIdentifier { get; }

    string RequestPath { get; }

    string RelativePath { get; }

    string RelativeUrl { get; }

    string VirtualPath { get; }

    string WebApplicationPath { get; }

    bool GetSessionValue(string sessionKey, out string sessionValue);

    bool SetSessionValue(string sessionKey, string sessionValue);

    void PartialResultsReady();
  }
}
