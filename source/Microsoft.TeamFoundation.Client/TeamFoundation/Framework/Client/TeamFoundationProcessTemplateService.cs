// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.TeamFoundationProcessTemplateService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.TeamFoundation.Client.ProjectSettings;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class TeamFoundationProcessTemplateService : IProcessTemplates
  {
    private const string c_MimeBoundary = "----------------------------8e5m2D6l5Q4h6";
    private TfsTeamProjectCollection m_tfs;
    private ProcessTemplateWebService m_processTemplateWebService;

    public TeamFoundationProcessTemplateService(TfsTeamProjectCollection tfs)
    {
      this.m_tfs = tfs;
      this.m_processTemplateWebService = new ProcessTemplateWebService(tfs);
    }

    [Obsolete("Please use the TemplateHeaders method instead")]
    public XmlNode GetTemplateNames() => throw new NotImplementedException();

    public TemplateHeader[] TemplateHeaders() => this.ConvertToTemplateHeaders(this.m_processTemplateWebService.TemplateHeaders());

    public TemplateHeader[] DeleteTemplate(int templateId) => this.ConvertToTemplateHeaders(this.m_processTemplateWebService.DeleteTemplate(templateId));

    public TemplateHeader[] MakeDefaultTemplate(int templateId) => this.ConvertToTemplateHeaders(this.m_processTemplateWebService.MakeDefaultTemplate(templateId));

    [Obsolete("Please use AddUpdateTemplate instead")]
    public int AddTemplate(string name, string description, string metadata, string state) => throw new NotImplementedException();

    public int GetTemplateIndex(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(nameof (name), name);
      TemplateHeader[] templateHeaderArray = this.TemplateHeaders();
      for (int index = 0; index < templateHeaderArray.Length; ++index)
      {
        if (TFStringComparer.TemplateName.Equals(templateHeaderArray[index].Name, name))
          return templateHeaderArray[index].TemplateId;
      }
      return -1;
    }

    public void AddUpdateTemplate(
      string name,
      string description,
      string metadata,
      string state,
      string zipFileName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      ArgumentUtility.CheckStringForNullOrEmpty(metadata, nameof (metadata));
      ArgumentUtility.CheckStringForNullOrEmpty(zipFileName, nameof (zipFileName));
      FileInfo fileInfo = System.IO.File.Exists(zipFileName) ? new FileInfo(zipFileName) : throw new FileNotFoundException((string) null, zipFileName);
      long length = fileInfo.Length;
      byte[] buffer1 = this.FormatFormHeader(name, description, metadata, length);
      byte[] buffer2 = this.FormFooter();
      string uriString = this.m_tfs.GetService<ILocationService>().LocationForCurrentConnection("MethodologyUpload", FrameworkServiceIdentifiers.MethodologyUpload);
      bool flag1 = length < 4194304L;
      bool flag2 = true;
      HttpWebResponse response1 = (HttpWebResponse) null;
      while (flag2)
      {
        flag2 = false;
        try
        {
          using (FileStream fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read))
          {
            Uri requestUri = new Uri(uriString);
            TfsRequestSettings settings = TfsRequestSettings.Default.Clone();
            settings.SendTimeout = TimeSpan.FromMinutes(60.0);
            HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(requestUri);
            TfsHttpRequestHelpers.PrepareWebRequest(webRequest, this.m_tfs.SessionId, TfsConnection.OperationName, this.m_tfs.UICulture, settings, this.m_tfs.ClientCredentials, this.m_tfs.IdentityToImpersonate);
            webRequest.Method = "POST";
            webRequest.ContentType = "multipart/form-data; boundary=" + "----------------------------8e5m2D6l5Q4h6".Substring(2);
            webRequest.ContentLength = (long) buffer1.Length + length + (long) buffer2.Length;
            webRequest.AllowWriteStreamBuffering = flag1;
            if (!flag1)
              webRequest.ConnectionGroupName = Guid.NewGuid().ToString();
            using (Stream requestStream = webRequest.GetRequestStream())
            {
              requestStream.Write(buffer1, 0, buffer1.Length);
              byte[] buffer3 = new byte[32768];
              int count;
              for (long index = 0; index < length; index += (long) count)
              {
                count = fileStream.Read(buffer3, 0, buffer3.Length);
                requestStream.Write(buffer3, 0, count);
              }
              requestStream.Write(buffer2, 0, buffer2.Length);
            }
            response1 = (HttpWebResponse) webRequest.GetResponse();
            if (response1 != null)
              this.ProcessHttpWebResponse(response1);
          }
        }
        catch (WebException ex)
        {
          HttpWebResponse response2 = ex.Response as HttpWebResponse;
          if (!flag1 && response2 != null && response2.StatusCode == HttpStatusCode.Unauthorized)
          {
            flag1 = true;
            flag2 = true;
          }
          else
            throw;
        }
        finally
        {
          response1?.Close();
        }
      }
    }

    [Obsolete("Please use AddUpdateTemplate instead")]
    public void UploadMethodology(string fileName, int templateId) => throw new NotImplementedException();

    public string GetTemplateData(int methodologyIndex)
    {
      string str = this.m_tfs.GetService<ILocationService>().LocationForCurrentConnection("Methodology", FrameworkServiceIdentifiers.MethodologyDownload);
      string path = (string) null;
      // ISSUE: variable of a boxed type
      __Boxed<int> local = (ValueType) methodologyIndex;
      Uri requestUri = new Uri(str + "?methodologyIndex=" + (object) local);
      TfsRequestSettings settings = TfsRequestSettings.Default.Clone();
      settings.SendTimeout = TimeSpan.FromMinutes(60.0);
      HttpWebRequest httpWebRequest = TfsHttpRequestHelpers.PrepareWebRequest((HttpWebRequest) WebRequest.Create(requestUri), this.m_tfs.SessionId, TfsConnection.OperationName, this.m_tfs.UICulture, settings, this.m_tfs.ClientCredentials, this.m_tfs.IdentityToImpersonate);
      httpWebRequest.Method = "GET";
      try
      {
        path = FileSpec.GetTempFileName();
        using (HttpWebResponse response = (HttpWebResponse) httpWebRequest.GetResponse())
        {
          if (response != null)
          {
            this.ProcessHttpWebResponse(response);
            if (response.StatusCode == HttpStatusCode.OK)
            {
              Stream responseStream = response.GetResponseStream();
              using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
              {
                if (fileStream != null)
                {
                  byte[] buffer = new byte[65536];
                  int num = 0;
                  int count;
                  while ((count = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                  {
                    fileStream.Write(buffer, 0, count);
                    num += count;
                  }
                  return path;
                }
              }
            }
          }
          return (string) null;
        }
      }
      catch (WebException ex)
      {
        TeamFoundationTrace.TraceException((Exception) ex);
        if (path != null)
          System.IO.File.Delete(path);
        if (ex.Response != null && ((HttpWebResponse) ex.Response).StatusCode == HttpStatusCode.InternalServerError)
        {
          string end;
          using (Stream responseStream = ex.Response.GetResponseStream())
          {
            using (StreamReader streamReader = new StreamReader(responseStream))
              end = streamReader.ReadToEnd();
          }
          throw new InvalidOperationException(end, (Exception) ex);
        }
        throw;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
        if (path != null)
          System.IO.File.Delete(path);
        throw;
      }
    }

    private byte[] FormatFormHeader(
      string name,
      string description,
      string metadata,
      long fileLength)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary["templateName"] = name;
      dictionary[nameof (description)] = description;
      dictionary[nameof (metadata)] = metadata;
      UTF8Encoding utF8Encoding = new UTF8Encoding(false);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("----------------------------8e5m2D6l5Q4h6");
      stringBuilder.AppendLine("Content-Disposition: form-data; name=\"filelength\"");
      stringBuilder.AppendLine();
      stringBuilder.AppendFormat("{0}", (object) fileLength);
      stringBuilder.AppendLine();
      bool supportsFramework = this.m_tfs.GetService<TpcSettingsStore>().SupportsFramework;
      foreach (string key in dictionary.Keys)
      {
        string base64String = dictionary[key];
        if (supportsFramework)
          base64String = Convert.ToBase64String(utF8Encoding.GetBytes(base64String));
        stringBuilder.AppendLine("----------------------------8e5m2D6l5Q4h6");
        stringBuilder.AppendLine("Content-Disposition: form-data; name=\"" + key + "\"");
        stringBuilder.AppendLine();
        stringBuilder.AppendLine(base64String);
        stringBuilder.AppendLine();
      }
      stringBuilder.AppendLine("----------------------------8e5m2D6l5Q4h6");
      stringBuilder.AppendLine("Content-Disposition: form-data; name=\"content\"; filename=\"templateData\"");
      stringBuilder.AppendLine("Content-Type: application/octet-stream");
      stringBuilder.AppendLine();
      return utF8Encoding.GetBytes(stringBuilder.ToString());
    }

    private byte[] FormFooter()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine();
      stringBuilder.AppendLine("----------------------------8e5m2D6l5Q4h6");
      stringBuilder.AppendLine("--");
      return new UTF8Encoding(false).GetBytes(stringBuilder.ToString());
    }

    private void ProcessHttpWebResponse(HttpWebResponse response)
    {
      if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Found || response.StatusCode == HttpStatusCode.Found)
        throw new TeamFoundationServerUnauthorizedException();
    }

    private TemplateHeader[] ConvertToTemplateHeaders(
      FrameworkTemplateHeader[] frameworkTemplateHeaders)
    {
      if (frameworkTemplateHeaders == null)
        return (TemplateHeader[]) null;
      TemplateHeader[] templateHeaders = new TemplateHeader[frameworkTemplateHeaders.Length];
      int num = 0;
      foreach (FrameworkTemplateHeader frameworkTemplateHeader in frameworkTemplateHeaders)
        templateHeaders[num++] = new TemplateHeader(frameworkTemplateHeader);
      return templateHeaders;
    }
  }
}
