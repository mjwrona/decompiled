// Decompiled with JetBrains decompiler
// Type: WebGrease.Activities.ResourcesResolver
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text.RegularExpressions;

namespace WebGrease.Activities
{
  internal sealed class ResourcesResolver
  {
    internal static readonly Regex LocalizationResourceKeyRegex = new Regex("%([-./\\w_]+)(\\:\\w*)?%", RegexOptions.Compiled);
    private readonly List<ResourceDirectoryPath> resourceDirectoryPaths = new List<ResourceDirectoryPath>();
    private string outputDirectoryPath;
    private IEnumerable<string> resourceKeys;

    private ResourcesResolver(
      IWebGreaseContext context,
      string inputContentDirectory,
      string resourceGroupKey,
      string applicationDirectoryName,
      string siteName,
      IEnumerable<string> resourceKeys,
      string outputDirectoryPath)
    {
      ResourcesResolver resourcesResolver = this;
      DirectoryInfo contentDirectoryInfo = new DirectoryInfo(inputContentDirectory);
      Safe.FileLock((FileSystemInfo) contentDirectoryInfo, (Action) (() =>
      {
        foreach (DirectoryInfo enumerateDirectory1 in contentDirectoryInfo.EnumerateDirectories())
        {
          if (string.Compare(enumerateDirectory1.Name, applicationDirectoryName, StringComparison.OrdinalIgnoreCase) == 0)
          {
            string path = Path.Combine(enumerateDirectory1.FullName, siteName ?? string.Empty);
            if (Directory.Exists(path))
            {
              foreach (DirectoryInfo enumerateDirectory2 in new DirectoryInfo(path).EnumerateDirectories(resourceGroupKey, SearchOption.AllDirectories))
              {
                closure_0.resourceDirectoryPaths.Add(new ResourceDirectoryPath()
                {
                  AllowOverrides = true,
                  Directory = enumerateDirectory2.FullName
                });
                context.Cache.CurrentCacheSection.AddSourceDependency(enumerateDirectory2.FullName, "*.resx");
              }
            }
          }
          else
          {
            foreach (DirectoryInfo enumerateDirectory3 in enumerateDirectory1.EnumerateDirectories(resourceGroupKey, SearchOption.AllDirectories))
            {
              closure_0.resourceDirectoryPaths.Add(new ResourceDirectoryPath()
              {
                AllowOverrides = false,
                Directory = enumerateDirectory3.FullName
              });
              context.Cache.CurrentCacheSection.AddSourceDependency(enumerateDirectory3.FullName, "*.resx");
            }
          }
        }
        closure_0.outputDirectoryPath = outputDirectoryPath;
        ResourcesResolver resourcesResolver1 = closure_0;
        IEnumerable<string> strings = resourceKeys;
        if (strings == null)
          strings = (IEnumerable<string>) new List<string>()
          {
            "generic-generic"
          };
        resourcesResolver1.resourceKeys = strings;
      }));
    }

    internal static ResourcesResolver Factory(
      IWebGreaseContext context,
      string inputContentDirectory,
      string resourceGroupKey,
      string applicationDirectoryName,
      string siteName,
      IEnumerable<string> resourceKeys,
      string outputDirectoryPath)
    {
      return new ResourcesResolver(context, inputContentDirectory, resourceGroupKey, applicationDirectoryName, siteName, resourceKeys, outputDirectoryPath);
    }

    internal IDictionary<string, IDictionary<string, string>> GetMergedResources()
    {
      Dictionary<string, IDictionary<string, string>> mergedResources = new Dictionary<string, IDictionary<string, string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (string resourceKey in this.resourceKeys)
      {
        string lower = resourceKey.Trim().ToLower(CultureInfo.InvariantCulture);
        mergedResources.Add(lower, (IDictionary<string, string>) this.GetResources(resourceKey, lower));
      }
      return (IDictionary<string, IDictionary<string, string>>) mergedResources;
    }

    internal void ResolveHierarchy()
    {
      foreach (string resourceKey in this.resourceKeys)
      {
        string lower = resourceKey.Trim().ToLower(CultureInfo.InvariantCulture);
        SortedDictionary<string, string> resources = this.GetResources(resourceKey, lower);
        ResourcesResolver.WriteResources(this.outputDirectoryPath, lower, (IDictionary<string, string>) resources);
      }
    }

    private SortedDictionary<string, string> GetResources(
      string resourceKey,
      string localeOrThemeName)
    {
      SortedDictionary<string, string> output = new SortedDictionary<string, string>((IComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (ResourceDirectoryPath resourceDirectoryPath in (IEnumerable<ResourceDirectoryPath>) this.resourceDirectoryPaths.OrderBy<ResourceDirectoryPath, bool>((Func<ResourceDirectoryPath, bool>) (resourceDirectoryPath => resourceDirectoryPath.AllowOverrides)))
      {
        DirectoryInfo directoryInfo = new DirectoryInfo(resourceDirectoryPath.Directory);
        Dictionary<string, string> input = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        if (resourceKey != "generic-generic")
        {
          string str = Path.Combine(directoryInfo.FullName, "generic-generic.resx");
          if (File.Exists(str))
            input = ResourcesResolver.ReadResources(str);
        }
        Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        string str1 = Path.Combine(directoryInfo.FullName, localeOrThemeName + ".resx");
        if (File.Exists(str1))
          dictionary = ResourcesResolver.ReadResources(str1);
        ResourcesResolver.MergeResources((IDictionary<string, string>) dictionary, input, false, false);
        ResourcesResolver.MergeResources((IDictionary<string, string>) output, dictionary, resourceDirectoryPath.AllowOverrides, resourceDirectoryPath.AllowOverrides);
      }
      return output;
    }

    internal static Dictionary<string, string> ReadResources(string filePath)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      using (ResXResourceReader resXresourceReader = new ResXResourceReader(filePath))
      {
        foreach (DictionaryEntry dictionaryEntry in resXresourceReader)
        {
          string key = dictionaryEntry.Key as string;
          if (!string.IsNullOrWhiteSpace(key))
          {
            if (!(dictionaryEntry.Value is string empty))
              empty = string.Empty;
            string str = empty;
            if (dictionary.ContainsKey(key))
              throw new BuildWorkflowException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ResourceStrings.ResourceResolverDuplicateKeyExceptionMessage, new object[2]
              {
                (object) key,
                (object) filePath
              }));
            dictionary.Add(key, str);
          }
        }
      }
      return dictionary;
    }

    internal static string ExpandResourceKeys(string input, IDictionary<string, string> resources)
    {
      string str;
      return input == null || resources == null || resources.Count == 0 ? input : ResourcesResolver.LocalizationResourceKeyRegex.Replace(input, (MatchEvaluator) (match => !resources.TryGetValue(match.Result("$1"), out str) ? match.Value : str));
    }

    private static void MergeResources(
      IDictionary<string, string> output,
      Dictionary<string, string> input,
      bool allowOverrides,
      bool throwsException)
    {
      foreach (string key in input.Keys)
      {
        if (output.ContainsKey(key))
        {
          if (allowOverrides)
            output[key] = input[key];
          else if (throwsException)
            throw new ResourceOverrideException((string) null, key);
        }
        else
          output.Add(key, input[key]);
      }
    }

    private static void WriteResources(
      string outputDirectoryPath,
      string key,
      IDictionary<string, string> resources)
    {
      if (resources == null || resources.Count == 0)
        return;
      Directory.CreateDirectory(outputDirectoryPath);
      using (ResXResourceWriter resXresourceWriter = new ResXResourceWriter(Path.Combine(outputDirectoryPath, key + ".resx")))
      {
        foreach (string key1 in (IEnumerable<string>) resources.Keys)
          resXresourceWriter.AddResource(key1, resources[key1]);
      }
    }

    public static IEnumerable<Tuple<List<string>, Dictionary<string, string>>> GetGroupedUsedResourceKeys(
      string css,
      IDictionary<string, IDictionary<string, string>> resources)
    {
      HashSet<string> stringSet = new HashSet<string>(resources.Values.SelectMany<IDictionary<string, string>, string>((Func<IDictionary<string, string>, IEnumerable<string>>) (v => (IEnumerable<string>) v.Keys)).Distinct<string>());
      HashSet<string> usedResourceKeys = new HashSet<string>((IEnumerable<string>) ResourcesResolver.LocalizationResourceKeyRegex.Matches(css).OfType<Match>().Select<Match, string>((Func<Match, string>) (m => m.Groups[1].Value)).Where<string>(new Func<string, bool>(stringSet.Contains)).OrderBy<string, string>((Func<string, string>) (rk => rk)).ToArray<string>());
      Dictionary<string, Tuple<List<string>, Dictionary<string, string>>> dictionary = new Dictionary<string, Tuple<List<string>, Dictionary<string, string>>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (KeyValuePair<string, IDictionary<string, string>> resource in (IEnumerable<KeyValuePair<string, IDictionary<string, string>>>) resources)
      {
        Dictionary<string, string> source = new Dictionary<string, string>((IDictionary<string, string>) resource.Value.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (kvp => usedResourceKeys.Contains(kvp.Key))).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, string>, string>) (kvp => kvp.Value)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        string key = string.Join("%", source.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (kv => kv.ToString())));
        Tuple<List<string>, Dictionary<string, string>> tuple;
        if (!dictionary.TryGetValue(key, out tuple))
        {
          tuple = new Tuple<List<string>, Dictionary<string, string>>(new List<string>(), source);
          dictionary.Add(key, tuple);
        }
        tuple.Item1.Add(resource.Key);
      }
      return (IEnumerable<Tuple<List<string>, Dictionary<string, string>>>) dictionary.Values;
    }
  }
}
