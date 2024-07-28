// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.FileBufferedStreamBase
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  public abstract class FileBufferedStreamBase : Stream
  {
    public FileBufferedStreamBase(long sizeLimit) => this.SizeLimit = sizeLimit;

    public abstract string Name { get; }

    public abstract bool BufferingComplete { get; }

    public abstract Exception Exception { get; }

    public long SizeLimit { get; }

    protected static FileStream CreateFileWithTemporaryAttribute(
      string filePath,
      int bufferSize,
      System.IO.FileAccess fileAccess = System.IO.FileAccess.Read)
    {
      NativeMethods.FileAccess dwDesiredAccess = NativeMethods.FileAccess.GenericRead;
      if ((fileAccess & System.IO.FileAccess.Write) != (System.IO.FileAccess) 0)
        dwDesiredAccess |= NativeMethods.FileAccess.GenericWrite;
      SafeFileHandle file = NativeMethods.CreateFile(filePath, dwDesiredAccess, NativeMethods.FileShare.Read | NativeMethods.FileShare.Write, IntPtr.Zero, NativeMethods.CreationDisposition.New, NativeMethods.FileAttributes.Temporary | NativeMethods.FileAttributes.DeleteOnClose, IntPtr.Zero);
      if (file.IsInvalid)
        throw new Win32Exception();
      return new FileStream(file, fileAccess, bufferSize);
    }

    internal class TEST_Args
    {
      public bool ShouldWaitForEofIfSeekingFromEnd { get; set; }
    }
  }
}
