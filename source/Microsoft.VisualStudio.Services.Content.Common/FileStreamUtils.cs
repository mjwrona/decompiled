// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.FileStreamUtils
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public class FileStreamUtils
  {
    public const int RecommendedFileStreamBufferSize = 1;

    public static FileStream OpenFileStreamForAsync(
      string filePath,
      FileMode mode,
      FileAccess fileAccess,
      FileShare fileShare,
      FileOptions extraOptions = FileOptions.None)
    {
      try
      {
        return new FileStream(filePath, mode, fileAccess, fileShare, 1, FileOptions.Asynchronous | FileOptions.SequentialScan | extraOptions);
      }
      catch (PathTooLongException ex)
      {
        throw new PathTooLongException("Path is too long: '" + filePath + "'", (Exception) ex);
      }
    }
  }
}
