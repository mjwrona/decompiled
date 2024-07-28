// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.Iis80Installer
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class Iis80Installer : IisInstaller
  {
    public Iis80Installer(bool supportsIntegratedAuth, ITFLogger logger)
      : base(supportsIntegratedAuth, logger)
    {
    }

    protected override string GetAdditionalOptions() => "/all ";

    protected override string[] GetInstallerFeatureList()
    {
      List<string> stringList = new List<string>(10);
      stringList.Add("IIS-ManagementScriptingTools");
      stringList.Add("IIS-HttpCompressionStatic");
      stringList.Add("IIS-HttpCompressionDynamic");
      if (this.SupportsIntegratedAuth)
        stringList.Add("IIS-WindowsAuthentication");
      else
        stringList.Add("IIS-BasicAuthentication");
      stringList.Add("NetFx4Extended-ASPNET45");
      stringList.Add("IIS-NetFxExtensibility45");
      stringList.Add("IIS-ASPNET45");
      stringList.Add("IIS-StaticContent");
      stringList.Add("IIS-WebSockets");
      return stringList.ToArray();
    }

    protected override string[] GetRegisteredFeatureList()
    {
      List<string> stringList = new List<string>(10);
      stringList.Add("NetFxExtensibility45");
      stringList.Add("ASPNET45");
      stringList.Add("StaticContent");
      if (!OSDetails.IsServerCore)
        stringList.Add("ManagementConsole");
      stringList.Add("ManagementScriptingTools");
      if (this.SupportsIntegratedAuth)
        stringList.Add("WindowsAuthentication");
      else
        stringList.Add("BasicAuthentication");
      stringList.Add("HttpCompressionStatic");
      stringList.Add("HttpCompressionDynamic");
      stringList.Add("WebSockets");
      return stringList.ToArray();
    }
  }
}
