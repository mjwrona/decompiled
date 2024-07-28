// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TicketGenerator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TicketGenerator
  {
    private long m_expirationTime;
    private string m_instanceId;
    private string m_collectionPath;

    internal TicketGenerator(DateTime expirationTime, string instanceId, string collectionPath)
    {
      this.m_expirationTime = !(expirationTime == DateTime.MinValue) ? expirationTime.Ticks : DateTime.UtcNow.AddHours(24.0).Ticks;
      this.m_instanceId = instanceId;
      this.m_collectionPath = collectionPath;
    }

    internal TicketGenerator(string instanceId, string collectionPath)
      : this(DateTime.UtcNow.AddHours(24.0), instanceId, collectionPath)
    {
    }

    internal void GenerateRsaSignedTickets(
      IVssRequestContext requestContext,
      int[] fileIds,
      string[] tickets,
      int startIndex,
      int count)
    {
      IVssRequestContext context = requestContext.Elevate();
      ITeamFoundationSigningService service = context.GetService<ITeamFoundationSigningService>();
      string query = RequestSignatures.NormalizeQueryString(fileIds, this.m_expirationTime);
      byte[] queryStringHash = RequestSignatures.GenerateQueryStringHash(query);
      IVssRequestContext requestContext1 = context;
      Guid proxySigningKey = ProxyConstants.ProxySigningKey;
      byte[] message = queryStringHash;
      byte[] signature = service.Sign(requestContext1, proxySigningKey, message);
      string str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}&{2}&{3}={4}&{5}=", (object) "type", (object) "rsa", (object) query, (object) "s", (object) Uri.EscapeDataString(TicketGenerator.Base64EncodeSignature(signature)), (object) "fid");
      string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "&{0}={1}&{2}={3}", (object) "iid", (object) this.m_instanceId, (object) "cp", (object) this.m_collectionPath);
      for (int index = 0; index < count; ++index)
        tickets[index + startIndex] = str1 + fileIds[index + startIndex].ToString("D", (IFormatProvider) CultureInfo.InvariantCulture) + str2;
    }

    private static string Base64EncodeSignature(byte[] signature) => Convert.ToBase64String(signature);
  }
}
