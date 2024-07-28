// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.BuildInfoConfigComponentVersionContextInitializer
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.DataContracts;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility
{
  public class BuildInfoConfigComponentVersionContextInitializer : IContextInitializer
  {
    private string version;

    public void Initialize(TelemetryContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (!string.IsNullOrEmpty(context.Component.Version))
        return;
      string str = LazyInitializer.EnsureInitialized<string>(ref this.version, new Func<string>(this.GetVersion));
      context.Component.Version = str;
    }

    protected virtual XElement LoadBuildInfoConfig()
    {
      XElement xelement = (XElement) null;
      try
      {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BuildInfo.config");
        if (File.Exists(path))
        {
          using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
          {
            xelement = XDocument.Load((Stream) fileStream).Root;
            CoreEventSource.Log.LogVerbose("[BuildInfoConfigComponentVersionContextInitializer] File loaded." + path);
          }
        }
        else
          CoreEventSource.Log.LogVerbose("[BuildInfoConfigComponentVersionContextInitializer] No file." + path);
      }
      catch (XmlException ex)
      {
        CoreEventSource.Log.BuildInfoConfigBrokenXmlError(ex.Message);
      }
      return xelement;
    }

    private string GetVersion()
    {
      XElement xelement1 = this.LoadBuildInfoConfig();
      if (xelement1 == null)
        return "Unknown";
      XElement xelement2 = xelement1.Descendants().Where<XElement>((Func<XElement, bool>) (item => item.Name.LocalName == "Build")).Descendants<XElement>().Where<XElement>((Func<XElement, bool>) (item => item.Name.LocalName == "MSBuild")).Descendants<XElement>().SingleOrDefault<XElement>((Func<XElement, bool>) (item => item.Name.LocalName == "BuildLabel"));
      return xelement2 == null || string.IsNullOrEmpty(xelement2.Value) ? "Unknown" : xelement2.Value;
    }
  }
}
