// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.LegacyApi
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class LegacyApi : ILegacyApi
  {
    private static readonly object userIdCalculation = new object();
    private const string MachineIdRegPath = "SOFTWARE\\Microsoft\\SQMClient";
    private const string MachineIdRegKey = "MachineId";
    private const string UserIdRegPath = "SOFTWARE\\Microsoft\\SQMClient";
    private const string UserIdRegKey = "UserId";
    private IRegistryTools registryTools;

    public LegacyApi(IRegistryTools registryTools)
    {
      if (registryTools == null)
        registryTools = Platform.IsWindows ? (IRegistryTools) new RegistryTools() : (IRegistryTools) new FileBasedRegistryTools();
      this.registryTools = registryTools;
    }

    public Guid ReadSharedMachineId()
    {
      Guid guid = new Guid();
      string g = this.ReadMachineIdFromRegistry(this.registryTools, "SOFTWARE\\Microsoft\\SQMClient", "MachineId");
      if (g != null)
      {
        try
        {
          guid = new Guid(g);
        }
        catch (FormatException ex)
        {
        }
        catch (OverflowException ex)
        {
        }
      }
      return guid;
    }

    public bool SetSharedMachineId(Guid machineId) => this.SaveMachineIdToRegistry(this.registryTools, "SOFTWARE\\Microsoft\\SQMClient", "MachineId", LegacyApi.FormatGuid(machineId));

    public Guid ReadSharedUserId()
    {
      Guid guid = new Guid();
      lock (LegacyApi.userIdCalculation)
      {
        string fromCurrentUserRoot = (string) this.registryTools.GetRegistryValueFromCurrentUserRoot("SOFTWARE\\Microsoft\\SQMClient", "UserId");
        if (fromCurrentUserRoot != null)
        {
          try
          {
            guid = new Guid(fromCurrentUserRoot);
          }
          catch (FormatException ex)
          {
          }
          catch (OverflowException ex)
          {
          }
        }
        if (guid == new Guid())
        {
          guid = Guid.NewGuid();
          if (!this.registryTools.SetRegistryFromCurrentUserRoot("SOFTWARE\\Microsoft\\SQMClient", "UserId", (object) LegacyApi.FormatGuid(guid)))
            guid = Guid.Empty;
        }
      }
      return guid;
    }

    protected virtual string ReadMachineIdFromRegistry(
      IRegistryTools registry,
      string regPath,
      string regKey)
    {
      return (string) registry.GetRegistryValueFromLocalMachineRoot(regPath, regKey, true, (object) null);
    }

    protected virtual bool SaveMachineIdToRegistry(
      IRegistryTools registry,
      string regPath,
      string regKey,
      string machineId)
    {
      return registry.SetRegistryFromLocalMachineRoot(regPath, regKey, (object) machineId, true);
    }

    private static string FormatGuid(Guid guid) => guid.ToString("B").ToUpper();
  }
}
