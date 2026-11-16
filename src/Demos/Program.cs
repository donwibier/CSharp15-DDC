using Demos.Demos;

var demos = new (string Name, Action Run)[]
{
    ("01 Unions – before (C# 14)", Demos.Demos.Before.UnionsBefore.Run),
    ("02 Unions – after (C# 15)", Demos.Demos.After.UnionsAfter.Run),
    ("03 Closed hierarchies", Demos.Demos.Closed.ClosedHierarchies.Run),
    ("04 Collection expression arguments", CollectionArguments.Run),
    ("05 Extension indexers", ExtensionIndexers.Run),
    ("06 Labeled break & continue", LabeledJumps.Run),
    ("07 Static interface members on older runtimes", StaticInterfaceMembers.Run),
};

if (args is [var arg] && int.TryParse(arg, out var pick) && pick >= 1 && pick <= demos.Length)
{
    Run(demos[pick - 1]);
    return;
}

while (true)
{
    Console.WriteLine();
    foreach (var demo in demos)
        Console.WriteLine(demo.Name);
    Console.Write("Pick a demo (Enter = quit): ");
    if (!int.TryParse(Console.ReadLine(), out pick) || pick < 1 || pick > demos.Length)
        break;
    Run(demos[pick - 1]);
}

static void Run((string Name, Action Run) demo)
{
    Console.WriteLine();
    Console.WriteLine($"=== {demo.Name} ===");
    demo.Run();
}
