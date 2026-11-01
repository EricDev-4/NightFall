using Godot;
using System;

public partial class Character : CharacterBody2D
{
	public const float Speed = 100.0f;
	
	private AnimatedSprite2D  charSprite2D;
	private Node2D weaponAxe;
	private Node2D weaponPivot;
	private AnimationPlayer weaponAnimation;
	
	private float weaponRestRotation;
	private Vector2 weaponRestScale;


	[Export] private float slash_time = 0.2f;
	[Export] private float sword_return_time = 0.5f;
	[Export] private float weapon_damage = 1.0f;

	private bool can_slash = true;
	private PackedScene swordSlashPreload;
	public override void _Ready()
	{
		charSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		weaponPivot = GetNode<Node2D>("WeaponPivot");
		weaponAxe = GetNode<Node2D>("WeaponPivot/AttackPivot/Axe");
		weaponAnimation = GetNode<AnimationPlayer>("WeaponPivot/AttackPivot/AnimationPlayer");
		weaponRestRotation = weaponAxe.Rotation;
		weaponRestScale = weaponAxe.Scale;

		swordSlashPreload = GD.Load<PackedScene>("res://Scene/sword_slash.tscn");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		Vector2 mousePos = GetGlobalMousePosition();

		weaponPivot.LookAt(mousePos);
		float mirror = mousePos.X < weaponPivot.GlobalPosition.X ? -1.0f : 1.0f;
		// Mirror around the pivot's aim axis, including the axe's rest rotation.
		weaponAxe.Rotation = weaponRestRotation * mirror;
		weaponAxe.ZIndex = (int)mirror * 2;
		weaponAxe.Scale = new Vector2(weaponRestScale.X, weaponRestScale.Y * mirror);

		if (mousePos.X < weaponPivot.GlobalPosition.X)
		{
			charSprite2D.FlipH = true;
		}
		else
		{
			charSprite2D.FlipH = false;
		}
		
		Vector2 direction = Input.GetVector("Left", "Right", "Up", "Down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Y = direction.Y * Speed;
			charSprite2D.Play("Walk");
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
			charSprite2D.Play("Idle");
		}
		Velocity = velocity;
		MoveAndSlide();

		InventoryUI inventory = GetTree().GetFirstNodeInGroup("inventory_ui") as InventoryUI;
		if (Input.IsActionPressed("Attack") && can_slash && (inventory == null || !inventory.BlocksAttack))
		{
			weaponAnimation.SpeedScale = (float)weaponAnimation.GetAnimation("slash").Length / slash_time;
			weaponAnimation.Play("slash");
			can_slash = false;
		}
	}

	private void spawn_slash()
	{
		SwordSlash swordSlash = swordSlashPreload.Instantiate<SwordSlash>();
		swordSlash.GlobalPosition = GlobalPosition;
		swordSlash.GetNode<AnimationPlayer>("Sprite2D/AnimationPlayer").SpeedScale = 
			(float)swordSlash.GetNode<AnimationPlayer>("Sprite2D/AnimationPlayer").GetAnimation("Slash").Length / slash_time;
			swordSlash.GetNode<Sprite2D>("Sprite2D").FlipV =
				GetGlobalMousePosition() > GetGlobalPosition() ? true : false;
			swordSlash.weaponDamage = weapon_damage;
			GetParent().AddChild(swordSlash);
	}

	private void OnAnimationFinished(StringName animName)
	{
		if (animName == "slash")
		{
			weaponAnimation.SpeedScale = (float)weaponAnimation.GetAnimation("sword_return").Length / sword_return_time;
			
			weaponAnimation.Play("sword_return");
		}
		else
		{
			can_slash = true;
		}
	}
}
