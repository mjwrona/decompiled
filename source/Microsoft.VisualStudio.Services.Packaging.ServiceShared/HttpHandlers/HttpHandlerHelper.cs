// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers.HttpHandlerHelper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers.PackageUpload;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers
{
  public static class HttpHandlerHelper
  {
    public static HttpMethod[] MapHttpRoute(
      string routeName,
      string template,
      IRouteHandler routeHandler,
      params HttpMethod[] allowedMethods)
    {
      if (allowedMethods.Length == 0)
        throw new ArgumentNullException(nameof (allowedMethods));
      RouteTable.Routes.MapHttpRoute(routeName, template, (object) null, (object) new RouteValueDictionary()
      {
        {
          "TFS_HostType",
          (object) new TfsApiHostTypeConstraint(TeamFoundationHostType.ProjectCollection)
        },
        {
          "method",
          (object) new HttpMethodConstraint(((IEnumerable<HttpMethod>) allowedMethods).Select<HttpMethod, string>((Func<HttpMethod, string>) (x => x.Method)).ToArray<string>())
        }
      }).RouteHandler = routeHandler;
      return allowedMethods;
    }

    public static async Task<Collection<MultipartFileData>> ReadMultipartFileDataFromRequest(
      IVssRequestContext requestContext,
      HttpRequestMessage request,
      string tempPathForFeed)
    {
      return (await HttpHandlerHelper.ReadMultipartFormAndFileDataFromRequest(requestContext, request, tempPathForFeed)).FileData;
    }

    public static async Task<MultipartFormDataStreamProvider> ReadMultipartFormAndFileDataFromRequest(
      IVssRequestContext requestContext,
      HttpRequestMessage request,
      string tempPathForFeed)
    {
      int progressReportInterval = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Packaging/PushProgressReportIntervalBytes", true, 10000000);
      MultipartFormDataStreamProvider dataStreamProvider;
      using (Stream requestStream = await request.Content.ReadAsStreamAsync())
      {
        using (ReadProgressDelegatingStreamContent readProgressContent = new ReadProgressDelegatingStreamContent(request.Content, requestStream, true, (IStopwatch) new RealStopwatch(), (ITimeProvider) new DefaultTimeProvider(), (long) progressReportInterval))
        {
          MultipartFormDataStreamProvider formBasedMultipartStreamProvider = new MultipartFormDataStreamProvider(tempPathForFeed);
          try
          {
            dataStreamProvider = await readProgressContent.ReadAsMultipartAsync<MultipartFormDataStreamProvider>(formBasedMultipartStreamProvider, requestContext.CancellationToken);
          }
          catch (Exception ex) when (
          {
            // ISSUE: unable to correctly present filter
            int num;
            switch (ex)
            {
              case IOException _ when ex.Message.IndexOf("multipart", StringComparison.OrdinalIgnoreCase) >= 0:
                num = 1;
                break;
              case ArgumentNullException argumentNullException:
                num = argumentNullException.ParamName == "boundary" ? 1 : 0;
                break;
              default:
                num = 0;
                break;
            }
            if ((uint) num > 0U)
            {
              SuccessfulFiltering;
            }
            else
              throw;
          }
          )
          {
            throw new InvalidPackageException(Resources.Error_InvalidMultipartRequest());
          }
          finally
          {
            try
            {
              readProgressContent.RecordEntry();
              TransferRateResults results = readProgressContent.GetTransferRateResults();
              using (ITracerBlock tracerBlock = requestContext.GetTracerFacade().Enter((object) formBasedMultipartStreamProvider, nameof (ReadMultipartFormAndFileDataFromRequest)))
                tracerBlock.TraceConditionally((Func<string>) (() => results.Serialize<TransferRateResults>()));
              requestContext.AddPackagingTracesProperty("clientToPkgsUpload", (object) results.WithoutEntries());
            }
            catch (Exception ex)
            {
            }
          }
        }
      }
      return dataStreamProvider;
    }

    public static async Task<FileStream> ReadToTempFileAsync(
      IVssRequestContext requestContext,
      Stream requestStream,
      string tempFilePath,
      long? expectedByteCount)
    {
      IDisposable _;
      FileStream tempStream;
      ReadProgressStream readProgressStream;
      FileStream tempFileAsync;
      using (ITracerBlock tracer = requestContext.GetTracerFacade().Enter((object) null, nameof (ReadToTempFileAsync)))
      {
        _ = requestContext.CreateTimeToFirstPageExclusionBlock();
        try
        {
          int bytesInterval = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Packaging/PushProgressReportIntervalBytes", true, 10000000);
          tempStream = IngestionTemporaryFileHelper.OpenAutoDeletingTemporaryFile(tempFilePath);
          readProgressStream = new ReadProgressStream(requestStream, true, (IStopwatch) new RealStopwatch(), (ITimeProvider) new DefaultTimeProvider(), (long) bytesInterval);
          try
          {
            try
            {
              if (!expectedByteCount.HasValue)
              {
                await readProgressStream.CopyToAsync((Stream) tempStream);
              }
              else
              {
                byte[] buffer = new byte[4096];
                int bytesThisTime;
                for (long bytesRemaining = expectedByteCount.Value; bytesRemaining > 0L; bytesRemaining -= (long) bytesThisTime)
                {
                  bytesThisTime = await readProgressStream.ReadAsync(buffer, 0, (int) Math.Min(bytesRemaining, 4096L));
                  if (bytesThisTime == 0)
                    throw new InvalidPackageException(Resources.Error_PrematureEndOfUploadStream((object) expectedByteCount.Value, (object) (expectedByteCount.Value - bytesRemaining)));
                  await tempStream.WriteAsync(buffer, 0, bytesThisTime);
                }
                buffer = (byte[]) null;
              }
              tempStream.Position = 0L;
              tempFileAsync = tempStream;
            }
            catch
            {
              tempStream.Close();
              throw;
            }
            finally
            {
              try
              {
                readProgressStream.RecordProgressEntry();
                TransferRateResults results = readProgressStream.GetTransferRateResults();
                tracer.TraceConditionally((Func<string>) (() => results.Serialize<TransferRateResults>()));
                requestContext.AddPackagingTracesProperty("clientToPkgsUpload", (object) results.WithoutEntries());
              }
              catch (Exception ex)
              {
              }
            }
          }
          finally
          {
            readProgressStream?.Dispose();
          }
        }
        finally
        {
          _?.Dispose();
        }
      }
      _ = (IDisposable) null;
      tempStream = (FileStream) null;
      readProgressStream = (ReadProgressStream) null;
      return tempFileAsync;
    }
  }
}
