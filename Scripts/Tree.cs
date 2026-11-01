using Godot;
using System;

public partial class Tree : Node2D ,IDamageble
{
	[Export] private float HP = 10f;
	private PackedScene woodScene;
	private AnimationPlayer animationPlayer;
	private bool isDie = false;
	private int minCount=3, maxCount =5;
	public override void _Ready()
	{
		woodScene = GD.Load<PackedScene>("res://Scene/wood.tscn");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}
	
	public void TakeDamage(float damage)
	{
		if (isDie)
			return;
		HP -= damage;
		if (HP <= 0)
		{
			isDie = true;
			CallDeferred(MethodName.Die);
			return;
		}
		animationPlayer.Play("GetDamagedTree");
	}

	public void Die()
	{
		int random = GD.RandRange(minCount, maxCount);

		for (int i = 0; i < random; i++)
		{
			RigidBody2D wood = woodScene.Instantiate<RigidBody2D>();
			
			GetParent().AddChild(wood);
			wood.GlobalPosition = GlobalPosition;
			
			wood.ApplyCentralImpulse(new Vector2(
				(float)GD.RandRange(-80.0f, 80.0f),
				(float)GD.RandRange(-80.0f, 80.0f)
				));
		}
		QueueFree();
	}
	
}
