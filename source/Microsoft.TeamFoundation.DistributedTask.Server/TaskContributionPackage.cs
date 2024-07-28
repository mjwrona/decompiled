// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskContributionPackage
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Core.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public sealed class TaskContributionPackage : IDisposable
  {
    private bool m_disposed;

    internal TaskContributionPackage(ITaskPackageReader reader, IList<TaskContribution> tasks)
    {
      this.Reader = reader;
      this.Tasks = tasks;
    }

    internal ITaskPackageReader Reader { get; }

    public IList<TaskContribution> Tasks { get; }

    public void Dispose()
    {
      if (this.m_disposed)
        return;
      this.Reader?.Dispose();
      this.m_disposed = true;
    }

    public static TaskContributionPackage Create(
      IVssRequestContext requestContext,
      ExtensionIdentifier extensionId,
      string version,
      ITaskPackageReader reader)
    {
      using (new MethodScope(requestContext, nameof (TaskContributionPackage), nameof (Create)))
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<ExtensionIdentifier>(extensionId, nameof (extensionId));
        ArgumentUtility.CheckStringForNullOrEmpty(version, nameof (version));
        ArgumentUtility.CheckForNull<ITaskPackageReader>(reader, nameof (reader));
        if (!reader.Exists("extension.vsomanifest"))
          return (TaskContributionPackage) null;
        ExtensionManifest extensionManifest;
        using (Stream stream = reader.GetStream("extension.vsomanifest"))
          extensionManifest = JsonUtility.Deserialize<ExtensionManifest>(stream);
        List<Contribution> list1 = extensionManifest.Contributions.Where<Contribution>((Func<Contribution, bool>) (x => x.Type == TaskConstants.BuildTaskContributionType)).ToList<Contribution>();
        if (list1.Count == 0)
          return (TaskContributionPackage) null;
        List<TaskContribution> tasks = new List<TaskContribution>();
        string taskJsonSuffix = "/task.json";
        string taskZipSuffix = "/task.zip";
        List<string> list2 = reader.GetEntries().Where<string>((Func<string, bool>) (x => x.EndsWith(taskJsonSuffix, StringComparison.OrdinalIgnoreCase))).ToList<string>();
        List<string> list3 = reader.GetEntries().Where<string>((Func<string, bool>) (x => x.EndsWith(taskZipSuffix, StringComparison.OrdinalIgnoreCase))).ToList<string>();
        foreach (Contribution contribution in list1)
        {
          string str1;
          if (contribution.Properties.TryGetValue<string>("name", out str1))
          {
            string str2 = (string) null;
            TaskDefinition taskDefinition1 = (TaskDefinition) null;
            string taskPath = str1.Trim('/') + "/";
            foreach (string path1 in list2.Where<string>((Func<string, bool>) (x => x.StartsWith(taskPath, StringComparison.OrdinalIgnoreCase))).ToList<string>())
            {
              TaskDefinition taskDefinition2;
              using (Stream stream = reader.GetStream(path1))
                taskDefinition2 = JsonUtility.Deserialize<TaskDefinition>(stream);
              TaskDefinitionValidator.CheckTaskDefinition(taskDefinition2);
              taskDefinition2.ContributionIdentifier = ExtensionUtil.GetFullyQualifiedReference(extensionId.ToString(), contribution.Id, false);
              taskDefinition2.ContributionVersion = version;
              if (taskDefinition1 == null)
              {
                taskDefinition1 = taskDefinition2;
                str2 = path1;
              }
              else if (taskDefinition2.Id != taskDefinition1.Id)
                throw new TaskIdsDoNotMatchException(TaskResources.ContributionTaskIdsShouldMatch((object) contribution.Id, (object) taskDefinition1.Version, (object) taskDefinition2.Version, (object) str2, (object) path1));
              string path2 = path1.Substring(0, path1.Length - taskJsonSuffix.Length);
              tasks.Add(new TaskContribution(requestContext, taskDefinition2, reader.CreateReader(path2)));
            }
            foreach (string path in list3.Where<string>((Func<string, bool>) (x => x.StartsWith(taskPath, StringComparison.OrdinalIgnoreCase))).ToList<string>())
            {
              TaskDefinition taskDefinition3;
              using (Stream stream = reader.GetStream(path))
              {
                using (ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Read, true))
                {
                  ZipArchiveEntry entry = zipArchive.GetEntry("task.json");
                  if (entry != null)
                  {
                    using (entry.Open())
                      taskDefinition3 = JsonUtility.Deserialize<TaskDefinition>(entry.Open());
                  }
                  else
                    continue;
                }
              }
              TaskDefinitionValidator.CheckTaskDefinition(taskDefinition3);
              taskDefinition3.ContributionIdentifier = ExtensionUtil.GetFullyQualifiedReference(extensionId.ToString(), contribution.Id, false);
              taskDefinition3.ContributionVersion = version;
              if (taskDefinition1 == null)
              {
                taskDefinition1 = taskDefinition3;
                str2 = path;
              }
              else if (taskDefinition3.Id != taskDefinition1.Id)
                throw new TaskIdsDoNotMatchException(TaskResources.ContributionTaskIdsShouldMatch((object) contribution.Id, (object) taskDefinition1.Version, (object) taskDefinition3.Version, (object) str2, (object) path));
              ZipArchive archive = new ZipArchive(reader.GetStream(path));
              SegmentedMemoryStream segmentedMemoryStream = new SegmentedMemoryStream();
              using (Stream stream = reader.GetStream(path))
                stream.CopyTo((Stream) segmentedMemoryStream);
              tasks.Add(new TaskContribution(requestContext, taskDefinition3, (ITaskPackageReader) new ZipPackageReader(archive, true, (Stream) segmentedMemoryStream)));
            }
          }
        }
        return new TaskContributionPackage(reader, (IList<TaskContribution>) tasks);
      }
    }
  }
}
