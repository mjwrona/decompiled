// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore.BasicMetaDataStoreItemEncoder`1
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint;
using Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Definitions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore
{
  public abstract class BasicMetaDataStoreItemEncoder<T> : IMetaDataStoreEncoder<T> where T : BasicMetaDataStoreItem
  {
    private string m_previousPath = string.Empty;
    private string m_previousDocId = string.Empty;
    private string m_previousBranchInfo = string.Empty;

    public virtual void Decode(IFileReader reader, T item)
    {
      string path = BasicMetaDataStoreItemEncoder<T>.ReadString(reader, ref this.m_previousPath);
      int count = reader.ReadInt32();
      Hash contentId = new Hash(reader.ReadBytes(count));
      long size = reader.ReadInt64();
      int updateType = reader.ReadInt32();
      string docId = BasicMetaDataStoreItemEncoder<T>.ReadString(reader, ref this.m_previousDocId);
      long num = reader.ReadInt64();
      string s = BasicMetaDataStoreItemEncoder<T>.ReadString(reader, ref this.m_previousBranchInfo);
      List<BranchInfo> branchesInfo;
      if (s == null)
      {
        branchesInfo = new List<BranchInfo>();
      }
      else
      {
        using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(s)))
          branchesInfo = (List<BranchInfo>) new DataContractJsonSerializer(typeof (List<BranchInfo>)).ReadObject((Stream) memoryStream);
      }
      item.Initialize(contentId, path, (MetaDataStoreUpdateType) updateType, size, docId, new long?(num), branchesInfo);
    }

    public virtual void Encode(IFileWriter writer, T item)
    {
      BasicMetaDataStoreItemEncoder<T>.WriteString(writer, ref this.m_previousPath, item.Path);
      byte[] rawHash = item.ContentId.RawHash;
      writer.Write(rawHash.Length);
      writer.Write(rawHash, 0, rawHash.Length);
      writer.Write(item.Size);
      writer.Write((int) item.UpdateType);
      if (item.DocumentId == null)
        BasicMetaDataStoreItemEncoder<T>.WriteString(writer, ref this.m_previousDocId, string.Empty);
      else
        BasicMetaDataStoreItemEncoder<T>.WriteString(writer, ref this.m_previousDocId, item.DocumentId);
      if (item.FilePathId.HasValue)
        writer.Write(item.FilePathId.Value);
      else
        writer.Write(-1L);
      DataContractJsonSerializer contractJsonSerializer = new DataContractJsonSerializer(typeof (List<BranchInfo>));
      if (item.BranchesInfo == null)
      {
        BasicMetaDataStoreItemEncoder<T>.WriteString(writer, ref this.m_previousBranchInfo, string.Empty);
      }
      else
      {
        string str;
        using (MemoryStream memoryStream = new MemoryStream())
        {
          contractJsonSerializer.WriteObject((Stream) memoryStream, (object) item.BranchesInfo);
          str = Encoding.UTF8.GetString(memoryStream.ToArray());
        }
        BasicMetaDataStoreItemEncoder<T>.WriteString(writer, ref this.m_previousBranchInfo, str);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
    [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#")]
    public static string ReadString(IFileReader reader, ref string prefixState)
    {
      int length = BasicMetaDataStoreItemEncoder<T>.ReadSmallInt32(reader);
      string str = reader.ReadString();
      if (length > 0)
        str = prefixState.Substring(0, length) + str;
      prefixState = str;
      return str;
    }

    [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
    [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#")]
    public static void WriteString(IFileWriter writer, ref string prefixState, string value)
    {
      int sharedPrefix = BasicMetaDataStoreItemEncoder<T>.GetSharedPrefix(prefixState, value);
      BasicMetaDataStoreItemEncoder<T>.WriteSmallInt32(writer, sharedPrefix);
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

    public virtual void Initialize()
    {
      this.m_previousPath = string.Empty;
      this.m_previousBranchInfo = string.Empty;
      this.m_previousDocId = string.Empty;
    }

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
