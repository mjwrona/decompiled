// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MailQueueEntry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class MailQueueEntry
  {
    private const int ReadyForProcessingStatus = 0;
    private const int MailPriorityLow = 0;
    private const int MailPriorityNormal = 1;
    private const int MailPriorityHigh = 2;
    private const string c_Area = "MailService";
    private const string c_Layer = "Service";

    public static MailQueueEntry Create(
      MailRequest mailRequest,
      DateTime queuedTimeUtc,
      int maxRetryCount)
    {
      ArgumentUtility.CheckForNull<MailRequest>(mailRequest, nameof (mailRequest));
      MailMessage message = mailRequest.Message;
      return new MailQueueEntry()
      {
        Id = mailRequest.Id,
        TfId = mailRequest.RequesterTfId,
        Status = 0,
        QueuedTimeUtc = queuedTimeUtc,
        RetryCount = maxRetryCount,
        Subject = message.Subject,
        Body = message.Body,
        To = message.To.ToString(),
        CC = message.CC.ToString(),
        BCC = message.Bcc.ToString(),
        ReplyTo = message.ReplyToList.ToString(),
        Priority = MailQueueEntry.GetMessagePriority(message),
        Headers = MailQueueEntry.MessageHeadersToXml(message),
        From = message.From == null ? string.Empty : message.From.ToString(),
        Sender = message.Sender == null ? string.Empty : message.Sender.ToString()
      };
    }

    public Guid Id { get; set; }

    public Guid TfId { get; set; }

    public int Status { get; set; }

    public DateTime QueuedTimeUtc { get; set; }

    public int RetryCount { get; set; }

    public string Subject { get; set; }

    public string Body { get; set; }

    public string To { get; set; }

    public string CC { get; set; }

    public string BCC { get; set; }

    public string ReplyTo { get; set; }

    public int Priority { get; set; }

    public string Headers { get; set; }

    public string Sender { get; set; }

    public string From { get; set; }

    public MailRequest ToMailRequest(IVssRequestContext requestContext)
    {
      MailMessage message = new MailMessage();
      message.SubjectEncoding = TeamFoundationMailService.MailHeaderEncoding;
      message.Subject = this.Subject;
      message.BodyEncoding = Encoding.UTF8;
      message.IsBodyHtml = true;
      message.Body = this.Body;
      if (!string.IsNullOrWhiteSpace(this.From))
      {
        message.From = MailQueueEntry.GetMailAddressFromString(requestContext, this.From);
        ArgumentUtility.CheckForNull<MailAddress>(message.From, "MailRequest.From");
      }
      if (!string.IsNullOrWhiteSpace(this.Sender))
      {
        message.Sender = MailQueueEntry.GetMailAddressFromString(requestContext, this.Sender);
        ArgumentUtility.CheckForNull<MailAddress>(message.Sender, "MailRequest.Sender");
      }
      if (!string.IsNullOrWhiteSpace(this.To))
      {
        MailQueueEntry.PopulateAddressesFromString(requestContext, this.To, message.To);
        if (message.To.Count < 1)
          requestContext.Trace(1001090, TraceLevel.Warning, "MailService", "Service", string.Format("Message: {0} To '{1}' resulted in no addresses", (object) this.Id, (object) this.To));
      }
      if (!string.IsNullOrWhiteSpace(this.CC))
      {
        MailQueueEntry.PopulateAddressesFromString(requestContext, this.CC, message.CC);
        if (message.CC.Count < 1)
          requestContext.Trace(1001090, TraceLevel.Warning, "MailService", "Service", string.Format("Message: {0} CC '{1}' resulted in no addresses", (object) this.Id, (object) this.CC));
      }
      if (!string.IsNullOrWhiteSpace(this.BCC))
      {
        MailQueueEntry.PopulateAddressesFromString(requestContext, this.BCC, message.Bcc);
        if (message.Bcc.Count < 1)
          requestContext.Trace(1001090, TraceLevel.Warning, "MailService", "Service", string.Format("Message: {0} BCC '{1}' resulted in no addresses", (object) this.Id, (object) this.BCC));
      }
      if (!string.IsNullOrWhiteSpace(this.ReplyTo))
      {
        MailQueueEntry.PopulateAddressesFromString(requestContext, this.ReplyTo, message.ReplyToList);
        if (message.ReplyToList.Count < 1)
          requestContext.Trace(1001090, TraceLevel.Warning, "MailService", "Service", string.Format("Message: {0} ReplyTo '{1}' resulted in no addresses", (object) this.Id, (object) this.ReplyTo));
      }
      MailQueueEntry.SetMessagePriority(message, this.Priority);
      message.HeadersEncoding = TeamFoundationMailService.MailBodyEncoding;
      MailQueueEntry.MessageHeadersFromXml(message, this.Headers);
      return new MailRequest(message, this.Id, this.TfId);
    }

    internal static void PopulateAddressesFromString(
      IVssRequestContext requestContext,
      string addresses,
      MailAddressCollection mailAddresses)
    {
      bool flag = false;
      try
      {
        mailAddresses.Add(addresses);
      }
      catch (Exception ex)
      {
        flag = true;
        requestContext.TraceException(1001090, "MailService", "Service", ex);
      }
      if (!flag)
        return;
      MailQueueEntry.PopulateMailAddressesFromStringTheHardWay(addresses, mailAddresses);
    }

    internal static MailAddress GetMailAddressFromString(
      IVssRequestContext requestContext,
      string address)
    {
      MailAddress addressFromString = (MailAddress) null;
      bool flag = false;
      try
      {
        addressFromString = new MailAddress(address);
      }
      catch (Exception ex)
      {
        flag = true;
        requestContext.TraceException(1001090, "MailService", "Service", ex);
      }
      if (flag)
        addressFromString = MailQueueEntry.GetMailAddressFromStringTheHardWay(address);
      return addressFromString;
    }

    private static void PopulateMailAddressesFromStringTheHardWay(
      string addresses,
      MailAddressCollection mailAddresses)
    {
      int length = addresses.Length;
      int endIndex;
      for (int startIndex = 0; startIndex < length; startIndex = endIndex)
      {
        MailAddress theHardWayWorker = MailQueueEntry.GetMailAddressFromStringTheHardWayWorker(addresses, startIndex, out endIndex);
        if (theHardWayWorker != null)
          mailAddresses.Add(theHardWayWorker);
      }
    }

    private static MailAddress GetMailAddressFromStringTheHardWay(string address) => MailQueueEntry.GetMailAddressFromStringTheHardWayWorker(address, 0, out int _);

    private static MailAddress GetMailAddressFromStringTheHardWayWorker(
      string rawAddress,
      int startIndex,
      out int endIndex)
    {
      MailAddress theHardWayWorker = (MailAddress) null;
      int index = startIndex;
      int length = rawAddress.Length;
      StringBuilder stringBuilder = new StringBuilder();
      char c1;
      for (c1 = rawAddress[index]; char.IsWhiteSpace(c1) && index < length; c1 = rawAddress[index])
        ++index;
      if (c1 == '"')
      {
        bool flag = false;
        ++index;
        while (index < length)
        {
          char ch = rawAddress[index++];
          if (flag)
          {
            flag = false;
          }
          else
          {
            switch (ch)
            {
              case '"':
                ++index;
                goto label_11;
              case '\\':
                flag = true;
                continue;
              default:
                continue;
            }
          }
        }
      }
label_11:
      if (index < length)
      {
        char c2;
        for (c2 = rawAddress[index]; char.IsWhiteSpace(c2) && index < length; c2 = rawAddress[index])
          ++index;
        for (; !char.IsWhiteSpace(c2) && c2 != ',' && c2 != ';'; c2 = rawAddress[index])
        {
          stringBuilder.Append(c2);
          ++index;
          if (index >= length)
            break;
        }
        ++index;
        if (stringBuilder.Length > 0)
          theHardWayWorker = new MailAddress(stringBuilder.ToString());
      }
      endIndex = index;
      return theHardWayWorker;
    }

    private static void MessageHeadersFromXml(MailMessage message, string headerXmlString)
    {
      ArgumentUtility.CheckForNull<MailMessage>(message, nameof (message));
      if (string.IsNullOrWhiteSpace(headerXmlString))
        return;
      foreach (KeyValue<string, string> keyValue in TeamFoundationSerializationUtility.Deserialize<DictionaryWrapper<string, string>>(headerXmlString).SerializedValue)
        message.Headers.Add(keyValue.Key, keyValue.Value);
    }

    private static string MessageHeadersToXml(MailMessage message)
    {
      ArgumentUtility.CheckForNull<MailMessage>(message, nameof (message));
      string empty = string.Empty;
      NameValueCollection headers = message.Headers;
      if (headers.Count > 0)
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>(headers.Count);
        foreach (string key in headers.Keys)
          dictionary[key] = headers[key];
        empty = TeamFoundationSerializationUtility.SerializeToString<DictionaryWrapper<string, string>>(new DictionaryWrapper<string, string>((IDictionary<string, string>) dictionary));
      }
      return empty;
    }

    private static void SetMessagePriority(MailMessage message, int priority)
    {
      ArgumentUtility.CheckForNull<MailMessage>(message, nameof (message));
      MailMessage mailMessage = message;
      int num;
      switch (priority)
      {
        case 0:
          num = 1;
          break;
        case 2:
          num = 2;
          break;
        default:
          num = 0;
          break;
      }
      mailMessage.Priority = (MailPriority) num;
    }

    private static int GetMessagePriority(MailMessage message)
    {
      ArgumentUtility.CheckForNull<MailMessage>(message, nameof (message));
      switch (message.Priority)
      {
        case MailPriority.Low:
          return 0;
        case MailPriority.High:
          return 2;
        default:
          return 1;
      }
    }
  }
}
