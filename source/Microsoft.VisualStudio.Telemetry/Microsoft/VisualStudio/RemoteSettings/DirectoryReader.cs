// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.DirectoryReader
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal class DirectoryReader : IDirectoryReader
  {
    private readonly string path;
    private readonly string directoryName;
    private readonly bool markProcessed;
    private readonly IRemoteSettingsLogger logger;

    public DirectoryReader(
      string rootPath,
      string directoryName,
      bool markProcessed,
      int priority,
      IRemoteSettingsLogger logger)
    {
      this.path = Path.Combine(rootPath, directoryName);
      this.directoryName = directoryName;
      this.markProcessed = markProcessed;
      this.Priority = priority;
      this.logger = logger;
    }

    public int Priority { get; private set; }

    public IEnumerable<DirectoryReaderContext> ReadAllFiles()
    {
      this.logger.LogInfo("Reading all files from " + this.path);
      List<DirectoryReaderContext> directoryReaderContextList = new List<DirectoryReaderContext>();
      try
      {
        if (Directory.Exists(this.path))
        {
          foreach (string file in Directory.GetFiles(this.path, "*.json"))
          {
            this.logger.LogVerbose("Opening file for reading: " + file);
            try
            {
              string path = file;
              if (this.markProcessed)
              {
                this.logger.LogVerbose("Renaming file to .processed: " + file);
                string destFileName = file + ".processed";
                ReparsePointAware.MoveFile(file, destFileName);
                path = destFileName;
              }
              DirectoryReaderContext directoryReaderContext = new DirectoryReaderContext()
              {
                DirectoryName = this.directoryName,
                FileName = Path.GetFileNameWithoutExtension(file),
                Stream = (Stream) File.OpenRead(path)
              };
              directoryReaderContextList.Add(directoryReaderContext);
            }
            catch (Exception ex)
            {
              this.logger.LogError("Processing file failed", ex);
            }
          }
        }
      }
      catch (Exception ex)
      {
        this.logger.LogError("Checking directory failed", ex);
      }
      return (IEnumerable<DirectoryReaderContext>) directoryReaderContextList;
    }
  }
}
