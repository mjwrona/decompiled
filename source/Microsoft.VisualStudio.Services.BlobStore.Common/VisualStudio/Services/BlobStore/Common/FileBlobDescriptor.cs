// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.FileBlobDescriptor
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [CLSCompliant(false)]
  public sealed class FileBlobDescriptor : 
    IDropFile,
    IComparable<FileBlobDescriptor>,
    IEquatable<FileBlobDescriptor>
  {
    private static readonly DedupNode precomputedEmptyDirectoryDedupNode;
    private static readonly List<string> emptyNetworkPaths;
    public readonly IFileSystem FileSystem;
    public readonly string RootDirectory;
    public readonly string AbsolutePath;
    public readonly bool BlobIdentifierIsNodeOrChunk;
    public readonly List<string> NetworkPaths;
    public string SymbolicLink;
    public uint PermissionValue;

    static FileBlobDescriptor()
    {
      using (MemoryStream content = new MemoryStream())
        FileBlobDescriptor.precomputedEmptyDirectoryDedupNode = ChunkerHelper.CreateFromStreamAsync((Stream) content, CancellationToken.None, false).SyncResult<DedupNode>();
      FileBlobDescriptor.emptyNetworkPaths = new List<string>();
    }

    public static Task<FileBlobDescriptor> CalculateAsync(
      string rootDirectory,
      bool chunkDedup,
      string relativePath,
      FileBlobType fileBlobType,
      CancellationToken cancellationToken)
    {
      return FileBlobDescriptor.CalculateAsync((IFileSystem) Microsoft.VisualStudio.Services.Content.Common.FileSystem.Instance, rootDirectory, chunkDedup, relativePath, fileBlobType, cancellationToken);
    }

    public static async Task<FileBlobDescriptor> CalculateAsync(
      IFileSystem fileSystem,
      string rootDirectory,
      bool chunkDedup,
      string relativePath,
      FileBlobType fileBlobType,
      CancellationToken cancellationToken)
    {
      return await FileBlobDescriptor.CalculateAsync(fileSystem, rootDirectory, chunkDedup, relativePath, fileBlobType, false, false, cancellationToken);
    }

    public static async Task<FileBlobDescriptor> CalculateAsync(
      IFileSystem fileSystem,
      string rootDirectory,
      bool chunkDedup,
      string relativePath,
      FileBlobType fileBlobType,
      bool shouldPreserveSymbolicLink,
      bool shouldPreservePermissionValue,
      CancellationToken cancellationToken)
    {
      return await FileBlobDescriptor.CalculateAsync(fileSystem, rootDirectory, chunkDedup ? ChunkerHelper.DefaultChunkHashType : HashType.Vso0, relativePath, fileBlobType, shouldPreserveSymbolicLink, shouldPreservePermissionValue, cancellationToken);
    }

    public static Task<FileBlobDescriptor> CalculateAsync(
      string rootDirectory,
      HashType hashType,
      string relativePath,
      FileBlobType fileBlobType,
      CancellationToken cancellationToken)
    {
      return FileBlobDescriptor.CalculateAsync((IFileSystem) Microsoft.VisualStudio.Services.Content.Common.FileSystem.Instance, rootDirectory, hashType, relativePath, fileBlobType, cancellationToken);
    }

    public static async Task<FileBlobDescriptor> CalculateAsync(
      IFileSystem fileSystem,
      string rootDirectory,
      HashType hashType,
      string relativePath,
      FileBlobType fileBlobType,
      CancellationToken cancellationToken)
    {
      return await FileBlobDescriptor.CalculateAsync(fileSystem, rootDirectory, hashType, relativePath, fileBlobType, false, false, cancellationToken);
    }

    public static async Task<FileBlobDescriptor> CalculateAsync(
      IFileSystem fileSystem,
      string rootDirectory,
      HashType hashType,
      string relativePath,
      FileBlobType fileBlobType,
      bool shouldPreserveSymbolicLink,
      bool shouldPreservePermissionValue,
      CancellationToken cancellationToken)
    {
      string path;
      FileBlobDescriptor async;
      if (ChunkerHelper.IsHashTypeChunk(hashType))
      {
        if (fileBlobType == FileBlobType.EmptyDirectory)
        {
          async = new FileBlobDescriptor(FileBlobDescriptorConstants.EmptyDirectoryChunkBlobIdentifier, new DedupNode?(FileBlobDescriptor.precomputedEmptyDirectoryDedupNode), relativePath, new long?(0L), fileSystem, (string) null, rootDirectory, (List<string>) null);
        }
        else
        {
          path = Path.Combine(rootDirectory, relativePath);
          if ((!shouldPreserveSymbolicLink ? 0 : (OSUtilities.IsFileSymbolicLink(new FileInfo(path)) ? 1 : 0)) != 0)
          {
            async = new FileBlobDescriptor(FileBlobDescriptorConstants.EmptyDirectoryChunkBlobIdentifier, new DedupNode?(FileBlobDescriptor.precomputedEmptyDirectoryDedupNode), relativePath, new long?(0L), fileSystem, (string) null, rootDirectory, (List<string>) null);
            string symbolicLinkPath = OSUtilities.ReadSymbolicLink(path);
            if (!string.IsNullOrWhiteSpace(symbolicLinkPath))
              async.SymbolicLink = FileBlobDescriptor.IsSymbolicLinkTargetPathSubset(rootDirectory, symbolicLinkPath) ? symbolicLinkPath : throw new ArgumentException("SymbolicLinkTarget Path " + symbolicLinkPath + " is not a subset of " + rootDirectory);
          }
          else
          {
            DedupNode node = await ChunkerHelper.CreateFromFileAsync(fileSystem, path, false, hashType, cancellationToken).ConfigureAwait(false);
            ulong transitiveContentBytes = node.TransitiveContentBytes;
            async = new FileBlobDescriptor(node.GetDedupIdentifier().ToBlobIdentifier(), new DedupNode?(node), relativePath, new long?((long) transitiveContentBytes), fileSystem, (string) null, rootDirectory, (List<string>) null);
          }
          if (shouldPreservePermissionValue)
          {
            uint filePermissions = OSUtilities.GetFilePermissions(path);
            if (filePermissions != 0U)
              async.PermissionValue = filePermissions;
          }
          path = (string) null;
        }
      }
      else if (fileBlobType == FileBlobType.EmptyDirectory)
      {
        async = new FileBlobDescriptor(fileSystem, rootDirectory, relativePath, new long?(0L), VsoHash.OfNothing.BlobId, (List<string>) null);
      }
      else
      {
        path = Path.Combine(rootDirectory, relativePath);
        if ((!shouldPreserveSymbolicLink ? 0 : (OSUtilities.IsFileSymbolicLink(new FileInfo(path)) ? 1 : 0)) != 0)
        {
          async = new FileBlobDescriptor(fileSystem, rootDirectory, relativePath, new long?(0L), VsoHash.OfNothing.BlobId, (List<string>) null);
          string symbolicLinkPath = OSUtilities.ReadSymbolicLink(path);
          if (!string.IsNullOrWhiteSpace(symbolicLinkPath))
            async.SymbolicLink = FileBlobDescriptor.IsSymbolicLinkTargetPathSubset(rootDirectory, symbolicLinkPath) ? symbolicLinkPath : throw new ArgumentException("SymbolicLinkTarget Path " + symbolicLinkPath + " is not a subset of " + rootDirectory);
        }
        else
        {
          long fileSize;
          BlobIdentifier blobId;
          using (Stream stream = fileSystem.OpenStreamForFile(path, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete))
          {
            fileSize = stream.Length;
            if (fileSize >= 107374182400L)
              throw new InvalidOperationException(string.Format("File: {0} is {1} bytes and ", (object) relativePath, (object) fileSize) + string.Format("larger than or equal to the max allowed size of : {0} bytes. ", (object) 107374182400L) + "Consider using chunk dedup.");
            blobId = (await VsoHash.CalculateBlobIdentifierWithBlocksAsync(stream)).BlobId;
          }
          async = new FileBlobDescriptor(blobId, new DedupNode?(), relativePath, new long?(fileSize), fileSystem, (string) null, rootDirectory, (List<string>) null);
        }
        if (shouldPreservePermissionValue)
        {
          uint filePermissions = OSUtilities.GetFilePermissions(path);
          if (filePermissions != 0U)
            async.PermissionValue = filePermissions;
        }
        path = (string) null;
      }
      return async;
    }

    public static FileBlobDescriptor Deserialize(
      string rootDirectory,
      string serialized,
      bool lowercasePaths = false)
    {
      string[] strArray = serialized != null ? serialized.Split(new char[1]
      {
        '?'
      }, StringSplitOptions.RemoveEmptyEntries) : throw new ArgumentNullException(nameof (serialized));
      string str = strArray.Length >= 3 ? strArray[0] : throw new ArgumentException("Serialized value \"" + serialized + "\" didn't match expected syntax: [RelativePath]?[FileSizeBytes]?[BlobId]?[OptionalNetworkPaths]", nameof (serialized));
      if (lowercasePaths && !string.IsNullOrWhiteSpace(str))
        str = str.ToLowerInvariant();
      Microsoft.VisualStudio.Services.Content.Common.FileSystem instance = Microsoft.VisualStudio.Services.Content.Common.FileSystem.Instance;
      string rootDirectory1 = rootDirectory;
      string relativePath = str;
      long? fileSize = new long?(long.Parse(strArray[1]));
      BlobIdentifier blobIdentifier = BlobIdentifier.Deserialize(strArray[2]);
      List<string> networkPaths;
      if (strArray.Length <= 3)
        networkPaths = (List<string>) null;
      else
        networkPaths = ((IEnumerable<string>) strArray[3].Split(',')).ToList<string>();
      return new FileBlobDescriptor((IFileSystem) instance, rootDirectory1, relativePath, fileSize, blobIdentifier, networkPaths);
    }

    private FileBlobDescriptor(
      BlobIdentifier blobId,
      DedupNode? node,
      string relativePath,
      long? fileSize,
      IFileSystem fileSystem,
      string absolutePath,
      string rootDirectory,
      List<string> networkPaths)
    {
      ArgumentUtility.CheckForNull<BlobIdentifier>(blobId, nameof (blobId));
      ArgumentUtility.CheckStringForNullOrEmpty(relativePath, nameof (relativePath));
      this.BlobIdentifier = blobId;
      bool flag1 = blobId == ((IBlobHasher) VsoHash.Instance).OfNothing || blobId == ChunkBlobHasher.Instance.OfNothing;
      this.BlobIdentifierIsNodeOrChunk = ChunkerHelper.IsNodeOrChunk(blobId);
      this.RelativePath = relativePath;
      bool flag2 = relativePath.EndsWith(FileBlobDescriptorConstants.EmptyDirectoryEndingPattern);
      ArgumentUtility.CheckForNull<IFileSystem>(fileSystem, nameof (fileSystem));
      this.FileSystem = fileSystem;
      bool flag3 = !string.IsNullOrWhiteSpace(absolutePath);
      bool flag4 = !string.IsNullOrWhiteSpace(rootDirectory);
      if (flag3 & flag4)
        throw new ArgumentException("Both absolutePath and rootDirectory where specified, which is not allowed: " + absolutePath + " vs " + rootDirectory);
      if (flag4)
      {
        if (this.BlobIdentifierIsNodeOrChunk && !fileSystem.DirectoryExists(rootDirectory))
          throw new DirectoryNotFoundException(fileSystem.GetType().Name + " did not find rootDirectory: " + rootDirectory);
        this.RootDirectory = rootDirectory;
        this.AbsolutePath = Path.Combine(this.RootDirectory, relativePath);
      }
      else if (flag3)
        this.AbsolutePath = absolutePath;
      if (fileSize.HasValue)
      {
        if (flag1)
        {
          long? nullable = fileSize;
          long num = 0;
          if (!(nullable.GetValueOrDefault() == num & nullable.HasValue))
            throw new ArgumentException(string.Format("Expected {0} to be 0 for zero {1} {2}, but {3} was {4}", (object) nameof (fileSize), (object) nameof (blobId), (object) blobId.ValueString, (object) nameof (fileSize), (object) fileSize));
        }
        this.FileSize = fileSize;
      }
      else if (flag1)
        this.FileSize = new long?(0L);
      else if (this.BlobIdentifierIsNodeOrChunk && !flag2 && this.AbsolutePath != null)
        this.FileSize = fileSystem.FileExists(this.AbsolutePath) ? new long?(fileSystem.GetFileSize(this.AbsolutePath)) : throw new FileNotFoundException(fileSystem.GetType().Name + " did not find absolutePath, but it's needed to find the FileSize which wasn't specified: " + this.AbsolutePath);
      if (node.HasValue)
        this.Node = node.Value;
      else if (ChunkerHelper.IsChunk(this.BlobIdentifier.AlgorithmId))
        this.Node = new DedupNode(DedupNode.NodeType.ChunkLeaf, (ulong) this.FileSize.GetValueOrDefault(), blobId.AlgorithmResultBytes, new uint?());
      else if (ChunkerHelper.IsNode(this.BlobIdentifier.AlgorithmId))
        this.Node = new DedupNode(DedupNode.NodeType.InnerNode, (ulong) this.FileSize.GetValueOrDefault(), blobId.AlgorithmResultBytes, new uint?());
      this.NetworkPaths = networkPaths ?? FileBlobDescriptor.emptyNetworkPaths;
    }

    public FileBlobDescriptor(IDropFile copy, IFileSystem fileSystem)
      : this(copy.BlobIdentifier, new DedupNode?(), copy.RelativePath, copy.FileSize, fileSystem, (string) null, (string) null, (List<string>) null)
    {
    }

    public FileBlobDescriptor(IDropFile copy, string absolutePath)
      : this(copy, absolutePath, (IFileSystem) Microsoft.VisualStudio.Services.Content.Common.FileSystem.Instance)
    {
    }

    internal FileBlobDescriptor(IDropFile copy, string absolutePath, IFileSystem fileSystem)
      : this(copy.BlobIdentifier, new DedupNode?(), copy.RelativePath, copy.FileSize, fileSystem, absolutePath, (string) null, (List<string>) null)
    {
    }

    internal FileBlobDescriptor(
      IFileSystem fileSystem,
      string rootDirectory,
      string relativePath,
      long? fileSize,
      BlobIdentifier blobIdentifier,
      List<string> networkPaths)
      : this(blobIdentifier, new DedupNode?(), relativePath, fileSize, fileSystem, (string) null, rootDirectory, networkPaths)
    {
    }

    public DedupNode Node { get; private set; }

    public void SetFilledNode(DedupNode node)
    {
      byte algorithmId = this.BlobIdentifier.AlgorithmId;
      switch (algorithmId)
      {
        case 1:
        case 2:
          DedupNode node1 = this.Node;
          node.AssertFilled();
          BlobIdentifier blobIdentifier = node.GetDedupIdentifier().ToBlobIdentifier();
          if (blobIdentifier != this.BlobIdentifier)
            throw new ArgumentException("This DedupNode would change the existing BlobIdentifier from " + this.BlobIdentifier.ValueString + " to " + blobIdentifier.ValueString + " but a filled version of the same node was expected");
          long transitiveContentBytes = (long) node.TransitiveContentBytes;
          if (this.BlobIdentifier == ChunkBlobHasher.Instance.OfNothing && transitiveContentBytes != 0L)
            throw new ArgumentException(string.Format("This {0} has a {1} of {2} but 0 was expected because the existing {3} is the zero {4}", (object) "DedupNode", (object) "TransitiveContentBytes", (object) transitiveContentBytes, (object) "BlobIdentifier", (object) "ChunkLeaf"));
          long? fileSize = this.FileSize;
          if (fileSize.HasValue)
          {
            fileSize = this.FileSize;
            if (fileSize.Value != transitiveContentBytes)
            {
              object[] objArray = new object[5]
              {
                (object) "DedupNode",
                (object) "FileSize",
                null,
                null,
                null
              };
              fileSize = this.FileSize;
              objArray[2] = (object) fileSize.Value;
              objArray[3] = (object) transitiveContentBytes;
              objArray[4] = (object) "FileSize";
              throw new ArgumentException(string.Format("This {0} would change the existing {1} from {2} to {3} but this method is not expected to change a {4} specified previously by the caller", objArray));
            }
          }
          this.Node = node;
          this.FileSize = new long?(transitiveContentBytes);
          break;
        default:
          throw new InvalidOperationException(string.Format("{0} may only be used with a chunk-level {1} ({2} {3} or {4} {5}) but this one is {6} {7}.", (object) nameof (SetFilledNode), (object) nameof (FileBlobDescriptor), (object) "algorithmId", (object) (byte) 1, (object) "algorithmId", (object) (byte) 2, (object) "algorithmId", (object) algorithmId));
      }
    }

    public long? FileSize { get; private set; }

    public void SetFileSizeIfMissingThrowIfChanging(long fileSizeBytes)
    {
      if (this.BlobIdentifier == ChunkBlobHasher.Instance.OfNothing && fileSizeBytes != 0L)
        throw new ArgumentException(string.Format("{0} was {1} but 0 was expected because the existing {2} is the zero {3}", (object) nameof (fileSizeBytes), (object) fileSizeBytes, (object) "BlobIdentifier", (object) "ChunkLeaf"));
      if (!this.FileSize.HasValue)
        this.FileSize = new long?(fileSizeBytes);
      else if (this.FileSize.Value != fileSizeBytes)
        throw new InvalidOperationException(string.Format("{0} is already set to {1} but caller is attempting to overwrite with {2}", (object) "FileSize", (object) this.FileSize, (object) fileSizeBytes));
    }

    public string RelativePath { get; }

    public BlobIdentifier BlobIdentifier { get; private set; }

    public string Serialize() => FileBlobDescriptor.Serialize(this.RelativePath, this.FileSize.Value, this.BlobIdentifier, (IEnumerable<string>) this.NetworkPaths);

    private static string Serialize(
      string relativePath,
      long fileSize,
      BlobIdentifier blobIdentifier,
      IEnumerable<string> uncs)
    {
      return string.Format("{0}{1}{2}{3}{4}{5}{6}", (object) relativePath, (object) '?', (object) fileSize, (object) '?', (object) blobIdentifier.ValueString, (object) '?', (object) string.Join(",", uncs ?? Enumerable.Empty<string>()));
    }

    public int CompareTo(FileBlobDescriptor other)
    {
      long? fileSize1 = other.FileSize;
      long? fileSize2 = this.FileSize;
      if (!(fileSize1.GetValueOrDefault() == fileSize2.GetValueOrDefault() & fileSize1.HasValue == fileSize2.HasValue))
      {
        long? fileSize3 = this.FileSize;
        long? fileSize4 = other.FileSize;
        long? nullable = fileSize3.HasValue & fileSize4.HasValue ? new long?(fileSize3.GetValueOrDefault() - fileSize4.GetValueOrDefault()) : new long?();
        long num = 0;
        return !(nullable.GetValueOrDefault() < num & nullable.HasValue) ? 1 : -1;
      }
      if (!this.RelativePath.Equals(other.RelativePath))
        return string.Compare(this.RelativePath, other.RelativePath, StringComparison.Ordinal);
      if (!this.RootDirectory.Equals(other.RootDirectory))
        return string.Compare(this.RootDirectory, other.RootDirectory, StringComparison.Ordinal);
      if (!this.AbsolutePath.Equals(other.AbsolutePath))
        return string.Compare(this.AbsolutePath, other.AbsolutePath, StringComparison.Ordinal);
      return !this.BlobIdentifier.Equals(other.BlobIdentifier) ? this.BlobIdentifier.CompareTo((object) other.BlobIdentifier) : 0;
    }

    public override int GetHashCode() => this.Serialize().GetHashCode();

    public bool Equals(FileBlobDescriptor other) => this.CompareTo(other) == 0;

    public override bool Equals(object obj) => this.Equals(obj as FileBlobDescriptor);

    public override string ToString() => this.Serialize();

    private static bool IsSymbolicLinkTargetPathSubset(
      string rootDirectory,
      string symbolicLinkPath)
    {
      return (Path.IsPathRooted(symbolicLinkPath) ? symbolicLinkPath : Path.Combine(rootDirectory, symbolicLinkPath)).StartsWith(rootDirectory, StringComparison.Ordinal);
    }
  }
}
