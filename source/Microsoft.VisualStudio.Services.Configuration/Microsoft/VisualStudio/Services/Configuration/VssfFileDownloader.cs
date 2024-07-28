// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.VssfFileDownloader
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class VssfFileDownloader
  {
    private const string c_userAgent = "TfsDownloader";
    private readonly IVssfHttpClientFactory m_httpClientFactory;

    public VssfFileDownloader(IVssfHttpClientFactory httpClientFactory = null) => this.m_httpClientFactory = httpClientFactory ?? (IVssfHttpClientFactory) new VssfHttpClientFactory();

    public bool ValidateDownloadSiteIsReachable(Uri downloadUrl, ITFLogger logger)
    {
      ArgumentUtility.CheckForNull<Uri>(downloadUrl, nameof (downloadUrl));
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      using (HttpClient httpClient = this.m_httpClientFactory.CreateHttpClient(3, 30))
      {
        logger.Info("Sending HEAD {0} request.", (object) downloadUrl);
        using (HttpRequestMessage requestMessage = VssfFileDownloader.CreateRequestMessage(HttpMethod.Head, downloadUrl))
        {
          string message;
          try
          {
            HttpResponseMessage result = httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead).Result;
            logger.Info("Status code: {0}, Uri: {1}", (object) result.StatusCode, (object) result.RequestMessage.RequestUri.AbsoluteUri);
            result.EnsureSuccessStatusCode();
            return true;
          }
          catch (AggregateException ex)
          {
            logger.Error((Exception) ex);
            AggregateException aggregateException = ex.Flatten();
            if (aggregateException.InnerExceptions.Count > 0)
            {
              Exception innerException = aggregateException.InnerExceptions[0];
              if (innerException is HttpRequestException && innerException.InnerException != null)
                innerException = innerException.InnerException;
              message = innerException.Message;
            }
            else
              message = ex.Message;
          }
          catch (Exception ex)
          {
            message = ex.Message;
          }
          if (message != null)
            logger.Error("Validation failed. The following error was reported: {0}", (object) message);
        }
        return false;
      }
    }

    public void Download(
      Uri downloadUrl,
      string fileName,
      DownloadProgressChangedEventHandler onDownloadProgressChanged,
      bool validateMicrosoftSignature,
      ITFLogger logger)
    {
      ArgumentUtility.CheckForNull<Uri>(downloadUrl, nameof (downloadUrl));
      ArgumentUtility.CheckStringForNullOrEmpty(fileName, nameof (fileName));
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      string path = !System.IO.File.Exists(fileName) ? Path.GetDirectoryName(fileName) : throw new IOException(ConfigurationResources.FileAlreadyExists((object) fileName));
      if (!Directory.Exists(path))
      {
        logger.Info("Creating directory: {0}", (object) path);
        Directory.CreateDirectory(path);
      }
      string str = fileName + ".tfdownload";
      bool flag1 = false;
      bool flag2 = false;
      try
      {
        for (int index = 1; index <= 5; ++index)
        {
          if (!flag1)
          {
            flag1 = this.DownloadInternal(downloadUrl, str, onDownloadProgressChanged, logger);
            if (flag1)
            {
              if (validateMicrosoftSignature)
                logger.Info("Verifying file signature. File: {0}.", (object) str);
              uint winVerifyExitCode;
              if (validateMicrosoftSignature && !WinTrust.VerifyMicrosoftSignature(str, out winVerifyExitCode))
              {
                logger.Warning("Signature verification failed. Deleting {0}. Exit code: {1}.", (object) str, (object) winVerifyExitCode);
                System.IO.File.Delete(str);
                flag1 = false;
                flag2 = !flag2 ? true : throw new ApplicationException(ConfigurationResources.FileDownloadedSignatureVerificationFailed());
              }
              else
                System.IO.File.Move(str, fileName);
            }
          }
          else
            break;
        }
      }
      catch (Exception ex)
      {
        throw new VssfFileDownloadException(ConfigurationResources.FailedToDownloadFile((object) downloadUrl.AbsoluteUri), ex)
        {
          FilePath = fileName,
          DownloadUrl = downloadUrl.AbsoluteUri
        };
      }
      if (!flag1)
        throw new VssfFileDownloadException(ConfigurationResources.FailedToDownloadFile((object) downloadUrl))
        {
          FilePath = fileName,
          DownloadUrl = downloadUrl.AbsoluteUri
        };
    }

    private bool DownloadInternal(
      Uri downloadUrl,
      string fileName,
      DownloadProgressChangedEventHandler onDownloadProgressChanged,
      ITFLogger logger)
    {
      using (FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
      {
        if (fileStream.Seek(0L, SeekOrigin.End) > 0L)
          logger.Info("{0} already exists and have {1} bytes. Resuming download...", (object) fileName, (object) fileStream.Position);
        using (HttpClient httpClient = this.m_httpClientFactory.CreateHttpClient(5, 30))
        {
          HttpRequestMessage requestMessage1 = VssfFileDownloader.CreateRequestMessage(HttpMethod.Head, downloadUrl);
          logger.Info("Sending HEAD request. Url: {0}", (object) downloadUrl);
          HttpResponseMessage result1 = httpClient.SendAsync(requestMessage1, HttpCompletionOption.ResponseHeadersRead).Result;
          logger.Info("Status Code: {0}", (object) result1.StatusCode);
          result1.EnsureSuccessStatusCode();
          long totalBytesToReceive = result1.Content.Headers.ContentLength.Value;
          logger.Info("Content length: {0}", (object) totalBytesToReceive);
          if (fileStream.Position == totalBytesToReceive)
          {
            logger.Info("File already downloaded.");
            return true;
          }
          if (fileStream.Position > totalBytesToReceive)
          {
            logger.Info("{0} is bigger than the file that we need to download. FileSize: {1}, Content length: {2}. Resetting...", (object) fileStream.Name, (object) fileStream.Position, (object) totalBytesToReceive);
            fileStream.SetLength(0L);
          }
          HttpRequestMessage requestMessage2 = VssfFileDownloader.CreateRequestMessage(HttpMethod.Get, downloadUrl);
          requestMessage2.Headers.Range = new RangeHeaderValue(new long?(fileStream.Position), new long?());
          logger.Info("Sending GET request. Url: {0}", (object) downloadUrl);
          HttpResponseMessage result2 = httpClient.SendAsync(requestMessage2, HttpCompletionOption.ResponseHeadersRead).Result;
          logger.Info("Status code: {0}", (object) result2.StatusCode);
          result2.EnsureSuccessStatusCode();
          using (Stream result3 = result2.Content.ReadAsStreamAsync().Result)
          {
            byte[] buffer = new byte[524288];
            int count;
            long position;
            do
            {
              try
              {
                count = result3.Read(buffer, 0, 524288);
              }
              catch (Exception ex)
              {
                logger.Info("An error occurred while downloading file from {0}", (object) downloadUrl);
                logger.Warning(ex);
                return false;
              }
              fileStream.Write(buffer, 0, count);
              fileStream.Flush();
              position = fileStream.Position;
              if (onDownloadProgressChanged != null)
              {
                DownloadProgressChangedEventArgs changedEventArgs = VssfFileDownloader.CreateDownloadProgressChangedEventArgs((int) (position * 100L / totalBytesToReceive), (object) null, position, totalBytesToReceive);
                onDownloadProgressChanged((object) this, changedEventArgs);
              }
            }
            while (count != 0);
            if (position != totalBytesToReceive)
              logger.Info("bytesReceived does not match contentLength. bytesReceived: {0}, contentLength: {1}", (object) position, (object) totalBytesToReceive);
            return position == totalBytesToReceive;
          }
        }
      }
    }

    private static HttpRequestMessage CreateRequestMessage(HttpMethod method, Uri downloadUrl)
    {
      HttpRequestMessage requestMessage = new HttpRequestMessage(method, downloadUrl);
      requestMessage.Headers.Add("User-Agent", "TfsDownloader");
      return requestMessage;
    }

    private static DownloadProgressChangedEventArgs CreateDownloadProgressChangedEventArgs(
      int progressPercentage,
      object userToken,
      long bytesReceived,
      long totalBytesToReceive)
    {
      return (DownloadProgressChangedEventArgs) typeof (DownloadProgressChangedEventArgs).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, (Binder) null, new Type[4]
      {
        typeof (int),
        typeof (object),
        typeof (long),
        typeof (long)
      }, (ParameterModifier[]) null).Invoke(new object[4]
      {
        (object) progressPercentage,
        userToken,
        (object) bytesReceived,
        (object) totalBytesToReceive
      });
    }

    protected IVssfHttpClientFactory HttpClientFactory => this.m_httpClientFactory;
  }
}
