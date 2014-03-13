using System;
using System.Reflection;
using System.Reflection.Emit;
using ExpressionDemo.Common;
using ExpressionDemo.ConsoleApp.Filters;

namespace ExpressionDemo.ConsoleApp
{
    internal class AssemblyGeneratorApplication : IApplication
    {
        private const string ASSEMBLY_FILENAME = "TestAssembly.dll";
        private readonly CompiledMethodFilter _filter;

        public AssemblyGeneratorApplication(CompiledMethodFilter filter)
        {
            if (filter == null) throw new ArgumentNullException("filter");

            _filter = filter;
        }

        public void Run()
        {
            var expr = _filter.GetFilterExpression();

            AssemblyBuilder assembly =
                AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("TestAssembly"),
                    AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder mod = assembly.DefineDynamicModule("TestModule", ASSEMBLY_FILENAME, true);
            TypeBuilder type = mod.DefineType("TestType", TypeAttributes.Public, typeof(object), new []{typeof(IFilterImplementation)});
            
            MethodBuilder staticMethod = type.DefineMethod("FilterStatic", MethodAttributes.Public | MethodAttributes.Static,
                typeof(bool), new[] { typeof(IGeoDataLocation) });

            expr.CompileToMethod(staticMethod);

            GenerateInterfaceWrapper(type, staticMethod);
            SaveAssembly(type, assembly);
        }

        private static void SaveAssembly(TypeBuilder type, AssemblyBuilder assembly)
        {
            var dt = type.CreateType();
            assembly.Save(ASSEMBLY_FILENAME);
        }

        private static void GenerateInterfaceWrapper(TypeBuilder type, MethodBuilder staticMethod)
        {
            MethodBuilder method = type.DefineMethod("Filter", MethodAttributes.Public | MethodAttributes.Virtual,
                typeof (bool), new[] {typeof (IGeoDataLocation)});

            ILGenerator generator = method.GetILGenerator();

            generator.Emit(OpCodes.Nop);
            generator.Emit(OpCodes.Ldarg_1);
            generator.EmitCall(OpCodes.Call, staticMethod, new[] {typeof (IGeoDataLocation)});
            generator.Emit(OpCodes.Ret);
        }
    }
}