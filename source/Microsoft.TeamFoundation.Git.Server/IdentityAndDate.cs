// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.IdentityAndDate
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Globalization;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class IdentityAndDate
  {
    private static readonly char[] c_invalidNameChars = new char[2]
    {
      '<',
      '>'
    };

    internal IdentityAndDate(
      string nameAndEmail,
      string name,
      string email,
      DateTime time,
      TimeSpan baseUtcOffset)
    {
      this.NameAndEmail = nameAndEmail;
      this.Name = name;
      this.Email = email;
      this.Time = time;
      this.BaseUtcOffset = baseUtcOffset;
    }

    public string NameAndEmail { get; }

    public string Name { get; }

    public string Email { get; }

    public DateTime Time { get; }

    public TimeSpan BaseUtcOffset { get; }

    public DateTime LocalTime => IdentityAndDate.ConvertToLocalTime(this.Time, this.BaseUtcOffset);

    internal static DateTime ConvertToLocalTime(DateTime utcTime, TimeSpan baseUtcOffset)
    {
      TimeZoneInfo customTimeZone = TimeZoneInfo.CreateCustomTimeZone("CustomTimeZone", baseUtcOffset, string.Empty, string.Empty);
      return TimeZoneInfo.ConvertTimeFromUtc(utcTime, customTimeZone);
    }

    internal static bool TryParse(string line, out IdentityAndDate result)
    {
      int num1 = line.LastIndexOf(' ');
      if (num1 > 0 && num1 != line.Length - 1)
      {
        string s1 = line.Substring(num1 + 1);
        int result1;
        if (s1 != null && (s1.Length == 5 || s1.Length == 7) && (s1[0] == '+' || s1[0] == '-') && int.TryParse(s1, NumberStyles.AllowLeadingSign, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result1))
        {
          if (s1.Length == 7)
            result1 /= 100;
          TimeSpan baseUtcOffset = TimeSpan.FromSeconds((double) (3600 * (result1 / 100) + 60 * (result1 % 100)));
          int startIndex = line.LastIndexOf(' ', num1 - 1, num1 - 1);
          if (startIndex >= 1)
          {
            string s2 = line.Substring(startIndex + 1, num1 - startIndex - 1);
            if (!string.IsNullOrEmpty(s2) && (s2[0] != '0' || s2.Length <= 1))
            {
              long result2;
              if (long.TryParse(s2, NumberStyles.None, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result2))
              {
                DateTime time;
                try
                {
                  time = GitServerConstants.UtcEpoch.AddSeconds((double) result2);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                  time = result2 <= 0L ? DateTime.MinValue : DateTime.MaxValue;
                }
                if (line[startIndex - 1] == '>')
                {
                  int num2 = line.LastIndexOf('<', startIndex);
                  if (num2 >= 0)
                  {
                    string email = line.Substring(num2 + 1, startIndex - num2 - 2);
                    if (email.IndexOf('>') < 0 && (num2 <= 0 || line[num2 - 1] == ' '))
                    {
                      string name = line.Substring(0, num2 == 0 ? 0 : num2 - 1);
                      if (string.IsNullOrEmpty(name) || name.IndexOfAny(IdentityAndDate.c_invalidNameChars) < 0)
                      {
                        string nameAndEmail = line.Remove(startIndex);
                        result = new IdentityAndDate(nameAndEmail, name, email, time, baseUtcOffset);
                        return true;
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
      result = (IdentityAndDate) null;
      return false;
    }

    internal static string CreateIdentityString(
      string name,
      string email,
      DateTime time,
      TimeSpan baseUtcOffset)
    {
      StringBuilder stringBuilder = new StringBuilder(name.Length + email.Length + 30);
      TimeSpan timeSpan = baseUtcOffset.Duration();
      long num = Math.Max((long) time.Subtract(GitServerConstants.UtcEpoch).TotalSeconds, 0L);
      string str = baseUtcOffset >= TimeSpan.Zero ? "+" : "-";
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0} <{1}> {2} {3}{4:D2}{5:D2}", (object) name, (object) email, (object) num, (object) str, (object) timeSpan.Hours, (object) timeSpan.Minutes);
      return stringBuilder.ToString();
    }

    internal int EstimatedSize => CacheUtil.ObjectOverhead + IntPtr.Size + CacheUtil.ObjectOverhead + this.NameAndEmail.Length * 2 + IntPtr.Size + CacheUtil.ObjectOverhead + this.Name.Length * 2 + IntPtr.Size + CacheUtil.ObjectOverhead + this.Email.Length * 2 + 8 + 8;
  }
}
