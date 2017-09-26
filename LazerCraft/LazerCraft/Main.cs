using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace LazerCraft
{
    public enum screen
    {
        Title,
        Loadout,
        Lobby,
        MapEditor,
        Game,
    }

    public class Main : Microsoft.Xna.Framework.Game
    {        
        #region Object Memory Creation

        #region Game Necessities
        GraphicsDeviceManager graphics;
        public static GraphicsDevice graphicsDevice;
        SpriteBatch spriteBatch;
        #endregion

        #region Program Misc
        static public Vector2 prefferedDimensions = new Vector2(1024, 576);
        static public Vector2 screenDimensions;
        static public Vector2 screenChange;
        SpriteFont font;
        public static Random random = new Random();
        screen currentScreen = screen.Title;
        static public Color currentColor = Color.Black;
        Color targetColor = Color.Black;
        int currentlyHighlighted;
        Effect alphaShader;
        #endregion

        #region Title Screen
        int animationSequence;
        float titleAlpha = 0f;
        Vector2[] iconPositions;
        string[] iconStrings = new string[] { "Single Player", "Challenges", "Level Editor", "Matchmaking", "Custom Server", "Edit Loadout", "Settings", "Credits" };

        LaserTexture titleBorders;
        LaserTexture title;
        LaserTexture icons;
        string subtitle;

        int selectedIcon;
        int selectedIconVariable;
        static int totalTrials = 3;
        Texture2D challengeBorder;
        Texture2D challengeLargeArrow;
        Texture2D challengeSmallArrow;
        Texture2D challengeBox;
        LaserTexture saveSelectFrame;
        LaserTexture saveSelectBox;
        LaserTexture difficulties;
        #endregion

        #region External Data and Input
        int continueLevel;
        int unlockedLevels;
        float[] timeTrialTimes = new float[totalTrials];
        char keyUp;
        char keyDown;
        char keyLeft;
        char keyRight;
        char keySpecial3;
        char keySpecial4;
        bool muteSound;
        bool muteMusic;
        static float laserBrightness = 1f;

        public static int totalSaves;
        public static SinglePlayerSave[] singlePlayerSaves;

        KeyboardState currentKeyboardState;
        KeyboardState lastKeyboardState;
        MouseState currentMouseState;
        MouseState lastMouseState;
        Rectangle currentMouseRectangle;
        Rectangle lastMouseRectangle;
        static public Vector2 mouseLocation;
        static public bool keyUpPressed;
        static public bool keyDownPressed;
        static public bool keyLeftPressed;
        static public bool keyRightPressed;
        static public bool keySpecial1Pressed;
        static public bool keySpecial2Pressed;
        static public bool keySpecial3Pressed;
        static public bool keySpecial4Pressed;
        static public bool keySpecial1WasPressed;
        static public bool keySpecial2WasPressed;
        static public bool keySpecial3WasPressed;
        static public bool keySpecial4WasPressed;
        #endregion

        #region Game
        static public Vector2 screenPosition;
        static public List<Player> players;
        static public Texture2D chicken;
        static public Texture2D shield;
        Texture2D attributes;
        Texture2D abilityIconsEmpty;
        Texture2D abilityIconsFull;
        public static Level level;
        public static Texture2D[] levelBackgrounds;
        public static Texture2D tileTexture;
        public static Texture2D tileTextureFull;
        #endregion

        #region Abilities
        static public Texture2D shotColor;
        static public Texture2D shotNoColor;
        static public Texture2D blastColor;
        static public Texture2D blastNoColor;
        static public Texture2D boltColor;
        static public Texture2D boltNoColor;
        static public Texture2D chargeColor;
        static public Texture2D chargeNoColor;
        static public Texture2D missileColor;
        static public Texture2D missileNoColor;
        static public Texture2D circleColor;
        static public Texture2D circleNoColor;
        static public Texture2D mineColor;
        static public Texture2D mineNoColor;
        static public Texture2D shieldColor;
        static public Texture2D shieldNoColor;
        #endregion

        #region Editor
        public static Texture2D editorTypes;
        LaserTexture editorHud;
        string[] editorCategories = new string[]{"Options", "Types", "Texture", "Border"};
        string[] editorOptionCategories = new string[]{"Properties", "Load", "Save", "Help", "Test"};
        bool focused;
        int focus;


        /*
        Texture2D editorFrame;
        Texture2D editorHeaderRectangle;
        Texture2D editorTileBackdrop;
        Texture2D editorNoTile;
        Texture2D editorTileOutline;
        Texture2D editorSwitchActive;
        Texture2D editorSwitchInactive;
        byte editRotate;
        bool editTypesOrTextures;
        byte editorSelectedTile;
        string[] editorWords = new string[] {"New", "Save","Load","Help"};
         */
        #endregion

        #region Loadout
        Texture2D loadoutArrow;
        Texture2D loadoutFrame;
        Texture2D loadoutAbilityHighlighter;
        Texture2D attributesx2;
        Texture2D loadoutScale;
        string[] loadoutWords = new string[] { "New", "Save", "Clear", "Help" };
        #endregion

        #region Sounds

        #endregion

        #endregion

        public Main()
        {
            #region Game Necessities
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            #endregion
        }

        protected override void Initialize()
        {
            graphicsDevice = graphics.GraphicsDevice;
            spriteBatch = new SpriteBatch(graphicsDevice);
            LoadData();
            screenChange = (screenDimensions - prefferedDimensions)/2;
            graphics.PreferredBackBufferWidth = (int)screenDimensions.X;
            graphics.PreferredBackBufferHeight = (int)screenDimensions.Y;
            graphics.ApplyChanges();
            this.IsMouseVisible = true;
            level = new Level();
            singlePlayerSaves = new SinglePlayerSave[4];
            for (int index = 0; index < singlePlayerSaves.Count(); index += 1)
            {
                singlePlayerSaves[index] = new SinglePlayerSave(index+1);
                singlePlayerSaves[index].Load();
            }
            singlePlayerSaves[1].areasDiscovered[0, 0] = true;
            singlePlayerSaves[1].areasDiscovered[0, 1] = true;
            singlePlayerSaves[1].areasDiscovered[0, 2] = true;
            singlePlayerSaves[1].tapesDiscovered[0] = true;
            singlePlayerSaves[1].tapesDiscovered[1] = true;
            singlePlayerSaves[1].bossesKilled[0] = true;
            singlePlayerSaves[1].Save(false);
            ChangeScreen(screen.Title);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            #region Other Load Content
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>(@"Game/Font");
            alphaShader = Content.Load<Effect>(@"Game/Effects/AlphaMap");
            #endregion

            #region Title Screen
            title = new LaserTexture(Content.Load<Texture2D>(@"Game/Menu/titleInner"), Content.Load<Texture2D>(@"Game/Menu/titleOuter"));
            titleBorders = new LaserTexture(Content.Load<Texture2D>(@"Game/Menu/titleBordersInner"), Content.Load<Texture2D>(@"Game/Menu/titleBordersOuter"));
            subtitle = "Developed By Tabski";
            icons = new LaserTexture(Content.Load<Texture2D>(@"Game/Menu/iconsInner") ,Content.Load<Texture2D>(@"Game/Menu/iconsOuter"));
            challengeBorder = Content.Load<Texture2D>(@"Game/Menu/challengeBorder");
            challengeLargeArrow = Content.Load<Texture2D>(@"Game/Menu/challengeLargeArrow");
            challengeSmallArrow = Content.Load<Texture2D>(@"Game/Menu/challengeSmallArrow");
            challengeBox = Content.Load<Texture2D>(@"Game/Menu/challengeBox");
            saveSelectFrame = new LaserTexture(Content.Load<Texture2D>(@"Game/Menu/saveSelectFrameInner"), Content.Load<Texture2D>(@"Game/Menu/saveSelectFrameOuter"));
            saveSelectBox = new LaserTexture(Content.Load<Texture2D>(@"Game/Menu/saveSelectBoxInner"), Content.Load<Texture2D>(@"Game/Menu/saveSelectBoxOuter"));
            difficulties = new LaserTexture(Content.Load<Texture2D>(@"Game/Menu/difficultiesInner"), Content.Load<Texture2D>(@"Game/Menu/difficultiesOuter"));
            #endregion

            #region Game
            chicken = Content.Load<Texture2D>(@"Game/Game/chicken");
            shield = Content.Load<Texture2D>(@"Game/Game/shield");
            attributes = Content.Load<Texture2D>(@"Game/Game/attributes");
            abilityIconsEmpty = Content.Load<Texture2D>(@"Game/Game/abilityIconsEmpty");
            abilityIconsFull = Content.Load<Texture2D>(@"Game/Game/abilityIconsFull");

            #endregion

            #region Abilities
            shotColor = Content.Load<Texture2D>(@"Game/Abilities/shotColor");
            shotNoColor = Content.Load<Texture2D>(@"Game/Abilities/shotNoColor");
            blastColor = Content.Load<Texture2D>(@"Game/Abilities/blastColor");
            blastNoColor = Content.Load<Texture2D>(@"Game/Abilities/blastNoColor");
            boltColor = Content.Load<Texture2D>(@"Game/Abilities/boltColor");
            boltNoColor = Content.Load<Texture2D>(@"Game/Abilities/boltNoColor");
            chargeColor = Content.Load<Texture2D>(@"Game/Abilities/chargeColor");
            chargeNoColor = Content.Load<Texture2D>(@"Game/Abilities/chargeNoColor");
            missileColor = Content.Load<Texture2D>(@"Game/Abilities/missileColor");
            missileNoColor = Content.Load<Texture2D>(@"Game/Abilities/missileNoColor");
            circleColor = Content.Load<Texture2D>(@"Game/Abilities/circleColor");
            circleNoColor = Content.Load<Texture2D>(@"Game/Abilities/circleNoColor");
            mineColor = Content.Load<Texture2D>(@"Game/Abilities/mineColor");
            mineNoColor = Content.Load<Texture2D>(@"Game/Abilities/mineNoColor");
            mineColor = Content.Load<Texture2D>(@"Game/Abilities/mineColor");
            mineNoColor = Content.Load<Texture2D>(@"Game/Abilities/mineNoColor");
            #endregion

            #region Editor
            editorTypes = Content.Load<Texture2D>(@"Game/Editor/types");
            editorHud = new LaserTexture(Content.Load<Texture2D>(@"Game/Editor/hudInner"),Content.Load<Texture2D>(@"Game/Editor/hudOuter"));
            /*
            editorFrame = Content.Load<Texture2D>(@"Game/Editor/frame");
            editorHeaderRectangle = Content.Load<Texture2D>(@"Game/Editor/headerRectangle");
            editorTileBackdrop = Content.Load<Texture2D>(@"Game/Editor/tileBackdrop");
            editorNoTile = Content.Load<Texture2D>(@"Game/Editor/noTile");
            editorTileOutline = Content.Load<Texture2D>(@"Game/Editor/tileOutline");
            editorSwitchActive = Content.Load<Texture2D>(@"Game/Editor/switchActive");
            editorSwitchInactive = Content.Load<Texture2D>(@"Game/Editor/switchInactive");
            alphaShader.Parameters["TypeTexture"].SetValue(editorTypes);
            alphaShader.Parameters["typesSize"].SetValue(editorTypes.Width);
             */
            #endregion

            #region Loadout
            //STEP 2
            /////////////////////////////////////////////////////////////////////////////////
            loadoutArrow = Content.Load<Texture2D>(@"Game/Loadout/arrow");
            loadoutFrame = Content.Load<Texture2D>(@"Game/Loadout/frame");
            loadoutAbilityHighlighter = Content.Load<Texture2D>(@"Game/Loadout/abilityHighlighter");
            attributesx2 = Content.Load<Texture2D>(@"Game/Loadout/attributesx2");
            loadoutScale = Content.Load<Texture2D>(@"Game/Loadout/scale");
            ///////////////////////////////////////////////////////////////////////////////
            #endregion

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            UpdateInput();
            UpdateColor(gameTime);
            currentlyHighlighted = 0;

            #region Title
            if (currentScreen == screen.Title)
            {
                #region Intro Animation Sequence
                if (animationSequence == 1)
                {
                    if (titleAlpha < 1.0f)
                    {
                        titleAlpha += .001f * gameTime.ElapsedGameTime.Milliseconds;
                    }
                    else
                    {
                        titleAlpha = 0f;
                        animationSequence += 1;
                    }
                }
                else if (animationSequence == 2)
                {
                    if (titleAlpha < 1.0f)
                    {
                        titleAlpha += .001f * gameTime.ElapsedGameTime.Milliseconds;
                    }
                    else
                    {
                        titleAlpha = 0f;
                        animationSequence += 1;
                    }
                }
                #endregion

                if (currentScreen == screen.Title && animationSequence == 3)
                {
                    #region Get Icon
                    for (int index = 0; index < 8; index += 1)
                    {
                        Rectangle iconRectangle;
                        if (index == 6 || index == 7)
                        {
                            iconRectangle = new Rectangle((int)(iconPositions[index].X - 25 + screenChange.X), (int)(iconPositions[index].Y - 25 + screenChange.Y), 50, 50);
                        }
                        else
                        {
                            iconRectangle = new Rectangle((int)(iconPositions[index].X - 50 + screenChange.X), (int)(iconPositions[index].Y - 50 + screenChange.Y), 100, 100);
                        }
                        if (currentMouseRectangle.Intersects(iconRectangle))
                        {
                            currentlyHighlighted = index + 1;
                            if (lastMouseRectangle.Intersects(iconRectangle) != true)
                            {
                                //SoundManager.Instance.playSound("hover");
                            }
                            subtitle = iconStrings[index];
                            if (currentMouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton != ButtonState.Pressed)
                            {
                                selectedIcon = index;
                                selectedIconVariable = 1;
                                //SoundManager.Instance.playSound("click");
                            }
                        }
                    }
                    #endregion

                    if (selectedIcon == 0)
                    {

                        /*
                        level.levelName = "_L" + continueLevel.ToString();
                        level.LoadLevel();
                        ChangeScreen(screen.Game);
                         */
                    }

                    else if (selectedIcon == 1)
                    {
                        if (currentMouseRectangle.Intersects(new Rectangle(229 + (int)screenChange.X, 155 + (int)screenChange.Y, 566, 198)))
                        {
                            if (currentMouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton != ButtonState.Pressed)
                            {
                                level.levelName = "_L" + (selectedIconVariable).ToString();
                                level.LoadLevel();
                                ChangeScreen(screen.Game);
                            }
                        }
                        else if (currentMouseRectangle.Intersects(new Rectangle(155 + (int)screenChange.X, 358 + (int)screenChange.Y, 69, 63)) && selectedIconVariable > 1)
                        {
                            currentlyHighlighted = 11;
                            if (currentMouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton != ButtonState.Pressed)
                            {
                                if (selectedIconVariable > 0)
                                {
                                    selectedIconVariable += -1;
                                }
                            }
                        }
                        else if (currentMouseRectangle.Intersects(new Rectangle(800 + (int)screenChange.X, 358 + (int)screenChange.Y, 69, 63)) && selectedIconVariable < totalTrials)
                        {
                            currentlyHighlighted = 12;
                            if (currentMouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton != ButtonState.Pressed)
                            {
                                if (selectedIconVariable < 10)
                                {
                                    selectedIconVariable += 1;
                                }
                            }
                        }
                    }

                    else if (selectedIcon == 2)
                    {
                        ChangeScreen(screen.MapEditor);
                    }
                    else if (selectedIcon == 3)
                    {
                        level.levelName = "_L0x0";
                        level.LoadLevel();
                        ChangeScreen(screen.Game);
                        subtitle = "Feature not available :(";
                    }
                    else if (selectedIcon == 4)
                    {
                        subtitle = "Soon (TM)";
                    }
                    else if (selectedIcon == 5)
                    {
                        ChangeScreen(screen.Loadout);
                    }
                    else if (selectedIcon == 6)
                    {
                        subtitle = "Later";
                    }
                    else if (selectedIcon == 7)
                    {
                        subtitle = "Zzzzz";
                    }
                }
            }
            #endregion

            #region Map Editor
            else if (currentScreen == screen.MapEditor)
            {

                /*
                #region Click Tile
                int x = (int)((mouseLocation.X + screenPosition.X) / Tile.tileSize);
                int y = (int)((mouseLocation.Y + screenPosition.Y) / Tile.tileSize);
                if (level.CheckTile(x,y))
                {
                    if (currentMouseState.LeftButton == ButtonState.Pressed)
                    {
                        if (editTypesOrTextures == true)
                        {
                            level.tiles[x,y].type = editorSelectedTile;
                            level.tiles[x,y].typeRotation = editRotate;
                        }
                        else
                        {
                            level.tiles[x,y].texturePosition = editorSelectedTile;
                            level.tiles[x,y].textureRotation = editRotate;
                        }
                    }
                }
                #endregion

                else
                {
                    #region Header
                    for (int index = 0; index < 4; index += 1)
                    {
                        if (currentMouseRectangle.Intersects(new Rectangle(index * 64, 0, 64, 16)))
                        {
                            currentlyHighlighted = index + 1;
                            if (currentMouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton != ButtonState.Pressed)
                            {
                                if (index == 0)
                                {
                                    level.DefaultLevel();
                                }
                                else if (index == 1)
                                {
                                    level.levelName = "_temp";
                                    level.SaveLevel();
                                }
                                else if (index == 2)
                                {
                                    level.levelName = "_temp";
                                    level.LoadLevel();
                                }
                                else if (index == 3)
                                {
                                    
                                }
                            }
                        }
                    }
                    #endregion

                    #region Change Type Button
                    if (editTypesOrTextures == true)
                    {
                        if (currentMouseRectangle.Intersects(new Rectangle(128, 376, 112, 24)))
                        {
                            currentlyHighlighted = 5;
                            if (currentMouseState.LeftButton == ButtonState.Pressed)
                            {
                                editTypesOrTextures = false;
                                editorSelectedTile = 0;
                                currentlyHighlighted = 0;
                                editRotate = 0;
                            }
                        }
                    }
                    else
                    {
                        if (currentMouseRectangle.Intersects(new Rectangle(16, 376, 112, 24)))
                        {
                            currentlyHighlighted = 5;
                            if (currentMouseState.LeftButton == ButtonState.Pressed)
                            {
                                editTypesOrTextures = true;
                                editorSelectedTile = 0;
                                currentlyHighlighted = 0;
                            }
                        }
                    }
                    #endregion

                    #region Select Tile
                    if (currentMouseRectangle.Intersects(new Rectangle(16, 400, 224, 166)))
                    {
                        if (editTypesOrTextures == true)
                        {
                            for (int index = 0; index <= editorTypes.Width / Tile.tileSize; index += 1)
                            {
                                if (currentMouseRectangle.Intersects(new Rectangle((int)(18 + index % 6 * (Tile.tileSize + 4)), (int)(402 + (index / 6) * (Tile.tileSize + 4)), Tile.tileSize, Tile.tileSize)))
                                {
                                    if (currentMouseState.LeftButton == ButtonState.Pressed)
                                    {
                                        editorSelectedTile = (byte)index;
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int index = 0; index <= tileTexture.Width / Tile.tileSize; index += 1)
                            {
                                if (currentMouseRectangle.Intersects(new Rectangle((int)(18 + index % 6 * (Tile.tileSize + 4)), (int)(402 + (index / 6) * (Tile.tileSize + 4)), Tile.tileSize, Tile.tileSize)))
                                {
                                    if (currentMouseState.LeftButton == ButtonState.Pressed)
                                    {
                                        editorSelectedTile = (byte)index;
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                }

                #region Move Screen Position

                if (level.smallAdjustment.Y == 0)
                {
                    if (keyUpPressed == true)
                    {
                        screenPosition.Y = Math.Max(screenPosition.Y - 4, 0);
                    }
                    if (keyDownPressed == true)
                    {
                        screenPosition.Y = Math.Min(screenPosition.Y + 4, level.ySize * Tile.tileSize - screenDimensions.Y);
                    }
                }
                if (level.smallAdjustment.Y == 0)
                {
                    if (keyLeftPressed == true)
                    {
                        screenPosition.X = Math.Max(screenPosition.X - 4, 0);
                    }
                    if (keyRightPressed == true)
                    {
                        screenPosition.X = Math.Min(screenPosition.X + 4, level.xSize * Tile.tileSize - screenDimensions.X);
                    }
                }
                if (keySpecial3Pressed == true && keySpecial3WasPressed == false)
                {
                    if (editRotate == 0)
                        editRotate = 3;
                    else
                        editRotate--;
                } if (keySpecial4Pressed == true && keySpecial4WasPressed == false)
                {
                    if (editRotate == 3)
                        editRotate = 0;
                    else
                        editRotate++;
                }
                #endregion
                 */
            }
            #endregion

            #region Game
            else if (currentScreen == screen.Game)
            {
                players[0].UpdateSelf(gameTime);
            } 
            #endregion

            #region Loadout
            else if (currentScreen == screen.Loadout)
            {

            }
            #endregion

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null);

            #region Title
            if (currentScreen == screen.Title)
            {
                #region Animation Sequence

                if (animationSequence == 1)
                {
                    title.Draw(spriteBatch, new Vector2(510, 46) + screenChange,null, Color.White * titleAlpha,0f, new Vector2(title.Width,title.Height)/2,1f, SpriteEffects.None);
                }
                else
                {

                    title.Draw(spriteBatch, new Vector2(510, 46) + screenChange, null, Color.White, 0f, new Vector2(title.Width, title.Height) / 2, 1f, SpriteEffects.None);
                }
                if (animationSequence == 1)
                {
                }
                else if (animationSequence == 2)
                {
                    titleBorders.Draw(spriteBatch, prefferedDimensions / 2 + screenChange, null, currentColor * titleAlpha, 0f, new Vector2(titleBorders.Width, titleBorders.Height) / 2, 1f, SpriteEffects.None);
                    spriteBatch.DrawString(font, subtitle, new Vector2(512f, 525f) - font.MeasureString(subtitle) / 2 + screenChange, currentColor * titleAlpha);
                    for (int index = 0; index < 8; index += 1)
                    {
                        icons.Draw(spriteBatch, iconPositions[index] + screenChange, new Rectangle(0, icons.Width * index, icons.Width, icons.Width), GetInnerColor(currentColor * titleAlpha), 0f, new Vector2(icons.Width / 2), 1f, SpriteEffects.None);
                    }
                }
                else
                {
                    titleBorders.Draw(spriteBatch, Vector2.Zero + screenChange, currentColor);
                    spriteBatch.DrawString(font, subtitle, new Vector2(512f, 525f) - font.MeasureString(subtitle) / 2 + screenChange, currentColor);
                    for (int index = 0; index < 8; index += 1)
                    {
                        icons.Draw(spriteBatch, iconPositions[index] + screenChange, new Rectangle(0, icons.Width * index, icons.Width, icons.Width), CheckWhite(index + 1), 0f, new Vector2(icons.Width / 2), 1f, SpriteEffects.None);
                    }
                }
                #endregion

                if (selectedIcon == 0)
                {
                    saveSelectFrame.Draw(spriteBatch, new Vector2(512,292)+ screenChange,null, currentColor, 0f,new Vector2(saveSelectFrame.Width,saveSelectFrame.Height)/2, 1f, SpriteEffects.None);
                    for (int index = 0; index < 4; index += 1)
                    {
                        string temporaryString = "Save" + singlePlayerSaves[index].save;
                        saveSelectBox.Draw(spriteBatch, new Vector2(512, 173+79*index) + screenChange, null, CheckWhite(10+index), 0f, new Vector2(saveSelectBox.Width, saveSelectBox.Height) / 2, 1f, SpriteEffects.None);
                        spriteBatch.DrawString(font, temporaryString, new Vector2(396, 152 + 79 * index) + screenChange, CheckWhite(10 + index), 0f, new Vector2(0, font.MeasureString(temporaryString).Y / 2), .4f, SpriteEffects.None, 0f);
                        difficulties.Draw(spriteBatch, new Vector2(430, 172 + 79 * index) + screenChange, new Rectangle(0, singlePlayerSaves[index].difficulty * 40, 160, 40), Color.White,0f, new Vector2(difficulties.Width,difficulties.Height/4)/2, 1f,SpriteEffects.None);
                        temporaryString = singlePlayerSaves[index].lastPlayed;
                        spriteBatch.DrawString(font, temporaryString, new Vector2(392, 192 + 79 * index) + screenChange, CheckWhite(10 + index), 0f, new Vector2(0, font.MeasureString(temporaryString).Y / 2), .25f, SpriteEffects.None, 0f);
                        temporaryString = "Areas: " + singlePlayerSaves[index].areasDiscoveredInt + "/" + SinglePlayerSave.xTotalLevels * SinglePlayerSave.yTotalLevels;
                        spriteBatch.DrawString(font, temporaryString, new Vector2(486, 152 + 79 * index) + screenChange, CheckWhite(10 + index), 0f, new Vector2(0, font.MeasureString(temporaryString).Y / 2), .4f, SpriteEffects.None, 0f);
                        temporaryString = "Tapes: " + singlePlayerSaves[index].tapesDiscoveredInt + "/" + SinglePlayerSave.totalTapes;
                        spriteBatch.DrawString(font, temporaryString, new Vector2(486, 172 + 79 * index) + screenChange, CheckWhite(10 + index), 0f, new Vector2(0, font.MeasureString(temporaryString).Y / 2), .4f, SpriteEffects.None, 0f);
                        temporaryString = "Bosses: " + singlePlayerSaves[index].bossesKilledInt + "/" + SinglePlayerSave.totalBosses;
                        spriteBatch.DrawString(font, temporaryString, new Vector2(486, 192 + 79 * index) + screenChange, CheckWhite(10 + index), 0f, new Vector2(0, font.MeasureString(temporaryString).Y / 2), .4f, SpriteEffects.None, 0f);
                        temporaryString = (33f * singlePlayerSaves[index].areasDiscoveredInt / (SinglePlayerSave.xTotalLevels * SinglePlayerSave.yTotalLevels) + 33f * singlePlayerSaves[index].tapesDiscoveredInt / SinglePlayerSave.totalTapes + 33f * singlePlayerSaves[index].bossesKilledInt / SinglePlayerSave.totalBosses).ToString();
                        temporaryString.Remove(Math.Min(2,temporaryString.Length-1));
                        temporaryString += "%";
                        spriteBatch.DrawString(font, temporaryString, new Vector2(571, 172 + 79 * index) + screenChange, CheckWhite(10 + index), 0f, new Vector2(0, font.MeasureString(temporaryString).Y / 2), .5f, SpriteEffects.None, 0f);

                    }
                }
                else if (selectedIcon == 1)
                {
                    spriteBatch.Draw(challengeBorder, new Vector2(150, 150) + screenChange, currentColor);
                    //spriteBatch.Draw(levelPictures[selectedIconVariable], new Vector2(229, 155), Color.White);
                    for (int index = 0; index < totalTrials; index += 1)
                    {
                        if (index == selectedIconVariable - 1)
                            spriteBatch.Draw(challengeBox, new Vector2(234 + 56 * index, 358) + screenChange, Color.White);
                        else
                            spriteBatch.Draw(challengeBox, new Vector2(234 + 56 * index, 358) + screenChange, currentColor);
                    }
                    if (selectedIconVariable > 1)
                        spriteBatch.Draw(challengeSmallArrow, new Vector2(155, 358) + screenChange, CheckWhite(11));
                    if (selectedIconVariable < totalTrials)
                        spriteBatch.Draw(challengeSmallArrow, new Vector2(800, 358) + screenChange, null, CheckWhite(12), 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0f);
                }
                else if (selectedIcon == 2)
                {
                    
                }
            }
            #endregion

            #region Editor
            if (currentScreen == screen.MapEditor)
            {
                level.DrawEditor(spriteBatch);
                for (int index = 0; index < editorCategories.Length; index += 1)
                {
                    editorHud.Draw(spriteBatch, new Vector2(index * 48 - 5, -5), new Rectangle(5, 5, 58, 26), currentColor);
                    spriteBatch.DrawString(font, editorCategories[index], new Vector2(48*index + 24, 9), currentColor,0f, font.MeasureString(editorCategories[index])/2, .25f, SpriteEffects.None, 0f);
                }
                /*
                spriteBatch.Draw(editorTileBackdrop, new Vector2(16, 400), currentColor);
                spriteBatch.Draw(editorNoTile, new Vector2(18, 402), Color.White);
                if (editorSelectedTile == 0)
                    spriteBatch.Draw(editorTileOutline, new Vector2(17, 401), Color.White);
                else
                    spriteBatch.Draw(editorTileOutline, new Vector2(17, 401), currentColor);
                if (editTypesOrTextures == true)
                {
                 * 
                    for (int index = 1; index <= editorTypes.Width / Tile.tileSize; index++)
                    {
                        Vector2 temporaryVector =  new Vector2(18 + index % 6 * (Tile.tileSize + 4), 402 + (index / 6) * (Tile.tileSize + 4));
                        if (index == 10)
                        {
                            if (editRotate == 0)
                                spriteBatch.Draw(editorTypes, temporaryVector + Vector2.One * Tile.tileSize / 2f, new Rectangle((index - 1) * Tile.tileSize, 0, Tile.tileSize, Tile.tileSize), Color.Red, 0, Vector2.One * Tile.tileSize / 2f, 1f, SpriteEffects.None, 0f);
                            else if (editRotate == 1)
                                spriteBatch.Draw(editorTypes, temporaryVector + Vector2.One * Tile.tileSize / 2f, new Rectangle((index - 1) * Tile.tileSize, 0, Tile.tileSize, Tile.tileSize), Color.Green, 0, Vector2.One * Tile.tileSize / 2f, 1f, SpriteEffects.None, 0f);
                            else if (editRotate == 2)
                                spriteBatch.Draw(editorTypes, temporaryVector + Vector2.One * Tile.tileSize / 2f, new Rectangle((index - 1) * Tile.tileSize, 0, Tile.tileSize, Tile.tileSize), Color.Blue, 0, Vector2.One * Tile.tileSize / 2f, 1f, SpriteEffects.None, 0f);
                            else if (editRotate == 3)
                                spriteBatch.Draw(editorTypes, temporaryVector + Vector2.One * Tile.tileSize / 2f, new Rectangle((index - 1) * Tile.tileSize, 0, Tile.tileSize, Tile.tileSize), Color.Yellow, 0, Vector2.One * Tile.tileSize / 2f, 1f, SpriteEffects.None, 0f);
                        }
                        else
                        {
                            spriteBatch.Draw(editorTypes, temporaryVector + Vector2.One * Tile.tileSize / 2f, new Rectangle((index - 1) * Tile.tileSize, 0, Tile.tileSize, Tile.tileSize), Color.White, editRotate * (float)(Math.PI / 2), Vector2.One * Tile.tileSize / 2f, 1f, SpriteEffects.None, 0f);
                        }
                        
                        if (index == editorSelectedTile)
                            spriteBatch.Draw(editorTileOutline, temporaryVector - Vector2.One, Color.White);
                        else
                            spriteBatch.Draw(editorTileOutline, temporaryVector - Vector2.One, currentColor);
                    }
                }
                else
                {
                    for (int index = 1; index <= tileTexture.Width / Tile.tileSize; index++)
                    {
                        Vector2 temporaryVector = new Vector2(18 + index % 6 * (Tile.tileSize + 4), 402 + (index / 6) * (Tile.tileSize + 4));
                        spriteBatch.Draw(tileTexture, temporaryVector + new Vector2(Tile.halfTileSize), new Rectangle((index - 1) * Tile.tileSize, 0, Tile.tileSize, Tile.tileSize), Color.White, editRotate * (float)(Math.PI / 2), new Vector2(Tile.halfTileSize), 1f, SpriteEffects.None, 0f);
                        if (index == editorSelectedTile)
                            spriteBatch.Draw(editorTileOutline, temporaryVector - Vector2.One, Color.White);
                        else
                            spriteBatch.Draw(editorTileOutline, temporaryVector - Vector2.One, currentColor);
                    }
                }
                spriteBatch.Draw(editorFrame, Vector2.Zero, currentColor);
                if (editTypesOrTextures == true)
                {
                    spriteBatch.Draw(editorSwitchActive, new Vector2(16, 376), currentColor);
                    spriteBatch.Draw(editorSwitchInactive, new Vector2(128, 376), CheckWhite(5));
                }
                else
                {
                    spriteBatch.Draw(editorSwitchActive, new Vector2(128, 376), currentColor);
                    spriteBatch.Draw(editorSwitchInactive, new Vector2(16, 376), CheckWhite(5));
                }
                spriteBatch.DrawString(font, "Type", new Vector2(73, 388), currentColor, 0f, font.MeasureString("Type") / 2f, .5f, SpriteEffects.None, 1f);
                spriteBatch.DrawString(font, "Texture", new Vector2(185, 388), currentColor, 0f, font.MeasureString("Texture") / 2f, .5f, SpriteEffects.None, 1f);
                for (int index = 0; index < 4; index+= 1)
                {
                    spriteBatch.Draw(editorHeaderRectangle, new Vector2(index * 64 + 1,0), CheckWhite(index + 1));
                } 
                for (int index = 0; index < 4; index += 1)
                {
                    spriteBatch.DrawString(font, editorWords[index], new Vector2(index * 64 + 5, 0), Color.Black, 0f, Vector2.Zero, .3f, SpriteEffects.None, 0);
                }
                */
                

            }
            #endregion

            #region Game
            if (currentScreen == screen.Game)
            {
                level.Draw(spriteBatch);
                players[0].Draw(gameTime, spriteBatch);
                for (int index = 0; index < 4; index += 1)
                {
                    //float percent = players[0].loadout.abilities[index].cooldown / players[0].loadout.abilities[index].maxCooldown;
                    //spriteBatch.Draw(abilityIconsEmpty, new Vector2(372 + 72 * index, 504), new Rectangle(64 * index, 0, 64, 64), Color.White*.6f);
                    //spriteBatch.Draw(abilityIconsFull, new Vector2(372 + 72 * index, 512 + 48 * percent), new Rectangle(64 * index, 8 + (int)(48 * percent), 64, 64), Color.White * .6f);
                }
                //spriteBatch.Draw(tileTextureFull, Vector2.Zero, Color.White*.5f); DEBUG
            }
            #endregion

            #region Loadout
            else if (currentScreen == screen.Loadout)
            {
                for (int index = 0; index < 4; index += 1)
                {
                    //spriteBatch.Draw(editorHeaderRectangle, new Vector2(1, 1 + index * 16), CheckWhite(index + 1));
                }
                for (int index = 0; index < 4; index += 1)
                {
                    spriteBatch.DrawString(font, loadoutWords[index], new Vector2(5, 1 + index * 16), Color.Black, 0f, Vector2.Zero, .3f, SpriteEffects.None, 0);
                }
                if (true)//Check if first entry
                    spriteBatch.Draw(loadoutArrow, new Vector2(64, 0), null, CheckWhite(5), 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f); 
                if (true)//Check if last entry
                    spriteBatch.Draw(loadoutArrow, new Vector2(992, 0), null, CheckWhite(6), 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 1f);
                spriteBatch.Draw(loadoutFrame, Vector2.Zero, currentColor);
                for (int index = 0; index < 4; index += 1)
                {
                    spriteBatch.Draw(loadoutAbilityHighlighter, new Vector2(4 + 76 * index, 70), CheckWhite(10 + index));
                    spriteBatch.Draw(abilityIconsEmpty, new Vector2(8 + 76 * index, 74), new Rectangle(index * 64, 0, 64, 64), Color.White);
                    spriteBatch.Draw(abilityIconsFull, new Vector2(8 + 76 * index, 74), new Rectangle(index * 64, 0, 64, 64), Color.White);
                } 
                for (int index = 0; index < 4; index += 1)//Check maximum sub-abilities
                {
                    spriteBatch.Draw(loadoutAbilityHighlighter, new Vector2(4 + 76 * index, 148), CheckWhite(20 + index));
                }
                for (int index = 0; index < 12; index += 1)//Check maximum sub-sub-abilities
                {
                    spriteBatch.Draw(loadoutAbilityHighlighter, new Vector2(4 + 76 * (index%4), 226 + 76*(index/4)), CheckWhite(20 + index));
                }
                for (int index = 0; index < 6; index += 1)
                {
                    spriteBatch.Draw(attributesx2, new Vector2(310, 70 + index * 44), new Rectangle(index * 40, 0, 40, 40), Color.White);
                    Color tempColor;
                    if (index == 0)
                        tempColor = Color.Blue;
                    else if (index == 1)
                        tempColor = Color.Pink;
                    else if (index == 2)
                        tempColor = Color.Green;
                    else if (index == 3)
                        tempColor = Color.LightBlue;
                    else if (index == 4)
                        tempColor = Color.Yellow;
                    else
                        tempColor = Color.Red;
                    if (false) //Lowest
                        spriteBatch.Draw(loadoutScale, new Vector2(354, 76 + index * 44), new Rectangle(16, 0, 16, 28), tempColor);
                    else
                        spriteBatch.Draw(loadoutScale, new Vector2(354, 76 + index * 44), new Rectangle(0, 0, 16, 28), tempColor);
                    if (false) //Lowest
                        spriteBatch.Draw(loadoutScale, new Vector2(528, 76 + index * 44), new Rectangle(48, 0, 16, 28), tempColor);
                    else
                        spriteBatch.Draw(loadoutScale, new Vector2(528, 76 + index * 44), new Rectangle(64, 0, 16, 28), tempColor);
                    for (int index2 = 1; index2 <= 10; index2++)//Amount in attribute
                    {
                        spriteBatch.Draw(loadoutScale, new Vector2(354+index2*16, 76 + index * 44), new Rectangle(32, 0, 16, 28), tempColor);
                    }

                }
            }
            #endregion


            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected void UpdateInput()
        {
            #region Input Status
            lastKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            lastMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            lastMouseRectangle = currentMouseRectangle;
            currentMouseRectangle = new Rectangle(currentMouseState.X, currentMouseState.Y, 1, 1);
            mouseLocation = new Vector2(currentMouseRectangle.X, currentMouseRectangle.Y);
            keySpecial1WasPressed = keySpecial1Pressed;
            keySpecial2WasPressed = keySpecial2Pressed;
            keySpecial3WasPressed = keySpecial3Pressed;
            keySpecial4WasPressed = keySpecial4Pressed;
            if (currentMouseState.LeftButton == ButtonState.Pressed)
                keySpecial1Pressed = true;
            else
                keySpecial1Pressed = false;
            if (currentMouseState.RightButton == ButtonState.Pressed)
                keySpecial2Pressed = true;
            else
                keySpecial2Pressed = false;
            if (currentKeyboardState.IsKeyDown((Keys)((int)(char.ToUpper(keyUp)))))
                keyUpPressed = true;
            else
                keyUpPressed = false;
            if (currentKeyboardState.IsKeyDown((Keys)((int)(char.ToUpper(keyDown)))))
                keyDownPressed = true;
            else
                keyDownPressed = false;
            if (currentKeyboardState.IsKeyDown((Keys)((int)(char.ToUpper(keyLeft)))))
                keyLeftPressed = true;
            else
                keyLeftPressed = false;
            if (currentKeyboardState.IsKeyDown((Keys)((int)(char.ToUpper(keyRight)))))
                keyRightPressed = true;
            else
                keyRightPressed = false;
            if (currentKeyboardState.IsKeyDown((Keys)((int)(char.ToUpper(keySpecial3)))))
                keySpecial3Pressed = true;
            else
                keySpecial3Pressed = false;
            if (currentKeyboardState.IsKeyDown((Keys)((int)(char.ToUpper(keySpecial4)))))
                keySpecial4Pressed = true;
            else
                keySpecial4Pressed = false;
            #endregion

            #region Escape
            if (currentKeyboardState.IsKeyDown(Keys.Escape) && !lastKeyboardState.IsKeyDown(Keys.Escape))
            {
                if (currentScreen == screen.Title)
                {
                    this.Exit();
                }
                else if (currentScreen == screen.MapEditor)
                {
                    level.SaveLevel();
                    ChangeScreen(screen.Title);
                }
                else if (currentScreen == screen.Loadout)
                {
                    ChangeScreen(screen.Title);
                }
                else if (currentScreen == screen.Game)
                {
                    ChangeScreen(screen.Title);
                }
            }
            #endregion    
        }

        protected void UpdateColor(GameTime gameTime)
        {
            if (currentColor.Equals(targetColor))
            {
                targetColor = HueToRGB(random.Next(360));
            }
            if (currentColor.R > targetColor.R)
            {
                currentColor.R--;
            }
            else if (currentColor.R < targetColor.R)
            {
                currentColor.R++;
            }
            if (currentColor.B > targetColor.B)
            {
                currentColor.B--;
            }
            else if (currentColor.B < targetColor.B)
            {
                currentColor.B++;
            }
            if (currentColor.G > targetColor.G)
            {
                currentColor.G--;
            }
            else if (currentColor.G < targetColor.G)
            {
                currentColor.G++;
            }
        }

        protected Color CheckWhite(int highlight)
        {
            if (highlight == currentlyHighlighted)
                return Color.White;
            else
                return currentColor;
        }

        protected void ChangeScreen(screen toScreen)
        {
            currentScreen = toScreen;
            if (toScreen == screen.Title)
            {
                animationSequence = 1;
                selectedIcon = -1;
                level.levelName = "";
                iconPositions = new Vector2[] { new Vector2(51, 170), new Vector2(51, 286), new Vector2(51, 402), new Vector2(973, 170), new Vector2(973, 286), new Vector2(973, 402), new Vector2(157, 525), new Vector2(867, 525) };
            }
            else if (toScreen == screen.MapEditor)
            {
                level.DefaultLevel();

                focused = false;
                focus = 0;
                /*
                editTypesOrTextures = true;
                editorSelectedTile = 0;
                editRotate = 0;
                 */
            }
            else if (toScreen == screen.Loadout)
            {

            }
            else if (toScreen == screen.Game)
            {
                level.LoadLevel();
                players = new List<Player>();
                players.Add(new Player());
                players[0].position = level.GetSpawn(players[0]);
            }
        }

        protected void LoadData()
        {
            //string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/LazerCraft";
            string path = @"Content/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path); 
            }
            if (!File.Exists(path + "/GameData.data"))
            {
                continueLevel = 10;
                unlockedLevels = 10;
                for (int index = 0; index < timeTrialTimes.Length; index += 1)
                {
                    timeTrialTimes[index] = 999.999f;
                }
                keyUp = 'W';
                keyDown = 'S';
                keyLeft = 'A';
                keyRight = 'D';
                keySpecial3 = 'Q';
                keySpecial4 = 'E';
                muteSound = false;
                muteMusic = false;
                laserBrightness = .6f;
                screenDimensions = new Vector2(1024, 576);
                screenDimensions = new Vector2(1600, 900);
                graphics.IsFullScreen = false;
                SaveData();
            }
            else
            {
                using (BinaryReader reader = new BinaryReader(File.Open(path + "/GameData.data", FileMode.Open)))
                {
                    continueLevel = reader.ReadInt32();
                    unlockedLevels = reader.ReadInt32();
                    for (int index = 0; index < timeTrialTimes.Length; index += 1)
                    {
                        timeTrialTimes[index] = reader.ReadSingle();
                    }
                    keyUp = reader.ReadChar();
                    keyDown = reader.ReadChar();
                    keyLeft = reader.ReadChar();
                    keyRight = reader.ReadChar();
                    keySpecial3 = reader.ReadChar();
                    keySpecial4 = reader.ReadChar();
                    muteSound = reader.ReadBoolean();
                    muteMusic = reader.ReadBoolean();
                    laserBrightness = reader.ReadSingle();
                    screenDimensions = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                    graphics.IsFullScreen = reader.ReadBoolean();
                }
            }
        }

        protected void SaveData()
        {
            //string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/LazerCraft";
            string path = @"Content/";
            using (BinaryWriter writer = new BinaryWriter(File.Open(path + "/GameData.data", FileMode.OpenOrCreate)))
            {
                writer.Write(continueLevel);
                writer.Write(unlockedLevels);
                for (int index = 0; index < timeTrialTimes.Length; index += 1)
                {
                    writer.Write(timeTrialTimes[index]);
                }
                writer.Write(keyUp);
                writer.Write(keyDown);
                writer.Write(keyLeft);
                writer.Write(keyRight);
                writer.Write(keySpecial3);
                writer.Write(keySpecial4);
                writer.Write(muteSound);
                writer.Write(muteMusic);
                writer.Write(laserBrightness);
                writer.Write(screenDimensions.X);
                writer.Write(screenDimensions.Y);
                writer.Write(graphics.IsFullScreen);
            }
        }

        public static Vector2 RotateVelocity(Vector2 velocity, Vector2 plane, bool toAxis)
        {
            if (toAxis == true)
                return new Vector2(plane.X * velocity.X + plane.Y * velocity.Y, -plane.Y * velocity.X + plane.X * velocity.Y);
            else
                return new Vector2(velocity.X * plane.X - velocity.Y * plane.Y, velocity.X * plane.Y + velocity.Y * plane.X);
        }

        public List<Entity> getAllPlayers(int excludeTeam)
        {
            List<Entity> entities = new List<Entity>();
            foreach (Player p in players)
            {
                if (p.team != excludeTeam)
                    entities.Add(p);
            }
            return entities;
        }

        public static Color GetInnerColor(Color color)
        {
            return color * laserBrightness;
        }

        public static Color HueToRGB(int h)
        {
            // Finish Later
            int t = h / 60;
            h = h%60;
            int r = 0, g = 0, b = 0;
            if (t == 0)
            {
                r = 255;
                g = h * 255 / 60;
            }
            else if (t == 1)
            {
                r = 255 - h * 255 / 60;
                g = 255;
            }
            else if (t == 2)
            {
                g = 255;
                b = h * 255 / 60;
            }
            else if (t == 3)
            {
                g = 255 - h * 255 / 60;
                b = 255;
            }
            else if (t == 4)
            {
                b = 255;
                r = h * 255 / 60;
            }
            else if (t >= 5)
            {
                b = 255 - h * 255 / 60;
                r = 255;
            }
            return new Color(r, g, b);
        }
     }
}
