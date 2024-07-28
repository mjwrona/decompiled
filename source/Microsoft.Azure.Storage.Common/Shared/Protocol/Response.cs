// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Shared.Protocol.Response
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.Azure.Storage.Shared.Protocol
{
  internal static class Response
  {
    internal static async Task ReadSharedAccessIdentifiersAsync<T>(
      IDictionary<string, T> sharedAccessPolicies,
      AccessPolicyResponseBase<T> policyResponse,
      CancellationToken token)
      where T : new()
    {
      token.ThrowIfCancellationRequested();
      foreach (KeyValuePair<string, T> keyValuePair in await policyResponse.AccessIdentifiers.ConfigureAwait(false))
        sharedAccessPolicies.Add(keyValuePair.Key, keyValuePair.Value);
    }

    internal static IDictionary<string, string> ParseMetadata(XmlReader reader)
    {
      IDictionary<string, string> metadata = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      bool flag = true;
      while (!flag || reader.Read())
      {
        if (reader.NodeType == XmlNodeType.Element)
        {
          flag = false;
          if (reader.IsEmptyElement)
          {
            reader.Read();
          }
          else
          {
            string name = reader.Name;
            string str = reader.ReadElementContentAsString();
            if (name != "x-ms-invalid-name")
              metadata.Add(name, str);
          }
        }
        else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Metadata")
        {
          reader.Read();
          return metadata;
        }
      }
      return metadata;
    }

    internal static async Task<IDictionary<string, string>> ParseMetadataAsync(XmlReader reader)
    {
      IDictionary<string, string> metadata = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      bool needToRead = true;
      do
      {
        bool flag = needToRead;
        if (flag)
          flag = !await reader.ReadAsync().ConfigureAwait(false);
        if (flag)
          return metadata;
        if (reader.NodeType == XmlNodeType.Element)
        {
          needToRead = false;
          if (reader.IsEmptyElement)
          {
            int num = await reader.ReadAsync().ConfigureAwait(false) ? 1 : 0;
          }
          else
          {
            string elementName = reader.Name;
            string str = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
            if (elementName != "x-ms-invalid-name")
              metadata.Add(elementName, str);
            elementName = (string) null;
          }
        }
      }
      while (reader.NodeType != XmlNodeType.EndElement || !(reader.Name == "Metadata"));
      int num1 = await reader.ReadAsync().ConfigureAwait(false) ? 1 : 0;
      return metadata;
    }
  }
}
