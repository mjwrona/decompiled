// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Services.DefaultRegistryKeyValueStorage
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Experimentation;
using Microsoft.VisualStudio.RemoteSettings;
using Microsoft.VisualStudio.Utilities.Internal;
using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry.Services
{
  internal sealed class DefaultRegistryKeyValueStorage : ICollectionKeyValueStorage, IKeyValueStorage
  {
    private readonly IRegistryTools3 registryTools;

    public DefaultRegistryKeyValueStorage(IRegistryTools3 registryTools)
    {
      registryTools.RequiresArgumentNotNull<IRegistryTools3>(nameof (registryTools));
      this.registryTools = registryTools;
    }

    public bool CollectionExists(string collectionPath)
    {
      collectionPath.RequiresArgumentNotNullAndNotEmpty(nameof (collectionPath));
      collectionPath = collectionPath.NormalizePath();
      return this.registryTools.DoesRegistryKeyExistInCurrentUserRoot(collectionPath);
    }

    public bool PropertyExists(string collectionPath, string key)
    {
      collectionPath.RequiresArgumentNotNullAndNotEmpty(nameof (collectionPath));
      collectionPath = collectionPath.NormalizePath();
      key.RequiresArgumentNotNullAndNotEmpty(nameof (key));
      return this.registryTools.GetRegistryValueFromCurrentUserRoot(collectionPath, key) != null;
    }

    public IEnumerable<string> GetPropertyNames(string collectionPath)
    {
      collectionPath.RequiresArgumentNotNullAndNotEmpty(nameof (collectionPath));
      collectionPath = collectionPath.NormalizePath();
      return (IEnumerable<string>) this.registryTools.GetRegistryValueNamesFromCurrentUserRoot(collectionPath);
    }

    public T GetValue<T>(string key, T defaultValue)
    {
      key.RequiresArgumentNotNullAndNotEmpty(nameof (key));
      Tuple<string, string> pathComponents = this.GetPathComponents(key);
      T obj;
      this.GetValueInternal<T>(pathComponents.Item1, pathComponents.Item2, defaultValue, out obj);
      return obj;
    }

    public T GetValue<T>(string collectionPath, string key, T defaultValue)
    {
      collectionPath.RequiresArgumentNotNullAndNotEmpty(nameof (collectionPath));
      collectionPath = collectionPath.NormalizePath();
      key.RequiresArgumentNotNullAndNotEmpty(nameof (key));
      T obj;
      this.GetValueInternal<T>(collectionPath, key, defaultValue, out obj);
      return obj;
    }

    public bool TryGetValue<T>(string collectionPath, string key, out T value)
    {
      collectionPath.RequiresArgumentNotNullAndNotEmpty(nameof (collectionPath));
      collectionPath = collectionPath.NormalizePath();
      key.RequiresArgumentNotNullAndNotEmpty(nameof (key));
      return this.GetValueInternal<T>(collectionPath, key, default (T), out value);
    }

    public bool TryGetValueKind(string collectionPath, string key, out ValueKind kind)
    {
      collectionPath.RequiresArgumentNotNullAndNotEmpty(nameof (collectionPath));
      collectionPath = collectionPath.NormalizePath();
      key.RequiresArgumentNotNullAndNotEmpty(nameof (key));
      kind = ValueKind.Unknown;
      RegistryValueKind kind1;
      if (!this.registryTools.TryGetRegistryValueKindFromCurrentUserRoot(collectionPath, key, out kind1))
        return false;
      switch (kind1)
      {
        case RegistryValueKind.String:
          kind = ValueKind.String;
          break;
        case RegistryValueKind.DWord:
          kind = ValueKind.DWord;
          break;
        case RegistryValueKind.MultiString:
          kind = ValueKind.MultiString;
          break;
        case RegistryValueKind.QWord:
          kind = ValueKind.QWord;
          break;
        default:
          kind = ValueKind.Unknown;
          break;
      }
      return true;
    }

    public void SetValue<T>(string key, T value)
    {
      key.RequiresArgumentNotNullAndNotEmpty(nameof (key));
      Tuple<string, string> pathComponents = this.GetPathComponents(key);
      this.SetValueInternal<T>(pathComponents.Item1, pathComponents.Item2, value);
    }

    public void SetValue<T>(string collectionPath, string key, T value)
    {
      collectionPath.RequiresArgumentNotNullAndNotEmpty(nameof (collectionPath));
      collectionPath = collectionPath.NormalizePath();
      key.RequiresArgumentNotNullAndNotEmpty(nameof (key));
      this.SetValueInternal<T>(collectionPath, key, value);
    }

    private Tuple<string, string> GetPathComponents(string key)
    {
      string str = key.NormalizePath();
      int length = str.LastIndexOf('\\');
      return length != -1 ? Tuple.Create<string, string>(str.Substring(0, length), str.Substring(length + 1)) : throw new ArgumentException("invalid argument 'key'");
    }

    private bool GetValueInternal<T>(
      string collectionPath,
      string key,
      T defaultValue,
      out T value)
    {
      return this.registryTools.GetRegistryValueFromCurrentUserRoot(collectionPath, key).TryConvertToType<T>(defaultValue, out value);
    }

    private void SetValueInternal<T>(string collectionPath, string key, T value) => this.registryTools.SetRegistryFromCurrentUserRoot(collectionPath, key, (object) value);

    public IEnumerable<string> GetSubCollectionNames(string collectionPath)
    {
      collectionPath.RequiresArgumentNotNullAndNotEmpty(nameof (collectionPath));
      collectionPath = collectionPath.NormalizePath();
      return (IEnumerable<string>) this.registryTools.GetRegistrySubKeyNamesFromCurrentUserRoot(collectionPath);
    }

    public bool DeleteCollection(string collectionPath)
    {
      collectionPath.RequiresArgumentNotNullAndNotEmpty(nameof (collectionPath));
      collectionPath = collectionPath.NormalizePath();
      return this.registryTools.DeleteRegistryKeyFromCurrentUserRoot(collectionPath);
    }

    public bool DeleteProperty(string collectionPath, string propertyName)
    {
      collectionPath.RequiresArgumentNotNullAndNotEmpty(nameof (collectionPath));
      propertyName.RequiresArgumentNotNullAndNotEmpty(nameof (propertyName));
      return this.registryTools.DeleteRegistryValueFromCurrentUserRoot(collectionPath, propertyName);
    }
  }
}
