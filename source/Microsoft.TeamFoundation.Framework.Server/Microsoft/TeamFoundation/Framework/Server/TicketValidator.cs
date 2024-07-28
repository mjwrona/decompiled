// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TicketValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Specialized;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TicketValidator
  {
    public bool IsUnexpired(NameValueCollection parameters, DateTime arrivalTime)
    {
      string parameter = parameters["ts"];
      long result;
      return parameter != null && long.TryParse(parameter, out result) && result >= arrivalTime.Ticks;
    }

    public bool IsValidRsaSignedTicket(ISigner signer, NameValueCollection parameters)
    {
      string parameter1 = parameters["fid"];
      string parameter2 = parameters["sfid"];
      string parameter3 = parameters["s"];
      string parameter4 = parameters["ts"];
      if (parameter1 == null || parameter2 == null || parameter3 == null || parameter4 == null)
        return false;
      byte[] queryStringHash = RequestSignatures.GenerateQueryStringHash(RequestSignatures.NormalizeQueryString(parameter2, parameter4));
      byte[] signature = Convert.FromBase64String(parameter3);
      int result;
      if (!signer.VerifyHash(queryStringHash, signature) || !int.TryParse(parameter1, out result))
        return false;
      string str = parameter2;
      char[] chArray = new char[1]{ ',' };
      foreach (string s in str.Split(chArray))
      {
        if (int.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture) == result)
          return true;
      }
      return false;
    }
  }
}
