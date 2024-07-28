// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.NumberShortFormInfo
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  internal class NumberShortFormInfo
  {
    public string[] QuantitySymbols { get; private set; }

    public int NumberGroupSize { get; private set; }

    public string ThousandSymbol { get; private set; }

    public NumberShortFormInfo(CultureInfo cultureInfo)
    {
      string ietfLanguageTag = cultureInfo.Parent.IetfLanguageTag;
      if (ietfLanguageTag != null)
      {
        switch (ietfLanguageTag.Length)
        {
          case 2:
            switch (ietfLanguageTag[0])
            {
              case 'd':
                if (ietfLanguageTag == "de")
                {
                  this.QuantitySymbols = new string[3]
                  {
                    " Tsd.",
                    " Mio.",
                    " Mrd."
                  };
                  this.NumberGroupSize = 1000;
                  this.ThousandSymbol = this.QuantitySymbols[0];
                  return;
                }
                break;
              case 'e':
                if (ietfLanguageTag == "es")
                {
                  this.QuantitySymbols = new string[2]
                  {
                    " K",
                    " M"
                  };
                  this.NumberGroupSize = 1000;
                  this.ThousandSymbol = this.QuantitySymbols[0];
                  return;
                }
                break;
              case 'f':
                if (ietfLanguageTag == "fr")
                {
                  this.QuantitySymbols = new string[3]
                  {
                    " k",
                    " M",
                    " Md"
                  };
                  this.NumberGroupSize = 1000;
                  this.ThousandSymbol = this.QuantitySymbols[0];
                  return;
                }
                break;
              case 'i':
                if (ietfLanguageTag == "it")
                {
                  this.QuantitySymbols = new string[3]
                  {
                    " mila",
                    " Mln",
                    " Mld"
                  };
                  this.NumberGroupSize = 1000;
                  this.ThousandSymbol = this.QuantitySymbols[0];
                  return;
                }
                break;
              case 'j':
                if (ietfLanguageTag == "ja")
                {
                  this.QuantitySymbols = new string[2]
                  {
                    "万",
                    "億"
                  };
                  this.NumberGroupSize = 10000;
                  this.ThousandSymbol = "千";
                  return;
                }
                break;
              case 'k':
                if (ietfLanguageTag == "ko")
                {
                  this.QuantitySymbols = new string[2]
                  {
                    "만",
                    "억"
                  };
                  this.NumberGroupSize = 10000;
                  this.ThousandSymbol = "천";
                  return;
                }
                break;
              case 'r':
                if (ietfLanguageTag == "ru")
                {
                  this.QuantitySymbols = new string[3]
                  {
                    " тыс.",
                    " млн",
                    " млрд"
                  };
                  this.NumberGroupSize = 1000;
                  this.ThousandSymbol = this.QuantitySymbols[0];
                  return;
                }
                break;
              case 't':
                if (ietfLanguageTag == "tr")
                {
                  this.QuantitySymbols = new string[3]
                  {
                    " B",
                    " Mn",
                    " Mr"
                  };
                  this.NumberGroupSize = 1000;
                  this.ThousandSymbol = this.QuantitySymbols[0];
                  return;
                }
                break;
            }
            break;
          case 7:
            switch (ietfLanguageTag[6])
            {
              case 's':
                if (ietfLanguageTag == "zh-Hans")
                {
                  this.QuantitySymbols = new string[2]
                  {
                    "万",
                    "亿"
                  };
                  this.NumberGroupSize = 10000;
                  this.ThousandSymbol = "千";
                  return;
                }
                break;
              case 't':
                if (ietfLanguageTag == "zh-Hant")
                {
                  this.QuantitySymbols = new string[2]
                  {
                    "萬",
                    "億"
                  };
                  this.NumberGroupSize = 10000;
                  this.ThousandSymbol = "千";
                  return;
                }
                break;
            }
            break;
        }
      }
      this.QuantitySymbols = new string[3]{ "K", "M", "B" };
      this.NumberGroupSize = 1000;
      this.ThousandSymbol = this.QuantitySymbols[0];
    }
  }
}
