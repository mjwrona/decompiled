// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.MachinePropertyBag
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Services;
using Microsoft.VisualStudio.Utilities.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class MachinePropertyBag : IPersistentPropertyBag
  {
    private readonly string backUpStorageLocation;
    private readonly string storageLocation;
    private Dictionary<string, object> store;
    private bool storeWasChanged;

    internal MachinePropertyBag(string storageLocation)
    {
      this.storageLocation = storageLocation;
      this.backUpStorageLocation = MachinePropertyBag.GetBackupStoreLocation(this.storageLocation);
      this.LoadStore();
    }

    internal static string GetBackupStoreLocation(string primaryLocation) => primaryLocation + ".bak";

    public void Persist()
    {
      if (!this.storeWasChanged)
        return;
      this.storeWasChanged = false;
      this.Persist(this.storageLocation);
      this.Persist(this.backUpStorageLocation);
    }

    public void Clear()
    {
      this.store.Clear();
      this.storeWasChanged = true;
    }

    public IEnumerable<KeyValuePair<string, object>> GetAllProperties() => this.store.Select<KeyValuePair<string, object>, KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, KeyValuePair<string, object>>) (kvp => new KeyValuePair<string, object>(kvp.Key, kvp.Value)));

    public object GetProperty(string propertyName)
    {
      object property = (object) null;
      this.store.TryGetValue(propertyName, out property);
      return property;
    }

    public void RemoveProperty(string propertyName) => this.storeWasChanged = this.storeWasChanged || this.store.Remove(propertyName);

    public void SetProperty(string propertyName, int value) => this.SetPropertyInternal(propertyName, (object) value);

    public void SetProperty(string propertyName, string value) => this.SetPropertyInternal(propertyName, (object) value);

    public void SetProperty(string propertyName, double value) => this.SetPropertyInternal(propertyName, (object) value);

    private void Persist(string path)
    {
      DirectoryInfo directory = new FileInfo(path).Directory;
      if (!directory.Exists)
        ReparsePointAware.CreateDirectory(directory.FullName);
      try
      {
        using (FileStream output = ReparsePointAware.OpenFile(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
        {
          try
          {
            if (Platform.IsWindows)
            {
              FileSecurity accessControl = File.GetAccessControl(path);
              FileSystemRights fileSystemRights = FileSystemRights.Read | FileSystemRights.Write;
              accessControl.AddAccessRule(new FileSystemAccessRule((IdentityReference) new SecurityIdentifier("S-1-15-2-1"), fileSystemRights, AccessControlType.Allow));
              accessControl.AddAccessRule(new FileSystemAccessRule((IdentityReference) new SecurityIdentifier("S-1-1-0"), fileSystemRights, AccessControlType.Allow));
              File.SetAccessControl(path, accessControl);
            }
          }
          catch (Exception ex)
          {
          }
          using (BinaryWriter binaryWriter = new BinaryWriter((Stream) output))
          {
            binaryWriter.Write(JsonConvert.SerializeObject((object) this.store));
            if (output.Position == output.Length)
              return;
            output.SetLength(output.Position);
          }
        }
      }
      catch (UnauthorizedAccessException ex)
      {
      }
    }

    private void LoadStore()
    {
      if (this.ParseFromFile(this.storageLocation))
      {
        if (File.Exists(this.backUpStorageLocation))
          return;
        try
        {
          this.Persist(this.backUpStorageLocation);
        }
        catch
        {
        }
      }
      else if (this.ParseFromFile(this.backUpStorageLocation))
      {
        try
        {
          this.Persist(this.storageLocation);
        }
        catch
        {
        }
      }
      else
      {
        this.storeWasChanged = true;
        this.store = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      }
    }

    private bool ParseFromFile(string filepath)
    {
      if (File.Exists(filepath))
      {
        try
        {
          using (FileStream input = ReparsePointAware.OpenFile(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
          {
            this.store = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            using (BinaryReader binaryReader = new BinaryReader((Stream) input))
              JsonConvert.PopulateObject(binaryReader.ReadString(), (object) this.store, new JsonSerializerSettings()
              {
                DateParseHandling = DateParseHandling.None,
                TypeNameHandling = TypeNameHandling.None
              });
            foreach (KeyValuePair<string, object> keyValuePair in this.store.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (kvp => kvp.Value is long)).ToArray<KeyValuePair<string, object>>())
              this.store[keyValuePair.Key] = (object) (int) (long) keyValuePair.Value;
            return true;
          }
        }
        catch
        {
        }
      }
      return false;
    }

    private void SetPropertyInternal(string propertyName, object value)
    {
      object objA;
      if (this.store.TryGetValue(propertyName, out objA) && object.Equals(objA, value))
        return;
      this.storeWasChanged = true;
      this.store[propertyName] = value;
    }
  }
}
