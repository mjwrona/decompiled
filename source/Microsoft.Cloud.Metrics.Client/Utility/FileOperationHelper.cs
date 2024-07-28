// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Utility.FileOperationHelper
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Cloud.Metrics.Client.Utility
{
  internal static class FileOperationHelper
  {
    private static readonly object LogId = Logger.CreateCustomLogId(nameof (FileOperationHelper));

    internal static bool CreateFolderIfNotExists(string folder)
    {
      if (!Directory.Exists(folder))
      {
        try
        {
          Directory.CreateDirectory(folder);
        }
        catch (Exception ex)
        {
          Logger.Log(LoggerLevel.Error, FileOperationHelper.LogId, nameof (CreateFolderIfNotExists), string.Format("Fail to create folder {0}. {1}", (object) folder, (object) ex));
          return false;
        }
      }
      return true;
    }

    internal static void SaveContentToFile(string path, string content)
    {
      try
      {
        File.WriteAllText(path, content);
      }
      catch (Exception ex)
      {
        Logger.Log(LoggerLevel.Error, FileOperationHelper.LogId, "SaveConfig", string.Format("Fail to save file {0}. {1}", (object) path, (object) ex));
        throw;
      }
    }

    internal static async Task SaveContentToFileAsync(string path, string content)
    {
      try
      {
        using (StreamWriter writer = new StreamWriter(path, false))
          await writer.WriteAsync(content).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        Logger.Log(LoggerLevel.Error, FileOperationHelper.LogId, nameof (SaveContentToFileAsync), string.Format("Fail to save file {0}. {1}", (object) path, (object) ex));
        throw;
      }
    }
  }
}
