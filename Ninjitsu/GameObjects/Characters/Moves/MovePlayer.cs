using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using G.Globals;
using G.GameObjects.Collisions;

namespace G.GameObjects.Characters.Moves
{
    public class MovePlayer
    {
        #region Fields

        // Constants for controling horizontal movement
        private const float MoveAcceleration = 14000.0f;
        private const float MaxMoveSpeed = 2000.0f;
        private const float GroundDragFactor = 0.65f;
        private const float AirDragFactor = 0.65f;

        // Constants for controlling vertical movement
        private const float MaxJumpTime = 0.35f;
        private const float JumpLaunchVelocity = -4000.0f;
        private const float GravityAcceleration = 3500.0f;
        private const float MaxFallSpeed = 600.0f;
        private const float JumpControlPower = 0.14f;

        private bool soundPlayed = false;

         #endregion

        /// <summary>
        /// Updates the boss's move.
        /// </summary>
        public void UpdateBossMove(GameTime gameTime, Boss boss)
        {
            if (boss.Alive)
            {
                ApplyPhysics(gameTime, boss);

                switch (boss.MoveState)
                {
                    case Enums.MoveState.Idle:
                        boss.sprite.PlayAnimation(boss.idleAnimation);
                        break;
                    case Enums.MoveState.Punch:
                        boss.sprite.PlayAnimation(boss.punchAnimation);
                        break;
                    case Enums.MoveState.Walk:
                        boss.sprite.PlayAnimation(boss.idleAnimation);
                        break;
                    case Enums.MoveState.FinalCombo2:
                        boss.sprite.PlayAnimation(boss.lightingAnimation);
                        break;
                    case Enums.MoveState.Reaction:
                        boss.sprite.PlayAnimation(boss.idleAnimation);
                        break;
                    case Enums.MoveState.FireReaction:
                        boss.sprite.PlayAnimation(boss.fallAnimation);
                        boss.movement = -150;
                        break;
                    case Enums.MoveState.Fall:
                        boss.sprite.PlayAnimation(boss.fallAnimation);
                        boss.movement = -250;
                        break;
                    case Enums.MoveState.FireFall:
                        boss.sprite.PlayAnimation(boss.fallAnimation);
                        boss.movement = -450;
                        break;
                }

                // Clear Input

                if ((boss.sprite.FrameIndex == boss.sprite.animation.FrameCount - 1) &&
                    boss.SpecialAnimationDone == false)
                {
                    boss.SpecialAnimationDone = true;
                }

                boss.isJumping = false;
            }
        }

        /// <summary>
        /// Updates the enemy's move.
        /// </summary>
        public void UpdateEnemyMove(GameTime gameTime, Enemy enemy)
        {
            if (enemy.Alive)
            {
                ApplyPhysics(gameTime, enemy);

                switch (enemy.MoveState)
                {
                    case Enums.MoveState.Walk:
                        enemy.sprite.PlayAnimation(enemy.walkAnimation);
                        break;
                    case Enums.MoveState.FinalCombo1:
                        enemy.sprite.PlayAnimation(enemy.finalCombo1Animation);
                        break;
                    case Enums.MoveState.FinalCombo2:
                        enemy.sprite.PlayAnimation(enemy.finalCombo2Animation);
                        enemy.movement = 100f;
                        break;
                    case Enums.MoveState.Reaction:
                        enemy.movement = 0;
                        enemy.sprite.PlayAnimation(enemy.reactionAnimation);
                        break;
                    case Enums.MoveState.FireReaction:
                        enemy.sprite.PlayAnimation(enemy.fireReactionAnimation);
                        enemy.movement = -200;
                        break;
                    case Enums.MoveState.Fall:
                        enemy.sprite.PlayAnimation(enemy.fallAnimation);
                        enemy.movement = -450;
                        break;
                    case Enums.MoveState.FireFall:
                        enemy.sprite.PlayAnimation(enemy.fireFallAnimation);
                        enemy.movement = -450;
                        break;
                    case Enums.MoveState.Idle:
                        enemy.sprite.PlayAnimation(enemy.idleAnimation);
                        break;
                }

                // Clear Input

                if ((enemy.sprite.FrameIndex == enemy.sprite.animation.FrameCount - 1) &&
                    enemy.SpecialAnimationDone == false)
                {
                    enemy.SpecialAnimationDone = true;
                }

                enemy.isJumping = false;
            }
        }

        /// <summary>
        /// Updates the player's move based on input.
        /// </summary>
        public void UpdatePlayerMove(GameTime gameTime, LevelPlayer player)
        {
            if (player.Health > 0)
            {
                ApplyPhysics(gameTime, player);

                switch (player.MoveState)
                {
                    case Enums.MoveState.Run:
                        if (Math.Abs(player.Velocity.X) - 0.02f > 0 && player.IsOnGround)
                            player.sprite.PlayAnimation(player.runAnimation);
                        break;
                    case Enums.MoveState.Punch:
                        player.movement = 0;
                        player.sprite.PlayAnimation(player.punchAnimation);
                        if (!soundPlayed)
                        {
                            player.HitSound.Play();
                            soundPlayed = true;
                        }
                        break;
                    case Enums.MoveState.PunchUp:
                        player.sprite.PlayAnimation(player.punchUpAnimation);
                        player.movement = 0.1f;
                        if (!soundPlayed)
                        {
                            player.HitSound.Play();
                            soundPlayed = true;
                        }
                        break;
                    case Enums.MoveState.Kick:
                        player.sprite.PlayAnimation(player.kickAnimation);
                        player.movement = 0;
                        if (!soundPlayed)
                        {
                            player.HitSound.Play();
                            soundPlayed = true;
                        }
                        break;
                    case Enums.MoveState.ComboKick:
                        player.sprite.PlayAnimation(player.comboKickAnimation);
                        player.movement = 0.1f;
                        if (!soundPlayed)
                        {
                            player.HitSound.Play();
                            soundPlayed = true;
                        }
                        break;
                    case Enums.MoveState.FirePunch1:
                        player.sprite.PlayAnimation(player.firePunch1Animation);
                        player.movement = 0.1f;
                        player.firePunchCount = 1;
                        if (!soundPlayed)
                        {
                            player.firePunchSound.Play();
                            soundPlayed = true;
                        }
                        break;
                    case Enums.MoveState.FirePunch2:
                        player.sprite.PlayAnimation(player.firePunch2Animation);
                        player.movement = 0.2f;
                        player.firePunchCount = 2;
                        if (!soundPlayed)
                        {
                            player.firePunchSound.Play();
                            soundPlayed = true;
                        }
                        break;
                    case Enums.MoveState.FirePunch3:
                        player.sprite.PlayAnimation(player.firePunch3Animation);
                        player.movement = 0.2f;
                        player.firePunchCount = 0;
                        player.chainMoves.Clear();
                        if (!soundPlayed)
                        {
                            player.firePunchSound.Play();
                            soundPlayed = true;
                        }
                        break;
                    case Enums.MoveState.SpecialCombo:
                        player.sprite.PlayAnimation(player.specialAnimation);
                        if (!soundPlayed)
                        {
                            player.HitSound.Play();
                            soundPlayed = true;
                        }
                        break;
                    case Enums.MoveState.FinalCombo2:
                        if (player.ComboCount > 0 ||
                            player.Health <= 0.2 * player.fullHealth)
                         {
                             player.sprite.PlayAnimation(player.finalCombo2Animation);
                             player.movement = 0.2f;
                             if (!soundPlayed)
                             {
                                 player.finalComboSound.Play();
                                 soundPlayed = true;
                             }
                        }
                        break;
                    case Enums.MoveState.Reaction:
                        player.sprite.PlayAnimation(player.reactionAnimation);
                        player.movement = -0.1f;
                        break;
                    case Enums.MoveState.Fall:
                        player.sprite.PlayAnimation(player.fallAnimation);
                        player.movement = -0.4f ;
                        break;
                    case Enums.MoveState.Upgrade:
                        player.movement = 0;
                        player.sprite.PlayAnimation(player.upgradeAnimation);
                        break;
                    case Enums.MoveState.Idle:
                        if (player.IsOnGround)
                        {
                            player.movement = 0;
                            player.sprite.PlayAnimation(player.idleAnimation);
                        }
                        break;
                }

                // Clear input.
                if ((player.sprite.FrameIndex == player.sprite.animation.FrameCount - 1) &&
                    player.SpecialAnimationDone == false)
                {
                    soundPlayed = false;
                    player.SpecialAnimationDone = true;
                }

                if (player.SpecialAnimationDone)
                {
                    player.movement = 0.0f;
                    player.MoveState = Enums.MoveState.Idle;

                    if (player.punchHurtCount == 2 || player.kickHurtCount == 2)
                    {
                        if (player.punchHurtCount == 2)
                        {
                            player.MoveState = Enums.MoveState.PunchUp;
                            player.punchHurtCount = 0;
                        }

                        if (player.kickHurtCount == 2)
                        {
                            player.MoveState = Enums.MoveState.ComboKick;
                            player.kickHurtCount = 0;
                        }

                        player.movement = 10f;
                        player.SpecialAnimationDone = false;
                        player.AttackOnce = false;
                    }
                }

                player.isJumping = false;
            }
        }
        
        /// <summary>
        /// Updates the player's velocity and position based on input, gravity, etc.
        /// </summary>
        public void ApplyPhysics(GameTime gameTime, Character character)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = character.Position;

            // Base velocity is a combination of horizontal movement control and
            // acceleration downward due to gravity.
            if (character == Statics.LevelPlayer)
            {
                character.Velocity.X += (int)character.direction * character.movement * MoveAcceleration * elapsed;
            }
            else
                character.Velocity.X = (int)character.direction * character.movement;

            character.Velocity.Y = MathHelper.Clamp(character.Velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

            character.Velocity.Y = DoJump(character.Velocity.Y, gameTime, character);

            // Apply pseudo-drag horizontally.
            if (character.IsOnGround)
                character.Velocity.X *= GroundDragFactor;
            else
                character.Velocity.X *= AirDragFactor;

            // Prevent the character from running faster than his top speed.            
            character.Velocity.X = MathHelper.Clamp(character.Velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

            // Apply velocity.
            character.Position += character.Velocity * elapsed;
            character.Position = new Vector2((float)Math.Round(character.Position.X), (float)Math.Round(character.Position.Y));

            // If the character is now colliding with the level, separate them.
            Statics.Collision.CharacterAndTiles(character);

            // If the collision stopped us from moving, reset the velocity to zero.
            if (character.Position.X == previousPosition.X)
                character.Velocity.X = 0;

            if (character.Position.Y == previousPosition.Y)
                character.Velocity.Y = 0;
        }

        /// <summary>
        /// Calculates the Y velocity accounting for jumping and
        /// animates accordingly.
        /// </summary>
        /// <remarks>
        /// During the accent of a jump, the Y velocity is completely
        /// overridden by a power curve. During the decent, gravity takes
        /// over. The jump velocity is controlled by the jumpTime field
        /// which measures time into the accent of the current jump.
        /// </remarks>
        /// <param name="velocityY">
        /// The player's current velocity along the Y axis.
        /// </param>
        /// <returns>
        /// A new Y velocity if beginning or continuing a jump.
        /// Otherwise, the existing Y velocity.
        /// </returns>
        private static float DoJump(float velocityY, GameTime gameTime, Character character)
        {
            // If the player wants to jump
            if (character.isJumping)
            {
                // Begin or continue a jump
                if ((!character.wasJumping && character.IsOnGround) || character.jumpTime > 0.0f)
                {
                    if (character.jumpTime == 0.0f)
                    {
                        if(character == Statics.LevelPlayer)
                          character.jumpSound.Play();
                    }

                    character.jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    character.sprite.PlayAnimation(character.jumpAnimation);
                }

                // If we are in the ascent of the jump
                if (0.0f < character.jumpTime && character.jumpTime <= MaxJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(character.jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    // Reached the apex of the jump
                    character.jumpTime = 0.0f;
                }
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                character.jumpTime = 0.0f;
            }
            character.wasJumping = character.isJumping;

            return velocityY;
        }
    }
}
