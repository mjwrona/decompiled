// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CorrelationKey
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class CorrelationKey : IEquatable<CorrelationKey>
  {
    public readonly string UrlHost;
    public readonly string ServiceHost;

    public CorrelationKey(string urlHost, string serviceHost)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(urlHost, nameof (urlHost));
      ArgumentUtility.CheckStringForNullOrEmpty(serviceHost, nameof (serviceHost));
      this.UrlHost = urlHost;
      this.ServiceHost = serviceHost;
    }

    public override bool Equals(object obj) => this.Equals(obj as CorrelationKey);

    public bool Equals(CorrelationKey other) => other != null && string.Equals(this.UrlHost, other.UrlHost, StringComparison.OrdinalIgnoreCase) && string.Equals(this.ServiceHost, other.ServiceHost, StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode() => this.UrlHost.GetHashCode() ^ this.ServiceHost.GetHashCode();
  }
}
