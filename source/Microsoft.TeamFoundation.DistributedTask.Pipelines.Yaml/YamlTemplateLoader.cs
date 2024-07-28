// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.YamlTemplateLoader
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml
{
  internal sealed class YamlTemplateLoader : ITemplateLoader
  {
    private readonly IFileProviderFactory m_fileProviderFactory;
    private readonly ParseOptions m_parseOptions;
    private readonly PipelineResources m_resources;
    private readonly HashSet<string> m_referencedFiles = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, string> m_volatileCache = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public YamlTemplateLoader(
      ParseOptions parseOptions,
      IFileProviderFactory fileProviderFactory,
      PipelineResources resources = null)
    {
      this.m_parseOptions = new ParseOptions(parseOptions);
      this.m_fileProviderFactory = fileProviderFactory ?? throw new ArgumentNullException(nameof (fileProviderFactory));
      this.m_resources = resources;
    }

    public ParseOptions ParseOptions => this.m_parseOptions;

    public YamlTemplateComposition Composition { get; } = new YamlTemplateComposition();

    public LoadTemplateResult Load(
      TemplateContext context,
      string repository,
      string path,
      string templateType)
    {
      if (context.Errors.Count > 0)
        throw new InvalidOperationException("Expected error count to be 0 when attempting to load a new file");
      IFileProvider provider = this.m_fileProviderFactory.GetProvider(repository);
      path = provider.ResolvePath(string.Empty, path);
      provider.GetFileName(path);
      context.State["currentRepository"] = (object) repository;
      context.State["currentDirectory"] = (object) provider.GetDirectoryName(path);
      context.State["currentFile"] = (object) path;
      this.Composition.AddRootYamlFile(new YamlTemplateLocation(repository, path));
      return this.Load(context, provider, path, templateType);
    }

    public LoadTemplateResult Load(
      TemplateContext context,
      string name,
      LiteralToken referencePath,
      IDictionary<string, object> previousState)
    {
      if (context.Errors.Count > 0)
        throw new InvalidOperationException("Expected error count to be 0 when attempting to load a new file");
      string templateType;
      switch (name)
      {
        case "variablesTemplateReference":
          templateType = "variablesTemplate";
          break;
        case "stagesTemplateReference":
          templateType = "stagesTemplate";
          break;
        case "extendsTemplateReference":
          templateType = "extendsTemplate";
          break;
        case "parametersTemplateReference":
          templateType = "parametersTemplate";
          break;
        case "jobsTemplateReference":
          templateType = "jobsTemplate";
          break;
        case "stepsTemplateReference":
          templateType = "stepsTemplate";
          break;
        default:
          throw new NotSupportedException("Unexpected template reference name '" + name + "'");
      }
      string str1 = (string) null;
      string str2 = referencePath.Value;
      int length = str2.IndexOf("@");
      string path1;
      if (length >= 0)
      {
        path1 = str2.Substring(0, length);
        str1 = str2.Substring(length + 1);
      }
      else
        path1 = str2;
      string defaultRoot;
      if (string.IsNullOrEmpty(str1))
      {
        str1 = context.State["currentRepository"] as string;
        defaultRoot = context.State["currentDirectory"] as string;
      }
      else
        defaultRoot = !string.Equals(str1, context.State["currentRepository"] as string, StringComparison.OrdinalIgnoreCase) ? string.Empty : context.State["currentDirectory"] as string;
      IFileProvider provider = this.m_fileProviderFactory.GetProvider(str1);
      string path2 = provider.ResolvePath(defaultRoot, path1);
      context.State["currentRepository"] = (object) str1;
      context.State["currentDirectory"] = (object) provider.GetDirectoryName(path2);
      context.State["currentFile"] = (object) path2;
      string repositoryAlias = previousState["currentRepository"] as string;
      string path3 = previousState["currentFile"] as string;
      this.Composition.AddTemplate(new YamlTemplateLocation(str1, path2), new YamlTemplateLocation(repositoryAlias, path3), templateType.Equals("extendsTemplate"));
      LoadTemplateResult loadTemplateResult = this.Load(context, provider, path2, templateType);
      if (context.Errors.Count == 0)
      {
        MappingToken mappingToken = TemplateUtil.AssertMapping(loadTemplateResult.Value, "template");
        for (int index = 0; index < mappingToken.Count; ++index)
        {
          KeyValuePair<ScalarToken, TemplateToken> keyValuePair = mappingToken[index];
          if (string.Equals(TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "template key").Value, "resources", StringComparison.OrdinalIgnoreCase))
          {
            keyValuePair = mappingToken[index];
            MappingToken resources = TemplateUtil.AssertMapping(keyValuePair.Value, "resources");
            PipelineResources repositoryResources = TemplateResultConverter.ConvertToPipelineRepositoryResources(context, (TemplateToken) resources, false);
            foreach (RepositoryResource repository in (IEnumerable<RepositoryResource>) repositoryResources.Repositories)
              this.m_fileProviderFactory.AddRepository(repository);
            if (this.m_resources == null)
              throw new NotSupportedException("Resources are defined but resource object is null");
            this.m_resources.MergeWith(repositoryResources);
            break;
          }
        }
      }
      return loadTemplateResult;
    }

    private LoadTemplateResult Load(
      TemplateContext context,
      IFileProvider fileProvider,
      string path,
      string templateType)
    {
      string key = "repo=" + Uri.EscapeDataString(fileProvider.Repository.Alias) + "/file=" + Uri.EscapeDataString(path);
      this.m_referencedFiles.Add(key);
      if (this.m_parseOptions.MaxFiles > 0 && this.m_referencedFiles.Count > this.m_parseOptions.MaxFiles)
        throw new InvalidOperationException(YamlStrings.MaxFilesExceeded((object) this.m_parseOptions.MaxFiles));
      string str;
      if (!this.m_volatileCache.TryGetValue(key, out str))
      {
        context.CancellationToken.ThrowIfCancellationRequested();
        str = fileProvider.GetFileContent(path);
        context.Memory.AddFileSize(str.Length);
        if (str.Length > this.m_parseOptions.MaxFileSize)
          throw new InvalidOperationException(YamlStrings.MaxFileSizeExceeded((object) this.m_parseOptions.MaxFileSize));
        str = this.RemoveBOM(str);
        this.m_volatileCache[key] = str;
      }
      string displayPath = this.GetDisplayPath(fileProvider, path);
      int fileId = context.GetFileId(displayPath);
      int bytes;
      TemplateToken templateToken;
      using (StringReader input = new StringReader(str))
      {
        YamlObjectReader yamlObjectReader = new YamlObjectReader((TextReader) input);
        bool includeFileContentInErrors = fileProvider.Repository.Properties.Get<bool>(RepositoryPropertyNames.HasReadAccess, true);
        templateToken = TemplateReader.Read(context, templateType, (IObjectReader) yamlObjectReader, new int?(fileId), includeFileContentInErrors, out bytes);
      }
      return new LoadTemplateResult(templateType, templateToken, bytes, new int?(fileId));
    }

    private string GetDisplayPath(IFileProvider fileProvider, string path) => string.Equals(fileProvider.Repository.Alias, "self", StringComparison.OrdinalIgnoreCase) ? path : path + "@" + fileProvider.Repository.Alias;

    private void SetFileId(TemplateToken value, int fileId)
    {
      if (value == null)
        return;
      value.FileId = new int?(fileId);
      switch (value.Type)
      {
        case 0:
          break;
        case 1:
          using (IEnumerator<TemplateToken> enumerator = (value as SequenceToken).GetEnumerator())
          {
            while (enumerator.MoveNext())
              this.SetFileId(enumerator.Current, fileId);
            break;
          }
        case 2:
          using (IEnumerator<KeyValuePair<ScalarToken, TemplateToken>> enumerator = (value as MappingToken).GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              KeyValuePair<ScalarToken, TemplateToken> current = enumerator.Current;
              this.SetFileId((TemplateToken) current.Key, fileId);
              this.SetFileId(current.Value, fileId);
            }
            break;
          }
        case 3:
          break;
        case 4:
          break;
        case 5:
          break;
        case 6:
          break;
        case 7:
          break;
        case 8:
          break;
        default:
          throw new NotSupportedException(string.Format("Unexpected template type '{0}' encountered when setting file ID", (object) value.Type));
      }
    }

    private string RemoveBOM(string fileContent)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(fileContent);
      return bytes.Length >= 3 && bytes[0] == (byte) 239 && bytes[1] == (byte) 187 && bytes[2] == (byte) 191 ? Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3) : fileContent;
    }
  }
}
