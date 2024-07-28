// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.ReferenceComparer
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  internal class ReferenceComparer : IComparer<JSVariableField>
  {
    public static readonly IComparer<JSVariableField> Instance = (IComparer<JSVariableField>) new ReferenceComparer();

    private ReferenceComparer()
    {
    }

    public int Compare(JSVariableField left, JSVariableField right)
    {
      if (left == right || left == null && right == null)
        return 0;
      if (left == null)
        return 1;
      if (right == null)
        return -1;
      if ((left.FieldType == FieldType.Argument || left.FieldType == FieldType.CatchError) && (right.FieldType == FieldType.Argument || right.FieldType == FieldType.CatchError))
      {
        int num = left.Position - right.Position;
        if (num == 0)
        {
          num = ReferenceComparer.CompareContext(left.OriginalContext, right.OriginalContext);
          if (num == 0)
            num = string.Compare(left.Name, right.Name, StringComparison.Ordinal);
        }
        return num;
      }
      if (left.FieldType == FieldType.Argument || left.FieldType == FieldType.CatchError)
        return -1;
      if (right.FieldType == FieldType.Argument || right.FieldType == FieldType.CatchError)
        return 1;
      int num1 = right.RefCount - left.RefCount;
      if (num1 == 0)
      {
        num1 = ReferenceComparer.CompareContext(left.OriginalContext, right.OriginalContext);
        if (num1 == 0)
          num1 = string.Compare(left.Name, right.Name, StringComparison.Ordinal);
      }
      return num1;
    }

    private static int CompareContext(Context left, Context right)
    {
      int num = 0;
      if (left != null && right != null)
      {
        num = left.StartLineNumber - right.StartLineNumber;
        if (num == 0)
          num = left.StartColumn - right.StartColumn;
      }
      else if (left != null)
        num = -1;
      else if (right != null)
        num = 1;
      return num;
    }
  }
}
