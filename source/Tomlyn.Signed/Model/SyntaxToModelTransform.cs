// Decompiled with JetBrains decompiler
// Type: Tomlyn.Model.SyntaxToModelTransform
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Tomlyn.Model.Accessors;
using Tomlyn.Syntax;


#nullable enable
namespace Tomlyn.Model
{
  internal class SyntaxToModelTransform : SyntaxVisitor
  {
    private readonly DynamicModelReadContext _context;
    private object? _currentObject;
    private DynamicAccessor? _currentObjectAccessor;
    private Type? _currentTargetType;
    private object? _currentValue;
    private readonly Stack<SyntaxToModelTransform.ObjectPath> _objectStack;
    private readonly HashSet<object> _tableArrays;

    public SyntaxToModelTransform(DynamicModelReadContext context, object rootObject)
    {
      this._context = context;
      this._objectStack = new Stack<SyntaxToModelTransform.ObjectPath>();
      this._tableArrays = new HashSet<object>((IEqualityComparer<object>) SyntaxToModelTransform.ReferenceEqualityComparer.Instance);
      this.PushObject(rootObject);
    }

    private void PushObject(object obj)
    {
      DynamicAccessor accessor = this._context.GetAccessor(obj.GetType());
      this._objectStack.Push(new SyntaxToModelTransform.ObjectPath(obj, accessor));
      this._currentObject = obj;
      this._currentObjectAccessor = accessor;
    }

    private void PopStack(int targetCount)
    {
      while (this._objectStack.Count > targetCount)
        this.PopObject();
    }

    private object PopObject()
    {
      this._objectStack.Pop();
      SyntaxToModelTransform.ObjectPath objectPath = this._objectStack.Peek();
      this._currentObject = objectPath.Value;
      this._currentObjectAccessor = objectPath.DynamicAccessor;
      return this._currentObject;
    }

    public override void Visit(KeyValueSyntax keyValue)
    {
      int count = this._objectStack.Count;
      try
      {
        object currentValue = this._currentValue;
        Type currentTargetType = this._currentTargetType;
        string name;
        if (!this.TryFollowKeyPath(keyValue.Key, keyValue.Kind, out name))
          return;
        if (((ObjectDynamicAccessor) this._currentObjectAccessor).TryGetPropertyType(keyValue.Span, name, out this._currentTargetType))
        {
          keyValue.Value.Accept((SyntaxVisitor) this);
          ((ObjectDynamicAccessor) this._currentObjectAccessor).TrySetPropertyValue(keyValue.Span, this._currentObject, name, this._currentValue);
          this.AddMetadataToCurrentObject(name, (SyntaxNode) keyValue);
        }
        this._currentValue = currentValue;
        this._currentTargetType = currentTargetType;
      }
      finally
      {
        this.PopStack(count);
      }
    }

    private void AddMetadataToCurrentObject(string name, SyntaxNode syntax)
    {
      if (!(this._currentObject is ITomlMetadataProvider currentObject))
        return;
      TomlPropertiesMetadata propertiesMetadata = currentObject.PropertiesMetadata ?? new TomlPropertiesMetadata();
      TomlPropertyMetadata propertyMetadata = this.GetTomlPropertyMetadata(syntax);
      if (propertyMetadata != null)
        propertiesMetadata.SetProperty(name, propertyMetadata);
      currentObject.PropertiesMetadata = propertiesMetadata;
    }

    public override void Visit(TableSyntax table)
    {
      int count = this._objectStack.Count;
      try
      {
        string name;
        if (!this.TryFollowKeyPath(table.Name, table.Kind, out name))
          return;
        this.AddMetadataToCurrentObject(name, (SyntaxNode) table);
        base.Visit(table);
      }
      finally
      {
        this.PopStack(count);
      }
    }

    public override void Visit(TableArraySyntax table)
    {
      int count = this._objectStack.Count;
      try
      {
        string name;
        if (!this.TryFollowKeyPath(table.Name, table.Kind, out name))
          return;
        this.AddMetadataToCurrentObject(name, (SyntaxNode) table);
        base.Visit(table);
      }
      finally
      {
        this.PopStack(count);
      }
    }

    private bool TryFollowKeyPath(KeySyntax key, SyntaxKind kind, out string name)
    {
      name = this.GetStringFromBasic(key.Key) ?? string.Empty;
      SyntaxList<DottedKeyItemSyntax> dotKeys = key.DotKeys;
      SyntaxNode syntaxNode = (SyntaxNode) key;
      for (int index = 0; index < dotKeys.ChildrenCount; ++index)
      {
        if (!this.GetOrCreateSubObject(syntaxNode.Span, name, ObjectKind.Table))
          return false;
        DottedKeyItemSyntax child = dotKeys.GetChild(index);
        syntaxNode = (SyntaxNode) child;
        name = this.GetStringFromBasic(child.Key) ?? string.Empty;
      }
      return kind != SyntaxKind.Table && kind != SyntaxKind.TableArray || this.GetOrCreateSubObject(key.Span, name, SyntaxToModelTransform.GetKindFromSyntaxKind(kind));
    }

    private static ObjectKind GetKindFromSyntaxKind(SyntaxKind kind)
    {
      switch (kind)
      {
        case SyntaxKind.Array:
          return ObjectKind.Array;
        case SyntaxKind.InlineTable:
          return ObjectKind.InlineTable;
        case SyntaxKind.Table:
          return ObjectKind.Table;
        case SyntaxKind.TableArray:
          return ObjectKind.TableArray;
        default:
          throw new ArgumentOutOfRangeException(nameof (kind), (object) kind, string.Format("Invalid {0}", (object) kind));
      }
    }

    private bool GetOrCreateSubObject(SourceSpan span, string key, ObjectKind kind)
    {
      if (!(this._currentObjectAccessor is ObjectDynamicAccessor currentObjectAccessor))
      {
        this._context.Diagnostics.Error(span, "Unable to set a key " + key + " on an object accessor " + this._currentObjectAccessor.TargetType.FullName);
        return false;
      }
      object instance;
      if ((!currentObjectAccessor.TryGetPropertyValue(span, this._currentObject, key, out instance) || instance == null) && !currentObjectAccessor.TryCreateAndSetDefaultPropertyValue(span, this._currentObject, key, kind, out instance))
        return false;
      if (instance != null)
      {
        if (kind == ObjectKind.TableArray)
          this._tableArrays.Add(instance);
        if (this._tableArrays.Contains(instance))
        {
          ListDynamicAccessor accessor = (ListDynamicAccessor) this._context.GetAccessor(instance.GetType());
          if (kind == ObjectKind.TableArray)
          {
            object obj = this._context.CreateInstance(accessor.ElementType, ObjectKind.Table);
            accessor.AddElement(instance, obj);
            instance = obj;
          }
          else
            instance = accessor.GetLastElement(instance);
        }
      }
      if (instance == null)
        return false;
      this.PushObject(instance);
      return true;
    }

    private string? GetStringFromBasic(BareKeyOrStringValueSyntax value)
    {
      if (!(value is BareKeySyntax bareKeySyntax))
        return ((StringValueSyntax) value).Value;
      return bareKeySyntax.Key?.Text;
    }

    public override void Visit(BooleanValueSyntax boolValue) => this._currentValue = (object) boolValue.Value;

    public override void Visit(StringValueSyntax stringValue) => this._currentValue = (object) stringValue.Value;

    public override void Visit(DateTimeValueSyntax dateTimeValueSyntax) => this._currentValue = (object) dateTimeValueSyntax.Value;

    public override void Visit(FloatValueSyntax floatValueSyntax) => this._currentValue = (object) floatValueSyntax.Value;

    public override void Visit(IntegerValueSyntax integerValueSyntax) => this._currentValue = (object) integerValueSyntax.Value;

    public override void Visit(ArraySyntax arraySyntax)
    {
      int count = this._objectStack.Count;
      try
      {
        SyntaxList<ArrayItemSyntax> items = arraySyntax.Items;
        Type currentTargetType = this._currentTargetType;
        object list1 = currentTargetType.IsArray ? (object) Array.CreateInstance(currentTargetType.GetElementType(), items.ChildrenCount) : this._context.CreateInstance(currentTargetType, ObjectKind.Array);
        Array array = list1 as Array;
        IList list2 = list1 as IList;
        DynamicAccessor accessor = this._context.GetAccessor(list1.GetType());
        if (!(accessor is ListDynamicAccessor listDynamicAccessor))
        {
          this._context.Diagnostics.Error(arraySyntax.Span, string.Format("Invalid list type {0}. Getting a {1} instead.", (object) list1.GetType().FullName, (object) accessor));
        }
        else
        {
          for (int index = 0; index < items.ChildrenCount; ++index)
          {
            this._currentTargetType = listDynamicAccessor.ElementType;
            ArrayItemSyntax child = items.GetChild(index);
            child.Accept((SyntaxVisitor) this);
            object outputValue;
            if (this._context.TryConvertValue(child.Span, this._currentValue, listDynamicAccessor.ElementType, out outputValue))
            {
              if (array != null)
                array.SetValue(outputValue, index);
              else if (list2 != null)
                list2.Add(outputValue);
              else
                listDynamicAccessor.AddElement(list1, outputValue);
            }
          }
          this._currentValue = list1;
          this._currentTargetType = currentTargetType;
        }
      }
      finally
      {
        this.PopStack(count);
      }
    }

    public override void Visit(InlineTableSyntax inlineTable)
    {
      int count = this._objectStack.Count;
      object obj = this._context.CreateInstance(this._currentTargetType, ObjectKind.InlineTable);
      this.PushObject(obj);
      base.Visit(inlineTable);
      this.PopStack(count);
      this._currentValue = obj;
    }

    private TomlPropertyMetadata? GetTomlPropertyMetadata(SyntaxNode syntax)
    {
      TomlPropertyMetadata propertyMetadata = (TomlPropertyMetadata) null;
      switch (syntax)
      {
        case KeyValueSyntax keyValueSyntax:
          switch (keyValueSyntax.Value)
          {
            case DateTimeValueSyntax dateTimeValueSyntax:
              SyntaxToken token1 = dateTimeValueSyntax.Token;
              TokenKind tokenKind1 = token1 != null ? token1.TokenKind : TokenKind.Invalid;
              TomlPropertyDisplayKind propertyDisplayKind1 = TomlPropertyDisplayKind.Default;
              switch (tokenKind1)
              {
                case TokenKind.OffsetDateTimeByZ:
                  propertyDisplayKind1 = TomlPropertyDisplayKind.OffsetDateTimeByZ;
                  break;
                case TokenKind.OffsetDateTimeByNumber:
                  propertyDisplayKind1 = TomlPropertyDisplayKind.OffsetDateTimeByNumber;
                  break;
                case TokenKind.LocalDateTime:
                  propertyDisplayKind1 = TomlPropertyDisplayKind.LocalDateTime;
                  break;
                case TokenKind.LocalDate:
                  propertyDisplayKind1 = TomlPropertyDisplayKind.LocalDate;
                  break;
                case TokenKind.LocalTime:
                  propertyDisplayKind1 = TomlPropertyDisplayKind.LocalTime;
                  break;
              }
              if (propertyDisplayKind1 != TomlPropertyDisplayKind.Default)
              {
                propertyMetadata = new TomlPropertyMetadata()
                {
                  DisplayKind = propertyDisplayKind1
                };
                break;
              }
              break;
            case StringValueSyntax stringValueSyntax:
              SyntaxToken token2 = stringValueSyntax.Token;
              TokenKind tokenKind2 = token2 != null ? token2.TokenKind : TokenKind.Invalid;
              TomlPropertyDisplayKind propertyDisplayKind2 = TomlPropertyDisplayKind.Default;
              switch (tokenKind2)
              {
                case TokenKind.StringMulti:
                  propertyDisplayKind2 = TomlPropertyDisplayKind.StringMulti;
                  break;
                case TokenKind.StringLiteral:
                  propertyDisplayKind2 = TomlPropertyDisplayKind.StringLiteral;
                  break;
                case TokenKind.StringLiteralMulti:
                  propertyDisplayKind2 = TomlPropertyDisplayKind.StringLiteralMulti;
                  break;
              }
              if (propertyDisplayKind2 != TomlPropertyDisplayKind.Default)
              {
                propertyMetadata = new TomlPropertyMetadata()
                {
                  DisplayKind = propertyDisplayKind2
                };
                break;
              }
              break;
            case InlineTableSyntax _:
              propertyMetadata = new TomlPropertyMetadata()
              {
                DisplayKind = TomlPropertyDisplayKind.InlineTable
              };
              break;
            case IntegerValueSyntax integerValueSyntax:
              SyntaxToken token3 = integerValueSyntax.Token;
              TokenKind tokenKind3 = token3 != null ? token3.TokenKind : TokenKind.Invalid;
              TomlPropertyDisplayKind propertyDisplayKind3 = TomlPropertyDisplayKind.Default;
              switch (tokenKind3)
              {
                case TokenKind.IntegerHexa:
                  propertyDisplayKind3 = TomlPropertyDisplayKind.IntegerHexadecimal;
                  break;
                case TokenKind.IntegerOctal:
                  propertyDisplayKind3 = TomlPropertyDisplayKind.IntegerOctal;
                  break;
                case TokenKind.IntegerBinary:
                  propertyDisplayKind3 = TomlPropertyDisplayKind.IntegerBinary;
                  break;
              }
              if (propertyDisplayKind3 != TomlPropertyDisplayKind.Default)
              {
                propertyMetadata = new TomlPropertyMetadata()
                {
                  DisplayKind = propertyDisplayKind3
                };
                break;
              }
              break;
          }
          List<TomlSyntaxTriviaMetadata> syntaxTriviaMetadataList1 = this.ConvertTrivias(keyValueSyntax.LeadingTrivia);
          if (syntaxTriviaMetadataList1 != null && syntaxTriviaMetadataList1.Count > 0)
          {
            if (propertyMetadata == null)
              propertyMetadata = new TomlPropertyMetadata();
            propertyMetadata.LeadingTrivia = syntaxTriviaMetadataList1;
          }
          List<TomlSyntaxTriviaMetadata> syntaxTriviaMetadataList2 = this.ConvertTrivias(keyValueSyntax.Value?.GetChild(keyValueSyntax.Value.ChildrenCount - 1)?.TrailingTrivia);
          if (syntaxTriviaMetadataList2 != null && syntaxTriviaMetadataList2.Count > 0)
          {
            if (propertyMetadata == null)
              propertyMetadata = new TomlPropertyMetadata();
            propertyMetadata.TrailingTrivia = syntaxTriviaMetadataList2;
          }
          List<TomlSyntaxTriviaMetadata> syntaxTriviaMetadataList3 = this.ConvertTrivias(keyValueSyntax.EndOfLineToken?.TrailingTrivia);
          if (syntaxTriviaMetadataList3 != null && syntaxTriviaMetadataList3.Count > 0)
          {
            if (propertyMetadata == null)
              propertyMetadata = new TomlPropertyMetadata();
            propertyMetadata.TrailingTriviaAfterEndOfLine = syntaxTriviaMetadataList3;
            break;
          }
          break;
        case TableSyntaxBase tableSyntaxBase:
          List<TomlSyntaxTriviaMetadata> syntaxTriviaMetadataList4 = this.ConvertTrivias(tableSyntaxBase.LeadingTrivia);
          if (syntaxTriviaMetadataList4 != null && syntaxTriviaMetadataList4.Count > 0)
          {
            if (propertyMetadata == null)
              propertyMetadata = new TomlPropertyMetadata();
            propertyMetadata.LeadingTrivia = syntaxTriviaMetadataList4;
          }
          List<TomlSyntaxTriviaMetadata> syntaxTriviaMetadataList5 = this.ConvertTrivias(tableSyntaxBase.CloseBracket?.TrailingTrivia);
          if (syntaxTriviaMetadataList5 != null && syntaxTriviaMetadataList5.Count > 0)
          {
            if (propertyMetadata == null)
              propertyMetadata = new TomlPropertyMetadata();
            propertyMetadata.TrailingTrivia = syntaxTriviaMetadataList5;
          }
          List<TomlSyntaxTriviaMetadata> syntaxTriviaMetadataList6 = this.ConvertTrivias(tableSyntaxBase.EndOfLineToken?.TrailingTrivia);
          if (syntaxTriviaMetadataList6 != null && syntaxTriviaMetadataList6.Count > 0)
          {
            if (propertyMetadata == null)
              propertyMetadata = new TomlPropertyMetadata();
            propertyMetadata.TrailingTriviaAfterEndOfLine = syntaxTriviaMetadataList6;
            break;
          }
          break;
      }
      return propertyMetadata;
    }

    private List<TomlSyntaxTriviaMetadata>? ConvertTrivias(List<SyntaxTrivia>? trivias)
    {
      if (trivias == null || trivias.Count == 0)
        return (List<TomlSyntaxTriviaMetadata>) null;
      List<TomlSyntaxTriviaMetadata> syntaxTriviaMetadataList = new List<TomlSyntaxTriviaMetadata>();
      foreach (SyntaxTrivia trivia in trivias)
        syntaxTriviaMetadataList.Add((TomlSyntaxTriviaMetadata) trivia);
      return syntaxTriviaMetadataList;
    }

    private struct ObjectPath
    {
      public readonly object Value;
      public readonly DynamicAccessor DynamicAccessor;

      public ObjectPath(object value, DynamicAccessor dynamicAccessor)
      {
        this.Value = value;
        this.DynamicAccessor = dynamicAccessor;
      }
    }

    private class ReferenceEqualityComparer : IEqualityComparer<object>
    {
      public static readonly SyntaxToModelTransform.ReferenceEqualityComparer Instance = new SyntaxToModelTransform.ReferenceEqualityComparer();

      private ReferenceEqualityComparer()
      {
      }

      public bool Equals(object? x, object? y) => x == y;

      public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
    }
  }
}
