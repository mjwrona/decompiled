// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.DocumentFieldContainer
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public abstract class DocumentFieldContainer : IFieldContainer
  {
    private Dictionary<string, object> m_fieldCache = new Dictionary<string, object>();

    internal abstract object GetFieldValueInternal(string fieldName);

    public bool DualExecution { get; set; }

    public object GetFieldValue(string fieldName)
    {
      if (fieldName == null)
        throw new ArgumentNullException(nameof (fieldName));
      object fieldValue;
      if (!this.m_fieldCache.TryGetValue(fieldName, out fieldValue))
      {
        fieldValue = this.GetFieldValueInternal(fieldName);
        if (fieldValue is string[] array)
        {
          Array.Sort<string>(array, (IComparer<string>) VssStringComparer.StringFieldConditionEquality);
          fieldValue = (object) array;
        }
        this.m_fieldCache[fieldName] = fieldValue;
      }
      return fieldValue;
    }

    public abstract string GetDocumentString();

    public abstract void AddOrUpdateNode(string name, string value);

    public abstract IFieldContainer GetDynamicFieldContainer(DynamicFieldContainerType type);

    public override bool Equals(object obj)
    {
      bool flag = base.Equals(obj);
      DocumentFieldContainer documentFieldContainer = obj as DocumentFieldContainer;
      if (!flag && documentFieldContainer != null && string.Equals(this.GetDocumentString(), documentFieldContainer.GetDocumentString()))
        flag = true;
      return flag;
    }

    public override int GetHashCode() => this.GetDocumentString().SafeGetHashCode<string>();
  }
}
