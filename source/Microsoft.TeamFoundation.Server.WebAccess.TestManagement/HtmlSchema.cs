// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.HtmlSchema
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System.Collections;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal class HtmlSchema
  {
    private static ArrayList _htmlInlineElements;
    private static ArrayList _htmlBlockElements;
    private static ArrayList _htmlOtherOpenableElements;
    private static ArrayList _htmlEmptyElements;
    private static ArrayList _htmlElementsClosingOnParentElementEnd;
    private static ArrayList _htmlElementsClosingColgroup;
    private static ArrayList _htmlElementsClosingDD;
    private static ArrayList _htmlElementsClosingDT;
    private static ArrayList _htmlElementsClosingLI;
    private static ArrayList _htmlElementsClosingTbody;
    private static ArrayList _htmlElementsClosingTD;
    private static ArrayList _htmlElementsClosingTfoot;
    private static ArrayList _htmlElementsClosingThead;
    private static ArrayList _htmlElementsClosingTH;
    private static ArrayList _htmlElementsClosingTR;
    private static Hashtable _htmlCharacterEntities;

    static HtmlSchema()
    {
      HtmlSchema.InitializeInlineElements();
      HtmlSchema.InitializeBlockElements();
      HtmlSchema.InitializeOtherOpenableElements();
      HtmlSchema.InitializeEmptyElements();
      HtmlSchema.InitializeElementsClosingOnParentElementEnd();
      HtmlSchema.InitializeElementsClosingOnNewElementStart();
      HtmlSchema.InitializeHtmlCharacterEntities();
    }

    internal static bool IsEmptyElement(string xmlElementName) => HtmlSchema._htmlEmptyElements.Contains((object) xmlElementName.ToLowerInvariant());

    internal static bool IsBlockElement(string xmlElementName) => HtmlSchema._htmlBlockElements.Contains((object) xmlElementName);

    internal static bool IsInlineElement(string xmlElementName) => HtmlSchema._htmlInlineElements.Contains((object) xmlElementName);

    internal static bool IsKnownOpenableElement(string xmlElementName) => HtmlSchema._htmlOtherOpenableElements.Contains((object) xmlElementName);

    internal static bool ClosesOnParentElementEnd(string xmlElementName) => HtmlSchema._htmlElementsClosingOnParentElementEnd.Contains((object) xmlElementName.ToLowerInvariant());

    internal static bool ClosesOnNextElementStart(string currentElementName, string nextElementName)
    {
      if (currentElementName != null)
      {
        switch (currentElementName.Length)
        {
          case 1:
            if (currentElementName == "p")
              return HtmlSchema.IsBlockElement(nextElementName);
            break;
          case 2:
            switch (currentElementName[1])
            {
              case 'd':
                switch (currentElementName)
                {
                  case "dd":
                    return HtmlSchema._htmlElementsClosingDD.Contains((object) nextElementName) && HtmlSchema.IsBlockElement(nextElementName);
                  case "td":
                    return HtmlSchema._htmlElementsClosingTD.Contains((object) nextElementName);
                }
                break;
              case 'h':
                if (currentElementName == "th")
                  return HtmlSchema._htmlElementsClosingTH.Contains((object) nextElementName);
                break;
              case 'i':
                if (currentElementName == "li")
                  return HtmlSchema._htmlElementsClosingLI.Contains((object) nextElementName);
                break;
              case 'r':
                if (currentElementName == "tr")
                  return HtmlSchema._htmlElementsClosingTR.Contains((object) nextElementName);
                break;
              case 't':
                if (currentElementName == "dt" && HtmlSchema._htmlElementsClosingDT.Contains((object) nextElementName))
                  return HtmlSchema.IsBlockElement(nextElementName);
                break;
            }
            break;
          case 5:
            switch (currentElementName[1])
            {
              case 'b':
                if (currentElementName == "tbody")
                  return HtmlSchema._htmlElementsClosingTbody.Contains((object) nextElementName);
                break;
              case 'f':
                if (currentElementName == "tfoot")
                  return HtmlSchema._htmlElementsClosingTfoot.Contains((object) nextElementName);
                break;
              case 'h':
                if (currentElementName == "thead")
                  return HtmlSchema._htmlElementsClosingThead.Contains((object) nextElementName);
                break;
            }
            break;
          case 8:
            if (currentElementName == "colgroup" && HtmlSchema._htmlElementsClosingColgroup.Contains((object) nextElementName))
              return HtmlSchema.IsBlockElement(nextElementName);
            break;
        }
      }
      return false;
    }

    internal static bool IsEntity(string entityName) => HtmlSchema._htmlCharacterEntities.Contains((object) entityName);

    internal static char EntityCharacterValue(string entityName) => HtmlSchema._htmlCharacterEntities.Contains((object) entityName) ? (char) HtmlSchema._htmlCharacterEntities[(object) entityName] : char.MinValue;

    private static void InitializeInlineElements()
    {
      HtmlSchema._htmlInlineElements = new ArrayList();
      HtmlSchema._htmlInlineElements.Add((object) "a");
      HtmlSchema._htmlInlineElements.Add((object) "abbr");
      HtmlSchema._htmlInlineElements.Add((object) "acronym");
      HtmlSchema._htmlInlineElements.Add((object) "address");
      HtmlSchema._htmlInlineElements.Add((object) "b");
      HtmlSchema._htmlInlineElements.Add((object) "bdo");
      HtmlSchema._htmlInlineElements.Add((object) "big");
      HtmlSchema._htmlInlineElements.Add((object) "button");
      HtmlSchema._htmlInlineElements.Add((object) "code");
      HtmlSchema._htmlInlineElements.Add((object) "del");
      HtmlSchema._htmlInlineElements.Add((object) "dfn");
      HtmlSchema._htmlInlineElements.Add((object) "em");
      HtmlSchema._htmlInlineElements.Add((object) "font");
      HtmlSchema._htmlInlineElements.Add((object) "i");
      HtmlSchema._htmlInlineElements.Add((object) "ins");
      HtmlSchema._htmlInlineElements.Add((object) "kbd");
      HtmlSchema._htmlInlineElements.Add((object) "label");
      HtmlSchema._htmlInlineElements.Add((object) "legend");
      HtmlSchema._htmlInlineElements.Add((object) "q");
      HtmlSchema._htmlInlineElements.Add((object) "s");
      HtmlSchema._htmlInlineElements.Add((object) "samp");
      HtmlSchema._htmlInlineElements.Add((object) "small");
      HtmlSchema._htmlInlineElements.Add((object) "span");
      HtmlSchema._htmlInlineElements.Add((object) "strike");
      HtmlSchema._htmlInlineElements.Add((object) "strong");
      HtmlSchema._htmlInlineElements.Add((object) "sub");
      HtmlSchema._htmlInlineElements.Add((object) "sup");
      HtmlSchema._htmlInlineElements.Add((object) "u");
      HtmlSchema._htmlInlineElements.Add((object) "var");
    }

    private static void InitializeBlockElements()
    {
      HtmlSchema._htmlBlockElements = new ArrayList();
      HtmlSchema._htmlBlockElements.Add((object) "blockquote");
      HtmlSchema._htmlBlockElements.Add((object) "body");
      HtmlSchema._htmlBlockElements.Add((object) "caption");
      HtmlSchema._htmlBlockElements.Add((object) "center");
      HtmlSchema._htmlBlockElements.Add((object) "cite");
      HtmlSchema._htmlBlockElements.Add((object) "dd");
      HtmlSchema._htmlBlockElements.Add((object) "dir");
      HtmlSchema._htmlBlockElements.Add((object) "div");
      HtmlSchema._htmlBlockElements.Add((object) "dl");
      HtmlSchema._htmlBlockElements.Add((object) "dt");
      HtmlSchema._htmlBlockElements.Add((object) "form");
      HtmlSchema._htmlBlockElements.Add((object) "h1");
      HtmlSchema._htmlBlockElements.Add((object) "h2");
      HtmlSchema._htmlBlockElements.Add((object) "h3");
      HtmlSchema._htmlBlockElements.Add((object) "h4");
      HtmlSchema._htmlBlockElements.Add((object) "h5");
      HtmlSchema._htmlBlockElements.Add((object) "h6");
      HtmlSchema._htmlBlockElements.Add((object) "html");
      HtmlSchema._htmlBlockElements.Add((object) "li");
      HtmlSchema._htmlBlockElements.Add((object) "menu");
      HtmlSchema._htmlBlockElements.Add((object) "ol");
      HtmlSchema._htmlBlockElements.Add((object) "p");
      HtmlSchema._htmlBlockElements.Add((object) "pre");
      HtmlSchema._htmlBlockElements.Add((object) "table");
      HtmlSchema._htmlBlockElements.Add((object) "tbody");
      HtmlSchema._htmlBlockElements.Add((object) "td");
      HtmlSchema._htmlBlockElements.Add((object) "textarea");
      HtmlSchema._htmlBlockElements.Add((object) "tfoot");
      HtmlSchema._htmlBlockElements.Add((object) "th");
      HtmlSchema._htmlBlockElements.Add((object) "thead");
      HtmlSchema._htmlBlockElements.Add((object) "tr");
      HtmlSchema._htmlBlockElements.Add((object) "tt");
      HtmlSchema._htmlBlockElements.Add((object) "ul");
    }

    private static void InitializeEmptyElements()
    {
      HtmlSchema._htmlEmptyElements = new ArrayList();
      HtmlSchema._htmlEmptyElements.Add((object) "area");
      HtmlSchema._htmlEmptyElements.Add((object) "base");
      HtmlSchema._htmlEmptyElements.Add((object) "basefont");
      HtmlSchema._htmlEmptyElements.Add((object) "br");
      HtmlSchema._htmlEmptyElements.Add((object) "col");
      HtmlSchema._htmlEmptyElements.Add((object) "frame");
      HtmlSchema._htmlEmptyElements.Add((object) "hr");
      HtmlSchema._htmlEmptyElements.Add((object) "img");
      HtmlSchema._htmlEmptyElements.Add((object) "input");
      HtmlSchema._htmlEmptyElements.Add((object) "isindex");
      HtmlSchema._htmlEmptyElements.Add((object) "link");
      HtmlSchema._htmlEmptyElements.Add((object) "meta");
      HtmlSchema._htmlEmptyElements.Add((object) "param");
    }

    private static void InitializeOtherOpenableElements()
    {
      HtmlSchema._htmlOtherOpenableElements = new ArrayList();
      HtmlSchema._htmlOtherOpenableElements.Add((object) "applet");
      HtmlSchema._htmlOtherOpenableElements.Add((object) "base");
      HtmlSchema._htmlOtherOpenableElements.Add((object) "basefont");
      HtmlSchema._htmlOtherOpenableElements.Add((object) "colgroup");
      HtmlSchema._htmlOtherOpenableElements.Add((object) "fieldset");
      HtmlSchema._htmlOtherOpenableElements.Add((object) "frameset");
      HtmlSchema._htmlOtherOpenableElements.Add((object) "head");
      HtmlSchema._htmlOtherOpenableElements.Add((object) "iframe");
      HtmlSchema._htmlOtherOpenableElements.Add((object) "map");
      HtmlSchema._htmlOtherOpenableElements.Add((object) "noframes");
      HtmlSchema._htmlOtherOpenableElements.Add((object) "noscript");
      HtmlSchema._htmlOtherOpenableElements.Add((object) "object");
      HtmlSchema._htmlOtherOpenableElements.Add((object) "optgroup");
      HtmlSchema._htmlOtherOpenableElements.Add((object) "option");
      HtmlSchema._htmlOtherOpenableElements.Add((object) "script");
      HtmlSchema._htmlOtherOpenableElements.Add((object) "select");
      HtmlSchema._htmlOtherOpenableElements.Add((object) "style");
      HtmlSchema._htmlOtherOpenableElements.Add((object) "title");
    }

    private static void InitializeElementsClosingOnParentElementEnd()
    {
      HtmlSchema._htmlElementsClosingOnParentElementEnd = new ArrayList();
      HtmlSchema._htmlElementsClosingOnParentElementEnd.Add((object) "body");
      HtmlSchema._htmlElementsClosingOnParentElementEnd.Add((object) "colgroup");
      HtmlSchema._htmlElementsClosingOnParentElementEnd.Add((object) "dd");
      HtmlSchema._htmlElementsClosingOnParentElementEnd.Add((object) "dt");
      HtmlSchema._htmlElementsClosingOnParentElementEnd.Add((object) "head");
      HtmlSchema._htmlElementsClosingOnParentElementEnd.Add((object) "html");
      HtmlSchema._htmlElementsClosingOnParentElementEnd.Add((object) "li");
      HtmlSchema._htmlElementsClosingOnParentElementEnd.Add((object) "p");
      HtmlSchema._htmlElementsClosingOnParentElementEnd.Add((object) "tbody");
      HtmlSchema._htmlElementsClosingOnParentElementEnd.Add((object) "td");
      HtmlSchema._htmlElementsClosingOnParentElementEnd.Add((object) "tfoot");
      HtmlSchema._htmlElementsClosingOnParentElementEnd.Add((object) "thead");
      HtmlSchema._htmlElementsClosingOnParentElementEnd.Add((object) "th");
      HtmlSchema._htmlElementsClosingOnParentElementEnd.Add((object) "tr");
    }

    private static void InitializeElementsClosingOnNewElementStart()
    {
      HtmlSchema._htmlElementsClosingColgroup = new ArrayList();
      HtmlSchema._htmlElementsClosingColgroup.Add((object) "colgroup");
      HtmlSchema._htmlElementsClosingColgroup.Add((object) "tr");
      HtmlSchema._htmlElementsClosingColgroup.Add((object) "thead");
      HtmlSchema._htmlElementsClosingColgroup.Add((object) "tfoot");
      HtmlSchema._htmlElementsClosingColgroup.Add((object) "tbody");
      HtmlSchema._htmlElementsClosingDD = new ArrayList();
      HtmlSchema._htmlElementsClosingDD.Add((object) "dd");
      HtmlSchema._htmlElementsClosingDD.Add((object) "dt");
      HtmlSchema._htmlElementsClosingDT = new ArrayList();
      HtmlSchema._htmlElementsClosingDD.Add((object) "dd");
      HtmlSchema._htmlElementsClosingDD.Add((object) "dt");
      HtmlSchema._htmlElementsClosingLI = new ArrayList();
      HtmlSchema._htmlElementsClosingLI.Add((object) "li");
      HtmlSchema._htmlElementsClosingTbody = new ArrayList();
      HtmlSchema._htmlElementsClosingTbody.Add((object) "tbody");
      HtmlSchema._htmlElementsClosingTbody.Add((object) "thead");
      HtmlSchema._htmlElementsClosingTbody.Add((object) "tfoot");
      HtmlSchema._htmlElementsClosingTR = new ArrayList();
      HtmlSchema._htmlElementsClosingTR.Add((object) "thead");
      HtmlSchema._htmlElementsClosingTR.Add((object) "tfoot");
      HtmlSchema._htmlElementsClosingTR.Add((object) "tbody");
      HtmlSchema._htmlElementsClosingTR.Add((object) "tr");
      HtmlSchema._htmlElementsClosingTD = new ArrayList();
      HtmlSchema._htmlElementsClosingTD.Add((object) "td");
      HtmlSchema._htmlElementsClosingTD.Add((object) "th");
      HtmlSchema._htmlElementsClosingTD.Add((object) "tr");
      HtmlSchema._htmlElementsClosingTD.Add((object) "tbody");
      HtmlSchema._htmlElementsClosingTD.Add((object) "tfoot");
      HtmlSchema._htmlElementsClosingTD.Add((object) "thead");
      HtmlSchema._htmlElementsClosingTH = new ArrayList();
      HtmlSchema._htmlElementsClosingTH.Add((object) "td");
      HtmlSchema._htmlElementsClosingTH.Add((object) "th");
      HtmlSchema._htmlElementsClosingTH.Add((object) "tr");
      HtmlSchema._htmlElementsClosingTH.Add((object) "tbody");
      HtmlSchema._htmlElementsClosingTH.Add((object) "tfoot");
      HtmlSchema._htmlElementsClosingTH.Add((object) "thead");
      HtmlSchema._htmlElementsClosingThead = new ArrayList();
      HtmlSchema._htmlElementsClosingThead.Add((object) "tbody");
      HtmlSchema._htmlElementsClosingThead.Add((object) "tfoot");
      HtmlSchema._htmlElementsClosingTfoot = new ArrayList();
      HtmlSchema._htmlElementsClosingTfoot.Add((object) "tbody");
      HtmlSchema._htmlElementsClosingTfoot.Add((object) "thead");
    }

    private static void InitializeHtmlCharacterEntities()
    {
      HtmlSchema._htmlCharacterEntities = new Hashtable();
      HtmlSchema._htmlCharacterEntities[(object) "Aacute"] = (object) 'Á';
      HtmlSchema._htmlCharacterEntities[(object) "aacute"] = (object) 'á';
      HtmlSchema._htmlCharacterEntities[(object) "Acirc"] = (object) 'Â';
      HtmlSchema._htmlCharacterEntities[(object) "acirc"] = (object) 'â';
      HtmlSchema._htmlCharacterEntities[(object) "acute"] = (object) '´';
      HtmlSchema._htmlCharacterEntities[(object) "AElig"] = (object) 'Æ';
      HtmlSchema._htmlCharacterEntities[(object) "aelig"] = (object) 'æ';
      HtmlSchema._htmlCharacterEntities[(object) "Agrave"] = (object) 'À';
      HtmlSchema._htmlCharacterEntities[(object) "agrave"] = (object) 'à';
      HtmlSchema._htmlCharacterEntities[(object) "alefsym"] = (object) 'ℵ';
      HtmlSchema._htmlCharacterEntities[(object) "Alpha"] = (object) 'Α';
      HtmlSchema._htmlCharacterEntities[(object) "alpha"] = (object) 'α';
      HtmlSchema._htmlCharacterEntities[(object) "amp"] = (object) '&';
      HtmlSchema._htmlCharacterEntities[(object) "and"] = (object) '∧';
      HtmlSchema._htmlCharacterEntities[(object) "ang"] = (object) '∠';
      HtmlSchema._htmlCharacterEntities[(object) "Aring"] = (object) 'Å';
      HtmlSchema._htmlCharacterEntities[(object) "aring"] = (object) 'å';
      HtmlSchema._htmlCharacterEntities[(object) "asymp"] = (object) '≈';
      HtmlSchema._htmlCharacterEntities[(object) "Atilde"] = (object) 'Ã';
      HtmlSchema._htmlCharacterEntities[(object) "atilde"] = (object) 'ã';
      HtmlSchema._htmlCharacterEntities[(object) "Auml"] = (object) 'Ä';
      HtmlSchema._htmlCharacterEntities[(object) "auml"] = (object) 'ä';
      HtmlSchema._htmlCharacterEntities[(object) "bdquo"] = (object) '„';
      HtmlSchema._htmlCharacterEntities[(object) "Beta"] = (object) 'Β';
      HtmlSchema._htmlCharacterEntities[(object) "beta"] = (object) 'β';
      HtmlSchema._htmlCharacterEntities[(object) "brvbar"] = (object) '¦';
      HtmlSchema._htmlCharacterEntities[(object) "bull"] = (object) '•';
      HtmlSchema._htmlCharacterEntities[(object) "cap"] = (object) '∩';
      HtmlSchema._htmlCharacterEntities[(object) "Ccedil"] = (object) 'Ç';
      HtmlSchema._htmlCharacterEntities[(object) "ccedil"] = (object) 'ç';
      HtmlSchema._htmlCharacterEntities[(object) "cent"] = (object) '¢';
      HtmlSchema._htmlCharacterEntities[(object) "Chi"] = (object) 'Χ';
      HtmlSchema._htmlCharacterEntities[(object) "chi"] = (object) 'χ';
      HtmlSchema._htmlCharacterEntities[(object) "circ"] = (object) 'ˆ';
      HtmlSchema._htmlCharacterEntities[(object) "clubs"] = (object) '♣';
      HtmlSchema._htmlCharacterEntities[(object) "cong"] = (object) '≅';
      HtmlSchema._htmlCharacterEntities[(object) "copy"] = (object) '©';
      HtmlSchema._htmlCharacterEntities[(object) "crarr"] = (object) '↵';
      HtmlSchema._htmlCharacterEntities[(object) "cup"] = (object) '∪';
      HtmlSchema._htmlCharacterEntities[(object) "curren"] = (object) '¤';
      HtmlSchema._htmlCharacterEntities[(object) "dagger"] = (object) '†';
      HtmlSchema._htmlCharacterEntities[(object) "Dagger"] = (object) '‡';
      HtmlSchema._htmlCharacterEntities[(object) "darr"] = (object) '↓';
      HtmlSchema._htmlCharacterEntities[(object) "dArr"] = (object) '⇓';
      HtmlSchema._htmlCharacterEntities[(object) "deg"] = (object) '°';
      HtmlSchema._htmlCharacterEntities[(object) "Delta"] = (object) 'Δ';
      HtmlSchema._htmlCharacterEntities[(object) "delta"] = (object) 'δ';
      HtmlSchema._htmlCharacterEntities[(object) "diams"] = (object) '♦';
      HtmlSchema._htmlCharacterEntities[(object) "divide"] = (object) '÷';
      HtmlSchema._htmlCharacterEntities[(object) "Eacute"] = (object) 'É';
      HtmlSchema._htmlCharacterEntities[(object) "eacute"] = (object) 'é';
      HtmlSchema._htmlCharacterEntities[(object) "Ecirc"] = (object) 'Ê';
      HtmlSchema._htmlCharacterEntities[(object) "ecirc"] = (object) 'ê';
      HtmlSchema._htmlCharacterEntities[(object) "Egrave"] = (object) 'È';
      HtmlSchema._htmlCharacterEntities[(object) "egrave"] = (object) 'è';
      HtmlSchema._htmlCharacterEntities[(object) "empty"] = (object) '∅';
      HtmlSchema._htmlCharacterEntities[(object) "emsp"] = (object) ' ';
      HtmlSchema._htmlCharacterEntities[(object) "ensp"] = (object) ' ';
      HtmlSchema._htmlCharacterEntities[(object) "Epsilon"] = (object) 'Ε';
      HtmlSchema._htmlCharacterEntities[(object) "epsilon"] = (object) 'ε';
      HtmlSchema._htmlCharacterEntities[(object) "equiv"] = (object) '≡';
      HtmlSchema._htmlCharacterEntities[(object) "Eta"] = (object) 'Η';
      HtmlSchema._htmlCharacterEntities[(object) "eta"] = (object) 'η';
      HtmlSchema._htmlCharacterEntities[(object) "ETH"] = (object) 'Ð';
      HtmlSchema._htmlCharacterEntities[(object) "eth"] = (object) 'ð';
      HtmlSchema._htmlCharacterEntities[(object) "Euml"] = (object) 'Ë';
      HtmlSchema._htmlCharacterEntities[(object) "euml"] = (object) 'ë';
      HtmlSchema._htmlCharacterEntities[(object) "euro"] = (object) '€';
      HtmlSchema._htmlCharacterEntities[(object) "exist"] = (object) '∃';
      HtmlSchema._htmlCharacterEntities[(object) "fnof"] = (object) 'ƒ';
      HtmlSchema._htmlCharacterEntities[(object) "forall"] = (object) '∀';
      HtmlSchema._htmlCharacterEntities[(object) "frac12"] = (object) '\u00BD';
      HtmlSchema._htmlCharacterEntities[(object) "frac14"] = (object) '\u00BC';
      HtmlSchema._htmlCharacterEntities[(object) "frac34"] = (object) '\u00BE';
      HtmlSchema._htmlCharacterEntities[(object) "frasl"] = (object) '⁄';
      HtmlSchema._htmlCharacterEntities[(object) "Gamma"] = (object) 'Γ';
      HtmlSchema._htmlCharacterEntities[(object) "gamma"] = (object) 'γ';
      HtmlSchema._htmlCharacterEntities[(object) "ge"] = (object) '≥';
      HtmlSchema._htmlCharacterEntities[(object) "gt"] = (object) '>';
      HtmlSchema._htmlCharacterEntities[(object) "harr"] = (object) '↔';
      HtmlSchema._htmlCharacterEntities[(object) "hArr"] = (object) '⇔';
      HtmlSchema._htmlCharacterEntities[(object) "hearts"] = (object) '♥';
      HtmlSchema._htmlCharacterEntities[(object) "hellip"] = (object) '…';
      HtmlSchema._htmlCharacterEntities[(object) "Iacute"] = (object) 'Í';
      HtmlSchema._htmlCharacterEntities[(object) "iacute"] = (object) 'í';
      HtmlSchema._htmlCharacterEntities[(object) "Icirc"] = (object) 'Î';
      HtmlSchema._htmlCharacterEntities[(object) "icirc"] = (object) 'î';
      HtmlSchema._htmlCharacterEntities[(object) "iexcl"] = (object) '¡';
      HtmlSchema._htmlCharacterEntities[(object) "Igrave"] = (object) 'Ì';
      HtmlSchema._htmlCharacterEntities[(object) "igrave"] = (object) 'ì';
      HtmlSchema._htmlCharacterEntities[(object) "image"] = (object) 'ℑ';
      HtmlSchema._htmlCharacterEntities[(object) "infin"] = (object) '∞';
      HtmlSchema._htmlCharacterEntities[(object) "int"] = (object) '∫';
      HtmlSchema._htmlCharacterEntities[(object) "Iota"] = (object) 'Ι';
      HtmlSchema._htmlCharacterEntities[(object) "iota"] = (object) 'ι';
      HtmlSchema._htmlCharacterEntities[(object) "iquest"] = (object) '¿';
      HtmlSchema._htmlCharacterEntities[(object) "isin"] = (object) '∈';
      HtmlSchema._htmlCharacterEntities[(object) "Iuml"] = (object) 'Ï';
      HtmlSchema._htmlCharacterEntities[(object) "iuml"] = (object) 'ï';
      HtmlSchema._htmlCharacterEntities[(object) "Kappa"] = (object) 'Κ';
      HtmlSchema._htmlCharacterEntities[(object) "kappa"] = (object) 'κ';
      HtmlSchema._htmlCharacterEntities[(object) "Lambda"] = (object) 'Λ';
      HtmlSchema._htmlCharacterEntities[(object) "lambda"] = (object) 'λ';
      HtmlSchema._htmlCharacterEntities[(object) "lang"] = (object) '〈';
      HtmlSchema._htmlCharacterEntities[(object) "laquo"] = (object) '«';
      HtmlSchema._htmlCharacterEntities[(object) "larr"] = (object) '←';
      HtmlSchema._htmlCharacterEntities[(object) "lArr"] = (object) '⇐';
      HtmlSchema._htmlCharacterEntities[(object) "lceil"] = (object) '⌈';
      HtmlSchema._htmlCharacterEntities[(object) "ldquo"] = (object) '“';
      HtmlSchema._htmlCharacterEntities[(object) "le"] = (object) '≤';
      HtmlSchema._htmlCharacterEntities[(object) "lfloor"] = (object) '⌊';
      HtmlSchema._htmlCharacterEntities[(object) "lowast"] = (object) '∗';
      HtmlSchema._htmlCharacterEntities[(object) "loz"] = (object) '◊';
      HtmlSchema._htmlCharacterEntities[(object) "lrm"] = (object) '\u200E';
      HtmlSchema._htmlCharacterEntities[(object) "lsaquo"] = (object) '‹';
      HtmlSchema._htmlCharacterEntities[(object) "lsquo"] = (object) '‘';
      HtmlSchema._htmlCharacterEntities[(object) "lt"] = (object) '<';
      HtmlSchema._htmlCharacterEntities[(object) "macr"] = (object) '¯';
      HtmlSchema._htmlCharacterEntities[(object) "mdash"] = (object) '—';
      HtmlSchema._htmlCharacterEntities[(object) "micro"] = (object) 'µ';
      HtmlSchema._htmlCharacterEntities[(object) "middot"] = (object) '·';
      HtmlSchema._htmlCharacterEntities[(object) "minus"] = (object) '−';
      HtmlSchema._htmlCharacterEntities[(object) "Mu"] = (object) 'Μ';
      HtmlSchema._htmlCharacterEntities[(object) "mu"] = (object) 'μ';
      HtmlSchema._htmlCharacterEntities[(object) "nabla"] = (object) '∇';
      HtmlSchema._htmlCharacterEntities[(object) "nbsp"] = (object) ' ';
      HtmlSchema._htmlCharacterEntities[(object) "ndash"] = (object) '–';
      HtmlSchema._htmlCharacterEntities[(object) "ne"] = (object) '≠';
      HtmlSchema._htmlCharacterEntities[(object) "ni"] = (object) '∋';
      HtmlSchema._htmlCharacterEntities[(object) "not"] = (object) '¬';
      HtmlSchema._htmlCharacterEntities[(object) "notin"] = (object) '∉';
      HtmlSchema._htmlCharacterEntities[(object) "nsub"] = (object) '⊄';
      HtmlSchema._htmlCharacterEntities[(object) "Ntilde"] = (object) 'Ñ';
      HtmlSchema._htmlCharacterEntities[(object) "ntilde"] = (object) 'ñ';
      HtmlSchema._htmlCharacterEntities[(object) "Nu"] = (object) 'Ν';
      HtmlSchema._htmlCharacterEntities[(object) "nu"] = (object) 'ν';
      HtmlSchema._htmlCharacterEntities[(object) "Oacute"] = (object) 'Ó';
      HtmlSchema._htmlCharacterEntities[(object) "ocirc"] = (object) 'ô';
      HtmlSchema._htmlCharacterEntities[(object) "OElig"] = (object) 'Œ';
      HtmlSchema._htmlCharacterEntities[(object) "oelig"] = (object) 'œ';
      HtmlSchema._htmlCharacterEntities[(object) "Ograve"] = (object) 'Ò';
      HtmlSchema._htmlCharacterEntities[(object) "ograve"] = (object) 'ò';
      HtmlSchema._htmlCharacterEntities[(object) "oline"] = (object) '‾';
      HtmlSchema._htmlCharacterEntities[(object) "Omega"] = (object) 'Ω';
      HtmlSchema._htmlCharacterEntities[(object) "omega"] = (object) 'ω';
      HtmlSchema._htmlCharacterEntities[(object) "Omicron"] = (object) 'Ο';
      HtmlSchema._htmlCharacterEntities[(object) "omicron"] = (object) 'ο';
      HtmlSchema._htmlCharacterEntities[(object) "oplus"] = (object) '⊕';
      HtmlSchema._htmlCharacterEntities[(object) "or"] = (object) '∨';
      HtmlSchema._htmlCharacterEntities[(object) "ordf"] = (object) 'ª';
      HtmlSchema._htmlCharacterEntities[(object) "ordm"] = (object) 'º';
      HtmlSchema._htmlCharacterEntities[(object) "Oslash"] = (object) 'Ø';
      HtmlSchema._htmlCharacterEntities[(object) "oslash"] = (object) 'ø';
      HtmlSchema._htmlCharacterEntities[(object) "Otilde"] = (object) 'Õ';
      HtmlSchema._htmlCharacterEntities[(object) "otilde"] = (object) 'õ';
      HtmlSchema._htmlCharacterEntities[(object) "otimes"] = (object) '⊗';
      HtmlSchema._htmlCharacterEntities[(object) "Ouml"] = (object) 'Ö';
      HtmlSchema._htmlCharacterEntities[(object) "ouml"] = (object) 'ö';
      HtmlSchema._htmlCharacterEntities[(object) "para"] = (object) '¶';
      HtmlSchema._htmlCharacterEntities[(object) "part"] = (object) '∂';
      HtmlSchema._htmlCharacterEntities[(object) "permil"] = (object) '‰';
      HtmlSchema._htmlCharacterEntities[(object) "perp"] = (object) '⊥';
      HtmlSchema._htmlCharacterEntities[(object) "Phi"] = (object) 'Φ';
      HtmlSchema._htmlCharacterEntities[(object) "phi"] = (object) 'φ';
      HtmlSchema._htmlCharacterEntities[(object) "pi"] = (object) 'π';
      HtmlSchema._htmlCharacterEntities[(object) "piv"] = (object) 'ϖ';
      HtmlSchema._htmlCharacterEntities[(object) "plusmn"] = (object) '±';
      HtmlSchema._htmlCharacterEntities[(object) "pound"] = (object) '£';
      HtmlSchema._htmlCharacterEntities[(object) "prime"] = (object) '′';
      HtmlSchema._htmlCharacterEntities[(object) "Prime"] = (object) '″';
      HtmlSchema._htmlCharacterEntities[(object) "prod"] = (object) '∏';
      HtmlSchema._htmlCharacterEntities[(object) "prop"] = (object) '∝';
      HtmlSchema._htmlCharacterEntities[(object) "Psi"] = (object) 'Ψ';
      HtmlSchema._htmlCharacterEntities[(object) "psi"] = (object) 'ψ';
      HtmlSchema._htmlCharacterEntities[(object) "quot"] = (object) '"';
      HtmlSchema._htmlCharacterEntities[(object) "radic"] = (object) '√';
      HtmlSchema._htmlCharacterEntities[(object) "rang"] = (object) '〉';
      HtmlSchema._htmlCharacterEntities[(object) "raquo"] = (object) '»';
      HtmlSchema._htmlCharacterEntities[(object) "rarr"] = (object) '→';
      HtmlSchema._htmlCharacterEntities[(object) "rArr"] = (object) '⇒';
      HtmlSchema._htmlCharacterEntities[(object) "rceil"] = (object) '⌉';
      HtmlSchema._htmlCharacterEntities[(object) "rdquo"] = (object) '”';
      HtmlSchema._htmlCharacterEntities[(object) "real"] = (object) 'ℜ';
      HtmlSchema._htmlCharacterEntities[(object) "reg"] = (object) '®';
      HtmlSchema._htmlCharacterEntities[(object) "rfloor"] = (object) '⌋';
      HtmlSchema._htmlCharacterEntities[(object) "Rho"] = (object) 'Ρ';
      HtmlSchema._htmlCharacterEntities[(object) "rho"] = (object) 'ρ';
      HtmlSchema._htmlCharacterEntities[(object) "rlm"] = (object) '\u200F';
      HtmlSchema._htmlCharacterEntities[(object) "rsaquo"] = (object) '›';
      HtmlSchema._htmlCharacterEntities[(object) "rsquo"] = (object) '’';
      HtmlSchema._htmlCharacterEntities[(object) "sbquo"] = (object) '‚';
      HtmlSchema._htmlCharacterEntities[(object) "Scaron"] = (object) 'Š';
      HtmlSchema._htmlCharacterEntities[(object) "scaron"] = (object) 'š';
      HtmlSchema._htmlCharacterEntities[(object) "sdot"] = (object) '⋅';
      HtmlSchema._htmlCharacterEntities[(object) "sect"] = (object) '§';
      HtmlSchema._htmlCharacterEntities[(object) "shy"] = (object) '\u00AD';
      HtmlSchema._htmlCharacterEntities[(object) "Sigma"] = (object) 'Σ';
      HtmlSchema._htmlCharacterEntities[(object) "sigma"] = (object) 'σ';
      HtmlSchema._htmlCharacterEntities[(object) "sigmaf"] = (object) 'ς';
      HtmlSchema._htmlCharacterEntities[(object) "sim"] = (object) '∼';
      HtmlSchema._htmlCharacterEntities[(object) "spades"] = (object) '♠';
      HtmlSchema._htmlCharacterEntities[(object) "sub"] = (object) '⊂';
      HtmlSchema._htmlCharacterEntities[(object) "sube"] = (object) '⊆';
      HtmlSchema._htmlCharacterEntities[(object) "sum"] = (object) '∑';
      HtmlSchema._htmlCharacterEntities[(object) "sup"] = (object) '⊃';
      HtmlSchema._htmlCharacterEntities[(object) "sup1"] = (object) '\u00B9';
      HtmlSchema._htmlCharacterEntities[(object) "sup2"] = (object) '\u00B2';
      HtmlSchema._htmlCharacterEntities[(object) "sup3"] = (object) '\u00B3';
      HtmlSchema._htmlCharacterEntities[(object) "supe"] = (object) '⊇';
      HtmlSchema._htmlCharacterEntities[(object) "szlig"] = (object) 'ß';
      HtmlSchema._htmlCharacterEntities[(object) "Tau"] = (object) 'Τ';
      HtmlSchema._htmlCharacterEntities[(object) "tau"] = (object) 'τ';
      HtmlSchema._htmlCharacterEntities[(object) "there4"] = (object) '∴';
      HtmlSchema._htmlCharacterEntities[(object) "Theta"] = (object) 'Θ';
      HtmlSchema._htmlCharacterEntities[(object) "theta"] = (object) 'θ';
      HtmlSchema._htmlCharacterEntities[(object) "thetasym"] = (object) 'ϑ';
      HtmlSchema._htmlCharacterEntities[(object) "thinsp"] = (object) ' ';
      HtmlSchema._htmlCharacterEntities[(object) "THORN"] = (object) 'Þ';
      HtmlSchema._htmlCharacterEntities[(object) "thorn"] = (object) 'þ';
      HtmlSchema._htmlCharacterEntities[(object) "tilde"] = (object) '˜';
      HtmlSchema._htmlCharacterEntities[(object) "times"] = (object) '×';
      HtmlSchema._htmlCharacterEntities[(object) "trade"] = (object) '™';
      HtmlSchema._htmlCharacterEntities[(object) "Uacute"] = (object) 'Ú';
      HtmlSchema._htmlCharacterEntities[(object) "uacute"] = (object) 'ú';
      HtmlSchema._htmlCharacterEntities[(object) "uarr"] = (object) '↑';
      HtmlSchema._htmlCharacterEntities[(object) "uArr"] = (object) '⇑';
      HtmlSchema._htmlCharacterEntities[(object) "Ucirc"] = (object) 'Û';
      HtmlSchema._htmlCharacterEntities[(object) "ucirc"] = (object) 'û';
      HtmlSchema._htmlCharacterEntities[(object) "Ugrave"] = (object) 'Ù';
      HtmlSchema._htmlCharacterEntities[(object) "ugrave"] = (object) 'ù';
      HtmlSchema._htmlCharacterEntities[(object) "uml"] = (object) '¨';
      HtmlSchema._htmlCharacterEntities[(object) "upsih"] = (object) 'ϒ';
      HtmlSchema._htmlCharacterEntities[(object) "Upsilon"] = (object) 'Υ';
      HtmlSchema._htmlCharacterEntities[(object) "upsilon"] = (object) 'υ';
      HtmlSchema._htmlCharacterEntities[(object) "Uuml"] = (object) 'Ü';
      HtmlSchema._htmlCharacterEntities[(object) "uuml"] = (object) 'ü';
      HtmlSchema._htmlCharacterEntities[(object) "weierp"] = (object) '℘';
      HtmlSchema._htmlCharacterEntities[(object) "Xi"] = (object) 'Ξ';
      HtmlSchema._htmlCharacterEntities[(object) "xi"] = (object) 'ξ';
      HtmlSchema._htmlCharacterEntities[(object) "Yacute"] = (object) 'Ý';
      HtmlSchema._htmlCharacterEntities[(object) "yacute"] = (object) 'ý';
      HtmlSchema._htmlCharacterEntities[(object) "yen"] = (object) '¥';
      HtmlSchema._htmlCharacterEntities[(object) "Yuml"] = (object) 'Ÿ';
      HtmlSchema._htmlCharacterEntities[(object) "yuml"] = (object) 'ÿ';
      HtmlSchema._htmlCharacterEntities[(object) "Zeta"] = (object) 'Ζ';
      HtmlSchema._htmlCharacterEntities[(object) "zeta"] = (object) 'ζ';
      HtmlSchema._htmlCharacterEntities[(object) "zwj"] = (object) '\u200D';
      HtmlSchema._htmlCharacterEntities[(object) "zwnj"] = (object) '\u200C';
    }
  }
}
