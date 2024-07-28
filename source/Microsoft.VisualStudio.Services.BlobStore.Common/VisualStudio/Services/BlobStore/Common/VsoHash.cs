// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.VsoHash
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public class VsoHash : IBlobHasher
  {
    public const byte AlgorithmId = 0;
    public const int PagesPerBlock = 32;
    public const int PageSize = 65536;
    public const int BlockSize = 2097152;
    public static readonly BlobIdentifierWithBlocks OfNothing;
    public static readonly bool BCryptAvailable;
    private static readonly byte[] EmptyByteArray = Array.Empty<byte>();
    private static readonly ByteArrayPool PoolLocalBlockBuffer = new ByteArrayPool(2097152, 1000);
    private static readonly Pool<System.Security.Cryptography.SHA256CryptoServiceProvider> PoolSha256 = new Pool<System.Security.Cryptography.SHA256CryptoServiceProvider>((Func<System.Security.Cryptography.SHA256CryptoServiceProvider>) (() => new System.Security.Cryptography.SHA256CryptoServiceProvider()), (Action<System.Security.Cryptography.SHA256CryptoServiceProvider>) (sha256 => sha256.Initialize()), 2 * Environment.ProcessorCount);
    private static readonly Pool<VsoHash.BCrypt.BCryptVsoHashContext> PoolBCrypt = new Pool<VsoHash.BCrypt.BCryptVsoHashContext>((Func<VsoHash.BCrypt.BCryptVsoHashContext>) (() => new VsoHash.BCrypt.BCryptVsoHashContext()), (Action<VsoHash.BCrypt.BCryptVsoHashContext>) (_ => { }), 2 * Environment.ProcessorCount);
    public static readonly VsoHash Instance = new VsoHash();

    public static IPoolHandle<byte[]> BorrowBlockBuffer() => (IPoolHandle<byte[]>) VsoHash.PoolLocalBlockBuffer.Get();

    public static IPoolHandle<System.Security.Cryptography.SHA256CryptoServiceProvider> BorrowSHA256() => (IPoolHandle<System.Security.Cryptography.SHA256CryptoServiceProvider>) VsoHash.PoolSha256.Get();

    private VsoHash()
    {
    }

    static VsoHash()
    {
      if (true)
      {
        byte[] block = new byte[10];
        try
        {
          VsoHash.HashBlockBCrypt(block, block.Length);
          VsoHash.BCryptAvailable = true;
        }
        catch
        {
          VsoHash.BCryptAvailable = false;
        }
      }
      else
        VsoHash.BCryptAvailable = false;
      using (MemoryStream memoryStream = new MemoryStream())
        VsoHash.OfNothing = VsoHash.CalculateBlobIdentifierWithBlocks((Stream) memoryStream);
    }

    public static BlobBlockHash HashBlock(byte[] block, int blockLength) => !VsoHash.BCryptAvailable ? VsoHash.HashBlockCng(block, blockLength) : VsoHash.HashBlockBCrypt(block, blockLength);

    public static BlobBlockHash HashBlockBCrypt(byte[] block, int blockLength)
    {
      using (Pool<VsoHash.BCrypt.BCryptVsoHashContext>.PoolHandle poolHandle = VsoHash.PoolBCrypt.Get())
        return poolHandle.Value.HashBlock(block, blockLength);
    }

    public static BlobBlockHash HashBlockCng(byte[] block, int blockLength)
    {
      using (Pool<System.Security.Cryptography.SHA256CryptoServiceProvider>.PoolHandle poolHandle1 = VsoHash.PoolSha256.Get())
      {
        int count;
        using (Pool<System.Security.Cryptography.SHA256CryptoServiceProvider>.PoolHandle poolHandle2 = VsoHash.PoolSha256.Get())
        {
          for (int offset = 0; offset < blockLength; offset += count)
          {
            count = Math.Min(blockLength - offset, 65536);
            byte[] hash = poolHandle2.Value.ComputeHash(block, offset, count);
            poolHandle1.Value.TransformBlock(hash, 0, hash.Length, (byte[]) null, 0);
          }
        }
        poolHandle1.Value.TransformFinalBlock(VsoHash.EmptyByteArray, 0, 0);
        return new BlobBlockHash(poolHandle1.Value.Hash);
      }
    }

    public static async Task<BlobIdentifierWithBlocks> WalkAllBlobBlocksAsync(
      Stream stream,
      SemaphoreSlim blockActionSemaphore,
      bool multiBlocksInParallel,
      MultiBlockBlobCallbackAsync multiBlockCallback,
      long? bytesToReadFromStream = null)
    {
      bytesToReadFromStream = new long?(bytesToReadFromStream ?? stream.Length - stream.Position);
      BlobIdentifierWithBlocks blobIdWithBlocks = (BlobIdentifierWithBlocks) null;
      await VsoHash.WalkMultiBlockBlobAsync(stream, blockActionSemaphore, multiBlocksInParallel, multiBlockCallback, (MultiBlockBlobSealCallbackAsync) (computedBlobIdWithBlocks =>
      {
        blobIdWithBlocks = computedBlobIdWithBlocks;
        return (Task) Task.FromResult<int>(0);
      }), bytesToReadFromStream.GetValueOrDefault()).ConfigureAwait(false);
      return blobIdWithBlocks;
    }

    public static async Task WalkBlocksAsync(
      Stream stream,
      SemaphoreSlim blockActionSemaphore,
      bool multiBlocksInParallel,
      SingleBlockBlobCallbackAsync singleBlockCallback,
      MultiBlockBlobCallbackAsync multiBlockCallback,
      MultiBlockBlobSealCallbackAsync multiBlockSealCallback,
      long bytesToReadFromStream = -1)
    {
      bytesToReadFromStream = bytesToReadFromStream >= 0L ? bytesToReadFromStream : stream.Length - stream.Position;
      if (bytesToReadFromStream <= 2097152L)
        await VsoHash.WalkSingleBlockBlobAsync(stream, blockActionSemaphore, singleBlockCallback, bytesToReadFromStream).ConfigureAwait(false);
      else
        await VsoHash.WalkMultiBlockBlobAsync(stream, blockActionSemaphore, multiBlocksInParallel, multiBlockCallback, multiBlockSealCallback, bytesToReadFromStream).ConfigureAwait(false);
    }

    public static void WalkBlocks(
      Stream stream,
      SemaphoreSlim blockActionSemaphore,
      bool multiBlocksInParallel,
      SingleBlockBlobCallback singleBlockCallback,
      MultiBlockBlobCallback multiBlockCallback,
      MultiBlockBlobSealCallback multiBlockSealCallback,
      long bytesToReadFromStream = -1)
    {
      bytesToReadFromStream = bytesToReadFromStream >= 0L ? bytesToReadFromStream : stream.Length - stream.Position;
      if (bytesToReadFromStream <= 2097152L)
        VsoHash.WalkSingleBlockBlob(stream, blockActionSemaphore, singleBlockCallback, bytesToReadFromStream);
      else
        VsoHash.WalkMultiBlockBlob(stream, blockActionSemaphore, multiBlocksInParallel, multiBlockCallback, multiBlockSealCallback, bytesToReadFromStream);
    }

    public static BlobIdentifierWithBlocks CalculateBlobIdentifierWithBlocks(Stream stream)
    {
      BlobIdentifierWithBlocks result = (BlobIdentifierWithBlocks) null;
      VsoHash.WalkBlocks(stream, (SemaphoreSlim) null, false, (SingleBlockBlobCallback) ((block, blockLength, blobIdWithBlocks) => result = blobIdWithBlocks), (MultiBlockBlobCallback) ((block, blockLength, blockHash, isFinalBlock) => { }), (MultiBlockBlobSealCallback) (blobIdWithBlocks => result = blobIdWithBlocks));
      return !(result == (BlobIdentifierWithBlocks) null) ? result : throw new InvalidOperationException("Program error: CalculateBlobIdentifierWithBlocks did not calculate a value.");
    }

    public static BlobIdentifierWithBlocks CalculateBlobIdentifierWithBlocks(
      ArraySegment<byte> bytes)
    {
      return VsoHash.CalculateBlobIdentifierWithBlocks((Stream) bytes.AsMemoryStream());
    }

    public static async Task<BlobIdentifierWithBlocks> CalculateBlobIdentifierWithBlocksAsync(
      Stream stream)
    {
      BlobIdentifierWithBlocks result = (BlobIdentifierWithBlocks) null;
      await VsoHash.WalkBlocksAsync(stream, (SemaphoreSlim) null, false, (SingleBlockBlobCallbackAsync) ((block, blockLength, blobIdWithBlocks) =>
      {
        result = blobIdWithBlocks;
        return (Task) Task.FromResult<int>(0);
      }), (MultiBlockBlobCallbackAsync) ((block, blockLength, blockHash, isFinalBlock) => (Task) Task.FromResult<int>(0)), (MultiBlockBlobSealCallbackAsync) (blobIdWithBlocks =>
      {
        result = blobIdWithBlocks;
        return (Task) Task.FromResult<int>(0);
      })).ConfigureAwait(false);
      return !(result == (BlobIdentifierWithBlocks) null) ? result : throw new InvalidOperationException("Program error: CalculateBlobIdentifierWithBlocksAsync did not calculate a value.");
    }

    public static BlobIdentifier CalculateBlobIdentifier(Stream stream) => stream != null ? VsoHash.CalculateBlobIdentifierWithBlocks(stream).BlobId : throw new ArgumentNullException(nameof (stream));

    public static BlobIdentifier CalculateBlobIdentifier(byte[] bytes)
    {
      if (bytes == null)
        throw new ArgumentNullException(nameof (bytes));
      using (MemoryStream memoryStream = new MemoryStream(bytes))
        return VsoHash.CalculateBlobIdentifierWithBlocks((Stream) memoryStream).BlobId;
    }

    public static BlobIdentifier CalculateBlobIdentifier(ArraySegment<byte> bytes)
    {
      using (MemoryStream memoryStream = new MemoryStream(bytes.Array, bytes.Offset, bytes.Count))
        return VsoHash.CalculateBlobIdentifierWithBlocks((Stream) memoryStream).BlobId;
    }

    public static async Task<BlobIdentifier> CalculateBlobIdentifierAsync(Stream stream)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      return (await VsoHash.CalculateBlobIdentifierWithBlocksAsync(stream).ConfigureAwait(false)).BlobId;
    }

    public static async Task<BlobIdentifier> CalculateBlobIdentifierAsync(byte[] bytes)
    {
      if (bytes == null)
        throw new ArgumentNullException(nameof (bytes));
      BlobIdentifier blobId;
      using (MemoryStream stream = new MemoryStream(bytes))
        blobId = (await VsoHash.CalculateBlobIdentifierWithBlocksAsync((Stream) stream).ConfigureAwait(false)).BlobId;
      return blobId;
    }

    private static byte[] ComputeSHA256Hash(List<byte> bytes)
    {
      byte[] array = bytes.ToArray();
      using (Pool<System.Security.Cryptography.SHA256CryptoServiceProvider>.PoolHandle poolHandle = VsoHash.PoolSha256.Get())
        return poolHandle.Value.ComputeHash(array, 0, array.Length);
    }

    private static void ReadBlock(
      Stream stream,
      SemaphoreSlim blockActionSemaphore,
      long bytesLeftInBlob,
      VsoHash.BlockReadComplete readCallback)
    {
      blockActionSemaphore?.Wait();
      bool flag = true;
      try
      {
        Pool<byte[]>.PoolHandle blockBufferHandle = VsoHash.PoolLocalBlockBuffer.Get();
        try
        {
          byte[] numArray = blockBufferHandle.Value;
          int count = (int) Math.Min(2097152L, bytesLeftInBlob);
          int num1 = 0;
          while (count > 0)
          {
            int num2 = stream.Read(numArray, num1, count);
            count -= num2;
            num1 += num2;
            if (num2 == 0 && count > 0)
              throw new EndOfStreamException();
          }
          BlobBlockHash blockHash = VsoHash.HashBlock(numArray, num1);
          flag = false;
          readCallback(blockBufferHandle, num1, blockHash);
        }
        finally
        {
          if (flag)
            blockBufferHandle.Dispose();
        }
      }
      finally
      {
        if (flag && blockActionSemaphore != null)
          blockActionSemaphore.Release();
      }
    }

    private static async Task ReadBlockAsync(
      Stream stream,
      SemaphoreSlim blockActionSemaphore,
      long bytesLeftInBlob,
      VsoHash.BlockReadCompleteAsync readCallback)
    {
      if (blockActionSemaphore != null)
        await blockActionSemaphore.WaitAsync().ConfigureAwait(false);
      bool disposeNeeded = true;
      try
      {
        Pool<byte[]>.PoolHandle blockBufferHandle = VsoHash.PoolLocalBlockBuffer.Get();
        try
        {
          byte[] blockBuffer = blockBufferHandle.Value;
          int bytesToRead = (int) Math.Min(2097152L, bytesLeftInBlob);
          int bufferOffset = 0;
          while (bytesToRead > 0)
          {
            int num = await stream.ReadAsync(blockBuffer, bufferOffset, bytesToRead).ConfigureAwait(false);
            bytesToRead -= num;
            bufferOffset += num;
            if (num == 0 && bytesToRead > 0)
              throw new EndOfStreamException();
          }
          BlobBlockHash blockHash = VsoHash.HashBlock(blockBuffer, bufferOffset);
          disposeNeeded = false;
          await readCallback(blockBufferHandle, bufferOffset, blockHash).ConfigureAwait(false);
          blockBuffer = (byte[]) null;
        }
        finally
        {
          if (disposeNeeded)
            blockBufferHandle.Dispose();
        }
        blockBufferHandle = new Pool<byte[]>.PoolHandle();
      }
      finally
      {
        if (disposeNeeded)
          blockActionSemaphore?.Release();
      }
    }

    private static void WalkMultiBlockBlob(
      Stream stream,
      SemaphoreSlim blockActionSemaphore,
      bool multiBlocksInParallel,
      MultiBlockBlobCallback multiBlockCallback,
      MultiBlockBlobSealCallback multiBlockSealCallback,
      long bytesLeftInBlob)
    {
      VsoHash.RollingBlobIdentifierWithBlocks rollingId = new VsoHash.RollingBlobIdentifierWithBlocks();
      BlobIdentifierWithBlocks blobIdentifierWithBlocks = (BlobIdentifierWithBlocks) null;
      Lazy<List<Task>> tasks = new Lazy<List<Task>>((Func<List<Task>>) (() => new List<Task>()));
      do
      {
        VsoHash.ReadBlock(stream, blockActionSemaphore, bytesLeftInBlob, (VsoHash.BlockReadComplete) ((blockBufferHandle, blockLength, blockHash) =>
        {
          bytesLeftInBlob -= (long) blockLength;
          bool isFinalBlock = bytesLeftInBlob == 0L;
          try
          {
            if (isFinalBlock)
              blobIdentifierWithBlocks = rollingId.Finalize(blockHash);
            else
              rollingId.Update(blockHash);
          }
          catch (Exception ex)
          {
            VsoHash.CleanupBufferAndSemaphore(blockBufferHandle, blockActionSemaphore);
            ex.ReThrow();
            throw new InvalidOperationException("unreachable.");
          }
          if (multiBlocksInParallel)
          {
            tasks.Value.Add(Task.Run((Action) (() =>
            {
              try
              {
                multiBlockCallback(blockBufferHandle.Value, blockLength, blockHash, isFinalBlock);
              }
              finally
              {
                VsoHash.CleanupBufferAndSemaphore(blockBufferHandle, blockActionSemaphore);
              }
            })));
          }
          else
          {
            try
            {
              multiBlockCallback(blockBufferHandle.Value, blockLength, blockHash, isFinalBlock);
            }
            finally
            {
              VsoHash.CleanupBufferAndSemaphore(blockBufferHandle, blockActionSemaphore);
            }
          }
        }));
      }
      while (bytesLeftInBlob > 0L);
      if (tasks.IsValueCreated)
        Task.WaitAll(tasks.Value.ToArray());
      multiBlockSealCallback(blobIdentifierWithBlocks);
    }

    private static async Task WalkMultiBlockBlobAsync(
      Stream stream,
      SemaphoreSlim blockActionSemaphore,
      bool multiBlocksInParallel,
      MultiBlockBlobCallbackAsync multiBlockCallback,
      MultiBlockBlobSealCallbackAsync multiBlockSealCallback,
      long bytesLeftInBlob)
    {
      VsoHash.RollingBlobIdentifierWithBlocks rollingId = new VsoHash.RollingBlobIdentifierWithBlocks();
      BlobIdentifierWithBlocks blobIdentifierWithBlocks = (BlobIdentifierWithBlocks) null;
      List<Task> tasks = new List<Task>();
      do
      {
        await VsoHash.ReadBlockAsync(stream, blockActionSemaphore, bytesLeftInBlob, (VsoHash.BlockReadCompleteAsync) (async (blockBufferHandle, blockLength, blockHash) =>
        {
          bytesLeftInBlob -= (long) blockLength;
          bool isFinalBlock = bytesLeftInBlob == 0L;
          try
          {
            if (isFinalBlock)
              blobIdentifierWithBlocks = rollingId.Finalize(blockHash);
            else
              rollingId.Update(blockHash);
          }
          catch (Exception ex)
          {
            VsoHash.CleanupBufferAndSemaphore(blockBufferHandle, blockActionSemaphore);
            ex.ReThrow();
            throw new InvalidOperationException("unreachable.");
          }
          Task task = Task.Run((Func<Task>) (async () =>
          {
            try
            {
              await multiBlockCallback(blockBufferHandle.Value, blockLength, blockHash, isFinalBlock).ConfigureAwait(false);
            }
            finally
            {
              VsoHash.CleanupBufferAndSemaphore(blockBufferHandle, blockActionSemaphore);
            }
          }));
          tasks.Add(task);
          if (multiBlocksInParallel)
            return;
          await task.ConfigureAwait(false);
        })).ConfigureAwait(false);
      }
      while (bytesLeftInBlob > 0L);
      ConfiguredTaskAwaitable configuredTaskAwaitable = Task.WhenAll((IEnumerable<Task>) tasks).ConfigureAwait(false);
      await configuredTaskAwaitable;
      configuredTaskAwaitable = multiBlockSealCallback(blobIdentifierWithBlocks).ConfigureAwait(false);
      await configuredTaskAwaitable;
    }

    private static void CleanupBufferAndSemaphore(
      Pool<byte[]>.PoolHandle blockBufferHandle,
      SemaphoreSlim blockActionSemaphore)
    {
      blockBufferHandle.Dispose();
      blockActionSemaphore?.Release();
    }

    private static void WalkSingleBlockBlob(
      Stream stream,
      SemaphoreSlim blockActionSemaphore,
      SingleBlockBlobCallback singleBlockCallback,
      long bytesLeftInBlob)
    {
      VsoHash.ReadBlock(stream, blockActionSemaphore, bytesLeftInBlob, (VsoHash.BlockReadComplete) ((blockBufferHandle, blockLength, blockHash) =>
      {
        try
        {
          BlobIdentifierWithBlocks blobIdWithBlocks = new VsoHash.RollingBlobIdentifierWithBlocks().Finalize(blockHash);
          singleBlockCallback(blockBufferHandle.Value, blockLength, blobIdWithBlocks);
        }
        finally
        {
          blockBufferHandle.Dispose();
          blockActionSemaphore?.Release();
        }
      }));
    }

    private static Task WalkSingleBlockBlobAsync(
      Stream stream,
      SemaphoreSlim blockActionSemaphore,
      SingleBlockBlobCallbackAsync singleBlockCallback,
      long bytesLeftInBlob)
    {
      return VsoHash.ReadBlockAsync(stream, blockActionSemaphore, bytesLeftInBlob, (VsoHash.BlockReadCompleteAsync) (async (blockBufferHandle, blockLength, blockHash) =>
      {
        try
        {
          BlobIdentifierWithBlocks blobIdWithBlocks = new VsoHash.RollingBlobIdentifierWithBlocks().Finalize(blockHash);
          await singleBlockCallback(blockBufferHandle.Value, blockLength, blobIdWithBlocks).ConfigureAwait(false);
        }
        finally
        {
          blockBufferHandle.Dispose();
          blockActionSemaphore?.Release();
        }
      }));
    }

    BlobIdentifier IBlobHasher.OfNothing => VsoHash.OfNothing.BlobId;

    byte IBlobHasher.AlgorithmId => 0;

    public Task WalkBlocksAsync(
      Stream data,
      bool multiBlocksInParallel,
      SingleBlockBlobCallbackAsync singleBlockCallback,
      MultiBlockBlobCallbackAsync multiBlockCallback,
      MultiBlockBlobSealCallbackAsync multiBlockSealCallback)
    {
      return VsoHash.WalkBlocksAsync(data, (SemaphoreSlim) null, multiBlocksInParallel, singleBlockCallback, multiBlockCallback, multiBlockSealCallback);
    }

    async Task<BlobIdentifier> IBlobHasher.CalculateBlobIdentifierAsync(Stream data) => (await VsoHash.CalculateBlobIdentifierWithBlocksAsync(data).ConfigureAwait(false)).BlobId;

    BlobBlockHash IBlobHasher.CalculateBlobBlockHash(byte[] data, int length) => VsoHash.HashBlock(data, length);

    Task<BlobIdentifierWithBlocks> IBlobHasher.CalculateBlobIdentifierWithBlocksAsync(Stream data) => VsoHash.CalculateBlobIdentifierWithBlocksAsync(data);

    public BlobIdentifier CalculateBlobIdentifierFromBlobBlockHashes(
      IEnumerable<BlobBlockHash> blocks)
    {
      return VsoHash.CalculateFromBlobBlockHashes(blocks);
    }

    public static BlobIdentifier CalculateFromBlobBlockHashes(IEnumerable<BlobBlockHash> blocks)
    {
      VsoHash.RollingBlobIdentifier rollingBlobIdentifier = new VsoHash.RollingBlobIdentifier();
      IEnumerator<BlobBlockHash> enumerator = blocks.GetEnumerator();
      BlobBlockHash currentBlockIdentifier = enumerator.MoveNext() ? enumerator.Current : throw new InvalidDataException("Blob must have at least one block.");
      for (bool flag = !enumerator.MoveNext(); !flag; flag = !enumerator.MoveNext())
      {
        rollingBlobIdentifier.Update(currentBlockIdentifier);
        currentBlockIdentifier = enumerator.Current;
      }
      return rollingBlobIdentifier.Finalize(currentBlockIdentifier);
    }

    private delegate Task BlockReadCompleteAsync(
      Pool<byte[]>.PoolHandle blockBufferHandle,
      int blockLength,
      BlobBlockHash blockHash);

    private delegate void BlockReadComplete(
      Pool<byte[]>.PoolHandle blockBufferHandle,
      int blockLength,
      BlobBlockHash blockHash);

    public class RollingBlobIdentifierWithBlocks
    {
      private List<BlobBlockHash> blockHashes;
      private readonly VsoHash.RollingBlobIdentifier inner;

      public RollingBlobIdentifierWithBlocks()
      {
        this.inner = new VsoHash.RollingBlobIdentifier();
        this.blockHashes = new List<BlobBlockHash>();
      }

      public void Update(BlobBlockHash currentBlockIdentifier)
      {
        this.blockHashes.Add(currentBlockIdentifier);
        this.inner.Update(currentBlockIdentifier);
      }

      public BlobIdentifierWithBlocks Finalize(BlobBlockHash currentBlockIdentifier)
      {
        this.blockHashes.Add(currentBlockIdentifier);
        return new BlobIdentifierWithBlocks(this.inner.Finalize(currentBlockIdentifier), (IEnumerable<BlobBlockHash>) this.blockHashes);
      }
    }

    public class RollingBlobIdentifier
    {
      private static readonly byte[] InitialRollingId = Encoding.ASCII.GetBytes("VSO Content Identifier Seed");
      private byte[] rollingId = VsoHash.RollingBlobIdentifier.InitialRollingId;
      private bool finalAdded;

      public void Update(BlobBlockHash currentBlockIdentifier) => this.UpdateInternal(currentBlockIdentifier, false);

      public BlobIdentifier Finalize(BlobBlockHash currentBlockIdentifier)
      {
        this.UpdateInternal(currentBlockIdentifier, true);
        return new BlobIdentifier(this.rollingId, (byte) 0);
      }

      private void UpdateInternal(BlobBlockHash currentBlockIdentifier, bool isFinalBlock)
      {
        if (this.finalAdded & isFinalBlock)
          throw new InvalidOperationException("Final block already added.");
        List<byte> bytes = new List<byte>((IEnumerable<byte>) this.rollingId);
        bytes.AddRange((IEnumerable<byte>) currentBlockIdentifier.HashBytes);
        bytes.Add(Convert.ToByte(isFinalBlock));
        this.rollingId = VsoHash.ComputeSHA256Hash(bytes);
        if (!isFinalBlock)
          return;
        this.finalAdded = true;
      }
    }

    private static class BCrypt
    {
      [DllImport("BCrypt", SetLastError = true)]
      private static extern int BCryptDestroyHash(IntPtr hHash);

      [DllImport("BCrypt", SetLastError = true)]
      private static extern int BCryptCloseAlgorithmProvider(
        IntPtr algorithmHandle,
        VsoHash.BCrypt.BCryptCloseAlgorithmProviderFlags flags = VsoHash.BCrypt.BCryptCloseAlgorithmProviderFlags.None);

      [DllImport("BCrypt", CharSet = CharSet.Unicode, SetLastError = true)]
      private static extern int BCryptOpenAlgorithmProvider(
        out VsoHash.BCrypt.SafeAlgorithmHandle phAlgorithm,
        string pszAlgId,
        string pszImplementation,
        VsoHash.BCrypt.BCryptOpenAlgorithmProviderFlags dwFlags);

      [DllImport("BCrypt", SetLastError = true)]
      private static extern int BCryptCreateMultiHash(
        VsoHash.BCrypt.SafeAlgorithmHandle hAlgorithm,
        out VsoHash.BCrypt.SafeHashHandle phHash,
        int nHashes,
        byte[] pbHashObject,
        int cbHashObject,
        byte[] pbSecret,
        int cbSecret,
        VsoHash.BCrypt.BCryptCreateHashFlags dwFlags);

      [DllImport("BCrypt", SetLastError = true)]
      private static extern int BCryptProcessMultiOperations(
        VsoHash.BCrypt.SafeHashHandle hHash,
        VsoHash.BCrypt.MultiOperationType operationType,
        VsoHash.BCrypt.BCRYPT_MULTI_HASH_OPERATION[] pOperations,
        int cbOperations,
        int dwFlags = 0);

      private class SafeHashHandle : SafeHandle
      {
        public static readonly VsoHash.BCrypt.SafeHashHandle Null = new VsoHash.BCrypt.SafeHashHandle();

        public SafeHashHandle()
          : base(IntPtr.Zero, true)
        {
        }

        public SafeHashHandle(IntPtr preexistingHandle, bool ownsHandle = true)
          : base(IntPtr.Zero, ownsHandle)
        {
          this.SetHandle(preexistingHandle);
        }

        public override bool IsInvalid => this.handle == IntPtr.Zero;

        protected override bool ReleaseHandle() => VsoHash.BCrypt.BCryptDestroyHash(this.handle) == 0;
      }

      private class SafeAlgorithmHandle : SafeHandle
      {
        public static readonly VsoHash.BCrypt.SafeAlgorithmHandle Null = new VsoHash.BCrypt.SafeAlgorithmHandle();

        public SafeAlgorithmHandle()
          : base(IntPtr.Zero, true)
        {
        }

        public SafeAlgorithmHandle(IntPtr preexistingHandle, bool ownsHandle = true)
          : base(IntPtr.Zero, ownsHandle)
        {
          this.SetHandle(preexistingHandle);
        }

        public override bool IsInvalid => this.handle == IntPtr.Zero;

        protected override bool ReleaseHandle() => VsoHash.BCrypt.BCryptCloseAlgorithmProvider(this.handle) == 0;
      }

      private enum BCryptCloseAlgorithmProviderFlags
      {
        None,
      }

      private enum BCryptOpenAlgorithmProviderFlags
      {
        None = 0,
        BCRYPT_ALG_HANDLE_HMAC_FLAG = 8,
        BCRYPT_HASH_REUSABLE_FLAG = 32, // 0x00000020
        BCRYPT_MULTI_FLAG = 64, // 0x00000040
      }

      private enum HashOperationType
      {
        BCRYPT_HASH_OPERATION_HASH_DATA = 1,
        BCRYPT_HASH_OPERATION_FINISH_HASH = 2,
      }

      private struct BCRYPT_MULTI_HASH_OPERATION
      {
        public int iHash;
        public VsoHash.BCrypt.HashOperationType hashOperation;
        public unsafe byte* pbBuffer;
        public int cbBuffer;
      }

      private static class AlgorithmIdentifiers
      {
        public const string BCRYPT_SHA512_ALGORITHM = "SHA512";
        public const string BCRYPT_SHA256_ALGORITHM = "SHA256";
      }

      public enum BCryptCreateHashFlags
      {
        None = 0,
        BCRYPT_HASH_REUSABLE_FLAG = 32, // 0x00000020
      }

      public enum MultiOperationType
      {
        BCRYPT_OPERATION_TYPE_HASH = 1,
      }

      public sealed class BCryptVsoHashContext : IDisposable
      {
        private const int SHA256ByteCount = 32;
        private static readonly int MultiHashStructSize = Marshal.SizeOf<VsoHash.BCrypt.BCRYPT_MULTI_HASH_OPERATION>();
        private readonly VsoHash.BCrypt.BCRYPT_MULTI_HASH_OPERATION[] ops = new VsoHash.BCrypt.BCRYPT_MULTI_HASH_OPERATION[64];
        private readonly VsoHash.BCrypt.SafeAlgorithmHandle algorithm;
        private readonly VsoHash.BCrypt.SafeHashHandle hash;
        private readonly byte[] pageHashes = new byte[1024];

        public BCryptVsoHashContext()
        {
          VsoHash.BCrypt.BCryptOpenAlgorithmProvider(out this.algorithm, "SHA256", (string) null, VsoHash.BCrypt.BCryptOpenAlgorithmProviderFlags.BCRYPT_MULTI_FLAG);
          VsoHash.BCrypt.BCryptCreateMultiHash(this.algorithm, out this.hash, 32, (byte[]) null, 0, (byte[]) null, 0, VsoHash.BCrypt.BCryptCreateHashFlags.BCRYPT_HASH_REUSABLE_FLAG);
        }

        public unsafe BlobBlockHash HashBlock(byte[] block, int blockLength)
        {
          int num1 = (blockLength + 65536 - 1) / 65536;
          int cbOperations = num1 * 2 * VsoHash.BCrypt.BCryptVsoHashContext.MultiHashStructSize;
          byte[] hashValue = new byte[32];
          fixed (byte* numPtr1 = block)
            fixed (byte* numPtr2 = this.pageHashes)
            {
              byte* numPtr3 = numPtr1;
              int val1 = blockLength;
              for (int index = 0; index < num1; ++index)
              {
                this.ops[index].iHash = index;
                this.ops[index].hashOperation = VsoHash.BCrypt.HashOperationType.BCRYPT_HASH_OPERATION_HASH_DATA;
                this.ops[index].pbBuffer = numPtr3;
                int num2 = Math.Min(val1, 65536);
                this.ops[index].cbBuffer = num2;
                val1 -= num2;
                numPtr3 += 65536;
              }
              byte* numPtr4 = numPtr2;
              for (int index = 0; index < num1; ++index)
              {
                this.ops[num1 + index].iHash = index;
                this.ops[num1 + index].hashOperation = VsoHash.BCrypt.HashOperationType.BCRYPT_HASH_OPERATION_FINISH_HASH;
                this.ops[num1 + index].pbBuffer = numPtr4;
                this.ops[num1 + index].cbBuffer = 32;
                numPtr4 += 32;
              }
              VsoHash.BCrypt.BCryptProcessMultiOperations(this.hash, VsoHash.BCrypt.MultiOperationType.BCRYPT_OPERATION_TYPE_HASH, this.ops, cbOperations);
              fixed (byte* numPtr5 = hashValue)
              {
                this.ops[0].iHash = 0;
                this.ops[0].hashOperation = VsoHash.BCrypt.HashOperationType.BCRYPT_HASH_OPERATION_HASH_DATA;
                this.ops[0].pbBuffer = numPtr2;
                this.ops[0].cbBuffer = num1 * 32;
                this.ops[1].iHash = 0;
                this.ops[1].hashOperation = VsoHash.BCrypt.HashOperationType.BCRYPT_HASH_OPERATION_FINISH_HASH;
                this.ops[1].pbBuffer = numPtr5;
                this.ops[1].cbBuffer = 32;
                VsoHash.BCrypt.BCryptProcessMultiOperations(this.hash, VsoHash.BCrypt.MultiOperationType.BCRYPT_OPERATION_TYPE_HASH, this.ops, 2 * VsoHash.BCrypt.BCryptVsoHashContext.MultiHashStructSize);
              }
            }
          return new BlobBlockHash(hashValue);
        }

        public void Dispose()
        {
          this.hash.Dispose();
          this.algorithm.Dispose();
        }
      }
    }
  }
}
