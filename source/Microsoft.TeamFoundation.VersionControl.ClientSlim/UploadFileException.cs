// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Client.UploadFileException
// Assembly: Microsoft.TeamFoundation.VersionControl.ClientSlim, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CF6BF9DB-38AD-4731-862B-31BA91580FFB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.ClientSlim.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Client
{
  [Serializable]
  public class UploadFileException : VersionControlException
  {
    public UploadFileException(Exception firstException)
      : base(firstException.Message)
    {
      this.Errors = new List<Exception>();
      this.Errors.Add(firstException);
    }

    public List<Exception> Errors { get; set; }
  }
}
