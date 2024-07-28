// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.FileBlobDescriptorConstants
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using System.IO;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public static class FileBlobDescriptorConstants
  {
    public const long MaxFileSizeFileDedup = 107374182400;
    public const char PathIdentifierSeperator = '?';
    public static readonly string EmptyDirectoryEndingPattern = string.Format("{0}.", (object) Path.DirectorySeparatorChar);
    public const string EmptyDirectoryUriEndingPattern = "/.";
    public static readonly BlobIdentifier EmptyDirectoryChunkBlobIdentifier = ChunkBlobHasher.Instance.OfNothing;
  }
}
