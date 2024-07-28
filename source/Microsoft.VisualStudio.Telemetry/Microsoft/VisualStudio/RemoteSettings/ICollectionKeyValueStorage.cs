// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.ICollectionKeyValueStorage
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.RemoteSettings
{
  public interface ICollectionKeyValueStorage
  {
    T GetValue<T>(string collectionPath, string key, T defaultValue);

    bool TryGetValue<T>(string collectionPath, string key, out T value);

    bool TryGetValueKind(string collectionPath, string key, out ValueKind kind);

    void SetValue<T>(string collectionPath, string key, T value);

    IEnumerable<string> GetPropertyNames(string collectionPath);

    IEnumerable<string> GetSubCollectionNames(string collectionPath);

    bool CollectionExists(string collectionPath);

    bool PropertyExists(string collectionPath, string propertyName);

    bool DeleteCollection(string collectionPath);

    bool DeleteProperty(string collectionPath, string propertyName);
  }
}
