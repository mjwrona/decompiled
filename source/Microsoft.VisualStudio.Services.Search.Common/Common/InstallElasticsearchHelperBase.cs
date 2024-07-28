// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.InstallElasticsearchHelperBase
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Globalization;
using System.IO;
using System.Management.Automation.Runspaces;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public abstract class InstallElasticsearchHelperBase : PowerShellCommandExecutor
  {
    public bool ConfigureAndInstallElasticsearch(
      ElasticsearchConfigurationData configData,
      string modulePath,
      string logFilePath)
    {
      if (configData == null)
        throw new ArgumentNullException(nameof (configData));
      this.CreateLogFile(logFilePath);
      if (!this.ImportModulesFromPath(modulePath))
        throw new SearchServiceException("Failed to import required modules.");
      if (!this.ConfigureElasticsearch(configData))
        throw new SearchServiceException("Failed to configure Elasticsearch on the machine.");
      this.AppendTimeStampToFileName(logFilePath);
      return true;
    }

    public virtual bool ConfigureElasticsearch(ElasticsearchConfigurationData configData)
    {
      if (configData == null)
        throw new ArgumentNullException(nameof (configData));
      try
      {
        this.RemoveElasticsearch(configData);
        if (configData.Operation == OperationType.Install)
          this.InstallElasticsearch(configData);
        return true;
      }
      catch (Exception ex)
      {
        this.WriteLine(ex.ToString());
        return false;
      }
    }

    public bool RemoveElasticsearch(ElasticsearchConfigurationData configData)
    {
      if (configData == null)
        throw new ArgumentNullException(nameof (configData));
      this.WriteLine("Removing Elasticsearch...");
      Command command = new Command(configData.ConfigurationScriptPath);
      command.Parameters.Add(new CommandParameter("Operation", (object) "Remove"));
      command.Parameters.Add(new CommandParameter("RemovePreviousESData", (object) configData.RemoveElasticsearchData));
      command.Parameters.Add(new CommandParameter("ServiceName", (object) configData.ElasticsearchServiceName));
      command.Parameters.Add(new CommandParameter("IgnoreEnvironmentVariable", (object) true));
      command.Parameters.Add(new CommandParameter("Quiet", (object) true));
      this.AddCommadToPowershellInstance(command);
      if (this.ExecutePowershellCommand() != null)
        return true;
      this.WriteLine("Failed to remove Elasticsearch.");
      return false;
    }

    public bool InstallElasticsearch(ElasticsearchConfigurationData configData)
    {
      if (configData == null)
        throw new ArgumentNullException(nameof (configData));
      this.WriteLine("Installing Elasticsearch...");
      Command command = new Command(configData.ConfigurationScriptPath);
      command.Parameters.Add(new CommandParameter("Operation", (object) configData.Operation));
      command.Parameters.Add(new CommandParameter("TFSSearchInstallPath", (object) configData.ElasticsearchInstallPath));
      command.Parameters.Add(new CommandParameter("TFSSearchIndexPath", (object) configData.ElasticsearchIndexPath));
      command.Parameters.Add(new CommandParameter("Quiet", (object) true));
      command.Parameters.Add(new CommandParameter("ServiceName", (object) configData.ElasticsearchServiceName));
      command.Parameters.Add(new CommandParameter("IgnoreEnvironmentVariable", (object) true));
      command.Parameters.Add(new CommandParameter("Port", (object) configData.ElasticsearchPort));
      command.Parameters.Add(new CommandParameter("ClusterName", (object) configData.ClusterName));
      command.Parameters.Add(new CommandParameter("User", (object) configData.ElasticsearchUser));
      command.Parameters.Add(new CommandParameter("Password", (object) configData.ElasticsearchPassword));
      command.Parameters.Add(new CommandParameter("SecondaryPassword", (object) "password"));
      this.AddCommadToPowershellInstance(command);
      if (this.ExecutePowershellCommand(true) != null)
        return true;
      this.WriteLine("Failed to install Elasticsearch.");
      return false;
    }

    protected virtual void CreateLogFile(string filePath)
    {
      DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filePath));
      if (!directoryInfo.Exists)
        directoryInfo.Create();
      FileInfo fileInfo = new FileInfo(filePath);
      if (fileInfo.Exists)
        return;
      fileInfo.Create().Dispose();
    }

    protected virtual void AppendTimeStampToFileName(string fileName)
    {
      string destFileName = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + "-" + DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture).Replace(':', '-') + Path.GetExtension(fileName));
      this.WriteLine("Creating log file {0}.", (object) destFileName);
      new FileInfo(fileName).MoveTo(destFileName);
    }
  }
}
