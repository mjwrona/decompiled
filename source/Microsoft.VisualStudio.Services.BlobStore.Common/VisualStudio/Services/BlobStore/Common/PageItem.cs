// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.PageItem
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public struct PageItem
  {
    public readonly string Path;
    public readonly PageItemType Type;

    public PageItem(string path, PageItemType type)
    {
      this.Path = path;
      this.Type = type;
    }
  }
}
