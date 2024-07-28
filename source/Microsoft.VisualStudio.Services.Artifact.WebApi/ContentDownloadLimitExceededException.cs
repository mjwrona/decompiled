// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Artifact.WebApi.ContentDownloadLimitExceededException
// Assembly: Microsoft.VisualStudio.Services.Artifact.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D39C0B4C-25E7-402A-9BC9-E3DFE7654881
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Artifact.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Artifact.WebApi
{
  [Serializable]
  public class ContentDownloadLimitExceededException : VssServiceException
  {
    public ContentDownloadLimitExceededException(string message, Exception ex)
      : base(message, ex)
    {
    }

    public ContentDownloadLimitExceededException(string message)
      : this(message, (Exception) null)
    {
    }

    public static ContentDownloadLimitExceededException CreateNumOfFileLimit(
      int numOfFiles,
      int numOfFilesLimit)
    {
      return new ContentDownloadLimitExceededException("The content download exceeds the limit to the number of total files. " + Environment.NewLine + "Number of files: " + numOfFiles.ToString("N0") + ", Number of files limit: " + numOfFilesLimit.ToString("N0"));
    }

    public static ContentDownloadLimitExceededException CreateDownloadSizeLimit(
      long downloadSize,
      long downloadSizeLimit)
    {
      return new ContentDownloadLimitExceededException("The content download exceeds the size that is set for your account. " + Environment.NewLine + "Download size: " + downloadSize.ToString("N0") + " bytes, Download size limit: " + downloadSizeLimit.ToString("N0") + " bytes.");
    }
  }
}
