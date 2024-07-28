// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.ScopeParser
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal class ScopeParser
  {
    private readonly string expression;
    private readonly Stack<ScopeParser.Operand> output = new Stack<ScopeParser.Operand>();
    private readonly Stack<ScopeParser.Operator> operators = new Stack<ScopeParser.Operator>();
    private readonly IDictionary<string, IScopeFilterProvider> providedFilters;
    private readonly Regex stringRegex = new Regex("'(.*)?'");
    private int expressionIndex;

    internal ScopeParser(
      string expression,
      IDictionary<string, IScopeFilterProvider> providedFilters)
    {
      expression.RequiresArgumentNotNull<string>(nameof (expression));
      providedFilters.RequiresArgumentNotNull<IDictionary<string, IScopeFilterProvider>>(nameof (providedFilters));
      this.expression = expression;
      this.providedFilters = providedFilters;
    }

    public bool Run()
    {
      while (this.expressionIndex < this.expression.Length)
      {
        switch (this.expression[this.expressionIndex])
        {
          case '\t':
          case ' ':
            ++this.expressionIndex;
            continue;
          case '!':
          case '&':
          case '(':
          case ')':
          case '<':
          case '=':
          case '>':
          case '|':
            this.ParseOperator(this.ScanOperator());
            continue;
          default:
            this.ParseOperand(this.ScanOperand());
            continue;
        }
      }
      while (this.operators.Count != 0)
        this.EvaluateOutput();
      return this.output.Count == 1 ? this.output.Peek().ToBool() : throw new ScopeParserException("Operand and operator count mismatch");
    }

    private void ParseOperator(ScopeParser.Operator op)
    {
      if (op.IdValue == ScopeParser.Operator.Id.Opn)
        this.operators.Push(op);
      else if (op.IdValue == ScopeParser.Operator.Id.Cls)
      {
        while (this.operators.Count != 0 && this.operators.Peek().IdValue != ScopeParser.Operator.Id.Opn)
          this.EvaluateOutput();
        if (this.operators.Count == 0)
          return;
        this.operators.Pop();
      }
      else
      {
        while (this.operators.Count != 0 && ((int) op.Priority < (int) this.operators.Peek().Priority || (int) op.Priority == (int) this.operators.Peek().Priority && op.FixityValue == ScopeParser.Operator.Fixity.Left))
          this.EvaluateOutput();
        this.operators.Push(op);
      }
    }

    private void ParseOperand(ScopeParser.Operand op) => this.output.Push(op);

    private ScopeParser.Operator ScanOperator()
    {
      string key = this.expression[this.expressionIndex++].ToString();
      if (this.expressionIndex < this.expression.Length)
      {
        char ch = this.expression[this.expressionIndex];
        switch (ch)
        {
          case '&':
          case '=':
          case '|':
            key += ch.ToString();
            ++this.expressionIndex;
            break;
        }
      }
      ScopeParser.Operator @operator;
      if (!ScopeParser.Operator.StrMap.TryGetValue(key, out @operator))
        throw new ScopeParserException("Invalid operator: " + key);
      return @operator;
    }

    private ScopeParser.Operand ScanOperand()
    {
      Func<char, bool> isAlphabet = (Func<char, bool>) (c =>
      {
        if (c >= 'A' && c <= 'Z')
          return true;
        return c >= 'a' && c <= 'z';
      });
      Func<char, bool> isNumeral = (Func<char, bool>) (n => n >= '0' && n <= '9');
      Func<char, bool> isSpecial = (Func<char, bool>) (c => c == '.' || c == '_' || c == '\'');
      Func<char, bool> predicate = (Func<char, bool>) (c => isAlphabet(c) || isNumeral(c) || isSpecial(c));
      string str = new string(this.expression.Skip<char>(this.expressionIndex).TakeWhile<char>(predicate).ToArray<char>());
      this.expressionIndex += str.Length;
      if (str.Length == 0)
        throw new ScopeParserException("Missing operand: " + this.GetRestOfExpression());
      double result;
      if (double.TryParse(str, out result))
        return (ScopeParser.Operand) new ScopeParser.DoubleOperand(result);
      Match match = this.stringRegex.Match(str);
      if (match.Success)
        return (ScopeParser.Operand) new ScopeParser.StringOperand(match.Groups[1].Value);
      string subkey = (string) null;
      string[] strArray = str.Split('.');
      string key;
      if (strArray.Length == 2)
      {
        key = strArray[0];
        subkey = strArray[1];
      }
      else
        key = str;
      IScopeFilterProvider scopeFilterProvider;
      if (this.providedFilters.TryGetValue(key, out scopeFilterProvider))
      {
        ISingleValueScopeFilterProvider<ScopeValue> singleValue = scopeFilterProvider as ISingleValueScopeFilterProvider<ScopeValue>;
        if (singleValue != null)
        {
          if (subkey != null)
            throw new ScopeParserException("Filter has subkey, but only single-value provider");
          return (ScopeParser.Operand) new ScopeParser.LazyOperand((Func<ScopeParser.Operand>) (() => singleValue.Provide().GetOperand()));
        }
        IMultiValueScopeFilterProvider<ScopeValue> multiValue = scopeFilterProvider as IMultiValueScopeFilterProvider<ScopeValue>;
        if (multiValue != null)
          return subkey != null ? (ScopeParser.Operand) new ScopeParser.LazyOperand((Func<ScopeParser.Operand>) (() => multiValue.Provide(subkey).GetOperand())) : throw new ScopeParserException("Filter has no subkey, yet multi-value provider");
      }
      return (ScopeParser.Operand) new ScopeParser.InvalidOperand("Could not find provided scope with name: " + key);
    }

    private void EvaluateOutput()
    {
      ScopeParser.Operator @operator = this.operators.Count != 0 ? this.operators.Pop() : throw new ScopeParserException("Missing operator(s): " + this.GetRestOfExpression());
      if (@operator.TypeValue == ScopeParser.Operator.Type.Unary && this.output.Count >= 1)
      {
        ScopeParser.Operand operand = this.output.Pop();
        this.output.Push(@operator.TheFuncDelegate(operand, operand));
      }
      else
      {
        if (@operator.TypeValue != ScopeParser.Operator.Type.Binary || this.output.Count < 2)
          throw new ScopeParserException("Missing operand(s): " + this.GetRestOfExpression());
        ScopeParser.Operand b = this.output.Pop();
        ScopeParser.Operand a = this.output.Pop();
        this.output.Push(@operator.TheFuncDelegate(a, b));
      }
    }

    private string GetRestOfExpression() => this.expression.Substring(this.expressionIndex);

    internal class InvalidOperand : ScopeParser.Operand
    {
      private string errorMessage;

      public InvalidOperand(string errorMessage) => this.errorMessage = errorMessage;

      public override bool ToBool() => throw new ScopeParserException(this.errorMessage);

      public override int CompareTo(ScopeParser.Operand a) => throw new ScopeParserException(this.errorMessage);
    }

    internal class BoolOperand : ScopeParser.Operand
    {
      public BoolOperand(bool value) => this.Value = (object) value;

      public override bool ToBool() => (bool) this.Value;

      public override int CompareTo(ScopeParser.Operand a) => a.Value is bool ? ((bool) this.Value).CompareTo((bool) a.Value) : throw new ScopeParserException("Trying to compare two different types");
    }

    internal class LazyOperand : ScopeParser.Operand
    {
      private Lazy<ScopeParser.Operand> lazy;

      public override object Value => this.lazy.Value.Value;

      public LazyOperand(Func<ScopeParser.Operand> value) => this.lazy = new Lazy<ScopeParser.Operand>(value);

      public override int CompareTo(ScopeParser.Operand a) => this.lazy.Value.CompareTo(a);

      public override bool ToBool() => this.lazy.Value.ToBool();
    }

    internal class StringOperand : ScopeParser.Operand
    {
      public StringOperand(string value) => this.Value = (object) value;

      public override bool ToBool() => this.Value != null;

      public override int CompareTo(ScopeParser.Operand a) => a.Value is string ? ((string) this.Value).CompareTo((string) a.Value) : throw new ScopeParserException("Trying to compare two different types");
    }

    internal class DoubleOperand : ScopeParser.Operand
    {
      public DoubleOperand(double value) => this.Value = (object) value;

      public override bool ToBool() => (double) this.Value != 0.0;

      public override int CompareTo(ScopeParser.Operand a) => a is ScopeParser.DoubleOperand ? ((double) this.Value).CompareTo((double) a.Value) : throw new ScopeParserException("Trying to compare two different types");
    }

    internal abstract class Operand : IComparable<ScopeParser.Operand>
    {
      public virtual object Value { get; set; }

      public abstract int CompareTo(ScopeParser.Operand a);

      public bool Equals(ScopeParser.Operand y) => this.CompareTo(y) == 0;

      public abstract bool ToBool();

      public static ScopeParser.Operand operator &(ScopeParser.Operand a, ScopeParser.Operand b) => (ScopeParser.Operand) new ScopeParser.LazyOperand((Func<ScopeParser.Operand>) (() => (ScopeParser.Operand) new ScopeParser.BoolOperand(a.ToBool() && b.ToBool())));

      public static ScopeParser.Operand operator |(ScopeParser.Operand a, ScopeParser.Operand b) => (ScopeParser.Operand) new ScopeParser.LazyOperand((Func<ScopeParser.Operand>) (() => (ScopeParser.Operand) new ScopeParser.BoolOperand(a.ToBool() || b.ToBool())));

      public static bool operator true(ScopeParser.Operand a) => a.ToBool();

      public static bool operator false(ScopeParser.Operand a) => !a.ToBool();

      public static bool operator !(ScopeParser.Operand a) => !a.ToBool();
    }

    internal struct Operator
    {
      internal ScopeParser.Operator.Id IdValue;
      internal ushort Priority;
      internal ScopeParser.Operator.Fixity FixityValue;
      internal ScopeParser.Operator.Type TypeValue;
      internal ScopeParser.Operator.FuncDelegate TheFuncDelegate;
      internal static readonly ScopeParser.Operator.FuncDelegate Non = (ScopeParser.Operator.FuncDelegate) ((a, b) => (ScopeParser.Operand) new ScopeParser.BoolOperand(false));
      internal static readonly ScopeParser.Operator.FuncDelegate Not = (ScopeParser.Operator.FuncDelegate) ((a, b) => (ScopeParser.Operand) new ScopeParser.BoolOperand(!a));
      internal static readonly ScopeParser.Operator.FuncDelegate Gt = (ScopeParser.Operator.FuncDelegate) ((a, b) => (ScopeParser.Operand) new ScopeParser.BoolOperand(a.CompareTo(b) > 0));
      internal static readonly ScopeParser.Operator.FuncDelegate Lt = (ScopeParser.Operator.FuncDelegate) ((a, b) => (ScopeParser.Operand) new ScopeParser.BoolOperand(a.CompareTo(b) < 0));
      internal static readonly ScopeParser.Operator.FuncDelegate Gte = (ScopeParser.Operator.FuncDelegate) ((a, b) => (ScopeParser.Operand) new ScopeParser.BoolOperand(a.CompareTo(b) >= 0));
      internal static readonly ScopeParser.Operator.FuncDelegate Lte = (ScopeParser.Operator.FuncDelegate) ((a, b) => (ScopeParser.Operand) new ScopeParser.BoolOperand(a.CompareTo(b) <= 0));
      internal static readonly ScopeParser.Operator.FuncDelegate Eq = (ScopeParser.Operator.FuncDelegate) ((a, b) => (ScopeParser.Operand) new ScopeParser.BoolOperand(a.Equals(b)));
      internal static readonly ScopeParser.Operator.FuncDelegate Neq = (ScopeParser.Operator.FuncDelegate) ((a, b) => (ScopeParser.Operand) new ScopeParser.BoolOperand(!a.Equals(b)));
      internal static readonly ScopeParser.Operator.FuncDelegate And = (ScopeParser.Operator.FuncDelegate) ((a, b) =>
      {
        ScopeParser.Operand operand = a;
        return operand ? operand & b : operand;
      });
      internal static readonly ScopeParser.Operator.FuncDelegate Or = (ScopeParser.Operator.FuncDelegate) ((a, b) =>
      {
        ScopeParser.Operand operand = a;
        return !(operand ? true : false) ? operand | b : operand;
      });
      internal static readonly IDictionary<string, ScopeParser.Operator> StrMap = (IDictionary<string, ScopeParser.Operator>) new Dictionary<string, ScopeParser.Operator>()
      {
        {
          "(",
          new ScopeParser.Operator()
          {
            IdValue = ScopeParser.Operator.Id.Opn,
            Priority = (ushort) 0,
            FixityValue = ScopeParser.Operator.Fixity.Left,
            TypeValue = ScopeParser.Operator.Type.Unary,
            TheFuncDelegate = ScopeParser.Operator.Non
          }
        },
        {
          ")",
          new ScopeParser.Operator()
          {
            IdValue = ScopeParser.Operator.Id.Cls,
            Priority = (ushort) 0,
            FixityValue = ScopeParser.Operator.Fixity.Left,
            TypeValue = ScopeParser.Operator.Type.Unary,
            TheFuncDelegate = ScopeParser.Operator.Non
          }
        },
        {
          "!",
          new ScopeParser.Operator()
          {
            IdValue = ScopeParser.Operator.Id.Not,
            Priority = (ushort) 80,
            FixityValue = ScopeParser.Operator.Fixity.Right,
            TypeValue = ScopeParser.Operator.Type.Unary,
            TheFuncDelegate = ScopeParser.Operator.Not
          }
        },
        {
          ">",
          new ScopeParser.Operator()
          {
            IdValue = ScopeParser.Operator.Id.Gt,
            Priority = (ushort) 64,
            FixityValue = ScopeParser.Operator.Fixity.Left,
            TypeValue = ScopeParser.Operator.Type.Binary,
            TheFuncDelegate = ScopeParser.Operator.Gt
          }
        },
        {
          "<",
          new ScopeParser.Operator()
          {
            IdValue = ScopeParser.Operator.Id.Lt,
            Priority = (ushort) 64,
            FixityValue = ScopeParser.Operator.Fixity.Left,
            TypeValue = ScopeParser.Operator.Type.Binary,
            TheFuncDelegate = ScopeParser.Operator.Lt
          }
        },
        {
          ">=",
          new ScopeParser.Operator()
          {
            IdValue = ScopeParser.Operator.Id.Gte,
            Priority = (ushort) 64,
            FixityValue = ScopeParser.Operator.Fixity.Left,
            TypeValue = ScopeParser.Operator.Type.Binary,
            TheFuncDelegate = ScopeParser.Operator.Gte
          }
        },
        {
          "<=",
          new ScopeParser.Operator()
          {
            IdValue = ScopeParser.Operator.Id.Lte,
            Priority = (ushort) 64,
            FixityValue = ScopeParser.Operator.Fixity.Left,
            TypeValue = ScopeParser.Operator.Type.Binary,
            TheFuncDelegate = ScopeParser.Operator.Lte
          }
        },
        {
          "==",
          new ScopeParser.Operator()
          {
            IdValue = ScopeParser.Operator.Id.Eq,
            Priority = (ushort) 48,
            FixityValue = ScopeParser.Operator.Fixity.Left,
            TypeValue = ScopeParser.Operator.Type.Binary,
            TheFuncDelegate = ScopeParser.Operator.Eq
          }
        },
        {
          "!=",
          new ScopeParser.Operator()
          {
            IdValue = ScopeParser.Operator.Id.Neq,
            Priority = (ushort) 48,
            FixityValue = ScopeParser.Operator.Fixity.Left,
            TypeValue = ScopeParser.Operator.Type.Binary,
            TheFuncDelegate = ScopeParser.Operator.Neq
          }
        },
        {
          "&&",
          new ScopeParser.Operator()
          {
            IdValue = ScopeParser.Operator.Id.And,
            Priority = (ushort) 32,
            FixityValue = ScopeParser.Operator.Fixity.Left,
            TypeValue = ScopeParser.Operator.Type.Binary,
            TheFuncDelegate = ScopeParser.Operator.And
          }
        },
        {
          "||",
          new ScopeParser.Operator()
          {
            IdValue = ScopeParser.Operator.Id.Or,
            Priority = (ushort) 16,
            FixityValue = ScopeParser.Operator.Fixity.Left,
            TypeValue = ScopeParser.Operator.Type.Binary,
            TheFuncDelegate = ScopeParser.Operator.Or
          }
        }
      };

      internal delegate ScopeParser.Operand FuncDelegate(
        ScopeParser.Operand a,
        ScopeParser.Operand b);

      internal enum Id
      {
        Non,
        Opn,
        Cls,
        Not,
        And,
        Or,
        Eq,
        Neq,
        Gt,
        Lt,
        Gte,
        Lte,
      }

      internal enum Fixity
      {
        Left,
        Right,
      }

      internal enum Type
      {
        Unary,
        Binary,
      }
    }
  }
}
