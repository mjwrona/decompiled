// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteControl.FileReader
// Assembly: Microsoft.VisualStudio.RemoteControl, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4D9D0761-3208-49DD-A9E2-BF705DBE6B5D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.RemoteControl.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.RemoteControl
{
  internal class FileReader : IFileReader
  {
    private string filePath;
    private string fileDirectory;
    private const int MaxCharFilePath = 248;

    public FileReader(string filePath)
    {
      filePath.RequiresArgumentNotNull<string>(nameof (filePath));
      this.filePath = filePath.Length <= 248 ? filePath : throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "File path can be a maximum of {0} characters", new object[1]
      {
        (object) 248
      }), filePath);
      this.fileDirectory = Path.GetDirectoryName(filePath);
    }

    public Stream ReadFile() => (Stream) File.OpenRead(this.filePath);
  }
}
