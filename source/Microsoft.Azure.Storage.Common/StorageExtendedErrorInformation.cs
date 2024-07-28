// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.StorageExtendedErrorInformation
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.Azure.Storage
{
  [Serializable]
  public sealed class StorageExtendedErrorInformation
  {
    public string ErrorCode { get; internal set; }

    public string ErrorMessage { get; internal set; }

    public IDictionary<string, string> AdditionalDetails { get; internal set; }

    public static async Task<StorageExtendedErrorInformation> ReadFromStreamAsync(Stream inputStream)
    {
      CommonUtility.AssertNotNull(nameof (inputStream), (object) inputStream);
      if (inputStream.CanSeek && inputStream.Length < 1L)
        return (StorageExtendedErrorInformation) null;
      StorageExtendedErrorInformation extendedErrorInfo = new StorageExtendedErrorInformation();
      try
      {
        using (XmlReader reader = XMLReaderExtensions.CreateAsAsync(inputStream))
        {
          int num = await reader.ReadAsync().ConfigureAwait(false) ? 1 : 0;
          await extendedErrorInfo.ReadXmlAsync(reader, CancellationToken.None).ConfigureAwait(false);
        }
        return extendedErrorInfo;
      }
      catch (XmlException ex)
      {
        return (StorageExtendedErrorInformation) null;
      }
    }

    public static async Task<StorageExtendedErrorInformation> ReadFromStreamAsync(
      Stream inputStream,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (inputStream), (object) inputStream);
      if (inputStream.CanSeek && inputStream.Length < 1L)
        return (StorageExtendedErrorInformation) null;
      StorageExtendedErrorInformation extendedErrorInfo = new StorageExtendedErrorInformation();
      try
      {
        Stream input = inputStream;
        using (XmlReader reader = XmlReader.Create(input, new XmlReaderSettings()
        {
          Async = true
        }))
        {
          int num = await reader.ReadAsync().ConfigureAwait(false) ? 1 : 0;
          cancellationToken.ThrowIfCancellationRequested();
          await extendedErrorInfo.ReadXmlAsync(reader, cancellationToken).ConfigureAwait(false);
          cancellationToken.ThrowIfCancellationRequested();
        }
        return extendedErrorInfo;
      }
      catch (XmlException ex)
      {
        return (StorageExtendedErrorInformation) null;
      }
    }

    public async Task ReadXmlAsync(XmlReader reader, CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (reader), (object) reader);
      this.AdditionalDetails = (IDictionary<string, string>) new Dictionary<string, string>();
      await reader.ReadStartElementAsync().ConfigureAwait(false);
      cancellationToken.ThrowIfCancellationRequested();
      IDictionary<string, string> dictionary;
      while (true)
      {
        if (await reader.IsStartElementAsync().ConfigureAwait(false))
        {
          if (reader.IsEmptyElement)
          {
            await reader.SkipAsync().ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
          }
          else if (string.Compare(reader.LocalName, "Code", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(reader.LocalName, "code", StringComparison.Ordinal) == 0)
          {
            this.ErrorCode = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
          }
          else if (string.Compare(reader.LocalName, "Message", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(reader.LocalName, "message", StringComparison.Ordinal) == 0)
          {
            this.ErrorMessage = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
          }
          else if (string.Compare(reader.LocalName, "exceptiondetails", StringComparison.OrdinalIgnoreCase) == 0)
          {
            await reader.ReadStartElementAsync((string) null, (string) null).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            while (true)
            {
              if (await reader.IsStartElementAsync().ConfigureAwait(false))
              {
                switch (reader.LocalName)
                {
                  case "ExceptionMessage":
                    dictionary = this.AdditionalDetails;
                    dictionary.Add("ExceptionMessage", await reader.ReadElementContentAsStringAsync("ExceptionMessage", string.Empty).ConfigureAwait(false));
                    dictionary = (IDictionary<string, string>) null;
                    cancellationToken.ThrowIfCancellationRequested();
                    continue;
                  case "StackTrace":
                    dictionary = this.AdditionalDetails;
                    dictionary.Add("StackTrace", await reader.ReadElementContentAsStringAsync("StackTrace", string.Empty).ConfigureAwait(false));
                    dictionary = (IDictionary<string, string>) null;
                    cancellationToken.ThrowIfCancellationRequested();
                    continue;
                  default:
                    await reader.SkipAsync().ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();
                    continue;
                }
              }
              else
                break;
            }
            await reader.ReadEndElementAsync().ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
          }
          else
          {
            dictionary = this.AdditionalDetails;
            string key = reader.LocalName;
            dictionary.Add(key, await reader.ReadInnerXmlAsync().ConfigureAwait(false));
            dictionary = (IDictionary<string, string>) null;
            key = (string) null;
            cancellationToken.ThrowIfCancellationRequested();
          }
        }
        else
          break;
      }
      await reader.ReadEndElementAsync().ConfigureAwait(false);
      cancellationToken.ThrowIfCancellationRequested();
    }

    public void WriteXml(XmlWriter writer)
    {
      CommonUtility.AssertNotNull(nameof (writer), (object) writer);
      writer.WriteStartElement("Error");
      writer.WriteElementString("Code", this.ErrorCode);
      writer.WriteElementString("Message", this.ErrorMessage);
      foreach (string key in (IEnumerable<string>) this.AdditionalDetails.Keys)
        writer.WriteElementString(key, this.AdditionalDetails[key]);
      writer.WriteEndElement();
    }
  }
}
