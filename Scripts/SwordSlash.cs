using Godot;
using System;

public partial class SwordSlash : Node2D
{
    [Export] public float weaponDamage = 10.0f;
    public AnimationPlayer  animationPlayer;
    
    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("Sprite2D/AnimationPlayer");
        animationPlayer.AnimationFinished += OnAniationFinished;
        LookAt(GetGlobalMousePosition());
        animationPlayer.Play("Slash");
    }

    private void OnAniationFinished(StringName animName)
    {
        if(animName == "Slash")
            QueueFree();  
    }

    private void OnArea2DAreaEntered(Area2D area)
    {
        Node parentNode = area.GetParent();

        if (parentNode is IDamageble target)
        {
            target.TakeDamage(weaponDamage);
        }
    }
}
