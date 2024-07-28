// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.MachineInformationProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class MachineInformationProvider : IMachineInformationProvider
  {
    private static readonly object machineIdCalculationLock = new object();
    private readonly Lazy<Guid> machineId;
    private readonly ILegacyApi legacyApi;
    private readonly IUserInformationProvider userInformationProvider;
    private readonly IMACInformationProvider macInformationProvider;

    public MachineInformationProvider(
      ILegacyApi legacyApi,
      IUserInformationProvider userInformationProvider,
      IMACInformationProvider macInformationProvider)
    {
      legacyApi.RequiresArgumentNotNull<ILegacyApi>(nameof (legacyApi));
      userInformationProvider.RequiresArgumentNotNull<IUserInformationProvider>(nameof (userInformationProvider));
      macInformationProvider.RequiresArgumentNotNull<IMACInformationProvider>(nameof (macInformationProvider));
      this.legacyApi = legacyApi;
      this.userInformationProvider = userInformationProvider;
      this.macInformationProvider = macInformationProvider;
      this.machineId = new Lazy<Guid>((Func<Guid>) (() => this.CalculateMachineId()), LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public Guid MachineId => this.machineId.Value;

    private Guid CalculateMachineId()
    {
      Guid machineId;
      lock (MachineInformationProvider.machineIdCalculationLock)
      {
        machineId = this.legacyApi.ReadSharedMachineId();
        if (machineId == new Guid())
        {
          string macAddressHash = this.macInformationProvider.GetMACAddressHash();
          if (macAddressHash != null)
          {
            machineId = MachineInformationProvider.ConvertHexHashToGuid(macAddressHash);
            this.legacyApi.SetSharedMachineId(machineId);
          }
          else
          {
            machineId = Guid.NewGuid();
            if (!this.legacyApi.SetSharedMachineId(machineId))
              machineId = this.userInformationProvider.UserId;
          }
        }
      }
      return machineId;
    }

    private static Guid ConvertHexHashToGuid(string hex)
    {
      try
      {
        return new Guid(hex.Substring(0, 32));
      }
      catch
      {
      }
      return Guid.Empty;
    }
  }
}
