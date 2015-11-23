using System;
using System.Collections.Generic;
using System.Text;

using MLA.Xna.Gui;
using MLA.Xna.Gui.Controls;
using MLA.Utilities.Algorithms;
using MLA.Utilities.Algorithms.Data;
using MLA.Utilities.Helpers;
using MLA.Utilities;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Alliance.Data;
using Alliance.Utilities;
using Alliance.Pieces;
using Alliance.Invaders;
using Alliance.Projectiles;
using Alliance.Parameters;
using Alliance.Objects;

namespace Alliance.Components
{
  public partial class GridComponent : DrawableGameComponent
  {
    private const int CellWidth = 20;
    private const int CellHeight = 20;
    private const int HalfThroughWay = 6;
    private const int OrthogonalCost = 12;

    private GridCell[,] Cells;
    private int NumRows;
    private int NumCols;
    private int Width;
    private int Height;
    private GridCell HorzGoalCell;
    private GridCell VertGoalCell;
    private GridCell HorzStartCell;
    private GridCell VertStartCell;
    private SizeF MiddleOffset;
    private SpriteBatch mSpriteBatch;
    private Vector2 mPosition;
    private GridFillMode mFillMode;
    private SelectionPiece mSelectionPiece;
    private Piece mSelectedPiece;
    private Invader mSelectedInvader;

    private List<Piece> mPieces;
    private List<Invader> mInvaders;
    private List<Projectile> mProjectiles;

    private GuiManager mGui;
    private ListBox lstPieces;
    private TextBox txtDescription;
    private Caption cptDescription;
    private SpriteFont captionFont;
    private ShapeBatch shapeBatch;
    private BoxF currentArea = BoxF.Empty;

    public GridComponent(Game game)
      : base(game)
    {
      game.IsMouseVisible = true;
      mFillMode = GridFillMode.Solid;

      mGui = new GuiManager(game);
      game.Components.Add(mGui);
    }

    protected override void LoadContent()
    {
      mSpriteBatch = new SpriteBatch(GraphicsDevice);
      shapeBatch = new ShapeBatch(GraphicsDevice);
      captionFont = AllianceGame.Fonts["ComicSans"];
    }

    public override void Initialize()
    {
      base.Initialize();
      Player.InitializePlayer(100000, 5000);
      InitializeVariables();
      InitializeProperties();
      InitializeGrid();
      InitializeListBox();
      InitializeDescriptionBox();
    }

    public override void Update(GameTime gameTime)
    {
      // if the game isn't running slowly
      if (!gameTime.IsRunningSlowly)
      {
        // immediately add some invaders
        AddInvaders();
      }

      // get the input provider
      InputProvider input = (InputProvider)Game.Services.GetService(typeof(InputProvider));

      // construct new updata parameters for the functions
      UpdateParams uparams = new UpdateParams(gameTime, input, (Vector2)MiddleOffset + mPosition);

      // update the list
      UpdateSelectionList(uparams);

      // update the selection the user has made
      UpdateSelectionPiece(uparams);

      // process any requests from the user for a piece
      ProcessInput(uparams);

      // update the description text that is displayed
      UpdateDescriptionText(uparams);

      // update the radius around the selected piece
      UpdateSelectedPieceRadius(uparams);

      // update all pieces
      UpdatePieces(uparams);

      // update all invaliders
      UpdateInvaders(uparams);

      // update the piece targets
      UpdatePieceTargets(uparams);

      // update all projectiles
      UpdateProjectiles(uparams);

      base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
      DrawParams dparams = new DrawParams(gameTime, (Vector2)MiddleOffset + mPosition, mSpriteBatch, mFillMode);
      mSpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);

      DrawGrid(dparams);
      DrawPieces(dparams);
      DrawInvaders(dparams);
      DrawProjectiles(dparams);

      mSpriteBatch.End();

      DrawPieceRadius();
      base.Draw(gameTime);
    }

    #region Solve Grid
    private void SolveGrid()
    {
      SolveGrid(HorzGoalCell, DijkstraType.Horizontal);
      SolveGrid(VertGoalCell, DijkstraType.Vertical);
    }

    private void SolveGrid(GridCell goalCell, DijkstraType dijkstraType)
    {
      List<GridCell> Q = new List<GridCell>(Cells.Length);
      for (int c = 0; c < NumCols; ++c)
      {
        for (int r = 0; r < NumRows; ++r)
        {
          GridCell v = Cells[c, r];
          if (!IsIllegal(v))
          {
            v[dijkstraType].Reset();
            Q.Add(v);
          }
        }
      }

      goalCell[dijkstraType].Reset();
      goalCell[dijkstraType].Distance = 0;

      while (Q.Count > 0)
      {
        // extra the minimum value
        GridCell u = ExtractMin(dijkstraType, ref Q);

        // relax each of the neighbors
        foreach (GridCell v in u.AdjacentCells)
        {
          if (v == null || IsIllegal(v)) continue;
          Relax(dijkstraType, v, u);
        }
      }
    }

    private GridCell ExtractMin(DijkstraType dijkstraType, ref List<GridCell> Q)
    {
      int min = int.MaxValue;
      int idx = 0;

      for (int i = 0; i < Q.Count; ++i)
      {
        if (Q[i][dijkstraType].Distance < min)
        {
          min = Q[i][dijkstraType].Distance;
          idx = i;
        }
      }

      GridCell retval = Q[idx];
      Q.RemoveAt(idx);

      return retval;
    }

    private void Relax(DijkstraType dijkstraType, GridCell v, GridCell u)
    {
      int alt = u[dijkstraType].Distance + OrthogonalCost;
      if (alt < v[dijkstraType].Distance)
      {
        v[dijkstraType].Distance = alt;
        v[dijkstraType].Parent = u;
      }
    }

    private bool IsIllegal(GridCell node)
    {
      return node.Type != GridCellType.Empty || (!node.IsThroughway && node.IsOuter);
    }
    #endregion
  }
}
