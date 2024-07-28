// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ItemCacheEntry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public struct ItemCacheEntry : IEquatable<ItemCacheEntry>
  {
    internal StrongBoxItemInfo Item;
    internal byte[] ProtectedContent;
    internal X509Certificate2 NonExportableCertificate;
    internal X509Certificate2 ExportableCertificate;

    public bool Equals(ItemCacheEntry other)
    {
      if (this.Item != other.Item && (this.Item == null || other.Item == null || !this.Item.Equals(other.Item)) || this.ProtectedContent != other.ProtectedContent && (this.ProtectedContent == null || other.ProtectedContent == null || !((IEnumerable<byte>) this.ProtectedContent).SequenceEqual<byte>((IEnumerable<byte>) other.ProtectedContent)) || this.ExportableCertificate != other.ExportableCertificate && (this.ExportableCertificate == null || other.ExportableCertificate == null || !this.ExportableCertificate.Equals((X509Certificate) other.ExportableCertificate)))
        return false;
      if (this.NonExportableCertificate == other.NonExportableCertificate)
        return true;
      return this.NonExportableCertificate != null && other.NonExportableCertificate != null && this.NonExportableCertificate.Equals((X509Certificate) other.NonExportableCertificate);
    }
  }
}
