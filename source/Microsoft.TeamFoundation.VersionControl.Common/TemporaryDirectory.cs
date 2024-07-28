// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.TemporaryDirectory
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.ComponentModel;
using System.IO;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TemporaryDirectory : IDisposable
  {
    private string m_path;

    public TemporaryDirectory()
    {
      string tempFileName = FileSpec.GetTempFileName();
      FileSpec.DeleteFile(tempFileName);
      Directory.CreateDirectory(tempFileName);
      this.m_path = Directory.Exists(tempFileName) ? tempFileName : throw new DirectoryNotFoundException(tempFileName);
    }

    public string Path => this.m_path != null ? this.m_path : throw new ObjectDisposedException(this.ToString());

    public void Dispose()
    {
      try
      {
        if (this.m_path != null)
        {
          if (Directory.Exists(this.m_path))
            FileSpec.DeleteDirectory(this.m_path, true);
        }
      }
      catch (Exception ex)
      {
      }
      this.m_path = (string) null;
      GC.SuppressFinalize((object) this);
    }
  }
}
