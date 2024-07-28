// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskContribution
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public sealed class TaskContribution : IDisposable
  {
    private const int c_80KB = 81920;
    private const string c_fallbackLocale = "en-US";
    private readonly Lazy<CultureInfo> m_cultureInfo;
    private readonly IVssRequestContext m_requestContext;
    private static readonly HashSet<string> LegacyTasksAlwaysRezip = new HashSet<string>()
    {
      "df857559-8715-46eb-a74e-ac98b9178aa0_1",
      "80f3f6a0-82a6-4a22-ba7a-e5b8c541b9b9_1",
      "b832bec5-8c27-4fef-9fb8-6bec8524ad8a_0",
      "d8b84976-e99a-4b86-b885-4849694435b0_1",
      "46e4be58-730b-4389-8a2f-ea10b3e5e815_0",
      "537fdb7a-a601-4537-aa70-92645a2b5ce4_0",
      "72a1931b-effb-4d2e-8fd8-f8472a07cb62_1",
      "94a74903-f93f-4075-884f-dc11f34058b4_1",
      "497d490f-eea7-4f2b-ab94-48d9c1acdcb1_2",
      "dcbef2c9-e4f4-4929-82b2-ea7fc9166109_1",
      "d9bafed4-0b18-4f58-968d-86655b4d2ce9_1",
      "5bfb729a-a7c8-4a78-a7c3-8d717bb7c13c_1",
      "1d341bb0-2106-458c-8422-d00bcea6512a_1",
      "ad8974d8-de11-11e4-b2fe-7fb898a745f3_1",
      "52a38a6a-1517-41d7-96cc-73ee0c60d2b6_1",
      "5541a522-603c-47ad-91fc-a4b1d163081b_0",
      "5541a522-603c-47ad-91fc-a4b1d163081b_1",
      "8d8eebd8-2b94-4c97-85af-839254cc6da4_1",
      "d2eff759-736d-4b7b-8554-7ba0960d49d6_0",
      "d2eff759-736d-4b7b-8554-7ba0960d49d6_1",
      "0f9f66ca-250e-40fd-9678-309bcd439d5e_0",
      "9c3e8943-130d-4c78-ac63-8af81df62dfb_0",
      "c24b86d4-4256-4925-9a29-246f81aa64a7_1",
      "ac4ee482-65da-4485-a532-7b085873e532_1",
      "e213ff0f-5d5c-4791-802d-52ea3e7be1f1_1",
      "0675668a-7bba-4ccb-901d-5ad6554ca653_1",
      "ba761f24-cbd6-48cb-92f3-fc13396405b1_0",
      "d353d6a2-e361-4a8f-8d8c-123bebb71028_1",
      "97ef6e59-b8cc-48aa-9937-1a01e35e7584_1",
      "730d8de1-7a4f-424c-9542-fe7cc02604eb_1",
      "eae5b2cc-ac5e-4cba-b022-a06621f9c01f_1",
      "ad5cd22a-be4e-48bb-adce-181a32432da5_0",
      "ff50fc97-da8c-4683-b014-34c15315ee5f_0",
      "0f077e3a-af59-496d-81bc-ad971b7464e0_1",
      "6237827d-6244-4d52-b93e-47d8610fbd8a_1",
      "f54d001c-999f-408a-9867-0400c1838c5e_0",
      "1e78dc1b-9132-4b18-9c75-0e7ecc634b74_2",
      "1e78dc1b-9132-4b18-9c75-0e7ecc634b74_3",
      "1e78dc1b-9132-4b18-9c75-0e7ecc634b74_4"
    };

    internal TaskContribution(
      IVssRequestContext requestContext,
      TaskDefinition definition,
      ITaskPackageReader reader)
    {
      this.m_requestContext = requestContext;
      this.m_cultureInfo = new Lazy<CultureInfo>((Func<CultureInfo>) (() => this.m_requestContext.ServiceHost.DeploymentServiceHost.GetCulture(this.m_requestContext)));
      this.Reader = reader;
      this.Definition = definition;
    }

    public TaskDefinition Definition { get; }

    internal ITaskPackageReader Reader { get; }

    public void Dispose() => this.Reader?.Dispose();

    public async Task<TaskPackageResources> GetPackageResourcesAsync()
    {
      bool flag = this.Reader.OriginalStream != null && !TaskContribution.LegacyTasksAlwaysRezip.Contains(string.Format("{0}_{1}", (object) this.Definition.Id, (object) this.Definition.Version.Major)) && this.m_requestContext.IsFeatureEnabled("DistributedTask.PreserveOriginalTaskDataWhenPossible");
      if (!this.m_requestContext.ExecutionEnvironment.IsHostedDeployment)
        flag &= this.IsTaskIntegrityValidationEnabled();
      return flag ? await this.GetOriginalPackageResourcesAsync() : await this.GetLocalizedPackageResourcesAsync();
    }

    private bool IsTaskIntegrityValidationEnabled()
    {
      try
      {
        return Convert.ToBoolean(this.m_requestContext.GetService<IVssRegistryService>().GetValue<int>(this.m_requestContext, in RegistryKeys.EnableTaskIntegrityValidationOnPremise, 0));
      }
      catch
      {
        return false;
      }
    }

    private async Task<TaskPackageResources> GetOriginalPackageResourcesAsync()
    {
      bool successful = false;
      TaskPackageResource icon = (TaskPackageResource) null;
      TaskPackageResources packageResourcesAsync;
      try
      {
        if (this.Reader.Exists("icon.png"))
        {
          icon = new TaskPackageResource()
          {
            Name = "icon.png",
            Stream = (Stream) new MemoryStream()
          };
          using (Stream iconStream = this.Reader.GetStream("icon.png"))
            await iconStream.CopyToAsync(icon.Stream, 81920, this.m_requestContext.CancellationToken);
          icon.Length = icon.Stream.Length;
          icon.Stream.Seek(0L, SeekOrigin.Begin);
        }
        this.Reader.OriginalStream.Seek(0L, SeekOrigin.Begin);
        successful = true;
        packageResourcesAsync = new TaskPackageResources(this.Definition, icon, this.Reader.OriginalStream);
      }
      finally
      {
        if (!successful)
          icon?.Dispose();
      }
      icon = (TaskPackageResource) null;
      return packageResourcesAsync;
    }

    private async Task<TaskPackageResources> GetLocalizedPackageResourcesAsync()
    {
      bool successful = false;
      TaskPackageResource icon = (TaskPackageResource) null;
      SegmentedMemoryStream zipStream = (SegmentedMemoryStream) null;
      try
      {
        zipStream = new SegmentedMemoryStream();
        bool taskZipHasBeenModified = false;
        TaskDefinition definition = this.Definition.Clone();
        using (ZipArchive zipArchive = new ZipArchive((Stream) zipStream, ZipArchiveMode.Create, true))
        {
          foreach (string entryPath in this.Reader.GetEntries())
          {
            Stream sourceStream = (Stream) null;
            try
            {
              string zipEntryPath = entryPath.Replace('\\', '/').TrimStart('/');
              Stream iconStream;
              if (zipEntryPath.Equals("task.json", StringComparison.OrdinalIgnoreCase))
              {
                Stream stream = this.Reader.GetStream(entryPath);
                sourceStream = TaskContribution.GetLocalizedStream(this.Reader, this.m_cultureInfo.Value, stream);
                if (sourceStream != stream)
                {
                  taskZipHasBeenModified = true;
                  try
                  {
                    definition = JsonUtility.Deserialize<TaskDefinition>(sourceStream, true);
                    definition.ContributionIdentifier = this.Definition.ContributionIdentifier;
                    definition.ContributionVersion = this.Definition.ContributionVersion;
                  }
                  catch (JsonReaderException ex)
                  {
                    throw new InvalidTaskJsonException(TaskResources.TaskDefinitionDeserializationError((object) ex.Message)).Expected("DistributedTask");
                  }
                  sourceStream.Seek(0L, SeekOrigin.Begin);
                }
              }
              else
              {
                if (zipEntryPath.Equals("icon.png", StringComparison.OrdinalIgnoreCase))
                {
                  icon = new TaskPackageResource()
                  {
                    Name = "icon.png"
                  };
                  icon.Stream = (Stream) new MemoryStream();
                  iconStream = this.Reader.GetStream(entryPath);
                  try
                  {
                    await iconStream.CopyToAsync(icon.Stream, 81920, this.m_requestContext.CancellationToken);
                  }
                  finally
                  {
                    iconStream?.Dispose();
                  }
                  iconStream = (Stream) null;
                  icon.Length = icon.Stream.Length;
                  icon.Stream.Seek(0L, SeekOrigin.Begin);
                }
                sourceStream = this.Reader.GetStream(entryPath);
              }
              iconStream = zipArchive.CreateEntry(zipEntryPath).Open();
              try
              {
                await sourceStream.CopyToAsync(iconStream, 81920, this.m_requestContext.CancellationToken);
              }
              finally
              {
                iconStream?.Dispose();
              }
              iconStream = (Stream) null;
              zipEntryPath = (string) null;
            }
            finally
            {
              sourceStream?.Dispose();
            }
            sourceStream = (Stream) null;
          }
        }
        zipStream.Seek(0L, SeekOrigin.Begin);
        zipStream.MakeReadOnly();
        successful = true;
        if (taskZipHasBeenModified || this.Reader.OriginalStream == null || !this.m_requestContext.IsFeatureEnabled("DistributedTask.PreserveOriginalTaskDataWhenPossible"))
          return new TaskPackageResources(definition, icon, (Stream) zipStream);
        this.Reader.OriginalStream.Seek(0L, SeekOrigin.Begin);
        return new TaskPackageResources(definition, icon, this.Reader.OriginalStream);
      }
      finally
      {
        if (!successful)
        {
          icon?.Dispose();
          zipStream?.Dispose();
        }
      }
    }

    public static TaskContribution Create(
      IVssRequestContext requestContext,
      ITaskPackageReader reader)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ITaskPackageReader>(reader, nameof (reader));
      string path = "task.json";
      if (!reader.Exists(path))
        return new TaskContribution(requestContext, (TaskDefinition) null, reader);
      TaskDefinition definition;
      using (Stream stream = reader.GetStream(path))
      {
        try
        {
          definition = JsonUtility.Deserialize<TaskDefinition>(stream);
        }
        catch (JsonReaderException ex)
        {
          throw new InvalidTaskJsonException(TaskResources.TaskDefinitionDeserializationError((object) ex.Message)).Expected("DistributedTask");
        }
      }
      return new TaskContribution(requestContext, definition, reader);
    }

    internal static TaskContribution Create(
      IVssRequestContext requestContext,
      SegmentedMemoryStream packageStream)
    {
      bool flag = false;
      ZipArchive archive = (ZipArchive) null;
      ZipPackageReader reader = (ZipPackageReader) null;
      packageStream.MakeReadOnly();
      packageStream.Seek(0L, SeekOrigin.Begin);
      try
      {
        archive = new ZipArchive((Stream) packageStream, ZipArchiveMode.Read);
        reader = new ZipPackageReader(archive, true, (Stream) packageStream);
        TaskContribution taskContribution = TaskContribution.Create(requestContext, (ITaskPackageReader) reader);
        flag = true;
        return taskContribution;
      }
      finally
      {
        if (!flag)
        {
          reader?.Dispose();
          archive?.Dispose();
          packageStream.Dispose();
        }
      }
    }

    private static Stream GetLocalizedStream(
      ITaskPackageReader package,
      CultureInfo culture,
      Stream source)
    {
      string path1 = "task.loc.json";
      if (!package.Exists(path1))
        return source;
      string path2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Strings/resources.resjson/{0}/resources.resjson", (object) culture.Name);
      if (!package.Exists(path2))
        return source;
      JObject jobject = (JObject) null;
      using (Stream stream = package.GetStream(path2))
        jobject = TaskLocalization.ReadJsonStream(stream);
      string path3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Strings/resources.resjson/{0}/resources.resjson", (object) "en-US");
      if (package.Exists(path3))
      {
        JObject sourceDocument = (JObject) null;
        using (Stream stream = package.GetStream(path3))
          sourceDocument = TaskLocalization.ReadJsonStream(stream);
        TaskLocalization.MergeResourcesDocuments(jobject, sourceDocument);
      }
      JObject localizationDocument = (JObject) null;
      using (Stream stream = package.GetStream(path1))
        localizationDocument = TaskLocalization.ReadJsonStream(stream);
      source.Dispose();
      return TaskLocalization.LocalizeDocument(localizationDocument, jobject);
    }
  }
}
