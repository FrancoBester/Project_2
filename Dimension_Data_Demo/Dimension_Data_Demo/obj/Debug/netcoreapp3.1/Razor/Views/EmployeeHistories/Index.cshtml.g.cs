#pragma checksum "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\EmployeeHistories\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "cfb68f99498ad193afe505160cde16956b0e2c98"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_EmployeeHistories_Index), @"mvc.1.0.view", @"/Views/EmployeeHistories/Index.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\_ViewImports.cshtml"
using Dimension_Data_Demo;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\_ViewImports.cshtml"
using Dimension_Data_Demo.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"cfb68f99498ad193afe505160cde16956b0e2c98", @"/Views/EmployeeHistories/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e095a876f1a0191c1ad7e707605b82ee5b2623ab", @"/Views/_ViewImports.cshtml")]
    public class Views_EmployeeHistories_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IEnumerable<Dimension_Data_Demo.Models.EmployeeHistory>>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Edit", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Details", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\EmployeeHistories\Index.cshtml"
  
    ViewData["Title"] = "Index";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h1>Index</h1>\r\n\r\n<p>\r\n");
#nullable restore
#line 10 "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\EmployeeHistories\Index.cshtml"
     if (ViewBag.Message != null)
    {

#line default
#line hidden
#nullable disable
            WriteLiteral("        <script type=\"text/javascript\">\r\n                    window.onload = function () {\r\n                        alert(\"");
#nullable restore
#line 14 "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\EmployeeHistories\Index.cshtml"
                          Write(ViewBag.Message);

#line default
#line hidden
#nullable disable
            WriteLiteral("\");\r\n                    };\r\n        </script>\r\n");
#nullable restore
#line 17 "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\EmployeeHistories\Index.cshtml"
    }

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\r\n<table class=\"table\">\r\n    <thead>\r\n        <tr>\r\n            <th>\r\n                ");
#nullable restore
#line 23 "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\EmployeeHistories\Index.cshtml"
           Write(Html.DisplayNameFor(model => model.NumCompaniesWorked));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </th>\r\n            <th>\r\n                ");
#nullable restore
#line 26 "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\EmployeeHistories\Index.cshtml"
           Write(Html.DisplayNameFor(model => model.TotalWorkingYears));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </th>\r\n            <th>\r\n                ");
#nullable restore
#line 29 "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\EmployeeHistories\Index.cshtml"
           Write(Html.DisplayNameFor(model => model.YearsAtCompany));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </th>\r\n            <th>\r\n                ");
#nullable restore
#line 32 "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\EmployeeHistories\Index.cshtml"
           Write(Html.DisplayNameFor(model => model.YearsInCurrentRole));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </th>\r\n            <th>\r\n                ");
#nullable restore
#line 35 "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\EmployeeHistories\Index.cshtml"
           Write(Html.DisplayNameFor(model => model.YearsSinceLastPromotion));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </th>\r\n            <th>\r\n                ");
#nullable restore
#line 38 "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\EmployeeHistories\Index.cshtml"
           Write(Html.DisplayNameFor(model => model.YearsWithCurrManager));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </th>\r\n            <th>\r\n                ");
#nullable restore
#line 41 "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\EmployeeHistories\Index.cshtml"
           Write(Html.DisplayNameFor(model => model.TrainingTimesLastYear));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </th>\r\n            <th></th>\r\n        </tr>\r\n    </thead>\r\n    <tbody>\r\n");
#nullable restore
#line 47 "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\EmployeeHistories\Index.cshtml"
 foreach (var item in Model) {

#line default
#line hidden
#nullable disable
            WriteLiteral("        <tr>\r\n            <td>\r\n                ");
#nullable restore
#line 50 "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\EmployeeHistories\Index.cshtml"
           Write(Html.DisplayFor(modelItem => item.NumCompaniesWorked));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </td>\r\n            <td>\r\n                ");
#nullable restore
#line 53 "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\EmployeeHistories\Index.cshtml"
           Write(Html.DisplayFor(modelItem => item.TotalWorkingYears));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </td>\r\n            <td>\r\n                ");
#nullable restore
#line 56 "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\EmployeeHistories\Index.cshtml"
           Write(Html.DisplayFor(modelItem => item.YearsAtCompany));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </td>\r\n            <td>\r\n                ");
#nullable restore
#line 59 "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\EmployeeHistories\Index.cshtml"
           Write(Html.DisplayFor(modelItem => item.YearsInCurrentRole));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </td>\r\n            <td>\r\n                ");
#nullable restore
#line 62 "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\EmployeeHistories\Index.cshtml"
           Write(Html.DisplayFor(modelItem => item.YearsSinceLastPromotion));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </td>\r\n            <td>\r\n                ");
#nullable restore
#line 65 "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\EmployeeHistories\Index.cshtml"
           Write(Html.DisplayFor(modelItem => item.YearsWithCurrManager));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </td>\r\n            <td>\r\n                ");
#nullable restore
#line 68 "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\EmployeeHistories\Index.cshtml"
           Write(Html.DisplayFor(modelItem => item.TrainingTimesLastYear));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </td>\r\n            <td>\r\n                ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "cfb68f99498ad193afe505160cde16956b0e2c9810113", async() => {
                WriteLiteral("Edit");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-id", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#nullable restore
#line 71 "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\EmployeeHistories\Index.cshtml"
                                       WriteLiteral(item.HistoryId);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-id", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(" |\r\n                ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "cfb68f99498ad193afe505160cde16956b0e2c9812296", async() => {
                WriteLiteral("Details");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-id", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#nullable restore
#line 72 "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\EmployeeHistories\Index.cshtml"
                                          WriteLiteral(item.HistoryId);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-id", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(" |\r\n            </td>\r\n        </tr>\r\n");
#nullable restore
#line 75 "E:\Project_2\Dimension_Data_Demo\Dimension_Data_Demo\Views\EmployeeHistories\Index.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral("    </tbody>\r\n</table>\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IEnumerable<Dimension_Data_Demo.Models.EmployeeHistory>> Html { get; private set; }
    }
}
#pragma warning restore 1591
