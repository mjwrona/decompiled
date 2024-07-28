// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.Core.KexInitOptions
// Assembly: Microsoft.TeamFoundation.Ssh.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3DF8FBEE-AA1B-4659-8650-E7C7E1E085EB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Ssh.Server.Core
{
  public class KexInitOptions
  {
    public KexInitOptions(
      string kex_algorithms = null,
      string server_host_key_algorithms = null,
      string encryption_algorithms = null,
      string mac_algorithms = null,
      string compression_algorithms = null)
    {
      this.kex_algorithms = kex_algorithms ?? "diffie-hellman-group-exchange-sha256,diffie-hellman-group14-sha1,diffie-hellman-group1-sha1";
      this.server_host_key_algorithms = server_host_key_algorithms ?? "ssh-rsa,rsa-sha2-256,rsa-sha2-512";
      this.encryption_algorithms = encryption_algorithms ?? "aes256-cbc,aes192-cbc,aes128-cbc,aes128-ctr,aes256-ctr";
      this.mac_algorithms = mac_algorithms ?? "hmac-sha2-512,hmac-sha2-256";
      this.compression_algorithms = compression_algorithms ?? "none";
    }

    public static KexInitOptions FromRegistry(RegistryEntryCollection entries) => new KexInitOptions(entries.GetValueFromPath<string>("KexInitOptions/kex_algorithms", (string) null), entries.GetValueFromPath<string>("KexInitOptions/server_host_key_algorithms", (string) null), entries.GetValueFromPath<string>("KexInitOptions/encryption_algorithms", (string) null), entries.GetValueFromPath<string>("KexInitOptions/mac_algorithms", (string) null), entries.GetValueFromPath<string>("KexInitOptions/compression_algorithms", (string) null));

    public string kex_algorithms { get; }

    public string server_host_key_algorithms { get; }

    public string encryption_algorithms { get; }

    public string mac_algorithms { get; }

    public string compression_algorithms { get; }

    public string languages => "";
  }
}
