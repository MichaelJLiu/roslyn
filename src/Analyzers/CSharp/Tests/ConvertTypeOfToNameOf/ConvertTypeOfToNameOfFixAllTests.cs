﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.ConvertNameOf
{
    public partial class ConvertTypeOfToNameOfTests
    {
        [Fact]
        [Trait(Traits.Feature, Traits.Features.ConvertNameOf)]
        [Trait(Traits.Feature, Traits.Features.CodeActionsFixAllOccurrences)]
        public async Task FixAllDocumentBasic()
        {
            var input = @"class Test
{
    static void Main()
    {
        var typeName1 = {|FixAllInDocument:typeof(Test).Name|};
        var typeName2 = typeof(Test).Name;
        var typeName3 = typeof(Test).Name;
    }
}
";

            var expected = @"class Test
{
    static void Main()
    {
        var typeName1 = nameof(Test);
        var typeName2 = nameof(Test);
        var typeName3 = nameof(Test);
    }
}
";

            await TestInRegularAndScriptAsync(input, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.ConvertNameOf)]
        [Trait(Traits.Feature, Traits.Features.CodeActionsFixAllOccurrences)]
        public async Task FixAllDocumentVaried()
        {
            var input = @"class Test
{
    static void Main()
    {
        var typeName1 = {|FixAllInDocument:typeof(Test).Name|};
        var typeName2 = typeof(int).Name;
        var typeName3 = typeof(System.String).Name;
    }
}
";

            var expected = @"class Test
{
    static void Main()
    {
        var typeName1 = nameof(Test);
        var typeName2 = nameof(System.Int32);
        var typeName3 = nameof(System.String);
    }
}
";

            await TestInRegularAndScriptAsync(input, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.ConvertNameOf)]
        [Trait(Traits.Feature, Traits.Features.CodeActionsFixAllOccurrences)]
        public async Task FixAllDocumentVariedWithUsing()
        {
            var input = @"using System;

class Test
{
    static void Main()
    {
        var typeName1 = typeof(Test).Name;
        var typeName2 = typeof(int).Name;
        var typeName3 = typeof(String).Name;
        var typeName4 = {|FixAllInDocument:typeof(System.Double).Name|};
    }
}
";

            var expected = @"using System;

class Test
{
    static void Main()
    {
        var typeName1 = nameof(Test);
        var typeName2 = nameof(Int32);
        var typeName3 = nameof(String);
        var typeName4 = nameof(Double);
    }
}
";

            await TestInRegularAndScriptAsync(input, expected);
        }
    }
}
