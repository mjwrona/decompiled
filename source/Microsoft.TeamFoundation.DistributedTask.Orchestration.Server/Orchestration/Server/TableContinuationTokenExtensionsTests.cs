// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TableContinuationTokenExtensionsTests
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Xml;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal static class TableContinuationTokenExtensionsTests
  {
    private const string c_version = "2.0";
    private const string c_type = "Table";

    public static void WriteXml(this TableContinuationToken token, XmlWriter writer)
    {
      ArgumentUtility.CheckForNull<TableContinuationToken>(token, nameof (token));
      ArgumentUtility.CheckForNull<XmlWriter>(writer, nameof (writer));
      writer.WriteStartElement("ContinuationToken");
      writer.WriteElementString("Version", "2.0");
      writer.WriteElementString("Type", "Table");
      if (token.NextPartitionKey != null)
        writer.WriteElementString("NextPartitionKey", token.NextPartitionKey);
      if (token.NextRowKey != null)
        writer.WriteElementString("NextRowKey", token.NextRowKey);
      if (token.NextTableName != null)
        writer.WriteElementString("NextTableName", token.NextTableName);
      writer.WriteElementString("TargetLocation", token.TargetLocation.ToString());
      writer.WriteEndElement();
    }

    public static void ReadXml(this TableContinuationToken token, XmlReader reader)
    {
      ArgumentUtility.CheckForNull<TableContinuationToken>(token, nameof (token));
      ArgumentUtility.CheckForNull<XmlReader>(reader, nameof (reader));
      int content1 = (int) reader.MoveToContent();
      reader.ReadStartElement();
      int content2 = (int) reader.MoveToContent();
      if (reader.Name == "ContinuationToken")
        reader.ReadStartElement();
      while (reader.IsStartElement())
      {
        switch (reader.Name)
        {
          case "Version":
            reader.ReadElementContentAsString();
            continue;
          case "Type":
            reader.ReadElementContentAsString();
            continue;
          case "TargetLocation":
            string str = reader.ReadElementContentAsString();
            StorageLocation result;
            if (Enum.TryParse<StorageLocation>(str, out result))
            {
              token.TargetLocation = new StorageLocation?(result);
              continue;
            }
            if (!string.IsNullOrEmpty(str))
              throw new XmlException("Unexpected Location '" + str + "'");
            continue;
          case "NextPartitionKey":
            token.NextPartitionKey = reader.ReadElementContentAsString();
            continue;
          case "NextRowKey":
            token.NextRowKey = reader.ReadElementContentAsString();
            continue;
          case "NextTableName":
            token.NextTableName = reader.ReadElementContentAsString();
            continue;
          default:
            throw new XmlException("Unexpected Element '" + reader.Name + "'");
        }
      }
    }
  }
}
