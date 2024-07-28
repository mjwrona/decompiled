// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Shared.Protocol.GetUserDelegationKeyResponse
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.Azure.Storage.Shared.Protocol
{
  internal static class GetUserDelegationKeyResponse
  {
    internal static async Task<UserDelegationKey> ParseAsync(Stream stream, CancellationToken token)
    {
      UserDelegationKey key = (UserDelegationKey) null;
      using (XmlReader reader = XMLReaderExtensions.CreateAsAsync(stream))
      {
        token.ThrowIfCancellationRequested();
        if (await reader.ReadToFollowingAsync("UserDelegationKey").ConfigureAwait(false))
        {
          if (reader.IsEmptyElement)
            await reader.SkipAsync().ConfigureAwait(false);
          else
            key = await GetUserDelegationKeyResponse.ParseKey(reader, token).ConfigureAwait(false);
        }
      }
      UserDelegationKey async = key;
      key = (UserDelegationKey) null;
      return async;
    }

    private static async Task<UserDelegationKey> ParseKey(XmlReader reader, CancellationToken token)
    {
      token.ThrowIfCancellationRequested();
      Guid signedOid = new Guid();
      Guid signedTid = new Guid();
      DateTimeOffset signedStart = new DateTimeOffset();
      DateTimeOffset signedExpiry = new DateTimeOffset();
      string signedService = (string) null;
      string signedVersion = (string) null;
      string value = (string) null;
      await reader.ReadStartElementAsync().ConfigureAwait(false);
      while (true)
      {
        if (await reader.IsStartElementAsync().ConfigureAwait(false))
        {
          switch (reader.Name)
          {
            case "SignedExpiry":
              signedExpiry = DateTimeOffset.Parse(await reader.ReadElementContentAsStringAsync().ConfigureAwait(false));
              continue;
            case "SignedOid":
              signedOid = Guid.Parse(await reader.ReadElementContentAsStringAsync().ConfigureAwait(false));
              continue;
            case "SignedService":
              signedService = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
              continue;
            case "SignedStart":
              signedStart = DateTimeOffset.Parse(await reader.ReadElementContentAsStringAsync().ConfigureAwait(false));
              continue;
            case "SignedTid":
              signedTid = Guid.Parse(await reader.ReadElementContentAsStringAsync().ConfigureAwait(false));
              continue;
            case "SignedVersion":
              signedVersion = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
              continue;
            case "Value":
              value = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
              continue;
            default:
              await reader.SkipAsync().ConfigureAwait(false);
              continue;
          }
        }
        else
          break;
      }
      await reader.ReadEndElementAsync().ConfigureAwait(false);
      UserDelegationKey key = new UserDelegationKey()
      {
        SignedOid = new Guid?(signedOid),
        SignedTid = new Guid?(signedTid),
        SignedStart = new DateTimeOffset?(signedStart),
        SignedExpiry = new DateTimeOffset?(signedExpiry),
        SignedService = signedService,
        SignedVersion = signedVersion,
        Value = value
      };
      signedService = (string) null;
      signedVersion = (string) null;
      value = (string) null;
      return key;
    }
  }
}
