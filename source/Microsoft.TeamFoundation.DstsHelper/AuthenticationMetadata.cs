// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DstsHelper.AuthenticationMetadata
// Assembly: Microsoft.TeamFoundation.DstsHelper, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08D47267-3A15-4307-BBA0-1792E9C6BDF1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DstsHelper.dll

using Microsoft.VisualStudio.Services.Common;
using System.Text;

namespace Microsoft.TeamFoundation.DstsHelper
{
  internal class AuthenticationMetadata
  {
    public string Realm { get; }

    public string Authority { get; }

    public string Resource { get; }

    public AuthenticationMetadata(string realm, string authority, string resource)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(realm, nameof (realm));
      ArgumentUtility.CheckStringForNullOrEmpty(authority, nameof (authority));
      ArgumentUtility.CheckStringForNullOrEmpty(resource, nameof (resource));
      this.Realm = realm;
      this.Authority = authority;
      this.Resource = resource;
    }

    public override string ToString() => "realm=" + AuthenticationMetadata.EscapeAsQuotedString(this.Realm) + ", authorization_uri=" + AuthenticationMetadata.EscapeAsQuotedString(this.Authority) + ", resource=" + AuthenticationMetadata.EscapeAsQuotedString(this.Resource);

    private static string EscapeAsQuotedString(string str)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append('"');
      foreach (char ch in str)
      {
        if ((ch > '\u001F' || ch == '\t') && ch != '\u007F')
        {
          if (ch == '\\' || ch == '"')
            stringBuilder.Append('\\');
          stringBuilder.Append(ch);
        }
      }
      stringBuilder.Append('"');
      return stringBuilder.ToString();
    }
  }
}
