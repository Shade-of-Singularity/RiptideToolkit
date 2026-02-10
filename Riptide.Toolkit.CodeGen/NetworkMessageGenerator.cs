#define ADD_STORAGE

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Riptide.Toolkit.Messages;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

#if ADD_STORAGE
using Riptide.Toolkit.Storage;
#endif

namespace Riptide.Toolkit.CodeGen
{
    // Note: We use regular generator to support it inside Unity environment.
    [Generator]
    public sealed class NetworkMessageGenerator : ISourceGenerator
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Delegates
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public delegate string WriteHandler(string container, string member, string type);
        public delegate string ReadHandler(string container, string member, string type);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        static readonly Dictionary<string, WriteHandler> Writers = new(StringComparer.OrdinalIgnoreCase)
        {
            { "byte", (c, v, _) => $"{c}.{nameof(Message.AddByte)}({v});" },
            { "sbyte", (c, v, _) => $"{c}.{nameof(Message.AddSByte)}({v});" },
            { "short", (c, v, _) => $"{c}.{nameof(Message.AddShort)}({v});" },
            { "ushort", (c, v, _) => $"{c}.{nameof(Message.AddUShort)}({v});" },

            { "int", (c, v, _) => $"{c}.{nameof(Message.AddInt)}({v});" },
            { "uint", (c, v, _) => $"{c}.{nameof(Message.AddUInt)}({v});" },
            { "long", (c, v, _) => $"{c}.{nameof(Message.AddLong)}({v});" },
            { "ulong", (c, v, _) => $"{c}.{nameof(Message.AddULong)}({v});" },

            { "string", (c, v, _) => $"{c}.{nameof(Message.AddString)}({v});" },
            { "float", (c, v, _) => $"{c}.{nameof(Message.AddFloat)}({v});" },
            { "double", (c, v, _) => $"{c}.{nameof(Message.AddDouble)}({v});" },

            // Arrays:
            // TODO: Add reading using ref instead.
            { "byte[]", (c, v, _) => $"{c}.{nameof(Message.AddBytes)}({v});" },
            { "sbyte[]", (c, v, _) => $"{c}.{nameof(Message.AddSBytes)}({v});" },
            { "short[]", (c, v, _) => $"{c}.{nameof(Message.AddShorts)}({v});" },
            { "ushort[]", (c, v, _) => $"{c}.{nameof(Message.AddUShorts)}({v});" },

            { "int[]", (c, v, _) => $"{c}.{nameof(Message.AddInts)}({v});" },
            { "uint[]", (c, v, _) => $"{c}.{nameof(Message.AddUInts)}({v});" },
            { "long[]", (c, v, _) => $"{c}.{nameof(Message.AddLongs)}({v});" },
            { "ulong[]", (c, v, _) => $"{c}.{nameof(Message.AddULongs)}({v});" },

            { "string[]", (c, v, _) => $"{c}.{nameof(Message.AddStrings)}({v});" },
            { "float[]", (c, v, _) => $"{c}.{nameof(Message.AddFloats)}({v});" },
            { "double[]", (c, v, _) => $"{c}.{nameof(Message.AddDoubles)}({v});" },
        };

        static readonly Dictionary<string, ReadHandler> Readers = new()
        {
            { "byte", (c, v, _) => $"{v} = {c}.{nameof(Message.GetByte)}();" },
            { "sbyte", (c, v, _) => $"{v} = {c}.{nameof(Message.GetSByte)}();" },
            { "short", (c, v, _) => $"{v} = {c}.{nameof(Message.GetShort)}();" },
            { "ushort", (c, v, _) => $"{v} = {c}.{nameof(Message.GetUShort)}();" },

            { "int", (c, v, _) => $"{v} = {c}.{nameof(Message.GetInt)}();" },
            { "uint", (c, v, _) => $"{v} = {c}.{nameof(Message.GetUInt)}();" },
            { "long", (c, v, _) => $"{v} = {c}.{nameof(Message.GetLong)}();" },
            { "ulong", (c, v, _) => $"{v} = {c}.{nameof(Message.GetULong)}();" },

            { "string", (c, v, _) => $"{v} = {c}.{nameof(Message.GetString)}();" },
            { "float", (c, v, _) => $"{v} = {c}.{nameof(Message.GetFloat)}();" },
            { "double", (c, v, _) => $"{v} = {c}.{nameof(Message.GetDouble)}();" },

            // Arrays:
            // TODO: Add reading using ref instead.
            { "byte[]", (c, v, _) => $"{v} = {c}.{nameof(Message.GetBytes)}();" },
            { "sbyte[]", (c, v, _) => $"{v} = {c}.{nameof(Message.GetSBytes)}();" },
            { "short[]", (c, v, _) => $"{v} = {c}.{nameof(Message.GetShorts)}();" },
            { "ushort[]", (c, v, _) => $"{v} = {c}.{nameof(Message.GetUShorts)}();" },

            { "int[]", (c, v, _) => $"{v} = {c}.{nameof(Message.GetInts)}();" },
            { "uint[]", (c, v, _) => $"{v} = {c}.{nameof(Message.GetUInts)}();" },
            { "long[]", (c, v, _) => $"{v} = {c}.{nameof(Message.GetLongs)}();" },
            { "ulong[]", (c, v, _) => $"{v} = {c}.{nameof(Message.GetULongs)}();" },

            { "string[]", (c, v, _) => $"{v} = {c}.{nameof(Message.GetStrings)}();" },
            { "float[]", (c, v, _) => $"{v} = {c}.{nameof(Message.GetFloats)}();" },
            { "double[]", (c, v, _) => $"{v} = {c}.{nameof(Message.GetDoubles)}();" },
        };




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public void Initialize(GeneratorInitializationContext context) { }
        public void Execute(GeneratorExecutionContext context)
        {
            foreach (var tree in context.Compilation.SyntaxTrees)
            {
                Execute(context, tree.GetRoot());
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        static void Execute(GeneratorExecutionContext context, SyntaxNode root)
        {
            StringBuilder builder = null;
            if (root.ChildNodes().FirstOrDefault(n => n is NamespaceDeclarationSyntax) is not NamespaceDeclarationSyntax @namespace) return;

            const string TAB = "    ";
            const string DoubleTAB = TAB + TAB;
            const string TripleTAB = TAB + TAB + TAB;

            // TODO: Define container adaptively?
            const string Container = "message";
            foreach (var node in @namespace.ChildNodes())
            {
                // TODO: Add analysis for inside of the classes.
                if (node is not ClassDeclarationSyntax declaration) continue;
                if (!node.ChildTokens().Any(t => t.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword))) continue;

                BaseListSyntax baseList = declaration.BaseList;
                if (baseList is null) continue;

                if (!baseList.Types.SelectMany(t => t.DescendantNodes())
                    .Any(node => node is GenericNameSyntax t &&
                    string.Equals(t.Identifier.Text, nameof(Messages.NetworkMessage), System.StringComparison.Ordinal)))
                {
                    continue;
                }

                // We found our declaration!

                //bool hasDispose = false; // TODO: Implement.
                bool hasRead = declaration.Members.Any(m => m is MethodDeclarationSyntax method
                    && method.Identifier.Text.Equals(nameof(NetworkMessage.Read), System.StringComparison.Ordinal));
                bool hasWrite = declaration.Members.Any(m => m is MethodDeclarationSyntax method
                    && method.Identifier.Text.Equals(nameof(NetworkMessage.Write), System.StringComparison.Ordinal));

                if (hasRead && hasWrite) // && (hasDispose && hasReferenceFields)
                {
                    return;
                }

                // Defines header if needed.
                if (builder is null)
                {
                    // TODO: Replace with SyntaxTree construction instead.
                    builder = new(4096);
                    foreach (var @using in root.ChildNodes())
                    {
                        if (@using is UsingDirectiveSyntax) builder.AppendLine(@using.ToString());
                    }

                    if (builder.Length > 0) builder.AppendLine();
                    builder.Append("namespace ");
                    builder.AppendLine(@namespace.Name.ToString());
                    builder.AppendLine("{");
                }
                else
                {
                    // Another fancy-pants new line for better stylization.
                    builder.AppendLine();
                }

                string attribute;
#if ADD_STORAGE
                attribute = typeof(S1Attribute).FullName;
                builder.Append(TAB);
                builder.Append('[');
                builder.Append(attribute.Substring(0, attribute.Length - "Attribute".Length));
                builder.AppendLine("]");
#endif

                // Adds auto-gen attribute as well, just in case.
                attribute = typeof(GeneratedCodeAttribute).FullName;
                builder.Append(TAB);
                builder.Append('[');
                builder.Append(attribute.Substring(0, attribute.Length - "Attribute".Length));
                builder.Append("(\"");
                builder.Append("Riptide.Toolkit.CodeGen");
                builder.Append("\", \"");
                builder.Append(Assembly.GetExecutingAssembly().GetName().Version);
                builder.AppendLine("\")]");

                // Defines class.
                builder.Append(TAB);
                builder.Append(declaration.Modifiers.ToString());
                builder.Append(" class ");
                builder.AppendLine(declaration.Identifier.Text);
                builder.AppendLine(TAB + "{");

                // Looks for fields and properties.
                List<(string type, string variable, SyntaxNode node)> members = new();
                foreach (var member in declaration.Members)
                {
                    switch (member)
                    {
                        default: break;
                        case FieldDeclarationSyntax field:
                            members.Add((field.Declaration.Type.ToString(), field.Declaration.Variables.ToString(), field));
                            break;

                        case PropertyDeclarationSyntax property:
                            members.Add((property.Type.ToString(), property.Identifier.Text, property));
                            break;
                    }
                }

                if (!hasRead)
                {
                    // TODO: Add ways to define custom read-write methods for Auto-gen.
                    builder.AppendLine(DoubleTAB + "/// <inheritdoc/>");
                    builder.AppendLine(DoubleTAB + "public override Message Read(Message " + Container + ")");
                    builder.AppendLine(DoubleTAB + "{");
                    foreach (var (type, member, variable) in members)
                    {
                        if (!Readers.TryGetValue(type, out ReadHandler handler))
                        {
                            // TODO: Maybe ask custom types to implement IMessageCommon interface, or something like that?
                            //  And scan for static extension methods as well?
                            LogWarning(context, variable, $"Reading of a '{type}' is not supported by Auto-gen.");
                            continue;
                        }

                        builder.Append(TripleTAB);
                        builder.AppendLine(handler(Container, member, type));
                    }

                    // If no readers were written - at least generates a return method.
                    builder.AppendLine(TripleTAB + "return " + Container + ";");
                    builder.AppendLine(DoubleTAB + "}");
                }

                if (!hasWrite)
                {
                    // Fancy-pants new line for better stylization.
                    if (!hasRead) builder.AppendLine();

                    // TODO: Add ways to define custom read-write methods for Auto-gen.
                    builder.AppendLine(DoubleTAB + "/// <inheritdoc/>");
                    builder.AppendLine(DoubleTAB + "public override Message Write(Message " + Container + ")");
                    builder.AppendLine(DoubleTAB + "{");
                    foreach (var (type, member, variable) in members)
                    {
                        if (!Writers.TryGetValue(type, out WriteHandler handler))
                        {
                            // TODO: Maybe ask custom types to implement IMessageCommon interface, or something like that?
                            //  And scan for static extension methods as well?
                            LogWarning(context, variable, $"Writing of a '{type}' is not supported by Auto-gen.");
                            continue;
                        }

                        builder.Append(TripleTAB);
                        builder.AppendLine(handler(Container, member, type));
                    }

                    // If no readers were written - at least generates a return method.
                    builder.AppendLine(TripleTAB + "return " + Container + ";");
                    builder.AppendLine(DoubleTAB + "}");
                }

                builder.AppendLine(TAB + "}");
            }

            if (builder is not null)
            {
                builder.AppendLine("}");
                //Log(context, root, builder.ToString());
                context.AddSource(string.Concat(Path.GetFileNameWithoutExtension(root.SyntaxTree.FilePath), ".g.cs"), builder.ToString());
            }
        }

        static void LogWarning(GeneratorExecutionContext context, SyntaxNode node, object obj)
        {
            var location = node.GetLocation();
            var diagnostic = Diagnostic.Create(
                new DiagnosticDescriptor("RT0001", "RTA Title", obj.ToString(), "RTA Category", DiagnosticSeverity.Warning, true), location
            );

            context.ReportDiagnostic(diagnostic);
        }
    }
}
