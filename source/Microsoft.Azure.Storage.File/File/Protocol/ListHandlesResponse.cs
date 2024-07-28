// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.Protocol.ListHandlesResponse
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Microsoft.Azure.Storage.File.Protocol
{
  public sealed class ListHandlesResponse : ResponseParsingBase<FileHandle>
  {
    private string nextMarker;
    private bool nextMarkerConsumable;
    private int? maxResults;
    private bool maxResultsConsumable;

    public ListHandlesResponse(Stream stream)
      : base(stream)
    {
    }

    public IEnumerable<FileHandle> Handles => this.ObjectsToParse;

    public string NextMarker
    {
      get
      {
        this.Variable(ref this.nextMarkerConsumable);
        return this.nextMarker;
      }
    }

    public int? MaxResults
    {
      get
      {
        this.Variable(ref this.maxResultsConsumable);
        return this.maxResults;
      }
    }

    private FileHandle ParseHandle()
    {
      FileHandle handle = new FileHandle();
      this.reader.ReadStartElement();
      while (this.reader.IsStartElement())
      {
        if (this.reader.IsEmptyElement)
        {
          this.reader.Skip();
        }
        else
        {
          switch (this.reader.Name)
          {
            case "ClientIp":
              string[] strArray = this.reader.ReadElementContentAsString().Split(new char[1]
              {
                ':'
              }, 2);
              string ipString = strArray[0];
              string s = strArray.Length == 2 ? strArray[1] : (string) null;
              IPAddress ipAddress;
              ref IPAddress local = ref ipAddress;
              if (IPAddress.TryParse(ipString, out local))
                handle.ClientIp = ipAddress;
              int result1;
              if (int.TryParse(s, out result1))
              {
                handle.ClientPort = result1;
                continue;
              }
              continue;
            case "FileId":
              ulong result2;
              if (ulong.TryParse(this.reader.ReadElementContentAsString(), out result2))
              {
                handle.FileId = result2;
                continue;
              }
              continue;
            case "HandleId":
              ulong result3;
              if (ulong.TryParse(this.reader.ReadElementContentAsString(), out result3))
              {
                handle.HandleId = new ulong?(result3);
                continue;
              }
              continue;
            case "LastReconnectTime":
              DateTimeOffset result4;
              if (DateTimeOffset.TryParse(this.reader.ReadElementContentAsString(), out result4))
              {
                handle.LastReconnectTime = new DateTimeOffset?(result4);
                continue;
              }
              continue;
            case "OpenTime":
              DateTimeOffset result5;
              if (DateTimeOffset.TryParse(this.reader.ReadElementContentAsString(), out result5))
              {
                handle.OpenTime = result5;
                continue;
              }
              continue;
            case "ParentId":
              ulong result6;
              if (ulong.TryParse(this.reader.ReadElementContentAsString(), out result6))
              {
                handle.ParentId = result6;
                continue;
              }
              continue;
            case "Path":
              handle.Path = this.reader.ReadElementContentAsString();
              continue;
            case "SessionId":
              ulong result7;
              if (ulong.TryParse(this.reader.ReadElementContentAsString(), out result7))
              {
                handle.SessionId = result7;
                continue;
              }
              continue;
            default:
              this.reader.Skip();
              continue;
          }
        }
      }
      this.reader.ReadEndElement();
      return handle;
    }

    protected override IEnumerable<FileHandle> ParseXml()
    {
      ListHandlesResponse listHandlesResponse = this;
      if (listHandlesResponse.reader.ReadToFollowing("EnumerationResults"))
      {
        if (listHandlesResponse.reader.IsEmptyElement)
        {
          listHandlesResponse.reader.Skip();
        }
        else
        {
          listHandlesResponse.reader.ReadStartElement();
          while (listHandlesResponse.reader.IsStartElement())
          {
            while (listHandlesResponse.reader.IsEmptyElement)
              listHandlesResponse.reader.Skip();
            switch (listHandlesResponse.reader.Name)
            {
              case "Entries":
                listHandlesResponse.reader.ReadStartElement();
                while (listHandlesResponse.reader.IsStartElement())
                {
                  if (listHandlesResponse.reader.Name == "Handle")
                    yield return listHandlesResponse.ParseHandle();
                }
                listHandlesResponse.reader.ReadEndElement();
                continue;
              case "NextMarker":
                listHandlesResponse.nextMarker = listHandlesResponse.reader.ReadElementContentAsString();
                listHandlesResponse.nextMarkerConsumable = true;
                yield return (FileHandle) null;
                continue;
              case "MaxResults":
                listHandlesResponse.maxResults = new int?(listHandlesResponse.reader.ReadElementContentAsInt());
                listHandlesResponse.maxResultsConsumable = true;
                yield return (FileHandle) null;
                continue;
              default:
                listHandlesResponse.reader.Skip();
                continue;
            }
          }
          listHandlesResponse.allObjectsParsed = true;
        }
      }
    }
  }
}
