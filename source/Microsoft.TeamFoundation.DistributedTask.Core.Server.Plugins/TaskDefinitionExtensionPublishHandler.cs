// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.TaskDefinitionExtensionPublishHandler
// Assembly: Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6486B3F7-B3D2-46E4-8024-05D53FB42B10
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.Types.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Net;

namespace Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins
{
  public class TaskDefinitionExtensionPublishHandler : IExtensionPublishHandler
  {
    public bool UpdateExtension(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      PackageDetails packageDetails,
      Stream packageStream,
      string version,
      Guid validationId)
    {
      IList<Contribution> taskContributions = TaskDefinitionExtensionPublishHandler.GetBuildTaskContributions(packageStream);
      if (taskContributions == null || taskContributions.Count == 0)
      {
        requestContext.Trace(62600, TraceLevel.Info, "DistributedTaskCore", "GalleryBuildTaskValidation", "No build task contributions for Extension {0}, Publisher {1}", (object) extension.ExtensionName, (object) extension.Publisher.PublisherName);
        return true;
      }
      Dictionary<string, Guid> toBePublishedContributions = new Dictionary<string, Guid>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (Contribution contribution in (IEnumerable<Contribution>) taskContributions)
      {
        string taskRoot;
        if (contribution?.Properties == null || !contribution.Properties.TryGetValue<string>("name", out taskRoot) || taskRoot == null)
          throw new ContributionDoesNotTargetBuildTaskException(Resources.ContributionDoesNotTargetBuildTask((object) contribution?.Id));
        IList<TaskDefinition> buildTasks = TaskDefinitionExtensionPublishHandler.ExtractBuildTasks(packageStream, taskRoot);
        foreach (TaskDefinition taskDefinition in (IEnumerable<TaskDefinition>) buildTasks)
          TaskDefinitionValidator.CheckTaskDefinition(taskDefinition);
        string key = "Microsoft.VisualStudio.Services.TaskId" + (object) '.' + contribution.Id.ToUpperInvariant();
        toBePublishedContributions.Add(key, buildTasks.FirstOrDefault<TaskDefinition>().Id);
      }
      bool flag = true;
      if (extension.Flags.HasFlag((System.Enum) PublishedExtensionFlags.Public))
      {
        flag = TaskDefinitionValidator.ValidateContributions(requestContext, toBePublishedContributions, extension.Publisher.PublisherName, extension.ExtensionName);
        if (flag)
        {
          foreach (ExtensionVersion version1 in extension.Versions)
          {
            if (version1.Version == version)
            {
              if (version1.Properties == null)
                version1.Properties = new List<KeyValuePair<string, string>>();
              foreach (KeyValuePair<string, Guid> keyValuePair in toBePublishedContributions)
                version1.Properties.Add(new KeyValuePair<string, string>(keyValuePair.Key, keyValuePair.Value.ToString().ToUpperInvariant()));
            }
          }
        }
      }
      return flag;
    }

    public void ValidateExtension(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      PackageDetails packageDetails,
      Stream packageStream,
      string version,
      Guid validationId)
    {
    }

    private static IList<Contribution> GetBuildTaskContributions(Stream packageStream)
    {
      List<Contribution> taskContributions = new List<Contribution>();
      VSIXPackage.Parse(packageStream, (Func<ManifestFile, Stream, bool>) ((manifestFile, fileStream) =>
      {
        if (manifestFile.AssetType == "Microsoft.VisualStudio.Services.Manifest")
        {
          ExtensionManifest extensionManifest = JsonUtility.Deserialize<ExtensionManifest>(fileStream);
          if (extensionManifest.Contributions != null)
          {
            foreach (Contribution contribution in extensionManifest.Contributions)
            {
              if (contribution.Type == "ms.vss-distributed-task.task")
                taskContributions.Add(contribution);
            }
          }
        }
        return false;
      }));
      return (IList<Contribution>) taskContributions;
    }

    private static IList<TaskDefinition> ExtractBuildTasks(Stream packageStream, string taskRoot)
    {
      IList<TaskDefinition> buildTasks = (IList<TaskDefinition>) new List<TaskDefinition>();
      TaskDefinition taskDefinition1 = (TaskDefinition) null;
      TaskDefinition taskDefinition2 = (TaskDefinition) null;
      string str1 = (string) null;
      using (Package package = Package.Open(packageStream))
      {
        foreach (PackagePart part in package.GetParts())
        {
          if (TaskDefinitionValidator.IsTaskJsonPath(part.Uri.OriginalString, taskRoot))
          {
            string originalString = part.Uri.OriginalString;
            string str2 = part.Uri.OriginalString.Substring(1, originalString.Length - 11);
            try
            {
              taskDefinition2 = JsonUtility.Deserialize<TaskDefinition>(part.GetStream());
            }
            catch (Exception ex)
            {
              throw new InvalidTaskJsonException(Resources.TaskDefinitionCouldNotBeDeserialized((object) str2) + ex.Message);
            }
            buildTasks.Add(taskDefinition2);
            if (taskDefinition1 == null)
            {
              taskDefinition1 = taskDefinition2;
              str1 = str2;
            }
            else if (!taskDefinition1.Id.Equals(taskDefinition2.Id))
              throw new TaskIdsDoNotMatchException(Resources.ContributionTaskIdsShouldMatch((object) taskRoot, (object) taskDefinition1.Version, (object) taskDefinition2.Version, (object) str1, (object) str2));
          }
          else if (TaskDefinitionValidator.IsTaskZipPath(part.Uri.OriginalString, taskRoot))
          {
            Stream stream = part.GetStream();
            Stream streamToRead = (Stream) null;
            try
            {
              using (ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Read, true))
              {
                ZipArchiveEntry entry = zipArchive.GetEntry("task.json");
                if (entry != null)
                  streamToRead = entry.Open();
              }
              string originalString = part.Uri.OriginalString;
              string str3 = part.Uri.OriginalString.Substring(1, originalString.Length - "/task.zip".Length - 1);
              try
              {
                taskDefinition2 = JsonUtility.Deserialize<TaskDefinition>(streamToRead);
              }
              catch (Exception ex)
              {
                throw new InvalidTaskJsonException(Resources.TaskDefinitionCouldNotBeDeserialized((object) str3) + ex.Message);
              }
              buildTasks.Add(taskDefinition2);
              if (taskDefinition1 == null)
              {
                taskDefinition1 = taskDefinition2;
                str1 = str3;
              }
              else if (!taskDefinition1.Id.Equals(taskDefinition2.Id))
                throw new TaskIdsDoNotMatchException(Resources.ContributionTaskIdsShouldMatch((object) taskRoot, (object) taskDefinition1.Version, (object) taskDefinition2.Version, (object) str1, (object) str3));
            }
            finally
            {
              stream?.Dispose();
              streamToRead?.Dispose();
            }
          }
        }
      }
      if (taskDefinition2 == null)
        throw new TaskJsonNotFoundException(Resources.TaskJsonNotFound((object) taskRoot));
      return buildTasks;
    }

    private static Stream GetAssetByNameAsync(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version)
    {
      GalleryHttpClient client = requestContext.GetClient<GalleryHttpClient>();
      if (version == null)
        version = "latest";
      try
      {
        return client.GetAssetByNameAsync(publisherName, extensionName, version, "Microsoft.VisualStudio.Services.VSIXPackage").SyncResult<Stream>();
      }
      catch (VssServiceResponseException ex)
      {
        if (ex.HttpStatusCode == HttpStatusCode.Found)
        {
          requestContext.GetClient<LocationHttpClient>(new Guid("00000029-0000-8888-8000-000000000000")).GetConnectionDataAsync(ConnectOptions.None, 0L).SyncResult<ConnectionData>();
          return client.GetAssetByNameAsync(publisherName, extensionName, version, "Microsoft.VisualStudio.Services.VSIXPackage").SyncResult<Stream>();
        }
        throw;
      }
    }
  }
}
