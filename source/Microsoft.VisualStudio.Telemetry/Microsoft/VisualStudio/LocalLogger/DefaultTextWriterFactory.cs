// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.LocalLogger.DefaultTextWriterFactory
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Services;
using System.IO;

namespace Microsoft.VisualStudio.LocalLogger
{
  internal sealed class DefaultTextWriterFactory : ITextWriterFactory
  {
    public TextWriter CreateTextWriter(string fullPathName)
    {
      StreamWriter text = ReparsePointAware.CreateText(fullPathName);
      text.AutoFlush = true;
      return TextWriter.Synchronized((TextWriter) text);
    }
  }
}
