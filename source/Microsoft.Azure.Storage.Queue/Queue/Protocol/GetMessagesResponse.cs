// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Queue.Protocol.GetMessagesResponse
// Assembly: Microsoft.Azure.Storage.Queue, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3D35BFA0-638A-4C3C-8E74-B592D3B60EFD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Queue.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.Azure.Storage.Queue.Protocol
{
  public static class GetMessagesResponse
  {
    private static async Task<QueueMessage> ParseMessageEntryAsync(
      XmlReader reader,
      CancellationToken token)
    {
      token.ThrowIfCancellationRequested();
      string id = (string) null;
      string popReceipt = (string) null;
      DateTimeOffset? insertionTime = new DateTimeOffset?();
      DateTimeOffset? expirationTime = new DateTimeOffset?();
      DateTimeOffset? timeNextVisible = new DateTimeOffset?();
      string text = (string) null;
      int dequeueCount = 0;
      await reader.ReadStartElementAsync().ConfigureAwait(false);
      while (true)
      {
        if (await reader.IsStartElementAsync().ConfigureAwait(false))
        {
          token.ThrowIfCancellationRequested();
          if (reader.IsEmptyElement)
          {
            await reader.SkipAsync().ConfigureAwait(false);
          }
          else
          {
            switch (reader.Name)
            {
              case "DequeueCount":
                dequeueCount = await reader.ReadElementContentAsInt32Async().ConfigureAwait(false);
                continue;
              case "ExpirationTime":
                expirationTime = new DateTimeOffset?(await reader.ReadElementContentAsDateTimeOffsetAsync().ConfigureAwait(false));
                continue;
              case "InsertionTime":
                insertionTime = new DateTimeOffset?(await reader.ReadElementContentAsDateTimeOffsetAsync().ConfigureAwait(false));
                continue;
              case "MessageId":
                id = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                continue;
              case "MessageText":
                text = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                continue;
              case "PopReceipt":
                popReceipt = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                continue;
              case "TimeNextVisible":
                timeNextVisible = new DateTimeOffset?(await reader.ReadElementContentAsDateTimeOffsetAsync().ConfigureAwait(false));
                continue;
              default:
                await reader.SkipAsync().ConfigureAwait(false);
                continue;
            }
          }
        }
        else
          break;
      }
      await reader.ReadEndElementAsync().ConfigureAwait(false);
      QueueMessage queueMessage = new QueueMessage()
      {
        Text = text,
        Id = id,
        PopReceipt = popReceipt,
        DequeueCount = dequeueCount
      };
      if (insertionTime.HasValue)
        queueMessage.InsertionTime = new DateTimeOffset?(insertionTime.Value);
      if (expirationTime.HasValue)
        queueMessage.ExpirationTime = new DateTimeOffset?(expirationTime.Value);
      if (timeNextVisible.HasValue)
        queueMessage.NextVisibleTime = new DateTimeOffset?(timeNextVisible.Value);
      QueueMessage messageEntryAsync = queueMessage;
      id = (string) null;
      popReceipt = (string) null;
      text = (string) null;
      return messageEntryAsync;
    }

    internal static async Task<IEnumerable<QueueMessage>> ParseAsync(
      Stream stream,
      CancellationToken token)
    {
      IEnumerable<QueueMessage> async;
      using (XmlReader reader = XMLReaderExtensions.CreateAsAsync(stream))
      {
        token.ThrowIfCancellationRequested();
        List<QueueMessage> messages = new List<QueueMessage>();
        ConfiguredTaskAwaitable<bool> configuredTaskAwaitable = reader.ReadToFollowingAsync("QueueMessagesList").ConfigureAwait(false);
        if (await configuredTaskAwaitable)
        {
          if (reader.IsEmptyElement)
          {
            await reader.SkipAsync().ConfigureAwait(false);
          }
          else
          {
            await reader.ReadStartElementAsync().ConfigureAwait(false);
label_14:
            while (true)
            {
              configuredTaskAwaitable = reader.IsStartElementAsync().ConfigureAwait(false);
              if (await configuredTaskAwaitable)
              {
                token.ThrowIfCancellationRequested();
                if (reader.IsEmptyElement)
                  await reader.SkipAsync().ConfigureAwait(false);
                else if (!(reader.Name == "QueueMessage"))
                  await reader.SkipAsync().ConfigureAwait(false);
                else
                  break;
              }
              else
                goto label_16;
            }
            while (true)
            {
              configuredTaskAwaitable = reader.IsStartElementAsync().ConfigureAwait(false);
              if (await configuredTaskAwaitable)
              {
                List<QueueMessage> queueMessageList = messages;
                queueMessageList.Add(await GetMessagesResponse.ParseMessageEntryAsync(reader, token).ConfigureAwait(false));
                queueMessageList = (List<QueueMessage>) null;
              }
              else
                goto label_14;
            }
label_16:
            await reader.ReadEndElementAsync().ConfigureAwait(false);
          }
        }
        async = (IEnumerable<QueueMessage>) messages;
      }
      return async;
    }
  }
}
