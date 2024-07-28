// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.UxServices.SignInPlaceholderTransformer
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess.UxServices
{
  public class SignInPlaceholderTransformer : IContentTransformer
  {
    private const string SignInOutTextPlaceholder = ":SignInOutText:";
    private const string SignInOutLinkPlaceholder = ":SignInOutLink:";
    private const string ProfileTextPlaceholder = ":ProfileText:";
    private const string ProfileLinkPlaceholder = ":ProfileLink:";

    public bool CanHandleRequest(UxServicesRequestData uxRequestData, string content) => uxRequestData != null && content != null && "header".Equals(uxRequestData.Control, StringComparison.OrdinalIgnoreCase) && content.Contains(":SignInOutLink:");

    public string TransformContent(UxServicesRequestData uxRequestData, string content)
    {
      if (uxRequestData == null)
        throw new ArgumentNullException(nameof (uxRequestData));
      return string.IsNullOrEmpty(content) ? content : SignInPlaceholderTransformer.GetPlaceholderMap(uxRequestData).Aggregate<KeyValuePair<string, string>, string>(content, (Func<string, KeyValuePair<string, string>, string>) ((current, pair) => current.Replace(pair.Key, pair.Value)));
    }

    private static Dictionary<string, string> GetPlaceholderMap(UxServicesRequestData uxRequestData) => new Dictionary<string, string>()
    {
      {
        ":SignInOutText:",
        HttpUtility.HtmlEncode(uxRequestData.LoginContext.SignInOutText)
      },
      {
        ":SignInOutLink:",
        HttpUtility.HtmlEncode(uxRequestData.LoginContext.SignInOutLink)
      },
      {
        ":ProfileText:",
        HttpUtility.HtmlEncode(uxRequestData.LoginContext.ProfileText)
      },
      {
        ":ProfileLink:",
        HttpUtility.HtmlEncode(uxRequestData.LoginContext.ProfileLink)
      }
    };
  }
}
