// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.RemoteSettingsParser
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal class RemoteSettingsParser : IRemoteSettingsParser
  {
    internal static readonly string FileVersionNotFirstPropertyErrorMessage = "The FileVersion was not the first property in the remote settings stream.";
    internal static readonly string ChangesetIdNotSecondPropertyErrorMessage = "The ChangesetId was not the second property in the remote settings stream.";
    internal static readonly string TypeNotSupportedErrorMessageFormat = "Type {0} not supported.";
    internal static readonly string ScopesWasNotObjectErrorMessage = "Scopes is not of type object";
    internal static readonly string ScopeWasNotStringErrorMessage = "A scope was not of type string";
    internal static readonly string InvalidJsonErrorMessage = "The remote settings stream was not a valid json document.";
    internal static readonly string UnhandledExceptionErrorMessageFormat = "An unhandled exception occurred while parsing the remote settings json document. Exception:\r\n{0}";
    private readonly IRemoteSettingsValidator remoteSettingsValidator;

    public RemoteSettingsParser(IRemoteSettingsValidator remoteSettingsValidator)
    {
      remoteSettingsValidator.RequiresArgumentNotNull<IRemoteSettingsValidator>(nameof (remoteSettingsValidator));
      this.remoteSettingsValidator = remoteSettingsValidator;
    }

    public VersionedDeserializedRemoteSettings TryParseVersionedStream(Stream stream)
    {
      try
      {
        using (StreamReader reader1 = new StreamReader(stream))
        {
          using (JsonTextReader reader2 = new JsonTextReader((TextReader) reader1))
          {
            JObject jobject = JObject.Load((JsonReader) reader2);
            Queue<RemoteSettingsParser.PropertyEntry> q = new Queue<RemoteSettingsParser.PropertyEntry>();
            string fileVersion;
            string changesetId;
            using (IEnumerator<JProperty> enumerator = jobject.Properties().GetEnumerator())
            {
              if (!enumerator.MoveNext() || enumerator.Current.Name != "FileVersion" || enumerator.Current.Value.Type != JTokenType.String)
                return new VersionedDeserializedRemoteSettings(error: RemoteSettingsParser.FileVersionNotFirstPropertyErrorMessage);
              fileVersion = (string) enumerator.Current.Value;
              if (!enumerator.MoveNext() || enumerator.Current.Name != "ChangesetId" || enumerator.Current.Value.Type != JTokenType.String)
                return new VersionedDeserializedRemoteSettings(error: RemoteSettingsParser.ChangesetIdNotSecondPropertyErrorMessage);
              changesetId = (string) enumerator.Current.Value;
              while (enumerator.MoveNext())
                q.Enqueue(new RemoteSettingsParser.PropertyEntry(string.Empty, enumerator.Current));
            }
            return new VersionedDeserializedRemoteSettings(this.ParseInternal(q), fileVersion, changesetId);
          }
        }
      }
      catch (JsonReaderException ex)
      {
        return new VersionedDeserializedRemoteSettings(error: RemoteSettingsParser.InvalidJsonErrorMessage);
      }
      catch (Exception ex)
      {
        return new VersionedDeserializedRemoteSettings(error: string.Format(RemoteSettingsParser.UnhandledExceptionErrorMessageFormat, (object) ex));
      }
    }

    public DeserializedRemoteSettings TryParseFromJObject(JObject json, string globalScope = null)
    {
      Queue<RemoteSettingsParser.PropertyEntry> q = new Queue<RemoteSettingsParser.PropertyEntry>();
      foreach (JProperty property in json.Properties())
        q.Enqueue(new RemoteSettingsParser.PropertyEntry(string.Empty, property));
      return this.ParseInternal(q, globalScope);
    }

    private DeserializedRemoteSettings ParseInternal(
      Queue<RemoteSettingsParser.PropertyEntry> q,
      string globalScope = null)
    {
      RemoteSettingsParser.PropertyEntry scopesPropertyEntry = q.Where<RemoteSettingsParser.PropertyEntry>((Func<RemoteSettingsParser.PropertyEntry, bool>) (x => x.JProperty.Name == "Scopes")).FirstOrDefault<RemoteSettingsParser.PropertyEntry>();
      List<Scope> list1 = new List<Scope>();
      if (!scopesPropertyEntry.Equals((object) new RemoteSettingsParser.PropertyEntry()))
      {
        if (scopesPropertyEntry.JProperty.Value.Type != JTokenType.Object)
          return new DeserializedRemoteSettings(error: RemoteSettingsParser.ScopesWasNotObjectErrorMessage);
        foreach (JProperty property in ((JObject) scopesPropertyEntry.JProperty.Value).Properties())
        {
          if (property.Value.Type != JTokenType.String)
            return new DeserializedRemoteSettings(error: RemoteSettingsParser.ScopeWasNotStringErrorMessage);
          list1.Add(new Scope()
          {
            Name = property.Name,
            ScopeString = (string) property.Value
          });
        }
        q = new Queue<RemoteSettingsParser.PropertyEntry>(q.Where<RemoteSettingsParser.PropertyEntry>((Func<RemoteSettingsParser.PropertyEntry, bool>) (x => !x.Equals((object) scopesPropertyEntry))));
      }
      List<RemoteSetting> list2 = new List<RemoteSetting>();
      while (q.Count > 0)
      {
        RemoteSettingsParser.PropertyEntry propertyEntry = q.Dequeue();
        switch (propertyEntry.JProperty.Value.Type)
        {
          case JTokenType.Object:
            string path = propertyEntry.Path == string.Empty ? propertyEntry.JProperty.Name : Path.Combine(propertyEntry.Path, propertyEntry.JProperty.Name);
            using (IEnumerator<JProperty> enumerator = ((JObject) propertyEntry.JProperty.Value).Properties().GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                JProperty current = enumerator.Current;
                q.Enqueue(new RemoteSettingsParser.PropertyEntry(path, current));
              }
              continue;
            }
          case JTokenType.Integer:
            list2.Add(RemoteSettingsParser.ProcessRemoteSetting(propertyEntry.Path, propertyEntry.JProperty.Name, (object) (int) propertyEntry.JProperty.Value, globalScope));
            continue;
          case JTokenType.String:
            list2.Add(RemoteSettingsParser.ProcessRemoteSetting(propertyEntry.Path, propertyEntry.JProperty.Name, (object) (string) propertyEntry.JProperty.Value, globalScope));
            continue;
          case JTokenType.Boolean:
            list2.Add(RemoteSettingsParser.ProcessRemoteSetting(propertyEntry.Path, propertyEntry.JProperty.Name, (object) (bool) propertyEntry.JProperty.Value, globalScope));
            continue;
          default:
            return new DeserializedRemoteSettings(error: string.Format(RemoteSettingsParser.TypeNotSupportedErrorMessageFormat, (object) propertyEntry.JProperty.Value.Type));
        }
      }
      DeserializedRemoteSettings remoteSettings = new DeserializedRemoteSettings(new ReadOnlyCollection<Scope>((IList<Scope>) list1), new ReadOnlyCollection<RemoteSetting>((IList<RemoteSetting>) list2));
      try
      {
        this.remoteSettingsValidator.ValidateDeserialized(remoteSettings);
        return remoteSettings;
      }
      catch (RemoteSettingsValidationException ex)
      {
        return new DeserializedRemoteSettings(error: ex.Message);
      }
    }

    private static RemoteSetting ProcessRemoteSetting(
      string propertyPath,
      string propertyName,
      object value,
      string globalScope)
    {
      string name = propertyName;
      string scopeString = globalScope;
      int length = propertyName.IndexOf(':');
      if (length != -1)
      {
        name = propertyName.Substring(0, length);
        string str = propertyName.Substring(length + 1);
        scopeString = string.IsNullOrEmpty(globalScope) ? str : globalScope + " && " + str;
      }
      return new RemoteSetting(propertyPath, name, value, scopeString);
    }

    private struct PropertyEntry
    {
      public readonly string Path;
      public readonly JProperty JProperty;

      public PropertyEntry(string path, JProperty jProperty)
      {
        this.Path = path;
        this.JProperty = jProperty;
      }
    }
  }
}
