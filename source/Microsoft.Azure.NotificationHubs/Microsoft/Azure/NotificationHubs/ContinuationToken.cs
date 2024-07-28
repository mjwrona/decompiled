// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.ContinuationToken
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Azure.NotificationHubs
{
  internal class ContinuationToken
  {
    public ContinuationToken(string nextPartition, int skip)
    {
      this.NextPartition = !string.IsNullOrWhiteSpace(nextPartition) ? nextPartition : throw new ArgumentNullException(nameof (nextPartition));
      this.Skip = skip;
      this.Token = ContinuationToken.ToBase64UriEscapeString(nextPartition + ";" + skip.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.IsValid = true;
    }

    public ContinuationToken(string continuationTokenString)
    {
      this.IsValid = false;
      if (string.IsNullOrWhiteSpace(continuationTokenString))
      {
        this.IsValid = true;
      }
      else
      {
        string str = ContinuationToken.FromBase64UriEscapeString(continuationTokenString);
        if (str == null)
          return;
        string[] strArray = str.Split(';');
        if (strArray.Length != 2)
          return;
        this.NextPartition = strArray[0];
        int result = 0;
        if (!string.IsNullOrWhiteSpace(strArray[1]) && !int.TryParse(strArray[1], out result))
          result = 0;
        this.Skip = result;
        this.Token = continuationTokenString;
        this.IsValid = true;
      }
    }

    public bool IsValid { get; private set; }

    public string NextPartition { get; private set; }

    public int Skip { get; private set; }

    public string Token { get; private set; }

    private static string ToBase64UriEscapeString(string value) => string.IsNullOrWhiteSpace(value) ? value : Uri.EscapeDataString(Convert.ToBase64String(Encoding.UTF8.GetBytes(value)));

    private static string FromBase64UriEscapeString(string base64Value)
    {
      if (string.IsNullOrWhiteSpace(base64Value))
        return (string) null;
      try
      {
        return Encoding.UTF8.GetString(Convert.FromBase64String(Uri.UnescapeDataString(base64Value)));
      }
      catch (ArgumentNullException ex)
      {
        return (string) null;
      }
      catch (FormatException ex)
      {
        return (string) null;
      }
    }
  }
}
