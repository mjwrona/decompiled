// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.BlobContinuationTokenStorageService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.BlobWrappers;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class BlobContinuationTokenStorageService : 
    IBlobContinuationTokenStorageService,
    IVssFrameworkService
  {
    private Guid m_hostId;
    private const long c_minStorageFrequency = 5000;
    private const long c_defaultStorageFrequency = 100000;

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_hostId = systemRequestContext.ServiceHost.InstanceId;

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public long GetStorageFrequency(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.ValidateServiceHost(requestContext);
      return Math.Max(requestContext.GetService<IVssRegistryService>().GetValue<long>(requestContext, (RegistryQuery) FrameworkServerConstants.BlobCopyContinuationTokenStorageFrequency, 100000L), 5000L);
    }

    public void ClearContinuationToken(
      IVssRequestContext requestContext,
      string operationName,
      ICloudBlobContainerWrapper containerWrapper,
      Action<string> log = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(operationName, nameof (operationName));
      ArgumentUtility.CheckForNull<ICloudBlobContainerWrapper>(containerWrapper, nameof (containerWrapper));
      this.ValidateServiceHost(requestContext);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string tokenRegistryKey = this.GetContinuationTokenRegistryKey(operationName, containerWrapper);
      IVssRequestContext requestContext1 = requestContext;
      string path = tokenRegistryKey;
      service.SetValue(requestContext1, path, (object) null);
    }

    public void SetContinuationToken(
      IVssRequestContext requestContext,
      string operationName,
      ICloudBlobContainerWrapper containerWrapper,
      BlobContinuationToken continuationToken,
      Action<string> log = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(operationName, nameof (operationName));
      ArgumentUtility.CheckForNull<ICloudBlobContainerWrapper>(containerWrapper, nameof (containerWrapper));
      ArgumentUtility.CheckForNull<BlobContinuationToken>(continuationToken, nameof (continuationToken));
      this.ValidateServiceHost(requestContext);
      try
      {
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        string tokenRegistryKey = this.GetContinuationTokenRegistryKey(operationName, containerWrapper);
        using (StringWriter output = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
        {
          using (XmlWriter writer = XmlWriter.Create((TextWriter) output))
            continuationToken.WriteXml(writer);
          service.SetValue<string>(requestContext, tokenRegistryKey, output.ToString());
        }
      }
      catch (Exception ex)
      {
        if (log == null)
          return;
        log(string.Format("Warning: Failed to save continuation token.  Operation: {0}, Storage account: {1}, Container: {2}, Exception: {3}", (object) operationName, (object) containerWrapper.StorageAccountName, (object) containerWrapper.Name, (object) ex));
      }
    }

    public BlobContinuationToken GetContinuationToken(
      IVssRequestContext requestContext,
      string operationName,
      ICloudBlobContainerWrapper containerWrapper,
      Action<string> log = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(operationName, nameof (operationName));
      ArgumentUtility.CheckForNull<ICloudBlobContainerWrapper>(containerWrapper, nameof (containerWrapper));
      this.ValidateServiceHost(requestContext);
      try
      {
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        string tokenRegistryKey = this.GetContinuationTokenRegistryKey(operationName, containerWrapper);
        IVssRequestContext requestContext1 = requestContext;
        // ISSUE: explicit reference operation
        ref RegistryQuery local = @(RegistryQuery) tokenRegistryKey;
        string s = service.GetValue(requestContext1, in local, (string) null);
        if (s == null)
          return (BlobContinuationToken) null;
        BlobContinuationToken continuationToken = new BlobContinuationToken();
        using (StringReader stringReader = new StringReader(s))
        {
          StringReader input = stringReader;
          using (XmlReader reader = XmlReader.Create((TextReader) input, new XmlReaderSettings()
          {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = (XmlResolver) null,
            Async = true
          }))
            continuationToken.ReadXmlAsync(reader).GetAwaiter().GetResult();
        }
        return continuationToken;
      }
      catch (Exception ex)
      {
        if (log != null)
          log(string.Format("Warning: Failed to get continuation token.  Operation: {0}, Storage account: {1}, Container: {2}, Exception: {3}", (object) operationName, (object) containerWrapper.StorageAccountName, (object) containerWrapper.Name, (object) ex));
        return (BlobContinuationToken) null;
      }
    }

    public int ClearAllContinuationTokens(
      IVssRequestContext requestContext,
      string operationName,
      Action<string> log = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(operationName, nameof (operationName));
      this.ValidateServiceHost(requestContext);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/**", (object) FrameworkServerConstants.BlobCopyContinuationTokenRoot, (object) operationName);
      IVssRequestContext requestContext1 = requestContext;
      string registryPathPattern = str;
      return service.DeleteEntries(requestContext1, registryPathPattern);
    }

    protected string GetContinuationTokenRegistryKey(
      string operationName,
      ICloudBlobContainerWrapper containerWrapper)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}", (object) FrameworkServerConstants.BlobCopyContinuationTokenRoot, (object) operationName, (object) containerWrapper.StorageAccountName, (object) containerWrapper.Name);
    }

    protected void ValidateServiceHost(IVssRequestContext requestContext)
    {
      if (this.m_hostId != requestContext.ServiceHost.InstanceId)
        throw new InvalidOperationException(string.Format("{0} expected a request context on host '{1}' but received one on host '{2}'", (object) nameof (BlobContinuationTokenStorageService), (object) this.m_hostId, (object) requestContext.ServiceHost.InstanceId));
    }
  }
}
