// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotifHelpers
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class NotifHelpers
  {
    public static char[] EmailLocalSpecialChars = new char[20]
    {
      '!',
      '#',
      '$',
      '%',
      '&',
      '\'',
      '*',
      '+',
      '-',
      '.',
      '/',
      '=',
      '?',
      '^',
      '_',
      '`',
      '{',
      '|',
      '}',
      '~'
    };
    public static char[] EmailLocalTranslateFromChars = new char[13]
    {
      '"',
      '(',
      ')',
      '*',
      ',',
      ':',
      ';',
      '<',
      '>',
      '@',
      '[',
      '\\',
      ']'
    };
    public static char[] EmailLocalTranslateToChars = new char[13]
    {
      '\'',
      '{',
      '}',
      '+',
      '.',
      '.',
      '.',
      '{',
      '}',
      '#',
      '{',
      '/',
      '}'
    };
    private static Dictionary<char, char> AsciiCharCharMap = new Dictionary<char, char>()
    {
      {
        '¡',
        '!'
      },
      {
        'µ',
        'u'
      },
      {
        'À',
        'A'
      },
      {
        'Á',
        'A'
      },
      {
        'Â',
        'A'
      },
      {
        'Ã',
        'A'
      },
      {
        'Ä',
        'A'
      },
      {
        'Å',
        'A'
      },
      {
        'Ç',
        'C'
      },
      {
        'È',
        'E'
      },
      {
        'É',
        'E'
      },
      {
        'Ê',
        'E'
      },
      {
        'Ë',
        'E'
      },
      {
        'Ì',
        'I'
      },
      {
        'Í',
        'I'
      },
      {
        'Î',
        'I'
      },
      {
        'Ï',
        'I'
      },
      {
        'Ð',
        'D'
      },
      {
        'Ñ',
        'N'
      },
      {
        'Ò',
        'O'
      },
      {
        'Ó',
        'O'
      },
      {
        'Ô',
        'O'
      },
      {
        'Õ',
        'O'
      },
      {
        'Ö',
        'O'
      },
      {
        'Ø',
        'O'
      },
      {
        'Ù',
        'U'
      },
      {
        'Ú',
        'U'
      },
      {
        'Û',
        'U'
      },
      {
        'Ü',
        'U'
      },
      {
        'Ý',
        'Y'
      },
      {
        'Þ',
        'P'
      },
      {
        'à',
        'a'
      },
      {
        'á',
        'a'
      },
      {
        'â',
        'a'
      },
      {
        'ã',
        'a'
      },
      {
        'ä',
        'a'
      },
      {
        'å',
        'a'
      },
      {
        'ç',
        'c'
      },
      {
        'è',
        'e'
      },
      {
        'é',
        'e'
      },
      {
        'ê',
        'e'
      },
      {
        'ë',
        'e'
      },
      {
        'ì',
        'i'
      },
      {
        'í',
        'i'
      },
      {
        'î',
        'i'
      },
      {
        'ï',
        'i'
      },
      {
        'ð',
        'o'
      },
      {
        'ñ',
        'o'
      },
      {
        'ò',
        'n'
      },
      {
        'ó',
        'o'
      },
      {
        'ô',
        'o'
      },
      {
        'õ',
        'o'
      },
      {
        'ö',
        'o'
      },
      {
        'ø',
        'o'
      },
      {
        'ù',
        'u'
      },
      {
        'ú',
        'u'
      },
      {
        'û',
        'u'
      },
      {
        'ü',
        'u'
      },
      {
        'ý',
        'y'
      },
      {
        'þ',
        'p'
      },
      {
        'ÿ',
        'y'
      }
    };
    private static Dictionary<char, string> AsciiCharStringMap = new Dictionary<char, string>()
    {
      {
        '©',
        "(c)"
      },
      {
        '®',
        "(r)"
      },
      {
        'Æ',
        "AE"
      },
      {
        'æ',
        "ae"
      },
      {
        'ß',
        "ss"
      }
    };

    public static string FirstNonEmptyString(string a, string b, params object[] args)
    {
      string a1 = !string.IsNullOrEmpty(a) ? a : b;
      if (string.IsNullOrEmpty(a1) && args != null && args.Length != 0)
      {
        foreach (object obj in args)
        {
          a1 = obj as string;
          if (!string.IsNullOrEmpty(a1))
            break;
        }
      }
      if (string.Equals(a1, string.Empty))
        a1 = (string) null;
      return a1;
    }

    public static string ReplaceNonEmailAddressLocalChars(this string s)
    {
      if (s == null)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder(s.Length);
      char ch1 = ' ';
      foreach (char c in s)
      {
        char ch2;
        if (char.IsLetterOrDigit(c) || Array.BinarySearch<char>(NotifHelpers.EmailLocalSpecialChars, c) >= 0)
        {
          ch2 = c;
        }
        else
        {
          int index = Array.BinarySearch<char>(NotifHelpers.EmailLocalTranslateFromChars, c);
          ch2 = index < 0 || index >= NotifHelpers.EmailLocalTranslateToChars.Length ? '_' : NotifHelpers.EmailLocalTranslateToChars[index];
        }
        if ((int) ch2 != (int) ch1 || ch2 != '.' && ch2 != '_')
        {
          stringBuilder.Append(ch2);
          ch1 = ch2;
        }
      }
      return stringBuilder.ToString().Trim('.', '_');
    }

    public static string ReplaceNonSpaceWhitespace(this string s, char replaceWith = ' ')
    {
      if (s == null)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder(s.Length);
      foreach (char c in s)
        stringBuilder.Append(!char.IsWhiteSpace(c) || c == ' ' ? c : replaceWith);
      return stringBuilder.ToString();
    }

    private static void AppendToken(char separator, StringBuilder sb, object token)
    {
      string str = token.ToString();
      if (string.IsNullOrEmpty(str))
        return;
      if (sb.Length > 0)
        sb.Append(separator);
      sb.Append(str);
    }

    public static string SeparatorConcat(char separator, params object[] args)
    {
      StringBuilder sb = new StringBuilder();
      if (args != null && args.Length != 0)
      {
        foreach (object token1 in args)
        {
          if (token1 != null)
          {
            if (token1 is IEnumerable<object> objects)
            {
              foreach (object token2 in objects)
                NotifHelpers.AppendToken(separator, sb, token2);
            }
            else
              NotifHelpers.AppendToken(separator, sb, token1);
          }
        }
      }
      return sb.ToString();
    }

    public static string FormatNumberArray(int[] numbers)
    {
      if (numbers.Length == 0)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      Array.Sort<int>(numbers);
      int index = 0;
      do
      {
        int number1 = numbers[index];
        int num = numbers[index];
        for (++index; index < numbers.Length; ++index)
        {
          int number2 = numbers[index];
          if (number2 <= num + 1)
            num = number2;
          else
            break;
        }
        if (stringBuilder.Length > 0)
          stringBuilder.Append(", ");
        if (number1 == num)
          stringBuilder.Append(number1);
        else
          stringBuilder.Append(string.Format("{0}-{1}", (object) number1, (object) num));
      }
      while (index < numbers.Length);
      return stringBuilder.ToString();
    }

    public static string ToByteString(this string s, int maxChars = 256)
    {
      StringBuilder stringBuilder = new StringBuilder(maxChars * 5);
      int num = 0;
      foreach (char ch in s)
      {
        stringBuilder.Append(string.Format("{0:X4} ", (object) Convert.ToUInt16(ch)));
        if (++num >= maxChars)
          break;
      }
      return stringBuilder.ToString();
    }

    public static NotificationDeliveryData DeserializeDeliveryData(JObject data)
    {
      NotificationDeliveryData notificationDeliveryData = new NotificationDeliveryData();
      if (data != null)
      {
        JToken jtoken1 = data.GetValue("artifact");
        if (jtoken1 != null)
        {
          notificationDeliveryData.Artifact.FriendlyName = jtoken1.Value<string>((object) "friendlyName");
          notificationDeliveryData.Artifact.Id = jtoken1.Value<string>((object) "id");
          notificationDeliveryData.Artifact.Name = jtoken1.Value<string>((object) "name");
          notificationDeliveryData.Artifact.Tool = jtoken1.Value<string>((object) "tool");
          notificationDeliveryData.Artifact.Type = jtoken1.Value<string>((object) "type");
        }
        notificationDeliveryData.Action = data.Value<string>((object) "action");
        notificationDeliveryData.FromAddress = data.Value<string>((object) "fromAddress");
        JToken jtoken2 = data.GetValue("initiator");
        if (jtoken2 != null)
        {
          notificationDeliveryData.Initiator = new IdentityRef();
          notificationDeliveryData.Initiator.Id = jtoken2.Value<string>((object) "id");
          notificationDeliveryData.Initiator.DisplayName = jtoken2.Value<string>((object) "displayName");
        }
        JToken jtoken3 = data.GetValue("scopes");
        if (jtoken3 != null && jtoken3.HasValues)
        {
          foreach (JToken child in jtoken3.Children())
          {
            if (child != null && child.HasValues)
            {
              EventScope eventScope = new EventScope()
              {
                Name = child.Value<string>((object) "name"),
                Type = child.Value<string>((object) "type")
              };
              Guid result;
              if (Guid.TryParse(child.Value<string>((object) "id"), out result))
                eventScope.Id = result;
              notificationDeliveryData.AddScopes((object) eventScope);
            }
          }
        }
      }
      return notificationDeliveryData;
    }

    public static Guid GetEmailConversationId(
      NotificationDeliveryData extensionOptions,
      NotificationDeliveryData filterOptions)
    {
      byte[] numArray = new byte[16];
      return NotifHelpers.GetGuidFromTokens((IEnumerable<string>) new List<string>()
      {
        NotifHelpers.FirstNonEmptyString(extensionOptions.Artifact.Type, filterOptions.Artifact.Type),
        NotifHelpers.SelectScope('.', extensionOptions, filterOptions),
        NotifHelpers.SelectArtifactName(extensionOptions, filterOptions)
      });
    }

    public static Guid GetGuidFromTokens(IEnumerable<string> tokens)
    {
      byte[] b = new byte[16];
      int[] numArray = new int[4];
      int index1 = 0;
      int num1 = tokens != null ? tokens.Count<string>() : 0;
      int num2 = 140989193;
      if (num1 > 0)
      {
        foreach (string token in tokens)
        {
          int num3;
          if (!string.IsNullOrEmpty(token))
          {
            num3 = token.GetStableHashCode();
          }
          else
          {
            num3 = num2;
            num2 = num2 * 997 + 353;
          }
          numArray[index1++] += num3;
          if (index1 >= numArray.Length)
            index1 = 0;
        }
      }
      else
      {
        numArray[0] = num2;
        index1 = 1;
      }
      if (num1 < 4)
      {
        int index2 = 0;
        for (; index1 < 4; ++index1)
        {
          numArray[index1] = numArray[index2] * 53 + 11;
          ++index2;
        }
      }
      int index3 = 0;
      foreach (int num4 in numArray)
      {
        BitConverter.GetBytes(num4).CopyTo((Array) b, index3);
        index3 += 4;
      }
      return new Guid(b);
    }

    public static string SelectScope(
      char separator,
      NotificationDeliveryData extensionOptions,
      NotificationDeliveryData filterOptions)
    {
      string str = (string) null;
      List<EventScope> scopes = extensionOptions.Scopes;
      // ISSUE: explicit non-virtual call
      List<EventScope> eventScopeList = (scopes != null ? (__nonvirtual (scopes.Count) > 0 ? 1 : 0) : 0) != 0 ? extensionOptions.Scopes : filterOptions.Scopes;
      // ISSUE: explicit non-virtual call
      if (eventScopeList != null && __nonvirtual (eventScopeList.Count) > 0)
      {
        List<string> stringList = new List<string>();
        foreach (EventScope eventScope in eventScopeList)
        {
          if (eventScope != null)
          {
            string name = eventScope.Name;
            if (string.IsNullOrEmpty(name) && !eventScope.Id.Equals(Guid.Empty))
              name = eventScope.Id.ToString();
            if (!string.IsNullOrEmpty(name))
              stringList.Add(name);
          }
        }
        str = NotifHelpers.SeparatorConcat(separator, (object) stringList);
      }
      return str;
    }

    public static string SelectArtifactName(
      NotificationDeliveryData extensionOptions,
      NotificationDeliveryData filterOptions)
    {
      return NotifHelpers.FirstNonEmptyString(extensionOptions.Artifact.Name, extensionOptions.Artifact.Id, (object) filterOptions.Artifact.Name, (object) filterOptions.Artifact.Id) ?? Guid.NewGuid().ToString();
    }

    public static string SelectInitiator(
      NotificationDeliveryData extensionOptions,
      NotificationDeliveryData filterOptions)
    {
      return NotifHelpers.FirstNonEmptyString(extensionOptions.Initiator?.DisplayName, extensionOptions.Initiator?.Id, (object) filterOptions.Initiator?.DisplayName, (object) filterOptions.Initiator?.Id);
    }

    public static string GetEmailReferenceId(
      NotificationDeliveryData extensionOptions,
      NotificationDeliveryData filterOptions,
      string eventType)
    {
      return NotifHelpers.AsciifyString(NotifHelpers.SeparatorConcat('.', (object) NotifHelpers.FirstNonEmptyString(extensionOptions.Artifact.Type, filterOptions.Artifact.Type, (object) eventType), (object) NotifHelpers.SelectScope('.', extensionOptions, filterOptions), (object) NotifHelpers.SelectArtifactName(extensionOptions, filterOptions))).ReplaceNonEmailAddressLocalChars();
    }

    public static string GetEmailMessageId(
      NotificationDeliveryData extensionOptions,
      NotificationDeliveryData filterOptions,
      string referenceId)
    {
      return NotifHelpers.AsciifyString(NotifHelpers.SeparatorConcat('.', (object) referenceId, (object) NotifHelpers.FirstNonEmptyString(extensionOptions.Action, filterOptions.Action), (object) Guid.NewGuid())).ReplaceNonEmailAddressLocalChars();
    }

    public static DateTime RoundDownMinutes(this DateTime dateTime, int minuteInterval = 5) => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute / minuteInterval * minuteInterval, 0);

    public static bool IsAscii(string str)
    {
      foreach (char ch in str)
      {
        if (!NotifHelpers.IsAscii(ch) || NotifHelpers.IsControlChar(ch))
          return false;
      }
      return true;
    }

    public static bool IsAscii(char ch) => ch <= '\u007F';

    private static bool IsControlChar(char ch, bool stripNewlines = true)
    {
      if (!char.IsControl(ch))
        return false;
      if (!stripNewlines)
        return true;
      return ch != '\n' && ch != '\a';
    }

    public static string AsciifyString(string source, bool trim = true, bool stripNewlines = true)
    {
      string str1 = (trim ? source.Trim() : source).Normalize(NormalizationForm.FormC);
      StringBuilder stringBuilder = new StringBuilder(str1.Length);
      foreach (char ch1 in str1)
      {
        if (!NotifHelpers.IsControlChar(ch1, stripNewlines))
        {
          if (NotifHelpers.IsAscii(ch1))
          {
            stringBuilder.Append(ch1);
          }
          else
          {
            char ch2;
            if (NotifHelpers.AsciiCharCharMap.TryGetValue(ch1, out ch2))
            {
              stringBuilder.Append(ch2);
            }
            else
            {
              string str2;
              if (NotifHelpers.AsciiCharStringMap.TryGetValue(ch1, out str2))
                stringBuilder.Append(str2);
              else
                stringBuilder.Append(ch1);
            }
          }
        }
      }
      ASCIIEncoding asciiEncoding = new ASCIIEncoding();
      return asciiEncoding.GetString(asciiEncoding.GetBytes(stringBuilder.ToString()));
    }
  }
}
