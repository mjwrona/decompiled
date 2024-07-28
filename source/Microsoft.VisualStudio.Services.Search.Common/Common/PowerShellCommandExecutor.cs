// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.PowerShellCommandExecutor
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public abstract class PowerShellCommandExecutor : IPowerShellCommandExecutor, IDisposable
  {
    private readonly PowerShell m_powershellInstance;
    private bool m_disposedValue;

    protected abstract void WriteLine(string messageFormat, params object[] args);

    protected abstract void WriteLine([Localizable(false)] string text);

    protected abstract void WriteError(string messageFormat, params object[] args);

    protected abstract void WriteException(Exception exception);

    protected abstract void WriteError([Localizable(false)] string text);

    protected PowerShellCommandExecutor() => this.m_powershellInstance = PowerShell.Create();

    public virtual bool ImportModulesFromPath(string path)
    {
      if (string.IsNullOrWhiteSpace(path))
        throw new ArgumentNullException(path);
      try
      {
        this.m_powershellInstance.Commands.AddCommand("Set-Location").AddArgument((object) path);
        this.WriteLine("Importing modules from path {0}...", (object) path);
        foreach (string file in Directory.GetFiles(path + "\\modules", "*.psm1"))
        {
          this.m_powershellInstance.Commands.AddCommand("Import-Module").AddArgument((object) file);
          if (this.ExecutePowershellCommand() == null)
          {
            this.WriteLine("Failed to import module {0}...", (object) file);
            return false;
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        this.WriteException(ex);
        return false;
      }
    }

    public void AddCommadToPowershellInstance(Command command) => this.m_powershellInstance.Commands.AddCommand(command);

    public virtual Collection<PSObject> ExecutePowershellCommand() => this.ExecutePowershellCommand(false);

    public virtual Collection<PSObject> ExecutePowershellCommand(bool containSecrets)
    {
      try
      {
        foreach (Command command in (Collection<Command>) this.m_powershellInstance.Commands.Commands)
        {
          if (command.Parameters.Count > 0)
          {
            this.WriteLine("\nExecuting command {0}, with parameters:", (object) command.CommandText.ToString());
            if (containSecrets)
            {
              this.WriteLine("\t Hiding parameters as they might contain secrets.");
            }
            else
            {
              foreach (CommandParameter parameter in (Collection<CommandParameter>) command.Parameters)
                this.WriteLine("\t{0}: {1}", (object) parameter.Name, (object) parameter.Value.ToString());
            }
          }
          else
            this.WriteLine("\nExecuting command {0}", (object) command.CommandText.ToString());
        }
        Collection<PSObject> collection = this.m_powershellInstance.Invoke();
        if (this.m_powershellInstance.Streams.Verbose.Count > 0)
        {
          foreach (object obj in this.m_powershellInstance.Streams.Verbose)
            this.WriteLine(obj.ToString());
        }
        if (this.m_powershellInstance.Streams.Warning.Count > 0)
        {
          foreach (object obj in this.m_powershellInstance.Streams.Warning)
            this.WriteLine(obj.ToString());
        }
        if (this.m_powershellInstance.Streams.Error.Count > 0)
        {
          foreach (object obj in this.m_powershellInstance.Streams.Error)
            this.WriteError(obj.ToString());
        }
        this.m_powershellInstance.Commands.Clear();
        return collection;
      }
      catch (Exception ex)
      {
        this.WriteException(ex);
        return (Collection<PSObject>) null;
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.m_disposedValue)
        return;
      if (disposing && this.m_powershellInstance != null)
        this.m_powershellInstance.Dispose();
      this.m_disposedValue = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }
  }
}
