// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.IisInstaller
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;
using System.Text;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public abstract class IisInstaller
  {
    protected IisInstaller(bool supportsIntegratedAuth, ITFLogger logger)
    {
      this.SupportsIntegratedAuth = supportsIntegratedAuth;
      this.Logger = logger ?? (ITFLogger) new NullLogger();
    }

    public static IisInstaller CreateInstaller(
      WindowsVersion windowsVersion,
      bool supportsIntegratedAuth,
      ITFLogger logger)
    {
      return (IisInstaller) new Iis80Installer(supportsIntegratedAuth, logger);
    }

    protected ITFLogger Logger { get; private set; }

    protected bool SupportsIntegratedAuth { get; private set; }

    protected abstract string[] GetInstallerFeatureList();

    protected abstract string[] GetRegisteredFeatureList();

    protected virtual string GetAdditionalOptions() => string.Empty;

    public int ConfigureIis()
    {
      this.Logger.Info(nameof (ConfigureIis));
      return this.ConfigureIisDism();
    }

    internal virtual string GetInstallerPath() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "dism.exe");

    public bool CheckIisRequiredFeaturesInstalled() => this.CheckIisComponentInstalled(this.GetRegisteredFeatureList());

    private int ConfigureIisDism()
    {
      this.Logger.Info("ConfigureIis");
      this.Logger.Info("Installing Iis");
      string[] installerFeatureList = this.GetInstallerFeatureList();
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("/Online /NoRestart /Enable-Feature ");
      foreach (string str in installerFeatureList)
      {
        this.Logger.Info("Feature: {0}", (object) str);
        stringBuilder.Append("/FeatureName:");
        stringBuilder.Append(str);
        stringBuilder.Append(" ");
      }
      string additionalOptions = this.GetAdditionalOptions();
      if (!string.IsNullOrEmpty(additionalOptions))
      {
        this.Logger.Info("additional options: {0}", (object) additionalOptions);
        stringBuilder.Append(additionalOptions);
      }
      string installerPath = this.GetInstallerPath();
      this.Logger.Info(installerPath);
      if (!File.Exists(installerPath))
        this.Logger.Error("Installer not found.");
      string str1 = stringBuilder.ToString();
      this.Logger.Info(str1);
      try
      {
        return ProcessHandler.RunExe(installerPath, str1, this.Logger).ExitCode;
      }
      catch (Exception ex)
      {
        this.Logger.Warning(ex);
        return 1237;
      }
    }

    private bool CheckIisComponentInstalled(string[] featureIds)
    {
      bool flag = true;
      foreach (string featureId in featureIds)
      {
        if (!this.CheckIisComponentInstalled(featureId))
          flag = false;
      }
      return flag;
    }

    private bool CheckIisComponentInstalled(string featureId)
    {
      if (OSDetails.Version == WindowsVersion.Future)
        this.Logger.Warning("CheckIisComponentInstalled called with future Windows version");
      bool flag = this.CheckIisComponentRegistered(featureId);
      if (!flag)
        this.Logger.Info("Feature {0} is not registered.", (object) featureId);
      return flag;
    }

    private bool CheckIisComponentRegistered(string featureId)
    {
      int num = RegistryHandler.ReadRegistryValue<int>(RegistryHandler.LocalMachineView, "Software\\Microsoft\\InetStp\\Components", featureId);
      this.Logger.Info("IIS Feature value: {0}={1}", (object) featureId, (object) num);
      return num == 1;
    }
  }
}
