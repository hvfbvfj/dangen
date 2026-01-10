using System;
using Godot;

public partial class CharacterBody2d : CharacterBody2D
{
    private const float Speed = 150.0f;
    [Export] public float JumpForce = -600.0f;
    private Vector2 Gravity = new Vector2(0, 50f);

    private AnimatedSprite2D AnimatedSprite2D;
    private bool isCrouching = false;
    private bool isAttacking = false;

    private bool wasMousePressed = false;

    private int attackDamage = 5;
    private float attackDuration = 0.25f; // 5 кадрів
    private int lastDirectionX = 1; // 1 = вправо, -1 = вліво

    public override void _Ready()
    {
        AnimatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        //  Переконайся, що Loop = OFF для анімації "attack"
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        Vector2 direction = GetDirection();

        // ---------- Оновлюємо напрямок персонажа ----------
        if (direction.X != 0)
            lastDirectionX = direction.X > 0 ? 1 : -1;

        // ---------- Рух ----------
        float currentSpeed = Speed;
        if (isAttacking) currentSpeed *= 0.5f; // повільніше під час атаки
        if (isCrouching) currentSpeed *= 0.5f;

        velocity.X = direction.X * currentSpeed;

        // ---------- Гравітація ----------
        if (!IsOnFloor())
            velocity.Y += Gravity.Y;

        // ---------- Стрибок ----------
        if (Input.IsPhysicalKeyPressed(Key.Space) && IsOnFloor())
            velocity.Y = JumpForce;

        // ---------- Атака ----------
        bool mousePressed = Input.IsMouseButtonPressed(MouseButton.Left);
        if (mousePressed && !wasMousePressed && !isAttacking)
            StartAttack();
        wasMousePressed = mousePressed;

        // ---------- Анімація + напрямок ----------
        if (AnimatedSprite2D != null)
        {
            // Flip залежить від руху або останнього напрямку
            if (velocity.X != 0)
    AnimatedSprite2D.FlipH = velocity.X > 0;
else
    AnimatedSprite2D.FlipH = lastDirectionX > 0;

            if (isAttacking)
            {
                if (AnimatedSprite2D.Animation != "attack")
                    AnimatedSprite2D.Animation = "attack";
            }
            else
            {
                if (Math.Abs(velocity.X) > 0.01f)
                {
                    if (AnimatedSprite2D.SpriteFrames.HasAnimation("walk"))
                        AnimatedSprite2D.Animation = "walk";
                }
                else
                {
                    if (AnimatedSprite2D.SpriteFrames.HasAnimation("idle"))
                        AnimatedSprite2D.Animation = "idle";
                }
            }
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    private async void StartAttack()
    {
        isAttacking = true;

        if (AnimatedSprite2D.Animation != "attack")
            AnimatedSprite2D.Animation = "attack";

        AnimatedSprite2D.Play();

        GD.Print($"Удар! Урон: {attackDamage} у напрямку {(lastDirectionX > 0 ? "вправо" : "вліво")}");

        await ToSignal(GetTree().CreateTimer(attackDuration), "timeout");

        isAttacking = false;

        if (AnimatedSprite2D.SpriteFrames.HasAnimation("idle") && Velocity.X == 0)
            AnimatedSprite2D.Animation = "idle";
    }

    public Vector2 GetDirection()
    {
        Vector2 direction = new Vector2();

        if (Input.IsPhysicalKeyPressed(Key.W)) direction.Y -= 1;
        if (Input.IsPhysicalKeyPressed(Key.S)) direction.Y += 1;
        if (Input.IsPhysicalKeyPressed(Key.A)) direction.X -= 1;
        if (Input.IsPhysicalKeyPressed(Key.D)) direction.X += 1;

        return direction;
    }
}  