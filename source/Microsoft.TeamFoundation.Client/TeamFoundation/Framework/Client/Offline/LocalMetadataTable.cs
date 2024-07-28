// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.Offline.LocalMetadataTable
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Client.Offline
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class LocalMetadataTable : IDisposable
  {
    private bool m_flushToDisk;
    private bool m_dirty;
    private string m_fileName;
    private int m_retryCount;
    private bool m_loadFromBackup;
    private Microsoft.TeamFoundation.Common.Internal.NativeMethods.WIN32_FILE_ATTRIBUTE_DATA m_savedAttributes;
    private Microsoft.TeamFoundation.Common.Internal.NativeMethods.WIN32_FILE_ATTRIBUTE_DATA m_loadAttributes;
    private LocalMetadataTableLock m_tableLock;
    protected const string c_extensionSlotOne = ".tf1";
    protected const string c_extensionSlotTwo = ".tf2";
    protected const string c_extensionSlotThree = ".tf3";
    protected const string c_extensionSlotBackup = ".tfb";

    protected LocalMetadataTable(string fileName)
      : this(fileName, (LocalMetadataTable) null, 7, (object) null, false)
    {
    }

    protected LocalMetadataTable(string fileName, bool loadFromBackup)
      : this(fileName, (LocalMetadataTable) null, 7, (object) null, loadFromBackup)
    {
    }

    protected LocalMetadataTable(string fileName, LocalMetadataTable cachedLoadSource)
      : this(fileName, cachedLoadSource, 7, (object) null, false)
    {
    }

    protected LocalMetadataTable(
      string fileName,
      LocalMetadataTable cachedLoadSource,
      int retryCount)
      : this(fileName, cachedLoadSource, retryCount, (object) null, false)
    {
    }

    protected LocalMetadataTable(
      string fileName,
      LocalMetadataTable cachedLoadSource,
      int retryCount,
      object initializeData)
      : this(fileName, cachedLoadSource, retryCount, initializeData, false)
    {
    }

    protected LocalMetadataTable(
      string fileName,
      LocalMetadataTable cachedLoadSource,
      int retryCount,
      object initializeData,
      bool loadFromBackup)
    {
      this.m_fileName = fileName;
      this.m_retryCount = retryCount;
      this.m_loadFromBackup = loadFromBackup;
      this.IsDirty = false;
      this.IsAborted = false;
      if (this.m_loadFromBackup)
        cachedLoadSource = (LocalMetadataTable) null;
      try
      {
        this.m_tableLock = new LocalMetadataTableLock(this.m_fileName, this.m_retryCount, false);
        this.Recover();
        this.Initialize(initializeData);
        if (this.TryCachedLoad(cachedLoadSource))
          return;
        TeamFoundationTrace.Verbose(this.TraceKeywords, "Local metadata table load start ({0})", (object) this.GetType().Name);
        Stopwatch stopwatch = Stopwatch.StartNew();
        using (SafeFileHandle readHandle = this.GetReadHandle(loadFromBackup))
        {
          if (!readHandle.IsInvalid)
            this.Load(readHandle);
        }
        TeamFoundationTrace.Verbose(this.TraceKeywords, "  Local metadata table load end ({0}) ({1} ms elapsed)", (object) this.GetType().Name, (object) stopwatch.ElapsedMilliseconds);
      }
      catch
      {
        this.Dispose(false);
        throw;
      }
    }

    public void Close() => this.Dispose();

    public void Dispose() => this.Dispose(true);

    protected virtual void Dispose(bool disposing)
    {
      try
      {
        if (!disposing || this.m_tableLock == null)
          return;
        if (this.IsDirty && !this.IsAborted)
        {
          TeamFoundationTrace.Verbose(this.TraceKeywords, "Local metadata table save start ({0})", (object) this.GetType().Name);
          Stopwatch stopwatch = Stopwatch.StartNew();
          bool keepFile = true;
          using (SafeFileHandle writeHandle = this.GetWriteHandle())
          {
            using (SafeFileHandle fileHandle = new SafeFileHandle(writeHandle.DangerousGetHandle(), false))
              keepFile = this.Save(fileHandle);
            if (keepFile)
            {
              if (this.m_flushToDisk)
              {
                if (!Microsoft.TeamFoundation.Common.Internal.NativeMethods.FlushFileBuffers(writeHandle))
                  throw new Win32Exception();
              }
            }
          }
          this.PositionFile(keepFile);
          TeamFoundationTrace.Verbose(this.TraceKeywords, "  Local metadata table save end ({0}) ({1} ms elapsed)", (object) this.GetType().Name, (object) stopwatch.ElapsedMilliseconds);
          this.IsDirty = false;
          this.SaveComplete();
          this.MakeEligibleForCachedLoad(true);
        }
        else
        {
          if (this.IsDirty)
            return;
          this.MakeEligibleForCachedLoad(false);
        }
      }
      finally
      {
        if (this.m_tableLock != null)
          this.m_tableLock.Dispose();
      }
    }

    private void MakeEligibleForCachedLoad(bool dirty)
    {
      string slotOnePath = LocalMetadataTable.GetSlotOnePath(this.m_fileName);
      this.m_savedAttributes = new Microsoft.TeamFoundation.Common.Internal.NativeMethods.WIN32_FILE_ATTRIBUTE_DATA();
      while (Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetFileAttributesEx(slotOnePath, Microsoft.TeamFoundation.Common.Internal.NativeMethods.GET_FILEEX_INFO_LEVELS.GetFileExInfoStandard, ref this.m_savedAttributes))
      {
        if (dirty && LocalMetadataTable.AttributesMatch(this.m_loadAttributes, this.m_savedAttributes))
        {
          File.SetLastWriteTime(slotOnePath, DateTime.Now);
        }
        else
        {
          this.IsEligibleForCachedLoad = true;
          return;
        }
      }
      this.IsEligibleForCachedLoad = false;
    }

    protected abstract void Load(SafeFileHandle fileHandle);

    protected abstract bool Save(SafeFileHandle fileHandle);

    protected virtual void Initialize(object initializeData)
    {
    }

    protected virtual void SaveComplete()
    {
    }

    protected virtual bool CachedLoad(LocalMetadataTable source) => false;

    private bool TryCachedLoad(LocalMetadataTable source)
    {
      bool fileAttributesEx = Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetFileAttributesEx(LocalMetadataTable.GetSlotOnePath(this.m_fileName), Microsoft.TeamFoundation.Common.Internal.NativeMethods.GET_FILEEX_INFO_LEVELS.GetFileExInfoStandard, ref this.m_loadAttributes);
      if (!fileAttributesEx)
      {
        int lastWin32Error = Marshal.GetLastWin32Error();
        if (2 != lastWin32Error && 3 != lastWin32Error)
          throw new Win32Exception(lastWin32Error);
      }
      if (source != null)
      {
        source.IsEligibleForCachedLoad = false;
        if (fileAttributesEx && LocalMetadataTable.AttributesMatch(this.m_loadAttributes, source.SavedAttributes))
          return this.CachedLoad(source);
      }
      return false;
    }

    [CLSCompliant(false)]
    public static bool AttributesMatch(
      Microsoft.TeamFoundation.Common.Internal.NativeMethods.WIN32_FILE_ATTRIBUTE_DATA attrs1,
      Microsoft.TeamFoundation.Common.Internal.NativeMethods.WIN32_FILE_ATTRIBUTE_DATA attrs2)
    {
      return attrs1.fileSizeHigh == attrs2.fileSizeHigh && attrs1.fileSizeLow == attrs2.fileSizeLow && (int) attrs1.ftLastWriteTimeHigh == (int) attrs2.ftLastWriteTimeHigh && (int) attrs1.ftLastWriteTimeLow == (int) attrs2.ftLastWriteTimeLow;
    }

    protected abstract string[] TraceKeywords { get; }

    public bool IsDirty
    {
      get => this.m_dirty;
      set => this.m_dirty = value;
    }

    public bool FlushToDisk
    {
      get => this.m_flushToDisk;
      set => this.m_flushToDisk = value;
    }

    internal bool IsAborted { get; set; }

    internal bool IsEligibleForCachedLoad { get; set; }

    private void Recover()
    {
      string slotOnePath = LocalMetadataTable.GetSlotOnePath(this.m_fileName);
      string slotTwoPath = LocalMetadataTable.GetSlotTwoPath(this.m_fileName);
      if (!File.Exists(slotTwoPath))
        return;
      if (File.Exists(slotOnePath))
        FileSpec.DeleteFile(slotTwoPath);
      else if (!Microsoft.TeamFoundation.Common.Internal.NativeMethods.MoveFile(slotTwoPath, slotOnePath))
        throw new Win32Exception();
    }

    public void BackupFile()
    {
      string slotOnePath = LocalMetadataTable.GetSlotOnePath(this.m_fileName);
      string slotBackupPath = LocalMetadataTable.GetSlotBackupPath(this.m_fileName);
      Stopwatch stopwatch = Stopwatch.StartNew();
      TeamFoundationTrace.Verbose(this.TraceKeywords, "Local metadata table backup start ({0})", (object) this.GetType().Name);
      if (File.Exists(slotOnePath))
      {
        if (!Microsoft.TeamFoundation.Common.Internal.NativeMethods.CopyFileEx(slotOnePath, slotBackupPath, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, Microsoft.TeamFoundation.Common.Internal.NativeMethods.COPY_FILE_NO_BUFFERING))
        {
          int lastWin32Error = Marshal.GetLastWin32Error();
          if (87 != lastWin32Error && 5 != lastWin32Error)
            throw new Win32Exception(lastWin32Error);
          if (!Microsoft.TeamFoundation.Common.Internal.NativeMethods.CopyFileEx(slotOnePath, slotBackupPath, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0U))
            throw new Win32Exception();
        }
      }
      else
        FileSpec.DeleteFile(slotBackupPath);
      TeamFoundationTrace.Verbose(this.TraceKeywords, "  Local metadata table backup end ({0}) ({1} ms elapsed)", (object) this.GetType().Name, (object) stopwatch.ElapsedMilliseconds);
    }

    public bool NeedsBackup(int maximumPermissibleBackupAgeInSeconds)
    {
      string slotOnePath = LocalMetadataTable.GetSlotOnePath(this.m_fileName);
      string slotBackupPath = LocalMetadataTable.GetSlotBackupPath(this.m_fileName);
      Microsoft.TeamFoundation.Common.Internal.NativeMethods.WIN32_FILE_ATTRIBUTE_DATA fileAttributeData = new Microsoft.TeamFoundation.Common.Internal.NativeMethods.WIN32_FILE_ATTRIBUTE_DATA();
      Microsoft.TeamFoundation.Common.Internal.NativeMethods.WIN32_FILE_ATTRIBUTE_DATA lpFileInformation = new Microsoft.TeamFoundation.Common.Internal.NativeMethods.WIN32_FILE_ATTRIBUTE_DATA();
      ref Microsoft.TeamFoundation.Common.Internal.NativeMethods.WIN32_FILE_ATTRIBUTE_DATA local = ref fileAttributeData;
      int num = Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetFileAttributesEx(slotBackupPath, Microsoft.TeamFoundation.Common.Internal.NativeMethods.GET_FILEEX_INFO_LEVELS.GetFileExInfoStandard, ref local) ? 1 : 0;
      bool fileAttributesEx = Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetFileAttributesEx(slotOnePath, Microsoft.TeamFoundation.Common.Internal.NativeMethods.GET_FILEEX_INFO_LEVELS.GetFileExInfoStandard, ref lpFileInformation);
      if (num != 0)
      {
        if (fileAttributesEx)
        {
          DateTime dateTime1 = DateTime.FromFileTimeUtc((long) fileAttributeData.ftLastWriteTimeHigh << 32 | (long) fileAttributeData.ftLastWriteTimeLow);
          DateTime dateTime2 = DateTime.FromFileTimeUtc((long) lpFileInformation.ftLastWriteTimeHigh << 32 | (long) lpFileInformation.ftLastWriteTimeLow);
          if (dateTime1.AddSeconds((double) maximumPermissibleBackupAgeInSeconds) > dateTime2)
            return false;
        }
      }
      else if (!fileAttributesEx)
        return false;
      return true;
    }

    public void CorruptTable()
    {
      using (SafeFileHandle writeHandle = this.GetWriteHandle())
      {
        using (FileStream fileStream = new FileStream(writeHandle, System.IO.FileAccess.Write))
        {
          fileStream.WriteByte(byte.MaxValue);
          fileStream.WriteByte(byte.MaxValue);
          fileStream.WriteByte(byte.MaxValue);
          fileStream.WriteByte(byte.MaxValue);
        }
      }
      this.PositionFile(true);
    }

    private void PositionFile(bool keepFile)
    {
      int num1 = 1;
      string slotOnePath = LocalMetadataTable.GetSlotOnePath(this.m_fileName);
      string slotTwoPath = LocalMetadataTable.GetSlotTwoPath(this.m_fileName);
      string slotThreePath = LocalMetadataTable.GetSlotThreePath(this.m_fileName);
      if (!keepFile)
        FileSpec.DeleteFile(slotThreePath);
      int lastWin32Error1;
      while (true)
      {
        FileSpec.DeleteFile(slotTwoPath);
        if (keepFile && !Microsoft.TeamFoundation.Common.Internal.NativeMethods.MoveFile(slotThreePath, slotTwoPath))
        {
          lastWin32Error1 = Marshal.GetLastWin32Error();
          switch (lastWin32Error1)
          {
            case 5:
            case 32:
            case 183:
              if (num1++ <= this.m_retryCount)
              {
                Thread.Sleep(num1 * num1);
                continue;
              }
              goto label_6;
            default:
              goto label_6;
          }
        }
        else
          goto label_7;
      }
label_6:
      throw new Win32Exception(lastWin32Error1);
label_7:
      int num2 = 1;
      int lastWin32Error2;
      while (true)
      {
        FileSpec.DeleteFile(slotOnePath);
        if (keepFile && !Microsoft.TeamFoundation.Common.Internal.NativeMethods.MoveFile(slotTwoPath, slotOnePath))
        {
          lastWin32Error2 = Marshal.GetLastWin32Error();
          switch (lastWin32Error2)
          {
            case 5:
            case 32:
            case 183:
              if (num2++ <= this.m_retryCount)
              {
                Thread.Sleep(num2 * num2);
                continue;
              }
              goto label_12;
            default:
              goto label_12;
          }
        }
        else
          break;
      }
      return;
label_12:
      throw new Win32Exception(lastWin32Error2);
    }

    private SafeFileHandle GetReadHandle(bool loadFromBackup)
    {
      int num = 1;
      string lpFileName = LocalMetadataTable.GetSlotOnePath(this.m_fileName);
      if (loadFromBackup)
        lpFileName = LocalMetadataTable.GetSlotBackupPath(this.m_fileName);
      SafeFileHandle file;
      int lastWin32Error;
      while (true)
      {
        file = Microsoft.TeamFoundation.Common.Internal.NativeMethods.CreateFile(lpFileName, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileAccess.GenericRead, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileShare.Delete, IntPtr.Zero, Microsoft.TeamFoundation.Common.Internal.NativeMethods.CreationDisposition.OpenExisting, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileAttributes.Normal, IntPtr.Zero);
        lastWin32Error = Marshal.GetLastWin32Error();
        if (file.IsInvalid)
        {
          switch (lastWin32Error)
          {
            case 2:
              goto label_11;
            case 3:
              if (num++ <= this.m_retryCount)
              {
                Directory.CreateDirectory(Path.GetDirectoryName(this.m_fileName));
                continue;
              }
              goto label_9;
            case 5:
            case 32:
              if (num++ <= this.m_retryCount)
              {
                Thread.Sleep(num * num);
                continue;
              }
              goto label_6;
            default:
              goto label_10;
          }
        }
        else
          goto label_11;
      }
label_6:
      throw new Win32Exception(lastWin32Error);
label_9:
      throw new Win32Exception(lastWin32Error);
label_10:
      throw new Win32Exception(lastWin32Error);
label_11:
      return file;
    }

    private SafeFileHandle GetWriteHandle()
    {
      int num = 1;
      string slotThreePath = LocalMetadataTable.GetSlotThreePath(this.m_fileName);
      SafeFileHandle file;
      int lastWin32Error;
      while (true)
      {
        FileSpec.DeleteFile(slotThreePath);
        file = Microsoft.TeamFoundation.Common.Internal.NativeMethods.CreateFile(slotThreePath, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileAccess.GenericWrite, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileShare.Delete, IntPtr.Zero, Microsoft.TeamFoundation.Common.Internal.NativeMethods.CreationDisposition.CreateAlways, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileAttributes.Normal, IntPtr.Zero);
        lastWin32Error = Marshal.GetLastWin32Error();
        if (file.IsInvalid)
        {
          if (lastWin32Error <= 5)
          {
            if (lastWin32Error != 3)
            {
              if (lastWin32Error != 5)
                break;
            }
            else
            {
              if (num++ <= this.m_retryCount)
              {
                Directory.CreateDirectory(FileSpec.GetDirectoryName(slotThreePath));
                continue;
              }
              break;
            }
          }
          else if (lastWin32Error != 32 && lastWin32Error != 183)
            break;
          if (num++ <= this.m_retryCount)
            Thread.Sleep(num * num);
          else
            break;
        }
        else
          goto label_11;
      }
      throw new Win32Exception(lastWin32Error);
label_11:
      return file;
    }

    public static string GetSlotOnePath(string fileName) => fileName + ".tf1";

    public static string GetSlotTwoPath(string fileName) => fileName + ".tf2";

    public static string GetSlotThreePath(string fileName) => fileName + ".tf3";

    public static string GetSlotBackupPath(string fileName) => fileName + ".tfb";

    internal Microsoft.TeamFoundation.Common.Internal.NativeMethods.WIN32_FILE_ATTRIBUTE_DATA SavedAttributes => this.m_savedAttributes;

    internal Microsoft.TeamFoundation.Common.Internal.NativeMethods.WIN32_FILE_ATTRIBUTE_DATA LoadAttributes => this.m_loadAttributes;

    public string FileName => this.m_fileName;
  }
}
