namespace Chickensoft.GodotNodeInterfaces.Tests;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using GoDotTest;
using Moq;
using Shouldly;

public interface ITestNode : INode {
  public void TestAddChild(INode child);

  public INode[] TestGetChildren();

  public T? TestGetNodeOrNull<T>(NodePath path) where T : class, INode;
}

[Meta(typeof(IAutoNode))]
public partial class TestNode : Node, ITestNode {
  public override void _Notification(int what) => this.Notify(what);

  private INode MyChild { get; set; } = default!;

  public void OnReady() => MyChild = this.GetNodeOrNullEx<INode>("MyChild")!;

  public void TestAddChild(INode child) => this.AddChildEx(child);

  public INode[] TestGetChildren() => this.GetChildrenEx();

  public T? TestGetNodeOrNull<T>(NodePath path) where T : class, INode
    => this.GetNodeOrNullEx<T>(path);
}

public class INodeTests(Node testScene) : TestClass(testScene) {

  [Test]
  public void TestAddChildShouldUpdateFakeNodeTree() {
    RuntimeContext.IsTesting = true;

    var node = new TestNode();
    IFakeNodeTreeEnabled fakeTree = node;
    var myChild = new Mock<INode>();
    var iNodeChild = GodotInterfaces.Adapt<INode>(new Node());

    fakeTree.FakeNodes.ShouldNotBeNull();

    fakeTree.FakeNodes.GetChildCount().ShouldBe(0);
    node.TestGetChildren().ShouldBeEmpty();
    node.GetChildrenEx().ShouldBeEmpty();

    node.FakeNodeTree(new() {
      ["MyChild"] = myChild.Object
    });
    node.OnReady();

    fakeTree.FakeNodes.GetChildCount().ShouldBe(1);
    node.TestGetChildren().ShouldContain(myChild.Object);
    node.GetChildrenEx().ShouldContain(myChild.Object);
    node.TestGetNodeOrNull<INode>("MyChild").ShouldBe(myChild.Object);
    node.GetNodeOrNullEx<INode>("MyChild").ShouldBe(myChild.Object);
    node.GetChildren().ShouldBeEmpty();

    node.TestAddChild(iNodeChild);

    fakeTree.FakeNodes.GetChildCount().ShouldBe(2);
    node.TestGetChildren().ShouldContain(iNodeChild);
    node.GetChildrenEx().ShouldContain(iNodeChild);
    node.GetChildren().ShouldBeEmpty();
  }
}
