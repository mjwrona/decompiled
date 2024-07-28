// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ResourceUploader
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class ResourceUploader : TfsHttpClient
  {
    private Stream m_resourceStream;
    private string m_resourceName;
    private long m_length;
    private byte[] m_localHash;
    private byte[] m_buffer = new byte[65536];
    private Stopwatch m_progressTimer = new Stopwatch();
    private byte[][] m_parameters = new byte[8][];
    private static int s_uploadChunkSize;
    private static byte[] s_wrapper;
    private static int[] s_wrapperOffsets = new int[9];
    private static byte[] s_contentType;
    private const int c_defaultUploadChunkSize = 16777216;
    private const int NumberOfParams = 8;
    private const string MimeBoundary = "----------------------------8e5m2D6l5Q4h6";

    static ResourceUploader()
    {
      ResourceUploader.s_uploadChunkSize = TFCommonUtil.GetAppSettingAsInt(FrameworkClientConstants.ClientUploadChunkSize, 16777216);
      ResourceUploader.GenerateMimeWrapper();
    }

    public ResourceUploader(TfsConfigurationServer tfs)
      : base((TfsConnection) tfs)
    {
    }

    protected override string ComponentName => "Framework";

    protected override string ServiceType => "ServicingResourceUpload";

    protected override Guid ConfigurationServiceIdentifier => FrameworkServiceIdentifiers.ServicingResourceUpload;

    internal void UploadResource(string resourceName, Stream resourceStream)
    {
      this.m_progressTimer.Start();
      this.m_resourceName = resourceName;
      this.m_resourceStream = resourceStream;
      this.m_length = resourceStream.Length;
      this.m_parameters[0] = TfsRequestSettings.RequestEncoding.GetBytes(this.m_resourceName);
      this.m_parameters[1] = TfsRequestSettings.RequestEncoding.GetBytes(string.Empty);
      this.m_parameters[2] = TfsRequestSettings.RequestEncoding.GetBytes(string.Empty);
      this.m_parameters[3] = TfsRequestSettings.RequestEncoding.GetBytes(this.m_length.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.m_parameters[6] = TfsRequestSettings.RequestEncoding.GetBytes("resourcename");
      this.m_parameters[7] = ResourceUploader.s_contentType;
      resourceStream.Position = 0L;
      this.UploadResourceInternal();
    }

    private void UploadResourceInternal()
    {
      string methodName = "Framework-Servicing-Upload";
      this.CalculateHash();
      long requestId = TfsConnection.OnBeginWebRequest();
      TfsConnection.OnWebServiceCallBegin(this.Connection, requestId, methodName);
      this.UploadContent();
      TfsConnection.OnWebServiceCallEnd(this.Connection, requestId, methodName);
    }

    private void CalculateHash()
    {
      long elapsedMilliseconds = this.m_progressTimer.ElapsedMilliseconds;
      this.m_resourceStream.Position = 0L;
      this.m_localHash = TFUtil.CalculateMD5(this.m_resourceStream);
      int num = this.Settings.Tracing.TraceInfo ? 1 : 0;
    }

    private void UploadContent()
    {
      long end = (long) ResourceUploader.s_uploadChunkSize;
      if (end <= 2L)
        end = this.m_length;
      this.m_parameters[4] = TfsRequestSettings.RequestEncoding.GetBytes(Convert.ToBase64String(this.m_localHash, 0, this.m_localHash.Length, Base64FormattingOptions.None));
      this.m_resourceStream.Position = 0L;
      long num;
      do
      {
        if (end > this.m_length)
          end = this.m_length;
        this.UploadChunk(this.m_resourceStream, end);
        num = end;
        end += (long) ResourceUploader.s_uploadChunkSize;
      }
      while (num < this.m_length);
    }

    private void UploadChunk(Stream fileContentStream, long end)
    {
      HttpWebResponse response = (HttpWebResponse) null;
      Exception innerException = (Exception) null;
      try
      {
        try
        {
          HttpWebRequest soapRequest = TfsHttpRequestHelpers.CreateSoapRequest(this.GetServiceLocation(), this.Connection);
          soapRequest.ContentType = "multipart/form-data; boundary=" + "----------------------------8e5m2D6l5Q4h6".Substring(2);
          this.m_parameters[5] = this.GetRangeFieldMimeParameter(fileContentStream.Position, end - 1L);
          soapRequest.ContentLength = (long) ((int) (end - fileContentStream.Position) + ResourceUploader.s_wrapper.Length);
          for (int index = 0; index < this.m_parameters.Length; ++index)
            soapRequest.ContentLength += (long) this.m_parameters[index].Length;
          using (Stream requestStream = soapRequest.GetRequestStream())
            this.HandleRequest(this.m_resourceStream, requestStream, end);
          response = (HttpWebResponse) soapRequest.GetResponse();
        }
        catch (WebException ex)
        {
          innerException = (Exception) ex;
          response = ex.Response as HttpWebResponse;
        }
        catch (Exception ex)
        {
          innerException = ex;
        }
        if (response != null)
        {
          if (this.Settings.Tracing.TraceVerbose)
            TfsHttpRequestHelpers.TraceHeaders(response.Headers);
          if (response.StatusCode == HttpStatusCode.OK)
          {
            if (response.ContentLength == 0L)
              goto label_22;
          }
          innerException = this.HandleErrorResponse(response);
        }
      }
      finally
      {
        response?.Close();
      }
label_22:
      if (innerException != null)
        throw new TeamFoundationServerException(TFCommonResources.AColonB((object) this.m_resourceName, (object) innerException.Message), innerException);
    }

    private void LogWebResponse(WebResponse response)
    {
      if (response == null || !this.Settings.Tracing.TraceError)
        return;
      Stream responseStream = response.GetResponseStream();
      if (responseStream == null)
        return;
      using (responseStream)
      {
        using (new StreamReader(responseStream, TfsRequestSettings.RequestEncoding))
          ;
      }
    }

    protected Exception HandleErrorResponse(HttpWebResponse response)
    {
      try
      {
        string message = response.StatusCode != HttpStatusCode.Unauthorized ? TFCommonResources.InvalidServerResponse((object) TeamFoundationServerInvalidResponseException.FormatHttpStatus(response)) : throw new TeamFoundationServerUnauthorizedException(TFCommonResources.Unauthorized((object) response.Server));
        string str = (string) null;
        for (int index = 0; index < response.Headers.Count; ++index)
        {
          if (response.Headers.Keys[index] == "X-TeamFoundation-Exception")
          {
            str = response.Headers.GetValues(index)[0];
            using (StreamReader streamReader = new StreamReader(response.GetResponseStream(), TfsRequestSettings.RequestEncoding))
            {
              message = streamReader.ReadToEnd();
              int num = this.Settings.Tracing.TraceError ? 1 : 0;
              break;
            }
          }
        }
        if (str == null)
        {
          this.LogWebResponse((WebResponse) response);
          throw new TeamFoundationServerException(message);
        }
        throw new TeamFoundationServerException(message);
      }
      catch (Exception ex)
      {
        return ex;
      }
    }

    private void HandleRequest(Stream fileContentStream, Stream requestStream, long end)
    {
      if (!requestStream.CanWrite)
      {
        int num = this.Settings.Tracing.TraceInfo ? 1 : 0;
        throw new TeamFoundationServiceUnavailableException(TFCommonResources.ServicesUnavailable((object) this.Connection.Name, (object) TFCommonResources.UnableToEstablishConnection()));
      }
      int destinationIndex = 0;
      int sourceIndex = 0;
      int index = 0;
      int num1;
      while (true)
      {
        int length = ResourceUploader.s_wrapperOffsets[index] - sourceIndex;
        Array.Copy((Array) ResourceUploader.s_wrapper, sourceIndex, (Array) this.m_buffer, destinationIndex, length);
        num1 = destinationIndex + length;
        sourceIndex = ResourceUploader.s_wrapperOffsets[index];
        if (index != 8)
        {
          Array.Copy((Array) this.m_parameters[index], 0, (Array) this.m_buffer, num1, this.m_parameters[index].Length);
          destinationIndex = num1 + this.m_parameters[index].Length;
          ++index;
        }
        else
          break;
      }
      long position = fileContentStream.Position;
      int num2 = ResourceUploader.s_wrapper.Length - ResourceUploader.s_wrapperOffsets[8];
      bool flag = false;
      do
      {
        if (num1 < this.m_buffer.Length)
        {
          int count = this.m_buffer.Length - num1;
          if (position + (long) count > end)
            count = (int) (end - position);
          int num3 = fileContentStream.Read(this.m_buffer, num1, count);
          num1 += num3;
          position += (long) num3;
        }
        if (num1 + num2 < this.m_buffer.Length)
        {
          Array.Copy((Array) ResourceUploader.s_wrapper, ResourceUploader.s_wrapperOffsets[8], (Array) this.m_buffer, num1, num2);
          num1 += num2;
          flag = true;
        }
        requestStream.Write(this.m_buffer, 0, num1);
        num1 = 0;
      }
      while (position < end);
      if (flag)
        return;
      requestStream.Write(ResourceUploader.s_wrapper, ResourceUploader.s_wrapperOffsets[8], num2);
    }

    private byte[] GetRangeFieldMimeParameter(long beginning, long end)
    {
      StringBuilder stringBuilder = new StringBuilder(100);
      stringBuilder.Append("bytes=");
      stringBuilder.Append(beginning);
      stringBuilder.Append('-');
      stringBuilder.Append(end);
      stringBuilder.Append('/');
      stringBuilder.Append(this.m_length);
      stringBuilder.AppendLine();
      return TfsRequestSettings.RequestEncoding.GetBytes(stringBuilder.ToString());
    }

    private static void GenerateMimeWrapper()
    {
      using (MemoryStream memoryStream = new MemoryStream(1024))
      {
        using (StreamWriter streamWriter = new StreamWriter((Stream) memoryStream, TfsRequestSettings.RequestEncoding))
        {
          streamWriter.WriteLine("----------------------------8e5m2D6l5Q4h6");
          streamWriter.Write("Content-Disposition: form-data; name=\"");
          streamWriter.Write("resourcename");
          streamWriter.WriteLine("\"");
          streamWriter.WriteLine();
          streamWriter.Flush();
          ResourceUploader.s_wrapperOffsets[0] = (int) memoryStream.Position;
          ResourceUploader.s_wrapperOffsets[1] = (int) memoryStream.Position;
          ResourceUploader.s_wrapperOffsets[2] = (int) memoryStream.Position;
          streamWriter.WriteLine();
          streamWriter.WriteLine("----------------------------8e5m2D6l5Q4h6");
          streamWriter.Write("Content-Disposition: form-data; name=\"");
          streamWriter.Write("filelength");
          streamWriter.WriteLine("\"");
          streamWriter.WriteLine();
          streamWriter.Flush();
          ResourceUploader.s_wrapperOffsets[3] = (int) memoryStream.Position;
          streamWriter.WriteLine();
          streamWriter.WriteLine("----------------------------8e5m2D6l5Q4h6");
          streamWriter.Write("Content-Disposition: form-data; name=\"");
          streamWriter.Write("hash");
          streamWriter.WriteLine("\"");
          streamWriter.WriteLine();
          streamWriter.Flush();
          ResourceUploader.s_wrapperOffsets[4] = (int) memoryStream.Position;
          streamWriter.WriteLine();
          streamWriter.WriteLine("----------------------------8e5m2D6l5Q4h6");
          streamWriter.Write("Content-Disposition: form-data; name=\"");
          streamWriter.Write("range");
          streamWriter.WriteLine("\"");
          streamWriter.WriteLine();
          streamWriter.Flush();
          ResourceUploader.s_wrapperOffsets[5] = (int) memoryStream.Position;
          streamWriter.WriteLine();
          streamWriter.WriteLine("----------------------------8e5m2D6l5Q4h6");
          streamWriter.Write("Content-Disposition: form-data; name=\"");
          streamWriter.Write("content");
          streamWriter.Write("\"; filename=\"");
          streamWriter.Flush();
          ResourceUploader.s_wrapperOffsets[6] = (int) memoryStream.Position;
          streamWriter.WriteLine("\"");
          streamWriter.Write("Content-Type: ");
          streamWriter.Flush();
          ResourceUploader.s_wrapperOffsets[7] = (int) memoryStream.Position;
          streamWriter.WriteLine();
          streamWriter.WriteLine();
          streamWriter.Flush();
          ResourceUploader.s_wrapperOffsets[8] = (int) memoryStream.Position;
          streamWriter.WriteLine();
          streamWriter.Write("----------------------------8e5m2D6l5Q4h6");
          streamWriter.WriteLine("--");
        }
        ResourceUploader.s_wrapper = memoryStream.ToArray();
        ResourceUploader.s_contentType = TfsRequestSettings.RequestEncoding.GetBytes("application/octet-stream");
      }
    }
  }
}
