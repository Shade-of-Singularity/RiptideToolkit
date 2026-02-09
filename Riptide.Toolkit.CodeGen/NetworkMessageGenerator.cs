using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Riptide.Toolkit.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

// (Dark): Many thank to Latios for examples!
// https://github.com/Dreaming381/Latios-Framework-Documentation/blob/main/Tech%20Adventures/Part%201%20-%20Source%20Generators%201.md
// As well as Shawn Wildermuth! https://youtu.be/Yf8t7GqA6zA?si=ph-77ao1NDBGgqKL
namespace Riptide.Toolkit.CodeGen
{
    /// <summary>
    /// Generates <see cref="NetworkMessage.Read(Message)"/> and <see cref="NetworkMessage.Write(Message)"/> methods for messages.
    /// </summary>
    /// TODO: Create the same analyzer for writing/reading ALL data in classes as well.
    ///  Though to be fair, you might want to use structs instead anyway.
    [Generator]
    public sealed class NetworkMessageGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var provider = context.SyntaxProvider.CreateSyntaxProvider(NetworkMessagePredicate, NetworkMessageTransform);
            //var compilation = context.CompilationProvider.Combine(provider.Collect());

            context.RegisterSourceOutput(provider, Execute);
        }

        static bool NetworkMessagePredicate(SyntaxNode node, CancellationToken token)
        {
            return node is ClassDeclarationSyntax;
        }

        static ClassDeclarationSyntax NetworkMessageTransform(GeneratorSyntaxContext context, CancellationToken token)
        {
            return (ClassDeclarationSyntax)context.Node;
        }

        // TODO: Refactor.
        static void Execute(SourceProductionContext context, ClassDeclarationSyntax declaration)
        {
            // Looks for namespace.
            NamespaceDeclarationSyntax region = null;
            foreach (var child in declaration.SyntaxTree.GetRoot().ChildNodes())
            {
                if (child is NamespaceDeclarationSyntax t)
                {
                    region = t;
                    break;
                }
            }

            if (region is null)
            {
                return;
            }


            // Looks for the classes.
            // TODO: Heavily optimize.
            BaseListSyntax baseList = declaration.BaseList;
            if (baseList is null)
            {
                return;
            }

            if (!baseList.Types.SelectMany(t => t.DescendantNodes())
                .Any(node => node is GenericNameSyntax t && string.Equals(t.Identifier.Text, nameof(NetworkMessage), System.StringComparison.Ordinal)))
            {
                return;
            }

            //bool hasDispose = false; // TODO: Implement.
            bool hasRead = declaration.Members.Any(m => m is MethodDeclarationSyntax method
                && method.Identifier.Text.Equals(nameof(NetworkMessage.Read), System.StringComparison.Ordinal));
            bool hasWrite = declaration.Members.Any(m => m is MethodDeclarationSyntax method
                && method.Identifier.Text.Equals(nameof(NetworkMessage.Write), System.StringComparison.Ordinal));
            //Log($"Read? ({hasRead})   Write? ({hasWrite})");

            if (hasRead && hasWrite) // && (hasDispose && hasReferenceFields)
            {
                //Log("No need for AutoGen");
                return;
            }

            const string TAB = "    ";
            const string DoubleTAB = TAB + TAB;
            const string TrippleTAB = TAB + TAB + TAB;
            StringBuilder builder = new();
            bool anyUsings = false;
            foreach (var item in declaration.SyntaxTree.GetRoot().ChildNodes())
            {
                if (item is UsingDirectiveSyntax directive)
                {
                    // hopefully it works.
                    builder.AppendLine(item.ToString());
                    anyUsings = true;
                }
            }

            if (anyUsings) builder.AppendLine();

            builder.Append("namespace ");
            builder.AppendLine(region.Name.ToString());
            builder.AppendLine("{");

            // Code analysis here.
            builder.Append(TAB);
            builder.Append(declaration.Modifiers.ToString());
            builder.Append(" class ");
            builder.AppendLine(declaration.Identifier.Text);
            builder.AppendLine(TAB + "{");

            List<(string type, string variable)> members = new();
            foreach (var member in declaration.Members)
            {
                switch (member)
                {
                    default: break;
                    case FieldDeclarationSyntax field:
                        members.Add((field.Declaration.Type.ToString(), field.Declaration.Variables.ToString()));
                        break;

                    case PropertyDeclarationSyntax property:
                        members.Add((property.Type.ToString(), property.Identifier.Text));
                        break;
                }
            }

            const string container = "message";
            if (!hasRead)
            {
                // TODO: Add ways to define custom read-write methods for Auto-gen.
                builder.AppendLine(DoubleTAB + "public override Message Read(Message " + container + ")");
                builder.AppendLine(DoubleTAB + "{");
                foreach (var (type, variable) in members)
                {
                    builder.Append(TrippleTAB);
                    builder.Append(container);
                    builder.AppendLine(type switch
                    {
                        "ushort" => $".{nameof(Message.AddUShort)}({variable});",
                        "uint" => $".{nameof(Message.AddUInt)}({variable});",
                        "float" => $".{nameof(Message.AddFloat)}({variable});",
                        _ => throw new NotImplementedException("Custom variable types are not supported yet."),
                    });
                }
                builder.AppendLine(TrippleTAB + "return " + container + ";");
                builder.AppendLine(DoubleTAB + "}");
            }

            if (!hasWrite)
            {
                if (!hasRead) builder.AppendLine();

                // TODO: Add ways to define custom read-write methods for Auto-gen.
                builder.AppendLine(DoubleTAB + "public override Message Write(Message " + container + ")");
                builder.AppendLine(DoubleTAB + "{");
                foreach (var (type, variable) in members)
                {
                    builder.Append(TrippleTAB);
                    builder.Append(variable);
                    builder.Append(" = ");
                    builder.AppendLine(type switch
                    {
                        "ushort" => container + "." + nameof(Message.GetUShort) + "();",
                        "uint" => container + "." + nameof(Message.GetUInt) + "();",
                        "float" => container + "." + nameof(Message.GetFloat) + "();",
                        _ => throw new NotImplementedException("Custom variable types are not supported yet."),
                    });
                }
                builder.AppendLine(TrippleTAB + "return " + container + ";");
                builder.AppendLine(DoubleTAB + "}");
            }

            // Finishing here.
            builder?.AppendLine(TAB + "}\n}");
            //Log(builder);
            string hintName = Path.GetFileNameWithoutExtension(declaration.SyntaxTree.FilePath) + "." + declaration.Identifier.Text + ".g.cs";
            //Log(hintName);
            context.AddSource(hintName, builder.ToString());

            // Simplifications:
            //void LogError() => Log("Unexpected syntax.");
            //void LogType(object obj) => Log($"{obj.GetType().Name}: {obj}");
            //void Log(object obj) => NetworkMessageGenerator.Log(context, declaration, obj);
        }

        //static void Log(SourceProductionContext context, ClassDeclarationSyntax cls, object obj)
        //{
        //    var location = Location.Create( // Point to the class name
        //        cls.SyntaxTree,
        //        cls.Identifier.Span);

        //    var diagnostic = Diagnostic.Create(
        //        new DiagnosticDescriptor("RT0001", "RTA Title", obj.ToString(), "RTA Category", DiagnosticSeverity.Warning, true), location
        //    );

        //    context.ReportDiagnostic(diagnostic);
        //}

        //static void Walk(SourceProductionContext context, ClassDeclarationSyntax cls, SyntaxNode node, ref uint counter, string extra = "")
        //{
        //    Log(context, cls, $"[{counter:000}] {node}{extra}");
        //    counter++;
        //    foreach (var child in node.ChildNodes())
        //    {
        //        Walk(context, cls, child, ref counter, string.Concat(" <- ", child, extra));
        //    }
        //}
    }
}
