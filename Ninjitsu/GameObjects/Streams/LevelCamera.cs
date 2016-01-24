using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using G.GameObjects.Characters;
using G.GameObjects.Levels;
using G.GameObjects.Levels.Objects;

namespace G.GameObjects.Streams
{
    public class LevelCamera
    {
        #region Properties

        /// <summary>
        /// Camera position in Level world.
        /// </summary>
        public float CameraPositionYAxis
        {
            get { return cameraPositionYAxis; }
            set { cameraPositionYAxis = value; }
        }
        float cameraPositionYAxis;

        /// <summary>
        /// Camera position in Level world.
        /// </summary>
        public float CameraPositionXAxis
        {
            get { return cameraPositionXAxis; }
            set { cameraPositionXAxis = value; }
        }
        float cameraPositionXAxis;

        #endregion

        #region Methods

        public void ScrollCamera(Viewport viewport, LevelPlayer player, TileEngine tileEngine)
        {
#if ZUNE
const float ViewMargin = 0.45f;
#else
            const float ViewMargin = 0.35f;
#endif

            // Calculate the edges of the screen.
            float marginHeight = viewport.Height + 20.0f;
            float marginWidth = viewport.Width * ViewMargin;
            float marginLeft = CameraPositionXAxis + marginWidth;
            float marginRight = CameraPositionXAxis + viewport.Width - marginWidth;

            // Calculate how far to scroll when the player is near the edges of the screen.
            float cameraMovement = 0.0f;
            if (player.Position.X < marginLeft)
                cameraMovement = player.Position.X - marginLeft;
            else if (player.Position.X > marginRight)
                cameraMovement = player.Position.X - marginRight;

            const float TopMargin = 0.3f;
            const float BottomMargin = 0.1f;
            float marginTop = cameraPositionYAxis + viewport.Height * TopMargin;
            float marginBottom = cameraPositionYAxis + viewport.Height - viewport.Height * BottomMargin;

            float cameraMovementY = 0.0f;
            if (player.Position.Y < marginTop) //above the top margin   
                cameraMovementY = player.Position.Y - marginTop;
            else if (player.Position.Y > marginBottom) //below the bottom margin   
                cameraMovementY = player.Position.Y - marginBottom;   




            // Update the camera position, but prevent scrolling off the ends of the level.
            float maxCameraPositionXOffset = LevelTile.Width * tileEngine.Width - viewport.Width;
            CameraPositionXAxis = MathHelper.Clamp(CameraPositionXAxis + cameraMovement, 0.0f, maxCameraPositionXOffset);

            float maxCameraPositionYOffset = LevelTile.Height * tileEngine.Height - viewport.Height;  
            cameraPositionYAxis = MathHelper.Clamp(cameraPositionYAxis + cameraMovementY, 0.0f, maxCameraPositionYOffset);  
        }

        #endregion
    }
}
