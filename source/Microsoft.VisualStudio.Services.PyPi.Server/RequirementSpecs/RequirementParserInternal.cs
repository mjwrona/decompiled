// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.RequirementParserInternal
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Pegasus.Common;
using Pegasus.Common.Tracing;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs
{
  [GeneratedCode("Pegasus", "4.0.14.0")]
  internal class RequirementParserInternal
  {
    private Dictionary<CacheKey, object> storage;
    private ITracer tracer = (ITracer) NullTracer.Instance;
    private RequirementParserInternal.ExportedRules exported;

    private Exception ExceptionHelper(
      Cursor cursor,
      Func<Cursor, RequirementParserInternal.UnknownEnvVarError> wrappedCode)
    {
      RequirementParserInternal.UnknownEnvVarError unknownEnvVarError = wrappedCode(cursor);
      return (Exception) new RequirementParseException(Resources.Error_UnknownMarkerVariable((object) unknownEnvVarError.Identifier.Name, (object) unknownEnvVarError.IdentifierStartPosition.Location));
    }

    private Exception ExceptionHelper(
      Cursor cursor,
      Func<Cursor, RequirementParserInternal.UnparseableSpecError> wrappedCode)
    {
      return cursor.Location >= cursor.Subject.Length ? (Exception) new RequirementParseException(Resources.Error_RequirementParseErrorUnexpectedEOF((object) cursor.Location)) : (Exception) new RequirementParseException(Resources.Error_RequirementParseError((object) cursor.Subject[cursor.Location], (object) cursor.Location));
    }

    public RequirementSpec ParseRequirement(string subject, string fileName = null) => this.WrapRule<RequirementSpec>(new Cursor(subject, fileName: fileName), new ParseDelegate<RequirementSpec>(this.Specification), "Specification").Value;

    public Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList ParseVersionConstraintList(
      string subject,
      string fileName = null)
    {
      return this.WrapRule<Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList>(new Cursor(subject, fileName: fileName), new ParseDelegate<Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList>(this.VersionConstraintList), "VersionConstraintList").Value;
    }

    private IParseResult<T> WrapRule<T>(Cursor cursor, ParseDelegate<T> startRule, string ruleName)
    {
      return this.StartRuleHelper<T>(cursor, new ParseDelegate<T>(WrappedRule), ruleName);

      IParseResult<T> WrappedRule(ref Cursor cursor1) => startRule(ref cursor1) ?? throw this.ExceptionHelper(cursor1, (Func<Cursor, RequirementParserInternal.UnparseableSpecError>) (_ => new RequirementParserInternal.UnparseableSpecError()));
    }

    private IParseResult<T> LongestSubsequence<T>(
      ref Cursor cursor,
      params ParseDelegate<T>[] rules)
      where T : class
    {
      Cursor startCursor = cursor;
      using (IEnumerator<ParseDelegate<T>> enumerator = ((IEnumerable<ParseDelegate<T>>) rules).GetEnumerator())
      {
        enumerator.MoveNext();
        (IParseResult<T> result1, Cursor endCursor) = Run(enumerator.Current);
        Cursor end = result1 != null ? endCursor : startCursor;
        this.Tracer.TraceInfo(nameof (LongestSubsequence), cursor, string.Format("Best result: {0}, match length {1}", (object) (result1 != null ? result1.Value : default (T)), (object) MatchLength(end)));
        while (enumerator.MoveNext())
        {
          (IParseResult<T> result2, Cursor cursor1) = Run(enumerator.Current);
          if (result2 != null && MatchLength(cursor1) > MatchLength(end))
          {
            result1 = result2;
            end = cursor1;
          }
        }
        cursor = end;
        return result1;
      }

      (IParseResult<T> result, Cursor endCursor) Run<T>(ParseDelegate<T> arg) where T : class
      {
        Cursor startCursor = startCursor;
        IParseResult<T> parseResult = arg(ref startCursor);
        this.Tracer.TraceInfo(arg.Method.Name, startCursor, "<----");
        Cursor cursor = startCursor;
        return (parseResult, cursor);
      }

      int MatchLength<T>(Cursor end) where T : class => end.Location - startCursor.Location;
    }

    private Uri ConvertUri(string uri)
    {
      try
      {
        return new Uri(uri, UriKind.Absolute);
      }
      catch (UriFormatException ex)
      {
        throw new RequirementParseException(ex.Message, (Exception) ex);
      }
    }

    public ITracer Tracer
    {
      get => this.tracer;
      set => this.tracer = value ?? (ITracer) NullTracer.Instance;
    }

    public RequirementParserInternal.ExportedRules Exported => this.exported ?? (this.exported = new RequirementParserInternal.ExportedRules(this));

    public RequirementSpec Parse(string subject, string fileName = null) => this.StartRuleHelper<RequirementSpec>(new Cursor(subject, fileName: fileName), new ParseDelegate<RequirementSpec>(this.Specification), "Specification").Value;

    private IParseResult<string> wsp(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (wsp), cursor);
      IParseResult<string> parseResult = this.ParseClass(ref cursor, "  \t\t");
      this.tracer.TraceRuleExit<string>(nameof (wsp), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<string> letter(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (letter), cursor);
      IParseResult<string> parseResult = this.ParseClass(ref cursor, "az", ignoreCase: true);
      this.tracer.TraceRuleExit<string>(nameof (letter), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<string> digit(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (digit), cursor);
      IParseResult<string> parseResult = this.ParseClass(ref cursor, "09");
      this.tracer.TraceRuleExit<string>(nameof (digit), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<string> letterOrDigit(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (letterOrDigit), cursor);
      IParseResult<string> parseResult = this.ParseClass(ref cursor, "az09", ignoreCase: true);
      this.tracer.TraceRuleExit<string>(nameof (letterOrDigit), cursor, parseResult);
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

    private IParseResult<Operator> version_cmp(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (version_cmp), cursor);
      IParseResult<Operator> parseResult1 = (IParseResult<Operator>) null;
      Cursor startCursor1 = cursor;
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
        IParseResult<string> result = ((((((((IParseResult<string>) null ?? this.ParseLiteral(ref cursor, "<=")) ?? this.ParseLiteral(ref cursor, "<")) ?? this.ParseLiteral(ref cursor, "!=")) ?? this.ParseLiteral(ref cursor, "==")) ?? this.ParseLiteral(ref cursor, ">=")) ?? this.ParseLiteral(ref cursor, ">")) ?? this.ParseLiteral(ref cursor, "~=")) ?? this.ParseLiteral(ref cursor, "===");
        string op = this.ValueOrDefault<string>(result);
        if (result != null)
          parseResult1 = this.ReturnHelper<Operator>(startCursor1, ref cursor, (Func<Cursor, Operator>) (state => new Operator(op)));
        else
          cursor = startCursor1;
      }
      else
        cursor = startCursor1;
      this.tracer.TraceRuleExit<Operator>(nameof (version_cmp), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<Operator> legacy_version_cmp(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (legacy_version_cmp), cursor);
      IParseResult<Operator> parseResult1 = (IParseResult<Operator>) null;
      Cursor startCursor1 = cursor;
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
        IParseResult<string> result = ((((((IParseResult<string>) null ?? this.ParseLiteral(ref cursor, "<=")) ?? this.ParseLiteral(ref cursor, "<")) ?? this.ParseLiteral(ref cursor, "!=")) ?? this.ParseLiteral(ref cursor, "==")) ?? this.ParseLiteral(ref cursor, ">=")) ?? this.ParseLiteral(ref cursor, ">");
        string op = this.ValueOrDefault<string>(result);
        if (result != null)
          parseResult1 = this.ReturnHelper<Operator>(startCursor1, ref cursor, (Func<Cursor, Operator>) (state => new Operator(op)));
        else
          cursor = startCursor1;
      }
      else
        cursor = startCursor1;
      this.tracer.TraceRuleExit<Operator>(nameof (legacy_version_cmp), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<VersionIdentifier> version(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (version), cursor);
      IParseResult<VersionIdentifier> parseResult1 = (IParseResult<VersionIdentifier>) null;
      Cursor startCursor1 = cursor;
      Cursor startCursor3 = cursor;
      List<string> l0 = new List<string>();
      while (true)
      {
        IParseResult<string> parseResult2 = this.wsp(ref cursor);
        if (parseResult2 != null)
          l0.Add(parseResult2.Value);
        else
          break;
      }
      if (this.ReturnHelper<IList<string>>(startCursor3, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly())) != null)
      {
        IParseResult<string> result = (IParseResult<string>) null;
        Cursor startCursor2 = cursor;
        IParseResult<IList<string>> parseResult3 = (IParseResult<IList<string>>) null;
        Cursor startCursor4 = cursor;
        List<string> l1 = new List<string>();
        while (true)
        {
          IParseResult<string> parseResult4 = (((((((IParseResult<string>) null ?? this.letterOrDigit(ref cursor)) ?? this.ParseLiteral(ref cursor, "-")) ?? this.ParseLiteral(ref cursor, "_")) ?? this.ParseLiteral(ref cursor, ".")) ?? this.ParseLiteral(ref cursor, "*")) ?? this.ParseLiteral(ref cursor, "+")) ?? this.ParseLiteral(ref cursor, "!");
          if (parseResult4 != null)
            l1.Add(parseResult4.Value);
          else
            break;
        }
        if (l1.Count >= 1)
          parseResult3 = this.ReturnHelper<IList<string>>(startCursor4, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l1.AsReadOnly()));
        else
          cursor = startCursor4;
        if (parseResult3 != null)
        {
          int len = cursor.Location - startCursor2.Location;
          result = this.ReturnHelper<string>(startCursor2, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor2.Location, len)));
        }
        else
          cursor = startCursor2;
        string identifier = this.ValueOrDefault<string>(result);
        if (result != null)
          parseResult1 = this.ReturnHelper<VersionIdentifier>(startCursor1, ref cursor, (Func<Cursor, VersionIdentifier>) (state => (VersionIdentifier) new Pep440VersionIdentifier(identifier)));
        else
          cursor = startCursor1;
      }
      else
        cursor = startCursor1;
      this.tracer.TraceRuleExit<VersionIdentifier>(nameof (version), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<VersionIdentifier> legacy_version(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (legacy_version), cursor);
      IParseResult<VersionIdentifier> parseResult1 = (IParseResult<VersionIdentifier>) null;
      Cursor startCursor1 = cursor;
      Cursor startCursor3 = cursor;
      List<string> l0 = new List<string>();
      while (true)
      {
        IParseResult<string> parseResult2 = this.wsp(ref cursor);
        if (parseResult2 != null)
          l0.Add(parseResult2.Value);
        else
          break;
      }
      if (this.ReturnHelper<IList<string>>(startCursor3, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly())) != null)
      {
        IParseResult<string> result = (IParseResult<string>) null;
        Cursor startCursor2 = cursor;
        Cursor startCursor4 = cursor;
        List<string> l1 = new List<string>();
        while (true)
        {
          IParseResult<string> parseResult3 = this.ParseClass(ref cursor, ",,;;  \t\t))", true);
          if (parseResult3 != null)
            l1.Add(parseResult3.Value);
          else
            break;
        }
        if (this.ReturnHelper<IList<string>>(startCursor4, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l1.AsReadOnly())) != null)
        {
          int len = cursor.Location - startCursor2.Location;
          result = this.ReturnHelper<string>(startCursor2, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor2.Location, len)));
        }
        else
          cursor = startCursor2;
        string identifier = this.ValueOrDefault<string>(result);
        if (result != null)
          parseResult1 = this.ReturnHelper<VersionIdentifier>(startCursor1, ref cursor, (Func<Cursor, VersionIdentifier>) (state => (VersionIdentifier) new LegacyVersionIdentifier(identifier)));
        else
          cursor = startCursor1;
      }
      else
        cursor = startCursor1;
      this.tracer.TraceRuleExit<VersionIdentifier>(nameof (legacy_version), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<VersionConstraint> Pep440VersionOne(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (Pep440VersionOne), cursor);
      IParseResult<VersionConstraint> parseResult1 = (IParseResult<VersionConstraint>) null;
      Cursor startCursor1 = cursor;
      IParseResult<Operator> result1 = this.version_cmp(ref cursor);
      Operator op = this.ValueOrDefault<Operator>(result1);
      if (result1 != null)
      {
        IParseResult<VersionIdentifier> result2 = this.version(ref cursor);
        VersionIdentifier v = this.ValueOrDefault<VersionIdentifier>(result2);
        if (result2 != null)
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
            parseResult1 = this.ReturnHelper<VersionConstraint>(startCursor1, ref cursor, (Func<Cursor, VersionConstraint>) (state => new VersionConstraint(op, v)));
          else
            cursor = startCursor1;
        }
        else
          cursor = startCursor1;
      }
      else
        cursor = startCursor1;
      this.tracer.TraceRuleExit<VersionConstraint>(nameof (Pep440VersionOne), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<VersionConstraint> LegacyVersionOne(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (LegacyVersionOne), cursor);
      IParseResult<VersionConstraint> parseResult1 = (IParseResult<VersionConstraint>) null;
      Cursor startCursor1 = cursor;
      IParseResult<Operator> result1 = this.legacy_version_cmp(ref cursor);
      Operator op = this.ValueOrDefault<Operator>(result1);
      if (result1 != null)
      {
        IParseResult<VersionIdentifier> result2 = this.legacy_version(ref cursor);
        VersionIdentifier v = this.ValueOrDefault<VersionIdentifier>(result2);
        if (result2 != null)
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
            parseResult1 = this.ReturnHelper<VersionConstraint>(startCursor1, ref cursor, (Func<Cursor, VersionConstraint>) (state => new VersionConstraint(op, v)));
          else
            cursor = startCursor1;
        }
        else
          cursor = startCursor1;
      }
      else
        cursor = startCursor1;
      this.tracer.TraceRuleExit<VersionConstraint>(nameof (LegacyVersionOne), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<VersionConstraint> version_one(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (version_one), cursor);
      IParseResult<VersionConstraint> helper = this.ParseHelper<VersionConstraint>(ref cursor, (ParseDelegate<VersionConstraint>) ((ref Cursor state) => this.LongestSubsequence<VersionConstraint>(ref state, new ParseDelegate<VersionConstraint>(this.Pep440VersionOne), new ParseDelegate<VersionConstraint>(this.LegacyVersionOne))));
      this.tracer.TraceRuleExit<VersionConstraint>(nameof (version_one), cursor, helper);
      return helper;
    }

    private IParseResult<Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList> VersionConstraintList(
      ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (VersionConstraintList), cursor);
      IParseResult<Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList> parseResult1 = (IParseResult<Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList>) null;
      Cursor startCursor1 = cursor;
      IParseResult<VersionConstraint> result1 = this.version_one(ref cursor);
      VersionConstraint first = this.ValueOrDefault<VersionConstraint>(result1);
      if (result1 != null)
      {
        Cursor startCursor2 = cursor;
        List<VersionConstraint> l0 = new List<VersionConstraint>();
        while (true)
        {
          IParseResult<VersionConstraint> parseResult2 = (IParseResult<VersionConstraint>) null;
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
              IParseResult<VersionConstraint> result2 = this.version_one(ref cursor);
              VersionConstraint v = this.ValueOrDefault<VersionConstraint>(result2);
              if (result2 != null)
                parseResult2 = this.ReturnHelper<VersionConstraint>(startCursor3, ref cursor, (Func<Cursor, VersionConstraint>) (state => v));
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
        IParseResult<IList<VersionConstraint>> result3 = this.ReturnHelper<IList<VersionConstraint>>(startCursor2, ref cursor, (Func<Cursor, IList<VersionConstraint>>) (state => (IList<VersionConstraint>) l0.AsReadOnly()));
        IList<VersionConstraint> rest = this.ValueOrDefault<IList<VersionConstraint>>(result3);
        if (result3 != null)
          parseResult1 = this.ReturnHelper<Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList>(startCursor1, ref cursor, (Func<Cursor, Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList>) (state => new Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList((IEnumerable<VersionConstraint>) ImmutableList.Create<VersionConstraint>(first).AddRange((IEnumerable<VersionConstraint>) rest))));
        else
          cursor = startCursor1;
      }
      else
        cursor = startCursor1;
      this.tracer.TraceRuleExit<Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList>(nameof (VersionConstraintList), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList> VersionSpec(
      ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (VersionSpec), cursor);
      IParseResult<Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList> parseResult1 = (IParseResult<Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList>) null;
      if (parseResult1 == null)
      {
        Cursor startCursor = cursor;
        if (this.ParseLiteral(ref cursor, "(") != null)
        {
          IParseResult<Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList> result = this.VersionConstraintList(ref cursor);
          Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList v = this.ValueOrDefault<Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList>(result);
          if (result != null)
          {
            IParseResult<string> parseResult2 = (IParseResult<string>) null ?? this.ParseLiteral(ref cursor, ")");
            if (parseResult2 == null)
              throw this.ExceptionHelper(cursor, (Func<Cursor, RequirementParserInternal.UnparseableSpecError>) (state => new RequirementParserInternal.UnparseableSpecError()));
            if (parseResult2 != null)
              parseResult1 = this.ReturnHelper<Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList>(startCursor, ref cursor, (Func<Cursor, Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList>) (state => v));
            else
              cursor = startCursor;
          }
          else
            cursor = startCursor;
        }
        else
          cursor = startCursor;
      }
      if (parseResult1 == null)
        parseResult1 = this.VersionConstraintList(ref cursor);
      if (parseResult1 == null)
        parseResult1 = this.ReturnHelper<Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList>(cursor, ref cursor, (Func<Cursor, Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList>) (state => (Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList) null));
      this.tracer.TraceRuleExit<Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList>(nameof (VersionSpec), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<UrlSpec> urlspec(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (urlspec), cursor);
      IParseResult<UrlSpec> parseResult1 = (IParseResult<UrlSpec>) null;
      Cursor startCursor1 = cursor;
      if (this.ParseLiteral(ref cursor, "@") != null)
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
          IParseResult<string> result = this.URI_reference(ref cursor);
          string uri = this.ValueOrDefault<string>(result);
          if (result != null)
            parseResult1 = this.ReturnHelper<UrlSpec>(startCursor1, ref cursor, (Func<Cursor, UrlSpec>) (state => new UrlSpec(this.ConvertUri(uri))));
          else
            cursor = startCursor1;
        }
        else
          cursor = startCursor1;
      }
      else
        cursor = startCursor1;
      this.tracer.TraceRuleExit<UrlSpec>(nameof (urlspec), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<Operator> marker_op(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (marker_op), cursor);
      IParseResult<Operator> parseResult1 = (IParseResult<Operator>) null ?? this.version_cmp(ref cursor);
      if (parseResult1 == null)
      {
        Cursor startCursor1 = cursor;
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
          if (this.ParseLiteral(ref cursor, "in") != null)
            parseResult1 = this.ReturnHelper<Operator>(startCursor1, ref cursor, (Func<Cursor, Operator>) (state => new Operator("in")));
          else
            cursor = startCursor1;
        }
        else
          cursor = startCursor1;
      }
      if (parseResult1 == null)
      {
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
          if (this.ParseLiteral(ref cursor, "not") != null)
          {
            IParseResult<IList<string>> parseResult4 = (IParseResult<IList<string>>) null;
            Cursor startCursor5 = cursor;
            List<string> l2 = new List<string>();
            while (true)
            {
              IParseResult<string> parseResult5 = this.wsp(ref cursor);
              if (parseResult5 != null)
                l2.Add(parseResult5.Value);
              else
                break;
            }
            if (l2.Count >= 1)
              parseResult4 = this.ReturnHelper<IList<string>>(startCursor5, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l2.AsReadOnly()));
            else
              cursor = startCursor5;
            if (parseResult4 != null)
            {
              if (this.ParseLiteral(ref cursor, "in") != null)
                parseResult1 = this.ReturnHelper<Operator>(startCursor3, ref cursor, (Func<Cursor, Operator>) (state => new Operator("not in")));
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
      }
      this.tracer.TraceRuleExit<Operator>(nameof (marker_op), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> python_str_c(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (python_str_c), cursor);
      IParseResult<string> parseResult = ((((((((((((((((((((((((((((((((IParseResult<string>) null ?? this.wsp(ref cursor)) ?? this.letter(ref cursor)) ?? this.digit(ref cursor)) ?? this.ParseLiteral(ref cursor, "(")) ?? this.ParseLiteral(ref cursor, ")")) ?? this.ParseLiteral(ref cursor, ".")) ?? this.ParseLiteral(ref cursor, "{")) ?? this.ParseLiteral(ref cursor, "}")) ?? this.ParseLiteral(ref cursor, "-")) ?? this.ParseLiteral(ref cursor, "_")) ?? this.ParseLiteral(ref cursor, "*")) ?? this.ParseLiteral(ref cursor, "#")) ?? this.ParseLiteral(ref cursor, ":")) ?? this.ParseLiteral(ref cursor, ";")) ?? this.ParseLiteral(ref cursor, ",")) ?? this.ParseLiteral(ref cursor, "/")) ?? this.ParseLiteral(ref cursor, "?")) ?? this.ParseLiteral(ref cursor, "[")) ?? this.ParseLiteral(ref cursor, "]")) ?? this.ParseLiteral(ref cursor, "!")) ?? this.ParseLiteral(ref cursor, "~")) ?? this.ParseLiteral(ref cursor, "`")) ?? this.ParseLiteral(ref cursor, "@")) ?? this.ParseLiteral(ref cursor, "$")) ?? this.ParseLiteral(ref cursor, "%")) ?? this.ParseLiteral(ref cursor, "^")) ?? this.ParseLiteral(ref cursor, "&")) ?? this.ParseLiteral(ref cursor, "=")) ?? this.ParseLiteral(ref cursor, "+")) ?? this.ParseLiteral(ref cursor, "|")) ?? this.ParseLiteral(ref cursor, "<")) ?? this.ParseLiteral(ref cursor, ">");
      this.tracer.TraceRuleExit<string>(nameof (python_str_c), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<string> dquote(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (dquote), cursor);
      IParseResult<string> literal = this.ParseLiteral(ref cursor, "\"");
      this.tracer.TraceRuleExit<string>(nameof (dquote), cursor, literal);
      return literal;
    }

    private IParseResult<string> squote(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (squote), cursor);
      IParseResult<string> literal = this.ParseLiteral(ref cursor, "'");
      this.tracer.TraceRuleExit<string>(nameof (squote), cursor, literal);
      return literal;
    }

    private IParseResult<PythonString> python_str(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (python_str), cursor);
      IParseResult<PythonString> parseResult1 = (IParseResult<PythonString>) null;
      Cursor startCursor1 = cursor;
      IParseResult<string> result1 = (IParseResult<string>) null;
      if (result1 == null)
      {
        Cursor startCursor3 = cursor;
        if (this.squote(ref cursor) != null)
        {
          IParseResult<string> result2 = (IParseResult<string>) null;
          Cursor startCursor2 = cursor;
          Cursor startCursor4 = cursor;
          List<string> l0 = new List<string>();
          while (true)
          {
            IParseResult<string> parseResult2 = ((IParseResult<string>) null ?? this.python_str_c(ref cursor)) ?? this.dquote(ref cursor);
            if (parseResult2 != null)
              l0.Add(parseResult2.Value);
            else
              break;
          }
          if (this.ReturnHelper<IList<string>>(startCursor4, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly())) != null)
          {
            int len = cursor.Location - startCursor2.Location;
            result2 = this.ReturnHelper<string>(startCursor2, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor2.Location, len)));
          }
          else
            cursor = startCursor2;
          string s = this.ValueOrDefault<string>(result2);
          if (result2 != null)
          {
            if (this.squote(ref cursor) != null)
              result1 = this.ReturnHelper<string>(startCursor3, ref cursor, (Func<Cursor, string>) (state => s));
            else
              cursor = startCursor3;
          }
          else
            cursor = startCursor3;
        }
        else
          cursor = startCursor3;
      }
      if (result1 == null)
      {
        Cursor startCursor6 = cursor;
        if (this.dquote(ref cursor) != null)
        {
          IParseResult<string> result3 = (IParseResult<string>) null;
          Cursor startCursor5 = cursor;
          Cursor startCursor7 = cursor;
          List<string> l1 = new List<string>();
          while (true)
          {
            IParseResult<string> parseResult3 = ((IParseResult<string>) null ?? this.python_str_c(ref cursor)) ?? this.squote(ref cursor);
            if (parseResult3 != null)
              l1.Add(parseResult3.Value);
            else
              break;
          }
          if (this.ReturnHelper<IList<string>>(startCursor7, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l1.AsReadOnly())) != null)
          {
            int len = cursor.Location - startCursor5.Location;
            result3 = this.ReturnHelper<string>(startCursor5, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor5.Location, len)));
          }
          else
            cursor = startCursor5;
          string s = this.ValueOrDefault<string>(result3);
          if (result3 != null)
          {
            if (this.dquote(ref cursor) != null)
              result1 = this.ReturnHelper<string>(startCursor6, ref cursor, (Func<Cursor, string>) (state => s));
            else
              cursor = startCursor6;
          }
          else
            cursor = startCursor6;
        }
        else
          cursor = startCursor6;
      }
      string val = this.ValueOrDefault<string>(result1);
      if (result1 != null)
        parseResult1 = this.ReturnHelper<PythonString>(startCursor1, ref cursor, (Func<Cursor, PythonString>) (state => new PythonString(val)));
      else
        cursor = startCursor1;
      this.tracer.TraceRuleExit<PythonString>(nameof (python_str), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<MarkerEnvVar> env_var(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (env_var), cursor);
      IParseResult<MarkerEnvVar> parseResult = (IParseResult<MarkerEnvVar>) null;
      Cursor startCursor = cursor;
      IParseResult<string> result = ((((((((((((((((((IParseResult<string>) null ?? this.ParseLiteral(ref cursor, "python_version")) ?? this.ParseLiteral(ref cursor, "python_full_version")) ?? this.ParseLiteral(ref cursor, "os_name")) ?? this.ParseLiteral(ref cursor, "sys_platform")) ?? this.ParseLiteral(ref cursor, "platform_release")) ?? this.ParseLiteral(ref cursor, "platform_system")) ?? this.ParseLiteral(ref cursor, "platform_version")) ?? this.ParseLiteral(ref cursor, "platform_machine")) ?? this.ParseLiteral(ref cursor, "platform_python_implementation")) ?? this.ParseLiteral(ref cursor, "implementation_name")) ?? this.ParseLiteral(ref cursor, "implementation_version")) ?? this.ParseLiteral(ref cursor, "extra")) ?? this.ParseLiteral(ref cursor, "os.name")) ?? this.ParseLiteral(ref cursor, "sys.platform")) ?? this.ParseLiteral(ref cursor, "platform.version")) ?? this.ParseLiteral(ref cursor, "platform.machine")) ?? this.ParseLiteral(ref cursor, "platform.python_implementation")) ?? this.ParseLiteral(ref cursor, "python_implementation");
      string varname = this.ValueOrDefault<string>(result);
      if (result != null)
        parseResult = this.ReturnHelper<MarkerEnvVar>(startCursor, ref cursor, (Func<Cursor, MarkerEnvVar>) (state => new MarkerEnvVar(varname)));
      else
        cursor = startCursor;
      this.tracer.TraceRuleExit<MarkerEnvVar>(nameof (env_var), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<MarkerVariable> marker_var(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (marker_var), cursor);
      IParseResult<MarkerVariable> parseResult1 = (IParseResult<MarkerVariable>) null;
      Cursor startCursor1 = cursor;
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
        IParseResult<MarkerVariable> result1 = (IParseResult<MarkerVariable>) null;
        if (result1 == null)
        {
          Cursor startCursor3 = cursor;
          IParseResult<MarkerEnvVar> result2 = this.env_var(ref cursor);
          MarkerEnvVar env = this.ValueOrDefault<MarkerEnvVar>(result2);
          if (result2 != null)
            result1 = this.ReturnHelper<MarkerVariable>(startCursor3, ref cursor, (Func<Cursor, MarkerVariable>) (state => new MarkerVariable(env)));
          else
            cursor = startCursor3;
        }
        if (result1 == null)
        {
          Cursor startCursor4 = cursor;
          IParseResult<PythonString> result3 = this.python_str(ref cursor);
          PythonString str = this.ValueOrDefault<PythonString>(result3);
          if (result3 != null)
            result1 = this.ReturnHelper<MarkerVariable>(startCursor4, ref cursor, (Func<Cursor, MarkerVariable>) (state => new MarkerVariable(str)));
          else
            cursor = startCursor4;
        }
        if (result1 == null)
        {
          Cursor cursor1 = cursor;
          Cursor idStart = cursor;
          IParseResult<Identifier> result4 = this.identifier(ref cursor);
          Identifier id = this.ValueOrDefault<Identifier>(result4);
          if (result4 != null)
            throw this.ExceptionHelper(cursor, (Func<Cursor, RequirementParserInternal.UnknownEnvVarError>) (state => new RequirementParserInternal.UnknownEnvVarError(id, idStart)));
          cursor = cursor1;
        }
        MarkerVariable v = this.ValueOrDefault<MarkerVariable>(result1);
        if (result1 != null)
          parseResult1 = this.ReturnHelper<MarkerVariable>(startCursor1, ref cursor, (Func<Cursor, MarkerVariable>) (state => v));
        else
          cursor = startCursor1;
      }
      else
        cursor = startCursor1;
      this.tracer.TraceRuleExit<MarkerVariable>(nameof (marker_var), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<MarkerExpression> marker_expr(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (marker_expr), cursor);
      IParseResult<MarkerExpression> parseResult1 = (IParseResult<MarkerExpression>) null;
      if (parseResult1 == null)
      {
        Cursor startCursor = cursor;
        IParseResult<MarkerVariable> result1 = this.marker_var(ref cursor);
        MarkerVariable l = this.ValueOrDefault<MarkerVariable>(result1);
        if (result1 != null)
        {
          IParseResult<Operator> result2 = this.marker_op(ref cursor);
          Operator o = this.ValueOrDefault<Operator>(result2);
          if (result2 != null)
          {
            IParseResult<MarkerVariable> result3 = this.marker_var(ref cursor);
            MarkerVariable r = this.ValueOrDefault<MarkerVariable>(result3);
            if (result3 != null)
              parseResult1 = this.ReturnHelper<MarkerExpression>(startCursor, ref cursor, (Func<Cursor, MarkerExpression>) (state => (MarkerExpression) new MarkerBinaryExpression((MarkerExpression) l, o, (MarkerExpression) r)));
            else
              cursor = startCursor;
          }
          else
            cursor = startCursor;
        }
        else
          cursor = startCursor;
      }
      if (parseResult1 == null)
      {
        Cursor startCursor1 = cursor;
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
          if (this.ParseLiteral(ref cursor, "(") != null)
          {
            IParseResult<MarkerExpression> result = this.marker_or(ref cursor);
            MarkerExpression m = this.ValueOrDefault<MarkerExpression>(result);
            if (result != null)
            {
              Cursor startCursor3 = cursor;
              List<string> l1 = new List<string>();
              while (true)
              {
                IParseResult<string> parseResult3 = this.wsp(ref cursor);
                if (parseResult3 != null)
                  l1.Add(parseResult3.Value);
                else
                  break;
              }
              if (this.ReturnHelper<IList<string>>(startCursor3, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l1.AsReadOnly())) != null)
              {
                if (this.ParseLiteral(ref cursor, ")") != null)
                  parseResult1 = this.ReturnHelper<MarkerExpression>(startCursor1, ref cursor, (Func<Cursor, MarkerExpression>) (state => m));
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
        }
        else
          cursor = startCursor1;
      }
      this.tracer.TraceRuleExit<MarkerExpression>(nameof (marker_expr), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<MarkerExpression> marker_and(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (marker_and), cursor);
      IParseResult<MarkerExpression> parseResult1 = (IParseResult<MarkerExpression>) null;
      CacheKey cacheKey = new CacheKey(nameof (marker_and), cursor.StateKey, cursor.Location);
      if (this.storage.ContainsKey(cacheKey))
      {
        parseResult1 = (IParseResult<MarkerExpression>) this.storage[cacheKey];
        this.tracer.TraceCacheHit<MarkerExpression>(nameof (marker_and), cursor, cacheKey, parseResult1);
        if (parseResult1 != null)
          cursor = parseResult1.EndCursor;
      }
      else
      {
        this.tracer.TraceCacheMiss(nameof (marker_and), cursor, cacheKey);
        this.tracer.TraceInfo(nameof (marker_and), cursor, "Seeding left-recursion with an unsuccessful match.");
        this.storage[cacheKey] = (object) null;
        Cursor cursor1 = cursor;
        while (true)
        {
          IParseResult<MarkerExpression> parseResult2 = (IParseResult<MarkerExpression>) null;
          if (parseResult2 == null)
          {
            Cursor startCursor1 = cursor;
            IParseResult<MarkerExpression> result1 = this.marker_and(ref cursor);
            MarkerExpression l = this.ValueOrDefault<MarkerExpression>(result1);
            if (result1 != null)
            {
              Cursor startCursor2 = cursor;
              List<string> l0 = new List<string>();
              while (true)
              {
                IParseResult<string> parseResult3 = this.wsp(ref cursor);
                if (parseResult3 != null)
                  l0.Add(parseResult3.Value);
                else
                  break;
              }
              if (this.ReturnHelper<IList<string>>(startCursor2, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly())) != null)
              {
                if (this.ParseLiteral(ref cursor, "and") != null)
                {
                  IParseResult<MarkerExpression> result2 = this.marker_expr(ref cursor);
                  MarkerExpression r = this.ValueOrDefault<MarkerExpression>(result2);
                  if (result2 != null)
                    parseResult2 = this.ReturnHelper<MarkerExpression>(startCursor1, ref cursor, (Func<Cursor, MarkerExpression>) (state => (MarkerExpression) new MarkerBinaryExpression(l, new Operator("and"), r)));
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
          }
          if (parseResult2 == null)
          {
            Cursor startCursor = cursor;
            IParseResult<MarkerExpression> result = this.marker_expr(ref cursor);
            MarkerExpression m = this.ValueOrDefault<MarkerExpression>(result);
            if (result != null)
              parseResult2 = this.ReturnHelper<MarkerExpression>(startCursor, ref cursor, (Func<Cursor, MarkerExpression>) (state => m));
            else
              cursor = startCursor;
          }
          if (parseResult2 != null && (parseResult1 == null || parseResult1.EndCursor.Location < parseResult2.EndCursor.Location))
          {
            this.tracer.TraceInfo(nameof (marker_and), cursor, "Caching result and retrying.");
            this.storage[cacheKey] = (object) (parseResult1 = parseResult2);
            cursor = cursor1;
          }
          else
            break;
        }
        this.tracer.TraceInfo(nameof (marker_and), cursor, "No forward progress made, current cache entry will be kept.");
        if (parseResult1 != null)
          cursor = parseResult1.EndCursor;
      }
      this.tracer.TraceRuleExit<MarkerExpression>(nameof (marker_and), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<MarkerExpression> marker_or(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (marker_or), cursor);
      IParseResult<MarkerExpression> parseResult1 = (IParseResult<MarkerExpression>) null;
      CacheKey cacheKey = new CacheKey(nameof (marker_or), cursor.StateKey, cursor.Location);
      if (this.storage.ContainsKey(cacheKey))
      {
        parseResult1 = (IParseResult<MarkerExpression>) this.storage[cacheKey];
        this.tracer.TraceCacheHit<MarkerExpression>(nameof (marker_or), cursor, cacheKey, parseResult1);
        if (parseResult1 != null)
          cursor = parseResult1.EndCursor;
      }
      else
      {
        this.tracer.TraceCacheMiss(nameof (marker_or), cursor, cacheKey);
        this.tracer.TraceInfo(nameof (marker_or), cursor, "Seeding left-recursion with an unsuccessful match.");
        this.storage[cacheKey] = (object) null;
        Cursor cursor1 = cursor;
        while (true)
        {
          IParseResult<MarkerExpression> parseResult2 = (IParseResult<MarkerExpression>) null;
          if (parseResult2 == null)
          {
            Cursor startCursor1 = cursor;
            IParseResult<MarkerExpression> result1 = this.marker_or(ref cursor);
            MarkerExpression l = this.ValueOrDefault<MarkerExpression>(result1);
            if (result1 != null)
            {
              Cursor startCursor2 = cursor;
              List<string> l0 = new List<string>();
              while (true)
              {
                IParseResult<string> parseResult3 = this.wsp(ref cursor);
                if (parseResult3 != null)
                  l0.Add(parseResult3.Value);
                else
                  break;
              }
              if (this.ReturnHelper<IList<string>>(startCursor2, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly())) != null)
              {
                if (this.ParseLiteral(ref cursor, "or") != null)
                {
                  IParseResult<MarkerExpression> result2 = this.marker_and(ref cursor);
                  MarkerExpression r = this.ValueOrDefault<MarkerExpression>(result2);
                  if (result2 != null)
                    parseResult2 = this.ReturnHelper<MarkerExpression>(startCursor1, ref cursor, (Func<Cursor, MarkerExpression>) (state => (MarkerExpression) new MarkerBinaryExpression(l, new Operator("or"), r)));
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
          }
          if (parseResult2 == null)
          {
            Cursor startCursor = cursor;
            IParseResult<MarkerExpression> result = this.marker_and(ref cursor);
            MarkerExpression m = this.ValueOrDefault<MarkerExpression>(result);
            if (result != null)
              parseResult2 = this.ReturnHelper<MarkerExpression>(startCursor, ref cursor, (Func<Cursor, MarkerExpression>) (state => m));
            else
              cursor = startCursor;
          }
          if (parseResult2 != null && (parseResult1 == null || parseResult1.EndCursor.Location < parseResult2.EndCursor.Location))
          {
            this.tracer.TraceInfo(nameof (marker_or), cursor, "Caching result and retrying.");
            this.storage[cacheKey] = (object) (parseResult1 = parseResult2);
            cursor = cursor1;
          }
          else
            break;
        }
        this.tracer.TraceInfo(nameof (marker_or), cursor, "No forward progress made, current cache entry will be kept.");
        if (parseResult1 != null)
          cursor = parseResult1.EndCursor;
      }
      this.tracer.TraceRuleExit<MarkerExpression>(nameof (marker_or), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<MarkerExpression> marker(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (marker), cursor);
      IParseResult<MarkerExpression> parseResult = this.marker_or(ref cursor);
      this.tracer.TraceRuleExit<MarkerExpression>(nameof (marker), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<MarkerExpression> quoted_marker(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (quoted_marker), cursor);
      IParseResult<MarkerExpression> parseResult1 = (IParseResult<MarkerExpression>) null;
      Cursor startCursor1 = cursor;
      if (this.ParseLiteral(ref cursor, ";") != null)
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
          IParseResult<MarkerExpression> result = this.marker(ref cursor);
          MarkerExpression m = this.ValueOrDefault<MarkerExpression>(result);
          if (result != null)
            parseResult1 = this.ReturnHelper<MarkerExpression>(startCursor1, ref cursor, (Func<Cursor, MarkerExpression>) (state => m));
          else
            cursor = startCursor1;
        }
        else
          cursor = startCursor1;
      }
      else
        cursor = startCursor1;
      this.tracer.TraceRuleExit<MarkerExpression>(nameof (quoted_marker), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> identifier_end(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (identifier_end), cursor);
      IParseResult<string> parseResult1 = (IParseResult<string>) null ?? this.letterOrDigit(ref cursor);
      if (parseResult1 == null)
      {
        Cursor startCursor0 = cursor;
        Cursor startCursor = cursor;
        List<string> l0 = new List<string>();
        while (true)
        {
          IParseResult<string> parseResult2 = (((IParseResult<string>) null ?? this.ParseLiteral(ref cursor, "-")) ?? this.ParseLiteral(ref cursor, "_")) ?? this.ParseLiteral(ref cursor, ".");
          if (parseResult2 != null)
            l0.Add(parseResult2.Value);
          else
            break;
        }
        if (this.ReturnHelper<IList<string>>(startCursor, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly())) != null)
        {
          if (this.letterOrDigit(ref cursor) != null)
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
      this.tracer.TraceRuleExit<string>(nameof (identifier_end), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<Identifier> identifier(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (identifier), cursor);
      IParseResult<Identifier> parseResult1 = (IParseResult<Identifier>) null;
      Cursor startCursor2 = cursor;
      IParseResult<string> result = (IParseResult<string>) null;
      Cursor startCursor1 = cursor;
      if (this.letterOrDigit(ref cursor) != null)
      {
        Cursor startCursor3 = cursor;
        List<string> l0 = new List<string>();
        while (true)
        {
          IParseResult<string> parseResult2 = this.identifier_end(ref cursor);
          if (parseResult2 != null)
            l0.Add(parseResult2.Value);
          else
            break;
        }
        if (this.ReturnHelper<IList<string>>(startCursor3, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly())) != null)
        {
          int len = cursor.Location - startCursor1.Location;
          result = this.ReturnHelper<string>(startCursor1, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor1.Location, len)));
        }
        else
          cursor = startCursor1;
      }
      else
        cursor = startCursor1;
      string name = this.ValueOrDefault<string>(result);
      if (result != null)
        parseResult1 = this.ReturnHelper<Identifier>(startCursor2, ref cursor, (Func<Cursor, Identifier>) (state => new Identifier(name)));
      else
        cursor = startCursor2;
      this.tracer.TraceRuleExit<Identifier>(nameof (identifier), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<Identifier> name(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (name), cursor);
      IParseResult<Identifier> parseResult = this.identifier(ref cursor);
      this.tracer.TraceRuleExit<Identifier>(nameof (name), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<ExtrasList> extras_list(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (extras_list), cursor);
      IParseResult<ExtrasList> parseResult1 = (IParseResult<ExtrasList>) null;
      Cursor startCursor1 = cursor;
      IParseResult<Identifier> result1 = this.identifier(ref cursor);
      Identifier first = this.ValueOrDefault<Identifier>(result1);
      if (result1 != null)
      {
        Cursor startCursor2 = cursor;
        List<Identifier> l0 = new List<Identifier>();
        while (true)
        {
          IParseResult<Identifier> parseResult2 = (IParseResult<Identifier>) null;
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
                IParseResult<Identifier> result2 = this.identifier(ref cursor);
                Identifier i = this.ValueOrDefault<Identifier>(result2);
                if (result2 != null)
                  parseResult2 = this.ReturnHelper<Identifier>(startCursor3, ref cursor, (Func<Cursor, Identifier>) (state => i));
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
        IParseResult<IList<Identifier>> result3 = this.ReturnHelper<IList<Identifier>>(startCursor2, ref cursor, (Func<Cursor, IList<Identifier>>) (state => (IList<Identifier>) l0.AsReadOnly()));
        IList<Identifier> rest = this.ValueOrDefault<IList<Identifier>>(result3);
        if (result3 != null)
          parseResult1 = this.ReturnHelper<ExtrasList>(startCursor1, ref cursor, (Func<Cursor, ExtrasList>) (state => new ExtrasList((IEnumerable<Identifier>) ImmutableList.Create<Identifier>(first).AddRange((IEnumerable<Identifier>) rest))));
        else
          cursor = startCursor1;
      }
      else
        cursor = startCursor1;
      this.tracer.TraceRuleExit<ExtrasList>(nameof (extras_list), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<ExtrasList> extras(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (extras), cursor);
      IParseResult<ExtrasList> parseResult1 = (IParseResult<ExtrasList>) null;
      Cursor startCursor1 = cursor;
      if (this.ParseLiteral(ref cursor, "[") != null)
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
          Cursor startCursor3 = cursor;
          List<ExtrasList> l1 = new List<ExtrasList>();
          while (l1.Count < 1)
          {
            IParseResult<ExtrasList> parseResult3 = this.extras_list(ref cursor);
            if (parseResult3 != null)
              l1.Add(parseResult3.Value);
            else
              break;
          }
          IParseResult<IList<ExtrasList>> result = this.ReturnHelper<IList<ExtrasList>>(startCursor3, ref cursor, (Func<Cursor, IList<ExtrasList>>) (state => (IList<ExtrasList>) l1.AsReadOnly()));
          IList<ExtrasList> e = this.ValueOrDefault<IList<ExtrasList>>(result);
          if (result != null)
          {
            Cursor startCursor4 = cursor;
            List<string> l2 = new List<string>();
            while (true)
            {
              IParseResult<string> parseResult4 = this.wsp(ref cursor);
              if (parseResult4 != null)
                l2.Add(parseResult4.Value);
              else
                break;
            }
            if (this.ReturnHelper<IList<string>>(startCursor4, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l2.AsReadOnly())) != null)
            {
              IParseResult<string> parseResult5 = (IParseResult<string>) null ?? this.ParseLiteral(ref cursor, "]");
              if (parseResult5 == null)
                throw this.ExceptionHelper(cursor, (Func<Cursor, RequirementParserInternal.UnparseableSpecError>) (state => new RequirementParserInternal.UnparseableSpecError()));
              if (parseResult5 != null)
                parseResult1 = this.ReturnHelper<ExtrasList>(startCursor1, ref cursor, (Func<Cursor, ExtrasList>) (state => e.SingleOrDefault<ExtrasList>()));
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
      }
      else
        cursor = startCursor1;
      this.tracer.TraceRuleExit<ExtrasList>(nameof (extras), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<RequirementSpec> name_req(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (name_req), cursor);
      IParseResult<RequirementSpec> parseResult1 = (IParseResult<RequirementSpec>) null;
      Cursor startCursor1 = cursor;
      IParseResult<Identifier> result1 = this.name(ref cursor);
      Identifier n = this.ValueOrDefault<Identifier>(result1);
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
          Cursor startCursor3 = cursor;
          List<ExtrasList> l1 = new List<ExtrasList>();
          while (l1.Count < 1)
          {
            IParseResult<ExtrasList> parseResult3 = this.extras(ref cursor);
            if (parseResult3 != null)
              l1.Add(parseResult3.Value);
            else
              break;
          }
          IParseResult<IList<ExtrasList>> result2 = this.ReturnHelper<IList<ExtrasList>>(startCursor3, ref cursor, (Func<Cursor, IList<ExtrasList>>) (state => (IList<ExtrasList>) l1.AsReadOnly()));
          IList<ExtrasList> e = this.ValueOrDefault<IList<ExtrasList>>(result2);
          if (result2 != null)
          {
            Cursor startCursor4 = cursor;
            List<string> l2 = new List<string>();
            while (true)
            {
              IParseResult<string> parseResult4 = this.wsp(ref cursor);
              if (parseResult4 != null)
                l2.Add(parseResult4.Value);
              else
                break;
            }
            if (this.ReturnHelper<IList<string>>(startCursor4, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l2.AsReadOnly())) != null)
            {
              IParseResult<Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList> result3 = this.VersionSpec(ref cursor);
              Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList v = this.ValueOrDefault<Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList>(result3);
              if (result3 != null)
              {
                Cursor startCursor5 = cursor;
                List<string> l3 = new List<string>();
                while (true)
                {
                  IParseResult<string> parseResult5 = this.wsp(ref cursor);
                  if (parseResult5 != null)
                    l3.Add(parseResult5.Value);
                  else
                    break;
                }
                if (this.ReturnHelper<IList<string>>(startCursor5, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l3.AsReadOnly())) != null)
                {
                  Cursor startCursor6 = cursor;
                  List<MarkerExpression> l4 = new List<MarkerExpression>();
                  while (l4.Count < 1)
                  {
                    IParseResult<MarkerExpression> parseResult6 = this.quoted_marker(ref cursor);
                    if (parseResult6 != null)
                      l4.Add(parseResult6.Value);
                    else
                      break;
                  }
                  IParseResult<IList<MarkerExpression>> result4 = this.ReturnHelper<IList<MarkerExpression>>(startCursor6, ref cursor, (Func<Cursor, IList<MarkerExpression>>) (state => (IList<MarkerExpression>) l4.AsReadOnly()));
                  IList<MarkerExpression> m = this.ValueOrDefault<IList<MarkerExpression>>(result4);
                  if (result4 != null)
                    parseResult1 = this.ReturnHelper<RequirementSpec>(startCursor1, ref cursor, (Func<Cursor, RequirementSpec>) (state => new RequirementSpec(n, e.SingleOrDefault<ExtrasList>(), v, m.SingleOrDefault<MarkerExpression>())));
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
          }
          else
            cursor = startCursor1;
        }
        else
          cursor = startCursor1;
      }
      else
        cursor = startCursor1;
      this.tracer.TraceRuleExit<RequirementSpec>(nameof (name_req), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<RequirementSpec> url_req(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (url_req), cursor);
      IParseResult<RequirementSpec> parseResult1 = (IParseResult<RequirementSpec>) null;
      Cursor startCursor1 = cursor;
      IParseResult<Identifier> result1 = this.name(ref cursor);
      Identifier n = this.ValueOrDefault<Identifier>(result1);
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
          Cursor startCursor3 = cursor;
          List<ExtrasList> l1 = new List<ExtrasList>();
          while (l1.Count < 1)
          {
            IParseResult<ExtrasList> parseResult3 = this.extras(ref cursor);
            if (parseResult3 != null)
              l1.Add(parseResult3.Value);
            else
              break;
          }
          IParseResult<IList<ExtrasList>> result2 = this.ReturnHelper<IList<ExtrasList>>(startCursor3, ref cursor, (Func<Cursor, IList<ExtrasList>>) (state => (IList<ExtrasList>) l1.AsReadOnly()));
          IList<ExtrasList> e = this.ValueOrDefault<IList<ExtrasList>>(result2);
          if (result2 != null)
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
              IParseResult<UrlSpec> result3 = this.urlspec(ref cursor);
              UrlSpec v = this.ValueOrDefault<UrlSpec>(result3);
              if (result3 != null)
              {
                IParseResult<string> parseResult5 = (IParseResult<string>) null;
                if (parseResult5 == null)
                {
                  Cursor startCursor4 = cursor;
                  IParseResult<IList<string>> parseResult6 = (IParseResult<IList<string>>) null;
                  Cursor startCursor6 = cursor;
                  List<string> l3 = new List<string>();
                  while (true)
                  {
                    IParseResult<string> parseResult7 = this.wsp(ref cursor);
                    if (parseResult7 != null)
                      l3.Add(parseResult7.Value);
                    else
                      break;
                  }
                  if (l3.Count >= 1)
                    parseResult6 = this.ReturnHelper<IList<string>>(startCursor6, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l3.AsReadOnly()));
                  else
                    cursor = startCursor6;
                  if (parseResult6 != null)
                  {
                    int len = cursor.Location - startCursor4.Location;
                    parseResult5 = this.ReturnHelper<string>(startCursor4, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor4.Location, len)));
                  }
                  else
                    cursor = startCursor4;
                }
                if (parseResult5 == null)
                  parseResult5 = this.end(ref cursor);
                if (parseResult5 != null)
                {
                  Cursor startCursor7 = cursor;
                  List<MarkerExpression> l4 = new List<MarkerExpression>();
                  while (l4.Count < 1)
                  {
                    IParseResult<MarkerExpression> parseResult8 = this.quoted_marker(ref cursor);
                    if (parseResult8 != null)
                      l4.Add(parseResult8.Value);
                    else
                      break;
                  }
                  IParseResult<IList<MarkerExpression>> result4 = this.ReturnHelper<IList<MarkerExpression>>(startCursor7, ref cursor, (Func<Cursor, IList<MarkerExpression>>) (state => (IList<MarkerExpression>) l4.AsReadOnly()));
                  IList<MarkerExpression> m = this.ValueOrDefault<IList<MarkerExpression>>(result4);
                  if (result4 != null)
                    parseResult1 = this.ReturnHelper<RequirementSpec>(startCursor1, ref cursor, (Func<Cursor, RequirementSpec>) (state => new RequirementSpec(n, e.SingleOrDefault<ExtrasList>(), v, m.SingleOrDefault<MarkerExpression>())));
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
          }
          else
            cursor = startCursor1;
        }
        else
          cursor = startCursor1;
      }
      else
        cursor = startCursor1;
      this.tracer.TraceRuleExit<RequirementSpec>(nameof (url_req), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<RequirementSpec> Specification(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (Specification), cursor);
      IParseResult<RequirementSpec> parseResult1 = (IParseResult<RequirementSpec>) null;
      Cursor startCursor1 = cursor;
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
        IParseResult<RequirementSpec> result = ((IParseResult<RequirementSpec>) null ?? this.url_req(ref cursor)) ?? this.name_req(ref cursor);
        RequirementSpec spec = this.ValueOrDefault<RequirementSpec>(result);
        if (result != null)
        {
          Cursor startCursor3 = cursor;
          List<string> l1 = new List<string>();
          while (true)
          {
            IParseResult<string> parseResult3 = this.wsp(ref cursor);
            if (parseResult3 != null)
              l1.Add(parseResult3.Value);
            else
              break;
          }
          if (this.ReturnHelper<IList<string>>(startCursor3, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l1.AsReadOnly())) != null)
          {
            IParseResult<string> parseResult4 = (IParseResult<string>) null ?? this.end(ref cursor);
            if (parseResult4 == null)
              throw this.ExceptionHelper(cursor, (Func<Cursor, RequirementParserInternal.UnparseableSpecError>) (state => new RequirementParserInternal.UnparseableSpecError()));
            if (parseResult4 != null)
              parseResult1 = this.ReturnHelper<RequirementSpec>(startCursor1, ref cursor, (Func<Cursor, RequirementSpec>) (state => spec));
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
      this.tracer.TraceRuleExit<RequirementSpec>(nameof (Specification), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> URI_reference(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (URI_reference), cursor);
      IParseResult<string> parseResult = ((IParseResult<string>) null ?? this.URI(ref cursor)) ?? this.relative_ref(ref cursor);
      this.tracer.TraceRuleExit<string>(nameof (URI_reference), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<string> URI(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (URI), cursor);
      IParseResult<string> parseResult1 = (IParseResult<string>) null;
      Cursor startCursor0 = cursor;
      if (this.scheme(ref cursor) != null)
      {
        if (this.ParseLiteral(ref cursor, ":") != null)
        {
          if (this.hier_part(ref cursor) != null)
          {
            Cursor startCursor1 = cursor;
            List<string> l0 = new List<string>();
            while (l0.Count < 1)
            {
              IParseResult<string> parseResult2 = (IParseResult<string>) null;
              Cursor startCursor2 = cursor;
              if (this.ParseLiteral(ref cursor, "?") != null)
              {
                if (this.query(ref cursor) != null)
                {
                  int len = cursor.Location - startCursor2.Location;
                  parseResult2 = this.ReturnHelper<string>(startCursor2, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor2.Location, len)));
                }
                else
                  cursor = startCursor2;
              }
              else
                cursor = startCursor2;
              if (parseResult2 != null)
                l0.Add(parseResult2.Value);
              else
                break;
            }
            if (this.ReturnHelper<IList<string>>(startCursor1, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly())) != null)
            {
              Cursor startCursor2 = cursor;
              List<string> l1 = new List<string>();
              while (l1.Count < 1)
              {
                IParseResult<string> parseResult3 = (IParseResult<string>) null;
                Cursor startCursor4 = cursor;
                if (this.ParseLiteral(ref cursor, "#") != null)
                {
                  if (this.fragment(ref cursor) != null)
                  {
                    int len = cursor.Location - startCursor4.Location;
                    parseResult3 = this.ReturnHelper<string>(startCursor4, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor4.Location, len)));
                  }
                  else
                    cursor = startCursor4;
                }
                else
                  cursor = startCursor4;
                if (parseResult3 != null)
                  l1.Add(parseResult3.Value);
                else
                  break;
              }
              if (this.ReturnHelper<IList<string>>(startCursor2, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l1.AsReadOnly())) != null)
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
        }
        else
          cursor = startCursor0;
      }
      else
        cursor = startCursor0;
      this.tracer.TraceRuleExit<string>(nameof (URI), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> hier_part(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (hier_part), cursor);
      IParseResult<string> parseResult = (IParseResult<string>) null;
      if (parseResult == null)
      {
        Cursor startCursor0 = cursor;
        if (this.ParseLiteral(ref cursor, "//") != null)
        {
          if (this.authority(ref cursor) != null)
          {
            if (this.path_abempty(ref cursor) != null)
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
      }
      if (parseResult == null)
        parseResult = this.path_absolute(ref cursor);
      if (parseResult == null)
        parseResult = this.path_rootless(ref cursor);
      if (parseResult == null)
        parseResult = this.path_empty(ref cursor);
      this.tracer.TraceRuleExit<string>(nameof (hier_part), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<string> relative_ref(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (relative_ref), cursor);
      IParseResult<string> parseResult1 = (IParseResult<string>) null;
      Cursor startCursor0 = cursor;
      if (this.relative_part(ref cursor) != null)
      {
        Cursor startCursor1 = cursor;
        List<string> l0 = new List<string>();
        while (l0.Count < 1)
        {
          IParseResult<string> parseResult2 = (IParseResult<string>) null;
          Cursor startCursor2 = cursor;
          if (this.ParseLiteral(ref cursor, "?") != null)
          {
            if (this.query(ref cursor) != null)
            {
              int len = cursor.Location - startCursor2.Location;
              parseResult2 = this.ReturnHelper<string>(startCursor2, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor2.Location, len)));
            }
            else
              cursor = startCursor2;
          }
          else
            cursor = startCursor2;
          if (parseResult2 != null)
            l0.Add(parseResult2.Value);
          else
            break;
        }
        if (this.ReturnHelper<IList<string>>(startCursor1, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly())) != null)
        {
          Cursor startCursor2 = cursor;
          List<string> l1 = new List<string>();
          while (l1.Count < 1)
          {
            IParseResult<string> parseResult3 = (IParseResult<string>) null;
            Cursor startCursor4 = cursor;
            if (this.ParseLiteral(ref cursor, "#") != null)
            {
              if (this.fragment(ref cursor) != null)
              {
                int len = cursor.Location - startCursor4.Location;
                parseResult3 = this.ReturnHelper<string>(startCursor4, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor4.Location, len)));
              }
              else
                cursor = startCursor4;
            }
            else
              cursor = startCursor4;
            if (parseResult3 != null)
              l1.Add(parseResult3.Value);
            else
              break;
          }
          if (this.ReturnHelper<IList<string>>(startCursor2, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l1.AsReadOnly())) != null)
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
      this.tracer.TraceRuleExit<string>(nameof (relative_ref), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> relative_part(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (relative_part), cursor);
      IParseResult<string> parseResult = (IParseResult<string>) null;
      if (parseResult == null)
      {
        Cursor startCursor0 = cursor;
        if (this.ParseLiteral(ref cursor, "//") != null)
        {
          if (this.authority(ref cursor) != null)
          {
            if (this.path_abempty(ref cursor) != null)
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
      }
      if (parseResult == null)
        parseResult = this.path_absolute(ref cursor);
      if (parseResult == null)
        parseResult = this.path_noscheme(ref cursor);
      if (parseResult == null)
        parseResult = this.path_empty(ref cursor);
      this.tracer.TraceRuleExit<string>(nameof (relative_part), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<string> scheme(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (scheme), cursor);
      IParseResult<string> parseResult1 = (IParseResult<string>) null;
      Cursor startCursor0 = cursor;
      if (this.letter(ref cursor) != null)
      {
        Cursor startCursor = cursor;
        List<string> l0 = new List<string>();
        while (true)
        {
          IParseResult<string> parseResult2 = (((((IParseResult<string>) null ?? this.letter(ref cursor)) ?? this.digit(ref cursor)) ?? this.ParseLiteral(ref cursor, "+")) ?? this.ParseLiteral(ref cursor, "-")) ?? this.ParseLiteral(ref cursor, ".");
          if (parseResult2 != null)
            l0.Add(parseResult2.Value);
          else
            break;
        }
        if (this.ReturnHelper<IList<string>>(startCursor, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly())) != null)
        {
          int len = cursor.Location - startCursor0.Location;
          parseResult1 = this.ReturnHelper<string>(startCursor0, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor0.Location, len)));
        }
        else
          cursor = startCursor0;
      }
      else
        cursor = startCursor0;
      this.tracer.TraceRuleExit<string>(nameof (scheme), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> authority(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (authority), cursor);
      IParseResult<string> parseResult1 = (IParseResult<string>) null;
      Cursor startCursor0 = cursor;
      Cursor startCursor1 = cursor;
      List<string> l0 = new List<string>();
      while (l0.Count < 1)
      {
        IParseResult<string> parseResult2 = (IParseResult<string>) null;
        Cursor startCursor2 = cursor;
        if (this.userinfo(ref cursor) != null)
        {
          if (this.ParseLiteral(ref cursor, "@") != null)
          {
            int len = cursor.Location - startCursor2.Location;
            parseResult2 = this.ReturnHelper<string>(startCursor2, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor2.Location, len)));
          }
          else
            cursor = startCursor2;
        }
        else
          cursor = startCursor2;
        if (parseResult2 != null)
          l0.Add(parseResult2.Value);
        else
          break;
      }
      if (this.ReturnHelper<IList<string>>(startCursor1, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly())) != null)
      {
        if (this.host(ref cursor) != null)
        {
          Cursor startCursor2 = cursor;
          List<string> l1 = new List<string>();
          while (l1.Count < 1)
          {
            IParseResult<string> parseResult3 = (IParseResult<string>) null;
            Cursor startCursor4 = cursor;
            if (this.ParseLiteral(ref cursor, ":") != null)
            {
              if (this.port(ref cursor) != null)
              {
                int len = cursor.Location - startCursor4.Location;
                parseResult3 = this.ReturnHelper<string>(startCursor4, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor4.Location, len)));
              }
              else
                cursor = startCursor4;
            }
            else
              cursor = startCursor4;
            if (parseResult3 != null)
              l1.Add(parseResult3.Value);
            else
              break;
          }
          if (this.ReturnHelper<IList<string>>(startCursor2, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l1.AsReadOnly())) != null)
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
      this.tracer.TraceRuleExit<string>(nameof (authority), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<IList<string>> userinfo(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (userinfo), cursor);
      Cursor startCursor = cursor;
      List<string> l0 = new List<string>();
      while (true)
      {
        IParseResult<string> parseResult = ((((IParseResult<string>) null ?? this.unreserved(ref cursor)) ?? this.pct_encoded(ref cursor)) ?? this.sub_delims(ref cursor)) ?? this.ParseLiteral(ref cursor, ":");
        if (parseResult != null)
          l0.Add(parseResult.Value);
        else
          break;
      }
      IParseResult<IList<string>> parseResult1 = this.ReturnHelper<IList<string>>(startCursor, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly()));
      this.tracer.TraceRuleExit<IList<string>>(nameof (userinfo), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> host(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (host), cursor);
      IParseResult<string> parseResult = (((IParseResult<string>) null ?? this.IP_literal(ref cursor)) ?? this.IPv4address(ref cursor)) ?? this.reg_name(ref cursor);
      this.tracer.TraceRuleExit<string>(nameof (host), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<string> port(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (port), cursor);
      IParseResult<string> parseResult1 = (IParseResult<string>) null;
      Cursor startCursor0 = cursor;
      Cursor startCursor = cursor;
      List<string> l0 = new List<string>();
      while (true)
      {
        IParseResult<string> parseResult2 = this.digit(ref cursor);
        if (parseResult2 != null)
          l0.Add(parseResult2.Value);
        else
          break;
      }
      if (this.ReturnHelper<IList<string>>(startCursor, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly())) != null)
      {
        int len = cursor.Location - startCursor0.Location;
        parseResult1 = this.ReturnHelper<string>(startCursor0, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor0.Location, len)));
      }
      else
        cursor = startCursor0;
      this.tracer.TraceRuleExit<string>(nameof (port), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> IP_literal(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (IP_literal), cursor);
      IParseResult<string> parseResult = (IParseResult<string>) null;
      Cursor startCursor0 = cursor;
      if (this.ParseLiteral(ref cursor, "[") != null)
      {
        if ((((IParseResult<string>) null ?? this.IPv6address(ref cursor)) ?? this.IPvFuture(ref cursor)) != null)
        {
          if (this.ParseLiteral(ref cursor, "]") != null)
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
      this.tracer.TraceRuleExit<string>(nameof (IP_literal), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<string> IPvFuture(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (IPvFuture), cursor);
      IParseResult<string> parseResult1 = (IParseResult<string>) null;
      Cursor startCursor0 = cursor;
      if (this.ParseLiteral(ref cursor, "v") != null)
      {
        IParseResult<IList<string>> parseResult2 = (IParseResult<IList<string>>) null;
        Cursor startCursor1 = cursor;
        List<string> l0 = new List<string>();
        while (true)
        {
          IParseResult<string> parseResult3 = this.hexdig(ref cursor);
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
          if (this.ParseLiteral(ref cursor, ".") != null)
          {
            IParseResult<IList<string>> parseResult4 = (IParseResult<IList<string>>) null;
            Cursor startCursor2 = cursor;
            List<string> l1 = new List<string>();
            while (true)
            {
              IParseResult<string> parseResult5 = (((IParseResult<string>) null ?? this.unreserved(ref cursor)) ?? this.sub_delims(ref cursor)) ?? this.ParseLiteral(ref cursor, ":");
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
      }
      else
        cursor = startCursor0;
      this.tracer.TraceRuleExit<string>(nameof (IPvFuture), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> IPv6address(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (IPv6address), cursor);
      IParseResult<string> parseResult1 = (IParseResult<string>) null;
      if (parseResult1 == null)
      {
        Cursor startCursor0 = cursor;
        IParseResult<IList<string>> parseResult2 = (IParseResult<IList<string>>) null;
        Cursor startCursor = cursor;
        List<string> l0 = new List<string>();
        while (l0.Count < 6)
        {
          IParseResult<string> parseResult3 = (IParseResult<string>) null;
          Cursor startCursor2 = cursor;
          if (this.h16(ref cursor) != null)
          {
            if (this.ParseLiteral(ref cursor, ":") != null)
            {
              int len = cursor.Location - startCursor2.Location;
              parseResult3 = this.ReturnHelper<string>(startCursor2, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor2.Location, len)));
            }
            else
              cursor = startCursor2;
          }
          else
            cursor = startCursor2;
          if (parseResult3 != null)
            l0.Add(parseResult3.Value);
          else
            break;
        }
        if (l0.Count >= 6)
          parseResult2 = this.ReturnHelper<IList<string>>(startCursor, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly()));
        else
          cursor = startCursor;
        if (parseResult2 != null)
        {
          if (this.ls32(ref cursor) != null)
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
      if (parseResult1 == null)
      {
        Cursor startCursor3 = cursor;
        if (this.ParseLiteral(ref cursor, "::") != null)
        {
          IParseResult<IList<string>> parseResult4 = (IParseResult<IList<string>>) null;
          Cursor startCursor = cursor;
          List<string> l1 = new List<string>();
          while (l1.Count < 5)
          {
            IParseResult<string> parseResult5 = (IParseResult<string>) null;
            Cursor startCursor5 = cursor;
            if (this.h16(ref cursor) != null)
            {
              if (this.ParseLiteral(ref cursor, ":") != null)
              {
                int len = cursor.Location - startCursor5.Location;
                parseResult5 = this.ReturnHelper<string>(startCursor5, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor5.Location, len)));
              }
              else
                cursor = startCursor5;
            }
            else
              cursor = startCursor5;
            if (parseResult5 != null)
              l1.Add(parseResult5.Value);
            else
              break;
          }
          if (l1.Count >= 5)
            parseResult4 = this.ReturnHelper<IList<string>>(startCursor, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l1.AsReadOnly()));
          else
            cursor = startCursor;
          if (parseResult4 != null)
          {
            if (this.ls32(ref cursor) != null)
            {
              int len = cursor.Location - startCursor3.Location;
              parseResult1 = this.ReturnHelper<string>(startCursor3, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor3.Location, len)));
            }
            else
              cursor = startCursor3;
          }
          else
            cursor = startCursor3;
        }
        else
          cursor = startCursor3;
      }
      if (parseResult1 == null)
      {
        Cursor startCursor6 = cursor;
        Cursor startCursor1 = cursor;
        List<IList<string>> l2 = new List<IList<string>>();
        while (l2.Count < 1)
        {
          IParseResult<IList<string>> parseResult6 = this.h16(ref cursor);
          if (parseResult6 != null)
            l2.Add(parseResult6.Value);
          else
            break;
        }
        if (this.ReturnHelper<IList<IList<string>>>(startCursor1, ref cursor, (Func<Cursor, IList<IList<string>>>) (state => (IList<IList<string>>) l2.AsReadOnly())) != null)
        {
          if (this.ParseLiteral(ref cursor, "::") != null)
          {
            IParseResult<IList<string>> parseResult7 = (IParseResult<IList<string>>) null;
            Cursor startCursor2 = cursor;
            List<string> l3 = new List<string>();
            while (l3.Count < 4)
            {
              IParseResult<string> parseResult8 = (IParseResult<string>) null;
              Cursor startCursor9 = cursor;
              if (this.h16(ref cursor) != null)
              {
                if (this.ParseLiteral(ref cursor, ":") != null)
                {
                  int len = cursor.Location - startCursor9.Location;
                  parseResult8 = this.ReturnHelper<string>(startCursor9, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor9.Location, len)));
                }
                else
                  cursor = startCursor9;
              }
              else
                cursor = startCursor9;
              if (parseResult8 != null)
                l3.Add(parseResult8.Value);
              else
                break;
            }
            if (l3.Count >= 4)
              parseResult7 = this.ReturnHelper<IList<string>>(startCursor2, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l3.AsReadOnly()));
            else
              cursor = startCursor2;
            if (parseResult7 != null)
            {
              if (this.ls32(ref cursor) != null)
              {
                int len = cursor.Location - startCursor6.Location;
                parseResult1 = this.ReturnHelper<string>(startCursor6, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor6.Location, len)));
              }
              else
                cursor = startCursor6;
            }
            else
              cursor = startCursor6;
          }
          else
            cursor = startCursor6;
        }
        else
          cursor = startCursor6;
      }
      if (parseResult1 == null)
      {
        Cursor startCursor10 = cursor;
        Cursor startCursor3 = cursor;
        List<string> l4 = new List<string>();
        while (l4.Count < 1)
        {
          IParseResult<string> parseResult9 = (IParseResult<string>) null;
          Cursor startCursor12 = cursor;
          Cursor startCursor4 = cursor;
          List<string> l5 = new List<string>();
          while (l5.Count < 1)
          {
            IParseResult<string> parseResult10 = (IParseResult<string>) null;
            Cursor startCursor14 = cursor;
            if (this.h16(ref cursor) != null)
            {
              if (this.ParseLiteral(ref cursor, ":") != null)
              {
                int len = cursor.Location - startCursor14.Location;
                parseResult10 = this.ReturnHelper<string>(startCursor14, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor14.Location, len)));
              }
              else
                cursor = startCursor14;
            }
            else
              cursor = startCursor14;
            if (parseResult10 != null)
              l5.Add(parseResult10.Value);
            else
              break;
          }
          if (this.ReturnHelper<IList<string>>(startCursor4, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l5.AsReadOnly())) != null)
          {
            if (this.h16(ref cursor) != null)
            {
              int len = cursor.Location - startCursor12.Location;
              parseResult9 = this.ReturnHelper<string>(startCursor12, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor12.Location, len)));
            }
            else
              cursor = startCursor12;
          }
          else
            cursor = startCursor12;
          if (parseResult9 != null)
            l4.Add(parseResult9.Value);
          else
            break;
        }
        if (this.ReturnHelper<IList<string>>(startCursor3, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l4.AsReadOnly())) != null)
        {
          if (this.ParseLiteral(ref cursor, "::") != null)
          {
            IParseResult<IList<string>> parseResult11 = (IParseResult<IList<string>>) null;
            Cursor startCursor5 = cursor;
            List<string> l6 = new List<string>();
            while (l6.Count < 3)
            {
              IParseResult<string> parseResult12 = (IParseResult<string>) null;
              Cursor startCursor16 = cursor;
              if (this.h16(ref cursor) != null)
              {
                if (this.ParseLiteral(ref cursor, ":") != null)
                {
                  int len = cursor.Location - startCursor16.Location;
                  parseResult12 = this.ReturnHelper<string>(startCursor16, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor16.Location, len)));
                }
                else
                  cursor = startCursor16;
              }
              else
                cursor = startCursor16;
              if (parseResult12 != null)
                l6.Add(parseResult12.Value);
              else
                break;
            }
            if (l6.Count >= 3)
              parseResult11 = this.ReturnHelper<IList<string>>(startCursor5, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l6.AsReadOnly()));
            else
              cursor = startCursor5;
            if (parseResult11 != null)
            {
              if (this.ls32(ref cursor) != null)
              {
                int len = cursor.Location - startCursor10.Location;
                parseResult1 = this.ReturnHelper<string>(startCursor10, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor10.Location, len)));
              }
              else
                cursor = startCursor10;
            }
            else
              cursor = startCursor10;
          }
          else
            cursor = startCursor10;
        }
        else
          cursor = startCursor10;
      }
      if (parseResult1 == null)
      {
        Cursor startCursor17 = cursor;
        Cursor startCursor6 = cursor;
        List<string> l7 = new List<string>();
        while (l7.Count < 1)
        {
          IParseResult<string> parseResult13 = (IParseResult<string>) null;
          Cursor startCursor19 = cursor;
          Cursor startCursor7 = cursor;
          List<string> l8 = new List<string>();
          while (l8.Count < 2)
          {
            IParseResult<string> parseResult14 = (IParseResult<string>) null;
            Cursor startCursor21 = cursor;
            if (this.h16(ref cursor) != null)
            {
              if (this.ParseLiteral(ref cursor, ":") != null)
              {
                int len = cursor.Location - startCursor21.Location;
                parseResult14 = this.ReturnHelper<string>(startCursor21, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor21.Location, len)));
              }
              else
                cursor = startCursor21;
            }
            else
              cursor = startCursor21;
            if (parseResult14 != null)
              l8.Add(parseResult14.Value);
            else
              break;
          }
          if (this.ReturnHelper<IList<string>>(startCursor7, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l8.AsReadOnly())) != null)
          {
            if (this.h16(ref cursor) != null)
            {
              int len = cursor.Location - startCursor19.Location;
              parseResult13 = this.ReturnHelper<string>(startCursor19, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor19.Location, len)));
            }
            else
              cursor = startCursor19;
          }
          else
            cursor = startCursor19;
          if (parseResult13 != null)
            l7.Add(parseResult13.Value);
          else
            break;
        }
        if (this.ReturnHelper<IList<string>>(startCursor6, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l7.AsReadOnly())) != null)
        {
          if (this.ParseLiteral(ref cursor, "::") != null)
          {
            IParseResult<IList<string>> parseResult15 = (IParseResult<IList<string>>) null;
            Cursor startCursor8 = cursor;
            List<string> l9 = new List<string>();
            while (l9.Count < 2)
            {
              IParseResult<string> parseResult16 = (IParseResult<string>) null;
              Cursor startCursor23 = cursor;
              if (this.h16(ref cursor) != null)
              {
                if (this.ParseLiteral(ref cursor, ":") != null)
                {
                  int len = cursor.Location - startCursor23.Location;
                  parseResult16 = this.ReturnHelper<string>(startCursor23, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor23.Location, len)));
                }
                else
                  cursor = startCursor23;
              }
              else
                cursor = startCursor23;
              if (parseResult16 != null)
                l9.Add(parseResult16.Value);
              else
                break;
            }
            if (l9.Count >= 2)
              parseResult15 = this.ReturnHelper<IList<string>>(startCursor8, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l9.AsReadOnly()));
            else
              cursor = startCursor8;
            if (parseResult15 != null)
            {
              if (this.ls32(ref cursor) != null)
              {
                int len = cursor.Location - startCursor17.Location;
                parseResult1 = this.ReturnHelper<string>(startCursor17, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor17.Location, len)));
              }
              else
                cursor = startCursor17;
            }
            else
              cursor = startCursor17;
          }
          else
            cursor = startCursor17;
        }
        else
          cursor = startCursor17;
      }
      if (parseResult1 == null)
      {
        Cursor startCursor24 = cursor;
        Cursor startCursor9 = cursor;
        List<string> l10 = new List<string>();
        while (l10.Count < 1)
        {
          IParseResult<string> parseResult17 = (IParseResult<string>) null;
          Cursor startCursor26 = cursor;
          Cursor startCursor10 = cursor;
          List<string> l11 = new List<string>();
          while (l11.Count < 3)
          {
            IParseResult<string> parseResult18 = (IParseResult<string>) null;
            Cursor startCursor28 = cursor;
            if (this.h16(ref cursor) != null)
            {
              if (this.ParseLiteral(ref cursor, ":") != null)
              {
                int len = cursor.Location - startCursor28.Location;
                parseResult18 = this.ReturnHelper<string>(startCursor28, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor28.Location, len)));
              }
              else
                cursor = startCursor28;
            }
            else
              cursor = startCursor28;
            if (parseResult18 != null)
              l11.Add(parseResult18.Value);
            else
              break;
          }
          if (this.ReturnHelper<IList<string>>(startCursor10, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l11.AsReadOnly())) != null)
          {
            if (this.h16(ref cursor) != null)
            {
              int len = cursor.Location - startCursor26.Location;
              parseResult17 = this.ReturnHelper<string>(startCursor26, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor26.Location, len)));
            }
            else
              cursor = startCursor26;
          }
          else
            cursor = startCursor26;
          if (parseResult17 != null)
            l10.Add(parseResult17.Value);
          else
            break;
        }
        if (this.ReturnHelper<IList<string>>(startCursor9, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l10.AsReadOnly())) != null)
        {
          if (this.ParseLiteral(ref cursor, "::") != null)
          {
            if (this.h16(ref cursor) != null)
            {
              if (this.ParseLiteral(ref cursor, ":") != null)
              {
                if (this.ls32(ref cursor) != null)
                {
                  int len = cursor.Location - startCursor24.Location;
                  parseResult1 = this.ReturnHelper<string>(startCursor24, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor24.Location, len)));
                }
                else
                  cursor = startCursor24;
              }
              else
                cursor = startCursor24;
            }
            else
              cursor = startCursor24;
          }
          else
            cursor = startCursor24;
        }
        else
          cursor = startCursor24;
      }
      if (parseResult1 == null)
      {
        Cursor startCursor29 = cursor;
        Cursor startCursor11 = cursor;
        List<string> l12 = new List<string>();
        while (l12.Count < 1)
        {
          IParseResult<string> parseResult19 = (IParseResult<string>) null;
          Cursor startCursor31 = cursor;
          Cursor startCursor12 = cursor;
          List<string> l13 = new List<string>();
          while (l13.Count < 4)
          {
            IParseResult<string> parseResult20 = (IParseResult<string>) null;
            Cursor startCursor33 = cursor;
            if (this.h16(ref cursor) != null)
            {
              if (this.ParseLiteral(ref cursor, ":") != null)
              {
                int len = cursor.Location - startCursor33.Location;
                parseResult20 = this.ReturnHelper<string>(startCursor33, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor33.Location, len)));
              }
              else
                cursor = startCursor33;
            }
            else
              cursor = startCursor33;
            if (parseResult20 != null)
              l13.Add(parseResult20.Value);
            else
              break;
          }
          if (this.ReturnHelper<IList<string>>(startCursor12, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l13.AsReadOnly())) != null)
          {
            if (this.h16(ref cursor) != null)
            {
              int len = cursor.Location - startCursor31.Location;
              parseResult19 = this.ReturnHelper<string>(startCursor31, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor31.Location, len)));
            }
            else
              cursor = startCursor31;
          }
          else
            cursor = startCursor31;
          if (parseResult19 != null)
            l12.Add(parseResult19.Value);
          else
            break;
        }
        if (this.ReturnHelper<IList<string>>(startCursor11, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l12.AsReadOnly())) != null)
        {
          if (this.ParseLiteral(ref cursor, "::") != null)
          {
            if (this.ls32(ref cursor) != null)
            {
              int len = cursor.Location - startCursor29.Location;
              parseResult1 = this.ReturnHelper<string>(startCursor29, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor29.Location, len)));
            }
            else
              cursor = startCursor29;
          }
          else
            cursor = startCursor29;
        }
        else
          cursor = startCursor29;
      }
      if (parseResult1 == null)
      {
        Cursor startCursor34 = cursor;
        Cursor startCursor13 = cursor;
        List<string> l14 = new List<string>();
        while (l14.Count < 1)
        {
          IParseResult<string> parseResult21 = (IParseResult<string>) null;
          Cursor startCursor36 = cursor;
          Cursor startCursor14 = cursor;
          List<string> l15 = new List<string>();
          while (l15.Count < 5)
          {
            IParseResult<string> parseResult22 = (IParseResult<string>) null;
            Cursor startCursor38 = cursor;
            if (this.h16(ref cursor) != null)
            {
              if (this.ParseLiteral(ref cursor, ":") != null)
              {
                int len = cursor.Location - startCursor38.Location;
                parseResult22 = this.ReturnHelper<string>(startCursor38, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor38.Location, len)));
              }
              else
                cursor = startCursor38;
            }
            else
              cursor = startCursor38;
            if (parseResult22 != null)
              l15.Add(parseResult22.Value);
            else
              break;
          }
          if (this.ReturnHelper<IList<string>>(startCursor14, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l15.AsReadOnly())) != null)
          {
            if (this.h16(ref cursor) != null)
            {
              int len = cursor.Location - startCursor36.Location;
              parseResult21 = this.ReturnHelper<string>(startCursor36, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor36.Location, len)));
            }
            else
              cursor = startCursor36;
          }
          else
            cursor = startCursor36;
          if (parseResult21 != null)
            l14.Add(parseResult21.Value);
          else
            break;
        }
        if (this.ReturnHelper<IList<string>>(startCursor13, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l14.AsReadOnly())) != null)
        {
          if (this.ParseLiteral(ref cursor, "::") != null)
          {
            if (this.h16(ref cursor) != null)
            {
              int len = cursor.Location - startCursor34.Location;
              parseResult1 = this.ReturnHelper<string>(startCursor34, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor34.Location, len)));
            }
            else
              cursor = startCursor34;
          }
          else
            cursor = startCursor34;
        }
        else
          cursor = startCursor34;
      }
      if (parseResult1 == null)
      {
        Cursor startCursor39 = cursor;
        Cursor startCursor15 = cursor;
        List<string> l16 = new List<string>();
        while (l16.Count < 1)
        {
          IParseResult<string> parseResult23 = (IParseResult<string>) null;
          Cursor startCursor41 = cursor;
          Cursor startCursor16 = cursor;
          List<string> l17 = new List<string>();
          while (l17.Count < 6)
          {
            IParseResult<string> parseResult24 = (IParseResult<string>) null;
            Cursor startCursor43 = cursor;
            if (this.h16(ref cursor) != null)
            {
              if (this.ParseLiteral(ref cursor, ":") != null)
              {
                int len = cursor.Location - startCursor43.Location;
                parseResult24 = this.ReturnHelper<string>(startCursor43, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor43.Location, len)));
              }
              else
                cursor = startCursor43;
            }
            else
              cursor = startCursor43;
            if (parseResult24 != null)
              l17.Add(parseResult24.Value);
            else
              break;
          }
          if (this.ReturnHelper<IList<string>>(startCursor16, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l17.AsReadOnly())) != null)
          {
            if (this.h16(ref cursor) != null)
            {
              int len = cursor.Location - startCursor41.Location;
              parseResult23 = this.ReturnHelper<string>(startCursor41, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor41.Location, len)));
            }
            else
              cursor = startCursor41;
          }
          else
            cursor = startCursor41;
          if (parseResult23 != null)
            l16.Add(parseResult23.Value);
          else
            break;
        }
        if (this.ReturnHelper<IList<string>>(startCursor15, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l16.AsReadOnly())) != null)
        {
          if (this.ParseLiteral(ref cursor, "::") != null)
          {
            int len = cursor.Location - startCursor39.Location;
            parseResult1 = this.ReturnHelper<string>(startCursor39, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor39.Location, len)));
          }
          else
            cursor = startCursor39;
        }
        else
          cursor = startCursor39;
      }
      this.tracer.TraceRuleExit<string>(nameof (IPv6address), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<IList<string>> h16(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (h16), cursor);
      IParseResult<IList<string>> parseResult1 = (IParseResult<IList<string>>) null;
      Cursor startCursor = cursor;
      List<string> l0 = new List<string>();
      while (l0.Count < 4)
      {
        IParseResult<string> parseResult2 = this.hexdig(ref cursor);
        if (parseResult2 != null)
          l0.Add(parseResult2.Value);
        else
          break;
      }
      if (l0.Count >= 1)
        parseResult1 = this.ReturnHelper<IList<string>>(startCursor, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly()));
      else
        cursor = startCursor;
      this.tracer.TraceRuleExit<IList<string>>(nameof (h16), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> ls32(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (ls32), cursor);
      IParseResult<string> parseResult = (IParseResult<string>) null;
      if (parseResult == null)
      {
        Cursor startCursor0 = cursor;
        if (this.h16(ref cursor) != null)
        {
          if (this.ParseLiteral(ref cursor, ":") != null)
          {
            if (this.h16(ref cursor) != null)
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
      }
      if (parseResult == null)
        parseResult = this.IPv4address(ref cursor);
      this.tracer.TraceRuleExit<string>(nameof (ls32), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<string> IPv4address(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (IPv4address), cursor);
      IParseResult<string> parseResult = (IParseResult<string>) null;
      Cursor startCursor0 = cursor;
      if (this.dec_octet(ref cursor) != null)
      {
        if (this.ParseLiteral(ref cursor, ".") != null)
        {
          if (this.dec_octet(ref cursor) != null)
          {
            if (this.ParseLiteral(ref cursor, ".") != null)
            {
              if (this.dec_octet(ref cursor) != null)
              {
                if (this.ParseLiteral(ref cursor, ".") != null)
                {
                  if (this.dec_octet(ref cursor) != null)
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
            }
            else
              cursor = startCursor0;
          }
          else
            cursor = startCursor0;
        }
        else
          cursor = startCursor0;
      }
      else
        cursor = startCursor0;
      this.tracer.TraceRuleExit<string>(nameof (IPv4address), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<string> nz(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (nz), cursor);
      IParseResult<string> parseResult1 = (IParseResult<string>) null;
      Cursor startCursor0 = cursor;
      IParseResult<string> parseResult2 = (IParseResult<string>) null;
      Cursor cursor1 = cursor;
      if (this.ParseLiteral(ref cursor, "0") == null)
        parseResult2 = this.ReturnHelper<string>(cursor, ref cursor, (Func<Cursor, string>) (state => string.Empty));
      else
        cursor = cursor1;
      if (parseResult2 != null)
      {
        if (this.digit(ref cursor) != null)
        {
          int len = cursor.Location - startCursor0.Location;
          parseResult1 = this.ReturnHelper<string>(startCursor0, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor0.Location, len)));
        }
        else
          cursor = startCursor0;
      }
      else
        cursor = startCursor0;
      this.tracer.TraceRuleExit<string>(nameof (nz), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> dec_octet(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (dec_octet), cursor);
      IParseResult<string> parseResult1 = (IParseResult<string>) null ?? this.digit(ref cursor);
      if (parseResult1 == null)
      {
        Cursor startCursor0 = cursor;
        if (this.nz(ref cursor) != null)
        {
          if (this.digit(ref cursor) != null)
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
      if (parseResult1 == null)
      {
        Cursor startCursor1 = cursor;
        if (this.ParseLiteral(ref cursor, "1") != null)
        {
          IParseResult<IList<string>> parseResult2 = (IParseResult<IList<string>>) null;
          Cursor startCursor = cursor;
          List<string> l0 = new List<string>();
          while (l0.Count < 2)
          {
            IParseResult<string> parseResult3 = this.digit(ref cursor);
            if (parseResult3 != null)
              l0.Add(parseResult3.Value);
            else
              break;
          }
          if (l0.Count >= 2)
            parseResult2 = this.ReturnHelper<IList<string>>(startCursor, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly()));
          else
            cursor = startCursor;
          if (parseResult2 != null)
          {
            int len = cursor.Location - startCursor1.Location;
            parseResult1 = this.ReturnHelper<string>(startCursor1, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor1.Location, len)));
          }
          else
            cursor = startCursor1;
        }
        else
          cursor = startCursor1;
      }
      if (parseResult1 == null)
      {
        Cursor startCursor3 = cursor;
        if (this.ParseLiteral(ref cursor, "2") != null)
        {
          if (((((((IParseResult<string>) null ?? this.ParseLiteral(ref cursor, "0")) ?? this.ParseLiteral(ref cursor, "1")) ?? this.ParseLiteral(ref cursor, "2")) ?? this.ParseLiteral(ref cursor, "3")) ?? this.ParseLiteral(ref cursor, "4")) != null)
          {
            if (this.digit(ref cursor) != null)
            {
              int len = cursor.Location - startCursor3.Location;
              parseResult1 = this.ReturnHelper<string>(startCursor3, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor3.Location, len)));
            }
            else
              cursor = startCursor3;
          }
          else
            cursor = startCursor3;
        }
        else
          cursor = startCursor3;
      }
      if (parseResult1 == null)
      {
        Cursor startCursor4 = cursor;
        if (this.ParseLiteral(ref cursor, "25") != null)
        {
          if ((((((((IParseResult<string>) null ?? this.ParseLiteral(ref cursor, "0")) ?? this.ParseLiteral(ref cursor, "1")) ?? this.ParseLiteral(ref cursor, "2")) ?? this.ParseLiteral(ref cursor, "3")) ?? this.ParseLiteral(ref cursor, "4")) ?? this.ParseLiteral(ref cursor, "5")) != null)
          {
            int len = cursor.Location - startCursor4.Location;
            parseResult1 = this.ReturnHelper<string>(startCursor4, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor4.Location, len)));
          }
          else
            cursor = startCursor4;
        }
        else
          cursor = startCursor4;
      }
      this.tracer.TraceRuleExit<string>(nameof (dec_octet), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> reg_name(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (reg_name), cursor);
      IParseResult<string> parseResult1 = (IParseResult<string>) null;
      Cursor startCursor0 = cursor;
      Cursor startCursor = cursor;
      List<string> l0 = new List<string>();
      while (true)
      {
        IParseResult<string> parseResult2 = (((IParseResult<string>) null ?? this.unreserved(ref cursor)) ?? this.pct_encoded(ref cursor)) ?? this.sub_delims(ref cursor);
        if (parseResult2 != null)
          l0.Add(parseResult2.Value);
        else
          break;
      }
      if (this.ReturnHelper<IList<string>>(startCursor, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly())) != null)
      {
        int len = cursor.Location - startCursor0.Location;
        parseResult1 = this.ReturnHelper<string>(startCursor0, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor0.Location, len)));
      }
      else
        cursor = startCursor0;
      this.tracer.TraceRuleExit<string>(nameof (reg_name), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> path_abempty(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (path_abempty), cursor);
      IParseResult<string> parseResult1 = (IParseResult<string>) null;
      Cursor startCursor0 = cursor;
      Cursor startCursor = cursor;
      List<string> l0 = new List<string>();
      while (true)
      {
        IParseResult<string> parseResult2 = (IParseResult<string>) null;
        Cursor startCursor2 = cursor;
        if (this.ParseLiteral(ref cursor, "/") != null)
        {
          if (this.segment(ref cursor) != null)
          {
            int len = cursor.Location - startCursor2.Location;
            parseResult2 = this.ReturnHelper<string>(startCursor2, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor2.Location, len)));
          }
          else
            cursor = startCursor2;
        }
        else
          cursor = startCursor2;
        if (parseResult2 != null)
          l0.Add(parseResult2.Value);
        else
          break;
      }
      if (this.ReturnHelper<IList<string>>(startCursor, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly())) != null)
      {
        int len = cursor.Location - startCursor0.Location;
        parseResult1 = this.ReturnHelper<string>(startCursor0, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor0.Location, len)));
      }
      else
        cursor = startCursor0;
      this.tracer.TraceRuleExit<string>(nameof (path_abempty), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> path_absolute(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (path_absolute), cursor);
      IParseResult<string> parseResult1 = (IParseResult<string>) null;
      Cursor startCursor0 = cursor;
      if (this.ParseLiteral(ref cursor, "/") != null)
      {
        Cursor startCursor1 = cursor;
        List<string> l0 = new List<string>();
        while (l0.Count < 1)
        {
          IParseResult<string> parseResult2 = (IParseResult<string>) null;
          Cursor startCursor2 = cursor;
          if (this.segment_nz(ref cursor) != null)
          {
            Cursor startCursor3 = cursor;
            List<string> l1 = new List<string>();
            while (true)
            {
              IParseResult<string> parseResult3 = (IParseResult<string>) null;
              Cursor startCursor4 = cursor;
              if (this.ParseLiteral(ref cursor, "/") != null)
              {
                if (this.segment(ref cursor) != null)
                {
                  int len = cursor.Location - startCursor4.Location;
                  parseResult3 = this.ReturnHelper<string>(startCursor4, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor4.Location, len)));
                }
                else
                  cursor = startCursor4;
              }
              else
                cursor = startCursor4;
              if (parseResult3 != null)
                l1.Add(parseResult3.Value);
              else
                break;
            }
            if (this.ReturnHelper<IList<string>>(startCursor3, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l1.AsReadOnly())) != null)
            {
              int len = cursor.Location - startCursor2.Location;
              parseResult2 = this.ReturnHelper<string>(startCursor2, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor2.Location, len)));
            }
            else
              cursor = startCursor2;
          }
          else
            cursor = startCursor2;
          if (parseResult2 != null)
            l0.Add(parseResult2.Value);
          else
            break;
        }
        if (this.ReturnHelper<IList<string>>(startCursor1, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly())) != null)
        {
          int len = cursor.Location - startCursor0.Location;
          parseResult1 = this.ReturnHelper<string>(startCursor0, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor0.Location, len)));
        }
        else
          cursor = startCursor0;
      }
      else
        cursor = startCursor0;
      this.tracer.TraceRuleExit<string>(nameof (path_absolute), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> path_noscheme(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (path_noscheme), cursor);
      IParseResult<string> parseResult1 = (IParseResult<string>) null;
      Cursor startCursor0 = cursor;
      if (this.segment_nz_nc(ref cursor) != null)
      {
        Cursor startCursor = cursor;
        List<string> l0 = new List<string>();
        while (true)
        {
          IParseResult<string> parseResult2 = (IParseResult<string>) null;
          Cursor startCursor2 = cursor;
          if (this.ParseLiteral(ref cursor, "/") != null)
          {
            if (this.segment(ref cursor) != null)
            {
              int len = cursor.Location - startCursor2.Location;
              parseResult2 = this.ReturnHelper<string>(startCursor2, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor2.Location, len)));
            }
            else
              cursor = startCursor2;
          }
          else
            cursor = startCursor2;
          if (parseResult2 != null)
            l0.Add(parseResult2.Value);
          else
            break;
        }
        if (this.ReturnHelper<IList<string>>(startCursor, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly())) != null)
        {
          int len = cursor.Location - startCursor0.Location;
          parseResult1 = this.ReturnHelper<string>(startCursor0, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor0.Location, len)));
        }
        else
          cursor = startCursor0;
      }
      else
        cursor = startCursor0;
      this.tracer.TraceRuleExit<string>(nameof (path_noscheme), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> path_rootless(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (path_rootless), cursor);
      IParseResult<string> parseResult1 = (IParseResult<string>) null;
      Cursor startCursor0 = cursor;
      if (this.segment_nz(ref cursor) != null)
      {
        Cursor startCursor = cursor;
        List<string> l0 = new List<string>();
        while (true)
        {
          IParseResult<string> parseResult2 = (IParseResult<string>) null;
          Cursor startCursor2 = cursor;
          if (this.ParseLiteral(ref cursor, "/") != null)
          {
            if (this.segment(ref cursor) != null)
            {
              int len = cursor.Location - startCursor2.Location;
              parseResult2 = this.ReturnHelper<string>(startCursor2, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor2.Location, len)));
            }
            else
              cursor = startCursor2;
          }
          else
            cursor = startCursor2;
          if (parseResult2 != null)
            l0.Add(parseResult2.Value);
          else
            break;
        }
        if (this.ReturnHelper<IList<string>>(startCursor, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly())) != null)
        {
          int len = cursor.Location - startCursor0.Location;
          parseResult1 = this.ReturnHelper<string>(startCursor0, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor0.Location, len)));
        }
        else
          cursor = startCursor0;
      }
      else
        cursor = startCursor0;
      this.tracer.TraceRuleExit<string>(nameof (path_rootless), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> path_empty(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (path_empty), cursor);
      IParseResult<string> literal = this.ParseLiteral(ref cursor, "");
      this.tracer.TraceRuleExit<string>(nameof (path_empty), cursor, literal);
      return literal;
    }

    private IParseResult<string> segment(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (segment), cursor);
      IParseResult<string> parseResult1 = (IParseResult<string>) null;
      Cursor startCursor0 = cursor;
      Cursor startCursor = cursor;
      List<string> l0 = new List<string>();
      while (true)
      {
        IParseResult<string> parseResult2 = this.pchar(ref cursor);
        if (parseResult2 != null)
          l0.Add(parseResult2.Value);
        else
          break;
      }
      if (this.ReturnHelper<IList<string>>(startCursor, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly())) != null)
      {
        int len = cursor.Location - startCursor0.Location;
        parseResult1 = this.ReturnHelper<string>(startCursor0, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor0.Location, len)));
      }
      else
        cursor = startCursor0;
      this.tracer.TraceRuleExit<string>(nameof (segment), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> segment_nz(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (segment_nz), cursor);
      IParseResult<string> parseResult1 = (IParseResult<string>) null;
      Cursor startCursor0 = cursor;
      IParseResult<IList<string>> parseResult2 = (IParseResult<IList<string>>) null;
      Cursor startCursor = cursor;
      List<string> l0 = new List<string>();
      while (true)
      {
        IParseResult<string> parseResult3 = this.pchar(ref cursor);
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
      this.tracer.TraceRuleExit<string>(nameof (segment_nz), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> segment_nz_nc(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (segment_nz_nc), cursor);
      IParseResult<string> parseResult1 = (IParseResult<string>) null;
      Cursor startCursor0 = cursor;
      IParseResult<IList<string>> parseResult2 = (IParseResult<IList<string>>) null;
      Cursor startCursor = cursor;
      List<string> l0 = new List<string>();
      while (true)
      {
        IParseResult<string> parseResult3 = ((((IParseResult<string>) null ?? this.unreserved(ref cursor)) ?? this.pct_encoded(ref cursor)) ?? this.sub_delims(ref cursor)) ?? this.ParseLiteral(ref cursor, "@");
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
      this.tracer.TraceRuleExit<string>(nameof (segment_nz_nc), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> pchar(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (pchar), cursor);
      IParseResult<string> parseResult = (((((IParseResult<string>) null ?? this.unreserved(ref cursor)) ?? this.pct_encoded(ref cursor)) ?? this.sub_delims(ref cursor)) ?? this.ParseLiteral(ref cursor, ":")) ?? this.ParseLiteral(ref cursor, "@");
      this.tracer.TraceRuleExit<string>(nameof (pchar), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<string> query(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (query), cursor);
      IParseResult<string> parseResult1 = (IParseResult<string>) null;
      Cursor startCursor0 = cursor;
      Cursor startCursor = cursor;
      List<string> l0 = new List<string>();
      while (true)
      {
        IParseResult<string> parseResult2 = (((IParseResult<string>) null ?? this.pchar(ref cursor)) ?? this.ParseLiteral(ref cursor, "/")) ?? this.ParseLiteral(ref cursor, "?");
        if (parseResult2 != null)
          l0.Add(parseResult2.Value);
        else
          break;
      }
      if (this.ReturnHelper<IList<string>>(startCursor, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly())) != null)
      {
        int len = cursor.Location - startCursor0.Location;
        parseResult1 = this.ReturnHelper<string>(startCursor0, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor0.Location, len)));
      }
      else
        cursor = startCursor0;
      this.tracer.TraceRuleExit<string>(nameof (query), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> fragment(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (fragment), cursor);
      IParseResult<string> parseResult1 = (IParseResult<string>) null;
      Cursor startCursor0 = cursor;
      Cursor startCursor = cursor;
      List<string> l0 = new List<string>();
      while (true)
      {
        IParseResult<string> parseResult2 = (((IParseResult<string>) null ?? this.pchar(ref cursor)) ?? this.ParseLiteral(ref cursor, "/")) ?? this.ParseLiteral(ref cursor, "?");
        if (parseResult2 != null)
          l0.Add(parseResult2.Value);
        else
          break;
      }
      if (this.ReturnHelper<IList<string>>(startCursor, ref cursor, (Func<Cursor, IList<string>>) (state => (IList<string>) l0.AsReadOnly())) != null)
      {
        int len = cursor.Location - startCursor0.Location;
        parseResult1 = this.ReturnHelper<string>(startCursor0, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor0.Location, len)));
      }
      else
        cursor = startCursor0;
      this.tracer.TraceRuleExit<string>(nameof (fragment), cursor, parseResult1);
      return parseResult1;
    }

    private IParseResult<string> pct_encoded(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (pct_encoded), cursor);
      IParseResult<string> parseResult = (IParseResult<string>) null;
      Cursor startCursor0 = cursor;
      if (this.ParseLiteral(ref cursor, "%") != null)
      {
        if (this.hexdig(ref cursor) != null)
        {
          int len = cursor.Location - startCursor0.Location;
          parseResult = this.ReturnHelper<string>(startCursor0, ref cursor, (Func<Cursor, string>) (state => state.Subject.Substring(startCursor0.Location, len)));
        }
        else
          cursor = startCursor0;
      }
      else
        cursor = startCursor0;
      this.tracer.TraceRuleExit<string>(nameof (pct_encoded), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<string> unreserved(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (unreserved), cursor);
      IParseResult<string> parseResult = ((((((IParseResult<string>) null ?? this.letter(ref cursor)) ?? this.digit(ref cursor)) ?? this.ParseLiteral(ref cursor, "-")) ?? this.ParseLiteral(ref cursor, ".")) ?? this.ParseLiteral(ref cursor, "_")) ?? this.ParseLiteral(ref cursor, "~");
      this.tracer.TraceRuleExit<string>(nameof (unreserved), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<string> sub_delims(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (sub_delims), cursor);
      IParseResult<string> parseResult = (((((((((((IParseResult<string>) null ?? this.ParseLiteral(ref cursor, "!")) ?? this.ParseLiteral(ref cursor, "$")) ?? this.ParseLiteral(ref cursor, "&")) ?? this.ParseLiteral(ref cursor, "'")) ?? this.ParseLiteral(ref cursor, "(")) ?? this.ParseLiteral(ref cursor, ")")) ?? this.ParseLiteral(ref cursor, "*")) ?? this.ParseLiteral(ref cursor, "+")) ?? this.ParseLiteral(ref cursor, ",")) ?? this.ParseLiteral(ref cursor, ";")) ?? this.ParseLiteral(ref cursor, "=");
      this.tracer.TraceRuleExit<string>(nameof (sub_delims), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<string> hexdig(ref Cursor cursor)
    {
      this.tracer.TraceRuleEnter(nameof (hexdig), cursor);
      IParseResult<string> parseResult = (((((((((((((IParseResult<string>) null ?? this.digit(ref cursor)) ?? this.ParseLiteral(ref cursor, "a")) ?? this.ParseLiteral(ref cursor, "A")) ?? this.ParseLiteral(ref cursor, "b")) ?? this.ParseLiteral(ref cursor, "B")) ?? this.ParseLiteral(ref cursor, "c")) ?? this.ParseLiteral(ref cursor, "C")) ?? this.ParseLiteral(ref cursor, "d")) ?? this.ParseLiteral(ref cursor, "D")) ?? this.ParseLiteral(ref cursor, "e")) ?? this.ParseLiteral(ref cursor, "E")) ?? this.ParseLiteral(ref cursor, "f")) ?? this.ParseLiteral(ref cursor, "F");
      this.tracer.TraceRuleExit<string>(nameof (hexdig), cursor, parseResult);
      return parseResult;
    }

    private IParseResult<T> StartRuleHelper<T>(
      Cursor cursor,
      ParseDelegate<T> startRule,
      string ruleName)
    {
      try
      {
        this.storage = new Dictionary<CacheKey, object>();
        return startRule(ref cursor) ?? throw this.ExceptionHelper(cursor, (Func<Cursor, string>) (state => "Failed to parse '" + ruleName + "'."));
      }
      finally
      {
        this.storage = (Dictionary<CacheKey, object>) null;
      }
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

    private class UnknownEnvVarError
    {
      public Identifier Identifier { get; }

      public Cursor IdentifierStartPosition { get; }

      public UnknownEnvVarError(Identifier identifier, Cursor identifierStartPosition)
      {
        this.Identifier = identifier;
        this.IdentifierStartPosition = identifierStartPosition;
      }
    }

    public sealed class ExportedRules
    {
      private RequirementParserInternal parser;

      internal ExportedRules(RequirementParserInternal parser) => this.parser = parser;

      public IParseResult<VersionConstraint> Pep440VersionOne(ref Cursor cursor) => this.parser.Pep440VersionOne(ref cursor);

      public IParseResult<VersionConstraint> LegacyVersionOne(ref Cursor cursor) => this.parser.LegacyVersionOne(ref cursor);

      public IParseResult<Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.VersionConstraintList> VersionConstraintList(
        ref Cursor cursor)
      {
        return this.parser.VersionConstraintList(ref cursor);
      }

      public IParseResult<RequirementSpec> Specification(ref Cursor cursor) => this.parser.Specification(ref cursor);
    }
  }
}
