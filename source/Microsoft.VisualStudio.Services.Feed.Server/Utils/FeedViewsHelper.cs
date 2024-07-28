// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Utils.FeedViewsHelper
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Feed.Server.Utils
{
  public static class FeedViewsHelper
  {
    public static Dictionary<string, string> DeserializeView(string viewsString) => !string.IsNullOrEmpty(viewsString) ? JsonConvert.DeserializeObject<Dictionary<string, string>>(viewsString) : new Dictionary<string, string>();

    public static IList<FeedView> ToViews(string viewsString) => !string.IsNullOrEmpty(viewsString) ? JsonConvert.DeserializeObject<IList<FeedView>>(viewsString) : (IList<FeedView>) new List<FeedView>();

    public static string ParseFeedNameParameter(string feedNameParameter, out string viewName)
    {
      string feedNameParameter1;
      (feedNameParameter1, viewName) = FeedViewsHelper.ParseFeedNameParameter(feedNameParameter);
      return feedNameParameter1;
    }

    public static (string, string) ParseFeedNameParameter(string feedNameParameter)
    {
      string str1 = feedNameParameter;
      string str2 = (string) null;
      if (feedNameParameter.Contains("@"))
      {
        string[] strArray = feedNameParameter.Split(new char[1]
        {
          '@'
        }, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length == 2)
        {
          str1 = strArray[0];
          str2 = strArray[1];
        }
      }
      return (str1, str2);
    }
  }
}
