using Godot;

public partial class Ball : RigidBody2D
{
    private Area2D area;
    private Sprite2D sprite;

    private Texture2D textureClose;
    private Texture2D textureFar;

    private bool playerInside = false;

    public override void _Ready()
    {
        AddToGroup("balls");

        area = GetNode<Area2D>("Area2D");
        sprite = GetNode<Sprite2D>("Sprite2D");

        area.BodyEntered += OnBodyEntered;
        area.BodyExited += OnBodyExited;

        textureClose = GD.Load<Texture2D>("res://assets/Spritesheets/Balls/red/smile.png");
        textureFar   = GD.Load<Texture2D>("res://assets/Spritesheets/Balls/green/smile.png");

        sprite.Texture = textureFar;
    }

    private void OnBodyEntered(Node body)
    {
        if (body is CharacterBody2D)
        {
            playerInside = true;
            sprite.Texture = textureClose;
        }
    }

    private void OnBodyExited(Node body)
    {
        if (body is CharacterBody2D)
        {
            playerInside = false;
            sprite.Texture = textureFar;
        }
    }

    // 🔥 викликається персонажем під час атаки
    public void TryHit()
    {
        if (playerInside)
        {
            GD.Print("Ball destroyed");
            QueueFree();
        }
    }
}
