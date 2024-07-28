// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.DismHandler
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class DismHandler
  {
    private const string c_cimv2Scope = "root\\CIMV2";
    private const int c_retryCount = 3;
    private const int c_retryIntervalSeconds = 30;
    private readonly ITFLogger m_logger;
    private readonly string m_dismPath;

    public DismHandler(ITFLogger logger)
    {
      this.m_logger = logger ?? (ITFLogger) new NullLogger();
      this.m_dismPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "dism.exe");
    }

    public bool AreFeaturesInstalled(string[] featureNames)
    {
      ArgumentUtility.CheckForNull<string[]>(featureNames, nameof (featureNames));
      this.m_logger.Info("Checking if the following feature(s) installed: {0}", (object) string.Join(",", featureNames));
      if (featureNames.Length == 0)
        return true;
      string[] installedFeatures = this.GetInstalledFeatures();
      if (installedFeatures.Length < featureNames.Length)
        return false;
      foreach (string featureName in featureNames)
      {
        if (!((IEnumerable<string>) installedFeatures).Contains<string>(featureName, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        {
          this.m_logger.Info("{0} feature not found.", (object) featureName);
          return false;
        }
      }
      this.m_logger.Info("All features are installed.");
      return true;
    }

    public string[] GetInstalledFeatures()
    {
      this.m_logger.Info("Getting installed features...");
      using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT Name FROM Win32_OptionalFeature WHERE InstallState = 1"))
      {
        using (ManagementObjectCollection objectCollection = managementObjectSearcher.Get())
        {
          string[] installedFeatures = new string[objectCollection.Count];
          int num = 0;
          foreach (ManagementObject managementObject in objectCollection)
            installedFeatures[num++] = (string) managementObject["Name"];
          this.m_logger.Info("Found {0} installed features: {1}", (object) installedFeatures.Length, (object) string.Join(",", installedFeatures));
          return installedFeatures;
        }
      }
    }

    public int EnableFeatures(string[] featureNames)
    {
      try
      {
        if (this.AreFeaturesInstalled(featureNames))
        {
          this.m_logger.Info("Specified features are already installed.");
          return 0;
        }
      }
      catch (Exception ex)
      {
        this.m_logger.Info(ex.Message);
      }
      string args = this.GetInstallerArguments(featureNames);
      int returnCode = 1;
      int tries = 1;
      try
      {
        new RetryManager(3, TimeSpan.FromSeconds(30.0), (Action<Exception>) (ex => this.m_logger.Warning(ex))).Invoke((Action) (() =>
        {
          ProcessOutput output = ProcessHandler.RunExe(this.m_dismPath, args, this.m_logger);
          returnCode = output.ExitCode;
          if (returnCode == 0)
            return;
          this.m_logger.Info("dism returned {0}", (object) returnCode);
          this.LogOutput(output);
          if (returnCode != 3018 && returnCode != 3017 && returnCode != 3010 && returnCode != 3011)
            throw new ConfigurationException(string.Format("Dism configuration failed with error {0} on attempt {1}", (object) returnCode, (object) tries++));
        }));
      }
      catch (ConfigurationException ex)
      {
        this.m_logger.Error((Exception) ex);
      }
      return returnCode;
    }

    private string GetInstallerArguments(string[] featureNames)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("/Online /NoRestart /Enable-Feature ");
      stringBuilder.Append((object) DismHandler.GetFeatureList(featureNames));
      if (Environment.OSVersion.Version >= new Version(6, 2))
        stringBuilder.Append("/all");
      return stringBuilder.ToString();
    }

    private static StringBuilder GetFeatureList(string[] featureNames)
    {
      StringBuilder featureList = new StringBuilder();
      foreach (string featureName in featureNames)
      {
        featureList.Append("/FeatureName:");
        featureList.Append(featureName);
        featureList.Append(" ");
      }
      return featureList;
    }

    private void LogOutput(ProcessOutput output)
    {
      foreach (string matchingLine in output.GetMatchingLines(OutputType.StdOut))
        this.m_logger.Info(matchingLine);
    }
  }
}
