// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.ProtocolResolver
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  public class ProtocolResolver
  {
    private const string ProtocolQueryParameter = "clientProtocol";
    private readonly Version _minSupportedProtocol;
    private readonly Version _maxSupportedProtocol;
    private readonly Version _minimumDelayedStartVersion = new Version(1, 4);

    public ProtocolResolver()
      : this(new Version(1, 2), new Version(2, 0))
    {
    }

    public ProtocolResolver(Version min, Version max)
    {
      this._minSupportedProtocol = min;
      this._maxSupportedProtocol = max;
    }

    public Version Resolve(IRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      Version result;
      if (Version.TryParse(request.QueryString["clientProtocol"], out result))
      {
        if (result > this._maxSupportedProtocol)
          result = this._maxSupportedProtocol;
        else if (result < this._minSupportedProtocol)
          result = this._minSupportedProtocol;
      }
      Version version = result;
      return (object) version != null ? version : this._minSupportedProtocol;
    }

    public bool SupportsDelayedStart(IRequest request) => this.Resolve(request) >= this._minimumDelayedStartVersion;
  }
}
