// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessPackageFileTooLargeException
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using System;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  [Serializable]
  public class ProcessPackageFileTooLargeException : ProcessServiceException
  {
    public ProcessPackageFileTooLargeException(long uploadSize, long maxSize)
      : base(Resources.ProcessTemplateUploadTooLarge((object) uploadSize, (object) maxSize), 402611)
    {
      this.UploadSize = uploadSize;
      this.MaxSize = maxSize;
    }

    public long MaxSize { get; private set; }

    public long UploadSize { get; private set; }
  }
}
