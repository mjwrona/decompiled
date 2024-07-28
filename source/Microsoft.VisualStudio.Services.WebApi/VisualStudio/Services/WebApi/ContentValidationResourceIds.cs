// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.ContentValidationResourceIds
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public static class ContentValidationResourceIds
  {
    public const string Area = "contentValidation";
    public const string ContentViolationArea = "contentViolation";
    public const string CvsCallbackResource = "cvsCallback";
    public static readonly Guid CvsCallbackLocationId = new Guid("68FB0862-7B4F-45AD-9BDD-9B689E233E4F");
    public const string TakedownResource = "takedown";
    public static readonly Guid TakedownLocationId = new Guid("7AE2F97A-5CCA-4A0A-AC90-81DD689F26F5");
    public const string AvertCallbackResource = "avertCallback";
    public static readonly Guid AvertCallbackLocationId = new Guid("E55E0DCC-84AE-43AC-BA89-F1C6D685A97A");
    public const string ViolationReportsResource = "reports";
    public static readonly Guid ViolationReportsLocationId = new Guid("3505911E-EAD6-431A-8656-B61C5D3B07A3");
  }
}
