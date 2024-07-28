// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ITeamFoundationRegistry
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public interface ITeamFoundationRegistry
  {
    int DeleteEntries(params string[] registryPathPatterns);

    int DeleteUserEntries(params string[] registryPathPatterns);

    RegistryEntryCollection ReadEntries(string registryPathPattern);

    RegistryEntryCollection ReadEntries(string registryPathPattern, bool includeContainerHints);

    RegistryEntryCollection ReadUserEntries(string registryPathPattern);

    RegistryEntryCollection ReadUserEntries(string registryPathPattern, bool includeContainerHints);

    void WriteEntries(IEnumerable<RegistryEntry> registryEntries);

    void WriteUserEntries(IEnumerable<RegistryEntry> registryEntries);

    string GetValue(string path);

    string GetValue(string path, string defaultValue);

    string GetUserValue(string path);

    string GetUserValue(string path, string defaultValue);

    T GetValue<T>(string path);

    T GetValue<T>(string path, T defaultValue);

    T GetUserValue<T>(string path);

    T GetUserValue<T>(string path, T defaultValue);

    void SetValue(string path, string value);

    void SetValue<T>(string path, T value);

    void SetUserValue(string path, string value);

    void SetUserValue<T>(string path, T value);

    IEnumerable<RegistryAuditEntry> QueryAuditLog(int changeIndex, bool returnOlder);
  }
}
