// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using Xunit;

namespace Microsoft.CodeAnalysis.CSharp.UnitTests.CodeGen
{
    public class BreakContinueLabelTests : EmitMetadataTestBase
    {
        [Fact]
        public void ForLoopBreakContinueLabel()
        {
            string source =
@"class C
{
    static void Main()
    {
        int breakIterations = TestBreakLabel();
        int continueIterations = TestContinueLabel();
        System.Console.Write($""{breakIterations} - {continueIterations}"");
    }

    static int TestBreakLabel()
    {
        int iterations = 0;
    outer:
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                iterations++;
                if (iterations == 17)
                    break outer;
            }
        }

        return iterations;
    }
    static int TestContinueLabel()
    {
        int iterations = 0;
    outer:
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (j > i)
                    continue outer;

                iterations++;
            }
        }

        return iterations;
    }
}";
            var compilation = CompileAndVerify(source, expectedOutput: "17 - 15");

            compilation.VerifyIL("C.TestBreakLabel", @"
{
    // Code size       39 (0x27)
    .maxstack  2
    .locals init (int V_0, //iterations
                int V_1, //i
                int V_2) //j
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_0020
    IL_0006:  ldc.i4.0
    IL_0007:  stloc.2
    IL_0008:  br.s       IL_0017
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.1
    IL_000c:  add
    IL_000d:  stloc.0
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.s   17
    IL_0011:  beq.s      IL_0025
    IL_0013:  ldloc.2
    IL_0014:  ldc.i4.1
    IL_0015:  add
    IL_0016:  stloc.2
    IL_0017:  ldloc.2
    IL_0018:  ldc.i4.s   10
    IL_001a:  blt.s      IL_000a
    IL_001c:  ldloc.1
    IL_001d:  ldc.i4.1
    IL_001e:  add
    IL_001f:  stloc.1
    IL_0020:  ldloc.1
    IL_0021:  ldc.i4.s   10
    IL_0023:  blt.s      IL_0006
    IL_0025:  ldloc.0
    IL_0026:  ret
}
");
            compilation.VerifyIL("C.TestContinueLabel", @"
{
    // Code size       37 (0x25)
    .maxstack  2
    .locals init (int V_0, //iterations
                int V_1, //i
                int V_2) //j
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_001f
    IL_0006:  ldc.i4.0
    IL_0007:  stloc.2
    IL_0008:  br.s       IL_0016
    IL_000a:  ldloc.2
    IL_000b:  ldloc.1
    IL_000c:  bgt.s      IL_001b
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.1
    IL_0010:  add
    IL_0011:  stloc.0
    IL_0012:  ldloc.2
    IL_0013:  ldc.i4.1
    IL_0014:  add
    IL_0015:  stloc.2
    IL_0016:  ldloc.2
    IL_0017:  ldc.i4.s   10
    IL_0019:  blt.s      IL_000a
    IL_001b:  ldloc.1
    IL_001c:  ldc.i4.1
    IL_001d:  add
    IL_001e:  stloc.1
    IL_001f:  ldloc.1
    IL_0020:  ldc.i4.5
    IL_0021:  blt.s      IL_0006
    IL_0023:  ldloc.0
    IL_0024:  ret
}
");
        }

        [Fact]
        public void InvalidBreakLabel()
        {
            string source = GetBreakContinueTestCode("break");

            var compilation = CreateCompilation(source);
            compilation.VerifyDiagnostics(
                // (42,5): warning CS0164: This label has not been referenced
                //     label:
                Diagnostic(ErrorCode.WRN_UnreferencedLabel, "label").WithLocation(42, 5),
                // (46,17): error CS9210: Expected a label for a loop.
                //                 break label;
                Diagnostic(ErrorCode.ERR_NotLoopLabel, "break label;").WithLocation(46, 17));
        }

        [Fact]
        public void InvalidContinueLabel()
        {
            string source = GetBreakContinueTestCode("continue");

            var compilation = CreateCompilation(source);
            compilation.VerifyDiagnostics(
                // (42,5): warning CS0164: This label has not been referenced
                //     label:
                Diagnostic(ErrorCode.WRN_UnreferencedLabel, "label").WithLocation(42, 5),
                // (45,13): error CS8070: Control cannot fall out of switch from final case label ('case "wrong":')
                //             case "wrong":
                Diagnostic(ErrorCode.ERR_SwitchFallOut, @"case ""wrong"":").WithArguments(@"case ""wrong"":").WithLocation(45, 13),
                // (46,17): error CS0139: No enclosing loop out of which to break or continue
                //                 continue label;
                Diagnostic(ErrorCode.ERR_NoBreakOrCont, "continue label;").WithLocation(46, 17));
        }

        private static string GetBreakContinueTestCode(string controlOperation)
        {
            return $@"
using System;

class C
{{
    static void For()
    {{
    loop:
        for (int i = 0; i < 10; i++)
        {{
            if (i == 5)
                {controlOperation} loop;
        }}
    }}
    static void Foreach()
    {{
    loop:
        foreach (var s in new int[0])
        {{
            {controlOperation} loop;
        }}
    }}
    static void While()
    {{
    loop:
        while (true)
        {{
            {controlOperation} loop;
        }}
    }}
    static void DoWhile()
    {{
    loop:
        do
        {{
            {controlOperation} loop;
        }}
        while (true);
    }}
    static void Switch()
    {{
    label:
        switch (Console.ReadLine())
        {{
            case ""wrong"":
                {controlOperation} label;
        }}
    }}
}}";
        }

        [Fact]
        public void NestedBreakLabel()
        {
            string source =
@"class C
{
    static void Main()
    {
        int iterations = TestNested();
        System.Console.Write(iterations);
    }

    static int TestNested()
    {
        int iterations = 0;
    outer:
        for (int i = 0; i < 10; i++)
        {
            switch (i % 3)
            {
                case 0:
                    for (int j = 0; j < i; j++)
                    {
                        iterations++;
                        if (iterations == 7)
                            break outer;
                    }
                    break;
                case 1:
                    for (int j = 0; j < i; j++)
                    {
                        if (iterations < 7)
                            break;
                        iterations++;
                    }
                    break;
                case 2:
                    foreach (int j in new int[] { 0, 1, 2 })
                    {
                        if (j < i)
                            break;
                        iterations++;
                    }
                    break;
            }
        }

        return iterations;
    }
}";
            var compilation = CompileAndVerify(source, expectedOutput: "7");

            compilation.VerifyIL("C.TestNested", @"
{
  // Code size      136 (0x88)
  .maxstack  4
  .locals init (int V_0, //iterations
                int V_1, //i
                int V_2,
                int V_3, //j
                int V_4, //j
                int[] V_5,
                int V_6)
  IL_0000:  ldc.i4.0
  IL_0001:  stloc.0
  IL_0002:  ldc.i4.0
  IL_0003:  stloc.1
  IL_0004:  br.s       IL_0081
  IL_0006:  ldloc.1
  IL_0007:  ldc.i4.3
  IL_0008:  rem
  IL_0009:  stloc.2
  IL_000a:  ldloc.2
  IL_000b:  switch    (
        IL_001e,
        IL_0034,
        IL_004e)
  IL_001c:  br.s       IL_007d
  IL_001e:  ldc.i4.0
  IL_001f:  stloc.3
  IL_0020:  br.s       IL_002e
  IL_0022:  ldloc.0
  IL_0023:  ldc.i4.1
  IL_0024:  add
  IL_0025:  stloc.0
  IL_0026:  ldloc.0
  IL_0027:  ldc.i4.7
  IL_0028:  beq.s      IL_0086
  IL_002a:  ldloc.3
  IL_002b:  ldc.i4.1
  IL_002c:  add
  IL_002d:  stloc.3
  IL_002e:  ldloc.3
  IL_002f:  ldloc.1
  IL_0030:  blt.s      IL_0022
  IL_0032:  br.s       IL_007d
  IL_0034:  ldc.i4.0
  IL_0035:  stloc.s    V_4
  IL_0037:  br.s       IL_0047
  IL_0039:  ldloc.0
  IL_003a:  ldc.i4.7
  IL_003b:  blt.s      IL_007d
  IL_003d:  ldloc.0
  IL_003e:  ldc.i4.1
  IL_003f:  add
  IL_0040:  stloc.0
  IL_0041:  ldloc.s    V_4
  IL_0043:  ldc.i4.1
  IL_0044:  add
  IL_0045:  stloc.s    V_4
  IL_0047:  ldloc.s    V_4
  IL_0049:  ldloc.1
  IL_004a:  blt.s      IL_0039
  IL_004c:  br.s       IL_007d
  IL_004e:  ldc.i4.3
  IL_004f:  newarr     ""int""
  IL_0054:  dup
  IL_0055:  ldc.i4.1
  IL_0056:  ldc.i4.1
  IL_0057:  stelem.i4
  IL_0058:  dup
  IL_0059:  ldc.i4.2
  IL_005a:  ldc.i4.2
  IL_005b:  stelem.i4
  IL_005c:  stloc.s    V_5
  IL_005e:  ldc.i4.0
  IL_005f:  stloc.s    V_6
  IL_0061:  br.s       IL_0075
  IL_0063:  ldloc.s    V_5
  IL_0065:  ldloc.s    V_6
  IL_0067:  ldelem.i4
  IL_0068:  ldloc.1
  IL_0069:  blt.s      IL_007d
  IL_006b:  ldloc.0
  IL_006c:  ldc.i4.1
  IL_006d:  add
  IL_006e:  stloc.0
  IL_006f:  ldloc.s    V_6
  IL_0071:  ldc.i4.1
  IL_0072:  add
  IL_0073:  stloc.s    V_6
  IL_0075:  ldloc.s    V_6
  IL_0077:  ldloc.s    V_5
  IL_0079:  ldlen
  IL_007a:  conv.i4
  IL_007b:  blt.s      IL_0063
  IL_007d:  ldloc.1
  IL_007e:  ldc.i4.1
  IL_007f:  add
  IL_0080:  stloc.1
  IL_0081:  ldloc.1
  IL_0082:  ldc.i4.s   10
  IL_0084:  blt.s      IL_0006
  IL_0086:  ldloc.0
  IL_0087:  ret
}
");
        }
    }
}
