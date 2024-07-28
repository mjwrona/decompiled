// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CrossOriginEntry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class CrossOriginEntry
  {
    public string Host { get; set; }

    public CrossOriginEntryOptions AllowedOptions { get; set; }

    public bool AllowSubdomains { get; set; }

    public int? Port { get; set; }

    public string Scheme { get; set; }

    public virtual bool IsAllowed(Uri uri, CrossOriginEntryOptions feature) => this.AllowedOptions.HasFlag((Enum) feature) && (!this.Port.HasValue || uri.Port == this.Port.Value) && (string.IsNullOrEmpty(this.Scheme) || string.Equals(this.Scheme, uri.Scheme, StringComparison.OrdinalIgnoreCase)) && (string.Equals(this.Host, uri.Host, StringComparison.OrdinalIgnoreCase) || this.AllowSubdomains && uri.Host.EndsWith("." + this.Host, StringComparison.OrdinalIgnoreCase));
  }
}
