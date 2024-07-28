// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Services.FileVersion
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.VisualStudio.Telemetry.Services
{
  internal sealed class FileVersion
  {
    public FileVersion(FileVersionInfo fileVersionInfo)
    {
      this.FileMajorPart = fileVersionInfo.FileMajorPart;
      this.FileMinorPart = fileVersionInfo.FileMinorPart;
      this.FileBuildPart = fileVersionInfo.FileBuildPart;
      this.FileRevisionPart = fileVersionInfo.FilePrivatePart;
    }

    public FileVersion(int major, int minor, int build, int revision)
    {
      this.FileMajorPart = major;
      this.FileMinorPart = minor;
      this.FileBuildPart = build;
      this.FileRevisionPart = revision;
    }

    public static bool TryParse(string fileVersion, out FileVersion value)
    {
      if (fileVersion != null)
      {
        string[] strArray = fileVersion.Split('.');
        int result1;
        int result2;
        int result3;
        int result4;
        if (strArray.Length == 4 && int.TryParse(strArray[0], out result1) && int.TryParse(strArray[1], out result2) && int.TryParse(strArray[2], out result3) && int.TryParse(strArray[3], out result4))
        {
          value = new FileVersion(result1, result2, result3, result4);
          return true;
        }
      }
      value = (FileVersion) null;
      return false;
    }

    public int FileMajorPart { get; private set; }

    public int FileMinorPart { get; private set; }

    public int FileBuildPart { get; private set; }

    public int FileRevisionPart { get; private set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}", (object) this.FileMajorPart, (object) this.FileMinorPart, (object) this.FileBuildPart, (object) this.FileRevisionPart);
  }
}
