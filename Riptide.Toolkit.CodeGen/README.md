# How to use
Follow Unity's instruction guide:
https://docs.unity3d.com/2022.3/Documentation/Manual/roslyn-analyzers.html
*Note: if you have any issues - first try doing Project Clean-up and restarting Visual Studio*

Tip: You can install "**NuGet for Unity**" to download target `Microsoft.CodeAnalyzer.CSharp` package.
Target version is specified in Unity's guide.
https://github.com/GlitchEnzo/NuGetForUnity

*TODO: Rework this section once we make analyzers analyze to new variable types and once they implement NetworkMessage.Dispose().*
*Note: We can also still consider using structs instead of classes as data containers.*