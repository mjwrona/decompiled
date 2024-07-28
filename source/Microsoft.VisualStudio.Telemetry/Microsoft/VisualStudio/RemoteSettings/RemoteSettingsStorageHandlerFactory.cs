// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.RemoteSettingsStorageHandlerFactory
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Services;
using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal sealed class RemoteSettingsStorageHandlerFactory : IRemoteSettingsStorageHandlerFactory
  {
    private const string PathPrefix = "Software\\Microsoft\\VisualStudio\\RemoteSettings";
    private const string PathFormatWithPrefix = "{0}\\{1}";
    private const string IsDisabledName = "TurnOffSwitch";
    private const string IsLoggingEnabled = "LoggingEnabled";
    private const int RemoteSettingsExplicitlyDisabled = 1;
    private const int RemoteSettingsLoggingEnabled = 1;
    private readonly IRemoteSettingsLogger logger;
    private readonly Regex fileNameRegex = new Regex("^[a-zA-Z0-9_.-]+\\.json$");

    public RemoteSettingsStorageHandlerFactory(IRemoteSettingsLogger logger) => this.logger = logger;

    public static Func<bool> BuildIsUpdateDisabled(
      ICollectionKeyValueStorage storage,
      bool usePrefix)
    {
      storage.RequiresArgumentNotNull<ICollectionKeyValueStorage>(nameof (storage));
      string collectionPathPrefix = usePrefix ? "Software\\Microsoft\\VisualStudio\\RemoteSettings" : string.Empty;
      return (Func<bool>) (() => storage.GetValue<int>(collectionPathPrefix, "TurnOffSwitch", 0) == 1);
    }

    public static Func<bool> BuildIsLoggingEnabled(
      ICollectionKeyValueStorage storage,
      bool usePrefix)
    {
      storage.RequiresArgumentNotNull<ICollectionKeyValueStorage>(nameof (storage));
      string collectionPathPrefix = usePrefix ? "Software\\Microsoft\\VisualStudio\\RemoteSettings" : string.Empty;
      return (Func<bool>) (() => storage.GetValue<int>(collectionPathPrefix, "LoggingEnabled", 0) == 1);
    }

    public IVersionedRemoteSettingsStorageHandler BuildVersioned(
      ICollectionKeyValueStorage storage,
      bool usePrefix,
      string fileName,
      IScopeParserFactory scopeParserFactory)
    {
      if (!this.fileNameRegex.IsMatch(fileName))
        throw new ArgumentException("Filename is invalid", nameof (fileName));
      string collectionPathPrefix = !usePrefix ? fileName : string.Format("{0}\\{1}", (object) "Software\\Microsoft\\VisualStudio\\RemoteSettings", (object) fileName, (object) CultureInfo.InvariantCulture);
      return (IVersionedRemoteSettingsStorageHandler) new RemoteSettingsStorageHandler(storage, collectionPathPrefix, scopeParserFactory, true, this.logger);
    }

    public IRemoteSettingsStorageHandler Build(
      ICollectionKeyValueStorage storage,
      bool usePrefix,
      RemoteSettingsFilterProvider filterProvider,
      IScopeParserFactory scopeParserFactory)
    {
      filterProvider.RequiresArgumentNotNull<RemoteSettingsFilterProvider>(nameof (filterProvider));
      List<string> stringList = new List<string>();
      stringList.AddIfNotEmpty(filterProvider.GetApplicationName());
      stringList.AddIfNotEmpty(filterProvider.GetApplicationVersion());
      stringList.AddIfNotEmpty(filterProvider.GetBranchBuildFrom());
      string str = string.Join("\\", (IEnumerable<string>) stringList);
      string collectionPathPrefix = !usePrefix ? str : string.Format("{0}\\{1}", (object) "Software\\Microsoft\\VisualStudio\\RemoteSettings", (object) str, (object) CultureInfo.InvariantCulture);
      return (IRemoteSettingsStorageHandler) new RemoteSettingsStorageHandler(storage, collectionPathPrefix, scopeParserFactory, false, this.logger);
    }

    public IRemoteSettingsStorageHandler Build(
      ICollectionKeyValueStorage storage,
      bool usePrefix,
      string collectionPath,
      IScopeParserFactory scopeParserFactory)
    {
      string collectionPathPrefix = !usePrefix ? collectionPath : string.Format("{0}\\{1}", (object) "Software\\Microsoft\\VisualStudio\\RemoteSettings", (object) collectionPath, (object) CultureInfo.InvariantCulture);
      return (IRemoteSettingsStorageHandler) new RemoteSettingsStorageHandler(storage, collectionPathPrefix, scopeParserFactory, false, this.logger);
    }
  }
}
