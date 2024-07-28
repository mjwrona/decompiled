// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmDirectValueAnnotationsManager
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmDirectValueAnnotationsManager : IEdmDirectValueAnnotationsManager
  {
    private VersioningDictionary<IEdmElement, object> annotationsDictionary;
    private object annotationsDictionaryLock = new object();
    private VersioningList<IEdmElement> unsortedElements = VersioningList<IEdmElement>.Create();
    private object unsortedElementsLock = new object();

    public EdmDirectValueAnnotationsManager() => this.annotationsDictionary = VersioningDictionary<IEdmElement, object>.Create(new Func<IEdmElement, IEdmElement, int>(this.CompareElements));

    public IEnumerable<IEdmDirectValueAnnotation> GetDirectValueAnnotations(IEdmElement element)
    {
      VersioningDictionary<IEdmElement, object> annotationsDictionary = this.annotationsDictionary;
      IEnumerable<IEdmDirectValueAnnotation> attachedAnnotations = this.GetAttachedAnnotations(element);
      object transientAnnotations = EdmDirectValueAnnotationsManager.GetTransientAnnotations(element, annotationsDictionary);
      if (attachedAnnotations != null)
      {
        foreach (IEdmDirectValueAnnotation directValueAnnotation in attachedAnnotations)
        {
          if (!EdmDirectValueAnnotationsManager.IsDead(directValueAnnotation.NamespaceUri, directValueAnnotation.Name, transientAnnotations))
            yield return directValueAnnotation;
        }
      }
      foreach (IEdmDirectValueAnnotation transientAnnotation in EdmDirectValueAnnotationsManager.TransientAnnotations(transientAnnotations))
        yield return transientAnnotation;
    }

    public void SetAnnotationValue(
      IEdmElement element,
      string namespaceName,
      string localName,
      object value)
    {
      lock (this.annotationsDictionaryLock)
      {
        VersioningDictionary<IEdmElement, object> annotationsDictionary = this.annotationsDictionary;
        this.SetAnnotationValue(element, namespaceName, localName, value, ref annotationsDictionary);
        this.annotationsDictionary = annotationsDictionary;
      }
    }

    public void SetAnnotationValues(
      IEnumerable<IEdmDirectValueAnnotationBinding> annotations)
    {
      lock (this.annotationsDictionaryLock)
      {
        VersioningDictionary<IEdmElement, object> annotationsDictionary = this.annotationsDictionary;
        foreach (IEdmDirectValueAnnotationBinding annotation in annotations)
          this.SetAnnotationValue(annotation.Element, annotation.NamespaceUri, annotation.Name, annotation.Value, ref annotationsDictionary);
        this.annotationsDictionary = annotationsDictionary;
      }
    }

    public object GetAnnotationValue(IEdmElement element, string namespaceName, string localName)
    {
      VersioningDictionary<IEdmElement, object> annotationsDictionary = this.annotationsDictionary;
      return this.GetAnnotationValue(element, namespaceName, localName, annotationsDictionary);
    }

    public object[] GetAnnotationValues(
      IEnumerable<IEdmDirectValueAnnotationBinding> annotations)
    {
      VersioningDictionary<IEdmElement, object> annotationsDictionary = this.annotationsDictionary;
      object[] annotationValues = new object[annotations.Count<IEdmDirectValueAnnotationBinding>()];
      int num = 0;
      foreach (IEdmDirectValueAnnotationBinding annotation in annotations)
        annotationValues[num++] = this.GetAnnotationValue(annotation.Element, annotation.NamespaceUri, annotation.Name, annotationsDictionary);
      return annotationValues;
    }

    protected virtual IEnumerable<IEdmDirectValueAnnotation> GetAttachedAnnotations(
      IEdmElement element)
    {
      return (IEnumerable<IEdmDirectValueAnnotation>) null;
    }

    private static void SetAnnotation(
      IEnumerable<IEdmDirectValueAnnotation> immutableAnnotations,
      ref object transientAnnotations,
      string namespaceName,
      string localName,
      object value)
    {
      bool flag = false;
      if (immutableAnnotations != null && immutableAnnotations.Any<IEdmDirectValueAnnotation>((Func<IEdmDirectValueAnnotation, bool>) (existingAnnotation => existingAnnotation.NamespaceUri == namespaceName && existingAnnotation.Name == localName)))
        flag = true;
      if (value == null && !flag)
      {
        EdmDirectValueAnnotationsManager.RemoveTransientAnnotation(ref transientAnnotations, namespaceName, localName);
      }
      else
      {
        IEdmDirectValueAnnotation directValueAnnotation1 = value != null ? (IEdmDirectValueAnnotation) new EdmDirectValueAnnotation(namespaceName, localName, value) : (IEdmDirectValueAnnotation) new EdmDirectValueAnnotation(namespaceName, localName);
        if (transientAnnotations == null)
          transientAnnotations = (object) directValueAnnotation1;
        else if (transientAnnotations is IEdmDirectValueAnnotation directValueAnnotation3)
        {
          if (directValueAnnotation3.NamespaceUri == namespaceName && directValueAnnotation3.Name == localName)
            transientAnnotations = (object) directValueAnnotation1;
          else
            transientAnnotations = (object) VersioningList<IEdmDirectValueAnnotation>.Create().Add(directValueAnnotation3).Add(directValueAnnotation1);
        }
        else
        {
          VersioningList<IEdmDirectValueAnnotation> versioningList = (VersioningList<IEdmDirectValueAnnotation>) transientAnnotations;
          for (int index = 0; index < versioningList.Count; ++index)
          {
            IEdmDirectValueAnnotation directValueAnnotation2 = versioningList[index];
            if (directValueAnnotation2.NamespaceUri == namespaceName && directValueAnnotation2.Name == localName)
            {
              versioningList = versioningList.RemoveAt(index);
              break;
            }
          }
          transientAnnotations = (object) versioningList.Add(directValueAnnotation1);
        }
      }
    }

    private static IEdmDirectValueAnnotation FindTransientAnnotation(
      object transientAnnotations,
      string namespaceName,
      string localName)
    {
      if (transientAnnotations != null)
      {
        if (!(transientAnnotations is IEdmDirectValueAnnotation transientAnnotation))
          return ((IEnumerable<IEdmDirectValueAnnotation>) transientAnnotations).FirstOrDefault<IEdmDirectValueAnnotation>((Func<IEdmDirectValueAnnotation, bool>) (existingAnnotation => existingAnnotation.NamespaceUri == namespaceName && existingAnnotation.Name == localName));
        if (transientAnnotation.NamespaceUri == namespaceName && transientAnnotation.Name == localName)
          return transientAnnotation;
      }
      return (IEdmDirectValueAnnotation) null;
    }

    private static void RemoveTransientAnnotation(
      ref object transientAnnotations,
      string namespaceName,
      string localName)
    {
      if (transientAnnotations == null)
        return;
      if (transientAnnotations is IEdmDirectValueAnnotation directValueAnnotation1)
      {
        if (!(directValueAnnotation1.NamespaceUri == namespaceName) || !(directValueAnnotation1.Name == localName))
          return;
        transientAnnotations = (object) null;
      }
      else
      {
        VersioningList<IEdmDirectValueAnnotation> versioningList = (VersioningList<IEdmDirectValueAnnotation>) transientAnnotations;
        for (int index = 0; index < versioningList.Count; ++index)
        {
          IEdmDirectValueAnnotation directValueAnnotation = versioningList[index];
          if (directValueAnnotation.NamespaceUri == namespaceName && directValueAnnotation.Name == localName)
          {
            VersioningList<IEdmDirectValueAnnotation> source = versioningList.RemoveAt(index);
            if (source.Count == 1)
            {
              transientAnnotations = (object) source.Single<IEdmDirectValueAnnotation>();
              break;
            }
            transientAnnotations = (object) source;
            break;
          }
        }
      }
    }

    private static IEnumerable<IEdmDirectValueAnnotation> TransientAnnotations(
      object transientAnnotations)
    {
      if (transientAnnotations != null)
      {
        if (transientAnnotations is IEdmDirectValueAnnotation directValueAnnotation)
        {
          if (directValueAnnotation.Value != null)
            yield return directValueAnnotation;
        }
        else
        {
          foreach (IEdmDirectValueAnnotation transientAnnotation in (VersioningList<IEdmDirectValueAnnotation>) transientAnnotations)
          {
            if (transientAnnotation.Value != null)
              yield return transientAnnotation;
          }
        }
      }
    }

    private static bool IsDead(string namespaceName, string localName, object transientAnnotations) => EdmDirectValueAnnotationsManager.FindTransientAnnotation(transientAnnotations, namespaceName, localName) != null;

    private static object GetTransientAnnotations(
      IEdmElement element,
      VersioningDictionary<IEdmElement, object> annotationsDictionary)
    {
      object transientAnnotations;
      annotationsDictionary.TryGetValue(element, out transientAnnotations);
      return transientAnnotations;
    }

    private void SetAnnotationValue(
      IEdmElement element,
      string namespaceName,
      string localName,
      object value,
      ref VersioningDictionary<IEdmElement, object> annotationsDictionary)
    {
      object transientAnnotations = EdmDirectValueAnnotationsManager.GetTransientAnnotations(element, annotationsDictionary);
      object obj = transientAnnotations;
      EdmDirectValueAnnotationsManager.SetAnnotation(this.GetAttachedAnnotations(element), ref transientAnnotations, namespaceName, localName, value);
      if (transientAnnotations == obj)
        return;
      annotationsDictionary = annotationsDictionary.Set(element, transientAnnotations);
    }

    private object GetAnnotationValue(
      IEdmElement element,
      string namespaceName,
      string localName,
      VersioningDictionary<IEdmElement, object> annotationsDictionary)
    {
      IEdmDirectValueAnnotation transientAnnotation = EdmDirectValueAnnotationsManager.FindTransientAnnotation(EdmDirectValueAnnotationsManager.GetTransientAnnotations(element, annotationsDictionary), namespaceName, localName);
      if (transientAnnotation != null)
        return transientAnnotation.Value;
      IEnumerable<IEdmDirectValueAnnotation> attachedAnnotations = this.GetAttachedAnnotations(element);
      if (attachedAnnotations != null)
      {
        foreach (IEdmDirectValueAnnotation directValueAnnotation in attachedAnnotations)
        {
          if (directValueAnnotation.NamespaceUri == namespaceName && directValueAnnotation.Name == localName)
            return directValueAnnotation.Value;
        }
      }
      return (object) null;
    }

    private int CompareElements(IEdmElement left, IEdmElement right)
    {
      if (left == right)
        return 0;
      int hashCode1 = left.GetHashCode();
      int hashCode2 = right.GetHashCode();
      if (hashCode1 < hashCode2)
        return -1;
      if (hashCode1 > hashCode2)
        return 1;
      IEdmNamedElement edmNamedElement1 = left as IEdmNamedElement;
      IEdmNamedElement edmNamedElement2 = right as IEdmNamedElement;
      if (edmNamedElement1 == null)
      {
        if (edmNamedElement2 != null)
          return -1;
      }
      else
      {
        if (edmNamedElement2 == null)
          return 1;
        int num = string.Compare(edmNamedElement1.Name, edmNamedElement2.Name, StringComparison.Ordinal);
        if (num != 0)
          return num;
      }
      while (true)
      {
        foreach (IEdmElement unsortedElement in this.unsortedElements)
        {
          if (unsortedElement == left)
            return 1;
          if (unsortedElement == right)
            return -1;
        }
        lock (this.unsortedElementsLock)
          this.unsortedElements = this.unsortedElements.Add(left);
      }
    }
  }
}
