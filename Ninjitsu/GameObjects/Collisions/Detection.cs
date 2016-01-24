#region Using Statements
using Microsoft.Xna.Framework;
using G.Globals;
using G.GameObjects.Levels;
using G.GameObjects.Levels.Objects;
using System;
using G.GameObjects.Characters;
#endregion

namespace G.GameObjects.Collisions
{
    public class Detection
    {
        public static TimeSpan HitTime;

        /// <summary>
        /// Detects and resolves all collisions between the character and his neighboring
        /// tiles. When a collision is detected, the character is pushed away along one
        /// axis to prevent overlapping. There is some special logic for the Y axis to
        /// handle platforms which behave differently depending on direction of movement.
        /// </summary>
        public void CharacterAndTiles(Character character)
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = character.BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / LevelTile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / LevelTile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / LevelTile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / LevelTile.Height)) - 1;


            // Reset flag to search for ground collision.
            character.IsOnGround = false;
        
            //For each potentially colliding movable tile.  
            foreach (var movableTile in Statics.Level.TileEngine.movableTiles)
            {
                // Reset flag to search for movable tile collision.  
                movableTile.PlayerIsOn = false;

                //check to see if player is on tile.  
                if ((character.BoundingRectangle.Bottom == movableTile.BoundingRectangle.Top + 1) &&
                    (character.BoundingRectangle.Left >= movableTile.BoundingRectangle.Left - (character.BoundingRectangle.Width / 2) &&
                     character.BoundingRectangle.Right <= movableTile.BoundingRectangle.Right + (character.BoundingRectangle.Width / 2)))
                {
                    movableTile.PlayerIsOn = true;
                }

                bounds = HandleCollision(bounds, movableTile.Collision, movableTile.BoundingRectangle, character);

            }

            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // If this tile is collidable,
                    TileCollision collision = Statics.Level.TileEngine.GetCollision(x, y);
                    if (collision != TileCollision.Passable)
                    {
                        // Determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = Statics.Level.TileEngine.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);

                        if (collision == TileCollision.Spike)
                        {
                            character.Health--;
                        }
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            // Resolve the collision along the shallow axis.
                            if (absDepthY < absDepthX || collision == TileCollision.Platform)
                            {
                                // If we crossed the top of a tile, we are on the ground.
                                if (character.previousBottom <= tileBounds.Top)
                                    character.IsOnGround = true;

                                // Ignore platforms, unless we are on the ground.
                                if (collision == TileCollision.Impassable || character.IsOnGround)
                                {
                                    // Resolve the collision along the Y axis.
                                    character.Position = new Vector2(character.Position.X, character.Position.Y + depth.Y);

                                    // Perform further collisions with the new bounds.
                                    bounds = character.BoundingRectangle;
                                }
                            }
                            else if (collision == TileCollision.Impassable || collision == TileCollision.Spike) // Ignore platforms.
                            {

                                float temp;
                                // Resolve the collision along the X axis.
                                if (character.direction == Enums.FaceDirection.Left && character != Statics.LevelPlayer)
                                    temp = character.Position.X;
                                else
                                    temp = character.Position.X + depth.X;

                                character.Position = new Vector2(temp, character.Position.Y);

                                // Perform further collisions with the new bounds.
                                bounds = character.BoundingRectangle;
                            }
                        }
                    }
                }
            }
            
            // Save the new bounds bottom.
            character.previousBottom = bounds.Bottom;
        }

        /// <summary>
        /// Used for the movable platforms to push player
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="collision"></param>
        /// <param name="tileBounds"></param>
        /// <returns></returns>
        private Rectangle HandleCollision(Rectangle bounds, TileCollision collision, Rectangle tileBounds, Character character)
        {
            Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
            if (depth != Vector2.Zero)
            {
                float absDepthX = Math.Abs(depth.X);
                float absDepthY = Math.Abs(depth.Y);

                // Resolve the collision along the shallow axis.  
                if (absDepthY < absDepthX || collision == TileCollision.Platform)
                {
                    // If we crossed the top of a tile, we are on the ground.  
                    if (character.previousBottom <= tileBounds.Top)
                        character.IsOnGround = true;

                    // Ignore platforms, unless we are on the ground.  
                    if (collision == TileCollision.Impassable || character.IsOnGround)
                    {
                        // Resolve the collision along the Y axis.  
                        character.Position = new Vector2(character.Position.X, character.Position.Y + depth.Y);

                        // Perform further collisions with the new bounds.  
                        bounds = character.BoundingRectangle;
                    }
                }
                else if (collision == TileCollision.Impassable) // Ignore platforms.  
                {
                    // Resolve the collision along the X axis.  
                    character.Position = new Vector2(character.Position.X + depth.X, character.Position.Y);

                    // Perform further collisions with the new bounds.  
                    bounds = character.BoundingRectangle;
                }
            }
            return bounds;
        } 

        /// <summary>
        /// Animates each enemy and allow them to kill the player.
        /// </summary>
        public void PlayerAndEnemy(LevelPlayer player, Character enemy, GameTime gameTime)
        {
            if (player.oneFrame.Intersects(enemy.BoundingRectangle))
            {
                //Enemy will stop walking, if their bounding box intersect with player and they are not doing some speical animation
                if (enemy.SpecialAnimationDone || enemy == Statics.Level.Boss)
                {
                    if (enemy.MoveState != Enums.MoveState.Fall && enemy.MoveState != Enums.MoveState.FireFall && enemy.MoveState != Enums.MoveState.FireReaction)
                        enemy.movement = 0;
                }

                // Resolve the collision along the X axis between player and enemy, player is not allow to go through the enemy.  
                Vector2 depth = RectangleExtensions.GetIntersectionDepth(player.BoundingRectangle, enemy.BoundingRectangle);
                player.Position = new Vector2(player.Position.X + depth.X, player.Position.Y);

                if (player.MoveState == Enums.MoveState.SpecialCombo)
                {
                        if (enemy == Statics.Level.Boss)
                            enemy.MoveState = Enums.MoveState.Fall;
                        else
                            enemy.MoveState = Enums.MoveState.FireFall;

                        enemy.SpecialAnimationDone = false;
                        if (!player.AttackOnce)
                        {
                             enemy.Health -= player.AttackStrength;
                             player.Health -= 3;
                             player.AttackOnce = true;
                        }
                        player.movement = 0;
                }

                if (PerPixel.Collision(player.oneFrame, player.ColorData, enemy.oneFrame, enemy.ColorData))
                {
                    //Placing this test outside of the Intersects test ensures the player will be capable of damage from every enemy punch
                    //This resolves the problem of only once damage per time the player entered the bounding Rectangle of the enemy.  

                    if (!player.AttackOnce && 
                        enemy.MoveState != Enums.MoveState.FinalCombo2 &&
                        (player.ComboCount > 0 ||
                         player.Health <= 0.2 * player.fullHealth))
                    {
                        switch (player.MoveState)
                        {
                            case Enums.MoveState.Punch:
                                enemy.MoveState = Enums.MoveState.Reaction;
                                enemy.Health -= player.AttackStrength;
                                enemy.hurtCount++;
                                player.CountActions += 6;
                                player.punchHurtCount++;
                                break;
                            case Enums.MoveState.PunchUp:
                                enemy.MoveState = Enums.MoveState.Fall;
                                enemy.Health -= player.AttackStrength;
                                enemy.hurtCount += 2;
                                player.CountActions += 6;
                                break;
                            case Enums.MoveState.Kick:
                                enemy.MoveState = Enums.MoveState.Reaction;
                                enemy.Health -= player.AttackStrength;
                                enemy.hurtCount ++;
                                player.kickHurtCount++;
                                player.CountActions += 10;
                                break;
                            case Enums.MoveState.ComboKick:
                                enemy.MoveState = Enums.MoveState.Fall;
                                enemy.Health -= 2 * player.AttackStrength;
                                enemy.hurtCount += 2;
                                player.CountActions += 20;
                                break;
                            case Enums.MoveState.FirePunch1:
                                enemy.MoveState = Enums.MoveState.FireReaction;
                                enemy.Health -= 2 * player.AttackStrength;
                                player.CountActions += 15;
                                break;
                            case Enums.MoveState.FirePunch2:
                                enemy.MoveState = Enums.MoveState.FireReaction;
                                enemy.Health -= 2 * player.AttackStrength;
                                player.CountActions += 15;
                                break;
                            case Enums.MoveState.FirePunch3:
                                enemy.MoveState = Enums.MoveState.FireFall;
                                enemy.Health -= 2 * player.AttackStrength;
                                player.CountActions += 15;
                                break;
                            case Enums.MoveState.FinalCombo2:
                                enemy.MoveState = Enums.MoveState.FireFall;
                                enemy.Health -= 4 * player.AttackStrength;
                                player.ComboCount--;
                                break;
                        }

                        enemy.hurtSound.Play();
                        player.AttackOnce = true;
                        enemy.SpecialAnimationDone = false;
                        HitTime = gameTime.TotalRealTime;
                    }

                    if (!enemy.AttackOnce && player.SpecialAnimationDone)
                    {
                        if (enemy == Statics.Level.Boss)
                        {
                            switch (enemy.MoveState)
                            {
                                case Enums.MoveState.Punch:
                                    player.MoveState = Enums.MoveState.Fall;
                                    player.Health -= enemy.AttackStrength;
                                    enemy.hitCount++;
                                    break;
                                case Enums.MoveState.FinalCombo2:
                                    player.MoveState = Enums.MoveState.Fall;
                                    player.Health -= 2 * enemy.AttackStrength;
                                    break;
                            }
                        }
                        else
                        {
                            switch (enemy.MoveState)
                            {
                                case Enums.MoveState.FinalCombo1:
                                    player.MoveState = Enums.MoveState.Reaction;
                                    player.Health -= enemy.AttackStrength;
                                    enemy.hitCount++;
                                    break;
                                case Enums.MoveState.FinalCombo2:
                                    player.MoveState = Enums.MoveState.Fall;
                                    player.Health -= 2 * enemy.AttackStrength;
                                    break;
                            }
                        }

                        player.SpecialAnimationDone = false;
                        enemy.hurtSound.Play();
                        enemy.AttackOnce = true;
                    }
                }
            }
            //If no intersection between player rectangle and enemy rectangle frame
            //Add enemy movement towards player here...
            else
            {
                if (enemy.SpecialAnimationDone)
                {
                    if(enemy == Statics.Level.Boss)
                        enemy.movement = 200;
                    else
                        enemy.movement = 65f;
                    enemy.MoveState = Enums.MoveState.Walk;
                }
            }
        }     
    }
}