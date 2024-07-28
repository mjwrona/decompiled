// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Platform.PlatformImplementation
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Platform
{
  internal class PlatformImplementation : IPlatform
  {
    public IDictionary<string, object> GetApplicationSettings() => throw new NotImplementedException();

    public string ReadConfigurationXml()
    {
      string path = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), "ApplicationInsights.config");
      if (!File.Exists(path))
        path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ApplicationInsights.config");
      return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
    }

    public ExceptionDetails GetExceptionDetails(
      Exception exception,
      ExceptionDetails parentExceptionDetails)
    {
      return ExceptionConverter.ConvertToExceptionDetails(exception, parentExceptionDetails);
    }
  }
}
