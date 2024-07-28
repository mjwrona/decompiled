// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.RegistryPropertyBag
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class RegistryPropertyBag : IPersistentPropertyBag
  {
    private const string KeyPath = "Software\\Microsoft\\VisualStudio\\Telemetry\\PersistentPropertyBag\\";
    private const string StringPrefix = "s:";
    private const string DoublePrefix = "d:";
    private readonly string keyName;
    private readonly string fullKeyName;

    public RegistryPropertyBag(string processName)
    {
      this.keyName = "Software\\Microsoft\\VisualStudio\\Telemetry\\PersistentPropertyBag\\" + processName;
      this.fullKeyName = "HKEY_CURRENT_USER\\" + this.keyName;
    }

    public virtual IEnumerable<KeyValuePair<string, object>> GetAllProperties()
    {
      List<KeyValuePair<string, object>> result = new List<KeyValuePair<string, object>>();
      if (!this.SafeRegistryCall((Action) (() =>
      {
        foreach (KeyValuePair<string, object> readRegistryValue in this.ReadRegistryValues(this.keyName))
        {
          object obj = RegistryPropertyBag.InterpretRegistryValue(readRegistryValue.Value);
          result.Add(new KeyValuePair<string, object>(readRegistryValue.Key, obj));
        }
      })))
        result.Clear();
      return (IEnumerable<KeyValuePair<string, object>>) result;
    }

    public object GetProperty(string propertyName)
    {
      object result = (object) null;
      this.SafeRegistryCall((Action) (() => result = RegistryPropertyBag.InterpretRegistryValue(this.GetProperty(this.fullKeyName, this.keyName, propertyName, (object) null))));
      return result;
    }

    public void SetProperty(string propertyName, int value) => this.SetProperty(propertyName, (object) value);

    public void SetProperty(string propertyName, string value) => this.SetProperty(propertyName, (object) value);

    public void SetProperty(string propertyName, double value) => this.SetProperty(propertyName, (object) value);

    public void RemoveProperty(string propertyName) => this.SafeRegistryCall((Action) (() => this.RemoveProperty(this.keyName, propertyName)));

    public void Clear() => this.SafeRegistryCall((Action) (() => this.Clear(this.keyName)));

    protected virtual IEnumerable<KeyValuePair<string, object>> ReadRegistryValues(
      string registryKeyName)
    {
      List<KeyValuePair<string, object>> keyValuePairList = new List<KeyValuePair<string, object>>();
      using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(registryKeyName))
      {
        if (registryKey != null)
        {
          foreach (string valueName in registryKey.GetValueNames())
          {
            object obj = registryKey.GetValue(valueName);
            if (obj != null)
              keyValuePairList.Add(new KeyValuePair<string, object>(valueName, obj));
          }
        }
      }
      return (IEnumerable<KeyValuePair<string, object>>) keyValuePairList;
    }

    protected virtual object GetProperty(
      string fullRegistryKeyName,
      string registryKeyName,
      string propertyName,
      object defaultValue)
    {
      return Registry.GetValue(fullRegistryKeyName, propertyName, defaultValue);
    }

    protected virtual void SetProperty(string registryKeyName, string propertyName, object value)
    {
      using (RegistryKey subKey = Registry.CurrentUser.CreateSubKey(registryKeyName))
      {
        this.SetAccessControl(subKey);
        double? nullable = value as double?;
        if (nullable.HasValue)
          subKey.SetValue(propertyName, (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", new object[2]
          {
            (object) "d:",
            (object) nullable
          }));
        else if (value is string valueAsString)
        {
          if (valueAsString.StartsWith("s:"))
            valueAsString = RegistryPropertyBag.StringPrefixScrubber(valueAsString);
          subKey.SetValue(propertyName, (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", new object[2]
          {
            (object) "s:",
            (object) valueAsString
          }));
        }
        else
          subKey.SetValue(propertyName, value);
      }
    }

    protected virtual void RemoveProperty(string registryKeyName, string propertyName)
    {
      using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(registryKeyName, true))
        registryKey?.DeleteValue(propertyName, false);
    }

    protected virtual void Clear(string registryKeyName) => Registry.CurrentUser.DeleteSubKeyTree(registryKeyName, false);

    protected virtual void SetAccessControl(RegistryKey key)
    {
      RegistryAccessRule rule = new RegistryAccessRule((IdentityReference) new SecurityIdentifier("S-1-15-2-1"), RegistryRights.ExecuteKey | RegistryRights.SetValue | RegistryRights.CreateSubKey, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow);
      RegistrySecurity accessControl = key.GetAccessControl();
      accessControl.AddAccessRule(rule);
      key.SetAccessControl(accessControl);
    }

    private void SetProperty(string propertyName, object value) => this.SafeRegistryCall((Action) (() => this.SetProperty(this.keyName, propertyName, value)));

    private static object InterpretRegistryValue(object value)
    {
      if (value is string valueAsString)
      {
        if (valueAsString.StartsWith("d:"))
        {
          double result;
          if (double.TryParse(valueAsString.Substring("d:".Length), out result))
            return (object) result;
        }
        else if (valueAsString.StartsWith("s:"))
          return (object) RegistryPropertyBag.StringPrefixScrubber(valueAsString);
      }
      return value;
    }

    private static string StringPrefixScrubber(string valueAsString)
    {
      string str = valueAsString.Substring("s:".Length);
      while (str.StartsWith("s:"))
        str = str.Substring("s:".Length);
      return str;
    }

    private bool SafeRegistryCall(Action action)
    {
      try
      {
        action();
        return true;
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case IOException _:
          case SecurityException _:
          case UnauthorizedAccessException _:
          case InvalidOperationException _:
            return false;
          default:
            throw;
        }
      }
    }

    public void Persist()
    {
    }
  }
}
