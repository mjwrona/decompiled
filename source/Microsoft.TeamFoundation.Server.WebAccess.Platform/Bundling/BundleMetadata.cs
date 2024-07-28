// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.BundleMetadata
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  public class BundleMetadata
  {
    private const string c_bundleMetadataFolder = "Bundle_Metadata";
    private const string c_bundleMetadataSelector = "*.Bundle.Metadata.json";
    private static IDictionary<string, IDictionary<string, BundleModuleMetadata>> sm_modulesMetadataByVersion = (IDictionary<string, IDictionary<string, BundleModuleMetadata>>) new Dictionary<string, IDictionary<string, BundleModuleMetadata>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static IDictionary<string, short> sm_pathToIdMap = (IDictionary<string, short>) new Dictionary<string, short>();
    private static IDictionary<short, string> sm_idToPathMap = (IDictionary<short, string>) new Dictionary<short, string>();

    public static IDictionary<string, BundleModuleMetadata> LoadBundleManifests(string version = null)
    {
      if (string.IsNullOrEmpty(version))
        version = StaticResources.Versioned.Version;
      IDictionary<string, BundleModuleMetadata> modulesMetadata;
      if (!BundleMetadata.sm_modulesMetadataByVersion.TryGetValue(version, out modulesMetadata))
      {
        modulesMetadata = (IDictionary<string, BundleModuleMetadata>) new Dictionary<string, BundleModuleMetadata>();
        string physicalLocation = StaticResources.Versioned.GetPhysicalLocation("Bundle_Metadata", version: version);
        if (Directory.Exists(physicalLocation))
        {
          JsonSerializer serializer = new JsonSerializer()
          {
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
          };
          foreach (string file in Directory.GetFiles(physicalLocation, "*.Bundle.Metadata.json"))
            BundleMetadata.ReadBundleAreaMetadata(file, serializer, modulesMetadata);
        }
        BundleMetadata.sm_modulesMetadataByVersion[version] = modulesMetadata;
      }
      return modulesMetadata;
    }

    public static void ReadBundleAreaMetadata(
      string bundleMetadataPath,
      JsonSerializer serializer,
      IDictionary<string, BundleModuleMetadata> modulesMetadata)
    {
      try
      {
        using (StreamReader reader1 = new StreamReader(bundleMetadataPath, Encoding.UTF8))
        {
          using (JsonReader reader2 = (JsonReader) new JsonTextReader((TextReader) reader1))
          {
            BundleAreaMetadata bundleAreaMetadata = serializer.Deserialize<BundleAreaMetadata>(reader2);
            if (bundleAreaMetadata == null || bundleAreaMetadata.Modules == null)
              return;
            foreach (KeyValuePair<string, BundleModuleMetadata> module in (IEnumerable<KeyValuePair<string, BundleModuleMetadata>>) bundleAreaMetadata.Modules)
            {
              BundleModuleMetadata bundleModuleMetadata = new BundleModuleMetadata();
              bundleModuleMetadata.Hash = module.Value.Hash;
              if (module.Value.Dependencies != null)
              {
                List<short> shortList = new List<short>();
                foreach (short dependency in module.Value.Dependencies)
                {
                  string path = bundleAreaMetadata.PathMap[dependency];
                  short num;
                  short key;
                  if (!BundleMetadata.sm_pathToIdMap.TryGetValue(path, out num))
                  {
                    key = (short) BundleMetadata.sm_pathToIdMap.Count;
                    BundleMetadata.sm_pathToIdMap[path] = key;
                    BundleMetadata.sm_idToPathMap[key] = path;
                  }
                  else
                    key = num;
                  shortList.Add(key);
                }
                bundleModuleMetadata.Dependencies = shortList.ToArray();
              }
              modulesMetadata[module.Key] = bundleModuleMetadata;
            }
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Warning, "WebAccess", TfsTraceLayers.Content, nameof (ReadBundleAreaMetadata), ex);
      }
    }

    public static IEnumerable<string> GetDependencies(string moduleId, string version) => BundleMetadata.GetDependencies(moduleId, BundleMetadata.LoadBundleManifests(version));

    internal static IEnumerable<string> GetDependencies(
      string moduleId,
      IDictionary<string, BundleModuleMetadata> modulesMetadata)
    {
      List<string> dependencies = (List<string>) null;
      BundleModuleMetadata bundleModuleMetadata;
      if (modulesMetadata.TryGetValue(moduleId, out bundleModuleMetadata) && bundleModuleMetadata.Dependencies != null)
        dependencies = new List<string>(((IEnumerable<short>) bundleModuleMetadata.Dependencies).Select<short, string>((Func<short, string>) (pathId => BundleMetadata.sm_idToPathMap[pathId])));
      return (IEnumerable<string>) dependencies;
    }

    public static byte[] GetModuleHash(string moduleId, string version)
    {
      BundleModuleMetadata bundleModuleMetadata;
      return BundleMetadata.LoadBundleManifests(version).TryGetValue(moduleId, out bundleModuleMetadata) ? bundleModuleMetadata.Hash : (byte[]) null;
    }
  }
}
