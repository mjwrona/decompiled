// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.IisController
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class IisController
  {
    public IisController(ITFLogger logger) => this.Logger = logger ?? (ITFLogger) new NullLogger();

    protected ITFLogger Logger { get; private set; }

    public bool Start() => this.Control(IisController.IisControl.Start);

    public bool Stop() => this.Control(IisController.IisControl.Stop);

    public bool Restart() => this.Control(IisController.IisControl.Restart);

    private bool Control(IisController.IisControl action)
    {
      int num1 = 1;
      bool flag1 = true;
      bool flag2 = false;
      string str = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "iisreset.exe");
      if (!File.Exists(str))
        throw new ConfigurationException(ConfigurationResources.ResetIISFileNotFound());
      string args;
      switch (action)
      {
        case IisController.IisControl.Start:
          this.Logger.Info(ConfigurationResources.StartIIS());
          args = "/start";
          break;
        case IisController.IisControl.Stop:
          this.Logger.Info(ConfigurationResources.StopIIS());
          args = "/stop";
          break;
        default:
          this.Logger.Info(ConfigurationResources.ResetIIS());
          args = string.Empty;
          break;
      }
      while (flag1)
      {
        this.Logger.Info("IISReset attempt {0} of {1}", (object) num1, (object) 3);
        try
        {
          ProcessOutput processOutput = ProcessHandler.RunExe(str, args, this.Logger);
          if (processOutput.ExitCode == 0)
          {
            flag1 = false;
            flag2 = true;
          }
          else
            this.Logger.Warning("IISReset Failed with exit code: {0}", (object) processOutput.ExitCode);
        }
        catch (Win32Exception ex)
        {
          if (num1 >= 3)
          {
            this.Logger.Error("IISReset has failed {0} times.  No further attempts will be made.  Configuration may fail if IIS is not functioning properly.", (object) num1);
            throw new ConfigurationException(ConfigurationResources.FailedToResetIIS(), (Exception) ex);
          }
          this.Logger.Warning("Failed to reset IIS: {0}", (object) ex.Message);
          this.Logger.Warning(ex.ToString());
        }
        if (flag1)
        {
          if (num1 >= 3)
          {
            this.Logger.Error("IISReset has failed {0} times.  No further attempts will be made.  Configuration may fail if IIS is not functioning properly.", (object) num1);
            flag1 = false;
          }
          else
          {
            int num2 = num1 * 10;
            this.Logger.Info("IISReset will be attempted again.  Sleeping {0} seconds to let any pending operations finish", (object) num2);
            Thread.Sleep(num2 * 1000);
            ++num1;
          }
        }
      }
      return flag2;
    }

    private enum IisControl
    {
      Start,
      Stop,
      Restart,
    }
  }
}
