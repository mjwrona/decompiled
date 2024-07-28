// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations.WorkItemRuleDeclaration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations
{
  public abstract class WorkItemRuleDeclaration : 
    IEquatable<WorkItemRuleDeclaration>,
    IComparable<WorkItemRuleDeclaration>
  {
    public WorkItemRuleDeclaration()
    {
    }

    public WorkItemRuleDeclaration(XElement xmlRuleElement)
    {
      this.For = xmlRuleElement.Attribute((XName) "for")?.Value;
      this.Not = xmlRuleElement.Attribute((XName) "not")?.Value;
    }

    public abstract WorkItemRuleName Name { get; }

    public string For { get; set; }

    public string Not { get; set; }

    public bool Equals(WorkItemRuleDeclaration other) => this.Equals(other, true, (Func<WorkItemRuleDeclaration, bool>) null);

    public virtual bool Equals(
      WorkItemRuleDeclaration other,
      bool deep,
      Func<WorkItemRuleDeclaration, bool> applicable)
    {
      return other != null && this.GetType() == other.GetType() && StringComparer.OrdinalIgnoreCase.Equals(this.For ?? "", other.For ?? "") && StringComparer.OrdinalIgnoreCase.Equals(this.Not ?? "", other.Not ?? "");
    }

    public virtual int CompareTo(WorkItemRuleDeclaration other) => other != null ? this.Name.CompareTo((object) other.Name) : 1;

    public static WorkItemRuleDeclaration Parse(XElement xmlRuleElement, Action<string> logError)
    {
      WorkItemRuleDeclaration rule;
      if (WorkItemRuleDeclaration.TryParse(xmlRuleElement, out rule, logError))
        return rule;
      logError(string.Format("Unrecognized xml rule element '{0}", (object) xmlRuleElement.Name));
      return (WorkItemRuleDeclaration) null;
    }

    public static bool TryParse(
      XElement xmlRuleElement,
      out WorkItemRuleDeclaration rule,
      Action<string> logError)
    {
      string localName = xmlRuleElement.Name.LocalName;
      if (localName != null)
      {
        switch (localName.Length)
        {
          case 4:
            switch (localName[0])
            {
              case 'C':
                if (localName == "COPY")
                {
                  rule = (WorkItemRuleDeclaration) new CopyRuleDeclaration(xmlRuleElement, logError);
                  break;
                }
                goto label_49;
              case 'W':
                if (localName == "WHEN")
                {
                  rule = (WorkItemRuleDeclaration) new WhenRuleDeclaration(xmlRuleElement, logError);
                  break;
                }
                goto label_49;
              default:
                goto label_49;
            }
            break;
          case 5:
            switch (localName[0])
            {
              case 'E':
                if (localName == "EMPTY")
                {
                  rule = (WorkItemRuleDeclaration) new EmptyRuleDeclaration(xmlRuleElement);
                  break;
                }
                goto label_49;
              case 'M':
                if (localName == "MATCH")
                {
                  rule = (WorkItemRuleDeclaration) new MatchRuleDeclaration(xmlRuleElement, logError);
                  break;
                }
                goto label_49;
              default:
                goto label_49;
            }
            break;
          case 6:
            if (localName == "FROZEN")
            {
              rule = (WorkItemRuleDeclaration) new FrozenRuleDeclaration(xmlRuleElement);
              break;
            }
            goto label_49;
          case 7:
            switch (localName[0])
            {
              case 'D':
                if (localName == "DEFAULT")
                {
                  rule = (WorkItemRuleDeclaration) new DefaultRuleDeclaration(xmlRuleElement, logError);
                  break;
                }
                goto label_49;
              case 'W':
                if (localName == "WHENNOT")
                {
                  rule = (WorkItemRuleDeclaration) new WhenNotRuleDeclaration(xmlRuleElement, logError);
                  break;
                }
                goto label_49;
              default:
                goto label_49;
            }
            break;
          case 8:
            switch (localName[2])
            {
              case 'A':
                if (localName == "READONLY")
                {
                  rule = (WorkItemRuleDeclaration) new ReadOnlyRuleDeclaration(xmlRuleElement);
                  break;
                }
                goto label_49;
              case 'L':
                if (localName == "HELPTEXT")
                {
                  rule = (WorkItemRuleDeclaration) new HelpTextRuleDeclaration(xmlRuleElement);
                  break;
                }
                goto label_49;
              case 'Q':
                if (localName == "REQUIRED")
                {
                  rule = (WorkItemRuleDeclaration) new RequiredRuleDeclaration(xmlRuleElement);
                  break;
                }
                goto label_49;
              default:
                goto label_49;
            }
            break;
          case 9:
            switch (localName[0])
            {
              case 'N':
                if (localName == "NOTSAMEAS")
                {
                  rule = (WorkItemRuleDeclaration) new NotSameAsRuleDeclaration(xmlRuleElement, logError);
                  break;
                }
                goto label_49;
              case 'V':
                if (localName == "VALIDUSER")
                {
                  rule = (WorkItemRuleDeclaration) new ValidUserRuleDeclaration(xmlRuleElement);
                  break;
                }
                goto label_49;
              default:
                goto label_49;
            }
            break;
          case 11:
            if (localName == "WHENCHANGED")
            {
              rule = (WorkItemRuleDeclaration) new WhenChangedRuleDeclaration(xmlRuleElement, logError);
              break;
            }
            goto label_49;
          case 13:
            switch (localName[0])
            {
              case 'A':
                if (localName == "ALLOWEDVALUES")
                {
                  rule = (WorkItemRuleDeclaration) new AllowedValuesRuleDeclaration(xmlRuleElement, logError);
                  break;
                }
                goto label_49;
              case 'S':
                if (localName == "SERVERDEFAULT")
                {
                  rule = (WorkItemRuleDeclaration) new ServerDefaultRuleDeclaration(xmlRuleElement, logError);
                  break;
                }
                goto label_49;
              default:
                goto label_49;
            }
            break;
          case 14:
            if (localName == "WHENNOTCHANGED")
            {
              rule = (WorkItemRuleDeclaration) new WhenNotChangedRuleDeclaration(xmlRuleElement, logError);
              break;
            }
            goto label_49;
          case 15:
            switch (localName[0])
            {
              case 'C':
                if (localName == "CANNOTLOSEVALUE")
                {
                  rule = (WorkItemRuleDeclaration) new CannotLoseValueRuleDeclaration(xmlRuleElement);
                  break;
                }
                goto label_49;
              case 'S':
                if (localName == "SUGGESTEDVALUES")
                {
                  rule = (WorkItemRuleDeclaration) new SuggestedValuesRuleDeclaration(xmlRuleElement, logError);
                  break;
                }
                goto label_49;
              default:
                goto label_49;
            }
            break;
          case 16:
            if (localName == "PROHIBITEDVALUES")
            {
              rule = (WorkItemRuleDeclaration) new ProhibitedValuesRuleDeclaration(xmlRuleElement, logError);
              break;
            }
            goto label_49;
          case 18:
            if (localName == "ALLOWEXISTINGVALUE")
            {
              rule = (WorkItemRuleDeclaration) new AllowExistingValueRuleDeclaration(xmlRuleElement);
              break;
            }
            goto label_49;
          default:
            goto label_49;
        }
        return true;
      }
label_49:
      rule = (WorkItemRuleDeclaration) null;
      return false;
    }

    public static IReadOnlyCollection<WorkItemRuleDeclaration> Parse(
      IEnumerable<XElement> xmlRuleElements,
      Action<string> logError)
    {
      return xmlRuleElements == null ? (IReadOnlyCollection<WorkItemRuleDeclaration>) Array.Empty<WorkItemRuleDeclaration>() : (IReadOnlyCollection<WorkItemRuleDeclaration>) xmlRuleElements.Select<XElement, WorkItemRuleDeclaration>((Func<XElement, WorkItemRuleDeclaration>) (e => WorkItemRuleDeclaration.Parse(e, logError))).Where<WorkItemRuleDeclaration>((Func<WorkItemRuleDeclaration, bool>) (e => e != null)).ToArray<WorkItemRuleDeclaration>();
    }
  }
}
