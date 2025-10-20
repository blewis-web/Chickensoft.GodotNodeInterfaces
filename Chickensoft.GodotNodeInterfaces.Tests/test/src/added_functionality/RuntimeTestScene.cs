namespace Chickensoft.GodotNodeInterfaces.RuntimeTests;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IRuntimeTestScene : INode2D { }

[Meta(typeof(IAutoNode))]
public partial class RuntimeTestScene : Node2D, IRuntimeTestScene {
  public override void _Notification(int what) => this.Notify(what);

  [Node] public INode2D MyChild { get; set; } = default!;

  public void OnReady() {
    GD.Print("OnReady");

    // This would normally be set in Main.cs, but this project isn't set up like a normal consumer project.
    RuntimeContext.IsTesting = true;

    var myChild = GetNodeOrNull<Node2D>(nameof(MyChild));
    GD.Print($"Found {myChild.Name} via GetNodeOrNull");

    MyChild = this.GetNodeOrNullEx<INode2D>(nameof(MyChild))!;
    GD.Print($"Found {MyChild?.Name ?? "nothing"} via GetNodeOrNullEx");

    var myChildren = GetChildren();
    GD.Print($"Found {myChildren.Count} children via GetChildren");

    var myChildrenEx = this.GetChildrenEx()!;
    GD.Print($"Found {myChildrenEx.Length} children via GetChildrenEx");

    GD.Print("AddChild");
    AddChild(new Node2D() { Name = "MyNewChild" });
    GD.Print($"Found {GetNodeOrNull("MyNewChild")?.Name ?? "nothing"} with GetNodeOrNull after AddChild");
    GD.Print($"Found {this.GetNodeOrNullEx("MyNewChild")?.Name ?? "nothing"} with GetNodeOrNullEx after AddChild");

    GD.Print("AddChildEx");
    this.AddChildEx(GodotInterfaces.Adapt<INode2D>(new Node2D() { Name = "MyNewChildEx" }));
    GD.Print($"Found {GetNodeOrNull("MyNewChildEx")?.Name ?? "nothing"} with GetNodeOrNull after AddChildEx");
    GD.Print($"Found {this.GetNodeOrNullEx("MyNewChildEx")?.Name ?? "nothing"} with GetNodeOrNullEx after AddChildEx");
  }
}
