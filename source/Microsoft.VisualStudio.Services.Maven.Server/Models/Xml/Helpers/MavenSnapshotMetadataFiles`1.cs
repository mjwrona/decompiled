// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Models.Xml.Helpers.MavenSnapshotMetadataFiles`1
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Maven.Server.Models.Xml.Helpers
{
  public class MavenSnapshotMetadataFiles<T> : 
    SortedDictionary<DateTime, SortedDictionary<int, List<MavenSnapshotMetadataFile>>>
  {
    private readonly Func<T, string> pathExtractor;
    private IDictionary<MavenSnapshotInstanceId, IList<T>> instanceToFileMap = (IDictionary<MavenSnapshotInstanceId, IList<T>>) new Dictionary<MavenSnapshotInstanceId, IList<T>>();

    private MavenPackageName PackageName { get; }

    public MavenSnapshotMetadataFiles(
      MavenPackageName packageName,
      IEnumerable<T> files,
      Func<T, string> pathExtractor)
    {
      this.pathExtractor = pathExtractor;
      this.PackageName = packageName;
      this.Append(files);
    }

    private void Append(IEnumerable<T> fileList)
    {
      ArgumentUtility.CheckForNull<IEnumerable<T>>(fileList, nameof (fileList));
      this.AppendRange(this.PackageName, fileList);
    }

    private void Append(MavenPackageName name, T file)
    {
      ArgumentUtility.CheckForNull<MavenPackageName>(name, nameof (name));
      ArgumentUtility.CheckGenericForNull((object) file, nameof (file));
      string fileName = this.pathExtractor(file);
      int num = fileName.LastIndexOf('.');
      DateTime? time;
      int? build;
      string classifier;
      if (!MavenFileNameUtility.IsSnapshotFileName(fileName, true, out string _, out time, out build, out classifier) || !build.HasValue || !time.HasValue)
        return;
      MavenSnapshotMetadataFile snapshotMetadataFile = new MavenSnapshotMetadataFile()
      {
        Value = fileName.Substring(0, num),
        OriginalName = fileName,
        Classifier = classifier,
        Extension = fileName.Substring(num, fileName.Length - num).TrimStart('.'),
        BuildId = build.Value,
        Timestamp = time.Value
      };
      snapshotMetadataFile.Value = snapshotMetadataFile.Value.TrimStart(name.ArtifactId + "-");
      if (!string.IsNullOrWhiteSpace(snapshotMetadataFile.Classifier))
        snapshotMetadataFile.Value = snapshotMetadataFile.Value.TrimEnd("-" + snapshotMetadataFile.Classifier);
      if (!this.ContainsKey(snapshotMetadataFile.Timestamp))
        this.Add(snapshotMetadataFile.Timestamp, new SortedDictionary<int, List<MavenSnapshotMetadataFile>>());
      if (this[snapshotMetadataFile.Timestamp].ContainsKey(snapshotMetadataFile.BuildId))
        this[snapshotMetadataFile.Timestamp][snapshotMetadataFile.BuildId].Add(snapshotMetadataFile);
      else
        this[snapshotMetadataFile.Timestamp].Add(snapshotMetadataFile.BuildId, new List<MavenSnapshotMetadataFile>()
        {
          snapshotMetadataFile
        });
      MavenSnapshotInstanceId key = new MavenSnapshotInstanceId()
      {
        BuildId = snapshotMetadataFile.BuildId,
        Timestamp = snapshotMetadataFile.Timestamp
      };
      IList<T> objList;
      if (this.instanceToFileMap.TryGetValue(key, out objList))
        objList.Add(file);
      else
        this.instanceToFileMap[key] = (IList<T>) new List<T>()
        {
          file
        };
    }

    private void AppendRange(MavenPackageName name, IEnumerable<T> files)
    {
      ArgumentUtility.CheckForNull<IEnumerable<T>>(files, nameof (files));
      foreach (T file in files)
        this.Append(name, file);
    }

    public IDictionary<MavenSnapshotInstanceId, IList<T>> GetFilesByInstances() => this.instanceToFileMap;

    public IEnumerable<T> FilterFilesByInstances(
      IEnumerable<MavenSnapshotInstanceId> snapshotInstanceIds)
    {
      return this.GetFilesByInstances().Where<KeyValuePair<MavenSnapshotInstanceId, IList<T>>>((Func<KeyValuePair<MavenSnapshotInstanceId, IList<T>>, bool>) (instanceEntry => snapshotInstanceIds.Contains<MavenSnapshotInstanceId>(instanceEntry.Key))).Select<KeyValuePair<MavenSnapshotInstanceId, IList<T>>, IList<T>>((Func<KeyValuePair<MavenSnapshotInstanceId, IList<T>>, IList<T>>) (instanceId => instanceId.Value)).SelectMany<IList<T>, T>((Func<IList<T>, IEnumerable<T>>) (fileList => (IEnumerable<T>) fileList));
    }

    public MavenSnapshotMetadataFiles<T> GetNewestSnapshotInstances(int snapshotCount)
    {
      MavenSnapshotMetadataFiles<T> snapshotInstances = new MavenSnapshotMetadataFiles<T>(this.PackageName, Enumerable.Empty<T>(), this.pathExtractor);
      List<MavenSnapshotMetadataFile> source = new List<MavenSnapshotMetadataFile>();
      int num = 0;
      List<T> flattenedFiles = this.instanceToFileMap.Values.SelectMany<IList<T>, T>((Func<IList<T>, IEnumerable<T>>) (files => (IEnumerable<T>) files)).ToList<T>();
      foreach (DateTime key1 in this.Keys.Reverse<DateTime>())
      {
        foreach (int key2 in this[key1].Keys.Reverse<int>())
        {
          source.AddRange((IEnumerable<MavenSnapshotMetadataFile>) this[key1][key2]);
          ++num;
          if (num >= snapshotCount)
          {
            snapshotInstances.AppendRange(this.PackageName, source.Select<MavenSnapshotMetadataFile, T>((Func<MavenSnapshotMetadataFile, T>) (metadataFile => flattenedFiles.Single<T>((Func<T, bool>) (f => this.pathExtractor(f).Equals(metadataFile.OriginalName, StringComparison.OrdinalIgnoreCase))))));
            return snapshotInstances;
          }
        }
      }
      snapshotInstances.AppendRange(this.PackageName, source.Select<MavenSnapshotMetadataFile, T>((Func<MavenSnapshotMetadataFile, T>) (metadataFile => flattenedFiles.Single<T>((Func<T, bool>) (f => this.pathExtractor(f).Equals(metadataFile.OriginalName, StringComparison.OrdinalIgnoreCase))))));
      return snapshotInstances;
    }
  }
}
