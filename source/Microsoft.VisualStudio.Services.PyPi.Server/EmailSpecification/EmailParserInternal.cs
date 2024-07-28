// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.EmailSpecification.EmailParserInternal
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Pegasus.Common;
using Pegasus.Common.Tracing;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.PyPi.Server.EmailSpecification
{
  [GeneratedCode("Pegasus", "4.0.14.0")]
  internal class EmailParserInternal
  {
    private ITracer tracer = (ITracer) NullTracer.Instance;
    private EmailParserInternal.ExportedRules exported;

    public EmailSpec ParseEmail(string subject, string fileName = null) => this.WrapRule<EmailSpec>(new Cursor(subject, fileName: fileName), new ParseDelegate<EmailSpec>(this.Specification), "Specification").Value;

    private Exception ExceptionHelper(
      Cursor cursor,
      Func<Cursor, EmailParserInternal.UnparseableSpecError> wrappedCode)
    {
      return (Exception) new InvalidEmailException(Resources.Error_InvalidEmailAddress((object) cursor.Subject));
    }

    private IParseResult<T> WrapRule<T>(Cursor cursor, ParseDelegate<T> startRule, string ruleName)
    {
      return this.StartRuleHelper<T>(cursor, new ParseDelegate<T>(WrappedRule), ruleName);

      IParseResult<T> WrappedRule(ref Cursor cursor1) => startRule(ref cursor1) ?? throw this.ExceptionHelper(cursor1, (Func<Cursor, EmailParserInternal.UnparseableSpecError>) (_ => new EmailParserInternal.UnparseableSpecError()));
    }

    public ITracer Tracer
    {
      get => this.tracer;
      set => this.tracer = value ?? (ITracer) NullTracer.Instance;
    }

    public EmailParserInternal.ExportedRules Exported => this.exported ?? (this.exported = new EmailParserInternal.ExportedRules(this));

    public EmailSpec Parse(string subject, string fileName = null) => this.StartRuleHelper<EmailSpec>(new Cursor(subject, fileName: fileName), new ParseDelegate<EmailSpec>(this.Specification), "Specification").Value;

    private IParseResult<string> validChars(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (validChars), cursor);
      IParseResult<string> parseResult = this.ParseClass(ref cursor, "  \t\t..@@<<>>\"\",,", true);
      this.tracer.TraceRuleExit<string>(nameof (validChars), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<string> period(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (period), cursor);
      IParseResult<string> parseResult = this.ParseClass(ref cursor, "..");
      this.tracer.TraceRuleExit<string>(nameof (period), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<string> text(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (text), cursor);
      IParseResult<string> parseResult = this.ParseClass(ref cursor, "\"\"\r\r\n\n<<>>", true);
      this.tracer.TraceRuleExit<string>(nameof (text), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<string> quotedText(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (quotedText), cursor);
      IParseResult<string> parseResult = this.ParseClass(ref cursor, "\"\"\r\r\n\n", true);
      this.tracer.TraceRuleExit<string>(nameof (quotedText), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<string> wsp(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (wsp), cursor);
      IParseResult<string> parseResult = this.ParseClass(ref cursor, "  \t\t");
      this.tracer.TraceRuleExit<string>(nameof (wsp), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<string> end(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (end), cursor);
      IParseResult<string> parseResult = (IParseResult<string>) null;
      Cursor cursor1 = cursor;
      if (this.ParseAny(ref cursor) == null)
        parseResult = this.ReturnHelper<string>(cursor, ref cursor, (Func<Cursor, string>) (state => string.Empty));
      else
        cursor = cursor1;
      this.tracer.TraceRuleExit<string>(nameof (end), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<string> subject(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (subject), cursor);
      IParseResult<string> parseResult1 = (IParseResult<string>) null;
      Cursor startCursor0 = cursor;
      Cursor startCursor1 = cursor;
      List<string> l0 = new List<string>();
      while (true)
      {
        IParseResult<string> parseResult2 = this.wsp(ref cursor);
        if (parseResult2 != null)
          l0.Add(parseResult2.Value);
        else
          break;
      }
      if (this.ReturnHelper<IList<string>>(startCursor1, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly())) != null)
      {
        IParseResult<IList<string>> parseResult3 = (IParseResult<IList<string>>) null;
        Cursor startCursor2 = cursor;
        List<string> l1 = new List<string>();
        while (true)
        {
          IParseResult<string> parseResult4 = this.validChars(ref cursor);
          if (parseResult4 != null)
            l1.Add(parseResult4.Value);
          else
            break;
        }
        if (l1.Count >= 1)
          parseResult3 = this.ReturnHelper<IList<string>>(startCursor2, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l1.AsReadOnly()));
        else
          cursor = startCursor2;
        if (parseResult3 != null)
        {
          Cursor startCursor3 = cursor;
          List<string> l2 = new List<string>();
          while (true)
          {
            IParseResult<string> parseResult5 = (IParseResult<string>) null;
            Cursor startCursor4 = cursor;
            if (this.period(ref cursor) != null)
            {
              IParseResult<IList<string>> parseResult6 = (IParseResult<IList<string>>) null;
              Cursor startCursor5 = cursor;
              List<string> l3 = new List<string>();
              while (true)
              {
                IParseResult<string> parseResult7 = this.validChars(ref cursor);
                if (parseResult7 != null)
                  l3.Add(parseResult7.Value);
                else
                  break;
              }
              if (l3.Count >= 1)
                parseResult6 = this.ReturnHelper<IList<string>>(startCursor5, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l3.AsReadOnly()));
              else
                cursor = startCursor5;
              if (parseResult6 != null)
              {
                int len = cursor.Location - startCursor4.Location;
                parseResult5 = this.ReturnHelper<string>(startCursor4, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor4.Location, len)));
              }
              else
                cursor = startCursor4;
            }
            else
              cursor = startCursor4;
            if (parseResult5 != null)
              l2.Add(parseResult5.Value);
            else
              break;
          }
          if (this.ReturnHelper<IList<string>>(startCursor3, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l2.AsReadOnly())) != null)
          {
            int len = cursor.Location - startCursor0.Location;
            parseResult1 = this.ReturnHelper<string>(startCursor0, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor0.Location, len)));
          }
          else
            cursor = startCursor0;
        }
        else
          cursor = startCursor0;
      }
      else
        cursor = startCursor0;
      this.tracer.TraceRuleExit<string>(nameof (subject), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> domain(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (domain), cursor);
      IParseResult<string> parseResult1 = (IParseResult<string>) null;
      Cursor startCursor0 = cursor;
      IParseResult<IList<string>> parseResult2 = (IParseResult<IList<string>>) null;
      Cursor startCursor1 = cursor;
      List<string> l0 = new List<string>();
      while (true)
      {
        IParseResult<string> parseResult3 = this.validChars(ref cursor);
        if (parseResult3 != null)
          l0.Add(parseResult3.Value);
        else
          break;
      }
      if (l0.Count >= 1)
        parseResult2 = this.ReturnHelper<IList<string>>(startCursor1, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly()));
      else
        cursor = startCursor1;
      if (parseResult2 != null)
      {
        IParseResult<IList<string>> parseResult4 = (IParseResult<IList<string>>) null;
        Cursor startCursor2 = cursor;
        List<string> l1 = new List<string>();
        while (true)
        {
          IParseResult<string> parseResult5 = (IParseResult<string>) null;
          Cursor startCursor3 = cursor;
          if (this.period(ref cursor) != null)
          {
            IParseResult<IList<string>> parseResult6 = (IParseResult<IList<string>>) null;
            Cursor startCursor4 = cursor;
            List<string> l2 = new List<string>();
            while (true)
            {
              IParseResult<string> parseResult7 = this.validChars(ref cursor);
              if (parseResult7 != null)
                l2.Add(parseResult7.Value);
              else
                break;
            }
            if (l2.Count >= 1)
              parseResult6 = this.ReturnHelper<IList<string>>(startCursor4, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l2.AsReadOnly()));
            else
              cursor = startCursor4;
            if (parseResult6 != null)
            {
              int len = cursor.Location - startCursor3.Location;
              parseResult5 = this.ReturnHelper<string>(startCursor3, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor3.Location, len)));
            }
            else
              cursor = startCursor3;
          }
          else
            cursor = startCursor3;
          if (parseResult5 != null)
            l1.Add(parseResult5.Value);
          else
            break;
        }
        if (l1.Count >= 1)
          parseResult4 = this.ReturnHelper<IList<string>>(startCursor2, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l1.AsReadOnly()));
        else
          cursor = startCursor2;
        if (parseResult4 != null)
        {
          Cursor startCursor5 = cursor;
          List<string> l3 = new List<string>();
          while (true)
          {
            IParseResult<string> parseResult8 = this.wsp(ref cursor);
            if (parseResult8 != null)
              l3.Add(parseResult8.Value);
            else
              break;
          }
          if (this.ReturnHelper<IList<string>>(startCursor5, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l3.AsReadOnly())) != null)
          {
            int len = cursor.Location - startCursor0.Location;
            parseResult1 = this.ReturnHelper<string>(startCursor0, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor0.Location, len)));
          }
          else
            cursor = startCursor0;
        }
        else
          cursor = startCursor0;
      }
      else
        cursor = startCursor0;
      this.tracer.TraceRuleExit<string>(nameof (domain), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> emailStr(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (emailStr), cursor);
      IParseResult<string> parseResult = (IParseResult<string>) null;
      Cursor startCursor0 = cursor;
      if (this.subject(ref cursor) != null)
      {
        if (this.ParseLiteral(ref cursor, "@") != null)
        {
          if (this.domain(ref cursor) != null)
          {
            int len = cursor.Location - startCursor0.Location;
            parseResult = this.ReturnHelper<string>(startCursor0, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor0.Location, len)));
          }
          else
            cursor = startCursor0;
        }
        else
          cursor = startCursor0;
      }
      else
        cursor = startCursor0;
      this.tracer.TraceRuleExit<string>(nameof (emailStr), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<string> bracketedEmail(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (bracketedEmail), cursor);
      IParseResult<string> parseResult = (IParseResult<string>) null;
      Cursor startCursor = cursor;
      if (this.ParseLiteral(ref cursor, "<") != null)
      {
        IParseResult<string> result = this.emailStr(ref cursor);
        string e = this.ValueOrDefault<string>(result);
        if (result != null)
        {
          if (this.ParseLiteral(ref cursor, ">") != null)
            parseResult = this.ReturnHelper<string>(startCursor, ref cursor, (Func<Cursor, string>) (state => e.ToString()));
          else
            cursor = startCursor;
        }
        else
          cursor = startCursor;
      }
      else
        cursor = startCursor;
      this.tracer.TraceRuleExit<string>(nameof (bracketedEmail), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<string> name(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (name), cursor);
      IParseResult<string> parseResult1 = (IParseResult<string>) null;
      Cursor startCursor0 = cursor;
      IParseResult<IList<string>> parseResult2 = (IParseResult<IList<string>>) null;
      Cursor startCursor = cursor;
      List<string> l0 = new List<string>();
      while (true)
      {
        IParseResult<string> parseResult3 = this.text(ref cursor);
        if (parseResult3 != null)
          l0.Add(parseResult3.Value);
        else
          break;
      }
      if (l0.Count >= 1)
        parseResult2 = this.ReturnHelper<IList<string>>(startCursor, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly()));
      else
        cursor = startCursor;
      if (parseResult2 != null)
      {
        int len = cursor.Location - startCursor0.Location;
        parseResult1 = this.ReturnHelper<string>(startCursor0, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor0.Location, len)));
      }
      else
        cursor = startCursor0;
      this.tracer.TraceRuleExit<string>(nameof (name), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> quotedName(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (quotedName), cursor);
      IParseResult<string> parseResult1 = (IParseResult<string>) null;
      Cursor startCursor1 = cursor;
      if (this.ParseLiteral(ref cursor, "\"") != null)
      {
        IParseResult<IList<string>> result = (IParseResult<IList<string>>) null;
        Cursor startCursor2 = cursor;
        List<string> l0 = new List<string>();
        while (true)
        {
          IParseResult<string> parseResult2 = this.quotedText(ref cursor);
          if (parseResult2 != null)
            l0.Add(parseResult2.Value);
          else
            break;
        }
        if (l0.Count >= 1)
          result = this.ReturnHelper<IList<string>>(startCursor2, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly()));
        else
          cursor = startCursor2;
        IList<string> n = this.ValueOrDefault<IList<string>>(result);
        if (result != null)
        {
          if (this.ParseLiteral(ref cursor, "\"") != null)
            parseResult1 = this.ReturnHelper<string>(startCursor1, ref cursor, (Func<Cursor, string>) (state => n.ToString()));
          else
            cursor = startCursor1;
        }
        else
          cursor = startCursor1;
      }
      else
        cursor = startCursor1;
      this.tracer.TraceRuleExit<string>(nameof (quotedName), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<Email> emailWithName(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (emailWithName), cursor);
      IParseResult<Email> parseResult1 = (IParseResult<Email>) null;
      Cursor startCursor1 = cursor;
      IParseResult<string> result1 = this.name(ref cursor);
      string n = this.ValueOrDefault<string>(result1);
      if (result1 != null)
      {
        Cursor startCursor2 = cursor;
        List<string> l0 = new List<string>();
        while (true)
        {
          IParseResult<string> parseResult2 = this.wsp(ref cursor);
          if (parseResult2 != null)
            l0.Add(parseResult2.Value);
          else
            break;
        }
        if (this.ReturnHelper<IList<string>>(startCursor2, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly())) != null)
        {
          IParseResult<string> result2 = this.bracketedEmail(ref cursor);
          string e = this.ValueOrDefault<string>(result2);
          if (result2 != null)
            parseResult1 = this.ReturnHelper<Email>(startCursor1, ref cursor, (Func<Cursor, Email>) (state => new Email(n, e)));
          else
            cursor = startCursor1;
        }
        else
          cursor = startCursor1;
      }
      else
        cursor = startCursor1;
      this.tracer.TraceRuleExit<Email>(nameof (emailWithName), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<Email> emailWithQuotedName(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (emailWithQuotedName), cursor);
      IParseResult<Email> parseResult1 = (IParseResult<Email>) null;
      Cursor startCursor1 = cursor;
      IParseResult<string> result1 = this.quotedName(ref cursor);
      string qn = this.ValueOrDefault<string>(result1);
      if (result1 != null)
      {
        Cursor startCursor2 = cursor;
        List<string> l0 = new List<string>();
        while (true)
        {
          IParseResult<string> parseResult2 = this.wsp(ref cursor);
          if (parseResult2 != null)
            l0.Add(parseResult2.Value);
          else
            break;
        }
        if (this.ReturnHelper<IList<string>>(startCursor2, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly())) != null)
        {
          IParseResult<string> result2 = this.bracketedEmail(ref cursor);
          string e = this.ValueOrDefault<string>(result2);
          if (result2 != null)
            parseResult1 = this.ReturnHelper<Email>(startCursor1, ref cursor, (Func<Cursor, Email>) (state => new Email(qn, e)));
          else
            cursor = startCursor1;
        }
        else
          cursor = startCursor1;
      }
      else
        cursor = startCursor1;
      this.tracer.TraceRuleExit<Email>(nameof (emailWithQuotedName), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<Email> emailWithoutName(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (emailWithoutName), cursor);
      IParseResult<Email> parseResult = (IParseResult<Email>) null;
      Cursor startCursor = cursor;
      IParseResult<string> result = this.emailStr(ref cursor);
      string emailAddress = this.ValueOrDefault<string>(result);
      if (result != null)
        parseResult = this.ReturnHelper<Email>(startCursor, ref cursor, (Func<Cursor, Email>) (state => new Email((string) null, emailAddress)));
      else
        cursor = startCursor;
      this.tracer.TraceRuleExit<Email>(nameof (emailWithoutName), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<Email> email(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (email), cursor);
      IParseResult<Email> parseResult = (((IParseResult<Email>) null ?? this.emailWithoutName(ref cursor)) ?? this.emailWithQuotedName(ref cursor)) ?? this.emailWithName(ref cursor);
      this.tracer.TraceRuleExit<Email>(nameof (email), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<EmailSpec> Specification(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (Specification), cursor);
      IParseResult<EmailSpec> parseResult1 = (IParseResult<EmailSpec>) null;
      Cursor startCursor1 = cursor;
      IParseResult<Email> result1 = this.email(ref cursor);
      Email first = this.ValueOrDefault<Email>(result1);
      if (result1 != null)
      {
        Cursor startCursor2 = cursor;
        List<Email> l0 = new List<Email>();
        while (true)
        {
          IParseResult<Email> parseResult2 = (IParseResult<Email>) null;
          Cursor startCursor3 = cursor;
          Cursor startCursor4 = cursor;
          List<string> l1 = new List<string>();
          while (true)
          {
            IParseResult<string> parseResult3 = this.wsp(ref cursor);
            if (parseResult3 != null)
              l1.Add(parseResult3.Value);
            else
              break;
          }
          if (this.ReturnHelper<IList<string>>(startCursor4, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l1.AsReadOnly())) != null)
          {
            if (this.ParseLiteral(ref cursor, ",") != null)
            {
              Cursor startCursor5 = cursor;
              List<string> l2 = new List<string>();
              while (true)
              {
                IParseResult<string> parseResult4 = this.wsp(ref cursor);
                if (parseResult4 != null)
                  l2.Add(parseResult4.Value);
                else
                  break;
              }
              if (this.ReturnHelper<IList<string>>(startCursor5, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l2.AsReadOnly())) != null)
              {
                IParseResult<Email> result2 = this.email(ref cursor);
                Email e = this.ValueOrDefault<Email>(result2);
                if (result2 != null)
                  parseResult2 = this.ReturnHelper<Email>(startCursor3, ref cursor, (Func<Cursor, Email>) (state => e));
                else
                  cursor = startCursor3;
              }
              else
                cursor = startCursor3;
            }
            else
              cursor = startCursor3;
          }
          else
            cursor = startCursor3;
          if (parseResult2 != null)
            l0.Add(parseResult2.Value);
          else
            break;
        }
        IParseResult<IList<Email>> result3 = this.ReturnHelper<IList<Email>>(startCursor2, ref cursor, (Func<Cursor, IList<Email>>) (state => (IList<Email>) l0.AsReadOnly()));
        IList<Email> rest = this.ValueOrDefault<IList<Email>>(result3);
        if (result3 != null)
        {
          Cursor startCursor6 = cursor;
          List<string> l3 = new List<string>();
          while (true)
          {
            IParseResult<string> parseResult5 = this.wsp(ref cursor);
            if (parseResult5 != null)
              l3.Add(parseResult5.Value);
            else
              break;
          }
          if (this.ReturnHelper<IList<string>>(startCursor6, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l3.AsReadOnly())) != null)
          {
            IParseResult<string> parseResult6 = (IParseResult<string>) null ?? this.end(ref cursor);
            if (parseResult6 == null)
              throw this.ExceptionHelper(cursor, (Func<Cursor, EmailParserInternal.UnparseableSpecError>) (state => new EmailParserInternal.UnparseableSpecError()));
            if (parseResult6 != null)
              parseResult1 = this.ReturnHelper<EmailSpec>(startCursor1, ref cursor, (Func<Cursor, EmailSpec>) (state => new EmailSpec((IEnumerable<Email>) ImmutableList.Create<Email>(first).AddRange((IEnumerable<Email>) rest))));
            else
              cursor = startCursor1;
          }
          else
            cursor = startCursor1;
        }
        else
          cursor = startCursor1;
      }
      else
        cursor = startCursor1;
      this.tracer.TraceRuleExit<EmailSpec>(nameof (Specification), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<T> StartRuleHelper<T>(
      Cursor cursor,
      ParseDelegate<T> startRule,
      string ruleName)
    {
      return startRule(ref cursor) ?? throw this.ExceptionHelper(cursor, (Func<Cursor, string>) (state => "Failed to parse '" + ruleName + "'."));
    }

    private IParseResult<string> ParseLiteral(ref Cursor cursor, string literal, bool ignoreCase = false)
    {
      if (cursor.Location + literal.Length <= cursor.Subject.Length)
      {
        string substr = cursor.Subject.Substring(cursor.Location, literal.Length);
        if ((ignoreCase ? (substr.Equals(literal, StringComparison.OrdinalIgnoreCase) ? 1 : 0) : (substr == literal ? 1 : 0)) != 0)
        {
          Cursor endCursor = cursor.Advance(substr.Length);
          IParseResult<string> literal1 = this.ReturnHelper<string>(cursor, ref endCursor, (Func<Cursor, string>) (state => substr));
          cursor = endCursor;
          return literal1;
        }
      }
      return (IParseResult<string>) null;
    }

    private IParseResult<string> ParseClass(
      ref Cursor cursor,
      string characterRanges,
      bool negated = false,
      bool ignoreCase = false)
    {
      if (cursor.Location + 1 <= cursor.Subject.Length)
      {
        char c1 = cursor.Subject[cursor.Location];
        bool flag = false;
        for (int index = 0; !flag && index < characterRanges.Length; index += 2)
          flag = (int) c1 >= (int) characterRanges[index] && (int) c1 <= (int) characterRanges[index + 1];
        if (!flag & ignoreCase && (char.IsUpper(c1) || char.IsLower(c1)))
        {
          string str = c1.ToString();
          for (int index = 0; !flag && index < characterRanges.Length; index += 2)
          {
            int characterRange1 = (int) characterRanges[index];
            char characterRange2 = characterRanges[index + 1];
            for (char c2 = (char) characterRange1; !flag && (int) c2 <= (int) characterRange2; ++c2)
              flag = (char.IsUpper(c2) || char.IsLower(c2)) && str.Equals(c2.ToString(), StringComparison.CurrentCultureIgnoreCase);
          }
        }
        if (flag ^ negated)
        {
          Cursor endCursor = cursor.Advance(1);
          string substr = cursor.Subject.Substring(cursor.Location, 1);
          IParseResult<string> parseResult = this.ReturnHelper<string>(cursor, ref endCursor, (Func<Cursor, string>) (state => substr));
          cursor = endCursor;
          return parseResult;
        }
      }
      return (IParseResult<string>) null;
    }

    private IParseResult<string> ParseAny(ref Cursor cursor)
    {
      if (cursor.Location + 1 > cursor.Subject.Length)
        return (IParseResult<string>) null;
      string substr = cursor.Subject.Substring(cursor.Location, 1);
      Cursor endCursor = cursor.Advance(1);
      IParseResult<string> any = this.ReturnHelper<string>(cursor, ref endCursor, (Func<Cursor, string>) (state => substr));
      cursor = endCursor;
      return any;
    }

    private IParseResult<T> ReturnHelper<T>(
      Cursor startCursor,
      ref Cursor endCursor,
      Func<Cursor, T> wrappedCode)
    {
      T obj = wrappedCode(endCursor);
      if (obj is ILexical lexical && lexical.StartCursor == (Cursor) null && lexical.EndCursor == (Cursor) null)
      {
        lexical.StartCursor = startCursor;
        lexical.EndCursor = endCursor;
      }
      return (IParseResult<T>) new ParseResult<T>(startCursor, endCursor, obj);
    }

    private IParseResult<T> ParseHelper<T>(ref Cursor cursor, ParseDelegate<T> wrappedCode)
    {
      Cursor cursor1 = cursor;
      IParseResult<T> helper = wrappedCode(ref cursor);
      if (helper == null)
      {
        cursor = cursor1;
        return (IParseResult<T>) null;
      }
      cursor = cursor.WithMutability(false);
      return helper;
    }

    private Exception ExceptionHelper(Cursor cursor, Func<Cursor, string> wrappedCode)
    {
      FormatException formatException = new FormatException(wrappedCode(cursor));
      formatException.Data[(object) nameof (cursor)] = (object) cursor;
      return (Exception) formatException;
    }

    private T ValueOrDefault<T>(IParseResult<T> result) => result != null ? result.Value : default (T);

    private class UnparseableSpecError
    {
    }

    public sealed class ExportedRules
    {
      private EmailParserInternal parser;

      internal ExportedRules(EmailParserInternal parser) => this.parser = parser;

      public IParseResult<EmailSpec> Specification(ref Cursor cursor) => this.parser.Specification(ref cursor);
    }
  }
}
