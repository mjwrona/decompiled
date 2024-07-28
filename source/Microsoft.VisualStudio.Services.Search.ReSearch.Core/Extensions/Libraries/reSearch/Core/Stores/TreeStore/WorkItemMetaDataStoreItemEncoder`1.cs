// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore.WorkItemMetaDataStoreItemEncoder`1
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint;
using Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Definitions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore
{
  public abstract class WorkItemMetaDataStoreItemEncoder<T> : IMetaDataStoreEncoder<T> where T : BasicMetaDataStoreItem
  {
    private string m_previousPath;

    public virtual void Decode(IFileReader reader, T item)
    {
      string path = WorkItemMetaDataStoreItemEncoder<T>.ReadString(reader, ref this.m_previousPath);
      int count = reader.ReadInt32();
      Hash contentId = new Hash(reader.ReadBytes(count));
      long size = reader.ReadInt64();
      int updateType = reader.ReadInt32();
      string documentId = reader.ReadString();
      string contentKey = reader.ReadString();
      item.Initialize(contentId, path, (MetaDataStoreUpdateType) updateType, size, documentId, contentKey);
    }

    public virtual void Encode(IFileWriter writer, T item)
    {
      WorkItemMetaDataStoreItemEncoder<T>.WriteString(writer, ref this.m_previousPath, item.Path);
      byte[] rawHash = item.ContentId.RawHash;
      writer.Write(rawHash.Length);
      writer.Write(rawHash, 0, rawHash.Length);
      writer.Write(item.Size);
      writer.Write((int) item.UpdateType);
      writer.WriteString(item.DocumentId);
      writer.WriteString(item.ContentKey);
    }

    [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#")]
    private static string ReadString(IFileReader reader, ref string prefixState)
    {
      int length = WorkItemMetaDataStoreItemEncoder<T>.ReadSmallInt32(reader);
      string str = reader.ReadString();
      if (length > 0)
        str = prefixState.Substring(0, length) + str;
      prefixState = str;
      return str;
    }

    [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#")]
    private static void WriteString(IFileWriter writer, ref string prefixState, string value)
    {
      int sharedPrefix = WorkItemMetaDataStoreItemEncoder<T>.GetSharedPrefix(prefixState, value);
      WorkItemMetaDataStoreItemEncoder<T>.WriteSmallInt32(writer, sharedPrefix);
      writer.WriteString(value.Substring(sharedPrefix));
      prefixState = value;
    }

    private static int ReadSmallInt32(IFileReader reader)
    {
      byte num = reader.ReadByte();
      return num > (byte) 127 ? reader.ReadInt32() : (int) num;
    }

    private static void WriteSmallInt32(IFileWriter writer, int value)
    {
      if (value >= 0 && value <= (int) sbyte.MaxValue)
      {
        writer.Write((byte) value);
      }
      else
      {
        writer.Write(byte.MaxValue);
        writer.Write(value);
      }
    }

    private static int GetSharedPrefix(string path1, string path2)
    {
      int num = Math.Min(path1.Length, path2.Length);
      int index = 0;
      while (index < num && (int) path1[index] == (int) path2[index])
        ++index;
      return index;
    }

    public virtual void Initialize() => this.m_previousPath = string.Empty;

    private static string ToJson<ty>(T data)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        new DataContractJsonSerializer(typeof (T)).WriteObject((Stream) memoryStream, (object) data);
        return Encoding.Default.GetString(memoryStream.ToArray());
      }
    }
  }
}
