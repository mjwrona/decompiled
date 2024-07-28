// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Utils.PackageJsonUtils
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog.AdditionalObjects;
using Microsoft.VisualStudio.Services.Npm.Server.PackageIndex;
using Microsoft.VisualStudio.Services.Npm.WebApi.Converters;
using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.Utils
{
  public static class PackageJsonUtils
  {
    public static T DeserializeNpmJsonDocument<T>(
      DeflateCompressibleBytes packageJsonBytes,
      ITracerService? tracerService,
      string? packageNameForLogging,
      string? packageVersionForLogging,
      string? upstreamLocationForLogging,
      string callerSourceLocationContextForLogging,
      out IReadOnlyList<Exception> errors)
    {
      ArgumentUtility.CheckForNull<DeflateCompressibleBytes>(packageJsonBytes, nameof (packageJsonBytes));
      return PackageJsonUtils.DeserializeNpmJsonDocument<T>((TextReader) new StreamReader((Stream) new MemoryStream(packageJsonBytes.AsUncompressedBytes(), false)), tracerService, packageNameForLogging, packageVersionForLogging, upstreamLocationForLogging, callerSourceLocationContextForLogging, out errors);
    }

    public static T DeserializeNpmJsonDocument<T>(
      TextReader textReader,
      ITracerService? tracerService,
      string? packageNameForLogging,
      string? packageVersionForLogging,
      string? upstreamLocationForLogging,
      string callerSourceLocationContextForLogging,
      out IReadOnlyList<Exception> errors)
    {
      return PackageJsonUtils.DeserializeNpmJsonDocumentNoThrow<T>(textReader, tracerService, packageNameForLogging, packageVersionForLogging, upstreamLocationForLogging, callerSourceLocationContextForLogging, out errors) ?? throw new InvalidPackageJsonException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_BadPackageJson() + string.Join(Environment.NewLine, errors.Select<Exception, string>((Func<Exception, string>) (x => x.Message)).Prepend<string>("")));
    }

    public static T? DeserializeNpmJsonDocumentNoThrow<T>(
      TextReader textReader,
      ITracerService? tracerService,
      string? packageNameForLogging,
      string? packageVersionForLogging,
      string? upstreamLocationForLogging,
      string callerSourceLocationContextForLogging,
      out IReadOnlyList<Exception> errors)
    {
      ArgumentUtility.CheckForNull<TextReader>(textReader, nameof (textReader));
      List<Exception> errors1 = new List<Exception>();
      errors = (IReadOnlyList<Exception>) errors1;
      using (ITracerBlock tracer = tracerService?.Enter((object) null, nameof (DeserializeNpmJsonDocumentNoThrow)))
      {
        using (JsonTextReader reader = new JsonTextReader(textReader))
          return JsonSerializer.Create(PackageJsonUtils.GetJsonSerializerSettings(errors1, JsonErrorLogger.Create(tracer, packageNameForLogging, packageVersionForLogging, upstreamLocationForLogging, callerSourceLocationContextForLogging))).Deserialize<T>((JsonReader) reader);
      }
    }

    public static ProtocolMetadata GetProtocolMetadata(PackageJson packageMetadata)
    {
      NpmProtocolMetadata protocolMetadata1 = new NpmProtocolMetadata();
      Person person;
      if (packageMetadata.Author == null)
      {
        person = (Person) null;
      }
      else
      {
        person = new Person();
        person.Name = packageMetadata.Author.Name;
        person.Url = packageMetadata.Author.Url;
        person.Email = packageMetadata.Author.Email;
      }
      protocolMetadata1.Author = person;
      protocolMetadata1.Homepage = packageMetadata.Homepage;
      Repository repository;
      if (packageMetadata.Repository == null)
      {
        repository = (Repository) null;
      }
      else
      {
        repository = new Repository();
        repository.Url = packageMetadata.Repository.Url;
        repository.RepoType = packageMetadata.Repository.RepoType;
        repository.ShortcutSyntax = packageMetadata.Repository.ShortcutSyntax;
      }
      protocolMetadata1.Repository = repository;
      BugTracker bugTracker;
      if (packageMetadata.Bugs == null)
      {
        bugTracker = (BugTracker) null;
      }
      else
      {
        bugTracker = new BugTracker();
        bugTracker.Url = packageMetadata.Bugs.Url;
        bugTracker.Email = packageMetadata.Bugs.Email;
      }
      protocolMetadata1.Bugs = bugTracker;
      protocolMetadata1.License = packageMetadata.License;
      protocolMetadata1.Binaries = packageMetadata.Binaries;
      protocolMetadata1.ManualPages = packageMetadata.ManualPages;
      protocolMetadata1.Main = packageMetadata.Main;
      protocolMetadata1.EngineStrict = packageMetadata.EngineStrict;
      protocolMetadata1.Config = packageMetadata.Config;
      protocolMetadata1.PublishConfig = packageMetadata.PublishConfig;
      protocolMetadata1.PreferGlobal = packageMetadata.PreferGlobal;
      protocolMetadata1.PrivatePackage = packageMetadata.PrivatePackage;
      protocolMetadata1.Contributors = packageMetadata.Contributors != null ? Array.ConvertAll<Person, Person>(packageMetadata.Contributors, (Converter<Person, Person>) (x => new Person()
      {
        Name = x.Name,
        Url = x.Url,
        Email = x.Email
      })) : (Person[]) null;
      protocolMetadata1.Scripts = packageMetadata.Scripts;
      Directories directories;
      if (packageMetadata.Directories == null)
      {
        directories = (Directories) null;
      }
      else
      {
        directories = new Directories();
        directories.BinariesFolder = packageMetadata.Directories.BinariesFolder;
        directories.DocumentationFolder = packageMetadata.Directories.DocumentationFolder;
        directories.ExamplesFolder = packageMetadata.Directories.ExamplesFolder;
        directories.LibraryFolder = packageMetadata.Directories.LibraryFolder;
        directories.ManualPagesFolder = packageMetadata.Directories.ManualPagesFolder;
      }
      protocolMetadata1.Directories = directories;
      protocolMetadata1.Files = packageMetadata.Files;
      protocolMetadata1.OperatingSystem = packageMetadata.OperatingSystem;
      protocolMetadata1.ProcessorArchitecture = packageMetadata.ProcessorArchitecture;
      protocolMetadata1.Engines = packageMetadata.Engines;
      protocolMetadata1.Deprecated = packageMetadata.Deprecated;
      NpmProtocolMetadata protocolMetadata2 = protocolMetadata1;
      return new ProtocolMetadata()
      {
        SchemaVersion = 1,
        Data = (object) protocolMetadata2
      };
    }

    public static PackageJson ApplyOptions(this PackageJson packageJson, PackageJsonOptions? options)
    {
      if (options == null)
        return packageJson;
      if (packageJson.AdditionalData == null)
        packageJson.AdditionalData = (IDictionary<string, JToken>) new Dictionary<string, JToken>();
      IDictionary<string, JToken> additionalData = packageJson.AdditionalData;
      IDictionary<string, JToken> clientProvidedData = options.AdditionalClientProvidedData;
      JToken valueOrDefault = clientProvidedData != null ? clientProvidedData.GetValueOrDefault<string, JToken>("gitHead") : (JToken) null;
      additionalData.SetIfNotNull<string, JToken>("gitHead", valueOrDefault);
      if ((options.ContainsServerJsFileAtRoot ? 1 : (options.ContainsBindingGypFileAtRoot ? 1 : 0)) == 0)
        return packageJson;
      if (packageJson.Scripts == null)
        packageJson.Scripts = new Dictionary<string, string>();
      if (!packageJson.Scripts.ContainsKey("start") && options.ContainsServerJsFileAtRoot)
        packageJson.Scripts["start"] = "node server.js";
      if (!packageJson.Scripts.ContainsKey("install") && options.ContainsBindingGypFileAtRoot)
        packageJson.Scripts["install"] = "node-gyp rebuild";
      return packageJson;
    }

    public static JsonSerializerSettings GetJsonSerializerSettings(
      List<Exception> errors,
      IJsonErrorLogger logger)
    {
      JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
      serializerSettings.Converters.Add((JsonConverter) new PersonJsonConverter());
      serializerSettings.Error = (EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs>) ((sender, args) =>
      {
        if (args.ErrorContext.Error.GetType() == typeof (RequiredFieldPackageJsonException) || args.CurrentObject != args.ErrorContext.OriginalObject)
          return;
        logger.LogJsonError(args.ErrorContext);
        errors.Add(args.ErrorContext.Error);
        args.ErrorContext.Handled = true;
      });
      serializerSettings.CheckAdditionalContent = true;
      return serializerSettings;
    }
  }
}
