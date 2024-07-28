// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.AsyncFile
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class AsyncFile
  {
    private static readonly Task CompletedTask = (Task) Task.FromResult<int>(0);
    private const int ERROR_HANDLE_EOF = 38;
    private const int ERROR_IO_PENDING = 997;
    private const int ERROR_IO_DEVICE = 1117;

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern unsafe int ReadFile(
      SafeFileHandle handle,
      byte* bytes,
      int numBytesToRead,
      IntPtr numBytesRead_mustBeZero,
      NativeOverlapped* overlapped);

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern unsafe int WriteFile(
      SafeFileHandle handle,
      byte* bytes,
      int numBytesToWrite,
      IntPtr numBytesWritten_mustBeZero,
      NativeOverlapped* lpOverlapped);

    [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool DeviceIoControl(
      SafeFileHandle hDevice,
      AsyncFile.EIOControlCode IoControlCode,
      [MarshalAs(UnmanagedType.AsAny), In] object InBuffer,
      uint nInBufferSize,
      [MarshalAs(UnmanagedType.AsAny), Out] object OutBuffer,
      uint nOutBufferSize,
      ref uint pBytesReturned,
      [In] IntPtr Overlapped);

    private static unsafe void CompletionCallback(
      uint errorCode,
      uint numBytes,
      NativeOverlapped* pOverlap)
    {
      try
      {
        AsyncFile.AsyncResult asyncResult = (AsyncFile.AsyncResult) Overlapped.Unpack(pOverlap).AsyncResult;
        switch (errorCode)
        {
          case 0:
          case 38:
            asyncResult.CompletionSource.Value.SetResult((int) numBytes);
            break;
          case 1117:
            asyncResult.CompletionSource.Value.SetException((Exception) new IOException("Async file operation failed with ERROR_IO_DEVICE", (Exception) new Win32Exception((int) errorCode)));
            break;
          default:
            asyncResult.CompletionSource.Value.SetException((Exception) new Win32Exception((int) errorCode, string.Format("Async file operation failed with {0}", (object) errorCode)));
            break;
        }
      }
      finally
      {
        Overlapped.Free(pOverlap);
      }
    }

    public static int TryMarkSparse(SafeFileHandle hFile, bool sparse)
    {
      uint pBytesReturned = 0;
      short InBuffer = (short) sparse;
      return AsyncFile.DeviceIoControl(hFile, AsyncFile.EIOControlCode.FsctlSetSparse, (object) InBuffer, 2U, (object) IntPtr.Zero, 0U, ref pBytesReturned, IntPtr.Zero) ? 0 : Marshal.GetLastWin32Error();
    }

    private static async Task<int> ReadSomeAsyncNetStandard(
      FileStream file,
      long fileOffset,
      ArraySegment<byte> buffer)
    {
      int num;
      using (FileStream f = new FileStream(file.Name, FileMode.Open, FileAccess.Read, FileShare.Read, 1, true))
      {
        f.Position = fileOffset;
        num = await f.ReadAsync(buffer.Array, buffer.Offset, buffer.Count);
      }
      return num;
    }

    public static Task ReadWholeBufferAsync(
      FileStream file,
      long fileOffset,
      byte[] buffer,
      int bytesToRead)
    {
      return AsyncFile.ReadWholeBufferAsync(file, fileOffset, new ArraySegment<byte>(buffer, 0, bytesToRead));
    }

    public static async Task ReadWholeBufferAsync(
      FileStream file,
      long fileOffset,
      ArraySegment<byte> buffer,
      int maxReadSize = 2147483647)
    {
      int bytesAlreadyRead = 0;
      int num;
      for (int bytesLeftToRead = buffer.Count; bytesLeftToRead > 0; bytesLeftToRead -= num)
      {
        num = await AsyncFile.ReadSomeAsync(file, fileOffset + (long) bytesAlreadyRead, new ArraySegment<byte>(buffer.Array, buffer.Offset + bytesAlreadyRead, Math.Min(maxReadSize, bytesLeftToRead)));
        if (num == 0)
          throw new EndOfStreamException(string.Format("Only able to read the first {0} bytes of the {1} bytes buffer.", (object) bytesAlreadyRead, (object) buffer.Count));
        bytesAlreadyRead += num;
      }
    }

    public static Task<int> ReadSomeAsync(
      FileStream file,
      long fileOffset,
      byte[] buffer,
      int bytesToRead)
    {
      return AsyncFile.ReadSomeAsync(file, fileOffset, new ArraySegment<byte>(buffer, 0, bytesToRead));
    }

    public static unsafe Task<int> ReadSomeAsync(
      FileStream file,
      long fileOffset,
      ArraySegment<byte> buffer)
    {
      if (fileOffset < 0L)
        throw new ArgumentOutOfRangeException(nameof (fileOffset));
      if (!file.IsAsync)
        throw new ArgumentException("FileStream must be opened for async access");
      AsyncFile.AsyncResult ar = new AsyncFile.AsyncResult();
      Overlapped overlapped = new Overlapped((int) (fileOffset & (long) uint.MaxValue), (int) (fileOffset >> 32), IntPtr.Zero, (IAsyncResult) ar);
      fixed (byte* numPtr = buffer.Array)
      {
        byte* bytes = numPtr + buffer.Offset;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        IOCompletionCallback iocb = AsyncFile.\u003C\u003EO.\u003C0\u003E__CompletionCallback ?? (AsyncFile.\u003C\u003EO.\u003C0\u003E__CompletionCallback = new IOCompletionCallback(AsyncFile.CompletionCallback));
        byte[] array = buffer.Array;
        NativeOverlapped* nativeOverlappedPtr = overlapped.Pack(iocb, (object) array);
        bool flag = true;
        try
        {
          if (AsyncFile.ReadFile(file.SafeFileHandle, bytes, buffer.Count, IntPtr.Zero, nativeOverlappedPtr) != 0)
          {
            flag = false;
          }
          else
          {
            int lastWin32Error = Marshal.GetLastWin32Error();
            switch (lastWin32Error)
            {
              case 997:
                flag = false;
                break;
              case 1117:
                throw new IOException("ReadFile failed with system error ERROR_IO_DEVICE", (Exception) new Win32Exception(lastWin32Error));
              default:
                throw new Win32Exception(lastWin32Error, string.Format("ReadFile failed with system error code:{0}", (object) lastWin32Error));
            }
          }
        }
        finally
        {
          if (flag)
          {
            Overlapped.Unpack(nativeOverlappedPtr);
            Overlapped.Free(nativeOverlappedPtr);
          }
        }
        return ar.CompletionSource.Value.Task;
      }
    }

    private static async Task WriteAsyncNetStandard(
      FileStream file,
      long fileOffset,
      ArraySegment<byte> bytes)
    {
      using (FileStream f = new FileStream(file.Name, FileMode.Open, FileAccess.Write, FileShare.Write, 1, true))
      {
        f.Position = fileOffset;
        await f.WriteAsync(bytes.Array, bytes.Offset, bytes.Count);
      }
    }

    public static unsafe async Task WriteAsync(
      FileStream file,
      long fileOffset,
      ArraySegment<byte> bytes)
    {
      if (fileOffset < 0L)
        throw new ArgumentOutOfRangeException(nameof (fileOffset));
      if (!file.IsAsync)
        throw new ArgumentException("FileStream must be opened for async");
      AsyncFile.AsyncResult ar = new AsyncFile.AsyncResult();
      Overlapped overlapped = new Overlapped((int) (fileOffset & (long) uint.MaxValue), (int) (fileOffset >> 32), IntPtr.Zero, (IAsyncResult) ar);
      fixed (byte* numPtr = bytes.Array)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        IOCompletionCallback iocb = AsyncFile.\u003C\u003EO.\u003C0\u003E__CompletionCallback ?? (AsyncFile.\u003C\u003EO.\u003C0\u003E__CompletionCallback = new IOCompletionCallback(AsyncFile.CompletionCallback));
        byte[] array = bytes.Array;
        NativeOverlapped* nativeOverlappedPtr = overlapped.Pack(iocb, (object) array);
        bool flag = true;
        try
        {
          if (AsyncFile.WriteFile(file.SafeFileHandle, numPtr + bytes.Offset, bytes.Count, IntPtr.Zero, nativeOverlappedPtr) != 0)
          {
            flag = false;
          }
          else
          {
            int lastWin32Error = Marshal.GetLastWin32Error();
            switch (lastWin32Error)
            {
              case 997:
                flag = false;
                break;
              case 1117:
                throw new IOException("WriteFile failed with system error ERROR_IO_DEVICE", (Exception) new Win32Exception(lastWin32Error));
              default:
                throw new Win32Exception(lastWin32Error, string.Format("WriteFile failed with system error code:{0}", (object) lastWin32Error));
            }
          }
        }
        finally
        {
          if (flag)
          {
            Overlapped.Unpack(nativeOverlappedPtr);
            Overlapped.Free(nativeOverlappedPtr);
          }
        }
      }
      if (await ar.CompletionSource.Value.Task.ConfigureAwait(false) != bytes.Count)
        throw new EndOfStreamException("Could not write all the bytes. (Async)");
    }

    [Flags]
    private enum EMethod : uint
    {
      Buffered = 0,
    }

    [Flags]
    private enum EFileDevice : uint
    {
      FileSystem = 9,
    }

    [Flags]
    private enum EIOControlCode : uint
    {
      FsctlSetSparse = 590020, // 0x000900C4
    }

    internal sealed class AsyncResult : IAsyncResult
    {
      public readonly Lazy<SafeTaskCompletionSource<int>> CompletionSource = new Lazy<SafeTaskCompletionSource<int>>(LazyThreadSafetyMode.ExecutionAndPublication);

      public bool IsCompleted => throw new NotSupportedException();

      public WaitHandle AsyncWaitHandle => throw new NotSupportedException();

      public object AsyncState => throw new NotSupportedException();

      public bool CompletedSynchronously => throw new NotSupportedException();
    }
  }
}
