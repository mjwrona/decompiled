// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Utilities.Internal.FileBasedRegistry
// Assembly: Microsoft.VisualStudio.Utilities.Internal, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E02F1555-E063-4795-BC05-853CA7424F51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Utilities.Internal.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Utilities.Internal
{
  public sealed class FileBasedRegistry : IDisposable
  {
    private readonly string filepath;
    private readonly string currentUserRootPath;
    private readonly Hashtable values;
    private bool dropped;
    private bool modified;

    public FileBasedRegistry(string key)
    {
      if (Platform.IsWindows)
        throw new PlatformNotSupportedException();
      string path1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".mono", "registry", "CurrentUser");
      this.currentUserRootPath = path1;
      if (!Directory.Exists(this.currentUserRootPath))
        Directory.CreateDirectory(this.currentUserRootPath);
      string macosPath = FileBasedRegistry.ConvertWinRegistryPathToMacosPath(key);
      this.values = new Hashtable();
      this.filepath = Path.Combine(path1, macosPath, "values.xml");
      this.LoadXml();
    }

    public bool Exists => File.Exists(this.filepath);

    public void Dispose()
    {
      if (!this.modified)
        return;
      this.SaveXml();
    }

    public void Drop()
    {
      this.dropped = true;
      this.modified = true;
    }

    public void SetValue(string name, object value)
    {
      this.EnsureNotDropped();
      lock (this.values)
      {
        this.values[(object) name] = (object) RegistryValue.FromValue(name, value);
        this.modified = true;
      }
    }

    public string[] GetValueNames()
    {
      this.EnsureNotDropped();
      lock (this.values)
      {
        string[] valueNames = new string[this.values.Count];
        this.values.Keys.CopyTo((Array) valueNames, 0);
        return valueNames;
      }
    }

    public string[] GetSubKeyNames(string subKeyPath)
    {
      string[] directories = Directory.GetDirectories(Path.Combine(this.currentUserRootPath, FileBasedRegistry.ConvertWinRegistryPathToMacosPath(subKeyPath)));
      List<string> stringList = new List<string>();
      foreach (string path in directories)
        stringList.Add(new DirectoryInfo(path).Name);
      return stringList.ToArray();
    }

    public void RemoveValue(string key)
    {
      this.EnsureNotDropped();
      lock (this.values)
      {
        if (!this.values.ContainsKey((object) key))
          return;
        this.values.Remove((object) key);
        this.modified = true;
      }
    }

    public object GetValue(string key)
    {
      this.EnsureNotDropped();
      object obj;
      lock (this.values)
        obj = this.values[(object) (key ?? string.Empty)];
      return obj is RegistryValue registryValue && registryValue != null ? registryValue.Value : (object) null;
    }

    public void Clear()
    {
      this.EnsureNotDropped();
      lock (this.values)
      {
        if (this.values.Count <= 0)
          return;
        this.values.Clear();
        this.modified = true;
      }
    }

    private static string ConvertWinRegistryPathToMacosPath(string keyname)
    {
      if (keyname.IndexOf('\\') != -1)
        keyname = keyname.Replace('\\', '/');
      return keyname.ToLower();
    }

    private void EnsureNotDropped()
    {
      if (this.dropped)
        throw new IOException("Illegal operation attempted on a registry key that has been marked for deletion.");
    }

    private void LoadXml()
    {
      if (!File.Exists(this.filepath))
        return;
      RegistryValues registryValues;
      using (StreamReader input = new StreamReader(this.filepath, Encoding.UTF8))
      {
        using (XmlReader xmlReader = XmlReader.Create((TextReader) input))
          registryValues = (RegistryValues) new XmlSerializer(typeof (RegistryValues)).Deserialize(xmlReader);
        input.Close();
      }
      lock (this.values)
      {
        foreach (RegistryValue registryValue in (List<RegistryValue>) registryValues)
          this.values[(object) registryValue.Name] = (object) registryValue;
      }
    }

    private void SaveXml()
    {
      string directoryName = Path.GetDirectoryName(this.filepath);
      if (this.dropped)
      {
        if (!Directory.Exists(directoryName))
          return;
        Directory.Delete(directoryName, true);
      }
      else
      {
        Directory.CreateDirectory(directoryName);
        RegistryValues o = new RegistryValues();
        foreach (object key in (IEnumerable) this.values.Keys)
          o.Add((RegistryValue) this.values[key]);
        XmlSerializer xmlSerializer = new XmlSerializer(typeof (RegistryValues));
        XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new XmlQualifiedName[1]
        {
          XmlQualifiedName.Empty
        });
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.OmitXmlDeclaration = true;
        using (StreamWriter output = new StreamWriter(this.filepath, false, Encoding.UTF8))
        {
          using (XmlWriter xmlWriter = XmlWriter.Create((TextWriter) output, settings))
            xmlSerializer.Serialize(xmlWriter, (object) o, namespaces);
          output.Close();
        }
      }
    }
  }
}
