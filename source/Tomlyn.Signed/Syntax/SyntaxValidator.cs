// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.SyntaxValidator
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Tomlyn.Model;
using Tomlyn.Text;


#nullable enable
namespace Tomlyn.Syntax
{
  internal class SyntaxValidator : SyntaxVisitor
  {
    private readonly DiagnosticsBag _diagnostics;
    private SyntaxValidator.ObjectPath _currentPath;
    private readonly Dictionary<SyntaxValidator.ObjectPath, SyntaxValidator.ObjectPathValue> _maps;
    private int _currentArrayIndex;

    public SyntaxValidator(DiagnosticsBag diagnostics)
    {
      this._diagnostics = diagnostics ?? throw new ArgumentNullException(nameof (diagnostics));
      this._currentPath = new SyntaxValidator.ObjectPath();
      this._maps = new Dictionary<SyntaxValidator.ObjectPath, SyntaxValidator.ObjectPathValue>();
    }

    public override void Visit(KeyValueSyntax keyValue)
    {
      SyntaxValidator.ObjectPath objectPath = this._currentPath.Clone();
      if (keyValue.Key == null)
      {
        this._diagnostics.Error(keyValue.Span, "A KeyValueSyntax must have a non null Key");
      }
      else
      {
        if (!this.KeyNameToObjectPath(keyValue.Key, ObjectKind.Table, true))
          return;
        ObjectKind kind;
        switch (keyValue.Value)
        {
          case ArraySyntax _:
            kind = ObjectKind.Array;
            break;
          case BooleanValueSyntax _:
            kind = ObjectKind.Boolean;
            break;
          case DateTimeValueSyntax dateTimeValueSyntax:
            switch (dateTimeValueSyntax.Kind)
            {
              case SyntaxKind.OffsetDateTimeByZ:
                kind = ObjectKind.OffsetDateTimeByZ;
                break;
              case SyntaxKind.OffsetDateTimeByNumber:
                kind = ObjectKind.OffsetDateTimeByNumber;
                break;
              case SyntaxKind.LocalDateTime:
                kind = ObjectKind.LocalDateTime;
                break;
              case SyntaxKind.LocalDate:
                kind = ObjectKind.LocalDate;
                break;
              case SyntaxKind.LocalTime:
                kind = ObjectKind.LocalTime;
                break;
              default:
                throw new NotSupportedException(string.Format("Unsupported datetime kind `{0}` for the key-value `{1}`", (object) dateTimeValueSyntax.Kind, (object) keyValue));
            }
            break;
          case FloatValueSyntax _:
            kind = ObjectKind.Float;
            break;
          case InlineTableSyntax _:
            kind = ObjectKind.InlineTable;
            break;
          case IntegerValueSyntax _:
            kind = ObjectKind.Integer;
            break;
          case StringValueSyntax _:
            kind = ObjectKind.String;
            break;
          default:
            this._diagnostics.Error(keyValue.Span, keyValue.Value == null ? "A KeyValueSyntax must have a non null Value" : string.Format("Not supported type `{0}` for the value of a KeyValueSyntax", (object) keyValue.Value.Kind));
            return;
        }
        this.AddObjectPath((SyntaxNode) keyValue, kind, false, true);
        base.Visit(keyValue);
        this._currentPath = objectPath;
      }
    }

    public override void Visit(StringValueSyntax stringValue)
    {
      if (stringValue.Token == null)
        this._diagnostics.Error(stringValue.Span, "A StringValueSyntax must have a non null Token");
      base.Visit(stringValue);
    }

    public override void Visit(IntegerValueSyntax integerValue)
    {
      if (integerValue.Token == null)
        this._diagnostics.Error(integerValue.Span, "A IntegerValueSyntax must have a non null Token");
      base.Visit(integerValue);
    }

    public override void Visit(BooleanValueSyntax boolValue)
    {
      if (boolValue.Token == null)
        this._diagnostics.Error(boolValue.Span, "A BooleanValueSyntax must have a non null Token");
      base.Visit(boolValue);
    }

    public override void Visit(FloatValueSyntax floatValue)
    {
      if (floatValue.Token == null)
        this._diagnostics.Error(floatValue.Span, "A FloatValueSyntax must have a non null Token");
      base.Visit(floatValue);
    }

    public override void Visit(TableSyntax table)
    {
      this.VerifyTable((TableSyntaxBase) table);
      SyntaxValidator.ObjectPath objectPath = this._currentPath.Clone();
      if (table.Name == null || !this.KeyNameToObjectPath(table.Name, ObjectKind.Table))
        return;
      this.AddObjectPath((SyntaxNode) table, ObjectKind.Table, false, false);
      base.Visit(table);
      this._currentPath = objectPath;
    }

    public override void Visit(TableArraySyntax table)
    {
      this.VerifyTable((TableSyntaxBase) table);
      SyntaxValidator.ObjectPath objectPath = this._currentPath.Clone();
      if (table.Name == null || !this.KeyNameToObjectPath(table.Name, ObjectKind.TableArray))
        return;
      SyntaxValidator.ObjectPathValue objectPathValue = this.AddObjectPath((SyntaxNode) table, ObjectKind.TableArray, false, false);
      int currentArrayIndex = this._currentArrayIndex;
      this._currentArrayIndex = objectPathValue.ArrayIndex;
      base.Visit(table);
      ++objectPathValue.ArrayIndex;
      this._currentArrayIndex = currentArrayIndex;
      this._currentPath = objectPath;
    }

    public override void Visit(BareKeySyntax bareKey)
    {
      if (bareKey.Key == null)
        this._diagnostics.Error(bareKey.Span, "A BareKeySyntax must have a non null property Key");
      base.Visit(bareKey);
    }

    public override void Visit(KeySyntax key)
    {
      if (key.Key == null)
        this._diagnostics.Error(key.Span, "A KeySyntax must have a non null property Key");
      base.Visit(key);
    }

    public override void Visit(DateTimeValueSyntax dateTime)
    {
      if (dateTime.Token == null)
        this._diagnostics.Error(dateTime.Span, "A DateTimeValueSyntax must have a non null Token");
      base.Visit(dateTime);
    }

    private void VerifyTable(TableSyntaxBase table)
    {
      bool flag = table is TableArraySyntax;
      if (table.OpenBracket == null)
        this._diagnostics.Error(table.Span, string.Format("The table{0} must have an {1} `{2}`", flag ? (object) " array" : (object) string.Empty, (object) table.OpenTokenKind, (object) table.OpenTokenKind.ToText()));
      if (table.CloseBracket == null)
        this._diagnostics.Error(table.Span, string.Format("The table{0} must have an {1} `{2}`", flag ? (object) " array" : (object) string.Empty, (object) table.CloseTokenKind, (object) table.CloseTokenKind.ToText()));
      if (table.EndOfLineToken == null && table.Items.ChildrenCount > 0)
        this._diagnostics.Error(table.Span, "The table" + (flag ? " array" : string.Empty) + " must have a EndOfLine set after the open/closing brackets and before any elements");
      if (table.Name != null)
        return;
      this._diagnostics.Error(table.Span, "The table" + (flag ? " array" : string.Empty) + " must have a name");
    }

    private bool KeyNameToObjectPath(KeySyntax key, ObjectKind kind, bool fromDottedKeys = false)
    {
      if (key.Key == null)
        this._diagnostics.Error(key.Span, "The property KeySyntax.Key cannot be null");
      string stringFromBasic1 = this.GetStringFromBasic(key.Key);
      if (string.IsNullOrWhiteSpace(stringFromBasic1))
        return false;
      this._currentPath.Add(stringFromBasic1);
      SyntaxList<DottedKeyItemSyntax> dotKeys = key.DotKeys;
      for (int index = 0; index < dotKeys.ChildrenCount; ++index)
      {
        this.AddObjectPath((SyntaxNode) key, kind, true, fromDottedKeys);
        string stringFromBasic2 = this.GetStringFromBasic(dotKeys.GetChild(index).Key);
        if (string.IsNullOrWhiteSpace(stringFromBasic2))
          return false;
        this._currentPath.Add(stringFromBasic2);
      }
      return true;
    }

    private SyntaxValidator.ObjectPathValue AddObjectPath(
      SyntaxNode node,
      ObjectKind kind,
      bool isImplicit,
      bool fromDottedKeys)
    {
      SyntaxValidator.ObjectPath key = this._currentPath.Clone();
      if (kind == ObjectKind.TableArray & isImplicit)
        kind = ObjectKind.Table;
      SyntaxValidator.ObjectPathValue objectPathValue;
      if (this._maps.TryGetValue(key, out objectPathValue))
      {
        if (((kind != ObjectKind.Table || objectPathValue.IsImplicit && (!objectPathValue.FromDottedKeys || fromDottedKeys && objectPathValue.FromDottedKeys) ? 1 : (!isImplicit ? 0 : (!fromDottedKeys ? 1 : 0))) & (objectPathValue.Kind != ObjectKind.TableArray || kind != ObjectKind.TableArray && !(kind == ObjectKind.Table & isImplicit) ? (objectPathValue.Kind != ObjectKind.Table ? (false ? 1 : 0) : (kind == ObjectKind.Table ? 1 : 0)) : (true ? 1 : 0))) == 0)
          this._diagnostics.Error(node.Span, string.Format("The key `{0}` is already defined at {1} with `{2}` and cannot be redefined", (object) key, (object) objectPathValue.Node.Span.Start, (object) objectPathValue.Node.ToString().TrimEnd('\r', '\n').ToPrintableString()));
        else if (objectPathValue.Kind == ObjectKind.TableArray)
          this._currentPath.Add(objectPathValue.ArrayIndex);
      }
      else
      {
        objectPathValue = new SyntaxValidator.ObjectPathValue(node, kind, isImplicit, fromDottedKeys);
        this._maps.Add(key, objectPathValue);
      }
      return objectPathValue;
    }

    private string? GetStringFromBasic(BareKeyOrStringValueSyntax value) => !(value is BareKeySyntax bareKeySyntax) ? ((StringValueSyntax) value).Value : bareKeySyntax.Key?.Text;

    public override void Visit(ArraySyntax array)
    {
      int currentArrayIndex = this._currentArrayIndex;
      if (array.OpenBracket == null)
        this._diagnostics.Error(array.Span, "The array must have an OpenBracket `[`");
      else if (array.CloseBracket == null)
        this._diagnostics.Error(array.Span, "The array must have an CloseBracket `[`");
      SyntaxList<ArrayItemSyntax> items = array.Items;
      for (int index = 0; index < items.ChildrenCount; ++index)
      {
        ArrayItemSyntax child = items.GetChild(index);
        ValueSyntax valueSyntax = child.Value;
        if (index == 0)
        {
          int kind = (int) valueSyntax.Kind;
        }
        if (index + 1 < items.ChildrenCount && child.Comma == null)
          this._diagnostics.Error(child.Span, string.Format("The array item [{0}] must have a comma `,`", (object) index));
      }
      base.Visit(array);
      this._currentArrayIndex = currentArrayIndex;
    }

    public override void Visit(InlineTableItemSyntax inlineTableItem) => base.Visit(inlineTableItem);

    public override void Visit(ArrayItemSyntax arrayItem)
    {
      this._currentPath.Add(this._currentArrayIndex);
      if (arrayItem.Value == null)
        this._diagnostics.Error(arrayItem.Span, string.Format("The array item [{0}] must have a non null value", (object) this._currentArrayIndex));
      base.Visit(arrayItem);
      ++this._currentArrayIndex;
    }

    public override void Visit(DottedKeyItemSyntax dottedKeyItem) => base.Visit(dottedKeyItem);

    public override void Visit(InlineTableSyntax inlineTable) => base.Visit(inlineTable);

    private class ObjectPath : List<SyntaxValidator.ObjectPathItem>
    {
      private int _hashCode;

      public void Add(string key)
      {
        this._hashCode = this._hashCode * 397 ^ key.GetHashCode();
        this.Add(new SyntaxValidator.ObjectPathItem(key));
      }

      public void Add(int index)
      {
        this._hashCode = this._hashCode * 397 ^ index;
        this.Add(new SyntaxValidator.ObjectPathItem(index));
      }

      public SyntaxValidator.ObjectPath Clone() => (SyntaxValidator.ObjectPath) this.MemberwiseClone();

      public override bool Equals(object? obj)
      {
        // ISSUE: explicit non-virtual call
        int? nullable = obj is SyntaxValidator.ObjectPath objectPath ? new int?(__nonvirtual (objectPath.Count)) : new int?();
        int count = this.Count;
        if (!(nullable.GetValueOrDefault() == count & nullable.HasValue) || objectPath._hashCode != this._hashCode)
          return false;
        for (int index = 0; index < this.Count; ++index)
        {
          if (this[index] != objectPath[index])
            return false;
        }
        return true;
      }

      public override int GetHashCode() => this._hashCode;

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        for (int index = 0; index < this.Count; ++index)
        {
          if (index > 0)
            stringBuilder.Append('.');
          stringBuilder.Append((object) this[index]);
        }
        return stringBuilder.ToString();
      }
    }

    [DebuggerDisplay("{Node} - {Kind}")]
    private class ObjectPathValue
    {
      public readonly SyntaxNode Node;
      public readonly ObjectKind Kind;
      public readonly bool IsImplicit;
      public readonly bool FromDottedKeys;
      public int ArrayIndex;

      public ObjectPathValue(
        SyntaxNode node,
        ObjectKind kind,
        bool isImplicit,
        bool fromDottedKeys)
      {
        this.FromDottedKeys = fromDottedKeys;
        this.Node = node;
        this.Kind = kind;
        this.IsImplicit = isImplicit;
      }
    }

    private readonly struct ObjectPathItem : IEquatable<SyntaxValidator.ObjectPathItem>
    {
      public readonly string? Key;
      public readonly int Index;

      public ObjectPathItem(string key)
        : this()
      {
        this.Key = key;
      }

      public ObjectPathItem(int index)
        : this()
      {
        this.Index = index;
      }

      public bool Equals(SyntaxValidator.ObjectPathItem other) => string.Equals(this.Key, other.Key) && this.Index == other.Index;

      public override bool Equals(object? obj) => obj != null && obj is SyntaxValidator.ObjectPathItem other && this.Equals(other);

      public override int GetHashCode() => (this.Key != null ? this.Key.GetHashCode() : 0) * 397 ^ this.Index;

      public static bool operator ==(
        SyntaxValidator.ObjectPathItem left,
        SyntaxValidator.ObjectPathItem right)
      {
        return left.Equals(right);
      }

      public static bool operator !=(
        SyntaxValidator.ObjectPathItem left,
        SyntaxValidator.ObjectPathItem right)
      {
        return !left.Equals(right);
      }

      public override string ToString() => this.Key ?? string.Format("[{0}]", (object) this.Index);
    }

    private enum KeySource
    {
      Table,
      KeyValue,
    }
  }
}
