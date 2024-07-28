// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.Core.SshOptions
// Assembly: Microsoft.TeamFoundation.Ssh.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3DF8FBEE-AA1B-4659-8650-E7C7E1E085EB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Ssh.Server.Core
{
  public class SshOptions
  {
    public static readonly RegistryQuery RegistryQuery = (RegistryQuery) "/Configuration/SshServer/**";

    public SshOptions(
      TimeSpan? sessionTimeout = null,
      int? maxConcurrentConnections = null,
      int? port = null,
      bool? enabled = null,
      KexInitOptions kexInitOptions = null)
    {
      this.SessionTimeout = sessionTimeout ?? TimeSpan.FromMinutes(5.0);
      int? nullable = maxConcurrentConnections;
      this.MaxConcurrentConnections = nullable ?? 500;
      nullable = port;
      this.Port = nullable ?? 22;
      this.Enabled = enabled.GetValueOrDefault();
      this.KexInitOptions = kexInitOptions ?? new KexInitOptions();
    }

    public static SshOptions FromRegistry(IVssRequestContext rc)
    {
      RegistryEntryCollection entries = rc.GetService<IVssRegistryService>().ReadEntries(rc, SshOptions.RegistryQuery);
      RegistryEntry entry;
      return new SshOptions(entries.TryGetValue("SessionTimeoutSeconds", out entry) ? new TimeSpan?(TimeSpan.FromSeconds((double) entry.GetValue<int>())) : new TimeSpan?(), entries.GetValueFromPath<int?>("MaxConcurrentConnections", new int?()), entries.GetValueFromPath<int?>("Port", new int?()), entries.GetValueFromPath<bool?>("Enabled", new bool?()), KexInitOptions.FromRegistry(entries));
    }

    public TimeSpan SessionTimeout { get; }

    public int MaxConcurrentConnections { get; }

    public int Port { get; }

    public bool Enabled { get; }

    public KexInitOptions KexInitOptions { get; }
  }
}
