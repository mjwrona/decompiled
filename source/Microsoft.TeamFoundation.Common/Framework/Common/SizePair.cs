// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.SizePair
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

namespace Microsoft.TeamFoundation.Framework.Common
{
  public struct SizePair
  {
    public SizePair(long previousItemSize, long newItemSize)
    {
      this.PreviousItemSize = previousItemSize;
      this.NewItemSize = newItemSize;
    }

    public long PreviousItemSize { get; }

    public long NewItemSize { get; }
  }
}
