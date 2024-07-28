// Decompiled with JetBrains decompiler
// Type: Tomlyn.Model.ModelToTomlTransform
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tomlyn.Helpers;
using Tomlyn.Model.Accessors;
using Tomlyn.Syntax;
using Tomlyn.Text;


#nullable enable
namespace Tomlyn.Model
{
  internal class ModelToTomlTransform
  {
    private readonly object _rootObject;
    private readonly DynamicModelWriteContext _context;
    private readonly TextWriter _writer;
    private readonly List<ModelToTomlTransform.ObjectPath> _paths;
    private readonly List<ModelToTomlTransform.ObjectPath> _currentPaths;
    private readonly Stack<List<KeyValuePair<string, object?>>> _tempPropertiesStack;
    private ITomlMetadataProvider? _metadataProvider;

    public ModelToTomlTransform(object rootObject, DynamicModelWriteContext context)
    {
      this._rootObject = rootObject;
      this._context = context;
      this._writer = context.Writer;
      this._paths = new List<ModelToTomlTransform.ObjectPath>();
      this._tempPropertiesStack = new Stack<List<KeyValuePair<string, object>>>();
      this._currentPaths = new List<ModelToTomlTransform.ObjectPath>();
    }

    public void Run()
    {
      DynamicAccessor accessor1 = this._context.GetAccessor(this._rootObject.GetType());
      if (accessor1 is ObjectDynamicAccessor accessor2)
        this.VisitObject(accessor2, this._rootObject, false);
      else
        this._context.Diagnostics.Error(new SourceSpan(), string.Format("The root object must a class with properties or a dictionary. Cannot be of kind {0}.", (object) accessor1));
    }

    private void PushName(string name, bool isTableArray) => this._paths.Add(new ModelToTomlTransform.ObjectPath(name, isTableArray));

    private void WriteHeaderTable()
    {
      string name = this._paths[this._paths.Count - 1].Name;
      this.WriteLeadingTrivia(name);
      this._writer.Write("[");
      this.WriteDottedKeys();
      this._writer.Write("]");
      this.WriteTrailingTrivia(name);
      this._writer.WriteLine();
      this.WriteTrailingTriviaAfterEndOfLine(name);
      this._currentPaths.Clear();
      this._currentPaths.AddRange((IEnumerable<ModelToTomlTransform.ObjectPath>) this._paths);
    }

    private void WriteHeaderTableArray()
    {
      string name = this._paths[this._paths.Count - 1].Name;
      this.WriteLeadingTrivia(name);
      this._writer.Write("[[");
      this.WriteDottedKeys();
      this._writer.Write("]]");
      this.WriteTrailingTrivia(name);
      this._writer.WriteLine();
      this.WriteTrailingTriviaAfterEndOfLine(name);
      this._currentPaths.Clear();
      this._currentPaths.AddRange((IEnumerable<ModelToTomlTransform.ObjectPath>) this._paths);
    }

    private void WriteDottedKeys()
    {
      bool flag = true;
      foreach (ModelToTomlTransform.ObjectPath path in this._paths)
      {
        if (!flag)
          this._writer.Write(".");
        this.WriteKey(path.Name);
        flag = false;
      }
    }

    private void WriteKey(string name) => this._writer.Write(this.EscapeKey(name));

    private string EscapeKey(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        return "\"" + name.EscapeForToml() + "\"";
      foreach (char ch in name)
      {
        if ((ch < 'a' || ch > 'z') && (ch < 'A' || ch > 'Z') && (ch < '0' || ch > '9') && ch != '_' && ch != '-' || ch == '.')
          return "\"" + name.EscapeForToml() + "\"";
      }
      return name;
    }

    private void EnsureScope()
    {
      if (this.IsCurrentScopeValid())
        return;
      if (this._paths.Count == 0)
        this._currentPaths.Clear();
      else if (this._paths[this._paths.Count - 1].IsTableArray)
        this.WriteHeaderTableArray();
      else
        this.WriteHeaderTable();
    }

    private bool IsCurrentScopeValid()
    {
      if (this._paths.Count != this._currentPaths.Count)
        return false;
      for (int index = 0; index < this._paths.Count; ++index)
      {
        if (!this._paths[index].Equals(this._currentPaths[index]))
          return false;
      }
      return true;
    }

    private void PopName() => this._paths.RemoveAt(this._paths.Count - 1);

    private bool VisitObject(ObjectDynamicAccessor accessor, object currentObject, bool inline)
    {
      bool flag1 = false;
      bool flag2 = true;
      ITomlMetadataProvider metadataProvider = this._metadataProvider;
      this._metadataProvider = currentObject as ITomlMetadataProvider;
      List<KeyValuePair<string, object>> source = this._tempPropertiesStack.Count > 0 ? this._tempPropertiesStack.Pop() : new List<KeyValuePair<string, object>>();
      try
      {
        Func<object, object> convertToToml = this._context.ConvertToToml;
        if (convertToToml != null)
        {
          foreach (KeyValuePair<string, object> property in accessor.GetProperties(currentObject))
          {
            object obj1 = property.Value;
            if (obj1 != null)
            {
              object obj2 = convertToToml(obj1);
              if (obj2 != null)
                obj1 = obj2;
              source.Add(new KeyValuePair<string, object>(property.Key, obj1));
            }
          }
        }
        else
          source.AddRange(accessor.GetProperties(currentObject));
        source = source.OrderBy<KeyValuePair<string, object>, KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, KeyValuePair<string, object>>) (_ => _), (IComparer<KeyValuePair<string, object>>) Comparer<KeyValuePair<string, object>>.Create((Comparison<KeyValuePair<string, object>>) ((left, right) =>
        {
          object obj3 = left.Value;
          object obj4 = right.Value;
          if (obj3 == null)
            return obj4 != null ? -1 : 0;
          if (obj4 == null)
            return 1;
          DynamicAccessor accessor1 = this._context.GetAccessor(obj3.GetType());
          DynamicAccessor accessor2 = this._context.GetAccessor(obj4.GetType());
          return accessor1.Kind == ReflectionObjectKind.Primitive ? (accessor2.Kind != ReflectionObjectKind.Primitive ? -1 : 0) : (accessor2.Kind == ReflectionObjectKind.Primitive ? 1 : 0);
        }))).ToList<KeyValuePair<string, object>>();
        bool flag3 = inline;
        object obj = (object) null;
        bool flag4 = false;
        if (!inline)
        {
          foreach (KeyValuePair<string, object> keyValuePair in source)
          {
            obj = keyValuePair.Value;
            if (flag4 = this.IsRequiringInline(keyValuePair.Value))
              flag3 = true;
          }
        }
        foreach (KeyValuePair<string, object> keyValuePair in source)
        {
          if (keyValuePair.Value != null)
          {
            string key = keyValuePair.Key;
            if (inline && !flag2)
              this._writer.Write(", ");
            bool inline1 = ((obj == null ? 0 : (obj == keyValuePair.Value ? 1 : 0)) == 0 | flag4) & flag3;
            if (!inline & inline1)
              this.EnsureScope();
            if (!inline)
              this.WriteLeadingTrivia(key);
            DynamicAccessor dynamicAccessor = this.WriteKeyValue(key, keyValuePair.Value, inline1);
            if (!inline && dynamicAccessor is PrimitiveDynamicAccessor | inline1)
            {
              this.WriteTrailingTrivia(key);
              this._writer.WriteLine();
              this.WriteTrailingTriviaAfterEndOfLine(key);
            }
            flag1 = true;
            flag2 = false;
          }
        }
      }
      finally
      {
        this._metadataProvider = metadataProvider;
        source.Clear();
        this._tempPropertiesStack.Push(source);
      }
      return flag1;
    }

    private void VisitList(ListDynamicAccessor accessor, object currentObject, bool inline)
    {
      bool flag = true;
      foreach (object element in accessor.GetElements(currentObject))
      {
        if (element != null)
        {
          DynamicAccessor accessor1 = this._context.GetAccessor(element.GetType());
          if (inline)
          {
            if (!flag)
              this._writer.Write(", ");
            this.WriteValueInline(accessor1, element);
            flag = false;
          }
          else
          {
            ITomlMetadataProvider metadataProvider = this._metadataProvider;
            try
            {
              this._metadataProvider = element as ITomlMetadataProvider;
              this.WriteHeaderTableArray();
            }
            finally
            {
              this._metadataProvider = metadataProvider;
            }
            this.VisitObject((ObjectDynamicAccessor) accessor1, element, false);
          }
        }
      }
    }

    private DynamicAccessor WriteKeyValue(string name, object value, bool inline)
    {
      DynamicAccessor accessor1 = this._context.GetAccessor(value.GetType());
      switch (accessor1)
      {
        case ListDynamicAccessor accessor2:
          bool flag = inline;
          if (!inline)
            inline = this.IsRequiringInline(accessor2, value, 1);
          if (inline)
          {
            if (!flag)
              this.EnsureScope();
            this.WriteKey(name);
            this._writer.Write(" = [");
            this.VisitList(accessor2, value, true);
            this._writer.Write("]");
            break;
          }
          this.PushName(name, true);
          this.VisitList(accessor2, value, false);
          this.PopName();
          break;
        case ObjectDynamicAccessor accessor3:
          if (inline)
          {
            this.WriteKey(name);
            this._writer.Write(" = {");
            this.VisitObject(accessor3, value, true);
            this._writer.Write("}");
            break;
          }
          this.PushName(name, false);
          if (!this.VisitObject(accessor3, value, false))
          {
            ITomlMetadataProvider metadataProvider = this._metadataProvider;
            this._metadataProvider = value as ITomlMetadataProvider;
            try
            {
              this.EnsureScope();
            }
            finally
            {
              this._metadataProvider = metadataProvider;
            }
          }
          this.PopName();
          break;
        case PrimitiveDynamicAccessor _:
          this.EnsureScope();
          this.WriteKey(name);
          this._writer.Write(" = ");
          this.WritePrimitive(value, this.GetDisplayKind(name));
          break;
        default:
          throw new ArgumentOutOfRangeException("accessor");
      }
      return accessor1;
    }

    private TomlPropertyDisplayKind GetDisplayKind(string name)
    {
      TomlPropertyDisplayKind displayKind = TomlPropertyDisplayKind.Default;
      TomlPropertyMetadata propertyMetadata;
      if (this._metadataProvider != null && this._metadataProvider.PropertiesMetadata != null && this._metadataProvider.PropertiesMetadata.TryGetProperty(name, out propertyMetadata))
        displayKind = propertyMetadata.DisplayKind;
      return displayKind;
    }

    private bool IsRequiringInline(object? value) => value != null && this._context.GetAccessor(value.GetType()) is ListDynamicAccessor accessor && this.IsRequiringInline(accessor, value, 1);

    private bool IsRequiringInline(
      ListDynamicAccessor accessor,
      object value,
      int parentConsecutiveList)
    {
      foreach (object element in accessor.GetElements(value))
      {
        if (element != null)
        {
          switch (this._context.GetAccessor(element.GetType()))
          {
            case PrimitiveDynamicAccessor _:
              return true;
            case ListDynamicAccessor accessor1:
              return this.IsRequiringInline(accessor1, element, parentConsecutiveList + 1);
            case ObjectDynamicAccessor accessor2:
              return parentConsecutiveList > 1 || this.IsRequiringInline(accessor2, element);
            default:
              continue;
          }
        }
      }
      return parentConsecutiveList > 1;
    }

    private bool IsRequiringInline(ObjectDynamicAccessor accessor, object value)
    {
      foreach (KeyValuePair<string, object> property in accessor.GetProperties(value))
      {
        object obj = property.Value;
        if (obj != null)
        {
          DynamicAccessor accessor1 = this._context.GetAccessor(obj.GetType());
          if (accessor1 is ListDynamicAccessor accessor2)
            return this.IsRequiringInline(accessor2, obj, 1);
          if (accessor1 is ObjectDynamicAccessor accessor3)
            return this.IsRequiringInline(accessor3, obj);
        }
      }
      return false;
    }

    private void WriteLeadingTrivia(string name)
    {
      TomlPropertyMetadata propertyMetadata;
      if (this._metadataProvider?.PropertiesMetadata == null || !this._metadataProvider.PropertiesMetadata.TryGetProperty(name, out propertyMetadata) || propertyMetadata.LeadingTrivia == null)
        return;
      foreach (TomlSyntaxTriviaMetadata syntaxTriviaMetadata in propertyMetadata.LeadingTrivia)
      {
        if (syntaxTriviaMetadata.Text != null)
          this._writer.Write(syntaxTriviaMetadata.Text);
      }
    }

    private void WriteTrailingTrivia(string name)
    {
      TomlPropertyMetadata propertyMetadata;
      if (this._metadataProvider?.PropertiesMetadata == null || !this._metadataProvider.PropertiesMetadata.TryGetProperty(name, out propertyMetadata) || propertyMetadata.TrailingTrivia == null)
        return;
      foreach (TomlSyntaxTriviaMetadata syntaxTriviaMetadata in propertyMetadata.TrailingTrivia)
      {
        if (syntaxTriviaMetadata.Text != null)
          this._writer.Write(syntaxTriviaMetadata.Text);
      }
    }

    private void WriteTrailingTriviaAfterEndOfLine(string name)
    {
      TomlPropertyMetadata propertyMetadata;
      if (this._metadataProvider?.PropertiesMetadata == null || !this._metadataProvider.PropertiesMetadata.TryGetProperty(name, out propertyMetadata) || propertyMetadata.TrailingTriviaAfterEndOfLine == null)
        return;
      foreach (TomlSyntaxTriviaMetadata syntaxTriviaMetadata in propertyMetadata.TrailingTriviaAfterEndOfLine)
      {
        if (syntaxTriviaMetadata.Text != null)
          this._writer.Write(syntaxTriviaMetadata.Text);
      }
    }

    private void WriteValueInline(DynamicAccessor accessor, object? value)
    {
      if (value == null)
        return;
      switch (accessor)
      {
        case ListDynamicAccessor accessor1:
          this._writer.Write("[");
          this.VisitList(accessor1, value, true);
          this._writer.Write("]");
          break;
        case ObjectDynamicAccessor accessor2:
          this._writer.Write("{");
          this.VisitObject(accessor2, value, true);
          this._writer.Write("}");
          break;
        case PrimitiveDynamicAccessor _:
          this.WritePrimitive(value, TomlPropertyDisplayKind.Default);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (accessor));
      }
    }

    private void WritePrimitive(object primitive, TomlPropertyDisplayKind displayKind)
    {
      switch (primitive)
      {
        case bool b:
          this._writer.Write(TomlFormatHelper.ToString(b));
          break;
        case string s:
          this._writer.Write(TomlFormatHelper.ToString(s, displayKind));
          break;
        case int i32:
          this._writer.Write(TomlFormatHelper.ToString(i32, displayKind));
          break;
        case long i64:
          this._writer.Write(TomlFormatHelper.ToString(i64, displayKind));
          break;
        case uint u32:
          this._writer.Write(TomlFormatHelper.ToString(u32, displayKind));
          break;
        case ulong u64:
          this._writer.Write(TomlFormatHelper.ToString(u64, displayKind));
          break;
        case sbyte i8:
          this._writer.Write(TomlFormatHelper.ToString(i8, displayKind));
          break;
        case byte u8:
          this._writer.Write(TomlFormatHelper.ToString(u8, displayKind));
          break;
        case short i16:
          this._writer.Write(TomlFormatHelper.ToString(i16, displayKind));
          break;
        case ushort u16:
          this._writer.Write(TomlFormatHelper.ToString(u16, displayKind));
          break;
        case float num1:
          this._writer.Write(TomlFormatHelper.ToString(num1));
          break;
        case double num2:
          this._writer.Write(TomlFormatHelper.ToString(num2));
          break;
        case TomlDateTime tomlDateTime:
          this._writer.Write(TomlFormatHelper.ToString(tomlDateTime));
          break;
        case DateTime dateTime:
          this._writer.Write(TomlFormatHelper.ToString(dateTime, displayKind));
          break;
        case DateTimeOffset dateTimeOffset:
          this._writer.Write(TomlFormatHelper.ToString(dateTimeOffset, displayKind));
          break;
        default:
          throw new InvalidOperationException("Invalid primitive " + primitive.GetType().FullName);
      }
    }

    private record struct ObjectPath(string Name, bool IsTableArray);
  }
}
