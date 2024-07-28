// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.AspNetRegIis
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.IO;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public static class AspNetRegIis
  {
    public static bool HasAspNetBeenRegistered(string frameworkVersionFilter, ITFLogger logger)
    {
      if (!string.Equals(frameworkVersionFilter, "v4.0*", StringComparison.Ordinal))
        throw new ArgumentOutOfRangeException("Incorrect framework version filter '" + frameworkVersionFilter + "'");
      string xpath = "configuration/system.webServer/globalModules/add[@name='ManagedEngineV4.0_64bit']";
      string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "inetsrv\\config\\applicationHost.config");
      logger.Info("Loading " + path);
      if (!File.Exists(path))
        throw new FileNotFoundException("Failed to find " + path + ", Insure IIS is installed");
      XmlDocument xmlDocument = new XmlDocument();
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        IgnoreComments = true,
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = (XmlResolver) null
      };
      using (FileStream input = File.Open(path, FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
      {
        using (XmlReader reader = XmlReader.Create((Stream) input, settings))
          xmlDocument.Load(reader);
      }
      logger.Info("Checking for " + xpath);
      if (xmlDocument.SelectSingleNode(xpath) == null)
      {
        logger.Info("Not found");
        return false;
      }
      logger.Info("Found");
      return true;
    }

    public static int RunRegisterAspNet(string frameworkVersionFilter, ITFLogger logger) => ProcessHandler.RunExe(AspNetRegIis.GetPathToAspNetRegIis(frameworkVersionFilter), "-iru -enable", logger).ExitCode;

    public static int RunGrantMetabaseAccess(
      string serviceGroup,
      string frameworkVersionFilter,
      ITFLogger logger)
    {
      logger.Info("--- RunGrantMetabaseAccess ---");
      CommandLineBuilder args = new CommandLineBuilder();
      args.Append("-ga");
      if (EnvironmentHandler.IsDomainController())
        args.Append(serviceGroup);
      else
        args.AppendFormat("{0}\\{1}", (object) UserNameUtil.NetBiosName, (object) serviceGroup);
      return ProcessHandler.RunExe(AspNetRegIis.GetPathToAspNetRegIis(frameworkVersionFilter), args, logger).ExitCode;
    }

    private static string GetPathToAspNetRegIis(string frameworkVersionFilter) => NetFramework.GetFrameworkFilePath(frameworkVersionFilter, "aspnet_regiis.exe");
  }
}
