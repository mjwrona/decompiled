// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ServiceHostTags
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.HostManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class ServiceHostTags
  {
    internal static string TagsKey = "Tags=";
    internal static char TagSeparator = ';';
    public static readonly ServiceHostTags EmptyServiceHostTags = new ServiceHostTags();

    private Dictionary<string, string> tags { get; set; } = new Dictionary<string, string>();

    public ServiceHostTags()
    {
    }

    private ServiceHostTags(string str)
    {
      string str1 = str;
      char[] separator = new char[1]{ ';' };
      foreach (string str2 in str1.Split(separator, StringSplitOptions.RemoveEmptyEntries))
      {
        if (str2.Any<char>((Func<char, bool>) (c => c == '=')))
        {
          string[] strArray = str2.Split('=');
          this.tags.Add(strArray[0], strArray[1]);
        }
        else
          this.tags.Add(str2, true.ToString());
      }
    }

    private ServiceHostTags(ServiceHostProperties properties)
      : this(properties.Description)
    {
    }

    public IReadOnlyDictionary<string, string> Tags => (IReadOnlyDictionary<string, string>) this.tags;

    public void AddTag(string tagName) => this.tags.Add(tagName, true.ToString());

    public void AddTag(string tagName, string tagValue) => this.tags.Add(tagName, tagValue);

    public bool HasTag(string tagName) => this.tags.ContainsKey(tagName);

    public string GetTagValue(string tagName) => this.tags[tagName];

    public static ServiceHostTags FromString(string str) => string.IsNullOrEmpty(str) || !str.StartsWith(ServiceHostTags.TagsKey) ? ServiceHostTags.EmptyServiceHostTags : new ServiceHostTags(str.Substring(ServiceHostTags.TagsKey.Length));

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(ServiceHostTags.TagsKey);
      foreach (KeyValuePair<string, string> tag in this.tags)
      {
        bool result;
        if (bool.TryParse(tag.Value, out result) & result)
        {
          stringBuilder.Append(tag.Key);
        }
        else
        {
          stringBuilder.Append(tag.Key);
          stringBuilder.Append('=');
          stringBuilder.Append(tag.Value);
        }
        stringBuilder.Append(ServiceHostTags.TagSeparator);
      }
      return stringBuilder.ToString();
    }
  }
}
