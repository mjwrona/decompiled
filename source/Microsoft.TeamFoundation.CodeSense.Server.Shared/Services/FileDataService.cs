// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Services.FileDataService
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation.CodeSense.Server.Services
{
  public class FileDataService : IFileDataService
  {
    private readonly IFileService fileService;
    private static Lazy<IFileDataService> s_instance = new Lazy<IFileDataService>((Func<IFileDataService>) (() => (IFileDataService) new FileDataService(TeamFoundationFileServiceWrapper.GetInstance())), LazyThreadSafetyMode.ExecutionAndPublication);

    public static IFileDataService GetInstance() => FileDataService.s_instance.Value;

    public FileDataService(IFileService fileService)
    {
      ArgumentUtility.CheckForNull<IFileService>(fileService, nameof (fileService));
      this.fileService = fileService;
    }

    public T GetCachedData<T>(IVssRequestContext requestContext, int fileId) where T : class
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      Stream stream = this.fileService.RetrieveCachedFile(requestContext, fileId);
      T cachedData = default (T);
      if (stream != null)
      {
        try
        {
          using (new CodeSenseTraceWatch(requestContext, 1024043, true, TraceLayer.Job, "Deserialize json for fileId {0}", new object[1]
          {
            (object) fileId
          }))
          {
            using (JsonTextReader reader = new JsonTextReader((TextReader) new StreamReader(stream)))
              cachedData = JsonSerializer.Create(CodeSenseSerializationSettings.JsonSerializerSettings).Deserialize<T>((JsonReader) reader);
          }
        }
        catch (JsonException ex)
        {
          requestContext.Trace(1024040, TraceLayer.Service, "Deserialization failed for fileId {0}, Details: {1}", (object) fileId, (object) ex);
          throw;
        }
      }
      return cachedData;
    }

    public T GetData<T>(IVssRequestContext requestContext, int fileId) where T : class
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      T data = default (T);
      Stream stream = this.GetStream(requestContext, fileId);
      if (stream != null)
      {
        try
        {
          using (JsonTextReader reader = new JsonTextReader((TextReader) new StreamReader(stream)))
            data = JsonSerializer.Create(CodeSenseSerializationSettings.JsonSerializerSettings).Deserialize<T>((JsonReader) reader);
        }
        catch (JsonException ex)
        {
          requestContext.TraceException(1024050, "CodeSense", TraceLayer.Service, (Exception) ex);
        }
        catch (OutOfMemoryException ex)
        {
          requestContext.TraceException(1024050, "CodeSense", TraceLayer.Service, (Exception) ex);
        }
      }
      return data;
    }

    public string GetData(IVssRequestContext requestContext, int fileId)
    {
      Stream stream = this.GetStream(requestContext, fileId);
      if (stream != null)
      {
        using (StreamReader streamReader = new StreamReader(stream))
        {
          try
          {
            return streamReader.ReadToEnd();
          }
          catch (OutOfMemoryException ex)
          {
            requestContext.TraceException(1024055, "CodeSense", TraceLayer.Service, (Exception) ex);
          }
        }
      }
      return (string) null;
    }

    private Stream GetStream(IVssRequestContext requestContext, int fileId)
    {
      using (new CodeSenseTraceWatch(requestContext, 1024045, TraceLayer.ExternalFileService, "RetrieveFile {0}", new object[1]
      {
        (object) fileId
      }))
        return this.fileService.RetrieveFile(requestContext, fileId);
    }

    public int PersistData<T>(
      IVssRequestContext requestContext,
      T data,
      out long uploadedCompressedLength)
      where T : class
    {
      long streamLength = 0;
      int num = requestContext.WithTrace<int>(1024010, TraceLayer.Service, (Func<int>) (() =>
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<T>(data, nameof (data));
        MemoryStream content = new MemoryStream();
        using (JsonTextWriter jsonTextWriter = new JsonTextWriter((TextWriter) new StreamWriter((Stream) content, Encoding.UTF8)))
        {
          JsonSerializer.Create(CodeSenseSerializationSettings.JsonSerializerSettings).Serialize((JsonWriter) jsonTextWriter, (object) (T) data);
          jsonTextWriter.Flush();
          content.Seek(0L, SeekOrigin.Begin);
          CollectSizeInformationAttribute informationAttribute = ((IEnumerable<CollectSizeInformationAttribute>) ((T) data).GetType().GetCustomAttributes(typeof (CollectSizeInformationAttribute), true)).SingleOrDefault<CollectSizeInformationAttribute>();
          if (informationAttribute != null)
          {
            VssPerformanceCounterManager.GetPerformanceCounter(informationAttribute.CounterUri).IncrementBy(content.Length);
            VssPerformanceCounterManager.GetPerformanceCounter(informationAttribute.CounterBaseUri).Increment();
          }
          streamLength = content.Length;
          using (new CodeSenseTraceWatch(requestContext, 1024010, TraceLayer.ExternalFileService, "UploadFile", Array.Empty<object>()))
            return this.fileService.UploadFile(requestContext, (Stream) content);
        }
      }), nameof (PersistData));
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      uploadedCompressedLength = (long) ((double) streamLength * service.GetCompressionFactor(requestContext));
      return num;
    }

    public void DeleteData(IVssRequestContext requestContext, IEnumerable<int> fileIds)
    {
      using (new CodeSenseTraceWatch(requestContext, 1024010, TraceLayer.ExternalFileService, "DeleteFiles {0}", new object[1]
      {
        (object) fileIds
      }))
        this.fileService.DeleteFiles(requestContext, fileIds);
    }
  }
}
