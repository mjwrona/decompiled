// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.TransportAddressUri
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Documents.Rntbd;
using System;

namespace Microsoft.Azure.Documents
{
  internal sealed class TransportAddressUri : IEquatable<TransportAddressUri>
  {
    private static readonly TimeSpan oneMinute = TimeSpan.FromMinutes(1.0);
    private readonly string uriToString;
    private DateTime? lastFailedRequestUtc;

    public TransportAddressUri(Uri addressUri)
    {
      this.Uri = !(addressUri == (Uri) null) ? addressUri : throw new ArgumentNullException(nameof (addressUri));
      this.uriToString = addressUri.ToString();
      this.PathAndQuery = addressUri.PathAndQuery.TrimEnd(TransportSerialization.UrlTrim);
    }

    public Uri Uri { get; }

    public string PathAndQuery { get; }

    public bool IsUnhealthy()
    {
      DateTime? failedRequestUtc = this.lastFailedRequestUtc;
      if (!failedRequestUtc.HasValue || !failedRequestUtc.HasValue)
        return false;
      if (failedRequestUtc.Value + TransportAddressUri.oneMinute > DateTime.UtcNow)
        return true;
      this.lastFailedRequestUtc = new DateTime?();
      return false;
    }

    public void SetUnhealthy() => this.lastFailedRequestUtc = new DateTime?(DateTime.UtcNow);

    public override int GetHashCode() => this.Uri.GetHashCode();

    public override string ToString() => this.uriToString;

    public bool Equals(TransportAddressUri other) => this == other || this.Uri.Equals((object) other?.Uri);

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      return obj is TransportAddressUri other && this.Equals(other);
    }
  }
}
