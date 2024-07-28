// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Settings.GitOdbSettingsProvider
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Git.Server.Settings
{
  internal class GitOdbSettingsProvider
  {
    private readonly IVssRequestContext m_rc;
    private readonly IVssRegistryService m_regSvc;
    private readonly OdbId m_odbId;

    public GitOdbSettingsProvider(IVssRequestContext rc, IVssRegistryService regSvc, OdbId odbId)
    {
      this.m_rc = rc;
      this.m_regSvc = regSvc;
      this.m_odbId = odbId;
    }

    public GitOdbSettings GetSettings()
    {
      int? registrySetting1 = this.GetRegistrySetting<int?>("/Service/Git/Settings/RenameDetectionFileSize");
      int? registrySetting2 = this.GetRegistrySetting<int?>("/Service/Git/Settings/StablePackfileCapSize");
      int? registrySetting3 = this.GetRegistrySetting<int?>("/Service/Git/Settings/UnstablePackfileCapSize");
      int? registrySetting4 = this.GetRegistrySetting<int?>("/Service/Git/Settings/MaxMemoryStreamBytes");
      int? registrySetting5 = this.GetRegistrySetting<int?>("/Service/Git/Settings/MaxMemoryMappedFileBytes");
      int? registrySetting6 = this.GetRegistrySetting<int?>("/Service/Git/Settings/IndexTransactionLeaseRenewCount");
      long? registrySetting7 = this.GetRegistrySetting<long?>("/Service/Git/Settings/TipIndexSize");
      long? registrySetting8 = this.GetRegistrySetting<long?>("/Service/Git/Settings/StagingIndexSize");
      bool? registrySetting9 = this.GetRegistrySetting<bool?>("/Service/Git/Settings/UseShardedHashsetForROR");
      int? stablePackfileCapSize = registrySetting2;
      int? unstablePackfileCapSize = registrySetting3;
      int? maxMemoryStreamBytes = registrySetting4;
      int? maxMemoryMappedFileBytes = registrySetting5;
      int? indexTransactionLeaseRenewCount = registrySetting6;
      long? tipIndexSize = registrySetting7;
      long? stagingIndexSize = registrySetting8;
      bool? useShardedHashsetForROR = registrySetting9;
      return new GitOdbSettings(registrySetting1, stablePackfileCapSize, unstablePackfileCapSize, maxMemoryStreamBytes, maxMemoryMappedFileBytes, indexTransactionLeaseRenewCount, tipIndexSize, stagingIndexSize, useShardedHashsetForROR);
    }

    private T GetRegistrySetting<T>(string registryPath) => this.m_regSvc.GetValue<T>(this.m_rc, (RegistryQuery) (registryPath + "/" + this.m_odbId.Value.ToString()), default (T)) ?? this.m_regSvc.GetValue<T>(this.m_rc, (RegistryQuery) registryPath, true, default (T));

    private static class RegistryPaths
    {
      public const string MaxRenameDetectionFileSize = "/Service/Git/Settings/RenameDetectionFileSize";
      public const string StablePackfileCapSize = "/Service/Git/Settings/StablePackfileCapSize";
      public const string UnstablePackfileCapSize = "/Service/Git/Settings/UnstablePackfileCapSize";
      public const string MaxMemoryStreamBytes = "/Service/Git/Settings/MaxMemoryStreamBytes";
      public const string MaxMemoryMappedFileBytes = "/Service/Git/Settings/MaxMemoryMappedFileBytes";
      public const string IndexTransactionLeaseRenewCount = "/Service/Git/Settings/IndexTransactionLeaseRenewCount";
      public const string TipIndexSize = "/Service/Git/Settings/TipIndexSize";
      public const string StagingIndexSize = "/Service/Git/Settings/StagingIndexSize";
      public const string UseShardedHashsetForROR = "/Service/Git/Settings/UseShardedHashsetForROR";
    }
  }
}
