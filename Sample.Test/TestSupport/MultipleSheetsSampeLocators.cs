using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace MRoessler.BlazorBottomSheet.Sample.Test.TestSupport;

public static class MultipleSheetsSampeLocators
{
    public static ILocator Sheet1HeaderText(ILocator parent) => parent.GetByTestId("sheet1-header-text");
    public static ILocator Sheet2HeaderText(ILocator parent) => parent.GetByTestId("sheet2-header-text");
    public static ILocator Sheet1BodyText(ILocator parent) => parent.GetByTestId("sheet1-body-text");
    public static ILocator Sheet2BodyText(ILocator parent) => parent.GetByTestId("sheet2-body-text");
}
