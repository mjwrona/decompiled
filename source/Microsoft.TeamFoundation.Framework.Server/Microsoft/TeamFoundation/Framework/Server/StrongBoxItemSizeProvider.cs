// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StrongBoxItemSizeProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class StrongBoxItemSizeProvider : ISizeProvider<StrongBoxItemName, ItemCacheEntry>
  {
    private static int s_certificateSize;

    public long GetSize(StrongBoxItemName key, ItemCacheEntry value)
    {
      if (StrongBoxItemSizeProvider.s_certificateSize == 0 && (value.NonExportableCertificate ?? value.ExportableCertificate) != null)
        StrongBoxItemSizeProvider.s_certificateSize = (value.NonExportableCertificate ?? value.ExportableCertificate).RawData.Length * 2;
      return (value.ProtectedContent != null ? (long) value.ProtectedContent.Length : 0L) + (long) StrongBoxItemSizeProvider.s_certificateSize;
    }
  }
}
