// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.MessagingUtilities
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using Microsoft.Azure.NotificationHubs.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  internal static class MessagingUtilities
  {
    private static AsyncCallback safeCloseCommunicationObjectCallback;

    public static void EnsureTrailingSlash(UriBuilder uriBuilder)
    {
      if (uriBuilder.Path.EndsWith("/", StringComparison.Ordinal))
        return;
      uriBuilder.Path += "/";
    }

    public static UriBuilder CreateUriBuilderWithHttpsSchemeAndPort(Uri baseUri)
    {
      UriBuilder uriBuilder = !(baseUri == (Uri) null) ? new UriBuilder(baseUri) : throw Fx.Exception.AsError((Exception) new ArgumentNullException(nameof (baseUri), SRClient.UseOverloadWithBaseAddress));
      if (uriBuilder.Port == -1)
        uriBuilder.Port = RelayEnvironment.RelayHttpsPort;
      uriBuilder.Scheme = Uri.UriSchemeHttps;
      MessagingUtilities.EnsureTrailingSlash(uriBuilder);
      return uriBuilder;
    }

    public static void CheckValidSequenceNumbers(IEnumerable<long> sequenceNumbers)
    {
      if (sequenceNumbers == null || !sequenceNumbers.Any<long>())
        throw FxTrace.Exception.ArgumentNullOrEmpty(nameof (sequenceNumbers));
    }

    public static void CheckUriSchemeKey(string entityName, string paramName)
    {
      if (string.IsNullOrWhiteSpace(entityName))
        throw Fx.Exception.ArgumentNullOrWhiteSpace(paramName);
      string[] strArray = new string[3]{ "@", "?", "#" };
      foreach (string str in strArray)
      {
        if (entityName.Contains(str))
          throw Fx.Exception.Argument(paramName, Microsoft.Azure.NotificationHubs.SR.GetString(Resources.CharacterReservedForUriScheme, (object) paramName, (object) str));
      }
    }

    public static void AbortIfNotNull(ICommunicationObject communicationObject) => communicationObject?.Abort();

    public static void SafeAddFaulted(
      this ICommunicationObject communicationObject,
      EventHandler handler)
    {
      communicationObject.Faulted += handler;
      switch (communicationObject.State)
      {
        case CommunicationState.Created:
          break;
        case CommunicationState.Opening:
          break;
        case CommunicationState.Opened:
          break;
        default:
          handler((object) communicationObject, EventArgs.Empty);
          break;
      }
    }

    public static void SafeAddClosing(
      this ICommunicationObject communicationObject,
      EventHandler handler)
    {
      communicationObject.Closing += handler;
      switch (communicationObject.State)
      {
        case CommunicationState.Closing:
        case CommunicationState.Closed:
          handler((object) communicationObject, EventArgs.Empty);
          break;
      }
    }

    public static void SafeAddClosed(
      this ICommunicationObject communicationObject,
      EventHandler handler)
    {
      communicationObject.Closed += handler;
      if (communicationObject.State != CommunicationState.Closed)
        return;
      handler((object) communicationObject, EventArgs.Empty);
    }

    public static void SafeClose(this Message message)
    {
      try
      {
        if (message == null || message.State == MessageState.Closed)
          return;
        message.Close();
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
          throw;
        else
          Fx.Exception.TraceHandled(ex, "MessagingUtilities.SafeClose(Message)");
      }
    }

    public static void SafeClose(this ICommunicationObject communicationObject, TimeSpan? timeout = null)
    {
      try
      {
        if (communicationObject == null || communicationObject.State == CommunicationState.Closed)
          return;
        if (MessagingUtilities.safeCloseCommunicationObjectCallback == null)
          MessagingUtilities.safeCloseCommunicationObjectCallback = new AsyncCallback(MessagingUtilities.SafeCloseCommunicationObjectCallback);
        if (timeout.HasValue)
          communicationObject.BeginClose(timeout.Value, MessagingUtilities.safeCloseCommunicationObjectCallback, (object) communicationObject);
        else
          communicationObject.BeginClose(MessagingUtilities.safeCloseCommunicationObjectCallback, (object) communicationObject);
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
        {
          throw;
        }
        else
        {
          Fx.Exception.TraceHandled(ex, "MessagingUtilities.SafeClose(CommunicationObject)");
          communicationObject?.Abort();
        }
      }
    }

    public static void ThrowIfNullAddressesOrPathExists(
      IEnumerable<Uri> addresses,
      string paramName)
    {
      if (addresses == null)
        throw FxTrace.Exception.ArgumentNull(paramName);
      if (addresses.Count<Uri>() == 0)
        throw FxTrace.Exception.Argument(nameof (addresses), SRClient.NoAddressesFound((object) addresses));
      foreach (Uri address in addresses)
        MessagingUtilities.ThrowIfNullAddressOrPathExists(address, "address");
    }

    public static void ThrowIfNullAddressOrPathExists(Uri address, string paramName)
    {
      if (address == (Uri) null)
        throw FxTrace.Exception.ArgumentNull(paramName);
      if (!string.IsNullOrEmpty(address.AbsolutePath) && address.Segments.Length > 3)
        throw Fx.Exception.Argument(paramName, SRClient.InvalidAddressPath((object) address.AbsoluteUri));
    }

    public static IEnumerable<Uri> GetUriList(IEnumerable<string> addresses)
    {
      if (addresses == null)
        throw FxTrace.Exception.ArgumentNull(nameof (addresses));
      List<Uri> uriList = new List<Uri>();
      foreach (string address1 in addresses)
      {
        Uri address2;
        try
        {
          address2 = new Uri(address1);
        }
        catch (UriFormatException ex)
        {
          throw new UriFormatException(SRClient.BadUriFormat((object) address1), (Exception) ex);
        }
        MessagingUtilities.ThrowIfNullAddressOrPathExists(address2, "uriAddress");
        uriList.Add(address2);
      }
      return uriList.Count != 0 ? (IEnumerable<Uri>) uriList : throw Fx.Exception.Argument("uriAddresses", SRClient.InvalidAddressPath((object) uriList));
    }

    private static void SafeCloseCommunicationObjectCallback(IAsyncResult asyncResult)
    {
      ICommunicationObject asyncState = (ICommunicationObject) asyncResult.AsyncState;
      try
      {
        asyncState.EndClose(asyncResult);
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
        {
          throw;
        }
        else
        {
          Fx.Exception.TraceHandled(ex, "MessagingUtilities.SafeCloseCommunicationObjectCallback");
          asyncState.Abort();
        }
      }
    }

    public static string ConvertToString(this SecureString str)
    {
      if (str == null || str.Length == 0)
        return string.Empty;
      IntPtr num = IntPtr.Zero;
      try
      {
        num = Marshal.SecureStringToGlobalAllocUnicode(str);
        return Marshal.PtrToStringUni(num);
      }
      finally
      {
        Marshal.ZeroFreeGlobalAllocUnicode(num);
      }
    }
  }
}
